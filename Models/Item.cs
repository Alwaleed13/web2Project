namespace web2Project.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool Discount { get; set; }
        public int Category { get; set; }
        public int Quantity { get; set; }
        public string? ImgFile { get; set; }
    }
}
