using System;
using System.Globalization;
using System.Text;
using pcomps.Antlr.Runtime;

namespace pcomps.PCompiler
{
	// Token: 0x02000175 RID: 373
	public class PapyrusLexer : Lexer
	{
		// Token: 0x14000038 RID: 56
		// (add) Token: 0x06000BC0 RID: 3008 RVA: 0x00046418 File Offset: 0x00044618
		// (remove) Token: 0x06000BC1 RID: 3009 RVA: 0x00046434 File Offset: 0x00044634
		internal event InternalErrorEventHandler ErrorHandler;

		// Token: 0x06000BC2 RID: 3010 RVA: 0x00046450 File Offset: 0x00044650
		private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
        {
            ErrorHandler?.Invoke(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
        }

		// Token: 0x06000BC3 RID: 3011 RVA: 0x00046470 File Offset: 0x00044670
		public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			var errorMessage = GetErrorMessage(e, tokenNames);
			OnError(errorMessage, e.Line, e.CharPositionInLine);
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x0004649C File Offset: 0x0004469C
		public PapyrusLexer()
		{
			InitializeCyclicDFAs();
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x000464AC File Offset: 0x000446AC
		public PapyrusLexer(ICharStream input) : this(input, null)
		{
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x000464B8 File Offset: 0x000446B8
		public PapyrusLexer(ICharStream input, RecognizerSharedState state) : base(input, state)
		{
			InitializeCyclicDFAs();
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000BC7 RID: 3015 RVA: 0x000464C8 File Offset: 0x000446C8
		public override string GrammarFileName
		{
			get
			{
				return "Papyrus.g";
			}
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x000464D0 File Offset: 0x000446D0
		public void mSCRIPTNAME()
		{
			var type = 37;
			var channel = 0;
			Match("scriptname");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x00046514 File Offset: 0x00044714
		public void mFUNCTION()
		{
			var type = 6;
			var channel = 0;
			Match("function");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x00046558 File Offset: 0x00044758
		public void mENDFUNCTION()
		{
			var type = 45;
			var channel = 0;
			Match("endfunction");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x0004659C File Offset: 0x0004479C
		public void mEVENT()
		{
			var type = 7;
			var channel = 0;
			Match("event");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x000465E0 File Offset: 0x000447E0
		public void mENDEVENT()
		{
			var type = 48;
			var channel = 0;
			Match("endevent");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x00046624 File Offset: 0x00044824
		public void mNATIVE()
		{
			var type = 47;
			var channel = 0;
			Match("native");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x00046668 File Offset: 0x00044868
		public void mGLOBAL()
		{
			var type = 46;
			var channel = 0;
			Match("global");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x000466AC File Offset: 0x000448AC
		public void mRETURN()
		{
			var type = 83;
			var channel = 0;
			Match("return");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x000466F0 File Offset: 0x000448F0
		public void mAS()
		{
			var type = 79;
			var channel = 0;
			Match("as");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x00046734 File Offset: 0x00044934
		public void mIF()
		{
			var type = 84;
			var channel = 0;
			Match("if");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x00046778 File Offset: 0x00044978
		public void mELSEIF()
		{
			var type = 86;
			var channel = 0;
			Match("elseif");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x000467BC File Offset: 0x000449BC
		public void mELSE()
		{
			var type = 87;
			var channel = 0;
			Match("else");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x00046800 File Offset: 0x00044A00
		public void mENDIF()
		{
			var type = 85;
			var channel = 0;
			Match("endif");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x00046844 File Offset: 0x00044A44
		public void mEXTENDS()
		{
			var type = 39;
			var channel = 0;
			Match("extends");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x00046888 File Offset: 0x00044A88
		public void mIMPORT()
		{
			var type = 42;
			var channel = 0;
			Match("import");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x000468CC File Offset: 0x00044ACC
		public void mAUTO()
		{
			var type = 50;
			var channel = 0;
			Match("auto");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x00046910 File Offset: 0x00044B10
		public void mAUTOREADONLY()
		{
			var type = 56;
			var channel = 0;
			Match("autoreadonly");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x00046954 File Offset: 0x00044B54
		public void mSTATE()
		{
			var type = 51;
			var channel = 0;
			Match("state");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x00046998 File Offset: 0x00044B98
		public void mENDSTATE()
		{
			var type = 52;
			var channel = 0;
			Match("endstate");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x000469DC File Offset: 0x00044BDC
		public void mPROPERTY()
		{
			var type = 54;
			var channel = 0;
			Match("property");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x00046A20 File Offset: 0x00044C20
		public void mENDPROPERTY()
		{
			var type = 53;
			var channel = 0;
			Match("endproperty");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x00046A64 File Offset: 0x00044C64
		public void mWHILE()
		{
			var type = 88;
			var channel = 0;
			Match("while");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x00046AA8 File Offset: 0x00044CA8
		public void mENDWHILE()
		{
			var type = 89;
			var channel = 0;
			Match("endwhile");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x00046AEC File Offset: 0x00044CEC
		public void mBASETYPE()
		{
			var type = 55;
			var channel = 0;
			var num = input.LA(1);
			int num2;
			if (num <= 102)
			{
				if (num == 98)
				{
					num2 = 4;
					goto IL_7B;
				}
				if (num == 102)
				{
					num2 = 2;
					goto IL_7B;
				}
			}
			else
			{
				if (num == 105)
				{
					num2 = 1;
					goto IL_7B;
				}
				if (num == 115)
				{
					num2 = 3;
					goto IL_7B;
				}
			}
			if (state.backtracking > 0)
			{
				state.failed = true;
				return;
			}
			var ex = new NoViableAltException("", 1, 0, input);
			throw ex;
			IL_7B:
			switch (num2)
			{
			case 1:
				Match("int");
				if (state.failed)
				{
					return;
				}
				break;
			case 2:
				Match("float");
				if (state.failed)
				{
					return;
				}
				break;
			case 3:
				Match("string");
				if (state.failed)
				{
					return;
				}
				break;
			case 4:
				Match("bool");
				if (state.failed)
				{
					return;
				}
				break;
			}
			state.type = type;
			state.channel = channel;
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x00046C14 File Offset: 0x00044E14
		public void mNONE()
		{
			var type = 92;
			var channel = 0;
			Match("none");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x00046C58 File Offset: 0x00044E58
		public void mNEW()
		{
			var type = 80;
			var channel = 0;
			Match("new");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x00046C9C File Offset: 0x00044E9C
		public void mLENGTH()
		{
			var type = 82;
			var channel = 0;
			Match("length");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x00046CE0 File Offset: 0x00044EE0
		public void mBOOL()
		{
			var type = 91;
			var channel = 0;
			var num = input.LA(1);
			int num2;
			if (num == 116)
			{
				num2 = 1;
			}
			else if (num == 102)
			{
				num2 = 2;
			}
			else
			{
				if (state.backtracking > 0)
				{
					state.failed = true;
					return;
				}
				var ex = new NoViableAltException("", 2, 0, input);
				throw ex;
			}
			switch (num2)
			{
			case 1:
				Match("true");
				if (state.failed)
				{
					return;
				}
				break;
			case 2:
				Match("false");
				if (state.failed)
				{
					return;
				}
				break;
			}
			state.type = type;
			state.channel = channel;
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x00046DA8 File Offset: 0x00044FA8
		public void mID()
		{
			var type = 38;
			var channel = 0;
			if ((input.LA(1) >= 65 && input.LA(1) <= 90) || input.LA(1) == 95 || (input.LA(1) >= 97 && input.LA(1) <= 122))
			{
				input.Consume();
				state.failed = false;
				for (;;)
				{
					var num = 2;
					var num2 = input.LA(1);
					if ((num2 >= 48 && num2 <= 57) || (num2 >= 65 && num2 <= 90) || num2 == 95 || (num2 >= 97 && num2 <= 122))
					{
						num = 1;
					}
					var num3 = num;
					if (num3 != 1)
					{
						goto IL_1AB;
					}
					if ((input.LA(1) < 48 || input.LA(1) > 57) && (input.LA(1) < 65 || input.LA(1) > 90) && input.LA(1) != 95 && (input.LA(1) < 97 || input.LA(1) > 122))
					{
						break;
					}
					input.Consume();
					state.failed = false;
				}
				if (state.backtracking > 0)
				{
					state.failed = true;
					return;
				}
				var ex = new MismatchedSetException(null, input);
				Recover(ex);
				throw ex;
				IL_1AB:
				state.type = type;
				state.channel = channel;
			}
			else
			{
				if (state.backtracking <= 0)
				{
					var ex2 = new MismatchedSetException(null, input);
					Recover(ex2);
					throw ex2;
				}
				state.failed = true;
			}
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x00046F78 File Offset: 0x00045178
		public void mINTEGER()
		{
			var type = 81;
			var channel = 0;
			var num = input.LA(1);
			int num2;
			if (num == 45 || (num >= 49 && num <= 57))
			{
				num2 = 1;
			}
			else if (num == 48)
			{
				var num3 = input.LA(2);
				if (num3 == 120)
				{
					num2 = 2;
				}
				else
				{
					num2 = 1;
				}
			}
			else
			{
				if (state.backtracking > 0)
				{
					state.failed = true;
					return;
				}
				var ex = new NoViableAltException("", 7, 0, input);
				throw ex;
			}
			switch (num2)
			{
			case 1:
			{
				var num4 = 2;
				var num5 = input.LA(1);
				if (num5 == 45)
				{
					num4 = 1;
				}
				var num6 = num4;
				if (num6 == 1)
				{
					mMINUS();
					if (state.failed)
					{
						return;
					}
				}
				var num7 = 0;
				for (;;)
				{
					var num8 = 2;
					var num9 = input.LA(1);
					if (num9 >= 48 && num9 <= 57)
					{
						num8 = 1;
					}
					var num10 = num8;
					if (num10 != 1)
					{
						goto IL_116;
					}
					mDIGIT();
					if (state.failed)
					{
						break;
					}
					num7++;
				}
				return;
				IL_116:
				if (num7 < 1)
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return;
					}
					var ex2 = new EarlyExitException(5, input);
					throw ex2;
				}
				break;
			}
			case 2:
			{
				Match("0x");
				if (state.failed)
				{
					return;
				}
				var num11 = 0;
				for (;;)
				{
					var num12 = 2;
					var num13 = input.LA(1);
					if ((num13 >= 48 && num13 <= 57) || (num13 >= 65 && num13 <= 70) || (num13 >= 97 && num13 <= 102))
					{
						num12 = 1;
					}
					var num14 = num12;
					if (num14 != 1)
					{
						goto IL_26E;
					}
					if ((input.LA(1) < 48 || input.LA(1) > 57) && (input.LA(1) < 65 || input.LA(1) > 70) && (input.LA(1) < 97 || input.LA(1) > 102))
					{
						break;
					}
					input.Consume();
					state.failed = false;
					num11++;
				}
				if (state.backtracking > 0)
				{
					state.failed = true;
					return;
				}
				var ex3 = new MismatchedSetException(null, input);
				Recover(ex3);
				throw ex3;
				IL_26E:
				if (num11 < 1)
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return;
					}
					var ex4 = new EarlyExitException(6, input);
					throw ex4;
				}
				else if (state.backtracking == 0)
				{
					Text = HexToDecString(Text);
				}
				break;
			}
			}
			state.type = type;
			state.channel = channel;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x00047268 File Offset: 0x00045468
		public void mFLOAT()
		{
			var type = 93;
			var channel = 0;
			var num = 2;
			var num2 = input.LA(1);
			if (num2 == 45)
			{
				num = 1;
			}
			var num3 = num;
			if (num3 == 1)
			{
				mMINUS();
				if (state.failed)
				{
					return;
				}
			}
			var num4 = 0;
			for (;;)
			{
				var num5 = 2;
				var num6 = input.LA(1);
				if (num6 >= 48 && num6 <= 57)
				{
					num5 = 1;
				}
				var num7 = num5;
				if (num7 != 1)
				{
					goto IL_7F;
				}
				mDIGIT();
				if (state.failed)
				{
					break;
				}
				num4++;
			}
			return;
			IL_7F:
			if (num4 < 1)
			{
				if (state.backtracking <= 0)
				{
					var ex = new EarlyExitException(9, input);
					throw ex;
				}
				state.failed = true;
			}
			else
			{
				var num8 = 2;
				var num9 = input.LA(1);
				if (num9 == 46)
				{
					num8 = 1;
				}
				var num10 = num8;
				if (num10 == 1)
				{
					mDOT();
					if (state.failed)
					{
						return;
					}
					var num11 = 0;
					for (;;)
					{
						var num12 = 2;
						var num13 = input.LA(1);
						if (num13 >= 48 && num13 <= 57)
						{
							num12 = 1;
						}
						var num14 = num12;
						if (num14 != 1)
						{
							goto IL_13C;
						}
						mDIGIT();
						if (state.failed)
						{
							break;
						}
						num11++;
					}
					return;
					IL_13C:
					if (num11 < 1)
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return;
						}
						var ex2 = new EarlyExitException(10, input);
						throw ex2;
					}
				}
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x00047404 File Offset: 0x00045604
		public void mSTRING()
		{
			var type = 90;
			var channel = 0;
			mDQUOTE();
			if (!state.failed)
			{
				for (;;)
				{
					var num = 6;
					var num2 = input.LA(1);
					if ((num2 >= 0 && num2 <= 9) || (num2 >= 11 && num2 <= 12) || (num2 >= 14 && num2 <= 33) || (num2 >= 35 && num2 <= 91) || (num2 >= 93 && num2 <= 65535))
					{
						num = 1;
					}
					else if (num2 == 92)
					{
						var num3 = input.LA(2);
						if (num3 <= 92)
						{
							if (num3 != 34)
							{
								if (num3 == 92)
								{
									num = 4;
								}
							}
							else
							{
								num = 5;
							}
						}
						else if (num3 != 110)
						{
							if (num3 == 116)
							{
								num = 3;
							}
						}
						else
						{
							num = 2;
						}
					}
					switch (num)
					{
					case 1:
						if ((input.LA(1) >= 0 && input.LA(1) <= 9) || (input.LA(1) >= 11 && input.LA(1) <= 12) || (input.LA(1) >= 14 && input.LA(1) <= 33) || (input.LA(1) >= 35 && input.LA(1) <= 91) || (input.LA(1) >= 93 && input.LA(1) <= 65535))
						{
							input.Consume();
							state.failed = false;
							continue;
						}
						goto IL_18D;
					case 2:
						Match("\\n");
						if (state.failed)
						{
							goto Block_20;
						}
						continue;
					case 3:
						Match("\\t");
						if (state.failed)
						{
							goto Block_21;
						}
						continue;
					case 4:
						Match("\\\\");
						if (state.failed)
						{
							goto Block_22;
						}
						continue;
					case 5:
						Match("\\\"");
						if (state.failed)
						{
							goto Block_23;
						}
						continue;
					}
					break;
				}
				mDQUOTE();
				if (state.failed)
				{
					return;
				}
				state.type = type;
				state.channel = channel;
				return;
				IL_18D:
				if (state.backtracking <= 0)
				{
					var ex = new MismatchedSetException(null, input);
					Recover(ex);
					throw ex;
				}
				state.failed = true;
				Block_20:
				Block_21:
				Block_22:
				Block_23:;
			}
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0004767C File Offset: 0x0004587C
		public void mDOCSTRING()
		{
			var type = 40;
			var channel = 0;
			mLBRACE();
			if (!state.failed)
			{
				do
				{
					var num = 2;
					var num2 = input.LA(1);
					if (num2 == 125)
					{
						num = 2;
					}
					else if ((num2 >= 0 && num2 <= 124) || (num2 >= 126 && num2 <= 65535))
					{
						num = 1;
					}
					var num3 = num;
					if (num3 != 1)
					{
						goto IL_6A;
					}
					MatchAny();
				}
				while (!state.failed);
				return;
				IL_6A:
				mRBRACE();
				if (!state.failed)
				{
					if (state.backtracking == 0)
					{
						Text = DocStringToEscapedString(Text);
					}
					state.type = type;
					state.channel = channel;
				}
			}
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x00047740 File Offset: 0x00045940
		public void mLPAREN()
		{
			var type = 43;
			var channel = 0;
			Match(40);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x00047784 File Offset: 0x00045984
		public void mRPAREN()
		{
			var type = 44;
			var channel = 0;
			Match(41);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x000477C8 File Offset: 0x000459C8
		public void mLBRACE()
		{
			var type = 99;
			var channel = 0;
			Match(123);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x0004780C File Offset: 0x00045A0C
		public void mRBRACE()
		{
			var type = 100;
			var channel = 0;
			Match(125);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x00047850 File Offset: 0x00045A50
		public void mLBRACKET()
		{
			var type = 63;
			var channel = 0;
			Match(91);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x00047894 File Offset: 0x00045A94
		public void mRBRACKET()
		{
			var type = 64;
			var channel = 0;
			Match(93);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x000478D8 File Offset: 0x00045AD8
		public void mCOMMA()
		{
			var type = 49;
			var channel = 0;
			Match(44);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x0004791C File Offset: 0x00045B1C
		public void mEQUALS()
		{
			var type = 41;
			var channel = 0;
			Match(61);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x00047960 File Offset: 0x00045B60
		public void mPLUS()
		{
			var type = 73;
			var channel = 0;
			Match(43);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x000479A4 File Offset: 0x00045BA4
		public void mMINUS()
		{
			var type = 74;
			var channel = 0;
			Match(45);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x000479E8 File Offset: 0x00045BE8
		public void mMULT()
		{
			var type = 75;
			var channel = 0;
			Match(42);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x00047A2C File Offset: 0x00045C2C
		public void mDIVIDE()
		{
			var type = 76;
			var channel = 0;
			Match(47);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x00047A70 File Offset: 0x00045C70
		public void mMOD()
		{
			var type = 77;
			var channel = 0;
			Match(37);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x00047AB4 File Offset: 0x00045CB4
		public void mDOT()
		{
			var type = 62;
			var channel = 0;
			Match(46);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x00047AF8 File Offset: 0x00045CF8
		public void mDQUOTE()
		{
			var type = 98;
			var channel = 0;
			Match(34);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x00047B3C File Offset: 0x00045D3C
		public void mNOT()
		{
			var type = 78;
			var channel = 0;
			Match(33);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x00047B80 File Offset: 0x00045D80
		public void mEQ()
		{
			var type = 67;
			var channel = 0;
			Match("==");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x00047BC4 File Offset: 0x00045DC4
		public void mNE()
		{
			var type = 68;
			var channel = 0;
			Match("!=");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x00047C08 File Offset: 0x00045E08
		public void mGT()
		{
			var type = 69;
			var channel = 0;
			Match(62);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x00047C4C File Offset: 0x00045E4C
		public void mLT()
		{
			var type = 70;
			var channel = 0;
			Match(60);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x00047C90 File Offset: 0x00045E90
		public void mGTE()
		{
			var type = 71;
			var channel = 0;
			Match(">=");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x00047CD4 File Offset: 0x00045ED4
		public void mLTE()
		{
			var type = 72;
			var channel = 0;
			Match("<=");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x00047D18 File Offset: 0x00045F18
		public void mOR()
		{
			var type = 65;
			var channel = 0;
			Match("||");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x00047D5C File Offset: 0x00045F5C
		public void mAND()
		{
			var type = 66;
			var channel = 0;
			Match("&&");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x00047DA0 File Offset: 0x00045FA0
		public void mPLUSEQUALS()
		{
			var type = 57;
			var channel = 0;
			Match("+=");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x00047DE4 File Offset: 0x00045FE4
		public void mMINUSEQUALS()
		{
			var type = 58;
			var channel = 0;
			Match("-=");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x00047E28 File Offset: 0x00046028
		public void mMULTEQUALS()
		{
			var type = 59;
			var channel = 0;
			Match("*=");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x00047E6C File Offset: 0x0004606C
		public void mDIVEQUALS()
		{
			var type = 60;
			var channel = 0;
			Match("/=");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x00047EB0 File Offset: 0x000460B0
		public void mMODEQUALS()
		{
			var type = 61;
			var channel = 0;
			Match("%=");
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x00047EF4 File Offset: 0x000460F4
		public void mEOL()
		{
			var type = 94;
			var channel = 0;
			var num = 2;
			var num2 = input.LA(1);
			if (num2 == 13)
			{
				num = 1;
			}
			var num3 = num;
			if (num3 == 1)
			{
				Match(13);
				if (state.failed)
				{
					return;
				}
			}
			Match(10);
			if (!state.failed)
			{
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x00047F6C File Offset: 0x0004616C
		public void mWS()
		{
			var type = 102;
			var channel = 0;
			var num = 0;
			for (;;)
			{
				var num2 = 2;
				var num3 = input.LA(1);
				if (num3 == 9 || num3 == 32)
				{
					num2 = 1;
				}
				var num4 = num2;
				if (num4 != 1)
				{
					goto IL_42;
				}
				mWS_CHAR();
				if (state.failed)
				{
					break;
				}
				num++;
			}
			return;
			IL_42:
			if (num < 1)
			{
				if (state.backtracking <= 0)
				{
					var ex = new EarlyExitException(15, input);
					throw ex;
				}
				state.failed = true;
			}
			else
			{
				if (state.backtracking == 0)
				{
					channel = 99;
				}
				state.type = type;
				state.channel = channel;
			}
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0004801C File Offset: 0x0004621C
		public void mEAT_EOL()
		{
			var type = 103;
			var channel = 0;
			Match(92);
			if (!state.failed)
			{
				do
				{
					var num = 2;
					var num2 = input.LA(1);
					if (num2 == 9 || num2 == 32)
					{
						num = 1;
					}
					var num3 = num;
					if (num3 != 1)
					{
						goto IL_54;
					}
					mWS_CHAR();
				}
				while (!state.failed);
				return;
				IL_54:
				mEOL();
				if (!state.failed)
				{
					if (state.backtracking == 0)
					{
						channel = 99;
					}
					state.type = type;
					state.channel = channel;
				}
			}
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x000480BC File Offset: 0x000462BC
		public void mCOMMENT()
		{
			var type = 104;
			var channel = 0;
			switch (dfa19.Predict(input))
			{
			case 1:
				Match(";/");
				if (state.failed)
				{
					return;
				}
				do
				{
					var num = 2;
					var num2 = input.LA(1);
					if (num2 == 47)
					{
						var num3 = input.LA(2);
						if (num3 == 59)
						{
							num = 2;
						}
						else if ((num3 >= 0 && num3 <= 58) || (num3 >= 60 && num3 <= 65535))
						{
							num = 1;
						}
					}
					else if ((num2 >= 0 && num2 <= 46) || (num2 >= 48 && num2 <= 65535))
					{
						num = 1;
					}
					var num4 = num;
					if (num4 != 1)
					{
						goto IL_DA;
					}
					MatchAny();
				}
				while (!state.failed);
				return;
				IL_DA:
				Match("/;");
				if (state.failed)
				{
					return;
				}
				if (state.backtracking == 0)
				{
					channel = 99;
				}
				break;
			case 2:
			{
				Match(59);
				if (state.failed)
				{
					return;
				}
				for (;;)
				{
					var num5 = 2;
					var num6 = input.LA(1);
					if ((num6 >= 0 && num6 <= 9) || (num6 >= 11 && num6 <= 65535))
					{
						num5 = 1;
					}
					var num7 = num5;
					if (num7 != 1)
					{
						goto IL_1F6;
					}
					if ((input.LA(1) < 0 || input.LA(1) > 9) && (input.LA(1) < 11 || input.LA(1) > 65535))
					{
						break;
					}
					input.Consume();
					state.failed = false;
				}
				if (state.backtracking > 0)
				{
					state.failed = true;
					return;
				}
				var ex = new MismatchedSetException(null, input);
				Recover(ex);
				throw ex;
				IL_1F6:
				if (state.backtracking == 0)
				{
					channel = 99;
				}
				break;
			}
			}
			state.type = type;
			state.channel = channel;
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x000482E8 File Offset: 0x000464E8
		public void mALPHA()
		{
			if ((input.LA(1) >= 65 && input.LA(1) <= 90) || (input.LA(1) >= 97 && input.LA(1) <= 122))
			{
				input.Consume();
				state.failed = false;
			}
			else
			{
				if (state.backtracking <= 0)
				{
					var ex = new MismatchedSetException(null, input);
					Recover(ex);
					throw ex;
				}
				state.failed = true;
			}
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x00048380 File Offset: 0x00046580
		public void mDIGIT()
		{
			MatchRange(48, 57);
			if (state.failed)
			{
			}
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0004839C File Offset: 0x0004659C
		public void mHEX_DIGIT()
		{
			if ((input.LA(1) >= 65 && input.LA(1) <= 70) || (input.LA(1) >= 97 && input.LA(1) <= 102))
			{
				input.Consume();
				state.failed = false;
			}
			else
			{
				if (state.backtracking <= 0)
				{
					var ex = new MismatchedSetException(null, input);
					Recover(ex);
					throw ex;
				}
				state.failed = true;
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x00048434 File Offset: 0x00046634
		public void mWS_CHAR()
		{
			if (input.LA(1) == 9 || input.LA(1) == 32)
			{
				input.Consume();
				state.failed = false;
			}
			else
			{
				if (state.backtracking <= 0)
				{
					var ex = new MismatchedSetException(null, input);
					Recover(ex);
					throw ex;
				}
				state.failed = true;
			}
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x000484AC File Offset: 0x000466AC
		public override void mTokens()
		{
			switch (dfa20.Predict(input))
			{
			case 1:
				mSCRIPTNAME();
				if (state.failed)
				{
					return;
				}
				break;
			case 2:
				mFUNCTION();
				if (state.failed)
				{
					return;
				}
				break;
			case 3:
				mENDFUNCTION();
				if (state.failed)
				{
					return;
				}
				break;
			case 4:
				mEVENT();
				if (state.failed)
				{
					return;
				}
				break;
			case 5:
				mENDEVENT();
				if (state.failed)
				{
					return;
				}
				break;
			case 6:
				mNATIVE();
				if (state.failed)
				{
					return;
				}
				break;
			case 7:
				mGLOBAL();
				if (state.failed)
				{
					return;
				}
				break;
			case 8:
				mRETURN();
				if (state.failed)
				{
					return;
				}
				break;
			case 9:
				mAS();
				if (state.failed)
				{
					return;
				}
				break;
			case 10:
				mIF();
				if (state.failed)
				{
					return;
				}
				break;
			case 11:
				mELSEIF();
				if (state.failed)
				{
					return;
				}
				break;
			case 12:
				mELSE();
				if (state.failed)
				{
					return;
				}
				break;
			case 13:
				mENDIF();
				if (state.failed)
				{
					return;
				}
				break;
			case 14:
				mEXTENDS();
				if (state.failed)
				{
					return;
				}
				break;
			case 15:
				mIMPORT();
				if (state.failed)
				{
					return;
				}
				break;
			case 16:
				mAUTO();
				if (state.failed)
				{
					return;
				}
				break;
			case 17:
				mAUTOREADONLY();
				if (state.failed)
				{
					return;
				}
				break;
			case 18:
				mSTATE();
				if (state.failed)
				{
					return;
				}
				break;
			case 19:
				mENDSTATE();
				if (state.failed)
				{
					return;
				}
				break;
			case 20:
				mPROPERTY();
				if (state.failed)
				{
					return;
				}
				break;
			case 21:
				mENDPROPERTY();
				if (state.failed)
				{
					return;
				}
				break;
			case 22:
				mWHILE();
				if (state.failed)
				{
					return;
				}
				break;
			case 23:
				mENDWHILE();
				if (state.failed)
				{
					return;
				}
				break;
			case 24:
				mBASETYPE();
				if (state.failed)
				{
					return;
				}
				break;
			case 25:
				mNONE();
				if (state.failed)
				{
					return;
				}
				break;
			case 26:
				mNEW();
				if (state.failed)
				{
					return;
				}
				break;
			case 27:
				mLENGTH();
				if (state.failed)
				{
					return;
				}
				break;
			case 28:
				mBOOL();
				if (state.failed)
				{
					return;
				}
				break;
			case 29:
				mID();
				if (state.failed)
				{
					return;
				}
				break;
			case 30:
				mINTEGER();
				if (state.failed)
				{
					return;
				}
				break;
			case 31:
				mFLOAT();
				if (state.failed)
				{
					return;
				}
				break;
			case 32:
				mSTRING();
				if (state.failed)
				{
					return;
				}
				break;
			case 33:
				mDOCSTRING();
				if (state.failed)
				{
					return;
				}
				break;
			case 34:
				mLPAREN();
				if (state.failed)
				{
					return;
				}
				break;
			case 35:
				mRPAREN();
				if (state.failed)
				{
					return;
				}
				break;
			case 36:
				mLBRACE();
				if (state.failed)
				{
					return;
				}
				break;
			case 37:
				mRBRACE();
				if (state.failed)
				{
					return;
				}
				break;
			case 38:
				mLBRACKET();
				if (state.failed)
				{
					return;
				}
				break;
			case 39:
				mRBRACKET();
				if (state.failed)
				{
					return;
				}
				break;
			case 40:
				mCOMMA();
				if (state.failed)
				{
					return;
				}
				break;
			case 41:
				mEQUALS();
				if (state.failed)
				{
					return;
				}
				break;
			case 42:
				mPLUS();
				if (state.failed)
				{
					return;
				}
				break;
			case 43:
				mMINUS();
				if (state.failed)
				{
					return;
				}
				break;
			case 44:
				mMULT();
				if (state.failed)
				{
					return;
				}
				break;
			case 45:
				mDIVIDE();
				if (state.failed)
				{
					return;
				}
				break;
			case 46:
				mMOD();
				if (state.failed)
				{
					return;
				}
				break;
			case 47:
				mDOT();
				if (state.failed)
				{
					return;
				}
				break;
			case 48:
				mDQUOTE();
				if (state.failed)
				{
					return;
				}
				break;
			case 49:
				mNOT();
				if (state.failed)
				{
					return;
				}
				break;
			case 50:
				mEQ();
				if (state.failed)
				{
					return;
				}
				break;
			case 51:
				mNE();
				if (state.failed)
				{
					return;
				}
				break;
			case 52:
				mGT();
				if (state.failed)
				{
					return;
				}
				break;
			case 53:
				mLT();
				if (state.failed)
				{
					return;
				}
				break;
			case 54:
				mGTE();
				if (state.failed)
				{
					return;
				}
				break;
			case 55:
				mLTE();
				if (state.failed)
				{
					return;
				}
				break;
			case 56:
				mOR();
				if (state.failed)
				{
					return;
				}
				break;
			case 57:
				mAND();
				if (state.failed)
				{
					return;
				}
				break;
			case 58:
				mPLUSEQUALS();
				if (state.failed)
				{
					return;
				}
				break;
			case 59:
				mMINUSEQUALS();
				if (state.failed)
				{
					return;
				}
				break;
			case 60:
				mMULTEQUALS();
				if (state.failed)
				{
					return;
				}
				break;
			case 61:
				mDIVEQUALS();
				if (state.failed)
				{
					return;
				}
				break;
			case 62:
				mMODEQUALS();
				if (state.failed)
				{
					return;
				}
				break;
			case 63:
				mEOL();
				if (state.failed)
				{
					return;
				}
				break;
			case 64:
				mWS();
				if (state.failed)
				{
					return;
				}
				break;
			case 65:
				mEAT_EOL();
				if (state.failed)
				{
					return;
				}
				break;
			case 66:
			{
				mCOMMENT();
				var failed = state.failed;
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x00048BB8 File Offset: 0x00046DB8
		public void synpred1_Papyrus_fragment()
		{
			Match(";/");
			var failed = state.failed;
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x00048BD4 File Offset: 0x00046DD4
		public bool synpred1_Papyrus()
		{
			state.backtracking++;
			var marker = input.Mark();
			try
			{
				synpred1_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine($"impossible: {arg}");
			}
			var result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x00048C6C File Offset: 0x00046E6C
		private void InitializeCyclicDFAs()
		{
			dfa19 = new DFA19(this);
			dfa20 = new DFA20(this);
			dfa19.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA19_SpecialStateTransition);
			dfa20.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA20_SpecialStateTransition);
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x00048CC0 File Offset: 0x00046EC0
		protected internal int DFA19_SpecialStateTransition(DFA dfa, int s, IIntStream _input)
		{
			var stateNumber = s;
			switch (s)
			{
			case 0:
			{
				_input.LA(1);
				var index = _input.Index();
				_input.Rewind();
                s = synpred1_Papyrus() ? 5 : 3;
				_input.Seek(index);
                return s;
            }
			case 1:
			{
				var num = _input.LA(1);
				var index2 = _input.Index();
				_input.Rewind();
                s = num switch
                {
                    47 => 4,
                    >= 0 and <= 9 => 6,
                    >= 11 and <= 46 => 6,
                    >= 48 and <= 65535 => 6,
                    10 when synpred1_Papyrus() => 5,
                    _ => 3
                };
                _input.Seek(index2);
                return s;
            }
			case 2:
			{
				var num2 = _input.LA(1);
				var index3 = _input.Index();
				_input.Rewind();
                s = num2 switch
                {
                    47 => 4,
                    10 when synpred1_Papyrus() => 5,
                    >= 0 and <= 9 => 6,
                    >= 11 and <= 46 => 6,
                    >= 48 and <= 65535 => 6,
                    _ => 3
                };
                _input.Seek(index3);
                return s;
            }
			case 3:
			{
				var num3 = _input.LA(1);
				var index4 = _input.Index();
				_input.Rewind();
                s = num3 switch
                {
                    59 => 7,
                    47 => 4,
                    >= 0 and <= 9 => 6,
                    >= 11 and <= 46 => 6,
                    >= 48 and <= 58 => 6,
                    >= 60 and <= 65535 => 6,
                    10 when synpred1_Papyrus() => 5,
                    _ => 3
                };
                _input.Seek(index4);
                return s;
            }
			}
			if (state.backtracking > 0)
			{
				state.failed = true;
				return -1;
			}
			var ex = new NoViableAltException(dfa.Description, 19, stateNumber, _input);
			dfa.Error(ex);
			throw ex;
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x00048ED0 File Offset: 0x000470D0
		protected internal int DFA20_SpecialStateTransition(DFA dfa, int s, IIntStream _input)
		{
			var stateNumber = s;
			switch (s)
			{
			case 0:
			{
				var num = _input.LA(1);
                s = num is >= 0 and <= 65535 ? 72 : 71;
                return s;
            }
			case 1:
			{
				var num2 = _input.LA(1);
                s = num2 is >= 0 and <= 9 or >= 11 and <= 12 or >= 14 and <= 65535 ? 70 : 69;
                return s;
            }
			}
			if (state.backtracking > 0)
			{
				state.failed = true;
				return -1;
			}
			var ex = new NoViableAltException(dfa.Description, 20, stateNumber, _input);
			dfa.Error(ex);
			throw ex;
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x00048F90 File Offset: 0x00047190
		private string HexToDecString(string asHexString)
		{
			asHexString = asHexString[2..];
			return int.Parse(asHexString, NumberStyles.AllowHexSpecifier).ToString();
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x00048FBC File Offset: 0x000471BC
		private string DocStringToEscapedString(string asDocString)
		{
			var text = asDocString[1..^1];
			text = text.Trim();
			var stringBuilder = new StringBuilder();
			foreach (var c in text)
            {
                switch (c)
                {
                    case '\n':
                        stringBuilder.Append("\\n");
                        break;
                    case '\t':
                        stringBuilder.Append("\\t");
                        break;
                    case '\\':
                        stringBuilder.Append("\\\\");
                        break;
                    case '"':
                        stringBuilder.Append("\\\"");
                        break;
                    default:
                    {
                        if (c != '\r')
                        {
                            stringBuilder.Append(c);
                        }

                        break;
                    }
                }
            }
			return stringBuilder.ToString();
		}

		// Token: 0x0400074A RID: 1866
		public const int FUNCTION = 6;

		// Token: 0x0400074B RID: 1867
		public const int LT = 70;

		// Token: 0x0400074C RID: 1868
		public const int WHILE = 88;

		// Token: 0x0400074D RID: 1869
		public const int DIVEQUALS = 60;

		// Token: 0x0400074E RID: 1870
		public const int MOD = 77;

		// Token: 0x0400074F RID: 1871
		public const int PROPSET = 21;

		// Token: 0x04000750 RID: 1872
		public const int NEW = 80;

		// Token: 0x04000751 RID: 1873
		public const int DQUOTE = 98;

		// Token: 0x04000752 RID: 1874
		public const int PARAM = 9;

		// Token: 0x04000753 RID: 1875
		public const int EQUALS = 41;

		// Token: 0x04000754 RID: 1876
		public const int NOT = 78;

		// Token: 0x04000755 RID: 1877
		public const int EOF = -1;

		// Token: 0x04000756 RID: 1878
		public const int FNEGATE = 35;

		// Token: 0x04000757 RID: 1879
		public const int LBRACKET = 63;

		// Token: 0x04000758 RID: 1880
		public const int USER_FLAGS = 18;

		// Token: 0x04000759 RID: 1881
		public const int RPAREN = 44;

		// Token: 0x0400075A RID: 1882
		public const int IMPORT = 42;

		// Token: 0x0400075B RID: 1883
		public const int EOL = 94;

		// Token: 0x0400075C RID: 1884
		public const int FADD = 27;

		// Token: 0x0400075D RID: 1885
		public const int RETURN = 83;

		// Token: 0x0400075E RID: 1886
		public const int ENDIF = 85;

		// Token: 0x0400075F RID: 1887
		public const int VAR = 5;

		// Token: 0x04000760 RID: 1888
		public const int ENDWHILE = 89;

		// Token: 0x04000761 RID: 1889
		public const int EQ = 67;

		// Token: 0x04000762 RID: 1890
		public const int IMULTIPLY = 30;

		// Token: 0x04000763 RID: 1891
		public const int COMMENT = 104;

		// Token: 0x04000764 RID: 1892
		public const int IDIVIDE = 32;

		// Token: 0x04000765 RID: 1893
		public const int DIVIDE = 76;

		// Token: 0x04000766 RID: 1894
		public const int NE = 68;

		// Token: 0x04000767 RID: 1895
		public const int SCRIPTNAME = 37;

		// Token: 0x04000768 RID: 1896
		public const int MINUSEQUALS = 58;

		// Token: 0x04000769 RID: 1897
		public const int ARRAYFIND = 24;

		// Token: 0x0400076A RID: 1898
		public const int RBRACE = 100;

		// Token: 0x0400076B RID: 1899
		public const int ELSE = 87;

		// Token: 0x0400076C RID: 1900
		public const int BOOL = 91;

		// Token: 0x0400076D RID: 1901
		public const int NATIVE = 47;

		// Token: 0x0400076E RID: 1902
		public const int FDIVIDE = 33;

		// Token: 0x0400076F RID: 1903
		public const int UNARY_MINUS = 16;

		// Token: 0x04000770 RID: 1904
		public const int MULT = 75;

		// Token: 0x04000771 RID: 1905
		public const int ENDPROPERTY = 53;

		// Token: 0x04000772 RID: 1906
		public const int CALLPARAMS = 14;

		// Token: 0x04000773 RID: 1907
		public const int ALPHA = 95;

		// Token: 0x04000774 RID: 1908
		public const int WS = 102;

		// Token: 0x04000775 RID: 1909
		public const int FMULTIPLY = 31;

		// Token: 0x04000776 RID: 1910
		public const int ARRAYSET = 23;

		// Token: 0x04000777 RID: 1911
		public const int PROPERTY = 54;

		// Token: 0x04000778 RID: 1912
		public const int AUTOREADONLY = 56;

		// Token: 0x04000779 RID: 1913
		public const int NONE = 92;

		// Token: 0x0400077A RID: 1914
		public const int OR = 65;

		// Token: 0x0400077B RID: 1915
		public const int PROPGET = 20;

		// Token: 0x0400077C RID: 1916
		public const int IADD = 26;

		// Token: 0x0400077D RID: 1917
		public const int PROPFUNC = 17;

		// Token: 0x0400077E RID: 1918
		public const int GT = 69;

		// Token: 0x0400077F RID: 1919
		public const int CALL = 11;

		// Token: 0x04000780 RID: 1920
		public const int INEGATE = 34;

		// Token: 0x04000781 RID: 1921
		public const int BASETYPE = 55;

		// Token: 0x04000782 RID: 1922
		public const int ENDEVENT = 48;

		// Token: 0x04000783 RID: 1923
		public const int MULTEQUALS = 59;

		// Token: 0x04000784 RID: 1924
		public const int CALLPARENT = 13;

		// Token: 0x04000785 RID: 1925
		public const int LBRACE = 99;

		// Token: 0x04000786 RID: 1926
		public const int GTE = 71;

		// Token: 0x04000787 RID: 1927
		public const int FLOAT = 93;

		// Token: 0x04000788 RID: 1928
		public const int ENDSTATE = 52;

		// Token: 0x04000789 RID: 1929
		public const int ID = 38;

		// Token: 0x0400078A RID: 1930
		public const int AND = 66;

		// Token: 0x0400078B RID: 1931
		public const int LTE = 72;

		// Token: 0x0400078C RID: 1932
		public const int LPAREN = 43;

		// Token: 0x0400078D RID: 1933
		public const int LENGTH = 82;

		// Token: 0x0400078E RID: 1934
		public const int IF = 84;

		// Token: 0x0400078F RID: 1935
		public const int CALLGLOBAL = 12;

		// Token: 0x04000790 RID: 1936
		public const int AS = 79;

		// Token: 0x04000791 RID: 1937
		public const int OBJECT = 4;

		// Token: 0x04000792 RID: 1938
		public const int COMMA = 49;

		// Token: 0x04000793 RID: 1939
		public const int PLUSEQUALS = 57;

		// Token: 0x04000794 RID: 1940
		public const int AUTO = 50;

		// Token: 0x04000795 RID: 1941
		public const int ISUBTRACT = 28;

		// Token: 0x04000796 RID: 1942
		public const int PLUS = 73;

		// Token: 0x04000797 RID: 1943
		public const int ENDFUNCTION = 45;

		// Token: 0x04000798 RID: 1944
		public const int DIGIT = 96;

		// Token: 0x04000799 RID: 1945
		public const int HEADER = 8;

		// Token: 0x0400079A RID: 1946
		public const int RBRACKET = 64;

		// Token: 0x0400079B RID: 1947
		public const int DOT = 62;

		// Token: 0x0400079C RID: 1948
		public const int FSUBTRACT = 29;

		// Token: 0x0400079D RID: 1949
		public const int STRCAT = 36;

		// Token: 0x0400079E RID: 1950
		public const int INTEGER = 81;

		// Token: 0x0400079F RID: 1951
		public const int STATE = 51;

		// Token: 0x040007A0 RID: 1952
		public const int DOCSTRING = 40;

		// Token: 0x040007A1 RID: 1953
		public const int WS_CHAR = 101;

		// Token: 0x040007A2 RID: 1954
		public const int HEX_DIGIT = 97;

		// Token: 0x040007A3 RID: 1955
		public const int ARRAYRFIND = 25;

		// Token: 0x040007A4 RID: 1956
		public const int MINUS = 74;

		// Token: 0x040007A5 RID: 1957
		public const int EVENT = 7;

		// Token: 0x040007A6 RID: 1958
		public const int ARRAYGET = 22;

		// Token: 0x040007A7 RID: 1959
		public const int ELSEIF = 86;

		// Token: 0x040007A8 RID: 1960
		public const int AUTOPROP = 19;

		// Token: 0x040007A9 RID: 1961
		public const int PAREXPR = 15;

		// Token: 0x040007AA RID: 1962
		public const int BLOCK = 10;

		// Token: 0x040007AB RID: 1963
		public const int EAT_EOL = 103;

		// Token: 0x040007AC RID: 1964
		public const int GLOBAL = 46;

		// Token: 0x040007AD RID: 1965
		public const int MODEQUALS = 61;

		// Token: 0x040007AE RID: 1966
		public const int EXTENDS = 39;

		// Token: 0x040007AF RID: 1967
		public const int STRING = 90;

		// Token: 0x040007B0 RID: 1968
		private const string DFA19_eotS = "\u0001￿\u0002\u0003\u0001￿\u0001\u0003\u0001￿\u0001\u0003\u0001￿";

		// Token: 0x040007B1 RID: 1969
		private const string DFA19_eofS = "\b￿";

		// Token: 0x040007B2 RID: 1970
		private const string DFA19_minS = "\u0001;\u0001/\u0001\0\u0001￿\u0001\0\u0001￿\u0002\0";

		// Token: 0x040007B3 RID: 1971
		private const string DFA19_maxS = "\u0001;\u0001/\u0001￿\u0001￿\u0001￿\u0001￿\u0001￿\u0001\0";

		// Token: 0x040007B4 RID: 1972
		private const string DFA19_acceptS = "\u0003￿\u0001\u0002\u0001￿\u0001\u0001\u0002￿";

		// Token: 0x040007B5 RID: 1973
		private const string DFA19_specialS = "\u0002￿\u0001\u0002\u0001￿\u0001\u0003\u0001￿\u0001\u0001\u0001\0}>";

		// Token: 0x040007B6 RID: 1974
		private const string DFA20_eotS = "\u0001￿\r\u000e\u0001￿\u0001B\u0002C\u0001E\u0001G\u0006￿\u0001J\u0001L\u0001N\u0001P\u0001R\u0001￿\u0001T\u0001V\u0001X\u0006￿\u000e\u000e\u0001h\u0001\u000e\u0001j\a\u000e\u0018￿\f\u000e\u0001\u0083\u0002\u000e\u0001￿\u0001\u000e\u0001￿\u0001\u000e\u0001\u0088\u0012\u000e\u0001\u009c\u0002\u000e\u0001\u009f\u0001￿\u0002\u000e\u0001£\u0001\u000e\u0001￿\u0002\u000e\u0001\u0088\u0001\u000e\u0001¨\u0001\u000e\u0001ª\u0002\u000e\u0001\u0088\u0001¨\u0002\u000e\u0001¯\u0003\u000e\u0001³\u0001\u000e\u0001￿\u0002\u000e\u0001￿\u0003\u000e\u0001￿\u0002\u000e\u0001¼\u0001\u000e\u0001￿\u0001\u000e\u0001￿\u0001\u0088\u0003\u000e\u0001￿\u0003\u000e\u0001￿\u0001Å\u0001\u000e\u0001Ç\u0001È\u0001É\u0001\u000e\u0001Ë\u0001\u000e\u0001￿\u0001Í\a\u000e\u0001￿\u0001Õ\u0003￿\u0001\u000e\u0001￿\u0001\u000e\u0001￿\u0001\u000e\u0001Ù\u0001\u000e\u0001Û\u0001Ü\u0001\u000e\u0001Þ\u0001￿\u0001\u000e\u0001à\u0001\u000e\u0001￿\u0001\u000e\u0002￿\u0001\u000e\u0001￿\u0001\u000e\u0001￿\u0001å\u0003\u000e\u0001￿\u0001é\u0001ê\u0001\u000e\u0002￿\u0001ì\u0001￿";

		// Token: 0x040007B7 RID: 1975
		private const string DFA20_eofS = "í￿";

		// Token: 0x040007B8 RID: 1976
		private const string DFA20_minS = "\u0001\t\u0001c\u0001a\u0001l\u0001a\u0001l\u0001e\u0001s\u0001f\u0001r\u0001h\u0001o\u0001e\u0001r\u0001￿\u00010\u0002.\u0002\0\u0006￿\u0005=\u0001￿\u0003=\u0006￿\u0001r\u0001a\u0001n\u0001o\u0001l\u0001d\u0001e\u0001s\u0002t\u0001n\u0001w\u0001o\u0001t\u00010\u0001t\u00010\u0001p\u0001t\u0001o\u0001i\u0001o\u0001n\u0001u\u0018￿\u0001i\u0001t\u0001i\u0001c\u0001a\u0001s\u0001e\u0001n\u0002e\u0001i\u0001e\u00010\u0001b\u0001u\u0001￿\u0001o\u0001￿\u0001o\u00010\u0001p\u0002l\u0001g\u0001e\u0001p\u0001e\u0001n\u0002t\u0001e\u0001u\u0001v\u0001f\u0001t\u0001r\u0001h\u0001t\u00010\u0001n\u0001v\u00010\u0001￿\u0001a\u0001r\u00010\u0001r\u0001￿\u0002e\u00010\u0001t\u00010\u0001t\u00010\u0001g\u0001i\u00020\u0001n\u0001e\u00010\u0001a\u0001o\u0001i\u00010\u0001f\u0001￿\u0001d\u0001e\u0001￿\u0001l\u0001n\u0001e\u0001￿\u0001t\u0001r\u00010\u0001h\u0001￿\u0001n\u0001￿\u00010\u0001o\u0001c\u0001n\u0001￿\u0001t\u0001p\u0001l\u0001￿\u00010\u0001s\u00030\u0001a\u00010\u0001t\u0001￿\u00010\u0001a\u0001n\u0002t\u0003e\u0001￿\u00010\u0003￿\u0001d\u0001￿\u0001y\u0001￿\u0001m\u00010\u0001i\u00020\u0001r\u00010\u0001￿\u0001o\u00010\u0001e\u0001￿\u0001o\u0002￿\u0001t\u0001￿\u0001n\u0001￿\u00010\u0001n\u0001y\u0001l\u0001￿\u00020\u0001y\u0002￿\u00010\u0001￿";

		// Token: 0x040007B9 RID: 1977
		private const string DFA20_maxS = "\u0001}\u0001t\u0001u\u0001x\u0001o\u0001l\u0001e\u0001u\u0001n\u0001r\u0001h\u0001o\u0001e\u0001r\u0001￿\u0001=\u00029\u0002￿\u0006￿\u0005=\u0001￿\u0003=\u0006￿\u0002r\u0001n\u0001o\u0001l\u0001d\u0001e\u0001s\u0002t\u0001n\u0001w\u0001o\u0001t\u0001z\u0001t\u0001z\u0001p\u0001t\u0001o\u0001i\u0001o\u0001n\u0001u\u0018￿\u0001i\u0001t\u0001i\u0001c\u0001a\u0001s\u0001w\u0001n\u0002e\u0001i\u0001e\u0001z\u0001b\u0001u\u0001￿\u0001o\u0001￿\u0001o\u0001z\u0001p\u0002l\u0001g\u0001e\u0001p\u0001e\u0001n\u0002t\u0001e\u0001u\u0001v\u0001f\u0001t\u0001r\u0001h\u0001t\u0001z\u0001n\u0001v\u0001z\u0001￿\u0001a\u0001r\u0001z\u0001r\u0001￿\u0002e\u0001z\u0001t\u0001z\u0001t\u0001z\u0001g\u0001i\u0002z\u0001n\u0001e\u0001z\u0001a\u0001o\u0001i\u0001z\u0001f\u0001￿\u0001d\u0001e\u0001￿\u0001l\u0001n\u0001e\u0001￿\u0001t\u0001r\u0001z\u0001h\u0001￿\u0001n\u0001￿\u0001z\u0001o\u0001c\u0001n\u0001￿\u0001t\u0001p\u0001l\u0001￿\u0001z\u0001s\u0003z\u0001a\u0001z\u0001t\u0001￿\u0001z\u0001a\u0001n\u0002t\u0003e\u0001￿\u0001z\u0003￿\u0001d\u0001￿\u0001y\u0001￿\u0001m\u0001z\u0001i\u0002z\u0001r\u0001z\u0001￿\u0001o\u0001z\u0001e\u0001￿\u0001o\u0002￿\u0001t\u0001￿\u0001n\u0001￿\u0001z\u0001n\u0001y\u0001l\u0001￿\u0002z\u0001y\u0002￿\u0001z\u0001￿";

		// Token: 0x040007BA RID: 1978
		private const string DFA20_acceptS = "\u000e￿\u0001\u001d\u0005￿\u0001\"\u0001#\u0001%\u0001&\u0001'\u0001(\u0005￿\u0001/\u0003￿\u00018\u00019\u0001?\u0001@\u0001A\u0001B\u0018￿\u0001;\u0001+\u0001\u001e\u0001\u001f\u00010\u0001 \u0001$\u0001!\u00012\u0001)\u0001:\u0001*\u0001<\u0001,\u0001=\u0001-\u0001>\u0001.\u00013\u00011\u00016\u00014\u00017\u00015\u000f￿\u0001\t\u0001￿\u0001\n\u0018￿\u0001\u001a\u0004￿\u0001\u0018\u0013￿\u0001\f\u0002￿\u0001\u0019\u0003￿\u0001\u0010\u0004￿\u0001\u001c\u0001￿\u0001\u0012\u0004￿\u0001\r\u0003￿\u0001\u0004\b￿\u0001\u0016\b￿\u0001\v\u0001￿\u0001\u0006\u0001\a\u0001\b\u0001￿\u0001\u000f\u0001￿\u0001\u001b\a￿\u0001\u000e\u0003￿\u0001\u0002\u0001￿\u0001\u0005\u0001\u0013\u0001￿\u0001\u0017\u0001￿\u0001\u0014\u0004￿\u0001\u0001\u0003￿\u0001\u0003\u0001\u0015\u0001￿\u0001\u0011";

		// Token: 0x040007BB RID: 1979
		private const string DFA20_specialS = "\u0012￿\u0001\u0001\u0001\0Ù￿}>";

		// Token: 0x040007BD RID: 1981
		protected DFA19 dfa19;

		// Token: 0x040007BE RID: 1982
		protected DFA20 dfa20;

		// Token: 0x040007BF RID: 1983
		private static readonly string[] DFA19_transitionS = {
			"\u0001\u0001",
			"\u0001\u0002",
			"\n\u0006\u0001\u0005$\u0006\u0001\u0004￐\u0006",
			"",
			"\n\u0006\u0001\u0005$\u0006\u0001\u0004\v\u0006\u0001\aￄ\u0006",
			"",
			"\n\u0006\u0001\u0005$\u0006\u0001\u0004￐\u0006",
			"\u0001￿"
		};

		// Token: 0x040007C0 RID: 1984
		private static readonly short[] DFA19_eot = DFA.UnpackEncodedString("\u0001￿\u0002\u0003\u0001￿\u0001\u0003\u0001￿\u0001\u0003\u0001￿");

		// Token: 0x040007C1 RID: 1985
		private static readonly short[] DFA19_eof = DFA.UnpackEncodedString("\b￿");

		// Token: 0x040007C2 RID: 1986
		private static readonly char[] DFA19_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001;\u0001/\u0001\0\u0001￿\u0001\0\u0001￿\u0002\0");

		// Token: 0x040007C3 RID: 1987
		private static readonly char[] DFA19_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001;\u0001/\u0001￿\u0001￿\u0001￿\u0001￿\u0001￿\u0001\0");

		// Token: 0x040007C4 RID: 1988
		private static readonly short[] DFA19_accept = DFA.UnpackEncodedString("\u0003￿\u0001\u0002\u0001￿\u0001\u0001\u0002￿");

		// Token: 0x040007C5 RID: 1989
		private static readonly short[] DFA19_special = DFA.UnpackEncodedString("\u0002￿\u0001\u0002\u0001￿\u0001\u0003\u0001￿\u0001\u0001\u0001\0}>");

		// Token: 0x040007C6 RID: 1990
		private static readonly short[][] DFA19_transition = DFA.UnpackEncodedStringArray(DFA19_transitionS);

		// Token: 0x040007C7 RID: 1991
		private static readonly string[] DFA20_transitionS = {
			"\u0001&\u0001%\u0002￿\u0001%\u0012￿\u0001&\u0001 \u0001\u0012\u0002￿\u0001\u001e\u0001$\u0001￿\u0001\u0014\u0001\u0015\u0001\u001c\u0001\u001b\u0001\u0019\u0001\u000f\u0001\u001f\u0001\u001d\u0001\u0010\t\u0011\u0001￿\u0001(\u0001\"\u0001\u001a\u0001!\u0002￿\u001a\u000e\u0001\u0017\u0001'\u0001\u0018\u0001￿\u0001\u000e\u0001￿\u0001\a\u0001\v\u0002\u000e\u0001\u0003\u0001\u0002\u0001\u0005\u0001\u000e\u0001\b\u0002\u000e\u0001\f\u0001\u000e\u0001\u0004\u0001\u000e\u0001\t\u0001\u000e\u0001\u0006\u0001\u0001\u0001\r\u0002\u000e\u0001\n\u0003\u000e\u0001\u0013\u0001#\u0001\u0016",
			"\u0001)\u0010￿\u0001*",
			"\u0001-\n￿\u0001,\b￿\u0001+",
			"\u00010\u0001￿\u0001.\a￿\u0001/\u0001￿\u00011",
			"\u00012\u0003￿\u00014\t￿\u00013",
			"\u00015",
			"\u00016",
			"\u00017\u0001￿\u00018",
			"\u00019\u0006￿\u0001:\u0001;",
			"\u0001<",
			"\u0001=",
			"\u0001>",
			"\u0001?",
			"\u0001@",
			"",
			"\n\u0011\u0003￿\u0001A",
			"\u0001D\u0001￿\n\u0011",
			"\u0001D\u0001￿\n\u0011",
			"\nF\u0001￿\u0002F\u0001￿￲F",
			"\0H",
			"",
			"",
			"",
			"",
			"",
			"",
			"\u0001I",
			"\u0001K",
			"\u0001M",
			"\u0001O",
			"\u0001Q",
			"",
			"\u0001S",
			"\u0001U",
			"\u0001W",
			"",
			"",
			"",
			"",
			"",
			"",
			"\u0001Y",
			"\u0001Z\u0010￿\u0001[",
			"\u0001\\",
			"\u0001]",
			"\u0001^",
			"\u0001_",
			"\u0001`",
			"\u0001a",
			"\u0001b",
			"\u0001c",
			"\u0001d",
			"\u0001e",
			"\u0001f",
			"\u0001g",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001i",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001k",
			"\u0001l",
			"\u0001m",
			"\u0001n",
			"\u0001o",
			"\u0001p",
			"\u0001q",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"\u0001r",
			"\u0001s",
			"\u0001t",
			"\u0001u",
			"\u0001v",
			"\u0001w",
			"\u0001y\u0001x\u0002￿\u0001z\u0006￿\u0001|\u0002￿\u0001{\u0003￿\u0001}",
			"\u0001~",
			"\u0001\u007f",
			"\u0001\u0080",
			"\u0001\u0081",
			"\u0001\u0082",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001\u0084",
			"\u0001\u0085",
			"",
			"\u0001\u0086",
			"",
			"\u0001\u0087",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001\u0089",
			"\u0001\u008a",
			"\u0001\u008b",
			"\u0001\u008c",
			"\u0001\u008d",
			"\u0001\u008e",
			"\u0001\u008f",
			"\u0001\u0090",
			"\u0001\u0091",
			"\u0001\u0092",
			"\u0001\u0093",
			"\u0001\u0094",
			"\u0001\u0095",
			"\u0001\u0096",
			"\u0001\u0097",
			"\u0001\u0098",
			"\u0001\u0099",
			"\u0001\u009a",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\b\u000e\u0001\u009b\u0011\u000e",
			"\u0001\u009d",
			"\u0001\u009e",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"",
			"\u0001\u00a0",
			"\u0001¡",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u0011\u000e\u0001¢\b\u000e",
			"\u0001¤",
			"",
			"\u0001¥",
			"\u0001¦",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001§",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001©",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001«",
			"\u0001¬",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001­",
			"\u0001®",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001°",
			"\u0001±",
			"\u0001²",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001´",
			"",
			"\u0001µ",
			"\u0001¶",
			"",
			"\u0001·",
			"\u0001¸",
			"\u0001¹",
			"",
			"\u0001º",
			"\u0001»",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001½",
			"",
			"\u0001¾",
			"",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001¿",
			"\u0001À",
			"\u0001Á",
			"",
			"\u0001Â",
			"\u0001Ã",
			"\u0001Ä",
			"",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001Æ",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001Ê",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001Ì",
			"",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001Î",
			"\u0001Ï",
			"\u0001Ð",
			"\u0001Ñ",
			"\u0001Ò",
			"\u0001Ó",
			"\u0001Ô",
			"",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"",
			"",
			"",
			"\u0001Ö",
			"",
			"\u0001×",
			"",
			"\u0001Ø",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001Ú",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001Ý",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"",
			"\u0001ß",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001á",
			"",
			"\u0001â",
			"",
			"",
			"\u0001ã",
			"",
			"\u0001ä",
			"",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001æ",
			"\u0001ç",
			"\u0001è",
			"",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			"\u0001ë",
			"",
			"",
			"\n\u000e\a￿\u001a\u000e\u0004￿\u0001\u000e\u0001￿\u001a\u000e",
			""
		};

		// Token: 0x040007C8 RID: 1992
		private static readonly short[] DFA20_eot = DFA.UnpackEncodedString("\u0001￿\r\u000e\u0001￿\u0001B\u0002C\u0001E\u0001G\u0006￿\u0001J\u0001L\u0001N\u0001P\u0001R\u0001￿\u0001T\u0001V\u0001X\u0006￿\u000e\u000e\u0001h\u0001\u000e\u0001j\a\u000e\u0018￿\f\u000e\u0001\u0083\u0002\u000e\u0001￿\u0001\u000e\u0001￿\u0001\u000e\u0001\u0088\u0012\u000e\u0001\u009c\u0002\u000e\u0001\u009f\u0001￿\u0002\u000e\u0001£\u0001\u000e\u0001￿\u0002\u000e\u0001\u0088\u0001\u000e\u0001¨\u0001\u000e\u0001ª\u0002\u000e\u0001\u0088\u0001¨\u0002\u000e\u0001¯\u0003\u000e\u0001³\u0001\u000e\u0001￿\u0002\u000e\u0001￿\u0003\u000e\u0001￿\u0002\u000e\u0001¼\u0001\u000e\u0001￿\u0001\u000e\u0001￿\u0001\u0088\u0003\u000e\u0001￿\u0003\u000e\u0001￿\u0001Å\u0001\u000e\u0001Ç\u0001È\u0001É\u0001\u000e\u0001Ë\u0001\u000e\u0001￿\u0001Í\a\u000e\u0001￿\u0001Õ\u0003￿\u0001\u000e\u0001￿\u0001\u000e\u0001￿\u0001\u000e\u0001Ù\u0001\u000e\u0001Û\u0001Ü\u0001\u000e\u0001Þ\u0001￿\u0001\u000e\u0001à\u0001\u000e\u0001￿\u0001\u000e\u0002￿\u0001\u000e\u0001￿\u0001\u000e\u0001￿\u0001å\u0003\u000e\u0001￿\u0001é\u0001ê\u0001\u000e\u0002￿\u0001ì\u0001￿");

		// Token: 0x040007C9 RID: 1993
		private static readonly short[] DFA20_eof = DFA.UnpackEncodedString("í￿");

		// Token: 0x040007CA RID: 1994
		private static readonly char[] DFA20_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\t\u0001c\u0001a\u0001l\u0001a\u0001l\u0001e\u0001s\u0001f\u0001r\u0001h\u0001o\u0001e\u0001r\u0001￿\u00010\u0002.\u0002\0\u0006￿\u0005=\u0001￿\u0003=\u0006￿\u0001r\u0001a\u0001n\u0001o\u0001l\u0001d\u0001e\u0001s\u0002t\u0001n\u0001w\u0001o\u0001t\u00010\u0001t\u00010\u0001p\u0001t\u0001o\u0001i\u0001o\u0001n\u0001u\u0018￿\u0001i\u0001t\u0001i\u0001c\u0001a\u0001s\u0001e\u0001n\u0002e\u0001i\u0001e\u00010\u0001b\u0001u\u0001￿\u0001o\u0001￿\u0001o\u00010\u0001p\u0002l\u0001g\u0001e\u0001p\u0001e\u0001n\u0002t\u0001e\u0001u\u0001v\u0001f\u0001t\u0001r\u0001h\u0001t\u00010\u0001n\u0001v\u00010\u0001￿\u0001a\u0001r\u00010\u0001r\u0001￿\u0002e\u00010\u0001t\u00010\u0001t\u00010\u0001g\u0001i\u00020\u0001n\u0001e\u00010\u0001a\u0001o\u0001i\u00010\u0001f\u0001￿\u0001d\u0001e\u0001￿\u0001l\u0001n\u0001e\u0001￿\u0001t\u0001r\u00010\u0001h\u0001￿\u0001n\u0001￿\u00010\u0001o\u0001c\u0001n\u0001￿\u0001t\u0001p\u0001l\u0001￿\u00010\u0001s\u00030\u0001a\u00010\u0001t\u0001￿\u00010\u0001a\u0001n\u0002t\u0003e\u0001￿\u00010\u0003￿\u0001d\u0001￿\u0001y\u0001￿\u0001m\u00010\u0001i\u00020\u0001r\u00010\u0001￿\u0001o\u00010\u0001e\u0001￿\u0001o\u0002￿\u0001t\u0001￿\u0001n\u0001￿\u00010\u0001n\u0001y\u0001l\u0001￿\u00020\u0001y\u0002￿\u00010\u0001￿");

		// Token: 0x040007CB RID: 1995
		private static readonly char[] DFA20_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001}\u0001t\u0001u\u0001x\u0001o\u0001l\u0001e\u0001u\u0001n\u0001r\u0001h\u0001o\u0001e\u0001r\u0001￿\u0001=\u00029\u0002￿\u0006￿\u0005=\u0001￿\u0003=\u0006￿\u0002r\u0001n\u0001o\u0001l\u0001d\u0001e\u0001s\u0002t\u0001n\u0001w\u0001o\u0001t\u0001z\u0001t\u0001z\u0001p\u0001t\u0001o\u0001i\u0001o\u0001n\u0001u\u0018￿\u0001i\u0001t\u0001i\u0001c\u0001a\u0001s\u0001w\u0001n\u0002e\u0001i\u0001e\u0001z\u0001b\u0001u\u0001￿\u0001o\u0001￿\u0001o\u0001z\u0001p\u0002l\u0001g\u0001e\u0001p\u0001e\u0001n\u0002t\u0001e\u0001u\u0001v\u0001f\u0001t\u0001r\u0001h\u0001t\u0001z\u0001n\u0001v\u0001z\u0001￿\u0001a\u0001r\u0001z\u0001r\u0001￿\u0002e\u0001z\u0001t\u0001z\u0001t\u0001z\u0001g\u0001i\u0002z\u0001n\u0001e\u0001z\u0001a\u0001o\u0001i\u0001z\u0001f\u0001￿\u0001d\u0001e\u0001￿\u0001l\u0001n\u0001e\u0001￿\u0001t\u0001r\u0001z\u0001h\u0001￿\u0001n\u0001￿\u0001z\u0001o\u0001c\u0001n\u0001￿\u0001t\u0001p\u0001l\u0001￿\u0001z\u0001s\u0003z\u0001a\u0001z\u0001t\u0001￿\u0001z\u0001a\u0001n\u0002t\u0003e\u0001￿\u0001z\u0003￿\u0001d\u0001￿\u0001y\u0001￿\u0001m\u0001z\u0001i\u0002z\u0001r\u0001z\u0001￿\u0001o\u0001z\u0001e\u0001￿\u0001o\u0002￿\u0001t\u0001￿\u0001n\u0001￿\u0001z\u0001n\u0001y\u0001l\u0001￿\u0002z\u0001y\u0002￿\u0001z\u0001￿");

		// Token: 0x040007CC RID: 1996
		private static readonly short[] DFA20_accept = DFA.UnpackEncodedString("\u000e￿\u0001\u001d\u0005￿\u0001\"\u0001#\u0001%\u0001&\u0001'\u0001(\u0005￿\u0001/\u0003￿\u00018\u00019\u0001?\u0001@\u0001A\u0001B\u0018￿\u0001;\u0001+\u0001\u001e\u0001\u001f\u00010\u0001 \u0001$\u0001!\u00012\u0001)\u0001:\u0001*\u0001<\u0001,\u0001=\u0001-\u0001>\u0001.\u00013\u00011\u00016\u00014\u00017\u00015\u000f￿\u0001\t\u0001￿\u0001\n\u0018￿\u0001\u001a\u0004￿\u0001\u0018\u0013￿\u0001\f\u0002￿\u0001\u0019\u0003￿\u0001\u0010\u0004￿\u0001\u001c\u0001￿\u0001\u0012\u0004￿\u0001\r\u0003￿\u0001\u0004\b￿\u0001\u0016\b￿\u0001\v\u0001￿\u0001\u0006\u0001\a\u0001\b\u0001￿\u0001\u000f\u0001￿\u0001\u001b\a￿\u0001\u000e\u0003￿\u0001\u0002\u0001￿\u0001\u0005\u0001\u0013\u0001￿\u0001\u0017\u0001￿\u0001\u0014\u0004￿\u0001\u0001\u0003￿\u0001\u0003\u0001\u0015\u0001￿\u0001\u0011");

		// Token: 0x040007CD RID: 1997
		private static readonly short[] DFA20_special = DFA.UnpackEncodedString("\u0012￿\u0001\u0001\u0001\0Ù￿}>");

		// Token: 0x040007CE RID: 1998
		private static readonly short[][] DFA20_transition = DFA.UnpackEncodedStringArray(DFA20_transitionS);

		// Token: 0x02000176 RID: 374
		protected class DFA19 : DFA
		{
			// Token: 0x06000C17 RID: 3095 RVA: 0x00049B38 File Offset: 0x00047D38
			public DFA19(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 19;
				eot = DFA19_eot;
				eof = DFA19_eof;
				min = DFA19_min;
				max = DFA19_max;
				accept = DFA19_accept;
				special = DFA19_special;
				transition = DFA19_transition;
			}

			// Token: 0x1700016E RID: 366
			// (get) Token: 0x06000C18 RID: 3096 RVA: 0x00049BA8 File Offset: 0x00047DA8
			public override string Description
			{
				get
				{
					return "770:1: COMMENT : ( ( ';/' )=> ';/' ( . )* '/;' | ';' (~ '\\n' )* );";
				}
			}
		}

		// Token: 0x02000177 RID: 375
		protected class DFA20 : DFA
		{
			// Token: 0x06000C19 RID: 3097 RVA: 0x00049BB0 File Offset: 0x00047DB0
			public DFA20(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 20;
				eot = DFA20_eot;
				eof = DFA20_eof;
				min = DFA20_min;
				max = DFA20_max;
				accept = DFA20_accept;
				special = DFA20_special;
				transition = DFA20_transition;
			}

			// Token: 0x1700016F RID: 367
			// (get) Token: 0x06000C1A RID: 3098 RVA: 0x00049C20 File Offset: 0x00047E20
			public override string Description => "1:1: Tokens : ( SCRIPTNAME | FUNCTION | ENDFUNCTION | EVENT | ENDEVENT | NATIVE | GLOBAL | RETURN | AS | IF | ELSEIF | ELSE | ENDIF | EXTENDS | IMPORT | AUTO | AUTOREADONLY | STATE | ENDSTATE | PROPERTY | ENDPROPERTY | WHILE | ENDWHILE | BASETYPE | NONE | NEW | LENGTH | BOOL | ID | INTEGER | FLOAT | STRING | DOCSTRING | LPAREN | RPAREN | LBRACE | RBRACE | LBRACKET | RBRACKET | COMMA | EQUALS | PLUS | MINUS | MULT | DIVIDE | MOD | DOT | DQUOTE | NOT | EQ | NE | GT | LT | GTE | LTE | OR | AND | PLUSEQUALS | MINUSEQUALS | MULTEQUALS | DIVEQUALS | MODEQUALS | EOL | WS | EAT_EOL | COMMENT );";
        }
	}
}
