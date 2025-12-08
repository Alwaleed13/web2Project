using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using web2Project.Models;

namespace web2Project.Data
{
    public class web2ProjectContext : DbContext
    {
        public web2ProjectContext (DbContextOptions<web2ProjectContext> options)
            : base(options)
        {
        }

        public DbSet<web2Project.Models.UserAccount> UserAccount { get; set; } = default!;
        public DbSet<web2Project.Models.Item> Item { get; set; } = default!;
        public DbSet<web2Project.Models.Order> Order { get; set; } = default!;
    }
}
