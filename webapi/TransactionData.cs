using System.ComponentModel.DataAnnotations;

namespace webapi
{
    public class TransactionData
    {
        public virtual string ProcessingCode { get; set; }
        public virtual int SystemTraceNr { get; set; }
        public virtual int FunctionCode { get; set; }
        public virtual long CardNo { get; set; }
        public virtual string CardHolder { get; set; }
        public virtual decimal AmountTrxn { get; set; }
        public virtual int CurrencyCode { get; set; }

    }
}
