using Factory_Processing_and_Logistics_System.Components.Items;
using Factory_Processing_and_Logistics_System.Rendering;

namespace Factory_Processing_and_Logistics_System.Components
{
    /// <summary>
    /// Processes items from the order line one at a time with a random tick delay.
    /// Uses a seeded Random instance for reproducible pass/fail outcomes based on a configured quality percentage.
    /// Passed items move to Storage; failed items are discarded and logged.
    /// </summary>
    internal class QualityChecker
    {
        private readonly OrderLine _orderLine;
        private readonly Storage _storage;
        private readonly SimulationLogger _logger;
        private readonly Random _random;
        private readonly int _minTicks;
        private readonly int _maxTicks;
        private readonly int _qualityPercentage;
        private Item? _currentItem = null;
        private int _ticksRemaining = 0;

        public bool IsBusy => _currentItem != null;
        public string Status => _currentItem == null
            ? "Idle"
            : $"Processing item {_currentItem.Value.Id} ({_currentItem.Value.Type}) — {_ticksRemaining} tick(s) left";

        public QualityChecker(OrderLine orderLine, Storage storage, SimulationLogger logger, int minTicks, int maxTicks, int qualityPercentage, int randomSeed)
        {
            _orderLine = orderLine;
            _storage = storage;
            _logger = logger;
            _random = new Random(randomSeed);
            _minTicks = minTicks;
            _maxTicks = maxTicks;
            _qualityPercentage = qualityPercentage;
        }

        public void Tick(int currentTick)
        {
            if (_currentItem == null)
            {
                Item? item = _orderLine.TryDequeue();
                if (item == null) return;
                _currentItem = item;
                _ticksRemaining = _random.Next(_minTicks, _maxTicks + 1);
                _logger.Log($"[Tick {currentTick}] QualityChecker picked up item {_currentItem.Value.Id} ({_currentItem.Value.Type}), processing for {_ticksRemaining} tick(s).", ConsoleColor.Cyan);
                return;
            }

            _ticksRemaining--;
            if (_ticksRemaining > 0) return;

            bool passed = _random.Next(1, 101) <= _qualityPercentage;
            ItemStatus status = passed ? ItemStatus.Accepted : ItemStatus.Failed;
            Item processed = new Item(_currentItem.Value.Type, _currentItem.Value.Id, status);
            _currentItem = null;

            _logger.RecordResult(processed.Type, passed);

            if (passed)
            if (passed)
            {
                if (!_storage.AddItem(processed))
                    _logger.Log($"[Tick {currentTick}] Item {processed.Id} passed but storage is full — dropped.", ConsoleColor.DarkRed);
                else
                    _logger.Log($"[Tick {currentTick}] Item {processed.Id} ({processed.Type}) passed quality check → Storage.", ConsoleColor.Green);
            }
            else
            {
                _logger.Log($"[Tick {currentTick}] Item {processed.Id} ({processed.Type}) failed quality check — discarded.", ConsoleColor.Red);
            }
        }
    }
}
