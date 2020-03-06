using System;
using System.Collections.Generic;
using RecipeManagement.Domain.Recipes;

namespace RecipeManagement.Domain.Services
{
    public interface IRecipeService
    {
        Recipe AddRecipe(string title, string description, List<RecipeStep> steps, string notes, Guid userId);
        void UpdateRecipe(Guid recipeId, string title, string description, List<RecipeStep> steps, string notes,
            Guid userId);
        void DeleteRecipe(Guid recipeId, Guid userId);
        List<Recipe> GetUserRecipes(Guid userId);
        Recipe GetRecipeById(Guid recipeId);
    }
}