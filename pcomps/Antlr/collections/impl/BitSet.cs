using System;
using System.Collections;

namespace pcomps.Antlr.collections.impl
{
	// Token: 0x0200004D RID: 77
	public class BitSet : ICloneable
	{
		// Token: 0x060002D7 RID: 727 RVA: 0x00009564 File Offset: 0x00007764
		public BitSet() : this(64)
		{
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000957C File Offset: 0x0000777C
		public BitSet(long[] bits_)
		{
			this.dataBits = bits_;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00009598 File Offset: 0x00007798
		public BitSet(int nbits)
		{
			this.dataBits = new long[(nbits - 1 >> 6) + 1];
		}

		// Token: 0x060002DA RID: 730 RVA: 0x000095C0 File Offset: 0x000077C0
		public virtual void add(int el)
		{
			int num = BitSet.wordNumber(el);
			if (num >= this.dataBits.Length)
			{
				this.growToInclude(el);
			}
			long[] array;
			IntPtr intPtr;
			(array = this.dataBits)[(int)(intPtr = (IntPtr)num)] = (array[(int)intPtr] | BitSet.bitMask(el));
		}

		// Token: 0x060002DB RID: 731 RVA: 0x000095FC File Offset: 0x000077FC
		public virtual BitSet and(BitSet a)
		{
			BitSet bitSet = (BitSet)this.Clone();
			bitSet.andInPlace(a);
			return bitSet;
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00009620 File Offset: 0x00007820
		public virtual void andInPlace(BitSet a)
		{
			int num = Math.Min(this.dataBits.Length, a.dataBits.Length);
			for (int i = num - 1; i >= 0; i--)
			{
				long[] array;
				IntPtr intPtr;
				(array = this.dataBits)[(int)(intPtr = (IntPtr)i)] = (array[(int)intPtr] & a.dataBits[i]);
			}
			for (int j = num; j < this.dataBits.Length; j++)
			{
				this.dataBits[j] = 0L;
			}
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00009688 File Offset: 0x00007888
		private static long bitMask(int bitNumber)
		{
			int num = bitNumber & BitSet.MOD_MASK;
			return 1L << num;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x000096A4 File Offset: 0x000078A4
		public virtual void clear()
		{
			for (int i = this.dataBits.Length - 1; i >= 0; i--)
			{
				this.dataBits[i] = 0L;
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000096D0 File Offset: 0x000078D0
		public virtual void clear(int el)
		{
			int num = BitSet.wordNumber(el);
			if (num >= this.dataBits.Length)
			{
				this.growToInclude(el);
			}
			long[] array;
			IntPtr intPtr;
			(array = this.dataBits)[(int)(intPtr = (IntPtr)num)] = (array[(int)intPtr] & ~BitSet.bitMask(el));
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00009710 File Offset: 0x00007910
		public virtual object Clone()
		{
			BitSet bitSet;
			try
			{
				bitSet = new BitSet();
				bitSet.dataBits = new long[this.dataBits.Length];
				Array.Copy(this.dataBits, 0, bitSet.dataBits, 0, this.dataBits.Length);
			}
			catch
			{
				throw new ApplicationException();
			}
			return bitSet;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000976C File Offset: 0x0000796C
		public virtual int degree()
		{
			int num = 0;
			for (int i = this.dataBits.Length - 1; i >= 0; i--)
			{
				long num2 = this.dataBits[i];
				if (num2 != 0L)
				{
					for (int j = 63; j >= 0; j--)
					{
						if ((num2 & 1L << j) != 0L)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x000097C0 File Offset: 0x000079C0
		public override int GetHashCode()
		{
			return this.dataBits.GetHashCode();
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x000097D8 File Offset: 0x000079D8
		public override bool Equals(object obj)
		{
			if (obj != null && obj is BitSet)
			{
				BitSet bitSet = (BitSet)obj;
				int num = Math.Min(this.dataBits.Length, bitSet.dataBits.Length);
				int num2 = num;
				while (num2-- > 0)
				{
					if (this.dataBits[num2] != bitSet.dataBits[num2])
					{
						return false;
					}
				}
				if (this.dataBits.Length > num)
				{
					int num3 = this.dataBits.Length;
					while (num3-- > num)
					{
						if (this.dataBits[num3] != 0L)
						{
							return false;
						}
					}
				}
				else if (bitSet.dataBits.Length > num)
				{
					int num4 = bitSet.dataBits.Length;
					while (num4-- > num)
					{
						if (bitSet.dataBits[num4] != 0L)
						{
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00009894 File Offset: 0x00007A94
		public virtual void growToInclude(int bit)
		{
			int num = Math.Max(this.dataBits.Length << 1, this.numWordsToHold(bit));
			long[] destinationArray = new long[num];
			Array.Copy(this.dataBits, 0, destinationArray, 0, this.dataBits.Length);
			this.dataBits = destinationArray;
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x000098DC File Offset: 0x00007ADC
		public virtual bool member(int el)
		{
			int num = BitSet.wordNumber(el);
			return num < this.dataBits.Length && (this.dataBits[num] & BitSet.bitMask(el)) != 0L;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00009914 File Offset: 0x00007B14
		public virtual bool nil()
		{
			for (int i = this.dataBits.Length - 1; i >= 0; i--)
			{
				if (this.dataBits[i] != 0L)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00009948 File Offset: 0x00007B48
		public virtual BitSet not()
		{
			BitSet bitSet = (BitSet)this.Clone();
			bitSet.notInPlace();
			return bitSet;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00009968 File Offset: 0x00007B68
		public virtual void notInPlace()
		{
			for (int i = this.dataBits.Length - 1; i >= 0; i--)
			{
				this.dataBits[i] = ~this.dataBits[i];
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000999C File Offset: 0x00007B9C
		public virtual void notInPlace(int maxBit)
		{
			this.notInPlace(0, maxBit);
		}

		// Token: 0x060002EA RID: 746 RVA: 0x000099B4 File Offset: 0x00007BB4
		public virtual void notInPlace(int minBit, int maxBit)
		{
			this.growToInclude(maxBit);
			for (int i = minBit; i <= maxBit; i++)
			{
				int num = BitSet.wordNumber(i);
				long[] array;
				IntPtr intPtr;
				(array = this.dataBits)[(int)(intPtr = (IntPtr)num)] = (array[(int)intPtr] ^ BitSet.bitMask(i));
			}
		}

		// Token: 0x060002EB RID: 747 RVA: 0x000099F4 File Offset: 0x00007BF4
		private int numWordsToHold(int el)
		{
			return (el >> 6) + 1;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00009A08 File Offset: 0x00007C08
		public static BitSet of(int el)
		{
			BitSet bitSet = new BitSet(el + 1);
			bitSet.add(el);
			return bitSet;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00009A28 File Offset: 0x00007C28
		public virtual BitSet or(BitSet a)
		{
			BitSet bitSet = (BitSet)this.Clone();
			bitSet.orInPlace(a);
			return bitSet;
		}

		// Token: 0x060002EE RID: 750 RVA: 0x00009A4C File Offset: 0x00007C4C
		public virtual void orInPlace(BitSet a)
		{
			if (a.dataBits.Length > this.dataBits.Length)
			{
				this.setSize(a.dataBits.Length);
			}
			int num = Math.Min(this.dataBits.Length, a.dataBits.Length);
			for (int i = num - 1; i >= 0; i--)
			{
				long[] array;
				IntPtr intPtr;
				(array = this.dataBits)[(int)(intPtr = (IntPtr)i)] = (array[(int)intPtr] | a.dataBits[i]);
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00009AB8 File Offset: 0x00007CB8
		public virtual void remove(int el)
		{
			int num = BitSet.wordNumber(el);
			if (num >= this.dataBits.Length)
			{
				this.growToInclude(el);
			}
			long[] array;
			IntPtr intPtr;
			(array = this.dataBits)[(int)(intPtr = (IntPtr)num)] = (array[(int)intPtr] & ~BitSet.bitMask(el));
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00009AF8 File Offset: 0x00007CF8
		private void setSize(int nwords)
		{
			long[] destinationArray = new long[nwords];
			int length = Math.Min(nwords, this.dataBits.Length);
			Array.Copy(this.dataBits, 0, destinationArray, 0, length);
			this.dataBits = destinationArray;
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00009B34 File Offset: 0x00007D34
		public virtual int size()
		{
			return this.dataBits.Length << 6;
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00009B4C File Offset: 0x00007D4C
		public virtual int lengthInLongWords()
		{
			return this.dataBits.Length;
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00009B64 File Offset: 0x00007D64
		public virtual bool subset(BitSet a)
		{
			return a != null && this.and(a).Equals(this);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00009B84 File Offset: 0x00007D84
		public virtual void subtractInPlace(BitSet a)
		{
			if (a == null)
			{
				return;
			}
			int num = 0;
			while (num < this.dataBits.Length && num < a.dataBits.Length)
			{
				long[] array;
				IntPtr intPtr;
				(array = this.dataBits)[(int)(intPtr = (IntPtr)num)] = (array[(int)intPtr] & ~a.dataBits[num]);
				num++;
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00009BCC File Offset: 0x00007DCC
		public virtual int[] toArray()
		{
			int[] array = new int[this.degree()];
			int num = 0;
			for (int i = 0; i < this.dataBits.Length << 6; i++)
			{
				if (this.member(i))
				{
					array[num++] = i;
				}
			}
			return array;
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00009C10 File Offset: 0x00007E10
		public virtual long[] toPackedArray()
		{
			return this.dataBits;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00009C24 File Offset: 0x00007E24
		public override string ToString()
		{
			return this.ToString(",");
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00009C3C File Offset: 0x00007E3C
		public virtual string ToString(string separator)
		{
			string text = "";
			for (int i = 0; i < this.dataBits.Length << 6; i++)
			{
				if (this.member(i))
				{
					if (text.Length > 0)
					{
						text += separator;
					}
					text += i;
				}
			}
			return text;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00009C8C File Offset: 0x00007E8C
		public virtual string ToString(string separator, ArrayList vocabulary)
		{
			if (vocabulary == null)
			{
				return this.ToString(separator);
			}
			string text = "";
			for (int i = 0; i < this.dataBits.Length << 6; i++)
			{
				if (this.member(i))
				{
					if (text.Length > 0)
					{
						text += separator;
					}
					if (i >= vocabulary.Count)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"<bad element ",
							i,
							">"
						});
					}
					else if (vocabulary[i] == null)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"<",
							i,
							">"
						});
					}
					else
					{
						text += (string)vocabulary[i];
					}
				}
			}
			return text;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00009D68 File Offset: 0x00007F68
		public virtual string toStringOfHalfWords()
		{
			string text = new string("".ToCharArray());
			for (int i = 0; i < this.dataBits.Length; i++)
			{
				if (i != 0)
				{
					text += ", ";
				}
				long num = this.dataBits[i];
                ulong v = unchecked((ulong)-1);
                num &= (long)v;
				text = text + num + "UL";
				text += ", ";
				num = SupportClass.URShift(this.dataBits[i], 32);
				num &= (long)v;
				text = text + num + "UL";
			}
			return text;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00009DFC File Offset: 0x00007FFC
		public virtual string toStringOfWords()
		{
			string text = new string("".ToCharArray());
			for (int i = 0; i < this.dataBits.Length; i++)
			{
				if (i != 0)
				{
					text += ", ";
				}
				text = text + this.dataBits[i] + "L";
			}
			return text;
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00009E58 File Offset: 0x00008058
		private static int wordNumber(int bit)
		{
			return bit >> 6;
		}

		// Token: 0x040000E0 RID: 224
		protected internal const int BITS = 64;

		// Token: 0x040000E1 RID: 225
		protected internal const int NIBBLE = 4;

		// Token: 0x040000E2 RID: 226
		protected internal const int LOG_BITS = 6;

		// Token: 0x040000E3 RID: 227
		protected internal static readonly int MOD_MASK = 63;

		// Token: 0x040000E4 RID: 228
		protected internal long[] dataBits;
	}
}
