using Newtonsoft.Json;

namespace UsuariosLogin.Models
{
    public class EntityPhone
    {
        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }
        [JsonProperty(PropertyName = "area_code")]
        public int AreaCode { get; set; }
        [JsonProperty(PropertyName = "country_code")]
        public string CountryCode { get; set; }
    }
}
