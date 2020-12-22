using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using TcpCommunication.Classes.System;

namespace TcpCommunication.Classes.Business
{
    [DataContract]
    public class User : XmlStorage<User>
    {
        [DataMember]
        public string Login { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public int Permission { get; set; }

        public override bool InitializeFromObject(User Object)
        {
            this.Login = Object.Login;
            this.Password = Object.Password;
            this.Permission = Object.Permission;

            return true;
        }

        public void SaveAsXml(string a_sFileName)
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

        public static User LoadFromXml(string a_sFileName)
        {
            using var _log = Log.DEB("User", "LoadFromXml");

            User _oUser = null;

            try
            {
                _log.PR_DEB($"próba odczytu użytkownika z pliku <{a_sFileName}>...");

                using (var _file = new StreamReader(a_sFileName))
                {
                    var _sStrBuff = _file.ReadToEnd();

                    var _oBuffer = Encoding.UTF8.GetBytes(_sStrBuff);

                    _oUser = new User();
                    
                    _oUser.FromXml(new MemoryStream(_oBuffer));

                }
            }
            catch (Exception e)
            {
                _log.PR_DEB($"Błąd! {e.Message}");
            }

            return _oUser;
        }

        public static User Add()
        {
            User _oUser = new User();

            do
            {
                Console.Write("Podaj login:");

                _oUser.Login = Console.ReadLine().ToLower();

                if (string.IsNullOrEmpty(_oUser.Login))
                {
                    Console.WriteLine("Login nie może być pusty!");
                }
                else
                    break;
            }
            while (true);

            do
            {
                Console.Write("Podaj uprawnienia (1-2):");

                if (int.TryParse(Console.ReadLine(), out int _iPermission) && _iPermission >= 1 && _iPermission <=2)
                {
                    _oUser.Permission = _iPermission;
                    break;
                }
                else
                {
                    Console.WriteLine("Błędnie podane uprawnienia!");
                }
            }
            while (true);

            do
            {
                Console.Write("Wprowadź hasło:");
                _oUser.Password = Console.ReadLine();

                if (string.IsNullOrEmpty(_oUser.Password))
                {
                    Console.WriteLine("Hasło nie może być puste!");
                }
                else
                {                 
                    break;
                }
            }
            while (true);

            return _oUser;
        }

        public override string ToString()
        {
            return $"Login={Login}|Password={Password}|Permission={Permission}";
        }
    }
}
