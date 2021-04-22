using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Context;
using BookStore.Models;
using BookStore.Util;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BookStore.ViewModel;


namespace BookStore.Controllers
{
    public class UsersController : Controller
    {
        private readonly BookStoreContext _context;

        public UsersController(BookStoreContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var bookStoreContext = _context.Users.Include(u => u.Role);
            return View(await bookStoreContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Dob,Email,Passwordhash,Address,Roleid")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid", user.Roleid);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid", user.Roleid);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Dob,Email,Passwordhash,Address,Roleid")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid", user.Roleid);
            return View(user);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model)
        {    
            var roles = await _context.Roles.Where(i => i.Roletype == "Customer").FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                var client= new User(){
                    Id = Guid.NewGuid(),
                    Name = model.FullName,
                    Dob = model.DoB,
                    Address = model.Address,
                    Email =model.Email,
                    Passwordhash = Security.Encrypt(model.Password),
                    Roleid = roles.Roleid
                };                
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            else
            {
                return View(model);
            }        
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var passwordHash = Security.Encrypt(user.PasswordHash);;
                var result = await _context.Users.Where(i => i.Email == user.Email && i.Passwordhash == passwordHash)
                                              .Include(r => r.Role)
                                              .FirstOrDefaultAsync();

                if (result is null)
                {
                    ModelState.AddModelError("", "Invalid login credentials");
                    return View(user);
                }
                //create claimIdentity
                var claimsIdentity = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.Name, result.Email),
                    new Claim(ClaimTypes.Role, result.Role.Roletype),
                    new Claim(ClaimTypes.Sid, result.Id.ToString())
                }, "api_auth");
                //create claimPrincipal
                var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
                //sign in User
                await HttpContext.SignInAsync(claimPrincipal);
                return RedirectToAction("Index", "Books");
            }       
            else
            {
                return View(user);
            }                 
        } 

        public ActionResult LogOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Books");
        }
        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
