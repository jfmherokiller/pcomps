using System.Collections;
using System.IO;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000245 RID: 581
	public class InterfaceLexer : CharScanner, TokenStream
	{
		// Token: 0x06001193 RID: 4499 RVA: 0x000810F8 File Offset: 0x0007F2F8
		public InterfaceLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x00081108 File Offset: 0x0007F308
		public InterfaceLexer(TextReader r) : this(new CharBuffer(r))
		{
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00081118 File Offset: 0x0007F318
		public InterfaceLexer(InputBuffer ib) : this(new LexerSharedInputState(ib))
		{
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x00081128 File Offset: 0x0007F328
		public InterfaceLexer(LexerSharedInputState state) : base(state)
		{
			initialize();
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x00081138 File Offset: 0x0007F338
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
			literals = new Hashtable(100, 0.4f, null, Comparer.Default);
			literals.Add("optional", 7);
			literals.Add("interface", 4);
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x00081198 File Offset: 0x0007F398
		public override IToken nextToken()
		{
			IToken returnToken_10;
			for (;;)
			{
				resetText();
				try
				{
					try
					{
						switch (cached_LA1)
						{
						case '\t':
						case '\n':
						case '\f':
						case '\r':
						case ' ':
						{
							mWS(true);
							var returnToken_ = this.returnToken_;
							goto IL_2F4;
						}
						case '(':
						{
							mLPAREN(true);
							var returnToken_2 = returnToken_;
							goto IL_2F4;
						}
						case ')':
						{
							mRPAREN(true);
							var returnToken_3 = returnToken_;
							goto IL_2F4;
						}
						case ',':
						{
							mCOMMA(true);
							var returnToken_4 = returnToken_;
							goto IL_2F4;
						}
						case ':':
						{
							mCOLON(true);
							var returnToken_5 = returnToken_;
							goto IL_2F4;
						}
						case ';':
						{
							mSEMI(true);
							var returnToken_6 = returnToken_;
							goto IL_2F4;
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
							mID(true);
							var returnToken_7 = returnToken_;
							goto IL_2F4;
						}
						}
						if (cached_LA1 == '/' && cached_LA2 == '/')
						{
							mSL_COMMENT(true);
							var returnToken_8 = returnToken_;
						}
						else if (cached_LA1 == '/' && cached_LA2 == '*')
						{
							mML_COMMENT(true);
							var returnToken_9 = returnToken_;
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
						IL_2F4:
						if (this.returnToken_ == null)
						{
							continue;
						}
						var num = this.returnToken_.Type;
						num = testLiteralsTable(num);
						this.returnToken_.Type = num;
						returnToken_10 = this.returnToken_;
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
			return returnToken_10;
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x00081534 File Offset: 0x0007F734
		public void mID(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 5;
			switch (cached_LA1)
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
				matchRange('A', 'Z');
				break;
			case '[':
			case '\\':
			case ']':
			case '^':
			case '`':
				goto IL_12C;
			case '_':
				match('_');
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
				matchRange('a', 'z');
				break;
			default:
				goto IL_12C;
			}
			for (;;)
			{
				switch (cached_LA1)
				{
				case '-':
					match('-');
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
					matchRange('0', '9');
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
					matchRange('A', 'Z');
					continue;
				case '_':
					match('_');
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
					matchRange('a', 'z');
					continue;
				}
				break;
			}
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
			return;
			IL_12C:
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x0008185C File Offset: 0x0007FA5C
		public void mLPAREN(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 8;
			match('(');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x000818C0 File Offset: 0x0007FAC0
		public void mRPAREN(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 9;
			match(')');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x00081924 File Offset: 0x0007FB24
		public void mCOMMA(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 10;
			match(',');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00081988 File Offset: 0x0007FB88
		public void mSEMI(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 6;
			match(';');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x000819EC File Offset: 0x0007FBEC
		public void mCOLON(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 11;
			match(':');
			if (_createToken && token == null && num != Token.SKIP)
			{
				token = makeToken(num);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00081A50 File Offset: 0x0007FC50
		public void mSL_COMMENT(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			match("//");
			while (tokenSet_0_.member((int)cached_LA1))
			{
				match(tokenSet_0_);
			}
			if (cached_LA1 == '\n' || cached_LA1 == '\r')
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
			}
			var skip = Token.SKIP;
			newline();
			if (_createToken && token == null && skip != Token.SKIP)
			{
				token = makeToken(skip);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00081B38 File Offset: 0x0007FD38
		public void mML_COMMENT(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			match("/*");
			while (cached_LA1 != '*' || cached_LA2 != '/')
			{
				if ((cached_LA1 == '\n' || cached_LA1 == '\r') && cached_LA2 >= '\0' && cached_LA2 <= '￾')
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
					if (cached_LA1 < '\0' || cached_LA1 > '￾' || cached_LA2 < '\0' || cached_LA2 > '￾')
					{
						break;
					}
					matchNot(1);
				}
			}
			match("*/");
			var skip = Token.SKIP;
			if (_createToken && token == null && skip != Token.SKIP)
			{
				token = makeToken(skip);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x00081C74 File Offset: 0x0007FE74
		public void mWS(bool _createToken)
		{
			IToken token = null;
			var length = text.Length;
			var num = 0;
			for (;;)
			{
				var cached_LA = cached_LA1;
				switch (cached_LA)
				{
				case '\t':
					match('\t');
					break;
				case '\n':
				case '\r':
				{
					var cached_LA2 = cached_LA1;
					if (cached_LA2 != '\n')
					{
						if (cached_LA2 != '\r')
						{
							goto IL_7E;
						}
						match('\r');
					}
					match('\n');
					newline();
					break;
				}
				case '\v':
					goto IL_AC;
				case '\f':
					match('\f');
					break;
				default:
					if (cached_LA != ' ')
					{
						goto IL_AC;
					}
					match(' ');
					break;
				}
				num++;
			}
			IL_7E:
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			IL_AC:
			if (num < 1)
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			var skip = Token.SKIP;
			if (_createToken && token == null && skip != Token.SKIP)
			{
				token = makeToken(skip);
				token.setText(text.ToString(length, text.Length - length));
			}
			returnToken_ = token;
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x00081D9C File Offset: 0x0007FF9C
		private static long[] mk_tokenSet_0_()
		{
			var array = new long[2048];
			array[0] = -9217L;
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

		// Token: 0x04000ECC RID: 3788
		public const int EOF = 1;

		// Token: 0x04000ECD RID: 3789
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000ECE RID: 3790
		public const int LITERAL_interface = 4;

		// Token: 0x04000ECF RID: 3791
		public const int ID = 5;

		// Token: 0x04000ED0 RID: 3792
		public const int SEMI = 6;

		// Token: 0x04000ED1 RID: 3793
		public const int LITERAL_optional = 7;

		// Token: 0x04000ED2 RID: 3794
		public const int LPAREN = 8;

		// Token: 0x04000ED3 RID: 3795
		public const int RPAREN = 9;

		// Token: 0x04000ED4 RID: 3796
		public const int COMMA = 10;

		// Token: 0x04000ED5 RID: 3797
		public const int COLON = 11;

		// Token: 0x04000ED6 RID: 3798
		public const int SL_COMMENT = 12;

		// Token: 0x04000ED7 RID: 3799
		public const int ML_COMMENT = 13;

		// Token: 0x04000ED8 RID: 3800
		public const int WS = 14;

		// Token: 0x04000ED9 RID: 3801
		public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	}
}
