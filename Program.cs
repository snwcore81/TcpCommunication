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
            NetworkData _data = new NetworkData(100000);

            MessageFactory.Instance.Register<LoginMessage>();


            var _login = MessageFactory.Instance.Create("LoginMessage");

            _login.Login = "jacek";
            _login.Password = "12jacek34";

            _login.ProcessRequest();

            Console.WriteLine();

            _data.Buffer = ((_login as LoginMessage).ToXml() as MemoryStream).ToArray();

            Console.WriteLine($"BufferLength={_data.BufferLength} DataLength={_data.DataLength()}");

            for (int _i=0;_i<_data.DataLength();++_i)
            {
                Console.Write($"<{_data.Buffer[_i]}>");
            }

            Console.ReadKey();

            /*
            LoginMessage _login = new LoginMessage
            {
                Login = "jacek",
                Password = "12jacek34"
            }.ProcessRequest();

            var _stream = _login.ToXml() as MemoryStream;

            Console.WriteLine($"[{Encoding.UTF8.GetString(_stream.ToArray())}]");

            using (var _reader = XmlReader.Create(new MemoryStream(_stream.ToArray())))
            {
                _reader.Read();
                Console.WriteLine(_reader.Name);
                
            }
            
            var _response = new LoginMessage();
            _response.FromXml(new MemoryStream(_stream.ToArray()));

            Console.WriteLine(_response);

            Console.ReadKey();
            */
        }
    }
}
