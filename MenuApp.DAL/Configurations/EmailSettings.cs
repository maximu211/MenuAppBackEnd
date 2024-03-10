using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.DAL.Configurations
{
    public class EmailSettings
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public EmailSettings() { }

        public EmailSettings(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
