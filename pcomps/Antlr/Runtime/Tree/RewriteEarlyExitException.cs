using System;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000D1 RID: 209
	[Serializable]
	public class RewriteEarlyExitException : RewriteCardinalityException
	{
		// Token: 0x06000870 RID: 2160 RVA: 0x0001806C File Offset: 0x0001626C
		public RewriteEarlyExitException() : base(null)
		{
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x00018078 File Offset: 0x00016278
		public RewriteEarlyExitException(string elementDescription) : base(elementDescription)
		{
		}
	}
}
