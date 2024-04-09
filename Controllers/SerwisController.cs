using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;

namespace WypozyczeniaAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SerwisController : Controller
    {
        private readonly DBContext _context;

        public SerwisController(DBContext context)
        {
            _context = context;
        }

        // GET: Serwis
        // Wyświetla liste wszystkich serwisów
        public async Task<IActionResult> Index()
        {
            // Pobieramy listę serwisów z bazy
            var dBContext = _context.Serwis.Include(s => s.Admin).Include(s => s.Pojazd).Include(s => s.Pracownik);
            return View(await dBContext.ToListAsync());
        }

        // GET: Serwis/Details/5
        // Metoda wyświetlająca widok detali serwisu
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Serwis == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o wskazanym Id
            var serwis = await _context.Serwis
                .Include(s => s.Admin)
                .Include(s => s.Pojazd)
                .Include(s => s.Pracownik)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serwis == null)
            {
                return NotFound();
            }

            return View(serwis);
        }

        // GET: Serwis/Create
        // Metoda wyświetlająca formularz dodania serwisu
        public IActionResult Create()
        {
            // Przekazujemy dane o Id administratora, pracownika i pojazdu
            ViewData["AdminId"] = new SelectList(_context.Uzytkownik, "Id", "Id");
            ViewData["PojazdId"] = new SelectList(_context.Pojazd, "Id", "Id");
            ViewData["PracownikId"] = new SelectList(_context.Uzytkownik, "Id", "Id");

            return View();
        }

        // POST: Serwis/Create
        // Metoda tworząca wpis w bazie danych dotyczący serwisu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Data,PojazdId,AdminId,PracownikId")] Serwis serwis)
        {
            if (ModelState.IsValid)
            {
                // Dodajemy serwis do bazy
                _context.Add(serwis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Przekazujemy dane o Id administratora, pracownika i pojazdu
            ViewData["AdminId"] = new SelectList(_context.Uzytkownik, "Id", "Id", serwis.AdminId);
            ViewData["PojazdId"] = new SelectList(_context.Pojazd, "Id", "Id", serwis.PojazdId);
            ViewData["PracownikId"] = new SelectList(_context.Uzytkownik, "Id", "Id", serwis.PracownikId);
            
            return View(serwis);
        }

        // GET: Serwis/Edit/5
        // Metoda wyświetlająca formularz edycji serwisu
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Serwis == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var serwis = await _context.Serwis.FindAsync(id);
            if (serwis == null)
            {
                return NotFound();
            }

            // Przekazujemy dane o Id administratora, pracownika i pojazdu
            ViewData["AdminId"] = new SelectList(_context.Uzytkownik, "Id", "Id", serwis.AdminId);
            ViewData["PojazdId"] = new SelectList(_context.Pojazd, "Id", "Id", serwis.PojazdId);
            ViewData["PracownikId"] = new SelectList(_context.Uzytkownik, "Id", "Id", serwis.PracownikId);
            
            return View(serwis);
        }

        // POST: Serwis/Edit/5
        // Metoda edytująca wpis w bazie danych dotyczący serwisu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Data,PojazdId,AdminId,PracownikId")] Serwis serwis)
        {
            if (id != serwis.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Aktualizujemy wpis w bazie danych dotyczący pojazdu
                    _context.Update(serwis);
                    await _context.SaveChangesAsync();
                    TempData["Massage"] = "Wprowadzono zmiany";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SerwisExists(serwis.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["Massage"] = "Nie wprowadzono zmian w wyniku błędu";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Przekazujemy dane o Id administratora, pracownika i pojazdu
            ViewData["AdminId"] = new SelectList(_context.Uzytkownik, "Id", "Id", serwis.AdminId);
            ViewData["PojazdId"] = new SelectList(_context.Pojazd, "Id", "Id", serwis.PojazdId);
            ViewData["PracownikId"] = new SelectList(_context.Uzytkownik, "Id", "Id", serwis.PracownikId);

            ViewBag.Massage = "Wprowadzono zmiany";
            return View(serwis);
        }

        // GET: Serwis/Delete/5
        // Metoda wyświetlająca formularz usunięcia serwisu
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Serwis == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var serwis = await _context.Serwis
                .Include(s => s.Admin)
                .Include(s => s.Pojazd)
                .Include(s => s.Pracownik)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serwis == null)
            {
                return NotFound();
            }

            return View(serwis);
        }

        // POST: Serwis/Delete/5
        // Metoda usuwająca serwis
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Serwis == null)
            {
                return Problem("Entity set 'DBContext.Serwis'  is null.");
            }

            // Wyszukujemy serwis o podanym Id
            var serwis = await _context.Serwis.FindAsync(id);
            if (serwis != null)
            {
                // Usuwamy serwis
                _context.Serwis.Remove(serwis);
            }

            // Zapisujemy zmiany w bazie danych
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SerwisExists(int id)
        {
          return (_context.Serwis?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
