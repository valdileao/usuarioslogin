using System;
using System.Collections.Generic;
using System.Linq;
using UsuariosLogin.Data.Entity;
using UsuariosLogin.Models;

namespace UsuariosLogin.Infrastructure
{
    public class Utils
    {
        public static List<EntityPhone> MapperPhonesToEntitiesPhone(List<Phone> phones)
        {
            var entitiesPhone = new List<EntityPhone>();

            phones.ForEach(item => {
                entitiesPhone.Add(new EntityPhone()
                {
                    Number = item.Number,
                    AreaCode = item.AreaCode,
                    CountryCode = item.CountryCode
                });
            });

            return entitiesPhone;
        }

        public static List<Phone> MapperEntitiesPhoneToPhones(List<EntityPhone> entitiesPhone)
        {
            var phones = new List<Phone>();

            entitiesPhone.ForEach(item => {
                phones.Add(new Phone()
                {
                    Number = item.Number,
                    AreaCode = item.AreaCode,
                    CountryCode = item.CountryCode
                });
            });

            return phones;
        }

        public static UserDataResponse MapperApplicationUserToUserDataResponse(ApplicationUser user)
        {
            var userData = new UserDataResponse
            {
                FirstName = user.UserName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                Phones = Utils.MapperPhonesToEntitiesPhone(user.Phones.ToList())
            };


            return userData;
        }

        public static ApplicationUser MapperApplicationUserToUserDataResponse(RegisterEntity model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.Name,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                CreatedAt = DateTime.Now,
                LastLogin = DateTime.Now,
                Phones = Utils.MapperEntitiesPhoneToPhones(model.Phones)
            };

            return user;
        }

    }
}
