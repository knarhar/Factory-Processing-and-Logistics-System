namespace Factory_Processing_and_Logistics_System.Core
{
    /// <summary>
    /// Reads and holds all simulation parameters via interactive console prompts at startup.
    /// Each parameter has a validated default value the user can accept by pressing Enter.
    /// Exposes all settings as public read-only properties consumed by the simulation and its components.
    /// </summary>
    internal class Config
    {
        public int StartItem { get; private set; } = 100;
        public int OrderLineCapacity { get; private set; } = 5;
        public int StorageCapacity { get; private set; } = 50;
        public int StockCapacity { get; private set; } = 200;
        public int MinQualityCheckTick { get; private set; } = 1;
        public int MaxQualityCheckTick { get; private set; } = 3;
        public int QualityPercentage { get; private set; } = 70;
        public int TransportArrivalInterval { get; private set; } = 4;
        public int TransportCapacity { get; private set; } = 6;
        public int MachineAProdInterval { get; private set; } = 1;
        public int MachineBProdInterval { get; private set; } = 2;
        public int MachineCProdInterval { get; private set; } = 3;
        public int MachineAProdCount { get; private set; } = 1;
        public int MachineBProdCount { get; private set; } = 1;
        public int MachineCProdCount { get; private set; } = 1;
        public int RandomSeed { get; private set; } = 42;

        public Config()
        {
            StartItem = Prompt("Start item ID", StartItem, 1, 1000);
            OrderLineCapacity = Prompt("Order line capacity", OrderLineCapacity, 1, 100);
            StorageCapacity = Prompt("Storage capacity", StorageCapacity, 1, 1000);
            StockCapacity = Prompt("Stock capacity", StockCapacity, 1, 10000);
            MinQualityCheckTick = Prompt("Min quality check tick", MinQualityCheckTick, 1, 100);
            MaxQualityCheckTick = Prompt("Max quality check tick", MaxQualityCheckTick, MinQualityCheckTick, 100);
            QualityPercentage = Prompt("Quality percentage", QualityPercentage, 0, 100);
            RandomSeed = Prompt("Random seed", RandomSeed, 0, 100000000);
            TransportArrivalInterval = Prompt("Transport arrival interval", TransportArrivalInterval, 1, 100);
            TransportCapacity = Prompt("Transport capacity", TransportCapacity, 1, 100);
            MachineAProdInterval = Prompt("Machine A production interval", MachineAProdInterval, 1, 100);
            MachineBProdInterval = Prompt("Machine B production interval", MachineBProdInterval, 1, 100);
            MachineCProdInterval = Prompt("Machine C production interval", MachineCProdInterval, 1, 100);
            MachineAProdCount = Prompt("Machine A production count", MachineAProdCount, 1, 100);
            MachineBProdCount = Prompt("Machine B production count", MachineBProdCount, 1, 100);
            MachineCProdCount = Prompt("Machine C production count", MachineCProdCount, 1, 100);
        }

        private int Prompt(string name, int currentValue, int min, int max)
        {
            return ReadInt($"{name} [default: {currentValue}]", min, max, currentValue);
        }

        public int ReadInt(string textToDisplay, int min, int max, int defaultInt)
        {
            while (true)
            {
                Console.Write($"{textToDisplay}: ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    return defaultInt;
                if (int.TryParse(input, out int value) && value >= min && value <= max)
                    return value;
                Console.WriteLine($"Please enter a number between {min} and {max}, or press Enter for {defaultInt}.");
            }
        }
    }
}
