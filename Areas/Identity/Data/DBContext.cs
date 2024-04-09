using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Models;

namespace WypozyczeniaAPI.Data;

public class DBContext : IdentityDbContext<User>
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    // odowłania do tablic w bazie danych systemu
    public DbSet<WypozyczeniaAPI.Models.Pojazd>? Pojazd { get; set; }
    public DbSet<WypozyczeniaAPI.Areas.Identity.Data.User>? Uzytkownik { get; set; }
    public DbSet<WypozyczeniaAPI.Models.Rezerwacja>? Rezerwacja { get; set; }
    public DbSet<WypozyczeniaAPI.Models.Wypozyczenie>? Wypozyczenie { get; set; }
    public DbSet<WypozyczeniaAPI.Models.Pozycja>? Pozycja { get; set; }
    public DbSet<WypozyczeniaAPI.Models.Serwis>? Serwis { get; set; }
    public DbSet<WypozyczeniaAPI.Models.Opis>? Opis { get; set; }
    public DbSet<UzytkownikDto>? UzytkownikDto { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
