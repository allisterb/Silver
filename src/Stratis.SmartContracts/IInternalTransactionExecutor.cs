using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
  public interface IInternalTransactionExecutor
  {
        ITransferResult Transfer(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer);

   
        ITransferResult Call(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer, string methodName, object[]? parameters, ulong gasLimit);
         
    
        ICreateResult Create<T>(ISmartContractState smartContractState, ulong amountToTransfer, object[]? parameters, ulong gasLimit);                
    }
}
