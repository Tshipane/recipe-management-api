﻿using System;
using System.Linq;
using RecipeManagement.Domain;
using RecipeManagement.Domain.Services;
using RecipeManagement.Infrastructure.Cryptography;
using RecipeManagement.Infrastructure.Database;
using RecipeManagement.Infrastructure.Exceptions;

namespace RecipeManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly RecipeManagementContext _recipeManagementContext;
        private readonly ICryptographyService _cryptographyService;

        public UserService(RecipeManagementContext recipeManagementContext, ICryptographyService cryptographyService)
        {
            _recipeManagementContext = recipeManagementContext;
            _cryptographyService = cryptographyService;
        }

        public User AddUser(string name, string surname, string emailAddress, string cellphoneNumber, string password)
        {
            if (_recipeManagementContext.Users.AsEnumerable().Any(u => u.EmailAddress == emailAddress))
            {
                throw new RecipeManagementException("User with specified email address already exists");
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Name = name,
                Surname = surname,
                CellphoneNumber = cellphoneNumber,
                EmailAddress = emailAddress,
                PasswordSalt = _cryptographyService.CreateSalt()
            };
            user.Password = _cryptographyService.CreatePasswordHash(password, user.PasswordSalt);
            DateTime now = DateTime.Now;
            user.DateCreated = now;
            user.DateUpdated = now;

            _recipeManagementContext.Users.Add(user);

            _recipeManagementContext.SaveChanges();

            return user;
        }

        public void UpdateUser(Guid userId, string name, string surname, string emailAddress, string cellphoneNumber)
        {
            if (_recipeManagementContext.Users.AsEnumerable()
                .Any(u => u.EmailAddress == emailAddress && u.UserId != userId))
            {
                throw new RecipeManagementException("User with specified email address already exists");
            }

            User user = _recipeManagementContext.Users.Find(userId);
            user.Name = name;
            user.Surname = surname;
            user.CellphoneNumber = cellphoneNumber;
            user.EmailAddress = emailAddress;
            user.DateUpdated = DateTime.Now;

            _recipeManagementContext.SaveChanges();
        }

        public User GetUserByEmailAddress(string emailAddress)
        {
            return _recipeManagementContext.Users.AsEnumerable().FirstOrDefault(user => user.EmailAddress == emailAddress);
        }

        public bool AuthenticateUser(string emailAddress, string password)
        {
            User user = GetUserByEmailAddress(emailAddress);

            if (user == null) return false;

            string passwordHash = _cryptographyService.CreatePasswordHash(password, user.PasswordSalt);

            return passwordHash == user.Password;
        }

        public User GetUserById(Guid userId)
        {
            return _recipeManagementContext.Users.Find(userId);
        }

        public void ChangePassword(Guid userId, string password)
        {
            User user = _recipeManagementContext.Users.Find(userId);

            user.PasswordSalt = _cryptographyService.CreateSalt();
            user.Password = _cryptographyService.CreatePasswordHash(password, user.PasswordSalt);
            user.DateUpdated = DateTime.Now;

            _recipeManagementContext.SaveChanges();
        }
    }
}