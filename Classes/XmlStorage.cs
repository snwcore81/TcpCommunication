using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace TcpCommunication.Classes
{
    [DataContract]
    public abstract class XmlStorage<T> where T : class
    {
        [IgnoreDataMember]
        public T BaseObject { get; protected set; }

        public virtual Stream ToXml()
        {
            DataContractSerializer _oSerializer = new DataContractSerializer(typeof(T));

            using var _oStream = new MemoryStream();

            using var _oWriter = XmlDictionaryWriter.CreateTextWriter(_oStream, Encoding.UTF8);
            
            _oSerializer.WriteObject(_oWriter, this);

            return _oStream;
        }

        public virtual bool FromXml(Stream a_oStream)
        {
            DataContractSerializer _oSerializer = new DataContractSerializer(typeof(T));

            using var _oReader = XmlDictionaryReader.CreateTextReader(a_oStream, new XmlDictionaryReaderQuotas());

            return InitializeFromObject((T)_oSerializer.ReadObject(_oReader, false));
        }

        public abstract bool InitializeFromObject(T Object); 
    }
}
