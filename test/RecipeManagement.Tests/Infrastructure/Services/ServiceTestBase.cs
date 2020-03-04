using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RecipeManagement.Infrastructure.Database;

namespace RecipeManagement.Tests.Infrastructure.Services
{
    public class ServiceTestBase
    {
        protected RecipeManagementContext RecipeManagementContext;

        [SetUp]
        public void SetupBase()
        {
            DbContextOptionsBuilder<RecipeManagementContext> builder =
                new DbContextOptionsBuilder<RecipeManagementContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            RecipeManagementContext = new RecipeManagementContext(builder.Options);
        }
    }
}