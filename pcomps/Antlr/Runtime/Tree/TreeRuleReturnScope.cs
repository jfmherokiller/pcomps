namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000AE RID: 174
	public class TreeRuleReturnScope : RuleReturnScope
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x00012D00 File Offset: 0x00010F00
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x00012D08 File Offset: 0x00010F08
		public override object Start
		{
			get
			{
				return this.start;
			}
			set
			{
				this.start = value;
			}
		}

		// Token: 0x040001C2 RID: 450
		private object start;
	}
}
