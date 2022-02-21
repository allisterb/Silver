// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.Message
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

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

    private Address _ContractAddress;

    private Address _Sender;

    private ulong _Value;
  }
}
