using System.Text.Json.Serialization;

namespace BookLibraryMvcProj.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Year { get; set; }
        public int? AuthorId { get; set; }
        public int? BookCategoryId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Author? Author { get; set; }

        [JsonIgnore]
        public BookCategory? BookCategory { get; set; }
    }
}
