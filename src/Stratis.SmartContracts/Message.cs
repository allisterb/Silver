namespace Stratis.SmartContracts
{
    public sealed class Message : IMessage
    {
        public Address ContractAddress { get => _ContractAddress; }

        public Address Sender { get => _Sender; }

        public ulong Value { get => _Value; }
        public Message(Address contractAddress, Address sender, ulong value)
        {
            this._ContractAddress = contractAddress;
            this._Sender = sender;
            this._Value = value;
        }

        private ulong _Value;
        private Address _ContractAddress;
        private Address _Sender;
    }   
}

