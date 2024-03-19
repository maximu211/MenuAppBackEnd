using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.BLL.Services.MenuApp.BLL.Services;
using MenuApp.BLL.Utils.Authorization;
using MenuApp.BLL.Utils.Email;
using MenuApp.DAL.Models;
using MenuApp.DAL.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Serilog;

namespace MenuApp.BLL.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResult> RegisterUser(RegisterUserDTO user);
        Task<ServiceResult> RefreshToken(RefreshTokenDTO refreshToken);
        Task<ServiceResult> VerifyEmail(EmailVerifyDTO emailVerify);
        Task<ServiceResult> LogIn(LogInDTO logIn);
        Task<ServiceResult> LogOut(LogOutDTO logOut);
        Task<ServiceResult> UpdateEmailAndSendCode(
            UpdateEmailAndSendCodeDTO updateEmailAndSendCode
        );
        Task<ServiceResult> SendVerificationCodeToRecoverPassword(
            SendVerificatonCodeToRecoverPasswordDTO sendVerificatonCodeToRecoverPassword
        );
        Task<ServiceResult> VerifyPasswordRecover(VerifyPasswordRecoverDTO verifyPasswordRecover);
        Task<ServiceResult> SetNewPassword(RecoverPasswordDTO recoverPassword);
        Task<ServiceResult> RegisterNewEmail(RegisterNewEmailDTO newEmail);
        Task<ServiceResult> ResendConfirmationCode(
            ResendConfirmationCodeDTO resendConfirmationCode
        );
    }

    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IGenerateJwtToken _jwtTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailSender _emailSender;
        private readonly IConfirmationCodesRepository _confirmationCodesRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUsersRepository userRepository,
            IGenerateJwtToken jwtTokenGenerator,
            IPasswordHasher passwordHasher,
            IEmailSender emailSender,
            IConfirmationCodesRepository confirmationCodesRepository,
            ILogger<UserService> logger
        )
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
            _confirmationCodesRepository = confirmationCodesRepository;
            _logger = logger;
        }

        public async Task<ServiceResult> RefreshToken(RefreshTokenDTO refreshToken)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshToken(refreshToken.RefreshToken);

                if (user == null)
                {
                    _logger.LogInformation(
                        "No user found with refresh token: {RefreshToken}",
                        refreshToken.RefreshToken
                    );
                    return new ServiceResult(false, "No such refresh token in DB");
                }

                string newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken(
                    user.Id.ToString()
                );
                string newAccessToken = _jwtTokenGenerator.GenerateNewJwtToken(user.Id.ToString());

                await _userRepository.UpdateUserRefreshTokenByUserId(
                    user.Id,
                    refreshToken.RefreshToken
                );

                _logger.LogInformation(
                    "Token successfully refreshed for user with ID: {UserId}",
                    user.Id
                );
                return new ServiceResult(
                    true,
                    "Token successfully refreshed",
                    new { AccessToken = newAccessToken, RefreshToken = newRefreshToken }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing token");
                return new ServiceResult(false, "An error occurred while refreshing token");
            }
        }

        public async Task<ServiceResult> VerifyEmail(EmailVerifyDTO emailVerify)
        {
            try
            {
                bool isTokenValid = _jwtTokenGenerator.IsJwtTokenValid(emailVerify.Token);
                if (!isTokenValid)
                {
                    _logger.LogWarning("Token is not valid for email verification");
                    return new ServiceResult(false, "Token is not valid");
                }

                ObjectId userId = _jwtTokenGenerator.GetUserIdFromJwtToken(emailVerify.Token);
                var code = await _confirmationCodesRepository.GetConfirmationCodeByUserId(userId);

                if (code == emailVerify.VerificationCode)
                {
                    await _userRepository.SubmitUserEmail(userId);
                    _logger.LogInformation(
                        "Email successfully verified for user with ID: {UserId}",
                        userId
                    );
                    return new ServiceResult(true, "Email successfully verified");
                }
                else
                {
                    _logger.LogWarning(
                        "Verification code does not match for user with ID: {UserId}",
                        userId
                    );
                    return new ServiceResult(false, "Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying email");
                return new ServiceResult(
                    false,
                    $"An error occurred while verifying email: {ex.Message}"
                );
            }
        }

        public async Task<ServiceResult> LogIn(LogInDTO logIn)
        {
            try
            {
                var user = await _userRepository.GetUserByUsername(logIn.Username);

                if (user == null || !_passwordHasher.VerifyPassword(logIn.Password, user.Password))
                {
                    _logger.LogWarning(
                        "User with username {Username} or password does not exist",
                        logIn.Username
                    );
                    return new ServiceResult(
                        false,
                        "User with such name or password does not exist"
                    );
                }

                string accessToken = _jwtTokenGenerator.GenerateNewJwtToken(user.Id.ToString());
                string refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id.ToString());
                await _userRepository.UpdateUserRefreshTokenByUserId(user.Id, refreshToken);

                if (!user.IsEmailSubmited)
                {
                    _logger.LogWarning(
                        "Email is not verified for user with username: {Username}",
                        logIn.Username
                    );

                    string emailConfirmationCode = _emailSender.GenerateConfirmationCode();

                    ConfirmationCodes confirmationCode = new ConfirmationCodes
                    {
                        UserId = user.Id,
                        ConfirmationCode = emailConfirmationCode,
                    };

                    await _confirmationCodesRepository.UpsertConfirmationCode(confirmationCode);

                    try
                    {
                        string emailMessage = EmailTemplate.GenerateEmail(
                            user.Username,
                            emailConfirmationCode
                        );
                        string emailSubject = "Verify your Email";

                        await _emailSender.SendEmail(user.Email, emailSubject, emailMessage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error sending email for user with username: {Username}",
                            logIn.Username
                        );
                        return new ServiceResult(false, $"Error sending email: {ex.Message}");
                    }

                    return new ServiceResult(
                        false,
                        "Email is not verified",
                        new { IsEmailSubmited = false, UserEmail = user.Email }
                    );
                }

                _logger.LogInformation("User {Username} successfully logged in", logIn.Username);

                return new ServiceResult(
                    true,
                    "User successfuly log in",
                    new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        IsEmailSubmited = true
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred during login for user with username: {Username}",
                    logIn.Username
                );
                return new ServiceResult(false, $"An error occurred during login: {ex.Message}");
            }
        }

        public async Task<ServiceResult> LogOut(LogOutDTO logOut)
        {
            try
            {
                bool isTokenValid = _jwtTokenGenerator.IsJwtTokenValid(logOut.Token);
                if (!isTokenValid)
                {
                    _logger.LogWarning("Token is not valid for logout");
                    return new ServiceResult(false, "Token is not valid");
                }

                ObjectId userId = _jwtTokenGenerator.GetUserIdFromJwtToken(logOut.Token);

                await _userRepository.DeleteRefreshTokenByUserId(userId);

                _logger.LogInformation("User successfully logged out with ID: {UserId}", userId);
                return new ServiceResult(true, "User successfully log out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout");
                return new ServiceResult(false, $"An error occurred during logout: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateEmailAndSendCode(
            UpdateEmailAndSendCodeDTO updateEmailAndSendCode
        )
        {
            bool isTokenValid = _jwtTokenGenerator.IsJwtTokenValid(updateEmailAndSendCode.Token);
            if (!isTokenValid)
                return new ServiceResult(false, "Token is not valid");

            ObjectId userId = _jwtTokenGenerator.GetUserIdFromJwtToken(
                updateEmailAndSendCode.Token
            );

            await _userRepository.UpdateUserEmail(userId, updateEmailAndSendCode.NewEmail);

            string emailConfirmationCode = _emailSender.GenerateConfirmationCode();

            ConfirmationCodes confirmationCode = new ConfirmationCodes
            {
                UserId = userId,
                ConfirmationCode = emailConfirmationCode,
            };

            await _confirmationCodesRepository.UpsertConfirmationCode(confirmationCode);

            try
            {
                string emailMessage = EmailTemplate.GenerateEmail(emailConfirmationCode);

                string emailSubject = "Welcome to our application";

                await _emailSender.SendEmail(
                    updateEmailAndSendCode.NewEmail,
                    emailSubject,
                    emailMessage
                );
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, $"Error sending email: {ex.Message}");
            }

            return new ServiceResult(true, "Email successfuly updated");
        }

        public async Task<ServiceResult> SendVerificationCodeToRecoverPassword(
            SendVerificatonCodeToRecoverPasswordDTO sendVerificatonCodeToRecoverPassword
        )
        {
            var user = await _userRepository.GetUserByUsername(
                sendVerificatonCodeToRecoverPassword.Username
            );

            string emailConfirmationCode = _emailSender.GenerateConfirmationCode();

            ConfirmationCodes confirmationCode = new ConfirmationCodes
            {
                UserId = user.Id,
                ConfirmationCode = emailConfirmationCode,
            };

            await _confirmationCodesRepository.UpsertConfirmationCode(confirmationCode);

            try
            {
                string emailMessage = EmailTemplate.GenerateEmail(
                    user.Username,
                    emailConfirmationCode
                );

                string emailSubject = "Recovering password";

                await _emailSender.SendEmail(user.Email, emailSubject, emailMessage);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, $"Error sending email: {ex.Message}");
            }

            var accesToken = _jwtTokenGenerator.GenerateNewJwtToken(user.Id.ToString());

            return new ServiceResult(
                true,
                "A confirmation code has been sent to your email",
                new { IsUserFound = true, AccesToken = accesToken }
            );
        }

        public async Task<ServiceResult> VerifyPasswordRecover(
            VerifyPasswordRecoverDTO verifyPasswordRecover
        )
        {
            bool isTokenValid = _jwtTokenGenerator.IsJwtTokenValid(verifyPasswordRecover.Token);
            if (!isTokenValid)
                return new ServiceResult(false, "Token is not valid");

            var userId = _jwtTokenGenerator.GetUserIdFromJwtToken(verifyPasswordRecover.Token);
            var code = await _confirmationCodesRepository.GetConfirmationCodeByUserId(userId);

            return code == verifyPasswordRecover.VerificationCode
                ? new ServiceResult(true, "User successfuly verified", new { UserVerified = true })
                : new ServiceResult(false, "no such code exists", new { UserVerified = false });
        }

        public async Task<ServiceResult> SetNewPassword(RecoverPasswordDTO recoverPassword)
        {
            bool isTokenValid = _jwtTokenGenerator.IsJwtTokenValid(recoverPassword.Token);
            if (!isTokenValid)
                return new ServiceResult(false, "Token is not valid");

            var userId = _jwtTokenGenerator.GetUserIdFromJwtToken(recoverPassword.Token);

            if (
                !(recoverPassword.Password == recoverPassword.RepeatPassword)
                || (recoverPassword.Password.Length & recoverPassword.RepeatPassword.Length) < 8
            )
            {
                string hashedPassword = _passwordHasher.HashPassword(recoverPassword.Password);
                await _userRepository.SetNewPassword(userId, hashedPassword);
                return new ServiceResult(true, "New password succesfuly setted");
            }
            return new ServiceResult(false, "Error in setting new password");
        }

        public async Task<ServiceResult> RegisterNewEmail(RegisterNewEmailDTO registerNewEmail)
        {
            try
            {
                if (await _userRepository.IsUserExistsByEmail(registerNewEmail.Email))
                {
                    return new ServiceResult(false, "User with this email already registered");
                }

                Users newUser = new Users
                {
                    Password = string.Empty,
                    Email = registerNewEmail.Email,
                    Username = string.Empty,
                };

                await _userRepository.AddUser(newUser);

                var accessToken = _jwtTokenGenerator.GenerateNewJwtToken(newUser.Id.ToString());

                string emailConfirmationCode = _emailSender.GenerateConfirmationCode();

                ConfirmationCodes confirmationCode = new ConfirmationCodes
                {
                    UserId = newUser.Id,
                    ConfirmationCode = emailConfirmationCode,
                };

                await _confirmationCodesRepository.UpsertConfirmationCode(confirmationCode);

                try
                {
                    string emailMessage = EmailTemplate.GenerateEmail(
                        newUser.Username,
                        emailConfirmationCode
                    );
                    string emailSubject = "Welcome to our application";

                    await _emailSender.SendEmail(newUser.Email, emailSubject, emailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error sending email for user with email: {Email}",
                        newUser.Email
                    );
                    return new ServiceResult(false, $"Error sending email: {ex.Message}");
                }

                return new ServiceResult(
                    true,
                    "A confirmation email has been sent to the user",
                    new { AccessToken = accessToken }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error registering user by email: {Email}",
                    registerNewEmail.Email
                );
                return new ServiceResult(false, $"Error registering user by email: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RegisterUser(RegisterUserDTO registerUser)
        {
            try
            {
                if (!_jwtTokenGenerator.IsJwtTokenValid(registerUser.Token))
                    return new ServiceResult(false, "Token is not valid");

                if (registerUser.Password != registerUser.RepeatePassword)
                    return new ServiceResult(false, "Passwords are not same");

                if ((registerUser.Password.Length & registerUser.RepeatePassword.Length) < 8)
                    return new ServiceResult(false, "Password must be longer than 8 characters");

                if (await _userRepository.IsUserExistsByUsername(registerUser.Username))
                    return new ServiceResult(false, "User with this username already exist");

                ObjectId userId = _jwtTokenGenerator.GetUserIdFromJwtToken(registerUser.Token);

                await _userRepository.SetUsernameAndPassword(
                    userId,
                    registerUser.Username,
                    _passwordHasher.HashPassword(registerUser.Password)
                );

                await _userRepository.SetRefreshTokenById(
                    userId,
                    _jwtTokenGenerator.GenerateRefreshToken(userId.ToString())
                );

                return new ServiceResult(
                    true,
                    "User registred successfuly",
                    new { IsUserRegistredsuccesfuly = true }
                );
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, $"Error registering user by email: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ResendConfirmationCode(
            ResendConfirmationCodeDTO resendConfirmationCode
        )
        {
            if (!_jwtTokenGenerator.IsJwtTokenValid(resendConfirmationCode.Token))
                return new ServiceResult(false, "Token is not valid");

            ObjectId userId = _jwtTokenGenerator.GetUserIdFromJwtToken(
                resendConfirmationCode.Token
            );

            var user = await _userRepository.GetUserEmailByUserId(userId);

            string emailConfirmationCode = _emailSender.GenerateConfirmationCode();

            try
            {
                string emailMessage = EmailTemplate.GenerateEmail(
                    user.Username,
                    emailConfirmationCode
                );

                string emailSubject = "Verify email";

                await _emailSender.SendEmail(user.Email, emailSubject, emailMessage);
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, $"Error sending email: {ex.Message}");
            }

            return new ServiceResult(true, "A confirmation code has been sent to your email");
        }
    }
}
