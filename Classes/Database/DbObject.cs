using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace TcpCommunication.Classes.Database
{
    public enum FieldType
    {
        PrimaryKey      = 0x0001,
        ForeignKey      = 0x0010,
        Autoincrement   = 0x0020,
        Other           = 0x0100
    }

    public enum FieldConstraint
    {
        Nullable   = 0x0001,
        NotNull    = 0x0010
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class DbFieldAttribute : System.Attribute
    {
        public string           Name { get; private set; }
        public FieldType        Type { get; set; } = FieldType.Other;
        public FieldConstraint  Constraint { get; set; } = FieldConstraint.NotNull;

        public DbFieldAttribute([CallerMemberName] string Name = "")
        {
            this.Name = Name;
        }
        public override string ToString()
        {
            return $"[Nazwa={Name}|Typ={Type}|Więzy={Constraint}]";
        }
    }


    [DataContract]
    public abstract class DbObject<T> : XmlStorage<T> where T : class
    {
        [IgnoreDataMember]
        public const string DEFAULT_NAME = "init";

        [DbField, DataMember]
        public DateTime     InsertDate { get; set; } = DateTime.Now;
        [DbField, DataMember]
        public string       InsertName { get; set; } = DEFAULT_NAME;
        [DbField, DataMember]
        public DateTime     UpdateDate { get; set; } = DateTime.Now;
        [DbField, DataMember]
        public string       UpdateName { get; set; } = DEFAULT_NAME;
        [DbField, DataMember]
        public int          TSN { get; set; } = 0;

        public override string ToString()
        {
            string _sResult = "";

            foreach (var _property in typeof(T).GetProperties().ToList())
            {
                var _attrib = _property.GetCustomAttribute<DbFieldAttribute>();

                if (_attrib != null)
                {
                    _sResult += $"[{(_attrib.Type.HasFlag(FieldType.PrimaryKey) ? "*" : "")}{_property.Name}={_property.GetValue(this)}]";
                }
            }

            return _sResult;
        }
    }
}
