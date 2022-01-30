// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.Address
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

using System;
using Microsoft.Contracts;
namespace Stratis.SmartContracts
{
  
  public struct Address
  {
    public static Address Zero;
    public const int Width = 20;
    private readonly uint pn0;
    private readonly uint pn1;
    private readonly uint pn2;
    private readonly uint pn3;
    private readonly uint pn4;

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

    public byte[] ToBytes()
    {
      byte[] dst = new byte[20];
      Buffer.BlockCopy((Array) BitConverter.GetBytes(this.pn0), 0, (Array) dst, 0, 4);
      Buffer.BlockCopy((Array) BitConverter.GetBytes(this.pn1), 0, (Array) dst, 4, 4);
      Buffer.BlockCopy((Array) BitConverter.GetBytes(this.pn2), 0, (Array) dst, 8, 4);
      Buffer.BlockCopy((Array) BitConverter.GetBytes(this.pn3), 0, (Array) dst, 12, 4);
      Buffer.BlockCopy((Array) BitConverter.GetBytes(this.pn4), 0, (Array) dst, 16, 4);
      return dst;
    }

    public override string ToString() => Address.UIntToHexString(this.pn0) + Address.UIntToHexString(this.pn1) + Address.UIntToHexString(this.pn2) + Address.UIntToHexString(this.pn3) + Address.UIntToHexString(this.pn4);

    private static string UIntToHexString(uint val) => "0123456789ABCDEF"[(int) ((val & 240U) >> 4)].ToString() + (object) "0123456789ABCDEF"[(int) val & 15] + (object) "0123456789ABCDEF"[(int) ((val & 61440U) >> 12)] + (object) "0123456789ABCDEF"[(int) ((val & 3840U) >> 8)] + (object) "0123456789ABCDEF"[(int) ((val & 15728640U) >> 20)] + (object) "0123456789ABCDEF"[(int) ((val & 983040U) >> 16)] + (object) "0123456789ABCDEF"[(int) ((val & 4026531840U) >> 28)] + (object) "0123456789ABCDEF"[(int) ((val & 251658240U) >> 24)];

    [Pure]
    public static bool operator ==(Address obj1, Address obj2) => obj1.Equals(obj2);

    [Pure]
    public static bool operator !=(Address obj1, Address obj2) => !obj1.Equals(obj2);

    [Pure]
    public override bool Equals(object obj) => obj is Address address && this.Equals(address);

    [Pure]
    public bool Equals(Address obj) => (1 & ((int) this.pn0 == (int) obj.pn0 ? 1 : 0) & ((int) this.pn1 == (int) obj.pn1 ? 1 : 0) & ((int) this.pn2 == (int) obj.pn2 ? 1 : 0) & ((int) this.pn3 == (int) obj.pn3 ? 1 : 0) & ((int) this.pn4 == (int) obj.pn4 ? 1 : 0)) != 0;
  }
}
