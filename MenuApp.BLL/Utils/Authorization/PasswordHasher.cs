using System;
using System.Security.Cryptography;
using System.Text;

namespace MenuApp.BLL.Utils.Authorization
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        public bool VerifyPassword(string password, string hasedPassword);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        public string HashPassword(string password)
        {
            using (
                var algorithm = new Rfc2898DeriveBytes(
                    password,
                    SaltSize,
                    Iterations,
                    HashAlgorithmName.SHA256
                )
            )
            {
                byte[] key = algorithm.GetBytes(KeySize);
                byte[] salt = algorithm.Salt;

                byte[] hash = new byte[SaltSize + KeySize];
                Array.Copy(salt, 0, hash, 0, SaltSize);
                Array.Copy(key, 0, hash, SaltSize, KeySize);

                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using (
                var algorithm = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA256
                )
            )
            {
                byte[] key = algorithm.GetBytes(KeySize);

                for (int i = 0; i < KeySize; i++)
                {
                    if (hashBytes[i + SaltSize] != key[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
