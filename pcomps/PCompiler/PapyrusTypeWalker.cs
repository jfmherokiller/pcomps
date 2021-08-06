using System.Collections;
using System.Collections.Generic;
using pcomps.Antlr.Runtime;
using pcomps.Antlr.Runtime.Collections;
using pcomps.Antlr.Runtime.Tree;

namespace pcomps.PCompiler
{
	// Token: 0x02000178 RID: 376
	public class PapyrusTypeWalker : TreeParser
	{
		// Token: 0x06000C1B RID: 3099 RVA: 0x00049C28 File Offset: 0x00047E28
		private bool IsKnownType(ScriptVariableType akType)
		{
			bool result;
			if (akType.IsArray)
			{
				result = this.kKnownTypes.ContainsKey(akType.ArrayElementType);
			}
			else
			{
				result = this.kKnownTypes.ContainsKey(akType.VarType);
			}
			return result;
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x00049C68 File Offset: 0x00047E68
		private bool IsKnownType(string asType)
		{
			return this.IsKnownType(new ScriptVariableType(asType));
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x00049C78 File Offset: 0x00047E78
		private void AddKnownType(string asType)
		{
			if (!this.IsKnownType(asType))
			{
				this.kKnownTypes.Add(asType.ToLowerInvariant(), null);
			}
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x00049C98 File Offset: 0x00047E98
		private void AddKnownType(ScriptObjectType akObjType)
		{
			if (this.IsKnownType(akObjType.Name))
			{
				this.kKnownTypes[akObjType.Name] = akObjType;
				return;
			}
			this.kKnownTypes.Add(akObjType.Name, akObjType);
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x00049CD0 File Offset: 0x00047ED0
		private ScriptObjectType GetKnownType(ScriptVariableType akType)
		{
			string text = akType.VarType;
			if (akType.IsArray)
			{
				text = akType.ArrayElementType;
			}
			ScriptObjectType scriptObjectType = null;
			if (!this.kKnownTypes.TryGetValue(text, out scriptObjectType) || scriptObjectType == null)
			{
				scriptObjectType = this.kCompiler.LoadObject(text, this.kKnownTypes, false);
			}
			return scriptObjectType;
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x00049D20 File Offset: 0x00047F20
		private ScriptObjectType GetKnownType(string asType)
		{
			return this.GetKnownType(new ScriptVariableType(asType));
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000C21 RID: 3105 RVA: 0x00049D30 File Offset: 0x00047F30
		public Dictionary<string, ScriptObjectType> KnownTypes
		{
			get
			{
				return this.kKnownTypes;
			}
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x00049D38 File Offset: 0x00047F38
		private bool IsImportedType(string asType)
		{
			return this.kImportedTypes.ContainsKey(asType.ToLowerInvariant());
		}

		// Token: 0x06000C23 RID: 3107 RVA: 0x00049D4C File Offset: 0x00047F4C
		private void AddImportedType(ScriptObjectType akObjType)
		{
			if (this.IsImportedType(akObjType.Name))
			{
				this.kImportedTypes[akObjType.Name] = akObjType;
				return;
			}
			this.kImportedTypes.Add(akObjType.Name, akObjType);
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000C24 RID: 3108 RVA: 0x00049D84 File Offset: 0x00047F84
		public Dictionary<string, ScriptObjectType> ImportedTypes
		{
			get
			{
				return this.kImportedTypes;
			}
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x00049D8C File Offset: 0x00047F8C
		private void HandleParent(IToken akParent)
		{
			ScriptObjectType scriptObjectType = null;
			string text = akParent.Text.ToLowerInvariant();
			if (text == this.kObjType.Name)
			{
				this.OnError("cannot extend ourself", akParent.Line, akParent.CharPositionInLine);
			}
			else if (this.kChildren.Contains(text))
			{
				this.OnError(string.Format("cannot extend from {0} as it extends from us", akParent.Text), akParent.Line, akParent.CharPositionInLine);
			}
			else if (!this.kKnownTypes.TryGetValue(text, out scriptObjectType))
			{
				this.kChildren.Push(this.kObjType.Name);
				scriptObjectType = this.kCompiler.LoadObject(akParent.Text, this.kKnownTypes, this.kChildren, true, this.kObjType);
				this.kChildren.Pop();
			}
			else if (scriptObjectType == null)
			{
				this.OnError("cannot extend a built-in type", akParent.Line, akParent.CharPositionInLine);
			}
			if (scriptObjectType != null)
			{
				this.kObjType.kParent = scriptObjectType;
				this.AddImportedType(scriptObjectType);
			}
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x00049E94 File Offset: 0x00048094
		private bool ValueTypeMatches(ScriptVariableType akType, IToken akValue)
		{
			string varType;
			if ((varType = akType.VarType) != null)
			{
				if (varType == "int")
				{
					return akValue.Type == 81;
				}
				if (varType == "float")
				{
					return akValue.Type == 93;
				}
				if (varType == "bool")
				{
					return akValue.Type == 91;
				}
				if (varType == "string")
				{
					return akValue.Type == 90;
				}
			}
			return akValue.Type == 92;
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x00049F24 File Offset: 0x00048124
		private ScriptVariableType GetVariableType(ScriptFunctionType akFunction, ScriptScope akCurrentScope, IToken akVarToken)
		{
			string asName = akVarToken.Text.ToLowerInvariant();
			ScriptVariableType result;
			if (!akCurrentScope.TryGetVariable(asName, out result) && (akFunction.bGlobal || !this.kObjType.TryGetVariable(asName, out result)))
			{
				this.OnError(string.Format("variable {0} is undefined", akVarToken.Text), akVarToken.Line, akVarToken.CharPositionInLine);
				result = new ScriptVariableType("none");
			}
			return result;
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x00049F90 File Offset: 0x00048190
		private bool IsLocalProperty(string asName)
		{
			ScriptObjectType kParent = this.kObjType;
			bool flag = false;
			while (!flag && kParent != null)
			{
				ScriptPropertyType scriptPropertyType;
				flag = kParent.TryGetProperty(asName, out scriptPropertyType);
				kParent = kParent.kParent;
			}
			return flag;
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x00049FC0 File Offset: 0x000481C0
		private void GetPropertyInfo(ScriptVariableType akObjType, IToken akIDToken, bool abCheckForGetter, out ScriptVariableType akPropType)
		{
			akPropType = new ScriptVariableType("none");
			ScriptObjectType scriptObjectType = this.GetKnownType(akObjType.VarType);
			if (scriptObjectType == null)
			{
				this.OnError(string.Format("{0} is not a known user-defined type", akObjType.VarType), akIDToken.Line, akIDToken.CharPositionInLine);
				return;
			}
			ScriptPropertyType scriptPropertyType = null;
			while (scriptPropertyType == null && scriptObjectType != null)
			{
				if (!scriptObjectType.TryGetProperty(akIDToken.Text, out scriptPropertyType))
				{
					scriptObjectType = scriptObjectType.kParent;
				}
			}
			if (scriptPropertyType == null)
			{
				this.OnError(string.Format("{0} is not a property on script {1} or one of its parents", akIDToken.Text, akObjType.VarType), akIDToken.Line, akIDToken.CharPositionInLine);
				return;
			}
			if (abCheckForGetter)
			{
				if (scriptPropertyType.kGetFunction == null)
				{
					this.OnError(string.Format("property {0} on script {1} is write-only, you cannot retrieve a value from it", akIDToken.Text, scriptObjectType.Name), akIDToken.Line, akIDToken.CharPositionInLine);
					return;
				}
				akPropType = scriptPropertyType.kType;
				return;
			}
			else
			{
				if (scriptPropertyType.kSetFunction == null)
				{
					this.OnError(string.Format("property {0} on script {1} is read-only, you cannot give it a value", akIDToken.Text, scriptObjectType.Name), akIDToken.Line, akIDToken.CharPositionInLine);
					return;
				}
				akPropType = scriptPropertyType.kType;
				return;
			}
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x0004A0D8 File Offset: 0x000482D8
		private bool IsGlobalFunction(ScriptVariableType akType, string asFuncName)
		{
			bool result = false;
			ScriptObjectType knownType = this.GetKnownType(akType.VarType);
			ScriptFunctionType scriptFunctionType;
			if (knownType != null && knownType.TryGetFunction(asFuncName.ToLowerInvariant(), out scriptFunctionType))
			{
				result = scriptFunctionType.bGlobal;
			}
			return result;
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x0004A110 File Offset: 0x00048310
		private ScriptVariableType FindFunctionOwningType(string asFuncName, out bool abCallingOnSelf, IToken akTargetToken)
		{
			ScriptVariableType scriptVariableType = new ScriptVariableType(this.kObjType.Name);
			abCallingOnSelf = true;
			ScriptFunctionType scriptFunctionType;
			if (!this.kObjType.TryGetFunction(asFuncName, out scriptFunctionType))
			{
				bool flag = false;
				ScriptObjectType kParent = this.kObjType.kParent;
				while (kParent != null && !flag)
				{
					if (kParent.TryGetFunction(asFuncName, out scriptFunctionType))
					{
						flag = true;
					}
					kParent = kParent.kParent;
				}
				if (!flag)
				{
					foreach (ScriptObjectType scriptObjectType in this.kImportedTypes.Values)
					{
						if (scriptObjectType.TryGetFunction(asFuncName, out scriptFunctionType))
						{
							if (!abCallingOnSelf)
							{
								this.OnError(string.Format("Function {0} is ambiguous between types {1} and {2}", asFuncName, scriptVariableType.VarType, scriptObjectType.Name), akTargetToken.Line, akTargetToken.CharPositionInLine);
							}
							scriptVariableType = new ScriptVariableType(scriptObjectType.Name);
							abCallingOnSelf = false;
						}
					}
				}
			}
			return scriptVariableType;
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x0004A208 File Offset: 0x00048408
		private void CheckVariableDefinition(string asName, ScriptVariableType akType, IToken akInitialValue, bool abInFunctionBlock, IToken akTargetToken)
		{
			this.CheckTypeAndValue(akType, akInitialValue, akTargetToken);
			this.CheckVarOrPropName(asName, akTargetToken);
			if (abInFunctionBlock)
			{
				ScriptVariableType scriptVariableType;
				if (this.kObjType.TryGetVariable(asName.ToLowerInvariant(), out scriptVariableType))
				{
					this.OnError(string.Format("variable {0} already defined as a script variable", asName), akTargetToken.Line, akTargetToken.CharPositionInLine);
					return;
				}
				ScriptPropertyType scriptPropertyType;
				if (this.kObjType.TryGetProperty(asName.ToLowerInvariant(), out scriptPropertyType))
				{
					this.OnError(string.Format("variable {0} already defined as a property", asName), akTargetToken.Line, akTargetToken.CharPositionInLine);
				}
			}
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x0004A298 File Offset: 0x00048498
		private void CheckVarOrPropName(string asName, IToken akTargetToken)
		{
			ScriptObjectType knownType = this.GetKnownType(asName);
			if (knownType != null)
			{
				this.OnError("cannot name a variable or property the same as a known type or script", akTargetToken.Line, akTargetToken.CharPositionInLine);
			}
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x0004A2C8 File Offset: 0x000484C8
		private void CheckTypeAndValue(ScriptVariableType akType, IToken akValue, IToken akErrorToken)
		{
			if (!this.IsKnownType(akType))
			{
				if (this.GetKnownType(akType) == null)
				{
					this.OnError(string.Format("unknown type {0}", akType.VarType), akErrorToken.Line, akErrorToken.CharPositionInLine);
					return;
				}
			}
			else if (akValue != null && !this.ValueTypeMatches(akType, akValue))
			{
				this.OnError(string.Format("cannot initialize a {0} to {1}", akType.VarType, akValue.Text), akErrorToken.Line, akErrorToken.CharPositionInLine);
			}
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x0004A344 File Offset: 0x00048544
		private bool CanAutoCast(ScriptVariableType akTarget, ScriptVariableType akSource)
		{
			bool flag = akTarget.VarType == akSource.VarType;
			if (!flag)
			{
				if (akTarget.IsArray)
				{
					flag = (akSource.VarType == "none");
				}
				else if (!akTarget.IsObjectType)
				{
					if (akTarget.VarType == "bool")
					{
						flag = true;
					}
					else if (akTarget.VarType == "string")
					{
						flag = true;
					}
					else if (akTarget.VarType == "float")
					{
						flag = (akSource.VarType == "int");
					}
				}
				else if (akSource.VarType == "none")
				{
					flag = true;
				}
				else
				{
					ScriptObjectType scriptObjectType = this.GetKnownType(akSource.VarType);
					if (scriptObjectType != null)
					{
						scriptObjectType = scriptObjectType.kParent;
						while (!flag && scriptObjectType != null)
						{
							flag = (scriptObjectType.Name == akTarget.VarType);
							scriptObjectType = scriptObjectType.kParent;
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x0004A434 File Offset: 0x00048634
		private CommonTree AutoCast(IToken akTokenToCast, ScriptVariableType akSourceType, ScriptVariableType akDestType, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out string asNewTempVar)
		{
			CommonTree commonTree = new CommonTree(akTokenToCast);
			asNewTempVar = "";
			if (akTokenToCast != null && akSourceType.VarType != akDestType.VarType)
			{
				asNewTempVar = this.GenerateTempVariable(akDestType, akCurrentScope, akTempVars);
				CommonToken commonToken = new CommonToken(akTokenToCast);
				commonToken.Type = 79;
				commonToken.Text = "AS";
				CommonToken commonToken2 = new CommonToken(akTokenToCast);
				commonToken2.Type = 38;
				commonToken2.Text = asNewTempVar;
				commonTree = new CommonTree(commonToken);
				commonTree.AddChild(new CommonTree(commonToken2));
				commonTree.AddChild(new CommonTree(akTokenToCast));
			}
			return commonTree;
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x0004A4C8 File Offset: 0x000486C8
		private CommonTree AutoCastReturn(IToken akTokenToCast, ScriptVariableType akSourceType, string asFuncName, string asPropertyName, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, IToken akRetToken)
		{
			CommonTree result = null;
			ScriptFunctionType scriptFunctionType = null;
			ScriptPropertyType scriptPropertyType;
			if (asPropertyName == "")
			{
				if (!this.kObjType.TryGetFunction(asFuncName, out scriptFunctionType))
				{
					this.OnError(string.Format("internal error: cannot type-check return because function {0} is not on object {1}", asFuncName, this.kObjType.Name), akRetToken.Line, akRetToken.CharPositionInLine);
				}
			}
			else if (this.kObjType.TryGetProperty(asPropertyName, out scriptPropertyType))
			{
				string a = asFuncName.ToLowerInvariant();
				if (a == "get")
				{
					scriptFunctionType = scriptPropertyType.kGetFunction;
				}
				else if (a == "set")
				{
					scriptFunctionType = scriptPropertyType.kSetFunction;
				}
				else
				{
					this.OnError(string.Format("Error: Function {0} in property {1} must be either get or set", asFuncName, asPropertyName), akRetToken.Line, akRetToken.CharPositionInLine);
				}
			}
			else
			{
				this.OnError(string.Format("internal error: cannot type-check return because property {0} is not on object {1}", asPropertyName, this.kObjType.Name), akRetToken.Line, akRetToken.CharPositionInLine);
			}
			if (scriptFunctionType != null)
			{
				string asVarName;
				result = this.AutoCast(akTokenToCast, akSourceType, scriptFunctionType.kRetType, akCurrentScope, akTempVars, out asVarName);
				this.MarkTempVarAsUnused(scriptFunctionType.kRetType, asVarName, akRetToken);
			}
			return result;
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x0004A5E4 File Offset: 0x000487E4
		private void CheckAssignmentType(ScriptVariableType akTarget, ScriptVariableType akSource, IToken akTargetToken)
		{
			if (akTarget != null && akSource != null && !this.CanAutoCast(akTarget, akSource))
			{
				this.OnError(string.Format("type mismatch while assigning to a {0} (cast missing or types unrelated)", akTarget.VarType), akTargetToken.Line, akTargetToken.CharPositionInLine);
			}
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x0004A618 File Offset: 0x00048818
		private ScriptVariableType CheckComparisonType(ScriptVariableType akTypeA, ScriptVariableType akTypeB, IToken akOpToken, out bool abCastToA)
		{
			bool flag = akOpToken.Type != 67 && akOpToken.Type != 68;
			bool flag2 = akTypeA.VarType == "none" || akTypeB.VarType == "none";
			abCastToA = true;
			if (!flag2)
			{
				if (!this.CanAutoCast(akTypeA, akTypeB))
				{
					if (!this.CanAutoCast(akTypeB, akTypeA))
					{
						this.OnError(string.Format("cannot compare a {0} to a {1} (cast missing or types unrelated)", akTypeA.VarType, akTypeB.VarType), akOpToken.Line, akOpToken.CharPositionInLine);
					}
					else
					{
						abCastToA = false;
					}
				}
				else if (flag && (akTypeA.IsObjectType || akTypeA.IsArray || akTypeA.VarType == "bool"))
				{
					this.OnError(string.Format("cannot relatively compare variables of type {0}", akTypeA.VarType), akOpToken.Line, akOpToken.CharPositionInLine);
				}
			}
			else
			{
				ScriptVariableType scriptVariableType = (akTypeA.VarType == "none") ? akTypeB : akTypeA;
				if (!scriptVariableType.IsObjectType && !scriptVariableType.IsArray && scriptVariableType.VarType != "none")
				{
					this.OnError(string.Format("cannot compare a {0} to a {1} (cast missing or types unrelated)", akTypeA.VarType, akTypeB.VarType), akOpToken.Line, akOpToken.CharPositionInLine);
				}
				if (flag)
				{
					this.OnError("cannot relatively compare variables to None", akOpToken.Line, akOpToken.CharPositionInLine);
				}
			}
			return new ScriptVariableType("bool");
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x0004A790 File Offset: 0x00048990
		private void HandleComparisonExpression(string asExprAVar, ScriptVariableType akExprAType, IToken akExprAToken, string asExprBVar, ScriptVariableType akExprBType, IToken akExprBToken, IToken akComparisonToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out string asResultVar, out ScriptVariableType akResultType, out IToken akResultToken, out CommonTree akATreeOut, out CommonTree akBTreeOut)
		{
			bool flag;
			akResultType = this.CheckComparisonType(akExprAType, akExprBType, akComparisonToken, out flag);
			if (flag)
			{
				akATreeOut = new CommonTree(akExprAToken);
				string asVarName;
				akBTreeOut = this.AutoCast(akExprBToken, akExprBType, akExprAType, akCurrentScope, akTempVars, out asVarName);
				this.MarkTempVarAsUnused(akExprAType, asVarName, akComparisonToken);
			}
			else
			{
				string asVarName2;
				akATreeOut = this.AutoCast(akExprAToken, akExprAType, akExprBType, akCurrentScope, akTempVars, out asVarName2);
				akBTreeOut = new CommonTree(akExprBToken);
				this.MarkTempVarAsUnused(akExprBType, asVarName2, akComparisonToken);
			}
			this.MarkTempVarAsUnused(akExprAType, asExprAVar, akComparisonToken);
			this.MarkTempVarAsUnused(akExprBType, asExprBVar, akComparisonToken);
			asResultVar = this.GenerateTempVariable(akResultType, akCurrentScope, akTempVars);
			akResultToken = new CommonToken(akComparisonToken);
			akResultToken.Type = 38;
			akResultToken.Text = asResultVar;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x0004A848 File Offset: 0x00048A48
		private ScriptVariableType CheckAddSubtractType(ScriptVariableType akTypeA, ScriptVariableType akTypeB, IToken akOpToken, out bool abCastToA, out bool abIsConcat)
		{
			bool flag = akOpToken.Type == 73;
			bool flag2 = true;
			abIsConcat = (flag && (akTypeA.VarType == "string" || akTypeB.VarType == "string"));
			if (abIsConcat)
			{
				abCastToA = (akTypeA.VarType == "string");
				if (abCastToA)
				{
					flag2 = this.CanAutoCast(akTypeA, akTypeB);
				}
				else
				{
					flag2 = this.CanAutoCast(akTypeB, akTypeA);
				}
			}
			else
			{
				abCastToA = true;
				if (!this.CanAutoCast(akTypeA, akTypeB))
				{
					if (!this.CanAutoCast(akTypeB, akTypeA))
					{
						flag2 = false;
					}
					else
					{
						abCastToA = false;
					}
				}
				else if (akTypeA.VarType != "int" && akTypeA.VarType != "float")
				{
					flag2 = false;
				}
			}
			if (!flag2)
			{
				string asError;
				if (abIsConcat)
				{
					asError = string.Format("cannot concatenate a {1} and a {2}", akTypeA.VarType, akTypeB.VarType);
				}
				else
				{
					string text = flag ? "add" : "subtract";
					string text2 = flag ? "to" : "from";
					asError = string.Format("cannot {0} a {1} {2} a {3} (cast missing or types unrelated)", new object[]
					{
						text,
						akTypeA.VarType,
						text2,
						akTypeB.VarType
					});
				}
				this.OnError(asError, akOpToken.Line, akOpToken.CharPositionInLine);
			}
			if (!abCastToA)
			{
				return akTypeB;
			}
			return akTypeA;
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0004A9A0 File Offset: 0x00048BA0
		private void HandleAddSubtractExpression(string asExprAVar, ScriptVariableType akExprAType, IToken akExprAToken, string asExprBVar, ScriptVariableType akExprBType, IToken akExprBToken, IToken akMathToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out bool abIsConcat, out bool abIsInt, out string asResultVar, out ScriptVariableType akResultType, out IToken akResultToken, out CommonTree akATreeOut, out CommonTree akBTreeOut)
		{
			bool flag;
			akResultType = this.CheckAddSubtractType(akExprAType, akExprBType, akMathToken, out flag, out abIsConcat);
			abIsInt = (akResultType.VarType == "int");
			if (flag)
			{
				akATreeOut = new CommonTree(akExprAToken);
				string asVarName;
				akBTreeOut = this.AutoCast(akExprBToken, akExprBType, akExprAType, akCurrentScope, akTempVars, out asVarName);
				this.MarkTempVarAsUnused(akExprAType, asVarName, akMathToken);
			}
			else
			{
				string asVarName2;
				akATreeOut = this.AutoCast(akExprAToken, akExprAType, akExprBType, akCurrentScope, akTempVars, out asVarName2);
				akBTreeOut = new CommonTree(akExprBToken);
				this.MarkTempVarAsUnused(akExprBType, asVarName2, akMathToken);
			}
			this.MarkTempVarAsUnused(akExprAType, asExprAVar, akMathToken);
			this.MarkTempVarAsUnused(akExprBType, asExprBVar, akMathToken);
			asResultVar = this.GenerateTempVariable(akResultType, akCurrentScope, akTempVars);
			akResultToken = new CommonToken(akMathToken);
			akResultToken.Type = 38;
			akResultToken.Text = asResultVar;
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x0004AA70 File Offset: 0x00048C70
		private ScriptVariableType CheckMultDivideType(ScriptVariableType akTypeA, ScriptVariableType akTypeB, IToken akOpToken, out bool abCastToA)
		{
			string text = (akOpToken.Type == 75) ? "multiply" : "divide";
			string text2 = (akOpToken.Type == 75) ? "with" : "from";
			string asError = string.Format("cannot {0} a {1} {2} a {3} (cast missing or types unrelated)", new object[]
			{
				text,
				akTypeA.VarType,
				text2,
				akTypeB.VarType
			});
			abCastToA = true;
			if (!this.CanAutoCast(akTypeA, akTypeB))
			{
				if (!this.CanAutoCast(akTypeB, akTypeA))
				{
					this.OnError(asError, akOpToken.Line, akOpToken.CharPositionInLine);
				}
				else
				{
					abCastToA = false;
				}
			}
			else if (akTypeA.VarType != "int" && akTypeA.VarType != "float")
			{
				this.OnError(asError, akOpToken.Line, akOpToken.CharPositionInLine);
			}
			if (!abCastToA)
			{
				return akTypeB;
			}
			return akTypeA;
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x0004AB4C File Offset: 0x00048D4C
		private void HandleMultDivideExpression(string asExprAVar, ScriptVariableType akExprAType, IToken akExprAToken, string asExprBVar, ScriptVariableType akExprBType, IToken akExprBToken, IToken akMathToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out bool abIsInt, out string asResultVar, out ScriptVariableType akResultType, out IToken akResultToken, out CommonTree akATreeOut, out CommonTree akBTreeOut)
		{
			bool flag;
			akResultType = this.CheckMultDivideType(akExprAType, akExprBType, akMathToken, out flag);
			abIsInt = (akResultType.VarType == "int");
			if (flag)
			{
				akATreeOut = new CommonTree(akExprAToken);
				string asVarName;
				akBTreeOut = this.AutoCast(akExprBToken, akExprBType, akExprAType, akCurrentScope, akTempVars, out asVarName);
				this.MarkTempVarAsUnused(akExprAType, asVarName, akMathToken);
			}
			else
			{
				string asVarName2;
				akATreeOut = this.AutoCast(akExprAToken, akExprAType, akExprBType, akCurrentScope, akTempVars, out asVarName2);
				akBTreeOut = new CommonTree(akExprBToken);
				this.MarkTempVarAsUnused(akExprBType, asVarName2, akMathToken);
			}
			this.MarkTempVarAsUnused(akExprAType, asExprAVar, akMathToken);
			this.MarkTempVarAsUnused(akExprBType, asExprBVar, akMathToken);
			asResultVar = this.GenerateTempVariable(akResultType, akCurrentScope, akTempVars);
			akResultToken = new CommonToken(akMathToken);
			akResultToken.Type = 38;
			akResultToken.Text = asResultVar;
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x0004AC18 File Offset: 0x00048E18
		private ScriptVariableType CheckModType(ScriptVariableType akTypeA, ScriptVariableType akTypeB, IToken akOpToken)
		{
			if (akTypeA.VarType != "int" || akTypeB.VarType != "int")
			{
				this.OnError("Cannot calculate the modulus of non-integers", akOpToken.Line, akOpToken.CharPositionInLine);
			}
			return akTypeA;
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x0004AC58 File Offset: 0x00048E58
		private ScriptVariableType CheckNegationType(ScriptVariableType akType, IToken akOpToken)
		{
			if (akType.VarType != "int" && akType.VarType != "float")
			{
				this.OnError("Cannot negate a non-numeric type", akOpToken.Line, akOpToken.CharPositionInLine);
			}
			return akType;
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x0004AC98 File Offset: 0x00048E98
		private string GenerateTempVariable(ScriptVariableType akType, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
		{
			bool flag = false;
			string text;
			List<string> list;
			if (akType.VarType == "none")
			{
				text = "::nonevar";
				ScriptVariableType scriptVariableType;
				flag = !akCurrentScope.TryGetVariable(text, out scriptVariableType);
			}
			else if (this.kUnusedTempVarsByType.TryGetValue(akType.VarType, out list) && list.Count > 0)
			{
				text = list[0];
				list.RemoveAt(0);
				if (list.Count == 0)
				{
					this.kUnusedTempVarsByType.Remove(akType.VarType);
				}
			}
			else
			{
				text = string.Format("::temp{0}", this.iCurVarSuffix);
				this.iCurVarSuffix++;
				flag = true;
			}
			if (flag)
			{
				akTempVars.Add(text, akType);
				akCurrentScope.Root.TryDefineVariable(text, akType);
			}
			return text;
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x0004AD60 File Offset: 0x00048F60
		private void MarkTempVarAsUnused(ScriptVariableType akType, string asVarName, IToken akErrorToken)
		{
			if (asVarName.Length > 6 && asVarName.Substring(0, 6) == "::temp")
			{
				List<string> list;
				if (!this.kUnusedTempVarsByType.TryGetValue(akType.VarType, out list))
				{
					list = new List<string>();
					this.kUnusedTempVarsByType.Add(akType.VarType, list);
				}
				if (list.Contains(asVarName))
				{
					this.OnError(string.Format("Attempting to add temporary variable named {0} to free list multiple times", asVarName), akErrorToken.Line, akErrorToken.CharPositionInLine);
				}
				list.Add(asVarName);
			}
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x0004ADE4 File Offset: 0x00048FE4
		private int TypeToToken(ScriptVariableType akType)
		{
			int result = 38;
			string varType;
			if ((varType = akType.VarType) != null)
			{
				if (!(varType == "int"))
				{
					if (!(varType == "float"))
					{
						if (!(varType == "string"))
						{
							if (!(varType == "bool"))
							{
								if (varType == "none")
								{
									result = 92;
								}
							}
							else
							{
								result = 91;
							}
						}
						else
						{
							result = 90;
						}
					}
					else
					{
						result = 93;
					}
				}
				else
				{
					result = 81;
				}
			}
			return result;
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x0004AE5C File Offset: 0x0004905C
		private void CheckCanBeLValue(string asName, IToken akNameToken)
		{
			string a = asName.ToLowerInvariant();
			if (a == "self" || a == "parent")
			{
				this.OnError(string.Format("variable {0} is read-only, you cannot assign a value to it", asName), akNameToken.Line, akNameToken.CharPositionInLine);
			}
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x0004AEA8 File Offset: 0x000490A8
		private bool SortAndCheckFunctionParameters(ScriptFunctionType akFunction, List<string> akTargetParamNames, List<ScriptVariableType> akParamTypes, List<IToken> akParamTokens, ref List<CommonTree> akParamExpressions, out List<CommonTree> akAutoCastTrees, IToken akNameToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
		{
			bool flag = true;
			List<CommonTree> list = new List<CommonTree>(akFunction.ParamNames.Count);
			akAutoCastTrees = new List<CommonTree>(akFunction.ParamNames.Count);
			for (int i = 0; i < akFunction.ParamNames.Count; i++)
			{
				list.Add(null);
				akAutoCastTrees.Add(null);
			}
			if (akParamTypes != null)
			{
				Dictionary<string, ScriptVariableType> dictionary = new Dictionary<string, ScriptVariableType>();
				bool flag2 = false;
				bool flag3 = false;
				int num = 0;
				while (!flag3 && num < akParamTypes.Count)
				{
					bool flag4 = false;
					if (num >= akFunction.ParamTypes.Count)
					{
						flag = false;
						flag3 = true;
						this.OnError("too many arguments passed to function", akNameToken.Line, akNameToken.CharPositionInLine);
					}
					else if (flag2 && akTargetParamNames[num] == "")
					{
						flag = false;
						flag4 = true;
						this.OnError(string.Format("argument {0} must be explicitly assigned to a parameter", num + 1), akNameToken.Line, akNameToken.CharPositionInLine);
					}
					else if (!flag2 && akTargetParamNames[num] != "")
					{
						flag2 = true;
					}
					ScriptVariableType scriptVariableType = null;
					int num2 = num;
					if (!flag3 && !flag4)
					{
						if (flag2)
						{
							string text = akTargetParamNames[num].ToLowerInvariant();
							num2 = akFunction.ParamNames.IndexOf(text);
							if (num2 == -1)
							{
								flag = false;
								flag4 = true;
								this.OnError(string.Format("cannot find a parameter named {0}", text), akNameToken.Line, akNameToken.CharPositionInLine);
							}
							else if (list[num2] != null)
							{
								flag = false;
								flag4 = true;
								this.OnError(string.Format("parameter {0} was assigned to more then once", text), akNameToken.Line, akNameToken.CharPositionInLine);
							}
							else
							{
								scriptVariableType = akFunction.ParamTypes[num2];
								list[num2] = akParamExpressions[num];
							}
						}
						else
						{
							scriptVariableType = akFunction.ParamTypes[num];
							string text = akFunction.ParamNames[num];
							list[num2] = akParamExpressions[num];
						}
					}
					if (!flag3 && !flag4)
					{
						if (!this.CanAutoCast(scriptVariableType, akParamTypes[num]))
						{
							if (!scriptVariableType.IsObjectType || (scriptVariableType.IsObjectType && akParamTypes[num].VarType != "none"))
							{
								flag = false;
								this.OnError(string.Format("type mismatch on parameter {0} (did you forget a cast?)", num + 1), akNameToken.Line, akNameToken.CharPositionInLine);
							}
						}
						else
						{
							string text2;
							akAutoCastTrees[num2] = this.AutoCast(akParamTokens[num], akParamTypes[num], scriptVariableType, akCurrentScope, akTempVars, out text2);
							if (text2 != "")
							{
								dictionary.Add(text2, scriptVariableType);
							}
						}
					}
					num++;
				}
				foreach (KeyValuePair<string, ScriptVariableType> keyValuePair in dictionary)
				{
					this.MarkTempVarAsUnused(keyValuePair.Value, keyValuePair.Key, akNameToken);
				}
			}
			if (flag)
			{
				for (int j = 0; j < akFunction.ParamTypes.Count; j++)
				{
					if (list[j] == null)
					{
						ScriptVariableType scriptVariableType2 = akFunction.ParamTypes[j];
						if (scriptVariableType2.HasInitialValue)
						{
							int num3 = this.TypeToToken(scriptVariableType2);
							if (num3 == 38)
							{
								num3 = 92;
							}
							list[j] = new CommonTree(new CommonToken(num3, scriptVariableType2.InitialValue));
						}
						else
						{
							this.OnError(string.Format("argument {0} is not specified and has no default value", akFunction.ParamNames[j]), akNameToken.Line, akNameToken.CharPositionInLine);
							flag = false;
							list[j] = new CommonTree(new CommonToken(92, "none"));
						}
						akAutoCastTrees[j] = list[j];
					}
				}
			}
			akParamExpressions = list;
			return flag;
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x0004B2A0 File Offset: 0x000494A0
		private ScriptVariableType CheckArrayFunctionCall(ScriptVariableType akSelfType, string asName, List<string> akTargetParamNames, List<ScriptVariableType> akParamTypes, List<IToken> akParamTokens, ref List<CommonTree> akParamExpressions, out List<CommonTree> akAutoCastTrees, IToken akNameToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
		{
			akAutoCastTrees = new List<CommonTree>();
			ScriptVariableType result = new ScriptVariableType("none");
			ScriptVariableType akType = new ScriptVariableType(akSelfType.ArrayElementType);
			ScriptFunctionType scriptFunctionType = null;
			if (asName.ToLowerInvariant() == "find")
			{
				scriptFunctionType = new ScriptFunctionType("find", akSelfType.VarType, "", "");
				scriptFunctionType.kRetType = new ScriptVariableType("int");
				scriptFunctionType.bNative = true;
				scriptFunctionType.bGlobal = false;
				scriptFunctionType.TryAddParam("akElement", akType);
				scriptFunctionType.TryAddParam("aiStartIndex", new ScriptVariableType("int")
				{
					InitialValue = "0"
				});
			}
			else if (asName.ToLowerInvariant() == "rfind")
			{
				scriptFunctionType = new ScriptFunctionType("rfind", akSelfType.VarType, "", "");
				scriptFunctionType.kRetType = new ScriptVariableType("int");
				scriptFunctionType.bNative = true;
				scriptFunctionType.bGlobal = false;
				scriptFunctionType.TryAddParam("akElement", akType);
				scriptFunctionType.TryAddParam("aiStartIndex", new ScriptVariableType("int")
				{
					InitialValue = "-1"
				});
			}
			else
			{
				this.OnError(string.Format("{0} is not a function or does not exist", asName), akNameToken.Line, akNameToken.CharPositionInLine);
			}
			if (scriptFunctionType != null && this.SortAndCheckFunctionParameters(scriptFunctionType, akTargetParamNames, akParamTypes, akParamTokens, ref akParamExpressions, out akAutoCastTrees, akNameToken, akCurrentScope, akTempVars))
			{
				result = scriptFunctionType.kRetType;
			}
			return result;
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x0004B410 File Offset: 0x00049610
		private ScriptVariableType CheckGlobalFunctionCall(ScriptVariableType akSelfType, string asName, List<string> akTargetParamNames, List<ScriptVariableType> akParamTypes, List<IToken> akParamTokens, ref List<CommonTree> akParamExpressions, out List<CommonTree> akAutoCastTrees, IToken akNameToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
		{
			akAutoCastTrees = new List<CommonTree>();
			ScriptVariableType result = new ScriptVariableType("none");
			ScriptObjectType knownType = this.GetKnownType(akSelfType.VarType);
			if (knownType != null)
			{
				ScriptFunctionType scriptFunctionType;
				if (knownType.TryGetFunction(asName, out scriptFunctionType))
				{
					if (scriptFunctionType.bGlobal)
					{
						if (this.SortAndCheckFunctionParameters(scriptFunctionType, akTargetParamNames, akParamTypes, akParamTokens, ref akParamExpressions, out akAutoCastTrees, akNameToken, akCurrentScope, akTempVars))
						{
							result = scriptFunctionType.kRetType;
						}
					}
					else
					{
						this.OnError(string.Format("{0} is not a global function", asName), akNameToken.Line, akNameToken.CharPositionInLine);
					}
				}
				else
				{
					this.OnError(string.Format("{0} is not a function or does not exist", asName), akNameToken.Line, akNameToken.CharPositionInLine);
				}
			}
			else
			{
				this.OnError(string.Format("{0} is not a known user-defined type", akSelfType.VarType), akNameToken.Line, akNameToken.CharPositionInLine);
			}
			return result;
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x0004B4DC File Offset: 0x000496DC
		private ScriptVariableType CheckMemberFunctionCall(ScriptVariableType akSelfType, string asName, List<string> akTargetParamNames, List<ScriptVariableType> akParamTypes, List<IToken> akParamTokens, ref List<CommonTree> akParamExpressions, out List<CommonTree> akAutoCastTrees, IToken akNameToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
		{
			akAutoCastTrees = new List<CommonTree>();
			ScriptVariableType result = new ScriptVariableType("none");
			ScriptObjectType knownType = this.GetKnownType(akSelfType.VarType);
			if (knownType != null)
			{
				ScriptObjectType scriptObjectType = knownType;
				ScriptFunctionType scriptFunctionType = null;
				while (scriptObjectType != null && scriptFunctionType == null)
				{
					if (!scriptObjectType.TryGetFunction(asName, out scriptFunctionType))
					{
						scriptObjectType = scriptObjectType.kParent;
					}
				}
				if (scriptFunctionType != null)
				{
					if (!scriptFunctionType.bGlobal)
					{
						if (this.SortAndCheckFunctionParameters(scriptFunctionType, akTargetParamNames, akParamTypes, akParamTokens, ref akParamExpressions, out akAutoCastTrees, akNameToken, akCurrentScope, akTempVars))
						{
							result = scriptFunctionType.kRetType;
						}
					}
					else
					{
						this.OnError(string.Format("global function {0} cannot be called on an object", asName), akNameToken.Line, akNameToken.CharPositionInLine);
					}
				}
				else
				{
					this.OnError(string.Format("{0} is not a function or does not exist", asName), akNameToken.Line, akNameToken.CharPositionInLine);
				}
			}
			else
			{
				this.OnError(string.Format("{0} is not a known user-defined type", akSelfType.VarType), akNameToken.Line, akNameToken.CharPositionInLine);
			}
			return result;
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x0004B5C0 File Offset: 0x000497C0
		private void CheckReturnType(ScriptFunctionType akFunctionType, ScriptVariableType akType, IToken akReturnToken)
		{
			if (akType != null)
			{
				if (!this.CanAutoCast(akFunctionType.kRetType, akType) && (!akFunctionType.kRetType.IsObjectType || (akFunctionType.kRetType.IsObjectType && akType.VarType != "none")))
				{
					string arg = (akFunctionType.kRetType.VarType == "none") ? "the function does not return a value" : "the types do not match (cast missing or types unrelated)";
					this.OnError(string.Format("cannot return a {0} from {1}, {2}", akType.VarType, akFunctionType.Name, arg), akReturnToken.Line, akReturnToken.CharPositionInLine);
					return;
				}
			}
			else if (akFunctionType.kRetType.VarType != "none")
			{
				this.OnError(string.Format("you must return a {0} value from {1}", akFunctionType.kRetType.VarType, akFunctionType.Name), akReturnToken.Line, akReturnToken.CharPositionInLine);
			}
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x0004B6AC File Offset: 0x000498AC
		private void CheckFunction(ScriptFunctionType akFunctionType, IToken akScriptToken)
		{
			ScriptFunctionType scriptFunctionType = null;
			ScriptObjectType kParent = this.kObjType.kParent;
			bool flag = false;
			if (akFunctionType.StateName != "" && this.kObjType.TryGetFunction(akFunctionType.Name, out scriptFunctionType))
			{
				kParent = this.kObjType;
				flag = true;
			}
			while (scriptFunctionType == null && kParent != null)
			{
				if (!kParent.TryGetFunction(akFunctionType.Name, out scriptFunctionType))
				{
					kParent = kParent.kParent;
				}
			}
			if (scriptFunctionType == null)
			{
				string text = akFunctionType.Name.ToLowerInvariant();
				if (text == "onbeginstate" || text == "onendstate")
				{
					scriptFunctionType = new ScriptFunctionType(text, this.kObjType.Name, "", "");
				}
			}
			if (scriptFunctionType == null)
			{
				if (akFunctionType.StateName != "")
				{
					this.OnError(string.Format("function {0} cannot be defined in state {1} without also being defined in the empty state", akFunctionType.Name, akFunctionType.StateName), akScriptToken.Line, akScriptToken.CharPositionInLine);
					return;
				}
			}
			else
			{
				if (akFunctionType.bGlobal)
				{
					this.OnError(string.Format("global function {0} already defined in parent script {1}", akFunctionType.Name, kParent.Name), akScriptToken.Line, akScriptToken.CharPositionInLine);
					return;
				}
				if (this.kObjType.kNonOverridableFunctions.Contains(akFunctionType.Name.ToLowerInvariant()))
				{
					this.OnError(string.Format("cannot override function {0}", akFunctionType.Name), akScriptToken.Line, akScriptToken.CharPositionInLine);
					return;
				}
				string text2 = "";
				if (!akFunctionType.ParamTypesMatch(scriptFunctionType))
				{
					text2 = "parameter types";
				}
				else if (!akFunctionType.ParamDefaultsMatch(scriptFunctionType))
				{
					text2 = "parameter defaults";
				}
				else if (akFunctionType.kRetType.VarType != scriptFunctionType.kRetType.VarType)
				{
					text2 = "return type";
				}
				if (text2 != "")
				{
					string text3;
					if (akFunctionType.StateName == "")
					{
						text3 = "the empty state";
					}
					else
					{
						text3 = string.Format("state {0}", akFunctionType.StateName);
					}
					if (flag)
					{
						this.OnError(string.Format("the {0} of function {1} in {2} on script {3} does not match the empty state", new object[]
						{
							text2,
							akFunctionType.Name,
							text3,
							this.kObjType.Name
						}), akScriptToken.Line, akScriptToken.CharPositionInLine);
						return;
					}
					this.OnError(string.Format("the {0} of function {1} in {2} on script {3} do not match the parent script {4}", new object[]
					{
						text2,
						akFunctionType.Name,
						text3,
						this.kObjType.Name,
						kParent.Name
					}), akScriptToken.Line, akScriptToken.CharPositionInLine);
				}
			}
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x0004B948 File Offset: 0x00049B48
		private ScriptVariableType CheckCast(ScriptVariableType akSourceType, ScriptVariableType akTargetType, IToken akCastToken)
		{
			bool flag = true;
			if (akTargetType.VarType != akSourceType.VarType)
			{
				string varType;
				if ((varType = akSourceType.VarType) != null)
				{
					if (varType == "none")
					{
						flag = (akTargetType.VarType == "string" || akTargetType.VarType == "bool");
						goto IL_231;
					}
					if (varType == "string")
					{
						flag = (!akTargetType.IsObjectType && !akTargetType.IsArray);
						goto IL_231;
					}
					if (varType == "int")
					{
						flag = (!akTargetType.IsObjectType && !akTargetType.IsArray);
						goto IL_231;
					}
					if (varType == "float")
					{
						flag = (!akTargetType.IsObjectType && !akTargetType.IsArray);
						goto IL_231;
					}
					if (varType == "bool")
					{
						flag = (!akTargetType.IsObjectType && !akTargetType.IsArray);
						goto IL_231;
					}
				}
				if (akSourceType.IsArray)
				{
					flag = (akTargetType.VarType == "none" || akTargetType.VarType == "string" || akTargetType.VarType == "bool");
				}
				else if (akTargetType.VarType != akSourceType.VarType && akTargetType.VarType != "none" && akTargetType.VarType != "string" && akTargetType.VarType != "bool")
				{
					if (!akTargetType.IsObjectType)
					{
						flag = false;
					}
					else
					{
						ScriptObjectType knownType = this.GetKnownType(akTargetType.VarType);
						ScriptObjectType knownType2 = this.GetKnownType(akSourceType.VarType);
						if (knownType == null)
						{
							flag = false;
							this.OnError(string.Format("cannot convert to unknown type {0}", akTargetType.VarType), akCastToken.Line, akCastToken.CharPositionInLine);
						}
						else if (knownType2 == null)
						{
							flag = false;
							this.OnError(string.Format("cannot convert from unknown type {0}", akSourceType.VarType), akCastToken.Line, akCastToken.CharPositionInLine);
						}
						else
						{
							flag = (knownType2.IsChildOf(knownType.Name) || knownType.IsChildOf(knownType2.Name));
						}
					}
				}
			}
			IL_231:
			if (!flag)
			{
				this.OnError(string.Format("cannot cast a {0} to a {1}, types are incompatible", akSourceType.VarType, akTargetType.VarType), akCastToken.Line, akCastToken.CharPositionInLine);
			}
			return akTargetType;
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x0004BBB4 File Offset: 0x00049DB4
		private void CheckPropertyOverride(string asPropName, IToken akSourceToken)
		{
			ScriptObjectType kParent = this.kObjType.kParent;
			ScriptPropertyType scriptPropertyType = null;
			while (kParent != null && scriptPropertyType == null)
			{
				if (!kParent.TryGetProperty(asPropName, out scriptPropertyType))
				{
					kParent = kParent.kParent;
				}
			}
			if (scriptPropertyType != null)
			{
				this.OnError(string.Format("script property {0} already defined on parent {1}", asPropName, kParent.Name), akSourceToken.Line, akSourceToken.CharPositionInLine);
			}
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x0004BC10 File Offset: 0x00049E10
		private void CheckPropertyFunction(string asPropName, string asFuncName, out bool abIsGet, IToken akSourceToken)
		{
			abIsGet = false;
			ScriptVariableType scriptVariableType;
			if (this.kObjType.TryGetVariable(asPropName.ToLowerInvariant(), out scriptVariableType))
			{
				this.OnError(string.Format("property {0} cannot have the same name as a variable", asPropName), akSourceToken.Line, akSourceToken.CharPositionInLine);
				return;
			}
			ScriptPropertyType scriptPropertyType;
			if (this.kObjType.TryGetProperty(asPropName.ToLowerInvariant(), out scriptPropertyType))
			{
				ScriptFunctionType scriptFunctionType = null;
				string a = asFuncName.ToLowerInvariant();
				string arg = "";
				ScriptFunctionType scriptFunctionType2 = null;
				if (a == "get")
				{
					scriptFunctionType = scriptPropertyType.kGetFunction;
					scriptFunctionType2 = new ScriptFunctionType("dummy", this.kObjType.Name, "", asPropName);
					scriptFunctionType2.kRetType = scriptPropertyType.kType;
					arg = "getter";
					abIsGet = true;
				}
				else if (a == "set")
				{
					scriptFunctionType = scriptPropertyType.kSetFunction;
					scriptFunctionType2 = new ScriptFunctionType("dummy", this.kObjType.Name, "", asPropName);
					scriptFunctionType2.TryAddParam("value", scriptPropertyType.kType);
					arg = "setter";
				}
				else
				{
					this.OnError(string.Format("Function {0} on property {1} must be either a get or set", asFuncName, asPropName), akSourceToken.Line, akSourceToken.CharPositionInLine);
				}
				if (scriptFunctionType2 != null && scriptFunctionType != null)
				{
					string text = "";
					if (!scriptFunctionType.ParamTypesMatch(scriptFunctionType2))
					{
						text = "parameters";
					}
					else if (scriptFunctionType.kRetType.VarType != scriptFunctionType2.kRetType.VarType)
					{
						text = "return type";
					}
					if (text != "")
					{
						this.OnError(string.Format("{0} for property {1} does not have the correct {2}", arg, asPropName, text), akSourceToken.Line, akSourceToken.CharPositionInLine);
						return;
					}
				}
			}
			else
			{
				this.OnError(string.Format("internal error: cannot find property {0}", asPropName), akSourceToken.Line, akSourceToken.CharPositionInLine);
			}
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x0004BDD4 File Offset: 0x00049FD4
		private CommonTree CreateBlockTree(IToken akBlockToken, IList akStatements, Dictionary<string, ScriptVariableType> akTempVars)
		{
			CommonTree commonTree = new CommonTree(akBlockToken);
			foreach (string text in akTempVars.Keys)
			{
				ScriptVariableType scriptVariableType = akTempVars[text];
				CommonToken commonToken = new CommonToken(akBlockToken);
				if (scriptVariableType.IsObjectType)
				{
					commonToken.Type = 38;
				}
				else
				{
					commonToken.Type = 55;
				}
				commonToken.Text = scriptVariableType.VarType;
				CommonToken commonToken2 = new CommonToken(akBlockToken);
				commonToken2.Type = 38;
				commonToken2.Text = text;
				CommonTree commonTree2 = new CommonTree(new CommonToken(akBlockToken)
				{
					Type = 5,
					Text = "VAR"
				});
				commonTree2.AddChild(new CommonTree(commonToken));
				commonTree2.AddChild(new CommonTree(commonToken2));
				commonTree.AddChild(commonTree2);
			}
			if (akStatements != null)
			{
				foreach (object obj in akStatements)
				{
					ITree t = (ITree)obj;
					commonTree.AddChild(t);
				}
			}
			return commonTree;
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x0004BF14 File Offset: 0x0004A114
		private CommonTree CreateCallTree(IToken akCallToken, bool abIsGlobal, bool abIsArray, string asSelfVar, IToken akNameToken, ScriptVariableType akRetVarType, string asRetVarName, List<CommonTree> akParamAutocasts, List<CommonTree> akParamExpressions)
		{
			CommonTree commonTree = new CommonTree(new CommonToken(14, "CALLPARAMS"));
			if (akParamExpressions.Count == akParamAutocasts.Count)
			{
				for (int i = 0; i < akParamExpressions.Count; i++)
				{
					CommonTree commonTree2 = new CommonTree(new CommonToken(9, "PARAM"));
					commonTree2.AddChild(akParamAutocasts[i]);
					commonTree2.AddChild(akParamExpressions[i]);
					commonTree.AddChild(commonTree2);
				}
			}
			bool flag = asSelfVar.ToLowerInvariant() == "parent";
			IToken token = new CommonToken(akNameToken);
			token.Type = 38;
			if (flag && asSelfVar == "")
			{
				token.Text = "self";
			}
			else
			{
				token.Text = asSelfVar;
			}
			IToken token2 = new CommonToken(akNameToken);
			token2.Type = 38;
			token2.Text = asRetVarName;
			CommonTree commonTree3 = new CommonTree(akCallToken);
			if (abIsArray)
			{
				if (akNameToken.Text.ToLowerInvariant() == "find")
				{
					akCallToken.Type = 24;
					akCallToken.Text = "arrayfind";
				}
				else if (akNameToken.Text.ToLowerInvariant() == "rfind")
				{
					akCallToken.Type = 25;
					akCallToken.Text = "arrayrfind";
				}
				commonTree3.AddChild(new CommonTree(token));
				commonTree3.AddChild(new CommonTree(token2));
				commonTree3.AddChild(commonTree);
			}
			else
			{
				if (abIsGlobal)
				{
					akCallToken.Type = 12;
					akCallToken.Text = "callglobal";
				}
				else if (flag)
				{
					akCallToken.Type = 13;
					akCallToken.Text = "callparent";
				}
				commonTree3.AddChild(new CommonTree(token));
				commonTree3.AddChild(new CommonTree(akNameToken));
				commonTree3.AddChild(new CommonTree(token2));
				commonTree3.AddChild(commonTree);
			}
			return commonTree3;
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x0004C0DC File Offset: 0x0004A2DC
		private void CheckArrayNew(IToken akTypeToken, IToken akSizeToken)
		{
			if (!this.IsKnownType(new ScriptVariableType(akTypeToken.Text)))
			{
				this.OnError(string.Format("unknown type {0}", akTypeToken.Text), akTypeToken.Line, akTypeToken.CharPositionInLine);
			}
			int num;
			if (!int.TryParse(akSizeToken.Text, out num) || num <= 0)
			{
				this.OnError(string.Format("Array size of {0} is invalid. Must be greater than 0", akSizeToken.Text), akSizeToken.Line, akSizeToken.CharPositionInLine);
			}
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x0004C154 File Offset: 0x0004A354
		private void HandleArrayElementExpression(string asArrayVar, ScriptVariableType akArrayType, IToken akArrayToken, string asExprVar, ScriptVariableType akExprType, IToken akExprToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out string asResultVar, out ScriptVariableType akResultType, out IToken akResultToken, out CommonTree akTreeOut)
		{
			ScriptVariableType scriptVariableType = new ScriptVariableType("int");
			if (this.CanAutoCast(scriptVariableType, akExprType))
			{
				string asVarName;
				akTreeOut = this.AutoCast(akExprToken, akExprType, scriptVariableType, akCurrentScope, akTempVars, out asVarName);
				this.MarkTempVarAsUnused(scriptVariableType, asVarName, akArrayToken);
			}
			else
			{
				akTreeOut = new CommonTree(akExprToken);
				this.OnError("arrays can only be indexed with integers", akArrayToken.Line, akArrayToken.CharPositionInLine);
			}
			if (akArrayType.IsArray)
			{
				akResultType = new ScriptVariableType(akArrayType.ArrayElementType);
			}
			else
			{
				akResultType = new ScriptVariableType("none");
				this.OnError("only arrays can be indexed", akArrayToken.Line, akArrayToken.CharPositionInLine);
			}
			asResultVar = this.GenerateTempVariable(akResultType, akCurrentScope, akTempVars);
			this.MarkTempVarAsUnused(akArrayType, asArrayVar, akArrayToken);
			this.MarkTempVarAsUnused(akExprType, asExprVar, akArrayToken);
			akResultToken = new CommonToken(akArrayToken);
			akResultToken.Type = 38;
			akResultToken.Text = asResultVar;
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x0004C238 File Offset: 0x0004A438
		public PapyrusTypeWalker(ITreeNodeStream input) : this(input, new RecognizerSharedState())
		{
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x0004C248 File Offset: 0x0004A448
		public PapyrusTypeWalker(ITreeNodeStream input, RecognizerSharedState state) : base(input, state)
		{
			this.InitializeCyclicDFAs();
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000C4E RID: 3150 RVA: 0x0004C36C File Offset: 0x0004A56C
		// (set) Token: 0x06000C4F RID: 3151 RVA: 0x0004C374 File Offset: 0x0004A574
		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return this.adaptor;
			}
			set
			{
				this.adaptor = value;
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000C50 RID: 3152 RVA: 0x0004C380 File Offset: 0x0004A580
		public override string[] TokenNames
		{
			get
			{
				return PapyrusTypeWalker.tokenNames;
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000C51 RID: 3153 RVA: 0x0004C388 File Offset: 0x0004A588
		public override string GrammarFileName
		{
			get
			{
				return "PapyrusTypeWalker.g";
			}
		}

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x06000C52 RID: 3154 RVA: 0x0004C390 File Offset: 0x0004A590
		// (remove) Token: 0x06000C53 RID: 3155 RVA: 0x0004C3AC File Offset: 0x0004A5AC
		internal event InternalErrorEventHandler ErrorHandler;

		// Token: 0x06000C54 RID: 3156 RVA: 0x0004C3C8 File Offset: 0x0004A5C8
		private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
		{
			if (this.ErrorHandler != null)
			{
				this.ErrorHandler(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
			}
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x0004C3E8 File Offset: 0x0004A5E8
		public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			string errorMessage = this.GetErrorMessage(e, tokenNames);
			this.OnError(errorMessage, e.Line, e.CharPositionInLine);
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x0004C414 File Offset: 0x0004A614
		public PapyrusTypeWalker.script_return script(ScriptObjectType akObj, Compiler akCompiler, Dictionary<string, ScriptObjectType> akKnownTypes, Stack<string> akChildNames)
		{
			PapyrusTypeWalker.script_return script_return = new PapyrusTypeWalker.script_return();
			script_return.Start = this.input.LT(1);
			this.kKnownTypes = akKnownTypes;
			this.AddKnownType("int");
			this.AddKnownType("float");
			this.AddKnownType("string");
			this.AddKnownType("bool");
			this.AddKnownType(akObj);
			this.AddImportedType(akObj);
			this.kObjType = akObj;
			this.kCompiler = akCompiler;
			this.kChildren = akChildNames;
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 4, PapyrusTypeWalker.FOLLOW_OBJECT_in_script81);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_header_in_script83);
				PapyrusTypeWalker.header_return header_return = this.header();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, header_return.Tree);
				for (;;)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if ((num2 >= 5 && num2 <= 7) || num2 == 19 || num2 == 42 || num2 == 51 || num2 == 54)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_definitionOrBlock_in_script85);
					PapyrusTypeWalker.definitionOrBlock_return definitionOrBlock_return = this.definitionOrBlock();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, definitionOrBlock_return.Tree);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				script_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.kObjType.kAST = (CommonTree)script_return.Tree;
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return script_return;
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x0004C69C File Offset: 0x0004A89C
		public PapyrusTypeWalker.header_return header()
		{
			PapyrusTypeWalker.header_return header_return = new PapyrusTypeWalker.header_return();
			header_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			try
			{
				CommonTree commonTree2 = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree3 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
				commonTree3 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_header101);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree4 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree4);
				this.Match(this.input, 2, null);
				commonTree3 = (CommonTree)this.input.LT(1);
				CommonTree treeNode2 = (CommonTree)this.Match(this.input, 18, PapyrusTypeWalker.FOLLOW_USER_FLAGS_in_header103);
				CommonTree child = (CommonTree)this.adaptor.DupNode(treeNode2);
				this.adaptor.AddChild(commonTree4, child);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 38)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree3 = (CommonTree)this.input.LT(1);
					commonTree = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_header107);
					CommonTree child2 = (CommonTree)this.adaptor.DupNode(commonTree);
					this.adaptor.AddChild(commonTree4, child2);
				}
				int num4 = 2;
				int num5 = this.input.LA(1);
				if (num5 == 40)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					commonTree3 = (CommonTree)this.input.LT(1);
					CommonTree treeNode3 = (CommonTree)this.Match(this.input, 40, PapyrusTypeWalker.FOLLOW_DOCSTRING_in_header110);
					CommonTree child3 = (CommonTree)this.adaptor.DupNode(treeNode3);
					this.adaptor.AddChild(commonTree4, child3);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree2, commonTree4);
				if (commonTree != null)
				{
					this.HandleParent(commonTree.Token);
				}
				header_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree2);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return header_return;
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x0004C92C File Offset: 0x0004AB2C
		public PapyrusTypeWalker.definitionOrBlock_return definitionOrBlock()
		{
			PapyrusTypeWalker.definitionOrBlock_return definitionOrBlock_return = new PapyrusTypeWalker.definitionOrBlock_return();
			definitionOrBlock_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule import_obj");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num <= 19)
				{
					switch (num)
					{
					case 5:
						num2 = 1;
						goto IL_B9;
					case 6:
						num2 = 3;
						goto IL_B9;
					case 7:
						num2 = 4;
						goto IL_B9;
					default:
						if (num != 19)
						{
							goto IL_A2;
						}
						break;
					}
				}
				else
				{
					if (num == 42)
					{
						num2 = 2;
						goto IL_B9;
					}
					if (num == 51)
					{
						num2 = 5;
						goto IL_B9;
					}
					if (num != 54)
					{
						goto IL_A2;
					}
				}
				num2 = 6;
				goto IL_B9;
				IL_A2:
				NoViableAltException ex = new NoViableAltException("", 4, 0, this.input);
				throw ex;
				IL_B9:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_fieldDefinition_in_definitionOrBlock130);
					PapyrusTypeWalker.fieldDefinition_return fieldDefinition_return = this.fieldDefinition();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, fieldDefinition_return.Tree);
					break;
				}
				case 2:
				{
					CommonTree commonTree3 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_import_obj_in_definitionOrBlock136);
					PapyrusTypeWalker.import_obj_return import_obj_return = this.import_obj();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(import_obj_return.Tree);
					definitionOrBlock_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (definitionOrBlock_return != null) ? definitionOrBlock_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					commonTree = null;
					definitionOrBlock_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_function_in_definitionOrBlock148);
					PapyrusTypeWalker.function_return function_return = this.function("", "");
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, function_return.Tree);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree5 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_eventFunc_in_definitionOrBlock156);
					PapyrusTypeWalker.eventFunc_return eventFunc_return = this.eventFunc("");
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, eventFunc_return.Tree);
					break;
				}
				case 5:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree6 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_stateBlock_in_definitionOrBlock164);
					PapyrusTypeWalker.stateBlock_return stateBlock_return = this.stateBlock();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, stateBlock_return.Tree);
					break;
				}
				case 6:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree7 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_propertyBlock_in_definitionOrBlock170);
					PapyrusTypeWalker.propertyBlock_return propertyBlock_return = this.propertyBlock();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, propertyBlock_return.Tree);
					break;
				}
				}
				definitionOrBlock_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return definitionOrBlock_return;
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x0004CCE0 File Offset: 0x0004AEE0
		public PapyrusTypeWalker.fieldDefinition_return fieldDefinition()
		{
			PapyrusTypeWalker.fieldDefinition_return fieldDefinition_return = new PapyrusTypeWalker.fieldDefinition_return();
			fieldDefinition_return.Start = this.input.LT(1);
			PapyrusTypeWalker.constant_return constant_return = null;
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 5, PapyrusTypeWalker.FOLLOW_VAR_in_fieldDefinition184);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_type_in_fieldDefinition186);
				PapyrusTypeWalker.type_return type_return = this.type();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, type_return.Tree);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_fieldDefinition190);
				CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree4);
				this.adaptor.AddChild(commonTree3, child);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode2 = (CommonTree)this.Match(this.input, 18, PapyrusTypeWalker.FOLLOW_USER_FLAGS_in_fieldDefinition192);
				CommonTree child2 = (CommonTree)this.adaptor.DupNode(treeNode2);
				this.adaptor.AddChild(commonTree3, child2);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 81 || (num2 >= 90 && num2 <= 93))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_constant_in_fieldDefinition194);
					constant_return = this.constant();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, constant_return.Tree);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				if (((constant_return != null) ? ((CommonTree)constant_return.Start) : null) != null)
				{
					this.CheckVariableDefinition(commonTree4.Text, (type_return != null) ? type_return.kType : null, ((constant_return != null) ? ((CommonTree)constant_return.Start) : null).Token, false, commonTree4.Token);
				}
				else
				{
					this.CheckVariableDefinition(commonTree4.Text, (type_return != null) ? type_return.kType : null, null, false, commonTree4.Token);
				}
				fieldDefinition_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return fieldDefinition_return;
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x0004D004 File Offset: 0x0004B204
		public PapyrusTypeWalker.import_obj_return import_obj()
		{
			PapyrusTypeWalker.import_obj_return import_obj_return = new PapyrusTypeWalker.import_obj_return();
			import_obj_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 42, PapyrusTypeWalker.FOLLOW_IMPORT_in_import_obj215);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_import_obj217);
				CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree4);
				this.adaptor.AddChild(commonTree3, child);
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				ScriptObjectType scriptObjectType = this.kCompiler.LoadObject(commonTree4.Text, this.kKnownTypes);
				if (scriptObjectType != null)
				{
					this.AddImportedType(scriptObjectType);
				}
				import_obj_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return import_obj_return;
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x0004D1B8 File Offset: 0x0004B3B8
		public PapyrusTypeWalker.function_return function(string asStateName, string asPropertyName)
		{
			this.function_stack.Push(new PapyrusTypeWalker.function_scope());
			PapyrusTypeWalker.function_return function_return = new PapyrusTypeWalker.function_return();
			function_return.Start = this.input.LT(1);
			((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).sstateName = asStateName;
			((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).spropertyName = asPropertyName;
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 6, PapyrusTypeWalker.FOLLOW_FUNCTION_in_function256);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_functionHeader_in_function258);
				PapyrusTypeWalker.functionHeader_return functionHeader_return = this.functionHeader();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, functionHeader_return.Tree);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 10)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_codeBlock_in_function260);
					PapyrusTypeWalker.codeBlock_return codeBlock_return = this.codeBlock(((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).kfunctionType, ((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).kfunctionType.FunctionScope);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				function_return.sName = ((functionHeader_return != null) ? functionHeader_return.sFuncName : null);
				function_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				if (((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).spropertyName == "")
				{
					this.CheckFunction(((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).kfunctionType, ((CommonTree)function_return.Start).Token);
				}
				this.kUnusedTempVarsByType.Clear();
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.function_stack.Pop();
			}
			return function_return;
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x0004D4B0 File Offset: 0x0004B6B0
		public PapyrusTypeWalker.functionHeader_return functionHeader()
		{
			PapyrusTypeWalker.functionHeader_return functionHeader_return = new PapyrusTypeWalker.functionHeader_return();
			functionHeader_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			try
			{
				int num = this.input.LA(1);
				if (num != 8)
				{
					NoViableAltException ex = new NoViableAltException("", 13, 0, this.input);
					throw ex;
				}
				int num2 = this.input.LA(2);
				if (num2 != 2)
				{
					NoViableAltException ex2 = new NoViableAltException("", 13, 1, this.input);
					throw ex2;
				}
				int num3 = this.input.LA(3);
				int num4;
				if (num3 == 92)
				{
					num4 = 2;
				}
				else
				{
					if (num3 != 38 && num3 != 55)
					{
						NoViableAltException ex3 = new NoViableAltException("", 13, 2, this.input);
						throw ex3;
					}
					num4 = 1;
				}
				switch (num4)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode = (CommonTree)this.Match(this.input, 8, PapyrusTypeWalker.FOLLOW_HEADER_in_functionHeader290);
					CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_type_in_functionHeader292);
					PapyrusTypeWalker.type_return type_return = this.type();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, type_return.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_functionHeader296);
					CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree4);
					this.adaptor.AddChild(commonTree3, child);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode2 = (CommonTree)this.Match(this.input, 18, PapyrusTypeWalker.FOLLOW_USER_FLAGS_in_functionHeader298);
					CommonTree child2 = (CommonTree)this.adaptor.DupNode(treeNode2);
					this.adaptor.AddChild(commonTree3, child2);
					int num5 = 2;
					int num6 = this.input.LA(1);
					if (num6 == 9)
					{
						num5 = 1;
					}
					int num7 = num5;
					if (num7 == 1)
					{
						commonTree2 = (CommonTree)this.input.LT(1);
						base.PushFollow(PapyrusTypeWalker.FOLLOW_callParameters_in_functionHeader300);
						PapyrusTypeWalker.callParameters_return callParameters_return = this.callParameters();
						this.state.followingStackPointer--;
						this.adaptor.AddChild(commonTree3, callParameters_return.Tree);
					}
					for (;;)
					{
						int num8 = 2;
						int num9 = this.input.LA(1);
						if (num9 >= 46 && num9 <= 47)
						{
							num8 = 1;
						}
						int num10 = num8;
						if (num10 != 1)
						{
							break;
						}
						commonTree2 = (CommonTree)this.input.LT(1);
						base.PushFollow(PapyrusTypeWalker.FOLLOW_functionModifier_in_functionHeader303);
						PapyrusTypeWalker.functionModifier_return functionModifier_return = this.functionModifier();
						this.state.followingStackPointer--;
						this.adaptor.AddChild(commonTree3, functionModifier_return.Tree);
					}
					int num11 = 2;
					int num12 = this.input.LA(1);
					if (num12 == 40)
					{
						num11 = 1;
					}
					int num13 = num11;
					if (num13 == 1)
					{
						commonTree2 = (CommonTree)this.input.LT(1);
						CommonTree treeNode3 = (CommonTree)this.Match(this.input, 40, PapyrusTypeWalker.FOLLOW_DOCSTRING_in_functionHeader306);
						CommonTree child3 = (CommonTree)this.adaptor.DupNode(treeNode3);
						this.adaptor.AddChild(commonTree3, child3);
					}
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, commonTree3);
					functionHeader_return.sFuncName = commonTree4.Text;
					this.CheckTypeAndValue((type_return != null) ? type_return.kType : null, null, commonTree4.Token);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree5 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode4 = (CommonTree)this.Match(this.input, 8, PapyrusTypeWalker.FOLLOW_HEADER_in_functionHeader320);
					CommonTree newRoot2 = (CommonTree)this.adaptor.DupNode(treeNode4);
					commonTree5 = (CommonTree)this.adaptor.BecomeRoot(newRoot2, commonTree5);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode5 = (CommonTree)this.Match(this.input, 92, PapyrusTypeWalker.FOLLOW_NONE_in_functionHeader322);
					CommonTree child4 = (CommonTree)this.adaptor.DupNode(treeNode5);
					this.adaptor.AddChild(commonTree5, child4);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_functionHeader324);
					CommonTree child5 = (CommonTree)this.adaptor.DupNode(commonTree6);
					this.adaptor.AddChild(commonTree5, child5);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode6 = (CommonTree)this.Match(this.input, 18, PapyrusTypeWalker.FOLLOW_USER_FLAGS_in_functionHeader326);
					CommonTree child6 = (CommonTree)this.adaptor.DupNode(treeNode6);
					this.adaptor.AddChild(commonTree5, child6);
					int num14 = 2;
					int num15 = this.input.LA(1);
					if (num15 == 9)
					{
						num14 = 1;
					}
					int num16 = num14;
					if (num16 == 1)
					{
						commonTree2 = (CommonTree)this.input.LT(1);
						base.PushFollow(PapyrusTypeWalker.FOLLOW_callParameters_in_functionHeader328);
						PapyrusTypeWalker.callParameters_return callParameters_return2 = this.callParameters();
						this.state.followingStackPointer--;
						this.adaptor.AddChild(commonTree5, callParameters_return2.Tree);
					}
					for (;;)
					{
						int num17 = 2;
						int num18 = this.input.LA(1);
						if (num18 >= 46 && num18 <= 47)
						{
							num17 = 1;
						}
						int num19 = num17;
						if (num19 != 1)
						{
							break;
						}
						commonTree2 = (CommonTree)this.input.LT(1);
						base.PushFollow(PapyrusTypeWalker.FOLLOW_functionModifier_in_functionHeader331);
						PapyrusTypeWalker.functionModifier_return functionModifier_return2 = this.functionModifier();
						this.state.followingStackPointer--;
						this.adaptor.AddChild(commonTree5, functionModifier_return2.Tree);
					}
					int num20 = 2;
					int num21 = this.input.LA(1);
					if (num21 == 40)
					{
						num20 = 1;
					}
					int num22 = num20;
					if (num22 == 1)
					{
						commonTree2 = (CommonTree)this.input.LT(1);
						CommonTree treeNode7 = (CommonTree)this.Match(this.input, 40, PapyrusTypeWalker.FOLLOW_DOCSTRING_in_functionHeader334);
						CommonTree child7 = (CommonTree)this.adaptor.DupNode(treeNode7);
						this.adaptor.AddChild(commonTree5, child7);
					}
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, commonTree5);
					functionHeader_return.sFuncName = commonTree6.Text;
					break;
				}
				}
				functionHeader_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				if (((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).spropertyName == "")
				{
					this.kObjType.TryGetFunction(((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).sstateName, functionHeader_return.sFuncName, out ((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).kfunctionType);
				}
				else
				{
					ScriptPropertyType scriptPropertyType;
					this.kObjType.TryGetProperty(((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).spropertyName, out scriptPropertyType);
					string a = functionHeader_return.sFuncName.ToLowerInvariant();
					if (a == "get")
					{
						((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).kfunctionType = scriptPropertyType.kGetFunction;
					}
					else
					{
						((PapyrusTypeWalker.function_scope)this.function_stack.Peek()).kfunctionType = scriptPropertyType.kSetFunction;
					}
				}
			}
			catch (RecognitionException ex4)
			{
				this.ReportError(ex4);
				this.Recover(this.input, ex4);
			}
			return functionHeader_return;
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x0004DD30 File Offset: 0x0004BF30
		public PapyrusTypeWalker.functionModifier_return functionModifier()
		{
			PapyrusTypeWalker.functionModifier_return functionModifier_return = new PapyrusTypeWalker.functionModifier_return();
			functionModifier_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.input.LT(1);
				if (this.input.LA(1) < 46 || this.input.LA(1) > 47)
				{
					MismatchedSetException ex = new MismatchedSetException(null, this.input);
					throw ex;
				}
				this.input.Consume();
				CommonTree child = (CommonTree)this.adaptor.DupNode(treeNode);
				this.adaptor.AddChild(commonTree, child);
				this.state.errorRecovery = false;
				functionModifier_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return functionModifier_return;
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x0004DE40 File Offset: 0x0004C040
		public PapyrusTypeWalker.eventFunc_return eventFunc(string asStateName)
		{
			this.eventFunc_stack.Push(new PapyrusTypeWalker.eventFunc_scope());
			PapyrusTypeWalker.eventFunc_return eventFunc_return = new PapyrusTypeWalker.eventFunc_return();
			eventFunc_return.Start = this.input.LT(1);
			((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).sstateName = asStateName;
			((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).seventName = "";
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 7, PapyrusTypeWalker.FOLLOW_EVENT_in_eventFunc388);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_eventHeader_in_eventFunc390);
				PapyrusTypeWalker.eventHeader_return eventHeader_return = this.eventHeader();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, eventHeader_return.Tree);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 10)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_codeBlock_in_eventFunc392);
					PapyrusTypeWalker.codeBlock_return codeBlock_return = this.codeBlock(((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).kfunctionType, ((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).kfunctionType.FunctionScope);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				eventFunc_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				this.CheckFunction(((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).kfunctionType, ((CommonTree)eventFunc_return.Start).Token);
				this.kUnusedTempVarsByType.Clear();
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.eventFunc_stack.Pop();
			}
			return eventFunc_return;
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x0004E108 File Offset: 0x0004C308
		public PapyrusTypeWalker.eventHeader_return eventHeader()
		{
			PapyrusTypeWalker.eventHeader_return eventHeader_return = new PapyrusTypeWalker.eventHeader_return();
			eventHeader_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 8, PapyrusTypeWalker.FOLLOW_HEADER_in_eventHeader408);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode2 = (CommonTree)this.Match(this.input, 92, PapyrusTypeWalker.FOLLOW_NONE_in_eventHeader410);
				CommonTree child = (CommonTree)this.adaptor.DupNode(treeNode2);
				this.adaptor.AddChild(commonTree3, child);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_eventHeader412);
				CommonTree child2 = (CommonTree)this.adaptor.DupNode(commonTree4);
				this.adaptor.AddChild(commonTree3, child2);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode3 = (CommonTree)this.Match(this.input, 18, PapyrusTypeWalker.FOLLOW_USER_FLAGS_in_eventHeader414);
				CommonTree child3 = (CommonTree)this.adaptor.DupNode(treeNode3);
				this.adaptor.AddChild(commonTree3, child3);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 9)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_callParameters_in_eventHeader416);
					PapyrusTypeWalker.callParameters_return callParameters_return = this.callParameters();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, callParameters_return.Tree);
				}
				int num4 = 2;
				int num5 = this.input.LA(1);
				if (num5 == 47)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode4 = (CommonTree)this.Match(this.input, 47, PapyrusTypeWalker.FOLLOW_NATIVE_in_eventHeader419);
					CommonTree child4 = (CommonTree)this.adaptor.DupNode(treeNode4);
					this.adaptor.AddChild(commonTree3, child4);
				}
				int num7 = 2;
				int num8 = this.input.LA(1);
				if (num8 == 40)
				{
					num7 = 1;
				}
				int num9 = num7;
				if (num9 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode5 = (CommonTree)this.Match(this.input, 40, PapyrusTypeWalker.FOLLOW_DOCSTRING_in_eventHeader422);
					CommonTree child5 = (CommonTree)this.adaptor.DupNode(treeNode5);
					this.adaptor.AddChild(commonTree3, child5);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).seventName = commonTree4.Text;
				this.kObjType.TryGetFunction(((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).sstateName, ((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).seventName, out ((PapyrusTypeWalker.eventFunc_scope)this.eventFunc_stack.Peek()).kfunctionType);
				eventHeader_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return eventHeader_return;
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x0004E508 File Offset: 0x0004C708
		public PapyrusTypeWalker.callParameters_return callParameters()
		{
			PapyrusTypeWalker.callParameters_return callParameters_return = new PapyrusTypeWalker.callParameters_return();
			callParameters_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				int num = 0;
				for (;;)
				{
					int num2 = 2;
					int num3 = this.input.LA(1);
					if (num3 == 9)
					{
						num2 = 1;
					}
					int num4 = num2;
					if (num4 != 1)
					{
						break;
					}
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_callParameter_in_callParameters442);
					PapyrusTypeWalker.callParameter_return callParameter_return = this.callParameter();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, callParameter_return.Tree);
					num++;
				}
				if (num < 1)
				{
					EarlyExitException ex = new EarlyExitException(18, this.input);
					throw ex;
				}
				callParameters_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return callParameters_return;
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x0004E618 File Offset: 0x0004C818
		public PapyrusTypeWalker.callParameter_return callParameter()
		{
			PapyrusTypeWalker.callParameter_return callParameter_return = new PapyrusTypeWalker.callParameter_return();
			callParameter_return.Start = this.input.LT(1);
			PapyrusTypeWalker.constant_return constant_return = null;
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 9, PapyrusTypeWalker.FOLLOW_PARAM_in_callParameter456);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_type_in_callParameter458);
				PapyrusTypeWalker.type_return type_return = this.type();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, type_return.Tree);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_callParameter462);
				CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree4);
				this.adaptor.AddChild(commonTree3, child);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 81 || (num2 >= 90 && num2 <= 93))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_constant_in_callParameter464);
					constant_return = this.constant();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, constant_return.Tree);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				if (((constant_return != null) ? ((CommonTree)constant_return.Start) : null) != null)
				{
					this.CheckTypeAndValue((type_return != null) ? type_return.kType : null, ((constant_return != null) ? ((CommonTree)constant_return.Start) : null).Token, commonTree4.Token);
				}
				else
				{
					this.CheckTypeAndValue((type_return != null) ? type_return.kType : null, null, commonTree4.Token);
				}
				callParameter_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return callParameter_return;
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x0004E8D8 File Offset: 0x0004CAD8
		public PapyrusTypeWalker.stateBlock_return stateBlock()
		{
			PapyrusTypeWalker.stateBlock_return stateBlock_return = new PapyrusTypeWalker.stateBlock_return();
			stateBlock_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 51, PapyrusTypeWalker.FOLLOW_STATE_in_stateBlock485);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_stateBlock487);
				CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree4);
				this.adaptor.AddChild(commonTree3, child);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 50)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode2 = (CommonTree)this.Match(this.input, 50, PapyrusTypeWalker.FOLLOW_AUTO_in_stateBlock489);
					CommonTree child2 = (CommonTree)this.adaptor.DupNode(treeNode2);
					this.adaptor.AddChild(commonTree3, child2);
				}
				for (;;)
				{
					int num4 = 2;
					int num5 = this.input.LA(1);
					if (num5 >= 6 && num5 <= 7)
					{
						num4 = 1;
					}
					int num6 = num4;
					if (num6 != 1)
					{
						break;
					}
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_stateFuncOrEvent_in_stateBlock493);
					PapyrusTypeWalker.stateFuncOrEvent_return stateFuncOrEvent_return = this.stateFuncOrEvent(commonTree4.Text);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, stateFuncOrEvent_return.Tree);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				stateBlock_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return stateBlock_return;
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x0004EB60 File Offset: 0x0004CD60
		public PapyrusTypeWalker.stateFuncOrEvent_return stateFuncOrEvent(string asState)
		{
			PapyrusTypeWalker.stateFuncOrEvent_return stateFuncOrEvent_return = new PapyrusTypeWalker.stateFuncOrEvent_return();
			stateFuncOrEvent_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 6)
				{
					num2 = 1;
				}
				else
				{
					if (num != 7)
					{
						NoViableAltException ex = new NoViableAltException("", 22, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_function_in_stateFuncOrEvent510);
					PapyrusTypeWalker.function_return function_return = this.function(asState, "");
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, function_return.Tree);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree3 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_eventFunc_in_stateFuncOrEvent518);
					PapyrusTypeWalker.eventFunc_return eventFunc_return = this.eventFunc(asState);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, eventFunc_return.Tree);
					break;
				}
				}
				stateFuncOrEvent_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return stateFuncOrEvent_return;
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x0004ECF0 File Offset: 0x0004CEF0
		public PapyrusTypeWalker.propertyBlock_return propertyBlock()
		{
			this.propertyBlock_stack.Push(new PapyrusTypeWalker.propertyBlock_scope());
			PapyrusTypeWalker.propertyBlock_return propertyBlock_return = new PapyrusTypeWalker.propertyBlock_return();
			propertyBlock_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			PapyrusTypeWalker.propertyFunc_return propertyFunc_return = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token PROPERTY");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule propertyFunc");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule propertyHeader");
			((PapyrusTypeWalker.propertyBlock_scope)this.propertyBlock_stack.Peek()).bfunc0IsGet = false;
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 54)
				{
					num2 = 1;
				}
				else
				{
					if (num != 19)
					{
						NoViableAltException ex = new NoViableAltException("", 24, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 54, PapyrusTypeWalker.FOLLOW_PROPERTY_in_propertyBlock543);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_propertyHeader_in_propertyBlock545);
					PapyrusTypeWalker.propertyHeader_return propertyHeader_return = this.propertyHeader();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(propertyHeader_return.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_propertyFunc_in_propertyBlock549);
					PapyrusTypeWalker.propertyFunc_return propertyFunc_return2 = this.propertyFunc((propertyHeader_return != null) ? propertyHeader_return.sName : null);
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(propertyFunc_return2.Tree);
					int num3 = 2;
					int num4 = this.input.LA(1);
					if (num4 == 17)
					{
						num3 = 1;
					}
					int num5 = num3;
					if (num5 == 1)
					{
						commonTree2 = (CommonTree)this.input.LT(1);
						base.PushFollow(PapyrusTypeWalker.FOLLOW_propertyFunc_in_propertyBlock554);
						propertyFunc_return = this.propertyFunc((propertyHeader_return != null) ? propertyHeader_return.sName : null);
						this.state.followingStackPointer--;
						rewriteRuleSubtreeStream.Add(propertyFunc_return.Tree);
					}
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.CheckPropertyOverride((propertyHeader_return != null) ? propertyHeader_return.sName : null, commonTree3.Token);
					((PapyrusTypeWalker.propertyBlock_scope)this.propertyBlock_stack.Peek()).bfunc0IsGet = (propertyFunc_return2 != null && propertyFunc_return2.bIsGet);
					propertyBlock_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (propertyBlock_return != null) ? propertyBlock_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule func0", (propertyFunc_return2 != null) ? propertyFunc_return2.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule func1", (propertyFunc_return != null) ? propertyFunc_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					if (((PapyrusTypeWalker.propertyBlock_scope)this.propertyBlock_stack.Peek()).bfunc0IsGet && ((propertyFunc_return != null) ? ((CommonTree)propertyFunc_return.Start) : null) != null)
					{
						CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
						commonTree4 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
						this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream2.NextTree());
						this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
						this.adaptor.AddChild(commonTree, commonTree4);
					}
					else if (!((PapyrusTypeWalker.propertyBlock_scope)this.propertyBlock_stack.Peek()).bfunc0IsGet && ((propertyFunc_return != null) ? ((CommonTree)propertyFunc_return.Start) : null) != null)
					{
						CommonTree commonTree5 = (CommonTree)this.adaptor.GetNilNode();
						commonTree5 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
						this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream2.NextTree());
						this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream4.NextTree());
						this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree, commonTree5);
					}
					else if (((PapyrusTypeWalker.propertyBlock_scope)this.propertyBlock_stack.Peek()).bfunc0IsGet)
					{
						CommonTree commonTree6 = (CommonTree)this.adaptor.GetNilNode();
						commonTree6 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree6);
						this.adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream2.NextTree());
						this.adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree6, (CommonTree)this.adaptor.Create(17, commonTree3.Token, "propfunc"));
						this.adaptor.AddChild(commonTree, commonTree6);
					}
					else
					{
						CommonTree commonTree7 = (CommonTree)this.adaptor.GetNilNode();
						commonTree7 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree7);
						this.adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream2.NextTree());
						this.adaptor.AddChild(commonTree7, (CommonTree)this.adaptor.Create(17, commonTree3.Token, "propfunc"));
						this.adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree, commonTree7);
					}
					propertyBlock_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree8 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree9 = (CommonTree)this.Match(this.input, 19, PapyrusTypeWalker.FOLLOW_AUTOPROP_in_propertyBlock651);
					CommonTree newRoot = (CommonTree)this.adaptor.DupNode(commonTree9);
					commonTree8 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree8);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_propertyHeader_in_propertyBlock653);
					PapyrusTypeWalker.propertyHeader_return propertyHeader_return2 = this.propertyHeader();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree8, propertyHeader_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_propertyBlock655);
					CommonTree child2 = (CommonTree)this.adaptor.DupNode(treeNode);
					this.adaptor.AddChild(commonTree8, child2);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, commonTree8);
					this.CheckPropertyOverride((propertyHeader_return2 != null) ? propertyHeader_return2.sName : null, commonTree9.Token);
					break;
				}
				}
				propertyBlock_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.propertyBlock_stack.Pop();
			}
			return propertyBlock_return;
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x0004F4BC File Offset: 0x0004D6BC
		public PapyrusTypeWalker.propertyHeader_return propertyHeader()
		{
			PapyrusTypeWalker.propertyHeader_return propertyHeader_return = new PapyrusTypeWalker.propertyHeader_return();
			propertyHeader_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 8, PapyrusTypeWalker.FOLLOW_HEADER_in_propertyHeader678);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(commonTree4);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_type_in_propertyHeader680);
				PapyrusTypeWalker.type_return type_return = this.type();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, type_return.Tree);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree5 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_propertyHeader684);
				CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree5);
				this.adaptor.AddChild(commonTree3, child);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 18, PapyrusTypeWalker.FOLLOW_USER_FLAGS_in_propertyHeader686);
				CommonTree child2 = (CommonTree)this.adaptor.DupNode(treeNode);
				this.adaptor.AddChild(commonTree3, child2);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 40)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode2 = (CommonTree)this.Match(this.input, 40, PapyrusTypeWalker.FOLLOW_DOCSTRING_in_propertyHeader688);
					CommonTree child3 = (CommonTree)this.adaptor.DupNode(treeNode2);
					this.adaptor.AddChild(commonTree3, child3);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				propertyHeader_return.sName = commonTree5.Text;
				this.CheckVarOrPropName(propertyHeader_return.sName, commonTree4.Token);
				propertyHeader_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return propertyHeader_return;
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x0004F784 File Offset: 0x0004D984
		public PapyrusTypeWalker.propertyFunc_return propertyFunc(string asPropName)
		{
			PapyrusTypeWalker.propertyFunc_return propertyFunc_return = new PapyrusTypeWalker.propertyFunc_return();
			propertyFunc_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 17, PapyrusTypeWalker.FOLLOW_PROPFUNC_in_propertyFunc713);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(commonTree4);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_function_in_propertyFunc715);
				PapyrusTypeWalker.function_return function_return = this.function("", asPropName);
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, function_return.Tree);
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				this.CheckPropertyFunction(asPropName, (function_return != null) ? function_return.sName : null, out propertyFunc_return.bIsGet, commonTree4.Token);
				propertyFunc_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return propertyFunc_return;
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x0004F934 File Offset: 0x0004DB34
		public PapyrusTypeWalker.codeBlock_return codeBlock(ScriptFunctionType akFunctionType, ScriptScope akCurrentScope)
		{
			this.codeBlock_stack.Push(new PapyrusTypeWalker.codeBlock_scope());
			PapyrusTypeWalker.codeBlock_return codeBlock_return = new PapyrusTypeWalker.codeBlock_return();
			codeBlock_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			IList list = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token BLOCK");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule statement");
			((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType = akFunctionType;
			((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope = akCurrentScope;
			((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars = new Dictionary<string, ScriptVariableType>();
			((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild = 0;
			try
			{
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree child = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.Match(this.input, 10, PapyrusTypeWalker.FOLLOW_BLOCK_in_codeBlock747);
				rewriteRuleNodeStream.Add(commonTree3);
				if (this.input.LA(1) == 2)
				{
					this.Match(this.input, 2, null);
					for (;;)
					{
						int num = 2;
						int num2 = this.input.LA(1);
						if (num2 == 5 || num2 == 11 || (num2 >= 15 && num2 <= 16) || (num2 == 22 || num2 == 38 || num2 == 41 || num2 == 62 || (num2 >= 65 && num2 <= 84)) || num2 == 88 || (num2 >= 90 && num2 <= 93))
						{
							num = 1;
						}
						int num3 = num;
						if (num3 != 1)
						{
							break;
						}
						commonTree2 = (CommonTree)this.input.LT(1);
						commonTree2 = (CommonTree)this.input.LT(1);
						base.PushFollow(PapyrusTypeWalker.FOLLOW_statement_in_codeBlock752);
						PapyrusTypeWalker.statement_return statement_return = this.statement();
						this.state.followingStackPointer--;
						rewriteRuleSubtreeStream.Add(statement_return.Tree);
						if (list == null)
						{
							list = new ArrayList();
						}
						list.Add(statement_return.Tree);
					}
					this.Match(this.input, 3, null);
				}
				this.adaptor.AddChild(commonTree, child);
				codeBlock_return.Tree = commonTree;
				new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (codeBlock_return != null) ? codeBlock_return.Tree : null);
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				this.adaptor.AddChild(commonTree, this.CreateBlockTree(commonTree3.Token, list, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars));
				codeBlock_return.Tree = commonTree;
				codeBlock_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.codeBlock_stack.Pop();
			}
			return codeBlock_return;
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x0004FC58 File Offset: 0x0004DE58
		public PapyrusTypeWalker.statement_return statement()
		{
			this.statement_stack.Push(new PapyrusTypeWalker.statement_scope());
			PapyrusTypeWalker.statement_return statement_return = new PapyrusTypeWalker.statement_return();
			statement_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token EQUALS");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule l_value");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num <= 16)
				{
					if (num == 5)
					{
						num2 = 1;
						goto IL_1E0;
					}
					if (num != 11)
					{
						switch (num)
						{
						case 15:
						case 16:
							break;
						default:
							goto IL_1C8;
						}
					}
				}
				else if (num != 22 && num != 38)
				{
					switch (num)
					{
					case 41:
						num2 = 2;
						goto IL_1E0;
					case 42:
					case 43:
					case 44:
					case 45:
					case 46:
					case 47:
					case 48:
					case 49:
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 57:
					case 58:
					case 59:
					case 60:
					case 61:
					case 63:
					case 64:
					case 85:
					case 86:
					case 87:
					case 89:
						goto IL_1C8;
					case 62:
					case 65:
					case 66:
					case 67:
					case 68:
					case 69:
					case 70:
					case 71:
					case 72:
					case 73:
					case 74:
					case 75:
					case 76:
					case 77:
					case 78:
					case 79:
					case 80:
					case 81:
					case 82:
					case 90:
					case 91:
					case 92:
					case 93:
						break;
					case 83:
						num2 = 4;
						goto IL_1E0;
					case 84:
						num2 = 5;
						goto IL_1E0;
					case 88:
						num2 = 6;
						goto IL_1E0;
					default:
						goto IL_1C8;
					}
				}
				num2 = 3;
				goto IL_1E0;
				IL_1C8:
				NoViableAltException ex = new NoViableAltException("", 27, 0, this.input);
				throw ex;
				IL_1E0:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_localDefinition_in_statement782);
					PapyrusTypeWalker.localDefinition_return localDefinition_return = this.localDefinition();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, localDefinition_return.Tree);
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 41, PapyrusTypeWalker.FOLLOW_EQUALS_in_statement789);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_l_value_in_statement791);
					PapyrusTypeWalker.l_value_return l_value_return = this.l_value();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(l_value_return.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_statement793);
					PapyrusTypeWalker.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.CheckAssignmentType((l_value_return != null) ? l_value_return.kType : null, (expression_return != null) ? expression_return.kType : null, ((l_value_return != null) ? ((CommonTree)l_value_return.Start) : null).Token);
					string asVarName;
					((PapyrusTypeWalker.statement_scope)this.statement_stack.Peek()).kautoCastTree = this.AutoCast((expression_return != null) ? expression_return.kVarToken : null, (expression_return != null) ? expression_return.kType : null, (l_value_return != null) ? l_value_return.kType : null, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out asVarName);
					this.MarkTempVarAsUnused((l_value_return != null) ? l_value_return.kType : null, asVarName, ((l_value_return != null) ? ((CommonTree)l_value_return.Start) : null).Token);
					this.MarkTempVarAsUnused((l_value_return != null) ? l_value_return.kType : null, (l_value_return != null) ? l_value_return.sVarName : null, ((l_value_return != null) ? ((CommonTree)l_value_return.Start) : null).Token);
					this.MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, ((expression_return != null) ? ((CommonTree)expression_return.Start) : null).Token);
					statement_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (statement_return != null) ? statement_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree4 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
					this.adaptor.AddChild(commonTree4, (CommonTree)this.adaptor.Create(38, commonTree3.Token, (l_value_return != null) ? l_value_return.sVarName : null));
					this.adaptor.AddChild(commonTree4, ((PapyrusTypeWalker.statement_scope)this.statement_stack.Peek()).kautoCastTree);
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream2.NextTree());
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
					this.adaptor.AddChild(commonTree, commonTree4);
					statement_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_statement824);
					PapyrusTypeWalker.expression_return expression_return2 = this.expression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, expression_return2.Tree);
					this.MarkTempVarAsUnused((expression_return2 != null) ? expression_return2.kType : null, (expression_return2 != null) ? expression_return2.sVarName : null, ((expression_return2 != null) ? ((CommonTree)expression_return2.Start) : null).Token);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_return_stat_in_statement835);
					PapyrusTypeWalker.return_stat_return return_stat_return = this.return_stat();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, return_stat_return.Tree);
					break;
				}
				case 5:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_ifBlock_in_statement841);
					PapyrusTypeWalker.ifBlock_return ifBlock_return = this.ifBlock();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, ifBlock_return.Tree);
					break;
				}
				case 6:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_whileBlock_in_statement847);
					PapyrusTypeWalker.whileBlock_return whileBlock_return = this.whileBlock();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, whileBlock_return.Tree);
					break;
				}
				}
				statement_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.statement_stack.Pop();
			}
			return statement_return;
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x00050458 File Offset: 0x0004E658
		public PapyrusTypeWalker.localDefinition_return localDefinition()
		{
			this.localDefinition_stack.Push(new PapyrusTypeWalker.localDefinition_scope());
			PapyrusTypeWalker.localDefinition_return localDefinition_return = new PapyrusTypeWalker.localDefinition_return();
			localDefinition_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			PapyrusTypeWalker.expression_return expression_return = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token VAR");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule type");
			try
			{
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree child = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree el = (CommonTree)this.Match(this.input, 5, PapyrusTypeWalker.FOLLOW_VAR_in_localDefinition865);
				rewriteRuleNodeStream.Add(el);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_type_in_localDefinition867);
				PapyrusTypeWalker.type_return type_return = this.type();
				this.state.followingStackPointer--;
				rewriteRuleSubtreeStream2.Add(type_return.Tree);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_localDefinition871);
				rewriteRuleNodeStream2.Add(commonTree3);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 11 || (num2 >= 15 && num2 <= 16) || (num2 == 22 || num2 == 38 || num2 == 62 || (num2 >= 65 && num2 <= 82)) || (num2 >= 90 && num2 <= 93))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_localDefinition873);
					expression_return = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, child);
				this.CheckVariableDefinition(commonTree3.Text, (type_return != null) ? type_return.kType : null, null, true, commonTree3.Token);
				this.CheckAssignmentType((type_return != null) ? type_return.kType : null, (expression_return != null) ? expression_return.kType : null, commonTree3.Token);
				if (((expression_return != null) ? ((CommonTree)expression_return.Tree) : null) != null)
				{
					string asVarName;
					((PapyrusTypeWalker.localDefinition_scope)this.localDefinition_stack.Peek()).kautoCastTree = this.AutoCast((expression_return != null) ? expression_return.kVarToken : null, (expression_return != null) ? expression_return.kType : null, (type_return != null) ? type_return.kType : null, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out asVarName);
					this.MarkTempVarAsUnused((type_return != null) ? type_return.kType : null, asVarName, commonTree3.Token);
					this.MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, commonTree3.Token);
				}
				localDefinition_return.Tree = commonTree;
				RewriteRuleNodeStream rewriteRuleNodeStream3 = new RewriteRuleNodeStream(this.adaptor, "token name", commonTree3);
				new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (localDefinition_return != null) ? localDefinition_return.Tree : null);
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				if (((expression_return != null) ? ((CommonTree)expression_return.Tree) : null) == null)
				{
					CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree4 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream2.NextTree());
					this.adaptor.AddChild(commonTree4, rewriteRuleNodeStream3.NextNode());
					this.adaptor.AddChild(commonTree, commonTree4);
				}
				else
				{
					CommonTree commonTree5 = (CommonTree)this.adaptor.GetNilNode();
					commonTree5 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
					this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream2.NextTree());
					this.adaptor.AddChild(commonTree5, rewriteRuleNodeStream3.NextNode());
					this.adaptor.AddChild(commonTree5, ((PapyrusTypeWalker.localDefinition_scope)this.localDefinition_stack.Peek()).kautoCastTree);
					this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
					this.adaptor.AddChild(commonTree, commonTree5);
				}
				localDefinition_return.Tree = commonTree;
				localDefinition_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.localDefinition_stack.Pop();
			}
			return localDefinition_return;
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x0005097C File Offset: 0x0004EB7C
		public PapyrusTypeWalker.l_value_return l_value()
		{
			this.l_value_stack.Push(new PapyrusTypeWalker.l_value_scope());
			PapyrusTypeWalker.l_value_return l_value_return = new PapyrusTypeWalker.l_value_return();
			l_value_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token PAREXPR");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token ARRAYSET");
			RewriteRuleNodeStream rewriteRuleNodeStream3 = new RewriteRuleNodeStream(this.adaptor, "token DOT");
			RewriteRuleNodeStream rewriteRuleNodeStream4 = new RewriteRuleNodeStream(this.adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule expression");
			try
			{
				int num = this.input.LA(1);
				int num4;
				if (num != 23)
				{
					if (num != 38)
					{
						if (num != 62)
						{
							NoViableAltException ex = new NoViableAltException("", 29, 0, this.input);
							throw ex;
						}
						int num2 = this.input.LA(2);
						if (num2 != 2)
						{
							NoViableAltException ex2 = new NoViableAltException("", 29, 1, this.input);
							throw ex2;
						}
						int num3 = this.input.LA(3);
						if (num3 == 15)
						{
							num4 = 1;
						}
						else
						{
							if (num3 != 11 && num3 != 22 && num3 != 38 && num3 != 82)
							{
								NoViableAltException ex3 = new NoViableAltException("", 29, 4, this.input);
								throw ex3;
							}
							num4 = 3;
						}
					}
					else
					{
						num4 = 3;
					}
				}
				else
				{
					int num5 = this.input.LA(2);
					if (num5 != 2)
					{
						NoViableAltException ex4 = new NoViableAltException("", 29, 2, this.input);
						throw ex4;
					}
					int num6 = this.input.LA(3);
					if (num6 == 15)
					{
						num4 = 2;
					}
					else
					{
						if (num6 != 11 && num6 != 38 && num6 != 82)
						{
							NoViableAltException ex5 = new NoViableAltException("", 29, 5, this.input);
							throw ex5;
						}
						num4 = 3;
					}
				}
				switch (num4)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 62, PapyrusTypeWalker.FOLLOW_DOT_in_l_value936);
					rewriteRuleNodeStream3.Add(commonTree4);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree el = (CommonTree)this.Match(this.input, 15, PapyrusTypeWalker.FOLLOW_PAREXPR_in_l_value939);
					rewriteRuleNodeStream.Add(el);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_l_value943);
					PapyrusTypeWalker.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree3, child);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_l_value948);
					rewriteRuleNodeStream4.Add(commonTree5);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, commonTree3);
					this.MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, commonTree4.Token);
					this.GetPropertyInfo((expression_return != null) ? expression_return.kType : null, commonTree5.Token, false, out l_value_return.kType);
					l_value_return.sVarName = this.GenerateTempVariable(l_value_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					l_value_return.Tree = commonTree;
					RewriteRuleNodeStream rewriteRuleNodeStream5 = new RewriteRuleNodeStream(this.adaptor, "token b", commonTree5);
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (l_value_return != null) ? l_value_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (expression_return != null) ? expression_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree6 = (CommonTree)this.adaptor.GetNilNode();
					commonTree6 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream3.NextNode(), commonTree6);
					CommonTree commonTree7 = (CommonTree)this.adaptor.GetNilNode();
					commonTree7 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree7);
					this.adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream2.NextTree());
					this.adaptor.AddChild(commonTree6, commonTree7);
					CommonTree commonTree8 = (CommonTree)this.adaptor.GetNilNode();
					commonTree8 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(21, commonTree5.Token, "propset"), commonTree8);
					this.adaptor.AddChild(commonTree8, (CommonTree)this.adaptor.Create(38, commonTree5.Token, (expression_return != null) ? expression_return.sVarName : null));
					this.adaptor.AddChild(commonTree8, rewriteRuleNodeStream5.NextNode());
					this.adaptor.AddChild(commonTree8, (CommonTree)this.adaptor.Create(38, commonTree5.Token, l_value_return.sVarName));
					this.adaptor.AddChild(commonTree6, commonTree8);
					this.adaptor.AddChild(commonTree, commonTree6);
					l_value_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree9 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree el2 = (CommonTree)this.Match(this.input, 23, PapyrusTypeWalker.FOLLOW_ARRAYSET_in_l_value995);
					rewriteRuleNodeStream2.Add(el2);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child2 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree el3 = (CommonTree)this.Match(this.input, 15, PapyrusTypeWalker.FOLLOW_PAREXPR_in_l_value998);
					rewriteRuleNodeStream.Add(el3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_l_value1002);
					PapyrusTypeWalker.expression_return expression_return2 = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return2.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree9, child2);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_l_value1007);
					PapyrusTypeWalker.expression_return expression_return3 = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return3.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, commonTree9);
					this.HandleArrayElementExpression((expression_return2 != null) ? expression_return2.sVarName : null, (expression_return2 != null) ? expression_return2.kType : null, (expression_return2 != null) ? expression_return2.kVarToken : null, (expression_return3 != null) ? expression_return3.sVarName : null, (expression_return3 != null) ? expression_return3.kType : null, (expression_return3 != null) ? expression_return3.kVarToken : null, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out l_value_return.sVarName, out l_value_return.kType, out ((PapyrusTypeWalker.l_value_scope)this.l_value_stack.Peek()).kvarToken, out ((PapyrusTypeWalker.l_value_scope)this.l_value_stack.Peek()).kautoCastTree);
					l_value_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (l_value_return != null) ? l_value_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule index", (expression_return3 != null) ? expression_return3.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule array", (expression_return2 != null) ? expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree10 = (CommonTree)this.adaptor.GetNilNode();
					commonTree10 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree10);
					this.adaptor.AddChild(commonTree10, (CommonTree)this.adaptor.Create(38, ((PapyrusTypeWalker.l_value_scope)this.l_value_stack.Peek()).kvarToken));
					this.adaptor.AddChild(commonTree10, (CommonTree)this.adaptor.Create(38, (expression_return2 != null) ? expression_return2.kVarToken : null));
					this.adaptor.AddChild(commonTree10, ((PapyrusTypeWalker.l_value_scope)this.l_value_stack.Peek()).kautoCastTree);
					CommonTree commonTree11 = (CommonTree)this.adaptor.GetNilNode();
					commonTree11 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree11);
					this.adaptor.AddChild(commonTree11, rewriteRuleSubtreeStream4.NextTree());
					this.adaptor.AddChild(commonTree10, commonTree11);
					this.adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream3.NextTree());
					this.adaptor.AddChild(commonTree, commonTree10);
					l_value_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_basic_l_value_in_l_value1046);
					PapyrusTypeWalker.basic_l_value_return basic_l_value_return = this.basic_l_value(new ScriptVariableType("none"), "!first");
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, basic_l_value_return.Tree);
					l_value_return.kType = ((basic_l_value_return != null) ? basic_l_value_return.kType : null);
					l_value_return.sVarName = ((basic_l_value_return != null) ? basic_l_value_return.sVarName : null);
					break;
				}
				}
				l_value_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex6)
			{
				this.ReportError(ex6);
				this.Recover(this.input, ex6);
			}
			finally
			{
				this.l_value_stack.Pop();
			}
			return l_value_return;
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x00051468 File Offset: 0x0004F668
		public PapyrusTypeWalker.basic_l_value_return basic_l_value(ScriptVariableType akSelfType, string asSelfName)
		{
			this.basic_l_value_stack.Push(new PapyrusTypeWalker.basic_l_value_scope());
			PapyrusTypeWalker.basic_l_value_return basic_l_value_return = new PapyrusTypeWalker.basic_l_value_return();
			basic_l_value_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token ARRAYSET");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule func_or_id");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num != 23)
				{
					if (num != 38)
					{
						if (num != 62)
						{
							NoViableAltException ex = new NoViableAltException("", 30, 0, this.input);
							throw ex;
						}
						num2 = 1;
					}
					else
					{
						num2 = 3;
					}
				}
				else
				{
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 62, PapyrusTypeWalker.FOLLOW_DOT_in_basic_l_value1074);
					CommonTree newRoot = (CommonTree)this.adaptor.DupNode(commonTree4);
					commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_array_func_or_id_in_basic_l_value1078);
					PapyrusTypeWalker.array_func_or_id_return array_func_or_id_return = this.array_func_or_id(akSelfType, asSelfName);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, array_func_or_id_return.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_basic_l_value_in_basic_l_value1083);
					PapyrusTypeWalker.basic_l_value_return basic_l_value_return2 = this.basic_l_value((array_func_or_id_return != null) ? array_func_or_id_return.kType : null, (array_func_or_id_return != null) ? array_func_or_id_return.sVarName : null);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, basic_l_value_return2.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, commonTree3);
					this.MarkTempVarAsUnused((array_func_or_id_return != null) ? array_func_or_id_return.kType : null, (array_func_or_id_return != null) ? array_func_or_id_return.sVarName : null, commonTree4.Token);
					basic_l_value_return.kType = ((basic_l_value_return2 != null) ? basic_l_value_return2.kType : null);
					basic_l_value_return.sVarName = ((basic_l_value_return2 != null) ? basic_l_value_return2.sVarName : null);
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree el = (CommonTree)this.Match(this.input, 23, PapyrusTypeWalker.FOLLOW_ARRAYSET_in_basic_l_value1097);
					rewriteRuleNodeStream.Add(el);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_func_or_id_in_basic_l_value1099);
					PapyrusTypeWalker.func_or_id_return func_or_id_return = this.func_or_id(akSelfType, asSelfName);
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(func_or_id_return.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_basic_l_value1102);
					PapyrusTypeWalker.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.HandleArrayElementExpression((func_or_id_return != null) ? func_or_id_return.sVarName : null, (func_or_id_return != null) ? func_or_id_return.kType : null, (func_or_id_return != null) ? func_or_id_return.kVarToken : null, (expression_return != null) ? expression_return.sVarName : null, (expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.kVarToken : null, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out basic_l_value_return.sVarName, out basic_l_value_return.kType, out ((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).kvarToken, out ((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).kautoCastTree);
					basic_l_value_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (basic_l_value_return != null) ? basic_l_value_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree5 = (CommonTree)this.adaptor.GetNilNode();
					commonTree5 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
					this.adaptor.AddChild(commonTree5, (CommonTree)this.adaptor.Create(38, ((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).kvarToken));
					this.adaptor.AddChild(commonTree5, (CommonTree)this.adaptor.Create(38, (func_or_id_return != null) ? func_or_id_return.kVarToken : null));
					this.adaptor.AddChild(commonTree5, ((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).kautoCastTree);
					this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream2.NextTree());
					this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
					this.adaptor.AddChild(commonTree, commonTree5);
					basic_l_value_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_basic_l_value1135);
					rewriteRuleNodeStream2.Add(commonTree6);
					if (asSelfName != "!first")
					{
						if (asSelfName == "")
						{
							this.OnError("a property cannot be used directly on a type, it must be used on a variable", commonTree6.Line, commonTree6.CharPositionInLine);
						}
						else if (asSelfName.ToLowerInvariant() == "parent")
						{
							this.OnError("a property cannot be used on the special parent variable, use the property directly instead", commonTree6.Line, commonTree6.CharPositionInLine);
						}
						this.GetPropertyInfo(akSelfType, commonTree6.Token, false, out basic_l_value_return.kType);
						((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).bisProperty = true;
						basic_l_value_return.sVarName = this.GenerateTempVariable(basic_l_value_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					}
					else if (!((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType.bGlobal && this.IsLocalProperty(commonTree6.Text))
					{
						((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).bisProperty = true;
						this.GetPropertyInfo(new ScriptVariableType(this.kObjType.Name), commonTree6.Token, false, out basic_l_value_return.kType);
						if (basic_l_value_return.kType.ShadowVariableName.Length > 0)
						{
							((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).bisLocalAutoProperty = true;
							basic_l_value_return.sVarName = basic_l_value_return.kType.ShadowVariableName;
						}
						else
						{
							basic_l_value_return.sVarName = this.GenerateTempVariable(basic_l_value_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
							asSelfName = "self";
						}
					}
					else
					{
						this.CheckCanBeLValue(commonTree6.Text, commonTree6.Token);
						basic_l_value_return.kType = this.GetVariableType(((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, commonTree6.Token);
						((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).bisProperty = false;
						basic_l_value_return.sVarName = commonTree6.Text;
					}
					basic_l_value_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (basic_l_value_return != null) ? basic_l_value_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					if (((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).bisProperty && !((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).bisLocalAutoProperty)
					{
						CommonTree commonTree7 = (CommonTree)this.adaptor.GetNilNode();
						commonTree7 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(21, commonTree6.Token, "propset"), commonTree7);
						this.adaptor.AddChild(commonTree7, (CommonTree)this.adaptor.Create(38, commonTree6.Token, asSelfName));
						this.adaptor.AddChild(commonTree7, rewriteRuleNodeStream2.NextNode());
						this.adaptor.AddChild(commonTree7, (CommonTree)this.adaptor.Create(38, commonTree6.Token, basic_l_value_return.sVarName));
						this.adaptor.AddChild(commonTree, commonTree7);
					}
					else if (((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).bisProperty && ((PapyrusTypeWalker.basic_l_value_scope)this.basic_l_value_stack.Peek()).bisLocalAutoProperty)
					{
						CommonTree commonTree8 = (CommonTree)this.adaptor.GetNilNode();
						commonTree8 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(38, commonTree6.Token, basic_l_value_return.sVarName), commonTree8);
						this.adaptor.AddChild(commonTree, commonTree8);
					}
					else
					{
						this.adaptor.AddChild(commonTree, rewriteRuleNodeStream2.NextNode());
					}
					basic_l_value_return.Tree = commonTree;
					break;
				}
				}
				basic_l_value_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.basic_l_value_stack.Pop();
			}
			return basic_l_value_return;
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x00051ECC File Offset: 0x000500CC
		public PapyrusTypeWalker.expression_return expression()
		{
			PapyrusTypeWalker.expression_return expression_return = new PapyrusTypeWalker.expression_return();
			expression_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token OR");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule and_expression");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 65)
				{
					num2 = 1;
				}
				else
				{
					if (num != 11 && (num < 15 || num > 16) && (num != 22 && num != 38 && num != 62 && (num < 66 || num > 82)) && (num < 90 || num > 93))
					{
						NoViableAltException ex = new NoViableAltException("", 31, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 65, PapyrusTypeWalker.FOLLOW_OR_in_expression1204);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_expression1208);
					PapyrusTypeWalker.expression_return expression_return2 = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_and_expression_in_expression1212);
					PapyrusTypeWalker.and_expression_return and_expression_return = this.and_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(and_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.MarkTempVarAsUnused((expression_return2 != null) ? expression_return2.kType : null, (expression_return2 != null) ? expression_return2.sVarName : null, ((expression_return2 != null) ? ((CommonTree)expression_return2.Start) : null).Token);
					this.MarkTempVarAsUnused((and_expression_return != null) ? and_expression_return.kType : null, (and_expression_return != null) ? and_expression_return.sVarName : null, ((and_expression_return != null) ? ((CommonTree)and_expression_return.Start) : null).Token);
					expression_return.kType = new ScriptVariableType("bool");
					expression_return.sVarName = this.GenerateTempVariable(expression_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					expression_return.kVarToken = new CommonToken(commonTree3.Token);
					expression_return.kVarToken.Type = 38;
					expression_return.kVarToken.Text = expression_return.sVarName;
					expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (expression_return != null) ? expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (and_expression_return != null) ? and_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (expression_return2 != null) ? expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree4 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
					this.adaptor.AddChild(commonTree4, (CommonTree)this.adaptor.Create(38, commonTree3.Token, expression_return.sVarName));
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
					this.adaptor.AddChild(commonTree, commonTree4);
					expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_and_expression_in_expression1246);
					PapyrusTypeWalker.and_expression_return and_expression_return2 = this.and_expression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, and_expression_return2.Tree);
					expression_return.kType = ((and_expression_return2 != null) ? and_expression_return2.kType : null);
					expression_return.sVarName = ((and_expression_return2 != null) ? and_expression_return2.sVarName : null);
					expression_return.kVarToken = ((and_expression_return2 != null) ? and_expression_return2.kVarToken : null);
					break;
				}
				}
				expression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return expression_return;
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x000523C0 File Offset: 0x000505C0
		public PapyrusTypeWalker.and_expression_return and_expression()
		{
			PapyrusTypeWalker.and_expression_return and_expression_return = new PapyrusTypeWalker.and_expression_return();
			and_expression_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token AND");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule bool_expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule and_expression");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 66)
				{
					num2 = 1;
				}
				else
				{
					if (num != 11 && (num < 15 || num > 16) && (num != 22 && num != 38 && num != 62 && (num < 67 || num > 82)) && (num < 90 || num > 93))
					{
						NoViableAltException ex = new NoViableAltException("", 32, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 66, PapyrusTypeWalker.FOLLOW_AND_in_and_expression1268);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_and_expression_in_and_expression1272);
					PapyrusTypeWalker.and_expression_return and_expression_return2 = this.and_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(and_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_bool_expression_in_and_expression1276);
					PapyrusTypeWalker.bool_expression_return bool_expression_return = this.bool_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(bool_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.MarkTempVarAsUnused((and_expression_return2 != null) ? and_expression_return2.kType : null, (and_expression_return2 != null) ? and_expression_return2.sVarName : null, ((and_expression_return2 != null) ? ((CommonTree)and_expression_return2.Start) : null).Token);
					this.MarkTempVarAsUnused((bool_expression_return != null) ? bool_expression_return.kType : null, (bool_expression_return != null) ? bool_expression_return.sVarName : null, ((bool_expression_return != null) ? ((CommonTree)bool_expression_return.Start) : null).Token);
					and_expression_return.kType = new ScriptVariableType("bool");
					and_expression_return.sVarName = this.GenerateTempVariable(and_expression_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					and_expression_return.kVarToken = new CommonToken(commonTree3.Token);
					and_expression_return.kVarToken.Type = 38;
					and_expression_return.kVarToken.Text = and_expression_return.sVarName;
					and_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (and_expression_return != null) ? and_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (and_expression_return2 != null) ? and_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree4 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
					this.adaptor.AddChild(commonTree4, (CommonTree)this.adaptor.Create(38, commonTree3.Token, and_expression_return.sVarName));
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
					this.adaptor.AddChild(commonTree, commonTree4);
					and_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_bool_expression_in_and_expression1310);
					PapyrusTypeWalker.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, bool_expression_return2.Tree);
					and_expression_return.kType = ((bool_expression_return2 != null) ? bool_expression_return2.kType : null);
					and_expression_return.sVarName = ((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null);
					and_expression_return.kVarToken = ((bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null);
					break;
				}
				}
				and_expression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return and_expression_return;
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x000528B4 File Offset: 0x00050AB4
		public PapyrusTypeWalker.bool_expression_return bool_expression()
		{
			this.bool_expression_stack.Push(new PapyrusTypeWalker.bool_expression_scope());
			PapyrusTypeWalker.bool_expression_return bool_expression_return = new PapyrusTypeWalker.bool_expression_return();
			bool_expression_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token GT");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token LT");
			RewriteRuleNodeStream rewriteRuleNodeStream3 = new RewriteRuleNodeStream(this.adaptor, "token EQ");
			RewriteRuleNodeStream rewriteRuleNodeStream4 = new RewriteRuleNodeStream(this.adaptor, "token LTE");
			RewriteRuleNodeStream rewriteRuleNodeStream5 = new RewriteRuleNodeStream(this.adaptor, "token GTE");
			RewriteRuleNodeStream rewriteRuleNodeStream6 = new RewriteRuleNodeStream(this.adaptor, "token NE");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule bool_expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule add_expression");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num <= 16)
				{
					if (num != 11)
					{
						switch (num)
						{
						case 15:
						case 16:
							break;
						default:
							goto IL_1CE;
						}
					}
				}
				else if (num != 22 && num != 38)
				{
					switch (num)
					{
					case 62:
					case 73:
					case 74:
					case 75:
					case 76:
					case 77:
					case 78:
					case 79:
					case 80:
					case 81:
					case 82:
					case 90:
					case 91:
					case 92:
					case 93:
						break;
					case 63:
					case 64:
					case 65:
					case 66:
					case 83:
					case 84:
					case 85:
					case 86:
					case 87:
					case 88:
					case 89:
						goto IL_1CE;
					case 67:
						num2 = 1;
						goto IL_1E6;
					case 68:
						num2 = 2;
						goto IL_1E6;
					case 69:
						num2 = 3;
						goto IL_1E6;
					case 70:
						num2 = 4;
						goto IL_1E6;
					case 71:
						num2 = 5;
						goto IL_1E6;
					case 72:
						num2 = 6;
						goto IL_1E6;
					default:
						goto IL_1CE;
					}
				}
				num2 = 7;
				goto IL_1E6;
				IL_1CE:
				NoViableAltException ex = new NoViableAltException("", 33, 0, this.input);
				throw ex;
				IL_1E6:
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 67, PapyrusTypeWalker.FOLLOW_EQ_in_bool_expression1337);
					rewriteRuleNodeStream3.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_bool_expression_in_bool_expression1341);
					PapyrusTypeWalker.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_bool_expression1345);
					PapyrusTypeWalker.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree3.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree4 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream3.NextNode(), commonTree4);
					this.adaptor.AddChild(commonTree4, (CommonTree)this.adaptor.Create(38, commonTree3.Token, bool_expression_return.sVarName));
					this.adaptor.AddChild(commonTree4, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree);
					this.adaptor.AddChild(commonTree4, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
					this.adaptor.AddChild(commonTree, commonTree4);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child2 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 68, PapyrusTypeWalker.FOLLOW_NE_in_bool_expression1384);
					rewriteRuleNodeStream6.Add(commonTree5);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_bool_expression_in_bool_expression1388);
					PapyrusTypeWalker.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_bool_expression1392);
					PapyrusTypeWalker.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child2);
					this.HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree5.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree6 = (CommonTree)this.adaptor.GetNilNode();
					commonTree6 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream6.NextNode(), commonTree6);
					this.adaptor.AddChild(commonTree6, (CommonTree)this.adaptor.Create(38, commonTree5.Token, bool_expression_return.sVarName));
					this.adaptor.AddChild(commonTree6, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree);
					this.adaptor.AddChild(commonTree6, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					this.adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream6.NextTree());
					this.adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream5.NextTree());
					this.adaptor.AddChild(commonTree, commonTree6);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child3 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree7 = (CommonTree)this.Match(this.input, 69, PapyrusTypeWalker.FOLLOW_GT_in_bool_expression1431);
					rewriteRuleNodeStream.Add(commonTree7);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_bool_expression_in_bool_expression1435);
					PapyrusTypeWalker.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_bool_expression1439);
					PapyrusTypeWalker.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child3);
					this.HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree7.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream7 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream8 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree8 = (CommonTree)this.adaptor.GetNilNode();
					commonTree8 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree8);
					this.adaptor.AddChild(commonTree8, (CommonTree)this.adaptor.Create(38, commonTree7.Token, bool_expression_return.sVarName));
					this.adaptor.AddChild(commonTree8, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree);
					this.adaptor.AddChild(commonTree8, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					this.adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream8.NextTree());
					this.adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream7.NextTree());
					this.adaptor.AddChild(commonTree, commonTree8);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 4:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree9 = (CommonTree)this.Match(this.input, 70, PapyrusTypeWalker.FOLLOW_LT_in_bool_expression1478);
					rewriteRuleNodeStream2.Add(commonTree9);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_bool_expression_in_bool_expression1482);
					PapyrusTypeWalker.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_bool_expression1486);
					PapyrusTypeWalker.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child4);
					this.HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree9.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream9 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream10 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree10 = (CommonTree)this.adaptor.GetNilNode();
					commonTree10 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree10);
					this.adaptor.AddChild(commonTree10, (CommonTree)this.adaptor.Create(38, commonTree9.Token, bool_expression_return.sVarName));
					this.adaptor.AddChild(commonTree10, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree);
					this.adaptor.AddChild(commonTree10, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					this.adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream10.NextTree());
					this.adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream9.NextTree());
					this.adaptor.AddChild(commonTree, commonTree10);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 5:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child5 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree11 = (CommonTree)this.Match(this.input, 71, PapyrusTypeWalker.FOLLOW_GTE_in_bool_expression1525);
					rewriteRuleNodeStream5.Add(commonTree11);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_bool_expression_in_bool_expression1529);
					PapyrusTypeWalker.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_bool_expression1533);
					PapyrusTypeWalker.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child5);
					this.HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree11.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream11 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream12 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree12 = (CommonTree)this.adaptor.GetNilNode();
					commonTree12 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream5.NextNode(), commonTree12);
					this.adaptor.AddChild(commonTree12, (CommonTree)this.adaptor.Create(38, commonTree11.Token, bool_expression_return.sVarName));
					this.adaptor.AddChild(commonTree12, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree);
					this.adaptor.AddChild(commonTree12, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					this.adaptor.AddChild(commonTree12, rewriteRuleSubtreeStream12.NextTree());
					this.adaptor.AddChild(commonTree12, rewriteRuleSubtreeStream11.NextTree());
					this.adaptor.AddChild(commonTree, commonTree12);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 6:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child6 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree13 = (CommonTree)this.Match(this.input, 72, PapyrusTypeWalker.FOLLOW_LTE_in_bool_expression1572);
					rewriteRuleNodeStream4.Add(commonTree13);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_bool_expression_in_bool_expression1576);
					PapyrusTypeWalker.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_bool_expression1580);
					PapyrusTypeWalker.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child6);
					this.HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree13.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream13 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream14 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree14 = (CommonTree)this.adaptor.GetNilNode();
					commonTree14 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream4.NextNode(), commonTree14);
					this.adaptor.AddChild(commonTree14, (CommonTree)this.adaptor.Create(38, commonTree13.Token, bool_expression_return.sVarName));
					this.adaptor.AddChild(commonTree14, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kaTree);
					this.adaptor.AddChild(commonTree14, ((PapyrusTypeWalker.bool_expression_scope)this.bool_expression_stack.Peek()).kbTree);
					this.adaptor.AddChild(commonTree14, rewriteRuleSubtreeStream14.NextTree());
					this.adaptor.AddChild(commonTree14, rewriteRuleSubtreeStream13.NextTree());
					this.adaptor.AddChild(commonTree, commonTree14);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 7:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_bool_expression1618);
					PapyrusTypeWalker.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, add_expression_return2.Tree);
					bool_expression_return.kType = ((add_expression_return2 != null) ? add_expression_return2.kType : null);
					bool_expression_return.sVarName = ((add_expression_return2 != null) ? add_expression_return2.sVarName : null);
					bool_expression_return.kVarToken = ((add_expression_return2 != null) ? add_expression_return2.kVarToken : null);
					break;
				}
				}
				bool_expression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.bool_expression_stack.Pop();
			}
			return bool_expression_return;
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x00053F0C File Offset: 0x0005210C
		public PapyrusTypeWalker.add_expression_return add_expression()
		{
			this.add_expression_stack.Push(new PapyrusTypeWalker.add_expression_scope());
			PapyrusTypeWalker.add_expression_return add_expression_return = new PapyrusTypeWalker.add_expression_return();
			add_expression_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token PLUS");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token MINUS");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule add_expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule mult_expression");
			((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).bisInt = false;
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num <= 22)
				{
					if (num != 11)
					{
						switch (num)
						{
						case 15:
						case 16:
							break;
						default:
							if (num != 22)
							{
								goto IL_16A;
							}
							break;
						}
					}
				}
				else if (num != 38)
				{
					switch (num)
					{
					case 62:
					case 75:
					case 76:
					case 77:
					case 78:
					case 79:
					case 80:
					case 81:
					case 82:
						break;
					case 63:
					case 64:
					case 65:
					case 66:
					case 67:
					case 68:
					case 69:
					case 70:
					case 71:
					case 72:
						goto IL_16A;
					case 73:
						num2 = 1;
						goto IL_182;
					case 74:
						num2 = 2;
						goto IL_182;
					default:
						switch (num)
						{
						case 90:
						case 91:
						case 92:
						case 93:
							break;
						default:
							goto IL_16A;
						}
						break;
					}
				}
				num2 = 3;
				goto IL_182;
				IL_16A:
				NoViableAltException ex = new NoViableAltException("", 34, 0, this.input);
				throw ex;
				IL_182:
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 73, PapyrusTypeWalker.FOLLOW_PLUS_in_add_expression1650);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_add_expression1654);
					PapyrusTypeWalker.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(add_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_mult_expression_in_add_expression1658);
					PapyrusTypeWalker.mult_expression_return mult_expression_return = this.mult_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(mult_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.HandleAddSubtractExpression((add_expression_return2 != null) ? add_expression_return2.sVarName : null, (add_expression_return2 != null) ? add_expression_return2.kType : null, (add_expression_return2 != null) ? add_expression_return2.kVarToken : null, (mult_expression_return != null) ? mult_expression_return.sVarName : null, (mult_expression_return != null) ? mult_expression_return.kType : null, (mult_expression_return != null) ? mult_expression_return.kVarToken : null, commonTree3.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).bisConcat, out ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).bisInt, out add_expression_return.sVarName, out add_expression_return.kType, out add_expression_return.kVarToken, out ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kbTree);
					add_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (add_expression_return != null) ? add_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (add_expression_return2 != null) ? add_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					if (((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).bisConcat)
					{
						CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
						commonTree4 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(36, commonTree3.Token, "STRCAT"), commonTree4);
						this.adaptor.AddChild(commonTree4, (CommonTree)this.adaptor.Create(38, commonTree3.Token, add_expression_return.sVarName));
						this.adaptor.AddChild(commonTree4, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree4, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
						this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree, commonTree4);
					}
					else if (((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).bisInt)
					{
						CommonTree commonTree5 = (CommonTree)this.adaptor.GetNilNode();
						commonTree5 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(26, commonTree3.Token), commonTree5);
						this.adaptor.AddChild(commonTree5, (CommonTree)this.adaptor.Create(38, commonTree3.Token, add_expression_return.sVarName));
						this.adaptor.AddChild(commonTree5, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree5, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream4.NextTree());
						this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree, commonTree5);
					}
					else
					{
						CommonTree commonTree6 = (CommonTree)this.adaptor.GetNilNode();
						commonTree6 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(27, commonTree3.Token), commonTree6);
						this.adaptor.AddChild(commonTree6, (CommonTree)this.adaptor.Create(38, commonTree3.Token, add_expression_return.sVarName));
						this.adaptor.AddChild(commonTree6, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree6, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream4.NextTree());
						this.adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree, commonTree6);
					}
					add_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child2 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree7 = (CommonTree)this.Match(this.input, 74, PapyrusTypeWalker.FOLLOW_MINUS_in_add_expression1748);
					rewriteRuleNodeStream2.Add(commonTree7);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_add_expression_in_add_expression1752);
					PapyrusTypeWalker.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(add_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_mult_expression_in_add_expression1756);
					PapyrusTypeWalker.mult_expression_return mult_expression_return = this.mult_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(mult_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child2);
					this.HandleAddSubtractExpression((add_expression_return2 != null) ? add_expression_return2.sVarName : null, (add_expression_return2 != null) ? add_expression_return2.kType : null, (add_expression_return2 != null) ? add_expression_return2.kVarToken : null, (mult_expression_return != null) ? mult_expression_return.sVarName : null, (mult_expression_return != null) ? mult_expression_return.kType : null, (mult_expression_return != null) ? mult_expression_return.kVarToken : null, commonTree7.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).bisConcat, out ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).bisInt, out add_expression_return.sVarName, out add_expression_return.kType, out add_expression_return.kVarToken, out ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kbTree);
					add_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (add_expression_return != null) ? add_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (add_expression_return2 != null) ? add_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					if (((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).bisInt)
					{
						CommonTree commonTree8 = (CommonTree)this.adaptor.GetNilNode();
						commonTree8 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(28, commonTree7.Token), commonTree8);
						this.adaptor.AddChild(commonTree8, (CommonTree)this.adaptor.Create(38, commonTree7.Token, add_expression_return.sVarName));
						this.adaptor.AddChild(commonTree8, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree8, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream6.NextTree());
						this.adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream5.NextTree());
						this.adaptor.AddChild(commonTree, commonTree8);
					}
					else
					{
						CommonTree commonTree9 = (CommonTree)this.adaptor.GetNilNode();
						commonTree9 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(29, commonTree7.Token), commonTree9);
						this.adaptor.AddChild(commonTree9, (CommonTree)this.adaptor.Create(38, commonTree7.Token, add_expression_return.sVarName));
						this.adaptor.AddChild(commonTree9, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree9, ((PapyrusTypeWalker.add_expression_scope)this.add_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree9, rewriteRuleSubtreeStream6.NextTree());
						this.adaptor.AddChild(commonTree9, rewriteRuleSubtreeStream5.NextTree());
						this.adaptor.AddChild(commonTree, commonTree9);
					}
					add_expression_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_mult_expression_in_add_expression1820);
					PapyrusTypeWalker.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, mult_expression_return2.Tree);
					add_expression_return.kType = ((mult_expression_return2 != null) ? mult_expression_return2.kType : null);
					add_expression_return.sVarName = ((mult_expression_return2 != null) ? mult_expression_return2.sVarName : null);
					add_expression_return.kVarToken = ((mult_expression_return2 != null) ? mult_expression_return2.kVarToken : null);
					break;
				}
				}
				add_expression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.add_expression_stack.Pop();
			}
			return add_expression_return;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x00054BAC File Offset: 0x00052DAC
		public PapyrusTypeWalker.mult_expression_return mult_expression()
		{
			this.mult_expression_stack.Push(new PapyrusTypeWalker.mult_expression_scope());
			PapyrusTypeWalker.mult_expression_return mult_expression_return = new PapyrusTypeWalker.mult_expression_return();
			mult_expression_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token MULT");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token MOD");
			RewriteRuleNodeStream rewriteRuleNodeStream3 = new RewriteRuleNodeStream(this.adaptor, "token DIVIDE");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule unary_expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule mult_expression");
			((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).bisInt = false;
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num <= 22)
				{
					if (num != 11)
					{
						switch (num)
						{
						case 15:
						case 16:
							break;
						default:
							if (num != 22)
							{
								goto IL_15F;
							}
							break;
						}
					}
				}
				else if (num != 38 && num != 62)
				{
					switch (num)
					{
					case 75:
						num2 = 1;
						goto IL_177;
					case 76:
						num2 = 2;
						goto IL_177;
					case 77:
						num2 = 3;
						goto IL_177;
					case 78:
					case 79:
					case 80:
					case 81:
					case 82:
					case 90:
					case 91:
					case 92:
					case 93:
						break;
					case 83:
					case 84:
					case 85:
					case 86:
					case 87:
					case 88:
					case 89:
						goto IL_15F;
					default:
						goto IL_15F;
					}
				}
				num2 = 4;
				goto IL_177;
				IL_15F:
				NoViableAltException ex = new NoViableAltException("", 35, 0, this.input);
				throw ex;
				IL_177:
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 75, PapyrusTypeWalker.FOLLOW_MULT_in_mult_expression1852);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_mult_expression_in_mult_expression1856);
					PapyrusTypeWalker.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(mult_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_unary_expression_in_mult_expression1860);
					PapyrusTypeWalker.unary_expression_return unary_expression_return = this.unary_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(unary_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.HandleMultDivideExpression((mult_expression_return2 != null) ? mult_expression_return2.sVarName : null, (mult_expression_return2 != null) ? mult_expression_return2.kType : null, (mult_expression_return2 != null) ? mult_expression_return2.kVarToken : null, (unary_expression_return != null) ? unary_expression_return.sVarName : null, (unary_expression_return != null) ? unary_expression_return.kType : null, (unary_expression_return != null) ? unary_expression_return.kVarToken : null, commonTree3.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).bisInt, out mult_expression_return.sVarName, out mult_expression_return.kType, out mult_expression_return.kVarToken, out ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kbTree);
					mult_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (unary_expression_return != null) ? unary_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (mult_expression_return2 != null) ? mult_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					if (((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).bisInt)
					{
						CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
						commonTree4 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(30, commonTree3.Token), commonTree4);
						this.adaptor.AddChild(commonTree4, (CommonTree)this.adaptor.Create(38, commonTree3.Token, mult_expression_return.sVarName));
						this.adaptor.AddChild(commonTree4, ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree4, ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
						this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree, commonTree4);
					}
					else
					{
						CommonTree commonTree5 = (CommonTree)this.adaptor.GetNilNode();
						commonTree5 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(31, commonTree3.Token), commonTree5);
						this.adaptor.AddChild(commonTree5, (CommonTree)this.adaptor.Create(38, commonTree3.Token, mult_expression_return.sVarName));
						this.adaptor.AddChild(commonTree5, ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree5, ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream4.NextTree());
						this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
						this.adaptor.AddChild(commonTree, commonTree5);
					}
					mult_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child2 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 76, PapyrusTypeWalker.FOLLOW_DIVIDE_in_mult_expression1925);
					rewriteRuleNodeStream3.Add(commonTree6);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_mult_expression_in_mult_expression1929);
					PapyrusTypeWalker.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(mult_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_unary_expression_in_mult_expression1933);
					PapyrusTypeWalker.unary_expression_return unary_expression_return = this.unary_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(unary_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child2);
					this.HandleMultDivideExpression((mult_expression_return2 != null) ? mult_expression_return2.sVarName : null, (mult_expression_return2 != null) ? mult_expression_return2.kType : null, (mult_expression_return2 != null) ? mult_expression_return2.kVarToken : null, (unary_expression_return != null) ? unary_expression_return.sVarName : null, (unary_expression_return != null) ? unary_expression_return.kType : null, (unary_expression_return != null) ? unary_expression_return.kVarToken : null, commonTree6.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).bisInt, out mult_expression_return.sVarName, out mult_expression_return.kType, out mult_expression_return.kVarToken, out ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kaTree, out ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kbTree);
					mult_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (unary_expression_return != null) ? unary_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (mult_expression_return2 != null) ? mult_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					if (((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).bisInt)
					{
						CommonTree commonTree7 = (CommonTree)this.adaptor.GetNilNode();
						commonTree7 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(32, commonTree6.Token), commonTree7);
						this.adaptor.AddChild(commonTree7, (CommonTree)this.adaptor.Create(38, commonTree6.Token, mult_expression_return.sVarName));
						this.adaptor.AddChild(commonTree7, ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree7, ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream6.NextTree());
						this.adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream5.NextTree());
						this.adaptor.AddChild(commonTree, commonTree7);
					}
					else
					{
						CommonTree commonTree8 = (CommonTree)this.adaptor.GetNilNode();
						commonTree8 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(33, commonTree6.Token), commonTree8);
						this.adaptor.AddChild(commonTree8, (CommonTree)this.adaptor.Create(38, commonTree6.Token, mult_expression_return.sVarName));
						this.adaptor.AddChild(commonTree8, ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kaTree);
						this.adaptor.AddChild(commonTree8, ((PapyrusTypeWalker.mult_expression_scope)this.mult_expression_stack.Peek()).kbTree);
						this.adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream6.NextTree());
						this.adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream5.NextTree());
						this.adaptor.AddChild(commonTree, commonTree8);
					}
					mult_expression_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child3 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree9 = (CommonTree)this.Match(this.input, 77, PapyrusTypeWalker.FOLLOW_MOD_in_mult_expression1998);
					rewriteRuleNodeStream2.Add(commonTree9);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_mult_expression_in_mult_expression2002);
					PapyrusTypeWalker.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(mult_expression_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_unary_expression_in_mult_expression2006);
					PapyrusTypeWalker.unary_expression_return unary_expression_return = this.unary_expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(unary_expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child3);
					this.MarkTempVarAsUnused((mult_expression_return2 != null) ? mult_expression_return2.kType : null, (mult_expression_return2 != null) ? mult_expression_return2.sVarName : null, ((mult_expression_return2 != null) ? ((CommonTree)mult_expression_return2.Start) : null).Token);
					this.MarkTempVarAsUnused((unary_expression_return != null) ? unary_expression_return.kType : null, (unary_expression_return != null) ? unary_expression_return.sVarName : null, ((unary_expression_return != null) ? ((CommonTree)unary_expression_return.Start) : null).Token);
					mult_expression_return.kType = this.CheckModType((mult_expression_return2 != null) ? mult_expression_return2.kType : null, (unary_expression_return != null) ? unary_expression_return.kType : null, commonTree9.Token);
					mult_expression_return.sVarName = this.GenerateTempVariable(mult_expression_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					mult_expression_return.kVarToken = new CommonToken(commonTree9.Token);
					mult_expression_return.kVarToken.Type = 38;
					mult_expression_return.kVarToken.Text = mult_expression_return.sVarName;
					mult_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream7 = new RewriteRuleSubtreeStream(this.adaptor, "rule b", (unary_expression_return != null) ? unary_expression_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream8 = new RewriteRuleSubtreeStream(this.adaptor, "rule a", (mult_expression_return2 != null) ? mult_expression_return2.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree10 = (CommonTree)this.adaptor.GetNilNode();
					commonTree10 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree10);
					this.adaptor.AddChild(commonTree10, (CommonTree)this.adaptor.Create(38, commonTree9.Token, mult_expression_return.sVarName));
					this.adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream8.NextTree());
					this.adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream7.NextTree());
					this.adaptor.AddChild(commonTree, commonTree10);
					mult_expression_return.Tree = commonTree;
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_unary_expression_in_mult_expression2040);
					PapyrusTypeWalker.unary_expression_return unary_expression_return2 = this.unary_expression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, unary_expression_return2.Tree);
					mult_expression_return.kType = ((unary_expression_return2 != null) ? unary_expression_return2.kType : null);
					mult_expression_return.sVarName = ((unary_expression_return2 != null) ? unary_expression_return2.sVarName : null);
					mult_expression_return.kVarToken = ((unary_expression_return2 != null) ? unary_expression_return2.kVarToken : null);
					break;
				}
				}
				mult_expression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.mult_expression_stack.Pop();
			}
			return mult_expression_return;
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x00055A4C File Offset: 0x00053C4C
		public PapyrusTypeWalker.unary_expression_return unary_expression()
		{
			this.unary_expression_stack.Push(new PapyrusTypeWalker.unary_expression_scope());
			PapyrusTypeWalker.unary_expression_return unary_expression_return = new PapyrusTypeWalker.unary_expression_return();
			unary_expression_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token UNARY_MINUS");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token NOT");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule cast_atom");
			((PapyrusTypeWalker.unary_expression_scope)this.unary_expression_stack.Peek()).bisInt = false;
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num <= 22)
				{
					if (num != 11)
					{
						switch (num)
						{
						case 15:
							break;
						case 16:
							num2 = 1;
							goto IL_13C;
						default:
							if (num != 22)
							{
								goto IL_124;
							}
							break;
						}
					}
				}
				else if (num != 38 && num != 62)
				{
					switch (num)
					{
					case 78:
						num2 = 2;
						goto IL_13C;
					case 79:
					case 80:
					case 81:
					case 82:
					case 90:
					case 91:
					case 92:
					case 93:
						break;
					case 83:
					case 84:
					case 85:
					case 86:
					case 87:
					case 88:
					case 89:
						goto IL_124;
					default:
						goto IL_124;
					}
				}
				num2 = 3;
				goto IL_13C;
				IL_124:
				NoViableAltException ex = new NoViableAltException("", 36, 0, this.input);
				throw ex;
				IL_13C:
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 16, PapyrusTypeWalker.FOLLOW_UNARY_MINUS_in_unary_expression2072);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_cast_atom_in_unary_expression2074);
					PapyrusTypeWalker.cast_atom_return cast_atom_return = this.cast_atom();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(cast_atom_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.MarkTempVarAsUnused((cast_atom_return != null) ? cast_atom_return.kType : null, (cast_atom_return != null) ? cast_atom_return.sVarName : null, ((cast_atom_return != null) ? ((CommonTree)cast_atom_return.Start) : null).Token);
					unary_expression_return.kType = this.CheckNegationType((cast_atom_return != null) ? cast_atom_return.kType : null, commonTree3.Token);
					unary_expression_return.sVarName = this.GenerateTempVariable(unary_expression_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					((PapyrusTypeWalker.unary_expression_scope)this.unary_expression_stack.Peek()).bisInt = (unary_expression_return.kType.VarType == "int");
					unary_expression_return.kVarToken = new CommonToken(commonTree3.Token);
					unary_expression_return.kVarToken.Type = 38;
					unary_expression_return.kVarToken.Text = unary_expression_return.sVarName;
					unary_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (unary_expression_return != null) ? unary_expression_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					if (((PapyrusTypeWalker.unary_expression_scope)this.unary_expression_stack.Peek()).bisInt)
					{
						CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
						commonTree4 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(34, commonTree3.Token), commonTree4);
						this.adaptor.AddChild(commonTree4, (CommonTree)this.adaptor.Create(38, commonTree3.Token, unary_expression_return.sVarName));
						this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
						this.adaptor.AddChild(commonTree, commonTree4);
					}
					else
					{
						CommonTree commonTree5 = (CommonTree)this.adaptor.GetNilNode();
						commonTree5 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(35, commonTree3.Token), commonTree5);
						this.adaptor.AddChild(commonTree5, (CommonTree)this.adaptor.Create(38, commonTree3.Token, unary_expression_return.sVarName));
						this.adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
						this.adaptor.AddChild(commonTree, commonTree5);
					}
					unary_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child2 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 78, PapyrusTypeWalker.FOLLOW_NOT_in_unary_expression2123);
					rewriteRuleNodeStream2.Add(commonTree6);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_cast_atom_in_unary_expression2125);
					PapyrusTypeWalker.cast_atom_return cast_atom_return2 = this.cast_atom();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(cast_atom_return2.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child2);
					this.MarkTempVarAsUnused((cast_atom_return2 != null) ? cast_atom_return2.kType : null, (cast_atom_return2 != null) ? cast_atom_return2.sVarName : null, ((cast_atom_return2 != null) ? ((CommonTree)cast_atom_return2.Start) : null).Token);
					unary_expression_return.kType = new ScriptVariableType("bool");
					unary_expression_return.sVarName = this.GenerateTempVariable(unary_expression_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					unary_expression_return.kVarToken = new CommonToken(commonTree6.Token);
					unary_expression_return.kVarToken.Type = 38;
					unary_expression_return.kVarToken.Text = unary_expression_return.sVarName;
					unary_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (unary_expression_return != null) ? unary_expression_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree7 = (CommonTree)this.adaptor.GetNilNode();
					commonTree7 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree7);
					this.adaptor.AddChild(commonTree7, (CommonTree)this.adaptor.Create(38, commonTree6.Token, unary_expression_return.sVarName));
					this.adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream.NextTree());
					this.adaptor.AddChild(commonTree, commonTree7);
					unary_expression_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_cast_atom_in_unary_expression2155);
					PapyrusTypeWalker.cast_atom_return cast_atom_return3 = this.cast_atom();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, cast_atom_return3.Tree);
					unary_expression_return.kType = ((cast_atom_return3 != null) ? cast_atom_return3.kType : null);
					unary_expression_return.sVarName = ((cast_atom_return3 != null) ? cast_atom_return3.sVarName : null);
					unary_expression_return.kVarToken = ((cast_atom_return3 != null) ? cast_atom_return3.kVarToken : null);
					break;
				}
				}
				unary_expression_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.unary_expression_stack.Pop();
			}
			return unary_expression_return;
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x0005623C File Offset: 0x0005443C
		public PapyrusTypeWalker.cast_atom_return cast_atom()
		{
			PapyrusTypeWalker.cast_atom_return cast_atom_return = new PapyrusTypeWalker.cast_atom_return();
			cast_atom_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token AS");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule dot_atom");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule type");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 79)
				{
					num2 = 1;
				}
				else
				{
					if (num != 11 && num != 15 && num != 22 && num != 38 && num != 62 && (num < 80 || num > 82) && (num < 90 || num > 93))
					{
						NoViableAltException ex = new NoViableAltException("", 37, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 79, PapyrusTypeWalker.FOLLOW_AS_in_cast_atom2177);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_dot_atom_in_cast_atom2179);
					PapyrusTypeWalker.dot_atom_return dot_atom_return = this.dot_atom();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(dot_atom_return.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_type_in_cast_atom2181);
					PapyrusTypeWalker.type_return type_return = this.type();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(type_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					if (((dot_atom_return != null) ? dot_atom_return.sVarName : null) == "")
					{
						this.OnError(string.Format("{0} is not a variable", ((dot_atom_return != null) ? dot_atom_return.kVarToken : null).Text), ((dot_atom_return != null) ? dot_atom_return.kVarToken : null).Line, ((dot_atom_return != null) ? dot_atom_return.kVarToken : null).CharPositionInLine);
					}
					this.MarkTempVarAsUnused((dot_atom_return != null) ? dot_atom_return.kType : null, (dot_atom_return != null) ? dot_atom_return.sVarName : null, ((dot_atom_return != null) ? ((CommonTree)dot_atom_return.Start) : null).Token);
					cast_atom_return.kType = this.CheckCast((dot_atom_return != null) ? dot_atom_return.kType : null, (type_return != null) ? type_return.kType : null, commonTree3.Token);
					cast_atom_return.sVarName = this.GenerateTempVariable(cast_atom_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					cast_atom_return.kVarToken = new CommonToken(commonTree3.Token);
					cast_atom_return.kVarToken.Type = 38;
					cast_atom_return.kVarToken.Text = cast_atom_return.sVarName;
					cast_atom_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (cast_atom_return != null) ? cast_atom_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree4 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
					this.adaptor.AddChild(commonTree4, (CommonTree)this.adaptor.Create(38, commonTree3.Token, cast_atom_return.sVarName));
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
					this.adaptor.AddChild(commonTree, commonTree4);
					cast_atom_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_dot_atom_in_cast_atom2211);
					PapyrusTypeWalker.dot_atom_return dot_atom_return2 = this.dot_atom();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, dot_atom_return2.Tree);
					if (((dot_atom_return2 != null) ? dot_atom_return2.sVarName : null) == "")
					{
						this.OnError(string.Format("{0} is not a variable", ((dot_atom_return2 != null) ? dot_atom_return2.kVarToken : null).Text), ((dot_atom_return2 != null) ? dot_atom_return2.kVarToken : null).Line, ((dot_atom_return2 != null) ? dot_atom_return2.kVarToken : null).CharPositionInLine);
					}
					cast_atom_return.kType = ((dot_atom_return2 != null) ? dot_atom_return2.kType : null);
					cast_atom_return.sVarName = ((dot_atom_return2 != null) ? dot_atom_return2.sVarName : null);
					cast_atom_return.kVarToken = ((dot_atom_return2 != null) ? dot_atom_return2.kVarToken : null);
					break;
				}
				}
				cast_atom_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return cast_atom_return;
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x00056780 File Offset: 0x00054980
		public PapyrusTypeWalker.dot_atom_return dot_atom()
		{
			PapyrusTypeWalker.dot_atom_return dot_atom_return = new PapyrusTypeWalker.dot_atom_return();
			dot_atom_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num <= 22)
				{
					if (num != 11 && num != 15 && num != 22)
					{
						goto IL_AD;
					}
				}
				else
				{
					if (num > 62)
					{
						switch (num)
						{
						case 80:
						case 82:
							goto IL_A3;
						case 81:
							break;
						default:
							switch (num)
							{
							case 90:
							case 91:
							case 92:
							case 93:
								break;
							default:
								goto IL_AD;
							}
							break;
						}
						num2 = 3;
						goto IL_C5;
					}
					if (num != 38)
					{
						if (num != 62)
						{
							goto IL_AD;
						}
						num2 = 1;
						goto IL_C5;
					}
				}
				IL_A3:
				num2 = 2;
				goto IL_C5;
				IL_AD:
				NoViableAltException ex = new NoViableAltException("", 38, 0, this.input);
				throw ex;
				IL_C5:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree treeNode = (CommonTree)this.Match(this.input, 62, PapyrusTypeWalker.FOLLOW_DOT_in_dot_atom2233);
					CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_dot_atom_in_dot_atom2237);
					PapyrusTypeWalker.dot_atom_return dot_atom_return2 = this.dot_atom();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, dot_atom_return2.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_array_func_or_id_in_dot_atom2241);
					PapyrusTypeWalker.array_func_or_id_return array_func_or_id_return = this.array_func_or_id((dot_atom_return2 != null) ? dot_atom_return2.kType : null, (dot_atom_return2 != null) ? dot_atom_return2.sVarName : null);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, array_func_or_id_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, commonTree3);
					this.MarkTempVarAsUnused((dot_atom_return2 != null) ? dot_atom_return2.kType : null, (dot_atom_return2 != null) ? dot_atom_return2.sVarName : null, ((dot_atom_return2 != null) ? ((CommonTree)dot_atom_return2.Start) : null).Token);
					dot_atom_return.kType = ((array_func_or_id_return != null) ? array_func_or_id_return.kType : null);
					dot_atom_return.sVarName = ((array_func_or_id_return != null) ? array_func_or_id_return.sVarName : null);
					dot_atom_return.kVarToken = ((array_func_or_id_return != null) ? array_func_or_id_return.kVarToken : null);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_array_atom_in_dot_atom2254);
					PapyrusTypeWalker.array_atom_return array_atom_return = this.array_atom(new ScriptVariableType("none"), "!first");
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, array_atom_return.Tree);
					dot_atom_return.kType = ((array_atom_return != null) ? array_atom_return.kType : null);
					dot_atom_return.sVarName = ((array_atom_return != null) ? array_atom_return.sVarName : null);
					dot_atom_return.kVarToken = ((array_atom_return != null) ? array_atom_return.kVarToken : null);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_constant_in_dot_atom2267);
					PapyrusTypeWalker.constant_return constant_return = this.constant();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, constant_return.Tree);
					dot_atom_return.kType = ((constant_return != null) ? constant_return.kType : null);
					dot_atom_return.sVarName = ((constant_return != null) ? ((CommonTree)constant_return.Start) : null).Text;
					dot_atom_return.kVarToken = ((constant_return != null) ? constant_return.kVarToken : null);
					break;
				}
				}
				dot_atom_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return dot_atom_return;
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x00056BF8 File Offset: 0x00054DF8
		public PapyrusTypeWalker.array_atom_return array_atom(ScriptVariableType akSelfType, string asSelfName)
		{
			this.array_atom_stack.Push(new PapyrusTypeWalker.array_atom_scope());
			PapyrusTypeWalker.array_atom_return array_atom_return = new PapyrusTypeWalker.array_atom_return();
			array_atom_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token ARRAYGET");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule atom");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 22)
				{
					num2 = 1;
				}
				else
				{
					if (num != 11 && num != 15 && num != 38 && num != 80 && num != 82)
					{
						NoViableAltException ex = new NoViableAltException("", 39, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree el = (CommonTree)this.Match(this.input, 22, PapyrusTypeWalker.FOLLOW_ARRAYGET_in_array_atom2294);
					rewriteRuleNodeStream.Add(el);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_atom_in_array_atom2296);
					PapyrusTypeWalker.atom_return atom_return = this.atom(akSelfType, asSelfName);
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(atom_return.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_array_atom2299);
					PapyrusTypeWalker.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.HandleArrayElementExpression((atom_return != null) ? atom_return.sVarName : null, (atom_return != null) ? atom_return.kType : null, (atom_return != null) ? atom_return.kVarToken : null, (expression_return != null) ? expression_return.sVarName : null, (expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.kVarToken : null, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out array_atom_return.sVarName, out array_atom_return.kType, out array_atom_return.kVarToken, out ((PapyrusTypeWalker.array_atom_scope)this.array_atom_stack.Peek()).kautoCastTree);
					array_atom_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (array_atom_return != null) ? array_atom_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
					commonTree3 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree3);
					this.adaptor.AddChild(commonTree3, (CommonTree)this.adaptor.Create(38, array_atom_return.kVarToken));
					this.adaptor.AddChild(commonTree3, (CommonTree)this.adaptor.Create(38, (atom_return != null) ? atom_return.kVarToken : null));
					this.adaptor.AddChild(commonTree3, ((PapyrusTypeWalker.array_atom_scope)this.array_atom_stack.Peek()).kautoCastTree);
					this.adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream2.NextTree());
					this.adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream.NextTree());
					this.adaptor.AddChild(commonTree, commonTree3);
					array_atom_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_atom_in_array_atom2332);
					PapyrusTypeWalker.atom_return atom_return2 = this.atom(akSelfType, asSelfName);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, atom_return2.Tree);
					array_atom_return.kType = ((atom_return2 != null) ? atom_return2.kType : null);
					array_atom_return.sVarName = ((atom_return2 != null) ? atom_return2.sVarName : null);
					array_atom_return.kVarToken = ((atom_return2 != null) ? atom_return2.kVarToken : null);
					break;
				}
				}
				array_atom_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.array_atom_stack.Pop();
			}
			return array_atom_return;
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x000570D0 File Offset: 0x000552D0
		public PapyrusTypeWalker.atom_return atom(ScriptVariableType akSelfType, string asSelfName)
		{
			PapyrusTypeWalker.atom_return atom_return = new PapyrusTypeWalker.atom_return();
			atom_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			CommonTree commonTree2 = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token NEW");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token INTEGER");
			RewriteRuleNodeStream rewriteRuleNodeStream3 = new RewriteRuleNodeStream(this.adaptor, "token BASETYPE");
			RewriteRuleNodeStream rewriteRuleNodeStream4 = new RewriteRuleNodeStream(this.adaptor, "token ID");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num <= 15)
				{
					if (num != 11)
					{
						if (num != 15)
						{
							goto IL_CA;
						}
						num2 = 1;
						goto IL_E2;
					}
				}
				else if (num != 38)
				{
					switch (num)
					{
					case 80:
						num2 = 2;
						goto IL_E2;
					case 81:
						goto IL_CA;
					case 82:
						break;
					default:
						goto IL_CA;
					}
				}
				num2 = 3;
				goto IL_E2;
				IL_CA:
				NoViableAltException ex = new NoViableAltException("", 41, 0, this.input);
				throw ex;
				IL_E2:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree3 = (CommonTree)this.input.LT(1);
					CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree3 = (CommonTree)this.input.LT(1);
					CommonTree treeNode = (CommonTree)this.Match(this.input, 15, PapyrusTypeWalker.FOLLOW_PAREXPR_in_atom2356);
					CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
					commonTree4 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree4);
					this.Match(this.input, 2, null);
					commonTree3 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_atom2358);
					PapyrusTypeWalker.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree4, expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, commonTree4);
					atom_return.kType = ((expression_return != null) ? expression_return.kType : null);
					atom_return.sVarName = ((expression_return != null) ? expression_return.sVarName : null);
					atom_return.kVarToken = ((expression_return != null) ? expression_return.kVarToken : null);
					break;
				}
				case 2:
				{
					CommonTree commonTree3 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree3 = (CommonTree)this.input.LT(1);
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 80, PapyrusTypeWalker.FOLLOW_NEW_in_atom2371);
					rewriteRuleNodeStream.Add(commonTree5);
					this.Match(this.input, 2, null);
					int num3 = this.input.LA(1);
					int num4;
					if (num3 == 55)
					{
						num4 = 1;
					}
					else
					{
						if (num3 != 38)
						{
							NoViableAltException ex2 = new NoViableAltException("", 40, 0, this.input);
							throw ex2;
						}
						num4 = 2;
					}
					switch (num4)
					{
					case 1:
						commonTree3 = (CommonTree)this.input.LT(1);
						commonTree2 = (CommonTree)this.Match(this.input, 55, PapyrusTypeWalker.FOLLOW_BASETYPE_in_atom2376);
						rewriteRuleNodeStream3.Add(commonTree2);
						break;
					case 2:
						commonTree3 = (CommonTree)this.input.LT(1);
						commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_atom2382);
						rewriteRuleNodeStream4.Add(commonTree2);
						break;
					}
					commonTree3 = (CommonTree)this.input.LT(1);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 81, PapyrusTypeWalker.FOLLOW_INTEGER_in_atom2385);
					rewriteRuleNodeStream2.Add(commonTree6);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.CheckArrayNew(commonTree2.Token, commonTree6.Token);
					atom_return.kType = new ScriptVariableType(commonTree2.Text + "[]");
					atom_return.sVarName = this.GenerateTempVariable(atom_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					atom_return.kVarToken = new CommonToken(commonTree5.Token);
					atom_return.kVarToken.Type = 38;
					atom_return.kVarToken.Text = atom_return.sVarName;
					atom_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (atom_return != null) ? atom_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree7 = (CommonTree)this.adaptor.GetNilNode();
					commonTree7 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree7);
					this.adaptor.AddChild(commonTree7, rewriteRuleNodeStream2.NextNode());
					this.adaptor.AddChild(commonTree7, (CommonTree)this.adaptor.Create(38, atom_return.kVarToken));
					this.adaptor.AddChild(commonTree, commonTree7);
					atom_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree3 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_func_or_id_in_atom2411);
					PapyrusTypeWalker.func_or_id_return func_or_id_return = this.func_or_id(akSelfType, asSelfName);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, func_or_id_return.Tree);
					atom_return.kType = ((func_or_id_return != null) ? func_or_id_return.kType : null);
					atom_return.sVarName = ((func_or_id_return != null) ? func_or_id_return.sVarName : null);
					atom_return.kVarToken = ((func_or_id_return != null) ? func_or_id_return.kVarToken : null);
					break;
				}
				}
				atom_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex3)
			{
				this.ReportError(ex3);
				this.Recover(this.input, ex3);
			}
			return atom_return;
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x000576E0 File Offset: 0x000558E0
		public PapyrusTypeWalker.array_func_or_id_return array_func_or_id(ScriptVariableType akSelfType, string asSelfName)
		{
			this.array_func_or_id_stack.Push(new PapyrusTypeWalker.array_func_or_id_scope());
			PapyrusTypeWalker.array_func_or_id_return array_func_or_id_return = new PapyrusTypeWalker.array_func_or_id_return();
			array_func_or_id_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token ARRAYGET");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(this.adaptor, "rule func_or_id");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 22)
				{
					num2 = 1;
				}
				else
				{
					if (num != 11 && num != 38 && num != 82)
					{
						NoViableAltException ex = new NoViableAltException("", 42, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree el = (CommonTree)this.Match(this.input, 22, PapyrusTypeWalker.FOLLOW_ARRAYGET_in_array_func_or_id2439);
					rewriteRuleNodeStream.Add(el);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_func_or_id_in_array_func_or_id2441);
					PapyrusTypeWalker.func_or_id_return func_or_id_return = this.func_or_id(akSelfType, asSelfName);
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(func_or_id_return.Tree);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_array_func_or_id2444);
					PapyrusTypeWalker.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.HandleArrayElementExpression((func_or_id_return != null) ? func_or_id_return.sVarName : null, (func_or_id_return != null) ? func_or_id_return.kType : null, (func_or_id_return != null) ? func_or_id_return.kVarToken : null, (expression_return != null) ? expression_return.sVarName : null, (expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.kVarToken : null, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, out array_func_or_id_return.sVarName, out array_func_or_id_return.kType, out array_func_or_id_return.kVarToken, out ((PapyrusTypeWalker.array_func_or_id_scope)this.array_func_or_id_stack.Peek()).kautoCastTree);
					array_func_or_id_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (array_func_or_id_return != null) ? array_func_or_id_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
					commonTree3 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree3);
					this.adaptor.AddChild(commonTree3, (CommonTree)this.adaptor.Create(38, array_func_or_id_return.kVarToken));
					this.adaptor.AddChild(commonTree3, (CommonTree)this.adaptor.Create(38, (func_or_id_return != null) ? func_or_id_return.kVarToken : null));
					this.adaptor.AddChild(commonTree3, ((PapyrusTypeWalker.array_func_or_id_scope)this.array_func_or_id_stack.Peek()).kautoCastTree);
					this.adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream2.NextTree());
					this.adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream.NextTree());
					this.adaptor.AddChild(commonTree, commonTree3);
					array_func_or_id_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_func_or_id_in_array_func_or_id2477);
					PapyrusTypeWalker.func_or_id_return func_or_id_return2 = this.func_or_id(akSelfType, asSelfName);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, func_or_id_return2.Tree);
					array_func_or_id_return.kType = ((func_or_id_return2 != null) ? func_or_id_return2.kType : null);
					array_func_or_id_return.sVarName = ((func_or_id_return2 != null) ? func_or_id_return2.sVarName : null);
					array_func_or_id_return.kVarToken = ((func_or_id_return2 != null) ? func_or_id_return2.kVarToken : null);
					break;
				}
				}
				array_func_or_id_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.array_func_or_id_stack.Pop();
			}
			return array_func_or_id_return;
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x00057BAC File Offset: 0x00055DAC
		public PapyrusTypeWalker.func_or_id_return func_or_id(ScriptVariableType akSelfType, string asSelfName)
		{
			this.func_or_id_stack.Push(new PapyrusTypeWalker.func_or_id_scope());
			PapyrusTypeWalker.func_or_id_return func_or_id_return = new PapyrusTypeWalker.func_or_id_return();
			func_or_id_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token ID");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token LENGTH");
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num != 11)
				{
					if (num != 38)
					{
						if (num != 82)
						{
							NoViableAltException ex = new NoViableAltException("", 43, 0, this.input);
							throw ex;
						}
						num2 = 3;
					}
					else
					{
						num2 = 2;
					}
				}
				else
				{
					num2 = 1;
				}
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_function_call_in_func_or_id2504);
					PapyrusTypeWalker.function_call_return function_call_return = this.function_call(akSelfType, asSelfName);
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, function_call_return.Tree);
					func_or_id_return.kType = ((function_call_return != null) ? function_call_return.kType : null);
					func_or_id_return.sVarName = ((function_call_return != null) ? function_call_return.sVarName : null);
					func_or_id_return.kVarToken = ((function_call_return != null) ? function_call_return.kVarToken : null);
					break;
				}
				case 2:
				{
					CommonTree commonTree3 = (CommonTree)this.input.LT(1);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_func_or_id2516);
					rewriteRuleNodeStream.Add(commonTree4);
					ScriptObjectType knownType = this.GetKnownType(commonTree4.Text);
					if (knownType != null)
					{
						if (asSelfName != "!first")
						{
							this.OnError(string.Format("the type name {0} cannot be used as a property", commonTree4.Text), commonTree4.Line, commonTree4.CharPositionInLine);
						}
						func_or_id_return.kType = new ScriptVariableType(knownType.Name);
						func_or_id_return.sVarName = "";
						func_or_id_return.kVarToken = commonTree4.Token;
					}
					else if (asSelfName != "!first")
					{
						if (asSelfName == "")
						{
							this.OnError("a property cannot be used directly on a type, it must be used on a variable", commonTree4.Line, commonTree4.CharPositionInLine);
						}
						else if (asSelfName.ToLowerInvariant() == "parent")
						{
							this.OnError("a property cannot be used on the special parent variable, use the property directly instead", commonTree4.Line, commonTree4.CharPositionInLine);
						}
						this.GetPropertyInfo(akSelfType, commonTree4.Token, true, out func_or_id_return.kType);
						((PapyrusTypeWalker.func_or_id_scope)this.func_or_id_stack.Peek()).bisProperty = true;
						func_or_id_return.sVarName = this.GenerateTempVariable(func_or_id_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
						func_or_id_return.kVarToken = new CommonToken(commonTree4.Token);
						func_or_id_return.kVarToken.Text = func_or_id_return.sVarName;
					}
					else if (!((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType.bGlobal && this.IsLocalProperty(commonTree4.Text))
					{
						((PapyrusTypeWalker.func_or_id_scope)this.func_or_id_stack.Peek()).bisProperty = true;
						this.GetPropertyInfo(new ScriptVariableType(this.kObjType.Name), commonTree4.Token, true, out func_or_id_return.kType);
						if (func_or_id_return.kType.ShadowVariableName.Length > 0)
						{
							((PapyrusTypeWalker.func_or_id_scope)this.func_or_id_stack.Peek()).bisLocalAutoProperty = true;
							func_or_id_return.sVarName = func_or_id_return.kType.ShadowVariableName;
						}
						else
						{
							func_or_id_return.sVarName = this.GenerateTempVariable(func_or_id_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
							asSelfName = "self";
						}
						func_or_id_return.kVarToken = new CommonToken(commonTree4.Token);
						func_or_id_return.kVarToken.Text = func_or_id_return.sVarName;
					}
					else
					{
						func_or_id_return.kType = this.GetVariableType(((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, commonTree4.Token);
						((PapyrusTypeWalker.func_or_id_scope)this.func_or_id_stack.Peek()).bisProperty = false;
						func_or_id_return.sVarName = commonTree4.Text;
						func_or_id_return.kVarToken = commonTree4.Token;
					}
					func_or_id_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (func_or_id_return != null) ? func_or_id_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					if (((PapyrusTypeWalker.func_or_id_scope)this.func_or_id_stack.Peek()).bisProperty && !((PapyrusTypeWalker.func_or_id_scope)this.func_or_id_stack.Peek()).bisLocalAutoProperty)
					{
						CommonTree commonTree5 = (CommonTree)this.adaptor.GetNilNode();
						commonTree5 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(20, commonTree4.Token, "propget"), commonTree5);
						this.adaptor.AddChild(commonTree5, (CommonTree)this.adaptor.Create(38, commonTree4.Token, asSelfName));
						this.adaptor.AddChild(commonTree5, rewriteRuleNodeStream.NextNode());
						this.adaptor.AddChild(commonTree5, (CommonTree)this.adaptor.Create(38, commonTree4.Token, func_or_id_return.sVarName));
						this.adaptor.AddChild(commonTree, commonTree5);
					}
					else if (((PapyrusTypeWalker.func_or_id_scope)this.func_or_id_stack.Peek()).bisProperty && ((PapyrusTypeWalker.func_or_id_scope)this.func_or_id_stack.Peek()).bisLocalAutoProperty)
					{
						CommonTree commonTree6 = (CommonTree)this.adaptor.GetNilNode();
						commonTree6 = (CommonTree)this.adaptor.BecomeRoot((CommonTree)this.adaptor.Create(38, func_or_id_return.kVarToken, func_or_id_return.sVarName), commonTree6);
						this.adaptor.AddChild(commonTree, commonTree6);
					}
					else
					{
						this.adaptor.AddChild(commonTree, rewriteRuleNodeStream.NextNode());
					}
					func_or_id_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					CommonTree commonTree7 = (CommonTree)this.input.LT(1);
					CommonTree commonTree8 = (CommonTree)this.Match(this.input, 82, PapyrusTypeWalker.FOLLOW_LENGTH_in_func_or_id2571);
					rewriteRuleNodeStream2.Add(commonTree8);
					if (asSelfName == "!first")
					{
						this.OnError("length must be called on an array", commonTree8.Line, commonTree8.CharPositionInLine);
					}
					else if (!akSelfType.IsArray)
					{
						this.OnError(string.Format("cannot get the length of {0} as it isn't an array", asSelfName), commonTree8.Line, commonTree8.CharPositionInLine);
					}
					func_or_id_return.kType = new ScriptVariableType("int");
					func_or_id_return.sVarName = this.GenerateTempVariable(func_or_id_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					func_or_id_return.kVarToken = new CommonToken(commonTree8.Token);
					func_or_id_return.kVarToken.Type = 38;
					func_or_id_return.kVarToken.Text = func_or_id_return.sVarName;
					func_or_id_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (func_or_id_return != null) ? func_or_id_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree9 = (CommonTree)this.adaptor.GetNilNode();
					commonTree9 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree9);
					this.adaptor.AddChild(commonTree9, (CommonTree)this.adaptor.Create(38, commonTree8.Token, asSelfName));
					this.adaptor.AddChild(commonTree9, (CommonTree)this.adaptor.Create(38, commonTree8.Token, func_or_id_return.sVarName));
					this.adaptor.AddChild(commonTree, commonTree9);
					func_or_id_return.Tree = commonTree;
					break;
				}
				}
				func_or_id_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.func_or_id_stack.Pop();
			}
			return func_or_id_return;
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x00058418 File Offset: 0x00056618
		public PapyrusTypeWalker.return_stat_return return_stat()
		{
			this.return_stat_stack.Push(new PapyrusTypeWalker.return_stat_scope());
			PapyrusTypeWalker.return_stat_return return_stat_return = new PapyrusTypeWalker.return_stat_return();
			return_stat_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token RETURN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule expression");
			try
			{
				int num = this.input.LA(1);
				if (num != 83)
				{
					NoViableAltException ex = new NoViableAltException("", 44, 0, this.input);
					throw ex;
				}
				int num2 = this.input.LA(2);
				int num3;
				if (num2 == 2)
				{
					num3 = 1;
				}
				else
				{
					if (num2 != 3 && num2 != 5 && num2 != 11 && (num2 < 15 || num2 > 16) && (num2 != 22 && num2 != 38 && num2 != 41 && num2 != 62 && (num2 < 65 || num2 > 84)) && num2 != 88 && (num2 < 90 || num2 > 93))
					{
						NoViableAltException ex2 = new NoViableAltException("", 44, 1, this.input);
						throw ex2;
					}
					num3 = 2;
				}
				switch (num3)
				{
				case 1:
				{
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree child = (CommonTree)this.adaptor.GetNilNode();
					commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 83, PapyrusTypeWalker.FOLLOW_RETURN_in_return_stat2611);
					rewriteRuleNodeStream.Add(commonTree3);
					this.Match(this.input, 2, null);
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_return_stat2613);
					PapyrusTypeWalker.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					this.Match(this.input, 3, null);
					this.adaptor.AddChild(commonTree, child);
					this.CheckReturnType(((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType, (expression_return != null) ? expression_return.kType : null, commonTree3.Token);
					((PapyrusTypeWalker.return_stat_scope)this.return_stat_stack.Peek()).kautoCastTree = this.AutoCastReturn((expression_return != null) ? expression_return.kVarToken : null, (expression_return != null) ? expression_return.kType : null, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType.Name, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType.PropertyName, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars, commonTree3.Token);
					return_stat_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (return_stat_return != null) ? return_stat_return.Tree : null);
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.adaptor.GetNilNode();
					commonTree4 = (CommonTree)this.adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
					this.adaptor.AddChild(commonTree4, ((PapyrusTypeWalker.return_stat_scope)this.return_stat_stack.Peek()).kautoCastTree);
					this.adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
					this.adaptor.AddChild(commonTree, commonTree4);
					return_stat_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 83, PapyrusTypeWalker.FOLLOW_RETURN_in_return_stat2638);
					CommonTree child2 = (CommonTree)this.adaptor.DupNode(commonTree5);
					this.adaptor.AddChild(commonTree, child2);
					this.CheckReturnType(((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType, null, commonTree5.Token);
					break;
				}
				}
				return_stat_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex3)
			{
				this.ReportError(ex3);
				this.Recover(this.input, ex3);
			}
			finally
			{
				this.return_stat_stack.Pop();
			}
			return return_stat_return;
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x0005889C File Offset: 0x00056A9C
		public PapyrusTypeWalker.ifBlock_return ifBlock()
		{
			this.ifBlock_stack.Push(new PapyrusTypeWalker.ifBlock_scope());
			PapyrusTypeWalker.ifBlock_return ifBlock_return = new PapyrusTypeWalker.ifBlock_return();
			ifBlock_return.Start = this.input.LT(1);
			((PapyrusTypeWalker.ifBlock_scope)this.ifBlock_stack.Peek()).kchildScope = ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.Children[((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 84, PapyrusTypeWalker.FOLLOW_IF_in_ifBlock2672);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_ifBlock2674);
				PapyrusTypeWalker.expression_return expression_return = this.expression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, expression_return.Tree);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_codeBlock_in_ifBlock2676);
				PapyrusTypeWalker.codeBlock_return codeBlock_return = this.codeBlock(((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType, ((PapyrusTypeWalker.ifBlock_scope)this.ifBlock_stack.Peek()).kchildScope);
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				for (;;)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 == 86)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_elseIfBlock_in_ifBlock2679);
					PapyrusTypeWalker.elseIfBlock_return elseIfBlock_return = this.elseIfBlock();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, elseIfBlock_return.Tree);
				}
				int num4 = 2;
				int num5 = this.input.LA(1);
				if (num5 == 87)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_elseBlock_in_ifBlock2682);
					PapyrusTypeWalker.elseBlock_return elseBlock_return = this.elseBlock();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree3, elseBlock_return.Tree);
				}
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				this.MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, ((expression_return != null) ? ((CommonTree)expression_return.Start) : null).Token);
				ifBlock_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild++;
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.ifBlock_stack.Pop();
			}
			return ifBlock_return;
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x00058C5C File Offset: 0x00056E5C
		public PapyrusTypeWalker.elseIfBlock_return elseIfBlock()
		{
			this.elseIfBlock_stack.Push(new PapyrusTypeWalker.elseIfBlock_scope());
			PapyrusTypeWalker.elseIfBlock_return elseIfBlock_return = new PapyrusTypeWalker.elseIfBlock_return();
			elseIfBlock_return.Start = this.input.LT(1);
			((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild++;
			((PapyrusTypeWalker.elseIfBlock_scope)this.elseIfBlock_stack.Peek()).kchildScope = ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.Children[((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 86, PapyrusTypeWalker.FOLLOW_ELSEIF_in_elseIfBlock2711);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_elseIfBlock2713);
				PapyrusTypeWalker.expression_return expression_return = this.expression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, expression_return.Tree);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_codeBlock_in_elseIfBlock2715);
				PapyrusTypeWalker.codeBlock_return codeBlock_return = this.codeBlock(((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType, ((PapyrusTypeWalker.elseIfBlock_scope)this.elseIfBlock_stack.Peek()).kchildScope);
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				this.MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, ((expression_return != null) ? ((CommonTree)expression_return.Start) : null).Token);
				elseIfBlock_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.elseIfBlock_stack.Pop();
			}
			return elseIfBlock_return;
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x00058F34 File Offset: 0x00057134
		public PapyrusTypeWalker.elseBlock_return elseBlock()
		{
			this.elseBlock_stack.Push(new PapyrusTypeWalker.elseBlock_scope());
			PapyrusTypeWalker.elseBlock_return elseBlock_return = new PapyrusTypeWalker.elseBlock_return();
			elseBlock_return.Start = this.input.LT(1);
			((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild++;
			((PapyrusTypeWalker.elseBlock_scope)this.elseBlock_stack.Peek()).kchildScope = ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.Children[((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 87, PapyrusTypeWalker.FOLLOW_ELSE_in_elseBlock2744);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_codeBlock_in_elseBlock2746);
				PapyrusTypeWalker.codeBlock_return codeBlock_return = this.codeBlock(((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType, ((PapyrusTypeWalker.elseBlock_scope)this.elseBlock_stack.Peek()).kchildScope);
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				elseBlock_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.elseBlock_stack.Pop();
			}
			return elseBlock_return;
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x00059184 File Offset: 0x00057384
		public PapyrusTypeWalker.whileBlock_return whileBlock()
		{
			this.whileBlock_stack.Push(new PapyrusTypeWalker.whileBlock_scope());
			PapyrusTypeWalker.whileBlock_return whileBlock_return = new PapyrusTypeWalker.whileBlock_return();
			whileBlock_return.Start = this.input.LT(1);
			((PapyrusTypeWalker.whileBlock_scope)this.whileBlock_stack.Peek()).kchildScope = ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.Children[((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 88, PapyrusTypeWalker.FOLLOW_WHILE_in_whileBlock2777);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_whileBlock2779);
				PapyrusTypeWalker.expression_return expression_return = this.expression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, expression_return.Tree);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_codeBlock_in_whileBlock2781);
				PapyrusTypeWalker.codeBlock_return codeBlock_return = this.codeBlock(((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType, ((PapyrusTypeWalker.whileBlock_scope)this.whileBlock_stack.Peek()).kchildScope);
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				this.MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, ((expression_return != null) ? ((CommonTree)expression_return.Start) : null).Token);
				whileBlock_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
				((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild++;
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.whileBlock_stack.Pop();
			}
			return whileBlock_return;
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x0005945C File Offset: 0x0005765C
		public PapyrusTypeWalker.function_call_return function_call(ScriptVariableType akSelfType, string asSelfName)
		{
			this.function_call_stack.Push(new PapyrusTypeWalker.function_call_scope());
			PapyrusTypeWalker.function_call_return function_call_return = new PapyrusTypeWalker.function_call_return();
			function_call_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			RewriteRuleNodeStream rewriteRuleNodeStream = new RewriteRuleNodeStream(this.adaptor, "token CALL");
			RewriteRuleNodeStream rewriteRuleNodeStream2 = new RewriteRuleNodeStream(this.adaptor, "token ID");
			RewriteRuleNodeStream rewriteRuleNodeStream3 = new RewriteRuleNodeStream(this.adaptor, "token CALLPARAMS");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(this.adaptor, "rule parameters");
			((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).ktargetParamNames = new List<string>();
			((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTypes = new List<ScriptVariableType>();
			((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamVarNames = new List<string>();
			((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTokens = new List<IToken>();
			((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamExpressions = new List<CommonTree>();
			((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamAutoCasts = new List<CommonTree>();
			((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).bisGlobal = false;
			((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).bisArray = false;
			try
			{
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 11, PapyrusTypeWalker.FOLLOW_CALL_in_function_call2818);
				rewriteRuleNodeStream.Add(commonTree4);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree5 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_function_call2820);
				rewriteRuleNodeStream2.Add(commonTree5);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree child = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree el = (CommonTree)this.Match(this.input, 14, PapyrusTypeWalker.FOLLOW_CALLPARAMS_in_function_call2823);
				rewriteRuleNodeStream3.Add(el);
				if (this.input.LA(1) == 2)
				{
					this.Match(this.input, 2, null);
					int num = 2;
					int num2 = this.input.LA(1);
					if (num2 == 9)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 == 1)
					{
						commonTree2 = (CommonTree)this.input.LT(1);
						base.PushFollow(PapyrusTypeWalker.FOLLOW_parameters_in_function_call2825);
						PapyrusTypeWalker.parameters_return parameters_return = this.parameters();
						this.state.followingStackPointer--;
						rewriteRuleSubtreeStream.Add(parameters_return.Tree);
					}
					this.Match(this.input, 3, null);
				}
				this.adaptor.AddChild(commonTree3, child);
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				bool flag = false;
				if (asSelfName == "!first")
				{
					akSelfType = this.FindFunctionOwningType(commonTree5.Text, out flag, commonTree5.Token);
					asSelfName = "";
				}
				if (akSelfType != null)
				{
					if (asSelfName != "" && akSelfType.IsArray)
					{
						((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).bisArray = true;
						function_call_return.kType = this.CheckArrayFunctionCall(akSelfType, commonTree5.Text, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).ktargetParamNames, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTypes, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTokens, ref ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamExpressions, out ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamAutoCasts, commonTree5.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
					}
					else
					{
						((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).bisGlobal = this.IsGlobalFunction(akSelfType, commonTree5.Text);
						if (((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).bisGlobal)
						{
							function_call_return.kType = this.CheckGlobalFunctionCall(akSelfType, commonTree5.Text, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).ktargetParamNames, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTypes, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTokens, ref ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamExpressions, out ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamAutoCasts, commonTree5.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
							if (asSelfName != "")
							{
								this.OnError(string.Format("cannot call the global function {0} on the variable {1}, must call it alone or on a type", commonTree5.Text, asSelfName), commonTree4.Line, commonTree4.CharPositionInLine);
							}
							asSelfName = akSelfType.VarType;
						}
						else
						{
							function_call_return.kType = this.CheckMemberFunctionCall(akSelfType, commonTree5.Text, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).ktargetParamNames, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTypes, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTokens, ref ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamExpressions, out ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamAutoCasts, commonTree5.Token, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
							if (flag)
							{
								asSelfName = "self";
							}
							else if (asSelfName == "")
							{
								this.OnError(string.Format("cannot call the member function {0} alone or on a type, must call it on a variable", commonTree5.Text), commonTree4.Line, commonTree4.CharPositionInLine);
							}
						}
						if (flag && !((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).bisGlobal && ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType.bGlobal)
						{
							this.OnError(string.Format("cannot call member function {0} in global function {1} without a variable to call it on", commonTree5.Text, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kfunctionType.Name), commonTree4.Line, commonTree4.CharPositionInLine);
						}
					}
				}
				else
				{
					function_call_return.kType = new ScriptVariableType("none");
				}
				for (int i = 0; i < ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamVarNames.Count; i++)
				{
					this.MarkTempVarAsUnused(((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTypes[i], ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamVarNames[i], commonTree5.Token);
				}
				function_call_return.sVarName = this.GenerateTempVariable(function_call_return.kType, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope, ((PapyrusTypeWalker.codeBlock_scope)this.codeBlock_stack.Peek()).kTempVars);
				function_call_return.kVarToken = new CommonToken(commonTree5.Token);
				function_call_return.kVarToken.Text = function_call_return.sVarName;
				function_call_return.Tree = commonTree;
				new RewriteRuleSubtreeStream(this.adaptor, "rule retval", (function_call_return != null) ? function_call_return.Tree : null);
				commonTree = (CommonTree)this.adaptor.GetNilNode();
				this.adaptor.AddChild(commonTree, this.CreateCallTree(commonTree4.Token, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).bisGlobal, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).bisArray, asSelfName, commonTree5.Token, function_call_return.kType, function_call_return.sVarName, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamAutoCasts, ((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamExpressions));
				function_call_return.Tree = commonTree;
				function_call_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.function_call_stack.Pop();
			}
			return function_call_return;
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x00059D2C File Offset: 0x00057F2C
		public PapyrusTypeWalker.parameters_return parameters()
		{
			PapyrusTypeWalker.parameters_return parameters_return = new PapyrusTypeWalker.parameters_return();
			parameters_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				int num = 0;
				for (;;)
				{
					int num2 = 2;
					int num3 = this.input.LA(1);
					if (num3 == 9)
					{
						num2 = 1;
					}
					int num4 = num2;
					if (num4 != 1)
					{
						break;
					}
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_parameter_in_parameters2864);
					PapyrusTypeWalker.parameter_return parameter_return = this.parameter();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, parameter_return.Tree);
					num++;
				}
				if (num < 1)
				{
					EarlyExitException ex = new EarlyExitException(48, this.input);
					throw ex;
				}
				parameters_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return parameters_return;
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x00059E3C File Offset: 0x0005803C
		public PapyrusTypeWalker.parameter_return parameter()
		{
			PapyrusTypeWalker.parameter_return parameter_return = new PapyrusTypeWalker.parameter_return();
			parameter_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.adaptor.GetNilNode();
				CommonTree commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree3 = (CommonTree)this.adaptor.GetNilNode();
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree treeNode = (CommonTree)this.Match(this.input, 9, PapyrusTypeWalker.FOLLOW_PARAM_in_parameter2878);
				CommonTree newRoot = (CommonTree)this.adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)this.adaptor.BecomeRoot(newRoot, commonTree3);
				this.Match(this.input, 2, null);
				commonTree2 = (CommonTree)this.input.LT(1);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_parameter2880);
				CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree4);
				this.adaptor.AddChild(commonTree3, child);
				commonTree2 = (CommonTree)this.input.LT(1);
				base.PushFollow(PapyrusTypeWalker.FOLLOW_expression_in_parameter2882);
				PapyrusTypeWalker.expression_return expression_return = this.expression();
				this.state.followingStackPointer--;
				this.adaptor.AddChild(commonTree3, expression_return.Tree);
				this.Match(this.input, 3, null);
				this.adaptor.AddChild(commonTree, commonTree3);
				((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).ktargetParamNames.Add(commonTree4.Text);
				((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTypes.Add((expression_return != null) ? expression_return.kType : null);
				((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamVarNames.Add((expression_return != null) ? expression_return.sVarName : null);
				((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamTokens.Add((expression_return != null) ? expression_return.kVarToken : null);
				((PapyrusTypeWalker.function_call_scope)this.function_call_stack.Peek()).kparamExpressions.Add((expression_return != null) ? ((CommonTree)expression_return.Tree) : null);
				parameter_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return parameter_return;
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x0005A0E0 File Offset: 0x000582E0
		public PapyrusTypeWalker.constant_return constant()
		{
			PapyrusTypeWalker.constant_return constant_return = new PapyrusTypeWalker.constant_return();
			constant_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num != 81)
				{
					switch (num)
					{
					case 90:
						num2 = 2;
						goto IL_8C;
					case 91:
						num2 = 3;
						goto IL_8C;
					case 92:
						num2 = 4;
						goto IL_8C;
					case 93:
						break;
					default:
					{
						NoViableAltException ex = new NoViableAltException("", 49, 0, this.input);
						throw ex;
					}
					}
				}
				num2 = 1;
				IL_8C:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					base.PushFollow(PapyrusTypeWalker.FOLLOW_number_in_constant2905);
					PapyrusTypeWalker.number_return number_return = this.number();
					this.state.followingStackPointer--;
					this.adaptor.AddChild(commonTree, number_return.Tree);
					constant_return.kType = ((number_return != null) ? number_return.kType : null);
					constant_return.kVarToken = ((number_return != null) ? number_return.kVarToken : null);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree3 = (CommonTree)this.input.LT(1);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 90, PapyrusTypeWalker.FOLLOW_STRING_in_constant2916);
					CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree4);
					this.adaptor.AddChild(commonTree, child);
					constant_return.kType = new ScriptVariableType("string");
					constant_return.kVarToken = commonTree4.Token;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree5 = (CommonTree)this.input.LT(1);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 91, PapyrusTypeWalker.FOLLOW_BOOL_in_constant2927);
					CommonTree child2 = (CommonTree)this.adaptor.DupNode(commonTree6);
					this.adaptor.AddChild(commonTree, child2);
					constant_return.kType = new ScriptVariableType("bool");
					constant_return.kVarToken = commonTree6.Token;
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree7 = (CommonTree)this.input.LT(1);
					CommonTree commonTree8 = (CommonTree)this.Match(this.input, 92, PapyrusTypeWalker.FOLLOW_NONE_in_constant2938);
					CommonTree child3 = (CommonTree)this.adaptor.DupNode(commonTree8);
					this.adaptor.AddChild(commonTree, child3);
					constant_return.kType = new ScriptVariableType("none");
					constant_return.kVarToken = commonTree8.Token;
					break;
				}
				}
				constant_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return constant_return;
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x0005A3EC File Offset: 0x000585EC
		public PapyrusTypeWalker.number_return number()
		{
			PapyrusTypeWalker.number_return number_return = new PapyrusTypeWalker.number_return();
			number_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 81)
				{
					num2 = 1;
				}
				else
				{
					if (num != 93)
					{
						NoViableAltException ex = new NoViableAltException("", 50, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 81, PapyrusTypeWalker.FOLLOW_INTEGER_in_number2960);
					CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree3);
					this.adaptor.AddChild(commonTree, child);
					number_return.kType = new ScriptVariableType("int");
					number_return.kVarToken = commonTree3.Token;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.input.LT(1);
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 93, PapyrusTypeWalker.FOLLOW_FLOAT_in_number2971);
					CommonTree child2 = (CommonTree)this.adaptor.DupNode(commonTree5);
					this.adaptor.AddChild(commonTree, child2);
					number_return.kType = new ScriptVariableType("float");
					number_return.kVarToken = commonTree5.Token;
					break;
				}
				}
				number_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return number_return;
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x0005A5BC File Offset: 0x000587BC
		public PapyrusTypeWalker.type_return type()
		{
			PapyrusTypeWalker.type_return type_return = new PapyrusTypeWalker.type_return();
			type_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			try
			{
				int num = this.input.LA(1);
				int num3;
				if (num == 38)
				{
					int num2 = this.input.LA(2);
					if (num2 == 63)
					{
						num3 = 2;
					}
					else
					{
						if (num2 != 3 && num2 != 38)
						{
							NoViableAltException ex = new NoViableAltException("", 51, 1, this.input);
							throw ex;
						}
						num3 = 1;
					}
				}
				else
				{
					if (num != 55)
					{
						NoViableAltException ex2 = new NoViableAltException("", 51, 0, this.input);
						throw ex2;
					}
					int num4 = this.input.LA(2);
					if (num4 == 63)
					{
						num3 = 4;
					}
					else
					{
						if (num4 != 3 && num4 != 38)
						{
							NoViableAltException ex3 = new NoViableAltException("", 51, 2, this.input);
							throw ex3;
						}
						num3 = 3;
					}
				}
				switch (num3)
				{
				case 1:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree2 = (CommonTree)this.input.LT(1);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_type2993);
					CommonTree child = (CommonTree)this.adaptor.DupNode(commonTree3);
					this.adaptor.AddChild(commonTree, child);
					type_return.kType = new ScriptVariableType(commonTree3.Text);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree4 = (CommonTree)this.input.LT(1);
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 38, PapyrusTypeWalker.FOLLOW_ID_in_type3004);
					CommonTree child2 = (CommonTree)this.adaptor.DupNode(commonTree5);
					this.adaptor.AddChild(commonTree, child2);
					CommonTree commonTree6 = (CommonTree)this.input.LT(1);
					CommonTree treeNode = (CommonTree)this.Match(this.input, 63, PapyrusTypeWalker.FOLLOW_LBRACKET_in_type3006);
					CommonTree child3 = (CommonTree)this.adaptor.DupNode(treeNode);
					this.adaptor.AddChild(commonTree, child3);
					CommonTree commonTree7 = (CommonTree)this.input.LT(1);
					CommonTree treeNode2 = (CommonTree)this.Match(this.input, 64, PapyrusTypeWalker.FOLLOW_RBRACKET_in_type3008);
					CommonTree child4 = (CommonTree)this.adaptor.DupNode(treeNode2);
					this.adaptor.AddChild(commonTree, child4);
					type_return.kType = new ScriptVariableType(commonTree5.Text + "[]");
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree8 = (CommonTree)this.input.LT(1);
					CommonTree commonTree9 = (CommonTree)this.Match(this.input, 55, PapyrusTypeWalker.FOLLOW_BASETYPE_in_type3019);
					CommonTree child5 = (CommonTree)this.adaptor.DupNode(commonTree9);
					this.adaptor.AddChild(commonTree, child5);
					type_return.kType = new ScriptVariableType(commonTree9.Text);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)this.adaptor.GetNilNode();
					CommonTree commonTree10 = (CommonTree)this.input.LT(1);
					CommonTree commonTree11 = (CommonTree)this.Match(this.input, 55, PapyrusTypeWalker.FOLLOW_BASETYPE_in_type3030);
					CommonTree child6 = (CommonTree)this.adaptor.DupNode(commonTree11);
					this.adaptor.AddChild(commonTree, child6);
					CommonTree commonTree12 = (CommonTree)this.input.LT(1);
					CommonTree treeNode3 = (CommonTree)this.Match(this.input, 63, PapyrusTypeWalker.FOLLOW_LBRACKET_in_type3032);
					CommonTree child7 = (CommonTree)this.adaptor.DupNode(treeNode3);
					this.adaptor.AddChild(commonTree, child7);
					CommonTree commonTree13 = (CommonTree)this.input.LT(1);
					CommonTree treeNode4 = (CommonTree)this.Match(this.input, 64, PapyrusTypeWalker.FOLLOW_RBRACKET_in_type3034);
					CommonTree child8 = (CommonTree)this.adaptor.DupNode(treeNode4);
					this.adaptor.AddChild(commonTree, child8);
					type_return.kType = new ScriptVariableType(commonTree11.Text + "[]");
					break;
				}
				}
				type_return.Tree = (CommonTree)this.adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex4)
			{
				this.ReportError(ex4);
				this.Recover(this.input, ex4);
			}
			return type_return;
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x0005AA58 File Offset: 0x00058C58
		private void InitializeCyclicDFAs()
		{
		}

		// Token: 0x040007CF RID: 1999
		public const int FUNCTION = 6;

		// Token: 0x040007D0 RID: 2000
		public const int LT = 70;

		// Token: 0x040007D1 RID: 2001
		public const int WHILE = 88;

		// Token: 0x040007D2 RID: 2002
		public const int DIVEQUALS = 60;

		// Token: 0x040007D3 RID: 2003
		public const int MOD = 77;

		// Token: 0x040007D4 RID: 2004
		public const int PROPSET = 21;

		// Token: 0x040007D5 RID: 2005
		public const int NEW = 80;

		// Token: 0x040007D6 RID: 2006
		public const int DQUOTE = 98;

		// Token: 0x040007D7 RID: 2007
		public const int PARAM = 9;

		// Token: 0x040007D8 RID: 2008
		public const int EQUALS = 41;

		// Token: 0x040007D9 RID: 2009
		public const int NOT = 78;

		// Token: 0x040007DA RID: 2010
		public const int EOF = -1;

		// Token: 0x040007DB RID: 2011
		public const int FNEGATE = 35;

		// Token: 0x040007DC RID: 2012
		public const int LBRACKET = 63;

		// Token: 0x040007DD RID: 2013
		public const int USER_FLAGS = 18;

		// Token: 0x040007DE RID: 2014
		public const int RPAREN = 44;

		// Token: 0x040007DF RID: 2015
		public const int IMPORT = 42;

		// Token: 0x040007E0 RID: 2016
		public const int EOL = 94;

		// Token: 0x040007E1 RID: 2017
		public const int FADD = 27;

		// Token: 0x040007E2 RID: 2018
		public const int RETURN = 83;

		// Token: 0x040007E3 RID: 2019
		public const int ENDIF = 85;

		// Token: 0x040007E4 RID: 2020
		public const int VAR = 5;

		// Token: 0x040007E5 RID: 2021
		public const int ENDWHILE = 89;

		// Token: 0x040007E6 RID: 2022
		public const int EQ = 67;

		// Token: 0x040007E7 RID: 2023
		public const int IMULTIPLY = 30;

		// Token: 0x040007E8 RID: 2024
		public const int COMMENT = 104;

		// Token: 0x040007E9 RID: 2025
		public const int IDIVIDE = 32;

		// Token: 0x040007EA RID: 2026
		public const int DIVIDE = 76;

		// Token: 0x040007EB RID: 2027
		public const int NE = 68;

		// Token: 0x040007EC RID: 2028
		public const int SCRIPTNAME = 37;

		// Token: 0x040007ED RID: 2029
		public const int MINUSEQUALS = 58;

		// Token: 0x040007EE RID: 2030
		public const int ARRAYFIND = 24;

		// Token: 0x040007EF RID: 2031
		public const int RBRACE = 100;

		// Token: 0x040007F0 RID: 2032
		public const int ELSE = 87;

		// Token: 0x040007F1 RID: 2033
		public const int BOOL = 91;

		// Token: 0x040007F2 RID: 2034
		public const int NATIVE = 47;

		// Token: 0x040007F3 RID: 2035
		public const int FDIVIDE = 33;

		// Token: 0x040007F4 RID: 2036
		public const int UNARY_MINUS = 16;

		// Token: 0x040007F5 RID: 2037
		public const int MULT = 75;

		// Token: 0x040007F6 RID: 2038
		public const int ENDPROPERTY = 53;

		// Token: 0x040007F7 RID: 2039
		public const int CALLPARAMS = 14;

		// Token: 0x040007F8 RID: 2040
		public const int ALPHA = 95;

		// Token: 0x040007F9 RID: 2041
		public const int WS = 102;

		// Token: 0x040007FA RID: 2042
		public const int FMULTIPLY = 31;

		// Token: 0x040007FB RID: 2043
		public const int ARRAYSET = 23;

		// Token: 0x040007FC RID: 2044
		public const int PROPERTY = 54;

		// Token: 0x040007FD RID: 2045
		public const int AUTOREADONLY = 56;

		// Token: 0x040007FE RID: 2046
		public const int NONE = 92;

		// Token: 0x040007FF RID: 2047
		public const int OR = 65;

		// Token: 0x04000800 RID: 2048
		public const int PROPGET = 20;

		// Token: 0x04000801 RID: 2049
		public const int IADD = 26;

		// Token: 0x04000802 RID: 2050
		public const int PROPFUNC = 17;

		// Token: 0x04000803 RID: 2051
		public const int GT = 69;

		// Token: 0x04000804 RID: 2052
		public const int CALL = 11;

		// Token: 0x04000805 RID: 2053
		public const int INEGATE = 34;

		// Token: 0x04000806 RID: 2054
		public const int BASETYPE = 55;

		// Token: 0x04000807 RID: 2055
		public const int ENDEVENT = 48;

		// Token: 0x04000808 RID: 2056
		public const int MULTEQUALS = 59;

		// Token: 0x04000809 RID: 2057
		public const int CALLPARENT = 13;

		// Token: 0x0400080A RID: 2058
		public const int LBRACE = 99;

		// Token: 0x0400080B RID: 2059
		public const int GTE = 71;

		// Token: 0x0400080C RID: 2060
		public const int FLOAT = 93;

		// Token: 0x0400080D RID: 2061
		public const int ENDSTATE = 52;

		// Token: 0x0400080E RID: 2062
		public const int ID = 38;

		// Token: 0x0400080F RID: 2063
		public const int AND = 66;

		// Token: 0x04000810 RID: 2064
		public const int LTE = 72;

		// Token: 0x04000811 RID: 2065
		public const int LPAREN = 43;

		// Token: 0x04000812 RID: 2066
		public const int LENGTH = 82;

		// Token: 0x04000813 RID: 2067
		public const int IF = 84;

		// Token: 0x04000814 RID: 2068
		public const int CALLGLOBAL = 12;

		// Token: 0x04000815 RID: 2069
		public const int AS = 79;

		// Token: 0x04000816 RID: 2070
		public const int OBJECT = 4;

		// Token: 0x04000817 RID: 2071
		public const int COMMA = 49;

		// Token: 0x04000818 RID: 2072
		public const int PLUSEQUALS = 57;

		// Token: 0x04000819 RID: 2073
		public const int AUTO = 50;

		// Token: 0x0400081A RID: 2074
		public const int ISUBTRACT = 28;

		// Token: 0x0400081B RID: 2075
		public const int PLUS = 73;

		// Token: 0x0400081C RID: 2076
		public const int ENDFUNCTION = 45;

		// Token: 0x0400081D RID: 2077
		public const int DIGIT = 96;

		// Token: 0x0400081E RID: 2078
		public const int HEADER = 8;

		// Token: 0x0400081F RID: 2079
		public const int RBRACKET = 64;

		// Token: 0x04000820 RID: 2080
		public const int DOT = 62;

		// Token: 0x04000821 RID: 2081
		public const int FSUBTRACT = 29;

		// Token: 0x04000822 RID: 2082
		public const int STRCAT = 36;

		// Token: 0x04000823 RID: 2083
		public const int INTEGER = 81;

		// Token: 0x04000824 RID: 2084
		public const int STATE = 51;

		// Token: 0x04000825 RID: 2085
		public const int DOCSTRING = 40;

		// Token: 0x04000826 RID: 2086
		public const int WS_CHAR = 101;

		// Token: 0x04000827 RID: 2087
		public const int HEX_DIGIT = 97;

		// Token: 0x04000828 RID: 2088
		public const int ARRAYRFIND = 25;

		// Token: 0x04000829 RID: 2089
		public const int MINUS = 74;

		// Token: 0x0400082A RID: 2090
		public const int EVENT = 7;

		// Token: 0x0400082B RID: 2091
		public const int ARRAYGET = 22;

		// Token: 0x0400082C RID: 2092
		public const int ELSEIF = 86;

		// Token: 0x0400082D RID: 2093
		public const int AUTOPROP = 19;

		// Token: 0x0400082E RID: 2094
		public const int PAREXPR = 15;

		// Token: 0x0400082F RID: 2095
		public const int BLOCK = 10;

		// Token: 0x04000830 RID: 2096
		public const int EAT_EOL = 103;

		// Token: 0x04000831 RID: 2097
		public const int GLOBAL = 46;

		// Token: 0x04000832 RID: 2098
		public const int MODEQUALS = 61;

		// Token: 0x04000833 RID: 2099
		public const int EXTENDS = 39;

		// Token: 0x04000834 RID: 2100
		public const int STRING = 90;

		// Token: 0x04000835 RID: 2101
		private Dictionary<string, ScriptObjectType> kKnownTypes;

		// Token: 0x04000836 RID: 2102
		private Dictionary<string, ScriptObjectType> kImportedTypes = new Dictionary<string, ScriptObjectType>();

		// Token: 0x04000837 RID: 2103
		private Stack<string> kChildren;

		// Token: 0x04000838 RID: 2104
		private ScriptObjectType kObjType;

		// Token: 0x04000839 RID: 2105
		private Compiler kCompiler;

		// Token: 0x0400083A RID: 2106
		private int iCurVarSuffix;

		// Token: 0x0400083B RID: 2107
		private Dictionary<string, List<string>> kUnusedTempVarsByType = new Dictionary<string, List<string>>();

		// Token: 0x0400083C RID: 2108
		public static readonly string[] tokenNames = new string[]
		{
			"<invalid>",
			"<EOR>",
			"<DOWN>",
			"<UP>",
			"OBJECT",
			"VAR",
			"FUNCTION",
			"EVENT",
			"HEADER",
			"PARAM",
			"BLOCK",
			"CALL",
			"CALLGLOBAL",
			"CALLPARENT",
			"CALLPARAMS",
			"PAREXPR",
			"UNARY_MINUS",
			"PROPFUNC",
			"USER_FLAGS",
			"AUTOPROP",
			"PROPGET",
			"PROPSET",
			"ARRAYGET",
			"ARRAYSET",
			"ARRAYFIND",
			"ARRAYRFIND",
			"IADD",
			"FADD",
			"ISUBTRACT",
			"FSUBTRACT",
			"IMULTIPLY",
			"FMULTIPLY",
			"IDIVIDE",
			"FDIVIDE",
			"INEGATE",
			"FNEGATE",
			"STRCAT",
			"SCRIPTNAME",
			"ID",
			"EXTENDS",
			"DOCSTRING",
			"EQUALS",
			"IMPORT",
			"LPAREN",
			"RPAREN",
			"ENDFUNCTION",
			"GLOBAL",
			"NATIVE",
			"ENDEVENT",
			"COMMA",
			"AUTO",
			"STATE",
			"ENDSTATE",
			"ENDPROPERTY",
			"PROPERTY",
			"BASETYPE",
			"AUTOREADONLY",
			"PLUSEQUALS",
			"MINUSEQUALS",
			"MULTEQUALS",
			"DIVEQUALS",
			"MODEQUALS",
			"DOT",
			"LBRACKET",
			"RBRACKET",
			"OR",
			"AND",
			"EQ",
			"NE",
			"GT",
			"LT",
			"GTE",
			"LTE",
			"PLUS",
			"MINUS",
			"MULT",
			"DIVIDE",
			"MOD",
			"NOT",
			"AS",
			"NEW",
			"INTEGER",
			"LENGTH",
			"RETURN",
			"IF",
			"ENDIF",
			"ELSEIF",
			"ELSE",
			"WHILE",
			"ENDWHILE",
			"STRING",
			"BOOL",
			"NONE",
			"FLOAT",
			"EOL",
			"ALPHA",
			"DIGIT",
			"HEX_DIGIT",
			"DQUOTE",
			"LBRACE",
			"RBRACE",
			"WS_CHAR",
			"WS",
			"EAT_EOL",
			"COMMENT"
		};

		// Token: 0x0400083D RID: 2109
		protected ITreeAdaptor adaptor = new CommonTreeAdaptor();

		// Token: 0x0400083F RID: 2111
		protected StackList function_stack = new StackList();

		// Token: 0x04000840 RID: 2112
		protected StackList eventFunc_stack = new StackList();

		// Token: 0x04000841 RID: 2113
		protected StackList propertyBlock_stack = new StackList();

		// Token: 0x04000842 RID: 2114
		protected StackList codeBlock_stack = new StackList();

		// Token: 0x04000843 RID: 2115
		protected StackList statement_stack = new StackList();

		// Token: 0x04000844 RID: 2116
		protected StackList localDefinition_stack = new StackList();

		// Token: 0x04000845 RID: 2117
		protected StackList l_value_stack = new StackList();

		// Token: 0x04000846 RID: 2118
		protected StackList basic_l_value_stack = new StackList();

		// Token: 0x04000847 RID: 2119
		protected StackList bool_expression_stack = new StackList();

		// Token: 0x04000848 RID: 2120
		protected StackList add_expression_stack = new StackList();

		// Token: 0x04000849 RID: 2121
		protected StackList mult_expression_stack = new StackList();

		// Token: 0x0400084A RID: 2122
		protected StackList unary_expression_stack = new StackList();

		// Token: 0x0400084B RID: 2123
		protected StackList array_atom_stack = new StackList();

		// Token: 0x0400084C RID: 2124
		protected StackList array_func_or_id_stack = new StackList();

		// Token: 0x0400084D RID: 2125
		protected StackList func_or_id_stack = new StackList();

		// Token: 0x0400084E RID: 2126
		protected StackList return_stat_stack = new StackList();

		// Token: 0x0400084F RID: 2127
		protected StackList ifBlock_stack = new StackList();

		// Token: 0x04000850 RID: 2128
		protected StackList elseIfBlock_stack = new StackList();

		// Token: 0x04000851 RID: 2129
		protected StackList elseBlock_stack = new StackList();

		// Token: 0x04000852 RID: 2130
		protected StackList whileBlock_stack = new StackList();

		// Token: 0x04000853 RID: 2131
		protected StackList function_call_stack = new StackList();

		// Token: 0x04000854 RID: 2132
		public static readonly BitSet FOLLOW_OBJECT_in_script81 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000855 RID: 2133
		public static readonly BitSet FOLLOW_header_in_script83 = new BitSet(new ulong[]
		{
			20270596370202856UL
		});

		// Token: 0x04000856 RID: 2134
		public static readonly BitSet FOLLOW_definitionOrBlock_in_script85 = new BitSet(new ulong[]
		{
			20270596370202856UL
		});

		// Token: 0x04000857 RID: 2135
		public static readonly BitSet FOLLOW_ID_in_header101 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000858 RID: 2136
		public static readonly BitSet FOLLOW_USER_FLAGS_in_header103 = new BitSet(new ulong[]
		{
			1374389534728UL
		});

		// Token: 0x04000859 RID: 2137
		public static readonly BitSet FOLLOW_ID_in_header107 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x0400085A RID: 2138
		public static readonly BitSet FOLLOW_DOCSTRING_in_header110 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400085B RID: 2139
		public static readonly BitSet FOLLOW_fieldDefinition_in_definitionOrBlock130 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400085C RID: 2140
		public static readonly BitSet FOLLOW_import_obj_in_definitionOrBlock136 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400085D RID: 2141
		public static readonly BitSet FOLLOW_function_in_definitionOrBlock148 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400085E RID: 2142
		public static readonly BitSet FOLLOW_eventFunc_in_definitionOrBlock156 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400085F RID: 2143
		public static readonly BitSet FOLLOW_stateBlock_in_definitionOrBlock164 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000860 RID: 2144
		public static readonly BitSet FOLLOW_propertyBlock_in_definitionOrBlock170 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000861 RID: 2145
		public static readonly BitSet FOLLOW_VAR_in_fieldDefinition184 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000862 RID: 2146
		public static readonly BitSet FOLLOW_type_in_fieldDefinition186 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000863 RID: 2147
		public static readonly BitSet FOLLOW_ID_in_fieldDefinition190 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x04000864 RID: 2148
		public static readonly BitSet FOLLOW_USER_FLAGS_in_fieldDefinition192 = new BitSet(new ulong[]
		{
			8UL,
			1006764032UL
		});

		// Token: 0x04000865 RID: 2149
		public static readonly BitSet FOLLOW_constant_in_fieldDefinition194 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000866 RID: 2150
		public static readonly BitSet FOLLOW_IMPORT_in_import_obj215 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000867 RID: 2151
		public static readonly BitSet FOLLOW_ID_in_import_obj217 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000868 RID: 2152
		public static readonly BitSet FOLLOW_FUNCTION_in_function256 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000869 RID: 2153
		public static readonly BitSet FOLLOW_functionHeader_in_function258 = new BitSet(new ulong[]
		{
			1032UL
		});

		// Token: 0x0400086A RID: 2154
		public static readonly BitSet FOLLOW_codeBlock_in_function260 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400086B RID: 2155
		public static readonly BitSet FOLLOW_HEADER_in_functionHeader290 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400086C RID: 2156
		public static readonly BitSet FOLLOW_type_in_functionHeader292 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x0400086D RID: 2157
		public static readonly BitSet FOLLOW_ID_in_functionHeader296 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x0400086E RID: 2158
		public static readonly BitSet FOLLOW_USER_FLAGS_in_functionHeader298 = new BitSet(new ulong[]
		{
			212205744161288UL
		});

		// Token: 0x0400086F RID: 2159
		public static readonly BitSet FOLLOW_callParameters_in_functionHeader300 = new BitSet(new ulong[]
		{
			212205744160776UL
		});

		// Token: 0x04000870 RID: 2160
		public static readonly BitSet FOLLOW_functionModifier_in_functionHeader303 = new BitSet(new ulong[]
		{
			212205744160776UL
		});

		// Token: 0x04000871 RID: 2161
		public static readonly BitSet FOLLOW_DOCSTRING_in_functionHeader306 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000872 RID: 2162
		public static readonly BitSet FOLLOW_HEADER_in_functionHeader320 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000873 RID: 2163
		public static readonly BitSet FOLLOW_NONE_in_functionHeader322 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000874 RID: 2164
		public static readonly BitSet FOLLOW_ID_in_functionHeader324 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x04000875 RID: 2165
		public static readonly BitSet FOLLOW_USER_FLAGS_in_functionHeader326 = new BitSet(new ulong[]
		{
			212205744161288UL
		});

		// Token: 0x04000876 RID: 2166
		public static readonly BitSet FOLLOW_callParameters_in_functionHeader328 = new BitSet(new ulong[]
		{
			212205744160776UL
		});

		// Token: 0x04000877 RID: 2167
		public static readonly BitSet FOLLOW_functionModifier_in_functionHeader331 = new BitSet(new ulong[]
		{
			212205744160776UL
		});

		// Token: 0x04000878 RID: 2168
		public static readonly BitSet FOLLOW_DOCSTRING_in_functionHeader334 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000879 RID: 2169
		public static readonly BitSet FOLLOW_set_in_functionModifier0 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400087A RID: 2170
		public static readonly BitSet FOLLOW_EVENT_in_eventFunc388 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400087B RID: 2171
		public static readonly BitSet FOLLOW_eventHeader_in_eventFunc390 = new BitSet(new ulong[]
		{
			1032UL
		});

		// Token: 0x0400087C RID: 2172
		public static readonly BitSet FOLLOW_codeBlock_in_eventFunc392 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400087D RID: 2173
		public static readonly BitSet FOLLOW_HEADER_in_eventHeader408 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400087E RID: 2174
		public static readonly BitSet FOLLOW_NONE_in_eventHeader410 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x0400087F RID: 2175
		public static readonly BitSet FOLLOW_ID_in_eventHeader412 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x04000880 RID: 2176
		public static readonly BitSet FOLLOW_USER_FLAGS_in_eventHeader414 = new BitSet(new ulong[]
		{
			141836999983624UL
		});

		// Token: 0x04000881 RID: 2177
		public static readonly BitSet FOLLOW_callParameters_in_eventHeader416 = new BitSet(new ulong[]
		{
			141836999983112UL
		});

		// Token: 0x04000882 RID: 2178
		public static readonly BitSet FOLLOW_NATIVE_in_eventHeader419 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x04000883 RID: 2179
		public static readonly BitSet FOLLOW_DOCSTRING_in_eventHeader422 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000884 RID: 2180
		public static readonly BitSet FOLLOW_callParameter_in_callParameters442 = new BitSet(new ulong[]
		{
			514UL
		});

		// Token: 0x04000885 RID: 2181
		public static readonly BitSet FOLLOW_PARAM_in_callParameter456 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000886 RID: 2182
		public static readonly BitSet FOLLOW_type_in_callParameter458 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000887 RID: 2183
		public static readonly BitSet FOLLOW_ID_in_callParameter462 = new BitSet(new ulong[]
		{
			8UL,
			1006764032UL
		});

		// Token: 0x04000888 RID: 2184
		public static readonly BitSet FOLLOW_constant_in_callParameter464 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000889 RID: 2185
		public static readonly BitSet FOLLOW_STATE_in_stateBlock485 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400088A RID: 2186
		public static readonly BitSet FOLLOW_ID_in_stateBlock487 = new BitSet(new ulong[]
		{
			1125899906842824UL
		});

		// Token: 0x0400088B RID: 2187
		public static readonly BitSet FOLLOW_AUTO_in_stateBlock489 = new BitSet(new ulong[]
		{
			200UL
		});

		// Token: 0x0400088C RID: 2188
		public static readonly BitSet FOLLOW_stateFuncOrEvent_in_stateBlock493 = new BitSet(new ulong[]
		{
			200UL
		});

		// Token: 0x0400088D RID: 2189
		public static readonly BitSet FOLLOW_function_in_stateFuncOrEvent510 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400088E RID: 2190
		public static readonly BitSet FOLLOW_eventFunc_in_stateFuncOrEvent518 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400088F RID: 2191
		public static readonly BitSet FOLLOW_PROPERTY_in_propertyBlock543 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000890 RID: 2192
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock545 = new BitSet(new ulong[]
		{
			131072UL
		});

		// Token: 0x04000891 RID: 2193
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock549 = new BitSet(new ulong[]
		{
			131080UL
		});

		// Token: 0x04000892 RID: 2194
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock554 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000893 RID: 2195
		public static readonly BitSet FOLLOW_AUTOPROP_in_propertyBlock651 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000894 RID: 2196
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock653 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000895 RID: 2197
		public static readonly BitSet FOLLOW_ID_in_propertyBlock655 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000896 RID: 2198
		public static readonly BitSet FOLLOW_HEADER_in_propertyHeader678 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000897 RID: 2199
		public static readonly BitSet FOLLOW_type_in_propertyHeader680 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000898 RID: 2200
		public static readonly BitSet FOLLOW_ID_in_propertyHeader684 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x04000899 RID: 2201
		public static readonly BitSet FOLLOW_USER_FLAGS_in_propertyHeader686 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x0400089A RID: 2202
		public static readonly BitSet FOLLOW_DOCSTRING_in_propertyHeader688 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400089B RID: 2203
		public static readonly BitSet FOLLOW_PROPFUNC_in_propertyFunc713 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400089C RID: 2204
		public static readonly BitSet FOLLOW_function_in_propertyFunc715 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400089D RID: 2205
		public static readonly BitSet FOLLOW_BLOCK_in_codeBlock747 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400089E RID: 2206
		public static readonly BitSet FOLLOW_statement_in_codeBlock752 = new BitSet(new ulong[]
		{
			4611688492332845096UL,
			1025507326UL
		});

		// Token: 0x0400089F RID: 2207
		public static readonly BitSet FOLLOW_localDefinition_in_statement782 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008A0 RID: 2208
		public static readonly BitSet FOLLOW_EQUALS_in_statement789 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008A1 RID: 2209
		public static readonly BitSet FOLLOW_l_value_in_statement791 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008A2 RID: 2210
		public static readonly BitSet FOLLOW_expression_in_statement793 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008A3 RID: 2211
		public static readonly BitSet FOLLOW_expression_in_statement824 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008A4 RID: 2212
		public static readonly BitSet FOLLOW_return_stat_in_statement835 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008A5 RID: 2213
		public static readonly BitSet FOLLOW_ifBlock_in_statement841 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008A6 RID: 2214
		public static readonly BitSet FOLLOW_whileBlock_in_statement847 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008A7 RID: 2215
		public static readonly BitSet FOLLOW_VAR_in_localDefinition865 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008A8 RID: 2216
		public static readonly BitSet FOLLOW_type_in_localDefinition867 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040008A9 RID: 2217
		public static readonly BitSet FOLLOW_ID_in_localDefinition871 = new BitSet(new ulong[]
		{
			4611686293309589512UL,
			1007157246UL
		});

		// Token: 0x040008AA RID: 2218
		public static readonly BitSet FOLLOW_expression_in_localDefinition873 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008AB RID: 2219
		public static readonly BitSet FOLLOW_DOT_in_l_value936 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008AC RID: 2220
		public static readonly BitSet FOLLOW_PAREXPR_in_l_value939 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008AD RID: 2221
		public static readonly BitSet FOLLOW_expression_in_l_value943 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008AE RID: 2222
		public static readonly BitSet FOLLOW_ID_in_l_value948 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008AF RID: 2223
		public static readonly BitSet FOLLOW_ARRAYSET_in_l_value995 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008B0 RID: 2224
		public static readonly BitSet FOLLOW_PAREXPR_in_l_value998 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008B1 RID: 2225
		public static readonly BitSet FOLLOW_expression_in_l_value1002 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008B2 RID: 2226
		public static readonly BitSet FOLLOW_expression_in_l_value1007 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008B3 RID: 2227
		public static readonly BitSet FOLLOW_basic_l_value_in_l_value1046 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008B4 RID: 2228
		public static readonly BitSet FOLLOW_DOT_in_basic_l_value1074 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008B5 RID: 2229
		public static readonly BitSet FOLLOW_array_func_or_id_in_basic_l_value1078 = new BitSet(new ulong[]
		{
			4611686293313683456UL
		});

		// Token: 0x040008B6 RID: 2230
		public static readonly BitSet FOLLOW_basic_l_value_in_basic_l_value1083 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008B7 RID: 2231
		public static readonly BitSet FOLLOW_ARRAYSET_in_basic_l_value1097 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008B8 RID: 2232
		public static readonly BitSet FOLLOW_func_or_id_in_basic_l_value1099 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008B9 RID: 2233
		public static readonly BitSet FOLLOW_expression_in_basic_l_value1102 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008BA RID: 2234
		public static readonly BitSet FOLLOW_ID_in_basic_l_value1135 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008BB RID: 2235
		public static readonly BitSet FOLLOW_OR_in_expression1204 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008BC RID: 2236
		public static readonly BitSet FOLLOW_expression_in_expression1208 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008BD RID: 2237
		public static readonly BitSet FOLLOW_and_expression_in_expression1212 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008BE RID: 2238
		public static readonly BitSet FOLLOW_and_expression_in_expression1246 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008BF RID: 2239
		public static readonly BitSet FOLLOW_AND_in_and_expression1268 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008C0 RID: 2240
		public static readonly BitSet FOLLOW_and_expression_in_and_expression1272 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008C1 RID: 2241
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression1276 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008C2 RID: 2242
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression1310 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008C3 RID: 2243
		public static readonly BitSet FOLLOW_EQ_in_bool_expression1337 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008C4 RID: 2244
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1341 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008C5 RID: 2245
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1345 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008C6 RID: 2246
		public static readonly BitSet FOLLOW_NE_in_bool_expression1384 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008C7 RID: 2247
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1388 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008C8 RID: 2248
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1392 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008C9 RID: 2249
		public static readonly BitSet FOLLOW_GT_in_bool_expression1431 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008CA RID: 2250
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1435 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008CB RID: 2251
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1439 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008CC RID: 2252
		public static readonly BitSet FOLLOW_LT_in_bool_expression1478 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008CD RID: 2253
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1482 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008CE RID: 2254
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1486 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008CF RID: 2255
		public static readonly BitSet FOLLOW_GTE_in_bool_expression1525 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008D0 RID: 2256
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1529 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008D1 RID: 2257
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1533 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008D2 RID: 2258
		public static readonly BitSet FOLLOW_LTE_in_bool_expression1572 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008D3 RID: 2259
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1576 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008D4 RID: 2260
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1580 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008D5 RID: 2261
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1618 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008D6 RID: 2262
		public static readonly BitSet FOLLOW_PLUS_in_add_expression1650 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008D7 RID: 2263
		public static readonly BitSet FOLLOW_add_expression_in_add_expression1654 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008D8 RID: 2264
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1658 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008D9 RID: 2265
		public static readonly BitSet FOLLOW_MINUS_in_add_expression1748 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008DA RID: 2266
		public static readonly BitSet FOLLOW_add_expression_in_add_expression1752 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008DB RID: 2267
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1756 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008DC RID: 2268
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1820 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008DD RID: 2269
		public static readonly BitSet FOLLOW_MULT_in_mult_expression1852 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008DE RID: 2270
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression1856 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008DF RID: 2271
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression1860 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008E0 RID: 2272
		public static readonly BitSet FOLLOW_DIVIDE_in_mult_expression1925 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008E1 RID: 2273
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression1929 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008E2 RID: 2274
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression1933 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008E3 RID: 2275
		public static readonly BitSet FOLLOW_MOD_in_mult_expression1998 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008E4 RID: 2276
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2002 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008E5 RID: 2277
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2006 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008E6 RID: 2278
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2040 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008E7 RID: 2279
		public static readonly BitSet FOLLOW_UNARY_MINUS_in_unary_expression2072 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008E8 RID: 2280
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2074 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008E9 RID: 2281
		public static readonly BitSet FOLLOW_NOT_in_unary_expression2123 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008EA RID: 2282
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2125 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008EB RID: 2283
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2155 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008EC RID: 2284
		public static readonly BitSet FOLLOW_AS_in_cast_atom2177 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008ED RID: 2285
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom2179 = new BitSet(new ulong[]
		{
			36029071896870912UL
		});

		// Token: 0x040008EE RID: 2286
		public static readonly BitSet FOLLOW_type_in_cast_atom2181 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008EF RID: 2287
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom2211 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008F0 RID: 2288
		public static readonly BitSet FOLLOW_DOT_in_dot_atom2233 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008F1 RID: 2289
		public static readonly BitSet FOLLOW_dot_atom_in_dot_atom2237 = new BitSet(new ulong[]
		{
			274882136064UL,
			327680UL
		});

		// Token: 0x040008F2 RID: 2290
		public static readonly BitSet FOLLOW_array_func_or_id_in_dot_atom2241 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008F3 RID: 2291
		public static readonly BitSet FOLLOW_array_atom_in_dot_atom2254 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008F4 RID: 2292
		public static readonly BitSet FOLLOW_constant_in_dot_atom2267 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008F5 RID: 2293
		public static readonly BitSet FOLLOW_ARRAYGET_in_array_atom2294 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008F6 RID: 2294
		public static readonly BitSet FOLLOW_atom_in_array_atom2296 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x040008F7 RID: 2295
		public static readonly BitSet FOLLOW_expression_in_array_atom2299 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008F8 RID: 2296
		public static readonly BitSet FOLLOW_atom_in_array_atom2332 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040008F9 RID: 2297
		public static readonly BitSet FOLLOW_PAREXPR_in_atom2356 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008FA RID: 2298
		public static readonly BitSet FOLLOW_expression_in_atom2358 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008FB RID: 2299
		public static readonly BitSet FOLLOW_NEW_in_atom2371 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040008FC RID: 2300
		public static readonly BitSet FOLLOW_BASETYPE_in_atom2376 = new BitSet(new ulong[]
		{
			0UL,
			131072UL
		});

		// Token: 0x040008FD RID: 2301
		public static readonly BitSet FOLLOW_ID_in_atom2382 = new BitSet(new ulong[]
		{
			0UL,
			131072UL
		});

		// Token: 0x040008FE RID: 2302
		public static readonly BitSet FOLLOW_INTEGER_in_atom2385 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040008FF RID: 2303
		public static readonly BitSet FOLLOW_func_or_id_in_atom2411 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000900 RID: 2304
		public static readonly BitSet FOLLOW_ARRAYGET_in_array_func_or_id2439 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000901 RID: 2305
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id2441 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x04000902 RID: 2306
		public static readonly BitSet FOLLOW_expression_in_array_func_or_id2444 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000903 RID: 2307
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id2477 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000904 RID: 2308
		public static readonly BitSet FOLLOW_function_call_in_func_or_id2504 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000905 RID: 2309
		public static readonly BitSet FOLLOW_ID_in_func_or_id2516 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000906 RID: 2310
		public static readonly BitSet FOLLOW_LENGTH_in_func_or_id2571 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000907 RID: 2311
		public static readonly BitSet FOLLOW_RETURN_in_return_stat2611 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000908 RID: 2312
		public static readonly BitSet FOLLOW_expression_in_return_stat2613 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000909 RID: 2313
		public static readonly BitSet FOLLOW_RETURN_in_return_stat2638 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400090A RID: 2314
		public static readonly BitSet FOLLOW_IF_in_ifBlock2672 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400090B RID: 2315
		public static readonly BitSet FOLLOW_expression_in_ifBlock2674 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x0400090C RID: 2316
		public static readonly BitSet FOLLOW_codeBlock_in_ifBlock2676 = new BitSet(new ulong[]
		{
			8UL,
			12582912UL
		});

		// Token: 0x0400090D RID: 2317
		public static readonly BitSet FOLLOW_elseIfBlock_in_ifBlock2679 = new BitSet(new ulong[]
		{
			8UL,
			12582912UL
		});

		// Token: 0x0400090E RID: 2318
		public static readonly BitSet FOLLOW_elseBlock_in_ifBlock2682 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400090F RID: 2319
		public static readonly BitSet FOLLOW_ELSEIF_in_elseIfBlock2711 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000910 RID: 2320
		public static readonly BitSet FOLLOW_expression_in_elseIfBlock2713 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x04000911 RID: 2321
		public static readonly BitSet FOLLOW_codeBlock_in_elseIfBlock2715 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000912 RID: 2322
		public static readonly BitSet FOLLOW_ELSE_in_elseBlock2744 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000913 RID: 2323
		public static readonly BitSet FOLLOW_codeBlock_in_elseBlock2746 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000914 RID: 2324
		public static readonly BitSet FOLLOW_WHILE_in_whileBlock2777 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000915 RID: 2325
		public static readonly BitSet FOLLOW_expression_in_whileBlock2779 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x04000916 RID: 2326
		public static readonly BitSet FOLLOW_codeBlock_in_whileBlock2781 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000917 RID: 2327
		public static readonly BitSet FOLLOW_CALL_in_function_call2818 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000918 RID: 2328
		public static readonly BitSet FOLLOW_ID_in_function_call2820 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x04000919 RID: 2329
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call2823 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400091A RID: 2330
		public static readonly BitSet FOLLOW_parameters_in_function_call2825 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400091B RID: 2331
		public static readonly BitSet FOLLOW_parameter_in_parameters2864 = new BitSet(new ulong[]
		{
			514UL
		});

		// Token: 0x0400091C RID: 2332
		public static readonly BitSet FOLLOW_PARAM_in_parameter2878 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400091D RID: 2333
		public static readonly BitSet FOLLOW_ID_in_parameter2880 = new BitSet(new ulong[]
		{
			4611686293309589504UL,
			1007157246UL
		});

		// Token: 0x0400091E RID: 2334
		public static readonly BitSet FOLLOW_expression_in_parameter2882 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400091F RID: 2335
		public static readonly BitSet FOLLOW_number_in_constant2905 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000920 RID: 2336
		public static readonly BitSet FOLLOW_STRING_in_constant2916 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000921 RID: 2337
		public static readonly BitSet FOLLOW_BOOL_in_constant2927 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000922 RID: 2338
		public static readonly BitSet FOLLOW_NONE_in_constant2938 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000923 RID: 2339
		public static readonly BitSet FOLLOW_INTEGER_in_number2960 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000924 RID: 2340
		public static readonly BitSet FOLLOW_FLOAT_in_number2971 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000925 RID: 2341
		public static readonly BitSet FOLLOW_ID_in_type2993 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000926 RID: 2342
		public static readonly BitSet FOLLOW_ID_in_type3004 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000927 RID: 2343
		public static readonly BitSet FOLLOW_LBRACKET_in_type3006 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000928 RID: 2344
		public static readonly BitSet FOLLOW_RBRACKET_in_type3008 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000929 RID: 2345
		public static readonly BitSet FOLLOW_BASETYPE_in_type3019 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400092A RID: 2346
		public static readonly BitSet FOLLOW_BASETYPE_in_type3030 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x0400092B RID: 2347
		public static readonly BitSet FOLLOW_LBRACKET_in_type3032 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x0400092C RID: 2348
		public static readonly BitSet FOLLOW_RBRACKET_in_type3034 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x02000179 RID: 377
		public class script_return : TreeRuleReturnScope
		{
			// Token: 0x17000175 RID: 373
			// (get) Token: 0x06000C85 RID: 3205 RVA: 0x0005C4C8 File Offset: 0x0005A6C8
			// (set) Token: 0x06000C86 RID: 3206 RVA: 0x0005C4D0 File Offset: 0x0005A6D0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400092D RID: 2349
			private CommonTree tree;
		}

		// Token: 0x0200017A RID: 378
		public class header_return : TreeRuleReturnScope
		{
			// Token: 0x17000176 RID: 374
			// (get) Token: 0x06000C88 RID: 3208 RVA: 0x0005C4E8 File Offset: 0x0005A6E8
			// (set) Token: 0x06000C89 RID: 3209 RVA: 0x0005C4F0 File Offset: 0x0005A6F0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400092E RID: 2350
			private CommonTree tree;
		}

		// Token: 0x0200017B RID: 379
		public class definitionOrBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000177 RID: 375
			// (get) Token: 0x06000C8B RID: 3211 RVA: 0x0005C508 File Offset: 0x0005A708
			// (set) Token: 0x06000C8C RID: 3212 RVA: 0x0005C510 File Offset: 0x0005A710
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400092F RID: 2351
			private CommonTree tree;
		}

		// Token: 0x0200017C RID: 380
		public class fieldDefinition_return : TreeRuleReturnScope
		{
			// Token: 0x17000178 RID: 376
			// (get) Token: 0x06000C8E RID: 3214 RVA: 0x0005C528 File Offset: 0x0005A728
			// (set) Token: 0x06000C8F RID: 3215 RVA: 0x0005C530 File Offset: 0x0005A730
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000930 RID: 2352
			private CommonTree tree;
		}

		// Token: 0x0200017D RID: 381
		public class import_obj_return : TreeRuleReturnScope
		{
			// Token: 0x17000179 RID: 377
			// (get) Token: 0x06000C91 RID: 3217 RVA: 0x0005C548 File Offset: 0x0005A748
			// (set) Token: 0x06000C92 RID: 3218 RVA: 0x0005C550 File Offset: 0x0005A750
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000931 RID: 2353
			private CommonTree tree;
		}

		// Token: 0x0200017E RID: 382
		protected class function_scope
		{
			// Token: 0x04000932 RID: 2354
			protected internal string sstateName;

			// Token: 0x04000933 RID: 2355
			protected internal string spropertyName;

			// Token: 0x04000934 RID: 2356
			protected internal ScriptFunctionType kfunctionType;
		}

		// Token: 0x0200017F RID: 383
		public class function_return : TreeRuleReturnScope
		{
			// Token: 0x1700017A RID: 378
			// (get) Token: 0x06000C95 RID: 3221 RVA: 0x0005C570 File Offset: 0x0005A770
			// (set) Token: 0x06000C96 RID: 3222 RVA: 0x0005C578 File Offset: 0x0005A778
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000935 RID: 2357
			public string sName;

			// Token: 0x04000936 RID: 2358
			private CommonTree tree;
		}

		// Token: 0x02000180 RID: 384
		public class functionHeader_return : TreeRuleReturnScope
		{
			// Token: 0x1700017B RID: 379
			// (get) Token: 0x06000C98 RID: 3224 RVA: 0x0005C590 File Offset: 0x0005A790
			// (set) Token: 0x06000C99 RID: 3225 RVA: 0x0005C598 File Offset: 0x0005A798
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000937 RID: 2359
			public string sFuncName;

			// Token: 0x04000938 RID: 2360
			private CommonTree tree;
		}

		// Token: 0x02000181 RID: 385
		public class functionModifier_return : TreeRuleReturnScope
		{
			// Token: 0x1700017C RID: 380
			// (get) Token: 0x06000C9B RID: 3227 RVA: 0x0005C5B0 File Offset: 0x0005A7B0
			// (set) Token: 0x06000C9C RID: 3228 RVA: 0x0005C5B8 File Offset: 0x0005A7B8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000939 RID: 2361
			private CommonTree tree;
		}

		// Token: 0x02000182 RID: 386
		protected class eventFunc_scope
		{
			// Token: 0x0400093A RID: 2362
			protected internal string sstateName;

			// Token: 0x0400093B RID: 2363
			protected internal string seventName;

			// Token: 0x0400093C RID: 2364
			protected internal ScriptFunctionType kfunctionType;
		}

		// Token: 0x02000183 RID: 387
		public class eventFunc_return : TreeRuleReturnScope
		{
			// Token: 0x1700017D RID: 381
			// (get) Token: 0x06000C9F RID: 3231 RVA: 0x0005C5D8 File Offset: 0x0005A7D8
			// (set) Token: 0x06000CA0 RID: 3232 RVA: 0x0005C5E0 File Offset: 0x0005A7E0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400093D RID: 2365
			private CommonTree tree;
		}

		// Token: 0x02000184 RID: 388
		public class eventHeader_return : TreeRuleReturnScope
		{
			// Token: 0x1700017E RID: 382
			// (get) Token: 0x06000CA2 RID: 3234 RVA: 0x0005C5F8 File Offset: 0x0005A7F8
			// (set) Token: 0x06000CA3 RID: 3235 RVA: 0x0005C600 File Offset: 0x0005A800
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400093E RID: 2366
			private CommonTree tree;
		}

		// Token: 0x02000185 RID: 389
		public class callParameters_return : TreeRuleReturnScope
		{
			// Token: 0x1700017F RID: 383
			// (get) Token: 0x06000CA5 RID: 3237 RVA: 0x0005C618 File Offset: 0x0005A818
			// (set) Token: 0x06000CA6 RID: 3238 RVA: 0x0005C620 File Offset: 0x0005A820
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400093F RID: 2367
			private CommonTree tree;
		}

		// Token: 0x02000186 RID: 390
		public class callParameter_return : TreeRuleReturnScope
		{
			// Token: 0x17000180 RID: 384
			// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x0005C638 File Offset: 0x0005A838
			// (set) Token: 0x06000CA9 RID: 3241 RVA: 0x0005C640 File Offset: 0x0005A840
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000940 RID: 2368
			private CommonTree tree;
		}

		// Token: 0x02000187 RID: 391
		public class stateBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000181 RID: 385
			// (get) Token: 0x06000CAB RID: 3243 RVA: 0x0005C658 File Offset: 0x0005A858
			// (set) Token: 0x06000CAC RID: 3244 RVA: 0x0005C660 File Offset: 0x0005A860
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000941 RID: 2369
			private CommonTree tree;
		}

		// Token: 0x02000188 RID: 392
		public class stateFuncOrEvent_return : TreeRuleReturnScope
		{
			// Token: 0x17000182 RID: 386
			// (get) Token: 0x06000CAE RID: 3246 RVA: 0x0005C678 File Offset: 0x0005A878
			// (set) Token: 0x06000CAF RID: 3247 RVA: 0x0005C680 File Offset: 0x0005A880
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000942 RID: 2370
			private CommonTree tree;
		}

		// Token: 0x02000189 RID: 393
		protected class propertyBlock_scope
		{
			// Token: 0x04000943 RID: 2371
			protected internal bool bfunc0IsGet;
		}

		// Token: 0x0200018A RID: 394
		public class propertyBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000183 RID: 387
			// (get) Token: 0x06000CB2 RID: 3250 RVA: 0x0005C6A0 File Offset: 0x0005A8A0
			// (set) Token: 0x06000CB3 RID: 3251 RVA: 0x0005C6A8 File Offset: 0x0005A8A8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000944 RID: 2372
			private CommonTree tree;
		}

		// Token: 0x0200018B RID: 395
		public class propertyHeader_return : TreeRuleReturnScope
		{
			// Token: 0x17000184 RID: 388
			// (get) Token: 0x06000CB5 RID: 3253 RVA: 0x0005C6C0 File Offset: 0x0005A8C0
			// (set) Token: 0x06000CB6 RID: 3254 RVA: 0x0005C6C8 File Offset: 0x0005A8C8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000945 RID: 2373
			public string sName;

			// Token: 0x04000946 RID: 2374
			private CommonTree tree;
		}

		// Token: 0x0200018C RID: 396
		public class propertyFunc_return : TreeRuleReturnScope
		{
			// Token: 0x17000185 RID: 389
			// (get) Token: 0x06000CB8 RID: 3256 RVA: 0x0005C6E0 File Offset: 0x0005A8E0
			// (set) Token: 0x06000CB9 RID: 3257 RVA: 0x0005C6E8 File Offset: 0x0005A8E8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000947 RID: 2375
			public bool bIsGet;

			// Token: 0x04000948 RID: 2376
			private CommonTree tree;
		}

		// Token: 0x0200018D RID: 397
		protected class codeBlock_scope
		{
			// Token: 0x04000949 RID: 2377
			protected internal Dictionary<string, ScriptVariableType> kTempVars;

			// Token: 0x0400094A RID: 2378
			protected internal ScriptFunctionType kfunctionType;

			// Token: 0x0400094B RID: 2379
			protected internal ScriptScope kcurrentScope;

			// Token: 0x0400094C RID: 2380
			protected internal int inextScopeChild;
		}

		// Token: 0x0200018E RID: 398
		public class codeBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000186 RID: 390
			// (get) Token: 0x06000CBC RID: 3260 RVA: 0x0005C708 File Offset: 0x0005A908
			// (set) Token: 0x06000CBD RID: 3261 RVA: 0x0005C710 File Offset: 0x0005A910
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400094D RID: 2381
			private CommonTree tree;
		}

		// Token: 0x0200018F RID: 399
		protected class statement_scope
		{
			// Token: 0x0400094E RID: 2382
			protected internal CommonTree kautoCastTree;
		}

		// Token: 0x02000190 RID: 400
		public class statement_return : TreeRuleReturnScope
		{
			// Token: 0x17000187 RID: 391
			// (get) Token: 0x06000CC0 RID: 3264 RVA: 0x0005C730 File Offset: 0x0005A930
			// (set) Token: 0x06000CC1 RID: 3265 RVA: 0x0005C738 File Offset: 0x0005A938
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400094F RID: 2383
			private CommonTree tree;
		}

		// Token: 0x02000191 RID: 401
		protected class localDefinition_scope
		{
			// Token: 0x04000950 RID: 2384
			protected internal CommonTree kautoCastTree;
		}

		// Token: 0x02000192 RID: 402
		public class localDefinition_return : TreeRuleReturnScope
		{
			// Token: 0x17000188 RID: 392
			// (get) Token: 0x06000CC4 RID: 3268 RVA: 0x0005C758 File Offset: 0x0005A958
			// (set) Token: 0x06000CC5 RID: 3269 RVA: 0x0005C760 File Offset: 0x0005A960
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000951 RID: 2385
			private CommonTree tree;
		}

		// Token: 0x02000193 RID: 403
		protected class l_value_scope
		{
			// Token: 0x04000952 RID: 2386
			protected internal IToken kvarToken;

			// Token: 0x04000953 RID: 2387
			protected internal CommonTree kautoCastTree;
		}

		// Token: 0x02000194 RID: 404
		public class l_value_return : TreeRuleReturnScope
		{
			// Token: 0x17000189 RID: 393
			// (get) Token: 0x06000CC8 RID: 3272 RVA: 0x0005C780 File Offset: 0x0005A980
			// (set) Token: 0x06000CC9 RID: 3273 RVA: 0x0005C788 File Offset: 0x0005A988
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000954 RID: 2388
			public ScriptVariableType kType;

			// Token: 0x04000955 RID: 2389
			public string sVarName;

			// Token: 0x04000956 RID: 2390
			private CommonTree tree;
		}

		// Token: 0x02000195 RID: 405
		protected class basic_l_value_scope
		{
			// Token: 0x04000957 RID: 2391
			protected internal bool bisProperty;

			// Token: 0x04000958 RID: 2392
			protected internal bool bisLocalAutoProperty;

			// Token: 0x04000959 RID: 2393
			protected internal IToken kvarToken;

			// Token: 0x0400095A RID: 2394
			protected internal CommonTree kautoCastTree;
		}

		// Token: 0x02000196 RID: 406
		public class basic_l_value_return : TreeRuleReturnScope
		{
			// Token: 0x1700018A RID: 394
			// (get) Token: 0x06000CCC RID: 3276 RVA: 0x0005C7A8 File Offset: 0x0005A9A8
			// (set) Token: 0x06000CCD RID: 3277 RVA: 0x0005C7B0 File Offset: 0x0005A9B0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400095B RID: 2395
			public ScriptVariableType kType;

			// Token: 0x0400095C RID: 2396
			public string sVarName;

			// Token: 0x0400095D RID: 2397
			private CommonTree tree;
		}

		// Token: 0x02000197 RID: 407
		public class expression_return : TreeRuleReturnScope
		{
			// Token: 0x1700018B RID: 395
			// (get) Token: 0x06000CCF RID: 3279 RVA: 0x0005C7C8 File Offset: 0x0005A9C8
			// (set) Token: 0x06000CD0 RID: 3280 RVA: 0x0005C7D0 File Offset: 0x0005A9D0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400095E RID: 2398
			public ScriptVariableType kType;

			// Token: 0x0400095F RID: 2399
			public string sVarName;

			// Token: 0x04000960 RID: 2400
			public IToken kVarToken;

			// Token: 0x04000961 RID: 2401
			private CommonTree tree;
		}

		// Token: 0x02000198 RID: 408
		public class and_expression_return : TreeRuleReturnScope
		{
			// Token: 0x1700018C RID: 396
			// (get) Token: 0x06000CD2 RID: 3282 RVA: 0x0005C7E8 File Offset: 0x0005A9E8
			// (set) Token: 0x06000CD3 RID: 3283 RVA: 0x0005C7F0 File Offset: 0x0005A9F0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000962 RID: 2402
			public ScriptVariableType kType;

			// Token: 0x04000963 RID: 2403
			public string sVarName;

			// Token: 0x04000964 RID: 2404
			public IToken kVarToken;

			// Token: 0x04000965 RID: 2405
			private CommonTree tree;
		}

		// Token: 0x02000199 RID: 409
		protected class bool_expression_scope
		{
			// Token: 0x04000966 RID: 2406
			protected internal CommonTree kaTree;

			// Token: 0x04000967 RID: 2407
			protected internal CommonTree kbTree;
		}

		// Token: 0x0200019A RID: 410
		public class bool_expression_return : TreeRuleReturnScope
		{
			// Token: 0x1700018D RID: 397
			// (get) Token: 0x06000CD6 RID: 3286 RVA: 0x0005C810 File Offset: 0x0005AA10
			// (set) Token: 0x06000CD7 RID: 3287 RVA: 0x0005C818 File Offset: 0x0005AA18
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000968 RID: 2408
			public ScriptVariableType kType;

			// Token: 0x04000969 RID: 2409
			public string sVarName;

			// Token: 0x0400096A RID: 2410
			public IToken kVarToken;

			// Token: 0x0400096B RID: 2411
			private CommonTree tree;
		}

		// Token: 0x0200019B RID: 411
		protected class add_expression_scope
		{
			// Token: 0x0400096C RID: 2412
			protected internal bool bisInt;

			// Token: 0x0400096D RID: 2413
			protected internal bool bisConcat;

			// Token: 0x0400096E RID: 2414
			protected internal CommonTree kaTree;

			// Token: 0x0400096F RID: 2415
			protected internal CommonTree kbTree;
		}

		// Token: 0x0200019C RID: 412
		public class add_expression_return : TreeRuleReturnScope
		{
			// Token: 0x1700018E RID: 398
			// (get) Token: 0x06000CDA RID: 3290 RVA: 0x0005C838 File Offset: 0x0005AA38
			// (set) Token: 0x06000CDB RID: 3291 RVA: 0x0005C840 File Offset: 0x0005AA40
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000970 RID: 2416
			public ScriptVariableType kType;

			// Token: 0x04000971 RID: 2417
			public string sVarName;

			// Token: 0x04000972 RID: 2418
			public IToken kVarToken;

			// Token: 0x04000973 RID: 2419
			private CommonTree tree;
		}

		// Token: 0x0200019D RID: 413
		protected class mult_expression_scope
		{
			// Token: 0x04000974 RID: 2420
			protected internal bool bisInt;

			// Token: 0x04000975 RID: 2421
			protected internal CommonTree kaTree;

			// Token: 0x04000976 RID: 2422
			protected internal CommonTree kbTree;
		}

		// Token: 0x0200019E RID: 414
		public class mult_expression_return : TreeRuleReturnScope
		{
			// Token: 0x1700018F RID: 399
			// (get) Token: 0x06000CDE RID: 3294 RVA: 0x0005C860 File Offset: 0x0005AA60
			// (set) Token: 0x06000CDF RID: 3295 RVA: 0x0005C868 File Offset: 0x0005AA68
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000977 RID: 2423
			public ScriptVariableType kType;

			// Token: 0x04000978 RID: 2424
			public string sVarName;

			// Token: 0x04000979 RID: 2425
			public IToken kVarToken;

			// Token: 0x0400097A RID: 2426
			private CommonTree tree;
		}

		// Token: 0x0200019F RID: 415
		protected class unary_expression_scope
		{
			// Token: 0x0400097B RID: 2427
			protected internal bool bisInt;
		}

		// Token: 0x020001A0 RID: 416
		public class unary_expression_return : TreeRuleReturnScope
		{
			// Token: 0x17000190 RID: 400
			// (get) Token: 0x06000CE2 RID: 3298 RVA: 0x0005C888 File Offset: 0x0005AA88
			// (set) Token: 0x06000CE3 RID: 3299 RVA: 0x0005C890 File Offset: 0x0005AA90
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400097C RID: 2428
			public ScriptVariableType kType;

			// Token: 0x0400097D RID: 2429
			public string sVarName;

			// Token: 0x0400097E RID: 2430
			public IToken kVarToken;

			// Token: 0x0400097F RID: 2431
			private CommonTree tree;
		}

		// Token: 0x020001A1 RID: 417
		public class cast_atom_return : TreeRuleReturnScope
		{
			// Token: 0x17000191 RID: 401
			// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x0005C8A8 File Offset: 0x0005AAA8
			// (set) Token: 0x06000CE6 RID: 3302 RVA: 0x0005C8B0 File Offset: 0x0005AAB0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000980 RID: 2432
			public ScriptVariableType kType;

			// Token: 0x04000981 RID: 2433
			public string sVarName;

			// Token: 0x04000982 RID: 2434
			public IToken kVarToken;

			// Token: 0x04000983 RID: 2435
			private CommonTree tree;
		}

		// Token: 0x020001A2 RID: 418
		public class dot_atom_return : TreeRuleReturnScope
		{
			// Token: 0x17000192 RID: 402
			// (get) Token: 0x06000CE8 RID: 3304 RVA: 0x0005C8C8 File Offset: 0x0005AAC8
			// (set) Token: 0x06000CE9 RID: 3305 RVA: 0x0005C8D0 File Offset: 0x0005AAD0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000984 RID: 2436
			public ScriptVariableType kType;

			// Token: 0x04000985 RID: 2437
			public string sVarName;

			// Token: 0x04000986 RID: 2438
			public IToken kVarToken;

			// Token: 0x04000987 RID: 2439
			private CommonTree tree;
		}

		// Token: 0x020001A3 RID: 419
		protected class array_atom_scope
		{
			// Token: 0x04000988 RID: 2440
			protected internal CommonTree kautoCastTree;
		}

		// Token: 0x020001A4 RID: 420
		public class array_atom_return : TreeRuleReturnScope
		{
			// Token: 0x17000193 RID: 403
			// (get) Token: 0x06000CEC RID: 3308 RVA: 0x0005C8F0 File Offset: 0x0005AAF0
			// (set) Token: 0x06000CED RID: 3309 RVA: 0x0005C8F8 File Offset: 0x0005AAF8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000989 RID: 2441
			public ScriptVariableType kType;

			// Token: 0x0400098A RID: 2442
			public string sVarName;

			// Token: 0x0400098B RID: 2443
			public IToken kVarToken;

			// Token: 0x0400098C RID: 2444
			private CommonTree tree;
		}

		// Token: 0x020001A5 RID: 421
		public class atom_return : TreeRuleReturnScope
		{
			// Token: 0x17000194 RID: 404
			// (get) Token: 0x06000CEF RID: 3311 RVA: 0x0005C910 File Offset: 0x0005AB10
			// (set) Token: 0x06000CF0 RID: 3312 RVA: 0x0005C918 File Offset: 0x0005AB18
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400098D RID: 2445
			public ScriptVariableType kType;

			// Token: 0x0400098E RID: 2446
			public string sVarName;

			// Token: 0x0400098F RID: 2447
			public IToken kVarToken;

			// Token: 0x04000990 RID: 2448
			private CommonTree tree;
		}

		// Token: 0x020001A6 RID: 422
		protected class array_func_or_id_scope
		{
			// Token: 0x04000991 RID: 2449
			protected internal CommonTree kautoCastTree;
		}

		// Token: 0x020001A7 RID: 423
		public class array_func_or_id_return : TreeRuleReturnScope
		{
			// Token: 0x17000195 RID: 405
			// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x0005C938 File Offset: 0x0005AB38
			// (set) Token: 0x06000CF4 RID: 3316 RVA: 0x0005C940 File Offset: 0x0005AB40
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000992 RID: 2450
			public ScriptVariableType kType;

			// Token: 0x04000993 RID: 2451
			public string sVarName;

			// Token: 0x04000994 RID: 2452
			public IToken kVarToken;

			// Token: 0x04000995 RID: 2453
			private CommonTree tree;
		}

		// Token: 0x020001A8 RID: 424
		protected class func_or_id_scope
		{
			// Token: 0x04000996 RID: 2454
			protected internal bool bisProperty;

			// Token: 0x04000997 RID: 2455
			protected internal bool bisLocalAutoProperty;

			// Token: 0x04000998 RID: 2456
			protected internal CommonTree kautoCastTree;
		}

		// Token: 0x020001A9 RID: 425
		public class func_or_id_return : TreeRuleReturnScope
		{
			// Token: 0x17000196 RID: 406
			// (get) Token: 0x06000CF7 RID: 3319 RVA: 0x0005C960 File Offset: 0x0005AB60
			// (set) Token: 0x06000CF8 RID: 3320 RVA: 0x0005C968 File Offset: 0x0005AB68
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x04000999 RID: 2457
			public ScriptVariableType kType;

			// Token: 0x0400099A RID: 2458
			public string sVarName;

			// Token: 0x0400099B RID: 2459
			public IToken kVarToken;

			// Token: 0x0400099C RID: 2460
			private CommonTree tree;
		}

		// Token: 0x020001AA RID: 426
		protected class return_stat_scope
		{
			// Token: 0x0400099D RID: 2461
			protected internal CommonTree kautoCastTree;
		}

		// Token: 0x020001AB RID: 427
		public class return_stat_return : TreeRuleReturnScope
		{
			// Token: 0x17000197 RID: 407
			// (get) Token: 0x06000CFB RID: 3323 RVA: 0x0005C988 File Offset: 0x0005AB88
			// (set) Token: 0x06000CFC RID: 3324 RVA: 0x0005C990 File Offset: 0x0005AB90
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x0400099E RID: 2462
			private CommonTree tree;
		}

		// Token: 0x020001AC RID: 428
		protected class ifBlock_scope
		{
			// Token: 0x0400099F RID: 2463
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x020001AD RID: 429
		public class ifBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000198 RID: 408
			// (get) Token: 0x06000CFF RID: 3327 RVA: 0x0005C9B0 File Offset: 0x0005ABB0
			// (set) Token: 0x06000D00 RID: 3328 RVA: 0x0005C9B8 File Offset: 0x0005ABB8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009A0 RID: 2464
			private CommonTree tree;
		}

		// Token: 0x020001AE RID: 430
		protected class elseIfBlock_scope
		{
			// Token: 0x040009A1 RID: 2465
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x020001AF RID: 431
		public class elseIfBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000199 RID: 409
			// (get) Token: 0x06000D03 RID: 3331 RVA: 0x0005C9D8 File Offset: 0x0005ABD8
			// (set) Token: 0x06000D04 RID: 3332 RVA: 0x0005C9E0 File Offset: 0x0005ABE0
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009A2 RID: 2466
			private CommonTree tree;
		}

		// Token: 0x020001B0 RID: 432
		protected class elseBlock_scope
		{
			// Token: 0x040009A3 RID: 2467
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x020001B1 RID: 433
		public class elseBlock_return : TreeRuleReturnScope
		{
			// Token: 0x1700019A RID: 410
			// (get) Token: 0x06000D07 RID: 3335 RVA: 0x0005CA00 File Offset: 0x0005AC00
			// (set) Token: 0x06000D08 RID: 3336 RVA: 0x0005CA08 File Offset: 0x0005AC08
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009A4 RID: 2468
			private CommonTree tree;
		}

		// Token: 0x020001B2 RID: 434
		protected class whileBlock_scope
		{
			// Token: 0x040009A5 RID: 2469
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x020001B3 RID: 435
		public class whileBlock_return : TreeRuleReturnScope
		{
			// Token: 0x1700019B RID: 411
			// (get) Token: 0x06000D0B RID: 3339 RVA: 0x0005CA28 File Offset: 0x0005AC28
			// (set) Token: 0x06000D0C RID: 3340 RVA: 0x0005CA30 File Offset: 0x0005AC30
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009A6 RID: 2470
			private CommonTree tree;
		}

		// Token: 0x020001B4 RID: 436
		protected class function_call_scope
		{
			// Token: 0x040009A7 RID: 2471
			protected internal List<string> ktargetParamNames;

			// Token: 0x040009A8 RID: 2472
			protected internal List<ScriptVariableType> kparamTypes;

			// Token: 0x040009A9 RID: 2473
			protected internal List<string> kparamVarNames;

			// Token: 0x040009AA RID: 2474
			protected internal List<IToken> kparamTokens;

			// Token: 0x040009AB RID: 2475
			protected internal List<CommonTree> kparamExpressions;

			// Token: 0x040009AC RID: 2476
			protected internal List<CommonTree> kparamAutoCasts;

			// Token: 0x040009AD RID: 2477
			protected internal bool bisGlobal;

			// Token: 0x040009AE RID: 2478
			protected internal bool bisArray;
		}

		// Token: 0x020001B5 RID: 437
		public class function_call_return : TreeRuleReturnScope
		{
			// Token: 0x1700019C RID: 412
			// (get) Token: 0x06000D0F RID: 3343 RVA: 0x0005CA50 File Offset: 0x0005AC50
			// (set) Token: 0x06000D10 RID: 3344 RVA: 0x0005CA58 File Offset: 0x0005AC58
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009AF RID: 2479
			public ScriptVariableType kType;

			// Token: 0x040009B0 RID: 2480
			public string sVarName;

			// Token: 0x040009B1 RID: 2481
			public IToken kVarToken;

			// Token: 0x040009B2 RID: 2482
			private CommonTree tree;
		}

		// Token: 0x020001B6 RID: 438
		public class parameters_return : TreeRuleReturnScope
		{
			// Token: 0x1700019D RID: 413
			// (get) Token: 0x06000D12 RID: 3346 RVA: 0x0005CA70 File Offset: 0x0005AC70
			// (set) Token: 0x06000D13 RID: 3347 RVA: 0x0005CA78 File Offset: 0x0005AC78
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009B3 RID: 2483
			private CommonTree tree;
		}

		// Token: 0x020001B7 RID: 439
		public class parameter_return : TreeRuleReturnScope
		{
			// Token: 0x1700019E RID: 414
			// (get) Token: 0x06000D15 RID: 3349 RVA: 0x0005CA90 File Offset: 0x0005AC90
			// (set) Token: 0x06000D16 RID: 3350 RVA: 0x0005CA98 File Offset: 0x0005AC98
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009B4 RID: 2484
			private CommonTree tree;
		}

		// Token: 0x020001B8 RID: 440
		public class constant_return : TreeRuleReturnScope
		{
			// Token: 0x1700019F RID: 415
			// (get) Token: 0x06000D18 RID: 3352 RVA: 0x0005CAB0 File Offset: 0x0005ACB0
			// (set) Token: 0x06000D19 RID: 3353 RVA: 0x0005CAB8 File Offset: 0x0005ACB8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009B5 RID: 2485
			public ScriptVariableType kType;

			// Token: 0x040009B6 RID: 2486
			public IToken kVarToken;

			// Token: 0x040009B7 RID: 2487
			private CommonTree tree;
		}

		// Token: 0x020001B9 RID: 441
		public class number_return : TreeRuleReturnScope
		{
			// Token: 0x170001A0 RID: 416
			// (get) Token: 0x06000D1B RID: 3355 RVA: 0x0005CAD0 File Offset: 0x0005ACD0
			// (set) Token: 0x06000D1C RID: 3356 RVA: 0x0005CAD8 File Offset: 0x0005ACD8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009B8 RID: 2488
			public ScriptVariableType kType;

			// Token: 0x040009B9 RID: 2489
			public IToken kVarToken;

			// Token: 0x040009BA RID: 2490
			private CommonTree tree;
		}

		// Token: 0x020001BA RID: 442
		public class type_return : TreeRuleReturnScope
		{
			// Token: 0x170001A1 RID: 417
			// (get) Token: 0x06000D1E RID: 3358 RVA: 0x0005CAF0 File Offset: 0x0005ACF0
			// (set) Token: 0x06000D1F RID: 3359 RVA: 0x0005CAF8 File Offset: 0x0005ACF8
			public override object Tree
			{
				get
				{
					return this.tree;
				}
				set
				{
					this.tree = (CommonTree)value;
				}
			}

			// Token: 0x040009BB RID: 2491
			public ScriptVariableType kType;

			// Token: 0x040009BC RID: 2492
			private CommonTree tree;
		}
	}
}
