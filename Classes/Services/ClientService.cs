using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using static TcpCommunication.Interfaces.INetworkAction;

namespace TcpCommunication.Classes.Services
{
    public class ClientService : NetworkService
    {
        private readonly TcpClient m_oNetObject;

        public ClientService(Socket a_oSocket, int a_iBufferLength = 100000) :
            base(ModeEnum.Client, a_iBufferLength)
        {
            m_oNetObject = new TcpClient
            {
                Client = a_oSocket
            };

        }

        public ClientService(IPAddress Address,int Port,int a_iBufferLength = 100000) : 
            base(ModeEnum.Client, Address, Port, a_iBufferLength)
        {
            m_oNetObject = new TcpClient();
        }
        public override bool IsConnected => (m_oNetObject?.Client?.Connected ?? false);

        public override Socket NetworkSocket => m_oNetObject?.Client ?? null;

        public override void Establish()
        {
            if (IsConnected)
                return;

            try
            {
                m_oNetObject.BeginConnect(Address, Port, new AsyncCallback(ConnectCallback), this);

                NetworkAction?.NetworkStateChanged(NetworkState.Connecting, new StateObject(this));

                return;
            }
            catch (Exception)
            {
            }

            NetworkAction?.NetworkStateChanged(NetworkState.Error,new StateObject(this));
        }

        protected virtual void ConnectCallback(IAsyncResult ar)
        {
            var _obj = ar.AsyncState as ClientService;

            try
            {
                _obj.NetworkSocket.EndConnect(ar);

                _obj?.NetworkAction?.NetworkStateChanged(NetworkState.Connected, new StateObject(this));

                return;
            }
            catch (Exception)
            {
            }

            _obj?.NetworkAction?.NetworkStateChanged(NetworkState.Error,new StateObject(this));
        }

      
    }
}
