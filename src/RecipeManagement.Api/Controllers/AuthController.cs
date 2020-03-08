using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using RecipeManagement.Api.Infrastructure.Extensions;
using RecipeManagement.Api.Infrastructure.Models.Account;
using RecipeManagement.Api.Infrastructure.Models.Errors;
using RecipeManagement.Api.Infrastructure.Models.Jwt;
using RecipeManagement.Api.Infrastructure.Services.Jwt;
using RecipeManagement.Domain;
using RecipeManagement.Domain.Services;

namespace RecipeManagement.Api.Controllers
{
    [AllowAnonymous]
    public class AuthController : RecipeManagementRestApiController
    {
        private readonly IUserService _userService;
        private readonly IJwtFactory _jwtFactory;
        private readonly JsonSerializerSettings _serializerSettings;

        public AuthController(IUserService userService, IJwtFactory jwtFactory)
        {
            _userService = userService;
            _jwtFactory = jwtFactory;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BadRequestError(ModelState.GetErrors().FirstOrDefault()));
            }

            (ClaimsIdentity claimsIdentity, User user) = await GetClaimsIdentity(loginModel);
            if (claimsIdentity == null)
            {
                return BadRequest(new ModelError("Login failed. Invalid email address or password."));
            }

            // Serialize and return the response
            var response = new
            {
                auth_token = await _jwtFactory.GenerateEncodedToken(loginModel.EmailAddress, claimsIdentity),
                expires_in = (int) JwtIssuerOptions.ValidFor.TotalSeconds,
                user_id = user.UserId
            };

            string json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordViewModel)
        {
            User user = _userService.GetUserById(UserId);
            if (await Task.Run(() => !_userService.AuthenticateUser(user.EmailAddress, changePasswordViewModel.CurrentPassword)))
            {
                return BadRequest(new BadRequestError("Your current password could not be validated"));
            }

            return await UpdateResource(() => _userService.ChangePassword(UserId, changePasswordViewModel.NewPassword));
        }
        
        [HttpGet("isAuthenticated")]
        public async Task<bool> Authenticated()
        {
            return await Task.Run(() => User.Identity.IsAuthenticated);
        }
        
        #region Private Methods

        private async Task<Tuple<ClaimsIdentity, User>> GetClaimsIdentity(LoginModel loginModel)
        {
            if (!string.IsNullOrEmpty(loginModel.EmailAddress) && !string.IsNullOrEmpty(loginModel.Password))
            {
                // get the user to verify
                User user = await Task.Run(() => _userService.GetUserByEmailAddress(loginModel.EmailAddress));

                if (user != null)
                {
                    // check the credentials  
                    if (await Task.Run(() => _userService.AuthenticateUser(loginModel.EmailAddress, loginModel.Password)))
                    {
                        ClaimsIdentity claimsIdentity = _jwtFactory.GenerateClaimsIdentity(user.UserId, user.EmailAddress);

                        return Tuple.Create(claimsIdentity, user);
                    }
                }
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.Run(() => Tuple.Create<ClaimsIdentity, User>(null, null));
        }

        #endregion
    }
}