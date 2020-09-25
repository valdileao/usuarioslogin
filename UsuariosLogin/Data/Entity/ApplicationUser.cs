using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace UsuariosLogin.Data.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [PersonalData]
        public DateTime CreatedAt { get; set; }
        [PersonalData]
        public DateTime LastLogin { get; set; }
        public ICollection<Phone> Phones { get; set; }
    }
}
