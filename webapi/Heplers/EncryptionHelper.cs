using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace webapi.Heplers
{
    public class EncryptionHelper
    {

        public static string EncryptData(string data, string encryptionKey)
        {
            try
            {
                byte[] key = Encoding.UTF8.GetBytes(encryptionKey);
                byte[] iv;

                using (AesManaged aes = new AesManaged())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    iv = aes.IV;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;

                    using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

                        // Combine IV and encrypted data
                        byte[] encryptedData = new byte[iv.Length + encryptedBytes.Length];
                        Array.Copy(iv, encryptedData, iv.Length);
                        Array.Copy(encryptedBytes, 0, encryptedData, iv.Length, encryptedBytes.Length);

                        // Convert to Base64 string
                        string encryptedBase64 = Convert.ToBase64String(encryptedData);
                        return encryptedBase64;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption failed. Error: " + ex.Message);
            }
        }


    }
}
