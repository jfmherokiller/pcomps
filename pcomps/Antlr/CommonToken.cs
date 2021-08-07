namespace pcomps.Antlr
{
	// Token: 0x02000021 RID: 33
	public class CommonToken : Token
	{
		// Token: 0x06000181 RID: 385 RVA: 0x00005CA8 File Offset: 0x00003EA8
		public CommonToken()
		{
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00005CC4 File Offset: 0x00003EC4
		public CommonToken(int t, string txt)
		{
			type_ = t;
			setText(txt);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00005CEC File Offset: 0x00003EEC
		public CommonToken(string s)
		{
			text = s;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00005D10 File Offset: 0x00003F10
		public override int getLine()
		{
			return line;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00005D24 File Offset: 0x00003F24
		public override string getText()
		{
			return text;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00005D38 File Offset: 0x00003F38
		public override void setLine(int l)
		{
			line = l;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00005D4C File Offset: 0x00003F4C
		public override void setText(string s)
		{
			text = s;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00005D60 File Offset: 0x00003F60
		public override string ToString()
		{
			return $"[\"{getText()}\",<{type_}>,line={line},col={col}]";
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00005DD8 File Offset: 0x00003FD8
		public override int getColumn()
		{
			return col;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00005DEC File Offset: 0x00003FEC
		public override void setColumn(int c)
		{
			col = c;
		}

		// Token: 0x04000063 RID: 99
		public static readonly CommonTokenCreator Creator = new();

		// Token: 0x04000064 RID: 100
		protected internal int line;

		// Token: 0x04000065 RID: 101
		protected internal string text = null;

		// Token: 0x04000066 RID: 102
		protected internal int col;

		// Token: 0x02000022 RID: 34
		public class CommonTokenCreator : TokenCreator
		{
			// Token: 0x17000019 RID: 25
			// (get) Token: 0x0600018D RID: 397 RVA: 0x00005E2C File Offset: 0x0000402C
			public override string TokenTypeName => typeof(CommonToken).FullName;

            // Token: 0x0600018E RID: 398 RVA: 0x00005E48 File Offset: 0x00004048
			public override IToken Create() => new CommonToken();
        }
	}
}
