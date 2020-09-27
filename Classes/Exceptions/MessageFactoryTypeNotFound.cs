using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunication.Classes.Exceptions
{
    public class MessageFactoryTypeNotFound : Exception
    {
        public MessageFactoryTypeNotFound(string a_sTypeName) :
            base($"Nie odnaleziono w fabryce typu <{a_sTypeName}>!")
        {
        }
    }
}
