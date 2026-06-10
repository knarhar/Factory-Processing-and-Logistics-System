namespace Factory_Processing_and_Logistics_System.Components
{
    /// <summary>
    /// Produces items
    /// 
    /// </summary>
    internal class Machine
    {
        private static int _globalId = 100; // default from config
        private int _lastProducedTick = 0;
        private int _producedItemsCount = 0;

        public ItemType Type { get; set; }
        public int lastId { get; set; }
        public int ItemsToProduce { get; set; }
        public int ProducingInterval { get; set; }
        public int ProducedCount => _producedItemsCount;

        public Machine(ItemType type, int itemCount, int tickCount)
        {
            Type = type;
            ItemsToProduce = itemCount;
            ProducingInterval = tickCount;
        }

        public static void SetStartId(int startId)
        {
            _globalId = startId;
        }

        public bool IsDone => _producedItemsCount >= ItemsToProduce;

        public Item? ProduceItem(int currentTick)
        {
            if (currentTick - _lastProducedTick >= ProducingInterval
                && _producedItemsCount < ItemsToProduce)
            {
                _lastProducedTick = currentTick;
                _producedItemsCount++;
                int id = Interlocked.Increment(ref _globalId);
                return new Item(Type, id);
            }
            return null;
        }
    }
}
