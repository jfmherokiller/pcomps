using System;
using pcomps.Antlr.Runtime.Tree;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000099 RID: 153
	[Serializable]
	public class CommonErrorNode : CommonTree
	{
		// Token: 0x06000586 RID: 1414 RVA: 0x000103C0 File Offset: 0x0000E5C0
		public CommonErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
		{
			if (stop == null || (stop.TokenIndex < start.TokenIndex && stop.Type != Antlr.Runtime.Token.EOF))
			{
				stop = start;
			}
			this.input = input;
			this.start = start;
			this.stop = stop;
			trappedException = e;
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x0001041C File Offset: 0x0000E61C
		public override bool IsNil
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x00010420 File Offset: 0x0000E620
		public override int Type
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x00010424 File Offset: 0x0000E624
		public override string Text
		{
			get
			{
				string result;
				if (start is IToken)
				{
					var tokenIndex = start.TokenIndex;
					var num = stop.TokenIndex;
					if (stop.Type == Antlr.Runtime.Token.EOF)
					{
						num = ((ITokenStream)input).Count;
					}
					result = ((ITokenStream)input).ToString(tokenIndex, num);
				}
				else if (start is ITree)
				{
					result = ((ITreeNodeStream)input).ToString(start, stop);
				}
				else
				{
					result = "<unknown>";
				}
				return result;
			}
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x000104D4 File Offset: 0x0000E6D4
		public override string ToString()
		{
			if (trappedException is MissingTokenException)
			{
				return $"<missing type: {((MissingTokenException)trappedException).MissingType}>";
			}
			if (trappedException is UnwantedTokenException)
			{
				return string.Concat(new object[]
				{
					"<extraneous: ",
					((UnwantedTokenException)trappedException).UnexpectedToken,
					", resync=",
					Text,
					">"
				});
			}
			if (trappedException is MismatchedTokenException)
			{
				return string.Concat(new object[]
				{
					"<mismatched token: ",
					trappedException.Token,
					", resync=",
					Text,
					">"
				});
			}
			if (trappedException is NoViableAltException)
			{
				return string.Concat(new object[]
				{
					"<unexpected: ",
					trappedException.Token,
					", resync=",
					Text,
					">"
				});
			}
			return $"<error: {Text}>";
		}

		// Token: 0x0400018A RID: 394
		public IIntStream input;

		// Token: 0x0400018B RID: 395
		public IToken start;

		// Token: 0x0400018C RID: 396
		public IToken stop;

		// Token: 0x0400018D RID: 397
		[NonSerialized]
		public RecognitionException trappedException;
	}
}
