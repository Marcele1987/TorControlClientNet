using System;

namespace TorControlClientNet.Helper
{
    public class StringValue : Attribute
    {
        #region Constructors

        public StringValue(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public string Value { get; }

        #endregion
    }

    public static class StringEnum
    {
        #region Members

        public static string GetStringValue(this Enum value)
        {
            string output = null;
            var type = value.GetType();

            var fi = type.GetField(value.ToString());
            var attrs =
                fi.GetCustomAttributes(typeof(StringValue),
                    false) as StringValue[];
            if (attrs.Length > 0) output = attrs[0].Value;

            return output;
        }

        #endregion
    }

    public static class SomeExtensions
    {
        #region Members

        public static bool CheckStatusCode(this string s, string code)
        {
            if (s.Length > 3) return s.Substring(0, 3).Contains(code);

            return false;
        }

        #endregion
    }
}