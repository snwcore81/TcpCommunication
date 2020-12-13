using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using TcpCommunication.Classes;
using TcpCommunication.Classes.Database.Objects;
using TcpCommunication.Classes.Messages;
using TcpCommunication.Classes.Exceptions;
using TcpCommunication.Classes.System;
using TcpCommunication.Classes.Database;
namespace TcpCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            Log.CurrentLevel = Log.LevelEnum.DEB;
            Log.LogWriter = new ConsoleLogWriter();

            using var _log = Log.DEB("Program", "Main");

            var _db = new MySqlSource
            {
                Host = "127.0.0.1",
                Port = 3306,
                Login = "admin",
                Password = "mydatabase1234",
                Schema = "mydatabase"
            };

            _db.Connect();

            

            try
            {
                _db.TransactionStart();

                RachunkiDbObject _oRachunek = new RachunkiDbObject
                {
                    IdRachunku = 1
                };

                if (_oRachunek.Select(_db))
                {
                    _oRachunek.Kwota = 1020f;
                    _oRachunek.Update(_db);
                }
                else
                {
                    _oRachunek.Wpisujacy = "Jacek K.";
                    _oRachunek.Kwota = 100f;
                    _oRachunek.Insert(_db);
                }

                _db.TransactionCommit();
            }
            catch (Exception e)
            {
                _log.PR_DEB($"Wyjątek! {e.Message}");

                _db.TransactionRollback();
            }

            _db.Disconnect();

                /*
                _db.TransactionStart();

                try
                {
                    foreach (var _oRecord in _db.ExecuteReader("SELECT * FROM Login_T"))
                    {
                        LoginDbObject _oLogin = new LoginDbObject();
                        _oLogin.Set(_oRecord);

                        if (_oLogin.Login == "jacek")
                        {
                            _oLogin.Delete(_db);
                        }
                        else
                        {
                            _oLogin.Password = "haslo";
                            _oLogin.Update(_db);
                        }
                    }

                    _db.TransactionCommit();
                }
                catch (Exception e)
                {
                    _log.PR_DEB($"Wyjątek! {e.Message}");

                    _db.TransactionRollback();
                }


                _db.Disconnect();

                /*
                LoginDbObject _oLogin = new LoginDbObject
                {
                    Login = "jacek"
                }; 

                try
                {
                    _db.TransactionStart();

                    if (_oLogin.Select(_db))
                    {
                        _oLogin.Delete(_db);
                    }
                    else
                    {
                        _oLogin.Password = "test123";
                        _oLogin.Insert(_db);
                    }

                    _db.TransactionCommit();
                }
                catch (Exception e)
                {
                    _log.PR_DEB($"Exception! {e.Message}");

                    _db.TransactionRollback();
                }
                */
            }
    }
}
