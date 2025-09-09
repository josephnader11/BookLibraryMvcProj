using Microsoft.AspNetCore.Mvc;
using BookLibraryMvcProj.Models;
using System.Text.Json;
using System.Text;

namespace BookLibraryMvc.Controllers
{
    public class AuthorMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorMvcController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("authors");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var authors = JsonSerializer.Deserialize<List<Author>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(authors);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"authors/{id}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var author = JsonSerializer.Deserialize<Author>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create() => View();

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid)
                return View(author);

            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(author);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("authors", content);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            return RedirectToAction(nameof(Index));
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"authors/{id}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var author = JsonSerializer.Deserialize<Author>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.AuthorId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(author);

            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(author);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"authors/{id}", content);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            return RedirectToAction(nameof(Index));
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"authors/{id}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var author = JsonSerializer.Deserialize<Author>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"authors/{id}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            return RedirectToAction(nameof(Index));
        }
    }
}
