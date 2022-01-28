// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.ISmartContractState
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

using System;

namespace Stratis.SmartContracts
{
  public interface ISmartContractState
  {
    IBlock Block { get; }

    IMessage Message { get; }

    IPersistentState PersistentState { get; }

    IContractLogger ContractLogger { get; }

    IInternalTransactionExecutor InternalTransactionExecutor { get; }

    IInternalHashHelper InternalHashHelper { get; }

    ISerializer Serializer { get; }

    Func<ulong> GetBalance { get; }
  }
}
