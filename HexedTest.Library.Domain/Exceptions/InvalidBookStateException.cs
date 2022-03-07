using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedTest.Library.Domain.Exceptions;

public class InvalidBookStateException : Exception
{
    public InvalidBookStateException(string message) : base(message)
    {

    }
}

