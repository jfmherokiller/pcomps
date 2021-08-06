using System;

namespace pcomps.Antlr.StringTemplate.Utils
{
	// Token: 0x02000253 RID: 595
	public class DateTimeFormatter
	{
		// Token: 0x060011EA RID: 4586 RVA: 0x00082CDC File Offset: 0x00080EDC
		protected DateTimeFormatter()
		{
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x00082CE4 File Offset: 0x00080EE4
		public DateTimeFormatter(DateTime dateObj)
		{
			this.dateObj = dateObj;
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060011EC RID: 4588 RVA: 0x00082CF4 File Offset: 0x00080EF4
		public string ShortDate
		{
			get
			{
				return this.dateObj.ToShortDateString();
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060011ED RID: 4589 RVA: 0x00082D04 File Offset: 0x00080F04
		public string ShortDateTime
		{
			get
			{
				return this.dateObj.ToString("g", null);
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060011EE RID: 4590 RVA: 0x00082D18 File Offset: 0x00080F18
		public string LongDate
		{
			get
			{
				return this.dateObj.ToLongDateString();
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060011EF RID: 4591 RVA: 0x00082D28 File Offset: 0x00080F28
		public string LongDateTime
		{
			get
			{
				return this.dateObj.ToString("f", null);
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x060011F0 RID: 4592 RVA: 0x00082D3C File Offset: 0x00080F3C
		public string SortableDateTime
		{
			get
			{
				return this.dateObj.ToString("u", null);
			}
		}

		// Token: 0x04000F2D RID: 3885
		protected DateTime dateObj;
	}
}
