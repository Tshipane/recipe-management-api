using System.Collections.Generic;
using RecipeManagement.Domain.Recipes;

namespace RecipeManagement.Api.Infrastructure.Models.Recipes
{
    public class RecipeModel
    {
        public RecipeModel(string title, string description, List<RecipeStep> steps, string notes)
        {
            Title = title;
            Description = description;
            Steps = steps;
            Notes = notes;
        }

        public string Title { get;}
        public string Description { get; }
        public List<RecipeStep> Steps { get; }
        public string Notes { get; }
    }
}