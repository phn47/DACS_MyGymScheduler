using Microsoft.EntityFrameworkCore;
using System.Linq;
using toanggg.Data;
using toanggg.Models;

namespace toanggg.Services
{
    public class SearchService
    {
        private readonly LinhContext _context;

        public SearchService(LinhContext context)
        {
            _context = context;
        }



        public async Task<SearchResultsViewModel> SearchAsync(string keyword)
        {
            var results = new SearchResultsViewModel();
            var keywordTokens = TokenizeString(keyword);

            // Tìm kiếm trong bảng Gyms
            var gymResults = await _context.Gyms
                .Select(g => new SearchResult
                {
                    TableName = "Gyms",
                    Id = g.GymId,
                    Name = g.Name,
                    Description = g.Description,
                    Address = g.Address
                })
                .ToListAsync();

            foreach (var gymResult in gymResults)
            {
                var descriptionTokens = TokenizeString(gymResult.Description);
                var addressTokens = TokenizeString(gymResult.Address);
                double matchRatio = CalculateTokenMatchRatio(keywordTokens, descriptionTokens.Concat(addressTokens).ToList());
                if (matchRatio >= 0.15)
                {
                    results.Gyms.Add(gymResult);
                }
            }

            // Tìm kiếm trong bảng Trainers
            var trainerResults = await _context.Trainers
                .Select(t => new SearchResult
                {
                    TableName = "Trainers",
                    Id = t.TrainerId,
                    Name = t.FullName,
                    Description = t.Description,
                    GymId = t.GymId // Assume Trainer has GymId
                })
                .ToListAsync();

            foreach (var trainerResult in trainerResults)
            {
                var descriptionTokens = TokenizeString(trainerResult.Description);
                double matchRatio = CalculateTokenMatchRatio(keywordTokens, descriptionTokens);
                if (matchRatio >= 0.3)
                {
                    results.Trainers.Add(trainerResult);
                }
            }

            // Tìm kiếm trong bảng MembershipTypes
            var membershipTypeResults = await _context.MembershipTypes
                .Select(mt => new SearchResult
                {
                    TableName = "MembershipTypes",
                    Id = mt.MembershipTypeId,
                    Name = mt.Name,
                    Description = mt.Description
                })
                .ToListAsync();

            foreach (var membershipTypeResult in membershipTypeResults)
            {
                var descriptionTokens = TokenizeString(membershipTypeResult.Description);
                double matchRatio = CalculateTokenMatchRatio(keywordTokens, descriptionTokens);
                if (matchRatio >= 0.3)
                {
                    results.MembershipTypes.Add(membershipTypeResult);
                }
            }

            // Tìm kiếm trong bảng GymTypes
            var gymTypeResults = await _context.GymTypes
                .Select(gt => new SearchResult
                {
                    TableName = "GymTypes",
                    Id = gt.GymTypeId,
                    Name = gt.Name,
                    Description = gt.Description
                })
                .ToListAsync();

            foreach (var gymTypeResult in gymTypeResults)
            {
                var descriptionTokens = TokenizeString(gymTypeResult.Description);
                double matchRatio = CalculateTokenMatchRatio(keywordTokens, descriptionTokens);
                if (matchRatio >= 0.3)
                {
                    results.GymTypes.Add(gymTypeResult);
                }
            }

            // Nếu không có kết quả trực tiếp cho Trainer, tìm Trainer thuộc Gym
            if (!results.Trainers.Any())
            {
                var gymIdsWithKeyword = results.Gyms.Select(g => g.Id).ToList();
                var trainersInGyms = await _context.Trainers
                    .Where(t => t.GymId.HasValue && gymIdsWithKeyword.Contains(t.GymId.Value))
                    .Select(t => new SearchResult
                    {
                        TableName = "Trainers",
                        Id = t.TrainerId,
                        Name = t.FullName,
                        Description = t.Description,
                        GymId = t.GymId
                    })
                    .ToListAsync();

                results.Trainers.AddRange(trainersInGyms);
            }

            return results;
        }

        /* public static List<string> TokenizeString(string input)
         {
             // Loại bỏ các ký tự đặc biệt và chia chuỗi thành các token
             string[] words = input.Split(new[] { ' ', ',', '.', ';', ':', '-', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
             return words.Select(word => word.ToLower()).Distinct().ToList();
         }

         public static double CalculateTokenMatchRatio(List<string> tokens1, List<string> tokens2)
         {
             // Đếm số lượng token chung
             int commonTokensCount = tokens1.Intersect(tokens2).Count();

             // Tính tỷ lệ khớp
             double matchRatio = (double)commonTokensCount / Math.Max(tokens1.Count, tokens2.Count);

             return matchRatio;
         }*/

        public static List<string> TokenizeString(string input)
        {
            // Loại bỏ các ký tự đặc biệt và chia chuỗi thành các token
            string[] words = input.Split(new[] { ' ', ',', '.', ';', ':', '-', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Chuyển tất cả các từ thành chữ thường và chia chúng thành các ký tự riêng lẻ
            var tokens = new List<string>();
            foreach (var word in words)
            {
                foreach (var ch in word.ToLower())
                {
                    tokens.Add(ch.ToString());
                }
            }
            return tokens.Distinct().ToList();
        }

        public static double CalculateTokenMatchRatio(List<string> tokens1, List<string> tokens2)
        {
            // Đếm số lượng token chung
            int commonTokensCount = tokens1.Intersect(tokens2).Count();

            // Tính tỷ lệ khớp
            double matchRatio = (double)commonTokensCount / Math.Max(tokens1.Count, tokens2.Count);

            return matchRatio;
        }

    }
}
