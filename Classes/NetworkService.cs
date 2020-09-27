using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpCommunication.Classes.Exceptions;
using TcpCommunication.Interfaces;

using static TcpCommunication.Interfaces.INetworkAction;

namespace TcpCommunication.Classes
{
    public class NetworkData
    {
        private readonly byte[] m_oBuffer;

        public NetworkData(int a_iBufferSize)
        {
            m_oBuffer = new byte[a_iBufferSize] ;
            Clear();
        }
        public void Clear()
        {
            Array.Fill<byte>(m_oBuffer, 0);
        }
        public int BufferLength => m_oBuffer?.Length ?? -1;
        public int DataLength(bool a_bWithZero = false)
        {
            return m_oBuffer.ToList().FindIndex(x => x == 0) + (a_bWithZero ? 1 : 0);
        }
        public bool HasAnyData => DataLength() > 0;        
        public byte[] Buffer
        {
            get
            {
                if (HasAnyData)
                {
                    return m_oBuffer;
                }

                throw new NetworkDataBufferIsEmpty("Główny");
            }
            set
            {
                if ((value?.Length ?? -1) < 1)
                    throw new NetworkDataBufferIsEmpty("Wejściowy");

                if (value.Length > BufferLength)
                    throw new NetworkDataBufferToLarge("Wejśćiowy", BufferLength);

                Clear();

                Array.Copy(value, m_oBuffer, value.Length);
            }
        }
    }

    public abstract class NetworkService
    {
        public enum ModeEnum
        {
            Client = 0x001,
            Server = 0x002
        }        
        public ModeEnum             Mode { get; protected set; }
        public INetworkAction       NetworkAction { get; set; }
        public IPAddress            Address { get; protected set; }
        public int                  Port { get; protected set; }
        public NetworkService(ModeEnum Mode, IPAddress Address, int Port)
        {
            this.Mode = Mode;
            this.NetworkAction = null;
            this.Address = Address;
            this.Port = Port;
        }

        public abstract bool IsConnected { get; }
        public abstract Socket NetworkSocket { get; }
        public virtual void FireSend(NetworkData a_oData)
        {
            NetworkSocket?.BeginSend(a_oData.Buffer, 0, a_oData.DataLength(true), SocketFlags.None, new AsyncCallback(SendCallback), this);
        }

        public virtual void FireReceive()
        {
            //NetworkSocket?.BeginReceive()
        }

        protected virtual void SendCallback(IAsyncResult ar)
        {
            NetworkService _obj = ar.AsyncState as NetworkService;

            if (_obj.NetworkSocket.EndSend(ar) > 0)
            {
                _obj.NetworkAction?.StateChanged(State.Sent);
            }
            else
            {
                _obj.NetworkAction?.StateChanged(State.Error);
            }                   
        }
        
    }
}
