// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.IInternalTransactionExecutor
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

namespace Stratis.SmartContracts
{
  public interface IInternalTransactionExecutor
  {
    ITransferResult Transfer(
      ISmartContractState smartContractState,
      Address addressTo,
      ulong amountToTransfer);

    ITransferResult Call(
      ISmartContractState smartContractState,
      Address addressTo,
      ulong amountToTransfer,
      string methodName,
      object[] parameters,
      ulong gasLimit = 0);

    ICreateResult Create<T>(
      ISmartContractState smartContractState,
      ulong amountToTransfer,
      object[] parameters,
      ulong gasLimit = 0);
  }
}
