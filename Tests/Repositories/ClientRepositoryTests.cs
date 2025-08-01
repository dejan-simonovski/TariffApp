using TariffApp.Data;
using TariffApp.Repositories;
using Tests.Helpers;

namespace TariffApp.Tests.Repositories
{
    public class ClientRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ClientRepository _repository;

        public ClientRepositoryTests()
        {
            _context = DbContextHelper.CreateInMemoryContext().SeedWithTestData();
            _repository = new ClientRepository(_context);
        }

        [Fact]
        public void GetById_WithValidId_ReturnsClient()
        {
            var existingClient = _context.Clients.First();

            var result = _repository.GetById(existingClient.ClientId);

            Assert.NotNull(result);
            Assert.Equal(existingClient.ClientId, result.ClientId);
            Assert.Equal(existingClient.Name, result.Name);
        }

        [Fact]
        public void GetById_WithInvalidId_ReturnsNull()
        {
            var result = _repository.GetById(Guid.NewGuid());

            Assert.Null(result);
        }
    }
}
