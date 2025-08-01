using Microsoft.EntityFrameworkCore;
using TariffApp.Data;
using TariffApp.Models.Enum;

namespace Tests.Helpers
{
    public static class DbContextHelper
    {
        public static ApplicationDbContext CreateInMemoryContext(string databaseName = "")
        {
            if (string.IsNullOrEmpty(databaseName))
                databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new ApplicationDbContext(options);
        }

        public static ApplicationDbContext SeedWithTestData(this ApplicationDbContext context)
        {
            var config = TestDataHelper.CreateTestConfiguration();
            context.Configuration.Add(config);

            var client1 = TestDataHelper.CreateTestClient(name: "Regular Client", creditScore: 600);
            var client2 = TestDataHelper.CreateTestClient(name: "Premium Client", creditScore: 800, segment: ClientSegment.Premium);
            var client3 = TestDataHelper.CreateTestClient(name: "Risky Client", isRisky: true);

            context.Clients.AddRange(client1, client2, client3);

            var transaction = TestDataHelper.CreateTestTransaction(senderId: client1.ClientId, receiverId: client2.ClientId);
            context.Transactions.Add(transaction);

            context.SaveChanges();
            return context;
        }
    }
}
