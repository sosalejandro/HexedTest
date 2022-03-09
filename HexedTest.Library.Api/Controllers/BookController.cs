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
            logger = logger;
        }
    }
}
