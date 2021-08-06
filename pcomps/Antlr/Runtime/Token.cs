namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000096 RID: 150
	public static class Token
	{
		// Token: 0x0400017E RID: 382
		public const int EOR_TOKEN_TYPE = 1;

		// Token: 0x0400017F RID: 383
		public const int DOWN = 2;

		// Token: 0x04000180 RID: 384
		public const int UP = 3;

		// Token: 0x04000181 RID: 385
		public const int INVALID_TOKEN_TYPE = 0;

		// Token: 0x04000182 RID: 386
		public const int DEFAULT_CHANNEL = 0;

		// Token: 0x04000183 RID: 387
		public const int HIDDEN_CHANNEL = 99;

		// Token: 0x04000184 RID: 388
		public static readonly int MIN_TOKEN_TYPE = 4;

		// Token: 0x04000185 RID: 389
		public static readonly int EOF = -1;

		// Token: 0x04000186 RID: 390
		public static readonly IToken EOF_TOKEN = new CommonToken(Token.EOF);

		// Token: 0x04000187 RID: 391
		public static readonly IToken INVALID_TOKEN = new CommonToken(0);

		// Token: 0x04000188 RID: 392
		public static readonly IToken SKIP_TOKEN = new CommonToken(0);
	}
}
