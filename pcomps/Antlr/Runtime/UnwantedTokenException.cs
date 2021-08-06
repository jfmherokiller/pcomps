namespace pcomps.Antlr.Runtime
{
	// Token: 0x0200009A RID: 154
	public class UnwantedTokenException : MismatchedTokenException
	{
		// Token: 0x0600058B RID: 1419 RVA: 0x00010614 File Offset: 0x0000E814
		public UnwantedTokenException()
		{
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x0001061C File Offset: 0x0000E81C
		public UnwantedTokenException(int expecting, IIntStream input) : base(expecting, input)
		{
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x00010628 File Offset: 0x0000E828
		public IToken UnexpectedToken
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00010630 File Offset: 0x0000E830
		public override string ToString()
		{
			string str = $", expected {base.Expecting}";
			if (base.Expecting == 0)
			{
				str = string.Empty;
			}
			if (this.token == null)
			{
				return $"UnwantedTokenException(found={null}{str})";
			}
			return $"UnwantedTokenException(found={this.token.Text}{str})";
		}
	}
}
