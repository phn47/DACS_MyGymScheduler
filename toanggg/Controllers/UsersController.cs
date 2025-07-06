using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using toanggg.Data;
using toanggg.Models;
using Microsoft.AspNetCore.Authorization;
using toanggg.Services;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using NuGet.DependencyResolver;


namespace toanggg.Controllers
{
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly LinhContext _context;

        public UsersController(LinhContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var linhContext = _context.Users.Include(u => u.MembershipType);
            return View(await linhContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.MembershipType)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "MembershipTypeId");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Password,Email,FullName,PhoneNumber,Gender,Dob,Address,Avatar,Role,Status,MembershipTypeId")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "MembershipTypeId", user.MembershipTypeId);
            return View(user);
        }

        // GET: Users/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "MembershipTypeId", user.MembershipTypeId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,Email,FullName,PhoneNumber,Gender,Dob,Address,Avatar,Role,Status,MembershipTypeId")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "MembershipTypeId", user.MembershipTypeId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.MembershipType)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        /*
                [HttpGet]
                public IActionResult DangNhap(string? ReturnUrl)
                {
                    ViewBag.ReturnUrl = ReturnUrl;
                    return View();
                }

                [HttpPost]
                public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
                {
                    if (ModelState.IsValid)
                    {
                        // Check user credentials (for demo purposes, you can replace this with your authentication logic)
                        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

                        if (user == null)
                        {
                            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                            return View(model);
                        }

                        if (!(user.IsemailOnfirmed ?? false))
                        {
                            ModelState.AddModelError(string.Empty, "Email của bạn chưa được xác thực. Vui lòng kiểm tra email và xác thực.");
                            return View(model);
                        }

                        // Lưu UserId vào session
                        HttpContext.Session.SetString("UserId", user.UserId.ToString());

                        // Tạo claims cho người dùng đã xác thực
                        var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Username),
                    // Thêm các claims khác nếu cần
                };

                        if (user.Role == "Nhan vien")
                        {
                            // Thêm role claim cho Nhan vien
                            claims.Add(new Claim(ClaimTypes.Role, "Nhan vien"));

                            // Lấy TrainerId và lưu vào session
                            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.UserId);
                            if (trainer != null)
                            {
                                HttpContext.Session.SetString("TrainerId", trainer.TrainerId.ToString());
                            }

                            // Chuyển hướng đến Trainers/Index
                            return RedirectToAction("Index", "Trainers");
                        }
                        if (user.Role == "PT")
                        {
                            // Thêm role claim cho Nhan vien
                            claims.Add(new Claim(ClaimTypes.Role, "PT"));

                            // Lấy TrainerId và lưu vào session
                            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.UserId);
                            if (trainer != null)
                            {
                                HttpContext.Session.SetString("TrainerId", trainer.TrainerId.ToString());
                            }

                            // Chuyển hướng đến Trainers/Index Classes/HuanLuyenRieng
                            return RedirectToAction("HuanLuyenRieng", "Classes");
                        }

                        // Tạo identity cho người dùng đã xác thực
                        var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        // Tạo principal cho người dùng đã xác thực
                        var userPrincipal = new ClaimsPrincipal(userIdentity);

                        // Đăng nhập người dùng
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                        // Chuyển hướng đến URL gốc hoặc URL mặc định
                        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    // Nếu model state không hợp lệ, trả về form đăng nhập với lỗi xác thực
                    return View(model);
                }


        */

        [HttpGet]
        public IActionResult Member()
        {

            return View();
        }





        [HttpGet]
        public IActionResult DangKy()
        {
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "Name");
            return View();
        }
        public async Task<IActionResult> DangKy(RegisterVM model, IFormFile Avatar)
        {
            if (!ModelState.IsValid)
            {
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
                    Role = "Nguoi dung",
                    Status = "1",
                    MembershipTypeId = model.MembershipTypeId,
                    EmailconfirmationToken = Guid.NewGuid().ToString(),
                    IsemailOnfirmed = false // Đặt ban đầu là false
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

                ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "Name", user.MembershipTypeId);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var confirmationLink = Url.Action("ConfirmEmail", "Users", new { userId = user.UserId, token = user.EmailconfirmationToken }, Request.Scheme);
                var emailSender = new EmailSender(_configuration);
                await emailSender.SendEmailAsync(user.Email, "Xác nhận email của bạn", $"Vui lòng xác nhận email của bạn bằng cách nhấn vào <a href=\"{confirmationLink}\">đây</a>.");

                return RedirectToAction("DangNhap", "Users");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            if (userId == 0 || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("DangNhap", "Users");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.EmailconfirmationToken == token);
            if (user == null)
            {
                return RedirectToAction("DangNhap", "Users");
            }

            user.IsemailOnfirmed = true;
            user.EmailconfirmationToken = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return View("ConfirmEmail"); // Bạn cần tạo View ConfirmEmail để hiển thị thông báo xác nhận
        }




        public IActionResult IBD()
        {

            return View();
        }



        [HttpGet]
        public async Task<IActionResult> DangKyLop()
        {
            var classTypes = await (from schedule in _context.ClassSchedules
                                    join cls in _context.Classes on schedule.ClassId equals cls.ClassId
                                    join trainer in _context.Trainers on cls.TrainerId equals trainer.TrainerId
                                    select new
                                    {
                                        ScheduleId = schedule.ScheduleId,
                                        ClassType = cls.ClassType,
                                        TrainerFullName = trainer.FullName,
                                        schedule.Weekday,
                                        schedule.StartTime,
                                        schedule.EndTime,

                                        TrainerAvatar = trainer.User.Avatar

                                    }).ToListAsync();

            ViewBag.AllClasses = classTypes;

            var userId = HttpContext.Session.GetString("UserId");
            var registeredSchedules = new List<dynamic>();

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                registeredSchedules = await (from cs in _context.ClassSchedules
                                             join cls in _context.Classes on cs.ClassId equals cls.ClassId
                                             join trainer in _context.Trainers on cls.TrainerId equals trainer.TrainerId
                                             where cs.Room != null && cs.Room !=""
                                             select new
                                             {
                                                 cs.ScheduleId,
                                                 cs.Weekday,
                                                 cs.StartTime,
                                                 cs.EndTime,
                                                 cls.ClassType,
                                                 TrainerFullName = trainer.FullName
                                             }).ToListAsync<dynamic>();
            }

            ViewBag.RegisteredSchedules = registeredSchedules;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKyLop(int ScheduleId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("DangNhap", "Users");
            }

            var classSchedule = await _context.ClassSchedules
                .Include(cs => cs.Class)
                .FirstOrDefaultAsync(cs => cs.ScheduleId == ScheduleId);
            if (classSchedule != null && int.TryParse(userId, out int userIdInt))
            {
                var user = await _context.Users
                    .Include(u => u.MembershipType)
                    .FirstOrDefaultAsync(u => u.UserId == userIdInt);

                if (user != null && user.MembershipType != null)
                {
                    if (int.TryParse(user.MembershipType.AccessHours, out int accessHours) && accessHours > 89)
                    {
                        var userIds = string.IsNullOrEmpty(classSchedule.Room) ?
                            new List<string>() : classSchedule.Room.Split('/').ToList();

                        if (userIds.Contains(userIdInt.ToString()))
                        {
                            ViewBag.ErrorMessage = "Đã đăng ký lớp học này rồi.";
                            return View("Error");
                        }

                        if (userIds.Count < 25)
                        {
                            userIds.Add(userIdInt.ToString());
                            classSchedule.Room = string.Join("/", userIds);
                            _context.Update(classSchedule);
                            await _context.SaveChangesAsync();

                            var userMembership = await _context.UserMemberships
                                .FirstOrDefaultAsync(um => um.UserId == userIdInt);

                            var emailSender = new EmailSender(_configuration);
                            var subject = "Thông báo đăng ký lớp học";
                            var membershipEndDate = userMembership?.EndDate.HasValue == true
                                ? userMembership.EndDate.Value.ToDateTime(TimeOnly.MinValue).ToString("dd/MM/yyyy")
                                : "N/A";

                            var message = $@"
                    <p>Chào {user.FullName},</p>
                    <p>Bạn đã đăng ký thành công lớp học <strong>{classSchedule.Class.Name}</strong> vào <strong>{classSchedule.Weekday}</strong> từ <strong>{classSchedule.StartTime}</strong> đến <strong>{classSchedule.EndTime}</strong>.</p>
                    <p>Gói <strong>{user.MembershipType?.Name ?? "N/A"}</strong> bạn đăng ký có thời hạn đến <strong>{membershipEndDate}</strong>.</p>
                    <p>Trân trọng,</p>
                    <p>Đội ngũ hỗ trợ của the Linh</p>
                    ";
                            await emailSender.SendEmailAsync(user.Email, subject, message);

                            return RedirectToAction("DangKyLop", "Users");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Lớp học đã đầy. Không thể đăng ký thêm.";
                            return View("Error");
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Chỉ gói 3 và 6 tháng mới được đăng ký lớp học!.";
                        return View("Error");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Không tìm thấy thông tin người dùng hoặc gói đăng ký.";
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Không tìm thấy lịch học.";
                return View("Error");
            }
        }



        public IActionResult GetClassDetailsForSchedule(int scheduleId)
        {
            var classDetails = (from schedule in _context.ClassSchedules
                                join cls in _context.Classes on schedule.ClassId equals cls.ClassId
                                join trainer in _context.Trainers on cls.TrainerId equals trainer.TrainerId
                                where schedule.ScheduleId == scheduleId
                                select new
                                {
                                    cls.ClassId,
                                    trainer.FullName,
                                    trainer.User.Avatar
                                }).FirstOrDefault();

            if (classDetails != null)
            {
                return Json(new
                {
                    classId = classDetails.ClassId,
                    trainerFullName = classDetails.FullName,
                    trainerImage = Url.Content($"~/Hinh/HinhGym/{classDetails.Avatar}")
                });
            }
            else
            {
                return Json(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> HuyDangKy(int scheduleId)
        {
            // Lấy UserId của người dùng từ session
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // Nếu UserId không tồn tại trong session, xử lý tùy ý của bạn, ví dụ: chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap", "Users");
            }

            // Tìm ClassSchedule dựa trên ScheduleId
            var classSchedule = await _context.ClassSchedules.FirstOrDefaultAsync(cs => cs.ScheduleId == scheduleId);

            if (classSchedule != null)
            {
                // Lấy danh sách các UserId hiện tại trong Room
                var userIds = string.IsNullOrEmpty(classSchedule.Room) ?
                    new List<string>() : classSchedule.Room.Split('/').ToList();

                // Kiểm tra xem UserId của người dùng có trong danh sách không
                if (userIds.Contains(userId))
                {
                    // Xóa UserId khỏi danh sách
                    userIds.Remove(userId);

                    // Cập nhật lại Room với danh sách mới
                    classSchedule.Room = string.Join("/", userIds);

                    // Cập nhật ClassSchedule trong cơ sở dữ liệu
                    _context.Update(classSchedule);
                    await _context.SaveChangesAsync();

                    // Chuyển hướng đến trang thành công hoặc thực hiện hành động mong muốn
                    return RedirectToAction("DangKyLop");
                }
            }

            // Xử lý trường hợp không tìm thấy hoặc không thể hủy đăng ký lớp học
            ViewBag.ErrorMessage = "Không thể hủy đăng ký lớp học.";
            return View("Error");
        }

        /*    [HttpPost]
            public async Task<IActionResult> DangKyLop(int ScheduleId)
            {


                // Lấy UserId của người dùng từ session
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                {
                    // Nếu UserId không tồn tại trong session, xử lý tùy ý của bạn, ví dụ: chuyển hướng đến trang đăng nhập
                    return RedirectToAction("DangNhap", "Users");
                }

                // Tìm ClassSchedule dựa trên ScheduleId
                var classSchedule = await _context.ClassSchedules.FirstOrDefaultAsync(cs => cs.ScheduleId == ScheduleId);

                if (classSchedule != null)
                {
                    if (int.TryParse(userId, out int userIdInt))
                    {
                        // Lấy danh sách các UserId hiện tại trong Room
                        var userIds = string.IsNullOrEmpty(classSchedule.Room) ?
                            new List<string>() : classSchedule.Room.Split('/').ToList();

                        // Kiểm tra nếu UserId đã tồn tại
                        if (userIds.Contains(userIdInt.ToString()))
                        {
                            // UserId đã tồn tại
                            ViewBag.ErrorMessage = "Đã đăng ký lớp học này rồi.";
                            return View("Error");
                        }

                        // Kiểm tra số lượng UserId hiện tại
                        if (userIds.Count < 25)
                        {
                            // Thêm UserId mới vào danh sách
                            userIds.Add(userIdInt.ToString());

                            // Gán lại giá trị cho Room với danh sách UserId mới
                            classSchedule.Room = string.Join("/", userIds);

                            // Cập nhật ClassSchedule trong cơ sở dữ liệu
                            _context.Update(classSchedule);
                            await _context.SaveChangesAsync();

                            // Chuyển hướng đến trang thành công hoặc thực hiện hành động mong muốn
                            return RedirectToAction("Index", "ClassSchedules");
                        }
                        else
                        {
                            // Xử lý trường hợp số lượng UserId đạt tối đa
                            ViewBag.ErrorMessage = "Lớp học đã đầy. Không thể đăng ký thêm.";
                            return View("Error");
                        }
                    }

                    // Nếu UserId không phải là số hợp lệ
                    ViewBag.ErrorMessage = "UserId không hợp lệ.";
                    return View("Error");
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy ClassSchedule, ví dụ: hiển thị thông báo lỗi
                    ViewBag.ErrorMessage = "Không tìm thấy lịch học.";
                    return View("Error");
                }
            }


    */







/*
        [HttpPost]
        public async Task<IActionResult> HuyDangKy(int scheduleId)
        {
            // Lấy UserId của người dùng từ session
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // Nếu UserId không tồn tại trong session, xử lý tùy ý của bạn, ví dụ: chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap", "Users");
            }

            // Tìm ClassSchedule dựa trên ScheduleId
            var classSchedule = await _context.ClassSchedules.FirstOrDefaultAsync(cs => cs.ScheduleId == scheduleId);

            if (classSchedule != null)
            {
                // Lấy danh sách các UserId hiện tại trong Room
                var userIds = string.IsNullOrEmpty(classSchedule.Room) ?
                    new List<string>() : classSchedule.Room.Split('/').ToList();

                // Kiểm tra xem UserId của người dùng có trong danh sách không
                if (userIds.Contains(userId))
                {
                    // Xóa UserId khỏi danh sách
                    userIds.Remove(userId);

                    // Cập nhật lại Room với danh sách mới
                    classSchedule.Room = string.Join("/", userIds);

                    // Cập nhật ClassSchedule trong cơ sở dữ liệu
                    _context.Update(classSchedule);
                    await _context.SaveChangesAsync();

                    // Chuyển hướng đến trang thành công hoặc thực hiện hành động mong muốn
                    return RedirectToAction("DangKyLop");
                }
            }

            // Xử lý trường hợp không tìm thấy hoặc không thể hủy đăng ký lớp học
            ViewBag.ErrorMessage = "Không thể hủy đăng ký lớp học.";
            return View("Error");
        }
*/

     /*   public IActionResult GetClassDetailsForSchedule(int scheduleId)
        {
            var classDetails = (from schedule in _context.ClassSchedules
                                join cls in _context.Classes on schedule.ClassId equals cls.ClassId
                                join trainer in _context.Trainers on cls.TrainerId equals trainer.TrainerId
                                where schedule.ScheduleId == scheduleId
                                select new
                                {
                                    ClassId = cls.ClassId,
                                    TrainerFullName = trainer.FullName
                                }).FirstOrDefault();

            if (classDetails != null)
            {
                return Json(new { classId = classDetails.ClassId, trainerFullName = classDetails.TrainerFullName });
            }
            else
            {
                return Json(null);
            }
        }*/


        public IActionResult GetClassId(int scheduleId)
        {
            // Logic to retrieve ClassId based on the selected ScheduleId
            var classId = _context.ClassSchedules
                                 .Where(schedule => schedule.ScheduleId == scheduleId)
                                 .Select(schedule => schedule.ClassId)
                                 .FirstOrDefault();

            return Json(classId);
        }



        [HttpGet]
        public JsonResult GetTrainerId(int scheduleId)
        {
            var trainerId = (from schedule in _context.ClassSchedules
                             join cls in _context.Classes
                             on schedule.ClassId equals cls.ClassId
                             where schedule.ScheduleId == scheduleId
                             select cls.TrainerId).FirstOrDefault();

            return Json(trainerId);
        }




        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        public IActionResult facebook()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
                user = new User
                {
                    Username = username,
                    Password = password,

                };
                if (user == null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName)
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult FacebookLogin()
        {
            var redirectUrl = Url.Action("FacebookResponse", "Users");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
                return RedirectToAction("Index", "Home");

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims
                .Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });

            var facebookId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var profilePictureUrl = claims.FirstOrDefault(c => c.Type == "urn:facebook:picture")?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FacebbookId == facebookId);

            if (user == null)
            {
                user = new User
                {
                    FacebbookId = facebookId,
                    FullName = name,
                    Email = email,
                    ProfilePictureUrl = profilePictureUrl,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Save the userId in TempData for the next request
                TempData["UserId"] = user.UserId;

                return RedirectToAction("CompleteRegistration");
            }

            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.FacebbookId));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CompleteRegistration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRegistration(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                if (TempData["UserId"] != null)
                {
                    int userId = (int)TempData["UserId"];
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.Username = model.Username;
                        user.Password = model.Password;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();

                        var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.FacebbookId));
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(model);
        }
        private string GenerateUserName(string email)
        {
            return email.Split('@')[0];
        }

        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8); // Random 8-character password
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("DangNhap", "Users");
            }

            var userInfo = await (from user in _context.Users
                                  join membership in _context.UserMemberships on user.UserId equals membership.UserId
                                  /*  join cls in _context.ClassSchedules on user.ClassSchedules equals cls.ScheduleId*/
                                  where user.UserId == userIdInt
                                  select new
                                  {
                                      user.FullName,
                                      user.Email,
                                      user.PhoneNumber,
                                      user.Avatar,
                                      user.Address,
                                      user.Dob,
                                      Gender = user.Gender == "male" ? "Nam" : "Nữ",
                                      MembershipType = user.MembershipType.Name,
                                      MembershipStarDate = membership.StartDate,
                                      MembershipEndDate = membership.EndDate
                                  }).FirstOrDefaultAsync();

            var registeredSchedules = await (from cs in _context.ClassSchedules
                                             join cls in _context.Classes on cs.ClassId equals cls.ClassId
                                             join trainer in _context.Trainers on cls.TrainerId equals trainer.TrainerId
                                             where cs.Room != null && cs.Room != ""
                                             select new
                                             {
                                                 cs.ScheduleId,
                                                 cs.Weekday,
                                                 cs.StartTime,
                                                 cs.EndTime,
                                                 cls.ClassType,
                                                 TrainerFullName = trainer.FullName
                                             }).ToListAsync();

            ViewBag.UserInfo = userInfo;
            ViewBag.RegisteredSchedules = registeredSchedules;
          
            return View();
        }


        [HttpPost]
        public IActionResult UpdateUserInfo(UserInfoViewModel model/*, int userId*/)
        {    // Lấy thông tin người dùng hiện tại từ cơ sở dữ liệu
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("DangNhap", "Users");
            }
            // Validate the model
            if (!ModelState.IsValid)
            {
                    return View(model); // Trả lại view với các lỗi
               }



            var user = _context.Users.SingleOrDefault(u => u.UserId == userIdInt);

            if (user != null)
            {
                // Cập nhật thông tin người dùng
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Gender = model.Gender;
                user.Address = model.Address;
                // Giả sử Dob và MembershipType là các trường tùy chọn
                user.Dob = model.Dob ?? user.Dob;
           /*     user.MembershipType = model.MembershipType ?? user.MembershipType;*/

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Users.Update(user);
                _context.SaveChanges();

                // Thông báo cập nhật thành công
                TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
            }

            return RedirectToAction("ThongTinCaNhan");
        }




        /* [HttpGet]
         public async Task<IActionResult> Logout()
         {
             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
             return RedirectToAction("Index", "Home");
         }
 */     [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra thông tin đăng nhập
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                    return View(model);
                }

                if (!(user.IsemailOnfirmed ?? false) && user.FacebbookId == null)
                {
                    ModelState.AddModelError(string.Empty, "Email của bạn chưa được xác thực. Vui lòng kiểm tra email và xác thực.");
                    return View(model);
                }

                // Lưu UserId vào session
                HttpContext.Session.SetString("UserId", user.UserId.ToString());


                // Tạo claims cho người dùng đã xác thực
                var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Username),
            // Thêm các claims khác nếu cần
        };

                /*// Lưu TrainerId vào session và chuyển hướng đến trang chi tiết của huấn luyện viên nếu người dùng là nhân viên hoặc PT
                if (user.Role == "Nhan vien" || user.Role == "PT")
                {
                    // Thêm role claim
                    claims.Add(new Claim(ClaimTypes.Role, user.Role));

                    // Lấy TrainerId và lưu vào session
                    var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.UserId);
                    if (trainer != null)
                    {
                        HttpContext.Session.SetString("TrainerId", trainer.TrainerId.ToString());
                        *//*return RedirectToAction("GetTrainerDetails", "Users");*//*
                        return RedirectToAction("Index", "Trainers");
                    }
                }*/
                if (user.Role == "Nhan vien")
                {
                    // Thêm role claim cho Nhan vien
                    claims.Add(new Claim(ClaimTypes.Role, "Nhan vien"));

                    // Lấy TrainerId và lưu vào session
                    var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.UserId);
                    if (trainer != null)
                    {
                        HttpContext.Session.SetString("TrainerId", trainer.TrainerId.ToString());
                    }

                    // Chuyển hướng đến Trainers/Index
                    return RedirectToAction("Index", "Trainers");
                }
                if (user.Role == "PT")
                {
                    // Thêm role claim cho Nhan vien
                    claims.Add(new Claim(ClaimTypes.Role, "PT"));

                    // Lấy TrainerId và lưu vào session
                    var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.UserId == user.UserId);
                    if (trainer != null)
                    {
                        HttpContext.Session.SetString("TrainerId", trainer.TrainerId.ToString());
                    }

                    // Chuyển hướng đến Trainers/Index Classes/HuanLuyenRieng
                    return RedirectToAction("HuanLuyenRieng", "Classes");
                }


                // Tạo identity cho người dùng đã xác thực
                var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Tạo principal cho người dùng đã xác thực
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                // Đăng nhập người dùng
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                // Chuyển hướng đến URL gốc hoặc URL mặc định
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Nếu model state không hợp lệ, trả về form đăng nhập với lỗi xác thực
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> linh2k3()
        {


            return RedirectToAction("GetTrainerDetails", "Users");
        }
        /* return RedirectToAction("GetTrainerDetails", "Users");*/
        [HttpGet]
        public async Task<IActionResult> GetTrainerDetails()
        {
            var trainerId = HttpContext.Session.GetString("TrainerId");
            if (string.IsNullOrEmpty(trainerId))
            {
                return RedirectToAction("DangNhap", "Users");
            }

            return RedirectToAction("TrainerDetails", new { trainerId = int.Parse(trainerId) });
        }

        [HttpGet]
        public async Task<IActionResult> TrainerDetails(int trainerId)
        {
            /*int? trainerId = HttpContext.Session.GetInt32("TrainerId");
            if (trainerId.HasValue && trainerId.Value > 0)
            {
                int userIdInt = trainerId.Value;
                // Tiếp tục xử lý logic của bạn
            }
            else
            {
                return RedirectToAction("DangNhap", "Users");
            }*/
            var trainerDetails = await (from trainer in _context.Trainers
                                        where trainer.TrainerId == trainerId
                                        select new
                                        {
                                            TrainerFullName = trainer.FullName,
                                            TrainerAvatar = trainer.User.Avatar,
                                            TrainerEmail = trainer.User.Email,
                                            TrainerPhone = trainer.User.PhoneNumber,
                                            TrainerAddress = trainer.User.Address,
                                            Classes = (from cls in _context.Classes
                                                       where cls.TrainerId == trainerId
                                                       select new
                                                       {
                                                           cls.ClassType,
                                                           ClassSchedules = (from schedule in _context.ClassSchedules
                                                                             where schedule.ClassId == cls.ClassId
                                                                             select new
                                                                             {
                                                                                 schedule.ScheduleId,
                                                                                 schedule.Weekday,
                                                                                 schedule.StartTime,
                                                                                 schedule.EndTime,
                                                                                 Students = (from user in _context.Users
                                                                                             join room in _context.ClassSchedules
                                                                                             on user.UserId.ToString() equals room.Room
                                                                                             where room.ScheduleId == schedule.ScheduleId
                                                                                             select new
                                                                                             {
                                                                                                 user.FullName,
                                                                                                 user.Email
                                                                                             }).ToList()
                                                                             }).ToList()
                                                       }).ToList()
                                        }).FirstOrDefaultAsync();

            if (trainerDetails == null)
            {
                return NotFound();
            }

            return View(trainerDetails);
        }


        /*     [HttpPost]
             public IActionResult UpdateTrainer(Trainer fullName)
             {




                 var trainerId = HttpContext.Session.GetString("TrainerId");
                 if (string.IsNullOrEmpty(trainerId) || !int.TryParse(trainerId, out int trainerIdInt))
                 {
                     return RedirectToAction("DangNhap", "Users");
                 }
                 var userId = HttpContext.Session.GetString("UserId");
                 if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                 {
                     return RedirectToAction("DangNhap", "Users");
                 }
                 // Lấy thông tin huấn luyện viên từ cơ sở dữ liệu
                 var trainer = _context.Trainers.Include(t => t.User).FirstOrDefault(u => u.TrainerId == trainerIdInt);



                 if (trainer != null)
                 {
                     // Cập nhật thông tin huấn luyện viên
                     trainer.FullName = fullName.FullName;
                     trainer.PhoneNumber = fullName.PhoneNumber ;
                     trainer.User.Address = fullName.User.Address;


                     // Cập nhật các trường khác tùy theo thông tin bạn muốn người dùng có thể chỉnh sửa
                     _context.SaveChanges();
                     TempData["SuccessMessage"] = "Cập nhật thông tin huấn luyện viên thành công!";
                 }
                 else
                 {
                     TempData["ErrorMessage"] = "Không tìm thấy thông tin huấn luyện viên.";
                 }

                 return RedirectToAction("TrainerDetails", new { trainerId = trainerIdInt });
             }*/
        [HttpPost]
        public IActionResult UpdateTrainer(Trainer fullName)
        {
            var userId = HttpContext.Session.GetString("TrainerId");
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("DangNhap", "Users");
            }

            // Tìm Trainer dựa trên userId và bao gồm thông tin User
            var trainer = _context.Trainers.Include(t => t.User).FirstOrDefault(t => t.TrainerId == userIdInt);

            if (trainer != null && trainer.User != null)
            {
                trainer.FullName = fullName.FullName;
                trainer.PhoneNumber = fullName.PhoneNumber;
                // Cập nhật địa chỉ của User liên kết với Trainer
                /*trainer.User.Address = fullName.User.Address;*/
                _context.Trainers.Update(trainer);
                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật thông tin huấn luyện viên thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin huấn luyện viên hoặc thông tin người dùng liên kết.";
            }

            return RedirectToAction("TrainerDetails", new { trainerId = userIdInt });
        }


            [HttpGet]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Users");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
                return RedirectToAction("Index", "Home");

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims
                .Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });

            var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FacebbookId == googleId);

            if (user == null)
            {
                user = new User
                {
                    FacebbookId = googleId,
                    FullName = name,
                    Email = email,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Save the userId in TempData for the next request
                TempData["UserId"] = user.UserId;

                return RedirectToAction("CompleteRegistration");
            }

            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.FacebbookId));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public async Task<IActionResult> thongtinnhanvien()
        {


            return RedirectToAction("GetNVDetails", "Users");
        }
        /* return RedirectToAction("GetTrainerDetails", "Users");*/
        [HttpGet]
        public async Task<IActionResult> GetNVDetails()
        {
            var trainerId = HttpContext.Session.GetString("TrainerId");
            if (string.IsNullOrEmpty(trainerId))
            {
                return RedirectToAction("DangNhap", "Users");
            }

            return RedirectToAction("NVDetails", new { trainerId = int.Parse(trainerId) });
        }

        [HttpGet]
        public async Task<IActionResult> NVDetails(int trainerId)
        {
            /*int? trainerId = HttpContext.Session.GetInt32("TrainerId");
            if (trainerId.HasValue && trainerId.Value > 0)
            {
                int userIdInt = trainerId.Value;
                // Tiếp tục xử lý logic của bạn
            }
            else
            {
                return RedirectToAction("DangNhap", "Users");
            }*/
            var trainerDetails = await (from trainer in _context.Trainers
                                        where trainer.TrainerId == trainerId
                                        select new
                                        {
                                            TrainerFullName = trainer.FullName,
                                            TrainerAvatar = trainer.User.Avatar,
                                            TrainerEmail = trainer.User.Email,
                                            TrainerPhone = trainer.User.PhoneNumber,
                                            TrainerAddress = trainer.User.Address,
                                            Classes = (from cls in _context.Classes
                                                       where cls.TrainerId == trainerId
                                                       select new
                                                       {
                                                           cls.ClassType,
                                                           ClassSchedules = (from schedule in _context.ClassSchedules
                                                                             where schedule.ClassId == cls.ClassId
                                                                             select new
                                                                             {
                                                                                 schedule.ScheduleId,
                                                                                 schedule.Weekday,
                                                                                 schedule.StartTime,
                                                                                 schedule.EndTime,
                                                                                 Students = (from user in _context.Users
                                                                                             join room in _context.ClassSchedules
                                                                                             on user.UserId.ToString() equals room.Room
                                                                                             where room.ScheduleId == schedule.ScheduleId
                                                                                             select new
                                                                                             {
                                                                                                 user.FullName,
                                                                                                 user.Email
                                                                                             }).ToList()
                                                                             }).ToList()
                                                       }).ToList()
                                        }).FirstOrDefaultAsync();

            if (trainerDetails == null)
            {
                return NotFound();
            }

            return View(trainerDetails);
        }



    }
}
