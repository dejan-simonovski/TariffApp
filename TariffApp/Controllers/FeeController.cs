using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TariffApp.Data;
using TariffApp.Models;
using TariffApp.Services;
using TariffApp.Services.Interfaces;

namespace TariffApp.Controllers
{
    public class FeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFeeCalculationService _feeCalculationService;

        public FeeController(ApplicationDbContext context, IFeeCalculationService feeCalculationService)
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
                    if (transaction.TransactionId != Guid.Empty &&
                        _context.Transactions.Any(t => t.TransactionId == transaction.TransactionId))
                    {
                        throw new InvalidOperationException($"Transaction with ID {transaction.TransactionId} already exists.");
                    }

                    if (transaction.TransactionId == Guid.Empty)
                    {
                        transaction.TransactionId = Guid.NewGuid();
                    }

                    if (transaction.Timestamp == default)
                        transaction.Timestamp = DateTime.UtcNow;

                    if (transaction.Sender != null)
                    {
                        if (transaction.Sender.ClientId != Guid.Empty &&
                            _context.Clients.Any(c => c.ClientId == transaction.Sender.ClientId))
                        {
                            throw new InvalidOperationException($"Sender with ID {transaction.Sender.ClientId} already exists.");
                        }

                        if (transaction.Sender.ClientId == Guid.Empty)
                        {
                            transaction.Sender.ClientId = Guid.NewGuid();
                        }

                        transaction.SenderId = transaction.Sender.ClientId;
                    }

                    if (transaction.Receiver != null)
                    {
                        if (transaction.Receiver.ClientId != Guid.Empty &&
                            _context.Clients.Any(c => c.ClientId == transaction.Receiver.ClientId))
                        {
                            throw new InvalidOperationException($"Receiver with ID {transaction.Receiver.ClientId} already exists.");
                        }

                        if (transaction.Receiver.ClientId == Guid.Empty)
                        {
                            transaction.Receiver.ClientId = Guid.NewGuid();
                        }

                        transaction.ReceiverId = transaction.Receiver.ClientId;
                    }

                    _feeCalculationService.ApplyFee(transaction);

                    _context.Transactions.Add(transaction);
                }

                _context.SaveChanges();

                model.Message = $"Saved {transactions.Count} transactions successfully.";
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"Validation error: {ex.Message}");
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
