using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using toanggg.Data;
using toanggg.Models;
using System.Data.SqlClient;
using Dapper;
using toanggg.Data;
using NuGet.DependencyResolver;
using toanggg.Services;

namespace toanggg.Controllers
{
    public class TrainersController : Controller
    {
        private readonly LinhContext _context;
        private readonly IConfiguration _configuration;

        public TrainersController(LinhContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            var linhContext = _context.Trainers.Include(t => t.Gym).Include(t => t.User);
            return View(await linhContext.ToListAsync());
        }

        // GET: Trainers/Details/5
        [HttpGet]
        public IActionResult Details()
        {
        
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", trainer.GymId);
          
            return View(trainer);
        }

        // GET: Trainers/Create
        public IActionResult Create()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name");
         /*   ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName");*/
            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterVM model, IFormFile Avatar)
        {
            /*if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", trainer.GymId);
*//*            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", trainer.UserId);*//*
            return View(trainer);*/
            if (!ModelState.IsValid)
            {
                // Tạo một người dùng với vai trò "Nhân viên"
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Dob = model.Dob,
                    Address = model.Address,
                    Avatar = model.Avatar,
                    Role = "PT",
                    Status = "1", // Giả sử mặc định trạng thái là Active
                    EmailconfirmationToken = Guid.NewGuid().ToString(),
                    IsemailOnfirmed = false // Email chưa xác thực
                };

                if (Avatar != null)
                {
                    var filename = Path.GetFileName(Avatar.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HinhGym", filename);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await Avatar.CopyToAsync(stream);
                    }

                    user.Avatar = filename;
                }

                // Thêm người dùng vào cơ sở dữ liệu
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Lấy user_id mới được tạo ra
                int userId = user.UserId; // Giả sử user_id là khóa chính và được tự động tạo

                // Tạo một bản ghi trong bảng Trainer
                var trainer = new Trainer
                {
                    UserId = userId, // Sử dụng user_id mới tạo ra
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Description = "",
                    Status = "Active" // Giả sử mặc định trạng thái là Active
                };

                // Thêm trainer vào cơ sở dữ liệu
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();

                // Gửi email xác nhận
                var confirmationLink = Url.Action("ConfirmEmail", "Users", new { userId = user.UserId, token = user.EmailconfirmationToken }, Request.Scheme);
                var emailSender = new EmailSender(_configuration);
                await emailSender.SendEmailAsync(user.Email, "Xác nhận email của bạn", $"Vui lòng xác nhận email của bạn bằng cách nhấn vào <a href=\"{confirmationLink}\">đây</a>.");

                // Chuyển hướng đến trang đăng nhập hoặc bất kỳ trang nào khác
                return RedirectToAction("DangNhap", "Users");
            }

            // Nếu trạng thái của mô hình không hợp lệ, hiển thị form đăng ký lại với lỗi
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "Name", model.MembershipTypeId);
            return View(model);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", trainer.GymId);
           
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainerId,FullName,PhoneNumber,Email,Description,Expertise,Status,GymId,UserId")] Trainer trainer)
        {
            if (id != trainer.TrainerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.TrainerId))
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
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", trainer.GymId);
 
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }
            ViewData["GymName"] = trainer.Gym?.Name ?? string.Empty;
            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.TrainerId == id);
        }


        [HttpGet]
        public IActionResult DangKyNhanVien()
        {
            if (HttpContext.Session.GetString("PinVerified") == "true")
            {
                // Nếu đã xác minh, xóa session
                HttpContext.Session.Remove("PinVerified");
            }
            else
            {
                // Nếu chưa xác minh, chuyển hướng về trang ViewPin
                return RedirectToAction("ViewPin", "Trainers");
            }

            // Trả về view DangKy với một instance mới của NhanVienVM
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKyNhanVien(RegisterVM model, IFormFile Avatar)
        {
            if (!ModelState.IsValid)
            {
                // Tạo một người dùng với vai trò "Nhân viên"
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Dob = model.Dob,
                    Address = model.Address,
                    Avatar = model.Avatar,
                    Role = "Nhan vien",
                    Status = "1", // Giả sử mặc định trạng thái là Active
                    EmailconfirmationToken = Guid.NewGuid().ToString(),
                    IsemailOnfirmed = false // Email chưa xác thực
                };

                if (Avatar != null)
                {
                    var filename = Path.GetFileName(Avatar.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HinhGym", filename);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await Avatar.CopyToAsync(stream);
                    }

                    user.Avatar = filename;
                }

                // Thêm người dùng vào cơ sở dữ liệu
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Lấy user_id mới được tạo ra
                int userId = user.UserId; // Giả sử user_id là khóa chính và được tự động tạo

                // Tạo một bản ghi trong bảng Trainer
                var trainer = new Trainer
                {
                    UserId = userId, // Sử dụng user_id mới tạo ra
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    Description = "",
                    Status = "Active" // Giả sử mặc định trạng thái là Active
                };

                // Thêm trainer vào cơ sở dữ liệu
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();

                // Gửi email xác nhận
                var confirmationLink = Url.Action("ConfirmEmail", "Users", new { userId = user.UserId, token = user.EmailconfirmationToken }, Request.Scheme);
                var emailSender = new EmailSender(_configuration);
                await emailSender.SendEmailAsync(user.Email, "Xác nhận email của bạn", $"Vui lòng xác nhận email của bạn bằng cách nhấn vào <a href=\"{confirmationLink}\">đây</a>.");

                // Chuyển hướng đến trang đăng nhập hoặc bất kỳ trang nào khác
                return RedirectToAction("DangNhap", "Users");
            }

            // Nếu trạng thái của mô hình không hợp lệ, hiển thị form đăng ký lại với lỗi
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "Name", model.MembershipTypeId);
            return View(model);
        }

        [HttpGet]
        public IActionResult ViewPin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult OnPost(string pin)
        {
            if (pin == "1234")
            {
                HttpContext.Session.SetString("PinVerified", "true");
                return RedirectToAction("DangKyNhanVien", "Trainers");
            }
            else
            {
                ViewBag.Message = "Mã pin không đúng. Vui lòng thử lại.";
                return View("ViewPin");
            }
        }



        [HttpGet]
        public IActionResult TrainerLaAi()
        {
            // Truy vấn dữ liệu từ bảng Gym để lấy danh sách các chi nhánh phòng gym
            List<Trainer> trainers = _context.Trainers.ToList();

            // Trả về view HeThongGym với danh sách các chi nhánh phòng gym để hiển thị
            return View(trainers);
        }
        [HttpGet]
        public IActionResult TrainerDetail(int id)
        {
            var trainer = _context.Trainers.FirstOrDefault(t => t.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

    }
}
