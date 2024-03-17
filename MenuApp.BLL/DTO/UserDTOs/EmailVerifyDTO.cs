using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO.UserDTOs
{
    public class EmailVerifyDTO
    {
        public required string VerificationCode { get; set; }
        public required string Token { get; set; }
    }
}
