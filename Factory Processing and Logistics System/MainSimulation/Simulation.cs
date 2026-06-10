using Factory_Processing_and_Logistics_System.Components;
using Factory_Processing_and_Logistics_System.Core;
using Factory_Processing_and_Logistics_System.Rendering;

namespace Factory_Processing_and_Logistics_System.MainSimulation
{
    internal class Simulation
    {
        private readonly Config _config;
        private readonly Machine[] _machines;
        private readonly OrderLine _orderLine;
        private readonly QualityChecker _qualityChecker;
        private readonly Storage _storage;
        private readonly Stock _stock;
        private readonly TransportSystem _transport;
        private readonly SimulationLogger _logger;

        private int _currentTick = 0;
        private bool _running = false;

        public Simulation()
        {
            _config = new Config();

            Machine.SetStartId(_config.StartItem);

            _logger = new SimulationLogger(historySize: 10);

            _orderLine = new OrderLine(_config.OrderLineCapacity);
            _storage = new Storage(_config.StorageCapacity);
            _stock = new Stock(_config.StockCapacity);

            _machines = new Machine[]
            {
            new Machine(ItemType.A, _config.MachineAProdCount, _config.MachineAProdInterval),
            new Machine(ItemType.B, _config.MachineBProdCount, _config.MachineBProdInterval),
            new Machine(ItemType.C, _config.MachineCProdCount, _config.MachineCProdInterval),
            };

            _qualityChecker = new QualityChecker(_orderLine, _storage, _logger, _config.MinQualityCheckTick,
                _config.MaxQualityCheckTick, _config.QualityPercentage, _config.RandomSeed);

            _transport = new TransportSystem(_storage, _stock, _logger,
                _config.TransportArrivalInterval, _config.TransportCapacity);
        }

        private bool IsComplete()
        {
            bool machinesDone = true;
            foreach (var m in _machines)
                if (!m.IsDone) machinesDone = false;

            return machinesDone
                && _orderLine.Count == 0
                && _orderLine.WaitCount == 0
                && !_qualityChecker.IsBusy
                && _storage.IsEmpty;
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            _running = true;
            Console.WriteLine("Simulation started. Press Q to quit.");
            Thread.Sleep(1500);

            while (_running)
            {
                _currentTick++;

                foreach (var machine in _machines)
                {
                    Item? item = machine.ProduceItem(_currentTick);
                    if (item != null)
                        _orderLine.AddItem(item.Value);
                }

                _qualityChecker.Tick(_currentTick);
                _transport.Tick(_currentTick);

                _logger.Render(_currentTick, _machines, _orderLine, _qualityChecker, _storage, _stock);

                if (IsComplete())
                {
                    _logger.Render(_currentTick, _machines, _orderLine, _qualityChecker, _storage, _stock);
                    Console.WriteLine("\n  ✓ All items processed. Simulation complete.");
                    _logger.PrintSummary(_machines);
                    break;
                }

                if (Console.KeyAvailable && Console.ReadKey(intercept: true).Key == ConsoleKey.Q)
                    _running = false;

                Thread.Sleep(300);
            }

            Console.WriteLine("\nSimulation stopped.");
        }
    }
}
