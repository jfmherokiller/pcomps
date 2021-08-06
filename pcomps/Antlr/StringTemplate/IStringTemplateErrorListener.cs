using System;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000214 RID: 532
	public interface IStringTemplateErrorListener
	{
		// Token: 0x06000F21 RID: 3873
		void Error(string msg, Exception e);

		// Token: 0x06000F22 RID: 3874
		void Warning(string msg);
	}
}
