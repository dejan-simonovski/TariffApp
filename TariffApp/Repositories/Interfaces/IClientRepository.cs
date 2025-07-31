using TariffApp.Models;

namespace TariffApp.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Client? GetById(Guid id);
    }
}
