using System;
using System.Text;

namespace pcomps.Antlr
{
	// Token: 0x02000033 RID: 51
	[Serializable]
	public class NoViableAltForCharException : RecognitionException
	{
		// Token: 0x06000233 RID: 563 RVA: 0x000079FC File Offset: 0x00005BFC
		public NoViableAltForCharException(char c, CharScanner scanner) : base("NoViableAlt", scanner.getFilename(), scanner.getLine(), scanner.getColumn())
		{
			this.foundChar = c;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00007A30 File Offset: 0x00005C30
		public NoViableAltForCharException(char c, string fileName, int line, int column) : base("NoViableAlt", fileName, line, column)
		{
			this.foundChar = c;
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000235 RID: 565 RVA: 0x00007A54 File Offset: 0x00005C54
		public override string Message
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("unexpected char: ");
				if (this.foundChar >= ' ' && this.foundChar <= '~')
				{
					stringBuilder.Append('\'');
					stringBuilder.Append(this.foundChar);
					stringBuilder.Append('\'');
				}
				else
				{
					stringBuilder.Append("0x");
					StringBuilder stringBuilder2 = stringBuilder;
					int num = (int)this.foundChar;
					stringBuilder2.Append(num.ToString("X"));
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x040000AC RID: 172
		public char foundChar;
	}
}
