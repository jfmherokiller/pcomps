using System.IO;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000221 RID: 545
	internal sealed class NullTemplateLoader : StringTemplateLoader
	{
		// Token: 0x06000F60 RID: 3936 RVA: 0x0006EA94 File Offset: 0x0006CC94
		public NullTemplateLoader() : base(null, false)
		{
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x0006EAA0 File Offset: 0x0006CCA0
		public override bool HasChanged(string templateName) => false;

        // Token: 0x06000F62 RID: 3938 RVA: 0x0006EAA4 File Offset: 0x0006CCA4
		protected override string InternalLoadTemplateContents(string templateName) => null;

        // Token: 0x06000F63 RID: 3939 RVA: 0x0006EAA8 File Offset: 0x0006CCA8
		public override string GetLocationFromTemplateName(string templateName) => $"{templateName}.st";

        // Token: 0x06000F64 RID: 3940 RVA: 0x0006EAB8 File Offset: 0x0006CCB8
		public override string GetTemplateNameFromLocation(string location) => Path.ChangeExtension(location, null);
    }
}
