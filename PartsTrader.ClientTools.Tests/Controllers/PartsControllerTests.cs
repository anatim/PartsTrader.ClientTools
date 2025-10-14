using Microsoft.AspNetCore.Mvc;
using Moq;
using PartsTrader.ClientTools.Controllers;
using PartsTrader.ClientTools.Contracts;
using PartsTrader.ClientTools.Domain.ApplicationServices;
using Xunit;

namespace PartsTrader.ClientTools.Tests.Controllers;

public class PartsControllerTests
{
    private readonly Mock<IPartsApplicationService> _service = new();
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<PartsController>> _logger = new();

    private PartsController Create() => new(_service.Object, _logger.Object);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123-abcd")]     // too few digits
    [InlineData("12345-abcd")]   // too many digits
    [InlineData("1234-abc")]     // suffix too short
    [InlineData("1234-ab d")]    // space
    [InlineData("1234-ab_c")]    // underscore not allowed

    public void InvalidPartNumbers_ReturnBadRequest(string? input)
    {
        var controller = Create();

        var result = controller.ListCompatiblePartsByPartNumber(input ?? string.Empty);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Invalid part number", bad.Value!.ToString());
        _service.Verify(s => s.ListCompatiblePartsByPartNumber(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ExcludedPart_ReturnsEmpty()
    {
        var controller = Create();

        var result = controller.ListCompatiblePartsByPartNumber("1111-Invoice");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<PartsContract>>(ok.Value);
        Assert.Empty(list);
        _service.Verify(s => s.ListCompatiblePartsByPartNumber(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ValidNonExcludedPart_CallsServiceAndReturnsData()
    {
        var canonical = "2222-abcd";
        var returned = new[]
        {
            new PartsContract { Id = Guid.NewGuid(), PartId="2222", PartCode="abcd", PartNumber=canonical, Description="demo" }
        };
        _service.Setup(s => s.ListCompatiblePartsByPartNumber(canonical)).Returns(returned);

        var controller = Create();
        var result = controller.ListCompatiblePartsByPartNumber("2222-ABCD");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<PartsContract>>(ok.Value);
        Assert.Single(list);
        Assert.Equal(canonical, list.First().PartNumber);
        _service.Verify(s => s.ListCompatiblePartsByPartNumber(canonical), Times.Once);
    }

    [Fact]
    public void TrimmingAndLowercasing_AppliedBeforeServiceCall()
    {
        var canonical = "3333-zzzz";
        _service.Setup(s => s.ListCompatiblePartsByPartNumber(canonical)).Returns(Array.Empty<PartsContract>());

        var controller = Create();
        var result = controller.ListCompatiblePartsByPartNumber("  3333-ZZZZ  ");

        Assert.IsType<OkObjectResult>(result.Result);
        _service.Verify(s => s.ListCompatiblePartsByPartNumber(canonical), Times.Once);
    }
}
