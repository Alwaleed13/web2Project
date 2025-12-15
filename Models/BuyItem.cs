namespace web2Project.Models
{
    public class BuyItem
    {
        public int Id { get; set; }
        public string Name { get; set; }      
        public decimal Price { get; set; }     
        public int Quantity { get; set; }       
        public string ImgFile { get; set; }     
        public string Description { get; set; } 
        public string Discount { get; set; }
    }

}
