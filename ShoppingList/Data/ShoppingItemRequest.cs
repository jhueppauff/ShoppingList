using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingList.Data
{
    public class ShoppingItemRequest
    {
        public string Name { get; set; }

        public double Amount { get; set; }

        public string Owner { get; set; }

        public string ListName { get; set; }
    }
}
