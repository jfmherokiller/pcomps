using System;
using System.IO;
using System.Reflection;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x0200021B RID: 539
	public class EmbeddedResourceTemplateLoader : StringTemplateLoader
	{
		// Token: 0x06000F42 RID: 3906 RVA: 0x0006E5E4 File Offset: 0x0006C7E4
		private EmbeddedResourceTemplateLoader()
		{
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x0006E5EC File Offset: 0x0006C7EC
		public EmbeddedResourceTemplateLoader(Assembly assembly, string namespaceRoot) : this(assembly, namespaceRoot, false)
		{
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0006E5F8 File Offset: 0x0006C7F8
		public EmbeddedResourceTemplateLoader(Assembly assembly, string namespaceRoot, bool raiseExceptionForEmptyTemplate) : base(namespaceRoot, raiseExceptionForEmptyTemplate)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly", "An assembly must be specified");
			}
			if (namespaceRoot == null)
			{
				throw new ArgumentNullException("namespaceRoot", "A namespace must be specified");
			}
			this.assembly = assembly;
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x0006E630 File Offset: 0x0006C830
		public override bool HasChanged(string templateName)
		{
			return false;
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x0006E634 File Offset: 0x0006C834
		protected override string InternalLoadTemplateContents(string templateName)
		{
			string text = null;
			StreamReader streamReader = null;
			try
			{
				string name = $"{LocationRoot}.{GetLocationFromTemplateName(templateName)}";
				Stream manifestResourceStream = assembly.GetManifestResourceStream(name);
				if (manifestResourceStream != null)
				{
					streamReader = new StreamReader(manifestResourceStream);
					text = streamReader.ReadToEnd();
					if (text != null)
					{
						int length = text.Length;
					}
				}
			}
			finally
			{
                ((IDisposable)streamReader)?.Dispose();
                streamReader = null;
			}
			return text;
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x0006E6A8 File Offset: 0x0006C8A8
		public override string GetLocationFromTemplateName(string templateName)
		{
			return $"{templateName.Replace('\\', '.').Replace('/', '.')}.st";
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x0006E6C8 File Offset: 0x0006C8C8
		public override string GetTemplateNameFromLocation(string location)
		{
			return Path.ChangeExtension(location, null);
		}

		// Token: 0x04000CC8 RID: 3272
		protected Assembly assembly;
	}
}
