using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingList.Data
{
    public class ShoppingListItemService
    {
        private readonly IConfiguration configuration;

        public ShoppingListItemService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task CreateShoppingListItemAsync(string owner, string listName, string itemName, double amout, string unit)
        {
            ShoppingItemRequest shoppingItemRequest = new ShoppingItemRequest()
            {
                Owner = owner,
                ListName = listName,
                Amount = amout,
                Name = itemName,
                Unit = unit
            };

            RestClient client = new RestClient(configuration.GetSection("FunctionHost").Value);
            RestRequest request = new RestRequest("/CreateShoppingListItem", Method.POST);
            var cancellationTokenSource = new CancellationTokenSource();

            string body = JsonConvert.SerializeObject(shoppingItemRequest);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddHeader("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var result = await client.ExecuteTaskAsync<List<Shared.Model.ShoppingListItem>>(request, cancellationTokenSource.Token, Method.POST).ConfigureAwait(false);

                if (!result.IsSuccessful)
                {
                    throw new Exception($"Error while creating the Item - {result.StatusCode} : {result.ErrorMessage}");
                }
            }
            catch (Exception error)
            {
                // log.LogError(error.Message);
                // log.LogDebug(error.StackTrace);
                throw;
            }
        }

        public async Task<List<Shared.Model.ShoppingListItem>> GetShoppingListItemsAsync(string owner, string listName)
        {
            ShoppingItemRequest shoppingItemRequest = new ShoppingItemRequest()
            {
                Owner = owner,
                ListName = listName
            };

            RestClient client = new RestClient(configuration.GetSection("FunctionHost").Value);
            RestRequest request = new RestRequest("/GetShoppingListItems", Method.POST);
            var cancellationTokenSource = new CancellationTokenSource();

            string body = JsonConvert.SerializeObject(shoppingItemRequest);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddHeader("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var result = await client.ExecuteTaskAsync<List<Shared.Model.ShoppingListItem>>(request, cancellationTokenSource.Token, Method.POST).ConfigureAwait(false);

                if (!result.IsSuccessful)
                {
                    // log.LogError($"Rest Request wasn't successful: {result.ErrorMessage}");
                    return new List<Shared.Model.ShoppingListItem>()
                    {
                        new Shared.Model.ShoppingListItem()
                        {
                            PartitionKey = "Error",
                            RowKey = "1"
                        }
                    };
                }

                return result.Data;
            }
            catch (Exception error)
            {
                // log.LogError(error.Message);
                // log.LogDebug(error.StackTrace);
                throw;
            }
        }

        public async Task<List<Shared.Model.ShoppingListItem>> CompleteItemAsync(string owner, string listName, string itemId, bool completed)
        {
            ShoppingItemRequest shoppingItemRequest = new ShoppingItemRequest()
            {
                Owner = owner,
                ListName = listName,
                Id = itemId,
                Completed = completed
            };

            RestClient client = new RestClient(configuration.GetSection("FunctionHost").Value);

#if (DEBUG)
            RestRequest request = new RestRequest("/CompleteListItem", Method.POST);
#else
            RestRequest request = new RestRequest("/v1/CompleteListItem", Method.POST);
#endif
            var cancellationTokenSource = new CancellationTokenSource();
            string body = JsonConvert.SerializeObject(shoppingItemRequest);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddHeader("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var result = await client.ExecuteTaskAsync<List<Shared.Model.ShoppingListItem>>(request, cancellationTokenSource.Token, Method.POST).ConfigureAwait(false);

                if (!result.IsSuccessful)
                {
                    // log.LogError($"Rest Request wasn't successful: {result.ErrorMessage}");
                    return new List<Shared.Model.ShoppingListItem>()
                    {
                        new Shared.Model.ShoppingListItem()
                        {
                            PartitionKey = "Error",
                            RowKey = "1"
                        }
                    };
                }

                return result.Data;
            }
            catch (Exception error)
            {
                // log.LogError(error.Message);
                // log.LogDebug(error.StackTrace);
                throw;
            }
        }

        public async Task<List<Shared.Model.ShoppingListItem>> DeleteItemAsync(string owner, string listName, string itemId)
        {
            ShoppingItemRequest shoppingItemRequest = new ShoppingItemRequest()
            {
                Owner = owner,
                ListName = listName,
                Id = itemId,
            };

            RestClient client = new RestClient(configuration.GetSection("FunctionHost").Value);

#if (DEBUG)
            RestRequest request = new RestRequest("/DeleteShoppingListItem", Method.POST);
#else
            RestRequest request = new RestRequest("/v1/DeleteShoppingListItem", Method.POST);
#endif
            var cancellationTokenSource = new CancellationTokenSource();
            string body = JsonConvert.SerializeObject(shoppingItemRequest);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddHeader("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var result = await client.ExecuteTaskAsync<List<Shared.Model.ShoppingListItem>>(request, cancellationTokenSource.Token, Method.POST).ConfigureAwait(false);

                if (!result.IsSuccessful)
                {
                    // log.LogError($"Rest Request wasn't successful: {result.ErrorMessage}");
                    return new List<Shared.Model.ShoppingListItem>()
                    {
                        new Shared.Model.ShoppingListItem()
                        {
                            PartitionKey = "Error",
                            RowKey = "1"
                        }
                    };
                }

                return result.Data;
            }
            catch (Exception error)
            {
                // log.LogError(error.Message);
                // log.LogDebug(error.StackTrace);
                throw;
            }
        }
    }
}
