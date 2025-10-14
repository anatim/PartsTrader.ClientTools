using PartsTrader.ClientTools.Contracts;

namespace PartsTrader.ClientTools.Domain.ApplicationServices
{
    public interface IPartsApplicationService
    {

        ///<summary>
        /// A stub retrieving compatible parts for the given part number.
        /// </summary>
        IEnumerable<PartsContract> ListCompatiblePartsByPartNumber(string partNumber);
    }
}
