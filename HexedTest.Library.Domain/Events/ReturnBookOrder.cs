namespace HexedTest.Library.Domain.Events;

public record ReturnBookOrder(List<string> ISBNs, Guid UserId) : IDomainEvent { }
