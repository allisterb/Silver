using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratis.SmartContracts
{
    public class SilverVM 
    {
        public static bool RandomBool { get => Rnd.Next() > 1000 ? true : false; }

        public static int RandomInt { get => Rnd.Next(); }

        public static uint RandomUInt { get => (uint)Rnd.Next(); }

        public static long RandomLong { get => Rnd.Next(); }

        public static ulong RandomULong { get => (ulong)Rnd.Next(); }

        public static UInt128 RandomUInt128 { get => new UInt128(RandomByteArray); }

        public static byte[] RandomByteArray { get => new byte[RandomInt]; }

        public static Address RandomAddress { get => new Address(RandomUInt, RandomUInt, RandomUInt, RandomUInt, RandomUInt); }

        public static RandomTransferResult RandomTransferResult { get => new RandomTransferResult(); }

        public static RandomCreateResult RandomCreateResult { get => new RandomCreateResult(); }

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
