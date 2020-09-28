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
        private readonly TcpListener    m_oNetObject;

        private readonly List<T>        m_oConnectedClients;

        public ServerService(IPAddress Address, int Port) : base(ModeEnum.Server,Address,Port)
        {
            m_oNetObject = new TcpListener(Address, Port);

            m_oConnectedClients = new List<T>();
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

            NetworkAction?.StateChanged(State.Error,new StateObject(this));
        }

        protected virtual void AcceptConnection()
        {
            try
            {
                NetworkAction?.StateChanged(State.Listening, new StateObject(this));

                m_oNetObject?.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), this);                
            }
            catch (Exception e)
            {
                NetworkAction?.StateChanged(State.Error, new StateObject(this,e));
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
                _client.HostedBy = _obj;

                _obj?.m_oConnectedClients?.Add(_client);

                _obj?.NetworkAction?.StateChanged(State.Established, new StateObject(this,_client));

                _client.FireReceive();

                _obj?.AcceptConnection();

                return;
            }
            catch (Exception)
            {
            }

            _obj?.NetworkAction?.StateChanged(State.Error,new StateObject(this));
        }

        public List<T> ConnectedClients
        {
            get
            {
                m_oConnectedClients.RemoveAll((x) =>
                {
                    try
                    {
                        return !x.IsConnected;
                    }
                    catch (Exception)
                    {
                        return true;
                    }
                });

                return m_oConnectedClients;
            }
        }

    }
}
