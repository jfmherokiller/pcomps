using System;
using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x0200001B RID: 27
	public class CommonAST : BaseAST
	{
		// Token: 0x0600014B RID: 331 RVA: 0x00005800 File Offset: 0x00003A00
		[Obsolete("Deprecated since version 2.7.2. Use ASTFactory.dup() instead.", false)]
		protected CommonAST(CommonAST another)
		{
			this.ttype = another.ttype;
			this.text = ((another.text == null) ? null : string.Copy(another.text));
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00005844 File Offset: 0x00003A44
		public override string getText()
		{
			return this.text;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00005858 File Offset: 0x00003A58
		// (set) Token: 0x0600014E RID: 334 RVA: 0x0000586C File Offset: 0x00003A6C
		public override int Type
		{
			get
			{
				return this.ttype;
			}
			set
			{
				this.ttype = value;
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00005880 File Offset: 0x00003A80
		public override void initialize(int t, string txt)
		{
			this.Type = t;
			this.setText(txt);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000589C File Offset: 0x00003A9C
		public override void initialize(AST t)
		{
			this.setText(t.getText());
			this.Type = t.Type;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x000058C4 File Offset: 0x00003AC4
		public CommonAST()
		{
		}

		// Token: 0x06000152 RID: 338 RVA: 0x000058E0 File Offset: 0x00003AE0
		public CommonAST(IToken tok)
		{
			this.initialize(tok);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00005904 File Offset: 0x00003B04
		public override void initialize(IToken tok)
		{
			this.setText(tok.getText());
			this.Type = tok.Type;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000592C File Offset: 0x00003B2C
		public override void setText(string text_)
		{
			this.text = text_;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00005940 File Offset: 0x00003B40
		public override void setType(int ttype_)
		{
			this.Type = ttype_;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00005954 File Offset: 0x00003B54
		[Obsolete("Deprecated since version 2.7.2. Use ASTFactory.dup() instead.", false)]
		public override object Clone()
		{
			return new CommonAST(this);
		}

		// Token: 0x04000056 RID: 86
		public static readonly CommonAST.CommonASTCreator Creator = new CommonAST.CommonASTCreator();

		// Token: 0x04000057 RID: 87
		internal int ttype = 0;

		// Token: 0x04000058 RID: 88
		internal string text;

		// Token: 0x0200001C RID: 28
		public class CommonASTCreator : ASTNodeCreator
		{
			// Token: 0x17000015 RID: 21
			// (get) Token: 0x06000159 RID: 345 RVA: 0x00005994 File Offset: 0x00003B94
			public override string ASTNodeTypeName
			{
				get
				{
					return typeof(CommonAST).FullName;
				}
			}

			// Token: 0x0600015A RID: 346 RVA: 0x000059B0 File Offset: 0x00003BB0
			public override AST Create()
			{
				return new CommonAST();
			}
		}
	}
}
