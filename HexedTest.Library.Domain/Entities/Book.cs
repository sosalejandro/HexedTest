using HexedTest.Library.Domain.ValueObjects;

namespace HexedTest.Library.Domain.Entities;

public class Book
{
    public string ISBN { get; init; }
    public string Author { get; init; }
    public string Title { get; init; }
    public DateTime YearPublished { get; init; }
    public BookStock Stock { get; private set; }

    internal Book(string isbn, string author, string title, DateTime yearPublished, BookStock stock)
    {
        ISBN = isbn;
        Author = author;
        Title = title;
        YearPublished = yearPublished;
        Stock = stock;
    }

    public static Book Create(string isbn, string author, string title, DateTime yearPublished, BookStock stock)
    {
        return new Book(isbn, author, title, yearPublished, stock);
    }
}

