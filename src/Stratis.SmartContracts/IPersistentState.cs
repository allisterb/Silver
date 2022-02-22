// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.IPersistentState
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

using System;

namespace Stratis.SmartContracts
{
  public interface IPersistentState
  {
    bool IsContract(Address address);

    byte[] GetBytes(byte[] key);

    byte[] GetBytes(string key);

    char GetChar(string key);

    Address GetAddress(string key);

    bool GetBool(string key);

    int GetInt32(string key);

    //@ modifies this.0;
    uint GetUInt32(string key);

    long GetInt64(string key);

    ulong GetUInt64(string key);

    UInt128 GetUInt128(string key);

    UInt256 GetUInt256(string key);

    string GetString(string key);

    T GetStruct<T>(string key) where T : struct;

    T[] GetArray<T>(string key);

    void SetBytes(byte[] key, byte[] value);

    void SetBytes(string key, byte[] value);

    void SetChar(string key, char value);

    void SetAddress(string key, Address value);

    void SetBool(string key, bool value);

    void SetInt32(string key, int value);

    void SetUInt32(string key, uint value);

    void SetInt64(string key, long value);

    void SetUInt64(string key, ulong value);

    void SetUInt128(string key, UInt128 value);

    void SetUInt256(string key, UInt256 value);

    void SetString(string key, string value);

    void SetStruct<T>(string key, T value) where T : struct;

    void SetArray(string key, Array a);

    void Clear(string key);
  }
}
