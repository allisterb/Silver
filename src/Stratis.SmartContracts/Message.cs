// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.Message
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

namespace Stratis.SmartContracts
{
  public sealed class Message : IMessage
  {
    public Address ContractAddress { get; }

    public Address Sender { get; }

    public ulong Value { get; }

    public Message(Address contractAddress, Address sender, ulong value)
    {
      this.ContractAddress = contractAddress;
      this.Sender = sender;
      this.Value = value;
    }
  }
}
