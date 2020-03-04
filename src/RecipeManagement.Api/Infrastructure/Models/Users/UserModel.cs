using System.ComponentModel.DataAnnotations;

namespace RecipeManagement.Api.Infrastructure.Models.Users
{
    public class UserModel
    {
        [Required] public string Name { get; set; }
        [Required] public string Surname { get; set; }
        [Required] public string EmailAddress { get; set; }
        [Required] public string CellphoneNumber { get; set; }
        public string Password { get; set; }
    }
}