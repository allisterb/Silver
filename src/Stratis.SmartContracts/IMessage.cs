namespace Stratis.SmartContracts
{
    public interface IMessage
    {
        Address ContractAddress { get; }

        Address Sender { get; }

        #if NETSTANDARD2_0_OR_GREATER
        ulong Value { get; }
        #else
        long Value { get; }
        #endif
    }
}
