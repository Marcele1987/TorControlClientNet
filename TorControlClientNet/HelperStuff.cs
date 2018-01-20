using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TorControlClientNet
{
    public class StringValue : System.Attribute
    {
        private readonly string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

    }

    public static class StringEnum
    {
        public static string GetStringValue(this Enum value)
        {
            string output = null;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs =
               fi.GetCustomAttributes(typeof(StringValue),
                                       false) as StringValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }

    public static class SomeExtensions
    {
        public static bool checkStatusCode(this string s, string code)
        {
            if (s.Length > 3)
            {
                return s.Substring(0, 3).Contains(code);
            }

            return false;
        }
    }
}
