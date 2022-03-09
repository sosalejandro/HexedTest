namespace HexedTest.Library.Domain.Tests;

[Collection("Library")]
public class BookStockTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(5, 0)]
    [InlineData(0, 0)]
    public void Create_Should_CreateAValidBookStock_AfterMethodIsCalledWithValidInputs
        (int originals, int copies)
    {
        // Arrange
        BookStock stock;
        Func<BookStock> TestMethodCreate = () => BookStock.Create(originals, copies);

        // Act
        stock = TestMethodCreate();

        // Assert
        Assert.NotNull(stock);
        Assert.True(stock.OriginalAmount == originals, "The originals input doesn't match.");
        Assert.True(stock.CopiesAmount == copies, "The copies input doesn't match.");
    }

    [Theory]
    [InlineData(1, -1)]
    [InlineData(-1, 1)]
    [InlineData(-1, -1)]
    [InlineData(0, -1)]
    [InlineData(-1, 0)]
    public void Create_Should_Throw_WhenStockHasRunOutOfItems
        (int originals, int copies)
    { 
        // Arrange
        Func<BookStock> TestMethodCreate = () => BookStock.Create(originals, copies);

        // Act & Assert
        Assert.Throws<OutOfStockException>(() => TestMethodCreate());
    }
}

