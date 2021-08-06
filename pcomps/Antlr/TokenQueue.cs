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
				init(16);
				return;
			}
			if (minSize >= 1073741823)
			{
				init(int.MaxValue);
				return;
			}
			int i;
			for (i = 2; i < minSize; i *= 2)
			{
			}
			init(i);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00008160 File Offset: 0x00006360
		public void append(IToken tok)
		{
			if (nbrEntries == buffer.Length)
			{
				expand();
			}
			buffer[offset + nbrEntries & sizeLessOne] = tok;
			nbrEntries++;
		}

		// Token: 0x0600025C RID: 604 RVA: 0x000081B0 File Offset: 0x000063B0
		public IToken elementAt(int idx)
		{
			return buffer[offset + idx & sizeLessOne];
		}

		// Token: 0x0600025D RID: 605 RVA: 0x000081D4 File Offset: 0x000063D4
		private void expand()
		{
			IToken[] array = new IToken[buffer.Length * 2];
			for (int i = 0; i < buffer.Length; i++)
			{
				array[i] = elementAt(i);
			}
			buffer = array;
			sizeLessOne = buffer.Length - 1;
			offset = 0;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000822C File Offset: 0x0000642C
		private void init(int size)
		{
			buffer = new IToken[size];
			sizeLessOne = size - 1;
			offset = 0;
			nbrEntries = 0;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000825C File Offset: 0x0000645C
		public void reset()
		{
			offset = 0;
			nbrEntries = 0;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00008278 File Offset: 0x00006478
		public void removeFirst()
		{
			offset = (offset + 1 & sizeLessOne);
			nbrEntries--;
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
