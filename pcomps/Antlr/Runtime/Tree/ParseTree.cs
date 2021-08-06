using System.Collections;
using System.Text;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000AC RID: 172
	public class ParseTree : BaseTree
	{
		// Token: 0x0600069C RID: 1692 RVA: 0x0001285C File Offset: 0x00010A5C
		public ParseTree(object label)
		{
			payload = label;
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600069D RID: 1693 RVA: 0x0001286C File Offset: 0x00010A6C
		public override int Type
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x00012870 File Offset: 0x00010A70
		public override string Text
		{
			get
			{
				return ToString();
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x00012878 File Offset: 0x00010A78
		// (set) Token: 0x060006A0 RID: 1696 RVA: 0x0001287C File Offset: 0x00010A7C
		public override int TokenStartIndex
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060006A1 RID: 1697 RVA: 0x00012880 File Offset: 0x00010A80
		// (set) Token: 0x060006A2 RID: 1698 RVA: 0x00012884 File Offset: 0x00010A84
		public override int TokenStopIndex
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x00012888 File Offset: 0x00010A88
		public override ITree DupNode()
		{
			return null;
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0001288C File Offset: 0x00010A8C
		public override string ToString()
		{
			if (!(payload is IToken))
			{
				return payload.ToString();
			}
			IToken token = (IToken)payload;
			if (token.Type == Token.EOF)
			{
				return "<EOF>";
			}
			return token.Text;
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x000128E0 File Offset: 0x00010AE0
		public string ToStringWithHiddenTokens()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (hiddenTokens != null)
			{
				for (int i = 0; i < hiddenTokens.Count; i++)
				{
					IToken token = (IToken)hiddenTokens[i];
					stringBuilder.Append(token.Text);
				}
			}
			string text = ToString();
			if (text != "<EOF>")
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x00012960 File Offset: 0x00010B60
		public string ToInputString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			_ToStringLeaves(stringBuilder);
			return stringBuilder.ToString();
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x00012980 File Offset: 0x00010B80
		public void _ToStringLeaves(StringBuilder buf)
		{
			if (payload is IToken)
			{
				buf.Append(ToStringWithHiddenTokens());
				return;
			}
			int num = 0;
			while (children != null && num < children.Count)
			{
				ParseTree parseTree = (ParseTree)children[num];
				parseTree._ToStringLeaves(buf);
				num++;
			}
		}

		// Token: 0x040001B7 RID: 439
		public object payload;

		// Token: 0x040001B8 RID: 440
		public IList hiddenTokens;
	}
}
