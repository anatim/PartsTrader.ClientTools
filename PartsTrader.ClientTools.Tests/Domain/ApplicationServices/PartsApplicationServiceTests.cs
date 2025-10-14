using Moq;
using PartsTrader.ClientTools.Domain.ApplicationServices;
using Xunit;

namespace PartsTrader.ClientTools.Tests.Domain.ApplicationServices;

public class PartsApplicationServiceTests
{
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<PartsApplicationService>> _logger = new();

    private PartsApplicationService Create() => new(_logger.Object);

    [Fact]
    public void NumericPartId_ReturnsThreeVariants_WithDecrementAndIncrementIds()
    {
        var svc = Create();

        var parts = svc.ListCompatiblePartsByPartNumber("5432-abcd").ToList();

        Assert.Equal(3, parts.Count);

        Assert.Equal("5432", parts[0].PartId);
        Assert.Equal("abcd", parts[0].PartCode);
        Assert.Equal("5432-abcd", parts[0].PartNumber);

        Assert.Equal("5431", parts[1].PartId);
        Assert.Equal("abcda", parts[1].PartCode);
        Assert.Equal("5431-abcda", parts[1].PartNumber);

        Assert.Equal("5433", parts[2].PartId);
        Assert.Equal("abcdb", parts[2].PartCode);
        Assert.Equal("5433-abcdb", parts[2].PartNumber);

        // Ensure uniqueness of random GUID IDs
        Assert.Equal(3, parts.Select(p => p.Id).Distinct().Count());
    }
}