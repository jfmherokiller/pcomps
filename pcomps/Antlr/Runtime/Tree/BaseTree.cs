using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000A2 RID: 162
	[Serializable]
	public abstract class BaseTree : ITree
	{
		// Token: 0x060005C4 RID: 1476 RVA: 0x00011100 File Offset: 0x0000F300
		public BaseTree()
		{
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00011108 File Offset: 0x0000F308
		public BaseTree(ITree node)
		{
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x00011110 File Offset: 0x0000F310
		public virtual int ChildCount
		{
			get
			{
				if (children == null)
				{
					return 0;
				}
				return children.Count;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x0001112C File Offset: 0x0000F32C
		public virtual bool IsNil
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060005C8 RID: 1480 RVA: 0x00011130 File Offset: 0x0000F330
		public virtual int Line
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x00011134 File Offset: 0x0000F334
		public virtual int CharPositionInLine
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00011138 File Offset: 0x0000F338
		public virtual ITree GetChild(int i)
		{
			if (children == null || i >= children.Count)
			{
				return null;
			}
			return (ITree)children[i];
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x0001116C File Offset: 0x0000F36C
		public IList Children
		{
			get
			{
				return children;
			}
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x00011174 File Offset: 0x0000F374
		public virtual void AddChild(ITree t)
		{
			if (t == null)
			{
				return;
			}
			BaseTree baseTree = (BaseTree)t;
			if (baseTree.IsNil)
			{
				if (children != null && children == baseTree.children)
				{
					throw new InvalidOperationException("attempt to add child list to itself");
				}
				if (baseTree.children != null)
				{
					if (children != null)
					{
						int count = baseTree.children.Count;
						for (int i = 0; i < count; i++)
						{
							ITree tree = (ITree)baseTree.Children[i];
							children.Add(tree);
							tree.Parent = this;
							tree.ChildIndex = children.Count - 1;
						}
					}
					else
					{
						children = baseTree.children;
						FreshenParentAndChildIndexes();
					}
				}
			}
			else
			{
				if (children == null)
				{
					children = CreateChildrenList();
				}
				children.Add(t);
				baseTree.Parent = this;
				baseTree.ChildIndex = children.Count - 1;
			}
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00011288 File Offset: 0x0000F488
		public void AddChildren(IList kids)
		{
			for (int i = 0; i < kids.Count; i++)
			{
				ITree t = (ITree)kids[i];
				AddChild(t);
			}
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x000112C0 File Offset: 0x0000F4C0
		public virtual void SetChild(int i, ITree t)
		{
			if (t == null)
			{
				return;
			}
			if (t.IsNil)
			{
				throw new ArgumentException("Can't set single child to a list");
			}
			if (children == null)
			{
				children = CreateChildrenList();
			}
			children[i] = t;
			t.Parent = this;
			t.ChildIndex = i;
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x0001131C File Offset: 0x0000F51C
		public virtual object DeleteChild(int i)
		{
			if (children == null)
			{
				return null;
			}
			ITree result = (ITree)children[i];
			children.RemoveAt(i);
			FreshenParentAndChildIndexes(i);
			return result;
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x0001135C File Offset: 0x0000F55C
		public virtual void ReplaceChildren(int startChildIndex, int stopChildIndex, object t)
		{
			if (children == null)
			{
				throw new ArgumentException("indexes invalid; no children in list");
			}
			int num = stopChildIndex - startChildIndex + 1;
			BaseTree baseTree = (BaseTree)t;
			IList list;
			if (baseTree.IsNil)
			{
				list = baseTree.Children;
			}
			else
			{
				list = new ArrayList(1);
				list.Add(baseTree);
			}
			int count = list.Count;
			int count2 = list.Count;
			int num2 = num - count;
			if (num2 == 0)
			{
				int num3 = 0;
				for (int i = startChildIndex; i <= stopChildIndex; i++)
				{
					BaseTree baseTree2 = (BaseTree)list[num3];
					children[i] = baseTree2;
					baseTree2.Parent = this;
					baseTree2.ChildIndex = i;
					num3++;
				}
			}
			else if (num2 > 0)
			{
				for (int j = 0; j < count2; j++)
				{
					children[startChildIndex + j] = list[j];
				}
				int num4 = startChildIndex + count2;
				for (int k = num4; k <= stopChildIndex; k++)
				{
					children.RemoveAt(num4);
				}
				FreshenParentAndChildIndexes(startChildIndex);
			}
			else
			{
				int l;
				for (l = 0; l < num; l++)
				{
					children[startChildIndex + l] = list[l];
				}
				while (l < count)
				{
					children.Insert(startChildIndex + l, list[l]);
					l++;
				}
				FreshenParentAndChildIndexes(startChildIndex);
			}
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x000114EC File Offset: 0x0000F6EC
		protected internal virtual IList CreateChildrenList()
		{
			return new ArrayList();
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x000114F4 File Offset: 0x0000F6F4
		public virtual void FreshenParentAndChildIndexes()
		{
			FreshenParentAndChildIndexes(0);
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x00011500 File Offset: 0x0000F700
		public virtual void FreshenParentAndChildIndexes(int offset)
		{
			int childCount = ChildCount;
			for (int i = offset; i < childCount; i++)
			{
				ITree child = GetChild(i);
				child.ChildIndex = i;
				child.Parent = this;
			}
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x0001153C File Offset: 0x0000F73C
		public virtual void SanityCheckParentAndChildIndexes()
		{
			SanityCheckParentAndChildIndexes(null, -1);
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x00011548 File Offset: 0x0000F748
		public virtual void SanityCheckParentAndChildIndexes(ITree parent, int i)
		{
			if (parent != Parent)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"parents don't match; expected ",
					parent,
					" found ",
					Parent
				}));
			}
			if (i != ChildIndex)
			{
				throw new NotSupportedException(string.Concat(new object[]
				{
					"child indexes don't match; expected ",
					i,
					" found ",
					ChildIndex
				}));
			}
			int childCount = ChildCount;
			for (int j = 0; j < childCount; j++)
			{
				CommonTree commonTree = (CommonTree)GetChild(j);
				commonTree.SanityCheckParentAndChildIndexes(this, j);
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x00011604 File Offset: 0x0000F804
		// (set) Token: 0x060005D7 RID: 1495 RVA: 0x00011608 File Offset: 0x0000F808
		public virtual int ChildIndex
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060005D8 RID: 1496 RVA: 0x0001160C File Offset: 0x0000F80C
		// (set) Token: 0x060005D9 RID: 1497 RVA: 0x00011610 File Offset: 0x0000F810
		public virtual ITree Parent
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x00011614 File Offset: 0x0000F814
		public bool HasAncestor(int ttype)
		{
			return GetAncestor(ttype) != null;
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x00011624 File Offset: 0x0000F824
		public ITree GetAncestor(int ttype)
		{
			for (ITree parent = ((ITree)this).Parent; parent != null; parent = parent.Parent)
			{
				if (parent.Type == ttype)
				{
					return parent;
				}
			}
			return null;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x0001165C File Offset: 0x0000F85C
		public IList GetAncestors()
		{
			if (Parent == null)
			{
				return null;
			}
			IList list = new ArrayList();
			for (ITree parent = ((ITree)this).Parent; parent != null; parent = parent.Parent)
			{
				list.Insert(0, parent);
			}
			return list;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x000116A0 File Offset: 0x0000F8A0
		public virtual string ToStringTree()
		{
			if (children == null || children.Count == 0)
			{
				return ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!IsNil)
			{
				stringBuilder.Append("(");
				stringBuilder.Append(ToString());
				stringBuilder.Append(' ');
			}
			int num = 0;
			while (children != null && num < children.Count)
			{
				ITree tree = (ITree)children[num];
				if (num > 0)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append(tree.ToStringTree());
				num++;
			}
			if (!IsNil)
			{
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060005DE RID: 1502
		public abstract override string ToString();

		// Token: 0x060005DF RID: 1503
		public abstract ITree DupNode();

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060005E0 RID: 1504
		public abstract int Type { get; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060005E1 RID: 1505
		// (set) Token: 0x060005E2 RID: 1506
		public abstract int TokenStartIndex { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060005E3 RID: 1507
		// (set) Token: 0x060005E4 RID: 1508
		public abstract int TokenStopIndex { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060005E5 RID: 1509
		public abstract string Text { get; }

		// Token: 0x0400019E RID: 414
		protected IList children;
	}
}
