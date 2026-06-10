using Factory_Processing_and_Logistics_System.Components.Items;

namespace Factory_Processing_and_Logistics_System.Components
{
    /// <summary>
    /// A circular buffer implementation of a FIFO queue.
    /// Supports both fixed-capacity (bounded) and resizable (unbounded) modes.
    /// Resizing preserves logical order by walking from front to rear rather than copying raw slots.
    /// </summary>
    internal class Queue
    {
        private Item[] _items;
        private int _front;
        private int _rear;
        private int _count;
        private bool _resizable = false;

        // the resizing feature
        public Queue()
        {
            _items = new Item[4];
            _front = 0;
            _rear = -1;
            _count = 0;
            _resizable = true;
        }

        public void Resize()
        {
            int newCapacity = _items.Length * 2;
            Item[] newArray = new Item[newCapacity];
            for (int i = 0; i < _count; i++)
            {
                newArray[i] = _items[(_front + i) % _items.Length];
            }
            _items = newArray;
            _front = 0;
            _rear = _count - 1;
        }

        public Queue(int capacity)
        {
            _items = new Item[capacity];
            _front = 0;
            _rear = -1;
            _count = 0;
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public bool IsFull => _count == _items.Length;

        public void Enqueue(Item item)
        {
            if (IsFull)
            {
                if (_resizable)
                    Resize();
                else
                    throw new InvalidOperationException("Queue is full.");
            }

            _rear = (_rear + 1) % _items.Length;
            _items[_rear] = item;
            _count++;
        }

        public Item Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            Item item = _items[_front];
            _front = (_front + 1) % _items.Length;
            _count--;

            return item;
        }

        public Item Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return _items[_front];
        }
    }
}

