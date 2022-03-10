using HexedTest.Library.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Infrastructure.Persistance
{
    public static class BookStore
    {
        public static HashSet<Book> GetBooksFromStore()
        {
            return new HashSet<Book>()
            {
                Book.Create(
                    "9780439139595",
                    "J.K. Rowling",
                    "Harry Potter and the Goblet of Fire",
                    new DateTime(2005, 11, 18),
                    BookStock.Create(20, 50)),
                Book.Create(
                    "9780007237500",
                    "George R. R. Martin",
                    "A Game of Thrones",
                    new DateTime(1996, 8, 1),
                    BookStock.Create(15, 0)),
                Book.Create(
                    "9781491903063",
                    "Martin Kleppmann",
                    "Designing Data-Intensive Applications",
                    new DateTime(2017, 3, 1),
                    BookStock.Create(0, 2)),
                Book.Create(
                    "9789584294586",
                    "Jaime Lopera",
                    "La Culpa Es de la Vaca - Remasterizado",
                    new DateTime(2021, 9, 21),
                    BookStock.Create(5, 10))
            };
        }
    }
}
