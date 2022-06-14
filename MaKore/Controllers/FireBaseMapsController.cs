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
    [Route("api")]
    public class FireBaseMapsController : BaseController
    {
        private readonly MaKoreContext _context;

        public FireBaseMapsController(MaKoreContext context)
        {
            _context = context;
        }





        // POST: FireBaseMaps
        [HttpPost("firebase")]
        public async Task<IActionResult> Create([Bind("UserName,Token,Key")] FireBaseMap fireBaseMap)
        {
            if (ModelState.IsValid)
            {
                var q = from fb in _context.FireBaseMap
                        where fb.UserName == fireBaseMap.UserName
                        select fb;
                // check if user already exist in map
                if (q.Any())
                {
                    _context.Update(fireBaseMap);
                }
                else
                {
                    _context.Add(fireBaseMap);
                }
                await _context.SaveChangesAsync();
                return (StatusCode(201));
            }
            return BadRequest();
        }






        // GET: FireBaseMaps
        public async Task<IActionResult> Index()
        {
              return View(await _context.FireBaseMap.ToListAsync());
        }

        // GET: FireBaseMaps/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.FireBaseMap == null)
            {
                return NotFound();
            }

            var fireBaseMap = await _context.FireBaseMap
                .FirstOrDefaultAsync(m => m.UserName == id);
            if (fireBaseMap == null)
            {
                return NotFound();
            }

            return View(fireBaseMap);
        }

        // GET: FireBaseMaps/Create
        public IActionResult Create()
        {
            return View();
        }


        // GET: FireBaseMaps/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.FireBaseMap == null)
            {
                return NotFound();
            }

            var fireBaseMap = await _context.FireBaseMap.FindAsync(id);
            if (fireBaseMap == null)
            {
                return NotFound();
            }
            return View(fireBaseMap);
        }

        // POST: FireBaseMaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserName,Token,Key")] FireBaseMap fireBaseMap)
        {
            if (id != fireBaseMap.UserName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fireBaseMap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FireBaseMapExists(fireBaseMap.UserName))
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
            return View(fireBaseMap);
        }

        // GET: FireBaseMaps/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.FireBaseMap == null)
            {
                return NotFound();
            }

            var fireBaseMap = await _context.FireBaseMap
                .FirstOrDefaultAsync(m => m.UserName == id);
            if (fireBaseMap == null)
            {
                return NotFound();
            }

            return View(fireBaseMap);
        }

        // POST: FireBaseMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.FireBaseMap == null)
            {
                return Problem("Entity set 'MaKoreContext.FireBaseMap'  is null.");
            }
            var fireBaseMap = await _context.FireBaseMap.FindAsync(id);
            if (fireBaseMap != null)
            {
                _context.FireBaseMap.Remove(fireBaseMap);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FireBaseMapExists(string id)
        {
          return _context.FireBaseMap.Any(e => e.UserName == id);
        }
    }
}
