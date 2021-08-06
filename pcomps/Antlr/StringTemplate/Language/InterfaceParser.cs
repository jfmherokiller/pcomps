using System;
using pcomps.Antlr.collections.impl;
using pcomps.Antlr.StringTemplate.Collections;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000246 RID: 582
	public class InterfaceParser : LLkParser
	{
		// Token: 0x060011A4 RID: 4516 RVA: 0x00081E10 File Offset: 0x00080010
		public override void reportError(RecognitionException e)
		{
			if (groupI != null)
			{
				groupI.Error("template group interface parse error", e);
				return;
			}
			Console.Error.WriteLine($"template group interface parse error: {e}");
			Console.Error.WriteLine(e.StackTrace);
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x00081E5C File Offset: 0x0008005C
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x00081E6C File Offset: 0x0008006C
		protected InterfaceParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x00081E7C File Offset: 0x0008007C
		public InterfaceParser(TokenBuffer tokenBuf) : this(tokenBuf, 3)
		{
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x00081E88 File Offset: 0x00080088
		protected InterfaceParser(TokenStream lexer, int k) : base(lexer, k)
		{
			initialize();
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x00081E98 File Offset: 0x00080098
		public InterfaceParser(TokenStream lexer) : this(lexer, 3)
		{
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x00081EA4 File Offset: 0x000800A4
		public InterfaceParser(ParserSharedInputState state) : base(state, 3)
		{
			initialize();
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x00081EB4 File Offset: 0x000800B4
		public void groupInterface(StringTemplateGroupInterface groupI)
		{
			this.groupI = groupI;
			try
			{
				match(4);
				var token = LT(1);
				match(5);
				groupI.Name = token.getText();
				match(6);
				var num = 0;
				while (LA(1) == 5 || LA(1) == 7)
				{
					template(groupI);
					num++;
				}
				if (num < 1)
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
				recover(ex, tokenSet_0_);
			}
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00081F58 File Offset: 0x00080158
		public void template(StringTemplateGroupInterface groupI)
		{
			IToken token = null;
			var formalArgs = new HashList();
			try
			{
				switch (LA(1))
				{
				case 5:
					goto IL_50;
				case 7:
					token = LT(1);
					match(7);
					goto IL_50;
				}
				throw new NoViableAltException(LT(1), getFilename());
				IL_50:
				var token2 = LT(1);
				match(5);
				match(8);
				var num = LA(1);
				if (num != 5)
				{
					if (num != 9)
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
				}
				else
				{
					formalArgs = args();
				}
				match(9);
				match(6);
				var text = token2.getText();
				groupI.DefineTemplate(text, formalArgs, token != null);
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
				recover(ex, tokenSet_1_);
			}
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x00082050 File Offset: 0x00080250
		public HashList args()
		{
			var hashList = new HashList();
			try
			{
				var token = LT(1);
				match(5);
				hashList[token.getText()] = new FormalArgument(token.getText());
				while (LA(1) == 10)
				{
					match(10);
					var token2 = LT(1);
					match(5);
					hashList[token2.getText()] = new FormalArgument(token2.getText());
				}
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
				recover(ex, tokenSet_2_);
			}
			return hashList;
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x000820F4 File Offset: 0x000802F4
		private void initializeFactory()
		{
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x000820F8 File Offset: 0x000802F8
		private static long[] mk_tokenSet_0_()
		{
			var array = new long[2];
			array[0] = 2L;
			return array;
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00082114 File Offset: 0x00080314
		private static long[] mk_tokenSet_1_()
		{
			var array = new long[2];
			array[0] = 162L;
			return array;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00082134 File Offset: 0x00080334
		private static long[] mk_tokenSet_2_()
		{
			var array = new long[2];
			array[0] = 512L;
			return array;
		}

		// Token: 0x04000EDA RID: 3802
		public const int EOF = 1;

		// Token: 0x04000EDB RID: 3803
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000EDC RID: 3804
		public const int LITERAL_interface = 4;

		// Token: 0x04000EDD RID: 3805
		public const int ID = 5;

		// Token: 0x04000EDE RID: 3806
		public const int SEMI = 6;

		// Token: 0x04000EDF RID: 3807
		public const int LITERAL_optional = 7;

		// Token: 0x04000EE0 RID: 3808
		public const int LPAREN = 8;

		// Token: 0x04000EE1 RID: 3809
		public const int RPAREN = 9;

		// Token: 0x04000EE2 RID: 3810
		public const int COMMA = 10;

		// Token: 0x04000EE3 RID: 3811
		public const int COLON = 11;

		// Token: 0x04000EE4 RID: 3812
		public const int SL_COMMENT = 12;

		// Token: 0x04000EE5 RID: 3813
		public const int ML_COMMENT = 13;

		// Token: 0x04000EE6 RID: 3814
		public const int WS = 14;

		// Token: 0x04000EE7 RID: 3815
		protected StringTemplateGroupInterface groupI;

		// Token: 0x04000EE8 RID: 3816
		public static readonly string[] tokenNames_ = new string[]
		{
			"\"<0>\"",
			"\"EOF\"",
			"\"<2>\"",
			"\"NULL_TREE_LOOKAHEAD\"",
			"\"interface\"",
			"\"ID\"",
			"\"SEMI\"",
			"\"optional\"",
			"\"LPAREN\"",
			"\"RPAREN\"",
			"\"COMMA\"",
			"\"COLON\"",
			"\"SL_COMMENT\"",
			"\"ML_COMMENT\"",
			"\"WS\""
		};

		// Token: 0x04000EE9 RID: 3817
		public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());

		// Token: 0x04000EEA RID: 3818
		public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());

		// Token: 0x04000EEB RID: 3819
		public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	}
}
