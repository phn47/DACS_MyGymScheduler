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
    public class MembershipTypesController : Controller
    {
        private readonly LinhContext _context;

        public MembershipTypesController(LinhContext context)
        {
            _context = context;
        }

        // GET: MembershipTypes
        public async Task<IActionResult> Index()
        {
            var linhContext = _context.MembershipTypes.Include(m => m.GymTypeAccessNavigation);
            return View(await linhContext.ToListAsync());
        }

        // GET: MembershipTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipTypes
                .Include(m => m.GymTypeAccessNavigation)
                .FirstOrDefaultAsync(m => m.MembershipTypeId == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // GET: MembershipTypes/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["GymTypeAccess"] = new SelectList(_context.GymTypes, "GymTypeId", "Name");
            return View();
        }

        // POST: MembershipTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipTypeId,Name,Description,GymTypeAccess,Price,Status")] MembershipType membershipType, string AccessHours, string Year, string Month)
        {
            if (!ModelState.IsValid)
            {
                  if (AccessHours == "Month")
                {
                    int yearValue = int.Parse(Month);
                    int days = yearValue * 30;
                    membershipType.AccessHours = days.ToString();
                    _context.Add(membershipType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

                if (ModelState.IsValid)
            {
                if (AccessHours == "Year")
                {
                    int yearValue = int.Parse(Year);
                    int days = yearValue * 365;
                    membershipType.AccessHours = days.ToString();
                }
                membershipType.Status = "1";

                _context.Add(membershipType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["GymTypeAccess"] = new SelectList(_context.GymTypes, "GymTypeId", "Name", membershipType.GymTypeAccess);
            return View(membershipType);
        }

        // GET: MembershipTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipTypes.FindAsync(id);
            if (membershipType == null)
            {
                return NotFound();
            }
            ViewData["GymTypeAccess"] = new SelectList(_context.GymTypes, "GymTypeId", "GymTypeId", membershipType.GymTypeAccess);
            return View(membershipType);
        }

        // POST: MembershipTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MembershipTypeId,Name,Description,GymTypeAccess,AccessHours,Price,Status")] MembershipType membershipType)
        {
            if (id != membershipType.MembershipTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipTypeExists(membershipType.MembershipTypeId))
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
            ViewData["GymTypeAccess"] = new SelectList(_context.GymTypes, "GymTypeId", "GymTypeId", membershipType.GymTypeAccess);
            return View(membershipType);
        }

        // GET: MembershipTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipType = await _context.MembershipTypes
                .Include(m => m.GymTypeAccessNavigation)
                .FirstOrDefaultAsync(m => m.MembershipTypeId == id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return View(membershipType);
        }

        // POST: MembershipTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membershipType = await _context.MembershipTypes.FindAsync(id);
            if (membershipType != null)
            {
                _context.MembershipTypes.Remove(membershipType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipTypeExists(int id)
        {
            return _context.MembershipTypes.Any(e => e.MembershipTypeId == id);
        }
    }
}
