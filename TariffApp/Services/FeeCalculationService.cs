using TariffApp.Data;
using TariffApp.Models;
using TariffApp.Models.Enum;
using TariffApp.Services.Interfaces;

namespace TariffApp.Services
{
    public class FeeCalculationService : IFeeCalculationService
    {
        private readonly ApplicationDbContext _context;

        public FeeCalculationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ApplyFee(Transaction transaction)
        {
            var config = _context.Configuration.FirstOrDefault(c => c.Id == 1);
            if (config == null)
                throw new InvalidOperationException("Missing Configuration with ID = 1");

            double provision = 0.0;
            string appliedRule = "";

            switch (transaction.Type)
            {
                case TransactionType.POS:
                    if (transaction.Amount <= 100)
                    {
                        provision = config.PosFixedFee;
                        appliedRule = "#1: POS fixed fee";
                    }
                    else
                    {
                        provision = transaction.Amount * config.PosPercentFee;
                        appliedRule = "#1: POS percentage fee";
                    }
                    break;

                case TransactionType.ECommerce:
                    provision = (transaction.Amount * 0.018) + 0.15;
                    if (provision > config.MaxFeeLimit)
                        provision = config.MaxFeeLimit;
                    appliedRule = "#2: E-Commerce fee with cap";
                    break;

                default:
                    appliedRule = "Default rule (no fee applied)";
                    break;
            }

            var sender = _context.Clients.FirstOrDefault(c => c.ClientId == transaction.SenderId);
            if (sender != null && sender.CreditScore > 400)
            {
                var discount = provision * 0.01;
                provision -= discount;
                appliedRule += " + #3: 1% discount for high credit score";
            }

            transaction.Provision = Math.Round(provision, 2);
            transaction.Metadata = appliedRule;
        }

    }
}
