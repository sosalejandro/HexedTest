namespace HexedTest.Library.Api.Commands;

public record ReturnBookCommand(List<string> ISBNs, Guid UserId);

