using TariffApp.Data;
using TariffApp.Repositories;
using Tests.Helpers;

namespace Tests.Repositories
{
    public class ConfigurationRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ConfigurationRepository _repository;

        public ConfigurationRepositoryTests()
        {
            _context = DbContextHelper.CreateInMemoryContext().SeedWithTestData();
            _repository = new ConfigurationRepository(_context);
        }

        [Fact]
        public void GetSingleton_ReturnsConfigurationWithIdOne()
        {
            var result = _repository.GetSingleton();

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void GetSingleton_WithNoConfiguration_ReturnsNull()
        {
            using var emptyContext = DbContextHelper.CreateInMemoryContext("empty");
            var emptyRepository = new ConfigurationRepository(emptyContext);

            var result = emptyRepository.GetSingleton();

            Assert.Null(result);
        }
    }
}

