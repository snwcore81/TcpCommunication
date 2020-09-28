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

namespace TcpCommunication
{
    public class TestServer : INetworkAction
    {
        private static readonly object LOCKOBJECT = new object();

        public ServerService<ClientService> Server;

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

                    case State.Listening:
                        break;

                    case State.Established:
                        break;

                    case State.Connecting:
                        break;

                    case State.Connected:                        
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

            Console.WriteLine($"OnReceived::{_message}");

            _message.ProcessRequest(a_oStateObj);

            _client.FireSend(new NetworkData(10000) { Buffer = _message.ToXml().ToArray() });
        }

        public virtual void Run()
        {
            Server = new ServerService<ClientService>(IPAddress.Loopback, 1000)
            {
                NetworkAction = this
            };

            Server.Establish();
        }
    }
}
