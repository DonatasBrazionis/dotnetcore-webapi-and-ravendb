using System;
using System.Collections.Generic;

namespace dotnetcore_webapi_and_ravendb.Models.Dtos
{
    public class OutputUserProfileDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserType Type { get; set; }
        public string Email { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
