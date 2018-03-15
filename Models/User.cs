using System;

namespace dotnetcore_webapi_and_ravendb.Models
{
    public class User
    {
        public string Id { get; set; }
        public UserType Type { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }

        // DateTime in UTC when ban ends, any 
        // time in the past is considered not banned.
        // If null - not banned.
        public DateTime? DateBanEndsUtc { get; set; }
        public string BanDescription { get; set; }
        public string BannedBy { get; set; }
    }
}
