using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using TcpCommunication.Classes;
using TcpCommunication.Classes.Messages;

namespace TcpCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageFactory.Instance.Register<LoginMessage>();

            new TestServer().Run();

            new TestClient().Run();

            Console.ReadKey();
        }
    }
}
