using MenuApp.BLL.DTO.UserDTOs;
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

        [HttpPost("register_user")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO user)
        {
            var result = await _userService.RegisterUser(user);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("register_email")]
        public async Task<IActionResult> RegisterNewEmail(
            [FromBody] RegisterNewEmailDTO registerNewEmail
        )
        {
            var result = await _userService.RegisterNewEmail(registerNewEmail);
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

        [HttpPost("verify_email")]
        public async Task<IActionResult> VerifyEmail(EmailVerifyDTO emailVerify)
        {
            var result = await _userService.VerifyEmail(emailVerify);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("log_in")]
        public async Task<IActionResult> LogIn(LogInDTO logIn)
        {
            var result = await _userService.LogIn(logIn);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("log_out")]
        public async Task<IActionResult> LogOut(LogOutDTO logOut)
        {
            var result = await _userService.LogOut(logOut);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("send_verification_code_to_recover_password")]
        public async Task<IActionResult> SendVerificationCodeToRecoverPassword(
            SendVerificatonCodeToRecoverPasswordDTO sendVerificatonCodeToRecoverPassword
        )
        {
            var result = await _userService.SendVerificationCodeToRecoverPassword(
                sendVerificatonCodeToRecoverPassword
            );
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("verify_password_recover")]
        public async Task<IActionResult> VerifyPasswordRecover(
            VerifyPasswordRecoverDTO verifyPasswordRecover
        )
        {
            var result = await _userService.VerifyPasswordRecover(verifyPasswordRecover);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("set_new_password")]
        public async Task<IActionResult> SetNewPassword(RecoverPasswordDTO recoverPassword)
        {
            var result = await _userService.SetNewPassword(recoverPassword);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }

        [HttpPost("resend_email_confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(
            ResendEmailConfirmationDTO resendEmailConfirmation
        )
        {
            var result = await _userService.ResendEmailConfirmation(resendEmailConfirmation);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }
    }
}
