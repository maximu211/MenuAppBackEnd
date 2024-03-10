using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO
{
    public class EmailVerifyDTO
    {
        public required string verificationCode { get; set; }
        public required string token { get; set; }
    }
}
