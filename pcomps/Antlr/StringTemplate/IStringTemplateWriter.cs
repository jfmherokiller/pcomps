namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x0200020F RID: 527
	public interface IStringTemplateWriter
	{
		// Token: 0x06000EF5 RID: 3829
		void PushIndentation(string indent);

		// Token: 0x06000EF6 RID: 3830
		string PopIndentation();

		// Token: 0x06000EF7 RID: 3831
		void PushAnchorPoint();

		// Token: 0x06000EF8 RID: 3832
		void PopAnchorPoint();

		// Token: 0x17000228 RID: 552
		// (set) Token: 0x06000EF9 RID: 3833
		int LineWidth { set; }

		// Token: 0x06000EFA RID: 3834
		int Write(string str);

		// Token: 0x06000EFB RID: 3835
		int Write(string str, string wrap);

		// Token: 0x06000EFC RID: 3836
		int WriteWrapSeparator(string wrap);

		// Token: 0x06000EFD RID: 3837
		int WriteSeparator(string str);
	}
}
