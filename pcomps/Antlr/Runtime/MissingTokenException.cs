using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000098 RID: 152
	[Serializable]
	public class MissingTokenException : MismatchedTokenException
	{
		// Token: 0x06000580 RID: 1408 RVA: 0x000102FC File Offset: 0x0000E4FC
		public MissingTokenException()
		{
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x00010304 File Offset: 0x0000E504
		public MissingTokenException(int expecting, IIntStream input, object inserted) : base(expecting, input)
		{
			this.inserted = inserted;
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x00010318 File Offset: 0x0000E518
		public int MissingType
		{
			get
			{
				return Expecting;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x00010320 File Offset: 0x0000E520
		// (set) Token: 0x06000584 RID: 1412 RVA: 0x00010328 File Offset: 0x0000E528
		public object Inserted
		{
			get
			{
				return inserted;
			}
			set
			{
				inserted = value;
			}
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x00010334 File Offset: 0x0000E534
		public override string ToString()
		{
			if (inserted != null && token != null)
			{
				return string.Concat("MissingTokenException(inserted ", inserted, " at ", token.Text, ")");
			}
			if (token != null)
			{
				return $"MissingTokenException(at {token.Text})";
			}
			return "MissingTokenException";
		}

		// Token: 0x04000189 RID: 393
		private object inserted;
	}
}
