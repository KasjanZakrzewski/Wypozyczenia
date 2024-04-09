using Microsoft.AspNetCore.Mvc;
using WypozyczeniaAPI.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
//using WypozyczeniaAPI.Data;
//using WypozyczeniaAPI.Services;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Data;
using Microsoft.AspNetCore.Authorization;
using WypozyczeniaAPI.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System;
using Humanizer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using WypozyczeniaAPI.Models.Dto;

namespace WypozyczeniaAPI.Controllers
{
    [Authorize]
    public class UzytInterfaceController : Controller
    {
        private readonly DBContext _context;
        private readonly IPojazdService _pojazdService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _UserManager;

        public UzytInterfaceController(/*ILogger<HomeController> logger,*/ DBContext context, IPojazdService pojazdService, IMapper mapper, UserManager<User> UserManager)
        {
            _context = context;
            _pojazdService = pojazdService;
            _mapper = mapper;
            _UserManager = UserManager;
        }

        // GET: UzytInterface
        // Metoda wyświetlająca główny widok użytkownika
        public async Task<IActionResult> Index()
        {
            try
            {
                // Generujemy modlel zawierający trzy listy rezewacji, wypożyczeń oraz pojazdów
                var myModel = new MyModel<RezWypDto, RezWypDto, Pojazd>();

                // Trzecią z list jest lista dostępnych pojazdów 
                myModel.List3 = _context.Pojazd.Where(p => p.Status == "Dostepny").ToList();

                // Pierwszą z list jest lista aktywnych rezerwacji 
                var rezerwacje = _context.Rezerwacja.Where(r => r.UzytkownikId == _UserManager.GetUserId(User)).Include(r => r.Pojazd).Where(r => r.DataZakończenia == null);
                var rezerwacjeDto = _mapper.Map<List<RezWypDto>>(rezerwacje);
                myModel.List1 = rezerwacjeDto;

                // Pierwszą z list jest lista aktywnych wypożyczeń 
                var wypozyczenie = _context.Wypozyczenie.Where(r => r.UzytkownikId == _UserManager.GetUserId(User)).Include(r => r.Pojazd).Where(r => r.DataZakończenia == null);
                var wypozyczenieDto = _mapper.Map<List<RezWypDto>>(wypozyczenie);
                myModel.List2 = wypozyczenieDto;

                if (TempData["Message"] != null)
                {
                    ViewBag.Message = TempData["Message"];
                }

                return View("Index", myModel);
            }
            catch (Exception ex)
            {
                return Problem("Coś sie zepsuło");
            }
        }

        // GET: UzytInterface/Details/...
        // Metoda wyświetlająca detale,
        // zależnie od przekazanej kategorii metoda przekierowuje do metod konkretnych kontrolerów
        public async Task<IActionResult> Details(String category, int? id)
        {
            //TempData["Layout"] = "~/Views/Shared/_Layout_uzyt.cshtml";

            switch (category)
            {
                // Jeśli metoda została wywołana z lisy lub mapy pojazdów, to 
                case "P":
                    if (id == null || _context.Pojazd == null)
                    {
                        return NotFound();
                    }
                    
                    // Wyszukujemy pojazd o wskazanym Id
                    var pojazd = await _context.Pojazd.FirstOrDefaultAsync(m => m.Id == id);
                    if (pojazd == null)
                    {
                        return NotFound();
                    }

                    return RedirectToAction("Details", "Pojazd", new { id = id });
                    break;

                // Jeśli metoda została wywołana z lisy lub mapy rezerwacji, to 
                case "R":
                    if (id == null || _context.Rezerwacja == null)
                    {
                        return NotFound();
                    }

                    // Wyszukujemy rezerwacje  o wskazanym Id
                    var rezerwacja = await _context.Rezerwacja.FirstOrDefaultAsync(m => m.Id == id);
                    if (rezerwacja == null)
                    {
                        return NotFound();
                    }

                    return RedirectToAction("Details", "Rezerwacja", new { id = id });
                    break;

                // Jeśli metoda została wywołana z lisy lub mapy wypożyczeń, to 
                case "W":
                    if (id == null || _context.Wypozyczenie == null)
                    {
                        return NotFound();
                    }

                    // Wyszukujemy wypożyczenie o wskazanym Id
                    var wypozyczenie = await _context.Wypozyczenie.FirstOrDefaultAsync(m => m.Id == id);
                    if (wypozyczenie == null)
                    {
                        return NotFound();
                    }

                    return RedirectToAction("Details", "Wypozyczenie", new { id = id });
                    break;
                default:
                    return NotFound();
                    break;
            }            
        }

        // GET: UzytInterface/End/...
        // Metoda wyświetlająca kończąca...,
        // zależnie od przekazanej kategorii metoda przekierowuje do metod konkretnych kontrolerów
        public async Task<IActionResult> End(String category, int? id)
        {
            //TempData["Layout"] = "~/Views/Shared/_Layout_uzyt.cshtml";
            switch (category)
            {
                // Jeśli metoda została wywołana z lisy lub mapy rezerwacji, to 
                case "R":
                    if (id == null || _context.Rezerwacja == null)
                    {
                        return NotFound();
                    }

                    // Wyszukujemy rezerwacje  o wskazanym Id
                    var rezerwacja = await _context.Rezerwacja.FirstOrDefaultAsync(m => m.Id == id);
                    
                    // Spr czy rezerwacja istnieje
                    if (rezerwacja == null)
                    {
                        return NotFound();
                    }

                    return RedirectToAction("End", "Rezerwacja", new { id = id });
                    break;

                // Jeśli metoda została wywołana z lisy lub mapy wypożyczeń, to
                case "W":
                    if (id == null || _context.Wypozyczenie == null)
                    {
                        return NotFound();
                    }

                    // Wyszukujemy wypożyczenie  o wskazanym Id
                    var wypozyczenie = await _context.Wypozyczenie.FirstOrDefaultAsync(m => m.Id == id);

                    // Spr czy wypożyczenie istnieje
                    if (wypozyczenie == null)
                    {
                        return NotFound();
                    }

                    return RedirectToAction("End", "Wypozyczenie", new { id = id });
                    break;

                default:
                    return NotFound();
                    break;
            }
        }

        // GET: UzytInterface/Zarezerwuj/5
        // Metoda odpowidzialnia za proces zarezerwowania pojazdu
        [HttpPost]
        public async Task<IActionResult> Zarezerwuj(int? id)
        {
            // Spr czy pojazd jest dostępny 
            Task<bool> task = _pojazdService.SprawdzDostepnosc((int)id);

            // Jeśli pojazd jest dostęny
            if (task.Result)
            {
                // Tworzymy obiekt rezerwacji
                Rezerwacja rezerwacja = new Rezerwacja();

                rezerwacja.PojazdId = (int)id;
                rezerwacja.UzytkownikId = _UserManager.GetUserId(User);
                rezerwacja.DataRozpoczęcia = DateTime.Now;

                try
                {
                    // Wprowadzamy zmiany
                    _context.Add(rezerwacja);

                    // Zmieniamy status pojazdu na "Zarezerwowany"
                    await _pojazdService.ZmStatusu((int)id, "Zarezerwowany");

                    // Zapisujemy zmiany w bazie danych
                    await _context.SaveChangesAsync();


                    return RedirectToAction("Rezerwacje");
                }
                catch (Exception e)
                {
 
                    // Przekazanie komunikatu do widoku przy pomocy ViewBag lub ViewData.
                    TempData["Message"] = "Wprowadzone dane sa bledne";

                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                // Przekazanie komunikatu do widoku przy pomocy ViewBag lub ViewData.
                TempData["Message"] = "Pojazd jest niedostęny";
                //ViewBag.Message = "Pojazd jest niedostęny";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: UzytInterface/Rezerwacje/5
        // Metoda odpowidzialnia za wyświetlanie widoku rezeracji
        public async Task<IActionResult> Rezerwacje(int? page)
        {
            // Tworzymy model zawierający trzy listy wypożyczeń, rezerwacji i pozycji na mapie
            var myModel = new MyModel<RezWypDto, RezWypDto, PojazdMap>();

            
            List<RezWypDto> rezerwacjeDto = null;
            try {
                // Pierwsza lista zawiera aktywne rezerwacje 
                List<Rezerwacja> rezerwacje = await _context.Rezerwacja.Where(r => r.UzytkownikId == _UserManager.GetUserId(User)).Include(r => r.Pojazd).Where(r => r.DataZakończenia == null).ToListAsync();
                rezerwacjeDto = _mapper.Map<List<RezWypDto>>(rezerwacje);
                myModel.List1 = rezerwacjeDto;

                // Trzecia lista zawiera pozycje na mapie
                var pojazdMap = _mapper.Map< List<PojazdMap>>(rezerwacje);
                myModel.List3 = pojazdMap;

                // Druga lista zawiera zakończone rezerwacje 
                rezerwacje = await _context.Rezerwacja.Where(r => r.UzytkownikId == _UserManager.GetUserId(User)).Include(r => r.Pojazd).Where(r => r.DataZakończenia != null).ToListAsync();
                rezerwacjeDto = _mapper.Map<List<RezWypDto>>(rezerwacje);
            }
            catch (Exception ex)
            {

            }

            if (page == null || page < 1)
            {
                page = 1;
            }
            
            double ilosc = rezerwacjeDto.Count();
            double max = Math.Ceiling(ilosc / 5);

            if (page > max)
            {
                page = (int)max;
            }

            rezerwacjeDto = page<RezWypDto>((int)page, rezerwacjeDto);
            ViewBag.page = (int)page;

            myModel.List2 = rezerwacjeDto;

            return View("Rezerwacje", myModel);
        }

        // GET: UzytInterface/Wypozycz/...
        // Metoda odpowidzialnia za proces wypożyczenia pojazdu,
        // zależnie od przekazanej kategorii metoda przekierowuje do metod konkretnych kontrolerów
        [HttpPost]
        public async Task<IActionResult> Wypozycz(String category, int? id)
        {
            // Tworzymy nowy obiekt wypozyczenia 
            Wypozyczenie wypozyczenie = new Wypozyczenie();

            switch (category)
            {
                // Jeśli metoda została wywołana z lisy lub mapy rezerwacji, to 
                case "P":
                    // Nadajemy wypożyczeniu stosowne dane
                    wypozyczenie.PojazdId = (int)id;
                    wypozyczenie.UzytkownikId = _UserManager.GetUserId(User);
                    wypozyczenie.DataRozpoczęcia = DateTime.Now;
                                   
                    break;

                // Jeśli metoda została wywołana z lisy lub mapy rezerwacji, to
                case "R":
                    // Pozyskujemy Id pojazdu poprzez rezerwacje
                    Rezerwacja rezerwacja = await _context.Rezerwacja.FirstOrDefaultAsync(r => r.Id == id);
                    if(rezerwacja == null)
                    {
                        return Problem("Rezerwacja nie istnieje");
                    }

                    var user = await _UserManager.GetUserAsync(User);
                    //var userId = await _UserManager.GetUserIdAsync(user);
                    if (rezerwacja.UzytkownikId != user.Id && !User.IsInRole("Admin"))
                    {
                        return Problem("Nie masz uprawnień do przeporowadzenia akcji");
                    }

                    id = rezerwacja.PojazdId;

                    // Nadajemy wypożyczeniu stosowne dane
                    wypozyczenie.PojazdId = (int)id;
                    wypozyczenie.UzytkownikId = _UserManager.GetUserId(User);
                    wypozyczenie.DataRozpoczęcia = DateTime.Now;

                    // Zakańczamy rezerwacje poprzez nadanie daty zakończenia
                    rezerwacja.DataZakończenia = DateTime.Now;
                    _context.Update(rezerwacja);

                    break;
                default:
                    break;
            }

            try
            {
                // Wprowadzamy i zapisujemy zmiany w bazie danych 
                _context.Add(wypozyczenie);
                await _context.SaveChangesAsync();

                // Zmieniamy status pojazdu na "Wypożyczony"
                await _pojazdService.ZmStatusu((int)id, "Wypozyczony");

                // Dodajemy wpis o aktualnej pozycji pojazdu 
                Pozycja pozycja = new Pozycja();
                pozycja.WE = wypozyczenie.Pojazd.WE;
                pozycja.NS = wypozyczenie.Pojazd.NS;
                pozycja.Data = wypozyczenie.DataRozpoczęcia;
                pozycja.PojazdId = wypozyczenie.PojazdId;

                // Wprowadzamy i zapisujemy zmiany w bazie danych 
                _context.Add(pozycja);
                await _context.SaveChangesAsync();

                return RedirectToAction("Wypozyczenia");
            }
            catch (Exception e)
            {
                // Przekazanie komunikatu do widoku przy pomocy ViewBag lub ViewData.
                //ViewBag.Message = "Wprowadzone dane sa bledne";
                TempData["Message"] = "Wprowadzone dane sa bledne";
                return RedirectToAction(nameof(Index));
            }

        }

        // GET: UzytInterface/Wypozyczenia/5
        // Metoda odpowidzialnia za wyświetlanie widoku wypozyczenia
        public async Task<IActionResult> Wypozyczenia(int? page)
        {
            // Tworzymy model zawierający trzy listy wypożyczeń, rezerwacji i pozycji na mapie
            var myModel = new MyModel<RezWypDto, RezWypDto , PojazdMap>();

            // Pierwsza zawiera aktywne wypożyczenia
            var wypozyczenie = _context.Wypozyczenie.Where(r => r.UzytkownikId == _UserManager.GetUserId(User)).Include(r => r.Pojazd).Where(r => r.DataZakończenia == null);
            var wypozyczenieDto = _mapper.Map<List<RezWypDto>>(wypozyczenie);
            myModel.List1 = wypozyczenieDto;

            // Trzecia zawiera pozycje pojazdów
            var pojazdMap = _mapper.Map<List<PojazdMap>>(wypozyczenie);
            myModel.List3 = pojazdMap;

            // Druga zawiera zakończone wypożyczenia
            wypozyczenie = _context.Wypozyczenie.Where(r => r.UzytkownikId == _UserManager.GetUserId(User)).Include(r => r.Pojazd).Where(r => r.DataZakończenia != null);
            wypozyczenieDto = _mapper.Map<List<RezWypDto>>(wypozyczenie);
            
            if (page == null || page < 1)
            {
                page = 1;
            }

            double ilosc = wypozyczenieDto.Count();
            double max = Math.Ceiling(ilosc / 5);

            if (page > max)
            {
                page = (int)max;
            }

            wypozyczenieDto = page<RezWypDto>((int)page, wypozyczenieDto);
            ViewBag.page = (int)page;

            myModel.List2 = wypozyczenieDto;

            return View("Wypozyczenia", myModel);
        }

        // GET: UzytInterface/Bilans
        // Metoda wyświetlenie widoku bilansu 
        public async Task<IActionResult> Bilans()
        {
            try
            {
                // Wyszukujemy wypożyczenia, które dotyczą zalogowanego użytkownika
                var wypozyczenia = await _context.Wypozyczenie.Where(w => w.UzytkownikId  == _UserManager.GetUserId(User)).ToListAsync();
                var budzetDto = _mapper.Map<List<BudzetDto>>(wypozyczenia);

                return View(budzetDto);
            }
            catch (Exception ex)
            {
                return Problem("Coś sie zepsuło");
            }
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
