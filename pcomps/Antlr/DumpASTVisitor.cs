using System;
using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x02000028 RID: 40
	public class DumpASTVisitor : ASTVisitor
	{
		// Token: 0x060001A5 RID: 421 RVA: 0x0000602C File Offset: 0x0000422C
		private void tabs()
		{
			for (int i = 0; i < level; i++)
			{
				Console.Out.Write("   ");
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000605C File Offset: 0x0000425C
		public void visit(AST node)
		{
			bool flag = false;
			for (AST ast = node; ast != null; ast = ast.getNextSibling())
			{
				if (ast.getFirstChild() != null)
				{
					flag = false;
					break;
				}
			}
			for (AST ast = node; ast != null; ast = ast.getNextSibling())
			{
				if (!flag || ast == node)
				{
					tabs();
				}
				if (ast.getText() == null)
				{
					Console.Out.Write("nil");
				}
				else
				{
					Console.Out.Write(ast.getText());
				}
				Console.Out.Write($" [{ast.Type}] ");
				if (flag)
				{
					Console.Out.Write(" ");
				}
				else
				{
					Console.Out.WriteLine("");
				}
				if (ast.getFirstChild() != null)
				{
					level++;
					visit(ast.getFirstChild());
					level--;
				}
			}
			if (flag)
			{
				Console.Out.WriteLine("");
			}
		}

		// Token: 0x0400006B RID: 107
		protected int level = 0;
	}
}
