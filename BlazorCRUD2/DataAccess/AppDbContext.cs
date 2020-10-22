using BlazorCRUD2.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorCRUD2.DataAccess
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Article> ArticleList { get; set; }
    }
}
