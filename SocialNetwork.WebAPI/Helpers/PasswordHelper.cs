using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace SocialNetwork.WebAPI.Helpers;

public static class PasswordHelper
{
    private const int MemorySizeKb = 65536;
    private const int Iterations = 4;
    private const int DegreeOfParallelism = 4;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    /// <summary>
    /// Hashes a plaintext password using Argon2id.
    /// </summary>
    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = MemorySizeKb,
            Iterations = Iterations,
            DegreeOfParallelism = DegreeOfParallelism
        };

        byte[] hash = argon2.GetBytes(HashSize);

        return $"$argon2id$v=19$m={MemorySizeKb},t={Iterations},p={DegreeOfParallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Verifies a plaintext password against a stored Argon2id hash string.
    /// </summary>
    public static bool VerifyPassword(string password, string storedHashString)
    {
        try
        {
            var parts = storedHashString.Split('$');
            if (parts.Length < 6 || parts[1] != "argon2id") return false;

            var @params = parts[3].Split(',');
            int memory = int.Parse(@params[0][2..]);
            int iterations = int.Parse(@params[1][2..]);
            int parallelism = int.Parse(@params[2][2..]);

            // Extract salt and hash bytes
            byte[] salt = Convert.FromBase64String(parts[4]);
            byte[] expectedHash = Convert.FromBase64String(parts[5]);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                MemorySize = memory,
                Iterations = iterations,
                DegreeOfParallelism = parallelism
            };

            byte[] actualHash = argon2.GetBytes(expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch
        {
            return false;
        }
    }
}