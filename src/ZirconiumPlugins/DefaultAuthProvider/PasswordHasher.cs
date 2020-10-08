using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace DefaultAuthProvider
{
    public static class PasswordHasher
    {
        // First item is password hash, and second item is a salt
        public static Tuple<byte[], byte[]> CreatePasswordHash(string password)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            var salt = createSalt();
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 2; // four cores
            argon2.Iterations = 1;
            argon2.MemorySize = 64 * 1024; // 1 GB

            return new Tuple<byte[], byte[]>(argon2.GetBytes(32), salt);
        }

        private static byte[] _createPasswordHashWithCustomSalt(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 2; // four cores
            argon2.Iterations = 1;
            argon2.MemorySize = 64 * 1024; // 1 GB

            return argon2.GetBytes(32);
        }

        private static byte[] createSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        public static bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = _createPasswordHashWithCustomSalt(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }
}