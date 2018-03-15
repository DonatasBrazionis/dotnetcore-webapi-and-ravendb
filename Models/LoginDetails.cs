using System;

namespace dotnetcore_webapi_and_ravendb.Models
{
    public class LoginDetails
    {
        // Cusstom Id. e.g. "login/john@email.com"
        public string Id { get; set; }

        public string UserId { get; set; }
        public string UniqueId { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }

        // Is lockout enabled for this user
        // DateTime in UTC when lockout ends, any 
        // time in the past is considered not locked out.
        // If null - not locked out.
        public DateTime? DateLockoutEndsUtc { get; set; }
        public int AccessFailedCount { get; set; }
    }
}
