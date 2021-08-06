﻿namespace pcomps.Antlr
{
	// Token: 0x02000012 RID: 18
	public class CharQueue
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00004418 File Offset: 0x00002618
		public CharQueue(int minSize)
		{
			if (minSize < 0)
			{
				this.init(16);
				return;
			}
			if (minSize >= 1073741823)
			{
				this.init(int.MaxValue);
				return;
			}
			int i;
			for (i = 2; i < minSize; i *= 2)
			{
			}
			this.init(i);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00004460 File Offset: 0x00002660
		public void append(char tok)
		{
			if (this.nbrEntries == this.buffer.Length)
			{
				this.expand();
			}
			this.buffer[this.offset + this.nbrEntries & this.sizeLessOne] = tok;
			this.nbrEntries++;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000044B0 File Offset: 0x000026B0
		public char elementAt(int idx)
		{
			return this.buffer[this.offset + idx & this.sizeLessOne];
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000044D4 File Offset: 0x000026D4
		private void expand()
		{
			char[] array = new char[this.buffer.Length * 2];
			for (int i = 0; i < this.buffer.Length; i++)
			{
				array[i] = this.elementAt(i);
			}
			this.buffer = array;
			this.sizeLessOne = this.buffer.Length - 1;
			this.offset = 0;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000452C File Offset: 0x0000272C
		public virtual void init(int size)
		{
			this.buffer = new char[size];
			this.sizeLessOne = size - 1;
			this.offset = 0;
			this.nbrEntries = 0;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000455C File Offset: 0x0000275C
		public void reset()
		{
			this.offset = 0;
			this.nbrEntries = 0;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00004578 File Offset: 0x00002778
		public void removeFirst()
		{
			this.offset = (this.offset + 1 & this.sizeLessOne);
			this.nbrEntries--;
		}

		// Token: 0x0400002E RID: 46
		protected internal char[] buffer;

		// Token: 0x0400002F RID: 47
		private int sizeLessOne;

		// Token: 0x04000030 RID: 48
		private int offset;

		// Token: 0x04000031 RID: 49
		protected internal int nbrEntries;
	}
}
