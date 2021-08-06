﻿using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using pcomps.Antlr.collections;
using pcomps.Antlr.StringTemplate.Collections;
using pcomps.Antlr.StringTemplate.Language;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000222 RID: 546
	public class StringTemplate
	{
		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000F65 RID: 3941 RVA: 0x0006EAC4 File Offset: 0x0006CCC4
		private static int GetNextTemplateCounter
		{
			get
			{
				int result;
				lock (typeof(StringTemplate))
				{
					StringTemplate.templateCounter++;
					result = StringTemplate.templateCounter;
				}
				return result;
			}
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x0006EB10 File Offset: 0x0006CD10
		public virtual StringTemplate GetInstanceOf()
		{
			StringTemplate stringTemplate = this.nativeGroup.CreateStringTemplate();
			this.Dup(this, stringTemplate);
			return stringTemplate;
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000F67 RID: 3943 RVA: 0x0006EB34 File Offset: 0x0006CD34
		// (set) Token: 0x06000F68 RID: 3944 RVA: 0x0006EB3C File Offset: 0x0006CD3C
		public virtual StringTemplate EnclosingInstance
		{
			get
			{
				return this.enclosingInstance;
			}
			set
			{
				if (this == value)
				{
					throw new ArgumentException($"cannot embed template {this.Name} in itself");
				}
				this.enclosingInstance = value;
				if (value != null)
				{
					this.enclosingInstance.AddEmbeddedInstance(this);
				}
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000F69 RID: 3945 RVA: 0x0006EB74 File Offset: 0x0006CD74
		public virtual StringTemplate OutermostEnclosingInstance
		{
			get
			{
				if (this.enclosingInstance != null)
				{
					return this.enclosingInstance.OutermostEnclosingInstance;
				}
				return this;
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000F6A RID: 3946 RVA: 0x0006EB8C File Offset: 0x0006CD8C
		// (set) Token: 0x06000F6B RID: 3947 RVA: 0x0006EB94 File Offset: 0x0006CD94
		public virtual IDictionary ArgumentContext
		{
			get
			{
				return this.argumentContext;
			}
			set
			{
				this.argumentContext = value;
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000F6C RID: 3948 RVA: 0x0006EBA0 File Offset: 0x0006CDA0
		// (set) Token: 0x06000F6D RID: 3949 RVA: 0x0006EBA8 File Offset: 0x0006CDA8
		public virtual StringTemplateAST ArgumentsAST
		{
			get
			{
				return this.argumentsAST;
			}
			set
			{
				this.argumentsAST = value;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000F6E RID: 3950 RVA: 0x0006EBB4 File Offset: 0x0006CDB4
		// (set) Token: 0x06000F6F RID: 3951 RVA: 0x0006EBBC File Offset: 0x0006CDBC
		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000F70 RID: 3952 RVA: 0x0006EBC8 File Offset: 0x0006CDC8
		public virtual string OutermostName
		{
			get
			{
				if (this.enclosingInstance != null)
				{
					return this.enclosingInstance.OutermostName;
				}
				return this.Name;
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000F71 RID: 3953 RVA: 0x0006EBE4 File Offset: 0x0006CDE4
		// (set) Token: 0x06000F72 RID: 3954 RVA: 0x0006EBEC File Offset: 0x0006CDEC
		public virtual StringTemplateGroup Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.group = value;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000F73 RID: 3955 RVA: 0x0006EBF8 File Offset: 0x0006CDF8
		// (set) Token: 0x06000F74 RID: 3956 RVA: 0x0006EC00 File Offset: 0x0006CE00
		public virtual StringTemplateGroup NativeGroup
		{
			get
			{
				return this.nativeGroup;
			}
			set
			{
				this.nativeGroup = value;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000F75 RID: 3957 RVA: 0x0006EC0C File Offset: 0x0006CE0C
		// (set) Token: 0x06000F76 RID: 3958 RVA: 0x0006EC28 File Offset: 0x0006CE28
		public int GroupFileLine
		{
			get
			{
				if (this.enclosingInstance != null)
				{
					return this.enclosingInstance.GroupFileLine;
				}
				return this.groupFileLine;
			}
			set
			{
				this.groupFileLine = value;
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000F77 RID: 3959 RVA: 0x0006EC34 File Offset: 0x0006CE34
		// (set) Token: 0x06000F78 RID: 3960 RVA: 0x0006EC3C File Offset: 0x0006CE3C
		public virtual string Template
		{
			get
			{
				return this.pattern;
			}
			set
			{
				this.pattern = value;
				this.BreakTemplateIntoChunks();
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000F79 RID: 3961 RVA: 0x0006EC4C File Offset: 0x0006CE4C
		// (set) Token: 0x06000F7A RID: 3962 RVA: 0x0006EC70 File Offset: 0x0006CE70
		public virtual IStringTemplateErrorListener ErrorListener
		{
			get
			{
				if (this.errorListener == NullErrorListener.DefaultNullListener)
				{
					return this.group.ErrorListener;
				}
				return this.errorListener;
			}
			set
			{
				if (value == null)
				{
					this.errorListener = NullErrorListener.DefaultNullListener;
					return;
				}
				this.errorListener = value;
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000F7B RID: 3963 RVA: 0x0006EC88 File Offset: 0x0006CE88
		public virtual int TemplateID
		{
			get
			{
				return this.templateID;
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000F7C RID: 3964 RVA: 0x0006EC90 File Offset: 0x0006CE90
		// (set) Token: 0x06000F7D RID: 3965 RVA: 0x0006EC98 File Offset: 0x0006CE98
		public virtual IDictionary Attributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000F7E RID: 3966 RVA: 0x0006ECA4 File Offset: 0x0006CEA4
		public virtual IList Chunks
		{
			get
			{
				return this.chunks;
			}
		}

		// Token: 0x1700023C RID: 572
		// (set) Token: 0x06000F7F RID: 3967 RVA: 0x0006ECAC File Offset: 0x0006CEAC
		public virtual bool PassThroughAttributes
		{
			set
			{
				this.passThroughAttributes = value;
			}
		}

		// Token: 0x1700023D RID: 573
		// (set) Token: 0x06000F80 RID: 3968 RVA: 0x0006ECB8 File Offset: 0x0006CEB8
		public virtual IDictionary AttributeRenderers
		{
			set
			{
				this.attributeRenderers = value;
			}
		}

		// Token: 0x1700023E RID: 574
		// (set) Token: 0x06000F81 RID: 3969 RVA: 0x0006ECC4 File Offset: 0x0006CEC4
		public static bool LintMode
		{
			set
			{
				StringTemplate.lintMode = value;
			}
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0006ECCC File Offset: 0x0006CECC
		public virtual string GetEnclosingInstanceStackTrace()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Hashtable hashtable = new Hashtable();
			for (StringTemplate stringTemplate = this; stringTemplate != null; stringTemplate = stringTemplate.enclosingInstance)
			{
				if (hashtable.Contains(stringTemplate))
				{
					stringBuilder.Append(stringTemplate.GetTemplateDeclaratorString());
					stringBuilder.Append(" (start of recursive cycle)");
					stringBuilder.Append("\n");
					stringBuilder.Append("...");
					break;
				}
				hashtable[stringTemplate] = null;
				stringBuilder.Append(stringTemplate.GetTemplateDeclaratorString());
				if (stringTemplate.attributes != null)
				{
					stringBuilder.Append(", attributes=[");
					int num = 0;
					foreach (object obj in stringTemplate.attributes.Keys)
					{
						string text = (string)obj;
						if (num > 0)
						{
							stringBuilder.Append(", ");
						}
						num++;
						stringBuilder.Append(text);
						object obj2 = stringTemplate.attributes[text];
						if (obj2 is StringTemplate)
						{
							StringTemplate stringTemplate2 = (StringTemplate)obj2;
							stringBuilder.Append("=");
							stringBuilder.Append("<");
							stringBuilder.Append(stringTemplate2.Name);
							stringBuilder.Append("()@");
							stringBuilder.Append(stringTemplate2.TemplateID.ToString());
							stringBuilder.Append(">");
						}
						else if (obj2 is IList)
						{
							stringBuilder.Append("=List[..");
							IList list = (IList)obj2;
							int num2 = 0;
							for (int i = 0; i < list.Count; i++)
							{
								object obj3 = list[i];
								if (obj3 is StringTemplate)
								{
									if (num2 > 0)
									{
										stringBuilder.Append(", ");
									}
									num2++;
									StringTemplate stringTemplate3 = (StringTemplate)obj3;
									stringBuilder.Append("<");
									stringBuilder.Append(stringTemplate3.Name);
									stringBuilder.Append("()@");
									stringBuilder.Append(stringTemplate3.TemplateID.ToString());
									stringBuilder.Append(">");
								}
							}
							stringBuilder.Append("..]");
						}
					}
					stringBuilder.Append("]");
				}
				if (stringTemplate.referencedAttributes != null)
				{
					stringBuilder.Append(", references=");
					stringBuilder.Append(CollectionUtils.ListToString(stringTemplate.referencedAttributes));
				}
				stringBuilder.Append(">\n");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x0006EF3C File Offset: 0x0006D13C
		public virtual string GetTemplateDeclaratorString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<");
			stringBuilder.Append(this.Name);
			stringBuilder.Append("(");
			stringBuilder.Append(this.formalArguments.Keys.ToString());
			stringBuilder.Append(")@");
			stringBuilder.Append(this.TemplateID.ToString());
			stringBuilder.Append(">");
			return stringBuilder.ToString();
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x0006EFC0 File Offset: 0x0006D1C0
		protected string GetTemplateHeaderString(bool showAttributes)
		{
			if (showAttributes)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.Name);
				if (this.attributes != null)
				{
					stringBuilder.Append("[");
					bool flag = false;
					foreach (object obj in this.attributes.Keys)
					{
						string value = (string)obj;
						if (flag)
						{
							stringBuilder.Append(", ");
						}
						flag = true;
						stringBuilder.Append(value);
					}
					stringBuilder.Append("]");
				}
				return stringBuilder.ToString();
			}
			return this.Name;
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x0006F07C File Offset: 0x0006D27C
		public virtual string GetEnclosingInstanceStackString()
		{
			IList list = new ArrayList();
			for (StringTemplate stringTemplate = this; stringTemplate != null; stringTemplate = stringTemplate.enclosingInstance)
			{
				string str = stringTemplate.Name;
				list.Insert(0, str + (stringTemplate.passThroughAttributes ? "(...)" : ""));
			}
			return CollectionUtils.ListToString(list).Replace(",", "");
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0006F0DC File Offset: 0x0006D2DC
		// (set) Token: 0x06000F87 RID: 3975 RVA: 0x0006F0E4 File Offset: 0x0006D2E4
		public bool IsRegion
		{
			get
			{
				return this.isRegion;
			}
			set
			{
				this.isRegion = value;
			}
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x0006F0F0 File Offset: 0x0006D2F0
		public void AddRegionName(string name)
		{
			if (this.regions == null)
			{
				this.regions = new Hashtable();
			}
			this.regions.Add(name, name);
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x0006F114 File Offset: 0x0006D314
		public bool ContainsRegionName(string name)
		{
			return this.regions != null && this.regions.Contains(name);
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000F8A RID: 3978 RVA: 0x0006F12C File Offset: 0x0006D32C
		// (set) Token: 0x06000F8B RID: 3979 RVA: 0x0006F134 File Offset: 0x0006D334
		public int RegionDefType
		{
			get
			{
				return this.regionDefType;
			}
			set
			{
				this.regionDefType = value;
			}
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x0006F140 File Offset: 0x0006D340
		public static void resetTemplateCounter()
		{
			StringTemplate.templateCounter = 0;
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0006F148 File Offset: 0x0006D348
		public StringTemplate()
		{
			this.group = StringTemplate.defaultGroup;
			this.nativeGroup = StringTemplate.defaultGroup;
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x0006F1A0 File Offset: 0x0006D3A0
		public StringTemplate(string template) : this(null, template)
		{
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x0006F1AC File Offset: 0x0006D3AC
		public StringTemplate(string template, Type lexer) : this()
		{
			this.Group = new StringTemplateGroup("defaultGroup", lexer);
			this.NativeGroup = this.Group;
			this.Template = template;
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x0006F1D8 File Offset: 0x0006D3D8
		public StringTemplate(StringTemplateGroup group, string template) : this()
		{
			if (group != null)
			{
				this.Group = group;
				this.NativeGroup = group;
			}
			this.Template = template;
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0006F1F8 File Offset: 0x0006D3F8
		public StringTemplate(StringTemplateGroup group, string template, Hashtable attributes) : this(group, template)
		{
			this.Attributes = attributes;
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x0006F20C File Offset: 0x0006D40C
		protected internal virtual void Dup(StringTemplate from, StringTemplate to)
		{
			to.attributeRenderers = from.attributeRenderers;
			to.pattern = from.pattern;
			to.chunks = from.chunks;
			to.formalArguments = from.formalArguments;
			to.numberOfDefaultArgumentValues = from.numberOfDefaultArgumentValues;
			to.name = from.name;
			to.group = from.group;
			to.nativeGroup = from.nativeGroup;
			to.errorListener = from.errorListener;
			to.regions = from.regions;
			to.isRegion = from.isRegion;
			to.regionDefType = from.regionDefType;
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x0006F2AC File Offset: 0x0006D4AC
		public virtual void AddEmbeddedInstance(StringTemplate embeddedInstance)
		{
			if (this.embeddedInstances == null)
			{
				this.embeddedInstances = new ArrayList();
			}
			this.embeddedInstances.Add(embeddedInstance);
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x0006F2D0 File Offset: 0x0006D4D0
		public virtual void Reset()
		{
			this.attributes = new Hashtable();
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x0006F2E0 File Offset: 0x0006D4E0
		public virtual void SetPredefinedAttributes()
		{
			if (!StringTemplate.IsInLintMode)
			{
			}
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0006F2EC File Offset: 0x0006D4EC
		public virtual void RemoveAttribute(string name)
		{
			if (this.attributes != null)
			{
				this.attributes.Remove(name);
			}
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x0006F304 File Offset: 0x0006D504
		public virtual void SetAttribute(string name, object val)
		{
			if (name == null || val == null)
			{
				return;
			}
			if (name.IndexOf('.') != -1)
			{
				throw new ArgumentException("cannot have '.' in attribute names", "name");
			}
			if (this.attributes == null)
			{
				this.attributes = new Hashtable();
			}
			if (val is StringTemplate)
			{
				((StringTemplate)val).EnclosingInstance = this;
			}
			object obj = this.attributes[name];
			if (obj != null)
			{
				StringTemplate.STAttributeList stattributeList;
				if (obj is StringTemplate.STAttributeList)
				{
					stattributeList = (StringTemplate.STAttributeList)obj;
				}
				else if (obj is IList)
				{
					stattributeList = new StringTemplate.STAttributeList((ICollection)obj);
					this.RawSetAttribute(this.attributes, name, stattributeList);
				}
				else
				{
					stattributeList = new StringTemplate.STAttributeList();
					this.RawSetAttribute(this.attributes, name, stattributeList);
					stattributeList.Add(obj);
				}
				if (!(val is IList))
				{
					stattributeList.Add(val);
					return;
				}
				if (stattributeList != val)
				{
					stattributeList.AddRange((ICollection)val);
					return;
				}
			}
			else
			{
				this.RawSetAttribute(this.attributes, name, val);
			}
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0006F3F0 File Offset: 0x0006D5F0
		public virtual void SetAttribute(string aggrSpec, params object[] values)
		{
			IList list = new ArrayList();
			if (aggrSpec.IndexOf(".{") == -1)
			{
				this.SetAttribute(aggrSpec, values);
				return;
			}
			string text = this.ParseAggregateAttributeSpec(aggrSpec, list);
			if (values == null || list.Count == 0)
			{
				throw new ArgumentException($"missing properties or values for '{aggrSpec}'");
			}
			if (values.Length != list.Count)
			{
				throw new ArgumentException($"number of properties in '{aggrSpec}' != number of values");
			}
			StringTemplate.Aggregate aggregate = new StringTemplate.Aggregate();
			for (int i = 0; i < values.Length; i++)
			{
				object obj = values[i];
				if (obj is StringTemplate)
				{
					((StringTemplate)obj).EnclosingInstance = this;
				}
				aggregate.Put((string)list[i], obj);
			}
			this.SetAttribute(text, aggregate);
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x0006F4B0 File Offset: 0x0006D6B0
		protected virtual string ParseAggregateAttributeSpec(string aggrSpec, IList properties)
		{
			int num = aggrSpec.IndexOf('.');
			if (num <= 0)
			{
				throw new ArgumentException($"invalid aggregate attribute format: {aggrSpec}");
			}
			string result = aggrSpec.Substring(0, num);
			string text = aggrSpec.Substring(num + 2, aggrSpec.Length - (num + 3));
			string[] array = text.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				properties.Add(array[i].Trim());
			}
			return result;
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x0006F534 File Offset: 0x0006D734
		protected internal virtual void RawSetAttribute(IDictionary attributes, string name, object objValue)
		{
			if (this.formalArguments != FormalArgument.UNKNOWN && this.GetFormalArgument(name) == null)
			{
				throw new InvalidOperationException(
                    $"no such attribute: {name} in template context {this.GetEnclosingInstanceStackString()}");
			}
			if (objValue == null)
			{
				return;
			}
			attributes[name] = objValue;
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x0006F574 File Offset: 0x0006D774
		protected internal virtual void RawSetArgumentAttribute(StringTemplate embedded, IDictionary attributes, string name, object objValue)
		{
			if (embedded.formalArguments != FormalArgument.UNKNOWN && embedded.GetFormalArgument(name) == null)
			{
				throw new InvalidOperationException(string.Concat(new string[]
				{
					"template ",
					embedded.Name,
					" has no such attribute: ",
					name,
					" in template context ",
					this.GetEnclosingInstanceStackString()
				}));
			}
			if (objValue == null)
			{
				return;
			}
			attributes[name] = objValue;
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0006F5E8 File Offset: 0x0006D7E8
		public virtual object GetAttribute(string name)
		{
			return this.Get(this, name);
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x0006F5F4 File Offset: 0x0006D7F4
		public virtual int Write(IStringTemplateWriter output)
		{
			if (this.group.debugTemplateOutput)
			{
				this.group.EmitTemplateStartDebugString(this, output);
			}
			int num = 0;
			this.SetPredefinedAttributes();
			this.SetDefaultArgumentValues();
			int num2 = 0;
			while (this.chunks != null && num2 < this.chunks.Count)
			{
				Expr expr = (Expr)this.chunks[num2];
				int num3 = expr.Write(this, output);
				if (num3 == 0 && num2 == 0 && num2 + 1 < this.chunks.Count && this.chunks[num2 + 1] is NewlineRef)
				{
					num2++;
				}
				else
				{
					if (num3 == 0 && num2 - 1 >= 0 && this.chunks[num2 - 1] is NewlineRef && num2 + 1 < this.chunks.Count && this.chunks[num2 + 1] is NewlineRef)
					{
						num2++;
					}
					num += num3;
				}
				num2++;
			}
			if (this.group.debugTemplateOutput)
			{
				this.group.EmitTemplateStopDebugString(this, output);
			}
			if (StringTemplate.lintMode)
			{
				this.CheckForTrouble();
			}
			return num;
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x0006F70C File Offset: 0x0006D90C
		public virtual object Get(StringTemplate self, string attribute)
		{
			if (self == null)
			{
				return null;
			}
			if (StringTemplate.lintMode)
			{
				self.TrackAttributeReference(attribute);
			}
			object obj = null;
			if (self.attributes != null)
			{
				obj = self.attributes[attribute];
			}
			if (obj == null)
			{
				IDictionary dictionary = self.ArgumentContext;
				if (dictionary != null)
				{
					obj = dictionary[attribute];
				}
			}
			if (obj == null && !self.passThroughAttributes && self.GetFormalArgument(attribute) != null)
			{
				return null;
			}
			if (obj == null && self.enclosingInstance != null)
			{
				object obj2 = this.Get(self.enclosingInstance, attribute);
				if (obj2 == null)
				{
					this.CheckNullAttributeAgainstFormalArguments(self, attribute);
				}
				obj = obj2;
			}
			else if (obj == null && self.enclosingInstance == null)
			{
				obj = self.group.GetMap(attribute);
			}
			return obj;
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0006F7B0 File Offset: 0x0006D9B0
		protected internal virtual void BreakTemplateIntoChunks()
		{
			if (this.pattern == null)
			{
				return;
			}
			try
			{
				Type templateLexerClass = this.group.TemplateLexerClass;
				ConstructorInfo constructor = templateLexerClass.GetConstructor(new Type[]
				{
					typeof(StringTemplate),
					typeof(TextReader)
				});
				CharScanner charScanner = (CharScanner)constructor.Invoke(new object[]
				{
					this,
					new StringReader(this.pattern)
				});
				charScanner.setTokenCreator(ChunkToken.Creator);
				TemplateParser templateParser = new TemplateParser(charScanner);
				templateParser.template(this);
			}
			catch (Exception e)
			{
				string text = "<unknown>";
				string outermostName = this.OutermostName;
				if (this.Name != null)
				{
					text = this.Name;
				}
				if (outermostName != null && !text.Equals(outermostName))
				{
					text = $"{text} nested in {outermostName}";
				}
				this.Error($"problem parsing template '{text}'", e);
			}
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x0006F8B0 File Offset: 0x0006DAB0
		public virtual ASTExpr ParseAction(string action)
		{
			ActionLexer actionLexer = new ActionLexer(new StringReader(action.ToString()));
			actionLexer.setTokenCreator(StringTemplateToken.Creator);
			ActionParser actionParser = new ActionParser(actionLexer, this);
			actionParser.getASTFactory().setASTNodeCreator(StringTemplateAST.Creator);
			ASTExpr result = null;
			try
			{
				IDictionary options = actionParser.action();
				AST ast = actionParser.getAST();
				if (ast != null)
				{
					if (ast.Type == 8)
					{
						result = new ConditionalExpr(this, ast);
					}
					else
					{
						result = new ASTExpr(this, ast, options);
					}
				}
			}
			catch (RecognitionException e)
			{
				this.Error($"Can't parse chunk: {action}", e);
			}
			catch (TokenStreamException e2)
			{
				this.Error($"Can't parse chunk: {action}", e2);
			}
			return result;
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x0006F97C File Offset: 0x0006DB7C
		protected internal virtual void AddChunk(Expr e)
		{
			if (this.chunks == null)
			{
				this.chunks = new ArrayList();
			}
			this.chunks.Add(e);
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x0006F9A0 File Offset: 0x0006DBA0
		// (set) Token: 0x06000FA3 RID: 4003 RVA: 0x0006F9A8 File Offset: 0x0006DBA8
		public virtual IDictionary FormalArguments
		{
			get
			{
				return this.formalArguments;
			}
			set
			{
				this.formalArguments = value;
			}
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x0006F9B4 File Offset: 0x0006DBB4
		public virtual void SetDefaultArgumentValues()
		{
			if (this.numberOfDefaultArgumentValues == 0)
			{
				return;
			}
			if (this.argumentContext == null)
			{
				this.argumentContext = new Hashtable();
			}
			if (this.formalArguments != FormalArgument.UNKNOWN)
			{
				ICollection keys = this.formalArguments.Keys;
				foreach (object obj in keys)
				{
					string key = (string)obj;
					FormalArgument formalArgument = (FormalArgument)this.formalArguments[key];
					if (formalArgument.defaultValueST != null && this.GetAttribute(key) == null)
					{
						this.argumentContext[key] = formalArgument.defaultValueST;
					}
				}
			}
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x0006FA4C File Offset: 0x0006DC4C
		public virtual FormalArgument LookupFormalArgument(string name)
		{
			FormalArgument formalArgument = this.GetFormalArgument(name);
			if (formalArgument == null && this.enclosingInstance != null)
			{
				formalArgument = this.enclosingInstance.LookupFormalArgument(name);
			}
			return formalArgument;
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0006FA7C File Offset: 0x0006DC7C
		public virtual FormalArgument GetFormalArgument(string name)
		{
			return (FormalArgument)this.formalArguments[name];
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0006FA90 File Offset: 0x0006DC90
		public virtual void DefineEmptyFormalArgumentList()
		{
			this.FormalArguments = new HashList();
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0006FAA0 File Offset: 0x0006DCA0
		public virtual void DefineFormalArgument(string name)
		{
			this.DefineFormalArgument(name, null);
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0006FAAC File Offset: 0x0006DCAC
		public virtual void DefineFormalArguments(IList names)
		{
			if (names == null)
			{
				return;
			}
			for (int i = 0; i < names.Count; i++)
			{
				string text = (string)names[i];
				this.DefineFormalArgument(text);
			}
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0006FAE4 File Offset: 0x0006DCE4
		public virtual void DefineFormalArgument(string name, StringTemplate defaultValue)
		{
			if (defaultValue != null)
			{
				this.numberOfDefaultArgumentValues++;
			}
			FormalArgument value = new FormalArgument(name, defaultValue);
			if (this.formalArguments == FormalArgument.UNKNOWN)
			{
				this.formalArguments = new HashList();
			}
			this.formalArguments[name] = value;
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0006FB30 File Offset: 0x0006DD30
		public virtual void RegisterAttributeRenderer(Type attributeClassType, IAttributeRenderer renderer)
		{
			if (this.attributeRenderers == null)
			{
				this.attributeRenderers = new Hashtable();
			}
			this.attributeRenderers[attributeClassType] = renderer;
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x0006FB54 File Offset: 0x0006DD54
		public virtual IAttributeRenderer GetAttributeRenderer(Type attributeClassType)
		{
			IAttributeRenderer attributeRenderer = null;
			if (this.attributeRenderers != null)
			{
				attributeRenderer = (IAttributeRenderer)this.attributeRenderers[attributeClassType];
			}
			if (attributeRenderer == null)
			{
				if (this.enclosingInstance != null)
				{
					attributeRenderer = this.enclosingInstance.GetAttributeRenderer(attributeClassType);
				}
				else
				{
					attributeRenderer = this.group.GetAttributeRenderer(attributeClassType);
				}
			}
			return attributeRenderer;
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0006FBA8 File Offset: 0x0006DDA8
		public virtual void Error(string msg)
		{
			this.Error(msg, null);
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0006FBB4 File Offset: 0x0006DDB4
		public virtual void Warning(string msg)
		{
			this.ErrorListener.Warning(msg);
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0006FBC4 File Offset: 0x0006DDC4
		public virtual void Error(string msg, Exception e)
		{
			this.ErrorListener.Error(msg, e);
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x0006FBD4 File Offset: 0x0006DDD4
		public static bool IsInLintMode
		{
			get
			{
				return StringTemplate.lintMode;
			}
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x0006FBDC File Offset: 0x0006DDDC
		protected internal virtual void TrackAttributeReference(string name)
		{
			if (this.referencedAttributes == null)
			{
				this.referencedAttributes = new ArrayList();
			}
			this.referencedAttributes.Add(name);
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x0006FC00 File Offset: 0x0006DE00
		public static bool IsRecursiveEnclosingInstance(StringTemplate st)
		{
			if (st == null)
			{
				return false;
			}
			StringTemplate stringTemplate = st.enclosingInstance;
			if (stringTemplate == st)
			{
				return true;
			}
			while (stringTemplate != null)
			{
				if (stringTemplate == st)
				{
					return true;
				}
				stringTemplate = stringTemplate.enclosingInstance;
			}
			return false;
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x0006FC30 File Offset: 0x0006DE30
		protected internal virtual void CheckNullAttributeAgainstFormalArguments(StringTemplate self, string attribute)
		{
			if (self.FormalArguments == FormalArgument.UNKNOWN)
			{
				if (self.enclosingInstance != null)
				{
					this.CheckNullAttributeAgainstFormalArguments(self.enclosingInstance, attribute);
				}
				return;
			}
			if (self.LookupFormalArgument(attribute) == null)
			{
				throw new InvalidOperationException(
                    $"no such attribute: {attribute} in template context {this.GetEnclosingInstanceStackString()}");
			}
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x0006FC88 File Offset: 0x0006DE88
		protected internal virtual void CheckForTrouble()
		{
			if (this.attributes == null)
			{
				return;
			}
			foreach (object obj in this.attributes.Keys)
			{
				string text = (string)obj;
				if (this.referencedAttributes != null && !this.referencedAttributes.Contains(text))
				{
					this.Warning($"{this.Name}: set but not used: {text}");
				}
			}
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x0006FD18 File Offset: 0x0006DF18
		public virtual string ToDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"template-{this.GetTemplateDeclaratorString()}:");
			stringBuilder.Append("chunks=");
			if (this.chunks != null)
			{
				stringBuilder.Append(CollectionUtils.ListToString(this.chunks));
			}
			stringBuilder.Append("attributes=[");
			if (this.attributes != null)
			{
				int num = 0;
				foreach (object obj in this.attributes.Keys)
				{
					string text = (string)obj;
					if (num > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append($"{text}=");
					object obj2 = this.attributes[text];
					if (obj2 is StringTemplate)
					{
						stringBuilder.Append(((StringTemplate)obj2).ToDebugString());
					}
					else
					{
						stringBuilder.Append(obj2);
					}
					num++;
				}
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x0006FE40 File Offset: 0x0006E040
		public string ToStructureString()
		{
			return this.ToStructureString(0);
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x0006FE4C File Offset: 0x0006E04C
		public string ToStructureString(int indent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 1; i <= indent; i++)
			{
				stringBuilder.Append("  ");
			}
			stringBuilder.Append(this.Name);
			if (this.attributes != null)
			{
				stringBuilder.Append("[");
				bool flag = false;
				foreach (object obj in this.attributes.Keys)
				{
					string value = (string)obj;
					if (flag)
					{
						stringBuilder.Append(", ");
					}
					flag = true;
					stringBuilder.Append(value);
				}
				stringBuilder.Append("]");
			}
			stringBuilder.Append(":\n");
			if (this.attributes != null)
			{
				foreach (object obj2 in this.attributes.Keys)
				{
					string key = (string)obj2;
					object obj3 = this.attributes[key];
					if (obj3 is StringTemplate)
					{
						stringBuilder.Append(((StringTemplate)obj3).ToStructureString(indent + 1));
					}
					else if (obj3 is IList)
					{
						IList list = (IList)obj3;
						for (int j = 0; j < list.Count; j++)
						{
							object obj4 = list[j];
							if (obj4 is StringTemplate)
							{
								stringBuilder.Append(((StringTemplate)obj4).ToStructureString(indent + 1));
							}
						}
					}
					else if (obj3 is IDictionary)
					{
						IDictionary dictionary = (IDictionary)obj3;
						ICollection values = dictionary.Values;
						foreach (object obj5 in values)
						{
							if (obj5 is StringTemplate)
							{
								stringBuilder.Append(((StringTemplate)obj5).ToStructureString(indent + 1));
							}
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x0007003C File Offset: 0x0006E23C
		public StringTemplate GetDOTForDependencyGraph(bool showAttributes)
		{
			string template = "digraph StringTemplateDependencyGraph {\nnode [shape=$shape$, $if(width)$width=$width$,$endif$      $if(height)$height=$height$,$endif$ fontsize=$fontsize$];\n$edges:{e|\"$e.src$\" -> \"$e.trg$\"\n}$}\n";
			StringTemplate stringTemplate = new StringTemplate(template);
			HashList hashList = new HashList();
			this.GetDependencyGraph(hashList, showAttributes);
			foreach (object obj in hashList.Keys)
			{
				string text = (string)obj;
				IList list = (IList)hashList[text];
				foreach (object obj2 in list)
				{
					string text2 = (string)obj2;
					stringTemplate.SetAttribute("edges.{src,trg}", new object[]
					{
						text,
						text2
					});
				}
			}
			stringTemplate.SetAttribute("shape", "none");
			stringTemplate.SetAttribute("fontsize", "11");
			stringTemplate.SetAttribute("height", "0");
			return stringTemplate;
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x00070154 File Offset: 0x0006E354
		public void GetDependencyGraph(IDictionary edges, bool showAttributes)
		{
			string templateHeaderString = this.GetTemplateHeaderString(showAttributes);
			if (this.attributes != null)
			{
				foreach (object obj in this.attributes.Keys)
				{
					string key = (string)obj;
					object obj2 = this.attributes[key];
					if (obj2 is StringTemplate)
					{
						string templateHeaderString2 = ((StringTemplate)obj2).GetTemplateHeaderString(showAttributes);
						this.PutToMultiValuedMap(edges, templateHeaderString, templateHeaderString2);
						((StringTemplate)obj2).GetDependencyGraph(edges, showAttributes);
					}
					else if (obj2 is IList)
					{
						IList list = (IList)obj2;
						for (int i = 0; i < list.Count; i++)
						{
							object obj3 = list[i];
							if (obj3 is StringTemplate)
							{
								string templateHeaderString3 = ((StringTemplate)obj3).GetTemplateHeaderString(showAttributes);
								this.PutToMultiValuedMap(edges, templateHeaderString, templateHeaderString3);
								((StringTemplate)obj3).GetDependencyGraph(edges, showAttributes);
							}
						}
					}
					else if (obj2 is IDictionary)
					{
						IDictionary dictionary = (IDictionary)obj2;
						ICollection values = dictionary.Values;
						foreach (object obj4 in values)
						{
							if (obj4 is StringTemplate)
							{
								string templateHeaderString4 = ((StringTemplate)obj4).GetTemplateHeaderString(showAttributes);
								this.PutToMultiValuedMap(edges, templateHeaderString, templateHeaderString4);
								((StringTemplate)obj4).GetDependencyGraph(edges, showAttributes);
							}
						}
					}
				}
			}
			int num = 0;
			while (this.chunks != null && num < this.chunks.Count)
			{
				Expr expr = (Expr)this.chunks[num];
				if (expr is ASTExpr)
				{
					ASTExpr astexpr = (ASTExpr)expr;
					AST ast = astexpr.AST;
					AST subtree = new CommonAST(new CommonToken(7, "include"));
					IEnumerator enumerator3 = ast.findAllPartial(subtree);
					while (enumerator3.MoveNext())
					{
						object obj5 = enumerator3.Current;
						AST ast2 = (AST)obj5;
						string text = ast2.getFirstChild().getText();
						Console.Out.WriteLine($"found include {text}");
						this.PutToMultiValuedMap(edges, templateHeaderString, text);
						StringTemplateGroup stringTemplateGroup = this.Group;
						if (stringTemplateGroup != null)
						{
							StringTemplate instanceOf = stringTemplateGroup.GetInstanceOf(text);
							instanceOf.GetDependencyGraph(edges, showAttributes);
						}
					}
				}
				num++;
			}
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x000703BC File Offset: 0x0006E5BC
		protected void PutToMultiValuedMap(IDictionary map, object key, object val)
		{
			ArrayList arrayList = (ArrayList)map[key];
			if (arrayList == null)
			{
				arrayList = new ArrayList();
				map[key] = arrayList;
			}
			if (!arrayList.Contains(val))
			{
				arrayList.Add(val);
			}
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x000703F8 File Offset: 0x0006E5F8
		public virtual void PrintDebugString()
		{
			Console.Out.WriteLine($"template-{this.Name}:");
			Console.Out.Write("chunks=");
			Console.Out.WriteLine(CollectionUtils.ListToString(this.chunks));
			if (this.attributes == null)
			{
				return;
			}
			Console.Out.Write("attributes=[");
			int num = 0;
			foreach (object obj in this.attributes.Keys)
			{
				string text = (string)obj;
				if (num > 0)
				{
					Console.Out.Write(',');
				}
				object obj2 = this.attributes[text];
				if (obj2 is StringTemplate)
				{
					Console.Out.Write($"{text}=");
					((StringTemplate)obj2).PrintDebugString();
				}
				else if (obj2 is IList)
				{
					ArrayList arrayList = (ArrayList)obj2;
					for (int i = 0; i < arrayList.Count; i++)
					{
						object obj3 = arrayList[i];
						Console.Out.Write(string.Concat(new object[]
						{
							text,
							"[",
							i,
							"] is ",
							obj3.GetType().FullName,
							"="
						}));
						if (obj3 is StringTemplate)
						{
							((StringTemplate)obj3).PrintDebugString();
						}
						else
						{
							Console.Out.WriteLine(obj3);
						}
					}
				}
				else
				{
					Console.Out.Write($"{text}=");
					Console.Out.WriteLine(obj2);
				}
				num++;
			}
			Console.Out.Write("]\n");
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x000705F4 File Offset: 0x0006E7F4
		public override string ToString()
		{
			return this.ToString(-1);
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x00070600 File Offset: 0x0006E800
		public virtual string ToString(int lineWidth)
		{
			StringWriter stringWriter = new StringWriter();
			IStringTemplateWriter stringTemplateWriter = this.group.CreateInstanceOfTemplateWriter(stringWriter);
			stringTemplateWriter.LineWidth = lineWidth;
			try
			{
				this.Write(stringTemplateWriter);
			}
			catch (IOException)
			{
				this.Error($"Got IOException writing to writer {stringTemplateWriter.GetType().FullName}");
			}
			stringTemplateWriter.LineWidth = -1;
			return stringWriter.ToString();
		}

		// Token: 0x04000CCD RID: 3277
		public const string VERSION = "3.1b1";

		// Token: 0x04000CCE RID: 3278
		internal const int REGION_IMPLICIT = 1;

		// Token: 0x04000CCF RID: 3279
		internal const int REGION_EMBEDDED = 2;

		// Token: 0x04000CD0 RID: 3280
		internal const int REGION_EXPLICIT = 3;

		// Token: 0x04000CD1 RID: 3281
		public const string ANONYMOUS_ST_NAME = "anonymous";

		// Token: 0x04000CD2 RID: 3282
		internal static bool lintMode = false;

		// Token: 0x04000CD3 RID: 3283
		protected internal IList referencedAttributes;

		// Token: 0x04000CD4 RID: 3284
		protected internal string name = "anonymous";

		// Token: 0x04000CD5 RID: 3285
		private static int templateCounter = 0;

		// Token: 0x04000CD6 RID: 3286
		protected internal int templateID = StringTemplate.GetNextTemplateCounter;

		// Token: 0x04000CD7 RID: 3287
		protected internal StringTemplate enclosingInstance;

		// Token: 0x04000CD8 RID: 3288
		protected internal IList embeddedInstances;

		// Token: 0x04000CD9 RID: 3289
		protected internal IDictionary argumentContext;

		// Token: 0x04000CDA RID: 3290
		protected internal StringTemplateAST argumentsAST;

		// Token: 0x04000CDB RID: 3291
		protected internal IDictionary formalArguments = FormalArgument.UNKNOWN;

		// Token: 0x04000CDC RID: 3292
		protected internal int numberOfDefaultArgumentValues;

		// Token: 0x04000CDD RID: 3293
		protected internal bool passThroughAttributes;

		// Token: 0x04000CDE RID: 3294
		protected StringTemplateGroup nativeGroup;

		// Token: 0x04000CDF RID: 3295
		protected internal StringTemplateGroup group;

		// Token: 0x04000CE0 RID: 3296
		protected int groupFileLine;

		// Token: 0x04000CE1 RID: 3297
		protected IStringTemplateErrorListener errorListener = NullErrorListener.DefaultNullListener;

		// Token: 0x04000CE2 RID: 3298
		protected internal string pattern;

		// Token: 0x04000CE3 RID: 3299
		protected internal IDictionary attributes;

		// Token: 0x04000CE4 RID: 3300
		protected internal IDictionary attributeRenderers;

		// Token: 0x04000CE5 RID: 3301
		protected internal IList chunks;

		// Token: 0x04000CE6 RID: 3302
		protected int regionDefType;

		// Token: 0x04000CE7 RID: 3303
		protected bool isRegion;

		// Token: 0x04000CE8 RID: 3304
		protected IDictionary regions;

		// Token: 0x04000CE9 RID: 3305
		protected internal static StringTemplateGroup defaultGroup = new StringTemplateGroup("defaultGroup", ".");

		// Token: 0x02000223 RID: 547
		public sealed class Aggregate
		{
			// Token: 0x06000FBF RID: 4031 RVA: 0x00070690 File Offset: 0x0006E890
			internal void Put(string propName, object propValue)
			{
				this.properties[propName] = propValue;
			}

			// Token: 0x06000FC0 RID: 4032 RVA: 0x000706A0 File Offset: 0x0006E8A0
			public object Get(string propName)
			{
				object result;
				try
				{
					result = this.properties[propName];
				}
				catch
				{
					result = null;
				}
				return result;
			}

			// Token: 0x06000FC1 RID: 4033 RVA: 0x000706D4 File Offset: 0x0006E8D4
			public override string ToString()
			{
				return CollectionUtils.DictionaryToString(this.properties);
			}

			// Token: 0x04000CEA RID: 3306
			internal Hashtable properties = new Hashtable();
		}

		// Token: 0x02000224 RID: 548
		internal class STAttributeList : ArrayList
		{
			// Token: 0x06000FC3 RID: 4035 RVA: 0x000706F8 File Offset: 0x0006E8F8
			public STAttributeList()
			{
			}

			// Token: 0x06000FC4 RID: 4036 RVA: 0x00070700 File Offset: 0x0006E900
			public STAttributeList(int capacity) : base(capacity)
			{
			}

			// Token: 0x06000FC5 RID: 4037 RVA: 0x0007070C File Offset: 0x0006E90C
			public STAttributeList(ICollection collection) : base(collection)
			{
			}
		}
	}
}
