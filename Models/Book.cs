namespace BookStoreApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        // رابطه‌ی چند به چند با دسته‌بندی‌ها
        public ICollection<BookCategory> BookCategories { get; set; }
    }
}
