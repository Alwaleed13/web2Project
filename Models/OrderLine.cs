namespace web2Project.Models
{
    public class OrderLine
    {
        public int Id { get; set; }
        public required string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
    }
}
