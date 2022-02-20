// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.ISerializer
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

using System;

namespace Stratis.SmartContracts
{
  public interface ISerializer
  {
    byte[] Serialize(char c);

    byte[] Serialize(Address address);

    byte[] Serialize(bool b);

    byte[] Serialize(int i);

    byte[] Serialize(long l);

    byte[] Serialize(uint u);

    byte[] Serialize(ulong ul);

    byte[] Serialize(UInt128 uInt128);

    byte[] Serialize(UInt256 uInt256);

    byte[] Serialize(string s);

    byte[] Serialize(Array a);

    byte[] Serialize<T>(T s) where T : struct;

    bool ToBool(byte[] val);

    Address ToAddress(byte[] val);

    Address ToAddress(string val);

    int ToInt32(byte[] val);

    uint ToUInt32(byte[] val);

    long ToInt64(byte[] val);

    ulong ToUInt64(byte[] val);

    UInt128 ToUInt128(byte[] val);

    UInt256 ToUInt256(byte[] val);

    string ToString(byte[] val);

    char ToChar(byte[] val);

    T[] ToArray<T>(byte[] val);

    T ToStruct<T>(byte[] val) where T : struct;
  }
}
