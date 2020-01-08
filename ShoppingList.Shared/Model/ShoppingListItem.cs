using Microsoft.WindowsAzure.Storage.Table;
using ShoppingList.Shared.Helper;
using System;

namespace ShoppingList.Shared.Model
{
    public class ShoppingListItem : TableEntity
    {
        public ShoppingListItem(string PartitionKey)
        {
            this.PartitionKey = HashHelper.ConvertToHash(PartitionKey);

            this.RowKey = Guid.NewGuid().ToString();
        }

        public ShoppingListItem(string PartitionKey, string RowKey)
        {
            this.PartitionKey = HashHelper.ConvertToHash(PartitionKey);

            this.RowKey = RowKey;
        }

        public ShoppingListItem() { }

        public string Name { get; set; }

        public double Amount { get; set; }

        public string Unit { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public bool Done { get; set; }
    }
}
