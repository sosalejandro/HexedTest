using HexedTest.Library.Api.Commands;
using HexedTest.Library.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HexedTest.Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly LibraryApplicationService applicationService;
        private readonly ILogger<BookController> logger;

        public BookController(LibraryApplicationService appService,
            ILogger<BookController> logger)
        {
            applicationService = appService;
            this.logger = logger;
        }

        [HttpPost("borrowBook")]
        public async Task<IActionResult> Post(BorrowBookCommand command)
        {
            try
            {
                await applicationService.HandleAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("returnBook")]
        public async Task<IActionResult> Post(ReturnBookCommand command)
        {
            try
            {
                await applicationService.HandleAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
