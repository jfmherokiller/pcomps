namespace pcomps.Antlr.collections.impl
{
	// Token: 0x0200004C RID: 76
	public class ASTArray
	{
		// Token: 0x060002D5 RID: 725 RVA: 0x00009514 File Offset: 0x00007714
		public ASTArray(int capacity)
		{
			this.array = new AST[capacity];
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000953C File Offset: 0x0000773C
		public virtual ASTArray add(AST node)
		{
			this.array[this.size++] = node;
			return this;
		}

		// Token: 0x040000DE RID: 222
		public int size = 0;

		// Token: 0x040000DF RID: 223
		public AST[] array;
	}
}
