using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TcpCommunication.Classes
{
    [DataContract]
    public class Response
    {
        [DataMember]
        public int ResponseCode { get; private set; }
        [DataMember]
        public object ResponseObject { get; private set; }

        public Response(int ResponseCode, object ResponseObject)
        {
            this.ResponseCode = ResponseCode;
            this.ResponseObject = ResponseObject;
        }

        public override string ToString()
        {
            return $"[Kod={ResponseCode}|Obiekt={ResponseObject}]";
        }
    }
}
