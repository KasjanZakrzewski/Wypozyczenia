using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;
using WypozyczeniaAPI.Services;
using WypozyczeniaAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace WypozyczeniaAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PozycjaController : Controller
    {
        private readonly DBContext _context;
        private readonly IPojazdService _pojazdService;

        public PozycjaController(DBContext context, IPojazdService pojazdService)
        {
            _context = context;
            _pojazdService = pojazdService;
        }

        // POST: Pozycja/Add
        // Metoda obsługująca dodanie pozycji przez podsystem pojazdu
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Add([FromBody] ArduinoDto pozycja)
        {
            // Sprawdzamy czy Hash podany w rządaniu zgadza się z tym zapisanym w bazie
            var result =  _pojazdService.sprHash(pozycja.PojazdId, pozycja.Hash);
            if (!result.Result)
            {
                return NoContent();
            }
            
            try
            {
                // Tworzymy nowy obiekt pozycji 
                Pozycja pozycjaNowa = new Pozycja();
                pozycjaNowa.Data = DateTime.Now;
                pozycjaNowa.WE = pozycja.WE;
                pozycjaNowa.NS = pozycja.NS;
                pozycjaNowa.PojazdId = pozycja.PojazdId;

                // Dodajemy nową pozycje do bazy
                _context.Add(pozycjaNowa);
                await _context.SaveChangesAsync();

                // Zmieniamy pozycjie pojazdu na aktualną
                result =  _pojazdService.ZmPozycji(pozycja.PojazdId, pozycja.NS, pozycja.WE);
                if (!result.Result)
                {
                    return NoContent();
                }
                else
                {
                    return Ok();
                }
                
            }catch(Exception ex)
            {
                return NoContent();
            }
        }

        // GET: Pozycja
        // Metoda wyświetlające listę pozycji
        public async Task<IActionResult> Index(int? page)
        {
            // Pobieramy listę pozycji z bazy
            var dane = await _context.Pozycja.Include(p => p.Pojazd).ToListAsync();

            if(page == null)
            {
                page = 1;
            }

            dane = page<Pozycja>((int)page, dane);

            if (TempData["Massage"] != null)
            {
                ViewBag.Message = TempData["Massage"];
            }

            return View(dane);
        }

        // GET: Pozycja/Details/5
        // Metoda wyświetlająca widok detali pozycji 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pozycja == null)
            {
                return NotFound();
            }
            // Wyszukujemy opis o wskazanym Id
            var pozycja = await _context.Pozycja
                .Include(p => p.Pojazd)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pozycja == null)
            {
                return NotFound();
            }

            return View(pozycja);
        }

        // GET: Pozycja/Create
        // Metoda wyświetlająca formularz dodania pozycji 
        public IActionResult Create()
        {
            ViewData["PojazdId"] = new SelectList(_context.Pojazd, "Id", "Id");
            return View();
        }

        // POST: Pozycja/Create
        // Metoda tworząca wpis w bazie danych dotyczący pozycji
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NS,WE,Data,PojazdId")] Pozycja pozycja)
        {
            if (ModelState.IsValid)
            {
                // Dodajemy pozycje do bazy
                _context.Add(pozycja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PojazdId"] = new SelectList(_context.Pojazd, "Id", "Id", pozycja.PojazdId);
            return View(pozycja);
        }

        // GET: Pozycja/Edit/5
        // Metoda wyświetlająca formularz edycji pozycji
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pozycja == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var pozycja = await _context.Pozycja.FindAsync(id);
            if (pozycja == null)
            {
                return NotFound();
            }

            ViewData["PojazdId"] = new SelectList(_context.Pojazd, "Id", "Id", pozycja.PojazdId);
            return View(pozycja);
        }

        // POST: Pozycja/Edit/5
        // Metoda edytująca wpis w bazie danych dotyczący pozycji
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NS,WE,Data,PojazdId")] Pozycja pozycja)
        {
            if (id != pozycja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Aktualizujemy wpis w bazie danych dotyczący pojazdu
                    _context.Update(pozycja);
                    await _context.SaveChangesAsync();
                    TempData["Massage"] = "Wprowadzono zmiany";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PozycjaExists(pozycja.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["Massage"] = "Nie wprowadzono zmian z powodu błędu";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PojazdId"] = new SelectList(_context.Pojazd, "Id", "Id", pozycja.PojazdId);
            return View(pozycja);
        }

        // GET: Pozycja/Delete/5
        // Metoda wyświetlająca formularz usunięcia pozycji
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pozycja == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var pozycja = await _context.Pozycja
                .Include(p => p.Pojazd)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pozycja == null)
            {
                return NotFound();
            }

            return View(pozycja);
        }

        // POST: Pozycja/Delete/5
        // Metoda usuwająca pojazd
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pozycja == null)
            {
                return Problem("Entity set 'DBContext.Pozycja'  is null.");
            }

            // Wyszukujemy pojazd o podanym Id
            var pozycja = await _context.Pozycja.FindAsync(id);
            if (pozycja != null)
            {
                // Usuwamy pojazd
                _context.Pozycja.Remove(pozycja);
            }

            // Zapisujemy zmiany w bazie danych
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PozycjaExists(int id)
        {
          return (_context.Pozycja?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public List<T> page<T>(int page, List<T> dane)
        {
            dane = dane.Take(page * 10).ToList();

            int ilosc = dane.Count();

            // int cale = ilosc - ilosc % 10;
            if (ilosc >= 10)
            {
                dane = dane.TakeLast(10).ToList();
            }

            return dane;
        }
    }
}
