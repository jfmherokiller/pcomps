using System.Collections;
using System.IO;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000234 RID: 564
	public class ActionLexer : CharScanner, TokenStream
	{
		// Token: 0x06001082 RID: 4226 RVA: 0x000739D8 File Offset: 0x00071BD8
		public ActionLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x000739E8 File Offset: 0x00071BE8
		public ActionLexer(TextReader r) : this(new CharBuffer(r))
		{
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x000739F8 File Offset: 0x00071BF8
		public ActionLexer(InputBuffer ib) : this(new LexerSharedInputState(ib))
		{
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x00073A08 File Offset: 0x00071C08
		public ActionLexer(LexerSharedInputState state) : base(state)
		{
			this.initialize();
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x00073A18 File Offset: 0x00071C18
		private void initialize()
		{
			this.caseSensitiveLiterals = true;
			this.setCaseSensitive(true);
			this.literals = new Hashtable(100, 0.4f, null, Comparer.Default);
			this.literals.Add("if", 8);
			this.literals.Add("rest", 26);
			this.literals.Add("last", 27);
			this.literals.Add("length", 28);
			this.literals.Add("strip", 29);
			this.literals.Add("trunc", 30);
			this.literals.Add("first", 25);
			this.literals.Add("super", 23);
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x00073B04 File Offset: 0x00071D04
		public override IToken nextToken()
		{
			IToken returnToken_18;
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
						case '\r':
						case ' ':
						{
							this.mWS(true);
							IToken returnToken_ = this.returnToken_;
							goto IL_383;
						}
						case '!':
						{
							this.mNOT(true);
							IToken returnToken_2 = this.returnToken_;
							goto IL_383;
						}
						case '"':
						{
							this.mSTRING(true);
							IToken returnToken_3 = this.returnToken_;
							goto IL_383;
						}
						case '(':
						{
							this.mLPAREN(true);
							IToken returnToken_4 = this.returnToken_;
							goto IL_383;
						}
						case ')':
						{
							this.mRPAREN(true);
							IToken returnToken_5 = this.returnToken_;
							goto IL_383;
						}
						case '+':
						{
							this.mPLUS(true);
							IToken returnToken_6 = this.returnToken_;
							goto IL_383;
						}
						case ',':
						{
							this.mCOMMA(true);
							IToken returnToken_7 = this.returnToken_;
							goto IL_383;
						}
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
						{
							this.mINT(true);
							IToken returnToken_8 = this.returnToken_;
							goto IL_383;
						}
						case ':':
						{
							this.mCOLON(true);
							IToken returnToken_9 = this.returnToken_;
							goto IL_383;
						}
						case ';':
						{
							this.mSEMI(true);
							IToken returnToken_10 = this.returnToken_;
							goto IL_383;
						}
						case '=':
						{
							this.mASSIGN(true);
							IToken returnToken_11 = this.returnToken_;
							goto IL_383;
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
							IToken returnToken_12 = this.returnToken_;
							goto IL_383;
						}
						case '[':
						{
							this.mLBRACK(true);
							IToken returnToken_13 = this.returnToken_;
							goto IL_383;
						}
						case ']':
						{
							this.mRBRACK(true);
							IToken returnToken_14 = this.returnToken_;
							goto IL_383;
						}
						case '{':
						{
							this.mANONYMOUS_TEMPLATE(true);
							IToken returnToken_15 = this.returnToken_;
							goto IL_383;
						}
						}
						if (this.cached_LA1 == '.' && this.cached_LA2 == '.')
						{
							this.mDOTDOTDOT(true);
							IToken returnToken_16 = this.returnToken_;
						}
						else if (this.cached_LA1 == '.')
						{
							this.mDOT(true);
							IToken returnToken_17 = this.returnToken_;
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
						IL_383:
						if (this.returnToken_ == null)
						{
							continue;
						}
						int type = this.returnToken_.Type;
						this.returnToken_.Type = type;
						returnToken_18 = this.returnToken_;
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
			return returnToken_18;
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x00073F28 File Offset: 0x00072128
		public void mID(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 18;
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
				goto IL_12D;
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
				goto IL_12D;
			}
			for (;;)
			{
				switch (this.cached_LA1)
				{
				case '/':
					this.match('/');
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
			num = this.testLiteralsTable(num);
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
			return;
			IL_12D:
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x00074250 File Offset: 0x00072450
		public void mINT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 33;
			int num2 = 0;
			while (this.cached_LA1 >= '0' && this.cached_LA1 <= '9')
			{
				this.matchRange('0', '9');
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

		// Token: 0x0600108A RID: 4234 RVA: 0x000742F4 File Offset: 0x000724F4
		public void mSTRING(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 32;
			int length2 = this.text.Length;
			this.match('"');
			this.text.Length = length2;
			for (;;)
			{
				if (this.cached_LA1 == '\\')
				{
					this.mESC_CHAR(false, true);
				}
				else
				{
					if (!ActionLexer.tokenSet_0_.member((int)this.cached_LA1))
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

		// Token: 0x0600108B RID: 4235 RVA: 0x000743C0 File Offset: 0x000725C0
		protected void mESC_CHAR(bool _createToken, bool doEscape)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 39;
			this.match('\\');
			if (this.cached_LA1 == 'n' && this.cached_LA2 >= '\u0003' && this.cached_LA2 <= '￾')
			{
				this.match('n');
				if (this.inputState.guessing == 0 && doEscape)
				{
					this.text.Length = length;
					this.text.Append("\n");
				}
			}
			else if (this.cached_LA1 == 'r' && this.cached_LA2 >= '\u0003' && this.cached_LA2 <= '￾')
			{
				this.match('r');
				if (this.inputState.guessing == 0 && doEscape)
				{
					this.text.Length = length;
					this.text.Append("\r");
				}
			}
			else if (this.cached_LA1 == 't' && this.cached_LA2 >= '\u0003' && this.cached_LA2 <= '￾')
			{
				this.match('t');
				if (this.inputState.guessing == 0 && doEscape)
				{
					this.text.Length = length;
					this.text.Append("\t");
				}
			}
			else if (this.cached_LA1 == 'b' && this.cached_LA2 >= '\u0003' && this.cached_LA2 <= '￾')
			{
				this.match('b');
				if (this.inputState.guessing == 0 && doEscape)
				{
					this.text.Length = length;
					this.text.Append("\b");
				}
			}
			else if (this.cached_LA1 == 'f' && this.cached_LA2 >= '\u0003' && this.cached_LA2 <= '￾')
			{
				this.match('f');
				if (this.inputState.guessing == 0 && doEscape)
				{
					this.text.Length = length;
					this.text.Append("\f");
				}
			}
			else
			{
				if (this.cached_LA1 < '\u0003' || this.cached_LA1 > '￾' || this.cached_LA2 < '\u0003' || this.cached_LA2 > '￾')
				{
					throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
				}
				char cached_LA = this.cached_LA1;
				this.matchNot(1);
				if (this.inputState.guessing == 0 && doEscape)
				{
					this.text.Length = length;
					this.text.Append(cached_LA);
				}
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x00074688 File Offset: 0x00072888
		public void mANONYMOUS_TEMPLATE(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 31;
			StringTemplateToken stringTemplateToken = null;
			int length2 = this.text.Length;
			this.match('{');
			this.text.Length = length2;
			bool flag = false;
			if (ActionLexer.tokenSet_1_.member((int)this.cached_LA1) && ActionLexer.tokenSet_2_.member((int)this.cached_LA2))
			{
				int pos = this.mark();
				flag = true;
				this.inputState.guessing++;
				try
				{
					this.mTEMPLATE_ARGS(false);
				}
				catch (RecognitionException)
				{
					flag = false;
				}
				this.rewind(pos);
				this.inputState.guessing--;
			}
			if (flag)
			{
				IList args = this.mTEMPLATE_ARGS(false);
				if (ActionLexer.tokenSet_3_.member((int)this.cached_LA1) && this.cached_LA2 >= '\u0003' && this.cached_LA2 <= '￾')
				{
					length2 = this.text.Length;
					this.mWS_CHAR(false);
					this.text.Length = length2;
				}
				else if (this.cached_LA1 < '\u0003' || this.cached_LA1 > '￾')
				{
					throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
				}
				if (this.inputState.guessing == 0)
				{
					stringTemplateToken = new StringTemplateToken(31, this.text.ToString(length, this.text.Length - length), args);
					token = stringTemplateToken;
				}
			}
			else if (this.cached_LA1 < '\u0003' || this.cached_LA1 > '￾')
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			for (;;)
			{
				if (this.cached_LA1 == '\\' && this.cached_LA2 == '{')
				{
					length2 = this.text.Length;
					this.match('\\');
					this.text.Length = length2;
					this.match('{');
				}
				else if (this.cached_LA1 == '\\' && this.cached_LA2 == '}')
				{
					length2 = this.text.Length;
					this.match('\\');
					this.text.Length = length2;
					this.match('}');
				}
				else if (this.cached_LA1 == '\\' && this.cached_LA2 >= '\u0003' && this.cached_LA2 <= '￾')
				{
					this.mESC_CHAR(false, false);
				}
				else if (this.cached_LA1 == '{')
				{
					this.mNESTED_ANONYMOUS_TEMPLATE(false);
				}
				else
				{
					if (!ActionLexer.tokenSet_4_.member((int)this.cached_LA1))
					{
						break;
					}
					this.matchNot('}');
				}
			}
			if (this.inputState.guessing == 0 && stringTemplateToken != null)
			{
				stringTemplateToken.setText(this.text.ToString(length, this.text.Length - length));
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

		// Token: 0x0600108D RID: 4237 RVA: 0x000749B8 File Offset: 0x00072BB8
		protected IList mTEMPLATE_ARGS(bool _createToken)
		{
			IList list = new ArrayList();
			IToken token = null;
			int length = this.text.Length;
			int num = 37;
			char cached_LA = this.cached_LA1;
			switch (cached_LA)
			{
			case '\t':
			case '\n':
			case '\r':
				break;
			case '\v':
			case '\f':
				goto IL_1E4;
			default:
				switch (cached_LA)
				{
				case ' ':
					break;
				case '!':
				case '"':
				case '#':
				case '$':
				case '%':
				case '&':
				case '\'':
				case '(':
				case ')':
				case '*':
				case '+':
				case ',':
				case '-':
				case '.':
				case '/':
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
				case ':':
				case ';':
				case '<':
				case '=':
				case '>':
				case '?':
				case '@':
				case '[':
				case '\\':
				case ']':
				case '^':
				case '`':
					goto IL_1E4;
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
					goto IL_202;
				default:
					goto IL_1E4;
				}
				break;
			}
			int length2 = this.text.Length;
			this.mWS_CHAR(false);
			this.text.Length = length2;
			goto IL_202;
			IL_1E4:
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_202:
			length2 = this.text.Length;
			this.mID(true);
			this.text.Length = length2;
			IToken returnToken_ = this.returnToken_;
			if (this.inputState.guessing == 0)
			{
				list.Add(returnToken_.getText());
			}
			while (ActionLexer.tokenSet_5_.member((int)this.cached_LA1) && ActionLexer.tokenSet_6_.member((int)this.cached_LA2))
			{
				char cached_LA2 = this.cached_LA1;
				switch (cached_LA2)
				{
				case '\t':
				case '\n':
				case '\r':
					goto IL_2A7;
				case '\v':
				case '\f':
					goto IL_2CA;
				default:
					if (cached_LA2 == ' ')
					{
						goto IL_2A7;
					}
					if (cached_LA2 != ',')
					{
						goto IL_2CA;
					}
					break;
				}
				IL_2E8:
				length2 = this.text.Length;
				this.match(',');
				this.text.Length = length2;
				char cached_LA3 = this.cached_LA1;
				switch (cached_LA3)
				{
				case '\t':
				case '\n':
				case '\r':
					goto IL_4A8;
				case '\v':
				case '\f':
					goto IL_4CB;
				default:
					switch (cached_LA3)
					{
					case ' ':
						goto IL_4A8;
					case '!':
					case '"':
					case '#':
					case '$':
					case '%':
					case '&':
					case '\'':
					case '(':
					case ')':
					case '*':
					case '+':
					case ',':
					case '-':
					case '.':
					case '/':
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
					case ':':
					case ';':
					case '<':
					case '=':
					case '>':
					case '?':
					case '@':
					case '[':
					case '\\':
					case ']':
					case '^':
					case '`':
						goto IL_4CB;
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
						break;
					default:
						goto IL_4CB;
					}
					break;
				}
				IL_4E9:
				length2 = this.text.Length;
				this.mID(true);
				this.text.Length = length2;
				IToken returnToken_2 = this.returnToken_;
				if (this.inputState.guessing == 0)
				{
					list.Add(returnToken_2.getText());
					continue;
				}
				continue;
				IL_4A8:
				length2 = this.text.Length;
				this.mWS_CHAR(false);
				this.text.Length = length2;
				goto IL_4E9;
				IL_4CB:
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
				IL_2A7:
				length2 = this.text.Length;
				this.mWS_CHAR(false);
				this.text.Length = length2;
				goto IL_2E8;
				IL_2CA:
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			char cached_LA4 = this.cached_LA1;
			switch (cached_LA4)
			{
			case '\t':
			case '\n':
			case '\r':
				break;
			case '\v':
			case '\f':
				goto IL_58C;
			default:
				if (cached_LA4 != ' ')
				{
					if (cached_LA4 != '|')
					{
						goto IL_58C;
					}
					goto IL_5AA;
				}
				break;
			}
			length2 = this.text.Length;
			this.mWS_CHAR(false);
			this.text.Length = length2;
			goto IL_5AA;
			IL_58C:
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_5AA:
			length2 = this.text.Length;
			this.match('|');
			this.text.Length = length2;
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
			return list;
		}

		// Token: 0x0600108E RID: 4238 RVA: 0x00074FD0 File Offset: 0x000731D0
		protected void mWS_CHAR(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 41;
			char cached_LA = this.cached_LA1;
			switch (cached_LA)
			{
			case '\t':
				this.match('\t');
				goto IL_93;
			case '\n':
				this.match('\n');
				if (this.inputState.guessing == 0)
				{
					this.newline();
					goto IL_93;
				}
				goto IL_93;
			case '\v':
			case '\f':
				break;
			case '\r':
				this.match('\r');
				goto IL_93;
			default:
				if (cached_LA == ' ')
				{
					this.match(' ');
					goto IL_93;
				}
				break;
			}
			throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			IL_93:
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600108F RID: 4239 RVA: 0x000750AC File Offset: 0x000732AC
		protected void mNESTED_ANONYMOUS_TEMPLATE(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 38;
			this.match('{');
			for (;;)
			{
				if (this.cached_LA1 == '\\' && this.cached_LA2 == '{')
				{
					int length2 = this.text.Length;
					this.match('\\');
					this.text.Length = length2;
					this.match('{');
				}
				else if (this.cached_LA1 == '\\' && this.cached_LA2 == '}')
				{
					int length3 = this.text.Length;
					this.match('\\');
					this.text.Length = length3;
					this.match('}');
				}
				else if (this.cached_LA1 == '\\' && this.cached_LA2 >= '\u0003' && this.cached_LA2 <= '￾')
				{
					this.mESC_CHAR(false, false);
				}
				else if (this.cached_LA1 == '{')
				{
					this.mNESTED_ANONYMOUS_TEMPLATE(false);
				}
				else
				{
					if (!ActionLexer.tokenSet_4_.member((int)this.cached_LA1))
					{
						break;
					}
					this.matchNot('}');
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

		// Token: 0x06001090 RID: 4240 RVA: 0x00075200 File Offset: 0x00073400
		public void mLBRACK(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 34;
			this.match('[');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x00075264 File Offset: 0x00073464
		public void mRBRACK(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 35;
			this.match(']');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x000752C8 File Offset: 0x000734C8
		public void mLPAREN(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 15;
			this.match('(');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x0007532C File Offset: 0x0007352C
		public void mRPAREN(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 16;
			this.match(')');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x00075390 File Offset: 0x00073590
		public void mCOMMA(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 17;
			this.match(',');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x000753F4 File Offset: 0x000735F4
		public void mDOT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 24;
			this.match('.');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x00075458 File Offset: 0x00073658
		public void mASSIGN(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 19;
			this.match('=');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x000754BC File Offset: 0x000736BC
		public void mCOLON(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 20;
			this.match(':');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x00075520 File Offset: 0x00073720
		public void mPLUS(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 22;
			this.match('+');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x00075584 File Offset: 0x00073784
		public void mSEMI(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 14;
			this.match(';');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x000755E8 File Offset: 0x000737E8
		public void mNOT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 21;
			this.match('!');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x0007564C File Offset: 0x0007384C
		public void mDOTDOTDOT(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 36;
			this.match("...");
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = this.makeToken(num);
				token.setText(this.text.ToString(length, this.text.Length - length));
			}
			this.returnToken_ = token;
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x000756B4 File Offset: 0x000738B4
		public void mWS(bool _createToken)
		{
			IToken token = null;
			int length = this.text.Length;
			int num = 40;
			int num2 = 0;
			for (;;)
			{
				char cached_LA = this.cached_LA1;
				switch (cached_LA)
				{
				case '\t':
					this.match('\t');
					break;
				case '\n':
					this.match('\n');
					if (this.inputState.guessing == 0)
					{
						this.newline();
					}
					break;
				case '\v':
				case '\f':
					goto IL_7A;
				case '\r':
					this.match('\r');
					break;
				default:
					if (cached_LA != ' ')
					{
						goto IL_7A;
					}
					this.match(' ');
					break;
				}
				num2++;
			}
			IL_7A:
			if (num2 < 1)
			{
				throw new NoViableAltForCharException(this.cached_LA1, this.getFilename(), this.getLine(), this.getColumn());
			}
			if (this.inputState.guessing == 0)
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

		// Token: 0x0600109D RID: 4253 RVA: 0x000757B8 File Offset: 0x000739B8
		private static long[] mk_tokenSet_0_()
		{
			long[] array = new long[2048];
			array[0] = -17179869192L;
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

		// Token: 0x0600109E RID: 4254 RVA: 0x00075824 File Offset: 0x00073A24
		private static long[] mk_tokenSet_1_()
		{
			long[] array = new long[1025];
			array[0] = 4294977024L;
			array[1] = 576460745995190270L;
			for (int i = 2; i <= 1024; i++)
			{
				array[i] = 0L;
			}
			return array;
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x0007586C File Offset: 0x00073A6C
		private static long[] mk_tokenSet_2_()
		{
			long[] array = new long[1025];
			array[0] = 288107235144377856L;
			array[1] = 1729382250602037246L;
			for (int i = 2; i <= 1024; i++)
			{
				array[i] = 0L;
			}
			return array;
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x000758B4 File Offset: 0x00073AB4
		private static long[] mk_tokenSet_3_()
		{
			long[] array = new long[1025];
			array[0] = 4294977024L;
			for (int i = 1; i <= 1024; i++)
			{
				array[i] = 0L;
			}
			return array;
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x000758F0 File Offset: 0x00073AF0
		private static long[] mk_tokenSet_4_()
		{
			long[] array = new long[2048];
			array[0] = -8L;
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

		// Token: 0x060010A2 RID: 4258 RVA: 0x0007595C File Offset: 0x00073B5C
		private static long[] mk_tokenSet_5_()
		{
			long[] array = new long[1025];
			array[0] = 17596481021440L;
			for (int i = 1; i <= 1024; i++)
			{
				array[i] = 0L;
			}
			return array;
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x00075998 File Offset: 0x00073B98
		private static long[] mk_tokenSet_6_()
		{
			long[] array = new long[1025];
			array[0] = 17596481021440L;
			array[1] = 576460745995190270L;
			for (int i = 2; i <= 1024; i++)
			{
				array[i] = 0L;
			}
			return array;
		}

		// Token: 0x04000D6F RID: 3439
		public const int EOF = 1;

		// Token: 0x04000D70 RID: 3440
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000D71 RID: 3441
		public const int APPLY = 4;

		// Token: 0x04000D72 RID: 3442
		public const int MULTI_APPLY = 5;

		// Token: 0x04000D73 RID: 3443
		public const int ARGS = 6;

		// Token: 0x04000D74 RID: 3444
		public const int INCLUDE = 7;

		// Token: 0x04000D75 RID: 3445
		public const int CONDITIONAL = 8;

		// Token: 0x04000D76 RID: 3446
		public const int VALUE = 9;

		// Token: 0x04000D77 RID: 3447
		public const int TEMPLATE = 10;

		// Token: 0x04000D78 RID: 3448
		public const int FUNCTION = 11;

		// Token: 0x04000D79 RID: 3449
		public const int SINGLEVALUEARG = 12;

		// Token: 0x04000D7A RID: 3450
		public const int LIST = 13;

		// Token: 0x04000D7B RID: 3451
		public const int SEMI = 14;

		// Token: 0x04000D7C RID: 3452
		public const int LPAREN = 15;

		// Token: 0x04000D7D RID: 3453
		public const int RPAREN = 16;

		// Token: 0x04000D7E RID: 3454
		public const int COMMA = 17;

		// Token: 0x04000D7F RID: 3455
		public const int ID = 18;

		// Token: 0x04000D80 RID: 3456
		public const int ASSIGN = 19;

		// Token: 0x04000D81 RID: 3457
		public const int COLON = 20;

		// Token: 0x04000D82 RID: 3458
		public const int NOT = 21;

		// Token: 0x04000D83 RID: 3459
		public const int PLUS = 22;

		// Token: 0x04000D84 RID: 3460
		public const int LITERAL_super = 23;

		// Token: 0x04000D85 RID: 3461
		public const int DOT = 24;

		// Token: 0x04000D86 RID: 3462
		public const int LITERAL_first = 25;

		// Token: 0x04000D87 RID: 3463
		public const int LITERAL_rest = 26;

		// Token: 0x04000D88 RID: 3464
		public const int LITERAL_last = 27;

		// Token: 0x04000D89 RID: 3465
		public const int LITERAL_length = 28;

		// Token: 0x04000D8A RID: 3466
		public const int LITERAL_strip = 29;

		// Token: 0x04000D8B RID: 3467
		public const int LITERAL_trunc = 30;

		// Token: 0x04000D8C RID: 3468
		public const int ANONYMOUS_TEMPLATE = 31;

		// Token: 0x04000D8D RID: 3469
		public const int STRING = 32;

		// Token: 0x04000D8E RID: 3470
		public const int INT = 33;

		// Token: 0x04000D8F RID: 3471
		public const int LBRACK = 34;

		// Token: 0x04000D90 RID: 3472
		public const int RBRACK = 35;

		// Token: 0x04000D91 RID: 3473
		public const int DOTDOTDOT = 36;

		// Token: 0x04000D92 RID: 3474
		public const int TEMPLATE_ARGS = 37;

		// Token: 0x04000D93 RID: 3475
		public const int NESTED_ANONYMOUS_TEMPLATE = 38;

		// Token: 0x04000D94 RID: 3476
		public const int ESC_CHAR = 39;

		// Token: 0x04000D95 RID: 3477
		public const int WS = 40;

		// Token: 0x04000D96 RID: 3478
		public const int WS_CHAR = 41;

		// Token: 0x04000D97 RID: 3479
		public static readonly BitSet tokenSet_0_ = new BitSet(ActionLexer.mk_tokenSet_0_());

		// Token: 0x04000D98 RID: 3480
		public static readonly BitSet tokenSet_1_ = new BitSet(ActionLexer.mk_tokenSet_1_());

		// Token: 0x04000D99 RID: 3481
		public static readonly BitSet tokenSet_2_ = new BitSet(ActionLexer.mk_tokenSet_2_());

		// Token: 0x04000D9A RID: 3482
		public static readonly BitSet tokenSet_3_ = new BitSet(ActionLexer.mk_tokenSet_3_());

		// Token: 0x04000D9B RID: 3483
		public static readonly BitSet tokenSet_4_ = new BitSet(ActionLexer.mk_tokenSet_4_());

		// Token: 0x04000D9C RID: 3484
		public static readonly BitSet tokenSet_5_ = new BitSet(ActionLexer.mk_tokenSet_5_());

		// Token: 0x04000D9D RID: 3485
		public static readonly BitSet tokenSet_6_ = new BitSet(ActionLexer.mk_tokenSet_6_());
	}
}
