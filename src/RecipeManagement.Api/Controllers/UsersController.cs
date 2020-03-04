using Microsoft.AspNetCore.Mvc;
using RecipeManagement.Domain.Services;

namespace RecipeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
    }
}