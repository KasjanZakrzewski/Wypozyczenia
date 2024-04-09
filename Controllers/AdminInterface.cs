using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;
using WypozyczeniaAPI.Models.Dto;
using WypozyczeniaAPI.Services;
using static System.Formats.Asn1.AsnWriter;

namespace WypozyczeniaAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminInterface : Controller
    {
        private readonly DBContext _context;
        private readonly IPojazdService _pojazdService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _UserManager;

        public AdminInterface(DBContext context, IPojazdService pojazdService, IMapper mapper, UserManager<User> UserManager)
        {
            _context = context;
            _pojazdService = pojazdService;
            _mapper = mapper;
            _UserManager = UserManager;
        }

        // GET: AdminInterface
        public async Task<IActionResult> Index()
        {
            // Obiekt modelu, który będzie przekazany do widoku 
            var myModel = new MyModel<Pojazd, Pojazd, Pojazd, Pojazd, Pojazd>();

            // Listy pojazdów o statusach kolejno Dostępny, Serwis, Zarezerwowany, Wypozyczony
            var dostepne = await _context.Pojazd.Where(p => p.Status == "Dostepny").ToListAsync();
            myModel.List1 = dostepne;
            var serwis = await _context.Pojazd.Where(p => p.Status == "Serwis").ToListAsync();
            myModel.List2 = serwis;
            var zarezerwowane = await _context.Pojazd.Where(p => p.Status == "Zarezerwowany").ToListAsync();
            myModel.List3 = zarezerwowane;
            var wypozyczone = await _context.Pojazd.Where(p => p.Status == "Wypozyczony").ToListAsync();
            myModel.List4 = wypozyczone;

            return View(myModel);
        }

        // GET: AdminInterface/DodajRole/5...
        // Metoda odpowiedzialna za wyświetlenia widoku potwierdzenia nadania roli 
        public IActionResult DodajRole(string? id, string? rola)
        {
            if(id == null)
            {
                return NoContent();
            }

            // Wyszukujemy użytkownika o podanym Id w systemie
            var user = _context.Users.Where(p => p.Id == id).FirstOrDefault();

            // Spr czy użytkownik o podanym Id istnieje,
            // jeśli nie to kończymy działanie metody
            if (user == null)
            {
                return NoContent();
            }

            // Przekazujemy role do widoku, przy pomocy ViewBag
            ViewBag.rola = (string)rola;
            return View(user);
        }

        // GET: AdminInterface/Dodaj/5...
        // Metoda HTTP Post, która odpowiada za dodnie roli użytkownikowi
        [HttpPost]
        public async Task<IActionResult> Dodaj(string? id, string? rola)
        {
            if(rola == null || id == null)
            {
                return NoContent();
            }

            // Wyszukujemy użytkownika o podanym Id w systemie
            var user = _context.Users.Where(p => p.Id == id).FirstOrDefault();

            // Spr czy użytkownik o podanym Id istnieje,
            // jeśli nie to kończymy działanie metody
            if (user == null)
            {
                return NoContent();
            }

            // Wyszukujemy role przydzielone do użytkownika
            var roles = await _UserManager.GetRolesAsync(user);        

            // Spr czy użytkownik posiada już wskazaną role 
            if (roles.Contains(rola))
            {
                return Problem("Użytkownik ma już zadaną role");
            }
            else
            {
                // <|spr czy działa na sensownych danych|>
                foreach (var role in roles)
                {
                    await _UserManager.RemoveFromRoleAsync(user, role);
                }

                // Nadajemy użytkownikowi role
                await _UserManager.AddToRoleAsync(user, rola);
            }

            TempData["Massage"] = "Nadano role";
            return RedirectToAction("Role");
        }

        // GET: AdminInterface/Role
        // Metoda odpowiedzialna za wyświetlenia widoku nadawania roli 
        public async Task<IActionResult> Role()
        {
            // Obiekt modelu, który będzie przekazany do widoku 
            var myModel = new MyModel<UzytkownikDto, UzytkownikDto>();

            // Pobieramy listy pracowników "Employee" i użytkowników "User"
            var employees = await _UserManager.GetUsersInRoleAsync("Employee");
            var usersInRole = await _UserManager.GetUsersInRoleAsync("User");

            myModel.List1 = _mapper.Map<List<UzytkownikDto>>((List<User>)employees);
            myModel.List2 = _mapper.Map<List<UzytkownikDto>>((List<User>)usersInRole);

            if (TempData["Massage"] != null)
            {
                ViewBag.Message = TempData["Massage"];
            }

            return View(myModel);
        }

        // Metoda służaca mapowaniu obiektów klasu User na UzytkownikDto
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
    }
}
