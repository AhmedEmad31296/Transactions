using System.ComponentModel.DataAnnotations;

namespace webapi
{
    public class EncryptedAuthRequest
    {
        public string EncryptedData { get; set; }
        public string EncryptionKey { get; set; }
    }
    public class AuthRequest
    {
        [Required]
        public long CardNo { get; set; }
        [Required]
        public int Password { get; set; }
    }
}
