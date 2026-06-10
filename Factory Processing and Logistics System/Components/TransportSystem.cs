using Factory_Processing_and_Logistics_System.Components.Items;
using Factory_Processing_and_Logistics_System.Rendering;

namespace Factory_Processing_and_Logistics_System.Components
{
    /// <summary>
    /// Periodically collects items from Storage and delivers them to Stock.
    /// Arrives every N ticks, picks up to its capacity using Storage.TakeOldestItem(),
    /// and logs each arrival and delivery count.
    /// </summary>
    internal class TransportSystem
    {
        private readonly Storage _storage;
        private readonly Stock _stock;
        private readonly SimulationLogger _logger;
        private readonly int _arrivalInterval;
        private readonly int _capacity;
        private int _lastArrivalTick = 0;

        public TransportSystem(Storage storage, Stock stock, SimulationLogger logger, int arrivalInterval, int capacity)
        {
            _storage = storage;
            _stock = stock;
            _logger = logger;
            _arrivalInterval = arrivalInterval;
            _capacity = capacity;
        }

        public void Tick(int currentTick)
        {
            if (currentTick - _lastArrivalTick < _arrivalInterval) return;

            _lastArrivalTick = currentTick;
            _logger.Log($"[Tick {currentTick}] Transport arrived.", ConsoleColor.Yellow);

            int loaded = 0;
            while (loaded < _capacity)
            {
                Item? item = _storage.TakeOldestItem();
                if (item == null) break;

                bool accepted = _stock.AddItem(item.Value);
                if (!accepted)
                {
                    _logger.Log($"[Tick {currentTick}] Stock full, item {item.Value.Id} lost.", ConsoleColor.DarkRed);
                    break;
                }

                loaded++;
            }

            _logger.Log($"[Tick {currentTick}] Transport delivered {loaded} item(s) to stock.", ConsoleColor.Yellow);
        }
    }
}