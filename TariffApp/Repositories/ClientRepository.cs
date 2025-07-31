using TariffApp.Data;
using TariffApp.Models;
using TariffApp.Repositories.Interfaces;

namespace TariffApp.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Client? GetById(Guid id)
        {
            return _context.Clients.FirstOrDefault(c => c.ClientId == id);
        }
    }
}
