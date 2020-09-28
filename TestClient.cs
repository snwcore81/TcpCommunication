using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using TcpCommunication.Classes.Services;
using TcpCommunication.Classes;
using TcpCommunication.Interfaces;

using static TcpCommunication.Interfaces.INetworkAction;
using System.Threading.Tasks;
using TcpCommunication.Classes.Messages;
using System.IO;

namespace TcpCommunication
{
    public class TestClient : INetworkAction
    {
        private static readonly object LOCKOBJECT = new object();

        public ClientService Client;

        public void StateChanged(State a_eState, StateObject a_oStateObj = null)
        {
            lock (LOCKOBJECT)
            {

                switch (a_eState)
                {
                    case State.Sending:
                        break;

                    case State.Sent:
                        break;

                    case State.Connecting:
                        break;

                    case State.Connected:
                        OnConnected(a_oStateObj);

                        break;

                    case State.Receiving:                        
                        break;

                    case State.Received:
                        OnReceived(a_oStateObj);
                        break;

                    case State.Error:
                        break;
                }
            }
        }

        protected void OnReceived(StateObject a_oStateObj)
        {
            var _client = a_oStateObj.GetObject<ClientService>();
            var _buffer = a_oStateObj.GetData<byte[]>();

            var _message = MessageFactory.Instance.Create(_buffer);

            try
            {
                _message.ProcessResponse(a_oStateObj);

                Console.WriteLine("Zalogowano do systemu!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystąpił błąd! {e.Message}");
            }
        }

        protected void OnConnected(StateObject a_oStateObj)
        {
            var _client = a_oStateObj.GetObject<ClientService>();

            Console.Write("Podaj login:");

            var _loginTelegram = new LoginMessage
            {
                Login = Console.ReadLine()
            };

            _client.FireSend(new NetworkData(10000)
            {
                Buffer = _loginTelegram.ToXml().ToArray()
            });
        }

        public virtual void Run()
        {
            Client = new ClientService(IPAddress.Loopback, 1000)
            {
                NetworkAction = this
            };

            Client.Establish();
        }
    }
}
