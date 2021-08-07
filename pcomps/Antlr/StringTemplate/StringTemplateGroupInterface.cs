using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using pcomps.Antlr.StringTemplate.Collections;
using pcomps.Antlr.StringTemplate.Language;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000227 RID: 551
	public class StringTemplateGroupInterface
	{
		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06001012 RID: 4114 RVA: 0x0007158C File Offset: 0x0006F78C
		// (set) Token: 0x06001013 RID: 4115 RVA: 0x00071594 File Offset: 0x0006F794
		public string Name
		{
			get => name;
            set => name = value;
        }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06001014 RID: 4116 RVA: 0x000715A0 File Offset: 0x0006F7A0
		// (set) Token: 0x06001015 RID: 4117 RVA: 0x000715A8 File Offset: 0x0006F7A8
		public virtual IStringTemplateErrorListener ErrorListener
		{
			get => errorListener;
            set => errorListener = value;
        }

		// Token: 0x06001016 RID: 4118 RVA: 0x000715B4 File Offset: 0x0006F7B4
		public StringTemplateGroupInterface(TextReader r) : this(r, DEFAULT_ERROR_LISTENER, null)
		{
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x000715C4 File Offset: 0x0006F7C4
		public StringTemplateGroupInterface(TextReader r, IStringTemplateErrorListener errorListener) : this(r, errorListener, null)
		{
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x000715D0 File Offset: 0x0006F7D0
		public StringTemplateGroupInterface(TextReader r, IStringTemplateErrorListener errorListener, StringTemplateGroupInterface superInterface)
		{
			ErrorListener = errorListener;
			SuperInterface = superInterface;
			ParseInterface(r);
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06001019 RID: 4121 RVA: 0x00071604 File Offset: 0x0006F804
		// (set) Token: 0x0600101A RID: 4122 RVA: 0x0007160C File Offset: 0x0006F80C
		public StringTemplateGroupInterface SuperInterface
		{
			get => superInterface;
            set => superInterface = value;
        }

		// Token: 0x0600101B RID: 4123 RVA: 0x00071618 File Offset: 0x0006F818
		protected void ParseInterface(TextReader r)
		{
			try
			{
				var lexer = new InterfaceLexer(r);
				var interfaceParser = new InterfaceParser(lexer);
				interfaceParser.groupInterface(this);
			}
			catch (Exception ex)
			{
				var text = (Name == null) ? "<unknown>" : Name;
				Error(string.Concat("problem parsing group ", text, ": ", ex), ex);
			}
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x00071698 File Offset: 0x0006F898
		public void DefineTemplate(string name, HashList formalArgs, bool optional)
		{
			var templateDefinition = new TemplateDefinition(name, formalArgs, optional);
			templates.Add(templateDefinition.name, templateDefinition);
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x000716C0 File Offset: 0x0006F8C0
		public IList GetMissingTemplates(StringTemplateGroup group)
		{
			ArrayList arrayList = null;
			foreach (var obj in templates.Keys)
			{
				var key = (string)obj;
				var templateDefinition = (TemplateDefinition)templates[key];
                if (templateDefinition.optional || @group.IsDefined(templateDefinition.name)) continue;
                arrayList ??= new ArrayList();
                arrayList.Add(templateDefinition.name);
            }
			return arrayList;
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x0007175C File Offset: 0x0006F95C
		public IList GetMismatchedTemplates(StringTemplateGroup group)
		{
			ArrayList arrayList = null;
			foreach (var obj in templates.Keys)
            {
                var key = (string)obj;
                var templateDefinition = (TemplateDefinition)templates[key];
                if (!@group.IsDefined(templateDefinition.name)) continue;
                var templateDefinition2 = @group.GetTemplateDefinition(templateDefinition.name);
                var formalArguments = templateDefinition2.FormalArguments;
                var flag = (templateDefinition.formalArgs != null && formalArguments == null) || (templateDefinition.formalArgs == null && formalArguments != null) || templateDefinition.formalArgs.Count != formalArguments.Count;
                if (!flag)
                {
                    if (formalArguments.Keys.Cast<string>().Any(key2 => templateDefinition.formalArgs[key2] == null))
                    {
                        flag = true;
                    }
                }

                if (!flag) continue;
                arrayList ??= new ArrayList();
                arrayList.Add(GetTemplateSignature(templateDefinition));
            }
            return arrayList;
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x000718AC File Offset: 0x0006FAAC
		public void Error(string msg)
		{
			ErrorListener.Error(msg, null);
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x000718BC File Offset: 0x0006FABC
		public void Error(string msg, Exception e)
		{
			ErrorListener.Error(msg, e);
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x000718CC File Offset: 0x0006FACC
		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append("interface ");
			stringBuilder.Append(Name);
			stringBuilder.Append(";\n");
			foreach (var obj in templates.Keys)
			{
				var key = (string)obj;
				var tdef = (TemplateDefinition)templates[key];
				stringBuilder.Append(GetTemplateSignature(tdef));
				stringBuilder.Append(";\n");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x00071984 File Offset: 0x0006FB84
		protected string GetTemplateSignature(TemplateDefinition tdef)
		{
			var stringBuilder = new StringBuilder();
			if (tdef.optional)
			{
				stringBuilder.Append("optional ");
			}
			stringBuilder.Append(tdef.name);
			if (tdef.formalArgs != null)
			{
				stringBuilder.Append('(');
				var flag = false;
				foreach (var obj in tdef.formalArgs.Keys)
				{
					var value = (string)obj;
					if (flag)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(value);
					flag = true;
				}
				stringBuilder.Append(')');
			}
			else
			{
				stringBuilder.Append("()");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000D00 RID: 3328
		public static IStringTemplateErrorListener DEFAULT_ERROR_LISTENER = ConsoleErrorListener.DefaultConsoleListener;

		// Token: 0x04000D01 RID: 3329
		protected string name;

		// Token: 0x04000D02 RID: 3330
		protected IDictionary templates = new HashList();

		// Token: 0x04000D03 RID: 3331
		protected StringTemplateGroupInterface superInterface;

		// Token: 0x04000D04 RID: 3332
		protected IStringTemplateErrorListener errorListener = DEFAULT_ERROR_LISTENER;

		// Token: 0x02000228 RID: 552
		protected sealed record TemplateDefinition
		{
			// Token: 0x06001024 RID: 4132 RVA: 0x00071A5C File Offset: 0x0006FC5C
			public TemplateDefinition(string name, HashList formalArgs, bool optional)
			{
				this.name = name;
				this.formalArgs = formalArgs;
				this.optional = optional;
			}

			// Token: 0x04000D05 RID: 3333
			public string name;

			// Token: 0x04000D06 RID: 3334
			public HashList formalArgs;

			// Token: 0x04000D07 RID: 3335
			public bool optional;
		}
	}
}
