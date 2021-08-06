using System;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000220 RID: 544
	internal sealed class NullErrorListener : IStringTemplateErrorListener
	{
		// Token: 0x06000F5C RID: 3932 RVA: 0x0006EA78 File Offset: 0x0006CC78
		public void Error(string s, Exception e)
		{
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x0006EA7C File Offset: 0x0006CC7C
		public void Warning(string s)
		{
		}

		// Token: 0x04000CCC RID: 3276
		public static IStringTemplateErrorListener DefaultNullListener = new NullErrorListener();
	}
}
