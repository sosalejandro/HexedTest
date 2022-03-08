using HexedTest.Library.Domain.Exceptions;
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
        Validate(isbn, author, title, yearPublished);
        return new Book(isbn, author, title, yearPublished, stock);
    }

    public void Validate()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(ISBN)) isValid = false;
        if (string.IsNullOrWhiteSpace(Author)) isValid = false;
        if (string.IsNullOrWhiteSpace(Title)) isValid = false;
        if (YearPublished == default) isValid = false;
        if (Stock is null) isValid = false;

        if (!isValid) throw new InvalidBookStateException("Invalid book state");
    }

    protected static void Validate(string isbn, string author, string title, DateTime yearPublished)
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(isbn)) isValid = false;
        if (string.IsNullOrWhiteSpace(author)) isValid = false;
        if (string.IsNullOrWhiteSpace(title)) isValid = false;
        if (yearPublished == default) isValid = false;

        if (!isValid) throw new InvalidBookStateException("Invalid book state");
    }

    public void SetNewStock(BookStock stock)
    {
        Stock = stock;
    }
}


