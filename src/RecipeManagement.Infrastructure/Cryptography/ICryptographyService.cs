namespace RecipeManagement.Infrastructure.Cryptography
{
    public interface ICryptographyService
    {
        string CreateSalt();
        string CreatePasswordHash(string password, string salt);
    }
}