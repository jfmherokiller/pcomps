using System;

namespace pcomps.Antlr
{
	// Token: 0x02000005 RID: 5
	[Serializable]
	public class ANTLRException : Exception
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00002A90 File Offset: 0x00000C90
		public ANTLRException()
		{
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002AA4 File Offset: 0x00000CA4
		public ANTLRException(string s) : base(s)
		{
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002AB8 File Offset: 0x00000CB8
		public ANTLRException(string s, Exception inner) : base(s, inner)
		{
		}
	}
}
