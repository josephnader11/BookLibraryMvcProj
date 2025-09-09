using Microsoft.AspNetCore.Mvc;
using BookLibraryMvcProj.Models;
using System.Text.Json;
using System.Text;

namespace BookLibraryMvc.Controllers
{
    public class BookMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BookMvcController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync("books");

                response.EnsureSuccessStatusCode(); // Throws if status code != 2xx

                var json = await response.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(books);
            }
            catch
            {
                // Redirect to friendly error page
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync($"books/{id}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (book != null)
                {
                    // Fetch Author details if AuthorId exists
                    if (book.AuthorId.HasValue && book.Author == null)
                    {
                        try
                        {
                            var authorResponse = await client.GetAsync($"authors/{book.AuthorId.Value}");
                            if (authorResponse.IsSuccessStatusCode)
                            {
                                var authorJson = await authorResponse.Content.ReadAsStringAsync();
                                var author = JsonSerializer.Deserialize<Author>(authorJson, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                book.Author = author;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error fetching author: {ex.Message}");
                        }
                    }

                    // Fetch Category details if BookCategoryId exists
                    if (book.BookCategoryId > 0 && book.BookCategory == null)
                    {
                        try
                        {
                            var categoryResponse = await client.GetAsync($"bookcategories/{book.BookCategoryId}");
                            if (categoryResponse.IsSuccessStatusCode)
                            {
                                var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
                                var category = JsonSerializer.Deserialize<BookCategory>(categoryJson, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                book.BookCategory = category;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error fetching category: {ex.Message}");
                        }
                    }
                }

                return View(book);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Details: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            await LoadAuthors();
            await LoadCategories();
            return View(new Book());
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var client = _httpClientFactory.CreateClient("ApiClient");
                    var response = await client.PostAsJsonAsync("books", book);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Book created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error creating book: {errorContent}");
                    }
                }

                // Reload dropdown data if validation fails or API call fails
                await LoadAuthors();
                await LoadCategories();
                return View(book);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                await LoadAuthors();
                await LoadCategories();
                return View(book);
            }
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync($"books/{id}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Load dropdown data
                await LoadAuthors();
                await LoadCategories();

                return View(book);
            }
            catch
            {
                await LoadAuthors();
                await LoadCategories();
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
                return BadRequest();

            try
            {
                if (ModelState.IsValid)
                {
                    var client = _httpClientFactory.CreateClient("ApiClient");
                    var response = await client.PutAsJsonAsync($"books/{id}", book);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Book updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error updating book: {errorContent}");
                    }
                }

                // Reload dropdown data if validation fails or API call fails
                await LoadAuthors();
                await LoadCategories();
                return View(book);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                await LoadAuthors();
                await LoadCategories();
                return View(book);
            }
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync($"books/{id}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(book);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.DeleteAsync($"books/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting book.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Helper method to load authors for dropdown
        private async Task LoadAuthors()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync("authors"); // Adjust endpoint as needed

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var authors = JsonSerializer.Deserialize<List<Author>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ViewBag.Authors = authors ?? new List<Author>();
                }
                else
                {
                    ViewBag.Authors = new List<Author>();
                }
            }
            catch
            {
                ViewBag.Authors = new List<Author>();
            }
        }

        // Helper method to load categories for dropdown
        private async Task LoadCategories()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.GetAsync("bookcategories"); // Adjust endpoint as needed

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<BookCategory>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ViewBag.Categories = categories ?? new List<BookCategory>();
                }
                else
                {
                    ViewBag.Categories = new List<BookCategory>();
                }
            }
            catch
            {
                ViewBag.Categories = new List<BookCategory>();
            }
        }
    }
}