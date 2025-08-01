using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Text.Json;
using TariffApp.Controllers;
using TariffApp.Data;
using TariffApp.Models;
using TariffApp.Models.Enum;
using TariffApp.Services.Interfaces;
using Tests.Helpers;

namespace Tests.Controllers
{
    public class FeeControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IFeeCalculationService> _mockFeeService;
        private readonly FeeController _controller;

        public FeeControllerTests()
        {
            _context = DbContextHelper.CreateInMemoryContext().SeedWithTestData();
            _mockFeeService = new Mock<IFeeCalculationService>();
            _controller = new FeeController(_context, _mockFeeService.Object);

            _controller.TempData = new TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void Index_Post_WithEmptyJsonInput_ReturnsViewWithError()
        {
            var model = TestDataHelper.CreateTestTransactionInputViewModel("");

            var result = _controller.Index(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Contains("JSON input cannot be empty.",
                _controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        [Fact]
        public void Index_Post_WithInvalidJson_ReturnsViewWithError()
        {
            var model = TestDataHelper.CreateTestTransactionInputViewModel("invalid json");

            var result = _controller.Index(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Contains("Invalid JSON format:",
                _controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).First());
        }


        [Fact]
        public void Index_Post_WithDuplicateTransactionId_ReturnsError()
        {
            var existingTransaction = TestDataHelper.CreateTestTransaction();
            _context.Transactions.Add(existingTransaction);
            _context.SaveChanges();

            var duplicateJson = JsonSerializer.Serialize(new[]
            {
                new
                {
                    TransactionId = existingTransaction.TransactionId,
                    Type = "POS",
                    Amount = 100.0,
                    Currency = "EUR",
                    IsDomestic = true,
                    SenderId = Guid.NewGuid(),
                    ReceiverId = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow
                }
            });

            var model = TestDataHelper.CreateTestTransactionInputViewModel(duplicateJson);

            var result = _controller.Index(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Contains($"Transaction with ID {existingTransaction.TransactionId} already exists.",
                _controller.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).First());
        }


        [Fact]
        public void Index_Post_WithMultipleTransactions_ProcessesAll()
        {
            var multipleTransactionsJson = JsonSerializer.Serialize(new[]
            {
                new
                {
                    TransactionId = Guid.NewGuid(),
                    Type = "POS",
                    Amount = 100.0,
                    Currency = "EUR",
                    IsDomestic = true,
                    SenderId = Guid.NewGuid(),
                    ReceiverId = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow
                },
                new
                {
                    TransactionId = Guid.NewGuid(),
                    Type = "ECommerce",
                    Amount = 200.0,
                    Currency = "USD",
                    IsDomestic = false,
                    SenderId = Guid.NewGuid(),
                    ReceiverId = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow
                }
            });

            var model = TestDataHelper.CreateTestTransactionInputViewModel(multipleTransactionsJson);

            _mockFeeService.Setup(x => x.ApplyFee(It.IsAny<Transaction>()))
                          .Callback<Transaction>(t => t.Provision = 3.0);

            var result = _controller.Index(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultModel = Assert.IsType<TransactionInputViewModel>(viewResult.Model);
            Assert.Contains("Saved 2 transactions successfully.", resultModel.Message);

            _mockFeeService.Verify(x => x.ApplyFee(It.IsAny<Transaction>()), Times.Exactly(2));

            Assert.Equal(2, _context.Transactions.Count(t => t.Provision == 3.0));
        }
    }
}

