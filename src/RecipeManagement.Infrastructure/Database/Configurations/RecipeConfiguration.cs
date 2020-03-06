using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManagement.Domain.Recipes;

namespace RecipeManagement.Infrastructure.Database.Configurations
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.ToContainer("Recipes");
            builder.OwnsMany(recipe => recipe.Steps);
            builder.HasNoDiscriminator();
        }
    }
}