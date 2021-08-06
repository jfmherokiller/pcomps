using System;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000229 RID: 553
	[Serializable]
	public class TemplateLoadException : StringTemplateException
	{
		// Token: 0x06001025 RID: 4133 RVA: 0x00071A7C File Offset: 0x0006FC7C
		public TemplateLoadException()
		{
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x00071A84 File Offset: 0x0006FC84
		public TemplateLoadException(object context) : base(context)
		{
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x00071A90 File Offset: 0x0006FC90
		public TemplateLoadException(string message) : base(message)
		{
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x00071A9C File Offset: 0x0006FC9C
		public TemplateLoadException(object context, string message) : base(message)
		{
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x00071AA8 File Offset: 0x0006FCA8
		public TemplateLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x00071AB4 File Offset: 0x0006FCB4
		public TemplateLoadException(object context, string message, Exception innerException) : base(context, message, innerException)
		{
		}
	}
}
