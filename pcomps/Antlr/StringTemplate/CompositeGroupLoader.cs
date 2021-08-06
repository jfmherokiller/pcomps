using System;
using System.Collections;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000213 RID: 531
	public class CompositeGroupLoader : IStringTemplateGroupLoader
	{
		// Token: 0x06000F1B RID: 3867 RVA: 0x0006E18C File Offset: 0x0006C38C
		protected CompositeGroupLoader()
		{
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0006E194 File Offset: 0x0006C394
		public CompositeGroupLoader(params IStringTemplateGroupLoader[] loaders)
		{
			if (loaders == null || loaders.Length < 1)
			{
				throw new ArgumentNullException("loaders", "At least one IStringTemplateGroupLoader must be specified");
			}
			this.loaders = new ArrayList(loaders);
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0006E1C4 File Offset: 0x0006C3C4
		public StringTemplateGroup LoadGroup(string groupName)
		{
			return this.LoadGroup(groupName, null, null);
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x0006E1D0 File Offset: 0x0006C3D0
		public StringTemplateGroup LoadGroup(string groupName, StringTemplateGroup superGroup)
		{
			return this.LoadGroup(groupName, superGroup, null);
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x0006E1DC File Offset: 0x0006C3DC
		public StringTemplateGroup LoadGroup(string groupName, StringTemplateGroup superGroup, Type lexer)
		{
			foreach (object obj in this.loaders)
			{
				IStringTemplateGroupLoader stringTemplateGroupLoader = (IStringTemplateGroupLoader)obj;
				StringTemplateGroup stringTemplateGroup = stringTemplateGroupLoader.LoadGroup(groupName, superGroup, lexer);
				if (stringTemplateGroup != null)
				{
					return stringTemplateGroup;
				}
			}
			return null;
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x0006E248 File Offset: 0x0006C448
		public StringTemplateGroupInterface LoadInterface(string interfaceName)
		{
			foreach (object obj in this.loaders)
			{
				IStringTemplateGroupLoader stringTemplateGroupLoader = (IStringTemplateGroupLoader)obj;
				StringTemplateGroupInterface stringTemplateGroupInterface = stringTemplateGroupLoader.LoadInterface(interfaceName);
				if (stringTemplateGroupInterface != null)
				{
					return stringTemplateGroupInterface;
				}
			}
			return null;
		}

		// Token: 0x04000CBE RID: 3262
		protected ArrayList loaders;
	}
}
