using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr
{
	// Token: 0x0200003E RID: 62
	public class TokenStreamHiddenTokenFilter : TokenStreamBasicFilter, TokenStream
	{
		// Token: 0x06000267 RID: 615 RVA: 0x00008368 File Offset: 0x00006568
		public TokenStreamHiddenTokenFilter(TokenStream input) : base(input)
		{
			this.hideMask = new BitSet();
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00008390 File Offset: 0x00006590
		protected internal virtual void consume()
		{
			this.nextMonitoredToken = (IHiddenStreamToken)this.input.nextToken();
		}

		// Token: 0x06000269 RID: 617 RVA: 0x000083B4 File Offset: 0x000065B4
		private void consumeFirst()
		{
			this.consume();
			IHiddenStreamToken hiddenStreamToken = null;
			while (this.hideMask.member(this.LA(1).Type) || this.discardMask.member(this.LA(1).Type))
			{
				if (this.hideMask.member(this.LA(1).Type))
				{
					if (hiddenStreamToken == null)
					{
						hiddenStreamToken = this.LA(1);
					}
					else
					{
						hiddenStreamToken.setHiddenAfter(this.LA(1));
						this.LA(1).setHiddenBefore(hiddenStreamToken);
						hiddenStreamToken = this.LA(1);
					}
					this.lastHiddenToken = hiddenStreamToken;
					if (this.firstHidden == null)
					{
						this.firstHidden = hiddenStreamToken;
					}
				}
				this.consume();
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00008464 File Offset: 0x00006664
		public virtual BitSet getDiscardMask()
		{
			return this.discardMask;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00008478 File Offset: 0x00006678
		public virtual IHiddenStreamToken getHiddenAfter(IHiddenStreamToken t)
		{
			return t.getHiddenAfter();
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000848C File Offset: 0x0000668C
		public virtual IHiddenStreamToken getHiddenBefore(IHiddenStreamToken t)
		{
			return t.getHiddenBefore();
		}

		// Token: 0x0600026D RID: 621 RVA: 0x000084A0 File Offset: 0x000066A0
		public virtual BitSet getHideMask()
		{
			return this.hideMask;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x000084B4 File Offset: 0x000066B4
		public virtual IHiddenStreamToken getInitialHiddenToken()
		{
			return this.firstHidden;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x000084C8 File Offset: 0x000066C8
		public virtual void hide(int m)
		{
			this.hideMask.add(m);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x000084E4 File Offset: 0x000066E4
		public virtual void hide(BitSet mask)
		{
			this.hideMask = mask;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x000084F8 File Offset: 0x000066F8
		protected internal virtual IHiddenStreamToken LA(int i)
		{
			return this.nextMonitoredToken;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000850C File Offset: 0x0000670C
		public override IToken nextToken()
		{
			if (this.LA(1) == null)
			{
				this.consumeFirst();
			}
			IHiddenStreamToken hiddenStreamToken = this.LA(1);
			hiddenStreamToken.setHiddenBefore(this.lastHiddenToken);
			this.lastHiddenToken = null;
			this.consume();
			IHiddenStreamToken hiddenStreamToken2 = hiddenStreamToken;
			while (this.hideMask.member(this.LA(1).Type) || this.discardMask.member(this.LA(1).Type))
			{
				if (this.hideMask.member(this.LA(1).Type))
				{
					hiddenStreamToken2.setHiddenAfter(this.LA(1));
					if (hiddenStreamToken2 != hiddenStreamToken)
					{
						this.LA(1).setHiddenBefore(hiddenStreamToken2);
					}
					hiddenStreamToken2 = (this.lastHiddenToken = this.LA(1));
				}
				this.consume();
			}
			return hiddenStreamToken;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x000085D0 File Offset: 0x000067D0
		public virtual void resetState()
		{
			this.firstHidden = null;
			this.lastHiddenToken = null;
			this.nextMonitoredToken = null;
		}

		// Token: 0x040000BF RID: 191
		protected internal BitSet hideMask;

		// Token: 0x040000C0 RID: 192
		private IHiddenStreamToken nextMonitoredToken;

		// Token: 0x040000C1 RID: 193
		protected internal IHiddenStreamToken lastHiddenToken;

		// Token: 0x040000C2 RID: 194
		protected internal IHiddenStreamToken firstHidden = null;
	}
}
