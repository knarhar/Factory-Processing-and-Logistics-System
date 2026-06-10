using Factory_Processing_and_Logistics_System.Components.Items;

namespace Factory_Processing_and_Logistics_System.Components
{
    /// <summary>
    /// Abstract base class for grouped item storage.
    /// Maintains one internal Queue per item type (A/B/C) with a shared capacity ceiling.
    /// Provides thread-safe AddItem and CountByType operations inherited by Storage and Stock.
    /// </summary>
    internal abstract class ItemShelf
    {
        protected readonly Queue[] _shelves;
        protected readonly object _lock = new object();
        protected readonly int _capacity;
        protected int _totalCount = 0;

        public bool IsFull => _totalCount >= _capacity;
        public bool IsEmpty => _totalCount == 0;
        public int TotalCount => _totalCount;

        protected ItemShelf(int capacity)
        {
            _capacity = capacity;
            _shelves = new Queue[3];
            _shelves[0] = new Queue();
            _shelves[1] = new Queue();
            _shelves[2] = new Queue();
        }

        public bool AddItem(Item item)
        {
            lock (_lock)
            {
                if (IsFull) return false;
                _shelves[(int)item.Type - 1].Enqueue(item);
                _totalCount++;
                return true;
            }
        }

        public int CountByType(ItemType type)
        {
            lock (_lock)
            {
                return _shelves[(int)type - 1].Count;
            }
        }
    }
}
