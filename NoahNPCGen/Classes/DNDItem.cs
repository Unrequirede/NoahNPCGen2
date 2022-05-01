namespace NoahNPCGen
{
    public class DNDItem
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DNDItem(string index, string name, int quantity)
        {
            Index = index;
            Name = name;
            Quantity = quantity;
        }
    }
}
