using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Models;
using dotnetcore_webapi_and_ravendb.Models.Dtos;
using dotnetcore_webapi_and_ravendb.Providers;
using Microsoft.AspNetCore.Mvc;

namespace dotnetcore_webapi_and_ravendb.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {
        public UsersController(RavenDBProvider ravenDBProvider)
        {
            RavenDBProvider = ravenDBProvider;
        }
        protected RavenDBProvider RavenDBProvider { get; set; }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var users = await RavenDBProvider.GetEntities<User>();
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
                var user = await RavenDBProvider.GetEntity<User>(id);
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

            await RavenDBProvider.CreateEntity(newUserEntity);

            return CreatedAtAction(nameof(Create), newUserEntity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery]string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest($"{nameof(id)} field may not be null, empty, or consists only of white-space characters.");
            }

            if (!await RavenDBProvider.IsEntityExists(id))
            {
                return NotFound($"The specified '{id}' entity not exists.");
            }

            await RavenDBProvider.DeleteEntity(id);

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

            if (!await RavenDBProvider.IsEntityExists(id))
            {
                return NotFound($"The specified '{id}' entity not exists.");
            }

            var user = await RavenDBProvider.GetEntity<User>(id);
            if (user == null)
            {
                return NotFound($"The specified '{id}' entity not exists.");
            }

            user.Name = dto.Name;
            user.Age = dto.Age;

            await RavenDBProvider.UpdateEntity(id, user);

            return Ok();
        }
    }
}
