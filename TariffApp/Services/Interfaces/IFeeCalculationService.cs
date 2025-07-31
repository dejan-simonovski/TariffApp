using TariffApp.Models;

namespace TariffApp.Services.Interfaces
{
    public interface IFeeCalculationService
    {
        void ApplyFee(Transaction transaction);
    }
}
