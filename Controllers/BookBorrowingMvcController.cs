using Microsoft.AspNetCore.Mvc;
using BookLibraryMvcProj.Models;
using System.Text.Json;
using System.Text;

namespace BookLibraryMvcProj.Controllers
{
    public class BookBorrowingMvcController : Controller
    {
        private readonly HttpClient _httpClient;

        public BookBorrowingMvcController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // GET: BookBorrowingMvc
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("BookBorrowing");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var borrowings = JsonSerializer.Deserialize<List<BookBorrowing>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(borrowings ?? new List<BookBorrowing>());
                }

                ViewBag.ErrorMessage = "Unable to load borrowing records.";
                return View(new List<BookBorrowing>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View(new List<BookBorrowing>());
            }
        }

        // GET: BookBorrowingMvc/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"BookBorrowing/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var borrowing = JsonSerializer.Deserialize<BookBorrowing>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(borrowing);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View();
            }
        }

        // GET: BookBorrowingMvc/Create
        public async Task<IActionResult> Create()
        {
            await LoadBooks(); // Load books for dropdown
            return View(new BookBorrowing { BorrowDate = DateTime.Now });
        }

        // POST: BookBorrowingMvc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookBorrowing bookBorrowing)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Set default values if needed
                    if (bookBorrowing.BorrowDate == default)
                    {
                        bookBorrowing.BorrowDate = DateTime.Now;
                    }

                    var json = JsonSerializer.Serialize(bookBorrowing, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("BookBorrowing", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = $"Book borrowing record created successfully! Status: {bookBorrowing.Status}";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"Error creating record: {errorContent}";
                        return RedirectToAction(nameof(Index));
                    }
                }

                // If we get here, model validation failed
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = $"Validation failed: {errors}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: BookBorrowingMvc/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"BookBorrowing/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var borrowing = JsonSerializer.Deserialize<BookBorrowing>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Make sure this is being called
                    await LoadBooks();

                    // Debug: Check if books are loaded
                    var books = ViewBag.Books as List<Book>;
                    System.Diagnostics.Debug.WriteLine($"Books loaded: {books?.Count ?? 0}");

                    return View(borrowing);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                // Still try to load books even if there's an error
                await LoadBooks();
                return View();
            }
        }

        // POST: BookBorrowingMvc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookBorrowing bookBorrowing)
        {
            try
            {
                if (id != bookBorrowing.BookId)
                {
                    return BadRequest();
                }

                if (ModelState.IsValid)
                {
                    var json = JsonSerializer.Serialize(bookBorrowing);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"BookBorrowing/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Book borrowing record updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error updating record: {errorContent}");
                    }
                }

                await LoadBooks();
                return View(bookBorrowing);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                await LoadBooks();
                return View(bookBorrowing);
            }
        }

        // GET: BookBorrowingMvc/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"BookBorrowing/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var borrowing = JsonSerializer.Deserialize<BookBorrowing>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return View(borrowing);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View();
            }
        }


        // POST: BookBorrowingMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"BookBorrowing/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book borrowing record deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting record.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: BookBorrowingMvc/ReturnBook/5
        [HttpPost]
        public async Task<IActionResult> ReturnBook(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"BookBorrowing/{id}/return", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book returned successfully!";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Error returning book: {errorContent}";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Helper method to load books for dropdown
        private async Task LoadBooks()
        {
            try
            {
                var response = await _httpClient.GetAsync("Book"); // Calls your Books API

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var books = JsonSerializer.Deserialize<List<Book>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    ViewBag.Books = books ?? new List<Book>();
                }
                else
                {
                    ViewBag.Books = new List<Book>();
                }
            }
            catch
            {
                ViewBag.Books = new List<Book>();
            }
        }
        // Helper method to check if book is already borrowed
        private async Task<bool> IsBookBorrowed(int bookId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"BookBorrowing/{bookId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var borrowing = JsonSerializer.Deserialize<BookBorrowing>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return borrowing?.Status == "Borrowed";
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Action to get book borrowing status (for AJAX calls)
        [HttpGet]
        public async Task<IActionResult> GetBookStatus(int bookId)
        {
            try
            {
                var isBorrowed = await IsBookBorrowed(bookId);
                return Json(new { isBorrowed = isBorrowed });
            }
            catch
            {
                return Json(new { isBorrowed = false });
            }
        }


        // Helper method to load authors for dropdown
        private async Task LoadAuthors()
        {
            try
            {
                var response = await _httpClient.GetAsync("Author"); // Calls your Authors API

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var authors = JsonSerializer.Deserialize<List<Author>>(jsonString, new JsonSerializerOptions
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

    }
}