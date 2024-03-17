using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.Utils.Email
{
    public class EmailTemplate
    {
        public static string GenerateEmail(string username, string confirmationCode)
        {
            string emailMessage =
                $"Dear {username},\n\n"
                + "Thank you for registering in our application.\n\n"
                + $"Your confirmation code {confirmationCode}";

            return emailMessage;
        }

        public static string GenerateEmail(string confirmationCode)
        {
            string emailMessage =
                "Thank you for registering in our application.\n\n"
                + $"Your confirmation code {confirmationCode}";

            return emailMessage;
        }
    }
}
