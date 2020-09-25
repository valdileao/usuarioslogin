using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UsuariosLogin.Infrastructure;

namespace UsuariosLogin.Models
{
    public class RegisterEntity
    {
        [Required]
        [Display(Name = "firstName")]
        [JsonProperty(PropertyName = "firstName")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "lastName")]
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "email")]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "Invalid fields.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [EnsureMinimumElements(1, ErrorMessage = "Missing fields")]
        [Display(Name = "phones")]
        [JsonProperty(PropertyName = "phones")]
        public List<EntityPhone> Phones { get; set; }

    }

}
