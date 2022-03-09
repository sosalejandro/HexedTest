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

        public Task AddLibrary(Domain.Entities.Library library, CancellationToken cancellation = default)
        {
            Context.Database.ExecuteSqlRaw("TRUNCATE TABLE [books]");
            Context.Database.ExecuteSqlRaw("TRUNCATE TABLE [borrowOrders]");

            Context.AddRange(library.Books, library.BorrowedBooks);

            UnitOfWork.SaveChangesAsync(cancellation);

            return Task.CompletedTask;
        }
    }
}
