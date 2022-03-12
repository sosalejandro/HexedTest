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

        /// <summary>
        /// Borrows a book. 0 for original. 1 for copy.
        /// </summary>
        /// <param name="command">Body object</param>
        /// <returns>No Content response</returns>
        /// <response code="200">Returns No Content</response>
        /// <response code="400">Returns Bad Request with the exception message</response>
        [HttpPost("borrowBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// <summary>
        /// Returns a book by given ISBNs and userId
        /// </summary>
        /// <param name="command">Body object</param>
        /// <returns>No Content response</returns>
        /// <response code="200">Returns No Content</response>
        /// <response code="400">Returns Bad Request with the exception message</response>
        [HttpPost("returnBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
