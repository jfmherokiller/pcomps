using System;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000D0 RID: 208
	[Serializable]
	public class RewriteCardinalityException : Exception
	{
		// Token: 0x0600086E RID: 2158 RVA: 0x00018044 File Offset: 0x00016244
		public RewriteCardinalityException(string elementDescription)
		{
			this.elementDescription = elementDescription;
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600086F RID: 2159 RVA: 0x00018054 File Offset: 0x00016254
		public override string Message
		{
			get
			{
				if (this.elementDescription != null)
				{
					return this.elementDescription;
				}
				return null;
			}
		}

		// Token: 0x0400023A RID: 570
		public string elementDescription;
	}
}
