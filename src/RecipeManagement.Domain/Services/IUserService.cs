using System;

namespace RecipeManagement.Domain.Services
{
    public interface IUserService
    {
        User AddUser(string name, string surname, string emailAddress, string cellphoneNumber, string password);
        void UpdateUser(Guid userId, string name, string surname, string emailAddress, string cellphoneNumber);
    }
}