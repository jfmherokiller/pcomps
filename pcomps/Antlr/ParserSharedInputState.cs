namespace pcomps.Antlr
{
	// Token: 0x02000034 RID: 52
	public class ParserSharedInputState
	{
		// Token: 0x06000236 RID: 566 RVA: 0x00007AD0 File Offset: 0x00005CD0
		public virtual void reset()
		{
			this.guessing = 0;
			this.filename = null;
			this.input.reset();
		}

		// Token: 0x040000AD RID: 173
		protected internal TokenBuffer input;

		// Token: 0x040000AE RID: 174
		public int guessing = 0;

		// Token: 0x040000AF RID: 175
		protected internal string filename;
	}
}
