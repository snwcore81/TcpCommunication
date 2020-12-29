using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using TcpCommunication.Classes.System;

namespace TcpCommunication.Classes.Business
{
    [DataContract]
    public class UserList : XmlStorage<UserList>
    {
        [DataMember]
        private List<User> UserCollection { get; set; }

        public UserList()
        {
            UserCollection = new List<User>();
        }

        public override bool InitializeFromObject(UserList Object)
        {
            this.UserCollection = new List<User>(Object.UserCollection);

            return true;
        }

        public virtual void SaveAsXml(string a_sFileName)
        {
            using var _log = Log.DEB(this, "SaveAsXml");

            try
            {
                using (var _file = new StreamWriter(a_sFileName))
                {
                    var _sStrBuff = Encoding.UTF8.GetString(ToXml().ToArray());

                    _log.PR_DEB(_sStrBuff);

                    _file.Write(_sStrBuff);
                }
            }
            catch (Exception e)
            {
                _log.PR_DEB($"Błąd! {e.Message}");
            }
        }

        public virtual void LoadFromXml(string a_sFileName)
        {
            using var _log = Log.DEB(this, "LoadFromXml");

            try
            {
                _log.PR_DEB($"próba odczytu użytkowników z pliku <{a_sFileName}>...");

                using (var _file = new StreamReader(a_sFileName))
                {
                    var _sStrBuff = _file.ReadToEnd();

                    var _oBuffer = Encoding.UTF8.GetBytes(_sStrBuff);

                    var _oUserList = new UserList();

                    _oUserList.FromXml(new MemoryStream(_oBuffer));

                    this.InitializeFromObject(_oUserList);

                }
            }
            catch (Exception e)
            {
                _log.PR_DEB($"Błąd! {e.Message}");
            }
        }

        public virtual User Add(User a_oUser)
        {
            UserCollection.Add(a_oUser);

            return a_oUser;
        }
        public virtual User Add() => Add(User.Add());
        public virtual User Get(int a_iIndex) => UserCollection[a_iIndex];
        public virtual int Count => UserCollection.Count;

        public List<User> Collection => UserCollection;
    }
}
