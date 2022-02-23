
using Microsoft.Contracts;
using System;

namespace Stratis.SmartContracts
{
  public interface ISmartContractState
  {
        [Pure]
        IBlock Block { get; }

        [Pure]
        IMessage Message { get; }

        [Pure]
        IPersistentState PersistentState { get; }

        [Pure]
        IContractLogger ContractLogger { get; }

        [Pure]
        IInternalTransactionExecutor InternalTransactionExecutor { get; }

        [Pure]
        IInternalHashHelper InternalHashHelper { get; }

        [Pure]
        ISerializer Serializer { get; }

        [Pure]
        Func<ulong> GetBalance { get; }
  }
}
