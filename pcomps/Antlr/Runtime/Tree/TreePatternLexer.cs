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
			this.n = pattern.Length;
			this.Consume();
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x00017A4C File Offset: 0x00015C4C
		public int NextToken()
		{
			this.sval.Length = 0;
			while (this.c != -1)
			{
				if (this.c == 32 || this.c == 10 || this.c == 13 || this.c == 9)
				{
					this.Consume();
				}
				else
				{
					if ((this.c >= 97 && this.c <= 122) || (this.c >= 65 && this.c <= 90) || this.c == 95)
					{
						this.sval.Append((char)this.c);
						this.Consume();
						while ((this.c >= 97 && this.c <= 122) || (this.c >= 65 && this.c <= 90) || (this.c >= 48 && this.c <= 57) || this.c == 95)
						{
							this.sval.Append((char)this.c);
							this.Consume();
						}
						return 3;
					}
					if (this.c == 40)
					{
						this.Consume();
						return 1;
					}
					if (this.c == 41)
					{
						this.Consume();
						return 2;
					}
					if (this.c == 37)
					{
						this.Consume();
						return 5;
					}
					if (this.c == 58)
					{
						this.Consume();
						return 6;
					}
					if (this.c == 46)
					{
						this.Consume();
						return 7;
					}
					if (this.c == 91)
					{
						this.Consume();
						while (this.c != 93)
						{
							if (this.c == 92)
							{
								this.Consume();
								if (this.c != 93)
								{
									this.sval.Append('\\');
								}
								this.sval.Append((char)this.c);
							}
							else
							{
								this.sval.Append((char)this.c);
							}
							this.Consume();
						}
						this.Consume();
						return 4;
					}
					this.Consume();
					this.error = true;
					return -1;
				}
			}
			return -1;
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x00017C90 File Offset: 0x00015E90
		protected void Consume()
		{
			this.p++;
			if (this.p >= this.n)
			{
				this.c = -1;
			}
			else
			{
				this.c = (int)this.pattern[this.p];
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
