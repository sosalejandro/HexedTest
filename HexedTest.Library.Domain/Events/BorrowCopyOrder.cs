using HexedTest.Library.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Events;

public record BorrowCopyOrder(string ISBN, Guid UserId) : IDomainEvent { }