using HexedTest.Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Repositories;

public interface IBookRepository
{
    // no books = empty library - use case 1
    // books in library = items in library - use case 2
    Task<ICollection<Book>> GetAvailableBooks(CancellationToken cancellation);
}

