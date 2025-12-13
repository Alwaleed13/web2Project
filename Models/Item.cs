using System.ComponentModel.DataAnnotations;
namespace web2Project.Models
{
    public class Item
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public int? Price { get; set; }
        public string? Discount { get; set; }
        public string? Category { get; set; }
        [Required]
        public int? Quantity { get; set; }
        public string? ImgFile { get; set; }
    }
}
