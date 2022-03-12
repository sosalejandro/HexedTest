using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace HexedTest.Library.Query.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/[controller]")]
public class LibraryQueryController : ControllerBase
{
    public IConfiguration configuration { get; }

    public LibraryQueryController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpGet("books/available")]
    public async Task<IActionResult> Get()
    {
        string sql = @"SELECT [ISBN], [Author], [Title], [YearPublished], [Stock_OriginalAmount], [Stock_CopiesAmount]
                        FROM [Library].[library].[books]
                        WHERE [Stock_OriginalAmount] > 0
                        OR [Stock_CopiesAmount] > 0";

        using var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        var ordelDetail = (await connection.QueryAsync(sql)).ToList();
        return Ok(ordelDetail);
    }
}

