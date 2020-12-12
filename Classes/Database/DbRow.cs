using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunication.Classes.Database
{
    public class DbRow 
    {
        public Dictionary<string,object> Data { get; set; }
        public DbRow()
        {
            Data = new Dictionary<string, object>();
        }
        public int Count => Data.Count;        
        public void Clear()
        {
            Data.Clear();
        }
        public void Add(string a_sKey, object a_oValue)
        {
            Data.Add(a_sKey, a_oValue);
        }
        public bool Contains(string a_sKey) => Data.ContainsKey(a_sKey);
        public object this[string a_sKey] => Data[a_sKey];

        public override string ToString()
        {
            string _sResult = "";

            foreach (var _oElement in Data)
                _sResult += $"[{_oElement.Key}={_oElement.Value}]";

            return _sResult;
        }
    }
}
