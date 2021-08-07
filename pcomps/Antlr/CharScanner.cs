using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using pcomps.Antlr.collections.impl;
using pcomps.Antlr.debug;

namespace pcomps.Antlr
{
	// Token: 0x02000016 RID: 22
	public abstract class CharScanner : TokenStream, ICharScannerDebugSubject
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x000045A8 File Offset: 0x000027A8
		protected internal EventHandlerList Events => events_;

        // Token: 0x060000E1 RID: 225 RVA: 0x000045BC File Offset: 0x000027BC
		public CharScanner()
		{
			text = new StringBuilder();
			setTokenCreator(new CommonToken.CommonTokenCreator());
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00004624 File Offset: 0x00002824
		public CharScanner(InputBuffer cb) : this()
		{
			inputState = new LexerSharedInputState(cb);
			cached_LA2 = inputState.input.LA(2);
			cached_LA1 = inputState.input.LA(1);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00004674 File Offset: 0x00002874
		public CharScanner(LexerSharedInputState sharedState) : this()
		{
			inputState = sharedState;
			if (inputState != null)
			{
				cached_LA2 = inputState.input.LA(2);
				cached_LA1 = inputState.input.LA(1);
			}
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060000E4 RID: 228 RVA: 0x000046C4 File Offset: 0x000028C4
		// (remove) Token: 0x060000E5 RID: 229 RVA: 0x000046E4 File Offset: 0x000028E4
		public event TraceEventHandler EnterRule
		{
			add => Events.AddHandler(EnterRuleEventKey, value);
            remove => Events.RemoveHandler(EnterRuleEventKey, value);
        }

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060000E6 RID: 230 RVA: 0x00004704 File Offset: 0x00002904
		// (remove) Token: 0x060000E7 RID: 231 RVA: 0x00004724 File Offset: 0x00002924
		public event TraceEventHandler ExitRule
		{
			add => Events.AddHandler(ExitRuleEventKey, value);
            remove => Events.RemoveHandler(ExitRuleEventKey, value);
        }

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x060000E8 RID: 232 RVA: 0x00004744 File Offset: 0x00002944
		// (remove) Token: 0x060000E9 RID: 233 RVA: 0x00004764 File Offset: 0x00002964
		public event TraceEventHandler Done
		{
			add => Events.AddHandler(DoneEventKey, value);
            remove => Events.RemoveHandler(DoneEventKey, value);
        }

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x060000EA RID: 234 RVA: 0x00004784 File Offset: 0x00002984
		// (remove) Token: 0x060000EB RID: 235 RVA: 0x000047A4 File Offset: 0x000029A4
		public event MessageEventHandler ErrorReported
		{
			add => Events.AddHandler(ReportErrorEventKey, value);
            remove => Events.RemoveHandler(ReportErrorEventKey, value);
        }

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060000EC RID: 236 RVA: 0x000047C4 File Offset: 0x000029C4
		// (remove) Token: 0x060000ED RID: 237 RVA: 0x000047E4 File Offset: 0x000029E4
		public event MessageEventHandler WarningReported
		{
			add => Events.AddHandler(ReportWarningEventKey, value);
            remove => Events.RemoveHandler(ReportWarningEventKey, value);
        }

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060000EE RID: 238 RVA: 0x00004804 File Offset: 0x00002A04
		// (remove) Token: 0x060000EF RID: 239 RVA: 0x00004824 File Offset: 0x00002A24
		public event NewLineEventHandler HitNewLine
		{
			add => Events.AddHandler(NewLineEventKey, value);
            remove => Events.RemoveHandler(NewLineEventKey, value);
        }

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x060000F0 RID: 240 RVA: 0x00004844 File Offset: 0x00002A44
		// (remove) Token: 0x060000F1 RID: 241 RVA: 0x00004864 File Offset: 0x00002A64
		public event MatchEventHandler MatchedChar
		{
			add => Events.AddHandler(MatchEventKey, value);
            remove => Events.RemoveHandler(MatchEventKey, value);
        }

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x060000F2 RID: 242 RVA: 0x00004884 File Offset: 0x00002A84
		// (remove) Token: 0x060000F3 RID: 243 RVA: 0x000048A4 File Offset: 0x00002AA4
		public event MatchEventHandler MatchedNotChar
		{
			add => Events.AddHandler(MatchNotEventKey, value);
            remove => Events.RemoveHandler(MatchNotEventKey, value);
        }

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x060000F4 RID: 244 RVA: 0x000048C4 File Offset: 0x00002AC4
		// (remove) Token: 0x060000F5 RID: 245 RVA: 0x000048E4 File Offset: 0x00002AE4
		public event MatchEventHandler MisMatchedChar
		{
			add => Events.AddHandler(MisMatchEventKey, value);
            remove => Events.RemoveHandler(MisMatchEventKey, value);
        }

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060000F6 RID: 246 RVA: 0x00004904 File Offset: 0x00002B04
		// (remove) Token: 0x060000F7 RID: 247 RVA: 0x00004924 File Offset: 0x00002B24
		public event MatchEventHandler MisMatchedNotChar
		{
			add => Events.AddHandler(MisMatchNotEventKey, value);
            remove => Events.RemoveHandler(MisMatchNotEventKey, value);
        }

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x060000F8 RID: 248 RVA: 0x00004944 File Offset: 0x00002B44
		// (remove) Token: 0x060000F9 RID: 249 RVA: 0x00004964 File Offset: 0x00002B64
		public event TokenEventHandler ConsumedChar
		{
			add => Events.AddHandler(ConsumeEventKey, value);
            remove => Events.RemoveHandler(ConsumeEventKey, value);
        }

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x060000FA RID: 250 RVA: 0x00004984 File Offset: 0x00002B84
		// (remove) Token: 0x060000FB RID: 251 RVA: 0x000049A4 File Offset: 0x00002BA4
		public event TokenEventHandler CharLA
		{
			add => Events.AddHandler(LAEventKey, value);
            remove => Events.RemoveHandler(LAEventKey, value);
        }

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x060000FC RID: 252 RVA: 0x000049C4 File Offset: 0x00002BC4
		// (remove) Token: 0x060000FD RID: 253 RVA: 0x000049E4 File Offset: 0x00002BE4
		public event SemanticPredicateEventHandler SemPredEvaluated
		{
			add => Events.AddHandler(SemPredEvaluatedEventKey, value);
            remove => Events.RemoveHandler(SemPredEvaluatedEventKey, value);
        }

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x060000FE RID: 254 RVA: 0x00004A04 File Offset: 0x00002C04
		// (remove) Token: 0x060000FF RID: 255 RVA: 0x00004A24 File Offset: 0x00002C24
		public event SyntacticPredicateEventHandler SynPredStarted
		{
			add => Events.AddHandler(SynPredStartedEventKey, value);
            remove => Events.RemoveHandler(SynPredStartedEventKey, value);
        }

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06000100 RID: 256 RVA: 0x00004A44 File Offset: 0x00002C44
		// (remove) Token: 0x06000101 RID: 257 RVA: 0x00004A64 File Offset: 0x00002C64
		public event SyntacticPredicateEventHandler SynPredFailed
		{
			add => Events.AddHandler(SynPredFailedEventKey, value);
            remove => Events.RemoveHandler(SynPredFailedEventKey, value);
        }

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06000102 RID: 258 RVA: 0x00004A84 File Offset: 0x00002C84
		// (remove) Token: 0x06000103 RID: 259 RVA: 0x00004AA4 File Offset: 0x00002CA4
		public event SyntacticPredicateEventHandler SynPredSucceeded
		{
			add => Events.AddHandler(SynPredSucceededEventKey, value);
            remove => Events.RemoveHandler(SynPredSucceededEventKey, value);
        }

		// Token: 0x06000104 RID: 260 RVA: 0x00004AC4 File Offset: 0x00002CC4
		public virtual IToken nextToken()
		{
			return null;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00004AD4 File Offset: 0x00002CD4
		public virtual void append(char c)
		{
			if (saveConsumedInput)
			{
				text.Append(c);
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00004AF8 File Offset: 0x00002CF8
		public virtual void append(string s)
		{
			if (saveConsumedInput)
			{
				text.Append(s);
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004B1C File Offset: 0x00002D1C
		public virtual void commit()
		{
			inputState.input.commit();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00004B3C File Offset: 0x00002D3C
		public virtual void recover(RecognitionException ex, BitSet tokenSet)
		{
			consume();
			consumeUntil(tokenSet);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00004B58 File Offset: 0x00002D58
		public virtual void consume()
		{
			if (inputState.guessing == 0)
            {
                append(caseSensitive ? cached_LA1 : inputState.input.LA(1));
                if (cached_LA1 == '\t')
				{
					tab();
				}
				else
				{
					inputState.column++;
				}
            }
			if (caseSensitive)
			{
				cached_LA1 = inputState.input.consume();
				cached_LA2 = inputState.input.LA(2);
				return;
			}
			cached_LA1 = toLower(inputState.input.consume());
			cached_LA2 = toLower(inputState.input.LA(2));
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00004C34 File Offset: 0x00002E34
		public virtual void consumeUntil(int c)
		{
			while (EOF_CHAR != cached_LA1 && c != cached_LA1)
			{
				consume();
			}
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00004C60 File Offset: 0x00002E60
		public virtual void consumeUntil(BitSet bset)
		{
			while (cached_LA1 != EOF_CHAR && !bset.member(cached_LA1))
			{
				consume();
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00004C90 File Offset: 0x00002E90
		public virtual bool getCaseSensitive()
		{
			return caseSensitive;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00004CA4 File Offset: 0x00002EA4
		public bool getCaseSensitiveLiterals()
		{
			return caseSensitiveLiterals;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00004CB8 File Offset: 0x00002EB8
		public virtual int getColumn()
		{
			return inputState.column;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00004CD0 File Offset: 0x00002ED0
		public virtual void setColumn(int c)
		{
			inputState.column = c;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00004CEC File Offset: 0x00002EEC
		public virtual bool getCommitToPath()
		{
			return commitToPath;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00004D00 File Offset: 0x00002F00
		public virtual string getFilename()
		{
			return inputState.filename;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00004D18 File Offset: 0x00002F18
		public virtual InputBuffer getInputBuffer()
		{
			return inputState.input;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00004D30 File Offset: 0x00002F30
		public virtual LexerSharedInputState getInputState()
		{
			return inputState;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00004D44 File Offset: 0x00002F44
		public virtual void setInputState(LexerSharedInputState state)
		{
			inputState = state;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00004D58 File Offset: 0x00002F58
		public virtual int getLine()
		{
			return inputState.line;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00004D70 File Offset: 0x00002F70
		public virtual string getText()
		{
			return text.ToString();
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00004D88 File Offset: 0x00002F88
		public virtual IToken getTokenObject()
		{
			return returnToken_;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00004D9C File Offset: 0x00002F9C
		public virtual char LA(int i)
        {
            return i switch
            {
                1 => cached_LA1,
                2 => cached_LA2,
                _ => caseSensitive ? inputState.input.LA(i) : toLower(inputState.input.LA(i))
            };
        }

		// Token: 0x06000119 RID: 281 RVA: 0x00004DF0 File Offset: 0x00002FF0
		protected internal virtual IToken makeToken(int t)
		{
			IToken token = null;
			bool flag;
			try
			{
				token = tokenCreator.Create();
				if (token != null)
				{
					token.Type = t;
					token.setColumn(inputState.tokenStartColumn);
					token.setLine(inputState.tokenStartLine);
					token.setFilename(inputState.filename);
				}
				flag = true;
			}
			catch
			{
				flag = false;
			}

            if (flag) return token;
            panic($"Can't create Token object '{tokenCreator.TokenTypeName}'");
            token = Token.badToken;
            return token;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00004E8C File Offset: 0x0000308C
		public virtual int mark()
		{
			return inputState.input.mark();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00004EAC File Offset: 0x000030AC
		public virtual void match(char c)
		{
			match((int)c);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00004EC0 File Offset: 0x000030C0
		public virtual void match(int c)
		{
			if (cached_LA1 != c)
			{
				throw new MismatchedCharException(cached_LA1, Convert.ToChar(c), false, this);
			}
			consume();
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00004EF0 File Offset: 0x000030F0
		public virtual void match(BitSet b)
		{
			if (!b.member(cached_LA1))
			{
				throw new MismatchedCharException(cached_LA1, b, false, this);
			}
			consume();
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00004F20 File Offset: 0x00003120
		public virtual void match(string s)
		{
			var length = s.Length;
			for (var i = 0; i < length; i++)
			{
				if (cached_LA1 != s[i])
				{
					throw new MismatchedCharException(cached_LA1, s[i], false, this);
				}
				consume();
			}
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00004F6C File Offset: 0x0000316C
		public virtual void matchNot(char c)
		{
			matchNot((int)c);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00004F80 File Offset: 0x00003180
		public virtual void matchNot(int c)
		{
			if (cached_LA1 == c)
			{
				throw new MismatchedCharException(cached_LA1, Convert.ToChar(c), true, this);
			}
			consume();
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00004FB0 File Offset: 0x000031B0
		public virtual void matchRange(int c1, int c2)
		{
			if (cached_LA1 < c1 || cached_LA1 > c2)
			{
				throw new MismatchedCharException(cached_LA1, Convert.ToChar(c1), Convert.ToChar(c2), false, this);
			}
			consume();
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00004FF0 File Offset: 0x000031F0
		public virtual void matchRange(char c1, char c2)
		{
			matchRange(c1, (int)c2);
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00005008 File Offset: 0x00003208
		public virtual void newline()
		{
			inputState.line++;
			inputState.column = 1;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00005034 File Offset: 0x00003234
		public virtual void tab()
		{
			var column = getColumn();
			var column2 = ((column - 1) / tabsize + 1) * tabsize + 1;
			setColumn(column2);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00005068 File Offset: 0x00003268
		public virtual void setTabSize(int size)
		{
			tabsize = size;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000507C File Offset: 0x0000327C
		public virtual int getTabSize()
		{
			return tabsize;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00005090 File Offset: 0x00003290
		public virtual void panic()
		{
			panic("");
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000050A8 File Offset: 0x000032A8
		public virtual void panic(string s)
		{
			throw new ANTLRPanicException($"CharScanner::panic: {s}");
		}

		// Token: 0x06000129 RID: 297 RVA: 0x000050C8 File Offset: 0x000032C8
		public virtual void reportError(RecognitionException ex)
		{
			Console.Error.WriteLine(ex);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000050E0 File Offset: 0x000032E0
		public virtual void reportError(string s)
		{
			if (getFilename() == null)
			{
				Console.Error.WriteLine($"error: {s}");
				return;
			}
			Console.Error.WriteLine($"{getFilename()}: error: {s}");
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00005128 File Offset: 0x00003328
		public virtual void reportWarning(string s)
		{
			if (getFilename() == null)
			{
				Console.Error.WriteLine($"warning: {s}");
				return;
			}
			Console.Error.WriteLine($"{getFilename()}: warning: {s}");
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00005170 File Offset: 0x00003370
		public virtual void refresh()
		{
			if (caseSensitive)
			{
				cached_LA2 = inputState.input.LA(2);
				cached_LA1 = inputState.input.LA(1);
				return;
			}
			cached_LA2 = toLower(inputState.input.LA(2));
			cached_LA1 = toLower(inputState.input.LA(1));
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000051F0 File Offset: 0x000033F0
		public virtual void resetState(InputBuffer ib)
		{
			text.Length = 0;
			traceDepth = 0;
			inputState.resetInput(ib);
			refresh();
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00005224 File Offset: 0x00003424
		public void resetState(Stream s)
		{
			resetState(new ByteBuffer(s));
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00005240 File Offset: 0x00003440
		public void resetState(TextReader tr)
		{
			resetState(new CharBuffer(tr));
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000525C File Offset: 0x0000345C
		public virtual void resetText()
		{
			text.Length = 0;
			inputState.tokenStartColumn = inputState.column;
			inputState.tokenStartLine = inputState.line;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000052A4 File Offset: 0x000034A4
		public virtual void rewind(int pos)
		{
			inputState.input.rewind(pos);
			if (caseSensitive)
			{
				cached_LA2 = inputState.input.LA(2);
				cached_LA1 = inputState.input.LA(1);
				return;
			}
			cached_LA2 = toLower(inputState.input.LA(2));
			cached_LA1 = toLower(inputState.input.LA(1));
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00005334 File Offset: 0x00003534
		public virtual void setCaseSensitive(bool t)
		{
			caseSensitive = t;
			if (caseSensitive)
			{
				cached_LA2 = inputState.input.LA(2);
				cached_LA1 = inputState.input.LA(1);
				return;
			}
			cached_LA2 = toLower(inputState.input.LA(2));
			cached_LA1 = toLower(inputState.input.LA(1));
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000053BC File Offset: 0x000035BC
		public virtual void setCommitToPath(bool commit)
		{
			commitToPath = commit;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000053D0 File Offset: 0x000035D0
		public virtual void setFilename(string f)
		{
			inputState.filename = f;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000053EC File Offset: 0x000035EC
		public virtual void setLine(int line)
		{
			inputState.line = line;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00005408 File Offset: 0x00003608
		public virtual void setText(string s)
		{
			resetText();
			text.Append(s);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00005428 File Offset: 0x00003628
		public virtual void setTokenObjectClass(string cl)
		{
			tokenCreator = new ReflectionBasedTokenCreator(this, cl);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00005444 File Offset: 0x00003644
		public virtual void setTokenCreator(TokenCreator tokenCreator)
		{
			this.tokenCreator = tokenCreator;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00005458 File Offset: 0x00003658
		public virtual int testLiteralsTable(int ttype)
		{
			var text = this.text.ToString();
			if (string.IsNullOrEmpty(text))
			{
				return ttype;
			}
			var obj = literals[text];
			if (obj != null)
			{
				return (int)obj;
			}
			return ttype;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000549C File Offset: 0x0000369C
		public virtual int testLiteralsTable(string someText, int ttype)
		{
			if (string.IsNullOrEmpty(someText))
			{
				return ttype;
			}
			var obj = literals[someText];
			if (obj != null)
			{
				return (int)obj;
			}
			return ttype;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x000054D4 File Offset: 0x000036D4
		public virtual char toLower(int c)
		{
			return char.ToLower(Convert.ToChar(c), CultureInfo.InvariantCulture);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000054F4 File Offset: 0x000036F4
		public virtual void traceIndent()
		{
			for (var i = 0; i < traceDepth; i++)
			{
				Console.Out.Write(" ");
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00005524 File Offset: 0x00003724
		public virtual void traceIn(string rname)
		{
			traceDepth++;
			traceIndent();
			Console.Out.WriteLine($"> lexer {rname}; c=={LA(1)}");
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00005580 File Offset: 0x00003780
		public virtual void traceOut(string rname)
		{
			traceIndent();
			Console.Out.WriteLine($"< lexer {rname}; c=={LA(1)}");
			traceDepth--;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000055DC File Offset: 0x000037DC
		public virtual void uponEOF()
		{
		}

		// Token: 0x04000032 RID: 50
		internal const char NO_CHAR = '\0';

		// Token: 0x04000033 RID: 51
		public static readonly char EOF_CHAR = char.MaxValue;

		// Token: 0x04000034 RID: 52
		private EventHandlerList events_ = new EventHandlerList();

		// Token: 0x04000035 RID: 53
		internal static readonly object EnterRuleEventKey = new object();

		// Token: 0x04000036 RID: 54
		internal static readonly object ExitRuleEventKey = new object();

		// Token: 0x04000037 RID: 55
		internal static readonly object DoneEventKey = new object();

		// Token: 0x04000038 RID: 56
		internal static readonly object ReportErrorEventKey = new object();

		// Token: 0x04000039 RID: 57
		internal static readonly object ReportWarningEventKey = new object();

		// Token: 0x0400003A RID: 58
		internal static readonly object NewLineEventKey = new object();

		// Token: 0x0400003B RID: 59
		internal static readonly object MatchEventKey = new object();

		// Token: 0x0400003C RID: 60
		internal static readonly object MatchNotEventKey = new object();

		// Token: 0x0400003D RID: 61
		internal static readonly object MisMatchEventKey = new object();

		// Token: 0x0400003E RID: 62
		internal static readonly object MisMatchNotEventKey = new object();

		// Token: 0x0400003F RID: 63
		internal static readonly object ConsumeEventKey = new object();

		// Token: 0x04000040 RID: 64
		internal static readonly object LAEventKey = new object();

		// Token: 0x04000041 RID: 65
		internal static readonly object SemPredEvaluatedEventKey = new object();

		// Token: 0x04000042 RID: 66
		internal static readonly object SynPredStartedEventKey = new object();

		// Token: 0x04000043 RID: 67
		internal static readonly object SynPredFailedEventKey = new object();

		// Token: 0x04000044 RID: 68
		internal static readonly object SynPredSucceededEventKey = new object();

		// Token: 0x04000045 RID: 69
		protected internal StringBuilder text;

		// Token: 0x04000046 RID: 70
		protected bool saveConsumedInput = true;

		// Token: 0x04000047 RID: 71
		protected TokenCreator tokenCreator;

		// Token: 0x04000048 RID: 72
		protected char cached_LA1;

		// Token: 0x04000049 RID: 73
		protected char cached_LA2;

		// Token: 0x0400004A RID: 74
		protected bool caseSensitive = true;

		// Token: 0x0400004B RID: 75
		protected bool caseSensitiveLiterals = true;

		// Token: 0x0400004C RID: 76
		protected Hashtable literals;

		// Token: 0x0400004D RID: 77
		protected internal int tabsize = 8;

		// Token: 0x0400004E RID: 78
		protected internal IToken returnToken_ = null;

		// Token: 0x0400004F RID: 79
		protected internal LexerSharedInputState inputState;

		// Token: 0x04000050 RID: 80
		protected internal bool commitToPath = false;

		// Token: 0x04000051 RID: 81
		protected internal int traceDepth = 0;

		// Token: 0x02000017 RID: 23
		private class ReflectionBasedTokenCreator : TokenCreator
		{
			// Token: 0x06000141 RID: 321 RVA: 0x000056A4 File Offset: 0x000038A4
			protected ReflectionBasedTokenCreator()
			{
			}

			// Token: 0x06000142 RID: 322 RVA: 0x000056B8 File Offset: 0x000038B8
			public ReflectionBasedTokenCreator(CharScanner owner, string tokenTypeName)
			{
				this.owner = owner;
				SetTokenType(tokenTypeName);
			}

			// Token: 0x06000143 RID: 323 RVA: 0x000056DC File Offset: 0x000038DC
			private void SetTokenType(string tokenTypeName)
			{
				this.tokenTypeName = tokenTypeName;
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						tokenTypeObject = assembly.GetType(tokenTypeName);
						if (tokenTypeObject != null)
						{
							break;
						}
					}
					catch
					{
						throw new TypeLoadException($"Unable to load Type for Token class '{tokenTypeName}'");
					}
				}
				if (tokenTypeObject == null)
				{
					throw new TypeLoadException($"Unable to load Type for Token class '{tokenTypeName}'");
				}
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x06000144 RID: 324 RVA: 0x0000576C File Offset: 0x0000396C
			public override string TokenTypeName => tokenTypeName;

            // Token: 0x06000145 RID: 325 RVA: 0x00005780 File Offset: 0x00003980
			public override IToken Create()
			{
				IToken result = null;
				try
				{
					result = (Token)Activator.CreateInstance(tokenTypeObject);
				}
				catch
				{
				}
				return result;
			}

			// Token: 0x04000052 RID: 82
			private CharScanner owner;

			// Token: 0x04000053 RID: 83
			private string tokenTypeName;

			// Token: 0x04000054 RID: 84
			private Type tokenTypeObject;
		}
	}
}
