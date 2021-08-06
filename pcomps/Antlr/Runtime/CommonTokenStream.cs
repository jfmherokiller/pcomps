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
			channel = 0;
			tokens = new ArrayList(500);
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
			if (p == -1)
			{
				FillBuffer();
			}
			if (k == 0)
			{
				return null;
			}
			if (k < 0)
			{
				return LB(-k);
			}
			if (p + k - 1 >= tokens.Count)
			{
				return Token.EOF_TOKEN;
			}
			var num = p;
			for (var i = 1; i < k; i++)
			{
				num = SkipOffTokenChannels(num + 1);
			}
			if (num >= tokens.Count)
			{
				return Token.EOF_TOKEN;
			}
			return (IToken)tokens[num];
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x00019E5C File Offset: 0x0001805C
		public virtual IToken Get(int i)
		{
			return (IToken)tokens[i];
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x00019E70 File Offset: 0x00018070
		// (set) Token: 0x060008FA RID: 2298 RVA: 0x00019E78 File Offset: 0x00018078
		public virtual ITokenSource TokenSource
		{
			get
			{
				return tokenSource;
			}
			set
			{
				tokenSource = value;
				tokens.Clear();
				p = -1;
				channel = 0;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060008FB RID: 2299 RVA: 0x00019EA8 File Offset: 0x000180A8
		public virtual string SourceName
		{
			get
			{
				return TokenSource.SourceName;
			}
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x00019EB8 File Offset: 0x000180B8
		public virtual string ToString(int start, int stop)
		{
			if (start < 0 || stop < 0)
			{
				return null;
			}
			if (p == -1)
			{
				FillBuffer();
			}
			if (stop >= tokens.Count)
			{
				stop = tokens.Count - 1;
			}
			var stringBuilder = new StringBuilder();
			for (var i = start; i <= stop; i++)
			{
				var token = (IToken)tokens[i];
				stringBuilder.Append(token.Text);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x00019F44 File Offset: 0x00018144
		public virtual string ToString(IToken start, IToken stop)
		{
			if (start != null && stop != null)
			{
				return ToString(start.TokenIndex, stop.TokenIndex);
			}
			return null;
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00019F74 File Offset: 0x00018174
		public virtual void Consume()
		{
			if (p < tokens.Count)
			{
				p++;
				p = SkipOffTokenChannels(p);
			}
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x00019FB8 File Offset: 0x000181B8
		public virtual int LA(int i)
		{
			return LT(i).Type;
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x00019FC8 File Offset: 0x000181C8
		public virtual int Mark()
		{
			if (p == -1)
			{
				FillBuffer();
			}
			lastMarker = Index();
			return lastMarker;
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00019FFC File Offset: 0x000181FC
		public virtual int Index()
		{
			return p;
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x0001A004 File Offset: 0x00018204
		public virtual void Rewind(int marker)
		{
			Seek(marker);
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x0001A010 File Offset: 0x00018210
		public virtual void Rewind()
		{
			Seek(lastMarker);
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x0001A020 File Offset: 0x00018220
		public virtual void Reset()
		{
			p = 0;
			lastMarker = 0;
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0001A030 File Offset: 0x00018230
		public virtual void Release(int marker)
		{
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0001A034 File Offset: 0x00018234
		public virtual void Seek(int index)
		{
			p = index;
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0001A040 File Offset: 0x00018240
		[Obsolete("Please use the property Count instead.")]
		public virtual int Size()
		{
			return Count;
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000908 RID: 2312 RVA: 0x0001A048 File Offset: 0x00018248
		public virtual int Count
		{
			get
			{
				return tokens.Count;
			}
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x0001A058 File Offset: 0x00018258
		protected virtual void FillBuffer()
		{
			var num = 0;
			var token = tokenSource.NextToken();
			while (token != null && token.Type != -1)
			{
				var flag = false;
                var obj = channelOverrideMap?[token.Type];
                if (obj != null)
                {
                    token.Channel = (int)obj;
                }
                if (discardSet != null && discardSet.Contains(token.Type.ToString()))
				{
					flag = true;
				}
				else if (discardOffChannelTokens && token.Channel != channel)
				{
					flag = true;
				}
				if (!flag)
				{
					token.TokenIndex = num;
					tokens.Add(token);
					num++;
				}
				token = tokenSource.NextToken();
			}
			p = 0;
			p = SkipOffTokenChannels(p);
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x0001A154 File Offset: 0x00018354
		protected virtual int SkipOffTokenChannels(int i)
		{
			var count = tokens.Count;
			while (i < count && ((IToken)tokens[i]).Channel != channel)
			{
				i++;
			}
			return i;
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x0001A1A0 File Offset: 0x000183A0
		protected virtual int SkipOffTokenChannelsReverse(int i)
		{
			while (i >= 0 && ((IToken)tokens[i]).Channel != channel)
			{
				i--;
			}
			return i;
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x0001A1D8 File Offset: 0x000183D8
		public virtual void SetTokenTypeChannel(int ttype, int channel)
		{
			if (channelOverrideMap == null)
			{
				channelOverrideMap = new Hashtable();
			}
			channelOverrideMap[ttype] = channel;
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x0001A208 File Offset: 0x00018408
		public virtual void DiscardTokenType(int ttype)
		{
			if (discardSet == null)
			{
				discardSet = new HashList();
			}
			discardSet.Add(ttype.ToString(), ttype);
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0001A244 File Offset: 0x00018444
		public virtual void DiscardOffChannelTokens(bool discardOffChannelTokens)
		{
			this.discardOffChannelTokens = discardOffChannelTokens;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x0001A250 File Offset: 0x00018450
		public virtual IList GetTokens()
		{
			if (p == -1)
			{
				FillBuffer();
			}
			return tokens;
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x0001A26C File Offset: 0x0001846C
		public virtual IList GetTokens(int start, int stop)
		{
			return GetTokens(start, stop, new BitSet());
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x0001A278 File Offset: 0x00018478
		public virtual IList GetTokens(int start, int stop, BitSet types)
		{
			if (p == -1)
			{
				FillBuffer();
			}
			if (stop >= tokens.Count)
			{
				stop = tokens.Count - 1;
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
			for (var i = start; i <= stop; i++)
			{
				var token = (IToken)tokens[i];
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
			return GetTokens(start, stop, new BitSet(types));
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x0001A334 File Offset: 0x00018534
		public virtual IList GetTokens(int start, int stop, int ttype)
		{
			return GetTokens(start, stop, BitSet.Of(ttype));
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0001A344 File Offset: 0x00018544
		protected virtual IToken LB(int k)
		{
			if (p == -1)
			{
				FillBuffer();
			}
			if (k == 0)
			{
				return null;
			}
			if (p - k < 0)
			{
				return null;
			}
			var num = p;
			for (var i = 1; i <= k; i++)
			{
				num = SkipOffTokenChannelsReverse(num - 1);
			}
			if (num < 0)
			{
				return null;
			}
			return (IToken)tokens[num];
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x0001A3B8 File Offset: 0x000185B8
		public override string ToString()
		{
			if (p == -1)
			{
				FillBuffer();
			}
			return ToString(0, tokens.Count - 1);
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
