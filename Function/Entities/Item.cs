using Microsoft.WindowsAzure.Storage.Table;

namespace ShoppingList.Functions.Entities
{
    public class Item : TableEntity
    {
        public Item()
        {    
        }

        public Item(string partitionKey, string RowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = RowKey;
        }
        
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string Name { get; set; }

        public string Amount { get; set; }

        public bool Done { get; set; }
    }
}