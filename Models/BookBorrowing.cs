using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookLibraryMvcProj.Models
{
    public class BookBorrowing
    {
        [Key]
        public int BookId { get; set; }

        [MaxLength(100)]
        public string? BorrowerName { get; set; }

        [MaxLength(100)]
        public string? BorrowerNumber { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        // Navigation property to Book
        [ForeignKey("BookId")]
        [JsonIgnore]
        public Book? Book { get; set; }
    }
}
