using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO.UserDTOs
{
    public class ResendConfirmationCodeDTO
    {
        public required string Token { get; set; }
    }
}
