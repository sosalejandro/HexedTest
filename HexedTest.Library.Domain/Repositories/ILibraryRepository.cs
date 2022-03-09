using HexedTest.Library.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Repositories;
public interface ILibraryRepository : IRepository<Entities.Library>
{
    Task AddLibrary(Entities.Library library, CancellationToken cancellation);
}

