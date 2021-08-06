using System;
using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x02000032 RID: 50
	[Serializable]
	public class NoViableAltException : RecognitionException
	{
		// Token: 0x06000230 RID: 560 RVA: 0x0000794C File Offset: 0x00005B4C
		public NoViableAltException(AST t) : base("NoViableAlt", "<AST>", -1, -1)
		{
			node = t;
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00007974 File Offset: 0x00005B74
		public NoViableAltException(IToken t, string fileName_) : base("NoViableAlt", fileName_, t.getLine(), t.getColumn())
		{
			token = t;
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000232 RID: 562 RVA: 0x000079A0 File Offset: 0x00005BA0
		public override string Message
		{
			get
			{
				if (token != null)
				{
					return $"unexpected token: {token}";
				}
				if (node == null || node == TreeParser.ASTNULL)
				{
					return "unexpected end of subtree";
				}
				return $"unexpected AST node: {node.ToString()}";
			}
		}

		// Token: 0x040000AA RID: 170
		public IToken token;

		// Token: 0x040000AB RID: 171
		public AST node;
	}
}
