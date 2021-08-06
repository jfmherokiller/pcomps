﻿using System.Collections;
using pcomps.Antlr.collections;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000235 RID: 565
	public class ActionParser : LLkParser
	{
		// Token: 0x060010A5 RID: 4261 RVA: 0x00075A58 File Offset: 0x00073C58
		public ActionParser(TokenStream lexer, StringTemplate self) : this(lexer, 2)
		{
			this.self = self;
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x00075A6C File Offset: 0x00073C6C
		public override void reportError(RecognitionException e)
		{
			StringTemplateGroup group = this.self.Group;
			if (group == StringTemplate.defaultGroup)
			{
				this.self.Error(
                    $"action parse error; template context is {this.self.GetEnclosingInstanceStackString()}", e);
				return;
			}
			this.self.Error(string.Concat(new object[]
			{
				"action parse error in group ",
				this.self.Group.Name,
				" line ",
				this.self.GroupFileLine,
				"; template context is ",
				this.self.GetEnclosingInstanceStackString()
			}), e);
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x00075B14 File Offset: 0x00073D14
		protected void initialize()
		{
			this.tokenNames = ActionParser.tokenNames_;
			this.initializeFactory();
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x00075B28 File Offset: 0x00073D28
		protected ActionParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			this.initialize();
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x00075B38 File Offset: 0x00073D38
		public ActionParser(TokenBuffer tokenBuf) : this(tokenBuf, 2)
		{
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x00075B44 File Offset: 0x00073D44
		protected ActionParser(TokenStream lexer, int k) : base(lexer, k)
		{
			this.initialize();
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x00075B54 File Offset: 0x00073D54
		public ActionParser(TokenStream lexer) : this(lexer, 2)
		{
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x00075B60 File Offset: 0x00073D60
		public ActionParser(ParserSharedInputState state) : base(state, 2)
		{
			this.initialize();
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x00075B70 File Offset: 0x00073D70
		public object dummyTopRule()
		{
			object result = null;
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				result = this.action();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
				}
				StringTemplateAST child = (StringTemplateAST)this.astFactory.create(this.LT(1));
				this.astFactory.addASTChild(ref astpair, child);
				this.match(1);
				returnAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_0_);
			}
			this.returnAST = returnAST;
			return result;
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x00075C38 File Offset: 0x00073E38
		public IDictionary action()
		{
			IDictionary result = null;
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				switch (this.LA(1))
				{
				case 8:
				{
					StringTemplateAST root = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.makeASTRoot(ref astpair, root);
					this.match(8);
					this.match(15);
					this.ifCondition();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					this.match(16);
					returnAST = (StringTemplateAST)astpair.root;
					goto IL_1AD;
				}
				case 15:
				case 18:
				case 23:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
				{
					this.templatesExpr();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					int num = this.LA(1);
					if (num != 1)
					{
						if (num != 14)
						{
							throw new NoViableAltException(this.LT(1), this.getFilename());
						}
						this.match(14);
						result = this.optionList();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
					}
					returnAST = (StringTemplateAST)astpair.root;
					goto IL_1AD;
				}
				}
				throw new NoViableAltException(this.LT(1), this.getFilename());
				IL_1AD:;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_0_);
			}
			this.returnAST = returnAST;
			return result;
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00075E44 File Offset: 0x00074044
		public void templatesExpr()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST stringTemplateAST = null;
			try
			{
				this.expr();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
				}
				int num = this.LA(1);
				if (num != 1)
				{
					switch (num)
					{
					case 14:
					case 16:
					case 20:
						goto IL_1BB;
					case 17:
					{
						int num2 = 0;
						while (this.LA(1) == 17)
						{
							this.match(17);
							this.expr();
							if (this.inputState.guessing == 0)
							{
								this.astFactory.addASTChild(ref astpair, this.returnAST);
							}
							num2++;
						}
						if (num2 < 1)
						{
							throw new NoViableAltException(this.LT(1), this.getFilename());
						}
						StringTemplateAST child = (StringTemplateAST)this.astFactory.create(this.LT(1));
						this.astFactory.addASTChild(ref astpair, child);
						this.match(20);
						this.anonymousTemplate();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
						if (this.inputState.guessing == 0)
						{
							stringTemplateAST = (StringTemplateAST)astpair.root;
							stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
							{
								(StringTemplateAST)this.astFactory.create(5, "MULTI_APPLY"),
								stringTemplateAST
							});
							astpair.root = stringTemplateAST;
							if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
							{
								astpair.child = stringTemplateAST.getFirstChild();
							}
							else
							{
								astpair.child = stringTemplateAST;
							}
							astpair.advanceChildToEnd();
							goto IL_284;
						}
						goto IL_284;
					}
					}
					throw new NoViableAltException(this.LT(1), this.getFilename());
				}
				IL_1BB:
				while (this.LA(1) == 20)
				{
					IToken tok = this.LT(1);
					StringTemplateAST stringTemplateAST2 = (StringTemplateAST)this.astFactory.create(tok);
					this.astFactory.makeASTRoot(ref astpair, stringTemplateAST2);
					this.match(20);
					if (this.inputState.guessing == 0)
					{
						stringTemplateAST2.setType(4);
					}
					this.template();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					while (this.LA(1) == 17)
					{
						this.match(17);
						this.template();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
					}
				}
				IL_284:
				stringTemplateAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_1_);
			}
			this.returnAST = stringTemplateAST;
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00076134 File Offset: 0x00074334
		public IDictionary optionList()
		{
			IDictionary dictionary = new Hashtable();
			this.returnAST = null;
			StringTemplateAST returnAST = null;
			try
			{
				this.option(dictionary);
				while (this.LA(1) == 17)
				{
					StringTemplateAST stringTemplateAST = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.match(17);
					this.option(dictionary);
				}
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_0_);
			}
			this.returnAST = returnAST;
			return dictionary;
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x000761CC File Offset: 0x000743CC
		public void ifCondition()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				switch (this.LA(1))
				{
				case 15:
				case 18:
				case 23:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
					this.expr();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					returnAST = (StringTemplateAST)astpair.root;
					goto IL_126;
				case 21:
				{
					StringTemplateAST root = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.makeASTRoot(ref astpair, root);
					this.match(21);
					this.expr();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					returnAST = (StringTemplateAST)astpair.root;
					goto IL_126;
				}
				}
				throw new NoViableAltException(this.LT(1), this.getFilename());
				IL_126:;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_2_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x0007634C File Offset: 0x0007454C
		public void option(IDictionary opts)
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			StringTemplateAST stringTemplateAST = null;
			object value = null;
			try
			{
				IToken tok = this.LT(1);
				StringTemplateAST stringTemplateAST2 = (StringTemplateAST)this.astFactory.create(tok);
				this.astFactory.addASTChild(ref astpair, stringTemplateAST2);
				this.match(18);
				int num = this.LA(1);
				if (num != 1)
				{
					switch (num)
					{
					case 17:
						goto IL_F0;
					case 19:
					{
						StringTemplateAST child = (StringTemplateAST)this.astFactory.create(this.LT(1));
						this.astFactory.addASTChild(ref astpair, child);
						this.match(19);
						this.expr();
						if (this.inputState.guessing == 0)
						{
							stringTemplateAST = (StringTemplateAST)this.returnAST;
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
						if (this.inputState.guessing == 0)
						{
							value = stringTemplateAST;
							goto IL_119;
						}
						goto IL_119;
					}
					}
					throw new NoViableAltException(this.LT(1), this.getFilename());
				}
				IL_F0:
				if (this.inputState.guessing == 0)
				{
					value = ASTExpr.EMPTY_OPTION;
				}
				IL_119:
				if (this.inputState.guessing == 0)
				{
					opts[stringTemplateAST2.getText()] = value;
				}
				returnAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_3_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x000764EC File Offset: 0x000746EC
		public void expr()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				this.primaryExpr();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
				}
				while (this.LA(1) == 22)
				{
					StringTemplateAST root = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.makeASTRoot(ref astpair, root);
					this.match(22);
					this.primaryExpr();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
				}
				returnAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_4_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x000765E0 File Offset: 0x000747E0
		public void anonymousTemplate()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				IToken token = this.LT(1);
				StringTemplateAST stringTemplateAST = (StringTemplateAST)this.astFactory.create(token);
				this.astFactory.addASTChild(ref astpair, stringTemplateAST);
				this.match(31);
				if (this.inputState.guessing == 0)
				{
					StringTemplate stringTemplate = new StringTemplate();
					stringTemplate.Group = this.self.Group;
					stringTemplate.EnclosingInstance = this.self;
					stringTemplate.Template = token.getText();
					stringTemplate.DefineFormalArguments(((StringTemplateToken)token).args);
					stringTemplateAST.StringTemplate = stringTemplate;
				}
				returnAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_5_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x000766E0 File Offset: 0x000748E0
		public void template()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST stringTemplateAST = null;
			try
			{
				int num = this.LA(1);
				if (num <= 18)
				{
					if (num != 15 && num != 18)
					{
						goto IL_86;
					}
				}
				else if (num != 23)
				{
					if (num != 31)
					{
						goto IL_86;
					}
					this.anonymousTemplate();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
						goto IL_99;
					}
					goto IL_99;
				}
				this.namedTemplate();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
					goto IL_99;
				}
				goto IL_99;
				IL_86:
				throw new NoViableAltException(this.LT(1), this.getFilename());
				IL_99:
				if (this.inputState.guessing == 0)
				{
					stringTemplateAST = (StringTemplateAST)astpair.root;
					stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
					{
						(StringTemplateAST)this.astFactory.create(10),
						stringTemplateAST
					});
					astpair.root = stringTemplateAST;
					if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
					{
						astpair.child = stringTemplateAST.getFirstChild();
					}
					else
					{
						astpair.child = stringTemplateAST;
					}
					astpair.advanceChildToEnd();
				}
				stringTemplateAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_5_);
			}
			this.returnAST = stringTemplateAST;
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00076860 File Offset: 0x00074A60
		public void primaryExpr()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST stringTemplateAST = null;
			try
			{
				switch (this.LA(1))
				{
				case 23:
				{
					this.match(23);
					this.match(24);
					IToken tok = this.LT(1);
					StringTemplateAST stringTemplateAST2 = (StringTemplateAST)this.astFactory.create(tok);
					this.astFactory.addASTChild(ref astpair, stringTemplateAST2);
					this.match(18);
					if (this.inputState.guessing == 0)
					{
						stringTemplateAST2.setText($"super.{stringTemplateAST2.getText()}");
					}
					this.argList();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					if (this.inputState.guessing == 0)
					{
						stringTemplateAST = (StringTemplateAST)astpair.root;
						stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
						{
							(StringTemplateAST)this.astFactory.create(7, "include"),
							stringTemplateAST
						});
						astpair.root = stringTemplateAST;
						if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
						{
							astpair.child = stringTemplateAST.getFirstChild();
						}
						else
						{
							astpair.child = stringTemplateAST;
						}
						astpair.advanceChildToEnd();
					}
					stringTemplateAST = (StringTemplateAST)astpair.root;
					goto IL_5BB;
				}
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
					this.function();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					stringTemplateAST = (StringTemplateAST)astpair.root;
					goto IL_5BB;
				case 34:
					this.list();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					stringTemplateAST = (StringTemplateAST)astpair.root;
					goto IL_5BB;
				}
				if (this.LA(1) == 18 && this.LA(2) == 15)
				{
					StringTemplateAST child = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child);
					this.match(18);
					this.argList();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					if (this.inputState.guessing == 0)
					{
						stringTemplateAST = (StringTemplateAST)astpair.root;
						stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
						{
							(StringTemplateAST)this.astFactory.create(7, "include"),
							stringTemplateAST
						});
						astpair.root = stringTemplateAST;
						if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
						{
							astpair.child = stringTemplateAST.getFirstChild();
						}
						else
						{
							astpair.child = stringTemplateAST;
						}
						astpair.advanceChildToEnd();
					}
					stringTemplateAST = (StringTemplateAST)astpair.root;
				}
				else if (ActionParser.tokenSet_6_.member(this.LA(1)) && ActionParser.tokenSet_7_.member(this.LA(2)))
				{
					this.atom();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					while (this.LA(1) == 24)
					{
						StringTemplateAST root = (StringTemplateAST)this.astFactory.create(this.LT(1));
						this.astFactory.makeASTRoot(ref astpair, root);
						this.match(24);
						int num = this.LA(1);
						if (num != 15)
						{
							if (num != 18)
							{
								throw new NoViableAltException(this.LT(1), this.getFilename());
							}
							StringTemplateAST child2 = (StringTemplateAST)this.astFactory.create(this.LT(1));
							this.astFactory.addASTChild(ref astpair, child2);
							this.match(18);
						}
						else
						{
							this.valueExpr();
							if (this.inputState.guessing == 0)
							{
								this.astFactory.addASTChild(ref astpair, this.returnAST);
							}
						}
					}
					stringTemplateAST = (StringTemplateAST)astpair.root;
				}
				else
				{
					bool flag = false;
					if (this.LA(1) == 15 && ActionParser.tokenSet_8_.member(this.LA(2)))
					{
						int pos = this.mark();
						flag = true;
						this.inputState.guessing++;
						try
						{
							this.indirectTemplate();
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
						this.indirectTemplate();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
						if (this.inputState.guessing == 0)
						{
							stringTemplateAST = (StringTemplateAST)astpair.root;
							stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
							{
								(StringTemplateAST)this.astFactory.create(7, "include"),
								stringTemplateAST
							});
							astpair.root = stringTemplateAST;
							if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
							{
								astpair.child = stringTemplateAST.getFirstChild();
							}
							else
							{
								astpair.child = stringTemplateAST;
							}
							astpair.advanceChildToEnd();
						}
						stringTemplateAST = (StringTemplateAST)astpair.root;
					}
					else
					{
						if (this.LA(1) != 15 || !ActionParser.tokenSet_8_.member(this.LA(2)))
						{
							throw new NoViableAltException(this.LT(1), this.getFilename());
						}
						this.valueExpr();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
						stringTemplateAST = (StringTemplateAST)astpair.root;
					}
				}
				IL_5BB:;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_9_);
			}
			this.returnAST = stringTemplateAST;
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x00076E90 File Offset: 0x00075090
		public void argList()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST stringTemplateAST = null;
			try
			{
				if (this.LA(1) == 15 && this.LA(2) == 16)
				{
					this.match(15);
					this.match(16);
					if (this.inputState.guessing == 0)
					{
						stringTemplateAST = (StringTemplateAST)astpair.root;
						stringTemplateAST = (StringTemplateAST)this.astFactory.create(6, "ARGS");
						astpair.root = stringTemplateAST;
						if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
						{
							astpair.child = stringTemplateAST.getFirstChild();
						}
						else
						{
							astpair.child = stringTemplateAST;
						}
						astpair.advanceChildToEnd();
					}
				}
				else
				{
					bool flag = false;
					if (this.LA(1) == 15 && ActionParser.tokenSet_8_.member(this.LA(2)))
					{
						int pos = this.mark();
						flag = true;
						this.inputState.guessing++;
						try
						{
							this.singleArg();
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
						this.singleArg();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
						stringTemplateAST = (StringTemplateAST)astpair.root;
					}
					else
					{
						if (this.LA(1) != 15 || (this.LA(2) != 18 && this.LA(2) != 36))
						{
							throw new NoViableAltException(this.LT(1), this.getFilename());
						}
						this.match(15);
						this.argumentAssignment();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
						while (this.LA(1) == 17)
						{
							this.match(17);
							this.argumentAssignment();
							if (this.inputState.guessing == 0)
							{
								this.astFactory.addASTChild(ref astpair, this.returnAST);
							}
						}
						this.match(16);
						if (this.inputState.guessing == 0)
						{
							stringTemplateAST = (StringTemplateAST)astpair.root;
							stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
							{
								(StringTemplateAST)this.astFactory.create(6, "ARGS"),
								stringTemplateAST
							});
							astpair.root = stringTemplateAST;
							if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
							{
								astpair.child = stringTemplateAST.getFirstChild();
							}
							else
							{
								astpair.child = stringTemplateAST;
							}
							astpair.advanceChildToEnd();
						}
						stringTemplateAST = (StringTemplateAST)astpair.root;
					}
				}
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_9_);
			}
			this.returnAST = stringTemplateAST;
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00077188 File Offset: 0x00075388
		public void atom()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				int num = this.LA(1);
				if (num != 18)
				{
					switch (num)
					{
					case 31:
					{
						StringTemplateAST child = (StringTemplateAST)this.astFactory.create(this.LT(1));
						this.astFactory.addASTChild(ref astpair, child);
						this.match(31);
						returnAST = (StringTemplateAST)astpair.root;
						break;
					}
					case 32:
					{
						StringTemplateAST child2 = (StringTemplateAST)this.astFactory.create(this.LT(1));
						this.astFactory.addASTChild(ref astpair, child2);
						this.match(32);
						returnAST = (StringTemplateAST)astpair.root;
						break;
					}
					case 33:
					{
						StringTemplateAST child3 = (StringTemplateAST)this.astFactory.create(this.LT(1));
						this.astFactory.addASTChild(ref astpair, child3);
						this.match(33);
						returnAST = (StringTemplateAST)astpair.root;
						break;
					}
					default:
						throw new NoViableAltException(this.LT(1), this.getFilename());
					}
				}
				else
				{
					StringTemplateAST child4 = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child4);
					this.match(18);
					returnAST = (StringTemplateAST)astpair.root;
				}
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_7_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x0007733C File Offset: 0x0007553C
		public void valueExpr()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				IToken tok = this.LT(1);
				StringTemplateAST stringTemplateAST = (StringTemplateAST)this.astFactory.create(tok);
				this.astFactory.makeASTRoot(ref astpair, stringTemplateAST);
				this.match(15);
				this.templatesExpr();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
				}
				this.match(16);
				if (this.inputState.guessing == 0)
				{
					stringTemplateAST.setType(9);
					stringTemplateAST.setText("value");
				}
				returnAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_7_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00077430 File Offset: 0x00075630
		public void function()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST stringTemplateAST = null;
			try
			{
				switch (this.LA(1))
				{
				case 25:
				{
					StringTemplateAST child = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child);
					this.match(25);
					break;
				}
				case 26:
				{
					StringTemplateAST child2 = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child2);
					this.match(26);
					break;
				}
				case 27:
				{
					StringTemplateAST child3 = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child3);
					this.match(27);
					break;
				}
				case 28:
				{
					StringTemplateAST child4 = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child4);
					this.match(28);
					break;
				}
				case 29:
				{
					StringTemplateAST child5 = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child5);
					this.match(29);
					break;
				}
				case 30:
				{
					StringTemplateAST child6 = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child6);
					this.match(30);
					break;
				}
				default:
					throw new NoViableAltException(this.LT(1), this.getFilename());
				}
				this.singleArg();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
				}
				if (this.inputState.guessing == 0)
				{
					stringTemplateAST = (StringTemplateAST)astpair.root;
					stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
					{
						(StringTemplateAST)this.astFactory.create(11),
						stringTemplateAST
					});
					astpair.root = stringTemplateAST;
					if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
					{
						astpair.child = stringTemplateAST.getFirstChild();
					}
					else
					{
						astpair.child = stringTemplateAST;
					}
					astpair.advanceChildToEnd();
				}
				stringTemplateAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_9_);
			}
			this.returnAST = stringTemplateAST;
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x000776D8 File Offset: 0x000758D8
		public void list()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				IToken tok = this.LT(1);
				StringTemplateAST stringTemplateAST = (StringTemplateAST)this.astFactory.create(tok);
				this.astFactory.makeASTRoot(ref astpair, stringTemplateAST);
				this.match(34);
				if (this.inputState.guessing == 0)
				{
					stringTemplateAST.setType(13);
					stringTemplateAST.setText("value");
				}
				this.expr();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
				}
				while (this.LA(1) == 17)
				{
					this.match(17);
					this.expr();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
				}
				this.match(35);
				returnAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_9_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x00077804 File Offset: 0x00075A04
		public void indirectTemplate()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST stringTemplateAST = null;
			StringTemplateAST stringTemplateAST2 = null;
			StringTemplateAST stringTemplateAST3 = null;
			try
			{
				StringTemplateAST stringTemplateAST4 = (StringTemplateAST)this.astFactory.create(this.LT(1));
				this.match(15);
				this.templatesExpr();
				if (this.inputState.guessing == 0)
				{
					stringTemplateAST2 = (StringTemplateAST)this.returnAST;
				}
				StringTemplateAST stringTemplateAST5 = (StringTemplateAST)this.astFactory.create(this.LT(1));
				this.match(16);
				this.argList();
				if (this.inputState.guessing == 0)
				{
					stringTemplateAST3 = (StringTemplateAST)this.returnAST;
				}
				if (this.inputState.guessing == 0)
				{
					stringTemplateAST = (StringTemplateAST)astpair.root;
					stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
					{
						(StringTemplateAST)this.astFactory.create(9, "value"),
						stringTemplateAST2,
						stringTemplateAST3
					});
					astpair.root = stringTemplateAST;
					if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
					{
						astpair.child = stringTemplateAST.getFirstChild();
					}
					else
					{
						astpair.child = stringTemplateAST;
					}
					astpair.advanceChildToEnd();
				}
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_9_);
			}
			this.returnAST = stringTemplateAST;
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x00077980 File Offset: 0x00075B80
		public void nonAlternatingTemplateExpr()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				this.expr();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
				}
				while (this.LA(1) == 20)
				{
					IToken tok = this.LT(1);
					StringTemplateAST stringTemplateAST = (StringTemplateAST)this.astFactory.create(tok);
					this.astFactory.makeASTRoot(ref astpair, stringTemplateAST);
					this.match(20);
					if (this.inputState.guessing == 0)
					{
						stringTemplateAST.setType(4);
					}
					this.template();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
				}
				returnAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_10_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x00077A90 File Offset: 0x00075C90
		public void singleArg()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST stringTemplateAST = null;
			try
			{
				this.match(15);
				this.nonAlternatingTemplateExpr();
				if (this.inputState.guessing == 0)
				{
					this.astFactory.addASTChild(ref astpair, this.returnAST);
				}
				this.match(16);
				if (this.inputState.guessing == 0)
				{
					stringTemplateAST = (StringTemplateAST)astpair.root;
					stringTemplateAST = (StringTemplateAST)this.astFactory.make(new AST[]
					{
						(StringTemplateAST)this.astFactory.create(12, "SINGLEVALUEARG"),
						stringTemplateAST
					});
					astpair.root = stringTemplateAST;
					if (stringTemplateAST != null && stringTemplateAST.getFirstChild() != null)
					{
						astpair.child = stringTemplateAST.getFirstChild();
					}
					else
					{
						astpair.child = stringTemplateAST;
					}
					astpair.advanceChildToEnd();
				}
				stringTemplateAST = (StringTemplateAST)astpair.root;
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_9_);
			}
			this.returnAST = stringTemplateAST;
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x00077BB4 File Offset: 0x00075DB4
		public void namedTemplate()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				int num = this.LA(1);
				if (num != 15)
				{
					if (num != 18)
					{
						if (num != 23)
						{
							throw new NoViableAltException(this.LT(1), this.getFilename());
						}
						this.match(23);
						this.match(24);
						IToken tok = this.LT(1);
						StringTemplateAST stringTemplateAST = (StringTemplateAST)this.astFactory.create(tok);
						this.astFactory.addASTChild(ref astpair, stringTemplateAST);
						this.match(18);
						if (this.inputState.guessing == 0)
						{
							stringTemplateAST.setText($"super.{stringTemplateAST.getText()}");
						}
						this.argList();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
						returnAST = (StringTemplateAST)astpair.root;
					}
					else
					{
						StringTemplateAST child = (StringTemplateAST)this.astFactory.create(this.LT(1));
						this.astFactory.addASTChild(ref astpair, child);
						this.match(18);
						this.argList();
						if (this.inputState.guessing == 0)
						{
							this.astFactory.addASTChild(ref astpair, this.returnAST);
						}
						returnAST = (StringTemplateAST)astpair.root;
					}
				}
				else
				{
					this.indirectTemplate();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					returnAST = (StringTemplateAST)astpair.root;
				}
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_5_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x00077D94 File Offset: 0x00075F94
		public void argumentAssignment()
		{
			this.returnAST = null;
			ASTPair astpair = default(ASTPair);
			StringTemplateAST returnAST = null;
			try
			{
				int num = this.LA(1);
				if (num != 18)
				{
					if (num != 36)
					{
						throw new NoViableAltException(this.LT(1), this.getFilename());
					}
					StringTemplateAST child = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child);
					this.match(36);
					returnAST = (StringTemplateAST)astpair.root;
				}
				else
				{
					StringTemplateAST child2 = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.addASTChild(ref astpair, child2);
					this.match(18);
					StringTemplateAST root = (StringTemplateAST)this.astFactory.create(this.LT(1));
					this.astFactory.makeASTRoot(ref astpair, root);
					this.match(19);
					this.nonAlternatingTemplateExpr();
					if (this.inputState.guessing == 0)
					{
						this.astFactory.addASTChild(ref astpair, this.returnAST);
					}
					returnAST = (StringTemplateAST)astpair.root;
				}
			}
			catch (RecognitionException ex)
			{
				if (this.inputState.guessing != 0)
				{
					throw ex;
				}
				this.reportError(ex);
				this.recover(ex, ActionParser.tokenSet_10_);
			}
			this.returnAST = returnAST;
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00077F0C File Offset: 0x0007610C
		public new StringTemplateAST getAST()
		{
			return (StringTemplateAST)this.returnAST;
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x00077F1C File Offset: 0x0007611C
		private void initializeFactory()
		{
			if (this.astFactory == null)
			{
				this.astFactory = new ASTFactory("Antlr.StringTemplate.Language.StringTemplateAST");
			}
			ActionParser.initializeASTFactory(this.astFactory);
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x00077F44 File Offset: 0x00076144
		public static void initializeASTFactory(ASTFactory factory)
		{
			factory.setMaxNodeType(41);
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x00077F50 File Offset: 0x00076150
		private static long[] mk_tokenSet_0_()
		{
			long[] array = new long[2];
			array[0] = 2L;
			return array;
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x00077F6C File Offset: 0x0007616C
		private static long[] mk_tokenSet_1_()
		{
			long[] array = new long[2];
			array[0] = 81922L;
			return array;
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x00077F8C File Offset: 0x0007618C
		private static long[] mk_tokenSet_2_()
		{
			long[] array = new long[2];
			array[0] = 65536L;
			return array;
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x00077FAC File Offset: 0x000761AC
		private static long[] mk_tokenSet_3_()
		{
			long[] array = new long[2];
			array[0] = 131074L;
			return array;
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x00077FCC File Offset: 0x000761CC
		private static long[] mk_tokenSet_4_()
		{
			long[] array = new long[2];
			array[0] = 34360999938L;
			return array;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00077FF0 File Offset: 0x000761F0
		private static long[] mk_tokenSet_5_()
		{
			long[] array = new long[2];
			array[0] = 1261570L;
			return array;
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x00078010 File Offset: 0x00076210
		private static long[] mk_tokenSet_6_()
		{
			long[] array = new long[2];
			array[0] = 15032647680L;
			return array;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x00078034 File Offset: 0x00076234
		private static long[] mk_tokenSet_7_()
		{
			long[] array = new long[2];
			array[0] = 34381971458L;
			return array;
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x00078058 File Offset: 0x00076258
		private static long[] mk_tokenSet_8_()
		{
			long[] array = new long[2];
			array[0] = 34334867456L;
			return array;
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0007807C File Offset: 0x0007627C
		private static long[] mk_tokenSet_9_()
		{
			long[] array = new long[2];
			array[0] = 34365194242L;
			return array;
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x000780A0 File Offset: 0x000762A0
		private static long[] mk_tokenSet_10_()
		{
			long[] array = new long[2];
			array[0] = 196608L;
			return array;
		}

		// Token: 0x04000D9E RID: 3486
		public const int EOF = 1;

		// Token: 0x04000D9F RID: 3487
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000DA0 RID: 3488
		public const int APPLY = 4;

		// Token: 0x04000DA1 RID: 3489
		public const int MULTI_APPLY = 5;

		// Token: 0x04000DA2 RID: 3490
		public const int ARGS = 6;

		// Token: 0x04000DA3 RID: 3491
		public const int INCLUDE = 7;

		// Token: 0x04000DA4 RID: 3492
		public const int CONDITIONAL = 8;

		// Token: 0x04000DA5 RID: 3493
		public const int VALUE = 9;

		// Token: 0x04000DA6 RID: 3494
		public const int TEMPLATE = 10;

		// Token: 0x04000DA7 RID: 3495
		public const int FUNCTION = 11;

		// Token: 0x04000DA8 RID: 3496
		public const int SINGLEVALUEARG = 12;

		// Token: 0x04000DA9 RID: 3497
		public const int LIST = 13;

		// Token: 0x04000DAA RID: 3498
		public const int SEMI = 14;

		// Token: 0x04000DAB RID: 3499
		public const int LPAREN = 15;

		// Token: 0x04000DAC RID: 3500
		public const int RPAREN = 16;

		// Token: 0x04000DAD RID: 3501
		public const int COMMA = 17;

		// Token: 0x04000DAE RID: 3502
		public const int ID = 18;

		// Token: 0x04000DAF RID: 3503
		public const int ASSIGN = 19;

		// Token: 0x04000DB0 RID: 3504
		public const int COLON = 20;

		// Token: 0x04000DB1 RID: 3505
		public const int NOT = 21;

		// Token: 0x04000DB2 RID: 3506
		public const int PLUS = 22;

		// Token: 0x04000DB3 RID: 3507
		public const int LITERAL_super = 23;

		// Token: 0x04000DB4 RID: 3508
		public const int DOT = 24;

		// Token: 0x04000DB5 RID: 3509
		public const int LITERAL_first = 25;

		// Token: 0x04000DB6 RID: 3510
		public const int LITERAL_rest = 26;

		// Token: 0x04000DB7 RID: 3511
		public const int LITERAL_last = 27;

		// Token: 0x04000DB8 RID: 3512
		public const int LITERAL_length = 28;

		// Token: 0x04000DB9 RID: 3513
		public const int LITERAL_strip = 29;

		// Token: 0x04000DBA RID: 3514
		public const int LITERAL_trunc = 30;

		// Token: 0x04000DBB RID: 3515
		public const int ANONYMOUS_TEMPLATE = 31;

		// Token: 0x04000DBC RID: 3516
		public const int STRING = 32;

		// Token: 0x04000DBD RID: 3517
		public const int INT = 33;

		// Token: 0x04000DBE RID: 3518
		public const int LBRACK = 34;

		// Token: 0x04000DBF RID: 3519
		public const int RBRACK = 35;

		// Token: 0x04000DC0 RID: 3520
		public const int DOTDOTDOT = 36;

		// Token: 0x04000DC1 RID: 3521
		public const int TEMPLATE_ARGS = 37;

		// Token: 0x04000DC2 RID: 3522
		public const int NESTED_ANONYMOUS_TEMPLATE = 38;

		// Token: 0x04000DC3 RID: 3523
		public const int ESC_CHAR = 39;

		// Token: 0x04000DC4 RID: 3524
		public const int WS = 40;

		// Token: 0x04000DC5 RID: 3525
		public const int WS_CHAR = 41;

		// Token: 0x04000DC6 RID: 3526
		protected StringTemplate self;

		// Token: 0x04000DC7 RID: 3527
		public static readonly string[] tokenNames_ = new string[]
		{
			"\"<0>\"",
			"\"EOF\"",
			"\"<2>\"",
			"\"NULL_TREE_LOOKAHEAD\"",
			"\"APPLY\"",
			"\"MULTI_APPLY\"",
			"\"ARGS\"",
			"\"INCLUDE\"",
			"\"if\"",
			"\"VALUE\"",
			"\"TEMPLATE\"",
			"\"FUNCTION\"",
			"\"SINGLEVALUEARG\"",
			"\"LIST\"",
			"\"SEMI\"",
			"\"LPAREN\"",
			"\"RPAREN\"",
			"\"COMMA\"",
			"\"ID\"",
			"\"ASSIGN\"",
			"\"COLON\"",
			"\"NOT\"",
			"\"PLUS\"",
			"\"super\"",
			"\"DOT\"",
			"\"first\"",
			"\"rest\"",
			"\"last\"",
			"\"length\"",
			"\"strip\"",
			"\"trunc\"",
			"\"ANONYMOUS_TEMPLATE\"",
			"\"STRING\"",
			"\"INT\"",
			"\"LBRACK\"",
			"\"RBRACK\"",
			"\"DOTDOTDOT\"",
			"\"TEMPLATE_ARGS\"",
			"\"NESTED_ANONYMOUS_TEMPLATE\"",
			"\"ESC_CHAR\"",
			"\"WS\"",
			"\"WS_CHAR\""
		};

		// Token: 0x04000DC8 RID: 3528
		public static readonly BitSet tokenSet_0_ = new BitSet(ActionParser.mk_tokenSet_0_());

		// Token: 0x04000DC9 RID: 3529
		public static readonly BitSet tokenSet_1_ = new BitSet(ActionParser.mk_tokenSet_1_());

		// Token: 0x04000DCA RID: 3530
		public static readonly BitSet tokenSet_2_ = new BitSet(ActionParser.mk_tokenSet_2_());

		// Token: 0x04000DCB RID: 3531
		public static readonly BitSet tokenSet_3_ = new BitSet(ActionParser.mk_tokenSet_3_());

		// Token: 0x04000DCC RID: 3532
		public static readonly BitSet tokenSet_4_ = new BitSet(ActionParser.mk_tokenSet_4_());

		// Token: 0x04000DCD RID: 3533
		public static readonly BitSet tokenSet_5_ = new BitSet(ActionParser.mk_tokenSet_5_());

		// Token: 0x04000DCE RID: 3534
		public static readonly BitSet tokenSet_6_ = new BitSet(ActionParser.mk_tokenSet_6_());

		// Token: 0x04000DCF RID: 3535
		public static readonly BitSet tokenSet_7_ = new BitSet(ActionParser.mk_tokenSet_7_());

		// Token: 0x04000DD0 RID: 3536
		public static readonly BitSet tokenSet_8_ = new BitSet(ActionParser.mk_tokenSet_8_());

		// Token: 0x04000DD1 RID: 3537
		public static readonly BitSet tokenSet_9_ = new BitSet(ActionParser.mk_tokenSet_9_());

		// Token: 0x04000DD2 RID: 3538
		public static readonly BitSet tokenSet_10_ = new BitSet(ActionParser.mk_tokenSet_10_());
	}
}
