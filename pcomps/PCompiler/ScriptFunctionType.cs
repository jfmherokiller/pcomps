using System;
using System.Collections.Generic;

namespace pcomps.PCompiler
{
	// Token: 0x0200020C RID: 524
	public class ScriptFunctionType
	{
		// Token: 0x06000EDC RID: 3804 RVA: 0x0006D674 File Offset: 0x0006B874
		public ScriptFunctionType(string asFuncName, string asObjName, string asStateName, string asPropName)
		{
			this.sName = asFuncName.ToLowerInvariant();
			this.sStateName = asStateName.ToLowerInvariant();
			this.sPropName = asPropName.ToLowerInvariant();
			if (asPropName == "")
			{
				this.kFunctionScope = new ScriptScope(null, string.Concat(new string[]
				{
					asObjName,
					".",
					asStateName,
					".",
					asFuncName
				}));
				return;
			}
			this.kFunctionScope = new ScriptScope(null, string.Concat(new string[]
			{
				asObjName,
				".",
				asStateName,
				".",
				asPropName,
				".",
				asFuncName
			}));
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000EDD RID: 3805 RVA: 0x0006D764 File Offset: 0x0006B964
		public string Name
		{
			get
			{
				return this.sName;
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0006D76C File Offset: 0x0006B96C
		public string StateName
		{
			get
			{
				return this.sStateName;
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000EDF RID: 3807 RVA: 0x0006D774 File Offset: 0x0006B974
		public string PropertyName
		{
			get
			{
				return this.sPropName;
			}
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x0006D77C File Offset: 0x0006B97C
		public bool ParamTypesMatch(ScriptFunctionType akOtherFunc)
		{
			bool flag = true;
			if (this.kParams.Count == akOtherFunc.kParams.Count)
			{
				int num = 0;
				while (flag)
				{
					if (num >= this.kParams.Count)
					{
						break;
					}
					if (this.kParams[num].VarType != akOtherFunc.kParams[num].VarType)
					{
						flag = false;
					}
					num++;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x0006D7EC File Offset: 0x0006B9EC
		public bool ParamDefaultsMatch(ScriptFunctionType akOtherFunc)
		{
			bool flag = true;
			if (this.kParams.Count == akOtherFunc.kParams.Count)
			{
				int num = 0;
				while (flag)
				{
					if (num >= this.kParams.Count)
					{
						break;
					}
					ScriptVariableType scriptVariableType = this.kParams[num];
					ScriptVariableType scriptVariableType2 = akOtherFunc.kParams[num];
					if (scriptVariableType.HasInitialValue == scriptVariableType2.HasInitialValue)
					{
						if (scriptVariableType.HasInitialValue)
						{
							if (scriptVariableType.VarType == "int")
							{
								int num2 = 0;
								int num3 = 0;
								if (!int.TryParse(scriptVariableType.InitialValue, out num2) || !int.TryParse(scriptVariableType2.InitialValue, out num3) || num2 != num3)
								{
									flag = false;
								}
							}
							else if (scriptVariableType.VarType == "float")
							{
								float num4 = 0f;
								float num5 = 0f;
								if (!float.TryParse(scriptVariableType.InitialValue, out num4) || !float.TryParse(scriptVariableType2.InitialValue, out num5) || num4 != num5)
								{
									flag = false;
								}
							}
							else if (scriptVariableType.InitialValue != scriptVariableType2.InitialValue)
							{
								flag = false;
							}
						}
					}
					else
					{
						flag = false;
					}
					num++;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x0006D914 File Offset: 0x0006BB14
		public List<string> ParamNames
		{
			get
			{
				return this.kParamNames;
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x0006D91C File Offset: 0x0006BB1C
		public List<ScriptVariableType> ParamTypes
		{
			get
			{
				return this.kParams;
			}
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x0006D924 File Offset: 0x0006BB24
		public bool TryGetParam(string asName, out ScriptVariableType akType)
		{
			this.sStringMatch = asName;
			int num = this.kParamNames.FindIndex(new Predicate<string>(this.StringMatchesCaseInsensitive));
			bool flag = num != -1;
			if (flag)
			{
				akType = this.kParams[num];
			}
			else
			{
				akType = null;
			}
			return flag;
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0006D970 File Offset: 0x0006BB70
		public bool TryAddParam(string asName, ScriptVariableType akType)
		{
			this.sStringMatch = asName;
			int num = this.kParamNames.FindIndex(new Predicate<string>(this.StringMatchesCaseInsensitive));
			bool flag = num != -1;
			if (!flag)
			{
				this.kParamNames.Add(asName.ToLowerInvariant());
				this.kParams.Add(akType);
			}
			return !flag;
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0006D9C8 File Offset: 0x0006BBC8
		private bool StringMatchesCaseInsensitive(string asOther)
		{
			return this.sStringMatch.ToLowerInvariant() == asOther.ToLowerInvariant();
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000EE7 RID: 3815 RVA: 0x0006D9E0 File Offset: 0x0006BBE0
		public ScriptScope FunctionScope
		{
			get
			{
				return this.kFunctionScope;
			}
		}

		// Token: 0x04000CA1 RID: 3233
		private string sName;

		// Token: 0x04000CA2 RID: 3234
		private string sStateName;

		// Token: 0x04000CA3 RID: 3235
		private string sPropName;

		// Token: 0x04000CA4 RID: 3236
		public ScriptVariableType kRetType = new ScriptVariableType("none");

		// Token: 0x04000CA5 RID: 3237
		public bool bNative;

		// Token: 0x04000CA6 RID: 3238
		public bool bGlobal;

		// Token: 0x04000CA7 RID: 3239
		private List<string> kParamNames = new List<string>();

		// Token: 0x04000CA8 RID: 3240
		private List<ScriptVariableType> kParams = new List<ScriptVariableType>();

		// Token: 0x04000CA9 RID: 3241
		private string sStringMatch = "";

		// Token: 0x04000CAA RID: 3242
		private ScriptScope kFunctionScope;
	}
}
