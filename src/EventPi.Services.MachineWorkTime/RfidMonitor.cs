using EventPi.Abstractions;
using EventPi.Events.MachineWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicroPlumberd;
using System.Collections.Concurrent;

namespace EventPi.Services.MachineWorkTime;

public interface IRfidHandler : IAsyncDisposable
{
    Task Post([FromBody] RfidRequest request);
    event Func<Task>? WorkOnMachineStopped;
    event Func<Task>? WorkOnMachineStarted;
}


class RfidHandler : IAsyncDisposable, IRfidHandler
{
    
    private readonly IRfidState _state;
    private readonly IPlumberInstance _plumber;
    private readonly ILogger<RfidHandler> _logger;
    private readonly ConcurrentDictionary<string, byte> _uniqueRequests;
    private readonly IEnvironment _environment;
    private bool _shutdown;
    private int _postInvocationParallelCounter = 0;
    public RfidHandler(IPlumberInstance plumber, IRfidState state, ILogger<RfidHandler> logger, IEnvironment environment)
    {
        _state = state;
        _logger = logger;
        _environment = environment;
        _uniqueRequests = new ConcurrentDictionary<string, byte>();
        _plumber = plumber;
        
    }

    //public async Task PostPico([FromBody] PicoRfidSwipe request)
    //{
    //    if (Interlocked.Increment(ref _postInvocationParallelCounter) != 1)
    //    {
    //        // we detected concurrent call.
    //        Interlocked.Decrement(ref _postInvocationParallelCounter);
    //        return;
    //    }
    //    if (_shutdown) return;

    //    try
    //    {
    //        await OnPostPico(request);
    //    }
    //    finally
    //    {
    //        Interlocked.Decrement(ref _postInvocationParallelCounter);
    //    }
    //}

    //private async Task OnPostPico(PicoRfidSwipe request)
    //{
    //    if (string.IsNullOrWhiteSpace(request.CardId))
    //    {
    //        _logger.LogError("Cannot post empty card-id");
    //        return;
    //    }

    //    var requestId = $"{request.Device}/{request.SecretNumber}";
    //    if (_uniqueRequests.Add(requestId))
    //    {
    //        Guid id = DeviceStreamSuffix.Create(request.Device);
    //        var picoScanned = new PicoScanned
    //        {
    //            Device = request.Device,
    //            Card = request.CardId,
    //            Time = request.Time
    //        };
            
    //        await _eventStoreStream.Write(id, picoScanned);
    //    }
    //}

    public async ValueTask DisposeAsync()
    {
        _shutdown = true;
        while (_postInvocationParallelCounter > 0)
            await Task.Delay(20);

        if (_state.IsMachineWorking)
        {
            await OnMachineStopped(_state.GetCardId(), string.Empty);
            _state.IsMachineWorking = false;
        }

        _logger.LogInformation("Rfid service has gracefully closed.");
    }
    
    public async Task Post([FromBody] RfidRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CardId))
        {
            _logger.LogError("Cannot post empty card-id.");
            return;
        }

        if (!_state.ShouldSwipe(request.CardId))
            return;

        if (!ShouldStartInvocation()) 
            return;

        try
        {
            await SwapMachineWorkStatus(request.CardId);
        }
        finally
        {
            OnEndInvocation();
        }
    }

    private void OnEndInvocation()
    {
        Interlocked.Decrement(ref _postInvocationParallelCounter);
    }

    private bool ShouldStartInvocation()
    {
        if (Interlocked.Increment(ref _postInvocationParallelCounter) != 1)
        {
            // we detected concurrent call.
            Interlocked.Decrement(ref _postInvocationParallelCounter);
            return false;
        }

        if (_shutdown) return false;
        return true;
    }

    private async Task SwapMachineWorkStatus(string swipedCardId)
    {
        if (_state.IsMachineWorking)
            await OnMachineStopped(_state.GetCardId(), swipedCardId);
        else
            await OnMachineStarted(swipedCardId);

        _state.IsMachineWorking = !_state.IsMachineWorking;
    }

    public event Func<Task>? WorkOnMachineStopped;
    public event Func<Task>? WorkOnMachineStarted;
    private async Task OnMachineStopped(string startedWith, string swipedWith)
    {
        if(WorkOnMachineStopped != null)
            await WorkOnMachineStopped();

        await _plumber.AppendEvent(
            new WorkOnMachineStopped {
                StartedWithCard = startedWith,
                SwipedWithCard = swipedWith,
                DeviceSN = _environment.HostName.ToString(),
                Duration = DateTime.Now.Subtract(_started)
            }, _environment.HostName);
        _state.ClearCardId();

    }

    private DateTime _started;
    

    private async Task OnMachineStarted(string cardId)
    {
        if(WorkOnMachineStarted != null)
            await WorkOnMachineStarted();

        await _plumber.AppendEvent(
            new WorkOnMachineStarted { CardNumber = cardId, DeviceSN = _environment.HostName.ToString() },
            _environment.HostName);
        _state.SetCardId(cardId);   
        _started = DateTime.Now;
    }
    
}

public record RfidRequest
{
    public string CardId { get; init; }
}