using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using TcpCommunication.Classes;
using TcpCommunication.Classes.Database.Objects;
using TcpCommunication.Classes.Messages;

namespace TcpCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
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

            /*
            Console.Clear();

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
            */
                       
        }
    }
}
