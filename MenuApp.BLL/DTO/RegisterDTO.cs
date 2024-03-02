using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO
{
    public class RegisterDTO
    {
        public required string Password { get; set; }
        public required string RepeatePassword { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
