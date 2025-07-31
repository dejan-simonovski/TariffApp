using TariffApp.Models;
using TariffApp.Models.Enum;
using TariffApp.Services.Interfaces;
using TariffApp.Repositories.Interfaces;

namespace TariffApp.Services
{
    public class FeeCalculationService : IFeeCalculationService
    {
        private readonly IConfigurationRepository _configRepo;
        private readonly IClientRepository _clientRepo;

        public FeeCalculationService(IConfigurationRepository configRepo, IClientRepository clientRepo)
        {
            _configRepo = configRepo;
            _clientRepo = clientRepo;
        }

        public void ApplyFee(Transaction transaction)
        {
            var config = _configRepo.GetSingleton();
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
                        appliedRule = "#1: POS percentage fee";  // This could be too much. No upper limit stated.
                    }
                    break;

                case TransactionType.ECommerce:
                    provision = (transaction.Amount * 0.018) + 0.15;
                    appliedRule = "#2: E-Commerce fee without";
                    if (provision > config.MaxFeeLimit) {
                        provision = config.MaxFeeLimit;
                        appliedRule = "#2: E-Commerce fee with cap";
                    }
                    break;

                default:
                    appliedRule = "Default rule (no fee applied)";
                    break;
            }

            // Additional fees
            if (transaction.Currency != config.DefaultCurrency)
            {
                var extraFee = transaction.Amount * 0.02;
                provision += extraFee;
                appliedRule += " + #4: 2% fee for non-default currency";
            }

            var sender = _clientRepo.GetById(transaction.SenderId);
            if (sender != null)
            {
                if (sender.IsRisky)
                {
                    provision += 5.0;
                    appliedRule += " + #5: Fixed €5 fee for risky client";
                }

                if (sender.Segment == ClientSegment.Premium && sender.CreditScore > config.CreditScoreDiscount)
                {
                    provision = 0;
                    appliedRule = "#6: Fee waived for premium high credit score client";
                }
            }

            if (sender != null && sender.CreditScore > config.CreditScoreDiscount)
            {
                var discount = provision * 0.01;
                provision -= discount;
                if(provision != 0)
                    appliedRule += " + #3: 1% discount for high credit score";
            }

            if (provision < 0)
                provision = 0;
            transaction.Provision = Math.Round(provision, 2);
            transaction.Metadata = appliedRule;
        }
    }
}
