

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Security.Cryptography.Xml;

using webapi.Heplers;

namespace webapi.Controllers
{
    [ApiController]
    public class TransactionController : ControllerBase
    {
        [HttpPost]
        [Route("api/transaction/generateEncryptionKey")]
        public IActionResult GenerateEncryptionKey()
        {
            // Generate Encryption key
            string encryptionKey = EncryptionKeyGenerator.GenerateEncryptionKey();

            return Ok(new { encryptionKey });
        }

        [System.Web.Http.Authorize]
        [HttpPost]
        [Route("api/transaction/processTransaction")]
        public IActionResult ProcessTransaction([FromBody] EncryptedTransactionRequest input)
        {
            // Decrypted transaction request
            string decryptedData = DecryptionHelper.DecryptData(input.EncryptedData, input.EncryptionKey);
           var unescapedData = decryptedData.Replace("\\\"", "\"");
            // Remove the leading and trailing double quotes
            var unescapedDataTrimed = unescapedData.Trim('"');

            TransactionData _transactionData = JsonConvert.DeserializeObject<TransactionData>(unescapedDataTrimed);

            // Process transaction business logic
            TransactionResponse transactionResponse = ProcessTransactionLogic.ProcessTransaction(_transactionData);
            if (transactionResponse.ResponseCode == "00")
            {
                // Encrypted transaction response
                string encryptedResponse = EncryptionHelper.EncryptData(JsonConvert.SerializeObject(transactionResponse), input.EncryptionKey);

                return Ok(new { encryptedResponse });

            }
            else
                return BadRequest(new { transactionResponse.Message });
        }

    }
}
