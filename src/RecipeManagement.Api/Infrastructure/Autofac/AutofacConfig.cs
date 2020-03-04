using Autofac;
using RecipeManagement.Infrastructure.Configurations;
using RecipeManagement.Infrastructure.Cryptography;
using RecipeManagement.Infrastructure.Database;
using RecipeManagement.Infrastructure.Services;

namespace RecipeManagement.Api.Infrastructure.Autofac
{
    public class AutofacConfig
    {
        public static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<RecipeManagementContext>().AsSelf();
            builder.RegisterType<AppSettingsAccessor>().AsImplementedInterfaces();
            builder.RegisterType<UserService>().AsImplementedInterfaces();
            builder.RegisterType<CryptographyService>().AsImplementedInterfaces();
        }
    }
}