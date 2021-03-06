using System;
using System.Text;
using pcomps.Antlr.collections;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr
{
	// Token: 0x02000030 RID: 48
	[Serializable]
	public class MismatchedTokenException : RecognitionException
	{
		// Token: 0x06000227 RID: 551 RVA: 0x00007468 File Offset: 0x00005668
		public MismatchedTokenException() : base("Mismatched Token: expecting any AST node", "<AST>", -1, -1)
		{
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00007490 File Offset: 0x00005690
		public MismatchedTokenException(string[] tokenNames_, AST node_, int lower, int upper_, bool matchNot) : base("Mismatched Token", "<AST>", -1, -1)
		{
			tokenNames = tokenNames_;
			node = node_;
			tokenText = node_ == null ? "<empty tree>" : node_.ToString();
			mismatchType = (matchNot ? TokenTypeEnum.NotRangeType : TokenTypeEnum.RangeType);
			expecting = lower;
			upper = upper_;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00007500 File Offset: 0x00005700
		public MismatchedTokenException(string[] tokenNames_, AST node_, int expecting_, bool matchNot) : base("Mismatched Token", "<AST>", -1, -1)
		{
			tokenNames = tokenNames_;
			node = node_;
			tokenText = node_ == null ? "<empty tree>" : node_.ToString();
			mismatchType = (matchNot ? TokenTypeEnum.NotTokenType : TokenTypeEnum.TokenType);
			expecting = expecting_;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00007568 File Offset: 0x00005768
		public MismatchedTokenException(string[] tokenNames_, AST node_, BitSet set_, bool matchNot) : base("Mismatched Token", "<AST>", -1, -1)
		{
			tokenNames = tokenNames_;
			node = node_;
			tokenText = node_ == null ? "<empty tree>" : node_.ToString();
			mismatchType = (matchNot ? TokenTypeEnum.NotSetType : TokenTypeEnum.SetType);
			bset = set_;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x000075D0 File Offset: 0x000057D0
		public MismatchedTokenException(string[] tokenNames_, IToken token_, int lower, int upper_, bool matchNot, string fileName_) : base("Mismatched Token", fileName_, token_.getLine(), token_.getColumn())
		{
			tokenNames = tokenNames_;
			token = token_;
			tokenText = token_.getText();
			mismatchType = (matchNot ? TokenTypeEnum.NotRangeType : TokenTypeEnum.RangeType);
			expecting = lower;
			upper = upper_;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00007634 File Offset: 0x00005834
		public MismatchedTokenException(string[] tokenNames_, IToken token_, int expecting_, bool matchNot, string fileName_) : base("Mismatched Token", fileName_, token_.getLine(), token_.getColumn())
		{
			tokenNames = tokenNames_;
			token = token_;
			tokenText = token_.getText();
			mismatchType = (matchNot ? TokenTypeEnum.NotTokenType : TokenTypeEnum.TokenType);
			expecting = expecting_;
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00007690 File Offset: 0x00005890
		public MismatchedTokenException(string[] tokenNames_, IToken token_, BitSet set_, bool matchNot, string fileName_) : base("Mismatched Token", fileName_, token_.getLine(), token_.getColumn())
		{
			tokenNames = tokenNames_;
			token = token_;
			tokenText = token_.getText();
			mismatchType = (matchNot ? TokenTypeEnum.NotSetType : TokenTypeEnum.SetType);
			bset = set_;
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600022E RID: 558 RVA: 0x000076EC File Offset: 0x000058EC
		public override string Message
		{
			get
			{
				var stringBuilder = new StringBuilder();
				switch (mismatchType)
				{
				case TokenTypeEnum.TokenType:
					stringBuilder.Append($"expecting {tokenName(expecting)}, found '{tokenText}'");
					break;
				case TokenTypeEnum.NotTokenType:
					stringBuilder.Append($"expecting anything but {tokenName(expecting)}; got it anyway");
					break;
				case TokenTypeEnum.RangeType:
					stringBuilder.Append(
                        $"expecting token in range: {tokenName(expecting)}..{tokenName(upper)}, found '{tokenText}'");
					break;
				case TokenTypeEnum.NotRangeType:
					stringBuilder.Append(
                        $"expecting token NOT in range: {tokenName(expecting)}..{tokenName(upper)}, found '{tokenText}'");
					break;
				case TokenTypeEnum.SetType:
				case TokenTypeEnum.NotSetType:
				{
					stringBuilder.Append(
                        $"expecting {((mismatchType == TokenTypeEnum.NotSetType) ? "NOT " : "")}one of (");
					var array = bset.toArray();
					foreach (var t in array)
                    {
                        stringBuilder.Append(' ');
                        stringBuilder.Append(tokenName(t));
                    }
					stringBuilder.Append($"), found '{tokenText}'");
					break;
				}
				default:
					stringBuilder.Append(base.Message);
					break;
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00007908 File Offset: 0x00005B08
		private string tokenName(int tokenType)
		{
			if (tokenType == 0)
			{
				return "<Set of tokens>";
			}
			if (tokenType < 0 || tokenType >= tokenNames.Length)
			{
				return $"<{tokenType}>";
			}
			return tokenNames[tokenType];
		}

		// Token: 0x0400009B RID: 155
		internal string[] tokenNames;

		// Token: 0x0400009C RID: 156
		public IToken token;

		// Token: 0x0400009D RID: 157
		public AST node;

		// Token: 0x0400009E RID: 158
		internal string tokenText = null;

		// Token: 0x0400009F RID: 159
		public TokenTypeEnum mismatchType;

		// Token: 0x040000A0 RID: 160
		public int expecting;

		// Token: 0x040000A1 RID: 161
		public int upper;

		// Token: 0x040000A2 RID: 162
		public BitSet bset;

		// Token: 0x02000031 RID: 49
		public enum TokenTypeEnum
		{
			// Token: 0x040000A4 RID: 164
			TokenType = 1,
			// Token: 0x040000A5 RID: 165
			NotTokenType,
			// Token: 0x040000A6 RID: 166
			RangeType,
			// Token: 0x040000A7 RID: 167
			NotRangeType,
			// Token: 0x040000A8 RID: 168
			SetType,
			// Token: 0x040000A9 RID: 169
			NotSetType
		}
	}
}
