using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization; // <-- added
using TariffApp.Data;
using TariffApp.Models;
using TariffApp.Services;

namespace TariffApp.Controllers
{
    public class FeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FeeCalculationService _feeCalculationService;

        public FeeController(ApplicationDbContext context, FeeCalculationService feeCalculationService)
        {
            _context = context;
            _feeCalculationService = feeCalculationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new TransactionInputViewModel());
        }

        [HttpPost]
        public IActionResult Index(TransactionInputViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.JsonInput))
            {
                ModelState.AddModelError("", "JSON input cannot be empty.");
                return View(model);
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                var transactions = JsonSerializer.Deserialize<List<Transaction>>(model.JsonInput, options);

                if (transactions == null)
                {
                    ModelState.AddModelError("", "Failed to parse transactions.");
                    return View(model);
                }

                foreach (var transaction in transactions)
                {
                    _feeCalculationService.ApplyFee(transaction);
                    _context.Transactions.Add(transaction);
                }
                _context.SaveChanges();

                model.Message = $"Saved {transactions.Count} transactions successfully.";
                return View(model);
            }
            catch (JsonException ex)
            {
                ModelState.AddModelError("", $"Invalid JSON format: {ex.Message}");
                return View(model);
            }
        }
    }
}
