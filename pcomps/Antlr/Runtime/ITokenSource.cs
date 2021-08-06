namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000097 RID: 151
	public interface ITokenSource
	{
		// Token: 0x0600057E RID: 1406
		IToken NextToken();

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600057F RID: 1407
		string SourceName { get; }
	}
}
