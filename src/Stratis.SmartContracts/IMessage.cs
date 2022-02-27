namespace Stratis.SmartContracts
{
    public interface IMessage
    {
        Address ContractAddress { get; }
        Address Sender { get; }
        ulong Value { get; }
    }
}
