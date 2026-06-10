namespace Factory_Processing_and_Logistics_System.Components
{
    public enum ItemType
    {
        A = 1,
        B = 2,
        C = 3,
    }

    public enum ItemStatus
    {
        Accepted = 1,
        Failed = 2,
    }

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
