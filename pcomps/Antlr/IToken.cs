namespace pcomps.Antlr
{
	// Token: 0x0200001F RID: 31
	public interface IToken
	{
		// Token: 0x06000167 RID: 359
		int getColumn();

		// Token: 0x06000168 RID: 360
		void setColumn(int c);

		// Token: 0x06000169 RID: 361
		int getLine();

		// Token: 0x0600016A RID: 362
		void setLine(int l);

		// Token: 0x0600016B RID: 363
		string getFilename();

		// Token: 0x0600016C RID: 364
		void setFilename(string name);

		// Token: 0x0600016D RID: 365
		string getText();

		// Token: 0x0600016E RID: 366
		void setText(string t);

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600016F RID: 367
		// (set) Token: 0x06000170 RID: 368
		int Type { get; set; }
	}
}
