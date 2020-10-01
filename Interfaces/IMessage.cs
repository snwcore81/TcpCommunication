using System;
using System.Collections.Generic;
using System.Text;
using TcpCommunication.Classes;

namespace TcpCommunication.Interfaces
{
    public interface IMessage
    {
        IMessage ProcessRequest(StateObject Object = null);
        IMessage ProcessResponse(StateObject Object = null);

        NetworkData AsNetworkData(int a_iDataSize = NetworkService.BUFFER_SIZE);
    }
}
