using System;

namespace pcomps.Antlr
{
	// Token: 0x02000038 RID: 56
	[Serializable]
	public class SemanticException : RecognitionException
	{
		// Token: 0x06000247 RID: 583 RVA: 0x00007DA4 File Offset: 0x00005FA4
		public SemanticException(string s) : base(s)
		{
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00007DB8 File Offset: 0x00005FB8
		[Obsolete("Replaced by SemanticException(string, string, int, int) since version 2.7.2.6", false)]
		public SemanticException(string s, string fileName, int line) : this(s, fileName, line, -1)
		{
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00007DD0 File Offset: 0x00005FD0
		public SemanticException(string s, string fileName, int line, int column) : base(s, fileName, line, column)
		{
		}
	}
}
