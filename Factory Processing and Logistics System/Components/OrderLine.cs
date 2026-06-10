using Factory_Processing_and_Logistics_System.Components.Items;

namespace Factory_Processing_and_Logistics_System.Components
{
    /// <summary>
    /// A bounded input buffer between machines and the quality checker.
    /// Maintains a fixed-capacity main queue and an unbounded overflow waitlist.
    /// Drains the waitlist automatically whenever a slot opens up to preserve FIFO order end-to-end.
    /// All operations are thread-safe via lock.
    /// </summary>
    internal class OrderLine
    {
        private readonly object _lock = new object();
        private readonly Queue _list;
        private readonly Queue _waitList;
        private int _count = 0;
        private int _capacity = 0;
        private int _waitlistCount = 0;
        public int Capacity => _capacity;
        public int Count => _count;
        public int WaitCount => _waitlistCount;

        public OrderLine(int capacity)
        {
            _capacity = capacity;
            _list = new Queue(capacity);
            _waitList = new Queue(); // unbounded, resizable
        }

        public void AddItem(Item item)
        {
            lock (_lock)
            {
                if (!_list.IsFull)
                {
                    while (!_waitList.IsEmpty && !_list.IsFull)
                    {
                        _list.Enqueue(_waitList.Dequeue());
                        _waitlistCount--;
                    }

                    if (!_list.IsFull)
                    {
                        _list.Enqueue(item);
                        _count++;
                    }
                    else
                    {
                        _waitList.Enqueue(item);
                        _waitlistCount++;
                    }
                }
                else
                {
                    _waitList.Enqueue(item);
                    _waitlistCount++;
                }
            }
        }

        public Item? TryDequeue()
        {
            lock (_lock)
            {
                if (_list.IsEmpty) return null;
                _count--;                   // was missing
                Item item = _list.Dequeue();

                // drain one waitlist item to fill the freed slot
                if (!_waitList.IsEmpty)
                {
                    _list.Enqueue(_waitList.Dequeue());
                    _waitlistCount--;
                    _count++;
                }

                return item;
            }
        }

        public bool IsEmpty
        {
            get { lock (_lock) { return _list.IsEmpty; } }
        }
    }

}
