namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000248 RID: 584
	public class StringRef : Expr
	{
		// Token: 0x060011B4 RID: 4532 RVA: 0x00082224 File Offset: 0x00080424
		public StringRef(StringTemplate enclosingTemplate, string str) : base(enclosingTemplate)
		{
			this.str = str;
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00082234 File Offset: 0x00080434
		public override int Write(StringTemplate self, IStringTemplateWriter output)
		{
			if (str != null)
			{
				return output.Write(str);
			}
			return 0;
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x0008225C File Offset: 0x0008045C
		public override string ToString()
		{
			if (str != null)
			{
				return str;
			}
			return "";
		}

		// Token: 0x04000EF9 RID: 3833
		private string str;
	}
}
