namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000083 RID: 131
	public interface ICharStream : IIntStream
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060004E8 RID: 1256
		// (set) Token: 0x060004E9 RID: 1257
		int Line { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060004EA RID: 1258
		// (set) Token: 0x060004EB RID: 1259
		int CharPositionInLine { get; set; }

		// Token: 0x060004EC RID: 1260
		int LT(int i);

		// Token: 0x060004ED RID: 1261
		string Substring(int start, int stop);
	}
}
