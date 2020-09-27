using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunication.Interfaces
{
    public interface IMessage<T1> where T1 : class  
    {
        T1 ProcessRequest();
        T1 ProcessResponse();
    }
}
