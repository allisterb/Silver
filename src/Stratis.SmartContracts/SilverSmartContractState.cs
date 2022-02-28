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
        #region Properties
        [Pure]
        public Block Block { get => this._Block; }

        [Pure]
        public Message Message { get => this._Message; }

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
        [Rep]
        private readonly Block _Block = new Block();
        
        [Rep]
        private readonly Message _Message = new Message(SilverVM.RandomAddress, SilverVM.RandomAddress, SilverVM.RandomUInt);
        
        [Rep]
        private readonly SilverSmartContractPersistentState _PersistentState = new SilverSmartContractPersistentState();
        #endregion
    }
}
