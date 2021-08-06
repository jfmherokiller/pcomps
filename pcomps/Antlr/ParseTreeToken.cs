using System.Text;

namespace pcomps.Antlr
{
	// Token: 0x02000037 RID: 55
	public class ParseTreeToken : ParseTree
	{
		// Token: 0x06000244 RID: 580 RVA: 0x00007D3C File Offset: 0x00005F3C
		public ParseTreeToken(IToken token)
		{
			this.token = token;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00007D58 File Offset: 0x00005F58
		protected internal override int getLeftmostDerivation(StringBuilder buf, int step)
		{
			buf.Append(' ');
			buf.Append(ToString());
			return step;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00007D7C File Offset: 0x00005F7C
		public override string ToString()
		{
			if (token != null)
			{
				return token.getText();
			}
			return "<missing token>";
		}

		// Token: 0x040000B3 RID: 179
		protected IToken token;
	}
}
