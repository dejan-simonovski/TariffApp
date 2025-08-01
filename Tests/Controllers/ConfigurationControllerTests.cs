using Microsoft.AspNetCore.Mvc;
using TariffApp.Controllers;
using TariffApp.Data;
using TariffApp.Models;
using Tests.Helpers;

namespace Tests.Controllers
{
    public class ConfigurationControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ConfigurationController _controller;

        public ConfigurationControllerTests()
        {
            _context = DbContextHelper.CreateInMemoryContext().SeedWithTestData();
            _controller = new ConfigurationController(_context);
        }

        [Fact]
        public async Task Index_ReturnsViewWithConfigurations()
        {
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Configuration>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewWithConfiguration()
        {
            var result = await _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Configuration>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound()
        {
            var result = await _controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_RedirectsToHome()
        {
            var config = _context.Configuration.First();
            config.MaxFeeLimit = 15.0;

            var result = await _controller.Edit(1, config);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            var updatedConfig = _context.Configuration.Find(1);
            Assert.Equal(15.0, updatedConfig?.MaxFeeLimit);
        }
    }
}
