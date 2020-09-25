using System.ComponentModel.DataAnnotations;

namespace UsuariosLogin.Data.Entity
{
    public class Phone
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int AreaCode { get; set; }
        public string CountryCode { get; set; }
        public string ApplicationUser_Id { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}