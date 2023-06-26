namespace webapi
{
    public static class ProcessTransactionLogic
    {
        public static TransactionResponse ProcessTransaction(TransactionData data)
        {
            TransactionResponse transactionResponse = new();

            if (IsValidTransaction(data))
            {
                if (IsCardValid(data.CardNo))
                {
                    decimal currentBalance = GetCardBalance(data.CardNo);

                    if (currentBalance >= data.AmountTrxn)
                    {
                        decimal updatedBalance = currentBalance - data.AmountTrxn;

                        // Update the card balance
                        UpdateCardBalance(data.CardNo, updatedBalance);

                        // Generate 6 digit approval code for the transaction
                        string approvalCode = new Random().Next(100000, 999999).ToString();

                        transactionResponse.ResponseCode = "00";
                        transactionResponse.Message = "Success";
                        transactionResponse.ApprovalCode = approvalCode;
                        transactionResponse.DateTime = DateTime.Now.ToString();
                    }
                    else
                    {
                        transactionResponse.ResponseCode = "01";
                        transactionResponse.Message = "Insufficient balance";
                    }
                }
                else
                {
                    transactionResponse.ResponseCode = "02";
                    transactionResponse.Message = "Invalid card";
                }
            }
            else
            {
                transactionResponse.ResponseCode = "03";
                transactionResponse.Message = "Invalid transaction data";
            }

            return transactionResponse;
        }
        private static bool IsValidTransaction(TransactionData data)
        {
            // Check this data
            if (data.ProcessingCode == "999000" &&
                data.SystemTraceNr > 0 &&
                data.FunctionCode == 1324 &&
                data.AmountTrxn > 0 &&
                data.CurrencyCode > 0 &&
                data.CardNo == 4712345601012222 &&
                !string.IsNullOrEmpty(data.CardHolder))
            {
                return true;
            }

            return false;
        }
        private static bool IsCardValid(long cardNumber)
        {
            bool isValid = false;

            if (CheckCardNumberPattern(cardNumber))
            {
                if (IsCardActive(cardNumber))
                {
                    if (!IsCardExpired(cardNumber))
                    {
                        isValid = true;
                    }
                }
            }

            return isValid;
        }
        private static bool CheckCardNumberPattern(long cardNumber)
        {
            // make the logic to Check Card Number Pattern 
            bool isValid = true;
            return isValid;
        }
        private static bool IsCardActive(long cardNumber)
        {
            // make the logic to Check Card Activation
            bool isActive = true;
            return isActive;
        }
        private static bool IsCardExpired(long cardNumber)
        {
            // make the logic to Check Card Expired
            bool isExpired = false;
            return isExpired;
        }
        private static decimal GetCardBalance(long cardNumber)
        {
            // get the card balance from database

            decimal balance = 1000000;
            return balance;
        }
        private static void UpdateCardBalance(long cardNumber, decimal newBalance)
        {
            // get the client card by cardNumber
            // update the card balance
        }

    }
}
