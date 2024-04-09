using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WypozyczeniaAPI.Areas.Identity.Data;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;

namespace WypozyczeniaAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UzytkownikController : Controller
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        public UzytkownikController(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

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

        public UzytkownikDto MyMap(User user)
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
            return userDto;
        }

        // GET: Uzytkownik
        public async Task<IActionResult> Index()
        {
            List<UzytkownikDto> userDTOList = MyMap(await _context.Users.ToListAsync());
            
            return _context.Users != null ? 
                          View(userDTOList) :
                          Problem("Entity set 'DBContext.UzytkownikDto'  is null.");
        }

        // GET: Uzytkownik/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.UzytkownikDto == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            var uzytkownikDto = MyMap(user);

            if (uzytkownikDto == null)
            {
                return NotFound();
            }

            return View(uzytkownikDto);
        }

        // GET: Uzytkownik/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Uzytkownik/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Imie,Nazwisko,Email,UserName,PhoneNumber")] UzytkownikDto uzytkownikDto)
        {
            if (ModelState.IsValid)
            {

                _context.Add(uzytkownikDto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uzytkownikDto);
        }

        // GET: Uzytkownik/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.UzytkownikDto == null)
            {
                return NotFound();
            }

            var uzytkownikDto = await _context.UzytkownikDto.FindAsync(id);
            if (uzytkownikDto == null)
            {
                return NotFound();
            }
            return View(uzytkownikDto);
        }

        // POST: Uzytkownik/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Imie,Nazwisko,Email,UserName,PhoneNumber")] UzytkownikDto uzytkownikDto)
        {
            if (id != uzytkownikDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uzytkownikDto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UzytkownikDtoExists(uzytkownikDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(uzytkownikDto);
        }

        // GET: Uzytkownik/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.UzytkownikDto == null)
            {
                return NotFound();
            }

            var uzytkownikDto = await _context.UzytkownikDto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (uzytkownikDto == null)
            {
                return NotFound();
            }

            return View(uzytkownikDto);
        }

        // POST: Uzytkownik/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.UzytkownikDto == null)
            {
                return Problem("Entity set 'DBContext.UzytkownikDto'  is null.");
            }
            var uzytkownikDto = await _context.UzytkownikDto.FindAsync(id);
            if (uzytkownikDto != null)
            {
                _context.UzytkownikDto.Remove(uzytkownikDto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UzytkownikDtoExists(string id)
        {
          return (_context.UzytkownikDto?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
