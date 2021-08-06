using System;
using System.IO;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000217 RID: 535
	public interface IStringTemplateGroupFactory
	{
		// Token: 0x06000F28 RID: 3880
		StringTemplateGroup CreateGroup(TextReader reader, Type lexer, IStringTemplateErrorListener errorListener, StringTemplateGroup superGroup);

		// Token: 0x06000F29 RID: 3881
		StringTemplateGroup CreateGroup(string name, StringTemplateLoader templateLoader, Type lexer, IStringTemplateErrorListener errorListener, StringTemplateGroup superGroup);

		// Token: 0x06000F2A RID: 3882
		StringTemplateGroupInterface CreateInterface(TextReader reader, IStringTemplateErrorListener errorListener, StringTemplateGroupInterface superInterface);
	}
}
