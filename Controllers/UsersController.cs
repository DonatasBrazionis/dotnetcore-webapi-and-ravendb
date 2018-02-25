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
            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo([FromQuery]string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest($"{nameof(id)} field may not be null, empty, or consists only of white-space characters.");
            }

            if (!await RavenDBProvider.IsEntityExists(id))
            {
                return NotFound($"The specified '{id}' entity not exists.");
            }

            var user = await RavenDBProvider.GetEntity<User>(id);
            return Ok(user);
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
    }
}
