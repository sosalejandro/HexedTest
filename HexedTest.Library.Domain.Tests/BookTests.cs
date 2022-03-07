using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Tests;

public class BookTests
{
    [Fact]
    public void Create_Should_initializeAValidObject_WhenMethodIsCalled()
    {
        // Arrange
        Book book;

        // Act
        book = CreateBaseBook();

        // Assert
        Assert.NotNull(book);

        Assert.False(string.IsNullOrWhiteSpace(book.ISBN));
        Assert.False(string.IsNullOrWhiteSpace(book.Author));
        Assert.False(string.IsNullOrWhiteSpace(book.Title));
        Assert.NotNull(book.Stock);
    }

    private static Book CreateBaseBook(string isbn = "978-1-0217-2611-7", int original = 15, int copies = 30)
    {

        string author = "Gaspar Noe";
        string title = "Black Swan";
        DateTime yearPubllished = DateTime.Now.AddYears(Random.Shared.Next(-60, -20));
        BookStock stock = BookStock.Create(original, copies);

        return Book.Create(isbn, author, title, yearPubllished, stock);
    }
}

