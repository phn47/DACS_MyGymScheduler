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
    public class GymsController : Controller
    {
        private readonly LinhContext _context;

        public GymsController(LinhContext context)
        {
            _context = context;
        }

        // GET: Gyms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gyms.ToListAsync());
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            // Truy vấn chi tiết phòng gym dựa trên ID
            var gym = _context.Gyms.FirstOrDefault(g => g.GymId == id);

            if (gym == null)
            {
                return NotFound();
            }

            // Trả về view Details với thông tin chi tiết phòng gym
            return View(gym);
        }

        // GET: Gyms/Details/5
        /*        public async Task<IActionResult> Details(int? id)
                {

                    // Truy vấn đối tượng Gym từ database dựa trên id
                    var gym = await _context.Gyms.FirstOrDefaultAsync(m => m.GymId == id);

                    if (gym == null)
                    {
                        return NotFound();
                    }

                    // Trả về view Details với đối tượng Gym để hiển thị dữ liệu
                    return View(gym);
                }*/

        // GET: Gyms/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gym/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GymId,Name,PhoneNumber,Email,Description,Website,OpenHours,CloseHours,PriceRange,Amenities,Images,Rating,Status")] Gym gym, IFormFile Images, string Province, string SpecificAddress)
        {
            if (ModelState.IsValid)
            {
                // Kết hợp giá trị của Province và SpecificAddress để gán cho Address
                gym.Address = $"{Province}, {SpecificAddress}";

                if (Images != null)
                {
                    var filename = Path.GetFileName(Images.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HinhGym", filename);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await Images.CopyToAsync(stream);
                    }

                    gym.Images = filename;
                }

                _context.Add(gym);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gym);
        }



        // GET: Gyms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gym = await _context.Gyms.FindAsync(id);
            if (gym == null)
            {
                return NotFound();
            }
            return View(gym);
        }

        // POST: Gyms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GymId,Name,Address,PhoneNumber,Email,Description,Website,OpenHours,CloseHours,PriceRange,Amenities,Images,Rating,Status")] Gym gym)
        {
            if (id != gym.GymId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gym);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymExists(gym.GymId))
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
            return View(gym);
        }

        // GET: Gyms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gym = await _context.Gyms
                .FirstOrDefaultAsync(m => m.GymId == id);
            if (gym == null)
            {
                return NotFound();
            }

            return View(gym);
        }

        // POST: Gyms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gym = await _context.Gyms.FindAsync(id);
            if (gym != null)
            {
                _context.Gyms.Remove(gym);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymExists(int id)
        {
            return _context.Gyms.Any(e => e.GymId == id);
        }


        [HttpGet]
        public IActionResult HeThongGym()
        {
            // Truy vấn dữ liệu từ bảng Gym để lấy danh sách các chi nhánh phòng gym
            List<Gym> gyms = _context.Gyms.ToList();

            // Trả về view HeThongGym với danh sách các chi nhánh phòng gym để hiển thị
            return View(gyms);
        }

    }
}
