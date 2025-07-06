using toanggg.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using toanggg.Models;
using toanggg.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using toanggg.Controllers;
using System.Security.Policy;

namespace toanggg.Services
{
    public class EmailService 
    {
        private readonly LinhContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(LinhContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }





        public async Task SendWeeklyEmails()
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

                bool emailSent = false;

                foreach (var schedule in classSchedules)
                {
                    var scheduleWeekdays = schedule.Weekday.Split(", ").ToList();

                    foreach (var day in scheduleWeekdays)
                    {
                        if (GetDayOfWeek(day) == currentDayOfWeek)
                        {
                            var trainer = schedule.Class.Trainer;
                            var emailService = new EmailSender(_configuration);

                            var token = Guid.NewGuid().ToString();
                            schedule.EmailconfirmationToken = token;
                            schedule.IsemailOnfirmed = false; // Đảm bảo là false trước khi gửi email
                            _context.Update(schedule);
                            await _context.SaveChangesAsync();

                            var confirmationLink = $"https://localhost:7014/ClassSchedules/DiemDanh?scheduleId={schedule.ScheduleId}&token={token}";
                            var userEmailBody = $"Thông báo! Bạn có lớp học với huấn luyện viên {trainer.FullName} vào {day}. Thời gian bắt đầu: {schedule.StartTime}. Thời gian kết thúc: {schedule.EndTime}.";
                            userEmailBody += $" Ngày kết thúc của UserMembership: {userMembership.EndDate}.";
                            userEmailBody += $" Vui lòng điểm danh bằng cách nhấn vào <a href=\"{confirmationLink}\">liên kết này</a>.";

                            await emailService.SendEmailAsync(userMembership.User.Email, "Nhắc nhở lớp học hàng tuần", userEmailBody);

                            emailSent = false; // Đánh dấu là email đã được gửi
                            break; // Thoát khỏi vòng lặp nếu email đã được gửi
                        }
                    }

                    if (emailSent)
                    {
                        break; // Nếu email đã được gửi, dừng vòng lặp các lịch trình
                    }
                }

                if (!emailSent)
                {
                    // Nếu không có ngày nào trong tuần khớp với ngày hiện tại, kết thúc hàm
                    return;
                }
            }
        }

        public async Task<IActionResult> DiemDanh(Guid scheduleId, string token)
        {
            var schedule = await _context.ClassSchedules.FindAsync(scheduleId);
            if (schedule != null && schedule.EmailconfirmationToken == token)
            {
                schedule.IsemailOnfirmed = true;
                _context.Update(schedule);
                await _context.SaveChangesAsync();
                return new RedirectResult("~/Home/Index");
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
