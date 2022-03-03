using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Exrensions
{
    public static class ReflectionExtension
    {
        public static string GetPropertyValue<T>(this T item , string PropertyName)
        {
            return item.GetType().GetProperty(PropertyName).GetValue(item, null).ToString();
        }
    }
}
