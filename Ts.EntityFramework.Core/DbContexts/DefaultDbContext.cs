using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;

namespace Ts.EntityFramework.Core
{
    [AppDbContext("Sqlite3ConnectionString", DbProvider.Sqlite)]
    public class DefaultDbContext : AppDbContext<DefaultDbContext>
    {
        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        {
        }
        
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseLazyLoadingProxies()
        //         .UseSqlite(DbProvider.GetConnectionString<DefaultDbContext>());
        //     base.OnConfiguring(optionsBuilder);
        // }
    }
}