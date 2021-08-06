using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000251 RID: 593
	public class TemplateParser : LLkParser
	{
		// Token: 0x060011DC RID: 4572 RVA: 0x00082634 File Offset: 0x00080834
		public override void reportError(RecognitionException e)
		{
			StringTemplateGroup group = this.self.Group;
			if (group == StringTemplate.defaultGroup)
			{
				this.self.Error(
                    $"template parse error; template context is {this.self.GetEnclosingInstanceStackString()}", e);
				return;
			}
			this.self.Error(string.Concat(new object[]
			{
				"template parse error in group ",
				this.self.Group.Name,
				" line ",
				this.self.GroupFileLine,
				"; template context is ",
				this.self.GetEnclosingInstanceStackString()
			}), e);
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x000826DC File Offset: 0x000808DC
		protected void initialize()
		{
			this.tokenNames = TemplateParser.tokenNames_;
		}

		// Token: 0x060011DE RID: 4574 RVA: 0x000826EC File Offset: 0x000808EC
		protected TemplateParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			this.initialize();
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x000826FC File Offset: 0x000808FC
		public TemplateParser(TokenBuffer tokenBuf) : this(tokenBuf, 1)
		{
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x00082708 File Offset: 0x00080908
		protected TemplateParser(TokenStream lexer, int k) : base(lexer, k)
		{
			this.initialize();
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x00082718 File Offset: 0x00080918
		public TemplateParser(TokenStream lexer) : this(lexer, 1)
		{
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x00082724 File Offset: 0x00080924
		public TemplateParser(ParserSharedInputState state) : base(state, 1)
		{
			this.initialize();
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x00082734 File Offset: 0x00080934
		public void template(StringTemplate self)
		{
			this.self = self;
			try
			{
				for (;;)
				{
					switch (this.LA(1))
					{
					case 4:
					{
						IToken token = this.LT(1);
						this.match(4);
						self.AddChunk(new StringRef(self, token.getText()));
						continue;
					}
					case 5:
					{
						IToken token2 = this.LT(1);
						this.match(5);
						if (this.LA(1) != 8 && this.LA(1) != 9)
						{
							self.AddChunk(new NewlineRef(self, token2.getText()));
							continue;
						}
						continue;
					}
					case 6:
					case 7:
					case 10:
					case 11:
						this.action(self);
						continue;
					}
					break;
				}
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, TemplateParser.tokenSet_0_);
			}
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x00082810 File Offset: 0x00080A10
		public void action(StringTemplate self)
		{
			try
			{
				switch (this.LA(1))
				{
				case 6:
				{
					IToken token = this.LT(1);
					this.match(6);
					string indentation = ((ChunkToken)token).Indentation;
					ASTExpr astexpr = self.ParseAction(token.getText());
					astexpr.Indentation = indentation;
					self.AddChunk(astexpr);
					goto IL_355;
				}
				case 7:
				{
					IToken token2 = this.LT(1);
					this.match(7);
					ConditionalExpr conditionalExpr = (ConditionalExpr)self.ParseAction(token2.getText());
					StringTemplate stringTemplate = new StringTemplate(self.Group, null);
					stringTemplate.EnclosingInstance = self;
					stringTemplate.Name = $"{token2.getText()}_subtemplate";
					self.AddChunk(conditionalExpr);
					this.template(stringTemplate);
					if (conditionalExpr != null)
					{
						conditionalExpr.Subtemplate = stringTemplate;
					}
					switch (this.LA(1))
					{
					case 8:
					{
						this.match(8);
						StringTemplate stringTemplate2 = new StringTemplate(self.Group, null);
						stringTemplate2.EnclosingInstance = self;
						stringTemplate2.Name = "else_subtemplate";
						this.template(stringTemplate2);
						if (conditionalExpr != null)
						{
							conditionalExpr.ElseSubtemplate = stringTemplate2;
						}
						break;
					}
					case 9:
						break;
					default:
						throw new NoViableAltException(this.LT(1), this.getFilename());
					}
					this.match(9);
					goto IL_355;
				}
				case 10:
				{
					IToken token3 = this.LT(1);
					this.match(10);
					string text = token3.getText();
					string text2 = null;
					bool flag = false;
					if (text.StartsWith("super."))
					{
						string text3 = text.Substring("super.".Length, text.Length - "super.".Length);
						string unMangledTemplateName = self.Group.GetUnMangledTemplateName(self.Name);
						StringTemplate stringTemplate3 = self.Group.LookupTemplate(unMangledTemplateName);
						if (stringTemplate3 == null)
						{
							self.Group.Error($"reference to region within undefined template: {unMangledTemplateName}");
							flag = true;
						}
						if (!stringTemplate3.ContainsRegionName(text3))
						{
							self.Group.Error($"template {unMangledTemplateName} has no region called {text3}");
							flag = true;
						}
						else
						{
							text2 = self.Group.GetMangledRegionName(unMangledTemplateName, text3);
							text2 = $"super.{text2}";
						}
					}
					else
					{
						StringTemplate stringTemplate4 = self.Group.DefineImplicitRegionTemplate(self, text);
						text2 = stringTemplate4.Name;
					}
					if (!flag)
					{
						string indentation2 = ((ChunkToken)token3).Indentation;
						ASTExpr astexpr2 = self.ParseAction($"{text2}()");
						astexpr2.Indentation = indentation2;
						self.AddChunk(astexpr2);
						goto IL_355;
					}
					goto IL_355;
				}
				case 11:
				{
					IToken token4 = this.LT(1);
					this.match(11);
					string text4 = token4.getText();
					int num = text4.IndexOf("::=");
					if (num >= 1)
					{
						string regionName = text4.Substring(0, num);
						string template = text4.Substring(num + 3, text4.Length - (num + 3));
						StringTemplate stringTemplate5 = self.Group.DefineRegionTemplate(self, regionName, template, 2);
						string indentation3 = ((ChunkToken)token4).Indentation;
						ASTExpr astexpr3 = self.ParseAction($"{stringTemplate5.Name}()");
						astexpr3.Indentation = indentation3;
						self.AddChunk(astexpr3);
						goto IL_355;
					}
					self.Error("embedded region definition screwed up");
					goto IL_355;
				}
				}
				throw new NoViableAltException(this.LT(1), this.getFilename());
				IL_355:;
			}
			catch (RecognitionException ex)
			{
				this.reportError(ex);
				this.recover(ex, TemplateParser.tokenSet_1_);
			}
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x00082BAC File Offset: 0x00080DAC
		private void initializeFactory()
		{
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x00082BB0 File Offset: 0x00080DB0
		private static long[] mk_tokenSet_0_()
		{
			long[] array = new long[2];
			array[0] = 768L;
			return array;
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x00082BD0 File Offset: 0x00080DD0
		private static long[] mk_tokenSet_1_()
		{
			long[] array = new long[2];
			array[0] = 4080L;
			return array;
		}

		// Token: 0x04000F05 RID: 3845
		public const int EOF = 1;

		// Token: 0x04000F06 RID: 3846
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000F07 RID: 3847
		public const int LITERAL = 4;

		// Token: 0x04000F08 RID: 3848
		public const int NEWLINE = 5;

		// Token: 0x04000F09 RID: 3849
		public const int ACTION = 6;

		// Token: 0x04000F0A RID: 3850
		public const int IF = 7;

		// Token: 0x04000F0B RID: 3851
		public const int ELSE = 8;

		// Token: 0x04000F0C RID: 3852
		public const int ENDIF = 9;

		// Token: 0x04000F0D RID: 3853
		public const int REGION_REF = 10;

		// Token: 0x04000F0E RID: 3854
		public const int REGION_DEF = 11;

		// Token: 0x04000F0F RID: 3855
		public const int EXPR = 12;

		// Token: 0x04000F10 RID: 3856
		public const int TEMPLATE = 13;

		// Token: 0x04000F11 RID: 3857
		public const int IF_EXPR = 14;

		// Token: 0x04000F12 RID: 3858
		public const int ESC = 15;

		// Token: 0x04000F13 RID: 3859
		public const int SUBTEMPLATE = 16;

		// Token: 0x04000F14 RID: 3860
		public const int NESTED_PARENS = 17;

		// Token: 0x04000F15 RID: 3861
		public const int INDENT = 18;

		// Token: 0x04000F16 RID: 3862
		public const int COMMENT = 19;

		// Token: 0x04000F17 RID: 3863
		protected StringTemplate self;

		// Token: 0x04000F18 RID: 3864
		public static readonly string[] tokenNames_ = new string[]
		{
			"\"<0>\"",
			"\"EOF\"",
			"\"<2>\"",
			"\"NULL_TREE_LOOKAHEAD\"",
			"\"LITERAL\"",
			"\"NEWLINE\"",
			"\"ACTION\"",
			"\"IF\"",
			"\"ELSE\"",
			"\"ENDIF\"",
			"\"REGION_REF\"",
			"\"REGION_DEF\"",
			"\"EXPR\"",
			"\"TEMPLATE\"",
			"\"IF_EXPR\"",
			"\"ESC\"",
			"\"SUBTEMPLATE\"",
			"\"NESTED_PARENS\"",
			"\"INDENT\"",
			"\"COMMENT\""
		};

		// Token: 0x04000F19 RID: 3865
		public static readonly BitSet tokenSet_0_ = new BitSet(TemplateParser.mk_tokenSet_0_());

		// Token: 0x04000F1A RID: 3866
		public static readonly BitSet tokenSet_1_ = new BitSet(TemplateParser.mk_tokenSet_1_());
	}
}
