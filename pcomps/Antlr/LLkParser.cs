using System;

namespace pcomps.Antlr
{
	// Token: 0x0200002C RID: 44
	public class LLkParser : Parser
	{
		// Token: 0x0600020F RID: 527 RVA: 0x00006E20 File Offset: 0x00005020
		public LLkParser(int k_)
		{
			k = k_;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00006E3C File Offset: 0x0000503C
		public LLkParser(ParserSharedInputState state, int k_)
		{
			k = k_;
			inputState = state;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00006E60 File Offset: 0x00005060
		public LLkParser(TokenBuffer tokenBuf, int k_)
		{
			k = k_;
			setTokenBuffer(tokenBuf);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00006E84 File Offset: 0x00005084
		public LLkParser(TokenStream lexer, int k_)
		{
			k = k_;
			TokenBuffer tokenBuffer = new TokenBuffer(lexer);
			setTokenBuffer(tokenBuffer);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00006EAC File Offset: 0x000050AC
		public override void consume()
		{
			inputState.input.consume();
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00006ECC File Offset: 0x000050CC
		public override int LA(int i)
		{
			return inputState.input.LA(i);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00006EEC File Offset: 0x000050EC
		public override IToken LT(int i)
		{
			return inputState.input.LT(i);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00006F0C File Offset: 0x0000510C
		private void trace(string ee, string rname)
		{
			traceIndent();
			Console.Out.Write(ee + rname + ((inputState.guessing > 0) ? "; [guessing]" : "; "));
			for (int i = 1; i <= k; i++)
			{
				if (i != 1)
				{
					Console.Out.Write(", ");
				}
				if (LT(i) != null)
				{
					Console.Out.Write(string.Concat(new object[]
					{
						"LA(",
						i,
						")==",
						LT(i).getText()
					}));
				}
				else
				{
					Console.Out.Write($"LA({i})==ull");
				}
			}
			Console.Out.WriteLine("");
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00006FEC File Offset: 0x000051EC
		public override void traceIn(string rname)
		{
			traceDepth++;
			trace("> ", rname);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00007014 File Offset: 0x00005214
		public override void traceOut(string rname)
		{
			trace("< ", rname);
			traceDepth--;
		}

		// Token: 0x0400008A RID: 138
		internal int k;
	}
}
