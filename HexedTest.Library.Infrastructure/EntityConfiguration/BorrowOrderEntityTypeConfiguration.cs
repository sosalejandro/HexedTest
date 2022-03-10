using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Infrastructure.EntityConfiguration
{  
    class BorrowOrderEntityTypeConfiguration : IEntityTypeConfiguration<BorrowOrder>
    {
        public void Configure(EntityTypeBuilder<BorrowOrder> borrowOrderConfiguration)
        {
            borrowOrderConfiguration.ToTable("borrowOrders", LibraryContext.DEFAULT_SCHEMA);

            borrowOrderConfiguration.HasKey(bo => bo.Id);

            borrowOrderConfiguration.Property(bo => bo.Id)
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired();

            borrowOrderConfiguration.HasOne(typeof(Book))
                .WithMany()
                .HasForeignKey("BookISBN");

            borrowOrderConfiguration.Property(bo => bo.UserId)
                .IsRequired();

            borrowOrderConfiguration.Property(bo => bo.IsCopy)
                .IsRequired()
                .HasDefaultValue(false);

            borrowOrderConfiguration.Property(bo => bo.IsReturned)
                .IsRequired()
                .HasDefaultValue(false);

            borrowOrderConfiguration.Property(bo => bo.DateRequested)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            borrowOrderConfiguration.Property(bo => bo.DateReturned)
                .HasDefaultValue(null);
        }
    }
}
