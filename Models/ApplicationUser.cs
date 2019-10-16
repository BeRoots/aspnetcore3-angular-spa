using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public static readonly string DEFAULTPREFEREDSTYLE = "default";
        public string PreferedStyle { get; set; }
        public string Gender { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Society { get; set; }
        public string Fulladdress { get; set; }
        public string Street { get; set; }
        public UInt32 Zipcode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        //public string Email inherit from IdentityUser
        //public string PhoneNumber inherit from IdentityUser
        //public string Password inherit from IdentityUser
        public UInt32 ConnectionTimestamp { get; set; }
    }
}
