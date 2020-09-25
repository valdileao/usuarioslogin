using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UsuariosLogin.Data.Entity;
using UsuariosLogin.Models;

namespace UsuariosLogin.Infrastructure
{
    public static class ExtensionsUsuariosLogin
    {

        public static ErrorDetails ReturnErrorDetailsHandlingIdentityErrors(this IEnumerable<IdentityError> errors)
        {
            var identityError = errors?.FirstOrDefault(error => error.Code.ToLower() == "duplicateusername");
            if (identityError != null)
            {
                return new ErrorDetails()
                {
                    Message = Startup.StaticConfig["MessagesError:EmailExist"],
                    ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:EmailExistErrorCode"])
                };
            }
            else
            {
                return new ErrorDetails()
                {
                    Message = Startup.StaticConfig["MessagesError:InvalidFields"],
                    ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:InvalidFieldsErrorCode"])
                };
            }
        }

        public static async Task<bool> UpdateLastLoginDateFromApplicationUserIfSucceeded(this UserManager<ApplicationUser> _userManager, ApplicationUser user)
        {
            user.LastLogin = DateTime.Now;
            var identityResult = await _userManager.UpdateAsync(user);
            return identityResult.Succeeded;
        }

        public static void WriteErrorDetailsResponseJson(this HttpContext Context, ErrorDetails errorDetails)
        {
            Context.Response.ContentType = "application/json";
            using Utf8JsonWriter writer = new System.Text.Json.Utf8JsonWriter(Context.Response.BodyWriter);
            writer.WriteStartObject();
            foreach (var item in errorDetails.GetType().GetProperties())
            {
                writer.WritePropertyName(item.Name);
                if (item.PropertyType == typeof(int))
                {
                    writer.WriteNumberValue((int)item.GetValue(errorDetails));
                }
                else if (item.PropertyType == typeof(string))
                {
                    writer.WriteStringValue(item.GetValue(errorDetails).ToString());
                }
            }

            writer.WriteEndObject();
            writer.Flush();
        }
    }
}
