using System.Text;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000CD RID: 205
	public class TreePatternLexer
	{
		// Token: 0x06000861 RID: 2145 RVA: 0x00017A18 File Offset: 0x00015C18
		public TreePatternLexer(string pattern)
		{
			this.pattern = pattern;
			n = pattern.Length;
			Consume();
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x00017A4C File Offset: 0x00015C4C
		public int NextToken()
		{
			sval.Length = 0;
			while (c != -1)
			{
				if (c == 32 || c == 10 || c == 13 || c == 9)
				{
					Consume();
				}
				else
				{
					if ((c >= 97 && c <= 122) || (c >= 65 && c <= 90) || c == 95)
					{
						sval.Append((char)c);
						Consume();
						while ((c >= 97 && c <= 122) || (c >= 65 && c <= 90) || (c >= 48 && c <= 57) || c == 95)
						{
							sval.Append((char)c);
							Consume();
						}
						return 3;
					}
					if (c == 40)
					{
						Consume();
						return 1;
					}
					if (c == 41)
					{
						Consume();
						return 2;
					}
					if (c == 37)
					{
						Consume();
						return 5;
					}
					if (c == 58)
					{
						Consume();
						return 6;
					}
					if (c == 46)
					{
						Consume();
						return 7;
					}
					if (c == 91)
					{
						Consume();
						while (c != 93)
						{
							if (c == 92)
							{
								Consume();
								if (c != 93)
								{
									sval.Append('\\');
								}
								sval.Append((char)c);
							}
							else
							{
								sval.Append((char)c);
							}
							Consume();
						}
						Consume();
						return 4;
					}
					Consume();
					error = true;
					return -1;
				}
			}
			return -1;
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x00017C90 File Offset: 0x00015E90
		protected void Consume()
		{
			p++;
			if (p >= n)
			{
				c = -1;
			}
			else
			{
				c = pattern[p];
			}
		}

		// Token: 0x04000228 RID: 552
		public const int EOF = -1;

		// Token: 0x04000229 RID: 553
		public const int BEGIN = 1;

		// Token: 0x0400022A RID: 554
		public const int END = 2;

		// Token: 0x0400022B RID: 555
		public const int ID = 3;

		// Token: 0x0400022C RID: 556
		public const int ARG = 4;

		// Token: 0x0400022D RID: 557
		public const int PERCENT = 5;

		// Token: 0x0400022E RID: 558
		public const int COLON = 6;

		// Token: 0x0400022F RID: 559
		public const int DOT = 7;

		// Token: 0x04000230 RID: 560
		protected string pattern;

		// Token: 0x04000231 RID: 561
		protected int p = -1;

		// Token: 0x04000232 RID: 562
		protected int c;

		// Token: 0x04000233 RID: 563
		protected int n;

		// Token: 0x04000234 RID: 564
		public StringBuilder sval = new StringBuilder();

		// Token: 0x04000235 RID: 565
		public bool error;
	}
}
