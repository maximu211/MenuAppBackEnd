using MenuApp.BLL.DTO;
using MenuApp.BLL.Services.MenuApp.BLL.Services;
using MenuApp.BLL.Utils;
using MenuApp.DAL.Models;
using MenuApp.DAL.Repositories;
using MongoDB.Bson;

namespace MenuApp.BLL.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResult> RegisterUser(RegisterDTO user);
        Task<ServiceResult> RefreshToken(RefreshTokenDTO refreshToken);
    }

    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IGenerateJwtToken _jwtTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUsersRepository userRepository,
            IGenerateJwtToken jwtTokenGenerator,
            IPasswordHasher passwordHasher
        )
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResult> RefreshToken(RefreshTokenDTO refreshToken)
        {
            var user = await _userRepository.GetUserByRefreshToken(refreshToken.RefreshToken);

            if (user == null)
            {
                return new ServiceResult(false, "No such refresh token in DB");
            }

            string newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id.ToString());
            user.RefreshToken = newRefreshToken;

            string newAccessToken = _jwtTokenGenerator.GenerateNewJwtToken(user.Id.ToString());

            await _userRepository.UpdateUserRefreshToken(user);

            return new ServiceResult(
                true,
                "Token successfully refreshed",
                new { AccessToken = newAccessToken, RefreshToken = newRefreshToken }
            );
        }

        public async Task<ServiceResult> RegisterUser(RegisterDTO user)
        {
            if (user.RepeatePassword != user.Password)
            {
                return new ServiceResult(false, "Passwords do not match ");
            }

            var existingUser = await _userRepository.GetUserByEmailOrUsername(
                user.Username,
                user.Email
            );

            if (existingUser != null)
            {
                return new ServiceResult(false, "User with this Email or username already exists");
            }

            Users registredUser = new Users
            {
                Email = user.Email,
                Username = user.Username,
                Password = user.Password
            };

            registredUser.Password = _passwordHasher.HashPassword(user.Password);

            string refreshToken = _jwtTokenGenerator.GenerateRefreshToken(
                registredUser.Id.ToString()
            );

            registredUser.RefreshToken = refreshToken;

            string accessToken = _jwtTokenGenerator.GenerateNewJwtToken(
                registredUser.Id.ToString()
            );

            await _userRepository.AddUser(registredUser);

            return new ServiceResult(
                true,
                "User registered successfully",
                new { AccessToken = accessToken, RefreshToken = refreshToken }
            );
        }
    }
}
