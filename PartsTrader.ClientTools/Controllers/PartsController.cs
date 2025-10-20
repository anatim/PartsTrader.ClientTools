using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsTrader.ClientTools.Contracts;
using PartsTrader.ClientTools.Data;
using PartsTrader.ClientTools.Domain.ApplicationServices;
using PartsTrader.ClientTools.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace PartsTrader.ClientTools.Controllers
{
    [Route("parts")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartsApplicationService _partsApplicationService;
        private readonly ILogger<PartsController> _logger;
        private static readonly Regex PartPattern = new(@"^[0-9]{4}-[A-Za-z0-9]{4,}$", RegexOptions.Compiled);

        public PartsController(
            IPartsApplicationService partsApplicationService,
            ILogger<PartsController> logger
            )
        {
            _partsApplicationService = partsApplicationService;
            _logger = logger;
        }

        /// A stub retrieving compatible parts for the given part number
        /// Returns empty array if excluded
        [HttpGet("compatible/{partNumber}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PartsContract>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<PartsContract>> ListCompatiblePartsByPartNumber(string partNumber)
        {
            var trimmed = partNumber.Trim();

            if (string.IsNullOrWhiteSpace(trimmed) || !PartPattern.IsMatch(trimmed))
            {
                _logger.LogWarning("Invalid part number: {PartNumber}", partNumber);
                return BadRequest(new
                {
                    error = $"Invalid part number: '{partNumber}'. Expected ####-XXXX (4 digits, dash, 4+ alphanumerics)."
                });
        }

            var canonical = trimmed.ToLowerInvariant();

            var exclusionsPath = Path.Combine(AppContext.BaseDirectory, "Data", "Exclusions.json");
            var exclusions = ExclusionsProvider.GetExclusions(exclusionsPath, _logger);
            if (exclusions.Contains(canonical))
            {
                _logger.LogInformation("Part {Part} is excluded.", canonical);
                return Ok(Array.Empty<PartsContract>());
            }

            var results = _partsApplicationService.ListCompatiblePartsByPartNumber(canonical);
            return Ok(results);

        }
    }
}
