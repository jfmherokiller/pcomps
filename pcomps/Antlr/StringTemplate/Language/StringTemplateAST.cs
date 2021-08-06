using pcomps.Antlr.collections;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x0200024D RID: 589
	public class StringTemplateAST : CommonAST
	{
		// Token: 0x060011CB RID: 4555 RVA: 0x0008254C File Offset: 0x0008074C
		public StringTemplateAST()
		{
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x00082554 File Offset: 0x00080754
		public StringTemplateAST(int type, string text)
		{
			this.Type = type;
			this.setText(text);
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x060011CD RID: 4557 RVA: 0x0008256C File Offset: 0x0008076C
		// (set) Token: 0x060011CE RID: 4558 RVA: 0x00082574 File Offset: 0x00080774
		public virtual StringTemplate StringTemplate
		{
			get
			{
				return this.st;
			}
			set
			{
				this.st = value;
			}
		}

		// Token: 0x04000F01 RID: 3841
		public new static readonly StringTemplateAST.StringTemplateASTCreator Creator = new StringTemplateAST.StringTemplateASTCreator();

		// Token: 0x04000F02 RID: 3842
		protected StringTemplate st;

		// Token: 0x0200024E RID: 590
		public class StringTemplateASTCreator : ASTNodeCreator
		{
			// Token: 0x17000271 RID: 625
			// (get) Token: 0x060011D1 RID: 4561 RVA: 0x00082594 File Offset: 0x00080794
			public override string ASTNodeTypeName
			{
				get
				{
					return typeof(StringTemplateAST).FullName;
				}
			}

			// Token: 0x060011D2 RID: 4562 RVA: 0x000825A8 File Offset: 0x000807A8
			public override AST Create()
			{
				return new StringTemplateAST();
			}
		}
	}
}
