using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using System.Text;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;

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

        [FunctionName("DeleteShoppingList")]
        [return: ServiceBus("deleteprocessing", Connection = "ServiceBusConnection")]
        public static async Task<Message> DeleteShoppingList(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] dynamic shoppingList, 
            [Table("ShoppingLists")] CloudTable cloudTable)
        {
            Shared.Model.ShoppingList list = new Shared.Model.ShoppingList(shoppingList.RowKey.ToString(), shoppingList.PartitionKey.ToString())
            {
                ETag = "*"
            };

            var operation = TableOperation.Delete(list);

            await cloudTable.ExecuteAsync(operation);

            return new Message()
            {
                Body = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(list)),
                ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddDays(1)
            };
        }

        [FunctionName("CleanupShoppingItemsOfDeletedLists")]
        public static async Task CleanupShoppingItemsOfDeletedLists(
            [ServiceBusTrigger("deleteprocessing", Connection = "ServiceBusConnection")] string item,
            [Table("ShoppingListItems")] CloudTable cloudTable)
        {
            Shared.Model.ShoppingList list = JsonConvert.DeserializeObject<Shared.Model.ShoppingList>(item);

            Shared.Model.ShoppingListItem listItem = new Shared.Model.ShoppingListItem(list.PartitionKey)
            {
                RowKey = list.RowKey
            };

            var operation = TableOperation.Delete(listItem);

            await cloudTable.ExecuteAsync(operation);
        }

        [FunctionName("CompleteListItem")]
        public static async Task CompleteListItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] dynamic shoppingListItem,
            [Table("ShoppingListItems")] CloudTable cloudTable)
        {
            var entity = new DynamicTableEntity(Shared.Helper.HashHelper.ConvertToHash($"{shoppingListItem.Owner.ToString()}-{shoppingListItem.ListName.ToString()}"), shoppingListItem.Id.ToString());
            entity.Properties.Add("Done", new EntityProperty(Convert.ToBoolean(shoppingListItem.Completed)));
            entity.Properties.Add("Modified", new EntityProperty(DateTime.UtcNow));
            entity.ETag = "*";

            var mergeOperation = TableOperation.Merge(entity);

            try
            {
                await cloudTable.ExecuteAsync(mergeOperation).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {

                throw;
            }

            
        }

        [FunctionName("DeleteListItem")]
        public static async Task DeleteListItem([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] dynamic shoppingListItem,
            [Table("ShoppingListItems")] CloudTable cloudTable, ILogger log)
        {
            var entity = new DynamicTableEntity(Shared.Helper.HashHelper.ConvertToHash($"{shoppingListItem.Owner.ToString()}-{shoppingListItem.ListName.ToString()}"), shoppingListItem.Id.ToString());
            entity.ETag = "*";

            var deleteOperation = TableOperation.Delete(entity);

            try
            {
                await cloudTable.ExecuteAsync(deleteOperation).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                log.LogWarning(ex.RequestInformation.ExtendedErrorInformation.ErrorMessage);
                throw;
            }
        }

    }
}
