namespace pcomps.PCompiler
{
	// Token: 0x020001C1 RID: 449
	public class ScriptVariableType
	{
		// Token: 0x06000D5B RID: 3419 RVA: 0x0005DE48 File Offset: 0x0005C048
		public ScriptVariableType(string asVarType)
		{
			VarType = asVarType;
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0005DE64 File Offset: 0x0005C064
		public ScriptVariableType(ScriptVariableType akVarType)
		{
			VarType = akVarType.VarType;
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000D5D RID: 3421 RVA: 0x0005DE84 File Offset: 0x0005C084
		// (set) Token: 0x06000D5E RID: 3422 RVA: 0x0005DE8C File Offset: 0x0005C08C
		public string VarType
		{
			get
			{
				return sVarType;
			}
			set
			{
				sVarType = value.ToLowerInvariant();
				if (sVarType.Length > 2 && sVarType.Substring(sVarType.Length - 2, 2) == "[]")
				{
					sElementType = sVarType.Substring(0, sVarType.Length - 2);
					bIsArray = true;
					return;
				}
				sElementType = "";
				bIsArray = false;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000D5F RID: 3423 RVA: 0x0005DF14 File Offset: 0x0005C114
		public bool IsArray
		{
			get
			{
				return bIsArray;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000D60 RID: 3424 RVA: 0x0005DF1C File Offset: 0x0005C11C
		public string ArrayElementType
		{
			get
			{
				return sElementType;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000D61 RID: 3425 RVA: 0x0005DF24 File Offset: 0x0005C124
		public bool IsObjectType
		{
			get
			{
				return VarType != "none" && VarType != "int" && VarType != "float" && VarType != "string" && VarType != "bool" && !IsArray;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000D62 RID: 3426 RVA: 0x0005DF98 File Offset: 0x0005C198
		public bool IsElementObjectType
		{
			get
			{
				return ArrayElementType != "none" && ArrayElementType != "int" && ArrayElementType != "float" && ArrayElementType != "string" && ArrayElementType != "bool";
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000D63 RID: 3427 RVA: 0x0005E000 File Offset: 0x0005C200
		public bool HasInitialValue
		{
			get
			{
				return bHasInitialValue;
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000D64 RID: 3428 RVA: 0x0005E008 File Offset: 0x0005C208
		// (set) Token: 0x06000D65 RID: 3429 RVA: 0x0005E02C File Offset: 0x0005C22C
		public string InitialValue
		{
			get
			{
				string result = "";
				if (HasInitialValue)
				{
					result = sInitialValue;
				}
				return result;
			}
			set
			{
				sInitialValue = value;
				bHasInitialValue = true;
			}
		}

		// Token: 0x040009DF RID: 2527
		public const string sNoneTypeC = "none";

		// Token: 0x040009E0 RID: 2528
		public const string sIntTypeC = "int";

		// Token: 0x040009E1 RID: 2529
		public const string sFloatTypeC = "float";

		// Token: 0x040009E2 RID: 2530
		public const string sStringTypeC = "string";

		// Token: 0x040009E3 RID: 2531
		public const string sBoolTypeC = "bool";

		// Token: 0x040009E4 RID: 2532
		private string sVarType;

		// Token: 0x040009E5 RID: 2533
		private bool bIsArray;

		// Token: 0x040009E6 RID: 2534
		private string sElementType;

		// Token: 0x040009E7 RID: 2535
		private bool bHasInitialValue;

		// Token: 0x040009E8 RID: 2536
		private string sInitialValue;

		// Token: 0x040009E9 RID: 2537
		public string ShadowVariableName = "";
	}
}
