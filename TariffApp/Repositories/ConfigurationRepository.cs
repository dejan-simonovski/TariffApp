using TariffApp.Data;
using TariffApp.Models;
using TariffApp.Repositories.Interfaces;

namespace TariffApp.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly ApplicationDbContext _context;

        public ConfigurationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Configuration? GetSingleton()
        {
            return _context.Configuration.FirstOrDefault(c => c.Id == 1);
        }
    }
}
