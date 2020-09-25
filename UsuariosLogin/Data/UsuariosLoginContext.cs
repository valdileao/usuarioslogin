using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsuariosLogin.Data.Entity;

namespace UsuariosLogin.Data
{
    public class UsuariosLoginContext : IdentityDbContext<ApplicationUser>
    {
        public UsuariosLoginContext(DbContextOptions<UsuariosLoginContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().OwnsMany(applicationUser => applicationUser.Phones, action => 
            {
                action.WithOwner().HasForeignKey("ApplicationUser_Id");
                action.Property<int>("Id");
                action.HasKey("Id");
            });

            base.OnModelCreating(builder);
        }
    }
}
