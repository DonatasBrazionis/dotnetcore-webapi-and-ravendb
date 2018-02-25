using System;
using System.Collections.Generic;

namespace dotnetcore_webapi_and_ravendb.Models.Dtos
{
    public class OutputUsersDto
    {
        public IList<User> Users { get; set; }
    }
}
