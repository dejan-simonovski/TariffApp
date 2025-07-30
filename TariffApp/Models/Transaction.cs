using TariffApp.Models.Enum;

namespace TariffApp.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public TransactionType Type { get; set; } = TransactionType.DomesticTransfer; // default
        public double Amount { get; set; }
        public string Currency { get; set; } = "EUR";
        public bool IsDomestic { get; set; }
        public double Provision { get; set; }
        public Guid SenderId { get; set; }
        public Client? Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public Client? Receiver { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
