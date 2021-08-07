using System.Collections;
using System.IO;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000240 RID: 576
	public class DefaultTemplateLexer : CharScanner, TokenStream
	{
		// Token: 0x06001131 RID: 4401 RVA: 0x0007C1A4 File Offset: 0x0007A3A4
		public DefaultTemplateLexer(StringTemplate self, TextReader r) : this(r)
		{
			this.self = self;
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x0007C1B4 File Offset: 0x0007A3B4
		public override void reportError(RecognitionException e)
		{
			self.Error("$...$ chunk lexer error", e);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x0007C1C8 File Offset: 0x0007A3C8
		protected bool upcomingELSE(int i)
		{
			return LA(i) == '$' && LA(i + 1) == 'e' && LA(i + 2) == 'l' && LA(i + 3) == 's' && LA(i + 4) == 'e' && LA(i + 5) == '$';
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0007C224 File Offset: 0x0007A424
		protected bool upcomingENDIF(int i)
		{
			return LA(i) == '$' && LA(i + 1) == 'e' && LA(i + 2) == 'n' && LA(i + 3) == 'd' && LA(i + 4) == 'i' && LA(i + 5) == 'f' && LA(i + 6) == '$';
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x0007C28C File Offset: 0x0007A48C
		protected bool upcomingAtEND(int i)
		{
			return LA(i) == '$' && LA(i + 1) == '@' && LA(i + 2) == 'e' && LA(i + 3) == 'n' && LA(i + 4) == 'd' && LA(i + 5) == '$';
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x0007C2E8 File Offset: 0x0007A4E8
		protected bool upcomingNewline(int i)
		{
			return (LA(i) == '\r' && LA(i + 1) == '\n') || LA(i) == '\n';
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x0007C310 File Offset: 0x0007A510
		public DefaultTemplateLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x0007C320 File Offset: 0x0007A520
		public DefaultTemplateLexer(TextReader r) : this(new CharBuffer(r))
		{
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x0007C330 File Offset: 0x0007A530
		public DefaultTemplateLexer(InputBuffer ib) : this(new LexerSharedInputState(ib))
		{
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x0007C340 File Offset: 0x0007A540
		public DefaultTemplateLexer(LexerSharedInputState state) : base(state)
		{
			initialize();
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x0007C350 File Offset: 0x0007A550
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
			literals = new Hashtable(100, 0.4f);
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x0007C378 File Offset: 0x0007A578
		public override IToken nextToken()
		{
			IToken returnToken_4;
			for (;;)
			{
				resetText();
				try
				{
					try
					{
						var cached_LA = cached_LA1;
						if (cached_LA != '\n' && cached_LA != '\r')
						{
							if (cached_LA != '$')
							{
								if (tokenSet_0_.member(cached_LA1) && cached_LA1 != '\r' && cached_LA1 != '\n')
								{
									mLITERAL(true);
									var returnToken_ = this.returnToken_;
								}
								else
								{
									if (cached_LA1 != EOF_CHAR)
									{
										throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
									}
									uponEOF();
									returnToken_ = makeToken(1);
								}
							}
							else
							{
								mACTION(true);
								var returnToken_2 = returnToken_;
							}
						}
						else
						{
							mNEWLINE(true);
							var returnToken_3 = returnToken_;
						}
						if (this.returnToken_ == null)
						{
							continue;
						}
						var num = this.returnToken_.Type;
						num = testLiteralsTable(num);
						this.returnToken_.Type = num;
						returnToken_4 = this.returnToken_;
					}
					catch (RecognitionException re)
					{
						throw new TokenStreamRecognitionException(re);
					}
				}
				catch (CharStreamException ex)
				{
					if (ex is CharStreamIOException)
					{
						throw new TokenStreamIOException(((CharStreamIOException)ex).io);
					}
					throw new TokenStreamException(ex.Message);
				}
				break;
			}
			return returnToken_4;
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x0007C4C4 File Offset: 0x0007A6C4
		public void mLITERAL(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 4;
			if (cached_LA1 == '\r' || cached_LA1 == '\n')
			{
				throw new SemanticException("((cached_LA1 != '\\r') && (cached_LA1 != '\\n'))");
			}
			var num2 = 0;
			for (;;)
			{
				var length2 = text.Length;
				var column = getColumn();
				if (cached_LA1 == '\\' && cached_LA2 == '$')
				{
					var length3 = text.Length;
					match('\\');
					text.Length = length3;
					match('$');
				}
				else if (cached_LA1 == '\\' && cached_LA2 == '\\')
				{
					var length4 = text.Length;
					match('\\');
					text.Length = length4;
					match('\\');
				}
				else if (cached_LA1 == '\\' && tokenSet_1_.member(cached_LA2))
				{
					match('\\');
					matchNot('$');
				}
				else if (cached_LA1 == '\t' || cached_LA1 == ' ')
				{
					mINDENT(true);
					var returnToken_ = this.returnToken_;
					if (column == 1 && cached_LA1 == '$')
					{
						currentIndent = returnToken_.getText();
						text.Length = length2;
					}
					else
					{
						currentIndent = null;
					}
				}
				else
				{
					if (!tokenSet_0_.member(cached_LA1))
					{
						break;
					}
					match(tokenSet_0_);
				}
				num2++;
			}
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			if (text.ToString(length, text.Length - length).Length == 0)
			{
				num = Token.SKIP;
			}
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x0007C6DC File Offset: 0x0007A8DC
		protected void mINDENT(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 18;
			var num2 = 0;
			for (;;)
			{
				if (cached_LA1 == ' ')
				{
					match(' ');
				}
				else
				{
					if (cached_LA1 != '\t')
					{
						break;
					}
					match('\t');
				}
				num2++;
			}
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x0007C788 File Offset: 0x0007A988
		public void mNEWLINE(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 5;
			var cached_LA = cached_LA1;
			if (cached_LA != '\n')
			{
				if (cached_LA != '\r')
				{
					throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
				}
				match('\r');
			}
			match('\n');
			newline();
			currentIndent = null;
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x0007C830 File Offset: 0x0007AA30
		public void mACTION(bool _createToken)
		{
			IToken token = null;
			var length = this.text.Length;
			var num = 6;
			var column = getColumn();
			switch (cached_LA1)
            {
                case '$' when this.cached_LA2 == '\\' && LA(3) == 'n' && LA(4) == '$':
                {
                    var length2 = text.Length;
                    match("$\\n$");
                    text.Length = length2;
                    text.Length = length;
                    text.Append('\n');
                    num = 4;
                    break;
                }
                case '$' when this.cached_LA2 == '\\' && LA(3) == 'r' && LA(4) == '$':
                {
                    var length3 = text.Length;
                    match("$\\r$");
                    text.Length = length3;
                    text.Length = length;
                    text.Append('\r');
                    num = 4;
                    break;
                }
                case '$' when this.cached_LA2 == '\\' && LA(3) == 't' && LA(4) == '$':
                {
                    var length4 = text.Length;
                    match("$\\t$");
                    text.Length = length4;
                    text.Length = length;
                    text.Append('\t');
                    num = 4;
                    break;
                }
                case '$' when this.cached_LA2 == '\\' && LA(3) == ' ' && LA(4) == '$':
                {
                    var length5 = text.Length;
                    match("$\\ $");
                    text.Length = length5;
                    text.Length = length;
                    text.Append(' ');
                    num = 4;
                    break;
                }
                case '$' when this.cached_LA2 == '!' && LA(3) >= '\u0001' && LA(3) <= '￾' && LA(4) >= '\u0001' && LA(4) <= '￾':
                    mCOMMENT(false);
                    num = Token.SKIP;
                    break;
                default:
                {
                    if (cached_LA1 != '$' || !tokenSet_1_.member(this.cached_LA2) || LA(3) < '\u0001' || LA(3) > '￾')
                    {
                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                    }
                    switch (cached_LA1)
                    {
                        case '$' when this.cached_LA2 == 'i' && LA(3) == 'f' && (LA(4) == ' ' || LA(4) == '(') && tokenSet_2_.member(LA(5)) && LA(6) >= '\u0001' && LA(6) <= '￾' && LA(7) >= '\u0001' && LA(7) <= '￾':
                        {
                            var length6 = text.Length;
                            match('$');
                            text.Length = length6;
                            match("if");
                            while (cached_LA1 == ' ')
                            {
                                length6 = text.Length;
                                match(' ');
                                text.Length = length6;
                            }
                            match("(");
                            mIF_EXPR(false);
                            match(")");
                            length6 = text.Length;
                            match('$');
                            text.Length = length6;
                            num = 7;
                            if (cached_LA1 == '\n' || cached_LA1 == '\r')
                            {
                                var cached_LA = cached_LA1;
                                if (cached_LA != '\n')
                                {
                                    if (cached_LA != '\r')
                                    {
                                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                    }
                                    length6 = text.Length;
                                    match('\r');
                                    text.Length = length6;
                                }
                                length6 = text.Length;
                                match('\n');
                                text.Length = length6;
                                newline();
                            }

                            break;
                        }
                        case '$' when this.cached_LA2 == 'e' && LA(3) == 'n' && LA(4) == 'd' && LA(5) == 'i' && LA(6) == 'f' && LA(7) == '$':
                        {
                            var length7 = text.Length;
                            match('$');
                            text.Length = length7;
                            match("endif");
                            length7 = text.Length;
                            match('$');
                            text.Length = length7;
                            num = 9;
                            if ((cached_LA1 == '\n' || cached_LA1 == '\r') && column == 1)
                            {
                                var cached_LA2 = cached_LA1;
                                if (cached_LA2 != '\n')
                                {
                                    if (cached_LA2 != '\r')
                                    {
                                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                    }
                                    length7 = text.Length;
                                    match('\r');
                                    text.Length = length7;
                                }
                                length7 = text.Length;
                                match('\n');
                                text.Length = length7;
                                newline();
                            }

                            break;
                        }
                        case '$' when this.cached_LA2 == 'e' && LA(3) == 'l' && LA(4) == 's' && LA(5) == 'e' && LA(6) == '$':
                        {
                            var length8 = text.Length;
                            match('$');
                            text.Length = length8;
                            match("else");
                            length8 = text.Length;
                            match('$');
                            text.Length = length8;
                            num = 8;
                            if (cached_LA1 == '\n' || cached_LA1 == '\r')
                            {
                                var cached_LA3 = cached_LA1;
                                if (cached_LA3 != '\n')
                                {
                                    if (cached_LA3 != '\r')
                                    {
                                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                    }
                                    length8 = text.Length;
                                    match('\r');
                                    text.Length = length8;
                                }
                                length8 = text.Length;
                                match('\n');
                                text.Length = length8;
                                newline();
                            }

                            break;
                        }
                        case '$' when this.cached_LA2 == '@' && tokenSet_3_.member(LA(3)) && LA(4) >= '\u0001' && LA(4) <= '￾' && LA(5) >= '\u0001' && LA(5) <= '￾' && LA(6) >= '\u0001' && LA(6) <= '￾':
                        {
                            var length9 = this.text.Length;
                            match('$');
                            this.text.Length = length9;
                            length9 = this.text.Length;
                            match('@');
                            this.text.Length = length9;
                            var num2 = 0;
                            while (tokenSet_3_.member(cached_LA1))
                            {
                                match(tokenSet_3_);
                                num2++;
                            }
                            if (num2 < 1)
                            {
                                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                            }
                            var cached_LA4 = cached_LA1;
                            if (cached_LA4 != '$')
                            {
                                if (cached_LA4 != '(')
                                {
                                    throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                }
                                length9 = text.Length;
                                match("()");
                                text.Length = length9;
                                length9 = text.Length;
                                match('$');
                                text.Length = length9;
                                num = 10;
                            }
                            else
                            {
                                length9 = this.text.Length;
                                match('$');
                                this.text.Length = length9;
                                num = 11;
                                var text = this.text.ToString(length, this.text.Length - length);
                                this.text.Length = length;
                                this.text.Append($"{text}::=");
                                if ((cached_LA1 == '\n' || cached_LA1 == '\r') && cached_LA2 >= '\u0001' && cached_LA2 <= '￾' && LA(3) >= '\u0001' && LA(3) <= '￾')
                                {
                                    var cached_LA5 = cached_LA1;
                                    if (cached_LA5 != '\n')
                                    {
                                        if (cached_LA5 != '\r')
                                        {
                                            throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                        }
                                        length9 = this.text.Length;
                                        match('\r');
                                        this.text.Length = length9;
                                    }
                                    length9 = this.text.Length;
                                    match('\n');
                                    this.text.Length = length9;
                                    newline();
                                }
                                else if (cached_LA1 < '\u0001' || cached_LA1 > '￾' || cached_LA2 < '\u0001' || cached_LA2 > '￾')
                                {
                                    throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                }
                                var flag = false;
                                var num3 = 0;
                                while (cached_LA1 >= '\u0001' && cached_LA1 <= '￾' && cached_LA2 >= '\u0001' && cached_LA2 <= '￾' && !upcomingAtEND(1) && (!upcomingNewline(1) || !upcomingAtEND(2)))
                                {
                                    if ((cached_LA1 == '\n' || cached_LA1 == '\r') && cached_LA2 >= '\u0001' && cached_LA2 <= '￾')
                                    {
                                        var cached_LA6 = cached_LA1;
                                        if (cached_LA6 != '\n')
                                        {
                                            if (cached_LA6 != '\r')
                                            {
                                                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                            }
                                            match('\r');
                                        }
                                        match('\n');
                                        newline();
                                        flag = true;
                                    }
                                    else
                                    {
                                        if (cached_LA1 < '\u0001' || cached_LA1 > '￾' || cached_LA2 < '\u0001' || cached_LA2 > '￾')
                                        {
                                            throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                        }
                                        matchNot(1);
                                        flag = false;
                                    }
                                    num3++;
                                }
                                if (num3 < 1)
                                {
                                    throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                }
                                switch (cached_LA1)
                                {
                                    case '\n' or '\r' when cached_LA2 >= '\u0001' && cached_LA2 <= '￾':
                                    {
                                        var cached_LA7 = cached_LA1;
                                        if (cached_LA7 != '\n')
                                        {
                                            if (cached_LA7 != '\r')
                                            {
                                                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                            }
                                            length9 = this.text.Length;
                                            match('\r');
                                            this.text.Length = length9;
                                        }
                                        length9 = this.text.Length;
                                        match('\n');
                                        this.text.Length = length9;
                                        newline();
                                        flag = true;
                                        break;
                                    }
                                    case < '\u0001':
                                    case > '￾':
                                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                }
                                if (cached_LA1 == '$' && cached_LA2 == '@')
                                {
                                    length9 = this.text.Length;
                                    match("$@end$");
                                    this.text.Length = length9;
                                }
                                else
                                {
                                    if (cached_LA1 < '\u0001' || cached_LA1 > '￾')
                                    {
                                        throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                    }
                                    matchNot(1);
                                    self.Error($"missing region {text} $@end$ tag");
                                }
                                if ((cached_LA1 == '\n' || cached_LA1 == '\r') && flag)
                                {
                                    var cached_LA8 = cached_LA1;
                                    if (cached_LA8 != '\n')
                                    {
                                        if (cached_LA8 != '\r')
                                        {
                                            throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                                        }
                                        length9 = this.text.Length;
                                        match('\r');
                                        this.text.Length = length9;
                                    }
                                    length9 = this.text.Length;
                                    match('\n');
                                    this.text.Length = length9;
                                    newline();
                                }
                            }

                            break;
                        }
                        default:
                        {
                            if (cached_LA1 != '$' || !tokenSet_1_.member(cached_LA2) || LA(3) < '\u0001' || LA(3) > '￾')
                            {
                                throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
                            }
                            var length10 = text.Length;
                            match('$');
                            text.Length = length10;
                            mEXPR(false);
                            length10 = text.Length;
                            match('$');
                            text.Length = length10;
                            break;
                        }
                    }
                    var chunkToken = new ChunkToken(num, this.text.ToString(length, this.text.Length - length), currentIndent);
                    token = chunkToken;
                    break;
                }
            }
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x0007D6DC File Offset: 0x0007B8DC
		protected void mCOMMENT(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 19;
			var column = getColumn();
			match("$!");
			while (cached_LA1 != '!' || this.cached_LA2 != '$')
			{
				if ((cached_LA1 == '\n' || cached_LA1 == '\r') && cached_LA2 >= '\u0001' && cached_LA2 <= '￾' && LA(3) >= '\u0001' && LA(3) <= '￾')
				{
					var cached_LA = cached_LA1;
					if (cached_LA != '\n')
					{
						if (cached_LA != '\r')
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						match('\r');
					}
					match('\n');
					newline();
				}
				else
				{
					if (cached_LA1 < '\u0001' || cached_LA1 > '￾' || cached_LA2 < '\u0001' || cached_LA2 > '￾' || LA(3) < '\u0001' || LA(3) > '￾')
					{
						break;
					}
					matchNot(1);
				}
			}
			match("!$");
			if ((cached_LA1 == '\n' || cached_LA1 == '\r') && column == 1)
			{
				var cached_LA2 = cached_LA1;
				if (cached_LA2 != '\n')
				{
					if (cached_LA2 != '\r')
					{
						throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
					}
					match('\r');
				}
				match('\n');
				newline();
			}
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x0007D8B0 File Offset: 0x0007BAB0
		protected void mIF_EXPR(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 14;
			var num2 = 0;
			for (;;)
			{
				var cached_LA = cached_LA1;
				if (cached_LA <= '\r')
				{
					if (cached_LA != '\n' && cached_LA != '\r')
					{
						goto IL_AD;
					}
					var cached_LA2 = cached_LA1;
					if (cached_LA2 != '\n')
					{
						if (cached_LA2 != '\r')
						{
							break;
						}
						match('\r');
					}
					match('\n');
					newline();
				}
				else if (cached_LA != '(')
				{
					if (cached_LA != '\\')
					{
						if (cached_LA != '{')
						{
							goto IL_AD;
						}
						mSUBTEMPLATE(false);
					}
					else
					{
						mESC(false);
					}
				}
				else
				{
					mNESTED_PARENS(false);
				}
				IL_EB:
				num2++;
				continue;
				IL_AD:
				if (tokenSet_4_.member(cached_LA1))
				{
					matchNot(')');
					goto IL_EB;
				}
				goto IL_C9;
			}
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			IL_C9:
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x0007D9F0 File Offset: 0x0007BBF0
		protected void mEXPR(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 12;
			var num2 = 0;
			for (;;)
			{
				var cached_LA = cached_LA1;
				if (cached_LA <= '\r')
				{
					if (cached_LA != '\n' && cached_LA != '\r')
					{
						goto IL_A4;
					}
					var cached_LA2 = cached_LA1;
					if (cached_LA2 != '\n')
					{
						if (cached_LA2 != '\r')
						{
							break;
						}
						match('\r');
					}
					match('\n');
					newline();
				}
				else if (cached_LA != '\\')
				{
					if (cached_LA != '{')
					{
						goto IL_A4;
					}
					mSUBTEMPLATE(false);
				}
				else
				{
					mESC(false);
				}
				IL_245:
				num2++;
				continue;
				IL_A4:
				if (cached_LA1 is '+' or '=' && this.cached_LA2 is '"' or '<')
				{
					var cached_LA3 = cached_LA1;
					if (cached_LA3 != '+')
					{
						if (cached_LA3 != '=')
						{
							goto IL_F4;
						}
						match('=');
					}
					else
					{
						match('+');
					}
					mTEMPLATE(false);
					goto IL_245;
				}
				if (cached_LA1 is '+' or '=' && this.cached_LA2 == '{')
				{
					var cached_LA4 = cached_LA1;
					if (cached_LA4 != '+')
					{
						if (cached_LA4 != '=')
						{
							goto IL_164;
						}
						match('=');
					}
					else
					{
						match('+');
					}
					mSUBTEMPLATE(false);
					goto IL_245;
				}
				if (cached_LA1 is '+' or '=' && tokenSet_5_.member(this.cached_LA2))
				{
					var cached_LA5 = cached_LA1;
					if (cached_LA5 != '+')
					{
						if (cached_LA5 != '=')
						{
							goto IL_1DC;
						}
						match('=');
					}
					else
					{
						match('+');
					}
					match(tokenSet_5_);
					goto IL_245;
				}
				if (tokenSet_6_.member(cached_LA1))
				{
					matchNot('$');
					goto IL_245;
				}
				goto IL_223;
			}
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			IL_F4:
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			IL_164:
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			IL_1DC:
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			IL_223:
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x0007DC88 File Offset: 0x0007BE88
		protected void mESC(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 15;
			match('\\');
			matchNot(1);
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x0007DCF4 File Offset: 0x0007BEF4
		protected void mSUBTEMPLATE(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 16;
			match('{');
			for (;;)
			{
				var cached_LA = cached_LA1;
				if (cached_LA != '\\')
				{
					if (cached_LA == '{')
					{
						mSUBTEMPLATE(false);
					}
					else
					{
						if (!tokenSet_7_.member(cached_LA1))
						{
							break;
						}
						matchNot('}');
					}
				}
				else
				{
					mESC(false);
				}
			}
			match('}');
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x0007DDA0 File Offset: 0x0007BFA0
		protected void mTEMPLATE(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 13;
			var cached_LA = cached_LA1;
			if (cached_LA != '"')
			{
				if (cached_LA != '<')
				{
					throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
				}
				match("<<");
				if ((cached_LA1 == '\n' || cached_LA1 == '\r') && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾' && LA(3) >= '\u0001' && LA(3) <= '￾' && LA(4) >= '\u0001' && LA(4) <= '￾')
				{
					var cached_LA2 = cached_LA1;
					int length2;
					if (cached_LA2 != '\n')
					{
						if (cached_LA2 != '\r')
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						length2 = text.Length;
						match('\r');
						text.Length = length2;
					}
					length2 = text.Length;
					match('\n');
					text.Length = length2;
					newline();
				}
				else if (cached_LA1 < '\u0001' || cached_LA1 > '￾' || this.cached_LA2 < '\u0001' || this.cached_LA2 > '￾' || LA(3) < '\u0001' || LA(3) > '￾')
				{
					throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
				}
				while (cached_LA1 != '>' || this.cached_LA2 != '>' || LA(3) < '\u0001' || LA(3) > '￾')
				{
					if (cached_LA1 == '\r' && cached_LA2 == '\n' && LA(3) >= '\u0001' && LA(3) <= '￾' && LA(4) >= '\u0001' && LA(4) <= '￾' && LA(5) >= '\u0001' && LA(5) <= '￾' && LA(3) == '>' && LA(4) == '>')
					{
						var length3 = text.Length;
						match('\r');
						text.Length = length3;
						length3 = text.Length;
						match('\n');
						text.Length = length3;
						newline();
					}
					else if (cached_LA1 == '\n' && cached_LA2 >= '\u0001' && cached_LA2 <= '￾' && LA(3) >= '\u0001' && LA(3) <= '￾' && LA(4) >= '\u0001' && LA(4) <= '￾' && LA(2) == '>' && LA(3) == '>')
					{
						var length4 = text.Length;
						match('\n');
						text.Length = length4;
						newline();
					}
					else if ((cached_LA1 == '\n' || cached_LA1 == '\r') && cached_LA2 >= '\u0001' && cached_LA2 <= '￾' && LA(3) >= '\u0001' && LA(3) <= '￾' && LA(4) >= '\u0001' && LA(4) <= '￾')
					{
						var cached_LA3 = cached_LA1;
						if (cached_LA3 != '\n')
						{
							if (cached_LA3 != '\r')
							{
								throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
							}
							match('\r');
						}
						match('\n');
						newline();
					}
					else
					{
						if (cached_LA1 < '\u0001' || cached_LA1 > '￾' || cached_LA2 < '\u0001' || cached_LA2 > '￾' || LA(3) < '\u0001' || LA(3) > '￾' || LA(4) < '\u0001' || LA(4) > '￾')
						{
							break;
						}
						matchNot(1);
					}
				}
				match(">>");
			}
			else
			{
				match('"');
				for (;;)
				{
					if (cached_LA1 == '\\')
					{
						mESC(false);
					}
					else
					{
						if (!tokenSet_8_.member(cached_LA1))
						{
							break;
						}
						matchNot('"');
					}
				}
				match('"');
			}
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x0007E298 File Offset: 0x0007C498
		protected void mNESTED_PARENS(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 17;
			match('(');
			var num2 = 0;
			for (;;)
			{
				var cached_LA = cached_LA1;
				if (cached_LA != '(')
				{
					if (cached_LA != '\\')
					{
						if (!tokenSet_9_.member(cached_LA1))
						{
							break;
						}
						matchNot(')');
					}
					else
					{
						mESC(false);
					}
				}
				else
				{
					mNESTED_PARENS(false);
				}
				num2++;
			}
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			match(')');
			if (_createToken && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x0007E370 File Offset: 0x0007C570
		private static long[] mk_tokenSet_0_()
		{
			var array = new long[2048];
			array[0] = -68719485954L;
			for (var i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x0007E3D4 File Offset: 0x0007C5D4
		private static long[] mk_tokenSet_1_()
		{
			var array = new long[2048];
			array[0] = -68719476738L;
			for (var i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x0007E438 File Offset: 0x0007C638
		private static long[] mk_tokenSet_2_()
		{
			var array = new long[2048];
			array[0] = -2199023255554L;
			for (var i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x0007E49C File Offset: 0x0007C69C
		private static long[] mk_tokenSet_3_()
		{
			var array = new long[2048];
			array[0] = -1168231104514L;
			for (var i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x0007E500 File Offset: 0x0007C700
		private static long[] mk_tokenSet_4_()
		{
			var array = new long[2048];
			array[0] = -3298534892546L;
			array[1] = -576460752571858945L;
			for (var i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x0007E570 File Offset: 0x0007C770
		private static long[] mk_tokenSet_5_()
		{
			var array = new long[2048];
			array[0] = -1152921521786716162L;
			array[1] = -576460752303423489L;
			for (var i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x0007E5E0 File Offset: 0x0007C7E0
		private static long[] mk_tokenSet_6_()
		{
			var array = new long[2048];
			array[0] = -2305851874026202114L;
			array[1] = -576460752571858945L;
			for (var i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x0007E650 File Offset: 0x0007C850
		private static long[] mk_tokenSet_7_()
		{
			var array = new long[2048];
			array[0] = -2L;
			array[1] = -2882303761785552897L;
			for (var i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0007E6BC File Offset: 0x0007C8BC
		private static long[] mk_tokenSet_8_()
		{
			var array = new long[2048];
			array[0] = -17179869186L;
			array[1] = -268435457L;
			for (var i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x0007E728 File Offset: 0x0007C928
		private static long[] mk_tokenSet_9_()
		{
			var array = new long[2048];
			array[0] = -3298534883330L;
			array[1] = -268435457L;
			for (var i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (var j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x04000E4D RID: 3661
		public const int EOF = 1;

		// Token: 0x04000E4E RID: 3662
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000E4F RID: 3663
		public const int LITERAL = 4;

		// Token: 0x04000E50 RID: 3664
		public const int NEWLINE = 5;

		// Token: 0x04000E51 RID: 3665
		public const int ACTION = 6;

		// Token: 0x04000E52 RID: 3666
		public const int IF = 7;

		// Token: 0x04000E53 RID: 3667
		public const int ELSE = 8;

		// Token: 0x04000E54 RID: 3668
		public const int ENDIF = 9;

		// Token: 0x04000E55 RID: 3669
		public const int REGION_REF = 10;

		// Token: 0x04000E56 RID: 3670
		public const int REGION_DEF = 11;

		// Token: 0x04000E57 RID: 3671
		public const int EXPR = 12;

		// Token: 0x04000E58 RID: 3672
		public const int TEMPLATE = 13;

		// Token: 0x04000E59 RID: 3673
		public const int IF_EXPR = 14;

		// Token: 0x04000E5A RID: 3674
		public const int ESC = 15;

		// Token: 0x04000E5B RID: 3675
		public const int SUBTEMPLATE = 16;

		// Token: 0x04000E5C RID: 3676
		public const int NESTED_PARENS = 17;

		// Token: 0x04000E5D RID: 3677
		public const int INDENT = 18;

		// Token: 0x04000E5E RID: 3678
		public const int COMMENT = 19;

		// Token: 0x04000E5F RID: 3679
		protected string currentIndent;

		// Token: 0x04000E60 RID: 3680
		protected StringTemplate self;

		// Token: 0x04000E61 RID: 3681
		public static readonly BitSet tokenSet_0_ = new(mk_tokenSet_0_());

		// Token: 0x04000E62 RID: 3682
		public static readonly BitSet tokenSet_1_ = new(mk_tokenSet_1_());

		// Token: 0x04000E63 RID: 3683
		public static readonly BitSet tokenSet_2_ = new(mk_tokenSet_2_());

		// Token: 0x04000E64 RID: 3684
		public static readonly BitSet tokenSet_3_ = new(mk_tokenSet_3_());

		// Token: 0x04000E65 RID: 3685
		public static readonly BitSet tokenSet_4_ = new(mk_tokenSet_4_());

		// Token: 0x04000E66 RID: 3686
		public static readonly BitSet tokenSet_5_ = new(mk_tokenSet_5_());

		// Token: 0x04000E67 RID: 3687
		public static readonly BitSet tokenSet_6_ = new(mk_tokenSet_6_());

		// Token: 0x04000E68 RID: 3688
		public static readonly BitSet tokenSet_7_ = new(mk_tokenSet_7_());

		// Token: 0x04000E69 RID: 3689
		public static readonly BitSet tokenSet_8_ = new(mk_tokenSet_8_());

		// Token: 0x04000E6A RID: 3690
		public static readonly BitSet tokenSet_9_ = new(mk_tokenSet_9_());
	}
}
