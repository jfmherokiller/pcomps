using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x02000009 RID: 9
	public abstract class ASTNodeCreator
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000040 RID: 64
		public abstract string ASTNodeTypeName { get; }

		// Token: 0x06000041 RID: 65
		public abstract AST Create();
	}
}
