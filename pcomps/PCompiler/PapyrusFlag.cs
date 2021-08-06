namespace pcomps.PCompiler
{
	// Token: 0x020001BB RID: 443
	internal class PapyrusFlag
	{
		// Token: 0x06000D21 RID: 3361 RVA: 0x0005CB10 File Offset: 0x0005AD10
		public PapyrusFlag(int aiIndex, bool abOnObj, bool abOnProp, bool abOnVar, bool abOnFunc)
		{
			if (aiIndex < 0)
			{
				iIndex = 0;
			}
			else if (aiIndex > 31)
			{
				iIndex = 31;
			}
			else
			{
				iIndex = aiIndex;
			}
			bAllowedOnObj = abOnObj;
			bAllowedOnProp = abOnProp;
			bAllowedOnVar = abOnVar;
			bAllowedOnFunc = abOnFunc;
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0005CB64 File Offset: 0x0005AD64
		public static bool IsValidFlagIndex(int aiIndex)
		{
			return aiIndex >= 0 && aiIndex <= 31;
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000D23 RID: 3363 RVA: 0x0005CB74 File Offset: 0x0005AD74
		public int Index
		{
			get
			{
				return iIndex;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000D24 RID: 3364 RVA: 0x0005CB7C File Offset: 0x0005AD7C
		public bool AllowedOnObj
		{
			get
			{
				return bAllowedOnObj;
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000D25 RID: 3365 RVA: 0x0005CB84 File Offset: 0x0005AD84
		public bool AllowedOnProp
		{
			get
			{
				return bAllowedOnProp;
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000D26 RID: 3366 RVA: 0x0005CB8C File Offset: 0x0005AD8C
		public bool AllowedOnVar
		{
			get
			{
				return bAllowedOnVar;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000D27 RID: 3367 RVA: 0x0005CB94 File Offset: 0x0005AD94
		public bool AllowedOnFunc
		{
			get
			{
				return bAllowedOnFunc;
			}
		}

		// Token: 0x040009BD RID: 2493
		private int iIndex;

		// Token: 0x040009BE RID: 2494
		private bool bAllowedOnObj;

		// Token: 0x040009BF RID: 2495
		private bool bAllowedOnProp;

		// Token: 0x040009C0 RID: 2496
		private bool bAllowedOnVar;

		// Token: 0x040009C1 RID: 2497
		private bool bAllowedOnFunc;
	}
}
