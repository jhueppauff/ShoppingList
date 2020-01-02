using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingList.Data
{
    public class ConfigurationService
    {
        private readonly IConfiguration configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string AuthenticationMode ()
        {
            return configuration.GetSection("AuthenticationMode").Value;
        }
    }
}
