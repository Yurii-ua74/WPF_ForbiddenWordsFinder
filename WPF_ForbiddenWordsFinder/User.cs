using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ForbiddenWordsFinder
{
    class User
    {
        private string login, pass, telefon;
        public int id { get; set; }
        public string Login 
        { 
            get { return login; } 
            set { login = value; }
        }
        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }
        public string Telefon
        {
            get { return telefon; }
            set { telefon = value; }
        }

        public User() { }   

        public User(string login, string pass, string telefon)
        {           
            this.login = login;
            this.pass = pass;
            this.telefon = telefon;
        }
    }
}
