using System;
using System.Collections.Generic;
using System.Text;
using TcpCommunication.Classes;

namespace TcpCommunication.Interfaces
{
    public interface IDbSource<T> where T : class
    {
        void Exists(T Object = null);
        void Select(T Object = null);
        void Insert(T Object = null);
        void Update(T Object = null);
        void Delete(T Object = null);
    }
}
