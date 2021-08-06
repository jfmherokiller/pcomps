namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000DE RID: 222
	public interface IToken
	{
		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000917 RID: 2327
		// (set) Token: 0x06000918 RID: 2328
		int Type { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000919 RID: 2329
		// (set) Token: 0x0600091A RID: 2330
		int Line { get; set; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600091B RID: 2331
		// (set) Token: 0x0600091C RID: 2332
		int CharPositionInLine { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600091D RID: 2333
		// (set) Token: 0x0600091E RID: 2334
		int Channel { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600091F RID: 2335
		// (set) Token: 0x06000920 RID: 2336
		int TokenIndex { get; set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000921 RID: 2337
		// (set) Token: 0x06000922 RID: 2338
		string Text { get; set; }
	}
}
