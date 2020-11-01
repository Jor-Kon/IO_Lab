using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTCPLibrary
{
    /// <summary>
    /// Klasa obiektu typu Account
    /// </summary>
    public class Account
    {
        string login;
        string password;

        public Account(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get => login; set => login = value; }
        public string Password { get => password; set => password = value; }
    }
}
