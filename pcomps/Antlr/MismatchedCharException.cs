using System;
using System.Text;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr
{
	// Token: 0x0200002E RID: 46
	[Serializable]
	public class MismatchedCharException : RecognitionException
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000221 RID: 545 RVA: 0x00007148 File Offset: 0x00005348
		public override string Message
		{
			get
			{
				var stringBuilder = new StringBuilder();
				switch (mismatchType)
				{
				case CharTypeEnum.CharType:
					stringBuilder.Append("expecting ");
					appendCharName(stringBuilder, expecting);
					stringBuilder.Append(", found ");
					appendCharName(stringBuilder, foundChar);
					break;
				case CharTypeEnum.NotCharType:
					stringBuilder.Append("expecting anything but '");
					appendCharName(stringBuilder, expecting);
					stringBuilder.Append("'; got it anyway");
					break;
				case CharTypeEnum.RangeType:
				case CharTypeEnum.NotRangeType:
					stringBuilder.Append("expecting token ");
					if (mismatchType == CharTypeEnum.NotRangeType)
					{
						stringBuilder.Append("NOT ");
					}
					stringBuilder.Append("in range: ");
					appendCharName(stringBuilder, expecting);
					stringBuilder.Append("..");
					appendCharName(stringBuilder, upper);
					stringBuilder.Append(", found ");
					appendCharName(stringBuilder, foundChar);
					break;
				case CharTypeEnum.SetType:
				case CharTypeEnum.NotSetType:
				{
					stringBuilder.Append(
                        $"expecting {((mismatchType == CharTypeEnum.NotSetType) ? "NOT " : "")}one of (");
					var array = bset.toArray();
					foreach (var t in array)
                    {
                        appendCharName(stringBuilder, t);
                    }
					stringBuilder.Append("), found ");
					appendCharName(stringBuilder, foundChar);
					break;
				}
				default:
					stringBuilder.Append(base.Message);
					break;
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x06000222 RID: 546 RVA: 0x000072D4 File Offset: 0x000054D4
		public MismatchedCharException() : base("Mismatched char")
		{
		}

		// Token: 0x06000223 RID: 547 RVA: 0x000072EC File Offset: 0x000054EC
		public MismatchedCharException(char c, char lower, char upper_, bool matchNot, CharScanner scanner_) : base("Mismatched char", scanner_.getFilename(), scanner_.getLine(), scanner_.getColumn())
		{
			mismatchType = (matchNot ? CharTypeEnum.NotRangeType : CharTypeEnum.RangeType);
			foundChar = c;
			expecting = lower;
			upper = upper_;
			scanner = scanner_;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00007344 File Offset: 0x00005544
		public MismatchedCharException(char c, char expecting_, bool matchNot, CharScanner scanner_) : base("Mismatched char", scanner_.getFilename(), scanner_.getLine(), scanner_.getColumn())
		{
			mismatchType = (matchNot ? CharTypeEnum.NotCharType : CharTypeEnum.CharType);
			foundChar = c;
			expecting = expecting_;
			scanner = scanner_;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00007394 File Offset: 0x00005594
		public MismatchedCharException(char c, BitSet set_, bool matchNot, CharScanner scanner_) : base("Mismatched char", scanner_.getFilename(), scanner_.getLine(), scanner_.getColumn())
		{
			mismatchType = (matchNot ? CharTypeEnum.NotSetType : CharTypeEnum.SetType);
			foundChar = c;
			bset = set_;
			scanner = scanner_;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x000073E4 File Offset: 0x000055E4
		private void appendCharName(StringBuilder sb, int c)
		{
			switch (c)
			{
			case 9:
				sb.Append("'\\t'");
				return;
			case 10:
				sb.Append("'\\n'");
				return;
			case 11:
			case 12:
				break;
			case 13:
				sb.Append("'\\r'");
				return;
			default:
				if (c == 65535)
				{
					sb.Append("'<EOF>'");
					return;
				}
				break;
			}
			sb.Append('\'');
			sb.Append((char)c);
			sb.Append('\'');
		}

		// Token: 0x0400008E RID: 142
		public CharTypeEnum mismatchType;

		// Token: 0x0400008F RID: 143
		public int foundChar;

		// Token: 0x04000090 RID: 144
		public int expecting;

		// Token: 0x04000091 RID: 145
		public int upper;

		// Token: 0x04000092 RID: 146
		public BitSet bset;

		// Token: 0x04000093 RID: 147
		public CharScanner scanner;

		// Token: 0x0200002F RID: 47
		public enum CharTypeEnum
		{
			// Token: 0x04000095 RID: 149
			CharType = 1,
			// Token: 0x04000096 RID: 150
			NotCharType,
			// Token: 0x04000097 RID: 151
			RangeType,
			// Token: 0x04000098 RID: 152
			NotRangeType,
			// Token: 0x04000099 RID: 153
			SetType,
			// Token: 0x0400009A RID: 154
			NotSetType
		}
	}
}
