// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.SmartContract
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

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
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            this.contractState = contractState;
        }
        #endregion

        #region Methods
        protected ITransferResult Transfer(Address addressTo, ulong amountToTransfer) => this.contractState.InternalTransactionExecutor.Transfer(this.contractState, addressTo, amountToTransfer);

        protected ITransferResult Call(
          Address addressTo,
          ulong amountToTransfer,
          string methodName,
          object[]/*?*/ parameters,
          ulong gasLimit)
        {
            //^ expose(this) {
                return this.contractState.InternalTransactionExecutor.Call(this.contractState, addressTo, amountToTransfer, methodName, parameters, gasLimit);
            //^ }
        }

        protected ITransferResult Call(
            Address addressTo,
            ulong amountToTransfer,
            string methodName,
            object[]/*?*/ parameters)
        {
            //^ expose(this) {
                return Call(addressTo, amountToTransfer, methodName, parameters, 0);
            //^ }
        }

        protected ITransferResult Call(
            Address addressTo,
            ulong amountToTransfer,
            string methodName)
            {
                //^ expose(this) {
                    return Call(addressTo, amountToTransfer, methodName, null, 0);
                //^ }
            }
        protected ICreateResult Create<T>(
          ulong amountToTransfer,
          object[] /*?*/ parameters,
          ulong gasLimit)
          where T : SmartContract
            {
                return this.contractState.InternalTransactionExecutor.Create<T>(this.contractState, amountToTransfer, parameters, gasLimit);
            }

        protected byte[] Keccak256(byte[] toHash) => this.contractState.InternalHashHelper.Keccak256(toHash);

        protected void Assert(bool condition, string message)
        {
          if (!condition)
            throw new SmartContractAssertException(message);
        }

        protected void Assert(bool condition)
        {
            Assert(condition, "Assert failed.");
        }

        protected void Log<T>(T toLog) where T : struct => this.contractState.ContractLogger.Log<T>(this.contractState, toLog);

        public virtual void Receive()
        {
        }
        #endregion

        #region Fields
        [Rep]
        private readonly ISmartContractState contractState;

        protected Address Address => this.contractState.Message.ContractAddress;

        public ulong Balance => this.contractState.GetBalance();

        public IBlock Block => this.contractState.Block;

        public IMessage Message => this.contractState.Message;

        [Obsolete("Please use State property as shorthand", false)]
        public IPersistentState PersistentState
        {
            get
            {
                //^ expose(this) {
                    return this.contractState.PersistentState;
                //^ }
            }
        }

        public IPersistentState State => this.contractState.PersistentState;

        public ISerializer Serializer => this.contractState.Serializer;

        #endregion
    }
}
