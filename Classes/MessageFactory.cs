using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TcpCommunication.Classes.Exceptions;
using TcpCommunication.Interfaces;

namespace TcpCommunication.Classes
{
    public sealed class MessageFactory
    {
        public static readonly MessageFactory Instance = new MessageFactory();

        private Dictionary<string, Type> m_oMessages;

        private MessageFactory()
        {
            m_oMessages = new Dictionary<string, Type>();
        }
        public void Register<T>() where T : class
        {
            Type _Type = typeof(T);
            string _sTypeName = _Type.Name;

            if (!(_Type.GetInterfaces()?.ToList()?.Contains(typeof(IMessage<T>)) ?? false))
                throw new MessageFactoryIfaceNotFound(_sTypeName);

            if (!m_oMessages.ContainsKey(_sTypeName))
            {
                m_oMessages.Add(_sTypeName, _Type);
            }
        }
        public T Create<T>(string a_sTypeName) where T : class
        {
            if (m_oMessages.ContainsKey(a_sTypeName))
            {
                return (T)Activator.CreateInstance(m_oMessages[a_sTypeName]);
            }
            throw new MessageFactoryTypeNotFound(a_sTypeName);
        }

        public dynamic Create(string a_sTypeName) => Create<dynamic>(a_sTypeName);
    }
}
