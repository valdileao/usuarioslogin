using System.ComponentModel.DataAnnotations;

namespace UsuariosLogin.Models
{
    public class LoginEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
