using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO.UserDTO
{
    public class UsersDTO
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public string? Image { get; set; }
    }
}
