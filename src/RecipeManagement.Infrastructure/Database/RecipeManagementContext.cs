using Microsoft.EntityFrameworkCore;
using RecipeManagement.Domain;
using RecipeManagement.Infrastructure.Configurations;
using RecipeManagement.Infrastructure.Database.Configurations;

namespace RecipeManagement.Infrastructure.Database
{
    public class RecipeManagementContext : DbContext
    {
        private readonly CosmosDbSettings _cosmosDbSettings;

        #region Consructors

        public RecipeManagementContext(DbContextOptions options)
            : base(options)
        {
        }

        public RecipeManagementContext(IAppSettingsAccessor appSettingsAccessor)
        {
            _cosmosDbSettings = appSettingsAccessor.AppSettings.CosmosDbSettings;
        }

        #endregion

        #region DbContext Overrides

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseCosmos(
                    _cosmosDbSettings.AccountEndpoint,
                    _cosmosDbSettings.AccountKey,
                    databaseName: _cosmosDbSettings.DatabaseName);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        #endregion

        #region Entities

        public DbSet<User> Users { get; set; }

        #endregion
    }
}