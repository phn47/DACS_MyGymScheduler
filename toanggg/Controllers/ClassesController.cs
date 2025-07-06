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
    public class ClassesController : Controller
    {
        private readonly LinhContext _context;

        public ClassesController(LinhContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            var linhContext = _context.Classes.Include(d => d.Gym).Include(d => d.Trainer);
            return View(await linhContext.ToListAsync());
          /*  return View();*/
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(d => d.Gym)
                .Include(d => d.Trainer)
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
            /*  return View();*/
        }

        // GET: Classes/Create
            public IActionResult Create()
        {
  /*          ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassType");*/
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name");

            ViewData["ClassType"] = new SelectList(_context.GymTypes, "GymTypeId", "Name");
            /*
                     ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "FullName");*/
            return View();
            }
 
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("ClassId,Name,StartTime,EndTime,Status,ClassType,GymId,TrainerId")] Class @class)
            {

            var trainerId = HttpContext.Session.GetString("TrainerId");
            if (string.IsNullOrEmpty(trainerId))
            {
                // Xử lý khi TrainerId không có trong session hoặc không hợp lệ
                ModelState.AddModelError(string.Empty, "Huấn luyện viên không hợp lệ.");
                ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", @class.GymId);
                return View(@class);
            }
            if (ModelState.IsValid)
            {
                int trainerIdInt = int.Parse(trainerId);
                @class.TrainerId = trainerIdInt;
                @class.Status = "1";
               
                _context.Add(@class);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassType");
          
               /* ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "FullName", @class.TrainerId);*/
                return View(@class);
            }





        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "GymId", @class.GymId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", @class.TrainerId);
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassId,Name,StartTime,EndTime,Status,ClassType,GymId,TrainerId")] Class @class)
        {
            if (id != @class.ClassId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.ClassId))
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
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "GymId", @class.GymId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "TrainerId", @class.TrainerId);
            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                 .Include(d => d.Gym)
                 .Include(d => d.Trainer)
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
   /*         return View();*/
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.ClassId == id);
        }




        // GET: Classes/HuanLuyenRieng
        // GET: Classes/HuanLuyenRieng
        // GET: Classes/HuanLuyenRieng
        public IActionResult HuanLuyenRieng()
        {
            ViewData["ClassType"] = new SelectList(_context.GymTypes, "GymTypeId", "Name");
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuanLuyenRieng([Bind("ClassId,Name,StartTime,EndTime,Status,ClassType,GymId")] Class @class)
        {
            // Lấy TrainerId hiện tại từ session hoặc bất kỳ nguồn nào bạn đang lưu
            var trainerId = HttpContext.Session.GetString("TrainerId");
            if (string.IsNullOrEmpty(trainerId))
            {
                // Xử lý khi TrainerId không có trong session hoặc không hợp lệ
                ModelState.AddModelError(string.Empty, "Huấn luyện viên không hợp lệ.");
                ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", @class.GymId);
                return View(@class);
            }

            if (ModelState.IsValid)
            {
                int trainerIdInt = int.Parse(trainerId); // Chuyển TrainerId từ string sang int

                // Kiểm tra xem huấn luyện viên đã có lớp học chưa và trạng thái là NULL
                var existingClass = await _context.Classes
                    .FirstOrDefaultAsync(c => c.TrainerId == trainerIdInt && (c.Status == "NULL" || c.Status == null));

                if (existingClass != null)
                {
                    // Huấn luyện viên đã có lớp học với trạng thái NULL hoặc chưa có lớp học
                    ModelState.AddModelError(string.Empty, "Huấn luyện viên này đã đăng ký!");
                    ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", @class.GymId);
                    return View(@class);
                }

                // Gán TrainerId hiện tại cho lớp học mới
                @class.TrainerId = trainerIdInt;
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", @class.GymId);
            return View(@class);
        }




    }
}
