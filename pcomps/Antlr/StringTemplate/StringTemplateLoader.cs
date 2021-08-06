namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x0200021A RID: 538
	public abstract class StringTemplateLoader
	{
		// Token: 0x06000F39 RID: 3897 RVA: 0x0006E56C File Offset: 0x0006C76C
		protected StringTemplateLoader()
		{
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x0006E574 File Offset: 0x0006C774
		public StringTemplateLoader(string locationRoot) : this(locationRoot, true)
		{
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x0006E580 File Offset: 0x0006C780
		protected StringTemplateLoader(string locationRoot, bool raiseExceptionForEmptyTemplate)
		{
			this.locationRoot = locationRoot;
			this.raiseExceptionForEmptyTemplate = raiseExceptionForEmptyTemplate;
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000F3C RID: 3900 RVA: 0x0006E598 File Offset: 0x0006C798
		public string LocationRoot
		{
			get
			{
				return this.locationRoot;
			}
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x0006E5A0 File Offset: 0x0006C7A0
		public string LoadTemplate(string templateName)
		{
			string text = this.InternalLoadTemplateContents(templateName);
			if (text != null && text.Length > 0)
			{
				return text;
			}
			if (this.raiseExceptionForEmptyTemplate)
			{
				throw new TemplateLoadException($"no text in template '{templateName}'");
			}
			return null;
		}

		// Token: 0x06000F3E RID: 3902
		public abstract bool HasChanged(string templateName);

		// Token: 0x06000F3F RID: 3903
		public abstract string GetLocationFromTemplateName(string templateName);

		// Token: 0x06000F40 RID: 3904
		public abstract string GetTemplateNameFromLocation(string location);

		// Token: 0x06000F41 RID: 3905
		protected abstract string InternalLoadTemplateContents(string templateName);

		// Token: 0x04000CC6 RID: 3270
		protected string locationRoot;

		// Token: 0x04000CC7 RID: 3271
		protected bool raiseExceptionForEmptyTemplate;
	}
}
