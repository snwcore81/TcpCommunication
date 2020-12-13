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
using TcpCommunication.Classes.System;

namespace TcpCommunication
{
    public class TestServer : INetworkAction
    {
        private static readonly object LOCKOBJECT = new object();

        public ServerService<ClientService> Server;

        public void NetworkStateChanged(NetworkState a_eState, StateObject a_oStateObj = null)
        {
            using var log = Log.DET(this, "StateChanged");

            log.PR_DEB($"state = <{a_eState}> object = <{a_oStateObj}>");


            lock (LOCKOBJECT)
            {

                switch (a_eState)
                {
                    case NetworkState.Sending:
                        break;

                    case NetworkState.Sent:
                        break;

                    case NetworkState.Listening:
                        break;

                    case NetworkState.Established:
                        OnEstablished(a_oStateObj);
                        break;

                    case NetworkState.Connecting:
                        break;

                    case NetworkState.Connected:                        
                        break;

                    case NetworkState.Receiving:
                        break;

                    case NetworkState.Received:
                        OnReceived(a_oStateObj);
                        break;

                    case NetworkState.Error:
                        break;
                }
            }
        }

        protected void OnReceived(StateObject a_oStateObj)
        {
            using var log = Log.DEB(this, "OnReceived");

            var _client = a_oStateObj.GetObject<ClientService>();

            var _message = MessageFactory.Instance.Create(_client.Data.BufferWithData) as IMessage;

            log.PR_DEB($"OnReceived::{_message}");

            if (_message.ProcessRequest(a_oStateObj) != null)
            {
                _client.AsyncSend(_message.AsNetworkData());
            }

            _client.AsyncReceive();
        }

        protected void OnEstablished(StateObject a_oStateObj)
        {
            using var _log = Log.DEB(this, "OnEstablished");

            a_oStateObj.GetData<ClientService>().AsyncReceive();
        }

        public virtual void Run()
        {
            using var log = Log.DEB(this, "Run");

            Server = new ServerService<ClientService>(IPAddress.Loopback, 1000)
            {
                NetworkAction = this
            };

            Server.Establish();

            Console.ReadKey();
        }
    }
}
