using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000081 RID: 129
	public class BitSet : ICloneable
	{
		// Token: 0x060004CA RID: 1226 RVA: 0x0000EB34 File Offset: 0x0000CD34
		public BitSet() : this(64)
		{
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0000EB40 File Offset: 0x0000CD40
		public BitSet(ulong[] bits_)
		{
			this.bits = bits_;
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0000EB50 File Offset: 0x0000CD50
		public BitSet(IList items) : this(64)
		{
			for (int i = 0; i < items.Count; i++)
			{
				int el = (int)items[i];
				this.Add(el);
			}
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x0000EB94 File Offset: 0x0000CD94
		public BitSet(int nbits)
		{
			this.bits = new ulong[(nbits - 1 >> 6) + 1];
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0000EBBC File Offset: 0x0000CDBC
		public static BitSet Of(int el)
		{
			BitSet bitSet = new BitSet(el + 1);
			bitSet.Add(el);
			return bitSet;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0000EBDC File Offset: 0x0000CDDC
		public static BitSet Of(int a, int b)
		{
			BitSet bitSet = new BitSet(Math.Max(a, b) + 1);
			bitSet.Add(a);
			bitSet.Add(b);
			return bitSet;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0000EC08 File Offset: 0x0000CE08
		public static BitSet Of(int a, int b, int c)
		{
			BitSet bitSet = new BitSet();
			bitSet.Add(a);
			bitSet.Add(b);
			bitSet.Add(c);
			return bitSet;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0000EC34 File Offset: 0x0000CE34
		public static BitSet Of(int a, int b, int c, int d)
		{
			BitSet bitSet = new BitSet();
			bitSet.Add(a);
			bitSet.Add(b);
			bitSet.Add(c);
			bitSet.Add(d);
			return bitSet;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0000EC64 File Offset: 0x0000CE64
		public virtual BitSet Or(BitSet a)
		{
			if (a == null)
			{
				return this;
			}
			BitSet bitSet = (BitSet)this.Clone();
			bitSet.OrInPlace(a);
			return bitSet;
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0000EC90 File Offset: 0x0000CE90
		public virtual void Add(int el)
		{
			int num = BitSet.WordNumber(el);
			if (num >= this.bits.Length)
			{
				this.GrowToInclude(el);
			}
			this.bits[num] |= BitSet.BitMask(el);
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0000ECD0 File Offset: 0x0000CED0
		public virtual void GrowToInclude(int bit)
		{
			int num = Math.Max(this.bits.Length << 1, this.NumWordsToHold(bit));
			ulong[] destinationArray = new ulong[num];
			Array.Copy(this.bits, 0, destinationArray, 0, this.bits.Length);
			this.bits = destinationArray;
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0000ED18 File Offset: 0x0000CF18
		public virtual void OrInPlace(BitSet a)
		{
			if (a == null)
			{
				return;
			}
			if (a.bits.Length > this.bits.Length)
			{
				this.SetSize(a.bits.Length);
			}
			int num = Math.Min(this.bits.Length, a.bits.Length);
			for (int i = num - 1; i >= 0; i--)
			{
				this.bits[i] |= a.bits[i];
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060004D7 RID: 1239 RVA: 0x0000ED94 File Offset: 0x0000CF94
		public virtual bool Nil
		{
			get
			{
				for (int i = this.bits.Length - 1; i >= 0; i--)
				{
					if (this.bits[i] != 0UL)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0000EDCC File Offset: 0x0000CFCC
		public virtual object Clone()
		{
			BitSet bitSet;
			try
			{
				bitSet = (BitSet)base.MemberwiseClone();
				bitSet.bits = new ulong[this.bits.Length];
				Array.Copy(this.bits, 0, bitSet.bits, 0, this.bits.Length);
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException("Unable to clone BitSet", innerException);
			}
			return bitSet;
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x0000EE3C File Offset: 0x0000D03C
		public virtual int Count
		{
			get
			{
				int num = 0;
				for (int i = this.bits.Length - 1; i >= 0; i--)
				{
					ulong num2 = this.bits[i];
					if (num2 != 0UL)
					{
						for (int j = 63; j >= 0; j--)
						{
							if ((num2 & 1UL << j) != 0UL)
							{
								num++;
							}
						}
					}
				}
				return num;
			}
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0000EE9C File Offset: 0x0000D09C
		public virtual bool Member(int el)
		{
			if (el < 0)
			{
				return false;
			}
			int num = BitSet.WordNumber(el);
			return num < this.bits.Length && (this.bits[num] & BitSet.BitMask(el)) != 0UL;
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0000EEE0 File Offset: 0x0000D0E0
		public virtual void Remove(int el)
		{
			int num = BitSet.WordNumber(el);
			if (num < this.bits.Length)
			{
				this.bits[num] &= ~BitSet.BitMask(el);
			}
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0000EF1C File Offset: 0x0000D11C
		public virtual int NumBits()
		{
			return this.bits.Length << 6;
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0000EF28 File Offset: 0x0000D128
		public virtual int LengthInLongWords()
		{
			return this.bits.Length;
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0000EF34 File Offset: 0x0000D134
		public virtual int[] ToArray()
		{
			int[] array = new int[this.Count];
			int num = 0;
			for (int i = 0; i < this.bits.Length << 6; i++)
			{
				if (this.Member(i))
				{
					array[num++] = i;
				}
			}
			return array;
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0000EF80 File Offset: 0x0000D180
		public virtual ulong[] ToPackedArray()
		{
			return this.bits;
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x0000EF88 File Offset: 0x0000D188
		private static int WordNumber(int bit)
		{
			return bit >> 6;
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x0000EF90 File Offset: 0x0000D190
		public override string ToString()
		{
			return this.ToString(null);
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x0000EF9C File Offset: 0x0000D19C
		public virtual string ToString(string[] tokenNames)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = ",";
			bool flag = false;
			stringBuilder.Append('{');
			for (int i = 0; i < this.bits.Length << 6; i++)
			{
				if (this.Member(i))
				{
					if (i > 0 && flag)
					{
						stringBuilder.Append(value);
					}
					if (tokenNames != null)
					{
						stringBuilder.Append(tokenNames[i]);
					}
					else
					{
						stringBuilder.Append(i);
					}
					flag = true;
				}
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x0000F02C File Offset: 0x0000D22C
		public override bool Equals(object other)
		{
			if (other == null || !(other is BitSet))
			{
				return false;
			}
			BitSet bitSet = (BitSet)other;
			int num = Math.Min(this.bits.Length, bitSet.bits.Length);
			for (int i = 0; i < num; i++)
			{
				if (this.bits[i] != bitSet.bits[i])
				{
					return false;
				}
			}
			if (this.bits.Length > num)
			{
				for (int j = num + 1; j < this.bits.Length; j++)
				{
					if (this.bits[j] != 0UL)
					{
						return false;
					}
				}
			}
			else if (bitSet.bits.Length > num)
			{
				for (int k = num + 1; k < bitSet.bits.Length; k++)
				{
					if (bitSet.bits[k] != 0UL)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0000F110 File Offset: 0x0000D310
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x0000F118 File Offset: 0x0000D318
		private static ulong BitMask(int bitNumber)
		{
			int num = bitNumber & BitSet.MOD_MASK;
			return 1UL << num;
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x0000F134 File Offset: 0x0000D334
		private void SetSize(int nwords)
		{
			ulong[] destinationArray = new ulong[nwords];
			int length = Math.Min(nwords, this.bits.Length);
			Array.Copy(this.bits, 0, destinationArray, 0, length);
			this.bits = destinationArray;
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0000F170 File Offset: 0x0000D370
		private int NumWordsToHold(int el)
		{
			return (el >> 6) + 1;
		}

		// Token: 0x04000145 RID: 325
		protected internal const int BITS = 64;

		// Token: 0x04000146 RID: 326
		protected internal const int LOG_BITS = 6;

		// Token: 0x04000147 RID: 327
		protected internal static readonly int MOD_MASK = 63;

		// Token: 0x04000148 RID: 328
		protected internal ulong[] bits;
	}
}
