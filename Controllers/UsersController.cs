using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Contracts;
using dotnetcore_webapi_and_ravendb.Models;
using dotnetcore_webapi_and_ravendb.Models.Dtos;
using dotnetcore_webapi_and_ravendb.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetcore_webapi_and_ravendb.Controllers
{
    public class UsersController : Controller
    {
        public UsersController(IRavenDatabaseProvider ravenDatabaseProvider, ILoginProvider loginProvider)
        {
            RavenDatabaseProvider = ravenDatabaseProvider;
            LoginProvider = loginProvider;
        }
        protected IRavenDatabaseProvider RavenDatabaseProvider { get; set; }
        public ILoginProvider LoginProvider { get; set; }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]InputUserRegistrationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userLoginId = LoginProvider.GenerateId(dto.Email);
            if (await RavenDatabaseProvider.IsEntityExists(userLoginId))
            {
                return BadRequest($"{nameof(dto.Email)} already exists.");
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                CreatedBy = "System",
                DateCreated = DateTime.UtcNow
            };
            await RavenDatabaseProvider.CreateEntity(user);

            var loginDetails = new LoginDetails
            {
                Id = userLoginId,
                UniqueId = dto.Email,
                UserId = user.Id,
                CreatedBy = "System",
                DateCreated = DateTime.UtcNow
            };
            LoginProvider.SetPassword(loginDetails, dto.Password);
            await RavenDatabaseProvider.CreateEntity(loginDetails);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var identity = User.Identity;
            if (identity == null)
            {
                return Forbid();
            }
            var userId = identity.Name;

            var user = await RavenDatabaseProvider.GetEntity<User>(userId);
            if (user == null)
            {
                return Forbid();
            }
            var userDto = new OutputUserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Type = user.Type,
                Email = user.Email,
                DateCreated = user.DateCreated,
                DateModified = user.DateModified
            };

            return Ok(userDto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetList()
        {
            var users = await RavenDatabaseProvider.GetEntities<User>();
            return Ok(users.Select(x => x.Id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInfo([FromBody]InputUsersDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = new List<User>();
            foreach (var id in dto.Ids)
            {
                var user = await RavenDatabaseProvider.GetEntity<User>(id);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            var result = new OutputUsersDto
            {
                Users = users
            };

            return Ok(result);
        }

    }
}
