using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TcpCommunication.Classes
{
    public static class Extensions
    {
        public static string CleanType(this string a_sTypeName)
        {
            if (!string.IsNullOrEmpty(a_sTypeName) && a_sTypeName.Contains('`')) 
            {
                a_sTypeName = a_sTypeName.Substring(0, a_sTypeName.IndexOf('`')) + "<?>";
            }

            return a_sTypeName;
        }

        public static string ToDb(this object a_oValue)
        {
            string _sResult = string.Empty;

            if (a_oValue == null)
                _sResult = "null";
            else if (a_oValue is string)
                _sResult = $"'{a_oValue}'";
            else if (Regex.IsMatch(a_oValue.ToString(), @"-?\d+(\.\d+)?"))
                _sResult = a_oValue.ToString();

            return _sResult;
        }
    }
}
