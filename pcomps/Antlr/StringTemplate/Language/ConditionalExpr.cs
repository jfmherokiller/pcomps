using pcomps.Antlr.collections;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x0200023F RID: 575
	public record ConditionalExpr : ASTExpr
	{
		// Token: 0x17000269 RID: 617
		// (get) Token: 0x0600112B RID: 4395 RVA: 0x0007C078 File Offset: 0x0007A278
		// (set) Token: 0x0600112C RID: 4396 RVA: 0x0007C080 File Offset: 0x0007A280
		public virtual StringTemplate Subtemplate
		{
			get => subtemplate;
            set => subtemplate = value;
        }

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x0600112D RID: 4397 RVA: 0x0007C08C File Offset: 0x0007A28C
		// (set) Token: 0x0600112E RID: 4398 RVA: 0x0007C094 File Offset: 0x0007A294
		public virtual StringTemplate ElseSubtemplate
		{
			get => elseSubtemplate;
            set => elseSubtemplate = value;
        }

		// Token: 0x0600112F RID: 4399 RVA: 0x0007C0A0 File Offset: 0x0007A2A0
		public ConditionalExpr(StringTemplate enclosingTemplate, AST tree) : base(enclosingTemplate, tree, null)
		{
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x0007C0AC File Offset: 0x0007A2AC
		public override int Write(StringTemplate self, IStringTemplateWriter output)
		{
			if (exprTree == null || self == null || output == null)
			{
				return 0;
			}
			var actionEvaluator = new ActionEvaluator(self, this, output);
			ActionParser.initializeASTFactory(actionEvaluator.getASTFactory());
			var result = 0;
			try
			{
				var firstChild = exprTree.getFirstChild();
				var flag = actionEvaluator.ifCondition(firstChild);
				if (flag)
				{
					var instanceOf = subtemplate.GetInstanceOf();
					instanceOf.EnclosingInstance = self;
					instanceOf.Group = self.Group;
					instanceOf.NativeGroup = self.NativeGroup;
					result = instanceOf.Write(output);
				}
				else if (elseSubtemplate != null)
				{
					var instanceOf2 = elseSubtemplate.GetInstanceOf();
					instanceOf2.EnclosingInstance = self;
					instanceOf2.Group = self.Group;
					instanceOf2.NativeGroup = self.NativeGroup;
					result = instanceOf2.Write(output);
				}
			}
			catch (RecognitionException e)
			{
				self.Error($"can't evaluate tree: {exprTree.ToStringList()}", e);
			}
			return result;
		}

		// Token: 0x04000E4B RID: 3659
		internal StringTemplate subtemplate;

		// Token: 0x04000E4C RID: 3660
		internal StringTemplate elseSubtemplate;
	}
}
