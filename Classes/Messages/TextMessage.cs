using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using TcpCommunication.Classes.Services;
using TcpCommunication.Interfaces;

namespace TcpCommunication.Classes.Messages
{
    [DataContract]
    public class TextMessage : XmlStorage<TextMessage>, IMessage
    {
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public string To { get; set; }
        [DataMember] 
        public string Text { get; set; }

        public TextMessage()
        {
            From = To = Text = string.Empty;
        }

        public override bool InitializeFromObject(TextMessage Object)
        {
            From = Object.From;
            To = Object.To;
            Text = Object.Text;

            return true;
        }
        public IMessage ProcessRequest(StateObject Object = null)
        {
            var _client = Object.GetObject<ClientService>();

            if (_client.HasRegisteredServer)
            {
                var _server = _client.GetRegisteredServer<ServerService<ClientService>>();

                if (To == "*")
                {
                    _server.AsyncSendBroadcast(AsNetworkData(),_client);
                }
                else
                {
                    _server.GetClientByIdentifier(To)?.AsyncSend(AsNetworkData());
                }
            }

            return null;
        }

        public IMessage ProcessResponse(StateObject Object = null)
        {
            Console.WriteLine(this);

            return this;
        }
        public NetworkData AsNetworkData(int a_iBufferSize = 100000)
        {
            return new NetworkData(a_iBufferSize)
            {
                Buffer = ToXml().ToArray()
            };
        }
        public override string ToString()
        {
            return $"Od:{From}|Do:{To}|Wiadomosc={Text}";
        }
    }
}
