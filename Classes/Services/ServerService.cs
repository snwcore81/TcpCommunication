using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using static TcpCommunication.Interfaces.INetworkAction;

namespace TcpCommunication.Classes.Services
{
    public class ServerService<T> : NetworkService where T : ClientService
    {
        private readonly TcpListener m_oNetObject;

        public ServerService(IPAddress Address, int Port) : base(ModeEnum.Server,Address,Port)
        {
            m_oNetObject = new TcpListener(Address, Port);
        }

        public override bool IsConnected => (m_oNetObject?.Server?.Connected ?? false);

        public override Socket NetworkSocket => (m_oNetObject?.Server ?? null);

        public override void Establish()
        {
            if (IsConnected)
                return;

            try
            {
                m_oNetObject.Start();

                AcceptConnection();

                return;
            }
            catch (Exception)
            {
            }

            NetworkAction?.StateChanged(State.Error,this);
        }

        protected virtual void AcceptConnection()
        {
            try
            {
                m_oNetObject?.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), this);

                NetworkAction?.StateChanged(State.Listening,this);
            }
            catch (Exception)
            {
                NetworkAction?.StateChanged(State.Error, this);
            }            
        }

        protected virtual void AcceptCallback(IAsyncResult ar)
        {
            var _obj = ar.AsyncState as ServerService<T>;

            try
            {
                var _client = (T)Activator.CreateInstance(typeof(T), new object[] 
                { 
                    _obj.NetworkSocket.EndAccept(ar), 
                    NetworkService.BUFFER_SIZE 
                });

                _client.NetworkAction = _obj.NetworkAction;

                _client.FireReceive();

                _obj?.NetworkAction?.StateChanged(State.Established,_client);

                _obj?.AcceptConnection();

                return;
            }
            catch (Exception)
            {
            }

            _obj?.NetworkAction?.StateChanged(State.Error,this);
        }
    }
}
