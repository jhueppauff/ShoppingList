using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Logging;

namespace ShoppingList.Function
{
    public static class ListOperations
    {
        [FunctionName("GetShoppingLists")]
        public static async Task<List<Shared.Model.ShoppingList>> GetShoppingLists(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] dynamic shoppingList,
            [Table("ShoppingLists")] CloudTable cloudTable, ILogger log)
        {
            try
            {
                string partitionKey = Shared.Helper.HashHelper.ConvertToHash(shoppingList.PartitionKey.ToString());

                TableQuery<Shared.Model.ShoppingList> rangeQuery = new TableQuery<Shared.Model.ShoppingList>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                List<Shared.Model.ShoppingList> lists = new List<Shared.Model.ShoppingList>();

                // Execute the query and loop through the results
                foreach (var entity in
                    await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null))
                {
                    lists.Add(entity);
                }

                return lists;
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogDebug(ex.StackTrace);
                throw;
            }
        }

        [FunctionName("GetShoppingListItems")]
        public static async Task<List<Shared.Model.ShoppingListItem>> GetShoppingListItems(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] dynamic shoppingListItem,
            [Table("ShoppingListItems")] CloudTable cloudTable, ILogger log)
        {
            try
            {
                string partitionKey = Shared.Helper.HashHelper.ConvertToHash($"{shoppingListItem.Owner.ToString()}-{shoppingListItem.ListName.ToString()}");

                TableQuery<Shared.Model.ShoppingListItem> rangeQuery = new TableQuery<Shared.Model.ShoppingListItem>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

                List<Shared.Model.ShoppingListItem> items = new List<Shared.Model.ShoppingListItem>();

                // Execute the query and loop through the results
                foreach (var entity in await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null).ConfigureAwait(false))
                {
                    items.Add(entity);
                }

                return items;
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogDebug(ex.StackTrace);
                throw;
            }
        }

        [FunctionName("CreateShoppingList")]
        [return: Table("ShoppingLists")]
        public static Shared.Model.ShoppingList CreateShoppingList(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] dynamic shoppingList)
        {
            return new Shared.Model.ShoppingList(shoppingList.RowKey.ToString(), shoppingList.PartitionKey.ToString()) { Created = DateTime.Now };
        }

        [FunctionName("CreateShoppingListItem")]
        [return: Table("ShoppingListItems")]
        public static Shared.Model.ShoppingListItem CreateShoppingListItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] dynamic shoppingListItem)
        {
            return new Shared.Model.ShoppingListItem($"{shoppingListItem.Owner.ToString()}-{shoppingListItem.ListName.ToString()}")
            {
                Name = shoppingListItem.Name.ToString(),
                Amount = Convert.ToDouble(shoppingListItem.Amount),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                Done = false
            };
        }

    }
}
