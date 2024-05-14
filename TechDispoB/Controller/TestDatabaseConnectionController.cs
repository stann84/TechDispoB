using Biblioteque.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Http;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace TechDispoWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDatabaseConnectionController : ControllerBase
    {
        //je me connecte directement a l'application web
        private readonly ApplicationDbContext _context;

        public TestDatabaseConnectionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                return Ok("Connection to the database is successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Unable to connect to the database: {ex.Message}");
            }
        }
    }
}
