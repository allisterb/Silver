using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
  public interface IContractLogger
  {
        [Pure]
        void Log<T>(ISmartContractState smartContractState, T toLog) where T : struct;
  }
}
