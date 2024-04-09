using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;

//<|do usunięcia|>
namespace WypozyczeniaAPI.Services
{
    public interface IPageService
    {

    }

    public class PageService : IPageService
    {
        
        public PageService(DBContext context)
        {
           
        }

        public async Task<List<T>> page<T>(int page, List<T> dane)
        {
            dane = dane.Take(page * 10).ToList();

            int ilosc = dane.Count();

            // int cale = ilosc - ilosc % 10;
            if (ilosc >= 10)
            {
                dane = dane.TakeLast(10).ToList();
            }

            return dane;
        }

    }
}
