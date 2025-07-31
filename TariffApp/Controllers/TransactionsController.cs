using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TariffApp.Data;
using TariffApp.Models;
using TariffApp.Models.Enum;
using TariffApp.Services.Interfaces;

namespace TariffApp.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFeeCalculationService _feeCalculationService;

        public TransactionsController(ApplicationDbContext context, IFeeCalculationService feeCalculationService)
        {
            _context = context;
            _feeCalculationService = feeCalculationService;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Receiver).Include(t => t.Sender);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Receiver)
                .Include(t => t.Sender)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewBag.Currency = new SelectList(Enum.GetValues(typeof(Currency)));
            ViewBag.TransactionType = new SelectList(Enum.GetValues(typeof(TransactionType)));
            ViewData["ReceiverId"] = new SelectList(_context.Clients, "ClientId", "Name");
            ViewData["SenderId"] = new SelectList(_context.Clients, "ClientId", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransactionId,Type,Amount,Currency,IsDomestic,Provision,SenderId,ReceiverId,Timestamp")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.TransactionId = Guid.NewGuid();
                _feeCalculationService.ApplyFee(transaction);
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceiverId"] = new SelectList(_context.Clients, "ClientId", "ClientId", transaction.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Clients, "ClientId", "ClientId", transaction.SenderId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewBag.Currency = new SelectList(Enum.GetValues(typeof(Currency)));
            ViewBag.TransactionType = new SelectList(Enum.GetValues(typeof(TransactionType)));
            ViewData["ReceiverId"] = new SelectList(_context.Clients, "ClientId", "ClientId", transaction.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Clients, "ClientId", "ClientId", transaction.SenderId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TransactionId,Type,Amount,Currency,IsDomestic,Provision,SenderId,ReceiverId,Timestamp")] Transaction transaction)
        {
            if (id != transaction.TransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _feeCalculationService.ApplyFee(transaction);
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceiverId"] = new SelectList(_context.Clients, "ClientId", "ClientId", transaction.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.Clients, "ClientId", "ClientId", transaction.SenderId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Receiver)
                .Include(t => t.Sender)
                .FirstOrDefaultAsync(m => m.TransactionId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(Guid id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}
