// Decompiled with JetBrains decompiler
// Type: Stratis.SmartContracts.UIntBase
// Assembly: Stratis.SmartContracts, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 313BA83E-A11B-42B1-9AF7-0994F99B5586
// Assembly location: C:\Users\Allister\Downloads\Stratis.SmartContracts.dll

using System;
using System.Globalization;
using System.Numerics;

namespace Stratis.SmartContracts
{
  internal struct UIntBase : IComparable
  {
    private int width;
    private BigInteger value;

    public UIntBase(int width)
    {
        if ((width & 3) != 0)
        {
            throw new ArgumentException("The 'width' must be a multiple of 4.");
        }
        this.width = width;
        this.value = new BigInteger(width);
    }

    public UIntBase(int width, BigInteger value)
      : this(width)
    {
      this.SetValue(value);
    }

    public UIntBase(int width, UIntBase value)
      : this(width)
    {
      this.SetValue(value.value);
    }

    public UIntBase(int width, ulong b)
      : this(width)
    {
      this.SetValue(new BigInteger(b));
    }

    public UIntBase(int width, byte[] vch, bool lendian)
      : this(width)
    {
      if (vch.Length > this.width)
        throw new FormatException(string.Format("The byte array should be {0} bytes or less.", (object) this.width));
      this.SetValue(new BigInteger(vch));
    }

    public UIntBase(int width, string str)
      : this(width)
    {
      if (str.StartsWith("0x"))
        this.SetValue(BigInteger.Parse("0" + str.Substring(2), NumberStyles.HexNumber));
      else
        this.SetValue(BigInteger.Parse(str));
    }

    public UIntBase(int width, uint[] array)
      : this(width)
    {
      int num = this.width / 4;
      if (array.Length != num)
        throw new FormatException(string.Format("The array length should be {0}.", (object) num));
      byte[] numArray = new byte[this.width];
      for (int index = 0; index < num; ++index)
        BitConverter.GetBytes(array[index]).CopyTo((Array) numArray, index * 4);
      this.SetValue(new BigInteger(numArray));
    }

    private bool TooBig(byte[] bytes)
    {

        return bytes.Length > this.width && (bytes.Length != this.width + 1 || bytes[this.width] != (byte)0);
    }
    private void SetValue(BigInteger value)
    {
        if (value.Sign < 0)
        {
            throw new OverflowException("Only positive or zero values are allowed.");
        }
        if (this.TooBig(value.ToByteArray()))
        {
            throw new OverflowException();
        }
        this.value = value;
    }

    public BigInteger GetValue()
    {
        return this.value;
    }

    private uint[] ToUIntArray()
    {
      byte[] bytes = this.ToBytes(true);
      int length = this.width / 4;
      uint[] uintArray = new uint[length];
      for (int index = 0; index < length; ++index)
        uintArray[index] = BitConverter.ToUInt32(bytes, index * 4);
      return uintArray;
    }

    public byte[] ToBytes(bool lendian)
    {
      byte[] byteArray = this.value.ToByteArray();
      byte[] bytes = new byte[this.width];
      Array.Copy((Array) byteArray, (Array) bytes, Math.Min(byteArray.Length, bytes.Length));
      if (!lendian)
        Array.Reverse(bytes);
      return bytes;
    }

    internal BigInteger ShiftRight(int shift) => this.value >> shift;

    internal BigInteger ShiftLeft(int shift) => this.value << shift;

    internal BigInteger Add(BigInteger value2) => this.value + value2;

    internal BigInteger Subtract(BigInteger value2)
    {
      if (this.value.CompareTo(value2) < 0)
        throw new OverflowException("Result cannot be negative.");
      return this.value - value2;
    }

    internal BigInteger Multiply(BigInteger value2) => this.value * value2;

    internal BigInteger Divide(BigInteger value2) => this.value / value2;

    internal BigInteger Mod(BigInteger value2) => this.value % value2;

    public int CompareTo(object/*?*/ b) => this.value.CompareTo(((UIntBase)b).value);
    public static int Comparison(UIntBase a, UIntBase b) => a.CompareTo((object/*?*/) b);

    public override int GetHashCode()
    {
      uint[] uintArray = this.ToUIntArray();
      uint hashCode = 0;
      for (int index = 0; index < uintArray.Length; ++index)
        hashCode ^= uintArray[index];
      return (int) hashCode;
    }

    public override bool Equals(object/*?*/ obj) => this.CompareTo(obj) == 0;

    private static string ByteArrayToString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "");

    public string ToHex() => UIntBase.ByteArrayToString(this.ToBytes(false)).ToLower();

    public override string/*?*/ ToString() => this.value.ToString();
  }
}
