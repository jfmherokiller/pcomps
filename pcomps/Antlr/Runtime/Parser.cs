namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000092 RID: 146
	public class Parser : BaseRecognizer
	{
		// Token: 0x0600054F RID: 1359 RVA: 0x0000FDAC File Offset: 0x0000DFAC
		public Parser(ITokenStream input)
		{
			this.TokenStream = input;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x0000FDBC File Offset: 0x0000DFBC
		public Parser(ITokenStream input, RecognizerSharedState state) : base(state)
		{
			this.TokenStream = input;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0000FDCC File Offset: 0x0000DFCC
		public override void Reset()
		{
			base.Reset();
			if (this.input != null)
			{
				this.input.Seek(0);
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0000FDEC File Offset: 0x0000DFEC
		protected override object GetCurrentInputSymbol(IIntStream input)
		{
			return ((ITokenStream)input).LT(1);
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0000FDFC File Offset: 0x0000DFFC
		protected override object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow)
		{
			string text;
			if (expectedTokenType == Token.EOF)
			{
				text = "<missing EOF>";
			}
			else
			{
				text = "<missing " + this.TokenNames[expectedTokenType] + ">";
			}
			CommonToken commonToken = new CommonToken(expectedTokenType, text);
			IToken token = ((ITokenStream)input).LT(1);
			if (token.Type == Token.EOF)
			{
				token = ((ITokenStream)input).LT(-1);
			}
			commonToken.line = token.Line;
			commonToken.CharPositionInLine = token.CharPositionInLine;
			commonToken.Channel = 0;
			return commonToken;
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000554 RID: 1364 RVA: 0x0000FE8C File Offset: 0x0000E08C
		// (set) Token: 0x06000555 RID: 1365 RVA: 0x0000FE94 File Offset: 0x0000E094
		public virtual ITokenStream TokenStream
		{
			get
			{
				return this.input;
			}
			set
			{
				this.input = null;
				this.Reset();
				this.input = value;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x0000FEAC File Offset: 0x0000E0AC
		public override string SourceName
		{
			get
			{
				return this.input.SourceName;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x0000FEBC File Offset: 0x0000E0BC
		public override IIntStream Input
		{
			get
			{
				return this.input;
			}
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x0000FEC4 File Offset: 0x0000E0C4
		public virtual void TraceIn(string ruleName, int ruleIndex)
		{
			base.TraceIn(ruleName, ruleIndex, this.input.LT(1));
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x0000FEDC File Offset: 0x0000E0DC
		public virtual void TraceOut(string ruleName, int ruleIndex)
		{
			base.TraceOut(ruleName, ruleIndex, this.input.LT(1));
		}

		// Token: 0x04000173 RID: 371
		protected internal ITokenStream input;
	}
}
