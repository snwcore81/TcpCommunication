using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using TcpCommunication.Interfaces;

namespace TcpCommunication.Classes.Messages
{
    [DataContract]
    public class LoginMessage : XmlStorage<LoginMessage>, IMessage<LoginMessage>
    {
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public Response Response { get; private set; }

        [IgnoreDataMember]
        public int UserId { get => Response?.ResponseCode ?? -1; }
        public LoginMessage() : base()
        {
            Login = Password = string.Empty;
            Response = null;
        }
        public override bool InitializeFromObject(LoginMessage Object)
        {
            Login = Object.Login;
            Password = Object.Password;
            Response = Object.Response;

            return true;
        }

        public LoginMessage ProcessRequest()
        {
            if (Login.Equals("jacek") && Password.Equals("34jacek12"))
                Response = new Response(0, "Login complete");
            else
                Response = new Response(-1, new Exception("Błędny login lub hasło"));

            return this;
        }

        public LoginMessage ProcessResponse()
        {
            return this;                   
        }

        public override string ToString()
        {
            return $"[Login={Login}|Password={Password}|UserId={UserId}|Response={Response}]";
        }
    }
}
