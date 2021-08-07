using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x0200008F RID: 143
	[Serializable]
	public class MismatchedTokenException : RecognitionException
	{
		// Token: 0x06000544 RID: 1348 RVA: 0x0000FC14 File Offset: 0x0000DE14
		public MismatchedTokenException()
		{
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0000FC1C File Offset: 0x0000DE1C
		public MismatchedTokenException(int expecting, IIntStream input) : base(input)
		{
			this.expecting = expecting;
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x0000FC2C File Offset: 0x0000DE2C
		// (set) Token: 0x06000547 RID: 1351 RVA: 0x0000FC34 File Offset: 0x0000DE34
		public int Expecting
		{
			get
			{
				return expecting;
			}
			set
			{
				expecting = value;
			}
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0000FC40 File Offset: 0x0000DE40
		public override string ToString()
		{
			return string.Concat("MismatchedTokenException(", UnexpectedType, "!=", expecting, ")");
		}

		// Token: 0x0400016E RID: 366
		private int expecting;
	}
}
