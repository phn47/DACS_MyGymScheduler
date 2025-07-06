using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using toanggg.Data;

namespace toanggg.Controllers
{
    public class GymTypesController : Controller
    {
        private readonly LinhContext _context;

        public GymTypesController(LinhContext context)
        {
            _context = context;
        }

        // GET: GymTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.GymTypes.ToListAsync());
        }

        // GET: GymTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymType = await _context.GymTypes
                .FirstOrDefaultAsync(m => m.GymTypeId == id);
            if (gymType == null)
            {
                return NotFound();
            }

            return View(gymType);
        }

        // GET: GymTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GymTypeId,Name,Description")] GymType gymType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymType);
        }

        // GET: GymTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymType = await _context.GymTypes.FindAsync(id);
            if (gymType == null)
            {
                return NotFound();
            }
            return View(gymType);
        }

        // POST: GymTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GymTypeId,Name,Description")] GymType gymType)
        {
            if (id != gymType.GymTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymTypeExists(gymType.GymTypeId))
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
            return View(gymType);
        }

        // GET: GymTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymType = await _context.GymTypes
                .FirstOrDefaultAsync(m => m.GymTypeId == id);
            if (gymType == null)
            {
                return NotFound();
            }

            return View(gymType);
        }

        // POST: GymTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymType = await _context.GymTypes.FindAsync(id);
            if (gymType != null)
            {
                _context.GymTypes.Remove(gymType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymTypeExists(int id)
        {
            return _context.GymTypes.Any(e => e.GymTypeId == id);
        }
    }
}
