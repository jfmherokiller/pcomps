using System.Collections.Generic;

namespace pcomps.PCompiler
{
	// Token: 0x02000135 RID: 309
	public class ScriptScope
	{
		// Token: 0x06000AD2 RID: 2770 RVA: 0x000326B4 File Offset: 0x000308B4
		public ScriptScope(ScriptScope akParentScope, string asLocalName)
		{
			this.kParent = akParentScope;
			if (this.kParent != null)
			{
				this.sName = this.kParent.Name + "." + asLocalName;
				this.kParent.kChildren.Add(this);
				return;
			}
			this.sName = asLocalName;
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000AD3 RID: 2771 RVA: 0x0003272C File Offset: 0x0003092C
		public string Name
		{
			get
			{
				return this.sName;
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x00032734 File Offset: 0x00030934
		public ScriptScope Root
		{
			get
			{
				ScriptScope scriptScope = this;
				while (scriptScope.Parent != null)
				{
					scriptScope = scriptScope.Parent;
				}
				return scriptScope;
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x00032758 File Offset: 0x00030958
		public ScriptScope Parent
		{
			get
			{
				return this.kParent;
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x00032760 File Offset: 0x00030960
		public List<ScriptScope> Children
		{
			get
			{
				return this.kChildren;
			}
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x00032768 File Offset: 0x00030968
		public bool TryDefineVariable(string asName, ScriptVariableType akType)
		{
			bool flag = !this.kVars.ContainsKey(asName.ToLowerInvariant());
			if (flag)
			{
				this.kVars.Add(asName.ToLowerInvariant(), new ScriptScope.ScopeVariable(akType));
			}
			return flag;
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x000327A8 File Offset: 0x000309A8
		public bool TryGetVariable(string asName, out ScriptVariableType akType)
		{
			ScriptScope.ScopeVariable scopeVariable;
			bool flag = this.kVars.TryGetValue(asName.ToLowerInvariant(), out scopeVariable);
			if (flag)
			{
				akType = scopeVariable.kType;
			}
			else if (this.kParent != null)
			{
				flag = this.kParent.TryGetVariable(asName, out akType);
			}
			else
			{
				akType = null;
			}
			return flag;
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x000327F4 File Offset: 0x000309F4
		public bool VariableWouldShadow(string asName)
		{
			ScriptVariableType scriptVariableType;
			return this.Parent != null && this.Parent.TryGetVariable(asName, out scriptVariableType);
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x0003281C File Offset: 0x00030A1C
		public bool TryFlagVarAsUsed(string asName)
		{
			ScriptScope.ScopeVariable scopeVariable;
			bool flag = this.kVars.TryGetValue(asName.ToLowerInvariant(), out scopeVariable);
			if (flag)
			{
				scopeVariable.bUsed = true;
			}
			else if (this.kParent != null)
			{
				flag = this.kParent.TryFlagVarAsUsed(asName);
			}
			return flag;
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x00032860 File Offset: 0x00030A60
		public bool TryGetVarUsed(string asName, out bool abUsed)
		{
			ScriptScope.ScopeVariable scopeVariable;
			bool flag = this.kVars.TryGetValue(asName.ToLowerInvariant(), out scopeVariable);
			if (flag)
			{
				abUsed = scopeVariable.bUsed;
			}
			else if (this.kParent != null)
			{
				flag = this.kParent.TryGetVarUsed(asName, out abUsed);
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
			foreach (KeyValuePair<string, ScriptScope.ScopeVariable> keyValuePair in this.kVars)
			{
				keyValuePair.Value.bUsed = false;
			}
			foreach (ScriptScope scriptScope in this.kChildren)
			{
				scriptScope.ClearUsedVars();
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000ADD RID: 2781 RVA: 0x00032948 File Offset: 0x00030B48
		public Dictionary<string, ScriptScope.ScopeVariable> Variables
		{
			get
			{
				return this.kVars;
			}
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x00032950 File Offset: 0x00030B50
		public string GetMangledVariableName(string asVarName)
		{
			string result = asVarName;
			if (this.kMangledVarNames.ContainsKey(asVarName.ToLowerInvariant()))
			{
				this.kMangledVarNames.TryGetValue(asVarName.ToLowerInvariant(), out result);
			}
			else if (this.Parent != null)
			{
				result = this.Parent.GetMangledVariableName(asVarName);
			}
			return result;
		}

		// Token: 0x04000500 RID: 1280
		private string sName;

		// Token: 0x04000501 RID: 1281
		private ScriptScope kParent;

		// Token: 0x04000502 RID: 1282
		private List<ScriptScope> kChildren = new List<ScriptScope>();

		// Token: 0x04000503 RID: 1283
		private Dictionary<string, ScriptScope.ScopeVariable> kVars = new Dictionary<string, ScriptScope.ScopeVariable>();

		// Token: 0x04000504 RID: 1284
		public Dictionary<string, string> kMangledVarNames = new Dictionary<string, string>();

		// Token: 0x02000136 RID: 310
		public class ScopeVariable
		{
			// Token: 0x06000ADF RID: 2783 RVA: 0x000329A0 File Offset: 0x00030BA0
			public ScopeVariable(ScriptVariableType akType)
			{
				this.kType = akType;
			}

			// Token: 0x04000505 RID: 1285
			public bool bUsed;

			// Token: 0x04000506 RID: 1286
			public ScriptVariableType kType;
		}
	}
}
