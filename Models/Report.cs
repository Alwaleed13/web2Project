using Microsoft.EntityFrameworkCore;

namespace web2Project.Models
{
    [Keyless]
    public class Report
    {
        public string Name { get; set; }
        public int Total { get; set; }
    }
}
