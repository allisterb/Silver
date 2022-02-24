using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
  public interface IInternalTransactionExecutor
  {
        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        ITransferResult Transfer(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        ITransferResult Call(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer, string methodName, object[]? parameters, ulong gasLimit);
         
        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        ICreateResult Create<T>(ISmartContractState smartContractState, ulong amountToTransfer, object[]? parameters, ulong gasLimit);                
    }
}
