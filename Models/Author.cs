namespace BookLibraryMvcProj.Models
{
    public class Author
    {
        public int AuthorId { get; set; }   // Primary Key
        public string? Name { get; set; }    // Author Name

        // Navigation property (one author can have many books)
        public List<Book>? Books { get; set; }
    }
}
