
using HexedTest.Library.Infrastructure.EntityConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Infrastructure
{
    public class LibraryContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "library";
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowOrder> BorrowOrders { get; set; }

        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BorrowOrderEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            return result == 1;
        }
    }
}
