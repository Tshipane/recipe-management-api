using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManagement.Api.Infrastructure.Extensions;
using RecipeManagement.Api.Infrastructure.Models.Errors;
using RecipeManagement.Api.Infrastructure.Models.Jwt;
using RecipeManagement.Infrastructure.Exceptions;

namespace RecipeManagement.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeManagementRestApiController : Controller
    {
        protected Guid UserId
        {
            get
            {
                if (HttpContext == null)
                {
                    return Guid.Empty;
                }

                return HttpContext.User.Identity.IsAuthenticated
                    ? new Guid(HttpContext.User.Claims.First(c => c.Type == JwtClaimIdentifiers.Id).Value)
                    : Guid.Empty;
            }
        }

        protected async Task<IActionResult> GetResourceList<T>(Func<List<T>> func)
        {
            try
            {
                List<T> entities = await Task.Run(func.Invoke);
                return Ok(entities);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        protected async Task<IActionResult> GetResource<T>(Func<T> func, Action<T> action = null)
        {
            try
            {
                T entity = await Task.Run(func.Invoke);
                if (entity == null)
                {
                    return new NotFoundObjectResult("Resource not found");
                }

                action?.Invoke(entity);
                return Ok(entity);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        protected async Task<IActionResult> CreateResource<T, TKey>(Func<T> func,
            Expression<Func<T, TKey>> keyExpression, string createdAtActionName = "GetById") where TKey : struct
        {
            T entity;
            if (ModelState.IsValid)
            {
                try
                {
                    entity = await Task.Run(func.Invoke);
                }
                catch (RecipeManagementException exception)
                {
                    return BadRequest(new BadRequestError(exception.Message));
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
            }
            else
            {
                return BadRequest(new BadRequestError(ModelState.GetErrors().FirstOrDefault()));
            }

            object propertyValue = null;
            var member = (MemberExpression) keyExpression.Body;
            var propInfo = (PropertyInfo) member.Member;
            PropertyInfo propertyInfo = entity.GetType().GetProperty(propInfo.Name);
            if (propertyInfo != null)
            {
                propertyValue = propertyInfo.GetValue(entity, null);
            }

            string url = Url.Action(createdAtActionName, new {id = propertyValue});
            return new CreatedResult(url, null);
        }

        protected async Task<IActionResult> UpdateResource(Action action)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await Task.Run(action.Invoke);
                }
                catch (RecipeManagementException exception)
                {
                    return BadRequest(new BadRequestError(exception.Message));
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
            }
            else
            {
                return BadRequest(new BadRequestError(ModelState.GetErrors().FirstOrDefault()));
            }

            return new NoContentResult();
        }

        protected async Task<IActionResult> DeleteResource(Action action)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await Task.Run(action.Invoke);
                }
                catch (RecipeManagementException exception)
                {
                    return BadRequest(new BadRequestError(exception.Message));
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
            }
            else
            {
                return BadRequest(new BadRequestError(ModelState.GetErrors().FirstOrDefault()));
            }

            return StatusCode(202);
        }
    }
}