using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecipeManagement.Api.Infrastructure.Models.Recipes;
using RecipeManagement.Domain.Services;

namespace RecipeManagement.Api.Controllers
{
    public class RecipesController : RecipeManagementRestApiController
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipes()
        {
            return await GetResourceList(() => _recipeService.GetUserRecipes(UserId));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return await GetResource(() => _recipeService.GetRecipeById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] RecipeModel recipeModel)
        {
            return await CreateResource(() => _recipeService.AddRecipe(recipeModel.Title, recipeModel.Description,
                recipeModel.Steps, recipeModel.Notes, UserId), r => r.RecipeId);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRecipe([FromBody] RecipeModel recipeModel, Guid id)
        {
            return await UpdateResource(() => _recipeService.UpdateRecipe(id, recipeModel.Title,
                recipeModel.Description, recipeModel.Steps, recipeModel.Notes, UserId));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRecipe(Guid id)
        {
            return await DeleteResource(() => _recipeService.DeleteRecipe(id, UserId));
        }
    }
}