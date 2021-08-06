using System;
using System.Collections;
using System.Text;
using pcomps.Antlr.Runtime.Collections;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000A6 RID: 166
	public class CommonTreeNodeStream : IIntStream, ITreeNodeStream, IEnumerable
	{
		// Token: 0x0600062F RID: 1583 RVA: 0x00011FA4 File Offset: 0x000101A4
		public CommonTreeNodeStream(object tree) : this(new CommonTreeAdaptor(), tree)
		{
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00011FB4 File Offset: 0x000101B4
		public CommonTreeNodeStream(ITreeAdaptor adaptor, object tree) : this(adaptor, tree, 100)
		{
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00011FC0 File Offset: 0x000101C0
		public CommonTreeNodeStream(ITreeAdaptor adaptor, object tree, int initialBufferSize)
		{
			this.root = tree;
			this.adaptor = adaptor;
			this.nodes = new ArrayList(initialBufferSize);
			this.down = adaptor.Create(2, "DOWN");
			this.up = adaptor.Create(3, "UP");
			this.eof = adaptor.Create(Token.EOF, "EOF");
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00012030 File Offset: 0x00010230
		public IEnumerator GetEnumerator()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			return new CommonTreeNodeStream.CommonTreeNodeStreamEnumerator(this);
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0001204C File Offset: 0x0001024C
		protected void FillBuffer()
		{
			this.FillBuffer(this.root);
			this.p = 0;
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00012064 File Offset: 0x00010264
		public void FillBuffer(object t)
		{
			bool flag = this.adaptor.IsNil(t);
			if (!flag)
			{
				this.nodes.Add(t);
			}
			int childCount = this.adaptor.GetChildCount(t);
			if (!flag && childCount > 0)
			{
				this.AddNavigationNode(2);
			}
			for (int i = 0; i < childCount; i++)
			{
				object child = this.adaptor.GetChild(t, i);
				this.FillBuffer(child);
			}
			if (!flag && childCount > 0)
			{
				this.AddNavigationNode(3);
			}
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x000120F0 File Offset: 0x000102F0
		protected int GetNodeIndex(object node)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			for (int i = 0; i < this.nodes.Count; i++)
			{
				object obj = this.nodes[i];
				if (obj == node)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00012144 File Offset: 0x00010344
		protected void AddNavigationNode(int ttype)
		{
			object value;
			if (ttype == 2)
			{
				if (this.HasUniqueNavigationNodes)
				{
					value = this.adaptor.Create(2, "DOWN");
				}
				else
				{
					value = this.down;
				}
			}
			else if (this.HasUniqueNavigationNodes)
			{
				value = this.adaptor.Create(3, "UP");
			}
			else
			{
				value = this.up;
			}
			this.nodes.Add(value);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x000121C0 File Offset: 0x000103C0
		public object Get(int i)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			return this.nodes[i];
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x000121E0 File Offset: 0x000103E0
		public object LT(int k)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			if (k == 0)
			{
				return null;
			}
			if (k < 0)
			{
				return this.LB(-k);
			}
			if (this.p + k - 1 >= this.nodes.Count)
			{
				return this.eof;
			}
			return this.nodes[this.p + k - 1];
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000639 RID: 1593 RVA: 0x00012250 File Offset: 0x00010450
		public virtual object CurrentSymbol
		{
			get
			{
				return this.LT(1);
			}
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0001225C File Offset: 0x0001045C
		protected object LB(int k)
		{
			if (k == 0)
			{
				return null;
			}
			if (this.p - k < 0)
			{
				return null;
			}
			return this.nodes[this.p - k];
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x0001228C File Offset: 0x0001048C
		public virtual object TreeSource
		{
			get
			{
				return this.root;
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x00012294 File Offset: 0x00010494
		public virtual string SourceName
		{
			get
			{
				return this.TokenStream.SourceName;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600063D RID: 1597 RVA: 0x000122A4 File Offset: 0x000104A4
		// (set) Token: 0x0600063E RID: 1598 RVA: 0x000122AC File Offset: 0x000104AC
		public virtual ITokenStream TokenStream
		{
			get
			{
				return this.tokens;
			}
			set
			{
				this.tokens = value;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600063F RID: 1599 RVA: 0x000122B8 File Offset: 0x000104B8
		// (set) Token: 0x06000640 RID: 1600 RVA: 0x000122C0 File Offset: 0x000104C0
		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return this.adaptor;
			}
			set
			{
				this.adaptor = value;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000641 RID: 1601 RVA: 0x000122CC File Offset: 0x000104CC
		// (set) Token: 0x06000642 RID: 1602 RVA: 0x000122D4 File Offset: 0x000104D4
		public bool HasUniqueNavigationNodes
		{
			get
			{
				return this.uniqueNavigationNodes;
			}
			set
			{
				this.uniqueNavigationNodes = value;
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x000122E0 File Offset: 0x000104E0
		public void Push(int index)
		{
			if (this.calls == null)
			{
				this.calls = new StackList();
			}
			this.calls.Push(this.p);
			this.Seek(index);
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00012318 File Offset: 0x00010518
		public int Pop()
		{
			int num = (int)this.calls.Pop();
			this.Seek(num);
			return num;
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00012340 File Offset: 0x00010540
		public void Reset()
		{
			this.p = -1;
			this.lastMarker = 0;
			if (this.calls != null)
			{
				this.calls.Clear();
			}
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x00012374 File Offset: 0x00010574
		public void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
		{
			if (parent != null)
			{
				this.adaptor.ReplaceChildren(parent, startChildIndex, stopChildIndex, t);
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x0001238C File Offset: 0x0001058C
		public virtual void Consume()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			this.p++;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x000123BC File Offset: 0x000105BC
		public virtual int LA(int i)
		{
			return this.adaptor.GetNodeType(this.LT(i));
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x000123D0 File Offset: 0x000105D0
		public virtual int Mark()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			this.lastMarker = this.Index();
			return this.lastMarker;
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00012404 File Offset: 0x00010604
		public virtual void Release(int marker)
		{
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00012408 File Offset: 0x00010608
		public virtual void Rewind(int marker)
		{
			this.Seek(marker);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00012414 File Offset: 0x00010614
		public void Rewind()
		{
			this.Seek(this.lastMarker);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00012424 File Offset: 0x00010624
		public virtual void Seek(int index)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			this.p = index;
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00012440 File Offset: 0x00010640
		public virtual int Index()
		{
			return this.p;
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00012448 File Offset: 0x00010648
		[Obsolete("Please use property Count instead.")]
		public virtual int Size()
		{
			return this.Count;
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000650 RID: 1616 RVA: 0x00012450 File Offset: 0x00010650
		public virtual int Count
		{
			get
			{
				if (this.p == -1)
				{
					this.FillBuffer();
				}
				return this.nodes.Count;
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00012470 File Offset: 0x00010670
		public override string ToString()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.nodes.Count; i++)
			{
				object t = this.nodes[i];
				stringBuilder.Append(" ");
				stringBuilder.Append(this.adaptor.GetNodeType(t));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000124E4 File Offset: 0x000106E4
		public string ToTokenString(int start, int stop)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = start;
			while (num < this.nodes.Count && num <= stop)
			{
				object treeNode = this.nodes[num];
				stringBuilder.Append(" ");
				stringBuilder.Append(this.adaptor.GetToken(treeNode));
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00012560 File Offset: 0x00010760
		public virtual string ToString(object start, object stop)
		{
			Console.Out.WriteLine("ToString");
			if (start == null || stop == null)
			{
				return null;
			}
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			if (start is CommonTree)
			{
				Console.Out.Write($"ToString: {((CommonTree)start).Token}, ");
			}
			else
			{
				Console.Out.WriteLine(start);
			}
			if (stop is CommonTree)
			{
				Console.Out.WriteLine(((CommonTree)stop).Token);
			}
			else
			{
				Console.Out.WriteLine(stop);
			}
			if (this.tokens != null)
			{
				int tokenStartIndex = this.adaptor.GetTokenStartIndex(start);
				int stop2 = this.adaptor.GetTokenStopIndex(stop);
				if (this.adaptor.GetNodeType(stop) == 3)
				{
					stop2 = this.adaptor.GetTokenStopIndex(start);
				}
				else if (this.adaptor.GetNodeType(stop) == Token.EOF)
				{
					stop2 = this.Count - 2;
				}
				return this.tokens.ToString(tokenStartIndex, stop2);
			}
			int i;
			for (i = 0; i < this.nodes.Count; i++)
			{
				object obj = this.nodes[i];
				if (obj == start)
				{
					break;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text;
			for (object obj = this.nodes[i]; obj != stop; obj = this.nodes[i])
			{
				text = this.adaptor.GetNodeText(obj);
				if (text == null)
				{
					text = $" {this.adaptor.GetNodeType(obj)}";
				}
				stringBuilder.Append(text);
				i++;
			}
			text = this.adaptor.GetNodeText(stop);
			if (text == null)
			{
				text = $" {this.adaptor.GetNodeType(stop)}";
			}
			stringBuilder.Append(text);
			return stringBuilder.ToString();
		}

		// Token: 0x040001A6 RID: 422
		public const int DEFAULT_INITIAL_BUFFER_SIZE = 100;

		// Token: 0x040001A7 RID: 423
		public const int INITIAL_CALL_STACK_SIZE = 10;

		// Token: 0x040001A8 RID: 424
		protected object down;

		// Token: 0x040001A9 RID: 425
		protected object up;

		// Token: 0x040001AA RID: 426
		protected object eof;

		// Token: 0x040001AB RID: 427
		protected IList nodes;

		// Token: 0x040001AC RID: 428
		protected internal object root;

		// Token: 0x040001AD RID: 429
		protected ITokenStream tokens;

		// Token: 0x040001AE RID: 430
		private ITreeAdaptor adaptor;

		// Token: 0x040001AF RID: 431
		protected bool uniqueNavigationNodes;

		// Token: 0x040001B0 RID: 432
		protected int p = -1;

		// Token: 0x040001B1 RID: 433
		protected int lastMarker;

		// Token: 0x040001B2 RID: 434
		protected StackList calls;

		// Token: 0x020000A7 RID: 167
		protected sealed class CommonTreeNodeStreamEnumerator : IEnumerator
		{
			// Token: 0x06000654 RID: 1620 RVA: 0x00012768 File Offset: 0x00010968
			internal CommonTreeNodeStreamEnumerator()
			{
			}

			// Token: 0x06000655 RID: 1621 RVA: 0x00012770 File Offset: 0x00010970
			internal CommonTreeNodeStreamEnumerator(CommonTreeNodeStream nodeStream)
			{
				this._nodeStream = nodeStream;
				this.Reset();
			}

			// Token: 0x06000656 RID: 1622 RVA: 0x00012788 File Offset: 0x00010988
			public void Reset()
			{
				this._index = 0;
				this._currentItem = null;
			}

			// Token: 0x1700009A RID: 154
			// (get) Token: 0x06000657 RID: 1623 RVA: 0x00012798 File Offset: 0x00010998
			public object Current
			{
				get
				{
					if (this._currentItem == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._currentItem;
				}
			}

			// Token: 0x06000658 RID: 1624 RVA: 0x000127B8 File Offset: 0x000109B8
			public bool MoveNext()
			{
				if (this._index >= this._nodeStream.nodes.Count)
				{
					int index = this._index;
					this._index++;
					if (index < this._nodeStream.nodes.Count)
					{
						this._currentItem = this._nodeStream.nodes[index];
					}
					this._currentItem = this._nodeStream.eof;
					return true;
				}
				this._currentItem = null;
				return false;
			}

			// Token: 0x040001B3 RID: 435
			private CommonTreeNodeStream _nodeStream;

			// Token: 0x040001B4 RID: 436
			private int _index;

			// Token: 0x040001B5 RID: 437
			private object _currentItem;
		}
	}
}
