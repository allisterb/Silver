// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.SmartContract
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

using System;
using System.Globalization;

namespace Stratis.SmartContracts
{
  public abstract class SmartContract
  {
    private readonly ISmartContractState contractState;

    protected Address Address => this.contractState.Message.ContractAddress;

    public ulong Balance => this.contractState.GetBalance();

    public IBlock Block => this.contractState.Block;

    public IMessage Message => this.contractState.Message;

    [Obsolete("Please use State property as shorthand", false)]
    public IPersistentState PersistentState => this.contractState.PersistentState;

    public IPersistentState State => this.contractState.PersistentState;

    public ISerializer Serializer => this.contractState.Serializer;

    public SmartContract(ISmartContractState contractState)
    {
      CultureInfo.CurrentCulture = new CultureInfo("en-US");
      this.contractState = contractState;
    }

    protected ITransferResult Transfer(Address addressTo, ulong amountToTransfer) => this.contractState.InternalTransactionExecutor.Transfer(this.contractState, addressTo, amountToTransfer);

    protected ITransferResult Call(
      Address addressTo,
      ulong amountToTransfer,
      string methodName,
      object[] parameters = null,
      ulong gasLimit = 0)
    {
      return this.contractState.InternalTransactionExecutor.Call(this.contractState, addressTo, amountToTransfer, methodName, parameters, gasLimit);
    }

    protected ICreateResult Create<T>(
      ulong amountToTransfer = 0,
      object[] parameters = null,
      ulong gasLimit = 0)
      where T : SmartContract
    {
      return this.contractState.InternalTransactionExecutor.Create<T>(this.contractState, amountToTransfer, parameters, gasLimit);
    }

    protected byte[] Keccak256(byte[] toHash) => this.contractState.InternalHashHelper.Keccak256(toHash);

    protected void Assert(bool condition, string message = "Assert failed.")
    {
      if (!condition)
        throw new SmartContractAssertException(message);
    }

    protected void Log<T>(T toLog) where T : struct => this.contractState.ContractLogger.Log<T>(this.contractState, toLog);

    public virtual void Receive()
    {
    }
  }
}
