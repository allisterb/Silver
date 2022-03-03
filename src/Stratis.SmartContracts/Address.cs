using System;
using Microsoft.Contracts;

namespace Stratis.SmartContracts
{
    #if NETSTANDARD2_0_OR_GREATER
    public struct Address
    #else   
    public class Address
    #endif
    {
        #region Constructors
        public Address(Address other)
        {
            this.pn0 = other.pn0;
            this.pn1 = other.pn1;
            this.pn2 = other.pn2;
            this.pn3 = other.pn3;
            this.pn4 = other.pn4;
        }

        public Address(uint pn0, uint pn1, uint pn2, uint pn3, uint pn4)
        {
            this.pn0 = pn0;
            this.pn1 = pn1;
            this.pn2 = pn2;
            this.pn3 = pn3;
            this.pn4 = pn4;
        }
        #endregion

        #region Methods
        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public byte[] ToBytes()
        {
            byte[] dst = new byte[20];
            Buffer.BlockCopy((Array)BitConverter.GetBytes(this.pn0), 0, (Array)dst, 0, 4);
            Buffer.BlockCopy((Array)BitConverter.GetBytes(this.pn1), 0, (Array)dst, 4, 4);
            Buffer.BlockCopy((Array)BitConverter.GetBytes(this.pn2), 0, (Array)dst, 8, 4);
            Buffer.BlockCopy((Array)BitConverter.GetBytes(this.pn3), 0, (Array)dst, 12, 4);
            Buffer.BlockCopy((Array)BitConverter.GetBytes(this.pn4), 0, (Array)dst, 16, 4);
            return dst;
        }

        [Pure]
        private static string UIntToHexString(uint val) => "0123456789ABCDEF"[(int)((val & 240U) >> 4)].ToString() + (object)"0123456789ABCDEF"[(int)val & 15] + (object)"0123456789ABCDEF"[(int)((val & 61440U) >> 12)] + (object)"0123456789ABCDEF"[(int)((val & 3840U) >> 8)] + (object)"0123456789ABCDEF"[(int)((val & 15728640U) >> 20)] + (object)"0123456789ABCDEF"[(int)((val & 983040U) >> 16)] + (object)"0123456789ABCDEF"[(int)((val & 4026531840U) >> 28)] + (object)"0123456789ABCDEF"[(int)((val & 251658240U) >> 24)];

        [Pure]
        [NoReferenceComparison]
        public static bool operator == (Address obj1, Address obj2) => obj1.pn0 == obj2.pn0 && obj1.pn1 == obj2.pn1 && obj1.pn2 == obj2.pn2 && obj1.pn3 == obj2.pn3 && obj1.pn4 == obj2.pn4;

        [Pure]
        [NoReferenceComparison]
        public static bool operator != (Address obj1, Address obj2) => obj1.pn0 != obj2.pn0 || obj1.pn1 != obj2.pn1 || obj1.pn2 != obj2.pn2 || obj1.pn3 != obj2.pn3 || obj1.pn4 != obj2.pn4;

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        [NoReferenceComparison]
        public bool Equals(Address obj) => pn0 == obj.pn0 && pn1 == obj.pn1 && pn2 == obj.pn2 && pn3 == obj.pn3 && pn4 == obj.pn4;
        #endregion

        #region Overrides
        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public override bool Equals(object? _obj)
        {
            if (_obj != null && _obj is Address)
            {
                Address obj = (Address)_obj;
                return pn0 == obj.pn0 && pn1 == obj.pn1 && pn2 == obj.pn2 && pn3 == obj.pn3 && pn4 == obj.pn4;           
            }
            else
            {
                return false;
            }
        }

        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public override string ToString() => Address.UIntToHexString(this.pn0) + Address.UIntToHexString(this.pn1) + Address.UIntToHexString(this.pn2) + Address.UIntToHexString(this.pn3) + Address.UIntToHexString(this.pn4);
        
        [Pure]
        [Reads(ReadsAttribute.Reads.Nothing)]
        public override int GetHashCode() => this.ToString().GetHashCode();
        #endregion

        #region Fields
        [Rep]
        public const int Width = 20;

        [Rep]
        public static readonly Address Zero = new Address(0, 0, 0, 0, 0);
        [Rep]
        private readonly uint pn0;
        [Rep]
        private readonly uint pn1;
        [Rep]
        private readonly uint pn2;
        [Rep]
        private readonly uint pn3;
        [Rep]
        private readonly uint pn4;
        #endregion
    }
}
