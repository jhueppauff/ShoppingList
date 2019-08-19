using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShoppingList.Functions.Entities;
using System.Security.Claims;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net.Http;
using System.Net;

namespace ShoppingList.Functions
{
    public static class Actions
    {
        [FunctionName("AddNewEntry")]
        [return: Table("outTable", Connection = "TableStorage")]
        public static Item AddNewEntry(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] dynamic input,
            ILogger log, ClaimsPrincipal claimsPrincipal)
        {
            log.LogInformation($"C# Item added {input.Name}.");

            return new Item
            {
                PartitionKey = $"{claimsPrincipal.Identity.Name}_{input.listName})",
                RowKey = Guid.NewGuid().ToString(),
                Name = input.Name,
                Amount = input.Amount
            };
        }

        [FunctionName("UpdateEntry")]
        public static async Task<HttpResponseMessage> UpdateEntry(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] dynamic input, CloudTable outputTable,
            ILogger log, ClaimsPrincipal claimsPrincipal)
        {
            log.LogInformation($"C# Item updated {input.Name}.");

            var item = new Item
            {
                PartitionKey = input.PartitionKey,
                RowKey = $"{claimsPrincipal.Identity.Name}_{input.listName})",
                Name = input.Name,
                Amount = input.Amount
            };

            var operation = TableOperation.Replace(item);
            await outputTable.ExecuteAsync(operation);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName("DeleteEntry")]
        public static async Task<HttpResponseMessage> DeleteEntry(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] dynamic input, CloudTable outputTable,
            ILogger log, ClaimsPrincipal claimsPrincipal)
        {
            log.LogInformation($"C# Item updated {input.Name}.");

            var item = new Item
            {
                PartitionKey = input.PartitionKey,
                RowKey = $"{claimsPrincipal.Identity.Name}_{input.listName})"
            };

            var operation = TableOperation.Delete(item);
            await outputTable.ExecuteAsync(operation);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
