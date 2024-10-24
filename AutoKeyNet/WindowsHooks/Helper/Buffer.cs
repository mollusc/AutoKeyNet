using System.Collections;

namespace AutoKeyNet.WindowsHooks.Helper;

public class Buffer<T> : IEnumerable<T>
{
    private readonly Queue<T> _buffer;
    private int _limit = -1;

    public int Limit
    {
        get => _limit;
        set
        {
            _limit = value;
            while (_buffer.Count > _limit)
            {
                _buffer.Dequeue();
            }
        }
    }
    public int Count => _buffer.Count;

    public Buffer(int limit)
    {
        _buffer = new Queue<T>(limit);
        this.Limit = limit;
    }

    public void Add(T item)
    {
        while (_buffer.Count >= _limit)
        {
            _buffer.Dequeue();
        }
        _buffer.Enqueue(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _buffer.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_buffer).GetEnumerator();
    }
}