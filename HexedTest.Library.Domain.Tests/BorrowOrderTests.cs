using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Tests;

public class BorrowOrderTests
{
    [Fact]
    public void Create_Should_CreateAValidBorrowOrder()
    {
        // Arrange
        BorrowOrder order;
        Guid userId = Guid.NewGuid();
        string bookISBN = "978-1-0217-2611-7";

        // Act
        order = BorrowOrder.Create(userId, bookISBN);

        // Assert
        Assert.NotNull(order);        
        Assert.Equal(userId, order.UserId);
        Assert.Equal(bookISBN, order.BookISBN);
        Assert.False(order.IsCopy, "Order is copy");
        Assert.False(order.IsReturned, "Order is returned");
        Assert.NotEqual(default, order.DateRequested);
        Assert.Null(order.DateReturned);
    }

    [Fact]
    public void SetCopy_Should_SetIsCopy_True()
    {
        // Arrange
        BorrowOrder order;
        Guid userId = Guid.NewGuid();
        string bookISBN = "978-1-0217-2611-7";
        order = BorrowOrder.Create(userId, bookISBN);

        // Act
        order.SetCopy();

        // Assert
        Assert.True(order.IsCopy);
        Assert.False(order.IsReturned);
    }    
}

