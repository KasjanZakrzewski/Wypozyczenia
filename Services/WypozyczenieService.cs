using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models.Dto;
using WypozyczeniaAPI.Models;
using AutoMapper;
using System;

namespace WypozyczeniaAPI.Services
{
    public interface IWypozyczenieService
    {
        // Metoda służąca do sprawdzenia czy wypożyczenie jest aktywne
        bool sprAktywność(int id);

        // Metoda zwraca dystans przebyty podczas wypożyczenia 
        Task<double> kilometraz(Wypozyczenie wypozyczenie);
    }

    public class WypozyczenieService : IWypozyczenieService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;

        public WypozyczenieService(DBContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // Metoda służąca do sprawdzenia czy wypożyczenie jest aktywne
        public bool sprAktywność(int id)
        {
            // Wyszukujemy wypożyczenie po Id
            var wypozyczenie = _context.Wypozyczenie.FirstOrDefault(r => r.Id == id);

            // Spr czy rezerwacja istnieje 
            if (wypozyczenie == null)
            {
                return false;
            }

            // Spr czy data zakończenia jest wpisana
            if (wypozyczenie.DataZakończenia == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        // Metoda zwraca dystans przebyty podczas wypożyczenia
        public async Task<double> kilometraz(Wypozyczenie wypozyczenie)
        {
            var zakData = DateTime.Now;
            if (wypozyczenie.DataZakończenia != null)
            {
                zakData = (DateTime)wypozyczenie.DataZakończenia;
            }

            var pozycje = await _context.Pozycja.Where(p => p.Data >= wypozyczenie.DataRozpoczęcia && p.Data <= zakData).OrderBy(p => p.Data).ToListAsync();
            var pozycjeDto = _mapper.Map<List<PozycjaDto>>(pozycje);

            double temp;
            double radius = 6371.0;
            float radian = (float)Math.PI/180;
            float postWE = pozycjeDto[0].WE * radian;
            float postNS = pozycjeDto[0].NS * radian;
            float dystWE = 0;
            float dystNS = 0;
            double dystans = 0;

            foreach (PozycjaDto pozycja in pozycjeDto)
            {
                pozycja.WE *= radian;
                pozycja.NS *= radian;


                dystWE = pozycja.WE - postWE;
                dystNS = pozycja.NS - postNS;

                temp = Math.Pow(Math.Sin(dystNS/2),2) + Math.Cos(postNS)* Math.Cos(pozycja.NS)* Math.Pow(Math.Sin(dystWE / 2), 2);
                var c = 2 * Math.Atan2(Math.Sqrt(temp), Math.Sqrt(1 - temp));
                dystans += c;

                postWE = pozycja.WE;
                postNS = pozycja.NS;
            }
            
            dystans *= radius;
            // wydaje mi sie że źle zaokrągla
            dystans = Math.Round(dystans, 5);
            return dystans;
        }
    }
}
