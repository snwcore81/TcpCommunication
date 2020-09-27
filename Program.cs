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
            var _testClass = new TestClass();

            _testClass.Run();

            Console.ReadKey();
        }
    }
}
