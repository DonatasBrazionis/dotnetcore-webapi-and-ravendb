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


    }
}
