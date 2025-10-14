using System.Text.Json;

namespace PartsTrader.ClientTools.Data
{
    public class ExclusionsProvider
    {
        public static HashSet<string> GetExclusions(string filePath, ILogger logger)
        {

            if (!File.Exists(filePath))
            {
                logger.LogError("Exclusions file not found at {Path}. Using empty exclusions set.", filePath);
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            try
            {
                var json = File.ReadAllText(filePath);
                using var doc = JsonDocument.Parse(json);
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        if (element.TryGetProperty("PartNumber", out var pn) &&
                                    pn.ValueKind == JsonValueKind.String)
                        {
                            Add(pn.GetString(), set);
                        }
                    }
                }
                else
                {
                    logger.LogWarning("Exclusions file root element is not an array. Using empty exclusions set.");
                }

                return set;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to read exclusions. Using empty exclusions set.");
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }
        static void Add(string? value, HashSet<string> set)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            set.Add(value.Trim());
        }
    }
}