using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x0200000C RID: 12
	public struct ASTPair
	{
		// Token: 0x06000079 RID: 121 RVA: 0x000034A8 File Offset: 0x000016A8
		public void advanceChildToEnd()
		{
			if (child != null)
			{
				while (child.getNextSibling() != null)
				{
					child = child.getNextSibling();
				}
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000034E0 File Offset: 0x000016E0
		public ASTPair copy()
		{
			return new ASTPair
			{
				root = root,
				child = child
			};
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00003510 File Offset: 0x00001710
		private void reset()
		{
			root = null;
			child = null;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000352C File Offset: 0x0000172C
		public override string ToString()
		{
			var text = (root == null) ? "null" : root.getText();
			var text2 = (child == null) ? "null" : child.getText();
			return string.Concat(new string[]
			{
				"[",
				text,
				",",
				text2,
				"]"
			});
		}

		// Token: 0x0400001E RID: 30
		public AST root;

		// Token: 0x0400001F RID: 31
		public AST child;
	}
}
