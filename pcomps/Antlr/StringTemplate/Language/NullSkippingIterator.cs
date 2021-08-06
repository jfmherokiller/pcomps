﻿using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x0200024A RID: 586
	public sealed class NullSkippingIterator : IEnumerator
	{
		// Token: 0x060011B8 RID: 4536 RVA: 0x00082280 File Offset: 0x00080480
		public NullSkippingIterator(IEnumerator enumerator)
		{
			this.innerEnumerator = enumerator;
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060011B9 RID: 4537 RVA: 0x00082290 File Offset: 0x00080490
		public object Current
		{
			get
			{
				if (!this.hasCurrent)
				{
					throw new InvalidOperationException("Enumeration not started or already finished.");
				}
				return this.innerEnumerator.Current;
			}
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x000822B0 File Offset: 0x000804B0
		public bool MoveNext()
		{
			while (this.innerEnumerator.MoveNext())
			{
				if (this.innerEnumerator.Current != null)
				{
					return this.hasCurrent = true;
				}
			}
			return this.hasCurrent = false;
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x000822F0 File Offset: 0x000804F0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (this.MoveNext())
			{
				object value = this.Current;
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x00082320 File Offset: 0x00080520
		public void Reset()
		{
			this.hasCurrent = false;
			this.innerEnumerator.Reset();
		}

		// Token: 0x04000EFA RID: 3834
		private IEnumerator innerEnumerator;

		// Token: 0x04000EFB RID: 3835
		private bool hasCurrent;
	}
}
