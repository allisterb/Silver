using System;
using System.Collections.Generic;

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
            Dictionary<Address, long> balances = new Dictionary<Address, long>();
            
            balances.Add(contractState.Message.ContractAddress, (long)contractState.GetBalance());
            balances.Add(contractState.Message.Sender, 0L - (long) contractState.Message.Value);
            this.Balances = balances;
        }
        #endregion

        #region Methods

        protected ITransferResult Transfer(Address addressTo, ulong amountToTransfer)
        //@ modifies this.Balances;
        
        //@ ensures this.Balances[addressTo] != old(this.Balances[addressTo]) + 10;
        {
            
            ITransferResult result = this.contractState.InternalTransactionExecutor.Transfer(this.contractState, addressTo, 10);
            
            this.Balances[addressTo] = this.Balances[addressTo] + 10;
            
            
            

            /*
            this.Balances[addressTo] = this.Balances[addressTo] + (long)amountToTransfer;
            
            if (result.Success)
            {
                if (this.Balances.ContainsKey(addressTo))
                {
                    this.Balances[addressTo] = this.Balances[addressTo] + (long) amountToTransfer;
                }
                else
                {
                    this.Balances.Add(addressTo, (long) amountToTransfer);
                }
            
            }*/
            return result;
        }
        protected ITransferResult Call(
          Address addressTo,
          ulong amountToTransfer,
          string methodName,
          object[]? parameters,
          ulong gasLimit)
        {
            ITransferResult result = this.contractState.InternalTransactionExecutor.Call(this.contractState, addressTo, amountToTransfer, methodName, parameters, gasLimit);
            if (result.Success)
            {
                if (this.Balances.ContainsKey(addressTo))
                {
                    this.Balances[addressTo] = this.Balances[addressTo] + (long)amountToTransfer;
                }
                else
                {
                    this.Balances.Add(addressTo, (long)amountToTransfer);
                }
            }
            return result;
        }
        protected ITransferResult Call(
            Address addressTo,
            ulong amountToTransfer,
            string methodName,
            object[]? parameters)
            => Call(addressTo, amountToTransfer, methodName, parameters, 0);
        
        protected ITransferResult Call(
            Address addressTo,
            ulong amountToTransfer,
            string methodName)
            => Call(addressTo, amountToTransfer, methodName, null, 0);        
        
        protected ICreateResult Create<T>(
          ulong amountToTransfer,
          object[] parameters,
          ulong gasLimit)
          where T : SmartContract
            => this.contractState.InternalTransactionExecutor.Create<T>(this.contractState, amountToTransfer, parameters, gasLimit);
            
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

        [Rep]
        protected Dictionary<Address, long> Balances;
        #endregion
    }
}
