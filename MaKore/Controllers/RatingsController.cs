using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;
using MaKore.Services;

namespace MaKore.Controllers
{
    public class RatingsController : Controller
    {
        public IRatingService _service;


        public RatingsController(MaKoreContext context)
        {
            _service = new RatingService(context);
        }

        // GET: Ratings
        public async Task<IActionResult> Index()
        {
            List<Rating> list = _service.Get();
            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }


        // GET: Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Rating r = _service.GetByIndex((int)id);
            if (r == null)
            {
                return NotFound();
            }
            return View(r);
        }


        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            if (query == null)
            {
                return NotFound();
            }

            List<Rating> list = _service.Search(query);
            if (list == null)
            {
                return NotFound();
            }
            return View(list);
        }


        // GET: Ratings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Grade,Feedback,Date")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                _service.Create(rating);
                return RedirectToAction(nameof(Index));
            }
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = _service.GetByIndex((int)id);
            if (rating == null)
            {
                return NotFound();
            }
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Grade,Feedback,Date")] Rating rating)
        {
            if (id != rating.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _service.Edit(rating);
                }
                catch (DbUpdateConcurrencyException)
                {
                     if (_service.GetByIndex(rating.ID) == null)
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
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Rating rating = _service.GetByIndex((int)id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _service.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
