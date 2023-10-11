using System;
using System.Reflection;

namespace Dribbly.Core.Extensions
{
    public static class EnumExtensions
    {
        public class EnumAttribute : Attribute
        {
            public int Value { get; internal set; }
            public string Subject { get; set; }
            public string Action { get; set; }
            public void SetValue(int value)
            {
                Value = value;
            }
        }

        public static TAttribute GetEnumAttribute<TAttribute>(this Enum value)
            where TAttribute : EnumAttribute
        {
            TAttribute attrib = value.GetAttribute<TAttribute>();
            attrib.SetValue((int)(object)value);
            return attrib;
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttribute<TAttribute>();
        }
    }
}
