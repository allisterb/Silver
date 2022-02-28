using System;
using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
    public class SilverVM 
    {
        public static bool RandomBool { [Pure] [Reads(ReadsAttribute.Reads.Nothing)] get => Rnd.Next() > 1000 ? true : false; }

        public static int RandomInt { [Pure][Reads(ReadsAttribute.Reads.Nothing)] get => Rnd.Next(); }

        public static uint RandomUInt { [Pure][Reads(ReadsAttribute.Reads.Nothing)] get => (uint)Rnd.Next(); }

        public static long RandomLong { [Pure][Reads(ReadsAttribute.Reads.Nothing)] get => Rnd.Next(); }

        public static ulong RandomULong { [Pure][Reads(ReadsAttribute.Reads.Nothing)] get => (ulong)Rnd.Next(); }

        public static UInt128 RandomUInt128 { [Pure][Reads(ReadsAttribute.Reads.Nothing)] get => new UInt128(RandomByteArray); }

        public static byte[] RandomByteArray 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get => new byte[RandomInt]; 
        }

        public static Address RandomAddress 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get => new Address(RandomUInt, RandomUInt, RandomUInt, RandomUInt, RandomUInt); 
        }

        public static RandomTransferResult RandomTransferResult 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get => new RandomTransferResult(); 
        }

        public static RandomCreateResult RandomCreateResult 
        {
            [Pure]
            [Reads(ReadsAttribute.Reads.Nothing)]
            [ResultNotNewlyAllocated]
            get => new RandomCreateResult(); 
        }

        #region Fields

        public static readonly Random Rnd = new Random();

        #endregion
    }

    public class RandomTransferResult : ITransferResult
    {
        public object ReturnValue { get => new object(); }

        public bool Success { get => SilverVM.RandomBool; }
    }

    public class RandomCreateResult: ICreateResult
    {
        public Address NewContractAddress { get => SilverVM.RandomAddress; }

        public bool Success { get => SilverVM.RandomBool; }
    }
}
