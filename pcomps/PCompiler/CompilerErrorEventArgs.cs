using System;

namespace pcomps.PCompiler
{
	// Token: 0x0200020D RID: 525
	public class CompilerErrorEventArgs : EventArgs
	{
		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x0006D9E8 File Offset: 0x0006BBE8
		public string Message
		{
			get
			{
				return this.sError;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000EE9 RID: 3817 RVA: 0x0006D9F0 File Offset: 0x0006BBF0
		public bool FilenameValid
		{
			get
			{
				return this.bHasFileInformation;
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000EEA RID: 3818 RVA: 0x0006D9F8 File Offset: 0x0006BBF8
		public string Filename
		{
			get
			{
				string result = this.sFilename;
				if (!this.FilenameValid)
				{
					result = "<unknown>";
				}
				return result;
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000EEB RID: 3819 RVA: 0x0006DA1C File Offset: 0x0006BC1C
		public bool LineInformationValid
		{
			get
			{
				return this.bHasFileInformation && this.bHasLineInformation;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000EEC RID: 3820 RVA: 0x0006DA30 File Offset: 0x0006BC30
		public int LineNumber
		{
			get
			{
				int result = this.iLineNumber;
				if (!this.LineInformationValid)
				{
					result = 0;
				}
				return result;
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000EED RID: 3821 RVA: 0x0006DA50 File Offset: 0x0006BC50
		public int ColumnNumber
		{
			get
			{
				int result = this.iColumnNumber;
				if (!this.LineInformationValid)
				{
					result = 0;
				}
				return result;
			}
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x0006DA70 File Offset: 0x0006BC70
		public CompilerErrorEventArgs(string asErrorText)
		{
			this.sError = asErrorText;
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x0006DA98 File Offset: 0x0006BC98
		public CompilerErrorEventArgs(string asErrorText, string asFilename)
		{
			this.sError = asErrorText;
			this.bHasFileInformation = true;
			this.sFilename = asFilename;
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x0006DACC File Offset: 0x0006BCCC
		public CompilerErrorEventArgs(string asErrorText, string asFilename, int aiLineNumber, int aiColumnNumber)
		{
			this.sError = asErrorText;
			this.bHasFileInformation = true;
			this.sFilename = asFilename;
			this.bHasLineInformation = true;
			this.iLineNumber = aiLineNumber;
			this.iColumnNumber = aiColumnNumber;
		}

		// Token: 0x04000CAB RID: 3243
		private readonly string sError = "";

		// Token: 0x04000CAC RID: 3244
		private readonly bool bHasFileInformation;

		// Token: 0x04000CAD RID: 3245
		private readonly string sFilename = "";

		// Token: 0x04000CAE RID: 3246
		private readonly bool bHasLineInformation;

		// Token: 0x04000CAF RID: 3247
		private readonly int iLineNumber;

		// Token: 0x04000CB0 RID: 3248
		private readonly int iColumnNumber;
	}
}
