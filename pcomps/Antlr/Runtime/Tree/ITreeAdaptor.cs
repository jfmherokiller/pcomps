namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000AA RID: 170
	public interface ITreeAdaptor
	{
		// Token: 0x06000675 RID: 1653
		object Create(IToken payload);

		// Token: 0x06000676 RID: 1654
		object DupNode(object treeNode);

		// Token: 0x06000677 RID: 1655
		object DupTree(object tree);

		// Token: 0x06000678 RID: 1656
		object GetNilNode();

		// Token: 0x06000679 RID: 1657
		object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e);

		// Token: 0x0600067A RID: 1658
		bool IsNil(object tree);

		// Token: 0x0600067B RID: 1659
		void AddChild(object t, object child);

		// Token: 0x0600067C RID: 1660
		object BecomeRoot(object newRoot, object oldRoot);

		// Token: 0x0600067D RID: 1661
		object RulePostProcessing(object root);

		// Token: 0x0600067E RID: 1662
		int GetUniqueID(object node);

		// Token: 0x0600067F RID: 1663
		object BecomeRoot(IToken newRoot, object oldRoot);

		// Token: 0x06000680 RID: 1664
		object Create(int tokenType, IToken fromToken);

		// Token: 0x06000681 RID: 1665
		object Create(int tokenType, IToken fromToken, string text);

		// Token: 0x06000682 RID: 1666
		object Create(int tokenType, string text);

		// Token: 0x06000683 RID: 1667
		int GetNodeType(object t);

		// Token: 0x06000684 RID: 1668
		void SetNodeType(object t, int type);

		// Token: 0x06000685 RID: 1669
		string GetNodeText(object t);

		// Token: 0x06000686 RID: 1670
		void SetNodeText(object t, string text);

		// Token: 0x06000687 RID: 1671
		IToken GetToken(object treeNode);

		// Token: 0x06000688 RID: 1672
		void SetTokenBoundaries(object t, IToken startToken, IToken stopToken);

		// Token: 0x06000689 RID: 1673
		int GetTokenStartIndex(object t);

		// Token: 0x0600068A RID: 1674
		int GetTokenStopIndex(object t);

		// Token: 0x0600068B RID: 1675
		object GetChild(object t, int i);

		// Token: 0x0600068C RID: 1676
		void SetChild(object t, int i, object child);

		// Token: 0x0600068D RID: 1677
		object DeleteChild(object t, int i);

		// Token: 0x0600068E RID: 1678
		int GetChildCount(object t);

		// Token: 0x0600068F RID: 1679
		object GetParent(object t);

		// Token: 0x06000690 RID: 1680
		void SetParent(object t, object parent);

		// Token: 0x06000691 RID: 1681
		int GetChildIndex(object t);

		// Token: 0x06000692 RID: 1682
		void SetChildIndex(object t, int index);

		// Token: 0x06000693 RID: 1683
		void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t);
	}
}
