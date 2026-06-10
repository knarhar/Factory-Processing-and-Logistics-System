using Factory_Processing_and_Logistics_System.Components;
using Factory_Processing_and_Logistics_System.Components.Items;

namespace Factory_Processing_and_Logistics_System.Rendering
{
    /// <summary>
    /// Central logging and rendering component for the simulation.
    /// Collects colored event messages from all pipeline stages, tracks pass/fail counts per item type,
    /// renders the full simulation state to the console each tick, and prints the final summary on completion.
    /// </summary>
    internal class SimulationLogger
    {
        private readonly RollingLog _log;
        private readonly object _lock = new object();
        private int _totalPassed = 0;
        private int _totalFailed = 0;
        private readonly int[] _passedByType = new int[3]; // A=0, B=1, C=2
        private readonly int[] _failedByType = new int[3];
        public SimulationLogger(int historySize = 10)
        {
            _log = new RollingLog(historySize);
        }

        public void RecordResult(ItemType type, bool passed)
        {
            lock (_lock)
            {
                if (passed)
                {
                    _passedByType[(int)type - 1]++;
                    _totalPassed++;
                }
                else
                {
                    _failedByType[(int)type - 1]++;
                    _totalFailed++;
                }
            }
        }
        public void Log(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            lock (_lock)
            {
                _log.Add((message, color));
            }
        }

        public void PrintSummary(Machine[] machines)
        {
            Console.WriteLine("\n╔══════════════════════════════════════════════╗");
            Console.WriteLine("║              Simulation Summary              ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");

            foreach (var m in machines)
            {
                int idx = (int)m.Type - 1;
                int passed = _passedByType[idx];
                int failed = _failedByType[idx];

                Console.Write($"  Machine {m.Type} — {m.ProducedCount} produced,  ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{passed} passed");
                Console.ResetColor();
                Console.Write(",  ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{failed} failed");
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.Write("  Total passed: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(_totalPassed);
            Console.ResetColor();

            Console.Write("  Total failed: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(_totalFailed);
            Console.ResetColor();
            Console.WriteLine();
        }

        public void Render(int tick, Machine[] machines, OrderLine orderLine, QualityChecker checker, Storage storage, Stock stock)
        {
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine($"║   Factory Simulation — Tick {tick,-17}║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");

            Console.WriteLine("\n── Machines ──────────────────────────────────");
            foreach (var m in machines)
                Console.WriteLine($"  Machine {m.Type,-4} | produced: {m.ProducedCount}");

            Console.WriteLine("\n── Order Line ────────────────────────────────");
            Console.WriteLine($"  Queued:   {orderLine.Count} / {orderLine.Capacity}");
            Console.WriteLine($"  Overflow: {orderLine.WaitCount}");

            Console.WriteLine("\n── Quality Checker ───────────────────────────");
            Console.WriteLine($"  Status: {checker.Status}");

            Console.WriteLine("\n── Storage ───────────────────────────────────");
            Console.WriteLine($"  Total:  {storage.TotalCount}");
            Console.WriteLine($"  Type A: {storage.CountByType(ItemType.A)}");
            Console.WriteLine($"  Type B: {storage.CountByType(ItemType.B)}");
            Console.WriteLine($"  Type C: {storage.CountByType(ItemType.C)}");

            Console.WriteLine("\n── Stock ─────────────────────────────────────");
            Console.WriteLine($"  Total:  {stock.TotalCount}");
            Console.WriteLine($"  Type A: {stock.CountByType(ItemType.A)}");
            Console.WriteLine($"  Type B: {stock.CountByType(ItemType.B)}");
            Console.WriteLine($"  Type C: {stock.CountByType(ItemType.C)}");

            Console.WriteLine("\n── Product Flow ──────────────────────────────");
            for (int i = 0; i < _log.Count; i++)
            {
                var entry = _log.Get(i);
                Console.ForegroundColor = entry.color;
                Console.WriteLine($"  {entry.message}");
                Console.ResetColor();
            }

            Console.WriteLine("\n──────────────────────────────────────────────");
            Console.WriteLine("  Press Q to quit.");
            Console.WriteLine();
        }
    }
}
