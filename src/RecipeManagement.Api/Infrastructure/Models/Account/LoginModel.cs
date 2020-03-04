using System.ComponentModel.DataAnnotations;

namespace RecipeManagement.Api.Infrastructure.Models.Account
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}