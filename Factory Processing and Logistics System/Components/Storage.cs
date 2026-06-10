using Factory_Processing_and_Logistics_System.Components.Items;

namespace Factory_Processing_and_Logistics_System.Components
{
    /// <summary>
    /// Intermediate warehouse that holds items which passed quality inspection.
    /// Extends ItemShelf with TakeOldestItem(), which compares the front of each type shelf
    /// and returns the item with the smallest ID, preserving global arrival order across types.
    /// </summary>
    internal class Storage : ItemShelf
    {
        public Storage(int capacity) : base(capacity) { }

        public Item? TakeOldestItem()
        {
            lock (_lock)
            {
                if (IsEmpty) return null;

                int oldestShelfIndex = -1;
                int oldestId = int.MaxValue;

                for (int i = 0; i < _shelves.Length; i++)
                {
                    if (!_shelves[i].IsEmpty)
                    {
                        Item front = _shelves[i].Peek();
                        if (front.Id < oldestId)
                        {
                            oldestId = front.Id;
                            oldestShelfIndex = i;
                        }
                    }
                }

                if (oldestShelfIndex == -1) return null;
                _totalCount--;
                return _shelves[oldestShelfIndex].Dequeue();
            }
        }
    }
}
