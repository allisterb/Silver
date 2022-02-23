using System;
using System.Globalization;

using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
  public abstract class SmartContract
  {
        #region Constructors
        public SmartContract(ISmartContractState contractState)
        {
            this.contractState = contractState;
            this.Address = contractState.Message.ContractAddress;
            this.Balance = contractState.GetBalance();
            this.Block = contractState.Block;
            this.Message = contractState.Message;
            this.State = contractState.PersistentState;
            this.PersistentState = contractState.PersistentState;
            this.Serializer = contractState.Serializer;  
        }
        #endregion

        #region Methods
        [Pure]
        protected ITransferResult Transfer(Address addressTo, ulong amountToTransfer) => this.contractState.InternalTransactionExecutor.Transfer(this.contractState, addressTo, amountToTransfer);

        [Pure]
        protected ITransferResult Call(
          Address addressTo,
          ulong amountToTransfer,
          string methodName,
          object[]? parameters,
          ulong gasLimit)
        {
                return this.contractState.InternalTransactionExecutor.Call(this.contractState, addressTo, amountToTransfer, methodName, parameters, gasLimit);
        }

        [Pure]
        protected ITransferResult Call(
            Address addressTo,
            ulong amountToTransfer,
            string methodName,
            object[]? parameters)
        {
            
                return Call(addressTo, amountToTransfer, methodName, parameters, 0);
        }

        [Pure]
        protected ITransferResult Call(
            Address addressTo,
            ulong amountToTransfer,
            string methodName)
            {
                
                 return Call(addressTo, amountToTransfer, methodName, null, 0);
                
            }

        [Pure]
        protected ICreateResult Create<T>(
          ulong amountToTransfer,
          object[] parameters,
          ulong gasLimit)
          where T : SmartContract
            {
                return this.contractState.InternalTransactionExecutor.Create<T>(this.contractState, amountToTransfer, parameters, gasLimit);
            }

        [Pure]
        protected byte[] Keccak256(byte[] toHash) => this.contractState.InternalHashHelper.Keccak256(toHash);

        [Pure]
        protected void Assert(bool condition, string message)
        {
          if (!condition)
            throw new SmartContractAssertException(message);
        }

        [Pure]
        protected void Assert(bool condition)
        {
            Assert(condition, "Assert failed.");
        }

        [Pure]
        protected void Log<T>(T toLog) where T : struct => this.contractState.ContractLogger.Log<T>(this.contractState, toLog);

        [Pure]
        public virtual void Receive()
        {
        }
        #endregion

        #region Fields
        [Rep]
        public readonly ISmartContractState contractState;

        [Rep]
        public readonly IPersistentState State;

        [Rep]
        public readonly Address Address; // = this.State.Message.ContractAddress;

        [Rep]
        public readonly ulong Balance; // => this.contractState.GetBalance();

        [Rep]
        public readonly IBlock Block; // => this.contractState.Block;

        [Rep]
        public readonly IMessage Message; // => this.contractState.Message;

        [Obsolete("Please use State property as shorthand", false)]
        [Rep]
        public IPersistentState PersistentState; //StateIndependentAttribute;

        [Rep]
        public ISerializer Serializer; //=> this.State.Serializer;

        #endregion
    }
}
