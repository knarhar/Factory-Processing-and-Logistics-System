namespace Factory_Processing_and_Logistics_System.Rendering
{
    /// <summary>
    /// A fixed-size circular event log that overwrites the oldest entry when full.
    /// Stores message and color pairs for colored console output.
    /// Exposes entries via an indexed getter to avoid Action or delegate dependencies.
    /// </summary>
    internal class RollingLog
    {
        private readonly (string message, ConsoleColor color)[] _entries;
        private int _front;
        private int _count;
        private readonly int _capacity;
        public int Count => _count;

        public RollingLog(int capacity)
        {
            _capacity = capacity;
            _entries = new (string, ConsoleColor)[capacity];
            _front = 0;
            _count = 0;
        }

        public void Add((string message, ConsoleColor color) entry)
        {
            int index = (_front + _count) % _capacity;
            if (_count < _capacity)
            {
                _entries[index] = entry;
                _count++;
            }
            else
            {
                // overwrite oldest
                _entries[_front] = entry;
                _front = (_front + 1) % _capacity;
            }
        }

        public (string message, ConsoleColor color) Get(int index)
        {
            return _entries[(_front + index) % _capacity];
        }
    }
}
