#define UNITTEST

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
using System.Collections.Generic;
using TcpCommunication.Classes.Business;

namespace TcpCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            UserList _oLista = new UserList();

            if (File.Exists(@"baza_uzytkownikow.xml"))
            {
                _oLista.LoadFromXml(@"baza_uzytkownikow.xml");
            }
            else
            {
                _oLista.Add(new User { Login = "jkuzmicz", Password = "test123", Permission = 2 });
                _oLista.Add(new User { Login = "zdyrman", Password = "test123", Permission = 1 });
                _oLista.Add(new User { Login = "rochucki", Password = "test123", Permission = 2 });

                _oLista.SaveAsXml(@"baza_uzytkownikow.xml");
            }

            foreach (var _user in _oLista.Collection)
            {
                Console.WriteLine(_user);
            }

            /*
            User.Add().SaveAsXml("user.xml");

            var _oUser = User.LoadFromXml(@"user.xml");

            Console.WriteLine(_oUser);
            */
                /*
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



                foreach (var _oLogin in _db.ExecuteReader<LoginDbObject>())
                {
                    try
                    {
                        _db.TransactionStart();

                        Console.WriteLine(_oLogin);

                        Console.Write($"Podaj nowe hasło dla użytkownika <{_oLogin.Login}>:");
                        _oLogin.Password = Console.ReadLine();

                        if (_oLogin.IsChanged)
                        {
                            _oLogin.LastUpdate = DateTime.Now;
                            _oLogin.Update(_db);
                        }

                        _db.TransactionCommit();

                    }
                    catch (Exception e)
                    {
                        _log.PR_DEB($"Wyjątek! {e.Message}");

                        _db.TransactionRollback();
                    }
                }

                _db.Disconnect();
                */
        }
    }
}
