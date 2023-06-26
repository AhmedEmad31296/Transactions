using System.Security.Cryptography;

namespace webapi.Heplers
{
    public static class EncryptionKeyGenerator
    {
        public static string GenerateEncryptionKey()
        {
            byte[] keyBytes = new byte[16]; 
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }

            // Convert the byte array to a base64 string
            string encryptionKey = Convert.ToBase64String(keyBytes);

            return encryptionKey;
        }
    }
}
