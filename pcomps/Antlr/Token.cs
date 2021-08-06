namespace pcomps.Antlr
{
	// Token: 0x02000020 RID: 32
	public class Token : IToken
	{
		// Token: 0x06000171 RID: 369 RVA: 0x00005B1C File Offset: 0x00003D1C
		public Token()
		{
			this.type_ = 0;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00005B38 File Offset: 0x00003D38
		public Token(int t)
		{
			this.type_ = t;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00005B54 File Offset: 0x00003D54
		public Token(int t, string txt)
		{
			this.type_ = t;
			this.setText(txt);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00005B78 File Offset: 0x00003D78
		public virtual int getColumn()
		{
			return 0;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00005B88 File Offset: 0x00003D88
		public virtual int getLine()
		{
			return 0;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00005B98 File Offset: 0x00003D98
		public virtual string getFilename()
		{
			return null;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00005BA8 File Offset: 0x00003DA8
		public virtual void setFilename(string name)
		{
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00005BB8 File Offset: 0x00003DB8
		public virtual string getText()
		{
			return "<no text>";
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00005BCC File Offset: 0x00003DCC
		// (set) Token: 0x0600017A RID: 378 RVA: 0x00005BE0 File Offset: 0x00003DE0
		public int Type
		{
			get
			{
				return this.type_;
			}
			set
			{
				this.type_ = value;
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00005BF4 File Offset: 0x00003DF4
		public virtual void setType(int newType)
		{
			this.Type = newType;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00005C08 File Offset: 0x00003E08
		public virtual void setColumn(int c)
		{
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00005C18 File Offset: 0x00003E18
		public virtual void setLine(int l)
		{
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00005C28 File Offset: 0x00003E28
		public virtual void setText(string t)
		{
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00005C38 File Offset: 0x00003E38
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[\"",
				this.getText(),
				"\",<",
				this.type_,
				">]"
			});
		}

		// Token: 0x0400005C RID: 92
		public const int MIN_USER_TYPE = 4;

		// Token: 0x0400005D RID: 93
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x0400005E RID: 94
		public const int INVALID_TYPE = 0;

		// Token: 0x0400005F RID: 95
		public const int EOF_TYPE = 1;

		// Token: 0x04000060 RID: 96
		public static readonly int SKIP = -1;

		// Token: 0x04000061 RID: 97
		protected int type_;

		// Token: 0x04000062 RID: 98
		public static Token badToken = new Token(0, "<no text>");
	}
}
