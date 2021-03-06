using System.Text;
using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x02000036 RID: 54
	public class ParseTreeRule : ParseTree
	{
		// Token: 0x0600023F RID: 575 RVA: 0x00007C00 File Offset: 0x00005E00
		public ParseTreeRule(string ruleName) : this(ruleName, -1)
		{
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00007C18 File Offset: 0x00005E18
		public ParseTreeRule(string ruleName, int altNumber)
		{
			this.ruleName = ruleName;
			this.altNumber = altNumber;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00007C3C File Offset: 0x00005E3C
		public string getRuleName()
		{
			return ruleName;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00007C50 File Offset: 0x00005E50
		protected internal override int getLeftmostDerivation(StringBuilder buf, int step)
		{
			var num = 0;
			if (step <= 0)
			{
				buf.Append(' ');
				buf.Append(ToString());
				return num;
			}
			var ast = getFirstChild();
			num = 1;
			while (ast != null)
			{
				if (num >= step || ast is ParseTreeToken)
				{
					buf.Append(' ');
					buf.Append(ast.ToString());
				}
				else
				{
					var step2 = step - num;
					var leftmostDerivation = ((ParseTree)ast).getLeftmostDerivation(buf, step2);
					num += leftmostDerivation;
				}
				ast = ast.getNextSibling();
			}
			return num;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00007CCC File Offset: 0x00005ECC
		public override string ToString()
		{
			if (altNumber == -1)
			{
				return $"<{ruleName}>";
			}
			return string.Concat('<', ruleName, "[", altNumber, "]>");
		}

		// Token: 0x040000B0 RID: 176
		public const int INVALID_ALT = -1;

		// Token: 0x040000B1 RID: 177
		protected string ruleName;

		// Token: 0x040000B2 RID: 178
		protected int altNumber;
	}
}
