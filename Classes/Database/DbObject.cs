using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using TcpCommunication.Classes.Exceptions;

namespace TcpCommunication.Classes.Database
{
    public enum FieldType
    {
        PrimaryKey      = 0x0001,
        Autoincrement   = 0x0020,
        Other           = 0x0100,
        All             = 0x1000
    }

    public enum FieldConstraint
    {
        Nullable   = 0x0001,
        NotNull    = 0x0010
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbFieldAttribute : Attribute
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
        #if DEFAULT_FIELDS
        [IgnoreDataMember]
        public const string DEFAULT_NAME = "init";
        #endif

        [IgnoreDataMember]
        private readonly Dictionary<string, Object> m_oValues = new Dictionary<string, object>();
        [IgnoreDataMember]
        private readonly Dictionary<string, Object> m_oOldValues = new Dictionary<string, object>();
        [IgnoreDataMember]
        private readonly Dictionary<string, bool> m_oChangedValues = new Dictionary<string, bool>();
        [IgnoreDataMember]
        private bool m_bNew = false;

        #if DEFAULT_FIELDS
        [DbField, DataMember]
        public DateTime     InsertDate { get => Get<DateTime>(); set => Set(value); }
        [DbField, DataMember]
        public string       InsertName { get => Get<string>(); set => Set(value); }
        [DbField, DataMember]
        public DateTime     UpdateDate { get => Get<DateTime>(); set => Set(value); }
        [DbField, DataMember]
        public string       UpdateName { get => Get<string>(); set => Set(value); }
        [DbField, DataMember]
        public int          Tsn { get => Get<int>(); set => Set(value); }
        #endif

        [DataMember]
        public abstract string TableName { get; }
        [DataMember]
        public bool LockForUpdate { get; set; }

        public DbObject()
        {
            #if DEFAULT_FIELDS
            InsertDate = DateTime.Now;
            InsertName = DEFAULT_NAME;
            UpdateDate = DateTime.Now;
            UpdateName = DEFAULT_NAME;
            Tsn = 0;
            #endif
            LockForUpdate = false;
        }

        public virtual bool HasProperty(string a_sPropName)
        {
            return typeof(T).GetProperties().First(x => x.Name.ToLower() == a_sPropName.ToLower()) != null;
        }

        public virtual U Get<U>([CallerMemberName] string a_sPropName = "")
        {
            a_sPropName = a_sPropName.ToLower();

            if (!HasProperty(a_sPropName))
                throw new DaoPropertyNotFound(this,a_sPropName);

            if (m_oValues.ContainsKey(a_sPropName))
            {
                return (U)Convert.ChangeType(m_oValues[a_sPropName], typeof(U));
            }

            return default;
        }

        public virtual void Set(object a_oValue, [CallerMemberName] string a_sPropName = "")
        {
            a_sPropName = a_sPropName.ToLower();

            if (!HasProperty(a_sPropName))
                throw new DaoPropertyNotFound(this, a_sPropName);

            bool _bIsNullable = (typeof(T).GetProperties().First(a => a.Name.ToLower() == a_sPropName)?.GetCustomAttribute<DbFieldAttribute>()?.Constraint == FieldConstraint.Nullable);

            if (!_bIsNullable && a_oValue == null)
                throw new DaoPropertyNotNullable(this, a_sPropName);

            if (m_oValues.ContainsKey(a_sPropName) && !m_oValues[a_sPropName].Equals(a_oValue))
            {
                m_oChangedValues[a_sPropName] = true;

                m_oOldValues[a_sPropName] = m_oValues[a_sPropName];
            }

            m_oValues[a_sPropName] = a_oValue;
        }
        public virtual bool IsChanged => m_oChangedValues.Count > 0;
        public virtual bool IsNew
        {
            get => m_bNew;
            set
            {
                m_bNew = value;

                if (!m_bNew)
                {
                    m_oOldValues.Clear();
                    m_oChangedValues.Clear();
                }
            }
        }
        public override string ToString()
        {
            string _sResult = "";

            foreach (var _property in typeof(T).GetProperties().ToList())
            {
                var _attrib = _property.GetCustomAttribute<DbFieldAttribute>();

                if (_attrib != null)
                {
                    _sResult += $"[{(_attrib.Type.HasFlag(FieldType.PrimaryKey) ? "*" : "")}{_attrib.Name}={_property.GetValue(this)}]";
                }
            }

            return _sResult;
        }
        public Dictionary<string,object> ChangedValues
        {
            get
            {
                Dictionary<string, object> _oResult = new Dictionary<string, object>();

                foreach (var _oValue in m_oValues)
                {
                    if (m_oChangedValues.ContainsKey(_oValue.Key) && m_oChangedValues[_oValue.Key])
                        _oResult.Add(_oValue.Key, _oValue.Value);

                }

                return _oResult;
            }            
        }
        public Dictionary<string, object> Values => m_oValues;

        public Dictionary<string, object> PrimaryKey
        {
            get
            {
                Dictionary<string, object> _oResult = new Dictionary<string, object>();

                foreach (var _property in typeof(T).GetProperties().ToList())
                {
                    var _attrib = _property.GetCustomAttribute<DbFieldAttribute>();

                    if (_attrib != null && (_attrib.Type.HasFlag(FieldType.PrimaryKey) || _attrib.Type.HasFlag(FieldType.Autoincrement)))
                    {
                        _oResult.Add(_attrib.Name, _property.GetValue(this));
                    }
                }

                return _oResult;
            }
        }

        public virtual string[] ColumnNames
        {
            get
            {
                List<string> _oResult = new List<string>();

                foreach (var _property in typeof(T).GetProperties().ToList())
                {
                    var _attrib = _property.GetCustomAttribute<DbFieldAttribute>();

                    if (_attrib != null)
                    {
                        _oResult.Add(_attrib.Name);
                    }
                }

                return _oResult.ToArray();
            }
        }

        public virtual string[] ValuesList(FieldType a_eField = FieldType.All)
        {
            List<string> _oResult = new List<string>();

            foreach (var _property in typeof(T).GetProperties().ToList())
            {
                var _attrib = _property.GetCustomAttribute<DbFieldAttribute>();

                if (_attrib != null)                
                {
                    if (a_eField == FieldType.All || _attrib.Type.HasFlag(a_eField))
                        _oResult.Add(_property.GetValue(this).ToDb());
                }
            }

            return _oResult.ToArray();
        }

        public virtual string[] PropertyValuesList(FieldType a_eField = FieldType.All, bool a_bOnlyChanged = true)
        {
            List<string> _oResult = new List<string>();

            foreach (var _property in typeof(T).GetProperties().ToList())
            {
                var _attrib = _property.GetCustomAttribute<DbFieldAttribute>();

                if (_attrib != null)
                {
                    if ((a_eField == FieldType.All || _attrib.Type.HasFlag(a_eField)) && 
                        (!a_bOnlyChanged || (m_oChangedValues.ContainsKey(_attrib.Name.ToLower()) && m_oChangedValues[_attrib.Name.ToLower()])))
                        _oResult.Add($"{_attrib.Name}={_property.GetValue(this).ToDb()}");
                }
            }

            return _oResult.ToArray();
        }

    }
}
