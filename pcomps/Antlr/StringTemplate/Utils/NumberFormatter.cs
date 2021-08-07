namespace pcomps.Antlr.StringTemplate.Utils
{
	// Token: 0x02000254 RID: 596
	public record NumberFormatter
	{
		// Token: 0x060011F1 RID: 4593 RVA: 0x00082D50 File Offset: 0x00080F50
		protected NumberFormatter()
		{
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x00082D58 File Offset: 0x00080F58
		public NumberFormatter(ulong number)
		{
			this.number = number;
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x060011F3 RID: 4595 RVA: 0x00082D68 File Offset: 0x00080F68
		public string HEX => number.ToString("X");

        // Token: 0x17000279 RID: 633
		// (get) Token: 0x060011F4 RID: 4596 RVA: 0x00082D7C File Offset: 0x00080F7C
		public string Hex => number.ToString("x");

        // Token: 0x1700027A RID: 634
		// (get) Token: 0x060011F5 RID: 4597 RVA: 0x00082D90 File Offset: 0x00080F90
		public string Number => number.ToString("N");

        // Token: 0x04000F2E RID: 3886
		protected ulong number;
	}
}
