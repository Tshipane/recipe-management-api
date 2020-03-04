using Microsoft.Extensions.Options;

namespace RecipeManagement.Infrastructure.Configurations
{
    public class AppSettingsAccessor : IAppSettingsAccessor
    {
        public AppSettingsAccessor(IOptions<AppSettings> optionsAccessor)
        {
            AppSettings = optionsAccessor.Value;
        }

        public AppSettings AppSettings { get; }
    }
}