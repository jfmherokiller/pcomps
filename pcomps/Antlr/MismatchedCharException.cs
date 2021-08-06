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
				StringBuilder stringBuilder = new StringBuilder();
				switch (this.mismatchType)
				{
				case MismatchedCharException.CharTypeEnum.CharType:
					stringBuilder.Append("expecting ");
					this.appendCharName(stringBuilder, this.expecting);
					stringBuilder.Append(", found ");
					this.appendCharName(stringBuilder, this.foundChar);
					break;
				case MismatchedCharException.CharTypeEnum.NotCharType:
					stringBuilder.Append("expecting anything but '");
					this.appendCharName(stringBuilder, this.expecting);
					stringBuilder.Append("'; got it anyway");
					break;
				case MismatchedCharException.CharTypeEnum.RangeType:
				case MismatchedCharException.CharTypeEnum.NotRangeType:
					stringBuilder.Append("expecting token ");
					if (this.mismatchType == MismatchedCharException.CharTypeEnum.NotRangeType)
					{
						stringBuilder.Append("NOT ");
					}
					stringBuilder.Append("in range: ");
					this.appendCharName(stringBuilder, this.expecting);
					stringBuilder.Append("..");
					this.appendCharName(stringBuilder, this.upper);
					stringBuilder.Append(", found ");
					this.appendCharName(stringBuilder, this.foundChar);
					break;
				case MismatchedCharException.CharTypeEnum.SetType:
				case MismatchedCharException.CharTypeEnum.NotSetType:
				{
					stringBuilder.Append(
                        $"expecting {((this.mismatchType == MismatchedCharException.CharTypeEnum.NotSetType) ? "NOT " : "")}one of (");
					int[] array = this.bset.toArray();
					for (int i = 0; i < array.Length; i++)
					{
						this.appendCharName(stringBuilder, array[i]);
					}
					stringBuilder.Append("), found ");
					this.appendCharName(stringBuilder, this.foundChar);
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
			this.mismatchType = (matchNot ? MismatchedCharException.CharTypeEnum.NotRangeType : MismatchedCharException.CharTypeEnum.RangeType);
			this.foundChar = (int)c;
			this.expecting = (int)lower;
			this.upper = (int)upper_;
			this.scanner = scanner_;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00007344 File Offset: 0x00005544
		public MismatchedCharException(char c, char expecting_, bool matchNot, CharScanner scanner_) : base("Mismatched char", scanner_.getFilename(), scanner_.getLine(), scanner_.getColumn())
		{
			this.mismatchType = (matchNot ? MismatchedCharException.CharTypeEnum.NotCharType : MismatchedCharException.CharTypeEnum.CharType);
			this.foundChar = (int)c;
			this.expecting = (int)expecting_;
			this.scanner = scanner_;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00007394 File Offset: 0x00005594
		public MismatchedCharException(char c, BitSet set_, bool matchNot, CharScanner scanner_) : base("Mismatched char", scanner_.getFilename(), scanner_.getLine(), scanner_.getColumn())
		{
			this.mismatchType = (matchNot ? MismatchedCharException.CharTypeEnum.NotSetType : MismatchedCharException.CharTypeEnum.SetType);
			this.foundChar = (int)c;
			this.bset = set_;
			this.scanner = scanner_;
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
		public MismatchedCharException.CharTypeEnum mismatchType;

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
