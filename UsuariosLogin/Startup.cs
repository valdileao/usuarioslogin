using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using UsuariosLogin.Data;
using UsuariosLogin.Data.Entity;
using UsuariosLogin.Infrastructure;
using UsuariosLogin.Models;

namespace UsuariosLogin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfig = configuration;
        }

        public IConfiguration Configuration { get; }

        public static IConfiguration StaticConfig { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<UsuariosLoginContext>(opt => opt.UseInMemoryDatabase(Configuration.GetConnectionString("tempDatabase")));
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                // User defined password policy settings.  
                config.Password.RequiredLength = 3;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<UsuariosLoginContext>()
            .AddDefaultTokenProviders();
            // Add Jwt Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(options =>
            {
                //Set default Authentication Schema as Bearer
                options.DefaultAuthenticateScheme =
                           JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme =
                           JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                           JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters =
                       new TokenValidationParameters
                       {
                           ValidateAudience = false,
                           ValidateIssuer = false,
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                           ClockSkew = TimeSpan.Zero // remove delay of token when expire
                       };
                cfg.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        if (!context.Response.Headers.ContainsKey("Token-Expired"))
                        {
                            context.HttpContext.WriteErrorDetailsResponseJson(new ErrorDetails()
                            {
                                Message = Startup.StaticConfig["MessagesError:Unauthorized"],
                                ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:UnauthorizedErrorCode"])
                            });
                        }
                        else
                        {
                            context.Response.Headers.Remove("Token-Expired");
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "True");

                            context.HttpContext.WriteErrorDetailsResponseJson(new ErrorDetails()
                            {
                                Message = Startup.StaticConfig["MessagesError:UnauthorizedInvalidSession"],
                                ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:UnauthorizedInvalidSessionErrorCode"])
                            });
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddNewtonsoftJson();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }

}
