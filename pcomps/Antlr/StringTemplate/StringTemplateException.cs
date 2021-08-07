using System;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000225 RID: 549
	[Serializable]
	public class StringTemplateException : ApplicationException
	{
		// Token: 0x06000FC6 RID: 4038 RVA: 0x00070718 File Offset: 0x0006E918
		public StringTemplateException()
		{
			_context = null;
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x00070728 File Offset: 0x0006E928
		public StringTemplateException(object context)
		{
			_context = context;
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x00070738 File Offset: 0x0006E938
		public StringTemplateException(string message) : base(message)
		{
			_context = null;
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x00070748 File Offset: 0x0006E948
		public StringTemplateException(object context, string message) : base(message)
		{
			_context = context;
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x00070758 File Offset: 0x0006E958
		public StringTemplateException(string message, Exception innerException) : base(message, innerException)
		{
			_context = null;
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0007076C File Offset: 0x0006E96C
		public StringTemplateException(object context, string message, Exception innerException) : base(message, innerException)
		{
			_context = context;
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000FCC RID: 4044 RVA: 0x00070780 File Offset: 0x0006E980
		public object StringTemplateContext => _context;

        // Token: 0x04000CEB RID: 3307
		private object _context;
	}
}
