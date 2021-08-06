using System;

namespace pcomps.Antlr
{
	// Token: 0x0200002D RID: 45
	[Serializable]
	public class RecognitionException : ANTLRException
	{
		// Token: 0x06000219 RID: 537 RVA: 0x0000703C File Offset: 0x0000523C
		public RecognitionException() : base("parsing error")
		{
			this.fileName = null;
			this.line = -1;
			this.column = -1;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000706C File Offset: 0x0000526C
		public RecognitionException(string s) : base(s)
		{
			this.fileName = null;
			this.line = -1;
			this.column = -1;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00007098 File Offset: 0x00005298
		public RecognitionException(string s, string fileName_, int line_, int column_) : base(s)
		{
			this.fileName = fileName_;
			this.line = line_;
			this.column = column_;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x000070C4 File Offset: 0x000052C4
		public virtual string getFilename()
		{
			return this.fileName;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x000070D8 File Offset: 0x000052D8
		public virtual int getLine()
		{
			return this.line;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x000070EC File Offset: 0x000052EC
		public virtual int getColumn()
		{
			return this.column;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00007100 File Offset: 0x00005300
		[Obsolete("Replaced by Message property since version 2.7.0", true)]
		public virtual string getErrorMessage()
		{
			return this.Message;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00007114 File Offset: 0x00005314
		public override string ToString()
		{
			return FileLineFormatter.getFormatter().getFormatString(this.fileName, this.line, this.column) + this.Message;
		}

		// Token: 0x0400008B RID: 139
		public string fileName;

		// Token: 0x0400008C RID: 140
		public int line;

		// Token: 0x0400008D RID: 141
		public int column;
	}
}
