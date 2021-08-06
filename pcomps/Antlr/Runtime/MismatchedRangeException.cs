using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x0200008D RID: 141
	[Serializable]
	public class MismatchedRangeException : RecognitionException
	{
		// Token: 0x0600053A RID: 1338 RVA: 0x0000FB08 File Offset: 0x0000DD08
		public MismatchedRangeException()
		{
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0000FB10 File Offset: 0x0000DD10
		public MismatchedRangeException(int a, int b, IIntStream input) : base(input)
		{
			this.a = a;
			this.b = b;
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600053C RID: 1340 RVA: 0x0000FB28 File Offset: 0x0000DD28
		// (set) Token: 0x0600053D RID: 1341 RVA: 0x0000FB30 File Offset: 0x0000DD30
		public int A
		{
			get
			{
				return a;
			}
			set
			{
				a = value;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x0000FB3C File Offset: 0x0000DD3C
		// (set) Token: 0x0600053F RID: 1343 RVA: 0x0000FB44 File Offset: 0x0000DD44
		public int B
		{
			get
			{
				return b;
			}
			set
			{
				b = value;
			}
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0000FB50 File Offset: 0x0000DD50
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"MismatchedNotSetException(",
				UnexpectedType,
				" not in [",
				a,
				",",
				b,
				"])"
			});
		}

		// Token: 0x0400016B RID: 363
		private int a;

		// Token: 0x0400016C RID: 364
		private int b;
	}
}
