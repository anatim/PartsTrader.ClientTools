namespace PartsTrader.ClientTools.Domain.Exceptions
{
    public class InvalidPartNumberException : Exception
    {
        public InvalidPartNumberException(string partNumber)
            : base($"Invalid part number: '{partNumber}'. Expected ####-XXXX (4 digits, dash, 4+ alphanumerics).")
        {
        }
    }
}