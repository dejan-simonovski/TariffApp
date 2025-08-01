using Moq;
using TariffApp.Models;
using TariffApp.Models.Enum;
using TariffApp.Repositories.Interfaces;
using TariffApp.Services;
using Tests.Helpers;

namespace TariffApp.Tests.Services
{
    public class FeeCalculationServiceTests
    {
        private readonly Mock<IConfigurationRepository> _mockConfigRepo;
        private readonly Mock<IClientRepository> _mockClientRepo;
        private readonly FeeCalculationService _service;
        private readonly Configuration _testConfig;

        public FeeCalculationServiceTests()
        {
            _mockConfigRepo = new Mock<IConfigurationRepository>();
            _mockClientRepo = new Mock<IClientRepository>();
            _service = new FeeCalculationService(_mockConfigRepo.Object, _mockClientRepo.Object);
            _testConfig = TestDataHelper.CreateTestConfiguration();
        }

        [Fact]
        public void ApplyFee_WithNoConfiguration_ThrowsException()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns((Configuration?)null);
            var transaction = TestDataHelper.CreateTestTransaction();

            var exception = Assert.Throws<InvalidOperationException>(() => _service.ApplyFee(transaction));
            Assert.Equal("Missing Configuration with ID = 1", exception.Message);
        }

        [Theory]
        [InlineData(50, 0.5)]
        [InlineData(100, 0.5)]
        public void ApplyFee_POSTransaction_SmallAmount_AppliesFixedFee(double amount, double expectedFee)
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var transaction = TestDataHelper.CreateTestTransaction(type: TransactionType.POS, amount: amount);

            _service.ApplyFee(transaction);

            Assert.Equal(expectedFee, transaction.Provision);
            Assert.Contains("#1: POS fixed fee", transaction.Metadata);
        }

        [Fact]
        public void ApplyFee_POSTransaction_LargeAmount_AppliesPercentageFee()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var transaction = TestDataHelper.CreateTestTransaction(type: TransactionType.POS, amount: 200);

            _service.ApplyFee(transaction);

            Assert.Equal(4.0, transaction.Provision);
            Assert.Contains("#1: POS percentage fee", transaction.Metadata);
        }

        [Fact]
        public void ApplyFee_ECommerceTransaction_WithoutCap_AppliesStandardFee()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var transaction = TestDataHelper.CreateTestTransaction(type: TransactionType.ECommerce, amount: 100);

            _service.ApplyFee(transaction);

            Assert.Equal(1.95, transaction.Provision);
            Assert.Contains("#2: E-Commerce fee without", transaction.Metadata);
        }

        [Fact]
        public void ApplyFee_ECommerceTransaction_WithCap_AppliesMaxFee()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var transaction = TestDataHelper.CreateTestTransaction(type: TransactionType.ECommerce, amount: 1000);

            _service.ApplyFee(transaction);

            Assert.Equal(_testConfig.MaxFeeLimit, transaction.Provision);
            Assert.Contains("#2: E-Commerce fee with cap", transaction.Metadata);
        }

        [Fact]
        public void ApplyFee_NonDefaultCurrency_AddsExtraFee()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var transaction = TestDataHelper.CreateTestTransaction(
                type: TransactionType.POS,
                amount: 50,
                currency: Currency.USD);

            _service.ApplyFee(transaction);

            Assert.Equal(1.5, transaction.Provision);
            Assert.Contains("#4: 2% fee for non-default currency", transaction.Metadata);
        }

        [Fact]
        public void ApplyFee_RiskyClient_AddsFixedFee()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var riskyClient = TestDataHelper.CreateTestClient(isRisky: true);
            _mockClientRepo.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(riskyClient);

            var transaction = TestDataHelper.CreateTestTransaction(
                type: TransactionType.POS,
                amount: 50,
                senderId: riskyClient.ClientId);

            _service.ApplyFee(transaction);

            Assert.Equal(5.5, transaction.Provision);
            Assert.Contains("#5: Fixed €5 fee for risky client", transaction.Metadata);
        }

        [Fact]
        public void ApplyFee_PremiumClientWithHighCreditScore_WaivesFee()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var premiumClient = TestDataHelper.CreateTestClient(
                segment: ClientSegment.Premium,
                creditScore: 800);
            _mockClientRepo.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(premiumClient);

            var transaction = TestDataHelper.CreateTestTransaction(senderId: premiumClient.ClientId);

            _service.ApplyFee(transaction);

            Assert.Equal(0, transaction.Provision);
            Assert.Equal("#6: Fee waived for premium high credit score client", transaction.Metadata);
        }

        [Fact]
        public void ApplyFee_HighCreditScore_AppliesDiscount()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var highCreditClient = TestDataHelper.CreateTestClient(creditScore: 800);
            _mockClientRepo.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(highCreditClient);

            var transaction = TestDataHelper.CreateTestTransaction(
                type: TransactionType.POS,
                amount: 200,
                senderId: highCreditClient.ClientId);

            _service.ApplyFee(transaction);

            Assert.Equal(3.96, transaction.Provision);
            Assert.Contains("#3: 1% discount for high credit score", transaction.Metadata);
        }

        [Fact]
        public void ApplyFee_NegativeProvision_SetsToZero()
        {
            _mockConfigRepo.Setup(x => x.GetSingleton()).Returns(_testConfig);
            var transaction = TestDataHelper.CreateTestTransaction(amount: 0);

            _service.ApplyFee(transaction);

            Assert.True(transaction.Provision >= 0);
        }
    }
}
