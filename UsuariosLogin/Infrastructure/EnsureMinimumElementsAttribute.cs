using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsuariosLogin.Infrastructure
{
    public class EnsureMinimumElementsAttribute : ValidationAttribute
    {
        private readonly int _minElements;
        public EnsureMinimumElementsAttribute(int minElements)
        {
            _minElements = minElements;
        }

        public override bool IsValid(object value)
        {
            if (value is IList list)
            {
                return list.Count >= _minElements;
            }
            return false;
        }
    }
}
