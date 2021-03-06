using System;
using System.Collections;
using System.Diagnostics;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000DB RID: 219
	public abstract class BaseRecognizer
	{
		// Token: 0x060008C6 RID: 2246 RVA: 0x00019044 File Offset: 0x00017244
		public BaseRecognizer()
		{
			state = new RecognizerSharedState();
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00019058 File Offset: 0x00017258
		public BaseRecognizer(RecognizerSharedState state)
        {
            state ??= new RecognizerSharedState();
            this.state = state;
        }

		// Token: 0x060008C9 RID: 2249 RVA: 0x00019080 File Offset: 0x00017280
		public virtual void BeginBacktrack(int level)
		{
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00019084 File Offset: 0x00017284
		public virtual void EndBacktrack(int level, bool successful)
		{
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060008CB RID: 2251
		public abstract IIntStream Input { get; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x00019088 File Offset: 0x00017288
		// (set) Token: 0x060008CD RID: 2253 RVA: 0x00019098 File Offset: 0x00017298
		public int BacktrackingLevel
		{
			get => state.backtracking;
            set => state.backtracking = value;
        }

		// Token: 0x060008CE RID: 2254 RVA: 0x000190A8 File Offset: 0x000172A8
		public bool Failed()
		{
			return state.failed;
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x000190B8 File Offset: 0x000172B8
		public virtual void Reset()
		{
			if (state == null)
			{
				return;
			}
			state.followingStackPointer = -1;
			state.errorRecovery = false;
			state.lastErrorIndex = -1;
			state.failed = false;
			state.syntaxErrors = 0;
			state.backtracking = 0;
			var num = 0;
			while (state.ruleMemo != null && num < state.ruleMemo.Length)
			{
				state.ruleMemo[num] = null;
				num++;
			}
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00019158 File Offset: 0x00017358
		public virtual object Match(IIntStream input, int ttype, BitSet follow)
		{
			var currentInputSymbol = GetCurrentInputSymbol(input);
			if (input.LA(1) == ttype)
			{
				input.Consume();
				state.errorRecovery = false;
				state.failed = false;
				return currentInputSymbol;
			}

            if (state.backtracking <= 0) return RecoverFromMismatchedToken(input, ttype, follow);
            state.failed = true;
            return currentInputSymbol;
        }

		// Token: 0x060008D1 RID: 2257 RVA: 0x000191C4 File Offset: 0x000173C4
		public virtual void MatchAny(IIntStream input)
		{
			state.errorRecovery = false;
			state.failed = false;
			input.Consume();
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x000191E4 File Offset: 0x000173E4
		public bool MismatchIsUnwantedToken(IIntStream input, int ttype)
		{
			return input.LA(2) == ttype;
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x000191F0 File Offset: 0x000173F0
		public bool MismatchIsMissingToken(IIntStream input, BitSet follow)
		{
			if (follow == null)
			{
				return false;
			}

            if (!follow.Member(1)) return follow.Member(input.LA(1)) || follow.Member(1);
            var a = ComputeContextSensitiveRuleFOLLOW();
            follow = follow.Or(a);
            if (state.followingStackPointer >= 0)
            {
                follow.Remove(1);
            }
            return follow.Member(input.LA(1)) || follow.Member(1);
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x0001925C File Offset: 0x0001745C
		public virtual void ReportError(RecognitionException e)
		{
			if (state.errorRecovery)
			{
				return;
			}
			state.syntaxErrors++;
			state.errorRecovery = true;
			DisplayRecognitionError(TokenNames, e);
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x0001929C File Offset: 0x0001749C
		public virtual void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			var errorHeader = GetErrorHeader(e);
			var errorMessage = GetErrorMessage(e, tokenNames);
			EmitErrorMessage($"{errorHeader} {errorMessage}");
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x000192CC File Offset: 0x000174CC
		public virtual string GetErrorMessage(RecognitionException e, string[] tokenNames)
		{
			var result = e.Message;
			switch (e)
            {
                case UnwantedTokenException ex:
                {
                    var str = ex.Expecting == Token.EOF ? "EOF" : tokenNames[ex.Expecting];
                    result = $"extraneous input {GetTokenErrorDisplay(ex.UnexpectedToken)} expecting {str}";
                    break;
                }
                case MissingTokenException ex2:
                {
                    var str2 = ex2.Expecting == Token.EOF ? "EOF" : tokenNames[ex2.Expecting];
                    result = $"missing {str2} at {GetTokenErrorDisplay(e.Token)}";
                    break;
                }
                case MismatchedTokenException ex3:
                {
                    var str3 = ex3.Expecting == Token.EOF ? "EOF" : tokenNames[ex3.Expecting];
                    result = $"mismatched input {GetTokenErrorDisplay(e.Token)} expecting {str3}";
                    break;
                }
                case MismatchedTreeNodeException ex4:
                {
                    var text = ex4.expecting == Token.EOF ? "EOF" : tokenNames[ex4.expecting];
                    result =
                        $"mismatched tree node: {((ex4.Node?.ToString() == null) ? string.Empty : ex4.Node)} expecting {text}";
                    break;
                }
                case NoViableAltException:
                    result = $"no viable alternative at input {GetTokenErrorDisplay(e.Token)}";
                    break;
                case EarlyExitException:
                    result = $"required (...)+ loop did not match anything at input {GetTokenErrorDisplay(e.Token)}";
                    break;
                case MismatchedSetException ex5:
                    result = $"mismatched input {GetTokenErrorDisplay(e.Token)} expecting set {ex5.expecting}";
                    break;
                case FailedPredicateException ex6:
                    result = $"rule {ex6.ruleName} failed predicate: {{{ex6.predicateText}}}?";
                    break;
            }
			return result;
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x000195D4 File Offset: 0x000177D4
		public int NumberOfSyntaxErrors => state.syntaxErrors;

        // Token: 0x060008D8 RID: 2264 RVA: 0x000195E4 File Offset: 0x000177E4
		public virtual string GetErrorHeader(RecognitionException e) => $"line {e.Line}:{e.CharPositionInLine}";

        // Token: 0x060008D9 RID: 2265 RVA: 0x00019628 File Offset: 0x00017828
		public virtual string GetTokenErrorDisplay(IToken t)
		{
			var text = t.Text ?? (t.Type == Token.EOF ? "<EOF>" : $"<{t.Type}>");
            text = text.Replace("\n", "\\\\n");
			text = text.Replace("\r", "\\\\r");
			text = text.Replace("\t", "\\\\t");
			return $"'{text}'";
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x000196BC File Offset: 0x000178BC
		public virtual void EmitErrorMessage(string msg)
		{
			Console.Error.WriteLine(msg);
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x000196CC File Offset: 0x000178CC
		public virtual void Recover(IIntStream input, RecognitionException re)
		{
			if (state.lastErrorIndex == input.Index())
			{
				input.Consume();
			}
			state.lastErrorIndex = input.Index();
			var set = ComputeErrorRecoverySet();
			BeginResync();
			ConsumeUntil(input, set);
			EndResync();
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x00019724 File Offset: 0x00017924
		public virtual void BeginResync()
		{
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x00019728 File Offset: 0x00017928
		public virtual void EndResync()
		{
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0001972C File Offset: 0x0001792C
		protected internal virtual object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
		{
			RecognitionException ex = null;
			if (MismatchIsUnwantedToken(input, ttype))
			{
				ex = new UnwantedTokenException(ttype, input);
				BeginResync();
				input.Consume();
				EndResync();
				ReportError(ex);
				var currentInputSymbol = GetCurrentInputSymbol(input);
				input.Consume();
				return currentInputSymbol;
			}
			if (MismatchIsMissingToken(input, follow))
			{
				var missingSymbol = GetMissingSymbol(input, ex, ttype, follow);
				ex = new MissingTokenException(ttype, input, missingSymbol);
				ReportError(ex);
				return missingSymbol;
			}
			ex = new MismatchedTokenException(ttype, input);
			throw ex;
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x000197AC File Offset: 0x000179AC
		public virtual object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
		{
            if (!MismatchIsMissingToken(input, follow)) throw e;
            ReportError(e);
            return GetMissingSymbol(input, e, 0, follow);
        }

		// Token: 0x060008E0 RID: 2272 RVA: 0x000197DC File Offset: 0x000179DC
		public virtual void ConsumeUntil(IIntStream input, int tokenType)
		{
			var num = input.LA(1);
			while (num != Token.EOF && num != tokenType)
			{
				input.Consume();
				num = input.LA(1);
			}
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x00019818 File Offset: 0x00017A18
		public virtual void ConsumeUntil(IIntStream input, BitSet set)
		{
			var num = input.LA(1);
			while (num != Token.EOF && !set.Member(num))
			{
				input.Consume();
				num = input.LA(1);
			}
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x00019858 File Offset: 0x00017A58
		public virtual IList GetRuleInvocationStack()
		{
			var fullName = GetType().FullName;
			return GetRuleInvocationStack(new Exception(), fullName);
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x0001987C File Offset: 0x00017A7C
		public static IList GetRuleInvocationStack(Exception e, string recognizerClassName)
		{
			IList list = new ArrayList();
			var stackTrace = new StackTrace(e);
			for (var i = stackTrace.FrameCount - 1; i >= 0; i--)
			{
				var frame = stackTrace.GetFrame(i);
                if (frame.GetMethod().DeclaringType.FullName.StartsWith("Antlr.Runtime.")) continue;
                if (frame.GetMethod().Name.Equals(NEXT_TOKEN_RULE_NAME)) continue;
                if (frame.GetMethod().DeclaringType.FullName.Equals(recognizerClassName))
                {
                    list.Add(frame.GetMethod()?.Name);
                }
            }
			return list;
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060008E4 RID: 2276 RVA: 0x00019930 File Offset: 0x00017B30
		public virtual string GrammarFileName => null;

        // Token: 0x170000DF RID: 223
		// (get) Token: 0x060008E5 RID: 2277
		public abstract string SourceName { get; }

		// Token: 0x060008E6 RID: 2278 RVA: 0x00019934 File Offset: 0x00017B34
		public virtual IList ToStrings(IList tokens)
		{
			if (tokens == null)
			{
				return null;
			}
			IList list = new ArrayList(tokens.Count);
			foreach (var t in tokens)
            {
                list.Add(((IToken)t).Text);
            }
			return list;
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x00019988 File Offset: 0x00017B88
		public virtual int GetRuleMemoization(int ruleIndex, int ruleStartIndex)
		{
			state.ruleMemo[ruleIndex] ??= new Hashtable();
			var obj = state.ruleMemo[ruleIndex][ruleStartIndex];
			if (obj == null)
			{
				return -1;
			}
			return (int)obj;
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x000199E4 File Offset: 0x00017BE4
		public virtual bool AlreadyParsedRule(IIntStream input, int ruleIndex)
		{
			var ruleMemoization = GetRuleMemoization(ruleIndex, input.Index());
			switch (ruleMemoization)
            {
                case -1:
                    return false;
                case -2:
                    state.failed = true;
                    break;
                default:
                    input.Seek(ruleMemoization + 1);
                    break;
            }

            return true;
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00019A2C File Offset: 0x00017C2C
		public virtual void Memoize(IIntStream input, int ruleIndex, int ruleStartIndex)
		{
			var num = (!state.failed) ? (input.Index() - 1) : -2;
			if (state.ruleMemo[ruleIndex] != null)
			{
				state.ruleMemo[ruleIndex][ruleStartIndex] = num;
			}
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00019A8C File Offset: 0x00017C8C
		public int GetRuleMemoizationCacheSize()
		{
			var num = 0;
			var num2 = 0;
			while (state.ruleMemo != null && num2 < state.ruleMemo.Length)
			{
				var dictionary = state.ruleMemo[num2];
				if (dictionary != null)
				{
					num += dictionary.Count;
				}
				num2++;
			}
			return num;
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00019AE8 File Offset: 0x00017CE8
		public virtual void TraceIn(string ruleName, int ruleIndex, object inputSymbol)
		{
			Console.Out.Write($"enter {ruleName} {inputSymbol}");
			if (state.backtracking > 0)
			{
				Console.Out.Write($" backtracking={state.backtracking}");
			}
			Console.Out.WriteLine();
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x00019B64 File Offset: 0x00017D64
		public virtual void TraceOut(string ruleName, int ruleIndex, object inputSymbol)
		{
			Console.Out.Write($"exit {ruleName} {inputSymbol}");
			if (state.backtracking > 0)
            {
                Console.Out.Write($" backtracking={state.backtracking}");
                Console.Out.WriteLine(state.failed ? $" failed{state.failed}" : $" succeeded{state.failed}");
            }
			Console.Out.WriteLine();
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060008ED RID: 2285 RVA: 0x00019C3C File Offset: 0x00017E3C
		public virtual string[] TokenNames => null;

        // Token: 0x060008EE RID: 2286 RVA: 0x00019C40 File Offset: 0x00017E40
		protected internal virtual BitSet ComputeErrorRecoverySet() => CombineFollows(false);

        // Token: 0x060008EF RID: 2287 RVA: 0x00019C4C File Offset: 0x00017E4C
		protected internal virtual BitSet ComputeContextSensitiveRuleFOLLOW() => CombineFollows(true);

        // Token: 0x060008F0 RID: 2288 RVA: 0x00019C58 File Offset: 0x00017E58
		protected internal virtual BitSet CombineFollows(bool exact)
		{
			var followingStackPointer = state.followingStackPointer;
			var bitSet = new BitSet();
			for (var i = followingStackPointer; i >= 0; i--)
			{
				var bitSet2 = state.following[i];
				bitSet.OrInPlace(bitSet2);
                if (!exact) continue;
                if (!bitSet2.Member(1))
                {
                    break;
                }
                if (i > 0)
                {
                    bitSet.Remove(1);
                }
            }
			return bitSet;
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x00019CCC File Offset: 0x00017ECC
		protected virtual object GetCurrentInputSymbol(IIntStream input) => null;

        // Token: 0x060008F2 RID: 2290 RVA: 0x00019CD0 File Offset: 0x00017ED0
		protected virtual object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow) => null;

        // Token: 0x060008F3 RID: 2291 RVA: 0x00019CD4 File Offset: 0x00017ED4
		protected void PushFollow(BitSet fset)
		{
			if (state.followingStackPointer + 1 >= state.following.Length)
			{
				var array = new BitSet[state.following.Length * 2];
				Array.Copy(state.following, 0, array, 0, state.following.Length);
				state.following = array;
			}
			state.following[++state.followingStackPointer] = fset;
		}

		// Token: 0x0400025F RID: 607
		public const int MEMO_RULE_FAILED = -2;

		// Token: 0x04000260 RID: 608
		public const int MEMO_RULE_UNKNOWN = -1;

		// Token: 0x04000261 RID: 609
		public const int INITIAL_FOLLOW_STACK_SIZE = 100;

		// Token: 0x04000262 RID: 610
		public const int DEFAULT_TOKEN_CHANNEL = 0;

		// Token: 0x04000263 RID: 611
		public const int HIDDEN = 99;

		// Token: 0x04000264 RID: 612
		public static readonly string NEXT_TOKEN_RULE_NAME = "nextToken";

		// Token: 0x04000265 RID: 613
		protected internal RecognizerSharedState state;
	}
}
