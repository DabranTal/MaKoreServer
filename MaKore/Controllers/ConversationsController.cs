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
    public class ConversationsController : Controller
    {
        private readonly MaKoreContext _context;

        public ConversationsController(MaKoreContext context)
        {
            _context = context;
        }

        // GET: Conversations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Convesations.ToListAsync());
        }

        // GET: Conversations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _context.Convesations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conversation == null)
            {
                return NotFound();
            }

            return View(conversation);
        }

        // GET: Conversations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Conversations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] Conversation conversation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(conversation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(conversation);
        }

        // GET: Conversations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _context.Convesations.FindAsync(id);
            if (conversation == null)
            {
                return NotFound();
            }
            return View(conversation);
        }

        // POST: Conversations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Conversation conversation)
        {
            if (id != conversation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conversation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConversationExists(conversation.Id))
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
            return View(conversation);
        }

        // GET: Conversations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _context.Convesations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conversation == null)
            {
                return NotFound();
            }

            return View(conversation);
        }

        // POST: Conversations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conversation = await _context.Convesations.FindAsync(id);
            _context.Convesations.Remove(conversation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConversationExists(int id)
        {
            return _context.Convesations.Any(e => e.Id == id);
        }
    }
}
