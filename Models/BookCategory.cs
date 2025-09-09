using System.Text.Json.Serialization;

namespace BookLibraryMvcProj.Models
{
    public class BookCategory
    {
        public int BookCategoryId { get; set; }
        public string? Name { get; set; }

        // Navigation property to Book

        [JsonIgnore]
        public List<Book>? Books { get; set; }
    }
}
