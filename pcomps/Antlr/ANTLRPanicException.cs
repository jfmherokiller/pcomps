using System;

namespace pcomps.Antlr
{
	// Token: 0x02000006 RID: 6
	[Serializable]
	public class ANTLRPanicException : ANTLRException
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002AD0 File Offset: 0x00000CD0
		public ANTLRPanicException()
		{
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002AE4 File Offset: 0x00000CE4
		public ANTLRPanicException(string s) : base(s)
		{
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002AF8 File Offset: 0x00000CF8
		public ANTLRPanicException(string s, Exception inner) : base(s, inner)
		{
		}
	}
}
