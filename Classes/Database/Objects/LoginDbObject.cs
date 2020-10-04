using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TcpCommunication.Classes.Database.Objects
{
    public class LoginDbObject : DbObject<LoginDbObject>
    {
        [DbField(Type=FieldType.PrimaryKey,Constraint=FieldConstraint.NotNull)]
        public string Login { get; set; }

        public LoginDbObject()
        {
            BaseObject = this;
        }

        public override bool InitializeFromObject(LoginDbObject Object)
        {
            this.Login = Object.Login;
            return true;
        }

        public void DisplayInfo()
        {
            foreach (var _prop in GetType().GetProperties().ToList())
            {
                if (Attribute.IsDefined(_prop,typeof(DbFieldAttribute)))
                {
                    Console.WriteLine(_prop.GetCustomAttribute<DbFieldAttribute>());
                }

                Console.WriteLine($"<{_prop.Name}> = { _prop.GetValue(this)}");
                
            }
        }
    }
}
