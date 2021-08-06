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
			this.ttype = tokenizer.NextToken();
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x00017D54 File Offset: 0x00015F54
		public object Pattern()
		{
			if (this.ttype == 1)
			{
				return this.ParseTree();
			}
			if (this.ttype != 3)
			{
				return null;
			}
			object result = this.ParseNode();
			if (this.ttype == -1)
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x00017D98 File Offset: 0x00015F98
		public object ParseTree()
		{
			if (this.ttype != 1)
			{
				Console.Out.WriteLine("no BEGIN");
				return null;
			}
			this.ttype = this.tokenizer.NextToken();
			object obj = this.ParseNode();
			if (obj == null)
			{
				return null;
			}
			while (this.ttype == 1 || this.ttype == 3 || this.ttype == 5 || this.ttype == 7)
			{
				if (this.ttype == 1)
				{
					object child = this.ParseTree();
					this.adaptor.AddChild(obj, child);
				}
				else
				{
					object obj2 = this.ParseNode();
					if (obj2 == null)
					{
						return null;
					}
					this.adaptor.AddChild(obj, obj2);
				}
			}
			if (this.ttype != 2)
			{
				Console.Out.WriteLine("no END");
				return null;
			}
			this.ttype = this.tokenizer.NextToken();
			return obj;
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x00017E88 File Offset: 0x00016088
		public object ParseNode()
		{
			string text = null;
			if (this.ttype == 5)
			{
				this.ttype = this.tokenizer.NextToken();
				if (this.ttype != 3)
				{
					return null;
				}
				text = this.tokenizer.sval.ToString();
				this.ttype = this.tokenizer.NextToken();
				if (this.ttype != 6)
				{
					return null;
				}
				this.ttype = this.tokenizer.NextToken();
			}
			if (this.ttype == 7)
			{
				this.ttype = this.tokenizer.NextToken();
				IToken payload = new CommonToken(0, ".");
				TreeWizard.TreePattern treePattern = new TreeWizard.WildcardTreePattern(payload);
				if (text != null)
				{
					treePattern.label = text;
				}
				return treePattern;
			}
			if (this.ttype != 3)
			{
				return null;
			}
			string text2 = this.tokenizer.sval.ToString();
			this.ttype = this.tokenizer.NextToken();
			if (text2.Equals("nil"))
			{
				return this.adaptor.GetNilNode();
			}
			string text3 = text2;
			string text4 = null;
			if (this.ttype == 4)
			{
				text4 = this.tokenizer.sval.ToString();
				text3 = text4;
				this.ttype = this.tokenizer.NextToken();
			}
			int tokenType = this.wizard.GetTokenType(text2);
			if (tokenType == 0)
			{
				return null;
			}
			object obj = this.adaptor.Create(tokenType, text3);
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
