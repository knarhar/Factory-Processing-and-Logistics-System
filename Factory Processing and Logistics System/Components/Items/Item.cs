namespace Factory_Processing_and_Logistics_System.Components.Items
{
    /// <summary>
    /// Represents a single unit moving through the pipeline.
    /// Holds a globally unique ID, an item type (A/B/C), and a quality status stamped by the QualityChecker.
    /// Implemented as a readonly struct for value semantics and immutability.
    /// </summary>
    public readonly struct Item
    {
        public readonly ItemType Type;
        public readonly int Id;
        public readonly ItemStatus Status;

        public Item(ItemType type, int id)
        {
            Type = type;
            Id = id;
            Status = ItemStatus.Accepted; // default
        }

        public Item(ItemType type, int id, ItemStatus status)
        {
            Type = type;
            Id = id;
            Status = status;
        }
    }
}
