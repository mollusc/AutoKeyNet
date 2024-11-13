using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoKeyNet.Helper
{
    public class CircularBuffer<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private readonly T[] _buffer;
        private int _start;
        private int _end;

        public CircularBuffer(int capacity)
          : this(capacity, Array.Empty<T>())
        {
        }

        public CircularBuffer(int capacity, T[] items)
        {
            if (capacity < 1)
                throw new ArgumentException("Circular buffer must have a capacity greater than 0.", nameof(capacity));
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (items.Length > capacity)
                throw new ArgumentException("Too many items to fit circular buffer", nameof(items));
            _buffer = new T[capacity];
            Array.Copy(items, _buffer, items.Length);
            Count = items.Length;
            _start = 0;
            _end = Count == capacity ? 0 : Count;
        }

        public int Capacity => _buffer.Length;

        public bool IsFull => Count == Capacity;

        public bool IsEmpty => Count == 0;

        public int Count { get; private set; }

        public bool IsReadOnly { get; }

        public bool IsFixedSize { get; } = true;

        public object SyncRoot { get; } = new object();

        public bool IsSynchronized { get; }

        public int IndexOf(T item)
        {
            for (int index = 0; index < Count; ++index)
            {
                if (Equals(this[index], item))
                    return index;
            }
            return -1;
        }

        public void Insert(int index, T item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        public bool Remove(T item) => throw new NotImplementedException();

        public T this[int index]
        {
            get => _buffer[InternalIndex(index)];
            set => _buffer[InternalIndex(index)] = value;
        }

        public void Add(T item)
        {
            if (IsFull)
            {
                _buffer[_end] = item;
                Increment(ref _end);
                _start = _end;
            }
            else
            {
                _buffer[_end] = item;
                Increment(ref _end);
                ++Count;
            }
        }

        public void Clear()
        {
            Count = 0;
            _start = 0;
            _end = 0;
        }

        public bool Contains(T item) => IndexOf(item) != -1;

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Array does not contain enough space for items");
            for (int index = 0; index < Count; ++index)
                array[index + arrayIndex] = this[index];
        }

        public T[] ToArray()
        {
            if (IsEmpty)
                return Array.Empty<T>();
            T[] array = new T[Count];
            for (int index = 0; index < Count; ++index)
                array[index] = this[index];
            return array;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int InternalIndex(int index)
        {
            if (IsEmpty)
                throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty");
            if (index >= Count)
                throw new IndexOutOfRangeException(
                    $"Cannot access index {index}. Buffer size is {Count}");
            return (_start + index) % Capacity;
        }

        private void Increment(ref int index)
        {
            if (++index < Capacity)
                return;
            index = 0;
        }

        private void Decrement(ref int index)
        {
            if (index <= 0)
                index = Capacity - 1;
            --index;
        }
        public void PopBack()
        {
            if (IsEmpty)
                throw new IndexOutOfRangeException($"Cannot pop from back. Buffer is empty");
            Decrement(ref _end);
            --Count;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (T c in this)
                sb.Append(c);
            return sb.ToString();
        }
    }
}
