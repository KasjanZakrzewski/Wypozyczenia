using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;
using WypozyczeniaAPI.Services;
using WypozyczeniaAPI.Models;
using Humanizer;
using WypozyczeniaAPI.Models.Dto;

namespace WypozyczeniaAPI.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class SerwisInterfaceController : Controller
    {
        private readonly DBContext _context;
        private readonly IPojazdService _pojazdService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _UserManager;

        public SerwisInterfaceController(/*ILogger<HomeController> logger,*/ DBContext context, IPojazdService pojazdService, IMapper mapper, UserManager<User> UserManager, IEmailService emailService)
        {
            _context = context;
            _pojazdService = pojazdService;
            _mapper = mapper;
            _UserManager = UserManager;
            _emailService = emailService;
        }

        // GET: SerwisInterface
        // Metoda wyświetlająca główny widok,
        // zależnie od roli użytkownika
        public async Task<IActionResult> Index(int? page)
        {
            // Dla użytkowników z rolą "Admin"
            if (User.IsInRole("Admin"))
            {
                // Generujemy modlel zawierający dwie listy pojazdów
                var myModel = new MyModel<Pojazd, Pojazd>();

                // Pierwsza lista zawiera tylko dostępne pojazdy
                myModel.List1 = _context.Pojazd
                    .Where(p => p.Status == "Dostepny")
                    .ToList();

                // Druga list zawiera pojazdy, które są aktualnie serwisowane
                // Zaczynamy od wyszukania aktynych serwisów
                var lista = _context.Serwis.Include(s => s.Pojazd)
                    .Where(p => p.Pojazd.Status == "Serwis")
                    .Where(s => s.DataZakonczenia == null)
                    .Where(s => s.AdminId == _UserManager.GetUserId(HttpContext.User))
                    .ToList();

                // Wyciągamy z listy informacje dotyczące pojazdów
                List<Pojazd> PojazdList = new List<Pojazd>();
                foreach (var item in lista)
                {
                    var pojazd = new Pojazd
                    {
                        Id = item.Pojazd.Id,
                        Rejestracja = item.Pojazd.Rejestracja,
                        Model = item.Pojazd.Model,
                        Marka = item.Pojazd.Marka,
                        Status = item.Pojazd.Status
                    };
                    PojazdList.Add(pojazd);
                }

                if (page == null || page < 1)
                {
                    page = 1;
                }

                double ilosc = PojazdList.Count();
                double max = Math.Ceiling(ilosc / 5);

                if (page > max)
                {
                    page = (int)max;
                }

                PojazdList = page<Pojazd>((int)page, PojazdList);
                ViewBag.page = (int)page;

                myModel.List2 = PojazdList;
                return View("Index", myModel);
            }

            // Dla użytkowników z rolą "Employee"
            if (User.IsInRole("Employee"))
            {
                //var myModel = new MyModel<SerwisDto, SerwisDto, Pojazd>();
                // Generujemy modlel zawierający trzy listy serwisów
                var myModel = new MyModel<SerwisDto, SerwisDto, SerwisDto>();

                // Pierwsza lita zawiera aktywne serwisy,
                // przypisane do danego pracowniaka
                var list = await _context.Serwis.Include(s => s.Pojazd)
                    .Include(s => s.Pojazd)
                    .Include(s => s.Admin)
                    .Include(s => s.Pracownik)
                    .Where(m => m.PracownikId == _UserManager.GetUserId(HttpContext.User))
                    .Where(m => m.DataZakonczenia != null)
                    .ToListAsync();

                // Mapujemy do odpowiedniego Dto
                myModel.List3 = _mapper.Map<IEnumerable<SerwisDto>>(list);

                // Druga lita zawiera serwisy oczekujące
                // na zatwierdzenie przez administratora
                list = await _context.Serwis.Include(s => s.Pojazd)
                    .Include(s => s.Pojazd)
                    .Include(s => s.Admin)
                    .Include(s => s.Pracownik)
                    .Where(m => m.Status == "Rozpoczete" || m.Status == "Weryfikacja")
                    .Where(m => m.PracownikId == _UserManager.GetUserId(HttpContext.User))
                .ToListAsync();

                // Mapujemy do odpowiedniego Dto
                myModel.List1 = _mapper.Map<IEnumerable<SerwisDto>>(list);

                // Trzecia lita zawiera serwisy zlecone
                // przez administratora i oczekujące na rozpoczęcie pzrez pracowniaka
                list = await _context.Serwis.Include(s => s.Pojazd)
                    .Include(s => s.Pojazd)
                    .Include(s => s.Admin)
                    .Include(s => s.Pracownik)
                    .Where(m => m.Status == "Zlecone")
                    .Where(m => m.PracownikId == _UserManager.GetUserId(HttpContext.User))
                .ToListAsync();

                // Mapujemy do odpowiedniego Dto
                myModel.List2 = _mapper.Map<IEnumerable<SerwisDto>>(list);

                return View("Details", myModel);
            }

            // Jeśli użytkownik nie posiada żadnej z powyższych ról
            return NotFound();
        }

        // GET: SerwisInterface/PojazdDetails
        // Metoda wyświetlająca serwisy dotyczące danego pojazdu
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PojazdDetails(int? id)
        {
            // Generujemy modlel zawierający trzy listy serwisów
            var myModel = new MyModel<SerwisDto, SerwisDto, SerwisDto>();

            // Pierwsza lita zawiera aktywne serwisy,
            // przypisane do danego administratora
            var list = _context.Serwis
                .Include(s => s.Pojazd)
                .Include(s => s.Admin)
                .Include(s => s.Pracownik)
                .Where(m => m.Status == "Rozpoczete" || m.Status == "Zlecone")
                .Where(s => s.PojazdId == id)
                .Where(s => s.AdminId == _UserManager.GetUserId(HttpContext.User))
                .ToList();

            // Mapujemy do odpowiedniego Dto
            myModel.List1 = _mapper.Map<IEnumerable<SerwisDto>>(list);

            // Druga lita zawiera serwisy oczekujące na potwierdznie,
            // przypisane do danego administratora
            list = _context.Serwis
                .Include(s => s.Pojazd)
                .Include(s => s.Admin)
                .Include(s => s.Pracownik)
                .Where(m => m.Status == "Weryfikacja")
                .Where(s => s.PojazdId == id)
                .Where(s => s.AdminId == _UserManager.GetUserId(HttpContext.User))
                .ToList();

            // Mapujemy do odpowiedniego Dto
            myModel.List2 = _mapper.Map<IEnumerable<SerwisDto>>(list);

            // Trzecia lita zawiera zakończone serwisy
            list = _context.Serwis
                .Include(s => s.Pojazd)
                .Include(s => s.Admin)
                .Include(s => s.Pracownik)
                .Where(s => s.PojazdId == id)
                .Where(s => s.DataZakonczenia != null)
                .ToList();

            // Mapujemy do odpowiedniego Dto
            myModel.List3 = _mapper.Map<IEnumerable<SerwisDto>>(list);

            TempData["Id"] = id;
            if (TempData["Massage"] != null)
            {
                ViewBag.Message = TempData["Massage"];
            }
            return View("Details", myModel);
        }

        // GET: SerwisInterface/Serwis
        // Metoda wyświetlająca widok służący do przypisywania pracowników do serwisu
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Serwis(int? id)
        {
            if (id == null || _context.Pojazd == null)
            {
                return NotFound();
            }

            // Wyszukujemy pojazd o wskazanym Id
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

            // Generujemy model zawierjący listę pojazdów i użytkowników (pracowników)
            var myModel = new MyModel<Pojazd, UzytkownikDto>();
            myModel.List1 = new List<Pojazd>() { pojazd };

            // Druga lista zawiera wszystkich pracowników
            var uzytkownicy = await _context.Users.ToListAsync();
            var pracowncy = new List<User>();
            foreach (var user in uzytkownicy)
            {
                if (_UserManager.IsInRoleAsync(user, "Employee").Result)
                {
                    pracowncy.Add(user);
                }                
            }

            myModel.List2 = MyMap(pracowncy);

            if (TempData["Massage"] != null)
            {
                ViewBag.Message = TempData["Massage"];
            }

            return View(myModel);
        }
        
        [Authorize(Roles = "Admin")]
        public List<UzytkownikDto> MyMap(List<User> lista)
        {
            List<UzytkownikDto> userDTOList = new List<UzytkownikDto>();
            foreach (var user in lista)
            {
                var userDto = new UzytkownikDto
                {
                    Id = user.Id,
                    Imie = user.Imie,
                    Nazwisko = user.Nazwisko,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber
                };

                userDTOList.Add(userDto);
            }
            return userDTOList;
        }

        // GET: SerwisInterface/Przydziel/5...
        // Metoda przydzielająca pracownika do serwisu
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Przydziel(string Pracownikid, int PojazdId)
        {
            // Tworzymy nowy obiekt serwisu,
            // ze statusem "Zlecone" i aktualną datą
            Serwis serwis = new Serwis();
            serwis.PojazdId = PojazdId;
            serwis.AdminId = _UserManager.GetUserId(HttpContext.User);
            serwis.PracownikId = Pracownikid;
            serwis.DataRozpoczecia = DateTime.Now;
            serwis.Status = "Zlecone";

            try
            {
                
                _context.Add(serwis);
                // Zmieniamy status pojazdu na "Serwis"
                await _pojazdService.ZmStatusu(PojazdId, "Serwis");

                // Zapisujemy zmiany w bazie danych
                await _context.SaveChangesAsync();

                // Generujemy email z powiadomieniem dla pracownika 
                var user = await _UserManager.GetUserAsync(HttpContext.User);
                var pojazd = await _context.Pojazd.Where(x => x.Id == serwis.PojazdId).FirstOrDefaultAsync();
                var massage = "Zostałeś przydzielony do serwisu:\n Marka: " + pojazd.Marka + "\nModel:" + pojazd.Model + "\nRejestracja: " + pojazd.Rejestracja;

                // Wysłanmy email z powiadomieniem dla pracownika 
                _emailService.SendEmail(user.Email, "Przydzielnie do Serwsiu", massage);

                TempData["Massage"] = "Przydzielono pracownika do serwisu";
                return RedirectToAction("PojazdDetails", new { id = PojazdId });
            }
            catch (Exception e)
            {
                TempData["Massage"] = "Nie wprowadzono wszystkich zmian z powodu błędu";
                return RedirectToAction("Serwis", PojazdId);
            }
        }

        // GET: SerwisInterface/Odrzuc/5...
        // Metoda pozwala odzrzucić prace zgłoszone przez pracownika
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Odrzuc(int? Id)
        {
            // Wyszukujemy serwis o wskazanym Id
            Serwis serwis = _context.Serwis.Where(s => s.Id == Id).FirstOrDefault();
            if (serwis == null)
            {
                return Problem("Serwis nie istnieje");
            }

            // Spr czy pracownik może sie tym zająć 
            if (serwis.Status != "Zakończony")
            {
                return Problem("Serwisem nie da sie już zająć");
            }

            // Zmieniamy status serwisu na "Rozpoczęte"
            serwis.Status = "Rozpoczete";

            // Dodajemy opis systemowy o wznowieniu prac przez administratora
            var opis = new Opis();
            opis.Typ = "System";
            opis.Tytul = "Wznowienie prac: Admin";
            opis.Data = DateTime.Now;
            opis.SerwisId = (int)Id;

            try
            {
                // Wprowadzamy i zapisujemy zmiany w bazie danych 
                _context.Update(serwis);
                _context.Add(opis);
                await _context.SaveChangesAsync();

                // Generujemy email z powiadomieniem dla pracownika 
                var user = await _context.Users.Where(u => u.Id == serwis.PracownikId).FirstOrDefaultAsync();
                var pojazd = await _context.Pojazd.Where(x => x.Id == serwis.PojazdId).FirstOrDefaultAsync();
                var massage = "Twój serwis został odrzucony, dotyczy pojazdu :\n Marka: " + pojazd.Marka + "\nModel:" + pojazd.Model + "\nRejestracja: " + pojazd.Rejestracja;

                // Wysłanmy email z powiadomieniem dla pracownika 
                _emailService.SendEmail(user.Email, "Odrzucenie prac serwsowych", massage);

                TempData["Massage"] = "Odrzucono wykonane prace serwisowe";
                return RedirectToAction("PojazdDetails", new { id = serwis.PojazdId });
            }
            catch (Exception e)
            {
                TempData["Massage"] = "Nie wprowadzono wszystkich zmian z powodu błędu";
                return RedirectToAction("Serwis", serwis.PojazdId);
            }
        }

        // GET: SerwisInterface/Rozpocznij/5...
        // Metoda pozwala udokumentować rozpoczęcie prac przez pracownika
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Rozpocznij(int? Id)
        {
            // Wyszukujemy serwis o wskazanym Id
            Serwis serwis = _context.Serwis.Where(s => s.Id == Id).FirstOrDefault();
            if (serwis == null)
            {
                return Problem("Serwis nie istnieje");
            }

            // Spr czy pracownik może sie tym zająć 
            if(serwis.Status != "Zlecone")
            {
                return Problem("Serwisem nie da sie już zająć");
            }

            // Zmieniamy status serwisu na "Rozpoczęte"
            serwis.Status = "Rozpoczete";

            // Dodajemy opis systemowy o Rozpoczęciu prac przez pracownika
            var opis = new Opis();
            opis.Typ = "System";
            opis.Tytul = "Rozpoczęcie prac: Pracownik";
            opis.Data = DateTime.Now;
            opis.SerwisId = (int)Id;

            try
            {
                // Wprowadzamy i zapisujemy zmiany w bazie danych 
                _context.Update(serwis);
                _context.Add(opis);
                await _context.SaveChangesAsync();

                // Generujemy email z powiadomieniem dla pracownika 
                var user = await _context.Users.Where(u => u.Id == serwis.AdminId).FirstOrDefaultAsync();
                var pojazd = await _context.Pojazd.Where(x => x.Id == serwis.PojazdId).FirstOrDefaultAsync();
                var massage = "Pracownik rozpoczął prace nad serwisem, dotyczy pojazdu :\nMarka: " + pojazd.Marka + "\nModel:" + pojazd.Model + "\nRejestracja: " + pojazd.Rejestracja;

                // Wysłanmy email z powiadomieniem dla pracownika 
                _emailService.SendEmail(user.Email, "Rozpoczęcie prac serwsowych", massage);

                TempData["Massage"] = "Rozpoczęto prace serwisowe";
                return RedirectToAction("Serwis", serwis.PojazdId); ;
            }
            catch (Exception e)
            {
                TempData["Massage"] = "Nie wprowadzono wszystkich zmian z powodu błędu";
                return RedirectToAction("Serwis", serwis.PojazdId);
            }
        }

        // GET: SerwisInterface/SerwisDetails/5...
        // Metoda wyświetlająca widok detali Serwisu
        // Both
        public async Task<IActionResult> SerwisDetails(int? id)
        {
            // Tworzymy model zawierający obiekty serwisu, pojadu i liste opisów
            var myModel = new MyModelSingle<SerwisDto, Pojazd, List<Opis>>();

            // Pobieramy informacje dotyczące serwisu
            var serwis = await _context.Serwis
                .Include(s => s.Admin)
                .Include(s => s.Pracownik)
                .FirstAsync(m => m.Id == id);

            // Pobieramy informacje dotyczące pojazdu przypiasnego do serwisu
            var serwisPlus = await _context.Serwis
                .Include(p => p.Pojazd)
                .FirstAsync(m => m.Id == id);

            var pojazd = new Pojazd
            {
                Id = serwisPlus.Pojazd.Id,
                Rejestracja = serwisPlus.Pojazd.Rejestracja,
                Model = serwisPlus.Pojazd.Model,
                Marka = serwisPlus.Pojazd.Marka,
                Status = serwisPlus.Pojazd.Status
            };
            
            // Pobieramy liste opisów dodanych dotyczących pojazdu
            var opisy = _context.Opis.Where(o => o.SerwisId == id).ToList();

            myModel.Obj1 = _mapper.Map<SerwisDto>(serwis);
            myModel.Obj2 = pojazd;
            myModel.Obj3 = opisy;

            TempData["Id"] = serwis.Id;

            return View(myModel);
        }

        // GET: SerwisInterface/AddOpis/5...
        // Metoda służąca do dodania opisy do serwisu
        // Both
        public async Task<IActionResult> AddOpis(int? id)
        {
            // Zależnie od roli typ opisu się zmienia
            if (User.IsInRole("Admin"))
            {
                TempData["Typ"] = "Uwaga";
            }
            if (User.IsInRole("Employee"))
            {
                TempData["Typ"] = "Opis";
            }

            // Data dodania opisu zostaje ustawiony na aktualną godzine systemu
            TempData["Data"] = DateTime.Now;
            TempData["SerwisId"] = id;

            return RedirectToAction("Create", "Opis");
        }

        // GET: SerwisInterface/End/5...
        // Metoda służąca do udokumentowania zakończenia prac serwisowych 
        // Both
        public async Task<IActionResult> End(int? id)
        {
            // Wyszukujemy serwis o podanym Id
            var serwis = await _context.Serwis.FirstAsync(m => m.Id == id);

            // Spr czy serwis istnieje
            if (serwis == null)
            {
                return NotFound();
            }

            // Tworzymy nowy opis
            var opis = new Opis();
            
            // Ustawiamy typ opisu na "System"
            opis.Typ = "System";

            // Jeżeli użytkownik kończący serwis a role "Admin"
            if (User.IsInRole("Admin"))
            {
                // Data zakończenia ustawiany jest na aktualną date systemową
                serwis.DataZakonczenia = DateTime.Now;
                
                // Status Serwsiu zostaje ustawiona na "Zakończony"
                serwis.Status = "Zakończony";
                
                // Treść zostaje ustawiona odpowiednia treść opisu 
                opis.Tytul = "Zakończenie: Admin";
                opis.Data = DateTime.Now;
                opis.SerwisId = (int)id;
            }

            if (User.IsInRole("Employee"))
            {
                // Status Serwsiu zostaje ustawiona na "Zakończony"
                serwis.Status = "Weryfikacja";
                
                // Treść zostaje ustawiona odpowiednia treść opisu 
                opis.Tytul = "Zakończenie: Pracownik";
                opis.Data = DateTime.Now;
                opis.SerwisId = (int)id;
            }
            
            try
            {
                // Wprowadzamy zmiany w bazie danych 
                _context.Update(serwis);
                _context.Add(opis);

                if (User.IsInRole("Admin"))
                {
                    // Zmieniamy status pojazdu 
                    await _pojazdService.ZmStatusu(serwis.PojazdId, "Dostepny");
                }
                else
                {
                    // Generujemy email z powiadomieniem dla pracownika 
                    var user = await _context.Users.Where(u => u.Id == serwis.AdminId).FirstOrDefaultAsync();
                    var pojazd = await _context.Pojazd.Where(x => x.Id == serwis.PojazdId).FirstOrDefaultAsync();
                    var massage = "Pracownik zakończył prace nad serwisem, dotyczy pojazdu :\nMarka: " + pojazd.Marka + "\nModel:" + pojazd.Model + "\nRejestracja: " + pojazd.Rejestracja;
                    
                    // Wysłanmy email z powiadomieniem dla pracownika 
                    _emailService.SendEmail(user.Email, "Zgłoszenie prac serwsowych", massage);
                }

                // Zapisujemy zmiany w bazie danych
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                await _pojazdService.ZmStatusu(serwis.PojazdId, "Błąd serwis");
            }

            return RedirectToAction("SerwisDetails", new { id = opis.SerwisId });
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
