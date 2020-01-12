using System;
using System.ComponentModel;
using System.Linq;

namespace SonoffApi.Client
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T e) where T : Enum
        {
            if (e is Enum)
            {
                var type = typeof(T);
                var attribute = type.GetMember(type.GetEnumName(e))[0].GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

                return attribute?.Description;
            }

            return e.ToString();
        }
    }
}