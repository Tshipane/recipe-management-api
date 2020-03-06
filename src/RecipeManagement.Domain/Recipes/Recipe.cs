using System;
using System.Collections.Generic;

namespace RecipeManagement.Domain.Recipes
{
    public class Recipe
    {
        public Guid RecipeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<RecipeStep> Steps { get; set; }
        public string Notes { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}