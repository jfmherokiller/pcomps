using System.Collections;
using System.Text;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr
{
	// Token: 0x02000042 RID: 66
	public class TokenStreamRewriteEngine : TokenStream
	{
		// Token: 0x06000278 RID: 632 RVA: 0x00008660 File Offset: 0x00006860
		public TokenStreamRewriteEngine(TokenStream upstream) : this(upstream, 1000)
		{
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000867C File Offset: 0x0000687C
		public TokenStreamRewriteEngine(TokenStream upstream, int initialSize)
		{
			this.stream = upstream;
			this.tokens = new ArrayList(initialSize);
			this.programs = new Hashtable();
			this.programs["default"] = new ArrayList(100);
			this.lastRewriteTokenIndexes = new Hashtable();
		}

		// Token: 0x0600027A RID: 634 RVA: 0x000086F0 File Offset: 0x000068F0
		public IToken nextToken()
		{
			TokenWithIndex tokenWithIndex;
			do
			{
				tokenWithIndex = (TokenWithIndex)this.stream.nextToken();
				if (tokenWithIndex != null)
				{
					tokenWithIndex.setIndex(this.index);
					if (tokenWithIndex.Type != 1)
					{
						this.tokens.Add(tokenWithIndex);
					}
					this.index++;
				}
			}
			while (tokenWithIndex != null && this.discardMask.member(tokenWithIndex.Type));
			return tokenWithIndex;
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00008758 File Offset: 0x00006958
		public void rollback(int instructionIndex)
		{
			this.rollback("default", instructionIndex);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00008774 File Offset: 0x00006974
		public void rollback(string programName, int instructionIndex)
		{
			ArrayList arrayList = (ArrayList)this.programs[programName];
			if (arrayList != null)
			{
				this.programs[programName] = arrayList.GetRange(0, instructionIndex);
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000087AC File Offset: 0x000069AC
		public void deleteProgram()
		{
			this.deleteProgram("default");
		}

		// Token: 0x0600027E RID: 638 RVA: 0x000087C4 File Offset: 0x000069C4
		public void deleteProgram(string programName)
		{
			this.rollback(programName, 0);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x000087DC File Offset: 0x000069DC
		protected void addToSortedRewriteList(TokenStreamRewriteEngine.RewriteOperation op)
		{
			this.addToSortedRewriteList("default", op);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x000087F8 File Offset: 0x000069F8
		protected void addToSortedRewriteList(string programName, TokenStreamRewriteEngine.RewriteOperation op)
		{
			ArrayList arrayList = (ArrayList)this.getProgram(programName);
			if (op.index >= this.getLastRewriteTokenIndex(programName))
			{
				arrayList.Add(op);
				this.setLastRewriteTokenIndex(programName, op.index);
				return;
			}
			int num = arrayList.BinarySearch(op, TokenStreamRewriteEngine.RewriteOperationComparer.Default);
			if (num < 0)
			{
				arrayList.Insert(-num - 1, op);
			}
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00008854 File Offset: 0x00006A54
		public void insertAfter(IToken t, string text)
		{
			this.insertAfter("default", t, text);
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00008870 File Offset: 0x00006A70
		public void insertAfter(int index, string text)
		{
			this.insertAfter("default", index, text);
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000888C File Offset: 0x00006A8C
		public void insertAfter(string programName, IToken t, string text)
		{
			this.insertAfter(programName, ((TokenWithIndex)t).getIndex(), text);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x000088AC File Offset: 0x00006AAC
		public void insertAfter(string programName, int index, string text)
		{
			this.insertBefore(programName, index + 1, text);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000088C4 File Offset: 0x00006AC4
		public void insertBefore(IToken t, string text)
		{
			this.insertBefore("default", t, text);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x000088E0 File Offset: 0x00006AE0
		public void insertBefore(int index, string text)
		{
			this.insertBefore("default", index, text);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000088FC File Offset: 0x00006AFC
		public void insertBefore(string programName, IToken t, string text)
		{
			this.insertBefore(programName, ((TokenWithIndex)t).getIndex(), text);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000891C File Offset: 0x00006B1C
		public void insertBefore(string programName, int index, string text)
		{
			this.addToSortedRewriteList(programName, new TokenStreamRewriteEngine.InsertBeforeOp(index, text));
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00008938 File Offset: 0x00006B38
		public void replace(int index, string text)
		{
			this.replace("default", index, index, text);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00008954 File Offset: 0x00006B54
		public void replace(int from, int to, string text)
		{
			this.replace("default", from, to, text);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00008970 File Offset: 0x00006B70
		public void replace(IToken indexT, string text)
		{
			this.replace("default", indexT, indexT, text);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000898C File Offset: 0x00006B8C
		public void replace(IToken from, IToken to, string text)
		{
			this.replace("default", from, to, text);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000089A8 File Offset: 0x00006BA8
		public void replace(string programName, int from, int to, string text)
		{
			this.addToSortedRewriteList(new TokenStreamRewriteEngine.ReplaceOp(from, to, text));
		}

		// Token: 0x0600028E RID: 654 RVA: 0x000089C4 File Offset: 0x00006BC4
		public void replace(string programName, IToken from, IToken to, string text)
		{
			this.replace(programName, ((TokenWithIndex)from).getIndex(), ((TokenWithIndex)to).getIndex(), text);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x000089F0 File Offset: 0x00006BF0
		public void delete(int index)
		{
			this.delete("default", index, index);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00008A0C File Offset: 0x00006C0C
		public void delete(int from, int to)
		{
			this.delete("default", from, to);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00008A28 File Offset: 0x00006C28
		public void delete(IToken indexT)
		{
			this.delete("default", indexT, indexT);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00008A44 File Offset: 0x00006C44
		public void delete(IToken from, IToken to)
		{
			this.delete("default", from, to);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00008A60 File Offset: 0x00006C60
		public void delete(string programName, int from, int to)
		{
			this.replace(programName, from, to, null);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00008A78 File Offset: 0x00006C78
		public void delete(string programName, IToken from, IToken to)
		{
			this.replace(programName, from, to, null);
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00008A90 File Offset: 0x00006C90
		public void discard(int ttype)
		{
			this.discardMask.add(ttype);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00008AAC File Offset: 0x00006CAC
		public TokenWithIndex getToken(int i)
		{
			return (TokenWithIndex)this.tokens[i];
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00008ACC File Offset: 0x00006CCC
		public int getTokenStreamSize()
		{
			return this.tokens.Count;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00008AE4 File Offset: 0x00006CE4
		public string ToOriginalString()
		{
			return this.ToOriginalString(0, this.getTokenStreamSize() - 1);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00008B00 File Offset: 0x00006D00
		public string ToOriginalString(int start, int end)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = start;
			while (num >= 0 && num <= end && num < this.tokens.Count)
			{
				stringBuilder.Append(this.getToken(num).getText());
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00008B4C File Offset: 0x00006D4C
		public override string ToString()
		{
			return this.ToString(0, this.getTokenStreamSize());
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00008B68 File Offset: 0x00006D68
		public string ToString(string programName)
		{
			return this.ToString(programName, 0, this.getTokenStreamSize());
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00008B84 File Offset: 0x00006D84
		public string ToString(int start, int end)
		{
			return this.ToString("default", start, end);
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00008BA0 File Offset: 0x00006DA0
		public string ToString(string programName, int start, int end)
		{
			IList list = (IList)this.programs[programName];
			if (list == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = start;
			while (num2 >= 0 && num2 <= end && num2 < this.tokens.Count)
			{
				if (num < list.Count)
				{
					TokenStreamRewriteEngine.RewriteOperation rewriteOperation = (TokenStreamRewriteEngine.RewriteOperation)list[num];
					while (num2 == rewriteOperation.index && num < list.Count)
					{
						num2 = rewriteOperation.execute(stringBuilder);
						num++;
						if (num < list.Count)
						{
							rewriteOperation = (TokenStreamRewriteEngine.RewriteOperation)list[num];
						}
					}
				}
				if (num2 < end)
				{
					stringBuilder.Append(this.getToken(num2).getText());
					num2++;
				}
			}
			for (int i = num; i < list.Count; i++)
			{
				TokenStreamRewriteEngine.RewriteOperation rewriteOperation2 = (TokenStreamRewriteEngine.RewriteOperation)list[i];
				rewriteOperation2.execute(stringBuilder);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00008C88 File Offset: 0x00006E88
		public string ToDebugString()
		{
			return this.ToDebugString(0, this.getTokenStreamSize());
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00008CA4 File Offset: 0x00006EA4
		public string ToDebugString(int start, int end)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = start;
			while (num >= 0 && num <= end && num < this.tokens.Count)
			{
				stringBuilder.Append(this.getToken(num));
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00008CEC File Offset: 0x00006EEC
		public int getLastRewriteTokenIndex()
		{
			return this.getLastRewriteTokenIndex("default");
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00008D04 File Offset: 0x00006F04
		protected int getLastRewriteTokenIndex(string programName)
		{
			object obj = this.lastRewriteTokenIndexes[programName];
			if (obj == null)
			{
				return -1;
			}
			return (int)obj;
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00008D2C File Offset: 0x00006F2C
		protected void setLastRewriteTokenIndex(string programName, int i)
		{
			this.lastRewriteTokenIndexes[programName] = i;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00008D4C File Offset: 0x00006F4C
		protected IList getProgram(string name)
		{
			IList list = (IList)this.programs[name];
			if (list == null)
			{
				list = this.initializeProgram(name);
			}
			return list;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00008D78 File Offset: 0x00006F78
		private IList initializeProgram(string name)
		{
			IList list = new ArrayList(100);
			this.programs[name] = list;
			return list;
		}

		// Token: 0x040000C5 RID: 197
		public const int MIN_TOKEN_INDEX = 0;

		// Token: 0x040000C6 RID: 198
		public const string DEFAULT_PROGRAM_NAME = "default";

		// Token: 0x040000C7 RID: 199
		public const int PROGRAM_INIT_SIZE = 100;

		// Token: 0x040000C8 RID: 200
		protected IList tokens;

		// Token: 0x040000C9 RID: 201
		protected IDictionary programs = null;

		// Token: 0x040000CA RID: 202
		protected IDictionary lastRewriteTokenIndexes = null;

		// Token: 0x040000CB RID: 203
		protected int index = 0;

		// Token: 0x040000CC RID: 204
		protected TokenStream stream;

		// Token: 0x040000CD RID: 205
		protected BitSet discardMask = new BitSet();

		// Token: 0x02000043 RID: 67
		protected class RewriteOperation
		{
			// Token: 0x060002A5 RID: 677 RVA: 0x00008D9C File Offset: 0x00006F9C
			protected RewriteOperation(int index, string text)
			{
				this.index = index;
				this.text = text;
			}

			// Token: 0x060002A6 RID: 678 RVA: 0x00008DC0 File Offset: 0x00006FC0
			public virtual int execute(StringBuilder buf)
			{
				return this.index;
			}

			// Token: 0x040000CE RID: 206
			protected internal int index;

			// Token: 0x040000CF RID: 207
			protected internal string text;
		}

		// Token: 0x02000044 RID: 68
		protected class InsertBeforeOp : TokenStreamRewriteEngine.RewriteOperation
		{
			// Token: 0x060002A7 RID: 679 RVA: 0x00008DD4 File Offset: 0x00006FD4
			public InsertBeforeOp(int index, string text) : base(index, text)
			{
			}

			// Token: 0x060002A8 RID: 680 RVA: 0x00008DEC File Offset: 0x00006FEC
			public override int execute(StringBuilder buf)
			{
				buf.Append(this.text);
				return this.index;
			}
		}

		// Token: 0x02000045 RID: 69
		protected class ReplaceOp : TokenStreamRewriteEngine.RewriteOperation
		{
			// Token: 0x060002A9 RID: 681 RVA: 0x00008E0C File Offset: 0x0000700C
			public ReplaceOp(int from, int to, string text) : base(from, text)
			{
				this.lastIndex = to;
			}

			// Token: 0x060002AA RID: 682 RVA: 0x00008E28 File Offset: 0x00007028
			public override int execute(StringBuilder buf)
			{
				if (this.text != null)
				{
					buf.Append(this.text);
				}
				return this.lastIndex + 1;
			}

			// Token: 0x040000D0 RID: 208
			protected int lastIndex;
		}

		// Token: 0x02000046 RID: 70
		protected class DeleteOp : TokenStreamRewriteEngine.ReplaceOp
		{
			// Token: 0x060002AB RID: 683 RVA: 0x00008E54 File Offset: 0x00007054
			public DeleteOp(int from, int to) : base(from, to, null)
			{
			}
		}

		// Token: 0x02000047 RID: 71
		public class RewriteOperationComparer : IComparer
		{
			// Token: 0x060002AC RID: 684 RVA: 0x00008E6C File Offset: 0x0000706C
			public virtual int Compare(object o1, object o2)
			{
				TokenStreamRewriteEngine.RewriteOperation rewriteOperation = (TokenStreamRewriteEngine.RewriteOperation)o1;
				TokenStreamRewriteEngine.RewriteOperation rewriteOperation2 = (TokenStreamRewriteEngine.RewriteOperation)o2;
				if (rewriteOperation.index < rewriteOperation2.index)
				{
					return -1;
				}
				if (rewriteOperation.index > rewriteOperation2.index)
				{
					return 1;
				}
				return 0;
			}

			// Token: 0x040000D1 RID: 209
			public static readonly TokenStreamRewriteEngine.RewriteOperationComparer Default = new TokenStreamRewriteEngine.RewriteOperationComparer();
		}
	}
}
