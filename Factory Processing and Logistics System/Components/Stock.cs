namespace Factory_Processing_and_Logistics_System.Components
{
    /// <summary>
    /// Final destination for all items delivered by the transport system.
    /// Extends ItemShelf with no removal operations — items are permanently stored here,
    /// organized by type, representing the factory's completed output.
    /// </summary>
    internal class Stock : ItemShelf
    {
        public Stock(int capacity) : base(capacity) { }
    }
}
