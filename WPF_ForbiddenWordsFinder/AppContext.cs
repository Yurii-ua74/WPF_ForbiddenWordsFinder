using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WPF_ForbiddenWordsFinder
{
    class AppContext:DbContext
    {
        public DbSet<User> Users { get; set; } = null!;  // не буде дорівнювати нуль
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=itprogect.db");
        }

    }
}
