namespace pcomps.Antlr
{
	// Token: 0x02000023 RID: 35
	public interface IHiddenStreamToken : IToken
	{
		// Token: 0x0600018F RID: 399
		IHiddenStreamToken getHiddenAfter();

		// Token: 0x06000190 RID: 400
		void setHiddenAfter(IHiddenStreamToken t);

		// Token: 0x06000191 RID: 401
		IHiddenStreamToken getHiddenBefore();

		// Token: 0x06000192 RID: 402
		void setHiddenBefore(IHiddenStreamToken t);
	}
}
