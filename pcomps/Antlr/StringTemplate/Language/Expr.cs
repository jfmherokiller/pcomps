namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000239 RID: 569
	public abstract class Expr
	{
		// Token: 0x060010F5 RID: 4341 RVA: 0x0007AA58 File Offset: 0x00078C58
		public Expr(StringTemplate enclosingTemplate)
		{
			this.enclosingTemplate = enclosingTemplate;
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060010F6 RID: 4342 RVA: 0x0007AA68 File Offset: 0x00078C68
		public virtual StringTemplate EnclosingTemplate
		{
			get
			{
				return this.enclosingTemplate;
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060010F7 RID: 4343 RVA: 0x0007AA70 File Offset: 0x00078C70
		// (set) Token: 0x060010F8 RID: 4344 RVA: 0x0007AA78 File Offset: 0x00078C78
		public virtual string Indentation
		{
			get
			{
				return this.indentation;
			}
			set
			{
				this.indentation = value;
			}
		}

		// Token: 0x060010F9 RID: 4345
		public abstract int Write(StringTemplate self, IStringTemplateWriter output);

		// Token: 0x04000E2C RID: 3628
		protected StringTemplate enclosingTemplate;

		// Token: 0x04000E2D RID: 3629
		protected string indentation;
	}
}
