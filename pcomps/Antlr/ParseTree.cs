using System.Text;
using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x02000035 RID: 53
	public abstract class ParseTree : BaseAST
	{
		// Token: 0x06000238 RID: 568 RVA: 0x00007B14 File Offset: 0x00005D14
		public string getLeftmostDerivationStep(int step)
		{
			if (step <= 0)
			{
				return this.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder(2000);
			this.getLeftmostDerivation(stringBuilder, step);
			return stringBuilder.ToString();
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00007B48 File Offset: 0x00005D48
		public string getLeftmostDerivation(int maxSteps)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			stringBuilder.Append("    " + this.ToString());
			stringBuilder.Append("\n");
			for (int i = 1; i < maxSteps; i++)
			{
				stringBuilder.Append(" =>");
				stringBuilder.Append(this.getLeftmostDerivationStep(i));
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600023A RID: 570
		protected internal abstract int getLeftmostDerivation(StringBuilder buf, int step);

		// Token: 0x0600023B RID: 571 RVA: 0x00007BBC File Offset: 0x00005DBC
		public override void initialize(int i, string s)
		{
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00007BCC File Offset: 0x00005DCC
		public override void initialize(AST ast)
		{
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00007BDC File Offset: 0x00005DDC
		public override void initialize(IToken token)
		{
		}
	}
}
