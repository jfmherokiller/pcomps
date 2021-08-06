namespace pcomps.Antlr
{
	// Token: 0x02000024 RID: 36
	public class CommonHiddenStreamToken : CommonToken, IHiddenStreamToken
	{
		// Token: 0x06000193 RID: 403 RVA: 0x00005E5C File Offset: 0x0000405C
		public CommonHiddenStreamToken()
		{
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00005E70 File Offset: 0x00004070
		public CommonHiddenStreamToken(int t, string txt) : base(t, txt)
		{
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00005E88 File Offset: 0x00004088
		public CommonHiddenStreamToken(string s) : base(s)
		{
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00005E9C File Offset: 0x0000409C
		public virtual IHiddenStreamToken getHiddenAfter()
		{
			return this.hiddenAfter;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00005EB0 File Offset: 0x000040B0
		public virtual IHiddenStreamToken getHiddenBefore()
		{
			return this.hiddenBefore;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00005EC4 File Offset: 0x000040C4
		public virtual void setHiddenAfter(IHiddenStreamToken t)
		{
			this.hiddenAfter = t;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00005ED8 File Offset: 0x000040D8
		public virtual void setHiddenBefore(IHiddenStreamToken t)
		{
			this.hiddenBefore = t;
		}

		// Token: 0x04000067 RID: 103
		public new static readonly CommonHiddenStreamToken.CommonHiddenStreamTokenCreator Creator = new CommonHiddenStreamToken.CommonHiddenStreamTokenCreator();

		// Token: 0x04000068 RID: 104
		protected internal IHiddenStreamToken hiddenBefore;

		// Token: 0x04000069 RID: 105
		protected internal IHiddenStreamToken hiddenAfter;

		// Token: 0x02000025 RID: 37
		public class CommonHiddenStreamTokenCreator : TokenCreator
		{
			// Token: 0x1700001A RID: 26
			// (get) Token: 0x0600019C RID: 412 RVA: 0x00005F18 File Offset: 0x00004118
			public override string TokenTypeName
			{
				get
				{
					return typeof(CommonHiddenStreamToken).FullName;
				}
			}

			// Token: 0x0600019D RID: 413 RVA: 0x00005F34 File Offset: 0x00004134
			public override IToken Create()
			{
				return new CommonHiddenStreamToken();
			}
		}
	}
}
