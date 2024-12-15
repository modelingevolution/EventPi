using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using EventPi.SignalProcessing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelingEvolution.VideoStreaming.Buffers;

namespace EventPi.SignalProcessing
{
    // All signals are going through one Channel. 
    // The read thread will append data to subscribed clients. 
    public class SignalHubServer
    {
        private readonly ILogger<SignalHubServer> _logger;

        // Single channel for all signals
        internal readonly Channel<SignalReceivedEvent> _channel =
            Channel.CreateUnbounded<SignalReceivedEvent>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });

        // Dictionary of clients subscribed to each signal
        private readonly ConcurrentDictionary<SignalName, ConcurrentBag<ClientPipe>> _clients =
            new ConcurrentDictionary<SignalName, ConcurrentBag<ClientPipe>>();

        private readonly ConcurrentDictionary<SignalName, ISignalMetadata> _metadata = new();

        public IEnumerable<ISignalMetadata> MetadataIndex => _metadata.Values;

        public ISignalMetadata GetMetadata(string name)
        {
            if (_metadata.TryGetValue((SignalName)name, out var metadata)) return metadata;

            throw new KeyNotFoundException($"Metadata for signal '{name}' not found.");
        }

        public SignalHubServer RegisterSink<T>(string name) where T : struct, INumber<T>
        {
            if (!_metadata.TryAdd((SignalName)name, new SignalMetadata<T>(name)))
                throw new InvalidOperationException("Sink already registered.");
            return this;
        }
        public SignalHubServer(ILogger<SignalHubServer> logger)
        {
            _logger = logger;
            // Start the chaser task to process incoming signals
            _ = Task.Run(Chaser);
        }

        public ISignalSink<T> GetSink<T>(SignalName name)
        {
            // Create a sink that writes to the shared channel
            return new SignalSink<T>(this, name);
        }

        public async Task PipeSignals(WebSocket destination, float frequency, CancellationToken token, params ISignalMetadata[] signals)
        {
            // Create a client pipe for the specific WebSocket
            var clientPipe = new ClientPipe(signals);
            _logger.LogInformation($"Client subscribed with {frequency}Hz.");
            // Add the client pipe to each signal's client bag
            foreach (var signal in signals)
            {
                _clients.AddOrUpdate(
                    signal.Name,
                    _ => new ConcurrentBag<ClientPipe> { clientPipe },
                    (_, existing) =>
                    {
                        existing.Add(clientPipe);
                        return existing;
                    }
                );
            }

            try
            {
                int intervalMs = (int)(1000 / frequency);

                using var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));

                while (await periodicTimer.WaitForNextTickAsync(token))
                {
                    // Swap buffers to prepare for next aggregation
                    clientPipe.Swap();
                    await clientPipe.WriteTo(destination, token);

                    // Optional: Add a way to break the loop if needed
                    if (token.IsCancellationRequested)
                        break;
                }
            }
            catch(OperationCanceledException)
            {
                
            }
            finally
            {
                // Remove client pipe from all signal subscriptions
                foreach (var signal in signals)
                {
                    if (_clients.TryGetValue(signal.Name, out var pipes))
                    {
                        pipes.TryTake(out _);
                    }
                }
            }
        }

       

        private async Task Chaser()
        {
            await foreach (var signal in _channel.Reader.ReadAllAsync())
            {
                // Dispatch signal to all subscribed client pipes
                if (_clients.TryGetValue(signal.Name, out var pipes))
                {
                    foreach (var pipe in pipes)
                    {
                        pipe.AggregateAvg(signal.Name, signal.Value);
                    }
                }
            }
        }
    }

    readonly struct SignalReceivedEvent
    {
        public readonly SignalName Name;
        public readonly object Value;

        public SignalReceivedEvent(SignalName name, object value)
        {
            Name = name;
            Value = value;
        }
    }
    public interface IAverage
    {
        object Calc();
        IAverage Reset();
        int Count { get; }
        IAverage Aggregate(IAverage average, object value);
        int WriteTo(in Span<byte> dst);
    }
    // Should have operator + defined with T, where the result is Average
    // Aggregate should use + to do the calc.
    public readonly struct Average<T> : IAverage 
        where T:struct,INumber<T>
    {
        private readonly T _sum;
        private readonly T _count;
        public Average() : this(T.Zero, T.Zero){}
        private Average(T sum, T count)
        {
            _sum = sum;
            _count = count;
        }

        public int Count => int.CreateTruncating(_count);
        public Average<T> Reset() => new Average<T>(T.Zero, T.Zero);
        IAverage IAverage.Reset() => this.Reset();
        public object Calc() => _count > T.Zero ? _sum / _count : default(T);
        public IAverage Aggregate(IAverage average, object value)
        {
            if (average is not Average<T> currentAvg || value is not T typedValue)
                throw new InvalidOperationException("Cannot aggregate incompatible types");

            return new Average<T>(
                currentAvg._sum + typedValue,
                currentAvg._count + T.One
            );
        }

        private static readonly int Size = Marshal.SizeOf<T>();
        public int WriteTo(in Span<byte> dst)
        {
            var result = _count > T.Zero ? _sum / _count : default(T);
            MemoryMarshal.Write(dst, result);
            return Size;
        }
    }
    public class ClientPipe
    {
        // Two buffers for double-buffering
        private readonly IAverage[] _b1;
        private readonly IAverage[] _b2;
        
        private IAverage[] _read;
        private IAverage[] _write;

        // Mapping of signal names to their index in the buffer
        private readonly FrozenDictionary<SignalName, int> _nameToIndexMap;

        public ClientPipe(params ISignalMetadata[] signals)
        {
            // Initialize buffers with default averagers
            _b1 = new IAverage[signals.Length];
            _b2 = new IAverage[signals.Length];
            for (int i = 0; i < signals.Length; i++)
            {
                _b1[i] = signals[i].CreateAverage();
                _b2[i] = signals[i].CreateAverage();
            }
            _read = _b2;
            _write = _b1;
            // Create name to index mapping
            _nameToIndexMap = signals
                .Select((name, index) => new { Name = name.Name, Index = index })
                .ToFrozenDictionary(x => x.Name, x => x.Index);
        }

        internal IAverage[] WriteBuffer => _write;

        public void Swap()
        {
            // preparing read-buf to be write, we need to reset everything.
            for (var index = 0; index < _read.Length; index++)
            {
                _read[index] = _read[index].Reset();
            }

            (_read, _write) = (_write, _read);
        }

        public void AggregateAvg(SignalName name, object value)
        {
            var index = _nameToIndexMap[name];
            _write[index] = _write[index].Aggregate(_write[index], value);
        }

        private readonly byte[] _buffer = new byte[64 * 1024];
        public async Task WriteTo(WebSocket destination, CancellationToken token)
        {
            int offset = 0;
            for (ushort index = 0; index < _read.Length; index++)
            {
                var i = _read[index];

                if (i.Count <= 0) continue;
                
                MemoryMarshal.Write(_buffer.AsSpan(offset), index);
                offset += sizeof(ushort);
                offset += i.WriteTo(_buffer.AsSpan(offset));
            }

            if (offset == 0) return;
            
            await destination.SendAsync(
                _buffer.AsMemory(0,offset),
                WebSocketMessageType.Binary,
                true,
                token
            );
        }
    }
    public record SignalSink(string Name, string Type);

    public static class ContainerExtensions
    {
        public static IServiceCollection AddSignalsServer(this IServiceCollection services, Action<SignalHubServer> onConfigure = null)
        {
            services.AddSingleton<SignalHubServer>(sp =>
            {
                var tmp = new SignalHubServer(sp.GetRequiredService<ILogger<SignalHubServer>>());
                onConfigure?.Invoke(tmp);
                return tmp;
            });
            return services;
        }
    }
    
    
    public class SignalHubClient(IHttpClientFactory factory)
    {
        private static readonly SignalMetadataFactory _factory = new SignalMetadataFactory();
        private readonly HttpClient _client = factory.CreateClient("default");
        public float Frequency { get; set; } = 2f;
        public string? RequestPath { get; set; } = "signals";

        private Uri? WsUrl
        {
            get
            {
                UriBuilder b = new UriBuilder(_client.BaseAddress ?? throw new ArgumentException("HttpClient base url must be set."));
                b.Scheme = b.Scheme.Replace("http", "ws");
                b.Path = $"{(RequestPath ?? string.Empty)}-stream";
                return b.Uri;
            }
        }

        public async Task<SignalSink[]> GetSignals()
        {
            return await _client.GetFromJsonAsync<SignalSink[]>(RequestPath) ?? Array.Empty<SignalSink>();
        }

        public Task<SignalsStream> Subscribe(params SignalSink[] signals)
        {
            return Subscribe(signals.Select(x => _factory.Create(x.Name, x.Type)).ToArray());

        }
        public async Task<SignalsStream> Subscribe(params ISignalMetadata[] signals)
        {
            var socket = new ClientWebSocket();

            // Create a message deserializer
            // You might want to pass in signal-specific deserializers based on your requirements
            var deserializers = signals
                .Select(signal => signal.CreateDeserializer()) // Example: using float as default
                .ToArray();
            var messageDeserializer = new MessageDeserializer(deserializers);

            // Create the receiver client
            var receiverClient = new SignalsStream(socket, messageDeserializer);
            
            // Connect to the WebSocket
            await socket.ConnectAsync(CreateUrl(WsUrl, Frequency,signals), CancellationToken.None);
            
            receiverClient.Start();
            
            return receiverClient;
        }

        private static Uri CreateUrl(Uri? url, float frequency, ISignalMetadata[] signals)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            NameValueCollection query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["frequency"] = frequency.ToString("F2", CultureInfo.InvariantCulture);
            foreach (var signal in signals) 
                query.Add("signals", signal.Name.ToString());
            
            var uriBuilder = new UriBuilder(url) { Query = query.ToString() };
            Console.WriteLine($"Connecting: {uriBuilder.Uri}");
            return uriBuilder.Uri;
        }
    }
    

    public interface ISignalDeserializer
    {
        // Deserializes primitive using protobuf-net.
        int Deserialize(in Span<byte> data, out object value);
    }
    class SignalDeserializer<T> : ISignalDeserializer
    where T:struct
    {
        private static int Size = Marshal.SizeOf<T>();
        // Span contains some data, T is struct, shall be converted stright from the memory.
        public int Deserialize(in Span<byte> data, out object value)
        {
            value = MemoryMarshal.Read<T>(data.Slice(0, Size));
            return Size;
        }
    }

    public class MessageDeserializer
    {
        private readonly FrozenDictionary<ushort, ISignalDeserializer> _signals;

        public MessageDeserializer(params ISignalDeserializer[] deserializers)
        {
            _signals = deserializers
                .Select((item, index) => new ValueTuple<ISignalDeserializer, ushort>(item, (ushort)index))
                .ToFrozenDictionary(x => x.Item2, x => x.Item1);

        }
        public SortedListDictionary<ushort, object> DeserializeMessage(in Span<byte> data)
        {
            var signals = new SortedListDictionary<ushort, object>();
            int offset = 0;

            // Continue reading while there's enough data to read a signal key (2 bytes)
            while (offset + sizeof(ushort) <= data.Length)
            {
                // Read the signal key (index)
                ushort key = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, sizeof(ushort)));
                offset += sizeof(ushort);

                // Check if we have a deserializer for this key
                if (!_signals.TryGetValue(key, out var deserializer))
                    throw new InvalidOperationException("No deserializer found");

                // Ensure enough data remains for deserialization
                if (offset >= data.Length)
                    break;


                int bytesRead = deserializer.Deserialize(data.Slice(offset), out var value);
                offset += bytesRead;

                // Add to signals dictionary
                signals[key] = value;

            }

            return signals;
        }
    }

    public class SignalsQueueStream : ISignalsStream
    {
        private readonly ConcurrentQueue<SortedListDictionary<ushort, object>> _index = new();
        public bool TryRead(out SortedListDictionary<ushort, object>? result)
        {
            return _index.TryDequeue(out result);
        }
       
        public void Write(SortedListDictionary<ushort, object> data)
        {
            _index.Enqueue(data);
        }
        public void Write(ushort id, object value)
        {
            var d = new SortedListDictionary<ushort, object>();
            d.Add(id, value);
            _index.Enqueue(d);
        }
    }
    public interface ISignalsStream
    {
        bool TryRead(out SortedListDictionary<ushort, object>? result);
    }

    public class SignalsStream : IDisposable, ISignalsStream
    {
        private readonly ClientWebSocket _socket;
        private readonly MessageDeserializer _deserializer;
        private readonly Channel<SortedListDictionary<ushort, object>> _channel;
        private readonly CancellationTokenSource _cancellationTokenSource;
        public SignalsStream(ClientWebSocket socket, MessageDeserializer deserializer)
        {
            _socket = socket;
            _cancellationTokenSource = new CancellationTokenSource();

            _deserializer = deserializer;

            // Create an unbounded channel for signal data
            _channel = Channel.CreateUnbounded<SortedListDictionary<ushort, object>>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = true
                });
        }
        // Deserializes signals. We expect that in the web socket each message contains pairs of Signal-Id (short) and value; 
        // The Signal-Id allows us to find appropriate deserializer - this is primitive deserializer. Values are primitive types, such as float, double, int, etc.
        public void Start(){ _ = Task.Factory.StartNew(ReceiveSignalsAsync, TaskCreationOptions.LongRunning); }
        public IAsyncEnumerable<SortedListDictionary<ushort, object>> ReadAll(CancellationToken ct = default) => _channel.Reader.ReadAllAsync(ct);

        public bool TryRead(out SortedListDictionary<ushort, object>? result)
        {
            return _channel.Reader.TryRead(out result);
        }
        private async Task ReceiveSignalsAsync()
        {
            var buffer = new byte[1024 * 4]; // 4KB buffer
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var result = await _socket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        _cancellationTokenSource.Token
                    );

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closing",
                            _cancellationTokenSource.Token
                        );
                        break;
                    }

                    if (result.MessageType != WebSocketMessageType.Binary || result.Count <= 0) continue;
                    
                    // Deserialize received signals
                    var signals = _deserializer.DeserializeMessage(buffer.AsSpan(0, result.Count));
                    await _channel.Writer.WriteAsync(signals);
                }
            }
            catch (Exception)
            {
                // Handle or log exception
            }
            finally
            {
                _channel.Writer.Complete();
            }
        }
        

        public void Dispose()
        {
            _socket.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }

    public class SignalMetadataFactory
    {
        private static readonly Dictionary<string, Type> TypeMapping = new()
        {
            { "bool", typeof(bool) },
            { "Boolean", typeof(bool) },
            { "byte", typeof(byte) },
            { "Byte", typeof(byte) },
            { "sbyte", typeof(sbyte) },
            { "SByte", typeof(sbyte) },
            { "char", typeof(char) },
            { "Char", typeof(char) },
            { "decimal", typeof(decimal) },
            { "Decimal", typeof(decimal) },
            { "double", typeof(double) },
            { "Double", typeof(double) },
            { "float", typeof(float) },
            { "Single", typeof(float) },
            { "int", typeof(int) },
            { "Int32", typeof(int) },
            { "uint", typeof(uint) },
            { "UInt32", typeof(uint) },
            { "long", typeof(long) },
            { "Int64", typeof(long) },
            { "ulong", typeof(ulong) },
            { "UInt64", typeof(ulong) },
            { "short", typeof(short) },
            { "Int16", typeof(short) },
            { "ushort", typeof(ushort) },
            { "UInt16", typeof(ushort) },
        };
        public ISignalMetadata Create<T>(string name) where T : struct, INumber<T>
        {
            return new SignalMetadata<T>(name);
        }

        public ISignalMetadata Create(string name, string type)
        {
            return Create(name, TypeMapping[type]);
        }
        public ISignalMetadata Create(string name, Type type)
        {
            if (!typeof(INumber<>).MakeGenericType(type).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type {type} must implement INumber<T>.");
            }
            var genericType = typeof(SignalMetadata<>).MakeGenericType(type);
            return (ISignalMetadata)Activator.CreateInstance(genericType, name)!;
        }
    }

    readonly record struct SignalMetadata<T>(string name) : ISignalMetadata where T : struct, INumber<T>
    {
        public SignalName Name => new SignalName(name);
        public Type Type => typeof(T);
        public IAverage CreateAverage() => new Average<T>();
        public ISignalDeserializer CreateDeserializer() => new SignalDeserializer<T>();
    }
    public interface ISignalMetadata
    {
        SignalName Name { get; }
        Type Type { get; }
        IAverage CreateAverage();
        ISignalDeserializer CreateDeserializer();
    }

   


    // strongly typed structure for storing signal names. Should implement IParsable and explicit operator from string and to string.
    public readonly record struct SignalName : IParsable<SignalName>
    {
        public string Value { get; }

        public SignalName(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static SignalName Parse(string s, IFormatProvider? provider = null)
        {
            return new SignalName(s);
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out SignalName result)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                result = default;
                return false;
            }

            result = new SignalName(s);
            return true;
        }

        public static implicit operator SignalName(string value) => new SignalName(value);
        public static implicit operator string(SignalName name) => name.Value;

        public override string ToString() => Value;
    }


 public interface ISignalSink {}
    public interface ISignalSink<in T>
    {
        void Write(T value);
    }

    class SignalSink<T>(SignalHubServer srv, SignalName name) : ISignalSink<T>
    {
        public void Write(T value)
        {
            srv._channel.Writer.TryWrite(new SignalReceivedEvent(name, value));
        }

        
    }
}
