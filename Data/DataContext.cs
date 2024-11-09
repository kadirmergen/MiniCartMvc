using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.EntityFrameworkCore;
using MiniCartMvc.Models;

namespace MiniCartMvc.Data
{
    public class DataContext :DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {
                
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
