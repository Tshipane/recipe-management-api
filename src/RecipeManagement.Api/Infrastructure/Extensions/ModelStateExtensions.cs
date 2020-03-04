using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RecipeManagement.Api.Infrastructure.Extensions
{
    public static class ModelStateExtensions
    {
        public static IEnumerable<string> GetErrors(this ModelStateDictionary modelState)
        {
            List<string> errors = new List<string>();
            foreach (ModelStateEntry modelStateEntry in modelState.Values)
            {
                foreach (ModelError error in modelStateEntry.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            return errors;
        }
    }
}