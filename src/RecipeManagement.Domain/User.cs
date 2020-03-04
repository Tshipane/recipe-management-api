using System;
using Newtonsoft.Json;

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
        [JsonIgnore] public string PasswordSalt { get; set; }
        [JsonIgnore] public string Password { get; set; }
        [JsonIgnore] public DateTime DateCreated { get; set; }
        [JsonIgnore] public DateTime DateUpdated { get; set; }
    }
}