using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsuariosLogin.Models
{
    public class UserDataResponse
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        public List<EntityPhone> Phones { get; set; }
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty(PropertyName = "last_login")]
        public DateTime LastLogin { get; set; }
    }
}
