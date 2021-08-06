using System;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000CF RID: 207
	public class TreePatternParser
	{
		// Token: 0x0600086A RID: 2154 RVA: 0x00017D28 File Offset: 0x00015F28
		public TreePatternParser(TreePatternLexer tokenizer, TreeWizard wizard, ITreeAdaptor adaptor)
		{
			this.tokenizer = tokenizer;
			this.wizard = wizard;
			this.adaptor = adaptor;
			ttype = tokenizer.NextToken();
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x00017D54 File Offset: 0x00015F54
		public object Pattern()
		{
			if (ttype == 1)
			{
				return ParseTree();
			}
			if (ttype != 3)
			{
				return null;
			}
			var result = ParseNode();
			if (ttype == -1)
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x00017D98 File Offset: 0x00015F98
		public object ParseTree()
		{
			if (ttype != 1)
			{
				Console.Out.WriteLine("no BEGIN");
				return null;
			}
			ttype = tokenizer.NextToken();
			var obj = ParseNode();
			if (obj == null)
			{
				return null;
			}
			while (ttype == 1 || ttype == 3 || ttype == 5 || ttype == 7)
			{
				if (ttype == 1)
				{
					var child = ParseTree();
					adaptor.AddChild(obj, child);
				}
				else
				{
					var obj2 = ParseNode();
					if (obj2 == null)
					{
						return null;
					}
					adaptor.AddChild(obj, obj2);
				}
			}
			if (ttype != 2)
			{
				Console.Out.WriteLine("no END");
				return null;
			}
			ttype = tokenizer.NextToken();
			return obj;
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x00017E88 File Offset: 0x00016088
		public object ParseNode()
		{
			string text = null;
			if (ttype == 5)
			{
				ttype = tokenizer.NextToken();
				if (ttype != 3)
				{
					return null;
				}
				text = tokenizer.sval.ToString();
				ttype = tokenizer.NextToken();
				if (ttype != 6)
				{
					return null;
				}
				ttype = tokenizer.NextToken();
			}
			if (ttype == 7)
			{
				ttype = tokenizer.NextToken();
				IToken payload = new CommonToken(0, ".");
				TreeWizard.TreePattern treePattern = new TreeWizard.WildcardTreePattern(payload);
				if (text != null)
				{
					treePattern.label = text;
				}
				return treePattern;
			}
			if (ttype != 3)
			{
				return null;
			}
			var text2 = tokenizer.sval.ToString();
			ttype = tokenizer.NextToken();
			if (text2.Equals("nil"))
			{
				return adaptor.GetNilNode();
			}
			var text3 = text2;
			string text4 = null;
			if (ttype == 4)
			{
				text4 = tokenizer.sval.ToString();
				text3 = text4;
				ttype = tokenizer.NextToken();
			}
			var tokenType = wizard.GetTokenType(text2);
			if (tokenType == 0)
			{
				return null;
			}
			var obj = adaptor.Create(tokenType, text3);
			if (text != null && obj.GetType() == typeof(TreeWizard.TreePattern))
			{
				((TreeWizard.TreePattern)obj).label = text;
			}
			if (text4 != null && obj.GetType() == typeof(TreeWizard.TreePattern))
			{
				((TreeWizard.TreePattern)obj).hasTextArg = true;
			}
			return obj;
		}

		// Token: 0x04000236 RID: 566
		protected TreePatternLexer tokenizer;

		// Token: 0x04000237 RID: 567
		protected int ttype;

		// Token: 0x04000238 RID: 568
		protected TreeWizard wizard;

		// Token: 0x04000239 RID: 569
		protected ITreeAdaptor adaptor;
	}
}
