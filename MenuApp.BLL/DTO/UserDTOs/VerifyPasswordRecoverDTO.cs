﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO.UserDTOs
{
    public class VerifyPasswordRecoverDTO
    {
        public required string VerificationCode { get; set; }
        public required string Token { get; set; }
    }
}
