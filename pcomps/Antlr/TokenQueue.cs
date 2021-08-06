namespace pcomps.Antlr
{
	// Token: 0x0200003B RID: 59
	internal class TokenQueue
	{
		// Token: 0x0600025A RID: 602 RVA: 0x00008118 File Offset: 0x00006318
		public TokenQueue(int minSize)
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

		// Token: 0x0600025B RID: 603 RVA: 0x00008160 File Offset: 0x00006360
		public void append(IToken tok)
		{
			if (this.nbrEntries == this.buffer.Length)
			{
				this.expand();
			}
			this.buffer[this.offset + this.nbrEntries & this.sizeLessOne] = tok;
			this.nbrEntries++;
		}

		// Token: 0x0600025C RID: 604 RVA: 0x000081B0 File Offset: 0x000063B0
		public IToken elementAt(int idx)
		{
			return this.buffer[this.offset + idx & this.sizeLessOne];
		}

		// Token: 0x0600025D RID: 605 RVA: 0x000081D4 File Offset: 0x000063D4
		private void expand()
		{
			IToken[] array = new IToken[this.buffer.Length * 2];
			for (int i = 0; i < this.buffer.Length; i++)
			{
				array[i] = this.elementAt(i);
			}
			this.buffer = array;
			this.sizeLessOne = this.buffer.Length - 1;
			this.offset = 0;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000822C File Offset: 0x0000642C
		private void init(int size)
		{
			this.buffer = new IToken[size];
			this.sizeLessOne = size - 1;
			this.offset = 0;
			this.nbrEntries = 0;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000825C File Offset: 0x0000645C
		public void reset()
		{
			this.offset = 0;
			this.nbrEntries = 0;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00008278 File Offset: 0x00006478
		public void removeFirst()
		{
			this.offset = (this.offset + 1 & this.sizeLessOne);
			this.nbrEntries--;
		}

		// Token: 0x040000B9 RID: 185
		private IToken[] buffer;

		// Token: 0x040000BA RID: 186
		private int sizeLessOne;

		// Token: 0x040000BB RID: 187
		private int offset;

		// Token: 0x040000BC RID: 188
		protected internal int nbrEntries;
	}
}
