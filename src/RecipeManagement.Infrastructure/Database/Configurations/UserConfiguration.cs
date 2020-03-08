using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeManagement.Domain;
using RecipeManagement.Infrastructure.Configurations;
using RecipeManagement.Infrastructure.Cryptography;

namespace RecipeManagement.Infrastructure.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly AppSettings _appSettings;

        public UserConfiguration(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToContainer("Users");
            builder.HasNoDiscriminator();

            var converter = new ValueConverter<string, string>(
                value =>CryptographyService.Encrypt(value, _appSettings.EncryptionKey),
                value =>CryptographyService.Decrypt(value, _appSettings.EncryptionKey));

            builder.Property(user => user.Name).HasConversion(converter);
            builder.Property(user => user.Surname).HasConversion(converter);
            builder.Property(user => user.CellphoneNumber).HasConversion(converter);
            builder.Property(user => user.EmailAddress).HasConversion(converter);
            builder.Property(user => user.Password).HasConversion(converter);
            builder.Property(user => user.PasswordSalt).HasConversion(converter);
        }
    }
}