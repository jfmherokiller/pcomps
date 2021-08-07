using System;
using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x0200001D RID: 29
	public class CommonASTWithHiddenTokens : CommonAST
	{
		// Token: 0x0600015B RID: 347 RVA: 0x000059C4 File Offset: 0x00003BC4
		public CommonASTWithHiddenTokens()
		{
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000059D8 File Offset: 0x00003BD8
		public CommonASTWithHiddenTokens(IToken tok) : base(tok)
		{
		}

		// Token: 0x0600015D RID: 349 RVA: 0x000059EC File Offset: 0x00003BEC
		[Obsolete("Deprecated since version 2.7.2. Use ASTFactory.dup() instead.", false)]
		protected CommonASTWithHiddenTokens(CommonASTWithHiddenTokens another) : base(another)
		{
			hiddenBefore = another.hiddenBefore;
			hiddenAfter = another.hiddenAfter;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00005A18 File Offset: 0x00003C18
		public virtual IHiddenStreamToken getHiddenAfter()
		{
			return hiddenAfter;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00005A2C File Offset: 0x00003C2C
		public virtual IHiddenStreamToken getHiddenBefore()
		{
			return hiddenBefore;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00005A40 File Offset: 0x00003C40
		public override void initialize(AST t)
		{
			hiddenBefore = ((CommonASTWithHiddenTokens)t).getHiddenBefore();
			hiddenAfter = ((CommonASTWithHiddenTokens)t).getHiddenAfter();
			base.initialize(t);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00005A78 File Offset: 0x00003C78
		public override void initialize(IToken tok)
		{
			var hiddenStreamToken = (IHiddenStreamToken)tok;
			base.initialize(hiddenStreamToken);
			hiddenBefore = hiddenStreamToken.getHiddenBefore();
			hiddenAfter = hiddenStreamToken.getHiddenAfter();
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00005AAC File Offset: 0x00003CAC
		[Obsolete("Deprecated since version 2.7.2. Use ASTFactory.dup() instead.", false)]
		public override object Clone()
		{
			return new CommonASTWithHiddenTokens(this);
		}

		// Token: 0x04000059 RID: 89
		public new static readonly CommonASTWithHiddenTokensCreator Creator = new CommonASTWithHiddenTokensCreator();

		// Token: 0x0400005A RID: 90
		protected internal IHiddenStreamToken hiddenBefore;

		// Token: 0x0400005B RID: 91
		protected internal IHiddenStreamToken hiddenAfter;

		// Token: 0x0200001E RID: 30
		public class CommonASTWithHiddenTokensCreator : ASTNodeCreator
		{
			// Token: 0x17000016 RID: 22
			// (get) Token: 0x06000165 RID: 357 RVA: 0x00005AEC File Offset: 0x00003CEC
			public override string ASTNodeTypeName => typeof(CommonASTWithHiddenTokens).FullName;

            // Token: 0x06000166 RID: 358 RVA: 0x00005B08 File Offset: 0x00003D08
			public override AST Create()
			{
				return new CommonASTWithHiddenTokens();
			}
		}
	}
}
