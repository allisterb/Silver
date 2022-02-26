using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
    public class SilverSmartContractPersistentState: IPersistentState
    {
        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public bool IsContract(Address address) => SilverSmartContractVM.RandomBool;

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public byte[] GetBytes(byte[] key) => Get<byte[]>(key.ToString());

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public byte[] GetBytes(string key) => Get<byte[]>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public char GetChar(string key) => Get<char>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public Address GetAddress(string key) => Get<Address>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public bool GetBool(string key) => Get<bool>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public int GetInt32(string key) => Get<int>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public uint GetUInt32(string key) => Get<uint>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public long GetInt64(string key) => Get<long>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public ulong GetUInt64(string key) => Get<ulong>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public UInt128 GetUInt128(string key) => Get<UInt128>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public UInt256 GetUInt256(string key) => Get<UInt256>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public T[] GetArray<T>(string key) => Get<T[]>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public string GetString(string key) => Get<string>(key);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public  T GetStruct<T>(string key) where T : struct => Get<T>(key);


        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetBytes(byte[] key, byte[] value) => Set(key.ToString(), value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetBytes(string key, byte[] value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetChar(string key, char value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetAddress(string key, Address value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetBool(string key, bool value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetInt32(string key, int value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetUInt32(string key, uint value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetInt64(string key, long value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetUInt64(string key, ulong value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetUInt128(string key, UInt128 value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetUInt256(string key, UInt256 value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetString(string key, string value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetStruct<T>(string key, T value) where T : struct => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void SetArray(string key, Array value) => Set(key, value);

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public void Clear(string key)
        {
            if (this.State.ContainsKey(key))
            {
                this.State.Remove(key);
            }
        }

        private T Get<T>(string key)
        {
            if (this.State.ContainsKey(key))
            {
                return (T) this.State[key];
            }
            else
            {
                this.State.Add(key, new object());
                return (T) this.State[key];
            }
        }

        private void Set<T>(string key, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (this.State.ContainsKey(key))
            {
                this.State[key] = value;
            }
            else
            {
                this.State.Add(key, value);
            }
        }

        #region Fields
        private readonly Dictionary<string, object> State = new Dictionary<string, object>();
        #endregion
    }
}
