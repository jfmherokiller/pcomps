using System;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000D2 RID: 210
	[Serializable]
	public class RewriteEmptyStreamException : RewriteCardinalityException
	{
		// Token: 0x06000872 RID: 2162 RVA: 0x00018084 File Offset: 0x00016284
		public RewriteEmptyStreamException(string elementDescription) : base(elementDescription)
		{
		}
	}
}
