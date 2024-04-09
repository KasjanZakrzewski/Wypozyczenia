using AutoMapper;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Models;
using WypozyczeniaAPI.Models.Dto;

namespace WypozyczeniaAPI.Prof
{
    public class WypozyczalniaProfil : Profile
    {
        public WypozyczalniaProfil()
        {
            CreateMap<PozycjaDto, Pozycja>();
            CreateMap<Pozycja, PozycjaDto>();
            CreateMap<UzytkownikDto, User>();
            CreateMap<Wypozyczenie, Pojazd>();
            CreateMap<Rezerwacja, Pojazd>();
            CreateMap<User, UzytkownikDto>()
                .ForMember(d => d.Id, u => u.MapFrom(c => c.Id))
                .ForMember(d => d.Imie, u => u.MapFrom(c => c.Imie))
                .ForMember(d => d.Nazwisko, u => u.MapFrom(c => c.Nazwisko))
                .ForMember(d => d.Email, u => u.MapFrom(c => c.Email))
                .ForMember(d => d.UserName, u => u.MapFrom(c => c.UserName))
                .ForMember(d => d.PhoneNumber, u => u.MapFrom(c => c.PhoneNumber));
            CreateMap<Rezerwacja, RezWypDto>()
                .ForMember(r => r.Marka, c => c.MapFrom(s => s.Pojazd.Marka))
                .ForMember(r => r.Model, c => c.MapFrom(s => s.Pojazd.Model));
            CreateMap<Wypozyczenie, RezWypDto>()
                .ForMember(r => r.Marka, c => c.MapFrom(s => s.Pojazd.Marka))
                .ForMember(r => r.Model, c => c.MapFrom(s => s.Pojazd.Model));
            CreateMap<Serwis, SerwisDto>()
                .ForMember(d => d.Marka, c => c.MapFrom(s => s.Pojazd.Marka))
                .ForMember(d => d.Model, c => c.MapFrom(s => s.Pojazd.Model))
                .ForMember(d => d.Rejestracja, c => c.MapFrom(s => s.Pojazd.Rejestracja))
                .ForMember(d => d.Admin, c => c.MapFrom(s => s.Admin.UserName))
                .ForMember(d => d.Pracownik, c => c.MapFrom(s => s.Pracownik.UserName));
            CreateMap<Rezerwacja, WypoRezDto2>()
                .ForMember(d => d.Marka, c => c.MapFrom(s => s.Pojazd.Marka))
                .ForMember(d => d.Model, c => c.MapFrom(s => s.Pojazd.Model))
                .ForMember(d => d.Rejestracja, c => c.MapFrom(s => s.Pojazd.Rejestracja))
                .ForMember(d => d.Email, c => c.MapFrom(s => s.Uzytkownik.Email));
            CreateMap<Wypozyczenie, WypoRezDto2>()
                .ForMember(d => d.Marka, c => c.MapFrom(s => s.Pojazd.Marka))
                .ForMember(d => d.Model, c => c.MapFrom(s => s.Pojazd.Model))
                .ForMember(d => d.Rejestracja, c => c.MapFrom(s => s.Pojazd.Rejestracja))
                .ForMember(d => d.Email, c => c.MapFrom(s => s.Uzytkownik.Email));

            CreateMap<Rezerwacja, PojazdMap>()
                .ForMember(d => d.Id, c => c.MapFrom(s => s.Pojazd.Id))
                .ForMember(d => d.RezerwacjaId, c => c.MapFrom(s => s.Id))
                .ForMember(d => d.WypozyczenieId, c => c.MapFrom(s => s.Pojazd.Id)) // tu null powinien być
                .ForMember(d => d.Marka, c => c.MapFrom(s => s.Pojazd.Marka))
                .ForMember(d => d.Model, c => c.MapFrom(s => s.Pojazd.Model))
                .ForMember(d => d.NS, c => c.MapFrom(s => s.Pojazd.NS))
                .ForMember(d => d.WE, c => c.MapFrom(s => s.Pojazd.WE))
                .ForMember(d => d.Rejestracja, c => c.MapFrom(s => s.Pojazd.Rejestracja)); 

            CreateMap<Wypozyczenie, PojazdMap>()
                .ForMember(d => d.Id, c => c.MapFrom(s => s.Pojazd.Id))
                .ForMember(d => d.WypozyczenieId, c => c.MapFrom(s => s.Id))
                .ForMember(d => d.RezerwacjaId, c => c.MapFrom(s => s.Pojazd.Id)) // tu null powinien być
                .ForMember(d => d.Marka, c => c.MapFrom(s => s.Pojazd.Marka))
                .ForMember(d => d.Model, c => c.MapFrom(s => s.Pojazd.Model))
                .ForMember(d => d.NS, c => c.MapFrom(s => s.Pojazd.NS))
                .ForMember(d => d.WE, c => c.MapFrom(s => s.Pojazd.WE))
                .ForMember(d => d.Rejestracja, c => c.MapFrom(s => s.Pojazd.Rejestracja));

            CreateMap<Wypozyczenie, BudzetDto>()
                .ForMember(d => d.Data, c => c.MapFrom(s => s.DataRozpoczęcia));

            CreateMap<Wypozyczenie, WypoRaportDto>()
                .ForMember(d => d.Marka, c => c.MapFrom(s => s.Pojazd.Marka))
                .ForMember(d => d.Model, c => c.MapFrom(s => s.Pojazd.Model))
                .ForMember(d => d.Rejestracja, c => c.MapFrom(s => s.Pojazd.Rejestracja))
                .ForMember(d => d.Email, c => c.MapFrom(s => s.Uzytkownik.Email));
        }
    }
}
