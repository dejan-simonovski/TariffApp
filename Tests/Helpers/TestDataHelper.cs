using TariffApp.Models;
using TariffApp.Models.Enum;

namespace Tests.Helpers
{
    public static class TestDataHelper
    {
        public static Client CreateTestClient(
            Guid? clientId = null,
            string name = "Test Client",
            int creditScore = 500,
            ClientSegment segment = ClientSegment.Standard,
            bool isRisky = false)
        {
            return new Client
            {
                ClientId = clientId ?? Guid.NewGuid(),
                Name = name,
                CreditScore = creditScore,
                Segment = segment,
                IsRisky = isRisky
            };
        }

        public static Transaction CreateTestTransaction(
            Guid? transactionId = null,
            TransactionType type = TransactionType.POS,
            double amount = 100.0,
            Currency currency = Currency.EUR,
            Guid? senderId = null,
            Guid? receiverId = null)
        {
            return new Transaction
            {
                TransactionId = transactionId ?? Guid.NewGuid(),
                Type = type,
                Amount = amount,
                Currency = currency,
                SenderId = senderId ?? Guid.NewGuid(),
                ReceiverId = receiverId ?? Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                IsDomestic = true
            };
        }

        public static Configuration CreateTestConfiguration()
        {
            return new Configuration
            {
                Id = 1,
                DefaultCurrency = Currency.EUR,
                MaxFeeLimit = 10.0,
                PosFixedFee = 0.5,
                PosPercentFee = 0.02,
                CreditScoreDiscount = 700,
                LastUpdated = DateTime.UtcNow
            };
        }

        public static TransactionInputViewModel CreateTestTransactionInputViewModel(string jsonInput = "")
        {
            return new TransactionInputViewModel
            {
                JsonInput = jsonInput,
                Message = ""
            };
        }

        public static List<Transaction> CreateTestTransactionList()
        {
            return new List<Transaction>
            {
                CreateTestTransaction(
                    transactionId: Guid.NewGuid(),
                    type: TransactionType.POS,
                    amount: 100,
                    currency: Currency.EUR
                ),
                CreateTestTransaction(
                    transactionId: Guid.NewGuid(),
                    type: TransactionType.ECommerce,
                    amount: 200,
                    currency: Currency.USD
                )
            };
        }

        public static string CreateValidTransactionJson()
        {
            var transactions = new[]
            {
                new
                {
                    TransactionId = Guid.NewGuid(),
                    Type = "POS",
                    Amount = 100.0,
                    Currency = "EUR",
                    IsDomestic = true,
                    SenderId = Guid.NewGuid(),
                    ReceiverId = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow,
                    Sender = new
                    {
                        ClientId = Guid.NewGuid(),
                        Name = "Test Sender",
                        CreditScore = 600,
                        Segment = "Regular",
                        IsRisky = false
                    },
                    Receiver = new
                    {
                        ClientId = Guid.NewGuid(),
                        Name = "Test Receiver",
                        CreditScore = 700,
                        Segment = "Premium",
                        IsRisky = false
                    }
                }
            };

            return System.Text.Json.JsonSerializer.Serialize(transactions);
        }
    }
}
