using pcomps.Antlr.Runtime;

namespace pcomps.PCompiler
{
	// Token: 0x0200020A RID: 522
	public class FlagsLexer : Lexer
	{
		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06000EC1 RID: 3777 RVA: 0x0006CBB0 File Offset: 0x0006ADB0
		// (remove) Token: 0x06000EC2 RID: 3778 RVA: 0x0006CBCC File Offset: 0x0006ADCC
		internal event InternalErrorEventHandler ErrorHandler;

		// Token: 0x06000EC3 RID: 3779 RVA: 0x0006CBE8 File Offset: 0x0006ADE8
		private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
		{
			if (this.ErrorHandler != null)
			{
				this.ErrorHandler(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
			}
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0006CC08 File Offset: 0x0006AE08
		public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			string errorMessage = this.GetErrorMessage(e, tokenNames);
			this.OnError(errorMessage, e.Line, e.CharPositionInLine);
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x0006CC34 File Offset: 0x0006AE34
		public FlagsLexer()
		{
			this.InitializeCyclicDFAs();
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0006CC44 File Offset: 0x0006AE44
		public FlagsLexer(ICharStream input) : this(input, null)
		{
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x0006CC50 File Offset: 0x0006AE50
		public FlagsLexer(ICharStream input, RecognizerSharedState state) : base(input, state)
		{
			this.InitializeCyclicDFAs();
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000EC8 RID: 3784 RVA: 0x0006CC60 File Offset: 0x0006AE60
		public override string GrammarFileName
		{
			get
			{
				return "Flags.g";
			}
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x0006CC68 File Offset: 0x0006AE68
		public void mFLAG()
		{
			int type = 4;
			int channel = 0;
			this.Match("flag");
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x0006CC9C File Offset: 0x0006AE9C
		public void mSCRIPT()
		{
			int type = 8;
			int channel = 0;
			this.Match("script");
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x0006CCD0 File Offset: 0x0006AED0
		public void mPROPERTY()
		{
			int type = 9;
			int channel = 0;
			this.Match("property");
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0006CD08 File Offset: 0x0006AF08
		public void mVARIABLE()
		{
			int type = 10;
			int channel = 0;
			this.Match("variable");
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x0006CD40 File Offset: 0x0006AF40
		public void mFUNCTION()
		{
			int type = 11;
			int channel = 0;
			this.Match("function");
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x0006CD78 File Offset: 0x0006AF78
		public void mOPEN_BRACE()
		{
			int type = 7;
			int channel = 0;
			this.Match(123);
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x0006CDAC File Offset: 0x0006AFAC
		public void mCLOSE_BRACE()
		{
			int type = 12;
			int channel = 0;
			this.Match(125);
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0006CDE0 File Offset: 0x0006AFE0
		public void mID()
		{
			int type = 5;
			int channel = 0;
			if ((this.input.LA(1) >= 65 && this.input.LA(1) <= 90) || this.input.LA(1) == 95 || (this.input.LA(1) >= 97 && this.input.LA(1) <= 122))
			{
				this.input.Consume();
				for (;;)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if ((num2 >= 48 && num2 <= 57) || (num2 >= 65 && num2 <= 90) || num2 == 95 || (num2 >= 97 && num2 <= 122))
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_157;
					}
					if ((this.input.LA(1) < 48 || this.input.LA(1) > 57) && (this.input.LA(1) < 65 || this.input.LA(1) > 90) && this.input.LA(1) != 95 && (this.input.LA(1) < 97 || this.input.LA(1) > 122))
					{
						break;
					}
					this.input.Consume();
				}
				MismatchedSetException ex = new MismatchedSetException(null, this.input);
				this.Recover(ex);
				throw ex;
				IL_157:
				this.state.type = type;
				this.state.channel = channel;
				return;
			}
			MismatchedSetException ex2 = new MismatchedSetException(null, this.input);
			this.Recover(ex2);
			throw ex2;
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x0006CF5C File Offset: 0x0006B15C
		public void mNUMBER()
		{
			int type = 6;
			int channel = 0;
			int num = 0;
			for (;;)
			{
				int num2 = 2;
				int num3 = this.input.LA(1);
				if (num3 >= 48 && num3 <= 57)
				{
					num2 = 1;
				}
				int num4 = num2;
				if (num4 != 1)
				{
					break;
				}
				this.mDIGIT();
				num++;
			}
			if (num < 1)
			{
				EarlyExitException ex = new EarlyExitException(2, this.input);
				throw ex;
			}
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x0006CFD0 File Offset: 0x0006B1D0
		public void mWS()
		{
			int type = 16;
			int num = 0;
			for (;;)
			{
				int num2 = 2;
				int num3 = this.input.LA(1);
				if ((num3 >= 9 && num3 <= 10) || num3 == 13 || num3 == 32)
				{
					num2 = 1;
				}
				int num4 = num2;
				if (num4 != 1)
				{
					break;
				}
				this.mWS_CHAR();
				num++;
			}
			if (num < 1)
			{
				EarlyExitException ex = new EarlyExitException(3, this.input);
				throw ex;
			}
			int channel = 99;
			this.state.type = type;
			this.state.channel = channel;
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x0006D054 File Offset: 0x0006B254
		public void mCOMMENT()
		{
			int type = 17;
			int channel = 0;
			int num = this.input.LA(1);
			if (num == 47)
			{
				int num2 = this.input.LA(2);
				int num3;
				if (num2 == 42)
				{
					num3 = 1;
				}
				else
				{
					if (num2 != 47)
					{
						NoViableAltException ex = new NoViableAltException("", 6, 1, this.input);
						throw ex;
					}
					num3 = 2;
				}
				switch (num3)
				{
				case 1:
					this.Match("/*");
					for (;;)
					{
						int num4 = 2;
						int num5 = this.input.LA(1);
						if (num5 == 42)
						{
							int num6 = this.input.LA(2);
							if (num6 == 47)
							{
								num4 = 2;
							}
							else if ((num6 >= 0 && num6 <= 46) || (num6 >= 48 && num6 <= 65535))
							{
								num4 = 1;
							}
						}
						else if ((num5 >= 0 && num5 <= 41) || (num5 >= 43 && num5 <= 65535))
						{
							num4 = 1;
						}
						int num7 = num4;
						if (num7 != 1)
						{
							break;
						}
						this.MatchAny();
					}
					this.Match("*/");
					channel = 99;
					break;
				case 2:
				{
					this.Match("//");
					for (;;)
					{
						int num8 = 2;
						int num9 = this.input.LA(1);
						if ((num9 >= 0 && num9 <= 9) || (num9 >= 11 && num9 <= 65535))
						{
							num8 = 1;
						}
						int num10 = num8;
						if (num10 != 1)
						{
							goto IL_1CA;
						}
						if ((this.input.LA(1) < 0 || this.input.LA(1) > 9) && (this.input.LA(1) < 11 || this.input.LA(1) > 65535))
						{
							break;
						}
						this.input.Consume();
					}
					MismatchedSetException ex2 = new MismatchedSetException(null, this.input);
					this.Recover(ex2);
					throw ex2;
					IL_1CA:
					channel = 99;
					break;
				}
				}
				this.state.type = type;
				this.state.channel = channel;
				return;
			}
			NoViableAltException ex3 = new NoViableAltException("", 6, 0, this.input);
			throw ex3;
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x0006D248 File Offset: 0x0006B448
		public void mALPHA()
		{
			if ((this.input.LA(1) >= 65 && this.input.LA(1) <= 90) || (this.input.LA(1) >= 97 && this.input.LA(1) <= 122))
			{
				this.input.Consume();
				return;
			}
			MismatchedSetException ex = new MismatchedSetException(null, this.input);
			this.Recover(ex);
			throw ex;
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x0006D2B8 File Offset: 0x0006B4B8
		public void mDIGIT()
		{
			this.MatchRange(48, 57);
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x0006D2C4 File Offset: 0x0006B4C4
		public void mWS_CHAR()
		{
			if ((this.input.LA(1) >= 9 && this.input.LA(1) <= 10) || this.input.LA(1) == 13 || this.input.LA(1) == 32)
			{
				this.input.Consume();
				return;
			}
			MismatchedSetException ex = new MismatchedSetException(null, this.input);
			this.Recover(ex);
			throw ex;
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x0006D334 File Offset: 0x0006B534
		public override void mTokens()
		{
			switch (this.dfa7.Predict(this.input))
			{
			case 1:
				this.mFLAG();
				return;
			case 2:
				this.mSCRIPT();
				return;
			case 3:
				this.mPROPERTY();
				return;
			case 4:
				this.mVARIABLE();
				return;
			case 5:
				this.mFUNCTION();
				return;
			case 6:
				this.mOPEN_BRACE();
				return;
			case 7:
				this.mCLOSE_BRACE();
				return;
			case 8:
				this.mID();
				return;
			case 9:
				this.mNUMBER();
				return;
			case 10:
				this.mWS();
				return;
			case 11:
				this.mCOMMENT();
				return;
			default:
				return;
			}
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x0006D3DC File Offset: 0x0006B5DC
		private void InitializeCyclicDFAs()
		{
			this.dfa7 = new FlagsLexer.DFA7(this);
		}

		// Token: 0x04000C82 RID: 3202
		public const int FUNCTION = 11;

		// Token: 0x04000C83 RID: 3203
		public const int SCRIPT = 8;

		// Token: 0x04000C84 RID: 3204
		public const int WS = 16;

		// Token: 0x04000C85 RID: 3205
		public const int VARIABLE = 10;

		// Token: 0x04000C86 RID: 3206
		public const int PROPERTY = 9;

		// Token: 0x04000C87 RID: 3207
		public const int WS_CHAR = 15;

		// Token: 0x04000C88 RID: 3208
		public const int NUMBER = 6;

		// Token: 0x04000C89 RID: 3209
		public const int CLOSE_BRACE = 12;

		// Token: 0x04000C8A RID: 3210
		public const int DIGIT = 14;

		// Token: 0x04000C8B RID: 3211
		public const int ID = 5;

		// Token: 0x04000C8C RID: 3212
		public const int COMMENT = 17;

		// Token: 0x04000C8D RID: 3213
		public const int EOF = -1;

		// Token: 0x04000C8E RID: 3214
		public const int OPEN_BRACE = 7;

		// Token: 0x04000C8F RID: 3215
		public const int FLAG = 4;

		// Token: 0x04000C90 RID: 3216
		public const int ALPHA = 13;

		// Token: 0x04000C91 RID: 3217
		private const string DFA7_eotS = "\u0001￿\u0004\a\u0006￿\n\a\u0001\u001a\u0004\a\u0001￿\u0005\a\u0001$\u0003\a\u0001￿\u0002\a\u0001*\u0001+\u0001,\u0003￿";

		// Token: 0x04000C92 RID: 3218
		private const string DFA7_eofS = "-￿";

		// Token: 0x04000C93 RID: 3219
		private const string DFA7_minS = "\u0001\t\u0001l\u0001c\u0001r\u0001a\u0006￿\u0001a\u0001n\u0001r\u0001o\u0001r\u0001g\u0001c\u0001i\u0001p\u0001i\u00010\u0001t\u0001p\u0001e\u0001a\u0001￿\u0001i\u0001t\u0001r\u0001b\u0001o\u00010\u0001t\u0001l\u0001n\u0001￿\u0001y\u0001e\u00030\u0003￿";

		// Token: 0x04000C94 RID: 3220
		private const string DFA7_maxS = "\u0001}\u0001u\u0001c\u0001r\u0001a\u0006￿\u0001a\u0001n\u0001r\u0001o\u0001r\u0001g\u0001c\u0001i\u0001p\u0001i\u0001z\u0001t\u0001p\u0001e\u0001a\u0001￿\u0001i\u0001t\u0001r\u0001b\u0001o\u0001z\u0001t\u0001l\u0001n\u0001￿\u0001y\u0001e\u0003z\u0003￿";

		// Token: 0x04000C95 RID: 3221
		private const string DFA7_acceptS = "\u0005￿\u0001\u0006\u0001\a\u0001\b\u0001\t\u0001\n\u0001\v\u000f￿\u0001\u0001\t￿\u0001\u0002\u0005￿\u0001\u0005\u0001\u0003\u0001\u0004";

		// Token: 0x04000C96 RID: 3222
		private const string DFA7_specialS = "-￿}>";

		// Token: 0x04000C98 RID: 3224
		protected FlagsLexer.DFA7 dfa7;

		// Token: 0x04000C99 RID: 3225
		private static readonly string[] DFA7_transitionS = new string[]
		{
			"\u0002\t\u0002￿\u0001\t\u0012￿\u0001\t\u000e￿\u0001\n\n\b\a￿\u001a\a\u0004￿\u0001\a\u0001￿\u0005\a\u0001\u0001\t\a\u0001\u0003\u0002\a\u0001\u0002\u0002\a\u0001\u0004\u0004\a\u0001\u0005\u0001￿\u0001\u0006",
			"\u0001\v\b￿\u0001\f",
			"\u0001\r",
			"\u0001\u000e",
			"\u0001\u000f",
			"",
			"",
			"",
			"",
			"",
			"",
			"\u0001\u0010",
			"\u0001\u0011",
			"\u0001\u0012",
			"\u0001\u0013",
			"\u0001\u0014",
			"\u0001\u0015",
			"\u0001\u0016",
			"\u0001\u0017",
			"\u0001\u0018",
			"\u0001\u0019",
			"\n\a\a￿\u001a\a\u0004￿\u0001\a\u0001￿\u001a\a",
			"\u0001\u001b",
			"\u0001\u001c",
			"\u0001\u001d",
			"\u0001\u001e",
			"",
			"\u0001\u001f",
			"\u0001 ",
			"\u0001!",
			"\u0001\"",
			"\u0001#",
			"\n\a\a￿\u001a\a\u0004￿\u0001\a\u0001￿\u001a\a",
			"\u0001%",
			"\u0001&",
			"\u0001'",
			"",
			"\u0001(",
			"\u0001)",
			"\n\a\a￿\u001a\a\u0004￿\u0001\a\u0001￿\u001a\a",
			"\n\a\a￿\u001a\a\u0004￿\u0001\a\u0001￿\u001a\a",
			"\n\a\a￿\u001a\a\u0004￿\u0001\a\u0001￿\u001a\a",
			"",
			"",
			""
		};

		// Token: 0x04000C9A RID: 3226
		private static readonly short[] DFA7_eot = DFA.UnpackEncodedString("\u0001￿\u0004\a\u0006￿\n\a\u0001\u001a\u0004\a\u0001￿\u0005\a\u0001$\u0003\a\u0001￿\u0002\a\u0001*\u0001+\u0001,\u0003￿");

		// Token: 0x04000C9B RID: 3227
		private static readonly short[] DFA7_eof = DFA.UnpackEncodedString("-￿");

		// Token: 0x04000C9C RID: 3228
		private static readonly char[] DFA7_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\t\u0001l\u0001c\u0001r\u0001a\u0006￿\u0001a\u0001n\u0001r\u0001o\u0001r\u0001g\u0001c\u0001i\u0001p\u0001i\u00010\u0001t\u0001p\u0001e\u0001a\u0001￿\u0001i\u0001t\u0001r\u0001b\u0001o\u00010\u0001t\u0001l\u0001n\u0001￿\u0001y\u0001e\u00030\u0003￿");

		// Token: 0x04000C9D RID: 3229
		private static readonly char[] DFA7_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001}\u0001u\u0001c\u0001r\u0001a\u0006￿\u0001a\u0001n\u0001r\u0001o\u0001r\u0001g\u0001c\u0001i\u0001p\u0001i\u0001z\u0001t\u0001p\u0001e\u0001a\u0001￿\u0001i\u0001t\u0001r\u0001b\u0001o\u0001z\u0001t\u0001l\u0001n\u0001￿\u0001y\u0001e\u0003z\u0003￿");

		// Token: 0x04000C9E RID: 3230
		private static readonly short[] DFA7_accept = DFA.UnpackEncodedString("\u0005￿\u0001\u0006\u0001\a\u0001\b\u0001\t\u0001\n\u0001\v\u000f￿\u0001\u0001\t￿\u0001\u0002\u0005￿\u0001\u0005\u0001\u0003\u0001\u0004");

		// Token: 0x04000C9F RID: 3231
		private static readonly short[] DFA7_special = DFA.UnpackEncodedString("-￿}>");

		// Token: 0x04000CA0 RID: 3232
		private static readonly short[][] DFA7_transition = DFA.UnpackEncodedStringArray(FlagsLexer.DFA7_transitionS);

		// Token: 0x0200020B RID: 523
		protected class DFA7 : DFA
		{
			// Token: 0x06000EDA RID: 3802 RVA: 0x0006D5FC File Offset: 0x0006B7FC
			public DFA7(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				this.decisionNumber = 7;
				this.eot = FlagsLexer.DFA7_eot;
				this.eof = FlagsLexer.DFA7_eof;
				this.min = FlagsLexer.DFA7_min;
				this.max = FlagsLexer.DFA7_max;
				this.accept = FlagsLexer.DFA7_accept;
				this.special = FlagsLexer.DFA7_special;
				this.transition = FlagsLexer.DFA7_transition;
			}

			// Token: 0x1700021B RID: 539
			// (get) Token: 0x06000EDB RID: 3803 RVA: 0x0006D66C File Offset: 0x0006B86C
			public override string Description
			{
				get
				{
					return "1:1: Tokens : ( FLAG | SCRIPT | PROPERTY | VARIABLE | FUNCTION | OPEN_BRACE | CLOSE_BRACE | ID | NUMBER | WS | COMMENT );";
				}
			}
		}
	}
}
