using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;
using WypozyczeniaAPI.Services;
using WypozyczeniaAPI.Models;
using WypozyczeniaAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Data;

//using WypozyczeniaAPI.Data;


namespace WypozyczeniaAPI.Controllers
{
    [Authorize]
    public class PojazdController : Controller
    {
        private readonly DBContext _context;
        private readonly IPojazdService _pojazdService;

        public PojazdController(DBContext context, IPojazdService pojazdService)
        {
            _context = context;
            _pojazdService = pojazdService;
        }

        [Authorize(Roles = "Admin,Employee")]
        // GET: Pojazd
        // Metoda wyświetlająca listę pojazdów
        public async Task<IActionResult> Index(int? page)
        {
            try
            {
                if (_context.Pojazd == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Pojazd'  is null.");
                }
                else
                {
                    // Pobieramy pojazdy z bazy danych
                    var pojazdy = await _context.Pojazd.ToListAsync();

                    // Podział danych na części zaleznie od numeru strony
                    if (page == null || page < 1)
                    {
                        page = 1;
                    }

                    double ilosc = pojazdy.Count();
                    double max = Math.Ceiling(ilosc / 5);

                    if (page > max)
                    {
                        page = (int)max;
                    }

                    pojazdy = page<Pojazd>((int)page, pojazdy);
                    ViewBag.page = (int)page;

                    if (TempData["Massage"] != null)
                    {
                        ViewBag.Message = TempData["Massage"];
                    }

                    return View(pojazdy);
                }                    
            }
            catch (Exception ex)
            {
                return Problem("Entity set 'ApplicationDbContext.Pojazd'  is null.");
            }
        }

        [Authorize(Roles = "Admin, Employee, User")]
        // GET: Pojazd/Details/5
        // Metoda wyświetlająca widok detali pojazdu 
        public async Task<IActionResult> Details(int? id)
        {
            
            if (id == null || _context.Pojazd == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o wskazanym Id
            Pojazd pojazd = null;
            try
            {
                pojazd = await _context.Pojazd.FirstOrDefaultAsync(m => m.Id == id);
            }
            catch(Exception ex) { 
            
            }
            
            // Sprawdzamy czy pojazd istnieje
            if (pojazd == null)
            {
                return NotFound();
            }
            
            return View(pojazd);
        }

        [Authorize(Roles = "Admin,Employee")]
        // GET: Pojazd/HistDetails/5
        // Metoda wyświetlająca widok detali pojazdu z trasą przejazdu
        public async Task<IActionResult> HistDetails(int? id)
        {

            if (id == null || _context.Pojazd == null || _context.Pozycja == null)
            {
                return NotFound();
            }
            // Wyszukujemy opis o wskazanym Id
            Pojazd pojazd = null;
            try
            {
                pojazd = await _context.Pojazd.FirstOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception ex)
            {

            }

            if (pojazd == null)
            {
                return NotFound();
            }

            // Pobieramy pozycje przypisane do danego pojadzu  
            List<Pozycja> pozycje = null;
            try
            {
                pozycje = await _context.Pozycja.Where(p => p.PojazdId == id).OrderBy(p => p.Data).ToListAsync();
            }
            catch (Exception ex)
            {

            }

            // Pobieramy wypożycznie dotyczące danego pojazdu
            var daty = await _context.Wypozyczenie.Where(w => w.PojazdId == id).OrderByDescending(w => w.DataRozpoczęcia).Select(w => new { w.DataRozpoczęcia, w.DataZakończenia }).ToListAsync();
            
            // Zapisujemy daty o Rozpoczęcia i zakończenia do listy
            Stack<DateTime> datyDto = new Stack<DateTime>();
            foreach (var d in daty)
            {
                var data = d.DataZakończenia;
                if (data != null)
                {
                    datyDto.Push((DateTime)data);
                }
                datyDto.Push(d.DataRozpoczęcia);
            }

            // Obiekt modelu, za pomocą którego przekaże pobrane listy do widoku
            var myModel = new MyModelSingle<Pojazd, List<Pozycja>, Stack<DateTime>>();
            myModel.Obj1 = pojazd;
            myModel.Obj2 = pozycje;
            myModel.Obj3 = datyDto;

            return View(myModel);
        }

        [Authorize(Roles = "Admin,Employee")]
        // GET: Pojazd/Create
        // Metoda wyświetlająca formularz dodania pojazdu
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Employee")]
        // POST: Pojazds/Create
        // Metoda tworząca wpis w bazie danych dotyczący pojazdu
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Rejestracja,Marka,Model,Status,NS,WE")] Pojazd pojazd)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Wyszukujemy pojazd w bazie 
                    var pom = await _context.Pojazd.Where(p => p.Rejestracja == pojazd.Rejestracja).ToListAsync();
                    
                    // Spr czy pojazd istnieje już wa bazie
                    if (pom.Count() == 0)
                    {
                        // Jeśli nie to:
                        // Generujemy Hash na bazie rejestracji
                        pojazd.Hash = _pojazdService.Hash(pojazd.Rejestracja);
                        
                        // Dodajemy pojzd do bazy
                        _context.Add(pojazd);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewBag.Message = "Pojazd o takiej rejestracji znajduje sie w bazie danych";
                        return View(pojazd);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Coś poszło nie tak";
                    return View(pojazd);
                }     
            }
            // Przekazanie komunikatu do widoku przy pomocy ViewBag lub ViewData.
            ViewBag.Message = "Wprowadzone dane sa bledne";
            return View(pojazd);
        }

        [Authorize(Roles = "Admin,Employee")]
        // GET: Pojazd/Edit/5
        // Metoda wyświetlająca formularz edycji pojazdu
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pojazd == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            Pojazd pojazd = null;
            try
            {
                pojazd = await _context.Pojazd.FindAsync(id);
            }catch (Exception ex)
            {

            }

            if (pojazd == null)
            {
                return NotFound();
            }
            return View(pojazd);
        }

        [Authorize(Roles = "Admin,Employee")]
        // POST: Pojazd/Edit/5
        // Metoda edytująca wpis w bazie danych dotyczący pojazdu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rejestracja,Marka,Model,Status,NS,WE")] Pojazd pojazd)
        {
            if (id != pojazd.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Aktualizujemy wpis w bazie danych dotyczący pojazdu
                    _context.Update(pojazd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Massage"] = "Wystąpił błąd bazy danych";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pojazd);
        }

        [Authorize(Roles = "Admin,Employee")]
        // GET: Pojazd/Delete/5
        // Metoda wyświetlająca formularz usunięcia pojazdu
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pojazd == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            Pojazd pojazd = null;
            try
            {
                pojazd = await _context.Pojazd.FirstOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception ex)
            {

            }

            if (pojazd == null)
            {
                return NotFound();
            }

            return View(pojazd);
        }

        [Authorize(Roles = "Admin,Employee")]
        // POST: Pojazd/Delete/5
        // Metoda usuwająca pojazd
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pojazd == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Pojazd'  is null.");
            }

            // Wyszukujemy pojazd o podanym Id
            Pojazd pojazd = null;
            try
            {
                pojazd = await _context.Pojazd.FindAsync(id);
            }
            catch (Exception ex)
            {

            }

            if (pojazd != null)
            {
                // Usuwamy pojazd 
                _context.Pojazd.Remove(pojazd);
            }

            // Zapisujemy zmiany w bazie danych
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public List<T> page<T>(int page, List<T> dane)
        {
            dane = dane.Take(page * 5).ToList();

            int ilosc = dane.Count();

            //int cale = ilosc / 10 + 1;

            if (ilosc >= 5)
            {
                dane = dane.TakeLast(5).ToList();
            }

            return dane;
        }
    }
}
