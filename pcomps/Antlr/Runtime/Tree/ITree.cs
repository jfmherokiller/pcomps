using System.Collections;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000A8 RID: 168
	public interface ITree
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000659 RID: 1625
		int ChildCount { get; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600065A RID: 1626
		// (set) Token: 0x0600065B RID: 1627
		ITree Parent { get; set; }

		// Token: 0x0600065C RID: 1628
		bool HasAncestor(int ttype);

		// Token: 0x0600065D RID: 1629
		ITree GetAncestor(int ttype);

		// Token: 0x0600065E RID: 1630
		IList GetAncestors();

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600065F RID: 1631
		// (set) Token: 0x06000660 RID: 1632
		int ChildIndex { get; set; }

		// Token: 0x06000661 RID: 1633
		void FreshenParentAndChildIndexes();

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000662 RID: 1634
		bool IsNil { get; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000663 RID: 1635
		int Type { get; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000664 RID: 1636
		string Text { get; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000665 RID: 1637
		int Line { get; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000666 RID: 1638
		int CharPositionInLine { get; }

		// Token: 0x06000667 RID: 1639
		ITree GetChild(int i);

		// Token: 0x06000668 RID: 1640
		void AddChild(ITree t);

		// Token: 0x06000669 RID: 1641
		void SetChild(int i, ITree t);

		// Token: 0x0600066A RID: 1642
		object DeleteChild(int i);

		// Token: 0x0600066B RID: 1643
		void ReplaceChildren(int startChildIndex, int stopChildIndex, object t);

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600066C RID: 1644
		// (set) Token: 0x0600066D RID: 1645
		int TokenStartIndex { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600066E RID: 1646
		// (set) Token: 0x0600066F RID: 1647
		int TokenStopIndex { get; set; }

		// Token: 0x06000670 RID: 1648
		ITree DupNode();

		// Token: 0x06000671 RID: 1649
		string ToStringTree();

		// Token: 0x06000672 RID: 1650
		string ToString();
	}
}
