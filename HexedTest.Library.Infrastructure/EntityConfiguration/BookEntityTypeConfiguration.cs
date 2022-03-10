using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Infrastructure.EntityConfiguration
{
    class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> bookConfiguration)
        {
            bookConfiguration.ToTable("books", LibraryContext.DEFAULT_SCHEMA);

            bookConfiguration.HasKey(b => b.ISBN);

            bookConfiguration.Property(b => b.Title)
                .IsRequired();

            bookConfiguration.Property(b => b.Author)
                .IsRequired();

            bookConfiguration.Property(b => b.YearPublished)
                .IsRequired();

            bookConfiguration
                .OwnsOne(b => b.Stock, s =>
                {
                    s.Property<string>("BookISBN");
                    s.WithOwner();
                });
        }
    }
}
