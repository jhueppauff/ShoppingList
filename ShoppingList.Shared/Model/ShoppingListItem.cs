using Microsoft.WindowsAzure.Storage.Table;
using ShoppingList.Shared.Helper;
using System;

namespace ShoppingList.Shared.Model
{
    public class ShoppingListItem : TableEntity
    {
        public ShoppingListItem(string owner)
        {
            this.PartitionKey = HashHelper.ConvertToHash(owner);

            this.RowKey = Guid.NewGuid().ToString();
        }

        public ShoppingListItem() { }

        public string Name { get; set; }

        public double Amount { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public bool Done { get; set; }
    }
}
