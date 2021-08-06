namespace pcomps.Antlr
{
	// Token: 0x02000018 RID: 24
	public abstract class TokenCreator
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000146 RID: 326
		public abstract string TokenTypeName { get; }

		// Token: 0x06000147 RID: 327
		public abstract IToken Create();
	}
}
