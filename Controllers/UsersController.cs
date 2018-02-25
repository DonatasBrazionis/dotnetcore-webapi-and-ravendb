using System.Threading.Tasks;
using dotnetcore_webapi_and_ravendb.Models;
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
    }
}
