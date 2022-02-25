using System;

using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
  public interface IPersistentState
  {
        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        bool IsContract(Address address);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        byte[] GetBytes(byte[] key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        byte[] GetBytes(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        char GetChar(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        Address GetAddress(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        bool GetBool(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        int GetInt32(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        uint GetUInt32(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        long GetInt64(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        ulong GetUInt64(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        UInt128 GetUInt128(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        UInt256 GetUInt256(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        string GetString(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        T GetStruct<T>(string key) where T : struct;

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        T[] GetArray<T>(string key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetBytes(byte[] key, byte[] value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetBytes(string key, byte[] value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetChar(string key, char value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetAddress(string key, Address value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetBool(string key, bool value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetInt32(string key, int value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetUInt32(string key, uint value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetInt64(string key, long value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetUInt64(string key, ulong value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetUInt128(string key, UInt128 value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetUInt256(string key, UInt256 value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetString(string key, string value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetStruct<T>(string key, T value) where T : struct;

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void SetArray(string key, Array a);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        void Clear(string key);
  }
}
