using Microsoft.AspNetCore.DataProtection.KeyManagement;

using Newtonsoft.Json;

using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;


namespace webapi.Heplers
{
   
    public static class DecryptionHelper
    {

        public static string DecryptData(string encryptedData,string encryptionKey)
        {
            try
            {

                byte[] key = Encoding.UTF8.GetBytes(encryptionKey);
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

                // Extract the IV from the encrypted data
                byte[] iv = new byte[16];
                Array.Copy(encryptedBytes, 0, iv, 0, 16);

                // Extract the actual encrypted message
                byte[] encryptedMessage = new byte[encryptedBytes.Length - 16];
                Array.Copy(encryptedBytes, 16, encryptedMessage, 0, encryptedBytes.Length - 16);

                string decryptedData;

                using (AesManaged aes = new AesManaged())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedMessage, 0, encryptedMessage.Length);
                        decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    }
                }

                return decryptedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption failed. Error: " + ex.Message);
            }
        }

    }
    
}
