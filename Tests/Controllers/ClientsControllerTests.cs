using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffApp.Controllers;
using TariffApp.Data;
using TariffApp.Models;
using Tests.Helpers;

namespace Tests.Controllers
{
    public class ClientsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ClientsController _controller;

        public ClientsControllerTests()
        {
            _context = DbContextHelper.CreateInMemoryContext().SeedWithTestData();
            _controller = new ClientsController(_context);

            _controller.TempData = new TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task Index_ReturnsViewWithAllClients()
        {
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Client>>(viewResult.Model);
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public async Task Details_ReturnsViewWithClient()
        {
            var client = _context.Clients.First();

            var result = await _controller.Details(client.ClientId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Client>(viewResult.Model);
            Assert.Equal(client.ClientId, model.ClientId);
        }


        [Fact]
        public void Create_Get_ReturnsViewWithSegmentList()
        {
            var result = _controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(_controller.ViewBag.SegmentList);
        }

        [Fact]
        public async Task Create_Post_RedirectsToIndex()
        {

            var client = TestDataHelper.CreateTestClient(name: "New Client");

            var result = await _controller.Create(client);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var addedClient = _context.Clients.FirstOrDefault(c => c.Name == "New Client");
            Assert.NotNull(addedClient);
        }


        [Fact]
        public async Task Edit_Get_ReturnsViewWithClient()
        {
            var client = _context.Clients.First();

            var result = await _controller.Edit(client.ClientId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Client>(viewResult.Model);
            Assert.Equal(client.ClientId, model.ClientId);
        }

        [Fact]
        public async Task Edit_Post_RedirectsToIndex()
        {
            var client = _context.Clients.First();
            client.Name = "Updated Name";

            var result = await _controller.Edit(client.ClientId, client);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var updatedClient = _context.Clients.Find(client.ClientId);
            Assert.Equal("Updated Name", updatedClient?.Name);
        }


        [Fact]
        public async Task Delete_Get_ReturnsViewWithClient()
        {
            var client = _context.Clients.First();

            var result = await _controller.Delete(client.ClientId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Client>(viewResult.Model);
            Assert.Equal(client.ClientId, model.ClientId);
        }

        [Fact]
        public async Task DeleteConfirmed_WithClientHavingTransactions_RedirectsToDeleteWithError()
        {
            var clientWithTransaction = _context.Clients.First(c =>
                _context.Transactions.Any(t => t.SenderId == c.ClientId || t.ReceiverId == c.ClientId));

            var result = await _controller.DeleteConfirmed(clientWithTransaction.ClientId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Delete", redirectResult.ActionName);
            Assert.Equal("Cannot delete client who has transactions.", _controller.TempData["DeleteError"]);
        }

        [Fact]
        public async Task DeleteConfirmed_WithClientWithoutTransactions_DeletesAndRedirectsToIndex()
        {
            var clientWithoutTransaction = TestDataHelper.CreateTestClient(name: "Client to Delete");
            _context.Clients.Add(clientWithoutTransaction);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(clientWithoutTransaction.ClientId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var deletedClient = _context.Clients.Find(clientWithoutTransaction.ClientId);
            Assert.Null(deletedClient);
        }
    }
}

