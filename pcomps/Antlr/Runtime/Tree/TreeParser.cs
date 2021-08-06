using System.Text.RegularExpressions;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000AD RID: 173
	public class TreeParser : BaseRecognizer
	{
		// Token: 0x060006A8 RID: 1704 RVA: 0x000129EC File Offset: 0x00010BEC
		public TreeParser(ITreeNodeStream input)
		{
			TreeNodeStream = input;
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x000129FC File Offset: 0x00010BFC
		public TreeParser(ITreeNodeStream input, RecognizerSharedState state) : base(state)
		{
			TreeNodeStream = input;
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x00012A68 File Offset: 0x00010C68
		// (set) Token: 0x060006AC RID: 1708 RVA: 0x00012A70 File Offset: 0x00010C70
		public virtual ITreeNodeStream TreeNodeStream
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

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x00012A7C File Offset: 0x00010C7C
		public override string SourceName
		{
			get
			{
				return input.SourceName;
			}
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x00012A8C File Offset: 0x00010C8C
		protected override object GetCurrentInputSymbol(IIntStream input)
		{
			return ((ITreeNodeStream)input).LT(1);
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x00012A9C File Offset: 0x00010C9C
		protected override object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow)
		{
			var text = $"<missing {TokenNames[expectedTokenType]}>";
			return new CommonTree(new CommonToken(expectedTokenType, text));
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x00012AD0 File Offset: 0x00010CD0
		public override void Reset()
		{
			base.Reset();
            input?.Seek(0);
        }

		// Token: 0x060006B1 RID: 1713 RVA: 0x00012AF0 File Offset: 0x00010CF0
		public override void MatchAny(IIntStream ignore)
		{
			state.errorRecovery = false;
			state.failed = false;
			var t = input.LT(1);
			if (input.TreeAdaptor.GetChildCount(t) == 0)
			{
				input.Consume();
				return;
			}
			var num = 0;
			var nodeType = input.TreeAdaptor.GetNodeType(t);
			while (nodeType != Token.EOF && (nodeType != 3 || num != 0))
			{
				input.Consume();
				t = input.LT(1);
				nodeType = input.TreeAdaptor.GetNodeType(t);
				if (nodeType == 2)
				{
					num++;
				}
				else if (nodeType == 3)
				{
					num--;
				}
			}
			input.Consume();
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060006B2 RID: 1714 RVA: 0x00012BC8 File Offset: 0x00010DC8
		public override IIntStream Input
		{
			get
			{
				return input;
			}
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00012BD0 File Offset: 0x00010DD0
		protected internal override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
		{
			throw new MismatchedTreeNodeException(ttype, (ITreeNodeStream)input);
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00012BE0 File Offset: 0x00010DE0
		public override string GetErrorHeader(RecognitionException e)
		{
			return string.Concat(new object[]
			{
				GrammarFileName,
				": node from ",
				(!e.approximateLineInfo) ? string.Empty : "after ",
				"line ",
				e.Line,
				":",
				e.CharPositionInLine
			});
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00012C54 File Offset: 0x00010E54
		public override string GetErrorMessage(RecognitionException e, string[] tokenNames)
		{
			if (this is TreeParser)
			{
				var treeAdaptor = ((ITreeNodeStream)e.Input).TreeAdaptor;
				e.Token = treeAdaptor.GetToken(e.Node);
				if (e.Token == null)
				{
					e.Token = new CommonToken(treeAdaptor.GetNodeType(e.Node), treeAdaptor.GetNodeText(e.Node));
				}
			}
			return base.GetErrorMessage(e, tokenNames);
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00012CC8 File Offset: 0x00010EC8
		public virtual void TraceIn(string ruleName, int ruleIndex)
		{
			base.TraceIn(ruleName, ruleIndex, input.LT(1));
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00012CE0 File Offset: 0x00010EE0
		public virtual void TraceOut(string ruleName, int ruleIndex)
		{
			base.TraceOut(ruleName, ruleIndex, input.LT(1));
		}

		// Token: 0x040001B9 RID: 441
		public const int DOWN = 2;

		// Token: 0x040001BA RID: 442
		public const int UP = 3;

		// Token: 0x040001BB RID: 443
		private static readonly string dotdot = ".*[^.]\\.\\.[^.].*";

		// Token: 0x040001BC RID: 444
		private static readonly string doubleEtc = ".*\\.\\.\\.\\s+\\.\\.\\..*";

		// Token: 0x040001BD RID: 445
		private static readonly string spaces = "\\s+";

		// Token: 0x040001BE RID: 446
		private static readonly Regex dotdotPattern = new Regex(dotdot, RegexOptions.Compiled);

		// Token: 0x040001BF RID: 447
		private static readonly Regex doubleEtcPattern = new Regex(doubleEtc, RegexOptions.Compiled);

		// Token: 0x040001C0 RID: 448
		private static readonly Regex spacesPattern = new Regex(spaces, RegexOptions.Compiled);

		// Token: 0x040001C1 RID: 449
		protected internal ITreeNodeStream input;
	}
}
