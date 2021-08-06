using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000E1 RID: 225
	public class TokenRewriteStream : CommonTokenStream
	{
		// Token: 0x06000944 RID: 2372 RVA: 0x0001AC10 File Offset: 0x00018E10
		public TokenRewriteStream()
		{
			Init();
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x0001AC20 File Offset: 0x00018E20
		public TokenRewriteStream(ITokenSource tokenSource) : base(tokenSource)
		{
			Init();
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x0001AC30 File Offset: 0x00018E30
		public TokenRewriteStream(ITokenSource tokenSource, int channel) : base(tokenSource, channel)
		{
			Init();
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x0001AC40 File Offset: 0x00018E40
		protected internal virtual void Init()
		{
			programs = new Hashtable();
			programs["default"] = new ArrayList(100);
			lastRewriteTokenIndexes = new Hashtable();
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0001AC70 File Offset: 0x00018E70
		public virtual void Rollback(int instructionIndex)
		{
			Rollback("default", instructionIndex);
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x0001AC80 File Offset: 0x00018E80
		public virtual void Rollback(string programName, int instructionIndex)
		{
			var list = (IList)programs[programName];
			if (list != null)
			{
				programs[programName] = ((ArrayList)list).GetRange(0, instructionIndex);
			}
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x0001ACC0 File Offset: 0x00018EC0
		public virtual void DeleteProgram()
		{
			DeleteProgram("default");
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x0001ACD0 File Offset: 0x00018ED0
		public virtual void DeleteProgram(string programName)
		{
			Rollback(programName, 0);
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x0001ACDC File Offset: 0x00018EDC
		public virtual void InsertAfter(IToken t, object text)
		{
			InsertAfter("default", t, text);
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0001ACEC File Offset: 0x00018EEC
		public virtual void InsertAfter(int index, object text)
		{
			InsertAfter("default", index, text);
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0001ACFC File Offset: 0x00018EFC
		public virtual void InsertAfter(string programName, IToken t, object text)
		{
			InsertAfter(programName, t.TokenIndex, text);
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x0001AD0C File Offset: 0x00018F0C
		public virtual void InsertAfter(string programName, int index, object text)
		{
			InsertBefore(programName, index + 1, text);
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0001AD1C File Offset: 0x00018F1C
		public virtual void InsertBefore(IToken t, object text)
		{
			InsertBefore("default", t, text);
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x0001AD2C File Offset: 0x00018F2C
		public virtual void InsertBefore(int index, object text)
		{
			InsertBefore("default", index, text);
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x0001AD3C File Offset: 0x00018F3C
		public virtual void InsertBefore(string programName, IToken t, object text)
		{
			InsertBefore(programName, t.TokenIndex, text);
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x0001AD4C File Offset: 0x00018F4C
		public virtual void InsertBefore(string programName, int index, object text)
		{
			RewriteOperation value = new InsertBeforeOp(index, text, this);
			var program = GetProgram(programName);
			program.Add(value);
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x0001AD74 File Offset: 0x00018F74
		public virtual void Replace(int index, object text)
		{
			Replace("default", index, index, text);
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x0001AD84 File Offset: 0x00018F84
		public virtual void Replace(int from, int to, object text)
		{
			Replace("default", from, to, text);
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x0001AD94 File Offset: 0x00018F94
		public virtual void Replace(IToken indexT, object text)
		{
			Replace("default", indexT, indexT, text);
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x0001ADA4 File Offset: 0x00018FA4
		public virtual void Replace(IToken from, IToken to, object text)
		{
			Replace("default", from, to, text);
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x0001ADB4 File Offset: 0x00018FB4
		public virtual void Replace(string programName, int from, int to, object text)
		{
			if (from > to || from < 0 || to < 0 || to >= tokens.Count)
			{
				throw new ArgumentOutOfRangeException(string.Concat(new object[]
				{
					"replace: range invalid: ",
					from,
					"..",
					to,
					"(size=",
					tokens.Count,
					")"
				}));
			}
			RewriteOperation rewriteOperation = new ReplaceOp(from, to, text, this);
			var program = GetProgram(programName);
			rewriteOperation.instructionIndex = program.Count;
			program.Add(rewriteOperation);
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x0001AE64 File Offset: 0x00019064
		public virtual void Replace(string programName, IToken from, IToken to, object text)
		{
			Replace(programName, from.TokenIndex, to.TokenIndex, text);
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x0001AE88 File Offset: 0x00019088
		public virtual void Delete(int index)
		{
			Delete("default", index, index);
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0001AE98 File Offset: 0x00019098
		public virtual void Delete(int from, int to)
		{
			Delete("default", from, to);
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x0001AEA8 File Offset: 0x000190A8
		public virtual void Delete(IToken indexT)
		{
			Delete("default", indexT, indexT);
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0001AEB8 File Offset: 0x000190B8
		public virtual void Delete(IToken from, IToken to)
		{
			Delete("default", from, to);
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x0001AEC8 File Offset: 0x000190C8
		public virtual void Delete(string programName, int from, int to)
		{
			Replace(programName, from, to, null);
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x0001AED4 File Offset: 0x000190D4
		public virtual void Delete(string programName, IToken from, IToken to)
		{
			Replace(programName, from, to, null);
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x0001AEE0 File Offset: 0x000190E0
		public virtual int GetLastRewriteTokenIndex()
		{
			return GetLastRewriteTokenIndex("default");
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0001AEF0 File Offset: 0x000190F0
		protected virtual int GetLastRewriteTokenIndex(string programName)
		{
			var obj = lastRewriteTokenIndexes[programName];
			if (obj == null)
			{
				return -1;
			}
			return (int)obj;
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0001AF1C File Offset: 0x0001911C
		protected virtual void SetLastRewriteTokenIndex(string programName, int i)
		{
			lastRewriteTokenIndexes[programName] = i;
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x0001AF30 File Offset: 0x00019130
		protected virtual IList GetProgram(string name)
		{
			var list = (IList)programs[name];
			if (list == null)
			{
				list = InitializeProgram(name);
			}
			return list;
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x0001AF60 File Offset: 0x00019160
		private IList InitializeProgram(string name)
		{
			IList list = new ArrayList(100);
			programs[name] = list;
			return list;
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0001AF84 File Offset: 0x00019184
		public virtual string ToOriginalString()
		{
			return ToOriginalString(0, Count - 1);
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x0001AF98 File Offset: 0x00019198
		public virtual string ToOriginalString(int start, int end)
		{
			var stringBuilder = new StringBuilder();
			var num = start;
			while (num >= 0 && num <= end && num < tokens.Count)
			{
				stringBuilder.Append(Get(num).Text);
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0001AFF0 File Offset: 0x000191F0
		public override string ToString()
		{
			return ToString(0, Count - 1);
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x0001B004 File Offset: 0x00019204
		public virtual string ToString(string programName)
		{
			return ToString(programName, 0, Count - 1);
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x0001B018 File Offset: 0x00019218
		public override string ToString(int start, int end)
		{
			return ToString("default", start, end);
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0001B028 File Offset: 0x00019228
		public virtual string ToString(string programName, int start, int end)
		{
			var list = (IList)programs[programName];
			if (end > tokens.Count - 1)
			{
				end = tokens.Count - 1;
			}
			if (start < 0)
			{
				start = 0;
			}
			if (list == null || list.Count == 0)
			{
				return ToOriginalString(start, end);
			}
			var stringBuilder = new StringBuilder();
			var dictionary = ReduceToSingleOperationPerIndex(list);
			var num = start;
			while (num <= end && num < tokens.Count)
			{
				var rewriteOperation = (RewriteOperation)dictionary[num];
				dictionary.Remove(num);
				var token = (IToken)tokens[num];
				if (rewriteOperation == null)
				{
					stringBuilder.Append(token.Text);
					num++;
				}
				else
				{
					num = rewriteOperation.Execute(stringBuilder);
				}
			}
			if (end == tokens.Count - 1)
			{
				foreach (var obj in dictionary.Values)
				{
					var insertBeforeOp = (InsertBeforeOp)obj;
					if (insertBeforeOp.index >= tokens.Count - 1)
					{
						stringBuilder.Append(insertBeforeOp.text);
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0001B180 File Offset: 0x00019380
		protected IDictionary ReduceToSingleOperationPerIndex(IList rewrites)
		{
			for (var i = 0; i < rewrites.Count; i++)
			{
				var rewriteOperation = (RewriteOperation)rewrites[i];
                if (rewriteOperation is ReplaceOp)
                {
                    var replaceOp = (ReplaceOp)rewrites[i];
                    var kindOfOps = GetKindOfOps(rewrites, typeof(InsertBeforeOp), i);
                    for (var j = 0; j < kindOfOps.Count; j++)
                    {
                        var insertBeforeOp = (InsertBeforeOp)kindOfOps[j];
                        if (insertBeforeOp.index >= replaceOp.index && insertBeforeOp.index <= replaceOp.lastIndex)
                        {
                            rewrites[insertBeforeOp.instructionIndex] = null;
                        }
                    }
                    var kindOfOps2 = GetKindOfOps(rewrites, typeof(ReplaceOp), i);
                    for (var k = 0; k < kindOfOps2.Count; k++)
                    {
                        var replaceOp2 = (ReplaceOp)kindOfOps2[k];
                        if (replaceOp2.index >= replaceOp.index && replaceOp2.lastIndex <= replaceOp.lastIndex)
                        {
                            rewrites[replaceOp2.instructionIndex] = null;
                        }
                        else
                        {
                            var flag = replaceOp2.lastIndex < replaceOp.index || replaceOp2.index > replaceOp.lastIndex;
                            var flag2 = replaceOp2.index == replaceOp.index && replaceOp2.lastIndex == replaceOp.lastIndex;
                            if (!flag && !flag2)
                            {
                                throw new ArgumentOutOfRangeException(string.Concat(new object[]
                                {
                                    "replace op boundaries of ",
                                    replaceOp,
                                    " overlap with previous ",
                                    replaceOp2
                                }));
                            }
                        }
                    }
                }
            }
			for (var l = 0; l < rewrites.Count; l++)
			{
				var rewriteOperation2 = (RewriteOperation)rewrites[l];
                if (rewriteOperation2 is InsertBeforeOp)
                {
                    var insertBeforeOp2 = (InsertBeforeOp)rewrites[l];
                    var kindOfOps3 = GetKindOfOps(rewrites, typeof(InsertBeforeOp), l);
                    for (var m = 0; m < kindOfOps3.Count; m++)
                    {
                        var insertBeforeOp3 = (InsertBeforeOp)kindOfOps3[m];
                        if (insertBeforeOp3.index == insertBeforeOp2.index)
                        {
                            insertBeforeOp2.text = CatOpText(insertBeforeOp2.text, insertBeforeOp3.text);
                            rewrites[insertBeforeOp3.instructionIndex] = null;
                        }
                    }
                    var kindOfOps4 = GetKindOfOps(rewrites, typeof(ReplaceOp), l);
                    for (var n = 0; n < kindOfOps4.Count; n++)
                    {
                        var replaceOp3 = (ReplaceOp)kindOfOps4[n];
                        if (insertBeforeOp2.index == replaceOp3.index)
                        {
                            replaceOp3.text = CatOpText(insertBeforeOp2.text, replaceOp3.text);
                            rewrites[l] = null;
                        }
                        else if (insertBeforeOp2.index >= replaceOp3.index && insertBeforeOp2.index <= replaceOp3.lastIndex)
                        {
                            throw new ArgumentOutOfRangeException(string.Concat(new object[]
                            {
                                "insert op ",
                                insertBeforeOp2,
                                " within boundaries of previous ",
                                replaceOp3
                            }));
                        }
                    }
                }
            }
			IDictionary dictionary = new Hashtable();
			for (var num = 0; num < rewrites.Count; num++)
			{
				var rewriteOperation3 = (RewriteOperation)rewrites[num];
				if (rewriteOperation3 != null)
				{
					if (dictionary[rewriteOperation3.index] != null)
					{
						throw new Exception("should only be one op per index");
					}
					dictionary[rewriteOperation3.index] = rewriteOperation3;
				}
			}
			return dictionary;
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0001B55C File Offset: 0x0001975C
		protected string CatOpText(object a, object b)
		{
			var str = string.Empty;
			var str2 = string.Empty;
			if (a != null)
			{
				str = a.ToString();
			}
			if (b != null)
			{
				str2 = b.ToString();
			}
			return str + str2;
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0001B598 File Offset: 0x00019798
		protected IList GetKindOfOps(IList rewrites, Type kind)
		{
			return GetKindOfOps(rewrites, kind, rewrites.Count);
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0001B5A8 File Offset: 0x000197A8
		protected IList GetKindOfOps(IList rewrites, Type kind, int before)
		{
			IList list = new ArrayList();
			var num = 0;
			while (num < before && num < rewrites.Count)
			{
				var rewriteOperation = (RewriteOperation)rewrites[num];
				if (rewriteOperation != null)
				{
					if (rewriteOperation.GetType() == kind)
					{
						list.Add(rewriteOperation);
					}
				}
				num++;
			}
			return list;
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x0001B608 File Offset: 0x00019808
		public virtual string ToDebugString()
		{
			return ToDebugString(0, Count - 1);
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0001B61C File Offset: 0x0001981C
		public virtual string ToDebugString(int start, int end)
		{
			var stringBuilder = new StringBuilder();
			var num = start;
			while (num >= 0 && num <= end && num < tokens.Count)
			{
				stringBuilder.Append(Get(num));
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0400027F RID: 639
		public const string DEFAULT_PROGRAM_NAME = "default";

		// Token: 0x04000280 RID: 640
		public const int PROGRAM_INIT_SIZE = 100;

		// Token: 0x04000281 RID: 641
		public const int MIN_TOKEN_INDEX = 0;

		// Token: 0x04000282 RID: 642
		protected IDictionary programs;

		// Token: 0x04000283 RID: 643
		protected IDictionary lastRewriteTokenIndexes;

		// Token: 0x020000E2 RID: 226
		private class RewriteOpComparer : IComparer
		{
			// Token: 0x06000972 RID: 2418 RVA: 0x0001B678 File Offset: 0x00019878
			public virtual int Compare(object o1, object o2)
			{
				var rewriteOperation = (RewriteOperation)o1;
				var rewriteOperation2 = (RewriteOperation)o2;
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
		}

		// Token: 0x020000E3 RID: 227
		protected internal class RewriteOperation
		{
			// Token: 0x06000973 RID: 2419 RVA: 0x0001B6BC File Offset: 0x000198BC
			protected internal RewriteOperation(int index, object text, TokenRewriteStream parent)
			{
				this.index = index;
				this.text = text;
				this.parent = parent;
			}

			// Token: 0x06000974 RID: 2420 RVA: 0x0001B6DC File Offset: 0x000198DC
			public virtual int Execute(StringBuilder buf)
			{
				return index;
			}

			// Token: 0x06000975 RID: 2421 RVA: 0x0001B6E4 File Offset: 0x000198E4
			public override string ToString()
			{
				var text = GetType().FullName;
				var num = text.IndexOf('$');
				text = text.Substring(num + 1, text.Length - (num + 1));
				return string.Concat(new object[]
				{
					"<",
					text,
					"@",
					index,
					":\"",
					this.text,
					"\">"
				});
			}

			// Token: 0x04000284 RID: 644
			protected internal int instructionIndex;

			// Token: 0x04000285 RID: 645
			protected internal int index;

			// Token: 0x04000286 RID: 646
			protected internal object text;

			// Token: 0x04000287 RID: 647
			protected internal TokenRewriteStream parent;
		}

		// Token: 0x020000E4 RID: 228
		protected internal class InsertBeforeOp : RewriteOperation
		{
			// Token: 0x06000976 RID: 2422 RVA: 0x0001B760 File Offset: 0x00019960
			public InsertBeforeOp(int index, object text, TokenRewriteStream parent) : base(index, text, parent)
			{
			}

			// Token: 0x06000977 RID: 2423 RVA: 0x0001B76C File Offset: 0x0001996C
			public override int Execute(StringBuilder buf)
			{
				buf.Append(text);
				buf.Append(parent.Get(index).Text);
				return index + 1;
			}
		}

		// Token: 0x020000E5 RID: 229
		protected internal class ReplaceOp : RewriteOperation
		{
			// Token: 0x06000978 RID: 2424 RVA: 0x0001B7AC File Offset: 0x000199AC
			public ReplaceOp(int from, int to, object text, TokenRewriteStream parent) : base(from, text, parent)
			{
				lastIndex = to;
			}

			// Token: 0x06000979 RID: 2425 RVA: 0x0001B7C0 File Offset: 0x000199C0
			public override int Execute(StringBuilder buf)
			{
				if (text != null)
				{
					buf.Append(text);
				}
				return lastIndex + 1;
			}

			// Token: 0x0600097A RID: 2426 RVA: 0x0001B7F0 File Offset: 0x000199F0
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"<ReplaceOp@",
					index,
					"..",
					lastIndex,
					":\"",
					text,
					"\">"
				});
			}

			// Token: 0x04000288 RID: 648
			protected internal int lastIndex;
		}

		// Token: 0x020000E6 RID: 230
		protected internal class DeleteOp : ReplaceOp
		{
			// Token: 0x0600097B RID: 2427 RVA: 0x0001B850 File Offset: 0x00019A50
			public DeleteOp(int from, int to, TokenRewriteStream parent) : base(from, to, null, parent)
			{
			}

			// Token: 0x0600097C RID: 2428 RVA: 0x0001B85C File Offset: 0x00019A5C
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"<DeleteOp@",
					index,
					"..",
					lastIndex,
					">"
				});
			}
		}
	}
}
