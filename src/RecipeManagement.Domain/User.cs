using System;

namespace RecipeManagement.Domain
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string CellphoneNumber { get; set; }
        public string FullName => $"{Name} {Surname}";
        public string PasswordSalt { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}