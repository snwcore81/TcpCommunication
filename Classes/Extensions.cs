using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
