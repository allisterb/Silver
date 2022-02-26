using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
    public abstract class SilverSmartContractState
    {
        #region Constructors

        #endregion

        #region Properties
        [Pure]
        public IBlock Block { get => this._Block; }

        [Pure]
        public IMessage Message { get => this._Message; }

        [Pure]
        public SilverSmartContractPersistentState PersistentState { get => this._PersistentState; }

        [Pure]
        public abstract IContractLogger ContractLogger { get; }

        [Pure]
        public abstract IInternalTransactionExecutor InternalTransactionExecutor { get; }

        [Pure]
        public abstract IInternalHashHelper InternalHashHelper { get; }

        [Pure]
        public abstract ISerializer Serializer { get; }

        [Pure]
        public abstract Func<ulong> GetBalance { get; }
        #endregion

        #region Fields
        private Block _Block = new Block();
        private Message _Message = new Message(Address.Zero, Address.Zero, 0);
        private SilverSmartContractPersistentState _PersistentState = new SilverSmartContractPersistentState();
        #endregion
    }
}
