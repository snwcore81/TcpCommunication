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
        public string Login { get => Get<string>() ; set => Set(value); }

        [DbField(Constraint = FieldConstraint.NotNull)]
        public string Password { get => Get<string>(); set => Set(value); }        

        [DbField(Constraint = FieldConstraint.Nullable)]
        public DateTime LastUpdate { get => Get<DateTime>(); set => Set(value); }
        public LoginDbObject()
        {
            TableName = "Login_T";
            BaseObject = this;
        }
        public override bool InitializeFromObject(LoginDbObject Object)
        {
            this.Login = Object.Login;
            this.Password = Object.Password;
            this.IsNew = false;

            return true;
        }
    }
}
