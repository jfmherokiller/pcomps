using System;
using System.IO;
using System.Reflection;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000219 RID: 537
	public class EmbeddedResourceGroupLoader : IStringTemplateGroupLoader
	{
		// Token: 0x06000F30 RID: 3888 RVA: 0x0006E350 File Offset: 0x0006C550
		protected EmbeddedResourceGroupLoader()
		{
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x0006E358 File Offset: 0x0006C558
		public EmbeddedResourceGroupLoader(IStringTemplateGroupFactory factory, Assembly assembly, string namespaceRoot) : this(factory, null, assembly, namespaceRoot)
		{
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0006E364 File Offset: 0x0006C564
		public EmbeddedResourceGroupLoader(IStringTemplateGroupFactory factory, IStringTemplateErrorListener errorListener, Assembly assembly, string namespaceRoot)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly", "An assembly must be specified");
			}
			if (namespaceRoot == null)
			{
				throw new ArgumentNullException("namespaceRoot", "A namespace must be specified");
			}
			this.factory = ((factory == null) ? new DefaultGroupFactory() : factory);
			this.errorListener = ((errorListener == null) ? NullErrorListener.DefaultNullListener : errorListener);
			this.assembly = assembly;
			this.namespaceRoot = namespaceRoot;
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0006E3D0 File Offset: 0x0006C5D0
		public StringTemplateGroup LoadGroup(string groupName)
		{
			return this.LoadGroup(groupName, null, null);
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0006E3DC File Offset: 0x0006C5DC
		public StringTemplateGroup LoadGroup(string groupName, StringTemplateGroup superGroup)
		{
			return this.LoadGroup(groupName, superGroup, null);
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0006E3E8 File Offset: 0x0006C5E8
		public StringTemplateGroup LoadGroup(string groupName, StringTemplateGroup superGroup, Type lexer)
		{
			StringTemplateGroup result = null;
			try
			{
				Stream manifestResourceStream = this.assembly.GetManifestResourceStream(
                    $"{this.namespaceRoot}.{groupName}.stg");
				result = this.factory.CreateGroup(new StreamReader(manifestResourceStream), lexer, this.errorListener, superGroup);
			}
			catch (Exception e)
			{
				this.Error(string.Concat(new string[]
				{
					"Resource Error: can't load group '",
					this.namespaceRoot,
					".",
					groupName,
					".stg' from assembly '",
					this.assembly.FullName,
					"'"
				}), e);
			}
			return result;
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0006E498 File Offset: 0x0006C698
		public StringTemplateGroupInterface LoadInterface(string interfaceName)
		{
			StringTemplateGroupInterface result = null;
			try
			{
				Stream manifestResourceStream = this.assembly.GetManifestResourceStream(
                    $"{this.namespaceRoot}.{interfaceName}.sti");
				if (manifestResourceStream != null)
				{
					result = this.factory.CreateInterface(new StreamReader(manifestResourceStream), this.errorListener, null);
				}
			}
			catch (Exception e)
			{
				this.Error(string.Concat(new string[]
				{
					"Resource Error: can't load interface '",
					this.namespaceRoot,
					".",
					interfaceName,
					".sti' from assembly '",
					this.assembly.FullName,
					"'"
				}), e);
			}
			return result;
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0006E54C File Offset: 0x0006C74C
		public void Error(string msg)
		{
			this.errorListener.Error(msg, null);
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0006E55C File Offset: 0x0006C75C
		public void Error(string msg, Exception e)
		{
			this.errorListener.Error(msg, e);
		}

		// Token: 0x04000CC2 RID: 3266
		protected IStringTemplateGroupFactory factory;

		// Token: 0x04000CC3 RID: 3267
		protected Assembly assembly;

		// Token: 0x04000CC4 RID: 3268
		protected string namespaceRoot;

		// Token: 0x04000CC5 RID: 3269
		protected IStringTemplateErrorListener errorListener;
	}
}
