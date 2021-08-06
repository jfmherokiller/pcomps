namespace pcomps.Antlr
{
	// Token: 0x02000026 RID: 38
	public abstract class FileLineFormatter
	{
		// Token: 0x0600019E RID: 414 RVA: 0x00005F48 File Offset: 0x00004148
		public static FileLineFormatter getFormatter()
		{
			return formatter;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00005F5C File Offset: 0x0000415C
		public static void setFormatter(FileLineFormatter f)
		{
			formatter = f;
		}

		// Token: 0x060001A0 RID: 416
		public abstract string getFormatString(string fileName, int line, int column);

		// Token: 0x0400006A RID: 106
		private static FileLineFormatter formatter = new DefaultFileLineFormatter();
	}
}
