using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using pcomps.Antlr.collections;
using pcomps.Antlr.StringTemplate.Collections;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x0200023A RID: 570
	public class ASTExpr : Expr
	{
		// Token: 0x060010FA RID: 4346 RVA: 0x0007AA84 File Offset: 0x00078C84
		static ASTExpr()
		{
			defaultOptionValues.Add("anchor", new StringTemplateAST(32, "true"));
			defaultOptionValues.Add("wrap", new StringTemplateAST(32, "\n"));
			supportedOptions.Add("anchor", "anchor");
			supportedOptions.Add("format", "format");
			supportedOptions.Add("null", "null");
			supportedOptions.Add("separator", "separator");
			supportedOptions.Add("wrap", "wrap");
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0007AB60 File Offset: 0x00078D60
		public ASTExpr(StringTemplate enclosingTemplate, AST exprTree, IDictionary options) : base(enclosingTemplate)
		{
			this.exprTree = exprTree;
			this.options = options;
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060010FC RID: 4348 RVA: 0x0007AB78 File Offset: 0x00078D78
		public virtual AST AST
		{
			get
			{
				return exprTree;
			}
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x0007AB80 File Offset: 0x00078D80
		public override int Write(StringTemplate self, IStringTemplateWriter output)
		{
			if (exprTree == null || self == null || output == null)
			{
				return 0;
			}
			output.PushIndentation(Indentation);
			var stringTemplateAST = (StringTemplateAST)GetOption("anchor");
			if (stringTemplateAST != null)
			{
				output.PushAnchorPoint();
			}
			HandleExprOptions(self);
			var actionEvaluator = new ActionEvaluator(self, this, output);
			ActionParser.initializeASTFactory(actionEvaluator.getASTFactory());
			var result = 0;
			try
			{
				result = actionEvaluator.action(exprTree);
			}
			catch (RecognitionException e)
			{
				self.Error($"can't evaluate tree: {exprTree.ToStringList()}", e);
			}
			output.PopIndentation();
			if (stringTemplateAST != null)
			{
				output.PopAnchorPoint();
			}
			return result;
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x0007AC30 File Offset: 0x00078E30
		private void HandleExprOptions(StringTemplate self)
		{
			formatString = null;
			var stringTemplateAST = (StringTemplateAST)GetOption("wrap");
			if (stringTemplateAST != null)
			{
				wrapString = EvaluateExpression(self, stringTemplateAST);
			}
			var stringTemplateAST2 = (StringTemplateAST)GetOption("null");
			if (stringTemplateAST2 != null)
			{
				nullValue = EvaluateExpression(self, stringTemplateAST2);
			}
			var stringTemplateAST3 = (StringTemplateAST)GetOption("separator");
			if (stringTemplateAST3 != null)
			{
				separatorString = EvaluateExpression(self, stringTemplateAST3);
			}
			var stringTemplateAST4 = (StringTemplateAST)GetOption("format");
			if (stringTemplateAST4 != null)
			{
				formatString = EvaluateExpression(self, stringTemplateAST4);
			}
			if (options != null)
			{
				foreach (var obj in options.Keys)
				{
					var text = (string)obj;
					if (!supportedOptions.Contains(text))
					{
						self.Warning($"ignoring unsupported option: {text}");
					}
				}
			}
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x0007AD20 File Offset: 0x00078F20
		public virtual object ApplyTemplateToListOfAttributes(StringTemplate self, IList attributes, StringTemplate templateToApply)
		{
			if (attributes == null || templateToApply == null || attributes.Count == 0)
			{
				return null;
			}
			IList list = new StringTemplate.STAttributeList();
			for (var i = 0; i < attributes.Count; i++)
			{
				var obj = attributes[i];
				if (obj != null)
				{
					obj = ConvertAnythingToIterator(obj);
					attributes[i] = obj;
				}
			}
			var num = attributes.Count;
			var hashList = (HashList)templateToApply.FormalArguments;
			if (hashList == null || hashList.Count == 0)
			{
				self.Error(
                    $"missing arguments in anonymous template in context {self.GetEnclosingInstanceStackString()}");
				return null;
			}
			var array = new object[hashList.Count];
			hashList.Keys.CopyTo(array, 0);
			if (array.Length != num)
			{
				self.Error(
                    $"number of arguments {hashList.Keys} mismatch between attribute list and anonymous template in context {self.GetEnclosingInstanceStackString()}");
				var num2 = Math.Min(array.Length, num);
				num = num2;
				var array2 = new object[num2];
				Array.Copy(array, 0, array2, 0, num2);
				array = array2;
			}
			var num3 = 0;
			for (;;)
			{
				IDictionary dictionary = new Hashtable();
				var num4 = 0;
				for (var j = 0; j < num; j++)
				{
					var enumerator = (IEnumerator)attributes[j];
					if (enumerator != null && enumerator.MoveNext())
					{
						var key = (string)array[j];
						var value = enumerator.Current;
						dictionary[key] = value;
					}
					else
					{
						num4++;
					}
				}
				if (num4 == num)
				{
					break;
				}
				dictionary["i"] = num3 + 1;
				dictionary["i0"] = num3;
				var instanceOf = templateToApply.GetInstanceOf();
				instanceOf.EnclosingInstance = self;
				instanceOf.ArgumentContext = dictionary;
				list.Add(instanceOf);
				num3++;
			}
			return list;
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x0007AEDC File Offset: 0x000790DC
		public virtual object ApplyListOfAlternatingTemplates(StringTemplate self, object attributeValue, IList templatesToApply)
		{
			if (attributeValue == null || templatesToApply == null || templatesToApply.Count == 0)
			{
				return null;
			}
			attributeValue = ConvertAnythingIteratableToIterator(attributeValue);
			StringTemplate stringTemplate;
			IDictionary dictionary;
			bool flag;
			bool flag2;
			if (attributeValue is IEnumerator)
			{
				IList list = new StringTemplate.STAttributeList();
				var enumerator = (IEnumerator)attributeValue;
				var num = 0;
				while (enumerator.MoveNext())
				{
					var obj = enumerator.Current;
					if (obj == null)
					{
						if (nullValue == null)
						{
							continue;
						}
						obj = nullValue;
					}
					var index = num % templatesToApply.Count;
					stringTemplate = (StringTemplate)templatesToApply[index];
					var argumentsAST = stringTemplate.ArgumentsAST;
					stringTemplate = stringTemplate.GetInstanceOf();
					stringTemplate.EnclosingInstance = self;
					stringTemplate.ArgumentsAST = argumentsAST;
					dictionary = new Hashtable();
					flag = (stringTemplate.Name == "anonymous");
					var formalArguments = stringTemplate.FormalArguments;
					SetSoleFormalArgumentToIthValue(stringTemplate, dictionary, obj);
					flag2 = (formalArguments != null && formalArguments.Count > 0);
					if (!flag || !flag2)
					{
						dictionary["it"] = obj;
						dictionary["attr"] = obj;
					}
					dictionary["i"] = num + 1;
					dictionary["i0"] = num;
					stringTemplate.ArgumentContext = dictionary;
					EvaluateArguments(stringTemplate);
					list.Add(stringTemplate);
					num++;
				}
				if (list.Count == 0)
				{
					list = null;
				}
				return list;
			}
			stringTemplate = (StringTemplate)templatesToApply[0];
			dictionary = new Hashtable();
			var formalArguments2 = stringTemplate.FormalArguments;
			SetSoleFormalArgumentToIthValue(stringTemplate, dictionary, attributeValue);
			flag = (stringTemplate.Name == "anonymous");
			flag2 = (formalArguments2 != null && formalArguments2.Count > 0);
			if (!flag || !flag2)
			{
				dictionary["it"] = attributeValue;
				dictionary["attr"] = attributeValue;
			}
			dictionary["i"] = 1;
			dictionary["i0"] = 0;
			stringTemplate.ArgumentContext = dictionary;
			EvaluateArguments(stringTemplate);
			return stringTemplate;
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x0007B0D0 File Offset: 0x000792D0
		protected internal virtual void SetSoleFormalArgumentToIthValue(StringTemplate embedded, IDictionary argumentContext, object ithValue)
		{
			var formalArguments = embedded.FormalArguments;
			if (formalArguments != null)
			{
				var flag = embedded.Name == "anonymous";
				if (formalArguments.Count == 1 || (flag && formalArguments.Count > 0))
				{
					if (flag && formalArguments.Count > 1)
					{
						embedded.Error(
                            $"too many arguments on {{...}} template: {CollectionUtils.DictionaryToString(formalArguments)}");
					}
					var enumerator = formalArguments.Keys.GetEnumerator();
					enumerator.MoveNext();
					var key = (string)enumerator.Current;
					argumentContext[key] = ithValue;
				}
			}
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x0007B158 File Offset: 0x00079358
		public virtual object GetObjectProperty(StringTemplate self, object o, string propertyName)
		{
			if (o == null || propertyName == null)
			{
				return null;
			}
			totalObjPropRefs++;
			var attributeStrategy = enclosingTemplate.Group.AttributeStrategy;
			object result;
			if (attributeStrategy != null && attributeStrategy.UseCustomGetObjectProperty)
			{
				result = attributeStrategy.GetObjectProperty(self, o, propertyName);
			}
			else
			{
				result = RawGetObjectProperty(self, o, propertyName);
			}
			return result;
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x0007B1B0 File Offset: 0x000793B0
		protected object RawGetObjectProperty(StringTemplate self, object o, string propertyName)
		{
			var type = o.GetType();
			object obj = null;
			if (type == typeof(StringTemplate.Aggregate))
			{
				obj = ((StringTemplate.Aggregate)o).Get(propertyName);
				return obj;
			}
			if (type == typeof(StringTemplate))
			{
				var attributes = ((StringTemplate)o).Attributes;
				if (attributes != null)
				{
					obj = attributes[propertyName];
					return obj;
				}
			}
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				var dictionary = (IDictionary)o;
				if (propertyName.Equals("keys"))
				{
					obj = dictionary.Keys;
				}
				else if (propertyName.Equals("values"))
				{
					obj = dictionary.Values;
				}
				else if (dictionary.Contains(propertyName))
				{
					obj = dictionary[propertyName];
				}
				else if (dictionary.Contains("_default_"))
				{
					obj = dictionary["_default_"];
				}
				if (obj == MAP_KEY_VALUE)
				{
					obj = propertyName;
				}
				return obj;
			}
			var text = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
			var propertyLookupParams = new PropertyLookupParams(self, type, o, propertyName, text);
			totalReflectionLookups++;
			if (!GetPropertyValueByName(propertyLookupParams, ref obj))
			{
				var flag = false;
				foreach (var str in new string[]
				{
					"get_",
					"Get",
					"Is",
					"get",
					"is"
				})
				{
					propertyLookupParams.lookupName = str + text;
					totalReflectionLookups++;
					if (flag = GetMethodValueByName(propertyLookupParams, ref obj))
					{
						break;
					}
				}
				if (!flag)
				{
					propertyLookupParams.lookupName = text;
					totalReflectionLookups++;
					if (!GetFieldValueByName(propertyLookupParams, ref obj))
					{
						totalReflectionLookups++;
						var property = type.GetProperty("Item", new Type[]
						{
							typeof(string)
						});
						if (property != null)
						{
							try
							{
								return property.GetValue(o, new object[]
								{
									propertyName
								});
							}
							catch (Exception e)
							{
								self.Error(string.Concat(new string[]
								{
									"Can't get property ",
									propertyName,
									" via C# string indexer from ",
									type.FullName,
									" instance"
								}), e);
								return obj;
							}
						}
						self.Error(string.Concat(new string[]
						{
							"Class ",
							type.FullName,
							" has no such attribute: ",
							propertyName,
							" in template context ",
							self.GetEnclosingInstanceStackString()
						}));
					}
				}
			}
			return obj;
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x0007B46C File Offset: 0x0007966C
		public virtual bool TestAttributeTrue(object a)
		{
			var attributeStrategy = enclosingTemplate.Group.AttributeStrategy;
			if (attributeStrategy != null && attributeStrategy.UseCustomTestAttributeTrue)
			{
				return attributeStrategy.TestAttributeTrue(a);
			}
			if (a == null)
			{
				return false;
			}
			if (a is bool)
			{
				return (bool)a;
			}
			if (a is ICollection)
			{
				return ((ICollection)a).Count > 0;
			}
			if (a is IDictionary)
			{
				return ((IDictionary)a).Count > 0;
			}
			return !(a is IEnumerator) || !CollectionUtils.IsEmptyEnumerator((IEnumerator)a);
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0007B4F8 File Offset: 0x000796F8
		public virtual object Add(object a, object b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return a.ToString() + b.ToString();
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x0007B518 File Offset: 0x00079718
		public virtual StringTemplate GetTemplateInclude(StringTemplate enclosing, string templateName, StringTemplateAST argumentsAST)
		{
			var group = enclosing.Group;
			var embeddedInstanceOf = group.GetEmbeddedInstanceOf(enclosing, templateName);
			if (embeddedInstanceOf == null)
			{
				enclosing.Error($"cannot make embedded instance of {templateName} in template {enclosing.Name}");
				return null;
			}
			embeddedInstanceOf.ArgumentsAST = argumentsAST;
			EvaluateArguments(embeddedInstanceOf);
			return embeddedInstanceOf;
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x0007B568 File Offset: 0x00079768
		public virtual int WriteAttribute(StringTemplate self, object o, IStringTemplateWriter output)
		{
			return Write(self, o, output);
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x0007B574 File Offset: 0x00079774
		protected internal virtual int Write(StringTemplate self, object o, IStringTemplateWriter output)
		{
			if (o == null)
			{
				if (nullValue == null)
				{
					return 0;
				}
				o = nullValue;
			}
			var num = 0;
			try
			{
				if (o is StringTemplate)
				{
					var stringTemplate = (StringTemplate)o;
					stringTemplate.EnclosingInstance = self;
					if (StringTemplate.IsInLintMode && StringTemplate.IsRecursiveEnclosingInstance(stringTemplate))
					{
						throw new SystemException(string.Concat(new string[]
						{
							"infinite recursion to ",
							stringTemplate.GetTemplateDeclaratorString(),
							" referenced in ",
							stringTemplate.EnclosingInstance.GetTemplateDeclaratorString(),
							"; stack trace:\n",
							stringTemplate.GetEnclosingInstanceStackTrace()
						}));
					}
					if (wrapString != null)
					{
						num = output.WriteWrapSeparator(wrapString);
					}
					if (formatString != null)
					{
						var attributeRenderer = self.GetAttributeRenderer(typeof(string));
						if (attributeRenderer != null)
						{
							var stringWriter = new StringWriter();
							var output2 = self.Group.CreateInstanceOfTemplateWriter(stringWriter);
							stringTemplate.Write(output2);
							num = output.Write(attributeRenderer.ToString(stringWriter.ToString(), formatString));
							return num;
						}
					}
					num = stringTemplate.Write(output);
					return num;
				}
				else
				{
					o = ConvertAnythingIteratableToIterator(o);
					if (!(o is IEnumerator))
					{
						var attributeRenderer2 = self.GetAttributeRenderer(o.GetType());
						string str;
						if (attributeRenderer2 != null)
						{
							if (formatString != null)
							{
								str = attributeRenderer2.ToString(o, formatString);
							}
							else
							{
								str = attributeRenderer2.ToString(o);
							}
						}
						else
						{
							str = o.ToString();
						}
						if (wrapString != null)
						{
							num = output.Write(str, wrapString);
						}
						else
						{
							num = output.Write(str);
						}
						return num;
					}
					var enumerator = (IEnumerator)o;
					var flag = false;
					while (enumerator.MoveNext())
					{
						var obj = enumerator.Current;
						if (obj == null)
						{
							obj = nullValue;
						}
						if (obj != null)
						{
							if (flag && separatorString != null)
							{
								num += output.WriteSeparator(separatorString);
							}
							flag = true;
							var num2 = Write(self, obj, output);
							num += num2;
						}
					}
				}
			}
			catch (IOException e)
			{
				self.Error($"problem writing object: {o}", e);
			}
			return num;
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0007B7A8 File Offset: 0x000799A8
		protected internal string EvaluateExpression(StringTemplate self, object expr)
		{
			if (expr == null)
			{
				return null;
			}
			if (expr is StringTemplateAST)
			{
				var stringTemplateAST = (StringTemplateAST)expr;
				var stringWriter = new StringWriter();
				var @out = self.group.CreateInstanceOfTemplateWriter(stringWriter);
				var actionEvaluator = new ActionEvaluator(self, this, @out);
				try
				{
					actionEvaluator.action(stringTemplateAST);
				}
				catch (RecognitionException e)
				{
					self.Error($"can't evaluate tree: {stringTemplateAST.ToStringList()}", e);
				}
				return stringWriter.ToString();
			}
			return expr.ToString();
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x0007B828 File Offset: 0x00079A28
		protected internal virtual void EvaluateArguments(StringTemplate self)
		{
			var argumentsAST = self.ArgumentsAST;
			if (argumentsAST == null || argumentsAST.getFirstChild() == null)
			{
				return;
			}
			var enclosingInstance = self.EnclosingInstance;
			var actionEvaluator = new ActionEvaluator(new StringTemplate(self.Group, "")
			{
				Name = $"<invoke {self.Name} arg context>",
				EnclosingInstance = enclosingInstance,
				ArgumentContext = self.ArgumentContext
			}, this, null);
			ActionParser.initializeASTFactory(actionEvaluator.getASTFactory());
			try
			{
				var argumentContext = actionEvaluator.argList(argumentsAST, self, self.ArgumentContext);
				self.ArgumentContext = argumentContext;
			}
			catch (RecognitionException e)
			{
				self.Error($"can't evaluate tree: {argumentsAST.ToStringList()}", e);
			}
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0007B8E8 File Offset: 0x00079AE8
		private static object ConvertAnythingIteratableToIterator(object o)
		{
			IEnumerator enumerator = null;
			if (o is IDictionary)
			{
				enumerator = ((IDictionary)o).Values.GetEnumerator();
			}
			else if (o is ICollection)
			{
				enumerator = ((ICollection)o).GetEnumerator();
			}
			else if (o is IEnumerator)
			{
				enumerator = (IEnumerator)o;
			}
			if (enumerator == null)
			{
				return o;
			}
			return enumerator;
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0007B940 File Offset: 0x00079B40
		internal static IEnumerator ConvertAnythingToIterator(object o)
		{
			IEnumerator enumerator = null;
			if (o is IDictionary)
			{
				enumerator = ((IDictionary)o).Values.GetEnumerator();
			}
			else if (o is ICollection)
			{
				enumerator = ((ICollection)o).GetEnumerator();
			}
			else if (o is IEnumerator)
			{
				enumerator = (IEnumerator)o;
			}
			if (enumerator == null)
			{
				return ((IEnumerable)new StringTemplate.STAttributeList(1)
				{
					o
				}).GetEnumerator();
			}
			return enumerator;
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x0007B9AC File Offset: 0x00079BAC
		public virtual object First(object attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			var result = attribute;
			attribute = ConvertAnythingIteratableToIterator(attribute);
			if (attribute is IEnumerator)
			{
				var enumerator = (IEnumerator)attribute;
				if (enumerator.MoveNext())
				{
					result = enumerator.Current;
				}
			}
			return result;
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x0007B9E8 File Offset: 0x00079BE8
		public virtual object Rest(object attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			if (attribute is ICollection)
			{
				if (((ICollection)attribute).Count > 1)
				{
					return new RestCollection((ICollection)attribute);
				}
			}
			else if (attribute is IEnumerator)
			{
				return new RestCollection((IEnumerator)attribute);
			}
			return null;
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x0007BA28 File Offset: 0x00079C28
		public virtual object Last(object attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			var result = attribute;
			attribute = ConvertAnythingIteratableToIterator(attribute);
			if (attribute is IEnumerator)
			{
				var enumerator = (IEnumerator)attribute;
				while (enumerator.MoveNext())
				{
					result = enumerator.Current;
				}
			}
			return result;
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x0007BA68 File Offset: 0x00079C68
		public object Strip(object attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			attribute = ConvertAnythingIteratableToIterator(attribute);
			if (attribute is IEnumerator)
			{
				return new NullSkippingIterator((IEnumerator)attribute);
			}
			return attribute;
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x0007BA8C File Offset: 0x00079C8C
		public object Trunc(object attribute)
		{
			return null;
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x0007BA90 File Offset: 0x00079C90
		public object Length(object attribute)
		{
			if (attribute == null)
			{
				return 0;
			}
			var num = 1;
			if (attribute is ICollection)
			{
				num = ((ICollection)attribute).Count;
			}
			else if (attribute is IEnumerator)
			{
				var enumerator = (IEnumerator)attribute;
				num = 0;
				while (enumerator.MoveNext())
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x0007BAE4 File Offset: 0x00079CE4
		public object GetOption(string name)
		{
			object obj = null;
			if (options != null)
			{
				obj = options[name];
				if (obj == EMPTY_OPTION)
				{
					return defaultOptionValues[name];
				}
			}
			return obj;
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x0007BB20 File Offset: 0x00079D20
		public override string ToString()
		{
			return exprTree.ToStringList();
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x0007BB30 File Offset: 0x00079D30
		private bool GetMethodValueByName(PropertyLookupParams paramBag, ref object val)
		{
			try
			{
				var method = paramBag.prototype.GetMethod(paramBag.lookupName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
				if (method != null)
				{
					return GetMethodValue(method, paramBag, ref val);
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x0007BB84 File Offset: 0x00079D84
		private bool GetMethodValue(MethodInfo mi, PropertyLookupParams paramBag, ref object value)
		{
			try
			{
				value = mi.Invoke(paramBag.instance, null);
				return true;
			}
			catch (Exception e)
			{
				paramBag.self.Error(string.Concat(new string[]
				{
					"Can't get property ",
					paramBag.lookupName,
					" using method get_/Get/Is/get/is as ",
					mi.Name,
					" from ",
					paramBag.prototype.FullName,
					" instance"
				}), e);
			}
			return false;
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x0007BC14 File Offset: 0x00079E14
		private bool GetPropertyValueByName(PropertyLookupParams paramBag, ref object val)
		{
			try
			{
				var property = paramBag.prototype.GetProperty(paramBag.lookupName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, Type.EmptyTypes, null);
				if (property != null)
				{
					if (property.CanRead)
					{
						return GetPropertyValue(property, paramBag, ref val);
					}
					paramBag.self.Error(string.Concat(new string[]
					{
						"Class ",
						paramBag.prototype.FullName,
						" property: ",
						paramBag.lookupName,
						" is write-only in template context ",
						paramBag.self.GetEnclosingInstanceStackString()
					}));
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x0007BCC4 File Offset: 0x00079EC4
		private bool GetPropertyValue(PropertyInfo pi, PropertyLookupParams paramBag, ref object value)
		{
			try
			{
				value = pi.GetValue(paramBag.instance, null);
				return true;
			}
			catch (Exception e)
			{
				paramBag.self.Error(string.Concat(new string[]
				{
					"Can't get property ",
					paramBag.propertyName,
					" as CLR property ",
					pi.Name,
					" from ",
					paramBag.prototype.FullName,
					" instance"
				}), e);
			}
			return false;
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x0007BD54 File Offset: 0x00079F54
		private bool GetFieldValueByName(PropertyLookupParams paramBag, ref object val)
		{
			try
			{
				var field = paramBag.prototype.GetField(paramBag.lookupName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					return GetFieldValue(field, paramBag, ref val);
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x0007BDA0 File Offset: 0x00079FA0
		private bool GetFieldValue(FieldInfo fi, PropertyLookupParams paramBag, ref object value)
		{
			try
			{
				value = fi.GetValue(paramBag.instance);
				return true;
			}
			catch (Exception e)
			{
				paramBag.self.Error(string.Concat(new string[]
				{
					"Can't get property ",
					fi.Name,
					" using direct field access from ",
					paramBag.prototype.FullName,
					" instance"
				}), e);
			}
			return false;
		}

		// Token: 0x04000E2E RID: 3630
		public const string DEFAULT_ATTRIBUTE_NAME = "it";

		// Token: 0x04000E2F RID: 3631
		public const string DEFAULT_ATTRIBUTE_NAME_DEPRECATED = "attr";

		// Token: 0x04000E30 RID: 3632
		public const string DEFAULT_INDEX_VARIABLE_NAME = "i";

		// Token: 0x04000E31 RID: 3633
		public const string DEFAULT_INDEX0_VARIABLE_NAME = "i0";

		// Token: 0x04000E32 RID: 3634
		public const string DEFAULT_MAP_VALUE_NAME = "_default_";

		// Token: 0x04000E33 RID: 3635
		public const string DEFAULT_MAP_KEY_NAME = "key";

		// Token: 0x04000E34 RID: 3636
		public static readonly StringTemplate MAP_KEY_VALUE = new StringTemplate();

		// Token: 0x04000E35 RID: 3637
		public static readonly string EMPTY_OPTION = "empty expr option";

		// Token: 0x04000E36 RID: 3638
		public static readonly IDictionary defaultOptionValues = new HybridDictionary();

		// Token: 0x04000E37 RID: 3639
		public static readonly IDictionary supportedOptions = new HybridDictionary();

		// Token: 0x04000E38 RID: 3640
		public static int totalObjPropRefs = 0;

		// Token: 0x04000E39 RID: 3641
		public static int totalReflectionLookups = 0;

		// Token: 0x04000E3A RID: 3642
		protected AST exprTree;

		// Token: 0x04000E3B RID: 3643
		private IDictionary options;

		// Token: 0x04000E3C RID: 3644
		private string wrapString;

		// Token: 0x04000E3D RID: 3645
		private string nullValue;

		// Token: 0x04000E3E RID: 3646
		private string separatorString;

		// Token: 0x04000E3F RID: 3647
		private string formatString;

		// Token: 0x0200023B RID: 571
		internal sealed class PropertyLookupParams
		{
			// Token: 0x0600111B RID: 4379 RVA: 0x0007BE20 File Offset: 0x0007A020
			public PropertyLookupParams(StringTemplate s, Type p, object i, string pn, string l)
			{
				SetParams(s, p, i, pn, l);
			}

			// Token: 0x0600111C RID: 4380 RVA: 0x0007BE38 File Offset: 0x0007A038
			public void SetParams(StringTemplate s, Type p, object i, string pn, string l)
			{
				self = s;
				prototype = p;
				instance = i;
				propertyName = pn;
				lookupName = l;
			}

			// Token: 0x04000E40 RID: 3648
			public StringTemplate self;

			// Token: 0x04000E41 RID: 3649
			public Type prototype;

			// Token: 0x04000E42 RID: 3650
			public object instance;

			// Token: 0x04000E43 RID: 3651
			public string propertyName;

			// Token: 0x04000E44 RID: 3652
			public string lookupName;
		}
	}
}
