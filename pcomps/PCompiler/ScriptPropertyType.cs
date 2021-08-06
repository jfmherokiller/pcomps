namespace pcomps.PCompiler
{
	// Token: 0x02000137 RID: 311
	public class ScriptPropertyType
	{
		// Token: 0x06000AE0 RID: 2784 RVA: 0x000329B0 File Offset: 0x00030BB0
		public ScriptPropertyType(ScriptVariableType akPropType)
		{
			kType = akPropType;
			if (akPropType.ShadowVariableName != "")
			{
				bIsAuto = true;
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x000329D8 File Offset: 0x00030BD8
		public bool IsAuto
		{
			get
			{
				return bIsAuto;
			}
		}

		// Token: 0x04000507 RID: 1287
		public ScriptVariableType kType;

		// Token: 0x04000508 RID: 1288
		public ScriptFunctionType kGetFunction;

		// Token: 0x04000509 RID: 1289
		public ScriptFunctionType kSetFunction;

		// Token: 0x0400050A RID: 1290
		private bool bIsAuto;
	}
}
