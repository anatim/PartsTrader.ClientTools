namespace PartsTrader.ClientTools.Contracts
{
    public class PartsContract
    {
        /// <summary>
        /// Contract for basic operations on Parts resources.
        /// </summary>
        public Guid Id { get; set; }

        public required string PartId { get; set; }

        public required string PartCode { get; set; }

        public required string PartNumber { get; set; }

        public string? Description { get; set; }

    }
}
