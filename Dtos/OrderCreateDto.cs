namespace BookStoreApi.Dtos
{
    public class OrderCreateDto
    {
        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
