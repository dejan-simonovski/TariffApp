using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using TariffApp.Controllers;
using TariffApp.Data;
using TariffApp.Models;
using TariffApp.Models.Enum;
using TariffApp.Services.Interfaces;
using Tests.Helpers;

namespace Tests.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IFeeCalculationService> _mockFeeService;
        private readonly TransactionsController _controller;

        public TransactionsControllerTests()
        {
            _context = DbContextHelper.CreateInMemoryContext().SeedWithTestData();
            _mockFeeService = new Mock<IFeeCalculationService>();
            _controller = new TransactionsController(_context, _mockFeeService.Object);

            _controller.TempData = new TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task Index_ReturnsViewWithTransactionsIncludingSenderAndReceiver()
        {
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Transaction>>(viewResult.Model);
            Assert.Single(model);

            var transaction = model.First();
            Assert.NotNull(transaction.Sender);
            Assert.NotNull(transaction.Receiver);
        }

        [Fact]
        public async Task Create_Post_AppliesFeeAndRedirectsToIndex()
        {
            var sender = _context.Clients.First();
            var receiver = _context.Clients.Skip(1).First();

            var transaction = TestDataHelper.CreateTestTransaction(
                type: TransactionType.POS,
                amount: 150,
                senderId: sender.ClientId,
                receiverId: receiver.ClientId);

            _mockFeeService.Setup(x => x.ApplyFee(It.IsAny<Transaction>()))
                          .Callback<Transaction>(t =>
                          {
                              t.Provision = 2.5;
                              t.Metadata = "Test fee applied";
                          });

            var result = await _controller.Create(transaction);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockFeeService.Verify(x => x.ApplyFee(It.IsAny<Transaction>()), Times.Once);

            var savedTransaction = _context.Transactions.FirstOrDefault(t => t.Provision == 2.5);
            Assert.NotNull(savedTransaction);
            Assert.Equal("Test fee applied", savedTransaction.Metadata);
        }


        [Fact]
        public async Task Edit_Get_ReturnsViewWithTransactionAndSelectLists()
        {
            var transaction = _context.Transactions.First();

            var result = await _controller.Edit(transaction.TransactionId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Transaction>(viewResult.Model);
            Assert.Equal(transaction.TransactionId, model.TransactionId);

            Assert.NotNull(_controller.ViewBag.Currency);
            Assert.NotNull(_controller.ViewBag.TransactionType);
            Assert.NotNull(_controller.ViewData["ReceiverId"]);
            Assert.NotNull(_controller.ViewData["SenderId"]);
        }

        [Fact]
        public async Task Edit_Post_AppliesFeeAndRedirectsToIndex()
        {
            var transaction = _context.Transactions.First();
            transaction.Amount = 250;

            _mockFeeService.Setup(x => x.ApplyFee(It.IsAny<Transaction>()))
                          .Callback<Transaction>(t =>
                          {
                              t.Provision = 5.0;
                              t.Metadata = "Updated fee applied";
                          });

            var result = await _controller.Edit(transaction.TransactionId, transaction);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockFeeService.Verify(x => x.ApplyFee(It.IsAny<Transaction>()), Times.Once);

            var updatedTransaction = _context.Transactions.Find(transaction.TransactionId);
            Assert.NotNull(updatedTransaction);
            Assert.Equal(250, updatedTransaction.Amount);
            Assert.Equal(5.0, updatedTransaction.Provision);
            Assert.Equal("Updated fee applied", updatedTransaction.Metadata);
        }


        [Fact]
        public async Task Delete_Get_ReturnsViewWithTransactionIncludingSenderAndReceiver()
        {
            var transaction = _context.Transactions.Include(t => t.Sender).Include(t => t.Receiver).First();

            var result = await _controller.Delete(transaction.TransactionId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Transaction>(viewResult.Model);
            Assert.Equal(transaction.TransactionId, model.TransactionId);
            Assert.NotNull(model.Sender);
            Assert.NotNull(model.Receiver);
        }


        [Fact]
        public async Task DeleteConfirmed_DeletesTransactionAndRedirectsToIndex()
        {
            var transactionToDelete = TestDataHelper.CreateTestTransaction();
            _context.Transactions.Add(transactionToDelete);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(transactionToDelete.TransactionId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var deletedTransaction = await _context.Transactions.FindAsync(transactionToDelete.TransactionId);
            Assert.Null(deletedTransaction);
        }
    }
}
