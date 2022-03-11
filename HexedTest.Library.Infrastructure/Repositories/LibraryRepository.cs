using HexedTest.Library.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Infrastructure.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly LibraryContext Context;

        public LibraryRepository(LibraryContext context)
        {
            Context = context;
        }
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return Context;
            }
        }

        public async Task AddLibrary(Domain.Entities.Library library, CancellationToken cancellation = default)
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            await Context.Books.AddRangeAsync(library.Books, cancellation);
            await Context.BorrowOrders.AddRangeAsync(library.BorrowedBooks, cancellation);

            await UnitOfWork.SaveChangesAsync(cancellation);

            
        }
    }
}
