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

namespace TcpCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            LoginDbObject _dao = new LoginDbObject
            {
                Login = "Jacek"
            };

            Console.WriteLine(_dao);

            _dao.SetPropValue("Login","Testowy");
            _dao.SetPropValue("Tsn", 10);

            foreach (var _name in _dao.GetPropNameByType(Classes.Database.FieldType.PrimaryKey))
                Console.WriteLine($"{_name}={_dao.GetPropValue<string>(_name)}");

            Console.ReadKey();
            */
            
            Console.Clear();

            Log.CurrentLevel = Log.LevelEnum.GOD;

            using (var _log = Log.DEB("Program", "Main"))
            {
                _log.PR_DEB("coś tutaj sobie wydrukuję");

                using (var _log1 = Log.DEB("Program","Inside"))
                {
                    _log1.PR_DEB("Teraz jestem w środku");

                    using (var _log2 = Log.DET("Program","InsindeInside"))
                    {
                        _log2.PR_DET("I idziemy dalej :D");

                        using (var _log3 = Log.DEB("Program", "InsindeInsideInside"))
                        {
                            _log3.PR_DEB("I idziemy dalej :D");
                            _log3.PR_DEB("I idziemy dalej :D");
                            _log3.PR_DEB("I idziemy dalej :D");
                            _log3.PR_DEB("I idziemy dalej :D");
                        }

                        _log2.PR_DET("I idziemy dalej :D");
                    }

                }

                _log.PR_DEB("a teraz na zewnątrz :-)");
            }

            if ((args?.Length ?? 0) < 1)
            {
                Console.WriteLine("Brak argumentu uruchomieniowego!\n1 - serwer\n2 - klient");

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


        }
    }
}
