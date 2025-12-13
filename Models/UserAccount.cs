using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace web2Project.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime? Pubdate { get; set; }
    }
}
