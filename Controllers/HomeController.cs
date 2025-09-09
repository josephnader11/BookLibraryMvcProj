using Microsoft.AspNetCore.Mvc;
using BookLibraryMvcProj.Models;
using System.Text.Json;

namespace BookLibraryMvcProj.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");

                // Get basic stats
                await GetBasicStats(client);

                return View();
            }
            catch
            {
                // Set defaults if API fails
                ViewBag.TotalBooks = 0;
                ViewBag.TotalAuthors = 0;
                ViewBag.RecentBooks = new List<Book>();
                ViewBag.PopularCategories = new List<BookCategory>();

                return View();
            }
        }

        private async Task GetBasicStats(HttpClient client)
        {
            // Get books
            try
            {
                var booksResponse = await client.GetAsync("books");
                if (booksResponse.IsSuccessStatusCode)
                {
                    var booksJson = await booksResponse.Content.ReadAsStringAsync();
                    var books = JsonSerializer.Deserialize<List<Book>>(booksJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ViewBag.TotalBooks = books?.Count ?? 0;
                    ViewBag.RecentBooks = books?.TakeLast(4).ToList() ?? new List<Book>();
                }
            }
            catch
            {
                ViewBag.TotalBooks = 0;
                ViewBag.RecentBooks = new List<Book>();
            }

            // Get authors count
            try
            {
                var authorsResponse = await client.GetAsync("authors");
                if (authorsResponse.IsSuccessStatusCode)
                {
                    var authorsJson = await authorsResponse.Content.ReadAsStringAsync();
                    var authors = JsonSerializer.Deserialize<List<Author>>(authorsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ViewBag.TotalAuthors = authors?.Count ?? 0;
                }
            }
            catch
            {
                ViewBag.TotalAuthors = 0;
            }

            // Get categories
            try
            {
                var categoriesResponse = await client.GetAsync("bookcategories");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    var categoriesJson = await categoriesResponse.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<BookCategory>>(categoriesJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ViewBag.PopularCategories = categories ?? new List<BookCategory>();
                }
            }
            catch
            {
                ViewBag.PopularCategories = new List<BookCategory>();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}