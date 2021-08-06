namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000A5 RID: 165
	public class CommonTreeAdaptor : BaseTreeAdaptor
	{
		// Token: 0x0600061E RID: 1566 RVA: 0x00011E00 File Offset: 0x00010000
		public override object DupNode(object t)
		{
            return ((ITree)t)?.DupNode();
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00011E18 File Offset: 0x00010018
		public override object Create(IToken payload)
		{
			return new CommonTree(payload);
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00011E20 File Offset: 0x00010020
		public override IToken CreateToken(int tokenType, string text)
		{
			return new CommonToken(tokenType, text);
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00011E2C File Offset: 0x0001002C
		public override IToken CreateToken(IToken fromToken)
		{
			return new CommonToken(fromToken);
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x00011E34 File Offset: 0x00010034
		public override void SetTokenBoundaries(object t, IToken startToken, IToken stopToken)
		{
			if (t == null)
			{
				return;
			}
			int tokenStartIndex = 0;
			int tokenStopIndex = 0;
			if (startToken != null)
			{
				tokenStartIndex = startToken.TokenIndex;
			}
			if (stopToken != null)
			{
				tokenStopIndex = stopToken.TokenIndex;
			}
			((ITree)t).TokenStartIndex = tokenStartIndex;
			((ITree)t).TokenStopIndex = tokenStopIndex;
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x00011E80 File Offset: 0x00010080
		public override int GetTokenStartIndex(object t)
		{
			if (t == null)
			{
				return -1;
			}
			return ((ITree)t).TokenStartIndex;
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x00011E98 File Offset: 0x00010098
		public override int GetTokenStopIndex(object t)
		{
			if (t == null)
			{
				return -1;
			}
			return ((ITree)t).TokenStopIndex;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x00011EB0 File Offset: 0x000100B0
		public override string GetNodeText(object t)
		{
            return ((ITree)t)?.Text;
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x00011EC8 File Offset: 0x000100C8
		public override int GetNodeType(object t)
		{
			if (t == null)
			{
				return 0;
			}
			return ((ITree)t).Type;
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x00011EE0 File Offset: 0x000100E0
		public override IToken GetToken(object treeNode)
		{
            return (treeNode as CommonTree)?.Token;
        }

		// Token: 0x06000628 RID: 1576 RVA: 0x00011EFC File Offset: 0x000100FC
		public override object GetChild(object t, int i)
		{
            return ((ITree)t)?.GetChild(i);
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00011F14 File Offset: 0x00010114
		public override int GetChildCount(object t)
		{
			if (t == null)
			{
				return 0;
			}
			return ((ITree)t).ChildCount;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00011F2C File Offset: 0x0001012C
		public override object GetParent(object t)
		{
            return ((ITree)t)?.Parent;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00011F44 File Offset: 0x00010144
		public override void SetParent(object t, object parent)
		{
			if (t == null)
			{
				((ITree)t).Parent = (ITree)parent;
			}
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00011F60 File Offset: 0x00010160
		public override int GetChildIndex(object t)
		{
			if (t == null)
			{
				return 0;
			}
			return ((ITree)t).ChildIndex;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x00011F78 File Offset: 0x00010178
		public override void SetChildIndex(object t, int index)
		{
			if (t == null)
			{
				((ITree)t).ChildIndex = index;
			}
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x00011F8C File Offset: 0x0001018C
		public override void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
        {
            ((ITree)parent)?.ReplaceChildren(startChildIndex, stopChildIndex, t);
        }
	}
}
