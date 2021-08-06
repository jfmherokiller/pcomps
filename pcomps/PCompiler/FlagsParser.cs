using System.Collections.Generic;
using pcomps.Antlr.Runtime;
using pcomps.Antlr.Runtime.Collections;

namespace pcomps.PCompiler
{
	// Token: 0x02000208 RID: 520
	public class FlagsParser : Parser
	{
		// Token: 0x06000EB0 RID: 3760 RVA: 0x0006C200 File Offset: 0x0006A400
		private void AddFlag(IToken akNameToken, int aiIndex, bool abAllowedOnObj, bool abAllowedOnProp, bool abAllowedOnVar, bool abAllowedOnFunc)
		{
			string text = akNameToken.Text.ToLowerInvariant();
			if (this.kFlagDict.ContainsKey(text))
			{
				this.OnError($"Flag {akNameToken.Text} has already been defined", akNameToken.Line, akNameToken.CharPositionInLine);
				return;
			}
			if (this.kFlagIndexDict.ContainsKey(aiIndex))
			{
				this.OnError($"Flag index {aiIndex} has already been defined", akNameToken.Line, akNameToken.CharPositionInLine);
				return;
			}
			if (!PapyrusFlag.IsValidFlagIndex(aiIndex))
			{
				this.OnError($"Flag index {aiIndex} is out of range.", akNameToken.Line, akNameToken.CharPositionInLine);
				return;
			}
			this.kFlagDict.Add(text, new PapyrusFlag(aiIndex, abAllowedOnObj, abAllowedOnProp, abAllowedOnVar, abAllowedOnFunc));
			this.kFlagIndexDict.Add(aiIndex, text);
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x0006C2CC File Offset: 0x0006A4CC
		private void DuplicateFlagItem(IToken akToken)
		{
			this.OnError($"Duplicate flag item: {akToken.Text}", akToken.Line, akToken.CharPositionInLine);
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000EB2 RID: 3762 RVA: 0x0006C2F0 File Offset: 0x0006A4F0
		internal Dictionary<string, PapyrusFlag> DefinedFlags
		{
			get
			{
				return this.kFlagDict;
			}
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0006C2F8 File Offset: 0x0006A4F8
		public FlagsParser(ITokenStream input) : this(input, new RecognizerSharedState())
		{
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x0006C308 File Offset: 0x0006A508
		public FlagsParser(ITokenStream input, RecognizerSharedState state) : base(input, state)
		{
			this.InitializeCyclicDFAs();
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x0006C33C File Offset: 0x0006A53C
		public override string[] TokenNames
		{
			get
			{
				return FlagsParser.tokenNames;
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000EB6 RID: 3766 RVA: 0x0006C344 File Offset: 0x0006A544
		public override string GrammarFileName
		{
			get
			{
				return "Flags.g";
			}
		}

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06000EB7 RID: 3767 RVA: 0x0006C34C File Offset: 0x0006A54C
		// (remove) Token: 0x06000EB8 RID: 3768 RVA: 0x0006C368 File Offset: 0x0006A568
		internal event InternalErrorEventHandler ErrorHandler;

		// Token: 0x06000EB9 RID: 3769 RVA: 0x0006C384 File Offset: 0x0006A584
		private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
		{
			if (this.ErrorHandler != null)
			{
				this.ErrorHandler(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
			}
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x0006C3A4 File Offset: 0x0006A5A4
		public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			string errorMessage = this.GetErrorMessage(e, tokenNames);
			this.OnError(errorMessage, e.Line, e.CharPositionInLine);
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x0006C3D0 File Offset: 0x0006A5D0
		public void flags()
		{
			try
			{
				for (;;)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 == 4)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					base.PushFollow(FlagsParser.FOLLOW_flagDefinition_in_flags81);
					this.flagDefinition();
					this.state.followingStackPointer--;
				}
				this.Match(this.input, -1, FlagsParser.FOLLOW_EOF_in_flags84);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x0006C45C File Offset: 0x0006A65C
		public void flagDefinition()
		{
			this.flagDefinition_stack.Push(new FlagsParser.flagDefinition_scope());
			((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnObj = false;
			((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnProp = false;
			((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnVar = false;
			((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnFunc = false;
			try
			{
				int num = this.input.LA(1);
				if (num != 4)
				{
					NoViableAltException ex = new NoViableAltException("", 2, 0, this.input);
					throw ex;
				}
				int num2 = this.input.LA(2);
				if (num2 != 5)
				{
					NoViableAltException ex2 = new NoViableAltException("", 2, 1, this.input);
					throw ex2;
				}
				int num3 = this.input.LA(3);
				if (num3 != 6)
				{
					NoViableAltException ex3 = new NoViableAltException("", 2, 2, this.input);
					throw ex3;
				}
				int num4 = this.input.LA(4);
				int num5;
				if (num4 == -1 || num4 == 4)
				{
					num5 = 1;
				}
				else
				{
					if (num4 != 7)
					{
						NoViableAltException ex4 = new NoViableAltException("", 2, 3, this.input);
						throw ex4;
					}
					num5 = 2;
				}
				switch (num5)
				{
				case 1:
				{
					this.Match(this.input, 4, FlagsParser.FOLLOW_FLAG_in_flagDefinition105);
					IToken akNameToken = (IToken)this.Match(this.input, 5, FlagsParser.FOLLOW_ID_in_flagDefinition109);
					IToken token = (IToken)this.Match(this.input, 6, FlagsParser.FOLLOW_NUMBER_in_flagDefinition113);
					this.AddFlag(akNameToken, int.Parse(token.Text), true, true, true, true);
					break;
				}
				case 2:
				{
					this.Match(this.input, 4, FlagsParser.FOLLOW_FLAG_in_flagDefinition124);
					IToken akNameToken = (IToken)this.Match(this.input, 5, FlagsParser.FOLLOW_ID_in_flagDefinition128);
					IToken token = (IToken)this.Match(this.input, 6, FlagsParser.FOLLOW_NUMBER_in_flagDefinition132);
					base.PushFollow(FlagsParser.FOLLOW_allowedBlock_in_flagDefinition134);
					this.allowedBlock();
					this.state.followingStackPointer--;
					this.AddFlag(akNameToken, int.Parse(token.Text), ((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnObj, ((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnProp, ((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnVar, ((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnFunc);
					break;
				}
				}
			}
			catch (RecognitionException ex5)
			{
				this.ReportError(ex5);
				this.Recover(this.input, ex5);
			}
			finally
			{
				this.flagDefinition_stack.Pop();
			}
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x0006C738 File Offset: 0x0006A938
		public void allowedBlock()
		{
			try
			{
				this.Match(this.input, 7, FlagsParser.FOLLOW_OPEN_BRACE_in_allowedBlock153);
				for (;;)
				{
					int num = 5;
					switch (this.input.LA(1))
					{
					case 8:
						num = 1;
						break;
					case 9:
						num = 2;
						break;
					case 10:
						num = 3;
						break;
					case 11:
						num = 4;
						break;
					}
					switch (num)
					{
					case 1:
					{
						IToken akToken = (IToken)this.Match(this.input, 8, FlagsParser.FOLLOW_SCRIPT_in_allowedBlock161);
						if (((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnObj)
						{
							this.DuplicateFlagItem(akToken);
						}
						((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnObj = true;
						continue;
					}
					case 2:
					{
						IToken akToken2 = (IToken)this.Match(this.input, 9, FlagsParser.FOLLOW_PROPERTY_in_allowedBlock176);
						if (((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnProp)
						{
							this.DuplicateFlagItem(akToken2);
						}
						((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnProp = true;
						continue;
					}
					case 3:
					{
						IToken akToken3 = (IToken)this.Match(this.input, 10, FlagsParser.FOLLOW_VARIABLE_in_allowedBlock191);
						if (((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnVar)
						{
							this.DuplicateFlagItem(akToken3);
						}
						((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnVar = true;
						continue;
					}
					case 4:
					{
						IToken akToken4 = (IToken)this.Match(this.input, 11, FlagsParser.FOLLOW_FUNCTION_in_allowedBlock206);
						if (((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnFunc)
						{
							this.DuplicateFlagItem(akToken4);
						}
						((FlagsParser.flagDefinition_scope)this.flagDefinition_stack.Peek()).bAllowedOnFunc = true;
						continue;
					}
					}
					break;
				}
				this.Match(this.input, 12, FlagsParser.FOLLOW_CLOSE_BRACE_in_allowedBlock221);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x0006C954 File Offset: 0x0006AB54
		private void InitializeCyclicDFAs()
		{
		}

		// Token: 0x04000C5B RID: 3163
		public const int FUNCTION = 11;

		// Token: 0x04000C5C RID: 3164
		public const int SCRIPT = 8;

		// Token: 0x04000C5D RID: 3165
		public const int WS = 16;

		// Token: 0x04000C5E RID: 3166
		public const int VARIABLE = 10;

		// Token: 0x04000C5F RID: 3167
		public const int PROPERTY = 9;

		// Token: 0x04000C60 RID: 3168
		public const int WS_CHAR = 15;

		// Token: 0x04000C61 RID: 3169
		public const int NUMBER = 6;

		// Token: 0x04000C62 RID: 3170
		public const int DIGIT = 14;

		// Token: 0x04000C63 RID: 3171
		public const int CLOSE_BRACE = 12;

		// Token: 0x04000C64 RID: 3172
		public const int COMMENT = 17;

		// Token: 0x04000C65 RID: 3173
		public const int ID = 5;

		// Token: 0x04000C66 RID: 3174
		public const int EOF = -1;

		// Token: 0x04000C67 RID: 3175
		public const int OPEN_BRACE = 7;

		// Token: 0x04000C68 RID: 3176
		public const int ALPHA = 13;

		// Token: 0x04000C69 RID: 3177
		public const int FLAG = 4;

		// Token: 0x04000C6A RID: 3178
		private Dictionary<string, PapyrusFlag> kFlagDict = new Dictionary<string, PapyrusFlag>();

		// Token: 0x04000C6B RID: 3179
		private Dictionary<int, string> kFlagIndexDict = new Dictionary<int, string>();

		// Token: 0x04000C6C RID: 3180
		public static readonly string[] tokenNames = new string[]
		{
			"<invalid>",
			"<EOR>",
			"<DOWN>",
			"<UP>",
			"FLAG",
			"ID",
			"NUMBER",
			"OPEN_BRACE",
			"SCRIPT",
			"PROPERTY",
			"VARIABLE",
			"FUNCTION",
			"CLOSE_BRACE",
			"ALPHA",
			"DIGIT",
			"WS_CHAR",
			"WS",
			"COMMENT"
		};

		// Token: 0x04000C6E RID: 3182
		protected StackList flagDefinition_stack = new StackList();

		// Token: 0x04000C6F RID: 3183
		public static readonly BitSet FOLLOW_flagDefinition_in_flags81 = new BitSet(new ulong[]
		{
			16UL
		});

		// Token: 0x04000C70 RID: 3184
		public static readonly BitSet FOLLOW_EOF_in_flags84 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000C71 RID: 3185
		public static readonly BitSet FOLLOW_FLAG_in_flagDefinition105 = new BitSet(new ulong[]
		{
			32UL
		});

		// Token: 0x04000C72 RID: 3186
		public static readonly BitSet FOLLOW_ID_in_flagDefinition109 = new BitSet(new ulong[]
		{
			64UL
		});

		// Token: 0x04000C73 RID: 3187
		public static readonly BitSet FOLLOW_NUMBER_in_flagDefinition113 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000C74 RID: 3188
		public static readonly BitSet FOLLOW_FLAG_in_flagDefinition124 = new BitSet(new ulong[]
		{
			32UL
		});

		// Token: 0x04000C75 RID: 3189
		public static readonly BitSet FOLLOW_ID_in_flagDefinition128 = new BitSet(new ulong[]
		{
			64UL
		});

		// Token: 0x04000C76 RID: 3190
		public static readonly BitSet FOLLOW_NUMBER_in_flagDefinition132 = new BitSet(new ulong[]
		{
			128UL
		});

		// Token: 0x04000C77 RID: 3191
		public static readonly BitSet FOLLOW_allowedBlock_in_flagDefinition134 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000C78 RID: 3192
		public static readonly BitSet FOLLOW_OPEN_BRACE_in_allowedBlock153 = new BitSet(new ulong[]
		{
			7936UL
		});

		// Token: 0x04000C79 RID: 3193
		public static readonly BitSet FOLLOW_SCRIPT_in_allowedBlock161 = new BitSet(new ulong[]
		{
			7936UL
		});

		// Token: 0x04000C7A RID: 3194
		public static readonly BitSet FOLLOW_PROPERTY_in_allowedBlock176 = new BitSet(new ulong[]
		{
			7936UL
		});

		// Token: 0x04000C7B RID: 3195
		public static readonly BitSet FOLLOW_VARIABLE_in_allowedBlock191 = new BitSet(new ulong[]
		{
			7936UL
		});

		// Token: 0x04000C7C RID: 3196
		public static readonly BitSet FOLLOW_FUNCTION_in_allowedBlock206 = new BitSet(new ulong[]
		{
			7936UL
		});

		// Token: 0x04000C7D RID: 3197
		public static readonly BitSet FOLLOW_CLOSE_BRACE_in_allowedBlock221 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x02000209 RID: 521
		protected class flagDefinition_scope
		{
			// Token: 0x04000C7E RID: 3198
			protected internal bool bAllowedOnObj;

			// Token: 0x04000C7F RID: 3199
			protected internal bool bAllowedOnProp;

			// Token: 0x04000C80 RID: 3200
			protected internal bool bAllowedOnVar;

			// Token: 0x04000C81 RID: 3201
			protected internal bool bAllowedOnFunc;
		}
	}
}
