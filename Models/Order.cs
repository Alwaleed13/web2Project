using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace web2Project.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        public int Total { get; set; }
    }
}
