using System;
using Microsoft.WindowsAzure.Storage.Table;
using ShoppingList.Shared.Helper;


namespace ShoppingList.Shared.Model
{
    public class ShoppingList : TableEntity
    {
        public ShoppingList(string name, string owner)
        {
            this.PartitionKey = HashHelper.ConvertToHash(owner);
            this.RowKey = name;
        }

        public ShoppingList() { }

        public DateTime Created { get; set; }
    }
}
