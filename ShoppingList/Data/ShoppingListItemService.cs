﻿using Microsoft.Extensions.Configuration;
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

        public async Task CreateShoppingListItemAsync(string owner, string listName, string itemName, double amout)
        {
            ShoppingItemRequest shoppingItemRequest = new ShoppingItemRequest()
            {
                Owner = owner,
                ListName = listName,
                Amount = amout,
                Name = itemName
            };

            RestClient client = new RestClient(configuration.GetSection("FunctionHost").Value);
            RestRequest request = new RestRequest("/api/CreateShoppingListItem", Method.POST);
            var cancellationTokenSource = new CancellationTokenSource();

            string body = JsonConvert.SerializeObject(shoppingItemRequest);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddParameter("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
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
            RestRequest request = new RestRequest("/api/GetShoppingListItems", Method.POST);
            var cancellationTokenSource = new CancellationTokenSource();

            string body = JsonConvert.SerializeObject(shoppingItemRequest);

            request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
            request.AddParameter("Ocp-Apim-Subscription-Key", configuration.GetSection("APIKey").Value);
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
