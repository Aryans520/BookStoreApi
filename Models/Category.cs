namespace BookStoreApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // رابطه‌ی چند به چند با کتاب‌ها
        public ICollection<BookCategory> BookCategories { get; set; }
    }
}
