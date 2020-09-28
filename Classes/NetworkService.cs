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
            get => m_oBuffer;
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
        public const int BUFFER_SIZE = 1000000;

        public enum ModeEnum
        {
            Client = 0x001,
            Server = 0x002
        }        
        public string               Identifier { get; set; }
        public ModeEnum             Mode { get; protected set; }
        public INetworkAction       NetworkAction { get; set; }
        public IPAddress            Address { get; protected set; }
        public int                  Port { get; protected set; }
        public NetworkData          Data { get; protected set; }
        public NetworkService       HostedBy { get; set; }

        public NetworkService(ModeEnum Mode, int a_iBufferSize = BUFFER_SIZE)
        {
            this.Identifier = GetHashCode().ToString("X8");
            this.Mode = Mode;
            this.Data = new NetworkData(a_iBufferSize);
            this.NetworkAction = null;
            this.HostedBy = null;
        }
        public NetworkService(ModeEnum Mode, IPAddress Address, int Port, int a_iBufferSize = BUFFER_SIZE) : 
            this(Mode,a_iBufferSize)
        {
            this.Address = Address;
            this.Port = Port;
        }
        public abstract bool IsConnected { get; }
        public abstract Socket NetworkSocket { get; }
        public abstract void Establish();
        public virtual void FireSend(NetworkData a_oData)
        {
            try
            {
                NetworkAction?.StateChanged(State.Sending, new StateObject(this,a_oData));

                NetworkSocket?.BeginSend(a_oData.Buffer, 0, a_oData.DataLength(true), SocketFlags.None, new AsyncCallback(SendCallback), this);
            }
            catch (Exception)
            {
                NetworkAction?.StateChanged(State.Error,new StateObject(this));
            }
        }

        public virtual void FireReceive()
        {
            Data?.Clear();

            try
            {
                NetworkAction?.StateChanged(State.Receiving, new StateObject(this));

                NetworkSocket?.BeginReceive(Data.Buffer, 0, Data.BufferLength, SocketFlags.None, new AsyncCallback(ReceiveCallback), this);
            }
            catch (Exception)
            {
                NetworkAction?.StateChanged(State.Error,new StateObject(this));
            }
        }

        protected virtual void SendCallback(IAsyncResult ar)
        {
            NetworkService _obj = ar.AsyncState as NetworkService;

            try
            {
                if (_obj.NetworkSocket.EndSend(ar) > 0)
                {
                    _obj.NetworkAction?.StateChanged(State.Sent,new StateObject(this));

                    return;
                }                
            }
            catch (Exception)
            {
            }

            _obj.NetworkAction?.StateChanged(State.Error);
        }

        protected virtual void ReceiveCallback(IAsyncResult ar)
        {
            NetworkService _obj = ar.AsyncState as NetworkService;

            try
            {
                int _iSize = _obj.NetworkSocket.EndReceive(ar);

                if (_iSize > 0 && (_obj.Data?.HasAnyData ?? false))
                {
                    _iSize = _obj.Data.DataLength();
                    byte[] _oBuffer = _obj.Data.Buffer.Take(_iSize).ToArray();

                    _obj.NetworkAction?.StateChanged(State.Received,new StateObject(this, _oBuffer));

                    _obj.FireReceive();

                    return;
                }
            }
            catch (Exception)
            {               
            }

            _obj.NetworkAction?.StateChanged(State.Error,new StateObject(this));
        }
        public override string ToString() => $"Identifier={Identifier}[{GetType().Name.CleanType()}={NetworkSocket?.LocalEndPoint}]";
        public virtual bool IsHostedBy => HostedBy != null;
        public virtual T GetHostedBy<T>() where T : NetworkService
        {
            return (T)HostedBy;
        }
    }
}
