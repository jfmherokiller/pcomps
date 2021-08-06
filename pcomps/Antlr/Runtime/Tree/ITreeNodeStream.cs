namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000AB RID: 171
	public interface ITreeNodeStream : IIntStream
	{
		// Token: 0x06000694 RID: 1684
		object Get(int i);

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000695 RID: 1685
		object TreeSource { get; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000696 RID: 1686
		ITokenStream TokenStream { get; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000697 RID: 1687
		ITreeAdaptor TreeAdaptor { get; }

		// Token: 0x170000A8 RID: 168
		// (set) Token: 0x06000698 RID: 1688
		bool HasUniqueNavigationNodes { set; }

		// Token: 0x06000699 RID: 1689
		object LT(int k);

		// Token: 0x0600069A RID: 1690
		string ToString(object start, object stop);

		// Token: 0x0600069B RID: 1691
		void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t);
	}
}
