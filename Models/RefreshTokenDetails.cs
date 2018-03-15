using System;

namespace dotnetcore_webapi_and_ravendb.Models
{
    public class RefreshTokenDetails
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DateExpires { get; set; }

        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
