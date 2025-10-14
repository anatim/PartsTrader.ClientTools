using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using PartsTrader.ClientTools.Data;
using Xunit;

namespace PartsTrader.ClientTools.Tests.Data;

public class ExclusionsProviderTests
{
    private readonly Mock<ILogger> _logger = new();

    private string CreateTempFile(string? content = null)
    {
        var path = Path.Combine(Path.GetTempPath(), $"excl_{Guid.NewGuid():N}.json");
        if (content != null)
        {
            File.WriteAllText(path, content, Encoding.UTF8);
        }
        return path;
    }

    [Fact]
    public void MissingFile_ReturnsEmpty()
    {
        var missingPath = CreateTempFile();
        if (File.Exists(missingPath)) File.Delete(missingPath);
        var result = ExclusionsProvider.GetExclusions(missingPath, _logger.Object);

        Assert.Empty(result);
    }

    [Fact]
    public void InvalidJson_ReturnsEmpty()
    {
        var path = CreateTempFile("[ { \"PartNumber\":  }"); // invalid JSON
        var result = ExclusionsProvider.GetExclusions(path, _logger.Object);

        Assert.Empty(result);
    }

    [Fact]
    public void ValidJson_CaseInsensitiveLookup()
    {
        var path = CreateTempFile("""
        [
          { "PartNumber": "1234-ABCD" },
          { "PartNumber": "9999-charge" }
        ]
        """);

        var set = ExclusionsProvider.GetExclusions(path, _logger.Object);

        Assert.Contains("1234-abcd", set);
        Assert.Contains("9999-CHARGE", set);
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void FileChange_NewEntryAppearsOnNextCall()
    {
        // Because caching was removed, each call should re-read the file.
        var path = CreateTempFile("""
        [
          { "PartNumber": "1111-test" }
        ]
        """);

        var first = ExclusionsProvider.GetExclusions(path, _logger.Object);
        Assert.Single(first);
        Assert.Contains("1111-test", first, StringComparer.OrdinalIgnoreCase);

        // Modify file: add a second part
        File.WriteAllText(path, """
        [
          { "PartNumber": "1111-test" },
          { "PartNumber": "2222-added" }
        ]
        """);

        var second = ExclusionsProvider.GetExclusions(path, _logger.Object);
        Assert.Equal(2, second.Count);
        Assert.Contains("2222-added", second, StringComparer.OrdinalIgnoreCase);
    }
}