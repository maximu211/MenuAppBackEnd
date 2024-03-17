using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO.UserDTOs
{
    public class LogInDTO
    {
        public required string Password { get; set; }
        public required string Username { get; set; }
    }
}
