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
        [Captured]
        public SilverSmartContractState() { }
        #endregion

        #region Properties
        public Block Block { [Pure] get => this._Block; }

        public Message Message 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get => this._Message; 
        }

        public SilverSmartContractPersistentState PersistentState 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get => this._PersistentState; 
        }

        public abstract IContractLogger ContractLogger 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get; 
        }

        public abstract IInternalTransactionExecutor InternalTransactionExecutor 
        { 
            [Pure]
            get; 
        }
        
        public abstract IInternalHashHelper InternalHashHelper 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get; 
        }

        public abstract ISerializer Serializer 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get; 
        }

        public abstract Func<ulong> GetBalance 
        { 
            [Pure]
            get; 
        }
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
