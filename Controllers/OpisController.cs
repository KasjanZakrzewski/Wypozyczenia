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
    [Authorize(Roles = "Admin,Employee")]
    public class OpisController : Controller
    {
        private readonly DBContext _context;

        public OpisController(DBContext context)
        {
            _context = context;
        }

        // GET: Opis
        // Metoda wyświetlająca listę opisów
        public async Task<IActionResult> Index()
        {
            var opisy = await _context.Opis.Include(o => o.Serwis).ToListAsync();
            return View(opisy);
        }

        // GET: Opis/Details/5
        // Metoda wyświetlająca widok detali opisu 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Opis == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o wskazanym Id
            var opis = await _context.Opis
                .Include(o => o.Serwis)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (opis == null)
            {
                return NotFound();
            }

            return View(opis);
        }

        // GET: Opis/Create
        // Metoda wyświetlająca formularz dodania opisu
        public IActionResult Add()
        {
            // Przekazujemy do widoku dane dotyczące Typ, Id serwisu i Daty
            TempData["Typ"] = TempData["Typ"];
            TempData["SerwisId"] = TempData["SerwisId"];
            TempData["Data"] = TempData["Data"];

            return View();
        }

        // GET: Opis/Create
        // Metoda wyświetlająca formularz dodania opisu
        public IActionResult Create()
        {
            // Przekazujemy do widoku dane dotyczące Typ, Id serwisu i Daty
            TempData["Typ"] = TempData["Typ"];
            TempData["SerwisId"] = TempData["SerwisId"];
            TempData["Data"] = TempData["Data"];

            return View("Add");
        }

        // POST: Opis/Create
        // Metoda tworząca wpis w bazie danych dotyczący opisu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Data,Typ,Tytul,Treść,SerwisId")] Opis opis)
        {
            if (ModelState.IsValid)
            {
                // Ustawiamy date opisu na aktualną date systemu
                opis.Data = DateTime.Now;
                _context.Add(opis);
                await _context.SaveChangesAsync();

                // Przekazujemy do widoku dane dotyczące Typ, Id serwisu i Daty
                TempData["Typ"] = opis.Treść;
                TempData["Data"] = opis.Data;
                TempData["SerwisId"] = opis.SerwisId;
                
                // Przekierowujemy di 
                return RedirectToAction("SerwisDetails", "SerwisInterface",new { id = opis.SerwisId });
            }
            ViewData["SerwisId"] = new SelectList(_context.Serwis, "Id", "Id", opis.SerwisId);
            return View("Add", opis);
        }

        // GET: Opis/Edit/5
        // Metoda wyświetlająca formularz edycji opisu
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Opis == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var opis = await _context.Opis.FindAsync(id);
            if (opis == null)
            {
                return NotFound();
            }

            ViewData["SerwisId"] = new SelectList(_context.Serwis, "Id", "Id", opis.SerwisId);
            return View(opis);
        }

        // POST: Opis/Edit/5
        // Metoda edytująca wpis w bazie danych dotyczący opisu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Data,Typ,Tytul,Treść,SerwisId")] Opis opis)
        {
            if (id != opis.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Aktualizujemy wpis w bazie danych dotyczący opisu
                    _context.Update(opis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpisExists(opis.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("SerwisDetails", "SerwisInterface", new {id = opis.SerwisId});
            }
            ViewData["SerwisId"] = new SelectList(_context.Serwis, "Id", "Id", opis.SerwisId);
            return View(opis);
        }

        // GET: Opis/Delete/5
        // Metoda wyświetlająca formularz usunięcia opisu
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Opis == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var opis = await _context.Opis
                .Include(o => o.Serwis)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (opis == null)
            {
                return NotFound();
            }

            return View(opis);
        }

        // POST: Opis/Delete/5
        // Metoda usuwająca opis
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Opis == null)
            {
                return Problem("Entity set 'DBContext.Opis'  is null.");
            }

            // Wyszukujemy opis o podanym Id
            var opis = await _context.Opis.FindAsync(id);
            if (opis != null)
            {
                // Usuwamy opis 
                _context.Opis.Remove(opis);
            }
            
            // Zapisujemy zmiany w bazie danych
            await _context.SaveChangesAsync();
            return RedirectToAction("SerwisDetails", "SerwisInterface", new { id = opis.SerwisId });
        }

        private bool OpisExists(int id)
        {
          return (_context.Opis?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
