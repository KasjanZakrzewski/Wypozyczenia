using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;

namespace WypozyczeniaAPI.Controllers
{
    public class ZgłoszenieController : Controller
    {
        private readonly DBContext _context;

        public ZgłoszenieController(DBContext context)
        {
            _context = context;
        }
        /*
        // GET: Zgłoszenie
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Zgłoszenie.Include(z => z.Admin).Include(z => z.Uzytkownik);
            return View(await dBContext.ToListAsync());
        }

        // GET: Zgłoszenie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Zgłoszenie == null)
            {
                return NotFound();
            }

            var zgłoszenie = await _context.Zgłoszenie
                .Include(z => z.Admin)
                .Include(z => z.Uzytkownik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zgłoszenie == null)
            {
                return NotFound();
            }

            return View(zgłoszenie);
        }

        // GET: Zgłoszenie/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Uzytkownik, "Id", "Id");
            ViewData["UzytkownikId"] = new SelectList(_context.Uzytkownik, "Id", "Id");
            return View();
        }

        // POST: Zgłoszenie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Data,Treść,UzytkownikId,AdminId")] Zgłoszenie zgłoszenie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zgłoszenie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Uzytkownik, "Id", "Id", zgłoszenie.AdminId);
            ViewData["UzytkownikId"] = new SelectList(_context.Uzytkownik, "Id", "Id", zgłoszenie.UzytkownikId);
            return View(zgłoszenie);
        }

        // GET: Zgłoszenie/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Zgłoszenie == null)
            {
                return NotFound();
            }

            var zgłoszenie = await _context.Zgłoszenie.FindAsync(id);
            if (zgłoszenie == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.Uzytkownik, "Id", "Id", zgłoszenie.AdminId);
            ViewData["UzytkownikId"] = new SelectList(_context.Uzytkownik, "Id", "Id", zgłoszenie.UzytkownikId);
            return View(zgłoszenie);
        }

        // POST: Zgłoszenie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Data,Treść,UzytkownikId,AdminId")] Zgłoszenie zgłoszenie)
        {
            if (id != zgłoszenie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zgłoszenie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZgłoszenieExists(zgłoszenie.Id))
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
            ViewData["AdminId"] = new SelectList(_context.Uzytkownik, "Id", "Id", zgłoszenie.AdminId);
            ViewData["UzytkownikId"] = new SelectList(_context.Uzytkownik, "Id", "Id", zgłoszenie.UzytkownikId);
            return View(zgłoszenie);
        }

        // GET: Zgłoszenie/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Zgłoszenie == null)
            {
                return NotFound();
            }

            var zgłoszenie = await _context.Zgłoszenie
                .Include(z => z.Admin)
                .Include(z => z.Uzytkownik)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zgłoszenie == null)
            {
                return NotFound();
            }

            return View(zgłoszenie);
        }

        // POST: Zgłoszenie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Zgłoszenie == null)
            {
                return Problem("Entity set 'DBContext.Zgłoszenie'  is null.");
            }
            var zgłoszenie = await _context.Zgłoszenie.FindAsync(id);
            if (zgłoszenie != null)
            {
                _context.Zgłoszenie.Remove(zgłoszenie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZgłoszenieExists(int id)
        {
          return (_context.Zgłoszenie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        */
    }
}
