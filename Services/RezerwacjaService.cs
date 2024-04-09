using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Data;


namespace WypozyczeniaAPI.Services
{
    public interface IRezerwacjaService
    {
        // Metoda służąca do sprawdzenia czy rezerwacja jest aktywna
        bool sprAktywność(int id);

        // Metoda służąca do zakańczania rezerwacji
        void endRezerwacja(int id);
    }

    public class RezerwacjaService : IRezerwacjaService
    {
        private readonly DBContext _context;
        private readonly UserManager<User> _UserManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RezerwacjaService(DBContext context, UserManager<User> UserManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _UserManager = UserManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // Metoda służąca do sprawdzenia czy rezerwacja jest aktywna
        public bool sprAktywność(int id)
        {
            // Wyszukujemy rezerwacje po Id
            var rezerwacja = _context.Rezerwacja.FirstOrDefault(r => r.Id == id);

            // Spr czy rezerwacja istnieje  
            if (rezerwacja == null)
            {
                return false;
            }

            // Spr czy data zakończenia jest wpisana
            if (rezerwacja.DataZakończenia == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        // Metoda służąca do zakańczania rezerwacji
        public async void endRezerwacja(int id)
        {
            // Rozpoczynamy tranzakcje
            using (var transaction = _context.Database.BeginTransaction())
            {
                // Wyszukujemy rezerwacje po Id
                var rezerwacja = _context.Rezerwacja.Where(r => r.DataZakończenia == null).FirstOrDefault(r => r.PojazdId == id);
                try
                {
                    if (rezerwacja != null)
                    {
                        var user = await _UserManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                        if(rezerwacja.UzytkownikId != user.Id ||!_UserManager.IsInRoleAsync(user,"Admin").Result)
                        {
                            transaction.Rollback();
                            return;
                        }
                        // Ustawiamy datę zakończenia na aktualną date w systemie
                        rezerwacja.DataZakończenia = DateTime.Now;
                        // Wprowadzamy i zapisujemy zmiany w bazie danych
                        _context.Update(rezerwacja);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    transaction.Rollback();
                }
                catch (DbUpdateConcurrencyException)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
