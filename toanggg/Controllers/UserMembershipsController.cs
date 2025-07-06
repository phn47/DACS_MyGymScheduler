using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using toanggg.Data;
using toanggg.Helpers;
using toanggg.Models;
using toanggg.Services;

namespace toanggg.Controllers
{
    public class UserMembershipsController : Controller
    {
        private readonly LinhContext _context;
        private readonly IVnPayService _vnPayservice;

        public UserMembershipsController(LinhContext context, IVnPayService vnPayservice)
        {
            _context = context;
            _vnPayservice = vnPayservice;
        }
        private User GetCustomer()
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID)?.Value;

            if (int.TryParse(customerId, out int customerIdInt)) // Kiểm tra xem customerId có thể được chuyển đổi sang int không
            {
                var customer = _context.Users.FirstOrDefault(kh => kh.UserId == customerIdInt);
                return customer;
            }
            else
            {
                // Xử lý trường hợp không thể chuyển đổi customerId sang int
                // Ví dụ: throw exception, trả về giá trị mặc định, hoặc làm gì đó phù hợp với logic của bạn
                // Ở đây, tôi sẽ trả về null
                return null;
            }
        }


        // GET: UserMemberships
        public async Task<IActionResult> Index()
        {
            var linhContext = _context.UserMemberships.Include(u => u.MembershipType).Include(u => u.User);
            return View(await linhContext.ToListAsync());
        }

        // GET: UserMemberships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userMembership = await _context.UserMemberships
                .Include(u => u.MembershipType)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserMembershipId == id);
            if (userMembership == null)
            {
                return NotFound();
            }

            return View(userMembership);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "Name");
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserMembershipId,UserId,MembershipTypeId,Status")] UserMembership userMembership, string payment = "Thanh toán VNPay")
        {
            if (ModelState.IsValid)
            {
                // Lấy UserId từ Session
                string userId = HttpContext.Session.GetString("UserId");
                if (userId != null)
                {
                    userMembership.UserId = int.Parse(userId);
                }

                // Kiểm tra xem tài khoản đã tồn tại và có MembershipTypeId hay chưa
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userMembership.UserId);
                if (existingUser != null && existingUser.MembershipTypeId.HasValue)
                {
                    TempData["ErrorMessage"] = "Hiện tại bạn đã đăng ký, không thể đăng ký thêm.";
                    return RedirectToAction("Create");
                }

                // Lấy thông tin MembershipType từ bảng MembershipTypes
                MembershipType membershipType = await _context.MembershipTypes
                    .FirstOrDefaultAsync(mt => mt.MembershipTypeId == userMembership.MembershipTypeId);

                if (membershipType != null && membershipType.AccessHours != null)
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = (double)(membershipType.Price ?? 0),
                        CreatedDate = DateTime.Now,
                        Description = string.Empty,// Mô tả đơn hàng dựa trên thông tin của userMembership,
                        FullName = string.Empty,// Tên đầy đủ của khách hàng dựa trên thông tin của userMembership,
                        OrderId = new Random().Next(1000, 100000)
                    };

                    // Phân tích chuỗi AccessHours thành giá trị số
                    if (int.TryParse(membershipType.AccessHours, out int accessHourValue))
                    {
                        // Gán StartDate và EndDate dựa trên accessHourValue
                        userMembership.StartDate = DateOnly.FromDateTime(DateTime.Today); // StartDate là ngày hiện tại
                        userMembership.EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(accessHourValue)); // EndDate là ngày hiện tại cộng với giá trị AccessHours
                    }
                    else
                    {
                        // Xử lý khi không thể phân tích chuỗi AccessHours thành giá trị số
                        // ...
                    }

                    // Lấy thông tin user từ cơ sở dữ liệu
                    var user = await _context.Users.FindAsync(userMembership.UserId);
                    if (user != null)
                    {
                        vnPayModel.FullName = user.FullName; // Lấy tên đầy đủ của user
                    }

                    _context.Add(userMembership);
                    await _context.SaveChangesAsync();
                    // Cập nhật MembershipTypeId cho user
                    if (existingUser != null)
                    {
                        existingUser.MembershipTypeId = userMembership.MembershipTypeId;
                        _context.Update(existingUser);
                        await _context.SaveChangesAsync();
                    }

                    return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                }
            }

            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "Name", userMembership.MembershipTypeId);
            return View(userMembership);
        }


        // GET: UserMemberships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userMembership = await _context.UserMemberships.FindAsync(id);
            if (userMembership == null)
            {
                return NotFound();
            }
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "MembershipTypeId", userMembership.MembershipTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", userMembership.UserId);
            return View(userMembership);
        }

        // POST: UserMemberships/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserMembershipId,UserId,MembershipTypeId,StartDate,EndDate,Status")] UserMembership userMembership)
        {
            if (id != userMembership.UserMembershipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userMembership);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserMembershipExists(userMembership.UserMembershipId))
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
            ViewData["MembershipTypeId"] = new SelectList(_context.MembershipTypes, "MembershipTypeId", "MembershipTypeId", userMembership.MembershipTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", userMembership.UserId);
            return View(userMembership);
        }

        // GET: UserMemberships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userMembership = await _context.UserMemberships
                .Include(u => u.MembershipType)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserMembershipId == id);
            if (userMembership == null)
            {
                return NotFound();
            }

            return View(userMembership);
        }

        // POST: UserMemberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userMembership = await _context.UserMemberships.FindAsync(id);
            if (userMembership != null)
            {
                _context.UserMemberships.Remove(userMembership);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserMembershipExists(int id)
        {
            return _context.UserMemberships.Any(e => e.UserMembershipId == id);
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();

        }
     

        public IActionResult Revenue(int selectedYear = 0)
        {
            var revenueByMonth = _context.UserMemberships
                .Include(e => e.MembershipType)
                .Where(e => e.StartDate.HasValue && e.EndDate.HasValue && e.MembershipType != null && e.MembershipType.AccessHours != null)
                .Select(membership => new
                {
                    StartMonth = membership.StartDate.Value.Month,
                    StartYear = membership.StartDate.Value.Year,
                    EndMonth = membership.EndDate.Value.Month,
                    EndYear = membership.EndDate.Value.Year,
                    TotalMonths = (membership.EndDate.Value.Year - membership.StartDate.Value.Year) * 12 + membership.EndDate.Value.Month - membership.StartDate.Value.Month,
                    PricePerMonth = membership.MembershipType.Price / ((membership.EndDate.Value.Year - membership.StartDate.Value.Year) * 12 + membership.EndDate.Value.Month - membership.StartDate.Value.Month)
                })
                .AsEnumerable()
                .SelectMany(membership => Enumerable.Range(0, membership.TotalMonths).Select(offset => new
                {
                    Year = membership.StartYear + (membership.StartMonth - 1 + offset) / 12,
                    Month = (membership.StartMonth - 1 + offset) % 12 + 1,
                    membership.PricePerMonth
                }))
                .GroupBy(membership => new { membership.Year, membership.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(membership => membership.PricePerMonth)
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            var distinctYears = revenueByMonth.Select(r => r.Year).Distinct().OrderBy(y => y).ToList();
            ViewBag.Years = distinctYears;
            ViewBag.SelectedYear = selectedYear;

            if (selectedYear != 0)
            {
                revenueByMonth = revenueByMonth.Where(r => r.Year == selectedYear).ToList();
            }

            return View(revenueByMonth);
        }


    }
}
