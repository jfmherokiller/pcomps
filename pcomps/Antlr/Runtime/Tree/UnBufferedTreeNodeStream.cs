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
			root = tree;
			this.adaptor = adaptor;
			Reset();
			down = adaptor.Create(2, "DOWN");
			up = adaptor.Create(3, "UP");
			eof = adaptor.Create(Token.EOF, "EOF");
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x00018528 File Offset: 0x00016728
		public virtual object TreeSource
		{
			get
			{
				return root;
			}
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x00018530 File Offset: 0x00016730
		public virtual void Reset()
		{
			currentNode = root;
			previousNode = null;
			currentChildIndex = -1;
			absoluteNodeIndex = -1;
			head = (tail = 0);
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x00018570 File Offset: 0x00016770
		public virtual bool MoveNext()
		{
			if (currentNode == null)
			{
				AddLookahead(eof);
				currentEnumerationNode = null;
				return false;
			}
			if (currentChildIndex == -1)
			{
				currentEnumerationNode = (ITree)handleRootNode();
				return true;
			}
			if (currentChildIndex < adaptor.GetChildCount(currentNode))
			{
				currentEnumerationNode = (ITree)VisitChild(currentChildIndex);
				return true;
			}
			WalkBackToMostRecentNodeWithUnvisitedChildren();
			if (currentNode != null)
			{
				currentEnumerationNode = (ITree)VisitChild(currentChildIndex);
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
				return currentEnumerationNode;
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
				return previousNode;
			}
			if (k < 0)
			{
				throw new ArgumentNullException("tree node streams cannot look backwards more than 1 node", "k");
			}
			if (k == 0)
			{
				return Tree.INVALID_NODE;
			}
			fill(k);
			return lookahead[(head + k - 1) % lookahead.Length];
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x00018690 File Offset: 0x00016890
		protected internal virtual void fill(int k)
		{
			var lookaheadSize = LookaheadSize;
			for (var i = 1; i <= k - lookaheadSize; i++)
			{
				MoveNext();
			}
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x000186C0 File Offset: 0x000168C0
		protected internal virtual void AddLookahead(object node)
		{
			lookahead[tail] = node;
			tail = (tail + 1) % lookahead.Length;
			if (tail == head)
			{
				var destinationArray = new object[2 * lookahead.Length];
				var num = lookahead.Length - head;
				Array.Copy(lookahead, head, destinationArray, 0, num);
				Array.Copy(lookahead, 0, destinationArray, num, tail);
				lookahead = destinationArray;
				head = 0;
				tail += num;
			}
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x00018768 File Offset: 0x00016968
		public virtual void Consume()
		{
			fill(1);
			absoluteNodeIndex++;
			previousNode = lookahead[head];
			head = (head + 1) % lookahead.Length;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x000187B4 File Offset: 0x000169B4
		public virtual int LA(int i)
		{
			object obj = (ITree)LT(i);
			if (obj == null)
			{
				return 0;
			}
			return adaptor.GetNodeType(obj);
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x000187E4 File Offset: 0x000169E4
		public virtual int Mark()
		{
			if (markers == null)
			{
				markers = new ArrayList();
				markers.Add(null);
			}
			markDepth++;
			TreeWalkState treeWalkState;
			if (markDepth >= markers.Count)
			{
				treeWalkState = new TreeWalkState();
				markers.Add(treeWalkState);
			}
			else
			{
				treeWalkState = (TreeWalkState)markers[markDepth];
			}
			treeWalkState.absoluteNodeIndex = absoluteNodeIndex;
			treeWalkState.currentChildIndex = currentChildIndex;
			treeWalkState.currentNode = currentNode;
			treeWalkState.previousNode = previousNode;
			treeWalkState.nodeStackSize = nodeStack.Count;
			treeWalkState.indexStackSize = indexStack.Count;
			var lookaheadSize = LookaheadSize;
			var num = 0;
			treeWalkState.lookahead = new object[lookaheadSize];
			var i = 1;
			while (i <= lookaheadSize)
			{
				treeWalkState.lookahead[num] = LT(i);
				i++;
				num++;
			}
			lastMarker = markDepth;
			return markDepth;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x00018908 File Offset: 0x00016B08
		public virtual void Release(int marker)
		{
			markDepth = marker;
			markDepth--;
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x00018920 File Offset: 0x00016B20
		public virtual void Rewind(int marker)
		{
			if (markers == null)
			{
				return;
			}
			var treeWalkState = (TreeWalkState)markers[marker];
			absoluteNodeIndex = treeWalkState.absoluteNodeIndex;
			currentChildIndex = treeWalkState.currentChildIndex;
			currentNode = treeWalkState.currentNode;
			previousNode = treeWalkState.previousNode;
			nodeStack.Capacity = treeWalkState.nodeStackSize;
			indexStack.Capacity = treeWalkState.indexStackSize;
			head = (tail = 0);
			while (tail < treeWalkState.lookahead.Length)
			{
				lookahead[tail] = treeWalkState.lookahead[tail];
				tail++;
			}
			Release(marker);
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x000189F4 File Offset: 0x00016BF4
		public void Rewind()
		{
			Rewind(lastMarker);
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x00018A04 File Offset: 0x00016C04
		public virtual void Seek(int index)
		{
			if (index < Index())
			{
				throw new ArgumentOutOfRangeException("can't seek backwards in node stream", "index");
			}
			while (Index() < index)
			{
				Consume();
			}
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00018A3C File Offset: 0x00016C3C
		public virtual int Index()
		{
			return absoluteNodeIndex + 1;
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00018A48 File Offset: 0x00016C48
		[Obsolete("Please use property Count instead.")]
		public virtual int Size()
		{
			return Count;
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x00018A50 File Offset: 0x00016C50
		public virtual int Count
		{
			get
			{
				var commonTreeNodeStream = new CommonTreeNodeStream(root);
				return commonTreeNodeStream.Count;
			}
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x00018A70 File Offset: 0x00016C70
		protected internal virtual object handleRootNode()
		{
			var obj = currentNode;
			currentChildIndex = 0;
			if (adaptor.IsNil(obj))
			{
				obj = VisitChild(currentChildIndex);
			}
			else
			{
				AddLookahead(obj);
				if (adaptor.GetChildCount(currentNode) == 0)
				{
					currentNode = null;
				}
			}
			return obj;
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x00018AD4 File Offset: 0x00016CD4
		protected internal virtual object VisitChild(int child)
		{
			nodeStack.Push(currentNode);
			indexStack.Push(child);
			if (child == 0 && !adaptor.IsNil(currentNode))
			{
				AddNavigationNode(2);
			}
			currentNode = adaptor.GetChild(currentNode, child);
			currentChildIndex = 0;
			var obj = currentNode;
			AddLookahead(obj);
			WalkBackToMostRecentNodeWithUnvisitedChildren();
			return obj;
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x00018B5C File Offset: 0x00016D5C
		protected internal virtual void AddNavigationNode(int ttype)
		{
			object node;
			if (ttype == 2)
			{
				if (HasUniqueNavigationNodes)
				{
					node = adaptor.Create(2, "DOWN");
				}
				else
				{
					node = down;
				}
			}
			else if (HasUniqueNavigationNodes)
			{
				node = adaptor.Create(3, "UP");
			}
			else
			{
				node = up;
			}
			AddLookahead(node);
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x00018BD0 File Offset: 0x00016DD0
		protected internal virtual void WalkBackToMostRecentNodeWithUnvisitedChildren()
		{
			while (currentNode != null && currentChildIndex >= adaptor.GetChildCount(currentNode))
			{
				currentNode = nodeStack.Pop();
				if (currentNode == null)
				{
					return;
				}
				currentChildIndex = (int)indexStack.Pop();
				currentChildIndex++;
				if (currentChildIndex >= adaptor.GetChildCount(currentNode))
				{
					if (!adaptor.IsNil(currentNode))
					{
						AddNavigationNode(3);
					}
					if (currentNode == root)
					{
						currentNode = null;
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
				return adaptor;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x00018CA4 File Offset: 0x00016EA4
		public string SourceName
		{
			get
			{
				return TokenStream.SourceName;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060008B0 RID: 2224 RVA: 0x00018CB4 File Offset: 0x00016EB4
		// (set) Token: 0x060008B1 RID: 2225 RVA: 0x00018CBC File Offset: 0x00016EBC
		public ITokenStream TokenStream
		{
			get
			{
				return tokens;
			}
			set
			{
				tokens = value;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060008B2 RID: 2226 RVA: 0x00018CC8 File Offset: 0x00016EC8
		// (set) Token: 0x060008B3 RID: 2227 RVA: 0x00018CD0 File Offset: 0x00016ED0
		public bool HasUniqueNavigationNodes
		{
			get
			{
				return uniqueNavigationNodes;
			}
			set
			{
				uniqueNavigationNodes = value;
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
			return ToString(root, null);
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x00018CF8 File Offset: 0x00016EF8
		protected int LookaheadSize
		{
			get
			{
				return (tail >= head) ? (tail - head) : (lookahead.Length - head + tail);
			}
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x00018D34 File Offset: 0x00016F34
		public virtual string ToString(object start, object stop)
		{
			if (start == null)
			{
				return null;
			}
			if (tokens != null)
			{
				var tokenStartIndex = adaptor.GetTokenStartIndex(start);
				var stop2 = adaptor.GetTokenStopIndex(stop);
				if (stop != null && adaptor.GetNodeType(stop) == 3)
				{
					stop2 = adaptor.GetTokenStopIndex(start);
				}
				else
				{
					stop2 = Count - 1;
				}
				return tokens.ToString(tokenStartIndex, stop2);
			}
			var stringBuilder = new StringBuilder();
			ToStringWork(start, stop, stringBuilder);
			return stringBuilder.ToString();
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x00018DC4 File Offset: 0x00016FC4
		protected internal virtual void ToStringWork(object p, object stop, StringBuilder buf)
		{
			if (!adaptor.IsNil(p))
			{
				var text = adaptor.GetNodeText(p);
				if (text == null)
				{
					text = $" {adaptor.GetNodeType(p)}";
				}
				buf.Append(text);
			}
			if (p == stop)
			{
				return;
			}
			var childCount = adaptor.GetChildCount(p);
			if (childCount > 0 && !adaptor.IsNil(p))
			{
				buf.Append(' ');
				buf.Append(2);
			}
			for (var i = 0; i < childCount; i++)
			{
				var child = adaptor.GetChild(p, i);
				ToStringWork(child, stop, buf);
			}
			if (childCount > 0 && !adaptor.IsNil(p))
			{
				buf.Append(' ');
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
