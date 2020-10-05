using System;
using System.Collections.Generic;
using System.Text;
using TcpCommunication.Classes;

namespace TcpCommunication.Interfaces
{
    public interface IDbSource<T> where T : class
    {
        bool Exists(T Object = null);
        T Select(T Object = null);
        T Insert(T Object = null);
        bool Update(T Object = null);
        bool Delete(T Object = null);
    }
}
