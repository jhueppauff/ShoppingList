using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace ShoppingList.Data
{
    public class ShoppingListService
    {
        private readonly IConfiguration configuration;

        public ShoppingListService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<Shared.Model.ShoppingList>> GetShoppingListsAsync(string owner)
        {
            Shared.Model.ShoppingList shoppingList = new Shared.Model.ShoppingList()
            {
                PartitionKey = owner
            };

            RestClient client = new RestClient(configuration.GetSection("FunctionHost").Value);
            RestRequest request = new RestRequest("/GetShoppingLists", Method.POST);
            var cancellationTokenSource = new CancellationTokenSource();

            string body = JsonConvert.SerializeObject(shoppingList);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddHeader("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var result = await client.ExecuteTaskAsync<List<Shared.Model.ShoppingList>>(request, cancellationTokenSource.Token, Method.POST).ConfigureAwait(false);

                if (!result.IsSuccessful)
                {
                    // log.LogError($"Rest Request wasn't successful: {result.ErrorMessage}");
                    return new List<Shared.Model.ShoppingList>()
                    {
                        new Shared.Model.ShoppingList()
                        {
                            PartitionKey = "Error"
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

        public async Task CreateShoppingListsAsync(string owner, string name)
        {
            Shared.Model.ShoppingList shoppingList = new Shared.Model.ShoppingList()
            {
                PartitionKey = owner,
                RowKey = name
            };

            RestClient client = new RestClient(configuration.GetSection("FunctionHost").Value);
            RestRequest request = new RestRequest("/CreateShoppingList", Method.POST);
            var cancellationTokenSource = new CancellationTokenSource();

            string body = JsonConvert.SerializeObject(shoppingList);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddHeader("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var result = await client.ExecuteTaskAsync<List<Shared.Model.ShoppingList>>(request, cancellationTokenSource.Token, Method.POST).ConfigureAwait(false);

                if (!result.IsSuccessful)
                {
                    throw new Exception($"Error while creating the List : {result.StatusCode} - {result.ErrorMessage}");
                }
            }
            catch (Exception error)
            {
                // log.LogError(error.Message);
                // log.LogDebug(error.StackTrace);
                throw;
            }
        }

        public async Task DeleteListAsync(string name, string owner)
        {
            Shared.Model.ShoppingList shoppingList = new Shared.Model.ShoppingList()
            {
                PartitionKey = owner,
                RowKey = name
            };

            RestClient client = new RestClient(configuration.GetSection("FunctionHost").Value);
#if (DEBUG)
            RestRequest request = new RestRequest("/DeleteShoppingList", Method.POST);
#else
            RestRequest request = new RestRequest("/v1/DeleteShoppingList", Method.POST);
#endif

            var cancellationTokenSource = new CancellationTokenSource();

            string body = JsonConvert.SerializeObject(shoppingList);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddHeader("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var result = await client.ExecuteTaskAsync<List<Shared.Model.ShoppingList>>(request, cancellationTokenSource.Token, Method.POST).ConfigureAwait(false);

                if (!result.IsSuccessful)
                {
                    throw new Exception($"Error while deleting the List : {result.StatusCode} - {result.ErrorMessage}");
                }
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
