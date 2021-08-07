namespace pcomps.Antlr
{
	// Token: 0x02000049 RID: 73
	public class TokenWithIndex : CommonToken
	{
		// Token: 0x060002BA RID: 698 RVA: 0x00009094 File Offset: 0x00007294
		public TokenWithIndex()
		{
		}

		// Token: 0x060002BB RID: 699 RVA: 0x000090A8 File Offset: 0x000072A8
		public TokenWithIndex(int i, string t) : base(i, t)
		{
		}

		// Token: 0x060002BC RID: 700 RVA: 0x000090C0 File Offset: 0x000072C0
		public void setIndex(int i)
		{
			index = i;
		}

		// Token: 0x060002BD RID: 701 RVA: 0x000090D4 File Offset: 0x000072D4
		public int getIndex()
		{
			return index;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x000090E8 File Offset: 0x000072E8
		public override string ToString()
		{
			return string.Concat("[", index, ":\"", getText(), "\",<", Type, ">,line=", line, ",col=", col, "]\n");
		}

		// Token: 0x040000D5 RID: 213
		private int index;
	}
}
