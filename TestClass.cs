using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using TcpCommunication.Classes.Services;
using TcpCommunication.Classes;
using TcpCommunication.Interfaces;

using static TcpCommunication.Interfaces.INetworkAction;

namespace TcpCommunication
{
    public class TestClass : INetworkAction
    {
        public ServerService<ClientService> Server;
        public ClientService Client;

        public void StateChanged(State a_eState, object a_oStateObject = null)
        {
            switch (a_eState)
            {
                case State.Listening:
                    Console.WriteLine($"{a_oStateObject ?? ""} tryb nasłuchu...{Server.Address}:{Server.Port}"); break;

                case State.Established:
                    Console.WriteLine($"{a_oStateObject ?? ""} podłączono klienta!"); break;

                case State.Connecting:
                    Console.WriteLine($"{a_oStateObject ?? ""} próba połączenia..."); break;

                case State.Connected:
                    Console.WriteLine($"{a_oStateObject ?? ""} połączono do serwera!");
                    (a_oStateObject as ClientService)?.FireReceive();

                    NetworkData _oData = new NetworkData(1000)
                    {
                        Buffer = Encoding.UTF8.GetBytes("łańcuch testowy:D")
                    };

                    (a_oStateObject as ClientService)?.FireSend(_oData);
                    break;

                case State.Receiving:
                    Console.WriteLine($"{a_oStateObject ?? ""} oczekiwanie na dane!"); break;

                case State.Received:

                    var _buffer = a_oStateObject as byte[];
                    
                    Console.WriteLine($"{a_oStateObject ?? ""} odebrano = {_buffer?.Length ?? 0} bajtow [{Encoding.UTF8.GetString(_buffer)}]"); 
                    break;

                case State.Error:
                    Console.WriteLine($"{a_oStateObject ?? ""} błąd!"); break;
            }
        }

        public virtual void Run()
        {
            Server = new ServerService<ClientService>(IPAddress.Loopback, 1000)
            {
                NetworkAction = this
            };

            Server.Establish();

            Thread.Sleep(100);

            Client = new ClientService(IPAddress.Loopback, 1000)
            {
                NetworkAction = this
            };

            Client.Establish();
        }
    }
}
