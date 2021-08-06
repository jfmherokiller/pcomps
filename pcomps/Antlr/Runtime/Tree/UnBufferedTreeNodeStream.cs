using System;
using System.Collections;
using System.Text;
using pcomps.Antlr.Runtime.Collections;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000D7 RID: 215
	public class UnBufferedTreeNodeStream : IIntStream, ITreeNodeStream
	{
		// Token: 0x06000896 RID: 2198 RVA: 0x00018494 File Offset: 0x00016694
		public UnBufferedTreeNodeStream(object tree) : this(new CommonTreeAdaptor(), tree)
		{
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x000184A4 File Offset: 0x000166A4
		public UnBufferedTreeNodeStream(ITreeAdaptor adaptor, object tree)
		{
			this.root = tree;
			this.adaptor = adaptor;
			this.Reset();
			this.down = adaptor.Create(2, "DOWN");
			this.up = adaptor.Create(3, "UP");
			this.eof = adaptor.Create(Token.EOF, "EOF");
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x00018528 File Offset: 0x00016728
		public virtual object TreeSource
		{
			get
			{
				return this.root;
			}
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x00018530 File Offset: 0x00016730
		public virtual void Reset()
		{
			this.currentNode = this.root;
			this.previousNode = null;
			this.currentChildIndex = -1;
			this.absoluteNodeIndex = -1;
			this.head = (this.tail = 0);
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x00018570 File Offset: 0x00016770
		public virtual bool MoveNext()
		{
			if (this.currentNode == null)
			{
				this.AddLookahead(this.eof);
				this.currentEnumerationNode = null;
				return false;
			}
			if (this.currentChildIndex == -1)
			{
				this.currentEnumerationNode = (ITree)this.handleRootNode();
				return true;
			}
			if (this.currentChildIndex < this.adaptor.GetChildCount(this.currentNode))
			{
				this.currentEnumerationNode = (ITree)this.VisitChild(this.currentChildIndex);
				return true;
			}
			this.WalkBackToMostRecentNodeWithUnvisitedChildren();
			if (this.currentNode != null)
			{
				this.currentEnumerationNode = (ITree)this.VisitChild(this.currentChildIndex);
				return true;
			}
			return false;
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600089B RID: 2203 RVA: 0x0001861C File Offset: 0x0001681C
		public virtual object Current
		{
			get
			{
				return this.currentEnumerationNode;
			}
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x00018624 File Offset: 0x00016824
		public virtual object Get(int i)
		{
			throw new NotSupportedException("stream is unbuffered");
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x00018630 File Offset: 0x00016830
		public virtual object LT(int k)
		{
			if (k == -1)
			{
				return this.previousNode;
			}
			if (k < 0)
			{
				throw new ArgumentNullException("tree node streams cannot look backwards more than 1 node", "k");
			}
			if (k == 0)
			{
				return Tree.INVALID_NODE;
			}
			this.fill(k);
			return this.lookahead[(this.head + k - 1) % this.lookahead.Length];
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00018690 File Offset: 0x00016890
		protected internal virtual void fill(int k)
		{
			int lookaheadSize = this.LookaheadSize;
			for (int i = 1; i <= k - lookaheadSize; i++)
			{
				this.MoveNext();
			}
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x000186C0 File Offset: 0x000168C0
		protected internal virtual void AddLookahead(object node)
		{
			this.lookahead[this.tail] = node;
			this.tail = (this.tail + 1) % this.lookahead.Length;
			if (this.tail == this.head)
			{
				object[] destinationArray = new object[2 * this.lookahead.Length];
				int num = this.lookahead.Length - this.head;
				Array.Copy(this.lookahead, this.head, destinationArray, 0, num);
				Array.Copy(this.lookahead, 0, destinationArray, num, this.tail);
				this.lookahead = destinationArray;
				this.head = 0;
				this.tail += num;
			}
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x00018768 File Offset: 0x00016968
		public virtual void Consume()
		{
			this.fill(1);
			this.absoluteNodeIndex++;
			this.previousNode = this.lookahead[this.head];
			this.head = (this.head + 1) % this.lookahead.Length;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x000187B4 File Offset: 0x000169B4
		public virtual int LA(int i)
		{
			object obj = (ITree)this.LT(i);
			if (obj == null)
			{
				return 0;
			}
			return this.adaptor.GetNodeType(obj);
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x000187E4 File Offset: 0x000169E4
		public virtual int Mark()
		{
			if (this.markers == null)
			{
				this.markers = new ArrayList();
				this.markers.Add(null);
			}
			this.markDepth++;
			UnBufferedTreeNodeStream.TreeWalkState treeWalkState;
			if (this.markDepth >= this.markers.Count)
			{
				treeWalkState = new UnBufferedTreeNodeStream.TreeWalkState();
				this.markers.Add(treeWalkState);
			}
			else
			{
				treeWalkState = (UnBufferedTreeNodeStream.TreeWalkState)this.markers[this.markDepth];
			}
			treeWalkState.absoluteNodeIndex = this.absoluteNodeIndex;
			treeWalkState.currentChildIndex = this.currentChildIndex;
			treeWalkState.currentNode = this.currentNode;
			treeWalkState.previousNode = this.previousNode;
			treeWalkState.nodeStackSize = this.nodeStack.Count;
			treeWalkState.indexStackSize = this.indexStack.Count;
			int lookaheadSize = this.LookaheadSize;
			int num = 0;
			treeWalkState.lookahead = new object[lookaheadSize];
			int i = 1;
			while (i <= lookaheadSize)
			{
				treeWalkState.lookahead[num] = this.LT(i);
				i++;
				num++;
			}
			this.lastMarker = this.markDepth;
			return this.markDepth;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x00018908 File Offset: 0x00016B08
		public virtual void Release(int marker)
		{
			this.markDepth = marker;
			this.markDepth--;
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x00018920 File Offset: 0x00016B20
		public virtual void Rewind(int marker)
		{
			if (this.markers == null)
			{
				return;
			}
			UnBufferedTreeNodeStream.TreeWalkState treeWalkState = (UnBufferedTreeNodeStream.TreeWalkState)this.markers[marker];
			this.absoluteNodeIndex = treeWalkState.absoluteNodeIndex;
			this.currentChildIndex = treeWalkState.currentChildIndex;
			this.currentNode = treeWalkState.currentNode;
			this.previousNode = treeWalkState.previousNode;
			this.nodeStack.Capacity = treeWalkState.nodeStackSize;
			this.indexStack.Capacity = treeWalkState.indexStackSize;
			this.head = (this.tail = 0);
			while (this.tail < treeWalkState.lookahead.Length)
			{
				this.lookahead[this.tail] = treeWalkState.lookahead[this.tail];
				this.tail++;
			}
			this.Release(marker);
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x000189F4 File Offset: 0x00016BF4
		public void Rewind()
		{
			this.Rewind(this.lastMarker);
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00018A04 File Offset: 0x00016C04
		public virtual void Seek(int index)
		{
			if (index < this.Index())
			{
				throw new ArgumentOutOfRangeException("can't seek backwards in node stream", "index");
			}
			while (this.Index() < index)
			{
				this.Consume();
			}
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00018A3C File Offset: 0x00016C3C
		public virtual int Index()
		{
			return this.absoluteNodeIndex + 1;
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00018A48 File Offset: 0x00016C48
		[Obsolete("Please use property Count instead.")]
		public virtual int Size()
		{
			return this.Count;
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x00018A50 File Offset: 0x00016C50
		public virtual int Count
		{
			get
			{
				CommonTreeNodeStream commonTreeNodeStream = new CommonTreeNodeStream(this.root);
				return commonTreeNodeStream.Count;
			}
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x00018A70 File Offset: 0x00016C70
		protected internal virtual object handleRootNode()
		{
			object obj = this.currentNode;
			this.currentChildIndex = 0;
			if (this.adaptor.IsNil(obj))
			{
				obj = this.VisitChild(this.currentChildIndex);
			}
			else
			{
				this.AddLookahead(obj);
				if (this.adaptor.GetChildCount(this.currentNode) == 0)
				{
					this.currentNode = null;
				}
			}
			return obj;
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x00018AD4 File Offset: 0x00016CD4
		protected internal virtual object VisitChild(int child)
		{
			this.nodeStack.Push(this.currentNode);
			this.indexStack.Push(child);
			if (child == 0 && !this.adaptor.IsNil(this.currentNode))
			{
				this.AddNavigationNode(2);
			}
			this.currentNode = this.adaptor.GetChild(this.currentNode, child);
			this.currentChildIndex = 0;
			object obj = this.currentNode;
			this.AddLookahead(obj);
			this.WalkBackToMostRecentNodeWithUnvisitedChildren();
			return obj;
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x00018B5C File Offset: 0x00016D5C
		protected internal virtual void AddNavigationNode(int ttype)
		{
			object node;
			if (ttype == 2)
			{
				if (this.HasUniqueNavigationNodes)
				{
					node = this.adaptor.Create(2, "DOWN");
				}
				else
				{
					node = this.down;
				}
			}
			else if (this.HasUniqueNavigationNodes)
			{
				node = this.adaptor.Create(3, "UP");
			}
			else
			{
				node = this.up;
			}
			this.AddLookahead(node);
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x00018BD0 File Offset: 0x00016DD0
		protected internal virtual void WalkBackToMostRecentNodeWithUnvisitedChildren()
		{
			while (this.currentNode != null && this.currentChildIndex >= this.adaptor.GetChildCount(this.currentNode))
			{
				this.currentNode = this.nodeStack.Pop();
				if (this.currentNode == null)
				{
					return;
				}
				this.currentChildIndex = (int)this.indexStack.Pop();
				this.currentChildIndex++;
				if (this.currentChildIndex >= this.adaptor.GetChildCount(this.currentNode))
				{
					if (!this.adaptor.IsNil(this.currentNode))
					{
						this.AddNavigationNode(3);
					}
					if (this.currentNode == this.root)
					{
						this.currentNode = null;
					}
				}
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x00018C9C File Offset: 0x00016E9C
		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return this.adaptor;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x00018CA4 File Offset: 0x00016EA4
		public string SourceName
		{
			get
			{
				return this.TokenStream.SourceName;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060008B0 RID: 2224 RVA: 0x00018CB4 File Offset: 0x00016EB4
		// (set) Token: 0x060008B1 RID: 2225 RVA: 0x00018CBC File Offset: 0x00016EBC
		public ITokenStream TokenStream
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

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060008B2 RID: 2226 RVA: 0x00018CC8 File Offset: 0x00016EC8
		// (set) Token: 0x060008B3 RID: 2227 RVA: 0x00018CD0 File Offset: 0x00016ED0
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

		// Token: 0x060008B4 RID: 2228 RVA: 0x00018CDC File Offset: 0x00016EDC
		public void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
		{
			throw new NotSupportedException("can't do stream rewrites yet");
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x00018CE8 File Offset: 0x00016EE8
		public override string ToString()
		{
			return this.ToString(this.root, null);
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x00018CF8 File Offset: 0x00016EF8
		protected int LookaheadSize
		{
			get
			{
				return (this.tail >= this.head) ? (this.tail - this.head) : (this.lookahead.Length - this.head + this.tail);
			}
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x00018D34 File Offset: 0x00016F34
		public virtual string ToString(object start, object stop)
		{
			if (start == null)
			{
				return null;
			}
			if (this.tokens != null)
			{
				int tokenStartIndex = this.adaptor.GetTokenStartIndex(start);
				int stop2 = this.adaptor.GetTokenStopIndex(stop);
				if (stop != null && this.adaptor.GetNodeType(stop) == 3)
				{
					stop2 = this.adaptor.GetTokenStopIndex(start);
				}
				else
				{
					stop2 = this.Count - 1;
				}
				return this.tokens.ToString(tokenStartIndex, stop2);
			}
			StringBuilder stringBuilder = new StringBuilder();
			this.ToStringWork(start, stop, stringBuilder);
			return stringBuilder.ToString();
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x00018DC4 File Offset: 0x00016FC4
		protected internal virtual void ToStringWork(object p, object stop, StringBuilder buf)
		{
			if (!this.adaptor.IsNil(p))
			{
				string text = this.adaptor.GetNodeText(p);
				if (text == null)
				{
					text = " " + this.adaptor.GetNodeType(p);
				}
				buf.Append(text);
			}
			if (p == stop)
			{
				return;
			}
			int childCount = this.adaptor.GetChildCount(p);
			if (childCount > 0 && !this.adaptor.IsNil(p))
			{
				buf.Append(" ");
				buf.Append(2);
			}
			for (int i = 0; i < childCount; i++)
			{
				object child = this.adaptor.GetChild(p, i);
				this.ToStringWork(child, stop, buf);
			}
			if (childCount > 0 && !this.adaptor.IsNil(p))
			{
				buf.Append(" ");
				buf.Append(3);
			}
		}

		// Token: 0x04000241 RID: 577
		public const int INITIAL_LOOKAHEAD_BUFFER_SIZE = 5;

		// Token: 0x04000242 RID: 578
		private ITree currentEnumerationNode;

		// Token: 0x04000243 RID: 579
		protected bool uniqueNavigationNodes;

		// Token: 0x04000244 RID: 580
		protected internal object root;

		// Token: 0x04000245 RID: 581
		protected ITokenStream tokens;

		// Token: 0x04000246 RID: 582
		private ITreeAdaptor adaptor;

		// Token: 0x04000247 RID: 583
		protected internal StackList nodeStack = new StackList();

		// Token: 0x04000248 RID: 584
		protected internal StackList indexStack = new StackList();

		// Token: 0x04000249 RID: 585
		protected internal object currentNode;

		// Token: 0x0400024A RID: 586
		protected internal object previousNode;

		// Token: 0x0400024B RID: 587
		protected internal int currentChildIndex;

		// Token: 0x0400024C RID: 588
		protected int absoluteNodeIndex;

		// Token: 0x0400024D RID: 589
		protected internal object[] lookahead = new object[5];

		// Token: 0x0400024E RID: 590
		protected internal int head;

		// Token: 0x0400024F RID: 591
		protected internal int tail;

		// Token: 0x04000250 RID: 592
		protected IList markers;

		// Token: 0x04000251 RID: 593
		protected int markDepth;

		// Token: 0x04000252 RID: 594
		protected int lastMarker;

		// Token: 0x04000253 RID: 595
		protected object down;

		// Token: 0x04000254 RID: 596
		protected object up;

		// Token: 0x04000255 RID: 597
		protected object eof;

		// Token: 0x020000D8 RID: 216
		protected class TreeWalkState
		{
			// Token: 0x04000256 RID: 598
			protected internal int currentChildIndex;

			// Token: 0x04000257 RID: 599
			protected internal int absoluteNodeIndex;

			// Token: 0x04000258 RID: 600
			protected internal object currentNode;

			// Token: 0x04000259 RID: 601
			protected internal object previousNode;

			// Token: 0x0400025A RID: 602
			protected internal int nodeStackSize;

			// Token: 0x0400025B RID: 603
			protected internal int indexStackSize;

			// Token: 0x0400025C RID: 604
			protected internal object[] lookahead;
		}
	}
}
