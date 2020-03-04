using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecipeManagement.Api.Infrastructure.Services.Jwt
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string emailAddress, ClaimsIdentity identity);

        ClaimsIdentity GenerateClaimsIdentity(Guid userId, string email);
    }
}