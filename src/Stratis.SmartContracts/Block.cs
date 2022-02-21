// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.Block
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

namespace Stratis.SmartContracts
{
  public struct Block : IBlock
  {
    public Address Coinbase { get => _coinbase; }

    public ulong Number { get => _number; }

    public Block(ulong number, Address coinbase)
    {
      this._number = number;
      this._coinbase = coinbase;
    }

    private ulong _number;
    private Address _coinbase;
  }
}
