using System;

namespace RecipeManagement.Infrastructure.Exceptions
{
    public class RecipeManagementException : Exception
    {
        public RecipeManagementException(string message) : base(message)
        {
        }
    }
}