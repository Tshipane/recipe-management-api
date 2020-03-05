using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagement.Api.Infrastructure.Models.Users;
using RecipeManagement.Domain.Services;

namespace RecipeManagement.Api.Controllers
{
    public class UsersController : RecipeManagementRestApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return await GetResource(() => _userService.GetUserById(id));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserModel userModel)
        {
            return await CreateResource(() => _userService.AddUser(userModel.Name, userModel.Surname,
                userModel.EmailAddress, userModel.CellphoneNumber, userModel.Password), u => u.UserId);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userModel, Guid id)
        {
            return await UpdateResource(() => _userService.UpdateUser(id, userModel.Name, userModel.Surname,
                userModel.EmailAddress, userModel.CellphoneNumber));
        }
        
        [HttpGet("me")]
        public async Task<IActionResult> LoggedInUser()
        {
            return await GetResource(() => _userService.GetUserById(UserId));
        }
    }
}