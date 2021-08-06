﻿using System.Collections;
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
			this.self.Error("$...$ chunk lexer error", e);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x0007C1C8 File Offset: 0x0007A3C8
		protected bool upcomingELSE(int i)
		{
			return this.LA(i) == '$' && this.LA(i + 1) == 'e' && this.LA(i + 2) == 'l' && this.LA(i + 3) == 's' && this.LA(i + 4) == 'e' && this.LA(i + 5) == '$';
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0007C224 File Offset: 0x0007A424
		protected bool upcomingENDIF(int i)
		{
			return this.LA(i) == '$' && this.LA(i + 1) == 'e' && this.LA(i + 2) == 'n' && this.LA(i + 3) == 'd' && this.LA(i + 4) == 'i' && this.LA(i + 5) == 'f' && this.LA(i + 6) == '$';
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x0007C28C File Offset: 0x0007A48C
		protected bool upcomingAtEND(int i)
		{
			return this.LA(i) == '$' && this.LA(i + 1) == '@' && this.LA(i + 2) == 'e' && this.LA(i + 3) == 'n' && this.LA(i + 4) == 'd' && this.LA(i + 5) == '$';
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x0007C2E8 File Offset: 0x0007A4E8
		protected bool upcomingNewline(int i)
		{
			return (this.LA(i) == '\r' && this.LA(i + 1) == '\n') || this.LA(i) == '\n';
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
			this.initialize();
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x0007C350 File Offset: 0x0007A550
		private void initialize()
		{
			this.caseSensitiveLiterals = true;
			this.setCaseSensitive(true);
			this.literals = new Hashtable(100, 0.4f, null, Comparer.Default);
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x0007C378 File Offset: 0x0007A578
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
							if (cached_LA != '$')
							{
								if (DefaultTemplateLexer.tokenSet_0_.member((int)this.cached_LA1) && this.cached_LA1 != '\r' && this.cached_LA1 != '\n')
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

		// Token: 0x0600113D RID: 4413 RVA: 0x0007C4C4 File Offset: 0x0007A6C4
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
				if (this.cached_LA1 == '\\' && this.cached_LA2 == '$')
				{
					int length3 = this.text.Length;
					this.match('\\');
					this.text.Length = length3;
					this.match('$');
				}
				else if (this.cached_LA1 == '\\' && this.cached_LA2 == '\\')
				{
					int length4 = this.text.Length;
					this.match('\\');
					this.text.Length = length4;
					this.match('\\');
				}
				else if (this.cached_LA1 == '\\' && DefaultTemplateLexer.tokenSet_1_.member((int)this.cached_LA2))
				{
					this.match('\\');
					this.matchNot('$');
				}
				else if (this.cached_LA1 == '\t' || this.cached_LA1 == ' ')
				{
					this.mINDENT(true);
					IToken returnToken_ = this.returnToken_;
					if (column == 1 && this.cached_LA1 == '$')
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
					if (!DefaultTemplateLexer.tokenSet_0_.member((int)this.cached_LA1))
					{
						break;
					}
					this.match(DefaultTemplateLexer.tokenSet_0_);
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

		// Token: 0x0600113E RID: 4414 RVA: 0x0007C6DC File Offset: 0x0007A8DC
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

		// Token: 0x0600113F RID: 4415 RVA: 0x0007C788 File Offset: 0x0007A988
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

		// Token: 0x06001140 RID: 4416 RVA: 0x0007C830 File Offset: 0x0007AA30
		public void mACTION(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 6;
			int column = this.getColumn();
			if (this.cached_LA1 == '$' && this.cached_LA2 == '\\' && this.LA(3) == 'n' && this.LA(4) == '$')
			{
				int length2 = this.text.Length;
				this.match("$\\n$");
				this.text.Length = length2;
				this.text.Length = length;
				this.text.Append('\n');
				num = 4;
			}
			else if (this.cached_LA1 == '$' && this.cached_LA2 == '\\' && this.LA(3) == 'r' && this.LA(4) == '$')
			{
				int length3 = this.text.Length;
				this.match("$\\r$");
				this.text.Length = length3;
				this.text.Length = length;
				this.text.Append('\r');
				num = 4;
			}
			else if (this.cached_LA1 == '$' && this.cached_LA2 == '\\' && this.LA(3) == 't' && this.LA(4) == '$')
			{
				int length4 = this.text.Length;
				this.match("$\\t$");
				this.text.Length = length4;
				this.text.Length = length;
				this.text.Append('\t');
				num = 4;
			}
			else if (this.cached_LA1 == '$' && this.cached_LA2 == '\\' && this.LA(3) == ' ' && this.LA(4) == '$')
			{
				int length5 = this.text.Length;
				this.match("$\\ $");
				this.text.Length = length5;
				this.text.Length = length;
				this.text.Append(' ');
				num = 4;
			}
			else if (this.cached_LA1 == '$' && this.cached_LA2 == '!' && this.LA(3) >= '\u0001' && this.LA(3) <= '￾' && this.LA(4) >= '\u0001' && this.LA(4) <= '￾')
			{
				this.mCOMMENT(false);
				num = Token.SKIP;
			}
			else
			{
				if (this.cached_LA1 != '$' || !DefaultTemplateLexer.tokenSet_1_.member((int)this.cached_LA2) || this.LA(3) < '\u0001' || this.LA(3) > '￾')
				{
					throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
				}
				if (this.cached_LA1 == '$' && this.cached_LA2 == 'i' && this.LA(3) == 'f' && (this.LA(4) == ' ' || this.LA(4) == '(') && DefaultTemplateLexer.tokenSet_2_.member((int)this.LA(5)) && this.LA(6) >= '\u0001' && this.LA(6) <= '￾' && this.LA(7) >= '\u0001' && this.LA(7) <= '￾')
				{
					int length6 = this.text.Length;
					this.match('$');
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
					this.match('$');
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
				else if (this.cached_LA1 == '$' && this.cached_LA2 == 'e' && this.LA(3) == 'n' && this.LA(4) == 'd' && this.LA(5) == 'i' && this.LA(6) == 'f' && this.LA(7) == '$')
				{
					int length7 = this.text.Length;
					this.match('$');
					this.text.Length = length7;
					this.match("endif");
					length7 = this.text.Length;
					this.match('$');
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
				else if (this.cached_LA1 == '$' && this.cached_LA2 == 'e' && this.LA(3) == 'l' && this.LA(4) == 's' && this.LA(5) == 'e' && this.LA(6) == '$')
				{
					int length8 = this.text.Length;
					this.match('$');
					this.text.Length = length8;
					this.match("else");
					length8 = this.text.Length;
					this.match('$');
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
				else if (this.cached_LA1 == '$' && this.cached_LA2 == '@' && DefaultTemplateLexer.tokenSet_3_.member((int)this.LA(3)) && this.LA(4) >= '\u0001' && this.LA(4) <= '￾' && this.LA(5) >= '\u0001' && this.LA(5) <= '￾' && this.LA(6) >= '\u0001' && this.LA(6) <= '￾')
				{
					int length9 = this.text.Length;
					this.match('$');
					this.text.Length = length9;
					length9 = this.text.Length;
					this.match('@');
					this.text.Length = length9;
					int num2 = 0;
					while (DefaultTemplateLexer.tokenSet_3_.member((int)this.cached_LA1))
					{
						this.match(DefaultTemplateLexer.tokenSet_3_);
						num2++;
					}
					if (num2 < 1)
					{
						throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
					}
					char cached_LA4 = this.cached_LA1;
					if (cached_LA4 != '$')
					{
						if (cached_LA4 != '(')
						{
							throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
						}
						length9 = this.text.Length;
						this.match("()");
						this.text.Length = length9;
						length9 = this.text.Length;
						this.match('$');
						this.text.Length = length9;
						num = 10;
					}
					else
					{
						length9 = this.text.Length;
						this.match('$');
						this.text.Length = length9;
						num = 11;
						string text = this.text.ToString(length, this.text.Length - length);
						this.text.Length = length;
						this.text.Append($"{text}::=");
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
						if (this.cached_LA1 == '$' && this.cached_LA2 == '@')
						{
							length9 = this.text.Length;
							this.match("$@end$");
							this.text.Length = length9;
						}
						else
						{
							if (this.cached_LA1 < '\u0001' || this.cached_LA1 > '￾')
							{
								throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
							}
							this.matchNot(1);
							this.self.Error($"missing region {text} $@end$ tag");
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
				}
				else
				{
					if (this.cached_LA1 != '$' || !DefaultTemplateLexer.tokenSet_1_.member((int)this.cached_LA2) || this.LA(3) < '\u0001' || this.LA(3) > '￾')
					{
						throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
					}
					int length10 = this.text.Length;
					this.match('$');
					this.text.Length = length10;
					this.mEXPR(false);
					length10 = this.text.Length;
					this.match('$');
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

		// Token: 0x06001141 RID: 4417 RVA: 0x0007D6DC File Offset: 0x0007B8DC
		protected void mCOMMENT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 19;
			int column = this.getColumn();
			this.match("$!");
			while (this.cached_LA1 != '!' || this.cached_LA2 != '$')
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
			this.match("!$");
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

		// Token: 0x06001142 RID: 4418 RVA: 0x0007D8B0 File Offset: 0x0007BAB0
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
				if (DefaultTemplateLexer.tokenSet_4_.member((int)this.cached_LA1))
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

		// Token: 0x06001143 RID: 4419 RVA: 0x0007D9F0 File Offset: 0x0007BBF0
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
				if ((this.cached_LA1 == '+' || this.cached_LA1 == '=') && DefaultTemplateLexer.tokenSet_5_.member((int)this.cached_LA2))
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
					this.match(DefaultTemplateLexer.tokenSet_5_);
					goto IL_245;
				}
				if (DefaultTemplateLexer.tokenSet_6_.member((int)this.cached_LA1))
				{
					this.matchNot('$');
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

		// Token: 0x06001144 RID: 4420 RVA: 0x0007DC88 File Offset: 0x0007BE88
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

		// Token: 0x06001145 RID: 4421 RVA: 0x0007DCF4 File Offset: 0x0007BEF4
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
						if (!DefaultTemplateLexer.tokenSet_7_.member((int)this.cached_LA1))
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

		// Token: 0x06001146 RID: 4422 RVA: 0x0007DDA0 File Offset: 0x0007BFA0
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
						if (!DefaultTemplateLexer.tokenSet_8_.member((int)this.cached_LA1))
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

		// Token: 0x06001147 RID: 4423 RVA: 0x0007E298 File Offset: 0x0007C498
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
						if (!DefaultTemplateLexer.tokenSet_9_.member((int)this.cached_LA1))
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

		// Token: 0x06001148 RID: 4424 RVA: 0x0007E370 File Offset: 0x0007C570
		private static long[] mk_tokenSet_0_()
		{
			long[] array = new long[2048];
			array[0] = -68719485954L;
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

		// Token: 0x06001149 RID: 4425 RVA: 0x0007E3D4 File Offset: 0x0007C5D4
		private static long[] mk_tokenSet_1_()
		{
			long[] array = new long[2048];
			array[0] = -68719476738L;
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

		// Token: 0x0600114A RID: 4426 RVA: 0x0007E438 File Offset: 0x0007C638
		private static long[] mk_tokenSet_2_()
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

		// Token: 0x0600114B RID: 4427 RVA: 0x0007E49C File Offset: 0x0007C69C
		private static long[] mk_tokenSet_3_()
		{
			long[] array = new long[2048];
			array[0] = -1168231104514L;
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

		// Token: 0x0600114C RID: 4428 RVA: 0x0007E500 File Offset: 0x0007C700
		private static long[] mk_tokenSet_4_()
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

		// Token: 0x0600114D RID: 4429 RVA: 0x0007E570 File Offset: 0x0007C770
		private static long[] mk_tokenSet_5_()
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

		// Token: 0x0600114E RID: 4430 RVA: 0x0007E5E0 File Offset: 0x0007C7E0
		private static long[] mk_tokenSet_6_()
		{
			long[] array = new long[2048];
			array[0] = -2305851874026202114L;
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

		// Token: 0x0600114F RID: 4431 RVA: 0x0007E650 File Offset: 0x0007C850
		private static long[] mk_tokenSet_7_()
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

		// Token: 0x06001150 RID: 4432 RVA: 0x0007E6BC File Offset: 0x0007C8BC
		private static long[] mk_tokenSet_8_()
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

		// Token: 0x06001151 RID: 4433 RVA: 0x0007E728 File Offset: 0x0007C928
		private static long[] mk_tokenSet_9_()
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
		public static readonly BitSet tokenSet_0_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_0_());

		// Token: 0x04000E62 RID: 3682
		public static readonly BitSet tokenSet_1_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_1_());

		// Token: 0x04000E63 RID: 3683
		public static readonly BitSet tokenSet_2_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_2_());

		// Token: 0x04000E64 RID: 3684
		public static readonly BitSet tokenSet_3_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_3_());

		// Token: 0x04000E65 RID: 3685
		public static readonly BitSet tokenSet_4_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_4_());

		// Token: 0x04000E66 RID: 3686
		public static readonly BitSet tokenSet_5_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_5_());

		// Token: 0x04000E67 RID: 3687
		public static readonly BitSet tokenSet_6_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_6_());

		// Token: 0x04000E68 RID: 3688
		public static readonly BitSet tokenSet_7_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_7_());

		// Token: 0x04000E69 RID: 3689
		public static readonly BitSet tokenSet_8_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_8_());

		// Token: 0x04000E6A RID: 3690
		public static readonly BitSet tokenSet_9_ = new BitSet(DefaultTemplateLexer.mk_tokenSet_9_());
	}
}
