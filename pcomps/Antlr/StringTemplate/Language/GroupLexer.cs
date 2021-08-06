using System.Collections;
using System.IO;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000242 RID: 578
	public class GroupLexer : CharScanner, TokenStream
	{
		// Token: 0x0600115A RID: 4442 RVA: 0x0007E980 File Offset: 0x0007CB80
		public GroupLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x0007E990 File Offset: 0x0007CB90
		public GroupLexer(TextReader r) : this(new CharBuffer(r))
		{
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x0007E9A0 File Offset: 0x0007CBA0
		public GroupLexer(InputBuffer ib) : this(new LexerSharedInputState(ib))
		{
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x0007E9B0 File Offset: 0x0007CBB0
		public GroupLexer(LexerSharedInputState state) : base(state)
		{
			this.initialize();
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0007E9C0 File Offset: 0x0007CBC0
		private void initialize()
		{
			this.caseSensitiveLiterals = true;
			this.setCaseSensitive(true);
			this.literals = new Hashtable(100, 0.4f, null, Comparer.Default);
			this.literals.Add("group", 4);
			this.literals.Add("implements", 7);
			this.literals.Add("default", 21);
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x0007EA38 File Offset: 0x0007CC38
		public override IToken nextToken()
		{
			IToken returnToken_22;
			for (;;)
			{
				this.resetText();
				try
				{
					try
					{
						switch (this.cached_LA1)
						{
						case '\t':
						case '\n':
						case '\f':
						case '\r':
						case ' ':
						{
							this.mWS(true);
							IToken returnToken_ = this.returnToken_;
							goto IL_3F7;
						}
						case '"':
						{
							this.mSTRING(true);
							IToken returnToken_2 = this.returnToken_;
							goto IL_3F7;
						}
						case '(':
						{
							this.mLPAREN(true);
							IToken returnToken_3 = this.returnToken_;
							goto IL_3F7;
						}
						case ')':
						{
							this.mRPAREN(true);
							IToken returnToken_4 = this.returnToken_;
							goto IL_3F7;
						}
						case '*':
						{
							this.mSTAR(true);
							IToken returnToken_5 = this.returnToken_;
							goto IL_3F7;
						}
						case '+':
						{
							this.mPLUS(true);
							IToken returnToken_6 = this.returnToken_;
							goto IL_3F7;
						}
						case ',':
						{
							this.mCOMMA(true);
							IToken returnToken_7 = this.returnToken_;
							goto IL_3F7;
						}
						case '.':
						{
							this.mDOT(true);
							IToken returnToken_8 = this.returnToken_;
							goto IL_3F7;
						}
						case ';':
						{
							this.mSEMI(true);
							IToken returnToken_9 = this.returnToken_;
							goto IL_3F7;
						}
						case '<':
						{
							this.mBIGSTRING(true);
							IToken returnToken_10 = this.returnToken_;
							goto IL_3F7;
						}
						case '=':
						{
							this.mASSIGN(true);
							IToken returnToken_11 = this.returnToken_;
							goto IL_3F7;
						}
						case '?':
						{
							this.mOPTIONAL(true);
							IToken returnToken_12 = this.returnToken_;
							goto IL_3F7;
						}
						case '@':
						{
							this.mAT(true);
							IToken returnToken_13 = this.returnToken_;
							goto IL_3F7;
						}
						case 'A':
						case 'B':
						case 'C':
						case 'D':
						case 'E':
						case 'F':
						case 'G':
						case 'H':
						case 'I':
						case 'J':
						case 'K':
						case 'L':
						case 'M':
						case 'N':
						case 'O':
						case 'P':
						case 'Q':
						case 'R':
						case 'S':
						case 'T':
						case 'U':
						case 'V':
						case 'W':
						case 'X':
						case 'Y':
						case 'Z':
						case '_':
						case 'a':
						case 'b':
						case 'c':
						case 'd':
						case 'e':
						case 'f':
						case 'g':
						case 'h':
						case 'i':
						case 'j':
						case 'k':
						case 'l':
						case 'm':
						case 'n':
						case 'o':
						case 'p':
						case 'q':
						case 'r':
						case 's':
						case 't':
						case 'u':
						case 'v':
						case 'w':
						case 'x':
						case 'y':
						case 'z':
						{
							this.mID(true);
							IToken returnToken_14 = this.returnToken_;
							goto IL_3F7;
						}
						case '[':
						{
							this.mLBRACK(true);
							IToken returnToken_15 = this.returnToken_;
							goto IL_3F7;
						}
						case ']':
						{
							this.mRBRACK(true);
							IToken returnToken_16 = this.returnToken_;
							goto IL_3F7;
						}
						case '{':
						{
							this.mANONYMOUS_TEMPLATE(true);
							IToken returnToken_17 = this.returnToken_;
							goto IL_3F7;
						}
						}
						if (this.cached_LA1 == ':' && this.cached_LA2 == ':')
						{
							this.mDEFINED_TO_BE(true);
							IToken returnToken_18 = this.returnToken_;
						}
						else if (this.cached_LA1 == '/' && this.cached_LA2 == '/')
						{
							this.mSL_COMMENT(true);
							IToken returnToken_19 = this.returnToken_;
						}
						else if (this.cached_LA1 == '/' && this.cached_LA2 == '*')
						{
							this.mML_COMMENT(true);
							IToken returnToken_20 = this.returnToken_;
						}
						else if (this.cached_LA1 == ':')
						{
							this.mCOLON(true);
							IToken returnToken_21 = this.returnToken_;
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
						IL_3F7:
						if (this.returnToken_ == null)
						{
							continue;
						}
						int num = this.returnToken_.Type;
						num = this.testLiteralsTable(num);
						this.returnToken_.Type = num;
						returnToken_22 = this.returnToken_;
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
			return returnToken_22;
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x0007EED8 File Offset: 0x0007D0D8
		public void mID(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 5;
			switch (this.cached_LA1)
			{
			case 'A':
			case 'B':
			case 'C':
			case 'D':
			case 'E':
			case 'F':
			case 'G':
			case 'H':
			case 'I':
			case 'J':
			case 'K':
			case 'L':
			case 'M':
			case 'N':
			case 'O':
			case 'P':
			case 'Q':
			case 'R':
			case 'S':
			case 'T':
			case 'U':
			case 'V':
			case 'W':
			case 'X':
			case 'Y':
			case 'Z':
				this.matchRange('A', 'Z');
				break;
			case '[':
			case '\\':
			case ']':
			case '^':
			case '`':
				goto IL_12C;
			case '_':
				this.match('_');
				break;
			case 'a':
			case 'b':
			case 'c':
			case 'd':
			case 'e':
			case 'f':
			case 'g':
			case 'h':
			case 'i':
			case 'j':
			case 'k':
			case 'l':
			case 'm':
			case 'n':
			case 'o':
			case 'p':
			case 'q':
			case 'r':
			case 's':
			case 't':
			case 'u':
			case 'v':
			case 'w':
			case 'x':
			case 'y':
			case 'z':
				this.matchRange('a', 'z');
				break;
			default:
				goto IL_12C;
			}
			for (;;)
			{
				switch (this.cached_LA1)
				{
				case '-':
					this.match('-');
					continue;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					this.matchRange('0', '9');
					continue;
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
				case 'G':
				case 'H':
				case 'I':
				case 'J':
				case 'K':
				case 'L':
				case 'M':
				case 'N':
				case 'O':
				case 'P':
				case 'Q':
				case 'R':
				case 'S':
				case 'T':
				case 'U':
				case 'V':
				case 'W':
				case 'X':
				case 'Y':
				case 'Z':
					this.matchRange('A', 'Z');
					continue;
				case '_':
					this.match('_');
					continue;
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
				case 'g':
				case 'h':
				case 'i':
				case 'j':
				case 'k':
				case 'l':
				case 'm':
				case 'n':
				case 'o':
				case 'p':
				case 'q':
				case 'r':
				case 's':
				case 't':
				case 'u':
				case 'v':
				case 'w':
				case 'x':
				case 'y':
				case 'z':
					this.matchRange('a', 'z');
					continue;
				}
				break;
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
			return;
			IL_12C:
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x0007F200 File Offset: 0x0007D400
		public void mSTRING(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 15;
			int length2 = this.text.Length;
			this.match('"');
			this.text.Length = length2;
			for (;;)
			{
				if (this.cached_LA1 == '\\' && this.cached_LA2 == '"')
				{
					length2 = this.text.Length;
					this.match('\\');
					this.text.Length = length2;
					this.match('"');
				}
				else if (this.cached_LA1 == '\\' && GroupLexer.tokenSet_0_.member((int)this.cached_LA2))
				{
					this.match('\\');
					this.matchNot('"');
				}
				else
				{
					if (!GroupLexer.tokenSet_1_.member((int)this.cached_LA1))
					{
						break;
					}
					this.matchNot('"');
				}
			}
			length2 = this.text.Length;
			this.match('"');
			this.text.Length = length2;
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0007F328 File Offset: 0x0007D528
		public void mBIGSTRING(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 16;
			int length2 = this.text.Length;
			this.match("<<");
			this.text.Length = length2;
			if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\0' && this.cached_LA2 <= '￾')
			{
				char cached_LA = this.cached_LA1;
				if (cached_LA != '\n')
				{
					if (cached_LA != '\r')
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
			else if (this.cached_LA1 < '\0' || this.cached_LA1 > '￾' || this.cached_LA2 < '\0' || this.cached_LA2 > '￾')
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			while (this.cached_LA1 != '>' || this.cached_LA2 != '>')
			{
				if (this.cached_LA1 == '\r' && this.cached_LA2 == '\n' && this.LA(3) == '>' && this.LA(4) == '>')
				{
					length2 = this.text.Length;
					this.match('\r');
					this.text.Length = length2;
					length2 = this.text.Length;
					this.match('\n');
					this.text.Length = length2;
					this.newline();
				}
				else if (this.cached_LA1 == '\n' && this.cached_LA2 >= '\0' && this.cached_LA2 <= '￾' && this.LA(2) == '>' && this.LA(3) == '>')
				{
					length2 = this.text.Length;
					this.match('\n');
					this.text.Length = length2;
					this.newline();
				}
				else if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\0' && this.cached_LA2 <= '￾')
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
				else if (this.cached_LA1 == '\\' && this.cached_LA2 == '>')
				{
					length2 = this.text.Length;
					this.match('\\');
					this.text.Length = length2;
					this.match('>');
				}
				else
				{
					if (this.cached_LA1 < '\0' || this.cached_LA1 > '￾' || this.cached_LA2 < '\0' || this.cached_LA2 > '￾')
					{
						break;
					}
					this.matchNot(1);
				}
			}
			length2 = this.text.Length;
			this.match(">>");
			this.text.Length = length2;
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x0007F6A0 File Offset: 0x0007D8A0
		public void mANONYMOUS_TEMPLATE(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 18;
			int length2 = this.text.Length;
			this.match('{');
			this.text.Length = length2;
			while (this.cached_LA1 != '}')
			{
				if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\0' && this.cached_LA2 <= '￾')
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
				else if (this.cached_LA1 == '\\' && this.cached_LA2 == '}')
				{
					length2 = this.text.Length;
					this.match('\\');
					this.text.Length = length2;
					this.match('}');
				}
				else
				{
					if (this.cached_LA1 < '\0' || this.cached_LA1 > '￾' || this.cached_LA2 < '\0' || this.cached_LA2 > '￾')
					{
						break;
					}
					this.matchNot(1);
				}
			}
			length2 = this.text.Length;
			this.match('}');
			this.text.Length = length2;
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0007F83C File Offset: 0x0007DA3C
		public void mAT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 10;
			this.match('@');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0007F8A0 File Offset: 0x0007DAA0
		public void mLPAREN(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 12;
			this.match('(');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x0007F904 File Offset: 0x0007DB04
		public void mRPAREN(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 13;
			this.match(')');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x0007F968 File Offset: 0x0007DB68
		public void mLBRACK(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 19;
			this.match('[');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0007F9CC File Offset: 0x0007DBCC
		public void mRBRACK(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 20;
			this.match(']');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x0007FA30 File Offset: 0x0007DC30
		public void mCOMMA(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 8;
			this.match(',');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x0007FA94 File Offset: 0x0007DC94
		public void mDOT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 11;
			this.match('.');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x0007FAF8 File Offset: 0x0007DCF8
		public void mDEFINED_TO_BE(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 14;
			this.match("::=");
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x0007FB60 File Offset: 0x0007DD60
		public void mSEMI(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 9;
			this.match(';');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x0007FBC4 File Offset: 0x0007DDC4
		public void mCOLON(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 6;
			this.match(':');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x0007FC28 File Offset: 0x0007DE28
		public void mSTAR(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 22;
			this.match('*');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x0007FC8C File Offset: 0x0007DE8C
		public void mPLUS(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 23;
			this.match('+');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x0007FCF0 File Offset: 0x0007DEF0
		public void mASSIGN(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 17;
			this.match('=');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x0007FD54 File Offset: 0x0007DF54
		public void mOPTIONAL(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 24;
			this.match('?');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x0007FDB8 File Offset: 0x0007DFB8
		public void mSL_COMMENT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			this.match("//");
			while (GroupLexer.tokenSet_2_.member((int)this.cached_LA1))
			{
				this.match(GroupLexer.tokenSet_2_);
			}
			if (this.cached_LA1 == '\n' || this.cached_LA1 == '\r')
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
			}
			int skip = Token.SKIP;
			this.newline();
			if (_createToken && token == null && skip != Token.SKIP)
			{
				token = this.makeToken(skip);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0007FEA0 File Offset: 0x0007E0A0
		public void mML_COMMENT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			this.match("/*");
			while (this.cached_LA1 != '*' || this.cached_LA2 != '/')
			{
				if ((this.cached_LA1 == '\n' || this.cached_LA1 == '\r') && this.cached_LA2 >= '\0' && this.cached_LA2 <= '￾')
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
					if (this.cached_LA1 < '\0' || this.cached_LA1 > '￾' || this.cached_LA2 < '\0' || this.cached_LA2 > '￾')
					{
						break;
					}
					this.matchNot(1);
				}
			}
			this.match("*/");
			int skip = Token.SKIP;
			if (_createToken && token == null && skip != Token.SKIP)
			{
				token = this.makeToken(skip);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x0007FFDC File Offset: 0x0007E1DC
		public void mWS(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 0;
			for (;;)
			{
				char cached_LA = this.cached_LA1;
				switch (cached_LA)
				{
				case '\t':
					this.match('\t');
					break;
				case '\n':
				case '\r':
				{
					char cached_LA2 = this.cached_LA1;
					if (cached_LA2 != '\n')
					{
						if (cached_LA2 != '\r')
						{
							goto IL_7E;
						}
						this.match('\r');
					}
					this.match('\n');
					this.newline();
					break;
				}
				case '\v':
					goto IL_AC;
				case '\f':
					this.match('\f');
					break;
				default:
					if (cached_LA != ' ')
					{
						goto IL_AC;
					}
					this.match(' ');
					break;
				}
				num++;
			}
			IL_7E:
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_AC:
			if (num < 1)
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			int skip = Token.SKIP;
			if (_createToken && token == null && skip != Token.SKIP)
			{
				token = this.makeToken(skip);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x00080104 File Offset: 0x0007E304
		private static long[] mk_tokenSet_0_()
		{
			long[] array = new long[2048];
			array[0] = -17179869185L;
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

		// Token: 0x06001176 RID: 4470 RVA: 0x00080168 File Offset: 0x0007E368
		private static long[] mk_tokenSet_1_()
		{
			long[] array = new long[2048];
			array[0] = -17179869185L;
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

		// Token: 0x06001177 RID: 4471 RVA: 0x000801D4 File Offset: 0x0007E3D4
		private static long[] mk_tokenSet_2_()
		{
			long[] array = new long[2048];
			array[0] = -9217L;
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

		// Token: 0x04000E73 RID: 3699
		public const int EOF = 1;

		// Token: 0x04000E74 RID: 3700
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000E75 RID: 3701
		public const int LITERAL_group = 4;

		// Token: 0x04000E76 RID: 3702
		public const int ID = 5;

		// Token: 0x04000E77 RID: 3703
		public const int COLON = 6;

		// Token: 0x04000E78 RID: 3704
		public const int LITERAL_implements = 7;

		// Token: 0x04000E79 RID: 3705
		public const int COMMA = 8;

		// Token: 0x04000E7A RID: 3706
		public const int SEMI = 9;

		// Token: 0x04000E7B RID: 3707
		public const int AT = 10;

		// Token: 0x04000E7C RID: 3708
		public const int DOT = 11;

		// Token: 0x04000E7D RID: 3709
		public const int LPAREN = 12;

		// Token: 0x04000E7E RID: 3710
		public const int RPAREN = 13;

		// Token: 0x04000E7F RID: 3711
		public const int DEFINED_TO_BE = 14;

		// Token: 0x04000E80 RID: 3712
		public const int STRING = 15;

		// Token: 0x04000E81 RID: 3713
		public const int BIGSTRING = 16;

		// Token: 0x04000E82 RID: 3714
		public const int ASSIGN = 17;

		// Token: 0x04000E83 RID: 3715
		public const int ANONYMOUS_TEMPLATE = 18;

		// Token: 0x04000E84 RID: 3716
		public const int LBRACK = 19;

		// Token: 0x04000E85 RID: 3717
		public const int RBRACK = 20;

		// Token: 0x04000E86 RID: 3718
		public const int LITERAL_default = 21;

		// Token: 0x04000E87 RID: 3719
		public const int STAR = 22;

		// Token: 0x04000E88 RID: 3720
		public const int PLUS = 23;

		// Token: 0x04000E89 RID: 3721
		public const int OPTIONAL = 24;

		// Token: 0x04000E8A RID: 3722
		public const int SL_COMMENT = 25;

		// Token: 0x04000E8B RID: 3723
		public const int ML_COMMENT = 26;

		// Token: 0x04000E8C RID: 3724
		public const int WS = 27;

		// Token: 0x04000E8D RID: 3725
		public static readonly BitSet tokenSet_0_ = new BitSet(GroupLexer.mk_tokenSet_0_());

		// Token: 0x04000E8E RID: 3726
		public static readonly BitSet tokenSet_1_ = new BitSet(GroupLexer.mk_tokenSet_1_());

		// Token: 0x04000E8F RID: 3727
		public static readonly BitSet tokenSet_2_ = new BitSet(GroupLexer.mk_tokenSet_2_());
	}
}
