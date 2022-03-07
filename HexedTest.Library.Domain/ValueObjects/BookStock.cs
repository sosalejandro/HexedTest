using HexedTest.Library.Domain.Exceptions;

namespace HexedTest.Library.Domain.ValueObjects;

public record BookStock
{
    public int OriginalAmount { get; private set; }
    public int CopiesAmount { get; private set; }

    internal BookStock(int originalAmount, int copiesAmount)
    {
        OriginalAmount = originalAmount;
        CopiesAmount = copiesAmount;
    }

    public static BookStock Create(int originalAmount, int copiesAmount)
    {
        Validate(originalAmount, copiesAmount);
        return new BookStock(originalAmount, copiesAmount);
    }

    private static void Validate(int original, int copy)
    {
        if (original == -1 || copy == -1)
        {
            throw new OutOfStockException("This book is out of stock");
        }
    }
}