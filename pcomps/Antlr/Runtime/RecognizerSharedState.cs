using System.Collections;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000DD RID: 221
	public class RecognizerSharedState
	{
		// Token: 0x0400026E RID: 622
		public BitSet[] following = new BitSet[100];

		// Token: 0x0400026F RID: 623
		public int followingStackPointer = -1;

		// Token: 0x04000270 RID: 624
		public bool errorRecovery;

		// Token: 0x04000271 RID: 625
		public int lastErrorIndex = -1;

		// Token: 0x04000272 RID: 626
		public bool failed;

		// Token: 0x04000273 RID: 627
		public int syntaxErrors;

		// Token: 0x04000274 RID: 628
		public int backtracking;

		// Token: 0x04000275 RID: 629
		public IDictionary[] ruleMemo;

		// Token: 0x04000276 RID: 630
		public IToken token;

		// Token: 0x04000277 RID: 631
		public int tokenStartCharIndex = -1;

		// Token: 0x04000278 RID: 632
		public int tokenStartLine;

		// Token: 0x04000279 RID: 633
		public int tokenStartCharPositionInLine;

		// Token: 0x0400027A RID: 634
		public int channel;

		// Token: 0x0400027B RID: 635
		public int type;

		// Token: 0x0400027C RID: 636
		public string text;
	}
}
