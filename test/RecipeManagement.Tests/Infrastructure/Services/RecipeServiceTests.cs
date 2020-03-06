using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RecipeManagement.Domain.Recipes;
using RecipeManagement.Infrastructure.Services;
using RecipeManagement.Tests.Extensions;

namespace RecipeManagement.Tests.Infrastructure.Services
{
    [TestFixture]
    public class RecipeServiceTests : ServiceTestBase
    {
        private RecipeService _recipeService;

        [SetUp]
        public void Setup()
        {
            _recipeService = new RecipeService(RecipeManagementContext);
        }

        [Test]
        public void AddRecipeTest()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var recipeSteps = new List<RecipeStep>
            {
                new RecipeStep
                {
                    Description = "Step 1"
                },
                new RecipeStep
                {
                    Description = "Step 2"
                }
            };

            //Act
            Recipe recipe = _recipeService.AddRecipe("Title", "Description", recipeSteps, "Notes", userId);

            //Assert
            Assert.That(RecipeManagementContext.Recipes.Count(), Is.EqualTo(1));
            Assert.That(recipe.Title, Is.EqualTo("Title"));
            Assert.That(recipe.Description, Is.EqualTo("Description"));
            Assert.That(recipe.Notes, Is.EqualTo("Notes"));
            Assert.That(recipe.Steps.Count, Is.EqualTo(2));
            Assert.That(recipe.Steps.First().StepNumber, Is.EqualTo(1));
            Assert.That(recipe.Steps.Last().StepNumber, Is.EqualTo(2));
            Assert.That(recipe.CreatedById, Is.EqualTo(userId));
            Assert.That(recipe.DateCreated, Is.EqualTo(DateTime.Now).Within(1).Minutes);
            Assert.That(recipe.DateUpdated, Is.EqualTo(DateTime.Now).Within(1).Minutes);

            RecipeManagementContext.VerifySave();
        }

        [Test]
        public void UpdateRecipeTest()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipeSteps = new List<RecipeStep>
            {
                new RecipeStep
                {
                    Description = "Step 1"
                },
                new RecipeStep
                {
                    Description = "Step 2"
                },
                new RecipeStep
                {
                    Description = "Step 3"
                }
            };

            var recipe = new Recipe
            {
                RecipeId = recipeId,
                Steps = new List<RecipeStep>
                {
                    new RecipeStep
                    {
                        Description = "Step 1"
                    },
                    new RecipeStep
                    {
                        Description = "Step 2"
                    },
                },
                DateCreated = DateTime.Now.AddDays(-1),
                CreatedById = userId
            };
            RecipeManagementContext.PrepareTestData(context => { context.Recipes.Add(recipe); });

            //Act
            _recipeService.UpdateRecipe(recipeId, "Title", "Description", recipeSteps, "Notes", userId);

            //Assert
            Assert.That(recipe.Title, Is.EqualTo("Title"));
            Assert.That(recipe.Description, Is.EqualTo("Description"));
            Assert.That(recipe.Notes, Is.EqualTo("Notes"));
            Assert.That(recipe.Steps.Count, Is.EqualTo(3));
            Assert.That(recipe.Steps.First().StepNumber, Is.EqualTo(1));
            Assert.That(recipe.Steps.Last().StepNumber, Is.EqualTo(3));
            Assert.That(recipe.CreatedById, Is.EqualTo(userId));
            Assert.That(recipe.DateCreated, Is.EqualTo(DateTime.Now.AddDays(-1)).Within(1).Minutes);
            Assert.That(recipe.DateUpdated, Is.EqualTo(DateTime.Now).Within(1).Minutes);

            RecipeManagementContext.VerifySave();
        }

        [Test]
        public void
            UpdateRecipeTest_Given_That_Recipe_Was_Created_By_Some_One_Then_UnAuthorized_Exception_Should_Be_Thrown()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var recipeSteps = new List<RecipeStep>
            {
                new RecipeStep
                {
                    Description = "Step 1"
                },
                new RecipeStep
                {
                    Description = "Step 2"
                },
                new RecipeStep
                {
                    Description = "Step 3"
                }
            };

            var recipe = new Recipe
            {
                RecipeId = recipeId,
                Steps = new List<RecipeStep>
                {
                    new RecipeStep
                    {
                        Description = "Step 1"
                    },
                    new RecipeStep
                    {
                        Description = "Step 2"
                    },
                },
                DateCreated = DateTime.Now.AddDays(-1),
                CreatedById = Guid.NewGuid()
            };
            RecipeManagementContext.PrepareTestData(context => { context.Recipes.Add(recipe); });

            //Act
            var unauthorizedAccessException = Assert.Throws<UnauthorizedAccessException>(() =>
                _recipeService.UpdateRecipe(recipeId, "Title", "Description", recipeSteps, "Notes", userId));

            //Assert
            Assert.IsNotNull(unauthorizedAccessException);
        }

        [Test]
        public void DeleteRecipeTest()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var recipe = new Recipe
            {
                RecipeId = recipeId,
                Steps = new List<RecipeStep>
                {
                    new RecipeStep
                    {
                        Description = "Step 1"
                    },
                    new RecipeStep
                    {
                        Description = "Step 2"
                    },
                },
                DateCreated = DateTime.Now.AddDays(-1),
                CreatedById = userId
            };
            RecipeManagementContext.PrepareTestData(context => { context.Recipes.Add(recipe); });

            //Act
            _recipeService.DeleteRecipe(recipeId, userId);

            //Assert
            Assert.That(RecipeManagementContext.Recipes.Count(), Is.EqualTo(0));

            RecipeManagementContext.VerifySave();
        }

        [Test]
        public void
            DeleteRecipeTest_Given_That_Recipe_Was_Created_By_Some_One_Then_UnAuthorized_Exception_Should_Be_Thrown()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();

            var recipe = new Recipe
            {
                RecipeId = recipeId,
                Steps = new List<RecipeStep>
                {
                    new RecipeStep
                    {
                        Description = "Step 1"
                    },
                    new RecipeStep
                    {
                        Description = "Step 2"
                    },
                },
                DateCreated = DateTime.Now.AddDays(-1),
                CreatedById = Guid.NewGuid()
            };
            RecipeManagementContext.PrepareTestData(context => { context.Recipes.Add(recipe); });

            //Act
            var unauthorizedAccessException = Assert.Throws<UnauthorizedAccessException>(() =>
                _recipeService.DeleteRecipe(recipeId, userId));

            //Assert
            Assert.IsNotNull(unauthorizedAccessException);
        }
    }
}