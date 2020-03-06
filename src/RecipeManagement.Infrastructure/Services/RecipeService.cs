using System;
using System.Collections.Generic;
using System.Linq;
using RecipeManagement.Domain.Recipes;
using RecipeManagement.Domain.Services;
using RecipeManagement.Infrastructure.Database;

namespace RecipeManagement.Infrastructure.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeManagementContext _recipeManagementContext;

        public RecipeService(RecipeManagementContext recipeManagementContext)
        {
            _recipeManagementContext = recipeManagementContext;
        }

        public Recipe AddRecipe(string title, string description, List<RecipeStep> steps, string notes, Guid userId)
        {
            DateTime now = DateTime.Now;

            var recipe = new Recipe
            {
                RecipeId = Guid.NewGuid(),
                Title = title,
                Description = description,
                Steps = OrderSteps(steps),
                Notes = notes,
                CreatedById = userId,
                DateCreated = now,
                DateUpdated = now
            };

            _recipeManagementContext.Recipes.Add(recipe);

            _recipeManagementContext.SaveChanges();

            return recipe;
        }

        private static ICollection<RecipeStep> OrderSteps(ICollection<RecipeStep> steps)
        {
            int orderNumber = 1;
            foreach (RecipeStep recipeStep in steps)
            {
                recipeStep.StepNumber = orderNumber++;
            }

            return steps;
        }

        public void UpdateRecipe(Guid recipeId, string title, string description, List<RecipeStep> steps, string notes,
            Guid userId)
        {
            Recipe recipe = _recipeManagementContext.Recipes.Find(recipeId);
            if (recipe.CreatedById != userId)
            {
                throw new UnauthorizedAccessException();
            }

            recipe.Title = title;
            recipe.Description = description;
            recipe.Steps = OrderSteps(steps);
            recipe.Notes = notes;
            recipe.DateUpdated = DateTime.Now;

            _recipeManagementContext.SaveChanges();
        }

        public void DeleteRecipe(Guid recipeId, Guid userId)
        {
            Recipe recipe = _recipeManagementContext.Recipes.Find(recipeId);
            if (recipe.CreatedById != userId)
            {
                throw new UnauthorizedAccessException();
            }

            _recipeManagementContext.Recipes.Remove(recipe);

            _recipeManagementContext.SaveChanges();
        }

        public List<Recipe> GetUserRecipes(Guid userId)
        {
            return _recipeManagementContext.Recipes.Where(recipe => recipe.CreatedById == userId).ToList();
        }

        public Recipe GetRecipeById(Guid recipeId)
        {
            return _recipeManagementContext.Recipes.Find(recipeId);
        }
    }
}