using System.ComponentModel.DataAnnotations;

namespace RecipeManagement.Api.Infrastructure.Models.Account
{
    public class ChangePasswordModel
    {
        [Required] public string CurrentPassword { get; set; }
        [Required] public string NewPassword { get; set; }
    }
}