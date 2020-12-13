using System;
using System.Collections.Generic;
using System.Text;
using TcpCommunication.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using TcpCommunication.Classes.System;

namespace TcpCommunication.Classes.Database
{
    [DataContract]
    public class MySqlSource : XmlStorage<MySqlSource>, IDbSource, IDisposable
    {
        [IgnoreDataMember]
        private MySqlConnection Connection;
        [IgnoreDataMember]
        private bool ActiveTransaction;
        [IgnoreDataMember]
        public bool IsConnected => (Connection?.State == ConnectionState.Open);

        [DataMember]
        public string Host { get; set; }
        [DataMember]
        public int Port { get; set; }
        [DataMember]
        public string Schema { get; set; }
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Password { get; set; }

        public bool Connect()
        {
            using var _log = Log.DEB(this, "Connect");

            if (IsConnected)
                return true;

            try
            {
                Connection = new MySqlConnection($"SERVER={Host};DATABASE={Schema};UID={Login};PASSWORD={Password};port={Port};charset=utf8;IgnorePrepare=true");

                Connection.Open();

                _log.PR_DEB($"Connected! {Connection} {Connection.ServerVersion}");

                return true;
            }
            catch (Exception e)
            {
                _log.PR_DEB($"Exception! {e.Message}");
            }

            return false;
        }

        public bool Disconnect()
        {
            using var _log = Log.DEB(this, "Disconnect");

            if (!IsConnected)
                return true;

            try
            {
                Connection.Close();
                Connection = null;
            }
            catch (Exception e)
            {
                _log.PR_DEB($"Exception! {e.Message}");
            }

            return false;
        }

        public void Dispose()
        {
            Disconnect();
        }
        public int ExecuteNonQuery(string a_sQuery) => ExecuteNonQuery(new string[] { a_sQuery });

        public int ExecuteNonQuery(string[] a_oQueries)
        {
            using var _log = Log.DEB(this, "ExecuteNonQuery");

            int _iQueriesExecuted = 0;

            foreach (var _sQuery in a_oQueries)
            {
                MySqlCommand _oSqlCmd = new MySqlCommand(_sQuery, Connection);

                _log.PR_DEB($"Query=[{_sQuery}]");

                _iQueriesExecuted += _oSqlCmd.ExecuteNonQuery();
            }

            return _iQueriesExecuted;
        }

        public List<DbRow> ExecuteReader(string a_sQuery)
        {
            using var _log = Log.DEB(this, "ExecuteReader");

            _log.PR_DEB($"Query=[{a_sQuery}]");

            List<DbRow> _oResult = new List<DbRow>();

            MySqlCommand _oSqlCmd = new MySqlCommand(a_sQuery, Connection);

            MySqlDataReader _oSqlDataReader = _oSqlCmd.ExecuteReader();

            while (_oSqlDataReader.Read())
            {
                DbRow _oRow = new DbRow();

                for (int i = 0; i < _oSqlDataReader.FieldCount; ++i)
                {
                    _oRow.Add(_oSqlDataReader.GetName(i), _oSqlDataReader[i]);
                }

                _oResult.Add(_oRow);

                _log.PR_DEB(_oRow.ToString());

            }

            _log.PR_DEB($"Fetched {_oResult.Count} row(s)");

            if (!_oSqlDataReader.IsClosed)
                _oSqlDataReader.Close();

            return _oResult;
        }

        public List<T> ExecuteReader<T>(string a_sWhereClausule = "", string a_sOrderByClausule = "") 
            where T : DbObject<T>, new()
        {
            List<T> _oResult = new List<T>();

            string _sQuery = $"SELECT * FROM {new T().TableName}";

            if (!string.IsNullOrEmpty(a_sWhereClausule))
                _sQuery += $"WHERE 1=1 AND {a_sWhereClausule} ";

            if (!string.IsNullOrEmpty(a_sOrderByClausule))
                _sQuery += $"ORDER BY {a_sOrderByClausule} ";

            foreach (var _oRow in ExecuteReader(_sQuery))
            {
                T _oRecord = new T();

                _oRecord.Set(_oRow);

                _oRecord.IsNew = false;

                _oResult.Add(_oRecord);
            }

            return _oResult;
        }

        public bool Delete<T>(DbObject<T> Object) where T : class
        {
            using var _log = Log.DEB(this, "Delete");

            int _iRowsAffected = 0;

            string _sQuery = $"DELETE FROM {Object.TableName} WHERE 1=1";

            foreach (var _oPK in Object.PrimaryKey)
            {
                _sQuery += $" AND {_oPK.Key}={_oPK.Value.ToDb()}";
            }

            _iRowsAffected = ExecuteNonQuery(_sQuery);

            if (_iRowsAffected < 1)
            {
                _log.PR_DEB("No row deleted");
            }
            else
            {
                _log.PR_DEB("Row deleted");
            }

            return _iRowsAffected>0;
        }

        public bool Exists<T>(DbObject<T> Object) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Insert<T>(DbObject<T> Object) where T : class
        {
            using var _log = Log.DEB(this, "Insert");

            string _sQuery = $"INSERT INTO {Object.TableName} ({string.Join(',', Object.PropertiesNamesList)}) " +
                             $"VALUES ({string.Join(',', Object.ValuesList())})";

            int _iRowAffected = ExecuteNonQuery(_sQuery);

            _log.PR_DEB($"{_iRowAffected} row(s) affected");

            return _iRowAffected > 0;
        }

        public bool Select<T>(DbObject<T> Object) where T : class
        {
            using var _log = Log.DEB(this, "Select");

            string _sQuery = $"SELECT * FROM {Object.TableName} WHERE 1=1";

            foreach (var _oPK in Object.PrimaryKey)
            {
                _sQuery += $" AND {_oPK.Key}={_oPK.Value.ToDb()}";
            }

            if (Object.LockForUpdate)
                _sQuery += " FOR UPDATE";

            foreach (var _oRow in ExecuteReader(_sQuery))
            {
                return Object.Set(_oRow);
            }

            return false;
        }
        public bool Update<T>(DbObject<T> Object) where T : class
        {
            using var _log = Log.DEB(this, "Update");

            int _iRowsAffected = 0;

            if (Object.IsChanged)
            {
                string _sQuery = $"UPDATE {Object.TableName} SET {string.Join(',',Object.PropertiesNamesAndValuesList(FieldType.Other))} WHERE 1=1";

                foreach (var _oPK in Object.PrimaryKey)
                {
                    _sQuery += $" AND {_oPK.Key}={_oPK.Value.ToDb()}";
                }

                _iRowsAffected = ExecuteNonQuery(_sQuery);
            }

            if (_iRowsAffected < 1)
            {
                _log.PR_DEB("No row update");
            }
            else
            {
                _log.PR_DEB("Row updated");
            }

            return _iRowsAffected>0;
        }

        public void TransactionCommit()
        {
            if (!ActiveTransaction)
                return;

            using var _log = Log.DEB(this, "TransactionCommit");

            ExecuteNonQuery("COMMIT");

            ActiveTransaction = false;
        }

        public void TransactionRollback()
        {
            if (!ActiveTransaction)
                return;

            using var _log = Log.DEB(this, "TransactionRollback");

            ExecuteNonQuery("ROLLBACK");

            ActiveTransaction = false;
        }

        public void TransactionStart()
        {
            if (ActiveTransaction)
                return;

            using var _log = Log.DEB(this, "TransactionStart");

            ExecuteNonQuery(new string[] { "START TRANSACTION WITH CONSISTENT SNAPSHOT", "SET autocommit = 0" });
            
            ActiveTransaction = true;
        }

        public override bool InitializeFromObject(MySqlSource Object)
        {
            this.Login = Object.Login;
            this.Password = Object.Password;
            this.Host = Object.Host;
            this.Port = Object.Port;
            this.Schema = Object.Schema;

            return true;
        }


    }
}
