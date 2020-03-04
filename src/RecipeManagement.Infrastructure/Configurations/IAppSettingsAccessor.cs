namespace RecipeManagement.Infrastructure.Configurations
{
    public interface IAppSettingsAccessor
    {
        AppSettings AppSettings { get; }
    }
}