namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000DF RID: 223
	public interface ITokenStream : IIntStream
	{
		// Token: 0x06000923 RID: 2339
		IToken LT(int k);

		// Token: 0x06000924 RID: 2340
		IToken Get(int i);

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000925 RID: 2341
		ITokenSource TokenSource { get; }

		// Token: 0x06000926 RID: 2342
		string ToString(int start, int stop);

		// Token: 0x06000927 RID: 2343
		string ToString(IToken start, IToken stop);
	}
}
