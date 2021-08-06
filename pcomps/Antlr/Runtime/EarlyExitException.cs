using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000089 RID: 137
	[Serializable]
	public class EarlyExitException : RecognitionException
	{
		// Token: 0x06000527 RID: 1319 RVA: 0x0000FA3C File Offset: 0x0000DC3C
		public EarlyExitException()
		{
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x0000FA44 File Offset: 0x0000DC44
		public EarlyExitException(int decisionNumber, IIntStream input) : base(input)
		{
			this.decisionNumber = decisionNumber;
		}

		// Token: 0x04000168 RID: 360
		public int decisionNumber;
	}
}
