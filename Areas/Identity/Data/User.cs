using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WypozyczeniaAPI.Models;

namespace WypozyczeniaAPI.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    [PersonalData]
    public String? Imie { get; set; }
    [PersonalData]
    public String? Nazwisko { get; set; }
    [PersonalData]
    public String NumerKarty { get; set; }
    [PersonalData]
    public String CVC { get; set; }

    // odwołanie, inna tabla odwołuje sie do tej 
    [ValidateNever]
    public virtual List<Wypozyczenie> Wypozyczenie { get; set; }
    [ValidateNever]
    public virtual List<Rezerwacja> Rezerwacja { get; set; }
}

