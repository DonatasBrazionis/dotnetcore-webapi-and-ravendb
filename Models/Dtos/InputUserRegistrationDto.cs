using System;

namespace dotnetcore_webapi_and_ravendb.Models.Dtos
{
    public class InputUserRegistrationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
