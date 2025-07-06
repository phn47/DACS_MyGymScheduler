using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using toanggg.Data;
using toanggg.Models;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Http;
using NuGet.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using toanggg.Data;
using toanggg.Models;
using toanggg.Services;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace toanggg.Controllers
{

    public class ClassSchedulesController : Controller
    {
        private readonly LinhContext _context;
        private readonly IConfiguration _configuration;
       /* private readonly IUrlHelper _urlHelper;*/
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ClassSchedulesController(LinhContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

     /*   public ActionResult Linh()
        {
            List<ClassSchedule> classSchedules = GetClassSchedulesFromDatabase();
            return View(classSchedules);
        }*/

        // GET: ClassSchedules
        public async Task<IActionResult> Index()
        {

            var linhContext = _context.ClassSchedules.Include(c => c.Class);
            return View(await linhContext.ToListAsync());
        }

        /*   private List<ClassSchedule> GetClassSchedulesFromDatabase()
           {
               string connectionString = "Data Source=LAPTOP-T4RGGNJJ;Initial Catalog=DACS;Integrated Security=True;Trust Server Certificate=True";

               using (SqlConnection connection = new SqlConnection(connectionString))
               {
                   connection.Open();

                   string query = "SELECT * FROM ClassSchedule"; // Thay đổi truy vấn theo cấu trúc bảng của bạn
                   var classSchedules = connection.Query<ClassSchedule>(query).ToList();

                   return classSchedules;
               }
           }*/

        // GET: ClassSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classSchedule = await _context.ClassSchedules
                .Include(c => c.Class)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (classSchedule == null)
            {
                return NotFound();
            }

            return View(classSchedule);
        }
        /*    [HttpGet]
            public IActionResult LichTrinh()
            {
                // Lấy thông tin lịch học từ cơ sở dữ liệu hoặc từ bất kỳ nguồn nào khác
                return View();
            }*/
        [HttpGet]
        public IActionResult LichTrinh(string weekday, TimeOnly startTime )
        {
         return View();
        }



        // GET: ClassSchedules/Create
        public IActionResult Create()
        {
            // Lấy tất cả các lớp từ bảng Classes
            var allClasses = _context.Classes;

            // Lấy các ClassId đã tồn tại trong bảng ClassSchedule
            var scheduledClassIds = _context.ClassSchedules.Select(cs => cs.ClassId).ToList();

            // Lọc các lớp chưa tồn tại trong bảng ClassSchedule
            var availableClasses = allClasses.Where(c => !scheduledClassIds.Contains(c.ClassId)).ToList();

            // Tạo SelectList chỉ với những lớp chưa tồn tại trong bảng ClassSchedule
            ViewData["ClassId"] = new SelectList(availableClasses, "ClassId", "Name");

            return View();
        }

        // POST: ClassSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*      [HttpPost]
              [ValidateAntiForgeryToken]
              public async Task<IActionResult> Create([Bind("ScheduleId,ClassId,StartDate,EndDate,Weekday,StartTime,EndTime,Room,Status")] ClassSchedule classSchedule)
              {
                  if (ModelState.IsValid)
                  {
                   *//*   if (classSchedule.Room == null)
                      {
                          classSchedule.Room = string.Empty;
                      }*//*
                      // Lấy thông tin lớp học từ ClassId
                      var selectedClass = await _context.Classes.FindAsync(classSchedule.ClassId);



                      // Thêm ClassSchedule vào context
                      _context.Add(classSchedule);
                      await _context.SaveChangesAsync();
                      return RedirectToAction(nameof(Index));
                  }
                  // Nếu ModelState không hợp lệ, trả về view với dữ liệu và thông báo lỗi
                  ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassType", classSchedule.ClassId);
                  return View(classSchedule);
              }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,ClassId,StartDate,EndDate,Weekday,StartTime,EndTime,Room,Status")] ClassSchedule classSchedule)
        {
            if (ModelState.IsValid)
            {
                // Set StartDate to today
                classSchedule.StartDate = DateOnly.FromDateTime(DateTime.Today);

                // Assuming you have a way to get the user membership, e.g., by UserId
                var userMembership = await _context.UserMemberships.FirstOrDefaultAsync(um => um.UserId == classSchedule.UserId);
                if (userMembership != null)
                {
                    classSchedule.EndDate = userMembership.EndDate;
                }

                // Find the class by ClassId
                var selectedClass = await _context.Classes.FindAsync(classSchedule.ClassId);
                if (selectedClass == null)
                {
                    ModelState.AddModelError("ClassId", "Class not found.");
                    ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "Name", classSchedule.ClassId);
                    return View(classSchedule);
                }

                // Add ClassSchedule to context
                _context.Add(classSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // If ModelState is invalid, return the view with data and error messages
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassType", classSchedule.ClassId);
            return View(classSchedule);
        }


        // GET: ClassSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classSchedule = await _context.ClassSchedules.FindAsync(id);
            if (classSchedule == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", classSchedule.ClassId);
            return View(classSchedule);
        }

        // POST: ClassSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,ClassId,StartDate,EndDate,Weekday,StartTime,EndTime,Room,Status")] ClassSchedule classSchedule)
        {
            if (id != classSchedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassScheduleExists(classSchedule.ScheduleId))
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", classSchedule.ClassId);
            return View(classSchedule);
        }

        // GET: ClassSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classSchedule = await _context.ClassSchedules
                .Include(c => c.Class)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (classSchedule == null)
            {
                return NotFound();
            }

            return View(classSchedule);
        }

        // POST: ClassSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classSchedule = await _context.ClassSchedules.FindAsync(id);
            if (classSchedule != null)
            {
                _context.ClassSchedules.Remove(classSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassScheduleExists(int id)
        {
            return _context.ClassSchedules.Any(e => e.ScheduleId == id);
        }

        // GET: ClassSchedules/DangKyLopPT
        // GET: ClassSchedules/DangKyLopPT
        // GET: ClassSchedules/DangKyLopPT
        public async Task<IActionResult> DangKyLopPT()
        {
            var classList = from c in _context.Classes
                            join t in _context.Trainers on c.TrainerId equals t.TrainerId
                            where c.Status == null && !_context.ClassSchedules.Any(cs => cs.ClassId == c.ClassId)
                            select new
                            {
                                ClassId = c.ClassId,
                                TrainerFullName = t.FullName
                            };

            ViewData["ClassId"] = new SelectList(await classList.ToListAsync(), "ClassId", "TrainerFullName");
            return View();
        }

        // POST: ClassSchedules/DangKyLopPT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyLopPT([Bind("ScheduleId,ClassId,StartDate,EndDate,Weekday,StartTime,EndTime,Room,Status")] ClassSchedule classSchedule)
        {
            if (ModelState.IsValid)
            {
                if (classSchedule.Room == null)
                {
                    classSchedule.Room = null;
                }
                // Lấy thông tin lớp học từ ClassId
                var selectedClass = await _context.Classes.FindAsync(classSchedule.ClassId);
                classSchedule.Weekday = "";

                // Thêm ClassSchedule vào context
                _context.Add(classSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, trả về view với dữ liệu và thông báo lỗi
            var classList = from c in _context.Classes
                            join t in _context.Trainers on c.TrainerId equals t.TrainerId
                            where c.Status == null && !_context.ClassSchedules.Any(cs => cs.ClassId == c.ClassId)
                            select new
                            {
                                ClassId = c.ClassId,
                                TrainerFullName = t.FullName
                            };

            ViewData["ClassId"] = new SelectList(await classList.ToListAsync(), "ClassId", "TrainerFullName", classSchedule.ClassId);
            return View(classSchedule);
        }









        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyLopPT2(string trainerFullName)
        {
            if (string.IsNullOrEmpty(trainerFullName))
            {
                // Xử lý khi không có tên huấn luyện viên được chọn
                return RedirectToAction("DisplaySchedulesWithNullRoom");
            }

            // Tìm lịch học dựa trên tên huấn luyện viên
            var classSchedule = await _context.ClassSchedules
                .Include(cs => cs.Class)
                .ThenInclude(cls => cls.Trainer)
                .FirstOrDefaultAsync(cs => cs.Class.Trainer.FullName == trainerFullName && cs.Room == "");

            if (classSchedule != null)
            {
                // Lấy UserId hiện tại từ Session
                var userId = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userId))
                {
                    classSchedule.UserId = Convert.ToInt32(userId);
                }

                // Gán giá trị cho StartTime và EndTime từ các trường nhập liệu
                var startTimeInput = Request.Form["startTimeInput"].ToString();
                var endTimeInput = Request.Form["endTimeInput"].ToString();

                DateTime startTimeDateTime;
                DateTime endTimeDateTime;

                if (DateTime.TryParse(startTimeInput, out startTimeDateTime) &&
                    DateTime.TryParse(endTimeInput, out endTimeDateTime))
                {
                    TimeOnly startTime = new TimeOnly(startTimeDateTime.Hour, startTimeDateTime.Minute, startTimeDateTime.Second);
                    TimeOnly endTime = new TimeOnly(endTimeDateTime.Hour, endTimeDateTime.Minute, endTimeDateTime.Second);

                    // Lấy các giá trị checkbox cho các ngày trong tuần và chuyển đổi thành tên ngày
                    var selectedWeekdays = Request.Form["weekdays"]
                        .Select(day => GetDayName(int.Parse(day)))
                        .ToList();
                    var weekdays = string.Join(", ", selectedWeekdays);

                    classSchedule.StartTime = startTime;
                    classSchedule.EndTime = endTime;
                    classSchedule.Weekday = weekdays;

                    // Cập nhật cột Room thành "PT"
                    classSchedule.Room = "PT";

                    _context.Update(classSchedule);
                    var overlappingSchedules = await _context.ClassSchedules
                        .Include(cs => cs.Class)
                        .ThenInclude(cls => cls.Trainer)
                        .Where(cs => cs.Class.TrainerId == classSchedule.Class.TrainerId &&
                                     cs.StartTime < endTime &&
                                     cs.EndTime > startTime &&
                                     cs.ClassId != classSchedule.ClassId)
                        .ToListAsync();

                    // Kiểm tra trùng lặp về thời gian và ngày trong tuần
                    var conflictingDays = new List<string>();
                    var conflictingSchedules = new List<string>();
                    foreach (var schedule in overlappingSchedules)
                    {
                        var scheduleWeekdays = schedule.Weekday.Split(", ").ToList();
                        var intersectingDays = scheduleWeekdays.Intersect(selectedWeekdays).ToList();
                        if (intersectingDays.Any())
                        {
                            conflictingDays.AddRange(intersectingDays);
                            conflictingSchedules.Add($"Ngày {string.Join(", ", intersectingDays)} huấn luyện viên {schedule.Class.Trainer.FullName} bị trùng lớp {schedule.Class.ClassType} từ {schedule.StartTime} đến {schedule.EndTime} ");
                        }
                    }

                    if (conflictingDays.Any())
                    {
                        ViewBag.ErrorMessage = "Huấn luyện viên có lịch dạy trùng vào các ngày và thời gian sau: " + string.Join("; ", conflictingSchedules.Distinct());
                        return View("Error");
                    }

                    await _context.SaveChangesAsync();

                    // Lấy email người dùng từ bảng Users
                    var user = await _context.Users.FindAsync(classSchedule.UserId);

                    // Lấy thông tin UserMembership của người dùng
                    var userMembership = await _context.UserMemberships.FirstOrDefaultAsync(um => um.UserId == classSchedule.UserId);

                    if (user != null && userMembership != null)
                    {
                        var trainer = classSchedule.Class.Trainer;
                        var emailService = new EmailSender(_configuration); // Giả sử bạn có dịch vụ gửi email

                        // Gửi email thông báo cho học viên
                        var userEmailBody = $"Chúc mừng! Bạn đã đăng ký thành công lớp học với huấn luyện viên {trainerFullName} vào các ngày {weekdays}. Thời gian bắt đầu: {classSchedule.StartTime}. Thời gian đến hạn gói: {classSchedule.EndTime}. Loại lớp: {classSchedule.Class.ClassType}.";
                        userEmailBody += $" Ngày kết thúc của UserMembership: {userMembership.EndDate}.";
                        await emailService.SendEmailAsync(user.Email, "Thông báo đăng ký lớp học thành công", userEmailBody);

                        // Gửi email thông báo cho huấn luyện viên
                        var trainerEmailBody = $"Thông báo! Học viên {user.FullName} đã đăng ký lớp học với bạn vào các ngày {weekdays}. Thời gian bắt đầu: {classSchedule.StartTime}. Thời gian kết thúc khóa PT: {classSchedule.EndTime}. Loại lớp: {classSchedule.Class.ClassType}.";
                        trainerEmailBody += $" Ngày kết thúc của UserMembership: {userMembership.EndDate}.";
                        await emailService.SendEmailAsync(trainer.Email, "Thông báo đăng ký lớp học", trainerEmailBody);
                    }

                    // Chuyển hướng đến trang thành công hoặc thực hiện hành động mong muốn
                    return RedirectToAction("Index", "ClassSchedules");
                }
                else
                {
                    // Xử lý trường hợp không thể chuyển đổi chuỗi thành DateTime
                    return RedirectToAction("DisplaySchedulesWithNullRoom");
                }
            }
            else
            {
                // Xử lý trường hợp không tìm thấy lịch học hoặc lớp đã đầy
                return RedirectToAction("DisplaySchedulesWithNullRoom");
            }
        }*/






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyLopPT2(string trainerFullName)
        {
            if (string.IsNullOrEmpty(trainerFullName))
            {
                // Xử lý khi không có tên huấn luyện viên được chọn
                return RedirectToAction("DisplaySchedulesWithNullRoom");
            }

            // Tìm lịch học dựa trên tên huấn luyện viên
            var classSchedule = await _context.ClassSchedules
                .Include(cs => cs.Class)
                .ThenInclude(cls => cls.Trainer)
                .FirstOrDefaultAsync(cs => cs.Class.Trainer.FullName == trainerFullName && cs.Room == null);

            if (classSchedule != null)
            {
                // Lấy UserId hiện tại từ Session
                var userId = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userId))
                {
                    classSchedule.UserId = Convert.ToInt32(userId);
                }

                // Kiểm tra AccesHour của MembershipType
                var userMembership = await _context.UserMemberships
                    .Include(um => um.MembershipType)
                    .FirstOrDefaultAsync(um => um.UserId == classSchedule.UserId);

                int accessHours;
                bool isAccessHoursValid = int.TryParse(userMembership?.MembershipType?.AccessHours, out accessHours);

                if (/*!isAccessHoursValid ||*/ accessHours < 180)
                {
                    // Xử lý trường hợp AccesHour <= 180
                    ViewBag.ErrorMessage = "Chỉ có gói 6 tháng mới được sử dụng dịch vụ này.";
                    return View("Error");
                }

                // Gán giá trị cho StartTime và EndTime từ các trường nhập liệu
                var startTimeInput = Request.Form["startTimeInput"].ToString();
                var endTimeInput = Request.Form["endTimeInput"].ToString();

                DateTime startTimeDateTime;
                DateTime endTimeDateTime;

                if (DateTime.TryParse(startTimeInput, out startTimeDateTime) &&
                    DateTime.TryParse(endTimeInput, out endTimeDateTime))
                {
                    TimeOnly startTime = new TimeOnly(startTimeDateTime.Hour, startTimeDateTime.Minute, startTimeDateTime.Second);
                    TimeOnly endTime = new TimeOnly(endTimeDateTime.Hour, endTimeDateTime.Minute, endTimeDateTime.Second);

                    // Lấy các giá trị checkbox cho các ngày trong tuần và chuyển đổi thành tên ngày
                    var selectedWeekdays = Request.Form["weekdays"]
                        .Select(day => GetDayName(int.Parse(day)))
                        .ToList();
                    var weekdays = string.Join(", ", selectedWeekdays);

                    classSchedule.StartTime = startTime;
                    classSchedule.EndTime = endTime;
                    classSchedule.Weekday = weekdays;

                    // Cập nhật cột Room thành "PT"
                    classSchedule.Room = "PT";

                    _context.Update(classSchedule);
                    var overlappingSchedules = await _context.ClassSchedules
                        .Include(cs => cs.Class)
                        .ThenInclude(cls => cls.Trainer)
                        .Where(cs => cs.Class.TrainerId == classSchedule.Class.TrainerId &&
                                     cs.StartTime < endTime &&
                                     cs.EndTime > startTime &&
                                     cs.ClassId != classSchedule.ClassId)
                        .ToListAsync();

                    // Kiểm tra trùng lặp về thời gian và ngày trong tuần
                    var conflictingDays = new List<string>();
                    var conflictingSchedules = new List<string>();
                    foreach (var schedule in overlappingSchedules)
                    {
                        var scheduleWeekdays = schedule.Weekday.Split(", ").ToList();
                        var intersectingDays = scheduleWeekdays.Intersect(selectedWeekdays).ToList();
                        if (intersectingDays.Any())
                        {
                            conflictingDays.AddRange(intersectingDays);
                            conflictingSchedules.Add($"Ngày {string.Join(", ", intersectingDays)} huấn luyện viên {schedule.Class.Trainer.FullName} bị trùng lớp {schedule.Class.ClassType} từ {schedule.StartTime} đến {schedule.EndTime} ");
                        }
                    }

                    if (conflictingDays.Any())
                    {
                        ViewBag.ErrorMessage = "Huấn luyện viên có lịch dạy trùng vào các ngày và thời gian sau: " + string.Join("; ", conflictingSchedules.Distinct());
                        return View("Error");
                    }

                    await _context.SaveChangesAsync();

                    // Lấy email người dùng từ bảng Users
                    var user = await _context.Users.FindAsync(classSchedule.UserId);

                    if (user != null && userMembership != null)
                    {
                        var trainer = classSchedule.Class.Trainer;
                        var emailService = new EmailSender(_configuration); // Giả sử bạn có dịch vụ gửi email

                        var cultureInfo = new System.Globalization.CultureInfo("vi-VN");
                        var dayOfWeek = cultureInfo.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);
                        var formattedDate = DateTime.Now.ToString("dd/MM/yyyy", cultureInfo);

                        // Gửi email thông báo cho học viên
                        var userEmailBody = $@"
                <html>
                <body>
                    <h2>Chúc mừng!</h2>
                    <p>Bạn đã đăng ký thành công lớp học với huấn luyện viên <strong>{trainerFullName}</strong> vào các ngày {weekdays}.</p>
                    <p><strong>Thời gian bắt đầu:</strong> {classSchedule.StartTime}<br>
                    <strong>Thời gian kết thúc:</strong> {classSchedule.EndTime}</p>
                    <p><strong>Loại lớp:</strong> {classSchedule.Class.ClassType}</p>
                    <p><strong>Ngày kết thúc của UserMembership:</strong> {userMembership.EndDate}</p>
                </body>
                </html>";

                        await emailService.SendEmailAsync(user.Email, "Thông báo đăng ký lớp học thành công", userEmailBody);

                        // Gửi email thông báo cho huấn luyện viên
                        var trainerEmailBody = $@"
                <html>
                <body>
                    <h2>Thông báo!</h2>
                    <p>Học viên <strong>{user.FullName}</strong> đã đăng ký lớp học với bạn vào các ngày {weekdays}.</p>
                    <p><strong>Thời gian bắt đầu:</strong> {classSchedule.StartTime}<br>
                    <strong>Thời gian kết thúc:</strong> {classSchedule.EndTime}</p>
                    <p><strong>Loại lớp:</strong> {classSchedule.Class.ClassType}</p>
                    <p><strong>Ngày kết thúc của UserMembership:</strong> {userMembership.EndDate}</p>
                </body>
                </html>";

                        await emailService.SendEmailAsync(trainer.Email, "Thông báo đăng ký lớp học", trainerEmailBody);
                    }

                    // Chuyển hướng đến trang thành công hoặc thực hiện hành động mong muốn
                    return RedirectToAction("Index", "ClassSchedules");
                }
                else
                {
                    // Xử lý trường hợp không thể chuyển đổi chuỗi thành DateTime
                    return RedirectToAction("DisplaySchedulesWithNullRoom");
                }
            }
            else
            {
                // Xử lý trường hợp không tìm thấy lịch học hoặc lớp đã đầy
                return RedirectToAction("DisplaySchedulesWithNullRoom");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckInClass(int scheduleId)
        {
            var classSchedule = await _context.ClassSchedules
                .Include(cs => cs.Class)
                .ThenInclude(cls => cls.Trainer)
                .FirstOrDefaultAsync(cs => cs.ScheduleId == scheduleId);

            if (classSchedule == null)
            {
                return NotFound();
            }

            classSchedule.IsemailOnfirmed = true;
            _context.Update(classSchedule);
            await _context.SaveChangesAsync();

            // Gửi email thông báo cho huấn luyện viên
            var user = await _context.Users.FindAsync(classSchedule.UserId);
            var trainer = classSchedule.Class.Trainer;
            var emailService = new EmailSender(_configuration); // Giả sử bạn có dịch vụ gửi email

            var currentDay = DateTime.Now;
            var cultureInfo = new System.Globalization.CultureInfo("vi-VN");
            var dayOfWeek = cultureInfo.DateTimeFormat.GetDayName(currentDay.DayOfWeek);
            var formattedDate = currentDay.ToString("dd/MM/yyyy", cultureInfo);

            var trainerEmailBody = $@"
        Thông báo! Học viên {user.FullName} đã điểm danh vào lớp học của bạn vào ngày {dayOfWeek}.<br>
        Thời gian bắt đầu: {classSchedule.StartTime}.<br>
        Thời gian kết thúc: {classSchedule.EndTime}.<br>
        Loại lớp: {classSchedule.Class.ClassType}.<br>
        Ngày điểm danh: {formattedDate}<br>
    ";
            await emailService.SendEmailAsync(trainer.Email, "Thông báo điểm danh", trainerEmailBody);

            return View("CheckInSuccess");
        }


        public IActionResult CheckInSuccess()
        {

            return View();
        }

/*
        [HttpGet]
        public async Task<IActionResult> DiemDanh(int scheduleId, string token)
        {
            var schedule = await _context.ClassSchedules
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId && s.EmailconfirmationToken == token);

            if (schedule == null)
            {
                return NotFound("Invalid schedule or token.");
            }

            schedule.IsemailOnfirmed = true;
            _context.Update(schedule);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
*/
        private string GetDayName(int dayNumber)
        {
            switch (dayNumber)
            {
                case 2:
                    return "Thứ 2";
                case 3:
                    return "Thứ 3";
                case 4:
                    return "Thứ 4";
                case 5:
                    return "Thứ 5";
                case 6:
                    return "Thứ 6";
                case 7:
                    return "Thứ 7";
                case 8:
                    return "Chủ nhật";
                default:
                    return "";
            }
        }


        public IActionResult Error()
        {
           
            return View();
        }


        // GET: ClassSchedules/DisplaySchedulesWithNullRoom
        public IActionResult DisplaySchedulesWithNullRoom()
        {
            var schedulesWithNullRoom = _context.ClassSchedules
                .Include(cs => cs.Class)
                .ThenInclude(c => c.Trainer)
                .Where(cs => cs.Weekday == null && cs.UserId == null && cs.Room == null)
                .ToList();

            // Kiểm tra nếu danh sách null hoặc rỗng
            if (schedulesWithNullRoom == null || !schedulesWithNullRoom.Any())
            {
                ViewBag.Message = "Không có lịch học nào có Room là null.";
                schedulesWithNullRoom = new List<ClassSchedule>();
            }

            return View(schedulesWithNullRoom);
        }

        /*   [HttpPost]
    *//*       [RequireHttps]*//*
           public async Task<IActionResult> SendWeeklyEmails()
           {
               var today = DateOnly.FromDateTime(DateTime.Today);
               var users = await _context.UserMemberships
                   .Include(um => um.User)
                   .Where(um => um.EndDate.HasValue && um.EndDate.Value >= today)
                   .ToListAsync();

               var currentDayOfWeek = DateTime.Today.DayOfWeek;

               foreach (var userMembership in users)
               {
                   var classSchedules = await _context.ClassSchedules
                       .Include(cs => cs.Class)
                       .ThenInclude(cls => cls.Trainer)
                       .Where(cs => cs.UserId == userMembership.UserId)
                       .ToListAsync();

                   foreach (var schedule in classSchedules)
                   {
                       var scheduleWeekdays = schedule.Weekday.Split(", ").ToList();
                       foreach (var day in scheduleWeekdays)
                       {
                           if (GetDayOfWeek(day) == currentDayOfWeek *//*&& !(schedule.IsemailOnfirmed ?? false)*//*)
                           {
                               var trainer = schedule.Class.Trainer;
                               var emailService = new EmailSender(_configuration);

                               var Token = Guid.NewGuid().ToString();
                               schedule.EmailconfirmationToken = Token;
                               schedule.IsemailOnfirmed = false; // Đảm bảo là false trước khi gửi email
                               _context.Update(schedule);
                               await _context.SaveChangesAsync();

                               var request = _httpContextAccessor.HttpContext?.Request;
                             *//*  var confirmationLink = Url.Action("ConfirmEmail", "Users", new { userId = schedule.UserId, token = user.EmailconfirmationToken }, Request.Scheme);*/
        /*                            var confirmationLink = Url.Action("DiemDanh", "ClassSchedules", new { scheduleId = schedule.ScheduleId, token = schedule.EmailconfirmationToken }, Request.Scheme);*/
        /* var confirmationLink = Url.Action("ConfirmEmail", "Users", new { userId = user.UserId, token = user.EmailconfirmationToken }, Request.Scheme);*//*
        var userEmailBody = $"Thông báo! Bạn có lớp học với huấn luyện viên {trainer.FullName} vào {day}. Thời gian bắt đầu: {schedule.StartTime}. Thời gian kết thúc: {schedule.EndTime}.";
            userEmailBody += $" Ngày kết thúc của UserMembership: {userMembership.EndDate}.";
            userEmailBody += $" Vui lòng điểm danh bằng cách nhấn vào <a href=\"\">liên kết này</a>.";

            await emailService.SendEmailAsync(userMembership.User.Email, "Nhắc nhở lớp học hàng tuần", userEmailBody);

    }
}
}
}

return Ok();
}*/
  /*      [HttpGet]
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
*/


        public async Task<IActionResult> DiemDanh(int scheduleId, string token)
        {
            var schedule = await _context.ClassSchedules.FindAsync(scheduleId);
            if (schedule != null && schedule.EmailconfirmationToken == token)
            {
                schedule.IsemailOnfirmed = true;
                _context.Update(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("CheckInClass", new { scheduleId = scheduleId });
            }
            return new NotFoundResult();
        }

        private DayOfWeek GetDayOfWeek(string dayName)
        {
            return dayName switch
            {
                "Thứ 2" => DayOfWeek.Monday,
                "Thứ 3" => DayOfWeek.Tuesday,
                "Thứ 4" => DayOfWeek.Wednesday,
                "Thứ 5" => DayOfWeek.Thursday,
                "Thứ 6" => DayOfWeek.Friday,
                "Thứ 7" => DayOfWeek.Saturday,
                "Chủ nhật" => DayOfWeek.Sunday,
                _ => throw new ArgumentException("Invalid day name", nameof(dayName)),
            };
        }

        private DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }




    }
}
