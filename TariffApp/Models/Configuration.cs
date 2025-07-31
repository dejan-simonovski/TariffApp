using TariffApp.Models.Enum;

namespace TariffApp.Models
{
    public class Configuration
    {
        public int Id { get; set; } = 1;
        public Currency DefaultCurrency { get; set; } = Currency.EUR;
        public double MaxFeeLimit { get; set; } = 120.0;
        public double PosFixedFee { get; set; } = 0.20;
        public double PosPercentFee { get; set; } = 0.002;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

}
