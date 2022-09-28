using System;
using System.Collections.Generic;

using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
    public abstract class SmartContract
    {
    #if NETSTANDARD2_0_OR_GREATER

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
          this.contractState = contractState;
        }

        protected ITransferResult Transfer(Address addressTo, ulong amountToTransfer) => this.contractState.InternalTransactionExecutor.Transfer(this.contractState, addressTo, amountToTransfer);

        protected ITransferResult Call(
          Address addressTo,
          ulong amountToTransfer,
          string methodName,
          object[]? parameters = null,
          ulong gasLimit = 0)
        {
          return this.contractState.InternalTransactionExecutor.Call(this.contractState, addressTo, amountToTransfer, methodName, parameters, gasLimit);
        }

        protected ICreateResult Create<T>(
          ulong amountToTransfer = 0,
          object[]? parameters = null,
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
    
    #else
        // Smart contract implementation for verifier
        #region Constructors
        public SmartContract(ISmartContractState __contractState)
        {
            SilverSmartContractState _contractState = (SilverSmartContractState) __contractState;
            contractState = _contractState;
            Address = _contractState.Message.ContractAddress;
            Balance = _contractState.GetBalance();
            Block = _contractState.Block;
            Message = _contractState.Message;
            Serializer = _contractState.Serializer;
            State = _contractState.PersistentState;
            PersistentState = _contractState.PersistentState;
            Balances.Add(_contractState.Message.ContractAddress, _contractState.GetBalance());
            Balances.Add(_contractState.Message.Sender, 0UL - _contractState.Message.Value);
        }
        #endregion

        #region Methods

        #region Stratis interface
        protected static ITransferResult Transfer(Address addressTo, ulong amountToTransfer)
        //@ ensures Balances.ContainsKey(addressTo);
        //@ ensures(result.Success && (GetBalance(addressTo) == old(GetBalance(addressTo)) + amountToTransfer)) || (!result.Success);
        {
            if (!Balances.ContainsKey(addressTo))
            {
                Balances.Add(addressTo, 0UL);
            }
            ITransferResult result = SilverVM.RandomTransferResult;
            if (result.Success)
            {
                Balances[addressTo] = Balances[addressTo] + amountToTransfer;
            }

            return result;
        }
        protected static ITransferResult Call(
          Address addressTo,
          ulong amountToTransfer,
          string methodName,
          object[]? parameters,
          ulong gasLimit)
        //@ ensures Balances.ContainsKey(addressTo);
        //@ ensures(result.Success && (GetBalance(addressTo) == old(GetBalance(addressTo)) + amountToTransfer)) || (!result.Success);
        {
            ITransferResult result = SilverVM.RandomTransferResult;
            if (result.Success)
            {
                if (Balances.ContainsKey(addressTo))
                {
                    Balances[addressTo] = Balances[addressTo] + amountToTransfer;
                }
                else
                {
                    Balances.Add(addressTo, amountToTransfer);
                }
            }
            return result;
        }
        protected static ITransferResult Call(
            Address addressTo,
            ulong amountToTransfer,
            string methodName,
            object[]? parameters)
            => Call(addressTo, amountToTransfer, methodName, parameters, 0);

        protected static ITransferResult Call(
            Address addressTo,
            ulong amountToTransfer,
            string methodName)
            => Call(addressTo, amountToTransfer, methodName, null, 0);

        protected static ICreateResult Create<T>(
          ulong amountToTransfer,
          object[]? parameters,
          ulong gasLimit)
          where T : SmartContract
            => SilverVM.RandomCreateResult;

        [Pure]
        protected byte[] Keccak256(byte[] toHash) => contractState.InternalHashHelper.Keccak256(toHash);

        [Pure]
        protected static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new SmartContractAssertException(message);
        }

        [Pure]
        protected static void Assert(bool condition)
        {
            Assert(condition, "Assert failed.");
        }

        [Pure]
        protected static void Log<T>(T toLog) where T : struct
        {

        }

        public virtual void Receive()
        {
        }
        #endregion

        #region Silver extensions
        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public static ulong GetBalance(Address address)
        
        {
            if (!Balances.ContainsKey(address))
            {
                Balances.Add(address, 0UL);
            }
            return Balances[address];
        }
        #endregion

        #endregion

        #region Fields
        [Rep]
        public readonly SilverSmartContractState contractState;

        [Rep]
        [NotNull]
        public readonly SilverSmartContractPersistentState State;

        [Rep]
        public readonly Address Address; // => this.State.Message.ContractAddress;

        [Rep]
        public readonly ulong Balance; // => this.contractState.GetBalance();

        [Rep]
        public readonly IBlock Block; // => this.contractState.Block;

        [Rep]
        public readonly Message Message; // => this.contractState.Message;

        [Rep]
        public readonly SilverSmartContractPersistentState PersistentState; // => this.contractState.PersistentState;

        [Rep]
        public ISerializer Serializer; //=> this.State.Serializer;

        [Rep]
        [ElementsPeer(0)]
        private static readonly Dictionary<Address, ulong> Balances = new Dictionary<Address, ulong>();
        #endregion

    #endif
    }
}
