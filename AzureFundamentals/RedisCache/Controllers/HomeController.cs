using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisCache.Data;
using RedisCache.Models;
using System.Diagnostics;

namespace RedisCache.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IDistributedCache cache)
        {
            _logger = logger;
            _db = db;
            _cache = cache;
        }

        public IActionResult Index()
        {
            List<Category> categories = new ();
            var cachedCategoryList = _cache.GetString("categoryList");
            if (!string.IsNullOrEmpty(cachedCategoryList))
            {
                //cache
                categories = JsonConvert.DeserializeObject<List<Category>>(cachedCategoryList);
            }
            else
            {
                categories = _db.Category.ToList();
                DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
                _cache.SetString("categoryList", JsonConvert.SerializeObject(categories), cacheOptions);
            }
            return View(categories);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}