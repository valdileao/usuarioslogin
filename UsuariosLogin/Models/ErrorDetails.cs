using Newtonsoft.Json;

namespace UsuariosLogin.Models
{
    public class ErrorDetails
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }

}
