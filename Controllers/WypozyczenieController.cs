using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

namespace WypozyczeniaAPI.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class WypozyczenieController : Controller
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IPojazdService _pojazdServices;
        private readonly IWypozyczenieService _wypozyczenieServices;
        private readonly IRezerwacjaService _rezerwacjaServices;
        private readonly UserManager<User> _UserManager;

        public WypozyczenieController(DBContext context, IPojazdService pojazdServices, IWypozyczenieService wypozyczenieServices, IRezerwacjaService rezerwacjaServices, IMapper mapper, UserManager<User> UserManager)
        {
            _context = context;
            _mapper = mapper;
            _pojazdServices = pojazdServices;
            _wypozyczenieServices = wypozyczenieServices;
            _rezerwacjaServices = rezerwacjaServices;
            _UserManager = UserManager;
        }

        // GET: Wypozyczenie
        // Metoda wyświetlająca listę wypożyczeń
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? page)
        {
            // Spr czy tablica istnieje
            if (_context.Wypozyczenie == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Rezerwacja'  is null.");
            }
            else
            {
                // Pobieram liste wypożyczeń w systemie,
                // nastęnie mapuję ją na odpowiednie Dto
                var wypozyczenia = await _context.Wypozyczenie.Include(p => p.Pojazd).Include(p => p.Uzytkownik).ToListAsync();
                var Dto = _mapper.Map<List<WypoRezDto2>>(wypozyczenia);

                if (page == null || page < 1)
                {
                    page = 1;
                }

                double ilosc = wypozyczenia.Count();
                double max = Math.Ceiling(ilosc / 5);

                if (page > max)
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

           // return _context.Wypozyczenie != null ? 
           //               View(await _context.Wypozyczenie.ToListAsync()) :
           //              Problem("Entity set 'ApplicationDbContext.Wypozyczenie'  is null.");
        }

        // GET: Wypozyczenie/Details/5
        // Metoda wyświetlająca widok detali wypożyczenia
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Wypozyczenie == null)
            {
                return NotFound();
            }

            // Wyszukujemy wypożyczenie o wskazanym Id
            var wypozyczenie = await _context.Wypozyczenie
                .FirstOrDefaultAsync(m => m.Id == id);
            
            // Spr czy wypożyczenie istnieje w systemie
            if (wypozyczenie == null)
            {
                return NotFound();
            }
            
            // Spr czy wypożyczenie zostało zakończone,
            // jeśli nie to data ostaje tymczasowo ustaiono
            // na aktulną dane w systemie
            var zakData = DateTime.Now;
            if (wypozyczenie.DataZakończenia != null){
                zakData = (DateTime)wypozyczenie.DataZakończenia;
            }
            
            // Pobieramy pozycje dotyczące pojazdu, którego dotyczy wypożycznie 
            // pozycje muszą być pobrane od daty rozpoczęcia do daty zakończenia wypożyczenia
            var pozycje = await _context.Pozycja.Where(p => p.Data >= wypozyczenie.DataRozpoczęcia && p.Data <= zakData && p.PojazdId == wypozyczenie.PojazdId).OrderBy(p => p.Data).ToListAsync();
            var pozycjeDto = _mapper.Map<List<PozycjaDto>>(pozycje);

            // Przesyłamy pobrane dane do widoku
            var myModel = new MyModelSingle<Wypozyczenie, List<PozycjaDto>>();
            myModel.Obj1 = wypozyczenie;
            myModel.Obj2 = pozycjeDto;

            if (TempData["Massage"] != null)
            {
                ViewBag.Message = TempData["Massage"];
            }

            return View(myModel);
        }

        // GET: Wypozyczenie/Create
        // Metoda wyświetlająca formularz dodania wypożyczenia 
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        /*
        // POST: Wypozyczenies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataRozpoczęcia,DataZakończenia,PojazdId,UzytkownikId")] Wypozyczenie wypozyczenie)
        {
            if (ModelState.IsValid)
            {
                bool rezerwacja = false;
                if (_pojazdServices.sprDostepnosc(wypozyczenie.PojazdId) || (rezerwacja = _pojazdServices.sprRezerwacje(wypozyczenie.PojazdId)))
                {
                    _pojazdServices.ZmStatusu(wypozyczenie.PojazdId, "Wypozyczony");

                    if (rezerwacja)
                    {
                        _rezerwacjaServices.endRezerwacja(wypozyczenie.PojazdId);
                    }

                    try
                    {
                        _context.Add(wypozyczenie);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        // Przekazanie komunikatu do widoku przy pomocy ViewBag lub ViewData.
                        ViewBag.Message = "blad bazy danych wypozyczenie";
                        return View(wypozyczenie);
                    }
                    
                }
                else
                {
                    // Przekazanie komunikatu do widoku przy pomocy ViewBag lub ViewData.
                    ViewBag.Message = "Pojazd nie jest dostepny";
                    return View(wypozyczenie);
                }
            }
            // kom że poszło nie tak
            return View(wypozyczenie);
        }
        */

        // GET: Wypozyczenie/Edit/5
        // Metoda wyświetlająca formularz edycji wypożyczenia
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Wypozyczenie == null)
            {
                return NotFound();
            }
            // Wyszukujemy wypożyczenie o podanym Id
            var wypozyczenie = await _context.Wypozyczenie.FindAsync(id);

            // Spr czy wypożyczenie istnieje
            if (wypozyczenie == null)
            {
                return NotFound();
            }
            return View(wypozyczenie);
        }

        // POST: Wypozyczenie/Edit/5
        // Metoda edytująca wpis w bazie danych dotyczący wypożyczenia
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataRozpoczęcia,DataZakończenia,PojazdId,UzytkownikId")] Wypozyczenie wypozyczenie)
        {
            if (id != wypozyczenie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Aktualizujemy wpis w bazie danych dotyczący rezerwacji
                    _context.Update(wypozyczenie);
                    await _context.SaveChangesAsync();
                    TempData["Massage"] = "Wprowadzono zmiany";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WypozyczenieExists(wypozyczenie.Id))
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
            return View(wypozyczenie);
        }

        // GET: Wypozyczenie/Delete/5
        // Metoda wyświetlająca formularz usunięcia wypożyczenia
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Wypozyczenie == null)
            {
                return NotFound();
            }

            // Wyszukujemy opis o podanym Id
            var wypozyczenie = await _context.Wypozyczenie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wypozyczenie == null)
            {
                return NotFound();
            }

            return View(wypozyczenie);
        }

        // POST: Wypozyczenie/Delete/5
        // Metoda usuwająca wypożyczenia
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Wypozyczenie == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Wypozyczenie'  is null.");
            }

            // Wyszukujemy pojazd o podanym Id
            var wypozyczenie = await _context.Wypozyczenie.FindAsync(id);
            if (wypozyczenie != null)
            {
                // Usuwamy pojazd 
                _context.Wypozyczenie.Remove(wypozyczenie);
            }

            // Zapisujemy zmiany w bazie danych
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        [Authorize(Roles = "Admin")]
        private bool WypozyczenieExists(int id)
        {
          return (_context.Wypozyczenie?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: Wypozyczenie/End/5
        // Metoda kończąca wypożyczenie 
        public async Task<IActionResult> End(int? id)
        {
            if (id == null || _context.Wypozyczenie == null)
            {
                return NotFound();
            }

            // Wyszukujemy pojazd o podanym Id
            var wypozyczenie = await _context.Wypozyczenie.Include(w => w.Pojazd).Where(w => w.Id == id).FirstOrDefaultAsync();
            if (wypozyczenie == null)
            {
                return NotFound();
            }

            var user = await _UserManager.GetUserAsync(User);
            if (wypozyczenie.UzytkownikId != user.Id && !User.IsInRole("Admin"))
            {
                return Problem("Nie masz uprawnień do przeporowadzenia akcji");
            }

            if (_wypozyczenieServices.sprAktywność((int)id))
            {
                try
                {
                    // Ustawiamy datę zakończenia na aktualną date systemu
                    wypozyczenie.DataZakończenia = DateTime.Now;

                    // Zapisujemy zmiany w bazie danych
                    _context.Update(wypozyczenie);
                    await _context.SaveChangesAsync();

                    // Dodajemy wpis o aktalnej pozycji pojazdu
                    Pozycja pozycja = new Pozycja();
                    pozycja.WE = wypozyczenie.Pojazd.WE;
                    pozycja.NS = wypozyczenie.Pojazd.NS;
                    pozycja.Data = (DateTime)wypozyczenie.DataZakończenia;
                    pozycja.PojazdId = wypozyczenie.PojazdId;
                    _context.Add(pozycja);
                    await _context.SaveChangesAsync();

                    // Obliczamy pokonany dystans i przypisujemy go do wypożyczenia
                    float dystans = (float)_wypozyczenieServices.kilometraz(wypozyczenie).Result;
                    wypozyczenie.Dystans = dystans;

                    // Obliczamy oplate i przypisujemy ją do wypożyczenia
                    // Wzór: 5 zł początkowe i 2,5 zł za km
                    float oplata = dystans * (float)2.5 + 5;
                    oplata = (float)Math.Round(oplata,2);
                    wypozyczenie.Oplata = oplata;

                    // Zapisujemy zmiany w bazie danych
                    _context.Update(wypozyczenie);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                // Zmieniamy status pojazdu na "Dostepny"
                await _pojazdServices.ZmStatusu(wypozyczenie.PojazdId, "Dostepny");
            }
            else
            {
                // wypożyczenie jest już zakończone
                ViewBag.Massage = "Wypożyczenie jest już zakończone";
            }
            
            return RedirectToAction("Details", new { id = id });
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
