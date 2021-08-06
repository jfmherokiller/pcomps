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
			this.state = new RecognizerSharedState();
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00019058 File Offset: 0x00017258
		public BaseRecognizer(RecognizerSharedState state)
		{
			if (state == null)
			{
				state = new RecognizerSharedState();
			}
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
			get
			{
				return this.state.backtracking;
			}
			set
			{
				this.state.backtracking = value;
			}
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x000190A8 File Offset: 0x000172A8
		public bool Failed()
		{
			return this.state.failed;
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x000190B8 File Offset: 0x000172B8
		public virtual void Reset()
		{
			if (this.state == null)
			{
				return;
			}
			this.state.followingStackPointer = -1;
			this.state.errorRecovery = false;
			this.state.lastErrorIndex = -1;
			this.state.failed = false;
			this.state.syntaxErrors = 0;
			this.state.backtracking = 0;
			int num = 0;
			while (this.state.ruleMemo != null && num < this.state.ruleMemo.Length)
			{
				this.state.ruleMemo[num] = null;
				num++;
			}
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x00019158 File Offset: 0x00017358
		public virtual object Match(IIntStream input, int ttype, BitSet follow)
		{
			object currentInputSymbol = this.GetCurrentInputSymbol(input);
			if (input.LA(1) == ttype)
			{
				input.Consume();
				this.state.errorRecovery = false;
				this.state.failed = false;
				return currentInputSymbol;
			}
			if (this.state.backtracking > 0)
			{
				this.state.failed = true;
				return currentInputSymbol;
			}
			return this.RecoverFromMismatchedToken(input, ttype, follow);
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x000191C4 File Offset: 0x000173C4
		public virtual void MatchAny(IIntStream input)
		{
			this.state.errorRecovery = false;
			this.state.failed = false;
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
			if (follow.Member(1))
			{
				BitSet a = this.ComputeContextSensitiveRuleFOLLOW();
				follow = follow.Or(a);
				if (this.state.followingStackPointer >= 0)
				{
					follow.Remove(1);
				}
			}
			return follow.Member(input.LA(1)) || follow.Member(1);
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x0001925C File Offset: 0x0001745C
		public virtual void ReportError(RecognitionException e)
		{
			if (this.state.errorRecovery)
			{
				return;
			}
			this.state.syntaxErrors++;
			this.state.errorRecovery = true;
			this.DisplayRecognitionError(this.TokenNames, e);
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x0001929C File Offset: 0x0001749C
		public virtual void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			string errorHeader = this.GetErrorHeader(e);
			string errorMessage = this.GetErrorMessage(e, tokenNames);
			this.EmitErrorMessage($"{errorHeader} {errorMessage}");
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x000192CC File Offset: 0x000174CC
		public virtual string GetErrorMessage(RecognitionException e, string[] tokenNames)
		{
			string result = e.Message;
			if (e is UnwantedTokenException)
			{
				UnwantedTokenException ex = (UnwantedTokenException)e;
				string str;
				if (ex.Expecting == Token.EOF)
				{
					str = "EOF";
				}
				else
				{
					str = tokenNames[ex.Expecting];
				}
				result = $"extraneous input {this.GetTokenErrorDisplay(ex.UnexpectedToken)} expecting {str}";
			}
			else if (e is MissingTokenException)
			{
				MissingTokenException ex2 = (MissingTokenException)e;
				string str2;
				if (ex2.Expecting == Token.EOF)
				{
					str2 = "EOF";
				}
				else
				{
					str2 = tokenNames[ex2.Expecting];
				}
				result = $"missing {str2} at {this.GetTokenErrorDisplay(e.Token)}";
			}
			else if (e is MismatchedTokenException)
			{
				MismatchedTokenException ex3 = (MismatchedTokenException)e;
				string str3;
				if (ex3.Expecting == Token.EOF)
				{
					str3 = "EOF";
				}
				else
				{
					str3 = tokenNames[ex3.Expecting];
				}
				result = $"mismatched input {this.GetTokenErrorDisplay(e.Token)} expecting {str3}";
			}
			else if (e is MismatchedTreeNodeException)
			{
				MismatchedTreeNodeException ex4 = (MismatchedTreeNodeException)e;
				string text;
				if (ex4.expecting == Token.EOF)
				{
					text = "EOF";
				}
				else
				{
					text = tokenNames[ex4.expecting];
				}
				result = string.Concat(new object[]
				{
					"mismatched tree node: ",
					(ex4.Node == null || ex4.Node.ToString() == null) ? string.Empty : ex4.Node,
					" expecting ",
					text
				});
			}
			else if (e is NoViableAltException)
			{
				result = $"no viable alternative at input {this.GetTokenErrorDisplay(e.Token)}";
			}
			else if (e is EarlyExitException)
			{
				result = $"required (...)+ loop did not match anything at input {this.GetTokenErrorDisplay(e.Token)}";
			}
			else if (e is MismatchedSetException)
			{
				MismatchedSetException ex5 = (MismatchedSetException)e;
				result = string.Concat(new object[]
				{
					"mismatched input ",
					this.GetTokenErrorDisplay(e.Token),
					" expecting set ",
					ex5.expecting
				});
			}
			else if (e is MismatchedNotSetException)
			{
				MismatchedNotSetException ex6 = (MismatchedNotSetException)e;
				result = string.Concat(new object[]
				{
					"mismatched input ",
					this.GetTokenErrorDisplay(e.Token),
					" expecting set ",
					ex6.expecting
				});
			}
			else if (e is FailedPredicateException)
			{
				FailedPredicateException ex7 = (FailedPredicateException)e;
				result = string.Concat(new string[]
				{
					"rule ",
					ex7.ruleName,
					" failed predicate: {",
					ex7.predicateText,
					"}?"
				});
			}
			return result;
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x000195D4 File Offset: 0x000177D4
		public int NumberOfSyntaxErrors
		{
			get
			{
				return this.state.syntaxErrors;
			}
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x000195E4 File Offset: 0x000177E4
		public virtual string GetErrorHeader(RecognitionException e)
		{
			return string.Concat(new object[]
			{
				"line ",
				e.Line,
				":",
				e.CharPositionInLine
			});
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x00019628 File Offset: 0x00017828
		public virtual string GetTokenErrorDisplay(IToken t)
		{
			string text = t.Text;
			if (text == null)
			{
				if (t.Type == Token.EOF)
				{
					text = "<EOF>";
				}
				else
				{
					text = $"<{t.Type}>";
				}
			}
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
			if (this.state.lastErrorIndex == input.Index())
			{
				input.Consume();
			}
			this.state.lastErrorIndex = input.Index();
			BitSet set = this.ComputeErrorRecoverySet();
			this.BeginResync();
			this.ConsumeUntil(input, set);
			this.EndResync();
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
			if (this.MismatchIsUnwantedToken(input, ttype))
			{
				ex = new UnwantedTokenException(ttype, input);
				this.BeginResync();
				input.Consume();
				this.EndResync();
				this.ReportError(ex);
				object currentInputSymbol = this.GetCurrentInputSymbol(input);
				input.Consume();
				return currentInputSymbol;
			}
			if (this.MismatchIsMissingToken(input, follow))
			{
				object missingSymbol = this.GetMissingSymbol(input, ex, ttype, follow);
				ex = new MissingTokenException(ttype, input, missingSymbol);
				this.ReportError(ex);
				return missingSymbol;
			}
			ex = new MismatchedTokenException(ttype, input);
			throw ex;
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x000197AC File Offset: 0x000179AC
		public virtual object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
		{
			if (this.MismatchIsMissingToken(input, follow))
			{
				this.ReportError(e);
				return this.GetMissingSymbol(input, e, 0, follow);
			}
			throw e;
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x000197DC File Offset: 0x000179DC
		public virtual void ConsumeUntil(IIntStream input, int tokenType)
		{
			int num = input.LA(1);
			while (num != Token.EOF && num != tokenType)
			{
				input.Consume();
				num = input.LA(1);
			}
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x00019818 File Offset: 0x00017A18
		public virtual void ConsumeUntil(IIntStream input, BitSet set)
		{
			int num = input.LA(1);
			while (num != Token.EOF && !set.Member(num))
			{
				input.Consume();
				num = input.LA(1);
			}
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x00019858 File Offset: 0x00017A58
		public virtual IList GetRuleInvocationStack()
		{
			string fullName = base.GetType().FullName;
			return BaseRecognizer.GetRuleInvocationStack(new Exception(), fullName);
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x0001987C File Offset: 0x00017A7C
		public static IList GetRuleInvocationStack(Exception e, string recognizerClassName)
		{
			IList list = new ArrayList();
			StackTrace stackTrace = new StackTrace(e);
			for (int i = stackTrace.FrameCount - 1; i >= 0; i--)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				if (!frame.GetMethod().DeclaringType.FullName.StartsWith("Antlr.Runtime."))
				{
					if (!frame.GetMethod().Name.Equals(BaseRecognizer.NEXT_TOKEN_RULE_NAME))
					{
						if (frame.GetMethod().DeclaringType.FullName.Equals(recognizerClassName))
						{
							list.Add(frame.GetMethod().Name);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060008E4 RID: 2276 RVA: 0x00019930 File Offset: 0x00017B30
		public virtual string GrammarFileName
		{
			get
			{
				return null;
			}
		}

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
			for (int i = 0; i < tokens.Count; i++)
			{
				list.Add(((IToken)tokens[i]).Text);
			}
			return list;
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x00019988 File Offset: 0x00017B88
		public virtual int GetRuleMemoization(int ruleIndex, int ruleStartIndex)
		{
			if (this.state.ruleMemo[ruleIndex] == null)
			{
				this.state.ruleMemo[ruleIndex] = new Hashtable();
			}
			object obj = this.state.ruleMemo[ruleIndex][ruleStartIndex];
			if (obj == null)
			{
				return -1;
			}
			return (int)obj;
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x000199E4 File Offset: 0x00017BE4
		public virtual bool AlreadyParsedRule(IIntStream input, int ruleIndex)
		{
			int ruleMemoization = this.GetRuleMemoization(ruleIndex, input.Index());
			if (ruleMemoization == -1)
			{
				return false;
			}
			if (ruleMemoization == -2)
			{
				this.state.failed = true;
			}
			else
			{
				input.Seek(ruleMemoization + 1);
			}
			return true;
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00019A2C File Offset: 0x00017C2C
		public virtual void Memoize(IIntStream input, int ruleIndex, int ruleStartIndex)
		{
			int num = (!this.state.failed) ? (input.Index() - 1) : -2;
			if (this.state.ruleMemo[ruleIndex] != null)
			{
				this.state.ruleMemo[ruleIndex][ruleStartIndex] = num;
			}
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00019A8C File Offset: 0x00017C8C
		public int GetRuleMemoizationCacheSize()
		{
			int num = 0;
			int num2 = 0;
			while (this.state.ruleMemo != null && num2 < this.state.ruleMemo.Length)
			{
				IDictionary dictionary = this.state.ruleMemo[num2];
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
			Console.Out.Write(string.Concat(new object[]
			{
				"enter ",
				ruleName,
				" ",
				inputSymbol
			}));
			if (this.state.backtracking > 0)
			{
				Console.Out.Write($" backtracking={this.state.backtracking}");
			}
			Console.Out.WriteLine();
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x00019B64 File Offset: 0x00017D64
		public virtual void TraceOut(string ruleName, int ruleIndex, object inputSymbol)
		{
			Console.Out.Write(string.Concat(new object[]
			{
				"exit ",
				ruleName,
				" ",
				inputSymbol
			}));
			if (this.state.backtracking > 0)
			{
				Console.Out.Write($" backtracking={this.state.backtracking}");
				if (this.state.failed)
				{
					Console.Out.WriteLine($" failed{this.state.failed}");
				}
				else
				{
					Console.Out.WriteLine($" succeeded{this.state.failed}");
				}
			}
			Console.Out.WriteLine();
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060008ED RID: 2285 RVA: 0x00019C3C File Offset: 0x00017E3C
		public virtual string[] TokenNames
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x00019C40 File Offset: 0x00017E40
		protected internal virtual BitSet ComputeErrorRecoverySet()
		{
			return this.CombineFollows(false);
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x00019C4C File Offset: 0x00017E4C
		protected internal virtual BitSet ComputeContextSensitiveRuleFOLLOW()
		{
			return this.CombineFollows(true);
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x00019C58 File Offset: 0x00017E58
		protected internal virtual BitSet CombineFollows(bool exact)
		{
			int followingStackPointer = this.state.followingStackPointer;
			BitSet bitSet = new BitSet();
			for (int i = followingStackPointer; i >= 0; i--)
			{
				BitSet bitSet2 = this.state.following[i];
				bitSet.OrInPlace(bitSet2);
				if (exact)
				{
					if (!bitSet2.Member(1))
					{
						break;
					}
					if (i > 0)
					{
						bitSet.Remove(1);
					}
				}
			}
			return bitSet;
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x00019CCC File Offset: 0x00017ECC
		protected virtual object GetCurrentInputSymbol(IIntStream input)
		{
			return null;
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x00019CD0 File Offset: 0x00017ED0
		protected virtual object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow)
		{
			return null;
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x00019CD4 File Offset: 0x00017ED4
		protected void PushFollow(BitSet fset)
		{
			if (this.state.followingStackPointer + 1 >= this.state.following.Length)
			{
				BitSet[] array = new BitSet[this.state.following.Length * 2];
				Array.Copy(this.state.following, 0, array, 0, this.state.following.Length);
				this.state.following = array;
			}
			this.state.following[++this.state.followingStackPointer] = fset;
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
