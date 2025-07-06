using Microsoft.AspNetCore.Mvc;
using toanggg.Services;

namespace toanggg.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }
        [HttpGet]
        public async Task<IActionResult> Search()
        {
       
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search2(string keyword)
        {   
            var results = await _searchService.SearchAsync(keyword);
            return View(results);
        }




      
/*
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var results = await _searchService.SearchAsync(keyword);
            return View(results); // Ensure this returns SearchResultsViewModel
        }*/
    }
}
