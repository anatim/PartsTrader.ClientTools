using PartsTrader.ClientTools.Domain.Exceptions;
using System.Diagnostics;
using PartsContract = PartsTrader.ClientTools.Contracts.PartsContract;

namespace PartsTrader.ClientTools.Domain.ApplicationServices
{
    // Stub implementation: returns 3 synthetic variants (original, numeric-1, numeric+1)
    // To be replaced with real data source in Production
    public class PartsApplicationService : IPartsApplicationService
    {
        private readonly ILogger<PartsApplicationService> _logger;
        private static Guid NextId() => Guid.NewGuid();

        public PartsApplicationService(
            ILogger<PartsApplicationService> logger
            )
        {
            _logger = logger;
        }

        public IEnumerable<PartsContract> ListCompatiblePartsByPartNumber(string partNumber)
        {
            var parts = partNumber.Split('-', 2);
            var partId = parts[0];
            var partCode = parts[1];

            string originalId = partId;
            string variantAId = partId;
            string variantBId = partId;

            if (int.TryParse(partId, out var numeric))
            {
                if (numeric > int.MinValue) variantAId = (numeric - 1).ToString();
                if (numeric < int.MaxValue) variantBId = (numeric + 1).ToString();
            }

            PartsContract Create(string partid, string partcode, string description) => new()
                {
                    Id = NextId(),
                    PartId = partid,
                    PartCode = partcode,
                    PartNumber = $"{partid}-{partcode}",
                    Description = description
                };

            _logger.LogInformation("Listing compatible parts for part number: {PartNumber}", partNumber);
            return new List<PartsContract>{
                Create(originalId, partCode, "Original part"),
                Create(variantAId, partCode + "a",  "Part variant A"),
                Create(variantBId, partCode + "b",  "Part variant B")
            };
        }
    }
}
