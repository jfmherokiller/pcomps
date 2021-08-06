using System.Collections;
using System.IO;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000237 RID: 567
	public class AngleBracketTemplateLexer : CharScanner, TokenStream
	{
		// Token: 0x060010D1 RID: 4305 RVA: 0x000782FC File Offset: 0x000764FC
		public AngleBracketTemplateLexer(StringTemplate self, TextReader r) : this(r)
		{
			this.self = self;
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x0007830C File Offset: 0x0007650C
		public override void reportError(RecognitionException e)
		{
			self.Error("<...> chunk lexer error", e);
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x00078320 File Offset: 0x00076520
		protected bool upcomingELSE(int i)
		{
			return LA(i) == '<' && LA(i + 1) == 'e' && LA(i + 2) == 'l' && LA(i + 3) == 's' && LA(i + 4) == 'e' && LA(i + 5) == '>';
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0007837C File Offset: 0x0007657C
		protected bool upcomingENDIF(int i)
		{
			return LA(i) == '<' && LA(i + 1) == 'e' && LA(i + 2) == 'n' && LA(i + 3) == 'd' && LA(i + 4) == 'i' && LA(i + 5) == 'f' && LA(i + 6) == '>';
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x000783E4 File Offset: 0x000765E4
		protected bool upcomingAtEND(int i)
		{
			return LA(i) == '<' && LA(i + 1) == '@' && LA(i + 2) == 'e' && LA(i + 3) == 'n' && LA(i + 4) == 'd' && LA(i + 5) == '>';
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x00078440 File Offset: 0x00076640
		protected bool upcomingNewline(int i)
		{
			return (LA(i) == '\r' && LA(i + 1) == '\n') || LA(i) == '\n';
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x00078468 File Offset: 0x00076668
		public AngleBracketTemplateLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x00078478 File Offset: 0x00076678
		public AngleBracketTemplateLexer(TextReader r) : this(new CharBuffer(r))
		{
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x00078488 File Offset: 0x00076688
		public AngleBracketTemplateLexer(InputBuffer ib) : this(new LexerSharedInputState(ib))
		{
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x00078498 File Offset: 0x00076698
		public AngleBracketTemplateLexer(LexerSharedInputState state) : base(state)
		{
			initialize();
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x000784A8 File Offset: 0x000766A8
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
			literals = new Hashtable(100, 0.4f, null, Comparer.Default);
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x000784D0 File Offset: 0x000766D0
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
							if (cached_LA != '<')
							{
								if (tokenSet_0_.member((int)cached_LA1) && cached_LA1 != '\r' && cached_LA1 != '\n')
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

		// Token: 0x060010DD RID: 4317 RVA: 0x0007861C File Offset: 0x0007681C
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
				if (cached_LA1 == '\\' && cached_LA2 == '<')
				{
					var length3 = text.Length;
					match('\\');
					text.Length = length3;
					match('<');
				}
				else if (cached_LA1 == '\\' && cached_LA2 == '>')
				{
					var length4 = text.Length;
					match('\\');
					text.Length = length4;
					match('>');
				}
				else if (cached_LA1 == '\\' && cached_LA2 == '\\')
				{
					var length5 = text.Length;
					match('\\');
					text.Length = length5;
					match('\\');
				}
				else if (cached_LA1 == '\\' && tokenSet_1_.member((int)cached_LA2))
				{
					match('\\');
					match(tokenSet_1_);
				}
				else if (cached_LA1 == '\t' || cached_LA1 == ' ')
				{
					mINDENT(true);
					var returnToken_ = this.returnToken_;
					if (column == 1 && cached_LA1 == '<')
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
					if (!tokenSet_0_.member((int)cached_LA1))
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
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0007887C File Offset: 0x00076A7C
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
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x00078928 File Offset: 0x00076B28
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
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x000789D0 File Offset: 0x00076BD0
		public void mACTION(bool _createToken)
		{
			IToken token = null;
			var length = this.text.Length;
			var num = 6;
			var column = getColumn();
			if (cached_LA1 == '<' && this.cached_LA2 == '\\' && LA(3) == 'n' && LA(4) == '>')
			{
				var length2 = text.Length;
				match("<\\n>");
				text.Length = length2;
				text.Length = length;
				text.Append('\n');
				num = 4;
			}
			else if (cached_LA1 == '<' && this.cached_LA2 == '\\' && LA(3) == 'r' && LA(4) == '>')
			{
				var length3 = text.Length;
				match("<\\r>");
				text.Length = length3;
				text.Length = length;
				text.Append('\r');
				num = 4;
			}
			else if (cached_LA1 == '<' && this.cached_LA2 == '\\' && LA(3) == 't' && LA(4) == '>')
			{
				var length4 = text.Length;
				match("<\\t>");
				text.Length = length4;
				text.Length = length;
				text.Append('\t');
				num = 4;
			}
			else if (cached_LA1 == '<' && this.cached_LA2 == '\\' && LA(3) == ' ' && LA(4) == '>')
			{
				var length5 = text.Length;
				match("<\\ >");
				text.Length = length5;
				text.Length = length;
				text.Append(' ');
				num = 4;
			}
			else if (cached_LA1 == '<' && this.cached_LA2 == '!' && LA(3) >= '\u0001' && LA(3) <= '￾' && LA(4) >= '\u0001' && LA(4) <= '￾')
			{
				mCOMMENT(false);
				num = Token.SKIP;
			}
			else
			{
				if (cached_LA1 != '<' || !tokenSet_2_.member((int)this.cached_LA2) || LA(3) < '\u0001' || LA(3) > '￾')
				{
					throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
				}
				if (cached_LA1 == '<' && this.cached_LA2 == 'i' && LA(3) == 'f' && (LA(4) == ' ' || LA(4) == '(') && tokenSet_3_.member((int)LA(5)) && LA(6) >= '\u0001' && LA(6) <= '￾' && LA(7) >= '\u0001' && LA(7) <= '￾')
				{
					var length6 = text.Length;
					match('<');
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
					match('>');
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
				}
				else if (cached_LA1 == '<' && this.cached_LA2 == 'e' && LA(3) == 'n' && LA(4) == 'd' && LA(5) == 'i' && LA(6) == 'f' && LA(7) == '>')
				{
					var length7 = text.Length;
					match('<');
					text.Length = length7;
					match("endif");
					length7 = text.Length;
					match('>');
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
				}
				else if (cached_LA1 == '<' && this.cached_LA2 == 'e' && LA(3) == 'l' && LA(4) == 's' && LA(5) == 'e' && LA(6) == '>')
				{
					var length8 = text.Length;
					match('<');
					text.Length = length8;
					match("else");
					length8 = text.Length;
					match('>');
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
				}
				else if (cached_LA1 == '<' && this.cached_LA2 == '@' && tokenSet_4_.member((int)LA(3)) && LA(4) >= '\u0001' && LA(4) <= '￾' && LA(5) >= '\u0001' && LA(5) <= '￾' && LA(6) >= '\u0001' && LA(6) <= '￾')
				{
					var length9 = this.text.Length;
					match('<');
					this.text.Length = length9;
					length9 = this.text.Length;
					match('@');
					this.text.Length = length9;
					var num2 = 0;
					while (tokenSet_4_.member((int)cached_LA1))
					{
						match(tokenSet_4_);
						num2++;
					}
					if (num2 < 1)
					{
						throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
					}
					var cached_LA4 = cached_LA1;
					if (cached_LA4 != '(')
					{
						if (cached_LA4 != '>')
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						length9 = this.text.Length;
						match('>');
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
						if ((cached_LA1 == '\n' || cached_LA1 == '\r') && cached_LA2 >= '\u0001' && cached_LA2 <= '￾')
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
						}
						else if (cached_LA1 < '\u0001' || cached_LA1 > '￾')
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}
						if (cached_LA1 == '<' && cached_LA2 == '@')
						{
							length9 = this.text.Length;
							match("<@end>");
							this.text.Length = length9;
						}
						else
						{
							if (cached_LA1 < '\u0001' || cached_LA1 > '￾')
							{
								throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
							}
							matchNot(1);
							self.Error($"missing region {text} <@end> tag");
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
					else
					{
						length9 = text.Length;
						match("()");
						text.Length = length9;
						length9 = text.Length;
						match('>');
						text.Length = length9;
						num = 10;
					}
				}
				else
				{
					if (cached_LA1 != '<' || !tokenSet_2_.member((int)cached_LA2) || LA(3) < '\u0001' || LA(3) > '￾')
					{
						throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
					}
					var length10 = text.Length;
					match('<');
					text.Length = length10;
					mEXPR(false);
					length10 = text.Length;
					match('>');
					text.Length = length10;
				}
				var chunkToken = new ChunkToken(num, this.text.ToString(length, this.text.Length - length), currentIndent);
				token = chunkToken;
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00079880 File Offset: 0x00077A80
		protected void mCOMMENT(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 19;
			var column = getColumn();
			match("<!");
			while (cached_LA1 != '!' || this.cached_LA2 != '>')
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
			match("!>");
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
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x00079A54 File Offset: 0x00077C54
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
				if (tokenSet_5_.member((int)cached_LA1))
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
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x00079B94 File Offset: 0x00077D94
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
				if ((cached_LA1 == '+' || cached_LA1 == '=') && (this.cached_LA2 == '"' || this.cached_LA2 == '<'))
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
				if ((cached_LA1 == '+' || cached_LA1 == '=') && this.cached_LA2 == '{')
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
				if ((cached_LA1 == '+' || cached_LA1 == '=') && tokenSet_6_.member((int)this.cached_LA2))
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
					match(tokenSet_6_);
					goto IL_245;
				}
				if (tokenSet_7_.member((int)cached_LA1))
				{
					matchNot('>');
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
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x00079E2C File Offset: 0x0007802C
		protected void mESC(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 15;
			match('\\');
			matchNot(1);
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00079E98 File Offset: 0x00078098
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
						if (!tokenSet_8_.member((int)cached_LA1))
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
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00079F44 File Offset: 0x00078144
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
						if (!tokenSet_9_.member((int)cached_LA1))
						{
							break;
						}
						matchNot('"');
					}
				}
				match('"');
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x0007A43C File Offset: 0x0007863C
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
						if (!tokenSet_10_.member((int)cached_LA1))
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
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x0007A514 File Offset: 0x00078714
		private static long[] mk_tokenSet_0_()
		{
			var array = new long[2048];
			array[0] = -1152921504606856194L;
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

		// Token: 0x060010E9 RID: 4329 RVA: 0x0007A578 File Offset: 0x00078778
		private static long[] mk_tokenSet_1_()
		{
			var array = new long[2048];
			array[0] = -5764607523034234882L;
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

		// Token: 0x060010EA RID: 4330 RVA: 0x0007A5DC File Offset: 0x000787DC
		private static long[] mk_tokenSet_2_()
		{
			var array = new long[2048];
			array[0] = -4611686018427387906L;
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

		// Token: 0x060010EB RID: 4331 RVA: 0x0007A640 File Offset: 0x00078840
		private static long[] mk_tokenSet_3_()
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

		// Token: 0x060010EC RID: 4332 RVA: 0x0007A6A4 File Offset: 0x000788A4
		private static long[] mk_tokenSet_4_()
		{
			var array = new long[2048];
			array[0] = -4611687117939015682L;
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

		// Token: 0x060010ED RID: 4333 RVA: 0x0007A708 File Offset: 0x00078908
		private static long[] mk_tokenSet_5_()
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

		// Token: 0x060010EE RID: 4334 RVA: 0x0007A778 File Offset: 0x00078978
		private static long[] mk_tokenSet_6_()
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

		// Token: 0x060010EF RID: 4335 RVA: 0x0007A7E8 File Offset: 0x000789E8
		private static long[] mk_tokenSet_7_()
		{
			var array = new long[2048];
			array[0] = -6917537823734113282L;
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

		// Token: 0x060010F0 RID: 4336 RVA: 0x0007A858 File Offset: 0x00078A58
		private static long[] mk_tokenSet_8_()
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

		// Token: 0x060010F1 RID: 4337 RVA: 0x0007A8C4 File Offset: 0x00078AC4
		private static long[] mk_tokenSet_9_()
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

		// Token: 0x060010F2 RID: 4338 RVA: 0x0007A930 File Offset: 0x00078B30
		private static long[] mk_tokenSet_10_()
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

		// Token: 0x04000DFB RID: 3579
		public const int EOF = 1;

		// Token: 0x04000DFC RID: 3580
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000DFD RID: 3581
		public const int LITERAL = 4;

		// Token: 0x04000DFE RID: 3582
		public const int NEWLINE = 5;

		// Token: 0x04000DFF RID: 3583
		public const int ACTION = 6;

		// Token: 0x04000E00 RID: 3584
		public const int IF = 7;

		// Token: 0x04000E01 RID: 3585
		public const int ELSE = 8;

		// Token: 0x04000E02 RID: 3586
		public const int ENDIF = 9;

		// Token: 0x04000E03 RID: 3587
		public const int REGION_REF = 10;

		// Token: 0x04000E04 RID: 3588
		public const int REGION_DEF = 11;

		// Token: 0x04000E05 RID: 3589
		public const int EXPR = 12;

		// Token: 0x04000E06 RID: 3590
		public const int TEMPLATE = 13;

		// Token: 0x04000E07 RID: 3591
		public const int IF_EXPR = 14;

		// Token: 0x04000E08 RID: 3592
		public const int ESC = 15;

		// Token: 0x04000E09 RID: 3593
		public const int SUBTEMPLATE = 16;

		// Token: 0x04000E0A RID: 3594
		public const int NESTED_PARENS = 17;

		// Token: 0x04000E0B RID: 3595
		public const int INDENT = 18;

		// Token: 0x04000E0C RID: 3596
		public const int COMMENT = 19;

		// Token: 0x04000E0D RID: 3597
		protected string currentIndent;

		// Token: 0x04000E0E RID: 3598
		protected StringTemplate self;

		// Token: 0x04000E0F RID: 3599
		public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());

		// Token: 0x04000E10 RID: 3600
		public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());

		// Token: 0x04000E11 RID: 3601
		public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());

		// Token: 0x04000E12 RID: 3602
		public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());

		// Token: 0x04000E13 RID: 3603
		public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());

		// Token: 0x04000E14 RID: 3604
		public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());

		// Token: 0x04000E15 RID: 3605
		public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());

		// Token: 0x04000E16 RID: 3606
		public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());

		// Token: 0x04000E17 RID: 3607
		public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());

		// Token: 0x04000E18 RID: 3608
		public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());

		// Token: 0x04000E19 RID: 3609
		public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	}
}
