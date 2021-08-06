using System;
using pcomps.Antlr.Runtime.Tree;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000094 RID: 148
	[Serializable]
	public class RecognitionException : Exception
	{
		// Token: 0x0600055F RID: 1375 RVA: 0x0000FF2C File Offset: 0x0000E12C
		public RecognitionException() : this(null, null, null)
		{
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0000FF38 File Offset: 0x0000E138
		public RecognitionException(string message) : this(message, null, null)
		{
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0000FF44 File Offset: 0x0000E144
		public RecognitionException(string message, Exception inner) : this(message, inner, null)
		{
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x0000FF50 File Offset: 0x0000E150
		public RecognitionException(IIntStream input) : this(null, null, input)
		{
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0000FF5C File Offset: 0x0000E15C
		public RecognitionException(string message, IIntStream input) : this(message, null, input)
		{
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0000FF68 File Offset: 0x0000E168
		public RecognitionException(string message, Exception inner, IIntStream input) : base(message, inner)
		{
			this.input = input;
			index = input.Index();
			if (input is ITokenStream)
			{
				token = ((ITokenStream)input).LT(1);
				line = token.Line;
				charPositionInLine = token.CharPositionInLine;
			}
			if (input is ITreeNodeStream)
			{
				ExtractInformationFromTreeNodeStream(input);
			}
			else if (input is ICharStream)
			{
				c = input.LA(1);
				line = ((ICharStream)input).Line;
				charPositionInLine = ((ICharStream)input).CharPositionInLine;
			}
			else
			{
				c = input.LA(1);
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000565 RID: 1381 RVA: 0x00010034 File Offset: 0x0000E234
		// (set) Token: 0x06000566 RID: 1382 RVA: 0x0001003C File Offset: 0x0000E23C
		public IIntStream Input
		{
			get
			{
				return input;
			}
			set
			{
				input = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x00010048 File Offset: 0x0000E248
		// (set) Token: 0x06000568 RID: 1384 RVA: 0x00010050 File Offset: 0x0000E250
		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x0001005C File Offset: 0x0000E25C
		// (set) Token: 0x0600056A RID: 1386 RVA: 0x00010064 File Offset: 0x0000E264
		public IToken Token
		{
			get
			{
				return token;
			}
			set
			{
				token = value;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x00010070 File Offset: 0x0000E270
		// (set) Token: 0x0600056C RID: 1388 RVA: 0x00010078 File Offset: 0x0000E278
		public object Node
		{
			get
			{
				return node;
			}
			set
			{
				node = value;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x00010084 File Offset: 0x0000E284
		// (set) Token: 0x0600056E RID: 1390 RVA: 0x0001008C File Offset: 0x0000E28C
		public int Char
		{
			get
			{
				return c;
			}
			set
			{
				c = value;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x00010098 File Offset: 0x0000E298
		// (set) Token: 0x06000570 RID: 1392 RVA: 0x000100A0 File Offset: 0x0000E2A0
		public int CharPositionInLine
		{
			get
			{
				return charPositionInLine;
			}
			set
			{
				charPositionInLine = value;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x000100AC File Offset: 0x0000E2AC
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x000100B4 File Offset: 0x0000E2B4
		public int Line
		{
			get
			{
				return line;
			}
			set
			{
				line = value;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x000100C0 File Offset: 0x0000E2C0
		public virtual int UnexpectedType
		{
			get
			{
				if (input is ITokenStream)
				{
					return token.Type;
				}
				if (input is ITreeNodeStream)
				{
					ITreeNodeStream treeNodeStream = (ITreeNodeStream)input;
					ITreeAdaptor treeAdaptor = treeNodeStream.TreeAdaptor;
					return treeAdaptor.GetNodeType(node);
				}
				return c;
			}
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x00010120 File Offset: 0x0000E320
		protected void ExtractInformationFromTreeNodeStream(IIntStream input)
		{
			ITreeNodeStream treeNodeStream = (ITreeNodeStream)input;
			node = treeNodeStream.LT(1);
			ITreeAdaptor treeAdaptor = treeNodeStream.TreeAdaptor;
			IToken token = treeAdaptor.GetToken(node);
			if (token != null)
			{
				this.token = token;
				if (token.Line <= 0)
				{
					int num = -1;
					for (object treeNode = treeNodeStream.LT(num); treeNode != null; treeNode = treeNodeStream.LT(num))
					{
						IToken token2 = treeAdaptor.GetToken(treeNode);
						if (token2 != null && token2.Line > 0)
						{
							line = token2.Line;
							charPositionInLine = token2.CharPositionInLine;
							approximateLineInfo = true;
							break;
						}
						num--;
					}
				}
				else
				{
					line = token.Line;
					charPositionInLine = token.CharPositionInLine;
				}
			}
			else if (node is ITree)
			{
				line = ((ITree)node).Line;
				charPositionInLine = ((ITree)node).CharPositionInLine;
				if (node is CommonTree)
				{
					this.token = ((CommonTree)node).Token;
				}
			}
			else
			{
				int nodeType = treeAdaptor.GetNodeType(node);
				string nodeText = treeAdaptor.GetNodeText(node);
				this.token = new CommonToken(nodeType, nodeText);
			}
		}

		// Token: 0x04000176 RID: 374
		[NonSerialized]
		protected IIntStream input;

		// Token: 0x04000177 RID: 375
		protected int index;

		// Token: 0x04000178 RID: 376
		protected IToken token;

		// Token: 0x04000179 RID: 377
		protected object node;

		// Token: 0x0400017A RID: 378
		protected int c;

		// Token: 0x0400017B RID: 379
		protected int line;

		// Token: 0x0400017C RID: 380
		protected int charPositionInLine;

		// Token: 0x0400017D RID: 381
		public bool approximateLineInfo;
	}
}
