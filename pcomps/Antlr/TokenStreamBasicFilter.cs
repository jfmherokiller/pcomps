using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr
{
	// Token: 0x0200003C RID: 60
	public class TokenStreamBasicFilter : TokenStream
	{
		// Token: 0x06000261 RID: 609 RVA: 0x000082A8 File Offset: 0x000064A8
		public TokenStreamBasicFilter(TokenStream input)
		{
			this.input = input;
			this.discardMask = new BitSet();
		}

		// Token: 0x06000262 RID: 610 RVA: 0x000082D0 File Offset: 0x000064D0
		public virtual void discard(int ttype)
		{
			this.discardMask.add(ttype);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x000082EC File Offset: 0x000064EC
		public virtual void discard(BitSet mask)
		{
			this.discardMask = mask;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x00008300 File Offset: 0x00006500
		public virtual IToken nextToken()
		{
			IToken token = this.input.nextToken();
			while (token != null && this.discardMask.member(token.Type))
			{
				token = this.input.nextToken();
			}
			return token;
		}

		// Token: 0x040000BD RID: 189
		protected internal BitSet discardMask;

		// Token: 0x040000BE RID: 190
		protected internal TokenStream input;
	}
}
