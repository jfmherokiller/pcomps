using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x0200008E RID: 142
	[Serializable]
	public class MismatchedSetException : RecognitionException
	{
		// Token: 0x06000541 RID: 1345 RVA: 0x0000FBB4 File Offset: 0x0000DDB4
		public MismatchedSetException()
		{
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0000FBBC File Offset: 0x0000DDBC
		public MismatchedSetException(BitSet expecting, IIntStream input) : base(input)
		{
			this.expecting = expecting;
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0000FBCC File Offset: 0x0000DDCC
		public override string ToString()
		{
			return string.Concat("MismatchedSetException(", UnexpectedType, "!=", expecting, ")");
		}

		// Token: 0x0400016D RID: 365
		public BitSet expecting;
	}
}
