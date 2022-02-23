using System;

using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
  public interface IPersistentState
  {
        [Pure]
        bool IsContract(Address address);

        [Pure]
        byte[] GetBytes(byte[] key);

        [Pure]
        byte[] GetBytes(string key);

        [Pure]
        char GetChar(string key);

        [Pure]
        Address GetAddress(string key);

        [Pure]
        bool GetBool(string key);

        [Pure]
        int GetInt32(string key);

        [Pure]
        uint GetUInt32(string key);

        [Pure]
        long GetInt64(string key);

        [Pure]
        ulong GetUInt64(string key);

        [Pure]
        UInt128 GetUInt128(string key);

        [Pure]
        UInt256 GetUInt256(string key);

        [Pure]
        string GetString(string key);

        [Pure]
        T GetStruct<T>(string key) where T : struct;

        [Pure]
        T[] GetArray<T>(string key);

        [Pure]
        void SetBytes(byte[] key, byte[] value);

        [Pure]
        void SetBytes(string key, byte[] value);

        [Pure]
        void SetChar(string key, char value);

        [Pure]
        void SetAddress(string key, Address value);

        [Pure]
        void SetBool(string key, bool value);

        [Pure]
        void SetInt32(string key, int value);

        [Pure]
        void SetUInt32(string key, uint value);

        [Pure]
        void SetInt64(string key, long value);

        [Pure]
        void SetUInt64(string key, ulong value);

        [Pure]
        void SetUInt128(string key, UInt128 value);

        [Pure]
        void SetUInt256(string key, UInt256 value);

        [Pure]
        void SetString(string key, string value);

        [Pure]
        void SetStruct<T>(string key, T value) where T : struct;

        [Pure]
        void SetArray(string key, Array a);

        [Pure]
        void Clear(string key);
  }
}
