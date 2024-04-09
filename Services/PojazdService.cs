using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;

namespace WypozyczeniaAPI.Services
{
    public interface IPojazdService
    {
        // Metoda sprawdza czy pojazd jest dostępny
        Task<bool> SprawdzDostepnosc(int id);

        // Metoda sprawdza czy pojazd jest zarezerwowany
        Task<bool> sprRezerwacje(int id);

        // Metoda pozwala na zmiany statusu pojazdu na wzkazany
        Task<bool> ZmStatusu(int id, string stat);

        // Metoda generujaca skrót z podanego ciągu znaków
        string Hash(string input);

        // Metoda służąca do sprawdzenia czy pojazd o wskazanym Id, posiada wskazany hash
        Task<bool> sprHash(int id, string hash);

        // Metoda pozwala na zmiane pozycji pojazdu na wzkazaną
        Task<bool> ZmPozycji(int id, float NS, float WE);

        // Metoda generująca liste pojazdów zawierające wskazane Id
        Task<List<Pojazd>> PozycjaPojazdow(List<int> id);
    }

    public class PojazdService : IPojazdService
    {
        private readonly DBContext _context;

        public PojazdService(DBContext context)
        {
            _context = context;
        }

        private bool PojazdExists(int id)
        {
            return (_context.Pojazd?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // Metoda sprawdza czy pojazd jest dostępny
        public async Task<bool> SprawdzDostepnosc(int id)
        {
            try
            {
                // Wyszukujemy pojazd o wskazanym Id
                Pojazd pojazd = await _context.Pojazd.FirstOrDefaultAsync(r => r.Id == id);

                // Spr czy pojazd istnieje 
                if (pojazd == null)
                {
                    return false;
                }

                // Spr czy status pojazdu jest ustawiony na dostęny 
                if (pojazd.Status == "Dostepny")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Metoda sprawdza czy pojazd jest zarezerwowany
        public async Task<bool> sprRezerwacje(int id)
        {
            // Wyszukujemy pojazd o wskazanym Id
            var pojazd = _context.Pojazd.FirstOrDefault(r => r.Id == id);

            // Spr czy pojazd istnieje 
            if (pojazd == null)
            {
                return false;
            }

            // Spr czy status pojazdu jest ustawiony na dostęny 
            if (pojazd.Status == "Zarezerwowany")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        // Metoda pozwala na zmiany statusu pojazdu na wzkazany
        public async Task<bool> ZmStatusu(int id, String stat)
        {
            // Rozpoczynamy tranzakcje na bazie danych
            using (var transaction = _context.Database.BeginTransaction())
            {
                Pojazd pojazd = null;
                try
                {
                    // Wyszukujemy pojazd o wskazanym Id
                    pojazd = _context.Pojazd.FirstOrDefault(r => r.Id == id);
                    
                    if (pojazd == null)
                    {
                        return false;
                    }
                    // Zmieniamy status pojazdu na podany
                    pojazd.Status = stat;

                    // Wprowadzamy i zapisujemy zmiany w bazie danych 
                    _context.Update(pojazd);
                    _context.SaveChanges();

                    // Zakańczamy tranzakcje na bazie danych          
                    transaction.Commit();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Wycofujemy się z tranzakcji na bazie danych   
                    transaction.Rollback();

                    return false;
                }
            }
        }

        // Metoda pozwala na zmiane pozycji pojazdu na wzkazaną
        public async Task<bool> ZmPozycji(int id, float NS, float WE)
        {
            // Rozpoczynamy tranzakcje na bazie danych
            using (var transaction = _context.Database.BeginTransaction())
            {
                Pojazd pojazd = null;
                try
                {
                    // Wyszukujemy pojazd o wskazanym Id
                    pojazd = _context.Pojazd.FirstOrDefault(r => r.Id == id);

                    if (pojazd == null)
                    {
                        return false;
                    }

                    // Zmieniamy współżędne pojazdu na podane
                    pojazd.NS = NS;
                    pojazd.WE = WE;

                    // Wprowadzamy i zapisujemy zmiany w bazie danych 
                    _context.Update(pojazd);
                    _context.SaveChanges();

                    // Zakańczamy tranzakcje na bazie danych  
                    transaction.Commit();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Wycofujemy się z tranzakcji na bazie danych   
                    transaction.Rollback();
                    return false;
                }
            }
        }

        // Metoda generujaca skrót z podanego ciągu znaków
        public string Hash(string input)
        {
            // Tworzenie instancji SHA-256
            using (SHA256 sha256 = SHA256.Create())
            {
                // Konwertowanie tekstu na bajty
                byte[] bajty = Encoding.UTF8.GetBytes(input);

                // Obliczanie hasza
                byte[] hash = sha256.ComputeHash(bajty);

                // Konwersja bajtów hasza na string w formacie heksadecymalnym
                string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                Console.WriteLine("Input: " + input);
                Console.WriteLine("SHA-256 Hash: " + hashString);

                return hashString;
            }
            return null;
        }

        // Metoda służąca do sprawdzenia czy pojazd o wskazanym Id, posiada wskazany hash
        public async Task<bool> sprHash(int id, string hash)
        {
            // Wyszykujemy pojazd o wskazanym Id
            var pojazd = _context.Pojazd.FirstOrDefault(r => r.Id == id);

            // Spr czy pojazd istnieje 
            if (pojazd == null)
            {
                return false;
            }

            // Spr czy status pojazdu jest ustawiony na dostęny 
            if (pojazd.Hash == hash)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        // Metoda generująca liste pojazdów zawierające wskazane Id
        public async Task<List<Pojazd>> PozycjaPojazdow(List<int> id)
        {
            Pojazd pojazd = null;
            List<Pojazd> outPojazd = new List<Pojazd>();

            // Dla każdego Id z listy: 
            foreach (var item in id)
            {
                // Wyszukujemy pojazd o danym Id
                pojazd = await _context.Pojazd.FirstOrDefaultAsync(r => r.Id == item);
                if (pojazd != null)
                {
                    // Jeśli pojazd istnieje to dodajemy go do listy wyjściowej
                    outPojazd.Add(pojazd);
                }
            }

            return outPojazd;
        }
    }
}
