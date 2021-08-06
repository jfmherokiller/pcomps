using System;
using System.Collections;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000A3 RID: 163
	public abstract class BaseTreeAdaptor : ITreeAdaptor
	{
		// Token: 0x060005E7 RID: 1511 RVA: 0x00011788 File Offset: 0x0000F988
		public virtual object GetNilNode()
		{
			return Create(null);
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x00011794 File Offset: 0x0000F994
		public virtual object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
		{
			return new CommonErrorNode(input, start, stop, e);
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x000117B0 File Offset: 0x0000F9B0
		public virtual bool IsNil(object tree)
		{
			return ((ITree)tree).IsNil;
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x000117C0 File Offset: 0x0000F9C0
		public virtual object DupTree(object tree)
		{
			return DupTree(tree, null);
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x000117CC File Offset: 0x0000F9CC
		public virtual object DupTree(object t, object parent)
		{
			if (t == null)
			{
				return null;
			}
			var obj = DupNode(t);
			SetChildIndex(obj, GetChildIndex(t));
			SetParent(obj, parent);
			var childCount = GetChildCount(t);
			for (var i = 0; i < childCount; i++)
			{
				var child = GetChild(t, i);
				var child2 = DupTree(child, t);
				AddChild(obj, child2);
			}
			return obj;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x00011838 File Offset: 0x0000FA38
		public virtual void AddChild(object t, object child)
		{
			if (t != null && child != null)
			{
				((ITree)t).AddChild((ITree)child);
			}
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00011858 File Offset: 0x0000FA58
		public virtual object BecomeRoot(object newRoot, object oldRoot)
		{
			var tree = (ITree)newRoot;
			var t = (ITree)oldRoot;
			if (oldRoot == null)
			{
				return newRoot;
			}
			if (tree.IsNil)
			{
				var childCount = tree.ChildCount;
				if (childCount == 1)
				{
					tree = tree.GetChild(0);
				}
				else if (childCount > 1)
				{
					throw new SystemException("more than one node as root (TODO: make exception hierarchy)");
				}
			}
			tree.AddChild(t);
			return tree;
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x000118BC File Offset: 0x0000FABC
		public virtual object RulePostProcessing(object root)
		{
			var tree = (ITree)root;
			if (tree != null && tree.IsNil)
			{
				if (tree.ChildCount == 0)
				{
					tree = null;
				}
				else if (tree.ChildCount == 1)
				{
					tree = tree.GetChild(0);
					tree.Parent = null;
					tree.ChildIndex = -1;
				}
			}
			return tree;
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x00011918 File Offset: 0x0000FB18
		public virtual object BecomeRoot(IToken newRoot, object oldRoot)
		{
			return BecomeRoot(Create(newRoot), oldRoot);
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x00011928 File Offset: 0x0000FB28
		public virtual object Create(int tokenType, IToken fromToken)
		{
			fromToken = CreateToken(fromToken);
			fromToken.Type = tokenType;
			return (ITree)Create(fromToken);
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00011954 File Offset: 0x0000FB54
		public virtual object Create(int tokenType, IToken fromToken, string text)
		{
			fromToken = CreateToken(fromToken);
			fromToken.Type = tokenType;
			fromToken.Text = text;
			return (ITree)Create(fromToken);
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00011988 File Offset: 0x0000FB88
		public virtual object Create(int tokenType, string text)
		{
			var param = CreateToken(tokenType, text);
			return (ITree)Create(param);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x000119AC File Offset: 0x0000FBAC
		public virtual int GetNodeType(object t)
		{
			return ((ITree)t).Type;
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x000119BC File Offset: 0x0000FBBC
		public virtual void SetNodeType(object t, int type)
		{
			throw new NotImplementedException("don't know enough about Tree node");
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x000119C8 File Offset: 0x0000FBC8
		public virtual string GetNodeText(object t)
		{
			return ((ITree)t).Text;
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x000119D8 File Offset: 0x0000FBD8
		public virtual void SetNodeText(object t, string text)
		{
			throw new NotImplementedException("don't know enough about Tree node");
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x000119E4 File Offset: 0x0000FBE4
		public virtual object GetChild(object t, int i)
		{
			return ((ITree)t).GetChild(i);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x000119F4 File Offset: 0x0000FBF4
		public virtual void SetChild(object t, int i, object child)
		{
			((ITree)t).SetChild(i, (ITree)child);
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00011A08 File Offset: 0x0000FC08
		public virtual object DeleteChild(object t, int i)
		{
			return ((ITree)t).DeleteChild(i);
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x00011A18 File Offset: 0x0000FC18
		public virtual int GetChildCount(object t)
		{
			return ((ITree)t).ChildCount;
		}

		// Token: 0x060005FB RID: 1531
		public abstract object DupNode(object param1);

		// Token: 0x060005FC RID: 1532
		public abstract object Create(IToken param1);

		// Token: 0x060005FD RID: 1533
		public abstract void SetTokenBoundaries(object param1, IToken param2, IToken param3);

		// Token: 0x060005FE RID: 1534
		public abstract int GetTokenStartIndex(object t);

		// Token: 0x060005FF RID: 1535
		public abstract int GetTokenStopIndex(object t);

		// Token: 0x06000600 RID: 1536
		public abstract IToken GetToken(object treeNode);

		// Token: 0x06000601 RID: 1537 RVA: 0x00011A28 File Offset: 0x0000FC28
		public int GetUniqueID(object node)
		{
			if (treeToUniqueIDMap == null)
			{
				treeToUniqueIDMap = new Hashtable();
			}
			var obj = treeToUniqueIDMap[node];
			if (obj != null)
			{
				return (int)obj;
			}
			var num = uniqueNodeID;
			treeToUniqueIDMap[node] = num;
			uniqueNodeID++;
			return num;
		}

		// Token: 0x06000602 RID: 1538
		public abstract IToken CreateToken(int tokenType, string text);

		// Token: 0x06000603 RID: 1539
		public abstract IToken CreateToken(IToken fromToken);

		// Token: 0x06000604 RID: 1540
		public abstract object GetParent(object t);

		// Token: 0x06000605 RID: 1541
		public abstract void SetParent(object t, object parent);

		// Token: 0x06000606 RID: 1542
		public abstract int GetChildIndex(object t);

		// Token: 0x06000607 RID: 1543
		public abstract void SetChildIndex(object t, int index);

		// Token: 0x06000608 RID: 1544
		public abstract void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t);

		// Token: 0x0400019F RID: 415
		protected IDictionary treeToUniqueIDMap;

		// Token: 0x040001A0 RID: 416
		protected int uniqueNodeID = 1;
	}
}
