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
			this.index = input.Index();
			if (input is ITokenStream)
			{
				this.token = ((ITokenStream)input).LT(1);
				this.line = this.token.Line;
				this.charPositionInLine = this.token.CharPositionInLine;
			}
			if (input is ITreeNodeStream)
			{
				this.ExtractInformationFromTreeNodeStream(input);
			}
			else if (input is ICharStream)
			{
				this.c = input.LA(1);
				this.line = ((ICharStream)input).Line;
				this.charPositionInLine = ((ICharStream)input).CharPositionInLine;
			}
			else
			{
				this.c = input.LA(1);
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000565 RID: 1381 RVA: 0x00010034 File Offset: 0x0000E234
		// (set) Token: 0x06000566 RID: 1382 RVA: 0x0001003C File Offset: 0x0000E23C
		public IIntStream Input
		{
			get
			{
				return this.input;
			}
			set
			{
				this.input = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x00010048 File Offset: 0x0000E248
		// (set) Token: 0x06000568 RID: 1384 RVA: 0x00010050 File Offset: 0x0000E250
		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x0001005C File Offset: 0x0000E25C
		// (set) Token: 0x0600056A RID: 1386 RVA: 0x00010064 File Offset: 0x0000E264
		public IToken Token
		{
			get
			{
				return this.token;
			}
			set
			{
				this.token = value;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x00010070 File Offset: 0x0000E270
		// (set) Token: 0x0600056C RID: 1388 RVA: 0x00010078 File Offset: 0x0000E278
		public object Node
		{
			get
			{
				return this.node;
			}
			set
			{
				this.node = value;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x00010084 File Offset: 0x0000E284
		// (set) Token: 0x0600056E RID: 1390 RVA: 0x0001008C File Offset: 0x0000E28C
		public int Char
		{
			get
			{
				return this.c;
			}
			set
			{
				this.c = value;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x00010098 File Offset: 0x0000E298
		// (set) Token: 0x06000570 RID: 1392 RVA: 0x000100A0 File Offset: 0x0000E2A0
		public int CharPositionInLine
		{
			get
			{
				return this.charPositionInLine;
			}
			set
			{
				this.charPositionInLine = value;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x000100AC File Offset: 0x0000E2AC
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x000100B4 File Offset: 0x0000E2B4
		public int Line
		{
			get
			{
				return this.line;
			}
			set
			{
				this.line = value;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x000100C0 File Offset: 0x0000E2C0
		public virtual int UnexpectedType
		{
			get
			{
				if (this.input is ITokenStream)
				{
					return this.token.Type;
				}
				if (this.input is ITreeNodeStream)
				{
					ITreeNodeStream treeNodeStream = (ITreeNodeStream)this.input;
					ITreeAdaptor treeAdaptor = treeNodeStream.TreeAdaptor;
					return treeAdaptor.GetNodeType(this.node);
				}
				return this.c;
			}
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x00010120 File Offset: 0x0000E320
		protected void ExtractInformationFromTreeNodeStream(IIntStream input)
		{
			ITreeNodeStream treeNodeStream = (ITreeNodeStream)input;
			this.node = treeNodeStream.LT(1);
			ITreeAdaptor treeAdaptor = treeNodeStream.TreeAdaptor;
			IToken token = treeAdaptor.GetToken(this.node);
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
							this.line = token2.Line;
							this.charPositionInLine = token2.CharPositionInLine;
							this.approximateLineInfo = true;
							break;
						}
						num--;
					}
				}
				else
				{
					this.line = token.Line;
					this.charPositionInLine = token.CharPositionInLine;
				}
			}
			else if (this.node is ITree)
			{
				this.line = ((ITree)this.node).Line;
				this.charPositionInLine = ((ITree)this.node).CharPositionInLine;
				if (this.node is CommonTree)
				{
					this.token = ((CommonTree)this.node).Token;
				}
			}
			else
			{
				int nodeType = treeAdaptor.GetNodeType(this.node);
				string nodeText = treeAdaptor.GetNodeText(this.node);
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
