using HexedTest.Library.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext Context;
    public BookRepository(LibraryContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICollection<Book>> GetAvailableBooks(CancellationToken cancellation)
    {
        var availableBooks
            = await Context.Books.Where(
                b => b.Stock.OriginalAmount > 0 || b.Stock.CopiesAmount > 0)
            .ToListAsync(cancellation);

        return availableBooks;        
    }
}

