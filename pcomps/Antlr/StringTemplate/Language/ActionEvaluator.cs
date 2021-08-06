using System.Collections;
using System.IO;
using pcomps.Antlr.collections;
using pcomps.Antlr.collections.impl;
using pcomps.Antlr.StringTemplate.Collections;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000231 RID: 561
	public class ActionEvaluator : TreeParser
	{
		// Token: 0x0600106D RID: 4205 RVA: 0x00072474 File Offset: 0x00070674
		public ActionEvaluator(StringTemplate self, ASTExpr chunk, IStringTemplateWriter @out)
		{
			this.self = self;
			this.chunk = chunk;
			this.@out = @out;
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x00072494 File Offset: 0x00070694
		public override void reportError(RecognitionException e)
		{
			self.Error("eval tree parse error", e);
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x000724A8 File Offset: 0x000706A8
		public ActionEvaluator()
		{
			tokenNames = tokenNames_;
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x000724BC File Offset: 0x000706BC
		public int action(AST _t)
		{
			int result = 0;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			try
			{
				object o = expr(_t);
				_t = retTree_;
				result = chunk.WriteAttribute(self, o, @out);
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x0007252C File Offset: 0x0007072C
		public object expr(AST _t)
		{
			object result = null;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			try
			{
				if (_t == null)
				{
					_t = ASTNULL;
				}
				int type = _t.Type;
				if (type <= 18)
				{
					switch (type)
					{
					case 4:
					case 5:
						result = templateApplication(_t);
						_t = retTree_;
						goto IL_1E8;
					case 6:
					case 8:
					case 10:
					case 12:
						goto IL_1E1;
					case 7:
						result = templateInclude(_t);
						_t = retTree_;
						goto IL_1E8;
					case 9:
					{
						AST ast = _t;
						if (_t != ASTNULL)
						{
							StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
						}
						match(_t, 9);
						_t = _t.getFirstChild();
						object o = expr(_t);
						_t = retTree_;
						_t = ast;
						_t = _t.getNextSibling();
						StringWriter stringWriter = new StringWriter();
						IStringTemplateWriter output = self.Group.CreateInstanceOfTemplateWriter(stringWriter);
						int num = chunk.WriteAttribute(self, o, output);
						if (num > 0)
						{
							result = stringWriter.ToString();
							goto IL_1E8;
						}
						goto IL_1E8;
					}
					case 11:
						result = function(_t);
						_t = retTree_;
						goto IL_1E8;
					case 13:
						result = list(_t);
						_t = retTree_;
						goto IL_1E8;
					default:
						if (type != 18)
						{
							goto IL_1E1;
						}
						break;
					}
				}
				else
				{
					switch (type)
					{
					case 22:
					{
						AST ast2 = _t;
						if (_t != ASTNULL)
						{
							StringTemplateAST stringTemplateAST3 = (StringTemplateAST)_t;
						}
						match(_t, 22);
						_t = _t.getFirstChild();
						object a = expr(_t);
						_t = retTree_;
						object b = expr(_t);
						_t = retTree_;
						result = chunk.Add(a, b);
						_t = ast2;
						_t = _t.getNextSibling();
						goto IL_1E8;
					}
					case 23:
						goto IL_1E1;
					case 24:
						break;
					default:
						switch (type)
						{
						case 31:
						case 32:
						case 33:
							break;
						default:
							goto IL_1E1;
						}
						break;
					}
				}
				result = attribute(_t);
				_t = retTree_;
				goto IL_1E8;
				IL_1E1:
				throw new NoViableAltException(_t);
				IL_1E8:;
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00072760 File Offset: 0x00070960
		public object templateApplication(AST _t)
		{
			object result = null;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			try
			{
				if (_t == null)
				{
					_t = ASTNULL;
				}
				switch (_t.Type)
				{
				case 4:
				{
					AST ast = _t;
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
					}
					match(_t, 4);
					_t = _t.getFirstChild();
					object obj = expr(_t);
					_t = retTree_;
					int num = 0;
					for (;;)
					{
						if (_t == null)
						{
							_t = ASTNULL;
						}
						if (_t.Type != 10)
						{
							break;
						}
						template(_t, arrayList);
						_t = retTree_;
						num++;
					}
					if (num < 1)
					{
						throw new NoViableAltException(_t);
					}
					result = chunk.ApplyListOfAlternatingTemplates(self, obj, arrayList);
					_t = ast;
					_t = _t.getNextSibling();
					break;
				}
				case 5:
				{
					AST ast2 = _t;
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST3 = (StringTemplateAST)_t;
					}
					match(_t, 5);
					_t = _t.getFirstChild();
					int num2 = 0;
					for (;;)
					{
						if (_t == null)
						{
							_t = ASTNULL;
						}
						if (!tokenSet_0_.member(_t.Type))
						{
							break;
						}
						object obj = expr(_t);
						_t = retTree_;
						arrayList2.Add(obj);
						num2++;
					}
					if (num2 < 1)
					{
						throw new NoViableAltException(_t);
					}
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST4 = (StringTemplateAST)_t;
					}
					match(_t, 20);
					_t = _t.getNextSibling();
					StringTemplateAST stringTemplateAST5 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
					match(_t, 31);
					_t = _t.getNextSibling();
					StringTemplate stringTemplate = stringTemplateAST5.StringTemplate;
					arrayList.Add(stringTemplate);
					result = chunk.ApplyTemplateToListOfAttributes(self, arrayList2, stringTemplateAST5.StringTemplate);
					_t = ast2;
					_t = _t.getNextSibling();
					break;
				}
				default:
					throw new NoViableAltException(_t);
				}
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x00072974 File Offset: 0x00070B74
		public object attribute(AST _t)
		{
			object result = null;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			string propertyName = null;
			try
			{
				if (_t == null)
				{
					_t = ASTNULL;
				}
				int type = _t.Type;
				if (type != 18)
				{
					if (type != 24)
					{
						switch (type)
						{
						case 31:
						{
							StringTemplateAST stringTemplateAST2 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
							match(_t, 31);
							_t = _t.getNextSibling();
							result = stringTemplateAST2.getText();
							if (stringTemplateAST2.getText() != null)
							{
								result = new StringTemplate(self.Group, stringTemplateAST2.getText())
								{
									EnclosingInstance = self,
									Name = "<anonymous template argument>"
								};
							}
							break;
						}
						case 32:
						{
							StringTemplateAST stringTemplateAST3 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
							match(_t, 32);
							_t = _t.getNextSibling();
							result = stringTemplateAST3.getText();
							break;
						}
						case 33:
						{
							StringTemplateAST stringTemplateAST4 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
							match(_t, 33);
							_t = _t.getNextSibling();
							result = int.Parse(stringTemplateAST4.getText());
							break;
						}
						default:
							throw new NoViableAltException(_t);
						}
					}
					else
					{
						AST ast = _t;
						if (_t != ASTNULL)
						{
							StringTemplateAST stringTemplateAST5 = (StringTemplateAST)_t;
						}
						match(_t, 24);
						_t = _t.getFirstChild();
						object o = expr(_t);
						_t = retTree_;
						if (_t == null)
						{
							_t = ASTNULL;
						}
						int type2 = _t.Type;
						if (type2 != 9)
						{
							if (type2 != 18)
							{
								throw new NoViableAltException(_t);
							}
							StringTemplateAST stringTemplateAST6 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
							match(_t, 18);
							_t = _t.getNextSibling();
							propertyName = stringTemplateAST6.getText();
						}
						else
						{
							AST ast2 = _t;
							if (_t != ASTNULL)
							{
								StringTemplateAST stringTemplateAST7 = (StringTemplateAST)_t;
							}
							match(_t, 9);
							_t = _t.getFirstChild();
							object obj = expr(_t);
							_t = retTree_;
							_t = ast2;
							_t = _t.getNextSibling();
							if (obj != null)
							{
								propertyName = obj.ToString();
							}
						}
						_t = ast;
						_t = _t.getNextSibling();
						result = chunk.GetObjectProperty(self, o, propertyName);
					}
				}
				else
				{
					StringTemplateAST stringTemplateAST8 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
					match(_t, 18);
					_t = _t.getNextSibling();
					result = self.GetAttribute(stringTemplateAST8.getText());
				}
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x00072C28 File Offset: 0x00070E28
		public object templateInclude(AST _t)
		{
			object result = null;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			string text = null;
			try
			{
				AST ast = _t;
				if (_t != ASTNULL)
				{
					StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
				}
				match(_t, 7);
				_t = _t.getFirstChild();
				if (_t == null)
				{
					_t = ASTNULL;
				}
				int type = _t.Type;
				StringTemplateAST argumentsAST;
				if (type != 9)
				{
					if (type != 18)
					{
						throw new NoViableAltException(_t);
					}
					StringTemplateAST stringTemplateAST3 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
					match(_t, 18);
					_t = _t.getNextSibling();
					StringTemplateAST stringTemplateAST4 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
					if (_t == null)
					{
						throw new MismatchedTokenException();
					}
					_t = _t.getNextSibling();
					text = stringTemplateAST3.getText();
					argumentsAST = stringTemplateAST4;
				}
				else
				{
					AST ast2 = _t;
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST5 = (StringTemplateAST)_t;
					}
					match(_t, 9);
					_t = _t.getFirstChild();
					object obj = expr(_t);
					_t = retTree_;
					StringTemplateAST stringTemplateAST6 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
					if (_t == null)
					{
						throw new MismatchedTokenException();
					}
					_t = _t.getNextSibling();
					_t = ast2;
					_t = _t.getNextSibling();
					if (obj != null)
					{
						text = obj.ToString();
					}
					argumentsAST = stringTemplateAST6;
				}
				_t = ast;
				_t = _t.getNextSibling();
				if (text != null)
				{
					result = chunk.GetTemplateInclude(self, text, argumentsAST);
				}
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x00072DC4 File Offset: 0x00070FC4
		public object function(AST _t)
		{
			object result = null;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			try
			{
				AST ast = _t;
				if (_t != ASTNULL)
				{
					StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
				}
				match(_t, 11);
				_t = _t.getFirstChild();
				if (_t == null)
				{
					_t = ASTNULL;
				}
				switch (_t.Type)
				{
				case 25:
				{
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST3 = (StringTemplateAST)_t;
					}
					match(_t, 25);
					_t = _t.getNextSibling();
					object attribute = singleFunctionArg(_t);
					_t = retTree_;
					result = chunk.First(attribute);
					break;
				}
				case 26:
				{
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST4 = (StringTemplateAST)_t;
					}
					match(_t, 26);
					_t = _t.getNextSibling();
					object attribute = singleFunctionArg(_t);
					_t = retTree_;
					result = chunk.Rest(attribute);
					break;
				}
				case 27:
				{
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST5 = (StringTemplateAST)_t;
					}
					match(_t, 27);
					_t = _t.getNextSibling();
					object attribute = singleFunctionArg(_t);
					_t = retTree_;
					result = chunk.Last(attribute);
					break;
				}
				case 28:
				{
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST6 = (StringTemplateAST)_t;
					}
					match(_t, 28);
					_t = _t.getNextSibling();
					object attribute = singleFunctionArg(_t);
					_t = retTree_;
					result = chunk.Length(attribute);
					break;
				}
				case 29:
				{
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST7 = (StringTemplateAST)_t;
					}
					match(_t, 29);
					_t = _t.getNextSibling();
					object attribute = singleFunctionArg(_t);
					_t = retTree_;
					result = chunk.Strip(attribute);
					break;
				}
				case 30:
				{
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST8 = (StringTemplateAST)_t;
					}
					match(_t, 30);
					_t = _t.getNextSibling();
					object attribute = singleFunctionArg(_t);
					_t = retTree_;
					result = chunk.Trunc(attribute);
					break;
				}
				default:
					throw new NoViableAltException(_t);
				}
				_t = ast;
				_t = _t.getNextSibling();
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x00073008 File Offset: 0x00071208
		public object list(AST _t)
		{
			object result = null;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			IList list = new ArrayList();
			result = new CatIterator(list);
			try
			{
				AST ast = _t;
				if (_t != ASTNULL)
				{
					StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
				}
				match(_t, 13);
				_t = _t.getFirstChild();
				int num = 0;
				for (;;)
				{
					if (_t == null)
					{
						_t = ASTNULL;
					}
					if (!tokenSet_0_.member(_t.Type))
					{
						break;
					}
					object obj = expr(_t);
					_t = retTree_;
					if (obj != null)
					{
						obj = ASTExpr.ConvertAnythingToIterator(obj);
						list.Add(obj);
					}
					num++;
				}
				if (num < 1)
				{
					throw new NoViableAltException(_t);
				}
				_t = ast;
				_t = _t.getNextSibling();
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x000730E4 File Offset: 0x000712E4
		public void template(AST _t, ArrayList templatesToApply)
		{
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			try
			{
				AST ast = _t;
				if (_t != ASTNULL)
				{
					StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
				}
				match(_t, 10);
				_t = _t.getFirstChild();
				if (_t == null)
				{
					_t = ASTNULL;
				}
				int type = _t.Type;
				if (type != 9)
				{
					if (type != 18)
					{
						if (type != 31)
						{
							throw new NoViableAltException(_t);
						}
						StringTemplateAST stringTemplateAST3 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
						match(_t, 31);
						_t = _t.getNextSibling();
						StringTemplate stringTemplate = stringTemplateAST3.StringTemplate;
						stringTemplate.Group = self.Group;
						templatesToApply.Add(stringTemplate);
					}
					else
					{
						StringTemplateAST stringTemplateAST4 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
						match(_t, 18);
						_t = _t.getNextSibling();
						StringTemplateAST argumentsAST = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
						if (_t == null)
						{
							throw new MismatchedTokenException();
						}
						_t = _t.getNextSibling();
						string text = stringTemplateAST4.getText();
						StringTemplateGroup group = self.Group;
						StringTemplate embeddedInstanceOf = group.GetEmbeddedInstanceOf(self, text);
						if (embeddedInstanceOf != null)
						{
							embeddedInstanceOf.ArgumentsAST = argumentsAST;
							templatesToApply.Add(embeddedInstanceOf);
						}
					}
				}
				else
				{
					AST ast2 = _t;
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST5 = (StringTemplateAST)_t;
					}
					match(_t, 9);
					_t = _t.getFirstChild();
					object obj = expr(_t);
					_t = retTree_;
					StringTemplateAST argumentsAST2 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
					if (_t == null)
					{
						throw new MismatchedTokenException();
					}
					_t = _t.getNextSibling();
					if (obj != null)
					{
						string name = obj.ToString();
						StringTemplateGroup group2 = self.Group;
						StringTemplate embeddedInstanceOf2 = group2.GetEmbeddedInstanceOf(self, name);
						if (embeddedInstanceOf2 != null)
						{
							embeddedInstanceOf2.ArgumentsAST = argumentsAST2;
							templatesToApply.Add(embeddedInstanceOf2);
						}
					}
					_t = ast2;
					_t = _t.getNextSibling();
				}
				_t = ast;
				_t = _t.getNextSibling();
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x00073320 File Offset: 0x00071520
		public object singleFunctionArg(AST _t)
		{
			object result = null;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			try
			{
				AST ast = _t;
				if (_t != ASTNULL)
				{
					StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
				}
				match(_t, 12);
				_t = _t.getFirstChild();
				result = expr(_t);
				_t = retTree_;
				_t = ast;
				_t = _t.getNextSibling();
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x000733A4 File Offset: 0x000715A4
		public bool ifCondition(AST _t)
		{
			bool result = false;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			try
			{
				if (_t == null)
				{
					_t = ASTNULL;
				}
				int type = _t.Type;
				object a;
				switch (type)
				{
				case 4:
				case 5:
				case 7:
				case 9:
				case 11:
				case 13:
					break;
				case 6:
				case 8:
				case 10:
				case 12:
					goto IL_FA;
				default:
					switch (type)
					{
					case 18:
					case 22:
					case 24:
						break;
					case 19:
					case 20:
					case 23:
						goto IL_FA;
					case 21:
					{
						AST ast = _t;
						if (_t != ASTNULL)
						{
							StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
						}
						match(_t, 21);
						_t = _t.getFirstChild();
						a = expr(_t);
						_t = retTree_;
						_t = ast;
						_t = _t.getNextSibling();
						result = !chunk.TestAttributeTrue(a);
						goto IL_101;
					}
					default:
						switch (type)
						{
						case 31:
						case 32:
						case 33:
							break;
						default:
							goto IL_FA;
						}
						break;
					}
					break;
				}
				a = expr(_t);
				_t = retTree_;
				result = chunk.TestAttributeTrue(a);
				goto IL_101;
				IL_FA:
				throw new NoViableAltException(_t);
				IL_101:;
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return result;
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x000734E4 File Offset: 0x000716E4
		public IDictionary argList(AST _t, StringTemplate embedded, IDictionary initialContext)
		{
			IDictionary dictionary = null;
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			dictionary = initialContext;
			if (dictionary == null)
			{
				dictionary = new Hashtable();
			}
			try
			{
				if (_t == null)
				{
					_t = ASTNULL;
				}
				int type = _t.Type;
				if (type != 6)
				{
					if (type != 12)
					{
						throw new NoViableAltException(_t);
					}
					singleTemplateArg(_t, embedded, dictionary);
					_t = retTree_;
				}
				else
				{
					AST ast = _t;
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
					}
					match(_t, 6);
					_t = _t.getFirstChild();
					for (;;)
					{
						if (_t == null)
						{
							_t = ASTNULL;
						}
						if (_t.Type != 19 && _t.Type != 36)
						{
							break;
						}
						argumentAssignment(_t, embedded, dictionary);
						_t = retTree_;
					}
					_t = ast;
					_t = _t.getNextSibling();
				}
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
			return dictionary;
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x000735CC File Offset: 0x000717CC
		public void argumentAssignment(AST _t, StringTemplate embedded, IDictionary argumentContext)
		{
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			try
			{
				if (_t == null)
				{
					_t = ASTNULL;
				}
				int type = _t.Type;
				if (type != 19)
				{
					if (type != 36)
					{
						throw new NoViableAltException(_t);
					}
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
					}
					match(_t, 36);
					_t = _t.getNextSibling();
					embedded.PassThroughAttributes = true;
				}
				else
				{
					AST ast = _t;
					if (_t != ASTNULL)
					{
						StringTemplateAST stringTemplateAST3 = (StringTemplateAST)_t;
					}
					match(_t, 19);
					_t = _t.getFirstChild();
					StringTemplateAST stringTemplateAST4 = (_t == ASTNULL) ? null : ((StringTemplateAST)_t);
					match(_t, 18);
					_t = _t.getNextSibling();
					object obj = expr(_t);
					_t = retTree_;
					_t = ast;
					_t = _t.getNextSibling();
					if (obj != null)
					{
						self.RawSetArgumentAttribute(embedded, argumentContext, stringTemplateAST4.getText(), obj);
					}
				}
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x000736E0 File Offset: 0x000718E0
		public void singleTemplateArg(AST _t, StringTemplate embedded, IDictionary argumentContext)
		{
			StringTemplateAST stringTemplateAST = (StringTemplateAST)_t;
			try
			{
				AST ast = _t;
				if (_t != ASTNULL)
				{
					StringTemplateAST stringTemplateAST2 = (StringTemplateAST)_t;
				}
				match(_t, 12);
				_t = _t.getFirstChild();
				object obj = expr(_t);
				_t = retTree_;
				_t = ast;
				_t = _t.getNextSibling();
				if (obj != null)
				{
					string name = null;
					bool flag = false;
					HashList hashList = (HashList)embedded.FormalArguments;
					if (hashList != null)
					{
						ICollection keys = hashList.Keys;
						if (keys.Count == 1)
						{
							IEnumerator enumerator = keys.GetEnumerator();
							enumerator.MoveNext();
							name = (string)enumerator.Current;
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						self.Error(
                            $"template {embedded.Name} must have exactly one formal arg in template context {self.GetEnclosingInstanceStackString()}");
					}
					else
					{
						self.RawSetArgumentAttribute(embedded, argumentContext, name, obj);
					}
				}
			}
			catch (RecognitionException ex)
			{
				reportError(ex);
                _t = _t?.getNextSibling();
            }
			retTree_ = _t;
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x000737F8 File Offset: 0x000719F8
		public new StringTemplateAST getAST()
		{
			return (StringTemplateAST)returnAST;
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x00073808 File Offset: 0x00071A08
		private static long[] mk_tokenSet_0_()
		{
			long[] array = new long[2];
			array[0] = 15053630128L;
			return array;
		}

		// Token: 0x04000D18 RID: 3352
		public const int EOF = 1;

		// Token: 0x04000D19 RID: 3353
		public const int NULL_TREE_LOOKAHEAD = 3;

		// Token: 0x04000D1A RID: 3354
		public const int APPLY = 4;

		// Token: 0x04000D1B RID: 3355
		public const int MULTI_APPLY = 5;

		// Token: 0x04000D1C RID: 3356
		public const int ARGS = 6;

		// Token: 0x04000D1D RID: 3357
		public const int INCLUDE = 7;

		// Token: 0x04000D1E RID: 3358
		public const int CONDITIONAL = 8;

		// Token: 0x04000D1F RID: 3359
		public const int VALUE = 9;

		// Token: 0x04000D20 RID: 3360
		public const int TEMPLATE = 10;

		// Token: 0x04000D21 RID: 3361
		public const int FUNCTION = 11;

		// Token: 0x04000D22 RID: 3362
		public const int SINGLEVALUEARG = 12;

		// Token: 0x04000D23 RID: 3363
		public const int LIST = 13;

		// Token: 0x04000D24 RID: 3364
		public const int SEMI = 14;

		// Token: 0x04000D25 RID: 3365
		public const int LPAREN = 15;

		// Token: 0x04000D26 RID: 3366
		public const int RPAREN = 16;

		// Token: 0x04000D27 RID: 3367
		public const int COMMA = 17;

		// Token: 0x04000D28 RID: 3368
		public const int ID = 18;

		// Token: 0x04000D29 RID: 3369
		public const int ASSIGN = 19;

		// Token: 0x04000D2A RID: 3370
		public const int COLON = 20;

		// Token: 0x04000D2B RID: 3371
		public const int NOT = 21;

		// Token: 0x04000D2C RID: 3372
		public const int PLUS = 22;

		// Token: 0x04000D2D RID: 3373
		public const int LITERAL_super = 23;

		// Token: 0x04000D2E RID: 3374
		public const int DOT = 24;

		// Token: 0x04000D2F RID: 3375
		public const int LITERAL_first = 25;

		// Token: 0x04000D30 RID: 3376
		public const int LITERAL_rest = 26;

		// Token: 0x04000D31 RID: 3377
		public const int LITERAL_last = 27;

		// Token: 0x04000D32 RID: 3378
		public const int LITERAL_length = 28;

		// Token: 0x04000D33 RID: 3379
		public const int LITERAL_strip = 29;

		// Token: 0x04000D34 RID: 3380
		public const int LITERAL_trunc = 30;

		// Token: 0x04000D35 RID: 3381
		public const int ANONYMOUS_TEMPLATE = 31;

		// Token: 0x04000D36 RID: 3382
		public const int STRING = 32;

		// Token: 0x04000D37 RID: 3383
		public const int INT = 33;

		// Token: 0x04000D38 RID: 3384
		public const int LBRACK = 34;

		// Token: 0x04000D39 RID: 3385
		public const int RBRACK = 35;

		// Token: 0x04000D3A RID: 3386
		public const int DOTDOTDOT = 36;

		// Token: 0x04000D3B RID: 3387
		public const int TEMPLATE_ARGS = 37;

		// Token: 0x04000D3C RID: 3388
		public const int NESTED_ANONYMOUS_TEMPLATE = 38;

		// Token: 0x04000D3D RID: 3389
		public const int ESC_CHAR = 39;

		// Token: 0x04000D3E RID: 3390
		public const int WS = 40;

		// Token: 0x04000D3F RID: 3391
		public const int WS_CHAR = 41;

		// Token: 0x04000D40 RID: 3392
		protected StringTemplate self;

		// Token: 0x04000D41 RID: 3393
		protected IStringTemplateWriter @out;

		// Token: 0x04000D42 RID: 3394
		protected ASTExpr chunk;

		// Token: 0x04000D43 RID: 3395
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

		// Token: 0x04000D44 RID: 3396
		public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());

		// Token: 0x02000232 RID: 562
		public class NameValuePair
		{
			// Token: 0x04000D45 RID: 3397
			public string name;

			// Token: 0x04000D46 RID: 3398
			public object value;
		}
	}
}
