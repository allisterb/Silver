﻿// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.IBlock
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

namespace Stratis.SmartContracts
{
  public interface IBlock
  {
    Address Coinbase { get; }

    ulong Number { get; }
  }
}
