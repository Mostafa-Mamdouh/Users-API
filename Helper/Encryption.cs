using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace UserAPI.Helper
{
    public class Encryption
    {
        
        public static string GenerateEmailId(string emailAddress)
        {

            string salt = "450d0b0db2bcf4adde5032eca1a7c416e560cf44";

            // Combine the email address and salt
            string combinedString = emailAddress + salt;

            // Convert the combined string to bytes
            byte[] combinedBytes = Encoding.UTF8.GetBytes(combinedString);

            // Compute the SHA1 hash
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(combinedBytes);

                // Convert the hash bytes to a hexadecimal string
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
    }
}

