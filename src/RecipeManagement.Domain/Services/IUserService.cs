using System;

namespace RecipeManagement.Domain.Services
{
    public interface IUserService
    {
        User AddUser(string name, string surname, string emailAddress, string cellphoneNumber, string password);
        void UpdateUser(Guid userId, string name, string surname, string emailAddress, string cellphoneNumber);
        User GetUserByEmailAddress(string emailAddress);
        bool AuthenticateUser(string emailAddress, string password);
        User GetUserById(Guid userId);
        void ChangePassword(Guid userId, string password);
    }
}