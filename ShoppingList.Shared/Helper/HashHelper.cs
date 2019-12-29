using System;
using System.Text;

namespace ShoppingList.Shared.Helper
{
    public static class HashHelper
    {
        public static string ConvertToHash(string input)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
