namespace ShoppingList.Data
{
    public class ShoppingItemRequest
    {
        public string Name { get; set; }

        public double Amount { get; set; }

        public string Owner { get; set; }

        public string ListName { get; set; }

        public string Id { get; set; }

        public bool Completed { get; set; }

        public string Unit { get; set; }
    }
}
