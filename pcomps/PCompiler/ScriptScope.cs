using System.Collections.Generic;

namespace pcomps.PCompiler
{
	// Token: 0x02000135 RID: 309
	public record ScriptScope
	{
		// Token: 0x06000AD2 RID: 2770 RVA: 0x000326B4 File Offset: 0x000308B4
		public ScriptScope(ScriptScope akParentScope, string asLocalName)
		{
			kParent = akParentScope;
            if (kParent == null)
            {
                sName = asLocalName;
            }
            else
            {
                sName = $"{kParent.Name}.{asLocalName}";
                kParent.kChildren.Add(this);
            }
        }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x0003272C File Offset: 0x0003092C
		public string Name => sName;

        // Token: 0x17000135 RID: 309
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x00032734 File Offset: 0x00030934
		public ScriptScope Root
		{
			get
			{
				var scriptScope = this;
				while (scriptScope.Parent != null)
				{
					scriptScope = scriptScope.Parent;
				}
				return scriptScope;
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x00032758 File Offset: 0x00030958
		public ScriptScope Parent => kParent;

        // Token: 0x17000137 RID: 311
		// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x00032760 File Offset: 0x00030960
		public List<ScriptScope> Children => kChildren;

        // Token: 0x06000AD7 RID: 2775 RVA: 0x00032768 File Offset: 0x00030968
		public bool TryDefineVariable(string asName, ScriptVariableType akType)
		{
			var flag = !kVars.ContainsKey(asName.ToLowerInvariant());
			if (flag)
			{
				kVars.Add(asName.ToLowerInvariant(), new ScopeVariable(akType));
			}
			return flag;
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x000327A8 File Offset: 0x000309A8
		public bool TryGetVariable(string asName, out ScriptVariableType akType)
		{
            var flag = kVars.TryGetValue(asName.ToLowerInvariant(), out var scopeVariable);
			if (flag)
			{
				akType = scopeVariable.kType;
			}
			else if (kParent != null)
			{
				flag = kParent.TryGetVariable(asName, out akType);
			}
			else
			{
				akType = null;
			}
			return flag;
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x000327F4 File Offset: 0x000309F4
		public bool VariableWouldShadow(string asName) => Parent != null && Parent.TryGetVariable(asName, out _);

        // Token: 0x06000ADA RID: 2778 RVA: 0x0003281C File Offset: 0x00030A1C
		public bool TryFlagVarAsUsed(string asName)
		{
            var flag = kVars.TryGetValue(asName.ToLowerInvariant(), out var scopeVariable);
			if (flag)
			{
				scopeVariable.bUsed = true;
			}
			else if (kParent != null)
			{
				flag = kParent.TryFlagVarAsUsed(asName);
			}
			return flag;
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x00032860 File Offset: 0x00030A60
		public bool TryGetVarUsed(string asName, out bool abUsed)
		{
            var flag = kVars.TryGetValue(asName.ToLowerInvariant(), out var scopeVariable);
			if (flag)
			{
				abUsed = scopeVariable.bUsed;
			}
			else if (kParent != null)
			{
				flag = kParent.TryGetVarUsed(asName, out abUsed);
			}
			else
			{
				abUsed = false;
			}
			return flag;
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x000328AC File Offset: 0x00030AAC
		public void ClearUsedVars()
		{
			foreach (var keyValuePair in kVars)
			{
				keyValuePair.Value.bUsed = false;
			}
			foreach (var scriptScope in kChildren)
			{
				scriptScope.ClearUsedVars();
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000ADD RID: 2781 RVA: 0x00032948 File Offset: 0x00030B48
		public Dictionary<string, ScopeVariable> Variables => kVars;

        // Token: 0x06000ADE RID: 2782 RVA: 0x00032950 File Offset: 0x00030B50
		public string GetMangledVariableName(string asVarName)
		{
			var result = asVarName;
			if (kMangledVarNames.ContainsKey(asVarName.ToLowerInvariant()))
			{
				kMangledVarNames.TryGetValue(asVarName.ToLowerInvariant(), out result);
			}
			else if (Parent != null)
			{
				result = Parent.GetMangledVariableName(asVarName);
			}
			return result;
		}

		// Token: 0x04000500 RID: 1280
		private string sName;

		// Token: 0x04000501 RID: 1281
		private ScriptScope kParent;

		// Token: 0x04000502 RID: 1282
		private List<ScriptScope> kChildren = new();

		// Token: 0x04000503 RID: 1283
		private Dictionary<string, ScopeVariable> kVars = new();

		// Token: 0x04000504 RID: 1284
		public Dictionary<string, string> kMangledVarNames = new();

		// Token: 0x02000136 RID: 310
		public record ScopeVariable
		{
			// Token: 0x06000ADF RID: 2783 RVA: 0x000329A0 File Offset: 0x00030BA0
			public ScopeVariable(ScriptVariableType akType)
			{
				kType = akType;
			}

			// Token: 0x04000505 RID: 1285
			public bool bUsed;

			// Token: 0x04000506 RID: 1286
			public ScriptVariableType kType;
		}
	}
}
