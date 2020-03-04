using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;
using RecipeManagement.Infrastructure.Database;

namespace RecipeManagement.Tests.Extensions
{
    public static class DbContextExtensions
    {
        public static void PrepareTestData(this RecipeManagementContext recipeManagementContext, Action<RecipeManagementContext> action)
        {
            Assert.That(recipeManagementContext.IsDirty, Is.EqualTo(false));
            action.Invoke(recipeManagementContext);
            Assert.That(recipeManagementContext.IsDirty, Is.EqualTo(true));
            recipeManagementContext.SaveChanges();
            Assert.That(recipeManagementContext.IsDirty, Is.EqualTo(false));
        }

        public static void VerifySave(this RecipeManagementContext recipeManagementContext)
        {
            Assert.That(recipeManagementContext.IsDirty(), Is.EqualTo(false));
        }

        private static bool IsDirty(this DbContext context)
        {
            if (context == null) return false;
            // Query the change tracker entries for any adds, modifications, or deletes.
            IEnumerable<EntityEntry> res = from e in context.ChangeTracker.Entries()
                where e.State.HasFlag(EntityState.Added) ||
                      e.State.HasFlag(EntityState.Modified) ||
                      e.State.HasFlag(EntityState.Deleted)
                select e;

            return res.Any();
        }
    }
}