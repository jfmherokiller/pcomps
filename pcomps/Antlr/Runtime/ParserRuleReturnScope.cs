namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000093 RID: 147
	public class ParserRuleReturnScope : RuleReturnScope
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x0000FEFC File Offset: 0x0000E0FC
		// (set) Token: 0x0600055C RID: 1372 RVA: 0x0000FF04 File Offset: 0x0000E104
		public override object Start
		{
			get
			{
				return start;
			}
			set
			{
				start = (IToken)value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600055D RID: 1373 RVA: 0x0000FF14 File Offset: 0x0000E114
		// (set) Token: 0x0600055E RID: 1374 RVA: 0x0000FF1C File Offset: 0x0000E11C
		public override object Stop
		{
			get
			{
				return stop;
			}
			set
			{
				stop = (IToken)value;
			}
		}

		// Token: 0x04000174 RID: 372
		private IToken start;

		// Token: 0x04000175 RID: 373
		private IToken stop;
	}
}
