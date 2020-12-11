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

            _db.Disconnect();

            
            /*
            Console.WriteLine(_oLogin);

            foreach (var _value in _oLogin.ChangedValues)
            {
                Console.WriteLine($"{_value.Key}={_value.Value}");
            }*/

            /*
            if ((args?.Length ?? 0) < 1)
            {
                _log.PR_DEB("Brak argumentu uruchomieniowego! 1 - serwer, 2 - klient");

            }
            else
            {
                int.TryParse(args[0], out int _iMode);

                XmlStorageTypes.Register<Exception>();

                MessageFactory.Instance.Register<LoginMessage>();
                MessageFactory.Instance.Register<TextMessage>();

                switch (_iMode)
                {
                    case 1:
                        new TestServer().Run(); break;

                    case 2:
                        new TestClient().Run(); break;

                }

            }
            */

        }
    }
}
