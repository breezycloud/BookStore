using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Context;
using BookStore.Models;
using System.Security.Claims;
using System.Threading;
using BookStore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace BookStore.Controllers
{
    public class BookingsController : Controller
    {
        private readonly BookStoreContext _context;

        public BookingsController(BookStoreContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookStoreContext = _context.Bookings.Include(b => b.Book).Include(b => b.User);
            return View(await bookStoreContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["Bookid"] = new SelectList(_context.Books, "Id", "Id");
            ViewData["Userid"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Bookid,Userid,Status,DateBooked,DateReturned")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.Id = Guid.NewGuid();
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Bookid"] = new SelectList(_context.Books, "Id", "Id", booking.Bookid);
            ViewData["Userid"] = new SelectList(_context.Users, "Id", "Name", booking.Userid);
            return View(booking);
        }

        public ActionResult Cart()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Cart(Guid userID)
        {
            userID = GetGuid();
            var itemStatus = BookStatus.Unchecked.ToString();
            var items = await _context.Bookings.Where(u => u.Userid == userID && u.Status == itemStatus)
                                               .Include(b => b.Book)
                                               .ToListAsync();

            return View(items);
        }


        [HttpPost]        
        public async Task<IActionResult> AddCart(Guid Id)
        {
            Booking model = new()
            {
                Id = Guid.NewGuid(),
                Userid = GetGuid(),
                Bookid = Id,                
                Status = BookStatus.Unchecked.ToString()
            };
            var bookExist = await _context.Bookings.Where(b => b.Bookid == Id).AsNoTracking().FirstOrDefaultAsync();
            if (bookExist.Status == BookStatus.Unchecked.ToString() ||
                bookExist.Status == BookStatus.Rented.ToString())
                return RedirectToAction(nameof(Cart));
            if (bookExist.Status == BookStatus.Returned.ToString())
            {
                model.Id = bookExist.Id;
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Cart));
            }
            
            if (ModelState.IsValid)
            {                
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Cart));            
            }
            else
                return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cart));
        }
        public ActionResult Order()
        {
            return View();
        }
        

        [HttpGet]
        public async Task<IActionResult> Order(Guid userID)
        {
            userID = GetGuid();
            var orders = await _context.Bookings.Where(b => b.Userid == userID
                                                 && b.Status != BookStatus.Unchecked.ToString())
                                                .Include(i => i.Book)
                                                .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut()
        {
            var userID = GetGuid();
            var itemStatus = BookStatus.Unchecked.ToString();
            var items = await _context.Bookings.Where(u => u.Userid == userID 
                                                && u.Status == itemStatus)
                                               .Include(b => b.Book)
                                               .AsNoTracking()
                                               .ToListAsync();

            foreach (var item in items)
            {
                var booking = GetBooking(item);
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Order");
        }

        private Booking GetBooking(Booking item)
        {
            Booking booking = new()
            {
                Id = item.Id,
                Bookid = item.Bookid,
                Userid = item.Userid,
                Status = BookStatus.Rented.ToString(),
                DateBooked = DateTime.Now.Date,
                DateReturned = null
            };
            return booking;
        }
        [HttpGet]
        public async Task<IActionResult> ReturnBook(Guid Id)
        {
            var booking = await _context.Bookings.Where(i => i.Id == Id)
                                               .Include(b => b.Book)
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync();
            if (booking is null)
            {
                return NoContent();
            }            

            booking.Status = BookStatus.Returned.ToString();
            booking.DateReturned = DateTime.Now.Date;

            try
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Order));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }            
        }
        public Guid GetGuid()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var uid = identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                                     .Select(c => c.Value)
                                     .SingleOrDefault();
                return Guid.Parse(uid);                                                         
            }                

            return Guid.NewGuid();
        }
        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["Bookid"] = new SelectList(_context.Books, "Id", "Id", booking.Bookid);
            ViewData["Userid"] = new SelectList(_context.Users, "Id", "Name", booking.Userid);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Bookid,Userid,Status,DateBooked,DateReturned")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
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
            ViewData["Bookid"] = new SelectList(_context.Books, "Id", "Id", booking.Bookid);
            ViewData["Userid"] = new SelectList(_context.Users, "Id", "Name", booking.Userid);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(Guid id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
