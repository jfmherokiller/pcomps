namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x0200021E RID: 542
	public interface IAttributeStrategy
	{
		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000F56 RID: 3926
		bool UseCustomGetObjectProperty { get; }

		// Token: 0x06000F57 RID: 3927
		object GetObjectProperty(StringTemplate self, object o, string propertyName);

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000F58 RID: 3928
		bool UseCustomTestAttributeTrue { get; }

		// Token: 0x06000F59 RID: 3929
		bool TestAttributeTrue(object a);
	}
}
