using System.Collections;

namespace EventPi.Abstractions;

public class CircularBuffer<T> : IEnumerable<T>
{
    private readonly T[] _buffer;
    private int _head;
    private int _tail;
    private int _count;

    public int Capacity { get; private set; }

    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Circular buffer capacity must be positive", nameof(capacity));
        }

        Capacity = capacity;
        _buffer = new T[Capacity];
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    public T Head()
    {
        return _buffer[_head];
    }
    public bool AddLast(T item)
    {

        bool ret = false;
        // If the buffer is full, move the head to the next oldest element
        if (_count == Capacity)
        {

            _head = (_head + 1) % Capacity;
            ret = true;
        }
        else
            _count++;

        // Add the new item at the tail and move the tail to the next position
        _buffer[_tail] = item;
        _tail = (_tail + 1) % Capacity;
        return ret;
    }
    public T? Remove()
    {
        if (_count == 0)
            return default(T);

        T item = _buffer[_head];
        _head = (_head + 1) % Capacity;
        _count--;
        return item;
    }
    public IEnumerator<T> GetEnumerator()
    {
        var h = _head;
        var c = _count;
        for (int i = 0; i < c; i++)
        {
            yield return _buffer[(h + i) % Capacity];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}