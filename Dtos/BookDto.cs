namespace BookStoreApi.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public List<CategoryDto> Categories { get; set; } // فقط اطلاعات ضروری
    }
}
