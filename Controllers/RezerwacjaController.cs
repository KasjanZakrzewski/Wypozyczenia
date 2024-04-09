using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;
using WypozyczeniaAPI.Models.Dto;
using WypozyczeniaAPI.Services;
//using WypozyczeniaAPI.Services;

namespace WypozyczeniaAPI.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class RezerwacjaController : Controller
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IPojazdService _pojazdServices;
        private readonly IRezerwacjaService _rezerwacjaServices;
        private readonly UserManager<User> _UserManager;

        public RezerwacjaController(DBContext context, IPojazdService pojazdServices, IMapper mapper, IRezerwacjaService rezerwacjaServices, UserManager<User> UserManager)
        {
            _context = context;
            _mapper = mapper;
            _pojazdServices = pojazdServices;
            _rezerwacjaServices = rezerwacjaServices;
            _UserManager = UserManager;
        }

        // GET: Rezerwacja
        // Metoda wyświetlająca Liste rezerwacji
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? page)
        {
            // Spr czy tablica istnieje
            if(_context.Rezerwacja == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Rezerwacja'  is null.");
            }
            else
            {
                // Pobieram liste rezerwacji w systemie,
                // nastęnie mapuję ją na odpowiednie Dto
                var rezerwacje = await _context.Rezerwacja.Include(p => p.Pojazd).Include(p => p.Uzytkownik).ToListAsync();
                var Dto = _mapper.Map<List<WypoRezDto2>>(rezerwacje);

                if(page == null || page < 1)
                {
                    page = 1;
                }

                double ilosc = rezerwacje.Count();
                double max = Math.Ceiling(ilosc/5);

                if(page > max)
                {
                    page = (int)max;
                }

                Dto = page<WypoRezDto2>((int)page, Dto);
                ViewBag.page = (int)page;

                if (TempData["Massage"] != null)
                {
                    ViewBag.Message = TempData["Massage"];
                }

                return View(Dto);
            }
        }

        // GET: Rezerwacja/Details/5
        // Metoda wyświetlająca widok detali rezerwacji
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rezerwacja == null)
            {
                return NotFound();
            }

            // Wyszukujemy rezerwacje o wskazanym Id
            var rezerwacja = await _context.Rezerwacja
                .FirstOrDefaultAsync(m => m.Id == id);

            // Spr czy rezerwacja istnieje w systemie
            if (rezerwacja == null)
            {
                return NotFound();
            }
                        
            return View(rezerwacja);
        }

        // GET: Rezerwacja/Create
        // Metoda wyświetlająca formularz dodania rezerwacji
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        { 
            return View();
        }
        /*
        // POST: Rezerwacjas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataRozpoczęcia,DataZakończenia,PojazdId,UzytkownikId")] Rezerwacja rezerwacja)
        {
            if (ModelState.IsValid)
            {
                if (_pojazdServices.sprDostepnosc(rezerwacja.PojazdId)) {
                    _pojazdServices.ZmStatusu(rezerwacja.PojazdId, "Zarezerwowany");

                    try
                    {
                        _context.Add(rezerwacja);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception e)
                    {
                        // Przekazanie komunikatu do widoku przy pomocy ViewBag lub ViewData.
                        ViewBag.Message = "Wprowadzone dane sa bledne";
                        return View();
                    }
                    
                }
                else
                {
                    // Przekazanie komunikatu do widoku przy pomocy ViewBag lub ViewData.
                    ViewBag.Message = "Pojazd jest niedostepny";
                    return View();
                }

                
            }
            // kom że poszło nie tak
            return View(rezerwacja);
        }
        */

        // GET: Rezerwacja/Edit/5
        // Metoda wyświetlająca formularz edycji rezerwacji
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rezerwacja == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var rezerwacja = await _context.Rezerwacja.FindAsync(id);
            if (rezerwacja == null)
            {
                return NotFound();
            }

            return View(rezerwacja);
        }

        // POST: Rezerwacja/Edit/5
        // Metoda edytująca wpis w bazie danych dotyczący rezerwacji
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataRozpoczęcia,DataZakończenia,PojazdId,UzytkownikId")] Rezerwacja rezerwacja)
        {
            if (id != rezerwacja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Aktualizujemy wpis w bazie danych dotyczący rezerwacji
                    _context.Update(rezerwacja);
                    await _context.SaveChangesAsync();
                    TempData["Massage"] = "Wprowadzono zmiany";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RezerwacjaExists(rezerwacja.Id))
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
            ViewBag.Massage = "Wprowadzone dane są błędne";
            return View(rezerwacja);
        }

        // GET: Rezerwacja/Delete/5
        // Metoda wyświetlająca formularz usunięcia rezerwacji
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rezerwacja == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var rezerwacja = await _context.Rezerwacja
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rezerwacja == null)
            {
                return NotFound();
            }

            return View(rezerwacja);
        }

        // POST: Rezerwacja/Delete/5
        // Metoda usuwająca rezerwacje
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rezerwacja == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Rezerwacja'  is null.");
            }

            // Wyszukujemy pojazd o podanym Id
            var rezerwacja = await _context.Rezerwacja.FindAsync(id);
            if (rezerwacja != null)
            {
                // Usuwamy pojazd 
                _context.Rezerwacja.Remove(rezerwacja);
            }

            // Zapisujemy zmiany w bazie danych
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        private bool RezerwacjaExists(int id)
        {
          return (_context.Rezerwacja?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        // GET: Rezerwacja/End/5
        // Metoda kończąca rezerwacje 
        public async Task<IActionResult> End(int? id)
        {
            if (id == null || _context.Rezerwacja == null)
            {
                return NotFound();
            }

            // Wyszukujemy pojazd o podanym Id
            var rezerwacja = await _context.Rezerwacja.FindAsync(id);
            if (rezerwacja == null)
            {
                return NotFound();
            }
            var user = await _UserManager.GetUserAsync(User);
            //var userId = await _UserManager.GetUserIdAsync(user);
            if (rezerwacja.UzytkownikId != user.Id && !User.IsInRole("Admin"))
            {
                return Problem("Nie masz uprawnień do przeporowadzenia akcji");
            }

            // <|dodanie timera, który ma zakończyć rezerwacje|> 
            if (_rezerwacjaServices.sprAktywność((int)id)){
                try
                {
                    // Ustawiamy datę zakończenia na aktualną date systemu
                    rezerwacja.DataZakończenia = DateTime.Now;

                    // Zapisujemy zmiany w bazie danych
                    _context.Update(rezerwacja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                // Zmieniamy status pojazdu na "Dostepny"
                await _pojazdServices.ZmStatusu(rezerwacja.PojazdId, "Dostepny");
            }
            else
            {
                // rezerwacja jest już zakończona 
                ViewBag.Massage = "Rezerwacja jest już zakończona";
            }

            return View("Details",rezerwacja);
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
