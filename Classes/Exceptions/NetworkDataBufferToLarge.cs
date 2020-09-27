using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunication.Classes.Exceptions
{
    public class NetworkDataBufferToLarge : Exception
    {
        public NetworkDataBufferToLarge(string a_sBufferName, int a_iMaxLength) : 
            base($"Dane ze zrodla <{a_sBufferName ?? ""}> sa zbyt duze! Maksymalny rozmiar: {a_iMaxLength} ")
        {
             
        }
    }
}
