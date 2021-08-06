using System;
using System.Collections;
using System.Text;
using pcomps.Antlr.Runtime.Collections;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000DC RID: 220
	public class CommonTokenStream : IIntStream, ITokenStream
	{
		// Token: 0x060008F4 RID: 2292 RVA: 0x00019D64 File Offset: 0x00017F64
		public CommonTokenStream()
		{
			this.channel = 0;
			this.tokens = new ArrayList(500);
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00019D98 File Offset: 0x00017F98
		public CommonTokenStream(ITokenSource tokenSource) : this()
		{
			this.tokenSource = tokenSource;
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x00019DA8 File Offset: 0x00017FA8
		public CommonTokenStream(ITokenSource tokenSource, int channel) : this(tokenSource)
		{
			this.channel = channel;
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x00019DB8 File Offset: 0x00017FB8
		public virtual IToken LT(int k)
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
			if (this.p + k - 1 >= this.tokens.Count)
			{
				return Token.EOF_TOKEN;
			}
			int num = this.p;
			for (int i = 1; i < k; i++)
			{
				num = this.SkipOffTokenChannels(num + 1);
			}
			if (num >= this.tokens.Count)
			{
				return Token.EOF_TOKEN;
			}
			return (IToken)this.tokens[num];
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x00019E5C File Offset: 0x0001805C
		public virtual IToken Get(int i)
		{
			return (IToken)this.tokens[i];
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x00019E70 File Offset: 0x00018070
		// (set) Token: 0x060008FA RID: 2298 RVA: 0x00019E78 File Offset: 0x00018078
		public virtual ITokenSource TokenSource
		{
			get
			{
				return this.tokenSource;
			}
			set
			{
				this.tokenSource = value;
				this.tokens.Clear();
				this.p = -1;
				this.channel = 0;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060008FB RID: 2299 RVA: 0x00019EA8 File Offset: 0x000180A8
		public virtual string SourceName
		{
			get
			{
				return this.TokenSource.SourceName;
			}
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x00019EB8 File Offset: 0x000180B8
		public virtual string ToString(int start, int stop)
		{
			if (start < 0 || stop < 0)
			{
				return null;
			}
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			if (stop >= this.tokens.Count)
			{
				stop = this.tokens.Count - 1;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = start; i <= stop; i++)
			{
				IToken token = (IToken)this.tokens[i];
				stringBuilder.Append(token.Text);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x00019F44 File Offset: 0x00018144
		public virtual string ToString(IToken start, IToken stop)
		{
			if (start != null && stop != null)
			{
				return this.ToString(start.TokenIndex, stop.TokenIndex);
			}
			return null;
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00019F74 File Offset: 0x00018174
		public virtual void Consume()
		{
			if (this.p < this.tokens.Count)
			{
				this.p++;
				this.p = this.SkipOffTokenChannels(this.p);
			}
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x00019FB8 File Offset: 0x000181B8
		public virtual int LA(int i)
		{
			return this.LT(i).Type;
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x00019FC8 File Offset: 0x000181C8
		public virtual int Mark()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			this.lastMarker = this.Index();
			return this.lastMarker;
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00019FFC File Offset: 0x000181FC
		public virtual int Index()
		{
			return this.p;
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x0001A004 File Offset: 0x00018204
		public virtual void Rewind(int marker)
		{
			this.Seek(marker);
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x0001A010 File Offset: 0x00018210
		public virtual void Rewind()
		{
			this.Seek(this.lastMarker);
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x0001A020 File Offset: 0x00018220
		public virtual void Reset()
		{
			this.p = 0;
			this.lastMarker = 0;
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0001A030 File Offset: 0x00018230
		public virtual void Release(int marker)
		{
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0001A034 File Offset: 0x00018234
		public virtual void Seek(int index)
		{
			this.p = index;
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0001A040 File Offset: 0x00018240
		[Obsolete("Please use the property Count instead.")]
		public virtual int Size()
		{
			return this.Count;
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000908 RID: 2312 RVA: 0x0001A048 File Offset: 0x00018248
		public virtual int Count
		{
			get
			{
				return this.tokens.Count;
			}
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x0001A058 File Offset: 0x00018258
		protected virtual void FillBuffer()
		{
			int num = 0;
			IToken token = this.tokenSource.NextToken();
			while (token != null && token.Type != -1)
			{
				bool flag = false;
				if (this.channelOverrideMap != null)
				{
					object obj = this.channelOverrideMap[token.Type];
					if (obj != null)
					{
						token.Channel = (int)obj;
					}
				}
				if (this.discardSet != null && this.discardSet.Contains(token.Type.ToString()))
				{
					flag = true;
				}
				else if (this.discardOffChannelTokens && token.Channel != this.channel)
				{
					flag = true;
				}
				if (!flag)
				{
					token.TokenIndex = num;
					this.tokens.Add(token);
					num++;
				}
				token = this.tokenSource.NextToken();
			}
			this.p = 0;
			this.p = this.SkipOffTokenChannels(this.p);
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x0001A154 File Offset: 0x00018354
		protected virtual int SkipOffTokenChannels(int i)
		{
			int count = this.tokens.Count;
			while (i < count && ((IToken)this.tokens[i]).Channel != this.channel)
			{
				i++;
			}
			return i;
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x0001A1A0 File Offset: 0x000183A0
		protected virtual int SkipOffTokenChannelsReverse(int i)
		{
			while (i >= 0 && ((IToken)this.tokens[i]).Channel != this.channel)
			{
				i--;
			}
			return i;
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x0001A1D8 File Offset: 0x000183D8
		public virtual void SetTokenTypeChannel(int ttype, int channel)
		{
			if (this.channelOverrideMap == null)
			{
				this.channelOverrideMap = new Hashtable();
			}
			this.channelOverrideMap[ttype] = channel;
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x0001A208 File Offset: 0x00018408
		public virtual void DiscardTokenType(int ttype)
		{
			if (this.discardSet == null)
			{
				this.discardSet = new HashList();
			}
			this.discardSet.Add(ttype.ToString(), ttype);
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0001A244 File Offset: 0x00018444
		public virtual void DiscardOffChannelTokens(bool discardOffChannelTokens)
		{
			this.discardOffChannelTokens = discardOffChannelTokens;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x0001A250 File Offset: 0x00018450
		public virtual IList GetTokens()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			return this.tokens;
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x0001A26C File Offset: 0x0001846C
		public virtual IList GetTokens(int start, int stop)
		{
			return this.GetTokens(start, stop, new BitSet());
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x0001A278 File Offset: 0x00018478
		public virtual IList GetTokens(int start, int stop, BitSet types)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			if (stop >= this.tokens.Count)
			{
				stop = this.tokens.Count - 1;
			}
			if (start < 0)
			{
				start = 0;
			}
			if (start > stop)
			{
				return null;
			}
			IList list = new ArrayList();
			for (int i = start; i <= stop; i++)
			{
				IToken token = (IToken)this.tokens[i];
				if (types == null || types.Member(token.Type))
				{
					list.Add(token);
				}
			}
			if (list.Count == 0)
			{
				list = null;
			}
			return list;
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x0001A324 File Offset: 0x00018524
		public virtual IList GetTokens(int start, int stop, IList types)
		{
			return this.GetTokens(start, stop, new BitSet(types));
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x0001A334 File Offset: 0x00018534
		public virtual IList GetTokens(int start, int stop, int ttype)
		{
			return this.GetTokens(start, stop, BitSet.Of(ttype));
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0001A344 File Offset: 0x00018544
		protected virtual IToken LB(int k)
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			if (k == 0)
			{
				return null;
			}
			if (this.p - k < 0)
			{
				return null;
			}
			int num = this.p;
			for (int i = 1; i <= k; i++)
			{
				num = this.SkipOffTokenChannelsReverse(num - 1);
			}
			if (num < 0)
			{
				return null;
			}
			return (IToken)this.tokens[num];
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x0001A3B8 File Offset: 0x000185B8
		public override string ToString()
		{
			if (this.p == -1)
			{
				this.FillBuffer();
			}
			return this.ToString(0, this.tokens.Count - 1);
		}

		// Token: 0x04000266 RID: 614
		protected ITokenSource tokenSource;

		// Token: 0x04000267 RID: 615
		protected IList tokens;

		// Token: 0x04000268 RID: 616
		protected IDictionary channelOverrideMap;

		// Token: 0x04000269 RID: 617
		protected HashList discardSet;

		// Token: 0x0400026A RID: 618
		protected int channel;

		// Token: 0x0400026B RID: 619
		protected bool discardOffChannelTokens;

		// Token: 0x0400026C RID: 620
		protected int lastMarker;

		// Token: 0x0400026D RID: 621
		protected int p = -1;
	}
}
