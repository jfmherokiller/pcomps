using System;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000211 RID: 529
	public interface IStringTemplateGroupLoader
	{
		// Token: 0x06000F0C RID: 3852
		StringTemplateGroup LoadGroup(string groupName);

		// Token: 0x06000F0D RID: 3853
		StringTemplateGroup LoadGroup(string groupName, StringTemplateGroup superGroup);

		// Token: 0x06000F0E RID: 3854
		StringTemplateGroup LoadGroup(string groupName, StringTemplateGroup superGroup, Type lexer);

		// Token: 0x06000F0F RID: 3855
		StringTemplateGroupInterface LoadInterface(string interfaceName);
	}
}
