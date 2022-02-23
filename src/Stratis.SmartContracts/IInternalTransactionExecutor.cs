using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
  public interface IInternalTransactionExecutor
  {
        [Pure]
        ITransferResult Transfer(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer);

        [Pure]
        ITransferResult Call(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer, string methodName, object[]? parameters, ulong gasLimit);

        [Pure]
        ICreateResult Create<T>(ISmartContractState smartContractState, ulong amountToTransfer, object[]? parameters, ulong gasLimit);
  }
}
