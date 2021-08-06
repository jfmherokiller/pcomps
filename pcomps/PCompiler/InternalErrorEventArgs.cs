using System;

namespace pcomps.PCompiler
{
	// Token: 0x020001C2 RID: 450
	internal class InternalErrorEventArgs : EventArgs
	{
		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000D66 RID: 3430 RVA: 0x0005E03C File Offset: 0x0005C23C
		public string ErrorText
		{
			get
			{
				return this.sError;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000D67 RID: 3431 RVA: 0x0005E044 File Offset: 0x0005C244
		public int LineNumber
		{
			get
			{
				return this.iLineNumber;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000D68 RID: 3432 RVA: 0x0005E04C File Offset: 0x0005C24C
		public int ColumnNumber
		{
			get
			{
				return this.iColumnNumber;
			}
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x0005E054 File Offset: 0x0005C254
		public InternalErrorEventArgs(string asErrorText, int aiLineNumber, int aiColumnNumber)
		{
			this.sError = asErrorText;
			this.iLineNumber = aiLineNumber;
			this.iColumnNumber = aiColumnNumber;
		}

		// Token: 0x040009EA RID: 2538
		private readonly string sError = "";

		// Token: 0x040009EB RID: 2539
		private readonly int iLineNumber;

		// Token: 0x040009EC RID: 2540
		private readonly int iColumnNumber;
	}
}
