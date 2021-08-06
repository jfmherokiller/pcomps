using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using pcomps.Antlr.StringTemplate.Collections;
using pcomps.Antlr.StringTemplate.Language;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000226 RID: 550
	public class StringTemplateGroup
	{
		// Token: 0x06000FCD RID: 4045 RVA: 0x00070788 File Offset: 0x0006E988
		public StringTemplateGroup(string name) : this(name, new FileSystemTemplateLoader(string.Empty, false), null, DEFAULT_ERROR_LISTENER, null)
		{
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x000707A4 File Offset: 0x0006E9A4
		public StringTemplateGroup(string name, Type lexer) : this(name, new FileSystemTemplateLoader(string.Empty, false), lexer, DEFAULT_ERROR_LISTENER, null)
		{
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x000707C0 File Offset: 0x0006E9C0
		public StringTemplateGroup(string name, string rootDir) : this(name, new FileSystemTemplateLoader(rootDir, false), typeof(DefaultTemplateLexer), DEFAULT_ERROR_LISTENER, null)
		{
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x000707E0 File Offset: 0x0006E9E0
		public StringTemplateGroup(string name, string rootDir, Type lexer) : this(name, new FileSystemTemplateLoader(rootDir, false), lexer, DEFAULT_ERROR_LISTENER, null)
		{
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x000707F8 File Offset: 0x0006E9F8
		public StringTemplateGroup(string name, StringTemplateLoader templateLoader) : this(name, templateLoader, typeof(DefaultTemplateLexer), DEFAULT_ERROR_LISTENER, null)
		{
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x00070814 File Offset: 0x0006EA14
		public StringTemplateGroup(string name, StringTemplateLoader templateLoader, Type lexer) : this(name, templateLoader, lexer, DEFAULT_ERROR_LISTENER, null)
		{
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x00070828 File Offset: 0x0006EA28
		public StringTemplateGroup(string name, StringTemplateLoader templateLoader, Type lexer, IStringTemplateErrorListener errorListener, StringTemplateGroup superGroup)
		{
			templates = new HashList();
			maps = new Hashtable();
			this.errorListener = DEFAULT_ERROR_LISTENER;
			//base..ctor();
			this.name = name;
			nameToGroupMap[name] = this;
			if (templateLoader == null)
			{
				this.templateLoader = new NullTemplateLoader();
			}
			else
			{
				this.templateLoader = templateLoader;
			}
			templateLexerClass = lexer;
			if (errorListener == null)
			{
				this.errorListener = DEFAULT_ERROR_LISTENER;
			}
			else
			{
				this.errorListener = errorListener;
			}
			this.superGroup = superGroup;
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x000708B0 File Offset: 0x0006EAB0
		public StringTemplateGroup(TextReader r) : this(r, null, DEFAULT_ERROR_LISTENER, null)
		{
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x000708C0 File Offset: 0x0006EAC0
		public StringTemplateGroup(TextReader r, IStringTemplateErrorListener errorListener) : this(r, null, errorListener, null)
		{
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x000708CC File Offset: 0x0006EACC
		public StringTemplateGroup(TextReader r, Type lexer) : this(r, lexer, null, null)
		{
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x000708D8 File Offset: 0x0006EAD8
		public StringTemplateGroup(TextReader r, Type lexer, IStringTemplateErrorListener errorListener) : this(r, lexer, errorListener, null)
		{
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x000708E4 File Offset: 0x0006EAE4
		public StringTemplateGroup(TextReader r, IStringTemplateErrorListener errorListener, StringTemplateGroup superGroup) : this(r, null, errorListener, superGroup)
		{
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x000708F0 File Offset: 0x0006EAF0
		public StringTemplateGroup(TextReader r, Type lexer, IStringTemplateErrorListener errorListener, StringTemplateGroup superGroup)
		{
			templates = new HashList();
			maps = new Hashtable();
			this.errorListener = DEFAULT_ERROR_LISTENER;
			//base..ctor();
			templatesDefinedInGroupFile = true;
			templateLexerClass = ((lexer == null) ? typeof(AngleBracketTemplateLexer) : lexer);
			this.errorListener = ((errorListener == null) ? DEFAULT_ERROR_LISTENER : errorListener);
			this.superGroup = superGroup;
			templateLoader = new NullTemplateLoader();
			ParseGroup(r);
			VerifyInterfaceImplementations();
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x00070974 File Offset: 0x0006EB74
		public static void RegisterDefaultLexer(Type lexerClass)
		{
			DEFAULT_TEMPLATE_LEXER_TYPE = lexerClass;
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x0007097C File Offset: 0x0006EB7C
		public static void RegisterGroupLoader(IStringTemplateGroupLoader loader)
		{
			groupLoader = loader;
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x00070984 File Offset: 0x0006EB84
		public static StringTemplateGroup LoadGroup(string name)
		{
			return LoadGroup(name, null);
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x00070990 File Offset: 0x0006EB90
		public static StringTemplateGroup LoadGroup(string name, StringTemplateGroup superGroup)
		{
            return groupLoader?.LoadGroup(name, superGroup);
        }

		// Token: 0x06000FDE RID: 4062 RVA: 0x000709A8 File Offset: 0x0006EBA8
		public static StringTemplateGroupInterface LoadInterface(string name)
		{
            return groupLoader?.LoadInterface(name);
        }

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000FDF RID: 4063 RVA: 0x000709C0 File Offset: 0x0006EBC0
		public virtual Type TemplateLexerClass
		{
			get
			{
				if (templateLexerClass != null)
				{
					return templateLexerClass;
				}
				return DEFAULT_TEMPLATE_LEXER_TYPE;
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000FE0 RID: 4064 RVA: 0x000709D8 File Offset: 0x0006EBD8
		// (set) Token: 0x06000FE1 RID: 4065 RVA: 0x000709E0 File Offset: 0x0006EBE0
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000FE2 RID: 4066 RVA: 0x000709EC File Offset: 0x0006EBEC
		// (set) Token: 0x06000FE3 RID: 4067 RVA: 0x000709F4 File Offset: 0x0006EBF4
		public virtual IStringTemplateErrorListener ErrorListener
		{
			get
			{
				return errorListener;
			}
			set
			{
				errorListener = value;
			}
		}

		// Token: 0x17000247 RID: 583
		// (set) Token: 0x06000FE4 RID: 4068 RVA: 0x00070A00 File Offset: 0x0006EC00
		public virtual IDictionary AttributeRenderers
		{
			set
			{
				attributeRenderers = value;
			}
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x00070A0C File Offset: 0x0006EC0C
		public virtual void SetSuperGroup(string groupName)
		{
			StringTemplateGroup stringTemplateGroup = (StringTemplateGroup)nameToGroupMap[groupName];
			if (stringTemplateGroup != null)
			{
				SuperGroup = stringTemplateGroup;
				return;
			}
			stringTemplateGroup = LoadGroup(groupName);
			if (stringTemplateGroup != null)
			{
				nameToGroupMap[groupName] = stringTemplateGroup;
				SuperGroup = stringTemplateGroup;
				return;
			}
			if (groupLoader == null)
			{
				Error("no group loader registered", null);
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x00070A68 File Offset: 0x0006EC68
		// (set) Token: 0x06000FE7 RID: 4071 RVA: 0x00070A70 File Offset: 0x0006EC70
		public virtual StringTemplateGroup SuperGroup
		{
			get
			{
				return superGroup;
			}
			set
			{
				superGroup = value;
			}
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00070A7C File Offset: 0x0006EC7C
		public void ImplementInterface(StringTemplateGroupInterface iface)
        {
            interfaces ??= new ArrayList();
            interfaces.Add(iface);
        }

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00070AA0 File Offset: 0x0006ECA0
		public void ImplementInterface(string interfaceName)
		{
			StringTemplateGroupInterface stringTemplateGroupInterface = (StringTemplateGroupInterface)nameToInterfaceMap[interfaceName];
			if (stringTemplateGroupInterface != null)
			{
				ImplementInterface(stringTemplateGroupInterface);
				return;
			}
			stringTemplateGroupInterface = LoadInterface(interfaceName);
			if (stringTemplateGroupInterface != null)
			{
				nameToInterfaceMap[interfaceName] = stringTemplateGroupInterface;
				ImplementInterface(stringTemplateGroupInterface);
				return;
			}
			if (groupLoader == null)
			{
				Error("no group loader registered", null);
			}
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x00070AFC File Offset: 0x0006ECFC
		public virtual StringTemplate CreateStringTemplate()
		{
			return new StringTemplate();
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x00070B04 File Offset: 0x0006ED04
		public virtual StringTemplate GetInstanceOf(StringTemplate enclosingInstance, string name)
		{
			StringTemplate stringTemplate = LookupTemplate(enclosingInstance, name);
            return stringTemplate?.GetInstanceOf();
        }

		// Token: 0x06000FEC RID: 4076 RVA: 0x00070B28 File Offset: 0x0006ED28
		public virtual StringTemplate GetInstanceOf(string name)
		{
			return GetInstanceOf(null, name);
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x00070B34 File Offset: 0x0006ED34
		public StringTemplate GetInstanceOf(string name, IDictionary attributes)
		{
			StringTemplate instanceOf = GetInstanceOf(name);
			instanceOf.Attributes = attributes;
			return instanceOf;
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x00070B54 File Offset: 0x0006ED54
		public virtual StringTemplate GetEmbeddedInstanceOf(StringTemplate enclosingInstance, string name)
		{
			StringTemplate instanceOf;
			if (name.StartsWith("super."))
			{
				instanceOf = enclosingInstance.NativeGroup.GetInstanceOf(enclosingInstance, name);
			}
			else
			{
				instanceOf = GetInstanceOf(enclosingInstance, name);
			}
			instanceOf.Group = this;
			instanceOf.EnclosingInstance = enclosingInstance;
			return instanceOf;
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x00070B98 File Offset: 0x0006ED98
		public virtual StringTemplate LookupTemplate(StringTemplate enclosingInstance, string name)
		{
			StringTemplate result;
			lock (this)
			{
				if (name.StartsWith("super."))
				{
					if (superGroup == null)
					{
						throw new StringTemplateException($"{Name} has no super group; invalid template: {name}");
					}
					int num = name.IndexOf('.');
					name = name.Substring(num + 1, name.Length - (num + 1));
					StringTemplate stringTemplate = superGroup.LookupTemplate(enclosingInstance, name);
					result = stringTemplate;
				}
				else
				{
					StringTemplate stringTemplate2 = (StringTemplate)templates[name];
					if (stringTemplate2 != null && stringTemplate2.NativeGroup.TemplateHasChanged(name))
					{
						templates.Remove(name);
						stringTemplate2 = null;
					}
					if (stringTemplate2 == null)
					{
						if (!templatesDefinedInGroupFile)
						{
							stringTemplate2 = LoadTemplate(name);
						}
						if (stringTemplate2 == null && superGroup != null)
						{
							stringTemplate2 = superGroup.GetInstanceOf(enclosingInstance, name);
							if (stringTemplate2 != null)
							{
								stringTemplate2.Group = this;
							}
						}
						if (stringTemplate2 == null)
						{
							templates[name] = NOT_FOUND_ST;
							string str = "";
							if (enclosingInstance != null)
							{
								str = $"; context is {enclosingInstance.GetEnclosingInstanceStackString()}";
							}
							throw new TemplateLoadException(this,
                                $"Can't load template '{GetLocationFromTemplateName(name)}'{str}");
						}
						templates[name] = stringTemplate2;
					}
					else if (stringTemplate2 == NOT_FOUND_ST)
					{
						return null;
					}
					result = stringTemplate2;
				}
			}
			return result;
		}

		// Token: 0x06000FF0 RID: 4080 RVA: 0x00070D0C File Offset: 0x0006EF0C
		public virtual StringTemplate LookupTemplate(string name)
		{
			return LookupTemplate(null, name);
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x00070D18 File Offset: 0x0006EF18
		protected virtual string GetLocationFromTemplateName(string templateName)
		{
			return templateLoader.GetLocationFromTemplateName(templateName);
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x00070D28 File Offset: 0x0006EF28
		protected virtual StringTemplate LoadTemplate(string templateName)
		{
			string text = templateLoader.LoadTemplate(templateName);
			if (text != null)
			{
				return DefineTemplate(templateName, text);
			}
			return null;
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x00070D50 File Offset: 0x0006EF50
		public virtual StringTemplate DefineTemplate(string name, string template)
		{
			StringTemplate result;
			lock (this)
			{
				if (name != null && name.IndexOf('.') != -1)
				{
					throw new ArgumentException("cannot have '.' in template names", "name");
				}
				StringTemplate stringTemplate = CreateStringTemplate();
				stringTemplate.Name = name;
				stringTemplate.Group = this;
				stringTemplate.NativeGroup = this;
				stringTemplate.Template = template;
				stringTemplate.ErrorListener = errorListener;
				templates[name] = stringTemplate;
				result = stringTemplate;
			}
			return result;
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x00070DDC File Offset: 0x0006EFDC
		public StringTemplate DefineRegionTemplate(string enclosingTemplateName, string regionName, string template, int type)
		{
			string mangledRegionName = GetMangledRegionName(enclosingTemplateName, regionName);
			StringTemplate stringTemplate = DefineTemplate(mangledRegionName, template);
			stringTemplate.IsRegion = true;
			stringTemplate.RegionDefType = type;
			return stringTemplate;
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x00070E0C File Offset: 0x0006F00C
		public StringTemplate DefineRegionTemplate(StringTemplate enclosingTemplate, string regionName, string template, int type)
		{
			StringTemplate result = DefineRegionTemplate(enclosingTemplate.OutermostName, regionName, template, type);
			enclosingTemplate.OutermostEnclosingInstance.AddRegionName(regionName);
			return result;
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x00070E38 File Offset: 0x0006F038
		public StringTemplate DefineImplicitRegionTemplate(StringTemplate enclosingTemplate, string name)
		{
			return DefineRegionTemplate(enclosingTemplate, name, "", 1);
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x00070E48 File Offset: 0x0006F048
		public string GetMangledRegionName(string enclosingTemplateName, string name)
		{
			return $"region__{enclosingTemplateName}__{name}";
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x00070E5C File Offset: 0x0006F05C
		public string GetUnMangledTemplateName(string mangledName)
		{
			return mangledName.Substring("region__".Length, mangledName.LastIndexOf("__", StringComparison.Ordinal) - "region__".Length);
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x00070E84 File Offset: 0x0006F084
		public virtual StringTemplate DefineTemplateAlias(string name, string target)
		{
			StringTemplate result;
			lock (this)
			{
				StringTemplate templateDefinition = GetTemplateDefinition(target);
				if (templateDefinition == null)
				{
					Error($"cannot alias {name} to undefined template: {target}");
					result = null;
				}
				else
				{
					templates[name] = templateDefinition;
					result = templateDefinition;
				}
			}
			return result;
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x00070EE8 File Offset: 0x0006F0E8
		public virtual bool IsDefinedInThisGroup(string name)
		{
			bool result;
			lock (this)
			{
				StringTemplate stringTemplate = (StringTemplate)templates[name];
				if (stringTemplate != null)
				{
					if (stringTemplate.IsRegion && stringTemplate.RegionDefType == 1)
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x00070F48 File Offset: 0x0006F148
		public virtual bool TemplateHasChanged(string templateName)
		{
			return !templatesDefinedInGroupFile && templateLoader.HasChanged(templateName);
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x00070F60 File Offset: 0x0006F160
		public virtual StringTemplate GetTemplateDefinition(string name)
		{
			StringTemplate result;
			lock (this)
			{
				result = (StringTemplate)templates[name];
			}
			return result;
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x00070FA4 File Offset: 0x0006F1A4
		public virtual bool IsDefined(string name)
		{
			bool result;
			try
			{
				result = (LookupTemplate(name) != null);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x00070FD8 File Offset: 0x0006F1D8
		public virtual void SetTemplateWriterType(Type c)
		{
			userSpecifiedWriter = c;
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x00070FE4 File Offset: 0x0006F1E4
		public virtual IStringTemplateWriter CreateInstanceOfTemplateWriter(TextWriter w)
		{
			IStringTemplateWriter stringTemplateWriter = null;
			if (userSpecifiedWriter != null)
			{
				try
				{
					ConstructorInfo constructor = userSpecifiedWriter.GetConstructor(new Type[]
					{
						typeof(TextWriter)
					});
					stringTemplateWriter = (IStringTemplateWriter)constructor.Invoke(new object[]
					{
						w
					});
				}
				catch (Exception e)
				{
					Error("problems getting IStringTemplateWriter", e);
				}
			}
			if (stringTemplateWriter == null)
			{
				stringTemplateWriter = new AutoIndentWriter(w);
			}
			return stringTemplateWriter;
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x00071064 File Offset: 0x0006F264
		public virtual void RegisterAttributeRenderer(Type attributeClassType, object renderer)
		{
			if (attributeRenderers == null)
			{
				attributeRenderers = Hashtable.Synchronized(new Hashtable());
			}
			attributeRenderers[attributeClassType] = renderer;
		}

		// Token: 0x06001001 RID: 4097 RVA: 0x0007108C File Offset: 0x0006F28C
		public virtual IAttributeRenderer GetAttributeRenderer(Type attributeClassType)
		{
			if (attributeRenderers != null)
			{
				IAttributeRenderer attributeRenderer = (IAttributeRenderer)attributeRenderers[attributeClassType];
				if (attributeRenderer == null && superGroup != null)
				{
					attributeRenderer = superGroup.GetAttributeRenderer(attributeClassType);
				}
				return attributeRenderer;
			}

            return superGroup?.GetAttributeRenderer(attributeClassType);
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x000710E4 File Offset: 0x0006F2E4
		public virtual IDictionary GetMap(string name)
		{
			if (maps != null)
			{
				IDictionary dictionary = (IDictionary)maps[name];
				if (dictionary == null && superGroup != null)
				{
					dictionary = superGroup.GetMap(name);
				}
				return dictionary;
			}

            return superGroup?.GetMap(name);
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x0007113C File Offset: 0x0006F33C
		public virtual void DefineMap(string name, IDictionary mapping)
		{
			maps[name] = mapping;
		}

		// Token: 0x06001004 RID: 4100 RVA: 0x0007114C File Offset: 0x0006F34C
		public virtual void Error(string msg)
		{
			errorListener.Error(msg, null);
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x0007115C File Offset: 0x0006F35C
		public virtual void Error(string msg, Exception e)
		{
			errorListener.Error(msg, e);
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x0007116C File Offset: 0x0006F36C
		public ICollection GetTemplateNames()
		{
			ICollection keys;
			lock (this)
			{
				keys = templates.Keys;
			}
			return keys;
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x000711A8 File Offset: 0x0006F3A8
		public virtual void EmitDebugStartStopStrings(bool emit)
		{
			debugTemplateOutput = emit;
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x000711B4 File Offset: 0x0006F3B4
		public virtual void DoNotEmitDebugStringsForTemplate(string templateName)
        {
            noDebugStartStopStrings ??= new Hashtable();
            noDebugStartStopStrings[templateName] = templateName;
        }

		// Token: 0x06001009 RID: 4105 RVA: 0x000711D8 File Offset: 0x0006F3D8
		public virtual void EmitTemplateStartDebugString(StringTemplate st, IStringTemplateWriter writer)
		{
			if (noDebugStartStopStrings == null || !noDebugStartStopStrings.Contains(st.Name))
			{
				writer.Write($"<{st.Name}>");
			}
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x00071214 File Offset: 0x0006F414
		public virtual void EmitTemplateStopDebugString(StringTemplate st, IStringTemplateWriter writer)
		{
			if (noDebugStartStopStrings == null || !noDebugStartStopStrings.Contains(st.Name))
			{
				writer.Write($"</{st.Name}>");
			}
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x00071250 File Offset: 0x0006F450
		public override string ToString()
		{
			return ToString(true);
		}

		// Token: 0x0600100C RID: 4108 RVA: 0x0007125C File Offset: 0x0006F45C
		public virtual string ToString(bool showTemplatePatterns)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"group {Name};\n");
			StringTemplate stringTemplate = new StringTemplate("$args;separator=\",\"$");
			foreach (var obj in new SortedList(templates))
			{
				string text = (string)((DictionaryEntry)obj).Key;
				StringTemplate stringTemplate2 = (StringTemplate)templates[text];
				if (stringTemplate2 != NOT_FOUND_ST)
				{
					stringTemplate = stringTemplate.GetInstanceOf();
					stringTemplate.SetAttribute("args", stringTemplate2?.FormalArguments);
					stringBuilder.Append(string.Concat(text, "(", stringTemplate, ")"));
					if (showTemplatePatterns)
					{
						stringBuilder.Append(" ::= ");
						stringBuilder.Append("<<");
						stringBuilder.Append(stringTemplate2?.Template);
						stringBuilder.Append(">>\n");
					}
					else
					{
						stringBuilder.Append('\n');
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x0600100D RID: 4109 RVA: 0x000713AC File Offset: 0x0006F5AC
		// (set) Token: 0x0600100E RID: 4110 RVA: 0x000713B4 File Offset: 0x0006F5B4
		public IAttributeStrategy AttributeStrategy
		{
			get => attributeStrategy;
            set => attributeStrategy = value;
        }

		// Token: 0x0600100F RID: 4111 RVA: 0x000713C0 File Offset: 0x0006F5C0
		protected internal virtual void ParseGroup(TextReader r)
		{
			try
			{
				GroupLexer lexer = new GroupLexer(r);
				GroupParser groupParser = new GroupParser(lexer);
				groupParser.group(this);
			}
			catch (Exception ex)
			{
				string text = "<unknown>";
				if (Name != null)
				{
					text = Name;
				}
				Error($"problem parsing group '{text}': {ex}", ex);
			}
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x00071440 File Offset: 0x0006F640
		protected void VerifyInterfaceImplementations()
		{
			int num = 0;
			while (interfaces != null && num < interfaces.Count)
			{
				StringTemplateGroupInterface stringTemplateGroupInterface = (StringTemplateGroupInterface)interfaces[num];
				IList missingTemplates = stringTemplateGroupInterface.GetMissingTemplates(this);
				IList mismatchedTemplates = stringTemplateGroupInterface.GetMismatchedTemplates(this);
				if (missingTemplates != null)
				{
					Error(
                        $"group '{Name}' does not satisfy interface '{stringTemplateGroupInterface.Name}': missing templates {CollectionUtils.ListToString(missingTemplates)}");
				}
				if (mismatchedTemplates != null)
				{
					Error(
                        $"group '{Name}' does not satisfy interface '{stringTemplateGroupInterface.Name}': mismatched arguments on these templates {CollectionUtils.ListToString(mismatchedTemplates)}");
				}
				num++;
			}
		}

		// Token: 0x04000CEC RID: 3308
		protected static Type DEFAULT_TEMPLATE_LEXER_TYPE = typeof(DefaultTemplateLexer);

		// Token: 0x04000CED RID: 3309
		protected static IDictionary nameToGroupMap = Hashtable.Synchronized(new Hashtable());

		// Token: 0x04000CEE RID: 3310
		protected static IDictionary nameToInterfaceMap = Hashtable.Synchronized(new Hashtable());

		// Token: 0x04000CEF RID: 3311
		private static IStringTemplateGroupLoader groupLoader = null;

		// Token: 0x04000CF0 RID: 3312
		public static IStringTemplateErrorListener DEFAULT_ERROR_LISTENER = ConsoleErrorListener.DefaultConsoleListener;

		// Token: 0x04000CF1 RID: 3313
		protected static readonly StringTemplate NOT_FOUND_ST = new StringTemplate();

		// Token: 0x04000CF2 RID: 3314
		protected string name;

		// Token: 0x04000CF3 RID: 3315
		protected IDictionary templates;

		// Token: 0x04000CF4 RID: 3316
		protected IDictionary maps;

		// Token: 0x04000CF5 RID: 3317
		protected Type templateLexerClass;

		// Token: 0x04000CF6 RID: 3318
		protected StringTemplateLoader templateLoader;

		// Token: 0x04000CF7 RID: 3319
		protected StringTemplateGroup superGroup;

		// Token: 0x04000CF8 RID: 3320
		protected IList interfaces;

		// Token: 0x04000CF9 RID: 3321
		protected bool templatesDefinedInGroupFile;

		// Token: 0x04000CFA RID: 3322
		protected Type userSpecifiedWriter;

		// Token: 0x04000CFB RID: 3323
		protected internal bool debugTemplateOutput;

		// Token: 0x04000CFC RID: 3324
		protected IDictionary noDebugStartStopStrings;

		// Token: 0x04000CFD RID: 3325
		protected IDictionary attributeRenderers;

		// Token: 0x04000CFE RID: 3326
		protected IStringTemplateErrorListener errorListener;

		// Token: 0x04000CFF RID: 3327
		protected IAttributeStrategy attributeStrategy;
	}
}
