#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;

namespace MaKore.Data
{
    public class MaKoreContext : DbContext
    {
        public MaKoreContext (DbContextOptions<MaKoreContext> options)
            : base(options)
        {
        }

        public DbSet<MaKore.Models.User> User { get; set; }
    }
}
