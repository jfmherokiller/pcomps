using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000E0 RID: 224
	public abstract class Lexer : BaseRecognizer, ITokenSource
	{
		// Token: 0x06000928 RID: 2344 RVA: 0x0001A424 File Offset: 0x00018624
		public Lexer()
		{
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x0001A42C File Offset: 0x0001862C
		public Lexer(ICharStream input)
		{
			this.input = input;
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x0001A43C File Offset: 0x0001863C
		public Lexer(ICharStream input, RecognizerSharedState state) : base(state)
		{
			this.input = input;
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x0600092B RID: 2347 RVA: 0x0001A44C File Offset: 0x0001864C
		// (set) Token: 0x0600092C RID: 2348 RVA: 0x0001A454 File Offset: 0x00018654
		public virtual ICharStream CharStream
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

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600092D RID: 2349 RVA: 0x0001A46C File Offset: 0x0001866C
		public override string SourceName
		{
			get
			{
				return this.input.SourceName;
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x0600092E RID: 2350 RVA: 0x0001A47C File Offset: 0x0001867C
		public override IIntStream Input
		{
			get
			{
				return this.input;
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x0001A484 File Offset: 0x00018684
		public virtual int Line
		{
			get
			{
				return this.input.Line;
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000930 RID: 2352 RVA: 0x0001A494 File Offset: 0x00018694
		public virtual int CharPositionInLine
		{
			get
			{
				return this.input.CharPositionInLine;
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x0001A4A4 File Offset: 0x000186A4
		public virtual int CharIndex
		{
			get
			{
				return this.input.Index();
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000932 RID: 2354 RVA: 0x0001A4B4 File Offset: 0x000186B4
		// (set) Token: 0x06000933 RID: 2355 RVA: 0x0001A4FC File Offset: 0x000186FC
		public virtual string Text
		{
			get
			{
				if (this.state.text != null)
				{
					return this.state.text;
				}
				return this.input.Substring(this.state.tokenStartCharIndex, this.CharIndex - 1);
			}
			set
			{
				this.state.text = value;
			}
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x0001A50C File Offset: 0x0001870C
		public override void Reset()
		{
			base.Reset();
			if (this.input != null)
			{
				this.input.Seek(0);
			}
			if (this.state == null)
			{
				return;
			}
			this.state.token = null;
			this.state.type = 0;
			this.state.channel = 0;
			this.state.tokenStartCharIndex = -1;
			this.state.tokenStartCharPositionInLine = -1;
			this.state.tokenStartLine = -1;
			this.state.text = null;
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x0001A598 File Offset: 0x00018798
		public virtual IToken NextToken()
		{
			for (;;)
			{
				this.state.token = null;
				this.state.channel = 0;
				this.state.tokenStartCharIndex = this.input.Index();
				this.state.tokenStartCharPositionInLine = this.input.CharPositionInLine;
				this.state.tokenStartLine = this.input.Line;
				this.state.text = null;
				if (this.input.LA(1) == -1)
				{
					break;
				}
				try
				{
					this.mTokens();
					if (this.state.token == null)
					{
						this.Emit();
					}
					else if (this.state.token == Token.SKIP_TOKEN)
					{
						continue;
					}
					return this.state.token;
				}
				catch (NoViableAltException ex)
				{
					this.ReportError(ex);
					this.Recover(ex);
				}
				catch (RecognitionException e)
				{
					this.ReportError(e);
				}
			}
			return Token.EOF_TOKEN;
		}

		// Token: 0x06000936 RID: 2358 RVA: 0x0001A6B8 File Offset: 0x000188B8
		public void Skip()
		{
			this.state.token = Token.SKIP_TOKEN;
		}

		// Token: 0x06000937 RID: 2359
		public abstract void mTokens();

		// Token: 0x06000938 RID: 2360 RVA: 0x0001A6CC File Offset: 0x000188CC
		public virtual void Emit(IToken token)
		{
			this.state.token = token;
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0001A6DC File Offset: 0x000188DC
		public virtual IToken Emit()
		{
			IToken token = new CommonToken(this.input, this.state.type, this.state.channel, this.state.tokenStartCharIndex, this.CharIndex - 1);
			token.Line = this.state.tokenStartLine;
			token.Text = this.state.text;
			token.CharPositionInLine = this.state.tokenStartCharPositionInLine;
			this.Emit(token);
			return token;
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x0001A75C File Offset: 0x0001895C
		public virtual void Match(string s)
		{
			int i = 0;
			while (i < s.Length)
			{
				if (this.input.LA(1) != (int)s[i])
				{
					if (this.state.backtracking > 0)
					{
						this.state.failed = true;
						return;
					}
					MismatchedTokenException ex = new MismatchedTokenException((int)s[i], this.input);
					this.Recover(ex);
					throw ex;
				}
				else
				{
					i++;
					this.input.Consume();
					this.state.failed = false;
				}
			}
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x0001A7EC File Offset: 0x000189EC
		public virtual void MatchAny()
		{
			this.input.Consume();
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x0001A7FC File Offset: 0x000189FC
		public virtual void Match(int c)
		{
			if (this.input.LA(1) == c)
			{
				this.input.Consume();
				this.state.failed = false;
				return;
			}
			if (this.state.backtracking > 0)
			{
				this.state.failed = true;
				return;
			}
			MismatchedTokenException ex = new MismatchedTokenException(c, this.input);
			this.Recover(ex);
			throw ex;
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0001A868 File Offset: 0x00018A68
		public virtual void MatchRange(int a, int b)
		{
			if (this.input.LA(1) >= a && this.input.LA(1) <= b)
			{
				this.input.Consume();
				this.state.failed = false;
				return;
			}
			if (this.state.backtracking > 0)
			{
				this.state.failed = true;
				return;
			}
			MismatchedRangeException ex = new MismatchedRangeException(a, b, this.input);
			this.Recover(ex);
			throw ex;
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0001A8E8 File Offset: 0x00018AE8
		public virtual void Recover(RecognitionException re)
		{
			this.input.Consume();
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0001A8F8 File Offset: 0x00018AF8
		public override void ReportError(RecognitionException e)
		{
			this.DisplayRecognitionError(this.TokenNames, e);
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x0001A908 File Offset: 0x00018B08
		public override string GetErrorMessage(RecognitionException e, string[] tokenNames)
		{
			string result;
			if (e is MismatchedTokenException)
			{
				MismatchedTokenException ex = (MismatchedTokenException)e;
				result = "mismatched character " + this.GetCharErrorDisplay(e.Char) + " expecting " + this.GetCharErrorDisplay(ex.Expecting);
			}
			else if (e is NoViableAltException)
			{
				NoViableAltException ex2 = (NoViableAltException)e;
				result = "no viable alternative at character " + this.GetCharErrorDisplay(ex2.Char);
			}
			else if (e is EarlyExitException)
			{
				EarlyExitException ex3 = (EarlyExitException)e;
				result = "required (...)+ loop did not match anything at character " + this.GetCharErrorDisplay(ex3.Char);
			}
			else if (e is MismatchedNotSetException)
			{
				MismatchedSetException ex4 = (MismatchedSetException)e;
				result = string.Concat(new object[]
				{
					"mismatched character ",
					this.GetCharErrorDisplay(ex4.Char),
					" expecting set ",
					ex4.expecting
				});
			}
			else if (e is MismatchedSetException)
			{
				MismatchedSetException ex5 = (MismatchedSetException)e;
				result = string.Concat(new object[]
				{
					"mismatched character ",
					this.GetCharErrorDisplay(ex5.Char),
					" expecting set ",
					ex5.expecting
				});
			}
			else if (e is MismatchedRangeException)
			{
				MismatchedRangeException ex6 = (MismatchedRangeException)e;
				result = string.Concat(new string[]
				{
					"mismatched character ",
					this.GetCharErrorDisplay(ex6.Char),
					" expecting set ",
					this.GetCharErrorDisplay(ex6.A),
					"..",
					this.GetCharErrorDisplay(ex6.B)
				});
			}
			else
			{
				result = base.GetErrorMessage(e, tokenNames);
			}
			return result;
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x0001AAC4 File Offset: 0x00018CC4
		public string GetCharErrorDisplay(int c)
		{
			string str;
			switch (c)
			{
			case 9:
				str = "\\t";
				break;
			case 10:
				str = "\\n";
				break;
			default:
				if (c != -1)
				{
					str = Convert.ToString((char)c);
				}
				else
				{
					str = "<EOF>";
				}
				break;
			case 13:
				str = "\\r";
				break;
			}
			return "'" + str + "'";
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x0001AB48 File Offset: 0x00018D48
		public virtual void TraceIn(string ruleName, int ruleIndex)
		{
			string inputSymbol = string.Concat(new object[]
			{
				(char)this.input.LT(1),
				" line=",
				this.Line,
				":",
				this.CharPositionInLine
			});
			base.TraceIn(ruleName, ruleIndex, inputSymbol);
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x0001ABAC File Offset: 0x00018DAC
		public virtual void TraceOut(string ruleName, int ruleIndex)
		{
			string inputSymbol = string.Concat(new object[]
			{
				(char)this.input.LT(1),
				" line=",
				this.Line,
				":",
				this.CharPositionInLine
			});
			base.TraceOut(ruleName, ruleIndex, inputSymbol);
		}

		// Token: 0x0400027D RID: 637
		private const int TOKEN_dot_EOF = -1;

		// Token: 0x0400027E RID: 638
		protected internal ICharStream input;
	}
}
