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
			this.self.Error("<...> chunk lexer error", e);
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x00078320 File Offset: 0x00076520
		protected bool upcomingELSE(int i)
		{
			return this.LA(i) == '<' && this.LA(i + 1) == 'e' && this.LA(i + 2) == 'l' && this.LA(i + 3) == 's' && this.LA(i + 4) == 'e' && this.LA(i + 5) == '>';
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0007837C File Offset: 0x0007657C
		protected bool upcomingENDIF(int i)
		{
			return this.LA(i) == '<' && this.LA(i + 1) == 'e' && this.LA(i + 2) == 'n' && this.LA(i + 3) == 'd' && this.LA(i + 4) == 'i' && this.LA(i + 5) == 'f' && this.LA(i + 6) == '>';
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x000783E4 File Offset: 0x000765E4
		protected bool upcomingAtEND(int i)
		{
			return this.LA(i) == '<' && this.LA(i + 1) == '@' && this.LA(i + 2) == 'e' && this.LA(i + 3) == 'n' && this.LA(i + 4) == 'd' && this.LA(i + 5) == '>';
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x00078440 File Offset: 0x00076640
		protected bool upcomingNewline(int i)
		{
			return (this.LA(i) == '\r' && this.LA(i + 1) == '\n') || this.LA(i) == '\n';
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
			this.initialize();
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x000784A8 File Offset: 0x000766A8
		private void initialize()
		{
			this.caseSensitiveLiterals = true;
			this.setCaseSensitive(true);
			this.literals = new Hashtable(100, 0.4f, null, Comparer.Default);
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x000784D0 File Offset: 0x000766D0
		public override IToken nextToken()
		{
			IToken returnToken_4;
			for (;;)
			{
				this.resetText();
				try
				{
					try
					{
						char cached_LA = this.cached_LA1;
						if (cached_LA != '\n' && cached_LA != '\r')
						{
							if (cached_LA != '<')
							{
								if (AngleBracketTemplateLexer.tokenSet_0_.member((int)this.cached_LA1) && this.cached_LA1 != '\r' && this.cached_LA1 != '\n')
								{
									this.mLITERAL(true);
									IToken returnToken_ = this.returnToken_;
								}
								else
								{
									if (this.cached_LA1 != CharScanner.EOF_CHAR)
									{
										throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
									}
									this.uponEOF();
									this.returnToken_ = this.makeToken(1);
								}
							}
							else
							{
								this.mACTION(true);
								IToken returnToken_2 = this.returnToken_;
							}
						}
						else
						{
							this.mNEWLINE(true);
							IToken returnToken_3 = this.returnToken_;
						}
						if (this.returnToken_ == null)
						{
							continue;
						}
						int num = this.returnToken_.Type;
						num = this.testLiteralsTable(num);
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
			int length = this.text.Length;
			int num = 4;
			if (this.cached_LA1 == '\r' || this.cached_LA1 == '\n')
			{
				throw new SemanticException("((cached_LA1 != '\\r') && (cached_LA1 != '\\n'))");
			}
			int num2 = 0;
			for (;;)
			{
				int length2 = this.text.Length;
				int column = this.getColumn();
				if (this.cached_LA1 == '\\' && this.cached_LA2 == '<')
				{
					int length3 = this.text.Length;
					this.match('\\');
					this.text.Length = length3;
					this.match('<');
				}
				else if (this.cached_LA1 == '\\' && this.cached_LA2 == '>')
				{
					int length4 = this.text.Length;
					this.match('\\');
					this.text.Length = length4;
					this.match('>');
				}
				else if (this.cached_LA1 == '\\' && this.cached_LA2 == '\\')
				{
					int length5 = this.text.Length;
					this.match('\\');
					this.text.Length = length5;
					this.match('\\');
				}
				else if (this.cached_LA1 == '\\' && AngleBracketTemplateLexer.tokenSet_1_.member((int)this.cached_LA2))
				{
					this.match('\\');
					this.match(AngleBracketTemplateLexer.tokenSet_1_);
				}
				else if (this.cached_LA1 == '\t' || this.cached_LA1 == ' ')
				{
					this.mINDENT(true);
					IToken returnToken_ = this.returnToken_;
					if (column == 1 && this.cached_LA1 == '<')
					{
						this.currentIndent = returnToken_.getText();
						this.text.Length = length2;
					}
					else
					{
						this.currentIndent = null;
					}
				}
				else
				{
					if (!AngleBracketTemplateLexer.tokenSet_0_.member((int)this.cached_LA1))
					{
						break;
					}
					this.match(AngleBracketTemplateLexer.tokenSet_0_);
				}
				num2++;
			}
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			if (this.text.ToString(length, this.text.Length - length).Length == 0)
			{
				num = Token.SKIP;
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0007887C File Offset: 0x00076A7C
		protected void mINDENT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 18;
			int num2 = 0;
			for (;;)
			{
				if (this.cached_LA1 == ' ')
				{
					this.match(' ');
				}
				else
				{
					if (this.cached_LA1 != '\t')
					{
						break;
					}
					this.match('\t');
				}
				num2++;
			}
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x00078928 File Offset: 0x00076B28
		public void mNEWLINE(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 5;
			char cached_LA = this.cached_LA1;
			if (cached_LA != '\n')
			{
				if (cached_LA != '\r')
				{
					throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
				}
				this.match('\r');
			}
			this.match('\n');
			this.newline();
			this.currentIndent = null;
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x000789D0 File Offset: 0x00076BD0
		public void mACTION(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 6;
			int column = this.getColumn();
			if (this.cached_LA1 == '<' && this.cached_LA2 == '\\' && this.LA(3) == 'n' && this.LA(4) == '>')
			{
				int length2 = this.text.Length;
				this.match("<\\n>");
				this.text.Length = length2;
				this.text.Length = length;
				this.text.Append('\n');
				num = 4;
			}
			else if (this.cached_LA1 == '<' && this.cached_LA2 == '\\' && this.LA(3) == 'r' && this.LA(4) == '>')
			{
				int length3 = this.text.Length;
				this.match("<\\r>");
				this.text.Length = length3;
				this.text.Length = length;
				this.text.Append('\r');
				num = 4;
			}
			else if (this.cached_LA1 == '<' && this.cached_LA2 == '\\' && this.LA(3) == 't' && this.LA(4) == '>')
			{
				int length4 = this.text.Length;
				this.match("<\\t>");
				this.text.Length = length4;
				this.text.Length = length;
				this.text.Append('\t');
				num = 4;
			}
			else if (this.cached_LA1 == '<' && this.cached_LA2 == '\\' && this.LA(3) == ' ' && this.LA(4) == '>')
			{
				int length5 = this.text.Length;
				this.match("<\\ >");
				this.text.Length = length5;
				this.text.Length = length;
				this.text.Append(' ');
				num = 4;
			}
			else if (this.cached_LA1 == '<' && this.cached_LA2 == '!' && this.LA(3) >= '\u0001' && this.LA(3) <= '￾' && this.LA(4) >= '\u0001' && this.LA(4) <= '￾')
			{
				this.mCOMMENT(false);
				num = Token.SKIP;
			}
			else
			{
				if (this.cached_LA1 != '<' || !AngleBracketTemplateLexer.tokenSet_2_.member((int)this.cached_LA2) || this.LA(3) < '\u0001' || this.LA(3) > '￾')
				{
					throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
				}
				if (this.cached_LA1 == '<' && this.cached_LA2 == 'i' && this.LA(3) == 'f' && (this.LA(4) == ' ' || this.LA(4) == '(') && AngleBracketTemplateLexer.tokenSet_3_.member((int)this.LA(5)) && this.LA(6) >= '\u0001' && this.LA(6) <= '￾' && this.LA(7) >= '\u0001' && this.LA(7) <= '￾')
				{
					int length6 = this.text.Length;
					this.match('<');
					this.text.Length = length6;
					this.match("if");
					while (this.cached_LA1 == ' ')
					{
						length6 = this.text.Length;
						this.match(' ');
						this.text.Length = length6;
					}
					this.match("(");
					this.mIF_EXPR(false);
					this.match(")");
					length6 = this.text.Length;
					this.match('>');
					this.text.Length = length6;
					num = 7;
					if (this.cached_LA1 == '\n' || this.cached_LA1 == '\r')
					{
						char cached_LA = this.cached_LA1;
						if (cached_LA != '\n')
						{
							if (cached_LA != '\r')
							{
								throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
							}
							length6 = this.text.Length;
							this.match('\r');
							this.text.Length = length6;
						}
						length6 = this.text.Length;
						this.match('\n');
						this.text.Length = length6;
						this.newline();
					}
				}
				else if (this.cached_LA1 == '<' && this.cached_LA2 == 'e' && this.LA(3) == 'n' && this.LA(4) == 'd' && this.LA(5) == 'i' && this.LA(6) == 'f' && this.LA(7) == '>')
				{
					int length7 = this.text.Length;
					this.match('<');
					this.text.Length = length7;
					this.match("endif");
					length7 = this.text.Length;
					this.match('>');
					this.text.Length = length7;
					num = 9;
					if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && column == 1)
					{
						char cached_LA2 = this.cached_LA1;
						if (cached_LA2 != '\n')
						{
							if (cached_LA2 != '\r')
							{
								throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
							}
							length7 = this.text.Length;
							this.match('\r');
							this.text.Length = length7;
						}
						length7 = this.text.Length;
						this.match('\n');
						this.text.Length = length7;
						this.newline();
					}
				}
				else if (this.cached_LA1 == '<' && this.cached_LA2 == 'e' && this.LA(3) == 'l' && this.LA(4) == 's' && this.LA(5) == 'e' && this.LA(6) == '>')
				{
					int length8 = this.text.Length;
					this.match('<');
					this.text.Length = length8;
					this.match("else");
					length8 = this.text.Length;
					this.match('>');
					this.text.Length = length8;
					num = 8;
					if (this.cached_LA1 == '\n' || this.cached_LA1 == '\r')
					{
						char cached_LA3 = this.cached_LA1;
						if (cached_LA3 != '\n')
						{
							if (cached_LA3 != '\r')
							{
								throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
							}
							length8 = this.text.Length;
							this.match('\r');
							this.text.Length = length8;
						}
						length8 = this.text.Length;
						this.match('\n');
						this.text.Length = length8;
						this.newline();
					}
				}
				else if (this.cached_LA1 == '<' && this.cached_LA2 == '@' && AngleBracketTemplateLexer.tokenSet_4_.member((int)this.LA(3)) && this.LA(4) >= '\u0001' && this.LA(4) <= '￾' && this.LA(5) >= '\u0001' && this.LA(5) <= '￾' && this.LA(6) >= '\u0001' && this.LA(6) <= '￾')
				{
					int length9 = this.text.Length;
					this.match('<');
					this.text.Length = length9;
					length9 = this.text.Length;
					this.match('@');
					this.text.Length = length9;
					int num2 = 0;
					while (AngleBracketTemplateLexer.tokenSet_4_.member((int)this.cached_LA1))
					{
						this.match(AngleBracketTemplateLexer.tokenSet_4_);
						num2++;
					}
					if (num2 < 1)
					{
						throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
					}
					char cached_LA4 = this.cached_LA1;
					if (cached_LA4 != '(')
					{
						if (cached_LA4 != '>')
						{
							throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
						}
						length9 = this.text.Length;
						this.match('>');
						this.text.Length = length9;
						num = 11;
						string text = this.text.ToString(length, this.text.Length - length);
						this.text.Length = length;
						this.text.Append(text + "::=");
						if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾' && this.LA(3) >= '\u0001' && this.LA(3) <= '￾')
						{
							char cached_LA5 = this.cached_LA1;
							if (cached_LA5 != '\n')
							{
								if (cached_LA5 != '\r')
								{
									throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
								}
								length9 = this.text.Length;
								this.match('\r');
								this.text.Length = length9;
							}
							length9 = this.text.Length;
							this.match('\n');
							this.text.Length = length9;
							this.newline();
						}
						else if (this.cached_LA1 < '\u0001' || this.cached_LA1 > '￾' || this.cached_LA2 < '\u0001' || this.cached_LA2 > '￾')
						{
							throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
						}
						bool flag = false;
						int num3 = 0;
						while (this.cached_LA1 >= '\u0001' && this.cached_LA1 <= '￾' && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾' && !this.upcomingAtEND(1) && (!this.upcomingNewline(1) || !this.upcomingAtEND(2)))
						{
							if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾')
							{
								char cached_LA6 = this.cached_LA1;
								if (cached_LA6 != '\n')
								{
									if (cached_LA6 != '\r')
									{
										throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
									}
									this.match('\r');
								}
								this.match('\n');
								this.newline();
								flag = true;
							}
							else
							{
								if (this.cached_LA1 < '\u0001' || this.cached_LA1 > '￾' || this.cached_LA2 < '\u0001' || this.cached_LA2 > '￾')
								{
									throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
								}
								this.matchNot(1);
								flag = false;
							}
							num3++;
						}
						if (num3 < 1)
						{
							throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
						}
						if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾')
						{
							char cached_LA7 = this.cached_LA1;
							if (cached_LA7 != '\n')
							{
								if (cached_LA7 != '\r')
								{
									throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
								}
								length9 = this.text.Length;
								this.match('\r');
								this.text.Length = length9;
							}
							length9 = this.text.Length;
							this.match('\n');
							this.text.Length = length9;
							this.newline();
							flag = true;
						}
						else if (this.cached_LA1 < '\u0001' || this.cached_LA1 > '￾')
						{
							throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
						}
						if (this.cached_LA1 == '<' && this.cached_LA2 == '@')
						{
							length9 = this.text.Length;
							this.match("<@end>");
							this.text.Length = length9;
						}
						else
						{
							if (this.cached_LA1 < '\u0001' || this.cached_LA1 > '￾')
							{
								throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
							}
							this.matchNot(1);
							this.self.Error("missing region " + text + " <@end> tag");
						}
						if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && flag)
						{
							char cached_LA8 = this.cached_LA1;
							if (cached_LA8 != '\n')
							{
								if (cached_LA8 != '\r')
								{
									throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
								}
								length9 = this.text.Length;
								this.match('\r');
								this.text.Length = length9;
							}
							length9 = this.text.Length;
							this.match('\n');
							this.text.Length = length9;
							this.newline();
						}
					}
					else
					{
						length9 = this.text.Length;
						this.match("()");
						this.text.Length = length9;
						length9 = this.text.Length;
						this.match('>');
						this.text.Length = length9;
						num = 10;
					}
				}
				else
				{
					if (this.cached_LA1 != '<' || !AngleBracketTemplateLexer.tokenSet_2_.member((int)this.cached_LA2) || this.LA(3) < '\u0001' || this.LA(3) > '￾')
					{
						throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
					}
					int length10 = this.text.Length;
					this.match('<');
					this.text.Length = length10;
					this.mEXPR(false);
					length10 = this.text.Length;
					this.match('>');
					this.text.Length = length10;
				}
				ChunkToken chunkToken = new ChunkToken(num, this.text.ToString(length, this.text.Length - length), this.currentIndent);
				token = chunkToken;
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00079880 File Offset: 0x00077A80
		protected void mCOMMENT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 19;
			int column = this.getColumn();
			this.match("<!");
			while (this.cached_LA1 != '!' || this.cached_LA2 != '>')
			{
				if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾' && this.LA(3) >= '\u0001' && this.LA(3) <= '￾')
				{
					char cached_LA = this.cached_LA1;
					if (cached_LA != '\n')
					{
						if (cached_LA != '\r')
						{
							throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
						}
						this.match('\r');
					}
					this.match('\n');
					this.newline();
				}
				else
				{
					if (this.cached_LA1 < '\u0001' || this.cached_LA1 > '￾' || this.cached_LA2 < '\u0001' || this.cached_LA2 > '￾' || this.LA(3) < '\u0001' || this.LA(3) > '￾')
					{
						break;
					}
					this.matchNot(1);
				}
			}
			this.match("!>");
			if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && column == 1)
			{
				char cached_LA2 = this.cached_LA1;
				if (cached_LA2 != '\n')
				{
					if (cached_LA2 != '\r')
					{
						throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
					}
					this.match('\r');
				}
				this.match('\n');
				this.newline();
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x00079A54 File Offset: 0x00077C54
		protected void mIF_EXPR(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 14;
			int num2 = 0;
			for (;;)
			{
				char cached_LA = this.cached_LA1;
				if (cached_LA <= '\r')
				{
					if (cached_LA != '\n' && cached_LA != '\r')
					{
						goto IL_AD;
					}
					char cached_LA2 = this.cached_LA1;
					if (cached_LA2 != '\n')
					{
						if (cached_LA2 != '\r')
						{
							break;
						}
						this.match('\r');
					}
					this.match('\n');
					this.newline();
				}
				else if (cached_LA != '(')
				{
					if (cached_LA != '\\')
					{
						if (cached_LA != '{')
						{
							goto IL_AD;
						}
						this.mSUBTEMPLATE(false);
					}
					else
					{
						this.mESC(false);
					}
				}
				else
				{
					this.mNESTED_PARENS(false);
				}
				IL_EB:
				num2++;
				continue;
				IL_AD:
				if (AngleBracketTemplateLexer.tokenSet_5_.member((int)this.cached_LA1))
				{
					this.matchNot(')');
					goto IL_EB;
				}
				goto IL_C9;
			}
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_C9:
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x00079B94 File Offset: 0x00077D94
		protected void mEXPR(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 12;
			int num2 = 0;
			for (;;)
			{
				char cached_LA = this.cached_LA1;
				if (cached_LA <= '\r')
				{
					if (cached_LA != '\n' && cached_LA != '\r')
					{
						goto IL_A4;
					}
					char cached_LA2 = this.cached_LA1;
					if (cached_LA2 != '\n')
					{
						if (cached_LA2 != '\r')
						{
							break;
						}
						this.match('\r');
					}
					this.match('\n');
					this.newline();
				}
				else if (cached_LA != '\\')
				{
					if (cached_LA != '{')
					{
						goto IL_A4;
					}
					this.mSUBTEMPLATE(false);
				}
				else
				{
					this.mESC(false);
				}
				IL_245:
				num2++;
				continue;
				IL_A4:
				if ((this.cached_LA1 == '+' || this.cached_LA1 == '=') && (this.cached_LA2 == '"' || this.cached_LA2 == '<'))
				{
					char cached_LA3 = this.cached_LA1;
					if (cached_LA3 != '+')
					{
						if (cached_LA3 != '=')
						{
							goto IL_F4;
						}
						this.match('=');
					}
					else
					{
						this.match('+');
					}
					this.mTEMPLATE(false);
					goto IL_245;
				}
				if ((this.cached_LA1 == '+' || this.cached_LA1 == '=') && this.cached_LA2 == '{')
				{
					char cached_LA4 = this.cached_LA1;
					if (cached_LA4 != '+')
					{
						if (cached_LA4 != '=')
						{
							goto IL_164;
						}
						this.match('=');
					}
					else
					{
						this.match('+');
					}
					this.mSUBTEMPLATE(false);
					goto IL_245;
				}
				if ((this.cached_LA1 == '+' || this.cached_LA1 == '=') && AngleBracketTemplateLexer.tokenSet_6_.member((int)this.cached_LA2))
				{
					char cached_LA5 = this.cached_LA1;
					if (cached_LA5 != '+')
					{
						if (cached_LA5 != '=')
						{
							goto IL_1DC;
						}
						this.match('=');
					}
					else
					{
						this.match('+');
					}
					this.match(AngleBracketTemplateLexer.tokenSet_6_);
					goto IL_245;
				}
				if (AngleBracketTemplateLexer.tokenSet_7_.member((int)this.cached_LA1))
				{
					this.matchNot('>');
					goto IL_245;
				}
				goto IL_223;
			}
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_F4:
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_164:
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_1DC:
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_223:
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x00079E2C File Offset: 0x0007802C
		protected void mESC(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 15;
			this.match('\\');
			this.matchNot(1);
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00079E98 File Offset: 0x00078098
		protected void mSUBTEMPLATE(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 16;
			this.match('{');
			for (;;)
			{
				char cached_LA = this.cached_LA1;
				if (cached_LA != '\\')
				{
					if (cached_LA == '{')
					{
						this.mSUBTEMPLATE(false);
					}
					else
					{
						if (!AngleBracketTemplateLexer.tokenSet_8_.member((int)this.cached_LA1))
						{
							break;
						}
						this.matchNot('}');
					}
				}
				else
				{
					this.mESC(false);
				}
			}
			this.match('}');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00079F44 File Offset: 0x00078144
		protected void mTEMPLATE(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 13;
			char cached_LA = this.cached_LA1;
			if (cached_LA != '"')
			{
				if (cached_LA != '<')
				{
					throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
				}
				this.match("<<");
				if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾' && this.LA(3) >= '\u0001' && this.LA(3) <= '￾' && this.LA(4) >= '\u0001' && this.LA(4) <= '￾')
				{
					char cached_LA2 = this.cached_LA1;
					int length2;
					if (cached_LA2 != '\n')
					{
						if (cached_LA2 != '\r')
						{
							throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
						}
						length2 = this.text.Length;
						this.match('\r');
						this.text.Length = length2;
					}
					length2 = this.text.Length;
					this.match('\n');
					this.text.Length = length2;
					this.newline();
				}
				else if (this.cached_LA1 < '\u0001' || this.cached_LA1 > '￾' || this.cached_LA2 < '\u0001' || this.cached_LA2 > '￾' || this.LA(3) < '\u0001' || this.LA(3) > '￾')
				{
					throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
				}
				while (this.cached_LA1 != '>' || this.cached_LA2 != '>' || this.LA(3) < '\u0001' || this.LA(3) > '￾')
				{
					if (this.cached_LA1 == '\r' && this.cached_LA2 == '\n' && this.LA(3) >= '\u0001' && this.LA(3) <= '￾' && this.LA(4) >= '\u0001' && this.LA(4) <= '￾' && this.LA(5) >= '\u0001' && this.LA(5) <= '￾' && this.LA(3) == '>' && this.LA(4) == '>')
					{
						int length3 = this.text.Length;
						this.match('\r');
						this.text.Length = length3;
						length3 = this.text.Length;
						this.match('\n');
						this.text.Length = length3;
						this.newline();
					}
					else if (this.cached_LA1 == '\n' && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾' && this.LA(3) >= '\u0001' && this.LA(3) <= '￾' && this.LA(4) >= '\u0001' && this.LA(4) <= '￾' && this.LA(2) == '>' && this.LA(3) == '>')
					{
						int length4 = this.text.Length;
						this.match('\n');
						this.text.Length = length4;
						this.newline();
					}
					else if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\u0001' && this.cached_LA2 <= '￾' && this.LA(3) >= '\u0001' && this.LA(3) <= '￾' && this.LA(4) >= '\u0001' && this.LA(4) <= '￾')
					{
						char cached_LA3 = this.cached_LA1;
						if (cached_LA3 != '\n')
						{
							if (cached_LA3 != '\r')
							{
								throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
							}
							this.match('\r');
						}
						this.match('\n');
						this.newline();
					}
					else
					{
						if (this.cached_LA1 < '\u0001' || this.cached_LA1 > '￾' || this.cached_LA2 < '\u0001' || this.cached_LA2 > '￾' || this.LA(3) < '\u0001' || this.LA(3) > '￾' || this.LA(4) < '\u0001' || this.LA(4) > '￾')
						{
							break;
						}
						this.matchNot(1);
					}
				}
				this.match(">>");
			}
			else
			{
				this.match('"');
				for (;;)
				{
					if (this.cached_LA1 == '\\')
					{
						this.mESC(false);
					}
					else
					{
						if (!AngleBracketTemplateLexer.tokenSet_9_.member((int)this.cached_LA1))
						{
							break;
						}
						this.matchNot('"');
					}
				}
				this.match('"');
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x0007A43C File Offset: 0x0007863C
		protected void mNESTED_PARENS(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 17;
			this.match('(');
			int num2 = 0;
			for (;;)
			{
				char cached_LA = this.cached_LA1;
				if (cached_LA != '(')
				{
					if (cached_LA != '\\')
					{
						if (!AngleBracketTemplateLexer.tokenSet_10_.member((int)this.cached_LA1))
						{
							break;
						}
						this.matchNot(')');
					}
					else
					{
						this.mESC(false);
					}
				}
				else
				{
					this.mNESTED_PARENS(false);
				}
				num2++;
			}
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			this.match(')');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x0007A514 File Offset: 0x00078714
		private static long[] mk_tokenSet_0_()
		{
			long[] array = new long[2048];
			array[0] = -1152921504606856194L;
			for (int i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x0007A578 File Offset: 0x00078778
		private static long[] mk_tokenSet_1_()
		{
			long[] array = new long[2048];
			array[0] = -5764607523034234882L;
			for (int i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x0007A5DC File Offset: 0x000787DC
		private static long[] mk_tokenSet_2_()
		{
			long[] array = new long[2048];
			array[0] = -4611686018427387906L;
			for (int i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x0007A640 File Offset: 0x00078840
		private static long[] mk_tokenSet_3_()
		{
			long[] array = new long[2048];
			array[0] = -2199023255554L;
			for (int i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x0007A6A4 File Offset: 0x000788A4
		private static long[] mk_tokenSet_4_()
		{
			long[] array = new long[2048];
			array[0] = -4611687117939015682L;
			for (int i = 1; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x0007A708 File Offset: 0x00078908
		private static long[] mk_tokenSet_5_()
		{
			long[] array = new long[2048];
			array[0] = -3298534892546L;
			array[1] = -576460752571858945L;
			for (int i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x0007A778 File Offset: 0x00078978
		private static long[] mk_tokenSet_6_()
		{
			long[] array = new long[2048];
			array[0] = -1152921521786716162L;
			array[1] = -576460752303423489L;
			for (int i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x0007A7E8 File Offset: 0x000789E8
		private static long[] mk_tokenSet_7_()
		{
			long[] array = new long[2048];
			array[0] = -6917537823734113282L;
			array[1] = -576460752571858945L;
			for (int i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x0007A858 File Offset: 0x00078A58
		private static long[] mk_tokenSet_8_()
		{
			long[] array = new long[2048];
			array[0] = -2L;
			array[1] = -2882303761785552897L;
			for (int i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x0007A8C4 File Offset: 0x00078AC4
		private static long[] mk_tokenSet_9_()
		{
			long[] array = new long[2048];
			array[0] = -17179869186L;
			array[1] = -268435457L;
			for (int i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
			{
				array[j] = 0L;
			}
			return array;
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x0007A930 File Offset: 0x00078B30
		private static long[] mk_tokenSet_10_()
		{
			long[] array = new long[2048];
			array[0] = -3298534883330L;
			array[1] = -268435457L;
			for (int i = 2; i <= 1022; i++)
			{
				array[i] = -1L;
			}
			array[1023] = long.MaxValue;
			for (int j = 1024; j <= 2047; j++)
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
		public static readonly BitSet tokenSet_0_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_0_());

		// Token: 0x04000E10 RID: 3600
		public static readonly BitSet tokenSet_1_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_1_());

		// Token: 0x04000E11 RID: 3601
		public static readonly BitSet tokenSet_2_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_2_());

		// Token: 0x04000E12 RID: 3602
		public static readonly BitSet tokenSet_3_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_3_());

		// Token: 0x04000E13 RID: 3603
		public static readonly BitSet tokenSet_4_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_4_());

		// Token: 0x04000E14 RID: 3604
		public static readonly BitSet tokenSet_5_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_5_());

		// Token: 0x04000E15 RID: 3605
		public static readonly BitSet tokenSet_6_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_6_());

		// Token: 0x04000E16 RID: 3606
		public static readonly BitSet tokenSet_7_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_7_());

		// Token: 0x04000E17 RID: 3607
		public static readonly BitSet tokenSet_8_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_8_());

		// Token: 0x04000E18 RID: 3608
		public static readonly BitSet tokenSet_9_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_9_());

		// Token: 0x04000E19 RID: 3609
		public static readonly BitSet tokenSet_10_ = new BitSet(AngleBracketTemplateLexer.mk_tokenSet_10_());
	}
}
