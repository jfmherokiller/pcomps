using System;
using pcomps.Antlr.Runtime.Tree;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000090 RID: 144
	[Serializable]
	public class MismatchedTreeNodeException : RecognitionException
	{
		// Token: 0x06000549 RID: 1353 RVA: 0x0000FC8C File Offset: 0x0000DE8C
		public MismatchedTreeNodeException()
		{
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0000FC94 File Offset: 0x0000DE94
		public MismatchedTreeNodeException(int expecting, ITreeNodeStream input) : base(input)
		{
			this.expecting = expecting;
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0000FCA4 File Offset: 0x0000DEA4
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"MismatchedTreeNodeException(",
				this.UnexpectedType,
				"!=",
				this.expecting,
				")"
			});
		}

		// Token: 0x0400016F RID: 367
		public int expecting;
	}
}
