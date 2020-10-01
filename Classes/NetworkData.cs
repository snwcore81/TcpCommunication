using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpCommunication.Classes.Exceptions;

namespace TcpCommunication.Classes
{
    public class NetworkData
    {
        private readonly byte[] m_oBuffer;

        public NetworkData(int a_iBufferSize)
        {
            m_oBuffer = new byte[a_iBufferSize];
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

        public byte[] BufferWithData => Buffer.Take(DataLength()).ToArray();
    }
}
