namespace RecipeManagement.Infrastructure.Configurations
{
    public class AppSettings
    {
        public CosmosDbSettings CosmosDbSettings { get; set; }
        public string EncryptionKey { get; set; }
    }
}