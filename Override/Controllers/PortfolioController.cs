using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace Override
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly PortfolioContext _context;

        public PortfolioController(PortfolioContext context)
        {
            _context = context;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestPortfolio([FromQuery] string portfolioCode, [FromQuery] string source)
        {
            if (string.IsNullOrWhiteSpace(portfolioCode) || string.IsNullOrWhiteSpace(source))
            {
                return BadRequest(new { Message = "PortfolioCode and Source are required" });
            }

            try
            {
                // Retrieve the latest record based on updated_datetime
                var latestPortfolio = await _context.PortfolioOverride
                    .Where(p => p.portfolio_code == portfolioCode && p.source == source)
                    .OrderByDescending(p => p.updated_datetime)  // Ensure you are ordering by the correct column
                    .FirstOrDefaultAsync();

                if (latestPortfolio == null)
                {
                    return NotFound(new { Message = "No records found for the provided PortfolioCode and Source" });
                }

                return Ok(latestPortfolio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred", Details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertPortfolio([FromBody] PortfolioRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                     .Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Validation failed", Errors = errorMessages });
            }

            try
            {
                // Create Portfolio entity
                var portfolio = new Portfolio
                {
                    source = request.Source,
                    portfolio_code= request.PortfolioCode,  // No need for .Value, it's a string
                    modified_user = request.User,
                    data = request.Data.GetRawText()  // Get raw JSON as a string
                };

                // Insert into database
                await _context.PortfolioOverride.AddAsync(portfolio);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Portfolio inserted successfully", PortfolioId = portfolio.portfolio_code });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { Message = "Database error", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred", Details = ex.Message });
            }
        }
    }
}
