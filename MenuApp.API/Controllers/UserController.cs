﻿using MenuApp.BLL.DTO;
using MenuApp.BLL.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace MenuApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO user)
        {
            var result = await _userService.RegisterUser(user);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("refresh_token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO refreshToken)
        {
            var result = await _userService.RefreshToken(refreshToken);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }
    }
}