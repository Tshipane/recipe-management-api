using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecipeManagement.Infrastructure.Database;

namespace RecipeManagement.Api.Infrastructure.Extensions
{
    public static class MigrationManager
    {
        public static IHost EnsureDatabaseCreated(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using RecipeManagementContext dustBustersContext = scope.ServiceProvider.GetRequiredService<RecipeManagementContext>();
                try
                {
                    dustBustersContext.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            return host;
        }
    }
}