using System;
using System.IO;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000218 RID: 536
	public sealed class DefaultGroupFactory : IStringTemplateGroupFactory
	{
		// Token: 0x06000F2C RID: 3884 RVA: 0x0006E31C File Offset: 0x0006C51C
		public StringTemplateGroup CreateGroup(TextReader reader, Type lexer, IStringTemplateErrorListener errorListener, StringTemplateGroup superGroup)
		{
			return new StringTemplateGroup(reader, lexer, errorListener, superGroup);
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0006E328 File Offset: 0x0006C528
		public StringTemplateGroup CreateGroup(string name, StringTemplateLoader templateLoader, Type lexer, IStringTemplateErrorListener errorListener, StringTemplateGroup superGroup)
		{
			return new StringTemplateGroup(name, templateLoader, lexer, errorListener, superGroup);
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0006E338 File Offset: 0x0006C538
		public StringTemplateGroupInterface CreateInterface(TextReader reader, IStringTemplateErrorListener errorListener, StringTemplateGroupInterface superInterface)
		{
			return new StringTemplateGroupInterface(reader, errorListener, superInterface);
		}

		// Token: 0x04000CC1 RID: 3265
		public static readonly DefaultGroupFactory DefaultFactory = new DefaultGroupFactory();
	}
}
