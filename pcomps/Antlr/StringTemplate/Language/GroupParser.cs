﻿using System;
using System.Collections;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000243 RID: 579
	public class GroupParser : LLkParser
	{
		// Token: 0x06001179 RID: 4473 RVA: 0x00080264 File Offset: 0x0007E464
		public override void reportError(RecognitionException e)
		{
			if (this._group != null)
			{
				this._group.Error("template group parse error", e);
				return;
			}
			Console.Error.WriteLine("template group parse error: " + e);
			Console.Error.WriteLine(e.StackTrace);
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x000802B0 File Offset: 0x0007E4B0
		protected void initialize()
		{
			this.tokenNames = GroupParser.tokenNames_;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x000802C0 File Offset: 0x0007E4C0
		protected GroupParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			this.initialize();
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x000802D0 File Offset: 0x0007E4D0
		public GroupParser(TokenBuffer tokenBuf) : this(tokenBuf, 3)
		{
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x000802DC File Offset: 0x0007E4DC
		protected GroupParser(TokenStream lexer, int k) : base(lexer, k)
		{
			this.initialize();
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x000802EC File Offset: 0x0007E4EC
		public GroupParser(TokenStream lexer) : this(lexer, 3)
		{
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x000802F8 File Offset: 0x0007E4F8
		public GroupParser(ParserSharedInputState state) : base(state, 3)
		{
			this.initialize();
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x00080308 File Offset: 0x0007E508
		public void group(StringTemplateGroup g)
		{
			this._group = g;
			try
			{
				this.match(4);
				IToken token = this.LT(1);
				this.match(5);
				g.Name = token.getText();
				switch (this.LA(1))
				{
				case 6:
				{
					this.match(6);
					IToken token2 = this.LT(1);
					this.match(5);
					g.SetSuperGroup(token2.getText());
					goto IL_8C;
				}
				case 7:
				case 9:
					goto IL_8C;
				}
				throw new NoViableAltException(this.LT(1), this.getFilename());
				IL_8C:
				switch (this.LA(1))
				{
				case 7:
				{
					this.match(7);
					IToken token3 = this.LT(1);
					this.match(5);
					g.ImplementInterface(token3.getText());
					while (this.LA(1) == 8)
					{
						this.match(8);
						IToken token4 = this.LT(1);
						this.match(5);
						g.ImplementInterface(token4.getText());
					}
					goto IL_10F;
				}
				case 9:
					goto IL_10F;
				}
				throw new NoViableAltException(this.LT(1), this.getFilename());
				IL_10F:
				this.match(9);
				int num = 0;
				for (;;)
				{
					if ((this.LA(1) == 5 || this.LA(1) == 10) && (this.LA(2) == 5 || this.LA(2) == 12 || this.LA(2) == 14) && (this.LA(3) == 5 || this.LA(3) == 11 || this.LA(3) == 13))
					{
						this.template(g);
					}
					else
					{
						if (this.LA(1) != 5 || this.LA(2) != 14 || this.LA(3) != 19)
						{
							break;
						}
						this.mapdef(g);
					}
					num++;
				}
				if (num < 1)
				{
					throw new NoViableAltException(this.LT(1), this.getFilename());
				}
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_0_);
			}
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x00080510 File Offset: 0x0007E710
		public void template(StringTemplateGroup g)
		{
			int line = this.LT(1).getLine();
			try
			{
				if ((this.LA(1) == 5 || this.LA(1) == 10) && (this.LA(2) == 5 || this.LA(2) == 12))
				{
					int num = this.LA(1);
					StringTemplate stringTemplate;
					if (num != 5)
					{
						if (num != 10)
						{
							throw new NoViableAltException(this.LT(1), this.getFilename());
						}
						this.match(10);
						IToken token = this.LT(1);
						this.match(5);
						this.match(11);
						IToken token2 = this.LT(1);
						this.match(5);
						string text = g.GetMangledRegionName(token.getText(), token2.getText());
						if (g.IsDefinedInThisGroup(text))
						{
							g.Error(string.Concat(new object[]
							{
								"group ",
								g.Name,
								" line ",
								line,
								": redefinition of template region: @",
								token.getText(),
								".",
								token2.getText()
							}));
							stringTemplate = new StringTemplate();
						}
						else
						{
							bool flag = false;
							StringTemplate stringTemplate2 = g.LookupTemplate(token.getText());
							if (stringTemplate2 == null)
							{
								g.Error(string.Concat(new object[]
								{
									"group ",
									g.Name,
									" line ",
									line,
									": reference to region within undefined template: ",
									token.getText()
								}));
								flag = true;
							}
							if (!stringTemplate2.ContainsRegionName(token2.getText()))
							{
								g.Error(string.Concat(new object[]
								{
									"group ",
									g.Name,
									" line ",
									line,
									": template ",
									token.getText(),
									" has no region called ",
									token2.getText()
								}));
								flag = true;
							}
							if (flag)
							{
								stringTemplate = new StringTemplate();
							}
							else
							{
								stringTemplate = g.DefineRegionTemplate(token.getText(), token2.getText(), null, 3);
							}
						}
					}
					else
					{
						IToken token3 = this.LT(1);
						this.match(5);
						string text = token3.getText();
						if (g.IsDefinedInThisGroup(text))
						{
							g.Error("redefinition of template: " + text);
							stringTemplate = new StringTemplate();
						}
						else
						{
							stringTemplate = g.DefineTemplate(text, null);
						}
					}
					if (stringTemplate != null)
					{
						stringTemplate.GroupFileLine = line;
					}
					this.match(12);
					int num2 = this.LA(1);
					if (num2 != 5)
					{
						if (num2 != 13)
						{
							throw new NoViableAltException(this.LT(1), this.getFilename());
						}
						stringTemplate.DefineEmptyFormalArgumentList();
					}
					else
					{
						this.args(stringTemplate);
					}
					this.match(13);
					this.match(14);
					switch (this.LA(1))
					{
					case 15:
					{
						IToken token4 = this.LT(1);
						this.match(15);
						stringTemplate.Template = token4.getText();
						break;
					}
					case 16:
					{
						IToken token5 = this.LT(1);
						this.match(16);
						stringTemplate.Template = token5.getText();
						break;
					}
					default:
						throw new NoViableAltException(this.LT(1), this.getFilename());
					}
				}
				else
				{
					if (this.LA(1) != 5 || this.LA(2) != 14)
					{
						throw new NoViableAltException(this.LT(1), this.getFilename());
					}
					IToken token6 = this.LT(1);
					this.match(5);
					this.match(14);
					IToken token7 = this.LT(1);
					this.match(5);
					g.DefineTemplateAlias(token6.getText(), token7.getText());
				}
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_1_);
			}
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00080914 File Offset: 0x0007EB14
		public void mapdef(StringTemplateGroup g)
		{
			try
			{
				IToken token = this.LT(1);
				this.match(5);
				this.match(14);
				IDictionary mapping = this.map();
				if (g.GetMap(token.getText()) != null)
				{
					g.Error("redefinition of map: " + token.getText());
				}
				else if (g.IsDefinedInThisGroup(token.getText()))
				{
					g.Error("redefinition of template as map: " + token.getText());
				}
				else
				{
					g.DefineMap(token.getText(), mapping);
				}
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_1_);
			}
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x000809C4 File Offset: 0x0007EBC4
		public void args(StringTemplate st)
		{
			try
			{
				this.arg(st);
				while (this.LA(1) == 8)
				{
					this.match(8);
					this.arg(st);
				}
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_2_);
			}
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00080A1C File Offset: 0x0007EC1C
		public void arg(StringTemplate st)
		{
			StringTemplate stringTemplate = null;
			try
			{
				IToken token = this.LT(1);
				this.match(5);
				if (this.LA(1) == 17 && this.LA(2) == 15)
				{
					this.match(17);
					IToken token2 = this.LT(1);
					this.match(15);
					stringTemplate = new StringTemplate("$_val_$");
					stringTemplate.SetAttribute("_val_", token2.getText());
					stringTemplate.DefineFormalArgument("_val_");
					stringTemplate.Name = string.Concat(new string[]
					{
						"<",
						st.Name,
						"'s arg ",
						token.getText(),
						" default value subtemplate>"
					});
				}
				else if (this.LA(1) == 17 && this.LA(2) == 18)
				{
					this.match(17);
					IToken token3 = this.LT(1);
					this.match(18);
					stringTemplate = new StringTemplate(st.Group, token3.getText());
					stringTemplate.Name = string.Concat(new string[]
					{
						"<",
						st.Name,
						"'s arg ",
						token.getText(),
						" default value subtemplate>"
					});
				}
				else if (this.LA(1) != 8 && this.LA(1) != 13)
				{
					throw new NoViableAltException(this.LT(1), this.getFilename());
				}
				st.DefineFormalArgument(token.getText(), stringTemplate);
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_3_);
			}
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x00080BD8 File Offset: 0x0007EDD8
		public IDictionary map()
		{
			IDictionary dictionary = new Hashtable();
			try
			{
				this.match(19);
				this.mapPairs(dictionary);
				this.match(20);
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_1_);
			}
			return dictionary;
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x00080C2C File Offset: 0x0007EE2C
		public void mapPairs(IDictionary mapping)
		{
			try
			{
				int num = this.LA(1);
				if (num != 15)
				{
					if (num != 21)
					{
						throw new NoViableAltException(this.LT(1), this.getFilename());
					}
					this.defaultValuePair(mapping);
				}
				else
				{
					this.keyValuePair(mapping);
					while (this.LA(1) == 8 && this.LA(2) == 15)
					{
						this.match(8);
						this.keyValuePair(mapping);
					}
					int num2 = this.LA(1);
					if (num2 != 8)
					{
						if (num2 != 20)
						{
							throw new NoViableAltException(this.LT(1), this.getFilename());
						}
					}
					else
					{
						this.match(8);
						this.defaultValuePair(mapping);
					}
				}
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_4_);
			}
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x00080CF4 File Offset: 0x0007EEF4
		public void keyValuePair(IDictionary mapping)
		{
			try
			{
				IToken token = this.LT(1);
				this.match(15);
				this.match(6);
				StringTemplate value = this.keyValue();
				mapping[token.getText()] = value;
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_5_);
			}
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x00080D58 File Offset: 0x0007EF58
		public void defaultValuePair(IDictionary mapping)
		{
			try
			{
				this.match(21);
				this.match(6);
				StringTemplate value = this.keyValue();
				mapping["_default_"] = value;
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_4_);
			}
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x00080DB4 File Offset: 0x0007EFB4
		public StringTemplate keyValue()
		{
			StringTemplate result = null;
			try
			{
				int num = this.LA(1);
				if (num <= 8)
				{
					if (num != 5)
					{
						if (num != 8)
						{
							goto IL_C8;
						}
					}
					else
					{
						IToken token = this.LT(1);
						this.match(5);
						if (!token.getText().Equals("key"))
						{
							throw new SemanticException("k.getText().Equals(\"key\")");
						}
						result = ASTExpr.MAP_KEY_VALUE;
						goto IL_DB;
					}
				}
				else
				{
					switch (num)
					{
					case 15:
					{
						IToken token2 = this.LT(1);
						this.match(15);
						result = new StringTemplate(this._group, token2.getText());
						goto IL_DB;
					}
					case 16:
					{
						IToken token3 = this.LT(1);
						this.match(16);
						result = new StringTemplate(this._group, token3.getText());
						goto IL_DB;
					}
					default:
						if (num != 20)
						{
							goto IL_C8;
						}
						break;
					}
				}
				result = null;
				goto IL_DB;
				IL_C8:
				throw new NoViableAltException(this.LT(1), this.getFilename());
				IL_DB:;
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, GroupParser.tokenSet_5_);
			}
			return result;
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x00080EC8 File Offset: 0x0007F0C8
		private void initializeFactory()
		{
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x00080ECC File Offset: 0x0007F0CC
		private static long[] mk_tokenSet_0_()
		{
			long[] array = new long[2];
			array[0] = 2L;
			return array;
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x00080EE8 File Offset: 0x0007F0E8
		private static long[] mk_tokenSet_1_()
		{
			long[] array = new long[2];
			array[0] = 1058L;
			return array;
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x00080F08 File Offset: 0x0007F108
		private static long[] mk_tokenSet_2_()
		{
			long[] array = new long[2];
			array[0] = 8192L;
			return array;
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00080F28 File Offset: 0x0007F128
		private static long[] mk_tokenSet_3_()
		{
			long[] array = new long[2];
			array[0] = 8448L;
			return array;
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x00080F48 File Offset: 0x0007F148
		private static long[] mk_tokenSet_4_()
		{
			long[] array = new long[2];
			array[0] = 1048576L;
			return array;
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00080F68 File Offset: 0x0007F168
		private static long[] mk_tokenSet_5_()
		{
			long[] array = new long[2];
			array[0] = 1048832L;
			return array;
		}

		// Token: 0x04000E90 RID: 3728
		public const int EOF = 1;

		// Token: 0x04000E91 RID: 3729
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000E92 RID: 3730
		public const int LITERAL_group = 4;

		// Token: 0x04000E93 RID: 3731
		public const int ID = 5;

		// Token: 0x04000E94 RID: 3732
		public const int COLON = 6;

		// Token: 0x04000E95 RID: 3733
		public const int LITERAL_implements = 7;

		// Token: 0x04000E96 RID: 3734
		public const int COMMA = 8;

		// Token: 0x04000E97 RID: 3735
		public const int SEMI = 9;

		// Token: 0x04000E98 RID: 3736
		public const int AT = 10;

		// Token: 0x04000E99 RID: 3737
		public const int DOT = 11;

		// Token: 0x04000E9A RID: 3738
		public const int LPAREN = 12;

		// Token: 0x04000E9B RID: 3739
		public const int RPAREN = 13;

		// Token: 0x04000E9C RID: 3740
		public const int DEFINED_TO_BE = 14;

		// Token: 0x04000E9D RID: 3741
		public const int STRING = 15;

		// Token: 0x04000E9E RID: 3742
		public const int BIGSTRING = 16;

		// Token: 0x04000E9F RID: 3743
		public const int ASSIGN = 17;

		// Token: 0x04000EA0 RID: 3744
		public const int ANONYMOUS_TEMPLATE = 18;

		// Token: 0x04000EA1 RID: 3745
		public const int LBRACK = 19;

		// Token: 0x04000EA2 RID: 3746
		public const int RBRACK = 20;

		// Token: 0x04000EA3 RID: 3747
		public const int LITERAL_default = 21;

		// Token: 0x04000EA4 RID: 3748
		public const int STAR = 22;

		// Token: 0x04000EA5 RID: 3749
		public const int PLUS = 23;

		// Token: 0x04000EA6 RID: 3750
		public const int OPTIONAL = 24;

		// Token: 0x04000EA7 RID: 3751
		public const int SL_COMMENT = 25;

		// Token: 0x04000EA8 RID: 3752
		public const int ML_COMMENT = 26;

		// Token: 0x04000EA9 RID: 3753
		public const int WS = 27;

		// Token: 0x04000EAA RID: 3754
		protected StringTemplateGroup _group;

		// Token: 0x04000EAB RID: 3755
		public static readonly string[] tokenNames_ = new string[]
		{
			"\"<0>\"",
			"\"EOF\"",
			"\"<2>\"",
			"\"NULL_TREE_LOOKAHEAD\"",
			"\"group\"",
			"\"ID\"",
			"\"COLON\"",
			"\"implements\"",
			"\"COMMA\"",
			"\"SEMI\"",
			"\"AT\"",
			"\"DOT\"",
			"\"LPAREN\"",
			"\"RPAREN\"",
			"\"DEFINED_TO_BE\"",
			"\"STRING\"",
			"\"BIGSTRING\"",
			"\"ASSIGN\"",
			"\"ANONYMOUS_TEMPLATE\"",
			"\"LBRACK\"",
			"\"RBRACK\"",
			"\"default\"",
			"\"STAR\"",
			"\"PLUS\"",
			"\"OPTIONAL\"",
			"\"SL_COMMENT\"",
			"\"ML_COMMENT\"",
			"\"WS\""
		};

		// Token: 0x04000EAC RID: 3756
		public static readonly BitSet tokenSet_0_ = new BitSet(GroupParser.mk_tokenSet_0_());

		// Token: 0x04000EAD RID: 3757
		public static readonly BitSet tokenSet_1_ = new BitSet(GroupParser.mk_tokenSet_1_());

		// Token: 0x04000EAE RID: 3758
		public static readonly BitSet tokenSet_2_ = new BitSet(GroupParser.mk_tokenSet_2_());

		// Token: 0x04000EAF RID: 3759
		public static readonly BitSet tokenSet_3_ = new BitSet(GroupParser.mk_tokenSet_3_());

		// Token: 0x04000EB0 RID: 3760
		public static readonly BitSet tokenSet_4_ = new BitSet(GroupParser.mk_tokenSet_4_());

		// Token: 0x04000EB1 RID: 3761
		public static readonly BitSet tokenSet_5_ = new BitSet(GroupParser.mk_tokenSet_5_());
	}
}
