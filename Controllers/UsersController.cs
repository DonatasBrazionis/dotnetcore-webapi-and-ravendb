using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Contracts;
using dotnetcore_webapi_and_ravendb.Models;
using dotnetcore_webapi_and_ravendb.Models.Dtos;
using dotnetcore_webapi_and_ravendb.Providers;
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
        public async Task<IActionResult> GetList()
        {
            var users = await RavenDatabaseProvider.GetEntities<User>();
            return Ok(users.Select(x => x.Id));
        }

        [HttpPost]
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newUserEntity = new User
            {
                Name = dto.Name,
                Age = dto.Age
            };

            await RavenDatabaseProvider.CreateEntity(newUserEntity);

            return CreatedAtAction(nameof(Create), newUserEntity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery]string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest($"{nameof(id)} field may not be null, empty, or consists only of white-space characters.");
            }

            if (!await RavenDatabaseProvider.IsEntityExists(id))
            {
                return NotFound($"The specified '{id}' entity not exists.");
            }

            await RavenDatabaseProvider.DeleteEntity(id);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromQuery]string id, [FromBody]UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest($"{nameof(id)} field may not be null, empty, or consists only of white-space characters.");
            }

            if (!await RavenDatabaseProvider.IsEntityExists(id))
            {
                return NotFound($"The specified '{id}' entity not exists.");
            }

            var user = await RavenDatabaseProvider.GetEntity<User>(id);
            if (user == null)
            {
                return NotFound($"The specified '{id}' entity not exists.");
            }

            user.Name = dto.Name;
            user.Age = dto.Age;

            await RavenDatabaseProvider.UpdateEntity(id, user);

            return Ok();
        }
    }
}
