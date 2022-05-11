#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;



namespace MaKore.Controllers
{

    [ApiController]
    [Route("api/cocccntacts/{action}")]
    public class UsersController : Controller
    {

        private readonly MaKoreContext _context;

        public UsersController(MaKoreContext context)
        {
            _context = context;
          
        }

        // GET: Users/Create
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,Password,NickName")] User user)
        {
            if (ModelState.IsValid)
            {

                var isTakenUserName = from userName in _context.Users.Where(m => m.UserName == user.UserName) select userName;
                if (isTakenUserName.Any()) {
                    return View("Error");
                } else {
                    HttpContext.Session.SetString("username", isTakenUserName.First().UserName);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(user);
        }




        // GET: Users/Create
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,Password")] User user)
        {
            if (ModelState.IsValid)
            {

                var isRegistered = _context.Users.Where(m => m.UserName == user.UserName && m.Password == user.Password);
                if (isRegistered.Any())
                {
                    // we save info and when the user refreshes we know its him
                    HttpContext.Session.SetString("username", isRegistered.First().UserName);
                    // rediret with react
                    return View("yes");
                } else
                {
                    return View("no");
                }


            }
            return View(user);
        }

    }
}
