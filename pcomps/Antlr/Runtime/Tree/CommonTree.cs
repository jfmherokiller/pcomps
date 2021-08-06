using System;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000A4 RID: 164
	[Serializable]
	public class CommonTree : BaseTree
	{
		// Token: 0x06000609 RID: 1545 RVA: 0x00011A90 File Offset: 0x0000FC90
		public CommonTree()
		{
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00011AB0 File Offset: 0x0000FCB0
		public CommonTree(CommonTree node) : base(node)
		{
			token = node.token;
			startIndex = node.startIndex;
			stopIndex = node.stopIndex;
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00011B00 File Offset: 0x0000FD00
		public CommonTree(IToken t)
		{
			token = t;
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x00011B30 File Offset: 0x0000FD30
		public virtual IToken Token
		{
			get
			{
				return token;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600060D RID: 1549 RVA: 0x00011B38 File Offset: 0x0000FD38
		public override bool IsNil
		{
			get
			{
				return token == null;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x00011B44 File Offset: 0x0000FD44
		public override int Type
		{
			get
			{
				if (token == null)
				{
					return 0;
				}
				return token.Type;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600060F RID: 1551 RVA: 0x00011B60 File Offset: 0x0000FD60
		public override string Text
		{
			get
			{
                return token?.Text;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x00011B7C File Offset: 0x0000FD7C
		public override int Line
		{
			get
			{
				if (token != null && token.Line != 0)
				{
					return token.Line;
				}
				if (ChildCount > 0)
				{
					return GetChild(0).Line;
				}
				return 0;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000611 RID: 1553 RVA: 0x00011BCC File Offset: 0x0000FDCC
		public override int CharPositionInLine
		{
			get
			{
				if (token != null && token.CharPositionInLine != -1)
				{
					return token.CharPositionInLine;
				}
				if (ChildCount > 0)
				{
					return GetChild(0).CharPositionInLine;
				}
				return 0;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x00011C1C File Offset: 0x0000FE1C
		// (set) Token: 0x06000613 RID: 1555 RVA: 0x00011C48 File Offset: 0x0000FE48
		public override int TokenStartIndex
		{
			get
			{
				if (startIndex == -1 && token != null)
				{
					return token.TokenIndex;
				}
				return startIndex;
			}
			set
			{
				startIndex = value;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x00011C54 File Offset: 0x0000FE54
		// (set) Token: 0x06000615 RID: 1557 RVA: 0x00011C80 File Offset: 0x0000FE80
		public override int TokenStopIndex
		{
			get
			{
				if (stopIndex == -1 && token != null)
				{
					return token.TokenIndex;
				}
				return stopIndex;
			}
			set
			{
				stopIndex = value;
			}
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00011C8C File Offset: 0x0000FE8C
		public void SetUnknownTokenBoundaries()
		{
			if (children == null)
			{
				if (startIndex < 0 || stopIndex < 0)
				{
					startIndex = (stopIndex = token.TokenIndex);
				}
				return;
			}
			for (var i = 0; i < children.Count; i++)
			{
				((CommonTree)children[i]).SetUnknownTokenBoundaries();
			}
			if (startIndex >= 0 && stopIndex >= 0)
			{
				return;
			}
			if (children.Count > 0)
			{
				var commonTree = (CommonTree)children[0];
				var commonTree2 = (CommonTree)children[children.Count - 1];
				startIndex = commonTree.TokenStartIndex;
				stopIndex = commonTree2.TokenStopIndex;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000617 RID: 1559 RVA: 0x00011D7C File Offset: 0x0000FF7C
		// (set) Token: 0x06000618 RID: 1560 RVA: 0x00011D84 File Offset: 0x0000FF84
		public override int ChildIndex
		{
			get
			{
				return childIndex;
			}
			set
			{
				childIndex = value;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000619 RID: 1561 RVA: 0x00011D90 File Offset: 0x0000FF90
		// (set) Token: 0x0600061A RID: 1562 RVA: 0x00011D98 File Offset: 0x0000FF98
		public override ITree Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = (CommonTree)value;
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x00011DA8 File Offset: 0x0000FFA8
		public override ITree DupNode()
		{
			return new CommonTree(this);
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00011DB0 File Offset: 0x0000FFB0
		public override string ToString()
		{
			if (IsNil)
			{
				return "nil";
			}
			if (Type == 0)
			{
				return "<errornode>";
			}

            return token?.Text;
		}

		// Token: 0x040001A1 RID: 417
		public int startIndex = -1;

		// Token: 0x040001A2 RID: 418
		public int stopIndex = -1;

		// Token: 0x040001A3 RID: 419
		protected IToken token;

		// Token: 0x040001A4 RID: 420
		public CommonTree parent;

		// Token: 0x040001A5 RID: 421
		public int childIndex = -1;
	}
}
