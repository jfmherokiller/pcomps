using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x0200008C RID: 140
	[Serializable]
	public class MismatchedNotSetException : MismatchedSetException
	{
		// Token: 0x06000537 RID: 1335 RVA: 0x0000FAAC File Offset: 0x0000DCAC
		public MismatchedNotSetException()
		{
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x0000FAB4 File Offset: 0x0000DCB4
		public MismatchedNotSetException(BitSet expecting, IIntStream input) : base(expecting, input)
		{
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0000FAC0 File Offset: 0x0000DCC0
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"MismatchedNotSetException(",
				this.UnexpectedType,
				"!=",
				this.expecting,
				")"
			});
		}
	}
}
