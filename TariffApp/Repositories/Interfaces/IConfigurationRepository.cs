using TariffApp.Models;

namespace TariffApp.Repositories.Interfaces
{
    public interface IConfigurationRepository
    {
        Configuration? GetSingleton();
    }
}
