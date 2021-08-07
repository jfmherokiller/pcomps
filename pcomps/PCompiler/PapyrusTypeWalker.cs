using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var result = kKnownTypes.ContainsKey(akType.IsArray ? akType.ArrayElementType : akType.VarType);
            return result;
        }

        // Token: 0x06000C1C RID: 3100 RVA: 0x00049C68 File Offset: 0x00047E68
        private bool IsKnownType(string asType)
        {
            return IsKnownType(new ScriptVariableType(asType));
        }

        // Token: 0x06000C1D RID: 3101 RVA: 0x00049C78 File Offset: 0x00047E78
        private void AddKnownType(string asType)
        {
            if (!IsKnownType(asType))
            {
                kKnownTypes.Add(asType.ToLowerInvariant(), null);
            }
        }

        // Token: 0x06000C1E RID: 3102 RVA: 0x00049C98 File Offset: 0x00047E98
        private void AddKnownType(ScriptObjectType akObjType)
        {
            if (IsKnownType(akObjType.Name))
            {
                kKnownTypes[akObjType.Name] = akObjType;
                return;
            }
            kKnownTypes.Add(akObjType.Name, akObjType);
        }

        // Token: 0x06000C1F RID: 3103 RVA: 0x00049CD0 File Offset: 0x00047ED0
        private ScriptObjectType GetKnownType(ScriptVariableType akType)
        {
            var text = akType.VarType;
            if (akType.IsArray)
            {
                text = akType.ArrayElementType;
            }
            if (!kKnownTypes.TryGetValue(text, out var scriptObjectType) || scriptObjectType == null)
            {
                scriptObjectType = kCompiler.LoadObject(text, kKnownTypes, false);
            }
            return scriptObjectType;
        }

        // Token: 0x06000C20 RID: 3104 RVA: 0x00049D20 File Offset: 0x00047F20
        private ScriptObjectType GetKnownType(string asType)
        {
            return GetKnownType(new ScriptVariableType(asType));
        }

        // Token: 0x17000170 RID: 368
        // (get) Token: 0x06000C21 RID: 3105 RVA: 0x00049D30 File Offset: 0x00047F30
        public Dictionary<string, ScriptObjectType> KnownTypes => kKnownTypes;

        // Token: 0x06000C22 RID: 3106 RVA: 0x00049D38 File Offset: 0x00047F38
        private bool IsImportedType(string asType) => kImportedTypes.ContainsKey(asType.ToLowerInvariant());

        // Token: 0x06000C23 RID: 3107 RVA: 0x00049D4C File Offset: 0x00047F4C
        private void AddImportedType(ScriptObjectType akObjType)
        {
            if (IsImportedType(akObjType.Name))
            {
                kImportedTypes[akObjType.Name] = akObjType;
                return;
            }
            kImportedTypes.Add(akObjType.Name, akObjType);
        }

        // Token: 0x17000171 RID: 369
        // (get) Token: 0x06000C24 RID: 3108 RVA: 0x00049D84 File Offset: 0x00047F84
        public Dictionary<string, ScriptObjectType> ImportedTypes => kImportedTypes;

        // Token: 0x06000C25 RID: 3109 RVA: 0x00049D8C File Offset: 0x00047F8C
        private void HandleParent(IToken akParent)
        {
            ScriptObjectType scriptObjectType = null;
            var text = akParent.Text.ToLowerInvariant();
            if (text == kObjType.Name)
            {
                OnError("cannot extend ourself", akParent.Line, akParent.CharPositionInLine);
            }
            else if (kChildren.Contains(text))
            {
                OnError($"cannot extend from {akParent.Text} as it extends from us", akParent.Line, akParent.CharPositionInLine);
            }
            else if (!kKnownTypes.TryGetValue(text, out scriptObjectType))
            {
                kChildren.Push(kObjType.Name);
                scriptObjectType = kCompiler.LoadObject(akParent.Text, kKnownTypes, kChildren, true, kObjType);
                kChildren.Pop();
            }
            else if (scriptObjectType == null)
            {
                OnError("cannot extend a built-in type", akParent.Line, akParent.CharPositionInLine);
            }

            if (scriptObjectType == null) return;
            kObjType.kParent = scriptObjectType;
            AddImportedType(scriptObjectType);
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
            var asName = akVarToken.Text.ToLowerInvariant();
            if (akCurrentScope.TryGetVariable(asName, out var result) ||
                (!akFunction.bGlobal && kObjType.TryGetVariable(asName, out result))) return result;
            OnError($"variable {akVarToken.Text} is undefined", akVarToken.Line, akVarToken.CharPositionInLine);
            result = new ScriptVariableType("none");
            return result;
        }

        // Token: 0x06000C28 RID: 3112 RVA: 0x00049F90 File Offset: 0x00048190
        private bool IsLocalProperty(string asName)
        {
            var kParent = kObjType;
            var flag = false;
            while (!flag && kParent != null)
            {
                flag = kParent.TryGetProperty(asName, out var scriptPropertyType);
                kParent = kParent.kParent;
            }
            return flag;
        }

        // Token: 0x06000C29 RID: 3113 RVA: 0x00049FC0 File Offset: 0x000481C0
        private void GetPropertyInfo(ScriptVariableType akObjType, IToken akIDToken, bool abCheckForGetter, out ScriptVariableType akPropType)
        {
            akPropType = new ScriptVariableType("none");
            var scriptObjectType = GetKnownType(akObjType.VarType);
            if (scriptObjectType == null)
            {
                OnError($"{akObjType.VarType} is not a known user-defined type", akIDToken.Line, akIDToken.CharPositionInLine);
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
                OnError($"{akIDToken.Text} is not a property on script {akObjType.VarType} or one of its parents", akIDToken.Line, akIDToken.CharPositionInLine);
                return;
            }
            if (abCheckForGetter)
            {
                if (scriptPropertyType.kGetFunction == null)
                {
                    OnError(
                        $"property {akIDToken.Text} on script {scriptObjectType.Name} is write-only, you cannot retrieve a value from it", akIDToken.Line, akIDToken.CharPositionInLine);
                    return;
                }
                akPropType = scriptPropertyType.kType;
                return;
            }
            else
            {
                if (scriptPropertyType.kSetFunction == null)
                {
                    OnError(
                        $"property {akIDToken.Text} on script {scriptObjectType.Name} is read-only, you cannot give it a value", akIDToken.Line, akIDToken.CharPositionInLine);
                    return;
                }
                akPropType = scriptPropertyType.kType;
                return;
            }
        }

        // Token: 0x06000C2A RID: 3114 RVA: 0x0004A0D8 File Offset: 0x000482D8
        private bool IsGlobalFunction(ScriptVariableType akType, string asFuncName)
        {
            var result = false;
            var knownType = GetKnownType(akType.VarType);
            if (knownType != null && knownType.TryGetFunction(asFuncName.ToLowerInvariant(), out var scriptFunctionType))
            {
                result = scriptFunctionType.bGlobal;
            }
            return result;
        }

        // Token: 0x06000C2B RID: 3115 RVA: 0x0004A110 File Offset: 0x00048310
        private ScriptVariableType FindFunctionOwningType(string asFuncName, out bool abCallingOnSelf, IToken akTargetToken)
        {
            var scriptVariableType = new ScriptVariableType(kObjType.Name);
            abCallingOnSelf = true;
            if (kObjType.TryGetFunction(asFuncName, out var scriptFunctionType)) return scriptVariableType;
            var flag = false;
            var kParent = kObjType.kParent;
            while (kParent != null && !flag)
            {
                if (kParent.TryGetFunction(asFuncName, out scriptFunctionType))
                {
                    flag = true;
                }
                kParent = kParent.kParent;
            }

            if (flag) return scriptVariableType;
            foreach (var scriptObjectType in kImportedTypes.Values.Where(scriptObjectType => scriptObjectType.TryGetFunction(asFuncName, out scriptFunctionType)))
            {
                if (!abCallingOnSelf)
                {
                    OnError(
                        $"Function {asFuncName} is ambiguous between types {scriptVariableType.VarType} and {scriptObjectType.Name}", akTargetToken.Line, akTargetToken.CharPositionInLine);
                }
                scriptVariableType = new ScriptVariableType(scriptObjectType.Name);
                abCallingOnSelf = false;
            }
            return scriptVariableType;
        }

        // Token: 0x06000C2C RID: 3116 RVA: 0x0004A208 File Offset: 0x00048408
        private void CheckVariableDefinition(string asName, ScriptVariableType akType, IToken akInitialValue, bool abInFunctionBlock, IToken akTargetToken)
        {
            CheckTypeAndValue(akType, akInitialValue, akTargetToken);
            CheckVarOrPropName(asName, akTargetToken);
            if (!abInFunctionBlock) return;
            if (kObjType.TryGetVariable(asName.ToLowerInvariant(), out var scriptVariableType))
            {
                OnError($"variable {asName} already defined as a script variable", akTargetToken.Line, akTargetToken.CharPositionInLine);
                return;
            }

            if (kObjType.TryGetProperty(asName.ToLowerInvariant(), out var scriptPropertyType))
            {
                OnError($"variable {asName} already defined as a property", akTargetToken.Line, akTargetToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C2D RID: 3117 RVA: 0x0004A298 File Offset: 0x00048498
        private void CheckVarOrPropName(string asName, IToken akTargetToken)
        {
            var knownType = GetKnownType(asName);
            if (knownType != null)
            {
                OnError("cannot name a variable or property the same as a known type or script", akTargetToken.Line, akTargetToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C2E RID: 3118 RVA: 0x0004A2C8 File Offset: 0x000484C8
        private void CheckTypeAndValue(ScriptVariableType akType, IToken akValue, IToken akErrorToken)
        {
            if (!IsKnownType(akType))
            {
                if (GetKnownType(akType) == null)
                {
                    OnError($"unknown type {akType.VarType}", akErrorToken.Line, akErrorToken.CharPositionInLine);
                    return;
                }
            }
            else if (akValue != null && !ValueTypeMatches(akType, akValue))
            {
                OnError($"cannot initialize a {akType.VarType} to {akValue.Text}", akErrorToken.Line, akErrorToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C2F RID: 3119 RVA: 0x0004A344 File Offset: 0x00048544
        private bool CanAutoCast(ScriptVariableType akTarget, ScriptVariableType akSource)
        {
            var flag = akTarget.VarType == akSource.VarType;
            if (flag) return true;
            if (akTarget.IsArray)
            {
                flag = (akSource.VarType == "none");
            }
            else if (!akTarget.IsObjectType)
            {
                switch (akTarget.VarType)
                {
                    case "bool":
                    case "string":
                        flag = true;
                        break;
                    case "float":
                        flag = (akSource.VarType == "int");
                        break;
                }
            }
            else if (akSource.VarType == "none")
            {
                flag = true;
            }
            else
            {
                var scriptObjectType = GetKnownType(akSource.VarType);
                if (scriptObjectType == null) return false;
                scriptObjectType = scriptObjectType.kParent;
                while (!flag && scriptObjectType != null)
                {
                    flag = (scriptObjectType.Name == akTarget.VarType);
                    scriptObjectType = scriptObjectType.kParent;
                }
            }
            return flag;
        }

        // Token: 0x06000C30 RID: 3120 RVA: 0x0004A434 File Offset: 0x00048634
        private CommonTree AutoCast(IToken akTokenToCast, ScriptVariableType akSourceType, ScriptVariableType akDestType, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out string asNewTempVar)
        {
            var commonTree = new CommonTree(akTokenToCast);
            asNewTempVar = "";
            if (akTokenToCast == null || akSourceType.VarType == akDestType.VarType) return commonTree;
            asNewTempVar = GenerateTempVariable(akDestType, akCurrentScope, akTempVars);
            CommonToken commonToken = new(akTokenToCast)
            {
                Type = 79,
                Text = "AS"
            };
            CommonToken commonToken2 = new(akTokenToCast)
            {
                Type = 38,
                Text = asNewTempVar
            };
            commonTree = new CommonTree(commonToken);
            commonTree.AddChild(new CommonTree(commonToken2));
            commonTree.AddChild(new CommonTree(akTokenToCast));
            return commonTree;
        }

        // Token: 0x06000C31 RID: 3121 RVA: 0x0004A4C8 File Offset: 0x000486C8
        private CommonTree AutoCastReturn(IToken akTokenToCast, ScriptVariableType akSourceType, string asFuncName, string asPropertyName, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, IToken akRetToken)
        {
            CommonTree result = null;
            ScriptFunctionType scriptFunctionType = null;
            if (asPropertyName == "")
            {
                if (!kObjType.TryGetFunction(asFuncName, out scriptFunctionType))
                {
                    OnError(
                        $"internal error: cannot type-check return because function {asFuncName} is not on object {kObjType.Name}", akRetToken.Line, akRetToken.CharPositionInLine);
                }
            }
            else if (kObjType.TryGetProperty(asPropertyName, out var scriptPropertyType))
            {
                var a = asFuncName.ToLowerInvariant();
                switch (a)
                {
                    case "get":
                        scriptFunctionType = scriptPropertyType.kGetFunction;
                        break;
                    case "set":
                        scriptFunctionType = scriptPropertyType.kSetFunction;
                        break;
                    default:
                        OnError($"Error: Function {asFuncName} in property {asPropertyName} must be either get or set", akRetToken.Line, akRetToken.CharPositionInLine);
                        break;
                }
            }
            else
            {
                OnError(
                    $"internal error: cannot type-check return because property {asPropertyName} is not on object {kObjType.Name}", akRetToken.Line, akRetToken.CharPositionInLine);
            }

            if (scriptFunctionType == null) return null;
            result = AutoCast(akTokenToCast, akSourceType, scriptFunctionType.kRetType, akCurrentScope, akTempVars, out var asVarName);
            MarkTempVarAsUnused(scriptFunctionType.kRetType, asVarName, akRetToken);
            return result;
        }

        // Token: 0x06000C32 RID: 3122 RVA: 0x0004A5E4 File Offset: 0x000487E4
        private void CheckAssignmentType(ScriptVariableType akTarget, ScriptVariableType akSource, IToken akTargetToken)
        {
            if (akTarget != null && akSource != null && !CanAutoCast(akTarget, akSource))
            {
                OnError($"type mismatch while assigning to a {akTarget.VarType} (cast missing or types unrelated)", akTargetToken.Line, akTargetToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C33 RID: 3123 RVA: 0x0004A618 File Offset: 0x00048818
        private ScriptVariableType CheckComparisonType(ScriptVariableType akTypeA, ScriptVariableType akTypeB, IToken akOpToken, out bool abCastToA)
        {
            var flag = akOpToken.Type != 67 && akOpToken.Type != 68;
            var flag2 = akTypeA.VarType == "none" || akTypeB.VarType == "none";
            abCastToA = true;
            if (!flag2)
            {
                if (!CanAutoCast(akTypeA, akTypeB))
                {
                    if (!CanAutoCast(akTypeB, akTypeA))
                    {
                        OnError(
                            $"cannot compare a {akTypeA.VarType} to a {akTypeB.VarType} (cast missing or types unrelated)", akOpToken.Line, akOpToken.CharPositionInLine);
                    }
                    else
                    {
                        abCastToA = false;
                    }
                }
                else if (flag && (akTypeA.IsObjectType || akTypeA.IsArray || akTypeA.VarType == "bool"))
                {
                    OnError($"cannot relatively compare variables of type {akTypeA.VarType}", akOpToken.Line, akOpToken.CharPositionInLine);
                }
            }
            else
            {
                var scriptVariableType = (akTypeA.VarType == "none") ? akTypeB : akTypeA;
                if (!scriptVariableType.IsObjectType && !scriptVariableType.IsArray && scriptVariableType.VarType != "none")
                {
                    OnError(
                        $"cannot compare a {akTypeA.VarType} to a {akTypeB.VarType} (cast missing or types unrelated)", akOpToken.Line, akOpToken.CharPositionInLine);
                }
                if (flag)
                {
                    OnError("cannot relatively compare variables to None", akOpToken.Line, akOpToken.CharPositionInLine);
                }
            }
            return new ScriptVariableType("bool");
        }

        // Token: 0x06000C34 RID: 3124 RVA: 0x0004A790 File Offset: 0x00048990
        private void HandleComparisonExpression(string asExprAVar, ScriptVariableType akExprAType, IToken akExprAToken, string asExprBVar, ScriptVariableType akExprBType, IToken akExprBToken, IToken akComparisonToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out string asResultVar, out ScriptVariableType akResultType, out IToken akResultToken, out CommonTree akATreeOut, out CommonTree akBTreeOut)
        {
            akResultType = CheckComparisonType(akExprAType, akExprBType, akComparisonToken, out var flag);
            if (flag)
            {
                akATreeOut = new CommonTree(akExprAToken);
                akBTreeOut = AutoCast(akExprBToken, akExprBType, akExprAType, akCurrentScope, akTempVars, out var asVarName);
                MarkTempVarAsUnused(akExprAType, asVarName, akComparisonToken);
            }
            else
            {
                akATreeOut = AutoCast(akExprAToken, akExprAType, akExprBType, akCurrentScope, akTempVars, out var asVarName2);
                akBTreeOut = new CommonTree(akExprBToken);
                MarkTempVarAsUnused(akExprBType, asVarName2, akComparisonToken);
            }
            MarkTempVarAsUnused(akExprAType, asExprAVar, akComparisonToken);
            MarkTempVarAsUnused(akExprBType, asExprBVar, akComparisonToken);
            asResultVar = GenerateTempVariable(akResultType, akCurrentScope, akTempVars);
            akResultToken = new CommonToken(akComparisonToken)
            {
                Type = 38,
                Text = asResultVar
            };
        }

        // Token: 0x06000C35 RID: 3125 RVA: 0x0004A848 File Offset: 0x00048A48
        private ScriptVariableType CheckAddSubtractType(ScriptVariableType akTypeA, ScriptVariableType akTypeB, IToken akOpToken, out bool abCastToA, out bool abIsConcat)
        {
            var flag = akOpToken.Type == 73;
            var flag2 = true;
            abIsConcat = (flag && (akTypeA.VarType == "string" || akTypeB.VarType == "string"));
            if (abIsConcat)
            {
                abCastToA = (akTypeA.VarType == "string");
                flag2 = abCastToA ? CanAutoCast(akTypeA, akTypeB) : CanAutoCast(akTypeB, akTypeA);
            }
            else
            {
                abCastToA = true;
                if (!CanAutoCast(akTypeA, akTypeB))
                {
                    if (!CanAutoCast(akTypeB, akTypeA))
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
                    asError = $"cannot concatenate a {akTypeA.VarType} and a {akTypeB.VarType}";
                }
                else
                {
                    var text = flag ? "add" : "subtract";
                    var text2 = flag ? "to" : "from";
                    asError =
                        $"cannot {text} a {akTypeA.VarType} {text2} a {akTypeB.VarType} (cast missing or types unrelated)";
                }
                OnError(asError, akOpToken.Line, akOpToken.CharPositionInLine);
            }
            return !abCastToA ? akTypeB : akTypeA;
        }

        // Token: 0x06000C36 RID: 3126 RVA: 0x0004A9A0 File Offset: 0x00048BA0
        private void HandleAddSubtractExpression(string asExprAVar, ScriptVariableType akExprAType, IToken akExprAToken, string asExprBVar, ScriptVariableType akExprBType, IToken akExprBToken, IToken akMathToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out bool abIsConcat, out bool abIsInt, out string asResultVar, out ScriptVariableType akResultType, out IToken akResultToken, out CommonTree akATreeOut, out CommonTree akBTreeOut)
        {
            akResultType = CheckAddSubtractType(akExprAType, akExprBType, akMathToken, out var flag, out abIsConcat);
            abIsInt = (akResultType.VarType == "int");
            if (flag)
            {
                akATreeOut = new CommonTree(akExprAToken);
                akBTreeOut = AutoCast(akExprBToken, akExprBType, akExprAType, akCurrentScope, akTempVars, out var asVarName);
                MarkTempVarAsUnused(akExprAType, asVarName, akMathToken);
            }
            else
            {
                akATreeOut = AutoCast(akExprAToken, akExprAType, akExprBType, akCurrentScope, akTempVars, out var asVarName2);
                akBTreeOut = new CommonTree(akExprBToken);
                MarkTempVarAsUnused(akExprBType, asVarName2, akMathToken);
            }
            MarkTempVarAsUnused(akExprAType, asExprAVar, akMathToken);
            MarkTempVarAsUnused(akExprBType, asExprBVar, akMathToken);
            asResultVar = GenerateTempVariable(akResultType, akCurrentScope, akTempVars);
            akResultToken = new CommonToken(akMathToken);
            akResultToken.Type = 38;
            akResultToken.Text = asResultVar;
        }

        // Token: 0x06000C37 RID: 3127 RVA: 0x0004AA70 File Offset: 0x00048C70
        private ScriptVariableType CheckMultDivideType(ScriptVariableType akTypeA, ScriptVariableType akTypeB, IToken akOpToken, out bool abCastToA)
        {
            var text = (akOpToken.Type == 75) ? "multiply" : "divide";
            var text2 = (akOpToken.Type == 75) ? "with" : "from";
            var asError =
                $"cannot {text} a {akTypeA.VarType} {text2} a {akTypeB.VarType} (cast missing or types unrelated)";
            abCastToA = true;
            if (!CanAutoCast(akTypeA, akTypeB))
            {
                if (!CanAutoCast(akTypeB, akTypeA))
                {
                    OnError(asError, akOpToken.Line, akOpToken.CharPositionInLine);
                }
                else
                {
                    abCastToA = false;
                }
            }
            else if (akTypeA.VarType != "int" && akTypeA.VarType != "float")
            {
                OnError(asError, akOpToken.Line, akOpToken.CharPositionInLine);
            }
            return !abCastToA ? akTypeB : akTypeA;
        }

        // Token: 0x06000C38 RID: 3128 RVA: 0x0004AB4C File Offset: 0x00048D4C
        private void HandleMultDivideExpression(string asExprAVar, ScriptVariableType akExprAType, IToken akExprAToken, string asExprBVar, ScriptVariableType akExprBType, IToken akExprBToken, IToken akMathToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out bool abIsInt, out string asResultVar, out ScriptVariableType akResultType, out IToken akResultToken, out CommonTree akATreeOut, out CommonTree akBTreeOut)
        {
            akResultType = CheckMultDivideType(akExprAType, akExprBType, akMathToken, out var flag);
            abIsInt = (akResultType.VarType == "int");
            if (flag)
            {
                akATreeOut = new CommonTree(akExprAToken);
                akBTreeOut = AutoCast(akExprBToken, akExprBType, akExprAType, akCurrentScope, akTempVars, out var asVarName);
                MarkTempVarAsUnused(akExprAType, asVarName, akMathToken);
            }
            else
            {
                akATreeOut = AutoCast(akExprAToken, akExprAType, akExprBType, akCurrentScope, akTempVars, out var asVarName2);
                akBTreeOut = new CommonTree(akExprBToken);
                MarkTempVarAsUnused(akExprBType, asVarName2, akMathToken);
            }
            MarkTempVarAsUnused(akExprAType, asExprAVar, akMathToken);
            MarkTempVarAsUnused(akExprBType, asExprBVar, akMathToken);
            asResultVar = GenerateTempVariable(akResultType, akCurrentScope, akTempVars);
            akResultToken = new CommonToken(akMathToken)
            {
                Type = 38,
                Text = asResultVar
            };
        }

        // Token: 0x06000C39 RID: 3129 RVA: 0x0004AC18 File Offset: 0x00048E18
        private ScriptVariableType CheckModType(ScriptVariableType akTypeA, ScriptVariableType akTypeB, IToken akOpToken)
        {
            if (akTypeA.VarType != "int" || akTypeB.VarType != "int")
            {
                OnError("Cannot calculate the modulus of non-integers", akOpToken.Line, akOpToken.CharPositionInLine);
            }
            return akTypeA;
        }

        // Token: 0x06000C3A RID: 3130 RVA: 0x0004AC58 File Offset: 0x00048E58
        private ScriptVariableType CheckNegationType(ScriptVariableType akType, IToken akOpToken)
        {
            if (akType.VarType != "int" && akType.VarType != "float")
            {
                OnError("Cannot negate a non-numeric type", akOpToken.Line, akOpToken.CharPositionInLine);
            }
            return akType;
        }

        // Token: 0x06000C3B RID: 3131 RVA: 0x0004AC98 File Offset: 0x00048E98
        private string GenerateTempVariable(ScriptVariableType akType, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
        {
            var flag = false;
            string text;
            if (akType.VarType == "none")
            {
                text = "::nonevar";
                flag = !akCurrentScope.TryGetVariable(text, out var scriptVariableType);
            }
            else if (kUnusedTempVarsByType.TryGetValue(akType.VarType, out var list) && list.Count > 0)
            {
                text = list[0];
                list.RemoveAt(0);
                if (list.Count == 0)
                {
                    kUnusedTempVarsByType.Remove(akType.VarType);
                }
            }
            else
            {
                text = $"::temp{iCurVarSuffix}";
                iCurVarSuffix++;
                flag = true;
            }

            if (!flag) return text;
            akTempVars.Add(text, akType);
            akCurrentScope.Root.TryDefineVariable(text, akType);
            return text;
        }

        // Token: 0x06000C3C RID: 3132 RVA: 0x0004AD60 File Offset: 0x00048F60
        private void MarkTempVarAsUnused(ScriptVariableType akType, string asVarName, IToken akErrorToken)
        {
            if (asVarName.Length <= 6 || asVarName[..6] != "::temp") return;
            if (!kUnusedTempVarsByType.TryGetValue(akType.VarType, out var list))
            {
                list = new List<string>();
                kUnusedTempVarsByType.Add(akType.VarType, list);
            }
            if (list.Contains(asVarName))
            {
                OnError($"Attempting to add temporary variable named {asVarName} to free list multiple times", akErrorToken.Line, akErrorToken.CharPositionInLine);
            }
            list.Add(asVarName);
        }

        // Token: 0x06000C3D RID: 3133 RVA: 0x0004ADE4 File Offset: 0x00048FE4
        private int TypeToToken(ScriptVariableType akType)
        {
            var result = 38;
            string varType;
            if ((varType = akType.VarType) == null) return result;
            if (varType != "int")
            {
                if (varType != "float")
                {
                    if (varType != "string")
                    {
                        if (varType != "bool")
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
            return result;
        }

        // Token: 0x06000C3E RID: 3134 RVA: 0x0004AE5C File Offset: 0x0004905C
        private void CheckCanBeLValue(string asName, IToken akNameToken)
        {
            var a = asName.ToLowerInvariant();
            if (a is "self" or "parent")
            {
                OnError($"variable {asName} is read-only, you cannot assign a value to it", akNameToken.Line, akNameToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C3F RID: 3135 RVA: 0x0004AEA8 File Offset: 0x000490A8
        private bool SortAndCheckFunctionParameters(ScriptFunctionType akFunction, List<string> akTargetParamNames, List<ScriptVariableType> akParamTypes, List<IToken> akParamTokens, ref List<CommonTree> akParamExpressions, out List<CommonTree> akAutoCastTrees, IToken akNameToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
        {
            var flag = true;
            var list = new List<CommonTree>(akFunction.ParamNames.Count);
            akAutoCastTrees = new List<CommonTree>(akFunction.ParamNames.Count);
            for (var i = 0; i < akFunction.ParamNames.Count; i++)
            {
                list.Add(null);
                akAutoCastTrees.Add(null);
            }
            if (akParamTypes != null)
            {
                var dictionary = new Dictionary<string, ScriptVariableType>();
                var flag2 = false;
                var flag3 = false;
                var num = 0;
                while (!flag3 && num < akParamTypes.Count)
                {
                    var flag4 = false;
                    if (num >= akFunction.ParamTypes.Count)
                    {
                        flag = false;
                        flag3 = true;
                        OnError("too many arguments passed to function", akNameToken.Line, akNameToken.CharPositionInLine);
                    }
                    else if (flag2 && akTargetParamNames[num] == "")
                    {
                        flag = false;
                        flag4 = true;
                        OnError($"argument {num + 1} must be explicitly assigned to a parameter", akNameToken.Line, akNameToken.CharPositionInLine);
                    }
                    else if (!flag2 && akTargetParamNames[num] != "")
                    {
                        flag2 = true;
                    }
                    ScriptVariableType scriptVariableType = null;
                    var num2 = num;
                    if (!flag3 && !flag4)
                    {
                        if (flag2)
                        {
                            var text = akTargetParamNames[num].ToLowerInvariant();
                            num2 = akFunction.ParamNames.IndexOf(text);
                            if (num2 == -1)
                            {
                                flag = false;
                                flag4 = true;
                                OnError($"cannot find a parameter named {text}", akNameToken.Line, akNameToken.CharPositionInLine);
                            }
                            else if (list[num2] != null)
                            {
                                flag = false;
                                flag4 = true;
                                OnError($"parameter {text} was assigned to more then once", akNameToken.Line, akNameToken.CharPositionInLine);
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
                            var text = akFunction.ParamNames[num];
                            list[num2] = akParamExpressions[num];
                        }
                    }
                    if (!flag3 && !flag4)
                    {
                        if (!CanAutoCast(scriptVariableType, akParamTypes[num]))
                        {
                            if (!scriptVariableType.IsObjectType || (scriptVariableType.IsObjectType && akParamTypes[num].VarType != "none"))
                            {
                                flag = false;
                                OnError($"type mismatch on parameter {num + 1} (did you forget a cast?)", akNameToken.Line, akNameToken.CharPositionInLine);
                            }
                        }
                        else
                        {
                            akAutoCastTrees[num2] = AutoCast(akParamTokens[num], akParamTypes[num], scriptVariableType, akCurrentScope, akTempVars, out var text2);
                            if (text2 != "")
                            {
                                dictionary.Add(text2, scriptVariableType);
                            }
                        }
                    }
                    num++;
                }
                foreach (var (key, value) in dictionary)
                {
                    MarkTempVarAsUnused(value, key, akNameToken);
                }
            }
            if (flag)
            {
                for (var j = 0; j < akFunction.ParamTypes.Count; j++)
                {
                    if (list[j] == null)
                    {
                        var scriptVariableType2 = akFunction.ParamTypes[j];
                        if (scriptVariableType2.HasInitialValue)
                        {
                            var num3 = TypeToToken(scriptVariableType2);
                            if (num3 == 38)
                            {
                                num3 = 92;
                            }
                            list[j] = new CommonTree(new CommonToken(num3, scriptVariableType2.InitialValue));
                        }
                        else
                        {
                            OnError($"argument {akFunction.ParamNames[j]} is not specified and has no default value", akNameToken.Line, akNameToken.CharPositionInLine);
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
            var result = new ScriptVariableType("none");
            var akType = new ScriptVariableType(akSelfType.ArrayElementType);
            ScriptFunctionType scriptFunctionType = null;
            switch (asName.ToLowerInvariant())
            {
                case "find":
                    scriptFunctionType = new ScriptFunctionType("find", akSelfType.VarType, "", "")
                    {
                        kRetType = new ScriptVariableType("int"),
                        bNative = true,
                        bGlobal = false
                    };
                    scriptFunctionType.TryAddParam("akElement", akType);
                    scriptFunctionType.TryAddParam("aiStartIndex", new ScriptVariableType("int")
                    {
                        InitialValue = "0"
                    });
                    break;
                case "rfind":
                    scriptFunctionType = new ScriptFunctionType("rfind", akSelfType.VarType, "", "")
                    {
                        kRetType = new ScriptVariableType("int"),
                        bNative = true,
                        bGlobal = false
                    };
                    scriptFunctionType.TryAddParam("akElement", akType);
                    scriptFunctionType.TryAddParam("aiStartIndex", new ScriptVariableType("int")
                    {
                        InitialValue = "-1"
                    });
                    break;
                default:
                    OnError($"{asName} is not a function or does not exist", akNameToken.Line, akNameToken.CharPositionInLine);
                    break;
            }
            if (scriptFunctionType != null && SortAndCheckFunctionParameters(scriptFunctionType, akTargetParamNames, akParamTypes, akParamTokens, ref akParamExpressions, out akAutoCastTrees, akNameToken, akCurrentScope, akTempVars))
            {
                result = scriptFunctionType.kRetType;
            }
            return result;
        }

        // Token: 0x06000C41 RID: 3137 RVA: 0x0004B410 File Offset: 0x00049610
        private ScriptVariableType CheckGlobalFunctionCall(ScriptVariableType akSelfType, string asName, List<string> akTargetParamNames, List<ScriptVariableType> akParamTypes, List<IToken> akParamTokens, ref List<CommonTree> akParamExpressions, out List<CommonTree> akAutoCastTrees, IToken akNameToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
        {
            akAutoCastTrees = new List<CommonTree>();
            var result = new ScriptVariableType("none");
            var knownType = GetKnownType(akSelfType.VarType);
            if (knownType != null)
            {
                if (knownType.TryGetFunction(asName, out var scriptFunctionType))
                {
                    if (scriptFunctionType.bGlobal)
                    {
                        if (SortAndCheckFunctionParameters(scriptFunctionType, akTargetParamNames, akParamTypes, akParamTokens, ref akParamExpressions, out akAutoCastTrees, akNameToken, akCurrentScope, akTempVars))
                        {
                            result = scriptFunctionType.kRetType;
                        }
                    }
                    else
                    {
                        OnError($"{asName} is not a global function", akNameToken.Line, akNameToken.CharPositionInLine);
                    }
                }
                else
                {
                    OnError($"{asName} is not a function or does not exist", akNameToken.Line, akNameToken.CharPositionInLine);
                }
            }
            else
            {
                OnError($"{akSelfType.VarType} is not a known user-defined type", akNameToken.Line, akNameToken.CharPositionInLine);
            }
            return result;
        }

        // Token: 0x06000C42 RID: 3138 RVA: 0x0004B4DC File Offset: 0x000496DC
        private ScriptVariableType CheckMemberFunctionCall(ScriptVariableType akSelfType, string asName, List<string> akTargetParamNames, List<ScriptVariableType> akParamTypes, List<IToken> akParamTokens, ref List<CommonTree> akParamExpressions, out List<CommonTree> akAutoCastTrees, IToken akNameToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars)
        {
            akAutoCastTrees = new List<CommonTree>();
            var result = new ScriptVariableType("none");
            var knownType = GetKnownType(akSelfType.VarType);
            if (knownType != null)
            {
                var scriptObjectType = knownType;
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
                        if (SortAndCheckFunctionParameters(scriptFunctionType, akTargetParamNames, akParamTypes, akParamTokens, ref akParamExpressions, out akAutoCastTrees, akNameToken, akCurrentScope, akTempVars))
                        {
                            result = scriptFunctionType.kRetType;
                        }
                    }
                    else
                    {
                        OnError($"global function {asName} cannot be called on an object", akNameToken.Line, akNameToken.CharPositionInLine);
                    }
                }
                else
                {
                    OnError($"{asName} is not a function or does not exist", akNameToken.Line, akNameToken.CharPositionInLine);
                }
            }
            else
            {
                OnError($"{akSelfType.VarType} is not a known user-defined type", akNameToken.Line, akNameToken.CharPositionInLine);
            }
            return result;
        }

        // Token: 0x06000C43 RID: 3139 RVA: 0x0004B5C0 File Offset: 0x000497C0
        private void CheckReturnType(ScriptFunctionType akFunctionType, ScriptVariableType akType, IToken akReturnToken)
        {
            if (akType != null)
            {
                if (!CanAutoCast(akFunctionType.kRetType, akType) && (!akFunctionType.kRetType.IsObjectType || (akFunctionType.kRetType.IsObjectType && akType.VarType != "none")))
                {
                    var arg = (akFunctionType.kRetType.VarType == "none") ? "the function does not return a value" : "the types do not match (cast missing or types unrelated)";
                    OnError($"cannot return a {akType.VarType} from {akFunctionType.Name}, {arg}", akReturnToken.Line, akReturnToken.CharPositionInLine);
                    return;
                }
            }
            else if (akFunctionType.kRetType.VarType != "none")
            {
                OnError($"you must return a {akFunctionType.kRetType.VarType} value from {akFunctionType.Name}", akReturnToken.Line, akReturnToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C44 RID: 3140 RVA: 0x0004B6AC File Offset: 0x000498AC
        private void CheckFunction(ScriptFunctionType akFunctionType, IToken akScriptToken)
        {
            ScriptFunctionType scriptFunctionType = null;
            var kParent = kObjType.kParent;
            var flag = false;
            if (akFunctionType.StateName != "" && kObjType.TryGetFunction(akFunctionType.Name, out scriptFunctionType))
            {
                kParent = kObjType;
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
                var text = akFunctionType.Name.ToLowerInvariant();
                if (text is "onbeginstate" or "onendstate")
                {
                    scriptFunctionType = new ScriptFunctionType(text, kObjType.Name, "", "");
                }
            }
            if (scriptFunctionType == null)
            {
                if (akFunctionType.StateName != "")
                {
                    OnError(
                        $"function {akFunctionType.Name} cannot be defined in state {akFunctionType.StateName} without also being defined in the empty state", akScriptToken.Line, akScriptToken.CharPositionInLine);
                    return;
                }
            }
            else
            {
                if (akFunctionType.bGlobal)
                {
                    OnError($"global function {akFunctionType.Name} already defined in parent script {kParent.Name}", akScriptToken.Line, akScriptToken.CharPositionInLine);
                    return;
                }
                if (kObjType.kNonOverridableFunctions.Contains(akFunctionType.Name.ToLowerInvariant()))
                {
                    OnError($"cannot override function {akFunctionType.Name}", akScriptToken.Line, akScriptToken.CharPositionInLine);
                    return;
                }
                var text2 = "";
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

                if (text2 == "") return;
                var text3 = akFunctionType.StateName == "" ? "the empty state" : $"state {akFunctionType.StateName}";
                if (flag)
                {
                    OnError(
                        $"the {text2} of function {akFunctionType.Name} in {text3} on script {kObjType.Name} does not match the empty state", akScriptToken.Line, akScriptToken.CharPositionInLine);
                    return;
                }
                OnError(
                    $"the {text2} of function {akFunctionType.Name} in {text3} on script {kObjType.Name} do not match the parent script {kParent.Name}", akScriptToken.Line, akScriptToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C45 RID: 3141 RVA: 0x0004B948 File Offset: 0x00049B48
        private ScriptVariableType CheckCast(ScriptVariableType akSourceType, ScriptVariableType akTargetType, IToken akCastToken)
        {
            var flag = true;
            if (akTargetType.VarType != akSourceType.VarType)
            {
                string varType;
                if ((varType = akSourceType.VarType) != null)
                {
                    switch (varType)
                    {
                        case "none":
                            flag = akTargetType.VarType is "string" or "bool";
                            goto IL_231;
                        case "string":
                            flag = (!akTargetType.IsObjectType && !akTargetType.IsArray);
                            goto IL_231;
                        case "int":
                            flag = (!akTargetType.IsObjectType && !akTargetType.IsArray);
                            goto IL_231;
                        case "float":
                            flag = (!akTargetType.IsObjectType && !akTargetType.IsArray);
                            goto IL_231;
                        case "bool":
                            flag = (!akTargetType.IsObjectType && !akTargetType.IsArray);
                            goto IL_231;
                    }
                }
                if (akSourceType.IsArray)
                {
                    flag = akTargetType.VarType is "none" or "string" or "bool";
                }
                else if (akTargetType.VarType != akSourceType.VarType && akTargetType.VarType != "none" && akTargetType.VarType != "string" && akTargetType.VarType != "bool")
                {
                    if (!akTargetType.IsObjectType)
                    {
                        flag = false;
                    }
                    else
                    {
                        var knownType = GetKnownType(akTargetType.VarType);
                        var knownType2 = GetKnownType(akSourceType.VarType);
                        if (knownType == null)
                        {
                            flag = false;
                            OnError($"cannot convert to unknown type {akTargetType.VarType}", akCastToken.Line, akCastToken.CharPositionInLine);
                        }
                        else if (knownType2 == null)
                        {
                            flag = false;
                            OnError($"cannot convert from unknown type {akSourceType.VarType}", akCastToken.Line, akCastToken.CharPositionInLine);
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
                OnError($"cannot cast a {akSourceType.VarType} to a {akTargetType.VarType}, types are incompatible", akCastToken.Line, akCastToken.CharPositionInLine);
            }
            return akTargetType;
        }

        // Token: 0x06000C46 RID: 3142 RVA: 0x0004BBB4 File Offset: 0x00049DB4
        private void CheckPropertyOverride(string asPropName, IToken akSourceToken)
        {
            var kParent = kObjType.kParent;
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
                OnError($"script property {asPropName} already defined on parent {kParent.Name}", akSourceToken.Line, akSourceToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C47 RID: 3143 RVA: 0x0004BC10 File Offset: 0x00049E10
        private void CheckPropertyFunction(string asPropName, string asFuncName, out bool abIsGet, IToken akSourceToken)
        {
            abIsGet = false;
            if (kObjType.TryGetVariable(asPropName.ToLowerInvariant(), out var scriptVariableType))
            {
                OnError($"property {asPropName} cannot have the same name as a variable", akSourceToken.Line, akSourceToken.CharPositionInLine);
                return;
            }
            if (kObjType.TryGetProperty(asPropName.ToLowerInvariant(), out var scriptPropertyType))
            {
                ScriptFunctionType scriptFunctionType = null;
                var a = asFuncName.ToLowerInvariant();
                var arg = "";
                ScriptFunctionType scriptFunctionType2 = null;
                switch (a)
                {
                    case "get":
                        scriptFunctionType = scriptPropertyType.kGetFunction;
                        scriptFunctionType2 = new ScriptFunctionType("dummy", kObjType.Name, "", asPropName)
                        {
                            kRetType = scriptPropertyType.kType
                        };
                        arg = "getter";
                        abIsGet = true;
                        break;
                    case "set":
                        scriptFunctionType = scriptPropertyType.kSetFunction;
                        scriptFunctionType2 = new ScriptFunctionType("dummy", kObjType.Name, "", asPropName);
                        scriptFunctionType2.TryAddParam("value", scriptPropertyType.kType);
                        arg = "setter";
                        break;
                    default:
                        OnError($"Function {asFuncName} on property {asPropName} must be either a get or set", akSourceToken.Line, akSourceToken.CharPositionInLine);
                        break;
                }

                if (scriptFunctionType2 == null || scriptFunctionType == null) return;
                var text = "";
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
                    OnError($"{arg} for property {asPropName} does not have the correct {text}", akSourceToken.Line, akSourceToken.CharPositionInLine);
                    return;
                }
            }
            else
            {
                OnError($"internal error: cannot find property {asPropName}", akSourceToken.Line, akSourceToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C48 RID: 3144 RVA: 0x0004BDD4 File Offset: 0x00049FD4
        private CommonTree CreateBlockTree(IToken akBlockToken, IList akStatements, Dictionary<string, ScriptVariableType> akTempVars)
        {
            var commonTree = new CommonTree(akBlockToken);
            foreach (var text in akTempVars.Keys)
            {
                var scriptVariableType = akTempVars[text];
                var commonToken = new CommonToken(akBlockToken)
                {
                    Type = scriptVariableType.IsObjectType ? 38 : 55,
                    Text = scriptVariableType.VarType
                };
                var commonToken2 = new CommonToken(akBlockToken)
                {
                    Type = 38,
                    Text = text
                };
                var commonTree2 = new CommonTree(new CommonToken(akBlockToken)
                {
                    Type = 5,
                    Text = "VAR"
                });
                commonTree2.AddChild(new CommonTree(commonToken));
                commonTree2.AddChild(new CommonTree(commonToken2));
                commonTree.AddChild(commonTree2);
            }

            if (akStatements == null) return commonTree;
            foreach (var obj in akStatements)
            {
                var t = (ITree)obj;
                commonTree.AddChild(t);
            }
            return commonTree;
        }

        // Token: 0x06000C49 RID: 3145 RVA: 0x0004BF14 File Offset: 0x0004A114
        private CommonTree CreateCallTree(IToken akCallToken, bool abIsGlobal, bool abIsArray, string asSelfVar, IToken akNameToken, ScriptVariableType akRetVarType, string asRetVarName, List<CommonTree> akParamAutocasts, List<CommonTree> akParamExpressions)
        {
            var commonTree = new CommonTree(new CommonToken(14, "CALLPARAMS"));
            if (akParamExpressions.Count == akParamAutocasts.Count)
            {
                for (var i = 0; i < akParamExpressions.Count; i++)
                {
                    var commonTree2 = new CommonTree(new CommonToken(9, "PARAM"));
                    commonTree2.AddChild(akParamAutocasts[i]);
                    commonTree2.AddChild(akParamExpressions[i]);
                    commonTree.AddChild(commonTree2);
                }
            }
            var flag = asSelfVar.ToLowerInvariant() == "parent";
            IToken token = new CommonToken(akNameToken)
            {
                Type = 38
            };
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
            var commonTree3 = new CommonTree(akCallToken);
            if (abIsArray)
            {
                switch (akNameToken.Text.ToLowerInvariant())
                {
                    case "find":
                        akCallToken.Type = 24;
                        akCallToken.Text = "arrayfind";
                        break;
                    case "rfind":
                        akCallToken.Type = 25;
                        akCallToken.Text = "arrayrfind";
                        break;
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
            if (!IsKnownType(new ScriptVariableType(akTypeToken.Text)))
            {
                OnError($"unknown type {akTypeToken.Text}", akTypeToken.Line, akTypeToken.CharPositionInLine);
            }
            if (!int.TryParse(akSizeToken.Text, out var num) || num <= 0)
            {
                OnError($"Array size of {akSizeToken.Text} is invalid. Must be greater than 0", akSizeToken.Line, akSizeToken.CharPositionInLine);
            }
        }

        // Token: 0x06000C4B RID: 3147 RVA: 0x0004C154 File Offset: 0x0004A354
        private void HandleArrayElementExpression(string asArrayVar, ScriptVariableType akArrayType, IToken akArrayToken, string asExprVar, ScriptVariableType akExprType, IToken akExprToken, ScriptScope akCurrentScope, Dictionary<string, ScriptVariableType> akTempVars, out string asResultVar, out ScriptVariableType akResultType, out IToken akResultToken, out CommonTree akTreeOut)
        {
            var scriptVariableType = new ScriptVariableType("int");
            if (CanAutoCast(scriptVariableType, akExprType))
            {
                akTreeOut = AutoCast(akExprToken, akExprType, scriptVariableType, akCurrentScope, akTempVars, out var asVarName);
                MarkTempVarAsUnused(scriptVariableType, asVarName, akArrayToken);
            }
            else
            {
                akTreeOut = new CommonTree(akExprToken);
                OnError("arrays can only be indexed with integers", akArrayToken.Line, akArrayToken.CharPositionInLine);
            }
            if (akArrayType.IsArray)
            {
                akResultType = new ScriptVariableType(akArrayType.ArrayElementType);
            }
            else
            {
                akResultType = new ScriptVariableType("none");
                OnError("only arrays can be indexed", akArrayToken.Line, akArrayToken.CharPositionInLine);
            }
            asResultVar = GenerateTempVariable(akResultType, akCurrentScope, akTempVars);
            MarkTempVarAsUnused(akArrayType, asArrayVar, akArrayToken);
            MarkTempVarAsUnused(akExprType, asExprVar, akArrayToken);
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
            InitializeCyclicDFAs();
        }

        // Token: 0x17000172 RID: 370
        // (get) Token: 0x06000C4E RID: 3150 RVA: 0x0004C36C File Offset: 0x0004A56C
        // (set) Token: 0x06000C4F RID: 3151 RVA: 0x0004C374 File Offset: 0x0004A574
        public ITreeAdaptor TreeAdaptor
        {
            get => adaptor;
            set => adaptor = value;
        }

        // Token: 0x17000173 RID: 371
        // (get) Token: 0x06000C50 RID: 3152 RVA: 0x0004C380 File Offset: 0x0004A580
        public override string[] TokenNames => tokenNames;

        // Token: 0x17000174 RID: 372
        // (get) Token: 0x06000C51 RID: 3153 RVA: 0x0004C388 File Offset: 0x0004A588
        public override string GrammarFileName => "PapyrusTypeWalker.g";

        // Token: 0x14000039 RID: 57
        // (add) Token: 0x06000C52 RID: 3154 RVA: 0x0004C390 File Offset: 0x0004A590
        // (remove) Token: 0x06000C53 RID: 3155 RVA: 0x0004C3AC File Offset: 0x0004A5AC
        internal event InternalErrorEventHandler ErrorHandler;

        // Token: 0x06000C54 RID: 3156 RVA: 0x0004C3C8 File Offset: 0x0004A5C8
        private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
        {
            ErrorHandler?.Invoke(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
        }

        // Token: 0x06000C55 RID: 3157 RVA: 0x0004C3E8 File Offset: 0x0004A5E8
        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            var errorMessage = GetErrorMessage(e, tokenNames);
            OnError(errorMessage, e.Line, e.CharPositionInLine);
        }

        // Token: 0x06000C56 RID: 3158 RVA: 0x0004C414 File Offset: 0x0004A614
        public script_return script(ScriptObjectType akObj, Compiler akCompiler, Dictionary<string, ScriptObjectType> akKnownTypes, Stack<string> akChildNames)
        {
            var script_return = new script_return
            {
                Start = input.LT(1)
            };
            kKnownTypes = akKnownTypes;
            AddKnownType("int");
            AddKnownType("float");
            AddKnownType("string");
            AddKnownType("bool");
            AddKnownType(akObj);
            AddImportedType(akObj);
            kObjType = akObj;
            kCompiler = akCompiler;
            kChildren = akChildNames;
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 4, FOLLOW_OBJECT_in_script81);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_header_in_script83);
                var header_return = header();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, header_return.Tree);
                for (; ; )
                {
                    var num = 2;
                    var num2 = input.LA(1);
                    if (num2 is >= 5 and <= 7 or 19 or 42 or 51 or 54)
                    {
                        num = 1;
                    }
                    var num3 = num;
                    if (num3 != 1)
                    {
                        break;
                    }
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_definitionOrBlock_in_script85);
                    var definitionOrBlock_return = definitionOrBlock();
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, definitionOrBlock_return.Tree);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                script_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
                kObjType.kAST = (CommonTree)script_return.Tree;
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return script_return;
        }

        // Token: 0x06000C57 RID: 3159 RVA: 0x0004C69C File Offset: 0x0004A89C
        public header_return header()
        {
            var header_return = new header_return
            {
                Start = input.LT(1)
            };
            CommonTree commonTree = null;
            try
            {
                var commonTree2 = (CommonTree)adaptor.GetNilNode();
                var commonTree3 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)adaptor.GetNilNode();
                commonTree3 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 38, FOLLOW_ID_in_header101);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree4 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree4);
                Match(input, 2, null);
                commonTree3 = (CommonTree)input.LT(1);
                var treeNode2 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_header103);
                var child = (CommonTree)adaptor.DupNode(treeNode2);
                adaptor.AddChild(commonTree4, child);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 == 38)
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree3 = (CommonTree)input.LT(1);
                    commonTree = (CommonTree)Match(input, 38, FOLLOW_ID_in_header107);
                    var child2 = (CommonTree)adaptor.DupNode(commonTree);
                    adaptor.AddChild(commonTree4, child2);
                }
                var num4 = 2;
                var num5 = input.LA(1);
                if (num5 == 40)
                {
                    num4 = 1;
                }
                var num6 = num4;
                if (num6 == 1)
                {
                    commonTree3 = (CommonTree)input.LT(1);
                    var treeNode3 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_header110);
                    var child3 = (CommonTree)adaptor.DupNode(treeNode3);
                    adaptor.AddChild(commonTree4, child3);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree2, commonTree4);
                if (commonTree != null)
                {
                    HandleParent(commonTree.Token);
                }
                header_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree2);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return header_return;
        }

        // Token: 0x06000C58 RID: 3160 RVA: 0x0004C92C File Offset: 0x0004AB2C
        public definitionOrBlock_return definitionOrBlock()
        {
            var definitionOrBlock_return = new definitionOrBlock_return
            {
                Start = input.LT(1)
            };
            CommonTree commonTree = null;
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule import_obj");
            try
            {
                var num = input.LA(1);
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
                var ex = new NoViableAltException("", 4, 0, input);
                throw ex;
            IL_B9:
                switch (num2)
                {
                    case 1:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_fieldDefinition_in_definitionOrBlock130);
                            var fieldDefinition_return = fieldDefinition();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, fieldDefinition_return.Tree);
                            break;
                        }
                    case 2:
                        {
                            var commonTree3 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_import_obj_in_definitionOrBlock136);
                            var import_obj_return = import_obj();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(import_obj_return.Tree);
                            definitionOrBlock_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (definitionOrBlock_return != null) ? definitionOrBlock_return.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            commonTree = null;
                            definitionOrBlock_return.Tree = null;
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_function_in_definitionOrBlock148);
                            var function_return = function("", "");
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, function_return.Tree);
                            break;
                        }
                    case 4:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree5 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_eventFunc_in_definitionOrBlock156);
                            var eventFunc_return = eventFunc("");
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, eventFunc_return.Tree);
                            break;
                        }
                    case 5:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree6 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_stateBlock_in_definitionOrBlock164);
                            var stateBlock_return = stateBlock();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, stateBlock_return.Tree);
                            break;
                        }
                    case 6:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree7 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_propertyBlock_in_definitionOrBlock170);
                            var propertyBlock_return = propertyBlock();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, propertyBlock_return.Tree);
                            break;
                        }
                }
                definitionOrBlock_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return definitionOrBlock_return;
        }

        // Token: 0x06000C59 RID: 3161 RVA: 0x0004CCE0 File Offset: 0x0004AEE0
        public fieldDefinition_return fieldDefinition()
        {
            var fieldDefinition_return = new fieldDefinition_return();
            fieldDefinition_return.Start = input.LT(1);
            constant_return constant_return = null;
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 5, FOLLOW_VAR_in_fieldDefinition184);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_type_in_fieldDefinition186);
                var type_return = type();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, type_return.Tree);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_fieldDefinition190);
                var child = (CommonTree)adaptor.DupNode(commonTree4);
                adaptor.AddChild(commonTree3, child);
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode2 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_fieldDefinition192);
                var child2 = (CommonTree)adaptor.DupNode(treeNode2);
                adaptor.AddChild(commonTree3, child2);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 == 81 || (num2 >= 90 && num2 <= 93))
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_constant_in_fieldDefinition194);
                    constant_return = constant();
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, constant_return.Tree);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                CheckVariableDefinition(commonTree4.Text, type_return.kType,
                    ((CommonTree)constant_return?.Start)?.Token,
                    false, commonTree4.Token);
                fieldDefinition_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return fieldDefinition_return;
        }

        // Token: 0x06000C5A RID: 3162 RVA: 0x0004D004 File Offset: 0x0004B204
        public import_obj_return import_obj()
        {
            var import_obj_return = new import_obj_return
            {
                Start = input.LT(1)
            };
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 42, FOLLOW_IMPORT_in_import_obj215);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_import_obj217);
                var child = (CommonTree)adaptor.DupNode(commonTree4);
                adaptor.AddChild(commonTree3, child);
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                var scriptObjectType = kCompiler.LoadObject(commonTree4.Text, kKnownTypes);
                if (scriptObjectType != null)
                {
                    AddImportedType(scriptObjectType);
                }
                import_obj_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return import_obj_return;
        }

        // Token: 0x06000C5B RID: 3163 RVA: 0x0004D1B8 File Offset: 0x0004B3B8
        public function_return function(string asStateName, string asPropertyName)
        {
            function_stack.Push(new function_scope());
            var function_return = new function_return();
            function_return.Start = input.LT(1);
            ((function_scope)function_stack.Peek()).sstateName = asStateName;
            ((function_scope)function_stack.Peek()).spropertyName = asPropertyName;
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 6, FOLLOW_FUNCTION_in_function256);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_functionHeader_in_function258);
                var functionHeader_return = functionHeader();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, functionHeader_return.Tree);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 == 10)
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_codeBlock_in_function260);
                    var codeBlock_return = codeBlock(((function_scope)function_stack.Peek()).kfunctionType, ((function_scope)function_stack.Peek()).kfunctionType.FunctionScope);
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, codeBlock_return.Tree);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                function_return.sName = (functionHeader_return.sFuncName);
                function_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
                if (((function_scope)function_stack.Peek()).spropertyName == "")
                {
                    CheckFunction(((function_scope)function_stack.Peek()).kfunctionType, ((CommonTree)function_return.Start).Token);
                }
                kUnusedTempVarsByType.Clear();
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                function_stack.Pop();
            }
            return function_return;
        }

        // Token: 0x06000C5C RID: 3164 RVA: 0x0004D4B0 File Offset: 0x0004B6B0
        public functionHeader_return functionHeader()
        {
            var functionHeader_return = new functionHeader_return
            {
                Start = input.LT(1)
            };
            CommonTree commonTree = null;
            try
            {
                var num = input.LA(1);
                if (num != 8)
                {
                    var ex = new NoViableAltException("", 13, 0, input);
                    throw ex;
                }
                var num2 = input.LA(2);
                if (num2 != 2)
                {
                    var ex2 = new NoViableAltException("", 13, 1, input);
                    throw ex2;
                }
                var num3 = input.LA(3);
                int num4;
                if (num3 == 92)
                {
                    num4 = 2;
                }
                else
                {
                    if (num3 != 38 && num3 != 55)
                    {
                        var ex3 = new NoViableAltException("", 13, 2, input);
                        throw ex3;
                    }
                    num4 = 1;
                }
                switch (num4)
                {
                    case 1:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var treeNode = (CommonTree)Match(input, 8, FOLLOW_HEADER_in_functionHeader290);
                            var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                            commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_type_in_functionHeader292);
                            var type_return = type();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree3, type_return.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_functionHeader296);
                            var child = (CommonTree)adaptor.DupNode(commonTree4);
                            adaptor.AddChild(commonTree3, child);
                            commonTree2 = (CommonTree)input.LT(1);
                            var treeNode2 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_functionHeader298);
                            var child2 = (CommonTree)adaptor.DupNode(treeNode2);
                            adaptor.AddChild(commonTree3, child2);
                            var num5 = 2;
                            var num6 = input.LA(1);
                            if (num6 == 9)
                            {
                                num5 = 1;
                            }
                            var num7 = num5;
                            if (num7 == 1)
                            {
                                commonTree2 = (CommonTree)input.LT(1);
                                PushFollow(FOLLOW_callParameters_in_functionHeader300);
                                var callParameters_return = callParameters();
                                state.followingStackPointer--;
                                adaptor.AddChild(commonTree3, callParameters_return.Tree);
                            }
                            for (; ; )
                            {
                                var num8 = 2;
                                var num9 = input.LA(1);
                                if (num9 >= 46 && num9 <= 47)
                                {
                                    num8 = 1;
                                }
                                var num10 = num8;
                                if (num10 != 1)
                                {
                                    break;
                                }
                                commonTree2 = (CommonTree)input.LT(1);
                                PushFollow(FOLLOW_functionModifier_in_functionHeader303);
                                var functionModifier_return = functionModifier();
                                state.followingStackPointer--;
                                adaptor.AddChild(commonTree3, functionModifier_return.Tree);
                            }
                            var num11 = 2;
                            var num12 = input.LA(1);
                            if (num12 == 40)
                            {
                                num11 = 1;
                            }
                            var num13 = num11;
                            if (num13 == 1)
                            {
                                commonTree2 = (CommonTree)input.LT(1);
                                var treeNode3 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_functionHeader306);
                                var child3 = (CommonTree)adaptor.DupNode(treeNode3);
                                adaptor.AddChild(commonTree3, child3);
                            }
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, commonTree3);
                            functionHeader_return.sFuncName = commonTree4.Text;
                            CheckTypeAndValue(type_return.kType, null, commonTree4.Token);
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree5 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var treeNode4 = (CommonTree)Match(input, 8, FOLLOW_HEADER_in_functionHeader320);
                            var newRoot2 = (CommonTree)adaptor.DupNode(treeNode4);
                            commonTree5 = (CommonTree)adaptor.BecomeRoot(newRoot2, commonTree5);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            var treeNode5 = (CommonTree)Match(input, 92, FOLLOW_NONE_in_functionHeader322);
                            var child4 = (CommonTree)adaptor.DupNode(treeNode5);
                            adaptor.AddChild(commonTree5, child4);
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_functionHeader324);
                            var child5 = (CommonTree)adaptor.DupNode(commonTree6);
                            adaptor.AddChild(commonTree5, child5);
                            commonTree2 = (CommonTree)input.LT(1);
                            var treeNode6 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_functionHeader326);
                            var child6 = (CommonTree)adaptor.DupNode(treeNode6);
                            adaptor.AddChild(commonTree5, child6);
                            var num14 = 2;
                            var num15 = input.LA(1);
                            if (num15 == 9)
                            {
                                num14 = 1;
                            }
                            var num16 = num14;
                            if (num16 == 1)
                            {
                                commonTree2 = (CommonTree)input.LT(1);
                                PushFollow(FOLLOW_callParameters_in_functionHeader328);
                                var callParameters_return2 = callParameters();
                                state.followingStackPointer--;
                                adaptor.AddChild(commonTree5, callParameters_return2.Tree);
                            }
                            for (; ; )
                            {
                                var num17 = 2;
                                var num18 = input.LA(1);
                                if (num18 >= 46 && num18 <= 47)
                                {
                                    num17 = 1;
                                }
                                var num19 = num17;
                                if (num19 != 1)
                                {
                                    break;
                                }
                                commonTree2 = (CommonTree)input.LT(1);
                                PushFollow(FOLLOW_functionModifier_in_functionHeader331);
                                var functionModifier_return2 = functionModifier();
                                state.followingStackPointer--;
                                adaptor.AddChild(commonTree5, functionModifier_return2.Tree);
                            }
                            var num20 = 2;
                            var num21 = input.LA(1);
                            if (num21 == 40)
                            {
                                num20 = 1;
                            }
                            var num22 = num20;
                            if (num22 == 1)
                            {
                                commonTree2 = (CommonTree)input.LT(1);
                                var treeNode7 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_functionHeader334);
                                var child7 = (CommonTree)adaptor.DupNode(treeNode7);
                                adaptor.AddChild(commonTree5, child7);
                            }
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, commonTree5);
                            functionHeader_return.sFuncName = commonTree6.Text;
                            break;
                        }
                }
                functionHeader_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
                if (((function_scope)function_stack.Peek()).spropertyName == "")
                {
                    kObjType.TryGetFunction(((function_scope)function_stack.Peek()).sstateName, functionHeader_return.sFuncName, out ((function_scope)function_stack.Peek()).kfunctionType);
                }
                else
                {
                    kObjType.TryGetProperty(((function_scope)function_stack.Peek()).spropertyName, out var scriptPropertyType);
                    var a = functionHeader_return.sFuncName.ToLowerInvariant();
                    ((function_scope)function_stack.Peek()).kfunctionType = a == "get" ? scriptPropertyType.kGetFunction : scriptPropertyType.kSetFunction;
                }
            }
            catch (RecognitionException ex4)
            {
                ReportError(ex4);
                Recover(input, ex4);
            }
            return functionHeader_return;
        }

        // Token: 0x06000C5D RID: 3165 RVA: 0x0004DD30 File Offset: 0x0004BF30
        public functionModifier_return functionModifier()
        {
            var functionModifier_return = new functionModifier_return();
            functionModifier_return.Start = input.LT(1);
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)input.LT(1);
                if (input.LA(1) < 46 || input.LA(1) > 47)
                {
                    var ex = new MismatchedSetException(null, input);
                    throw ex;
                }
                input.Consume();
                var child = (CommonTree)adaptor.DupNode(treeNode);
                adaptor.AddChild(commonTree, child);
                state.errorRecovery = false;
                functionModifier_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return functionModifier_return;
        }

        // Token: 0x06000C5E RID: 3166 RVA: 0x0004DE40 File Offset: 0x0004C040
        public eventFunc_return eventFunc(string asStateName)
        {
            eventFunc_stack.Push(new eventFunc_scope());
            var eventFunc_return = new eventFunc_return
            {
                Start = input.LT(1)
            };
            ((eventFunc_scope)eventFunc_stack.Peek()).sstateName = asStateName;
            ((eventFunc_scope)eventFunc_stack.Peek()).seventName = "";
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 7, FOLLOW_EVENT_in_eventFunc388);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_eventHeader_in_eventFunc390);
                var eventHeader_return = eventHeader();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, eventHeader_return.Tree);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 == 10)
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_codeBlock_in_eventFunc392);
                    var codeBlock_return = codeBlock(((eventFunc_scope)eventFunc_stack.Peek()).kfunctionType, ((eventFunc_scope)eventFunc_stack.Peek()).kfunctionType.FunctionScope);
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, codeBlock_return.Tree);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                eventFunc_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
                CheckFunction(((eventFunc_scope)eventFunc_stack.Peek()).kfunctionType, ((CommonTree)eventFunc_return.Start).Token);
                kUnusedTempVarsByType.Clear();
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                eventFunc_stack.Pop();
            }
            return eventFunc_return;
        }

        // Token: 0x06000C5F RID: 3167 RVA: 0x0004E108 File Offset: 0x0004C308
        public eventHeader_return eventHeader()
        {
            var eventHeader_return = new eventHeader_return();
            eventHeader_return.Start = input.LT(1);
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 8, FOLLOW_HEADER_in_eventHeader408);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode2 = (CommonTree)Match(input, 92, FOLLOW_NONE_in_eventHeader410);
                var child = (CommonTree)adaptor.DupNode(treeNode2);
                adaptor.AddChild(commonTree3, child);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_eventHeader412);
                var child2 = (CommonTree)adaptor.DupNode(commonTree4);
                adaptor.AddChild(commonTree3, child2);
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode3 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_eventHeader414);
                var child3 = (CommonTree)adaptor.DupNode(treeNode3);
                adaptor.AddChild(commonTree3, child3);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 == 9)
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_callParameters_in_eventHeader416);
                    var callParameters_return = callParameters();
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, callParameters_return.Tree);
                }
                var num4 = 2;
                var num5 = input.LA(1);
                if (num5 == 47)
                {
                    num4 = 1;
                }
                var num6 = num4;
                if (num6 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    var treeNode4 = (CommonTree)Match(input, 47, FOLLOW_NATIVE_in_eventHeader419);
                    var child4 = (CommonTree)adaptor.DupNode(treeNode4);
                    adaptor.AddChild(commonTree3, child4);
                }
                var num7 = 2;
                var num8 = input.LA(1);
                if (num8 == 40)
                {
                    num7 = 1;
                }
                var num9 = num7;
                if (num9 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    var treeNode5 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_eventHeader422);
                    var child5 = (CommonTree)adaptor.DupNode(treeNode5);
                    adaptor.AddChild(commonTree3, child5);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                ((eventFunc_scope)eventFunc_stack.Peek()).seventName = commonTree4.Text;
                kObjType.TryGetFunction(((eventFunc_scope)eventFunc_stack.Peek()).sstateName, ((eventFunc_scope)eventFunc_stack.Peek()).seventName, out ((eventFunc_scope)eventFunc_stack.Peek()).kfunctionType);
                eventHeader_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return eventHeader_return;
        }

        // Token: 0x06000C60 RID: 3168 RVA: 0x0004E508 File Offset: 0x0004C708
        public callParameters_return callParameters()
        {
            var callParameters_return = new callParameters_return();
            callParameters_return.Start = input.LT(1);
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var num = 0;
                for (; ; )
                {
                    var num2 = 2;
                    var num3 = input.LA(1);
                    if (num3 == 9)
                    {
                        num2 = 1;
                    }
                    var num4 = num2;
                    if (num4 != 1)
                    {
                        break;
                    }
                    var commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_callParameter_in_callParameters442);
                    var callParameter_return = callParameter();
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree, callParameter_return.Tree);
                    num++;
                }
                if (num < 1)
                {
                    var ex = new EarlyExitException(18, input);
                    throw ex;
                }
                callParameters_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return callParameters_return;
        }

        // Token: 0x06000C61 RID: 3169 RVA: 0x0004E618 File Offset: 0x0004C818
        public callParameter_return callParameter()
        {
            var callParameter_return = new callParameter_return();
            callParameter_return.Start = input.LT(1);
            constant_return constant_return = null;
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 9, FOLLOW_PARAM_in_callParameter456);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_type_in_callParameter458);
                var type_return = type();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, type_return.Tree);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_callParameter462);
                var child = (CommonTree)adaptor.DupNode(commonTree4);
                adaptor.AddChild(commonTree3, child);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 == 81 || (num2 >= 90 && num2 <= 93))
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_constant_in_callParameter464);
                    constant_return = constant();
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, constant_return.Tree);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                if ((CommonTree)constant_return?.Start != null)
                {
                    CheckTypeAndValue(type_return.kType, (((CommonTree)constant_return.Start)).Token, commonTree4.Token);
                }
                else
                {
                    CheckTypeAndValue(type_return.kType, null, commonTree4.Token);
                }
                callParameter_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return callParameter_return;
        }

        // Token: 0x06000C62 RID: 3170 RVA: 0x0004E8D8 File Offset: 0x0004CAD8
        public stateBlock_return stateBlock()
        {
            var stateBlock_return = new stateBlock_return();
            stateBlock_return.Start = input.LT(1);
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 51, FOLLOW_STATE_in_stateBlock485);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_stateBlock487);
                var child = (CommonTree)adaptor.DupNode(commonTree4);
                adaptor.AddChild(commonTree3, child);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 == 50)
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    var treeNode2 = (CommonTree)Match(input, 50, FOLLOW_AUTO_in_stateBlock489);
                    var child2 = (CommonTree)adaptor.DupNode(treeNode2);
                    adaptor.AddChild(commonTree3, child2);
                }
                for (; ; )
                {
                    var num4 = 2;
                    var num5 = input.LA(1);
                    if (num5 >= 6 && num5 <= 7)
                    {
                        num4 = 1;
                    }
                    var num6 = num4;
                    if (num6 != 1)
                    {
                        break;
                    }
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_stateFuncOrEvent_in_stateBlock493);
                    var stateFuncOrEvent_return = stateFuncOrEvent(commonTree4.Text);
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, stateFuncOrEvent_return.Tree);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                stateBlock_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return stateBlock_return;
        }

        // Token: 0x06000C63 RID: 3171 RVA: 0x0004EB60 File Offset: 0x0004CD60
        public stateFuncOrEvent_return stateFuncOrEvent(string asState)
        {
            var stateFuncOrEvent_return = new stateFuncOrEvent_return
            {
                Start = input.LT(1)
            };
            CommonTree commonTree = null;
            try
            {
                var num = input.LA(1);
                int num2;
                if (num == 6)
                {
                    num2 = 1;
                }
                else
                {
                    if (num != 7)
                    {
                        var ex = new NoViableAltException("", 22, 0, input);
                        throw ex;
                    }
                    num2 = 2;
                }
                switch (num2)
                {
                    case 1:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_function_in_stateFuncOrEvent510);
                            var function_return = function(asState, "");
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, function_return.Tree);
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree3 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_eventFunc_in_stateFuncOrEvent518);
                            var eventFunc_return = eventFunc(asState);
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, eventFunc_return.Tree);
                            break;
                        }
                }
                stateFuncOrEvent_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return stateFuncOrEvent_return;
        }

        // Token: 0x06000C64 RID: 3172 RVA: 0x0004ECF0 File Offset: 0x0004CEF0
        public propertyBlock_return propertyBlock()
        {
            propertyBlock_stack.Push(new propertyBlock_scope());
            var propertyBlock_return = new propertyBlock_return
            {
                Start = input.LT(1)
            };
            CommonTree commonTree = null;
            propertyFunc_return propertyFunc_return = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token PROPERTY");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule propertyFunc");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule propertyHeader");
            ((propertyBlock_scope)propertyBlock_stack.Peek()).bfunc0IsGet = false;
            try
            {
                var num = input.LA(1);
                int num2;
                if (num == 54)
                {
                    num2 = 1;
                }
                else
                {
                    if (num != 19)
                    {
                        var ex = new NoViableAltException("", 24, 0, input);
                        throw ex;
                    }
                    num2 = 2;
                }
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 54, FOLLOW_PROPERTY_in_propertyBlock543);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_propertyHeader_in_propertyBlock545);
                            var propertyHeader_return = propertyHeader();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(propertyHeader_return.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_propertyFunc_in_propertyBlock549);
                            var propertyFunc_return2 = propertyFunc(propertyHeader_return.sName);
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(propertyFunc_return2.Tree);
                            var num3 = 2;
                            var num4 = input.LA(1);
                            if (num4 == 17)
                            {
                                num3 = 1;
                            }
                            var num5 = num3;
                            if (num5 == 1)
                            {
                                commonTree2 = (CommonTree)input.LT(1);
                                PushFollow(FOLLOW_propertyFunc_in_propertyBlock554);
                                propertyFunc_return = propertyFunc(propertyHeader_return.sName);
                                state.followingStackPointer--;
                                rewriteRuleSubtreeStream.Add(propertyFunc_return.Tree);
                            }
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            CheckPropertyOverride(propertyHeader_return.sName, commonTree3.Token);
                            ((propertyBlock_scope)propertyBlock_stack.Peek()).bfunc0IsGet = (propertyFunc_return2.bIsGet);
                            propertyBlock_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", propertyBlock_return.Tree);
                            var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule func0", propertyFunc_return2.Tree);
                            var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule func1", propertyFunc_return?.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            if (((propertyBlock_scope)propertyBlock_stack.Peek()).bfunc0IsGet && (CommonTree)propertyFunc_return?.Start != null)
                            {
                                var commonTree4 = (CommonTree)adaptor.GetNilNode();
                                commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
                                adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream2.NextTree());
                                adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
                                adaptor.AddChild(commonTree, commonTree4);
                            }
                            else if (!((propertyBlock_scope)propertyBlock_stack.Peek()).bfunc0IsGet && (CommonTree)propertyFunc_return?.Start != null)
                            {
                                var commonTree5 = (CommonTree)adaptor.GetNilNode();
                                commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
                                adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream2.NextTree());
                                adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream4.NextTree());
                                adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree, commonTree5);
                            }
                            else if (((propertyBlock_scope)propertyBlock_stack.Peek()).bfunc0IsGet)
                            {
                                var commonTree6 = (CommonTree)adaptor.GetNilNode();
                                commonTree6 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree6);
                                adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream2.NextTree());
                                adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree6, (CommonTree)adaptor.Create(17, commonTree3.Token, "propfunc"));
                                adaptor.AddChild(commonTree, commonTree6);
                            }
                            else
                            {
                                var commonTree7 = (CommonTree)adaptor.GetNilNode();
                                commonTree7 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree7);
                                adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream2.NextTree());
                                adaptor.AddChild(commonTree7, (CommonTree)adaptor.Create(17, commonTree3.Token, "propfunc"));
                                adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree, commonTree7);
                            }
                            propertyBlock_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree8 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree9 = (CommonTree)Match(input, 19, FOLLOW_AUTOPROP_in_propertyBlock651);
                            var newRoot = (CommonTree)adaptor.DupNode(commonTree9);
                            commonTree8 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree8);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_propertyHeader_in_propertyBlock653);
                            var propertyHeader_return2 = propertyHeader();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree8, propertyHeader_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            var treeNode = (CommonTree)Match(input, 38, FOLLOW_ID_in_propertyBlock655);
                            var child2 = (CommonTree)adaptor.DupNode(treeNode);
                            adaptor.AddChild(commonTree8, child2);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, commonTree8);
                            CheckPropertyOverride((propertyHeader_return2 != null) ? propertyHeader_return2.sName : null, commonTree9.Token);
                            break;
                        }
                }
                propertyBlock_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                propertyBlock_stack.Pop();
            }
            return propertyBlock_return;
        }

        // Token: 0x06000C65 RID: 3173 RVA: 0x0004F4BC File Offset: 0x0004D6BC
        public propertyHeader_return propertyHeader()
        {
            var propertyHeader_return = new propertyHeader_return();
            propertyHeader_return.Start = input.LT(1);
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 8, FOLLOW_HEADER_in_propertyHeader678);
                var newRoot = (CommonTree)adaptor.DupNode(commonTree4);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_type_in_propertyHeader680);
                var type_return = type();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, type_return.Tree);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_propertyHeader684);
                var child = (CommonTree)adaptor.DupNode(commonTree5);
                adaptor.AddChild(commonTree3, child);
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_propertyHeader686);
                var child2 = (CommonTree)adaptor.DupNode(treeNode);
                adaptor.AddChild(commonTree3, child2);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 == 40)
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    var treeNode2 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_propertyHeader688);
                    var child3 = (CommonTree)adaptor.DupNode(treeNode2);
                    adaptor.AddChild(commonTree3, child3);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                propertyHeader_return.sName = commonTree5.Text;
                CheckVarOrPropName(propertyHeader_return.sName, commonTree4.Token);
                propertyHeader_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return propertyHeader_return;
        }

        // Token: 0x06000C66 RID: 3174 RVA: 0x0004F784 File Offset: 0x0004D984
        public propertyFunc_return propertyFunc(string asPropName)
        {
            var propertyFunc_return = new propertyFunc_return();
            propertyFunc_return.Start = input.LT(1);
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 17, FOLLOW_PROPFUNC_in_propertyFunc713);
                var newRoot = (CommonTree)adaptor.DupNode(commonTree4);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_function_in_propertyFunc715);
                var function_return = function("", asPropName);
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, function_return.Tree);
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                CheckPropertyFunction(asPropName, function_return.sName, out propertyFunc_return.bIsGet, commonTree4.Token);
                propertyFunc_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return propertyFunc_return;
        }

        // Token: 0x06000C67 RID: 3175 RVA: 0x0004F934 File Offset: 0x0004DB34
        public codeBlock_return codeBlock(ScriptFunctionType akFunctionType, ScriptScope akCurrentScope)
        {
            codeBlock_stack.Push(new codeBlock_scope());
            var codeBlock_return = new codeBlock_return();
            codeBlock_return.Start = input.LT(1);
            CommonTree commonTree = null;
            IList list = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token BLOCK");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule statement");
            ((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType = akFunctionType;
            ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope = akCurrentScope;
            ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars = new Dictionary<string, ScriptVariableType>();
            ((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild = 0;
            try
            {
                var commonTree2 = (CommonTree)input.LT(1);
                var child = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)Match(input, 10, FOLLOW_BLOCK_in_codeBlock747);
                rewriteRuleNodeStream.Add(commonTree3);
                if (input.LA(1) == 2)
                {
                    Match(input, 2, null);
                    for (; ; )
                    {
                        var num = 2;
                        var num2 = input.LA(1);
                        if (num2 is 5 or 11 or >= 15 and <= 16 or 22 or 38 or 41 or 62 or >= 65 and <= 84 or 88 or >= 90 and <= 93)
                        {
                            num = 1;
                        }
                        var num3 = num;
                        if (num3 != 1)
                        {
                            break;
                        }
                        commonTree2 = (CommonTree)input.LT(1);
                        commonTree2 = (CommonTree)input.LT(1);
                        PushFollow(FOLLOW_statement_in_codeBlock752);
                        var statement_return = statement();
                        state.followingStackPointer--;
                        rewriteRuleSubtreeStream.Add(statement_return.Tree);
                        if (list == null)
                        {
                            list = new ArrayList();
                        }
                        list.Add(statement_return.Tree);
                    }
                    Match(input, 3, null);
                }
                adaptor.AddChild(commonTree, child);
                codeBlock_return.Tree = commonTree;
                new RewriteRuleSubtreeStream(adaptor, "rule retval", codeBlock_return.Tree);
                commonTree = (CommonTree)adaptor.GetNilNode();
                adaptor.AddChild(commonTree, CreateBlockTree(commonTree3.Token, list, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars));
                codeBlock_return.Tree = commonTree;
                codeBlock_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                codeBlock_stack.Pop();
            }
            return codeBlock_return;
        }

        // Token: 0x06000C68 RID: 3176 RVA: 0x0004FC58 File Offset: 0x0004DE58
        public statement_return statement()
        {
            statement_stack.Push(new statement_scope());
            var statement_return = new statement_return
            {
                Start = input.LT(1)
            };
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token EQUALS");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule l_value");
            try
            {
                var num = input.LA(1);
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
                var ex = new NoViableAltException("", 27, 0, input);
                throw ex;
            IL_1E0:
                switch (num2)
                {
                    case 1:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_localDefinition_in_statement782);
                            var localDefinition_return = localDefinition();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, localDefinition_return.Tree);
                            break;
                        }
                    case 2:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 41, FOLLOW_EQUALS_in_statement789);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_l_value_in_statement791);
                            var l_value_return = l_value();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(l_value_return.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_statement793);
                            var expression_return = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            CheckAssignmentType(l_value_return.kType, expression_return.kType, (((CommonTree)l_value_return.Start)).Token);
                            ((statement_scope)statement_stack.Peek()).kautoCastTree = AutoCast(expression_return.kVarToken, expression_return.kType, l_value_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out var asVarName);
                            MarkTempVarAsUnused(l_value_return.kType, asVarName, (((CommonTree)l_value_return.Start)).Token);
                            MarkTempVarAsUnused(l_value_return.kType, l_value_return.sVarName, (((CommonTree)l_value_return.Start)).Token);
                            MarkTempVarAsUnused(expression_return.kType, expression_return.sVarName, (((CommonTree)expression_return.Start)).Token);
                            statement_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", statement_return.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)adaptor.GetNilNode();
                            commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
                            adaptor.AddChild(commonTree4, (CommonTree)adaptor.Create(38, commonTree3.Token, l_value_return.sVarName));
                            adaptor.AddChild(commonTree4, ((statement_scope)statement_stack.Peek()).kautoCastTree);
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream2.NextTree());
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
                            adaptor.AddChild(commonTree, commonTree4);
                            statement_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_statement824);
                            var expression_return2 = expression();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, expression_return2.Tree);
                            MarkTempVarAsUnused(expression_return2.kType, expression_return2.sVarName, (((CommonTree)expression_return2.Start)).Token);
                            break;
                        }
                    case 4:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_return_stat_in_statement835);
                            var return_stat_return = return_stat();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, return_stat_return.Tree);
                            break;
                        }
                    case 5:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_ifBlock_in_statement841);
                            var ifBlock_return = ifBlock();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, ifBlock_return.Tree);
                            break;
                        }
                    case 6:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_whileBlock_in_statement847);
                            var whileBlock_return = whileBlock();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, whileBlock_return.Tree);
                            break;
                        }
                }
                statement_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                statement_stack.Pop();
            }
            return statement_return;
        }

        // Token: 0x06000C69 RID: 3177 RVA: 0x00050458 File Offset: 0x0004E658
        public localDefinition_return localDefinition()
        {
            localDefinition_stack.Push(new localDefinition_scope());
            var localDefinition_return = new localDefinition_return();
            localDefinition_return.Start = input.LT(1);
            CommonTree commonTree = null;
            expression_return expression_return = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token VAR");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule type");
            try
            {
                var commonTree2 = (CommonTree)input.LT(1);
                var child = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var el = (CommonTree)Match(input, 5, FOLLOW_VAR_in_localDefinition865);
                rewriteRuleNodeStream.Add(el);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_type_in_localDefinition867);
                var type_return = type();
                state.followingStackPointer--;
                rewriteRuleSubtreeStream2.Add(type_return.Tree);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_localDefinition871);
                rewriteRuleNodeStream2.Add(commonTree3);
                var num = 2;
                var num2 = input.LA(1);
                if (num2 is 11 or >= 15 and <= 16 or 22 or 38 or 62 or >= 65 and <= 82 or >= 90 and <= 93)
                {
                    num = 1;
                }
                var num3 = num;
                if (num3 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_expression_in_localDefinition873);
                    expression_return = expression();
                    state.followingStackPointer--;
                    rewriteRuleSubtreeStream.Add(expression_return.Tree);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, child);
                CheckVariableDefinition(commonTree3.Text, type_return.kType, null, true, commonTree3.Token);
                CheckAssignmentType(type_return.kType, expression_return?.kType, commonTree3.Token);
                if ((CommonTree)expression_return?.Tree != null)
                {
                    ((localDefinition_scope)localDefinition_stack.Peek()).kautoCastTree = AutoCast(expression_return.kVarToken, expression_return.kType, type_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out var asVarName);
                    MarkTempVarAsUnused(type_return.kType, asVarName, commonTree3.Token);
                    MarkTempVarAsUnused(expression_return.kType, expression_return.sVarName, commonTree3.Token);
                }
                localDefinition_return.Tree = commonTree;
                var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token name", commonTree3);
                new RewriteRuleSubtreeStream(adaptor, "rule retval", localDefinition_return.Tree);
                commonTree = (CommonTree)adaptor.GetNilNode();
                if ((CommonTree)expression_return?.Tree == null)
                {
                    var commonTree4 = (CommonTree)adaptor.GetNilNode();
                    commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
                    adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream2.NextTree());
                    adaptor.AddChild(commonTree4, rewriteRuleNodeStream3.NextNode());
                    adaptor.AddChild(commonTree, commonTree4);
                }
                else
                {
                    var commonTree5 = (CommonTree)adaptor.GetNilNode();
                    commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
                    adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream2.NextTree());
                    adaptor.AddChild(commonTree5, rewriteRuleNodeStream3.NextNode());
                    adaptor.AddChild(commonTree5, ((localDefinition_scope)localDefinition_stack.Peek()).kautoCastTree);
                    adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
                    adaptor.AddChild(commonTree, commonTree5);
                }
                localDefinition_return.Tree = commonTree;
                localDefinition_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                localDefinition_stack.Pop();
            }
            return localDefinition_return;
        }

        // Token: 0x06000C6A RID: 3178 RVA: 0x0005097C File Offset: 0x0004EB7C
        public l_value_return l_value()
        {
            l_value_stack.Push(new l_value_scope());
            var l_value_return = new l_value_return();
            l_value_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token PAREXPR");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ARRAYSET");
            var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token DOT");
            var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token ID");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            try
            {
                var num = input.LA(1);
                int num4;
                if (num != 23)
                {
                    if (num != 38)
                    {
                        if (num != 62)
                        {
                            var ex = new NoViableAltException("", 29, 0, input);
                            throw ex;
                        }
                        var num2 = input.LA(2);
                        if (num2 != 2)
                        {
                            var ex2 = new NoViableAltException("", 29, 1, input);
                            throw ex2;
                        }
                        var num3 = input.LA(3);
                        if (num3 == 15)
                        {
                            num4 = 1;
                        }
                        else
                        {
                            if (num3 != 11 && num3 != 22 && num3 != 38 && num3 != 82)
                            {
                                var ex3 = new NoViableAltException("", 29, 4, input);
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
                    var num5 = input.LA(2);
                    if (num5 != 2)
                    {
                        var ex4 = new NoViableAltException("", 29, 2, input);
                        throw ex4;
                    }
                    var num6 = input.LA(3);
                    if (num6 == 15)
                    {
                        num4 = 2;
                    }
                    else
                    {
                        if (num6 != 11 && num6 != 38 && num6 != 82)
                        {
                            var ex5 = new NoViableAltException("", 29, 5, input);
                            throw ex5;
                        }
                        num4 = 3;
                    }
                }
                switch (num4)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree4 = (CommonTree)Match(input, 62, FOLLOW_DOT_in_l_value936);
                            rewriteRuleNodeStream3.Add(commonTree4);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var el = (CommonTree)Match(input, 15, FOLLOW_PAREXPR_in_l_value939);
                            rewriteRuleNodeStream.Add(el);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_l_value943);
                            var expression_return = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree3, child);
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_l_value948);
                            rewriteRuleNodeStream4.Add(commonTree5);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, commonTree3);
                            MarkTempVarAsUnused(expression_return.kType, expression_return.sVarName, commonTree4.Token);
                            GetPropertyInfo(expression_return.kType, commonTree5.Token, false, out l_value_return.kType);
                            l_value_return.sVarName = GenerateTempVariable(l_value_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            l_value_return.Tree = commonTree;
                            var rewriteRuleNodeStream5 = new RewriteRuleNodeStream(adaptor, "token b", commonTree5);
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", l_value_return.Tree);
                            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule a", expression_return.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree6 = (CommonTree)adaptor.GetNilNode();
                            commonTree6 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream3.NextNode(), commonTree6);
                            var commonTree7 = (CommonTree)adaptor.GetNilNode();
                            commonTree7 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree7);
                            adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream2.NextTree());
                            adaptor.AddChild(commonTree6, commonTree7);
                            var commonTree8 = (CommonTree)adaptor.GetNilNode();
                            commonTree8 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(21, commonTree5.Token, "propset"), commonTree8);
                            adaptor.AddChild(commonTree8, (CommonTree)adaptor.Create(38, commonTree5.Token, expression_return.sVarName));
                            adaptor.AddChild(commonTree8, rewriteRuleNodeStream5.NextNode());
                            adaptor.AddChild(commonTree8, (CommonTree)adaptor.Create(38, commonTree5.Token, l_value_return.sVarName));
                            adaptor.AddChild(commonTree6, commonTree8);
                            adaptor.AddChild(commonTree, commonTree6);
                            l_value_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree9 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var el2 = (CommonTree)Match(input, 23, FOLLOW_ARRAYSET_in_l_value995);
                            rewriteRuleNodeStream2.Add(el2);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            var child2 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var el3 = (CommonTree)Match(input, 15, FOLLOW_PAREXPR_in_l_value998);
                            rewriteRuleNodeStream.Add(el3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_l_value1002);
                            var expression_return2 = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return2.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree9, child2);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_l_value1007);
                            var expression_return3 = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return3.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, commonTree9);
                            HandleArrayElementExpression(expression_return2.sVarName, expression_return2.kType, expression_return2.kVarToken, expression_return3.sVarName, expression_return3.kType, expression_return3.kVarToken, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out l_value_return.sVarName, out l_value_return.kType, out ((l_value_scope)l_value_stack.Peek()).kvarToken, out ((l_value_scope)l_value_stack.Peek()).kautoCastTree);
                            l_value_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", l_value_return.Tree);
                            var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule index", expression_return3.Tree);
                            var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule array", expression_return2.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree10 = (CommonTree)adaptor.GetNilNode();
                            commonTree10 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree10);
                            adaptor.AddChild(commonTree10, (CommonTree)adaptor.Create(38, ((l_value_scope)l_value_stack.Peek()).kvarToken));
                            adaptor.AddChild(commonTree10, (CommonTree)adaptor.Create(38, expression_return2.kVarToken));
                            adaptor.AddChild(commonTree10, ((l_value_scope)l_value_stack.Peek()).kautoCastTree);
                            var commonTree11 = (CommonTree)adaptor.GetNilNode();
                            commonTree11 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree11);
                            adaptor.AddChild(commonTree11, rewriteRuleSubtreeStream4.NextTree());
                            adaptor.AddChild(commonTree10, commonTree11);
                            adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream3.NextTree());
                            adaptor.AddChild(commonTree, commonTree10);
                            l_value_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_basic_l_value_in_l_value1046);
                            var basic_l_value_return = basic_l_value(new ScriptVariableType("none"), "!first");
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, basic_l_value_return.Tree);
                            l_value_return.kType = (basic_l_value_return.kType);
                            l_value_return.sVarName = (basic_l_value_return.sVarName);
                            break;
                        }
                }
                l_value_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex6)
            {
                ReportError(ex6);
                Recover(input, ex6);
            }
            finally
            {
                l_value_stack.Pop();
            }
            return l_value_return;
        }

        // Token: 0x06000C6B RID: 3179 RVA: 0x00051468 File Offset: 0x0004F668
        public basic_l_value_return basic_l_value(ScriptVariableType akSelfType, string asSelfName)
        {
            basic_l_value_stack.Push(new basic_l_value_scope());
            var basic_l_value_return = new basic_l_value_return();
            basic_l_value_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token ARRAYSET");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule func_or_id");
            try
            {
                var num = input.LA(1);
                int num2;
                if (num != 23)
                {
                    if (num != 38)
                    {
                        if (num != 62)
                        {
                            var ex = new NoViableAltException("", 30, 0, input);
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
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree4 = (CommonTree)Match(input, 62, FOLLOW_DOT_in_basic_l_value1074);
                            var newRoot = (CommonTree)adaptor.DupNode(commonTree4);
                            commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_array_func_or_id_in_basic_l_value1078);
                            var array_func_or_id_return = array_func_or_id(akSelfType, asSelfName);
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree3, array_func_or_id_return.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_basic_l_value_in_basic_l_value1083);
                            var basic_l_value_return2 = basic_l_value((array_func_or_id_return != null) ? array_func_or_id_return.kType : null, (array_func_or_id_return != null) ? array_func_or_id_return.sVarName : null);
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree3, basic_l_value_return2.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, commonTree3);
                            MarkTempVarAsUnused((array_func_or_id_return != null) ? array_func_or_id_return.kType : null, (array_func_or_id_return != null) ? array_func_or_id_return.sVarName : null, commonTree4.Token);
                            basic_l_value_return.kType = ((basic_l_value_return2 != null) ? basic_l_value_return2.kType : null);
                            basic_l_value_return.sVarName = ((basic_l_value_return2 != null) ? basic_l_value_return2.sVarName : null);
                            break;
                        }
                    case 2:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var el = (CommonTree)Match(input, 23, FOLLOW_ARRAYSET_in_basic_l_value1097);
                            rewriteRuleNodeStream.Add(el);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_func_or_id_in_basic_l_value1099);
                            var func_or_id_return = func_or_id(akSelfType, asSelfName);
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(func_or_id_return.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_basic_l_value1102);
                            var expression_return = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            HandleArrayElementExpression((func_or_id_return != null) ? func_or_id_return.sVarName : null, (func_or_id_return != null) ? func_or_id_return.kType : null, (func_or_id_return != null) ? func_or_id_return.kVarToken : null, (expression_return != null) ? expression_return.sVarName : null, (expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.kVarToken : null, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out basic_l_value_return.sVarName, out basic_l_value_return.kType, out ((basic_l_value_scope)basic_l_value_stack.Peek()).kvarToken, out ((basic_l_value_scope)basic_l_value_stack.Peek()).kautoCastTree);
                            basic_l_value_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (basic_l_value_return != null) ? basic_l_value_return.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree5 = (CommonTree)adaptor.GetNilNode();
                            commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
                            adaptor.AddChild(commonTree5, (CommonTree)adaptor.Create(38, ((basic_l_value_scope)basic_l_value_stack.Peek()).kvarToken));
                            adaptor.AddChild(commonTree5, (CommonTree)adaptor.Create(38, (func_or_id_return != null) ? func_or_id_return.kVarToken : null));
                            adaptor.AddChild(commonTree5, ((basic_l_value_scope)basic_l_value_stack.Peek()).kautoCastTree);
                            adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream2.NextTree());
                            adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
                            adaptor.AddChild(commonTree, commonTree5);
                            basic_l_value_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_basic_l_value1135);
                            rewriteRuleNodeStream2.Add(commonTree6);
                            if (asSelfName != "!first")
                            {
                                if (asSelfName == "")
                                {
                                    OnError("a property cannot be used directly on a type, it must be used on a variable", commonTree6.Line, commonTree6.CharPositionInLine);
                                }
                                else if (asSelfName.ToLowerInvariant() == "parent")
                                {
                                    OnError("a property cannot be used on the special parent variable, use the property directly instead", commonTree6.Line, commonTree6.CharPositionInLine);
                                }
                                GetPropertyInfo(akSelfType, commonTree6.Token, false, out basic_l_value_return.kType);
                                ((basic_l_value_scope)basic_l_value_stack.Peek()).bisProperty = true;
                                basic_l_value_return.sVarName = GenerateTempVariable(basic_l_value_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            }
                            else if (!((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType.bGlobal && IsLocalProperty(commonTree6.Text))
                            {
                                ((basic_l_value_scope)basic_l_value_stack.Peek()).bisProperty = true;
                                GetPropertyInfo(new ScriptVariableType(kObjType.Name), commonTree6.Token, false, out basic_l_value_return.kType);
                                if (basic_l_value_return.kType.ShadowVariableName.Length > 0)
                                {
                                    ((basic_l_value_scope)basic_l_value_stack.Peek()).bisLocalAutoProperty = true;
                                    basic_l_value_return.sVarName = basic_l_value_return.kType.ShadowVariableName;
                                }
                                else
                                {
                                    basic_l_value_return.sVarName = GenerateTempVariable(basic_l_value_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                                    asSelfName = "self";
                                }
                            }
                            else
                            {
                                CheckCanBeLValue(commonTree6.Text, commonTree6.Token);
                                basic_l_value_return.kType = GetVariableType(((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, commonTree6.Token);
                                ((basic_l_value_scope)basic_l_value_stack.Peek()).bisProperty = false;
                                basic_l_value_return.sVarName = commonTree6.Text;
                            }
                            basic_l_value_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (basic_l_value_return != null) ? basic_l_value_return.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            if (((basic_l_value_scope)basic_l_value_stack.Peek()).bisProperty && !((basic_l_value_scope)basic_l_value_stack.Peek()).bisLocalAutoProperty)
                            {
                                var commonTree7 = (CommonTree)adaptor.GetNilNode();
                                commonTree7 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(21, commonTree6.Token, "propset"), commonTree7);
                                adaptor.AddChild(commonTree7, (CommonTree)adaptor.Create(38, commonTree6.Token, asSelfName));
                                adaptor.AddChild(commonTree7, rewriteRuleNodeStream2.NextNode());
                                adaptor.AddChild(commonTree7, (CommonTree)adaptor.Create(38, commonTree6.Token, basic_l_value_return.sVarName));
                                adaptor.AddChild(commonTree, commonTree7);
                            }
                            else if (((basic_l_value_scope)basic_l_value_stack.Peek()).bisProperty && ((basic_l_value_scope)basic_l_value_stack.Peek()).bisLocalAutoProperty)
                            {
                                var commonTree8 = (CommonTree)adaptor.GetNilNode();
                                commonTree8 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(38, commonTree6.Token, basic_l_value_return.sVarName), commonTree8);
                                adaptor.AddChild(commonTree, commonTree8);
                            }
                            else
                            {
                                adaptor.AddChild(commonTree, rewriteRuleNodeStream2.NextNode());
                            }
                            basic_l_value_return.Tree = commonTree;
                            break;
                        }
                }
                basic_l_value_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                basic_l_value_stack.Pop();
            }
            return basic_l_value_return;
        }

        // Token: 0x06000C6C RID: 3180 RVA: 0x00051ECC File Offset: 0x000500CC
        public expression_return expression()
        {
            var expression_return = new expression_return();
            expression_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token OR");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule and_expression");
            try
            {
                var num = input.LA(1);
                int num2;
                if (num == 65)
                {
                    num2 = 1;
                }
                else
                {
                    if (num != 11 && num is < 15 or > 16 && (num != 22 && num != 38 && num != 62 && num is < 66 or > 82) && num is < 90 or > 93)
                    {
                        var ex = new NoViableAltException("", 31, 0, input);
                        throw ex;
                    }
                    num2 = 2;
                }
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 65, FOLLOW_OR_in_expression1204);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_expression1208);
                            var expression_return2 = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_and_expression_in_expression1212);
                            var and_expression_return = and_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(and_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            MarkTempVarAsUnused((expression_return2 != null) ? expression_return2.kType : null, (expression_return2 != null) ? expression_return2.sVarName : null, ((expression_return2 != null) ? ((CommonTree)expression_return2.Start) : null).Token);
                            MarkTempVarAsUnused((and_expression_return != null) ? and_expression_return.kType : null, (and_expression_return != null) ? and_expression_return.sVarName : null, ((and_expression_return != null) ? ((CommonTree)and_expression_return.Start) : null).Token);
                            expression_return.kType = new ScriptVariableType("bool");
                            expression_return.sVarName = GenerateTempVariable(expression_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            expression_return.kVarToken = new CommonToken(commonTree3.Token);
                            expression_return.kVarToken.Type = 38;
                            expression_return.kVarToken.Text = expression_return.sVarName;
                            expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (expression_return != null) ? expression_return.Tree : null);
                            var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule b", (and_expression_return != null) ? and_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule a", (expression_return2 != null) ? expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)adaptor.GetNilNode();
                            commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
                            adaptor.AddChild(commonTree4, (CommonTree)adaptor.Create(38, commonTree3.Token, expression_return.sVarName));
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
                            adaptor.AddChild(commonTree, commonTree4);
                            expression_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_and_expression_in_expression1246);
                            var and_expression_return2 = and_expression();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, and_expression_return2.Tree);
                            expression_return.kType = ((and_expression_return2 != null) ? and_expression_return2.kType : null);
                            expression_return.sVarName = ((and_expression_return2 != null) ? and_expression_return2.sVarName : null);
                            expression_return.kVarToken = ((and_expression_return2 != null) ? and_expression_return2.kVarToken : null);
                            break;
                        }
                }
                expression_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return expression_return;
        }

        // Token: 0x06000C6D RID: 3181 RVA: 0x000523C0 File Offset: 0x000505C0
        public and_expression_return and_expression()
        {
            var and_expression_return = new and_expression_return();
            and_expression_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token AND");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule bool_expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule and_expression");
            try
            {
                var num = input.LA(1);
                int num2;
                if (num == 66)
                {
                    num2 = 1;
                }
                else
                {
                    if (num != 11 && num is < 15 or > 16 && (num != 22 && num != 38 && num != 62 && num is (< 67 or > 82) and (< 90 or > 93)))
                    {
                        var ex = new NoViableAltException("", 32, 0, input);
                        throw ex;
                    }
                    num2 = 2;
                }
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 66, FOLLOW_AND_in_and_expression1268);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_and_expression_in_and_expression1272);
                            var and_expression_return2 = and_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(and_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_bool_expression_in_and_expression1276);
                            var bool_expression_return = bool_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(bool_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            MarkTempVarAsUnused((and_expression_return2 != null) ? and_expression_return2.kType : null, (and_expression_return2 != null) ? and_expression_return2.sVarName : null, ((and_expression_return2 != null) ? ((CommonTree)and_expression_return2.Start) : null).Token);
                            MarkTempVarAsUnused((bool_expression_return != null) ? bool_expression_return.kType : null, (bool_expression_return != null) ? bool_expression_return.sVarName : null, ((bool_expression_return != null) ? ((CommonTree)bool_expression_return.Start) : null).Token);
                            and_expression_return.kType = new ScriptVariableType("bool");
                            and_expression_return.sVarName = GenerateTempVariable(and_expression_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            and_expression_return.kVarToken = new CommonToken(commonTree3.Token);
                            and_expression_return.kVarToken.Type = 38;
                            and_expression_return.kVarToken.Text = and_expression_return.sVarName;
                            and_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (and_expression_return != null) ? and_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule b", (bool_expression_return != null) ? bool_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule a", (and_expression_return2 != null) ? and_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)adaptor.GetNilNode();
                            commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
                            adaptor.AddChild(commonTree4, (CommonTree)adaptor.Create(38, commonTree3.Token, and_expression_return.sVarName));
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
                            adaptor.AddChild(commonTree, commonTree4);
                            and_expression_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_bool_expression_in_and_expression1310);
                            var bool_expression_return2 = bool_expression();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, bool_expression_return2.Tree);
                            and_expression_return.kType = ((bool_expression_return2 != null) ? bool_expression_return2.kType : null);
                            and_expression_return.sVarName = ((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null);
                            and_expression_return.kVarToken = ((bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null);
                            break;
                        }
                }
                and_expression_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return and_expression_return;
        }

        // Token: 0x06000C6E RID: 3182 RVA: 0x000528B4 File Offset: 0x00050AB4
        public bool_expression_return bool_expression()
        {
            bool_expression_stack.Push(new bool_expression_scope());
            var bool_expression_return = new bool_expression_return();
            bool_expression_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token GT");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token LT");
            var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token EQ");
            var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token LTE");
            var rewriteRuleNodeStream5 = new RewriteRuleNodeStream(adaptor, "token GTE");
            var rewriteRuleNodeStream6 = new RewriteRuleNodeStream(adaptor, "token NE");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule bool_expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule add_expression");
            try
            {
                var num = input.LA(1);
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
                var ex = new NoViableAltException("", 33, 0, input);
                throw ex;
            IL_1E6:
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 67, FOLLOW_EQ_in_bool_expression1337);
                            rewriteRuleNodeStream3.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_bool_expression_in_bool_expression1341);
                            var bool_expression_return2 = bool_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_bool_expression1345);
                            var add_expression_return = add_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree3.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((bool_expression_scope)bool_expression_stack.Peek()).kaTree, out ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            bool_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)adaptor.GetNilNode();
                            commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream3.NextNode(), commonTree4);
                            adaptor.AddChild(commonTree4, (CommonTree)adaptor.Create(38, commonTree3.Token, bool_expression_return.sVarName));
                            adaptor.AddChild(commonTree4, ((bool_expression_scope)bool_expression_stack.Peek()).kaTree);
                            adaptor.AddChild(commonTree4, ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
                            adaptor.AddChild(commonTree, commonTree4);
                            bool_expression_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child2 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree5 = (CommonTree)Match(input, 68, FOLLOW_NE_in_bool_expression1384);
                            rewriteRuleNodeStream6.Add(commonTree5);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_bool_expression_in_bool_expression1388);
                            var bool_expression_return2 = bool_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_bool_expression1392);
                            var add_expression_return = add_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child2);
                            HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree5.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((bool_expression_scope)bool_expression_stack.Peek()).kaTree, out ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            bool_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree6 = (CommonTree)adaptor.GetNilNode();
                            commonTree6 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream6.NextNode(), commonTree6);
                            adaptor.AddChild(commonTree6, (CommonTree)adaptor.Create(38, commonTree5.Token, bool_expression_return.sVarName));
                            adaptor.AddChild(commonTree6, ((bool_expression_scope)bool_expression_stack.Peek()).kaTree);
                            adaptor.AddChild(commonTree6, ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream6.NextTree());
                            adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream5.NextTree());
                            adaptor.AddChild(commonTree, commonTree6);
                            bool_expression_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child3 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree7 = (CommonTree)Match(input, 69, FOLLOW_GT_in_bool_expression1431);
                            rewriteRuleNodeStream.Add(commonTree7);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_bool_expression_in_bool_expression1435);
                            var bool_expression_return2 = bool_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_bool_expression1439);
                            var add_expression_return = add_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child3);
                            HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree7.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((bool_expression_scope)bool_expression_stack.Peek()).kaTree, out ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            bool_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream7 = new RewriteRuleSubtreeStream(adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream8 = new RewriteRuleSubtreeStream(adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree8 = (CommonTree)adaptor.GetNilNode();
                            commonTree8 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree8);
                            adaptor.AddChild(commonTree8, (CommonTree)adaptor.Create(38, commonTree7.Token, bool_expression_return.sVarName));
                            adaptor.AddChild(commonTree8, ((bool_expression_scope)bool_expression_stack.Peek()).kaTree);
                            adaptor.AddChild(commonTree8, ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream8.NextTree());
                            adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream7.NextTree());
                            adaptor.AddChild(commonTree, commonTree8);
                            bool_expression_return.Tree = commonTree;
                            break;
                        }
                    case 4:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child4 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree9 = (CommonTree)Match(input, 70, FOLLOW_LT_in_bool_expression1478);
                            rewriteRuleNodeStream2.Add(commonTree9);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_bool_expression_in_bool_expression1482);
                            var bool_expression_return2 = bool_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_bool_expression1486);
                            var add_expression_return = add_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child4);
                            HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree9.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((bool_expression_scope)bool_expression_stack.Peek()).kaTree, out ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            bool_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream9 = new RewriteRuleSubtreeStream(adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream10 = new RewriteRuleSubtreeStream(adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree10 = (CommonTree)adaptor.GetNilNode();
                            commonTree10 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree10);
                            adaptor.AddChild(commonTree10, (CommonTree)adaptor.Create(38, commonTree9.Token, bool_expression_return.sVarName));
                            adaptor.AddChild(commonTree10, ((bool_expression_scope)bool_expression_stack.Peek()).kaTree);
                            adaptor.AddChild(commonTree10, ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream10.NextTree());
                            adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream9.NextTree());
                            adaptor.AddChild(commonTree, commonTree10);
                            bool_expression_return.Tree = commonTree;
                            break;
                        }
                    case 5:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child5 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree11 = (CommonTree)Match(input, 71, FOLLOW_GTE_in_bool_expression1525);
                            rewriteRuleNodeStream5.Add(commonTree11);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_bool_expression_in_bool_expression1529);
                            var bool_expression_return2 = bool_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_bool_expression1533);
                            var add_expression_return = add_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child5);
                            HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree11.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((bool_expression_scope)bool_expression_stack.Peek()).kaTree, out ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            bool_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream11 = new RewriteRuleSubtreeStream(adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream12 = new RewriteRuleSubtreeStream(adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree12 = (CommonTree)adaptor.GetNilNode();
                            commonTree12 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream5.NextNode(), commonTree12);
                            adaptor.AddChild(commonTree12, (CommonTree)adaptor.Create(38, commonTree11.Token, bool_expression_return.sVarName));
                            adaptor.AddChild(commonTree12, ((bool_expression_scope)bool_expression_stack.Peek()).kaTree);
                            adaptor.AddChild(commonTree12, ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            adaptor.AddChild(commonTree12, rewriteRuleSubtreeStream12.NextTree());
                            adaptor.AddChild(commonTree12, rewriteRuleSubtreeStream11.NextTree());
                            adaptor.AddChild(commonTree, commonTree12);
                            bool_expression_return.Tree = commonTree;
                            break;
                        }
                    case 6:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child6 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree13 = (CommonTree)Match(input, 72, FOLLOW_LTE_in_bool_expression1572);
                            rewriteRuleNodeStream4.Add(commonTree13);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_bool_expression_in_bool_expression1576);
                            var bool_expression_return2 = bool_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(bool_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_bool_expression1580);
                            var add_expression_return = add_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(add_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child6);
                            HandleComparisonExpression((bool_expression_return2 != null) ? bool_expression_return2.sVarName : null, (bool_expression_return2 != null) ? bool_expression_return2.kType : null, (bool_expression_return2 != null) ? bool_expression_return2.kVarToken : null, (add_expression_return != null) ? add_expression_return.sVarName : null, (add_expression_return != null) ? add_expression_return.kType : null, (add_expression_return != null) ? add_expression_return.kVarToken : null, commonTree13.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out bool_expression_return.sVarName, out bool_expression_return.kType, out bool_expression_return.kVarToken, out ((bool_expression_scope)bool_expression_stack.Peek()).kaTree, out ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            bool_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream13 = new RewriteRuleSubtreeStream(adaptor, "rule b", (add_expression_return != null) ? add_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream14 = new RewriteRuleSubtreeStream(adaptor, "rule a", (bool_expression_return2 != null) ? bool_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree14 = (CommonTree)adaptor.GetNilNode();
                            commonTree14 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream4.NextNode(), commonTree14);
                            adaptor.AddChild(commonTree14, (CommonTree)adaptor.Create(38, commonTree13.Token, bool_expression_return.sVarName));
                            adaptor.AddChild(commonTree14, ((bool_expression_scope)bool_expression_stack.Peek()).kaTree);
                            adaptor.AddChild(commonTree14, ((bool_expression_scope)bool_expression_stack.Peek()).kbTree);
                            adaptor.AddChild(commonTree14, rewriteRuleSubtreeStream14.NextTree());
                            adaptor.AddChild(commonTree14, rewriteRuleSubtreeStream13.NextTree());
                            adaptor.AddChild(commonTree, commonTree14);
                            bool_expression_return.Tree = commonTree;
                            break;
                        }
                    case 7:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_bool_expression1618);
                            var add_expression_return2 = add_expression();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, add_expression_return2.Tree);
                            bool_expression_return.kType = (add_expression_return2.kType);
                            bool_expression_return.sVarName = (add_expression_return2.sVarName);
                            bool_expression_return.kVarToken = (add_expression_return2.kVarToken);
                            break;
                        }
                }
                bool_expression_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                bool_expression_stack.Pop();
            }
            return bool_expression_return;
        }

        // Token: 0x06000C6F RID: 3183 RVA: 0x00053F0C File Offset: 0x0005210C
        public add_expression_return add_expression()
        {
            add_expression_stack.Push(new add_expression_scope());
            var add_expression_return = new add_expression_return();
            add_expression_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token PLUS");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token MINUS");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule add_expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule mult_expression");
            ((add_expression_scope)add_expression_stack.Peek()).bisInt = false;
            try
            {
                var num = input.LA(1);
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
                var ex = new NoViableAltException("", 34, 0, input);
                throw ex;
            IL_182:
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 73, FOLLOW_PLUS_in_add_expression1650);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_add_expression1654);
                            var add_expression_return2 = add_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(add_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_mult_expression_in_add_expression1658);
                            var mult_expression_return = mult_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(mult_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            HandleAddSubtractExpression(add_expression_return2.sVarName, add_expression_return2.kType, add_expression_return2.kVarToken, mult_expression_return.sVarName, mult_expression_return.kType, mult_expression_return.kVarToken, commonTree3.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out ((add_expression_scope)add_expression_stack.Peek()).bisConcat, out ((add_expression_scope)add_expression_stack.Peek()).bisInt, out add_expression_return.sVarName, out add_expression_return.kType, out add_expression_return.kVarToken, out ((add_expression_scope)add_expression_stack.Peek()).kaTree, out ((add_expression_scope)add_expression_stack.Peek()).kbTree);
                            add_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", add_expression_return.Tree);
                            var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule b", (mult_expression_return != null) ? mult_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule a", (add_expression_return2 != null) ? add_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            if (((add_expression_scope)add_expression_stack.Peek()).bisConcat)
                            {
                                var commonTree4 = (CommonTree)adaptor.GetNilNode();
                                commonTree4 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(36, commonTree3.Token, "STRCAT"), commonTree4);
                                adaptor.AddChild(commonTree4, (CommonTree)adaptor.Create(38, commonTree3.Token, add_expression_return.sVarName));
                                adaptor.AddChild(commonTree4, ((add_expression_scope)add_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree4, ((add_expression_scope)add_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
                                adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree, commonTree4);
                            }
                            else if (((add_expression_scope)add_expression_stack.Peek()).bisInt)
                            {
                                var commonTree5 = (CommonTree)adaptor.GetNilNode();
                                commonTree5 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(26, commonTree3.Token), commonTree5);
                                adaptor.AddChild(commonTree5, (CommonTree)adaptor.Create(38, commonTree3.Token, add_expression_return.sVarName));
                                adaptor.AddChild(commonTree5, ((add_expression_scope)add_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree5, ((add_expression_scope)add_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream4.NextTree());
                                adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree, commonTree5);
                            }
                            else
                            {
                                var commonTree6 = (CommonTree)adaptor.GetNilNode();
                                commonTree6 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(27, commonTree3.Token), commonTree6);
                                adaptor.AddChild(commonTree6, (CommonTree)adaptor.Create(38, commonTree3.Token, add_expression_return.sVarName));
                                adaptor.AddChild(commonTree6, ((add_expression_scope)add_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree6, ((add_expression_scope)add_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream4.NextTree());
                                adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree, commonTree6);
                            }
                            add_expression_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child2 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree7 = (CommonTree)Match(input, 74, FOLLOW_MINUS_in_add_expression1748);
                            rewriteRuleNodeStream2.Add(commonTree7);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_add_expression_in_add_expression1752);
                            var add_expression_return2 = add_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(add_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_mult_expression_in_add_expression1756);
                            var mult_expression_return = mult_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(mult_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child2);
                            HandleAddSubtractExpression(add_expression_return2.sVarName, add_expression_return2.kType, add_expression_return2.kVarToken, (mult_expression_return != null) ? mult_expression_return.sVarName : null, (mult_expression_return != null) ? mult_expression_return.kType : null, (mult_expression_return != null) ? mult_expression_return.kVarToken : null, commonTree7.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out ((add_expression_scope)add_expression_stack.Peek()).bisConcat, out ((add_expression_scope)add_expression_stack.Peek()).bisInt, out add_expression_return.sVarName, out add_expression_return.kType, out add_expression_return.kVarToken, out ((add_expression_scope)add_expression_stack.Peek()).kaTree, out ((add_expression_scope)add_expression_stack.Peek()).kbTree);
                            add_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", add_expression_return.Tree);
                            var rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule b", mult_expression_return.Tree);
                            var rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(adaptor, "rule a", add_expression_return2.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            if (((add_expression_scope)add_expression_stack.Peek()).bisInt)
                            {
                                var commonTree8 = (CommonTree)adaptor.GetNilNode();
                                commonTree8 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(28, commonTree7.Token), commonTree8);
                                adaptor.AddChild(commonTree8, (CommonTree)adaptor.Create(38, commonTree7.Token, add_expression_return.sVarName));
                                adaptor.AddChild(commonTree8, ((add_expression_scope)add_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree8, ((add_expression_scope)add_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream6.NextTree());
                                adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream5.NextTree());
                                adaptor.AddChild(commonTree, commonTree8);
                            }
                            else
                            {
                                var commonTree9 = (CommonTree)adaptor.GetNilNode();
                                commonTree9 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(29, commonTree7.Token), commonTree9);
                                adaptor.AddChild(commonTree9, (CommonTree)adaptor.Create(38, commonTree7.Token, add_expression_return.sVarName));
                                adaptor.AddChild(commonTree9, ((add_expression_scope)add_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree9, ((add_expression_scope)add_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree9, rewriteRuleSubtreeStream6.NextTree());
                                adaptor.AddChild(commonTree9, rewriteRuleSubtreeStream5.NextTree());
                                adaptor.AddChild(commonTree, commonTree9);
                            }
                            add_expression_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_mult_expression_in_add_expression1820);
                            var mult_expression_return2 = mult_expression();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, mult_expression_return2.Tree);
                            add_expression_return.kType = (mult_expression_return2.kType);
                            add_expression_return.sVarName = (mult_expression_return2.sVarName);
                            add_expression_return.kVarToken = (mult_expression_return2.kVarToken);
                            break;
                        }
                }
                add_expression_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                add_expression_stack.Pop();
            }
            return add_expression_return;
        }

        // Token: 0x06000C70 RID: 3184 RVA: 0x00054BAC File Offset: 0x00052DAC
        public mult_expression_return mult_expression()
        {
            mult_expression_stack.Push(new mult_expression_scope());
            var mult_expression_return = new mult_expression_return();
            mult_expression_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token MULT");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token MOD");
            var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token DIVIDE");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule unary_expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule mult_expression");
            ((mult_expression_scope)mult_expression_stack.Peek()).bisInt = false;
            try
            {
                var num = input.LA(1);
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
                var ex = new NoViableAltException("", 35, 0, input);
                throw ex;
            IL_177:
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 75, FOLLOW_MULT_in_mult_expression1852);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_mult_expression_in_mult_expression1856);
                            var mult_expression_return2 = mult_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(mult_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_unary_expression_in_mult_expression1860);
                            var unary_expression_return = unary_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(unary_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            HandleMultDivideExpression((mult_expression_return2 != null) ? mult_expression_return2.sVarName : null, (mult_expression_return2 != null) ? mult_expression_return2.kType : null, (mult_expression_return2 != null) ? mult_expression_return2.kVarToken : null, (unary_expression_return != null) ? unary_expression_return.sVarName : null, (unary_expression_return != null) ? unary_expression_return.kType : null, (unary_expression_return != null) ? unary_expression_return.kVarToken : null, commonTree3.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out ((mult_expression_scope)mult_expression_stack.Peek()).bisInt, out mult_expression_return.sVarName, out mult_expression_return.kType, out mult_expression_return.kVarToken, out ((mult_expression_scope)mult_expression_stack.Peek()).kaTree, out ((mult_expression_scope)mult_expression_stack.Peek()).kbTree);
                            mult_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule b", (unary_expression_return != null) ? unary_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule a", (mult_expression_return2 != null) ? mult_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            if (((mult_expression_scope)mult_expression_stack.Peek()).bisInt)
                            {
                                var commonTree4 = (CommonTree)adaptor.GetNilNode();
                                commonTree4 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(30, commonTree3.Token), commonTree4);
                                adaptor.AddChild(commonTree4, (CommonTree)adaptor.Create(38, commonTree3.Token, mult_expression_return.sVarName));
                                adaptor.AddChild(commonTree4, ((mult_expression_scope)mult_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree4, ((mult_expression_scope)mult_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream4.NextTree());
                                adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree, commonTree4);
                            }
                            else
                            {
                                var commonTree5 = (CommonTree)adaptor.GetNilNode();
                                commonTree5 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(31, commonTree3.Token), commonTree5);
                                adaptor.AddChild(commonTree5, (CommonTree)adaptor.Create(38, commonTree3.Token, mult_expression_return.sVarName));
                                adaptor.AddChild(commonTree5, ((mult_expression_scope)mult_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree5, ((mult_expression_scope)mult_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream4.NextTree());
                                adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
                                adaptor.AddChild(commonTree, commonTree5);
                            }
                            mult_expression_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child2 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree6 = (CommonTree)Match(input, 76, FOLLOW_DIVIDE_in_mult_expression1925);
                            rewriteRuleNodeStream3.Add(commonTree6);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_mult_expression_in_mult_expression1929);
                            var mult_expression_return2 = mult_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(mult_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_unary_expression_in_mult_expression1933);
                            var unary_expression_return = unary_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(unary_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child2);
                            HandleMultDivideExpression((mult_expression_return2 != null) ? mult_expression_return2.sVarName : null, (mult_expression_return2 != null) ? mult_expression_return2.kType : null, (mult_expression_return2 != null) ? mult_expression_return2.kVarToken : null, (unary_expression_return != null) ? unary_expression_return.sVarName : null, (unary_expression_return != null) ? unary_expression_return.kType : null, (unary_expression_return != null) ? unary_expression_return.kVarToken : null, commonTree6.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out ((mult_expression_scope)mult_expression_stack.Peek()).bisInt, out mult_expression_return.sVarName, out mult_expression_return.kType, out mult_expression_return.kVarToken, out ((mult_expression_scope)mult_expression_stack.Peek()).kaTree, out ((mult_expression_scope)mult_expression_stack.Peek()).kbTree);
                            mult_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule b", (unary_expression_return != null) ? unary_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(adaptor, "rule a", (mult_expression_return2 != null) ? mult_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            if (((mult_expression_scope)mult_expression_stack.Peek()).bisInt)
                            {
                                var commonTree7 = (CommonTree)adaptor.GetNilNode();
                                commonTree7 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(32, commonTree6.Token), commonTree7);
                                adaptor.AddChild(commonTree7, (CommonTree)adaptor.Create(38, commonTree6.Token, mult_expression_return.sVarName));
                                adaptor.AddChild(commonTree7, ((mult_expression_scope)mult_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree7, ((mult_expression_scope)mult_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream6.NextTree());
                                adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream5.NextTree());
                                adaptor.AddChild(commonTree, commonTree7);
                            }
                            else
                            {
                                var commonTree8 = (CommonTree)adaptor.GetNilNode();
                                commonTree8 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(33, commonTree6.Token), commonTree8);
                                adaptor.AddChild(commonTree8, (CommonTree)adaptor.Create(38, commonTree6.Token, mult_expression_return.sVarName));
                                adaptor.AddChild(commonTree8, ((mult_expression_scope)mult_expression_stack.Peek()).kaTree);
                                adaptor.AddChild(commonTree8, ((mult_expression_scope)mult_expression_stack.Peek()).kbTree);
                                adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream6.NextTree());
                                adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream5.NextTree());
                                adaptor.AddChild(commonTree, commonTree8);
                            }
                            mult_expression_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child3 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree9 = (CommonTree)Match(input, 77, FOLLOW_MOD_in_mult_expression1998);
                            rewriteRuleNodeStream2.Add(commonTree9);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_mult_expression_in_mult_expression2002);
                            var mult_expression_return2 = mult_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(mult_expression_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_unary_expression_in_mult_expression2006);
                            var unary_expression_return = unary_expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(unary_expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child3);
                            MarkTempVarAsUnused((mult_expression_return2 != null) ? mult_expression_return2.kType : null, (mult_expression_return2 != null) ? mult_expression_return2.sVarName : null, ((mult_expression_return2 != null) ? ((CommonTree)mult_expression_return2.Start) : null).Token);
                            MarkTempVarAsUnused((unary_expression_return != null) ? unary_expression_return.kType : null, (unary_expression_return != null) ? unary_expression_return.sVarName : null, ((unary_expression_return != null) ? ((CommonTree)unary_expression_return.Start) : null).Token);
                            mult_expression_return.kType = CheckModType((mult_expression_return2 != null) ? mult_expression_return2.kType : null, (unary_expression_return != null) ? unary_expression_return.kType : null, commonTree9.Token);
                            mult_expression_return.sVarName = GenerateTempVariable(mult_expression_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            mult_expression_return.kVarToken = new CommonToken(commonTree9.Token);
                            mult_expression_return.kVarToken.Type = 38;
                            mult_expression_return.kVarToken.Text = mult_expression_return.sVarName;
                            mult_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream7 = new RewriteRuleSubtreeStream(adaptor, "rule b", (unary_expression_return != null) ? unary_expression_return.Tree : null);
                            var rewriteRuleSubtreeStream8 = new RewriteRuleSubtreeStream(adaptor, "rule a", (mult_expression_return2 != null) ? mult_expression_return2.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree10 = (CommonTree)adaptor.GetNilNode();
                            commonTree10 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree10);
                            adaptor.AddChild(commonTree10, (CommonTree)adaptor.Create(38, commonTree9.Token, mult_expression_return.sVarName));
                            adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream8.NextTree());
                            adaptor.AddChild(commonTree10, rewriteRuleSubtreeStream7.NextTree());
                            adaptor.AddChild(commonTree, commonTree10);
                            mult_expression_return.Tree = commonTree;
                            break;
                        }
                    case 4:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_unary_expression_in_mult_expression2040);
                            var unary_expression_return2 = unary_expression();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, unary_expression_return2.Tree);
                            mult_expression_return.kType = ((unary_expression_return2 != null) ? unary_expression_return2.kType : null);
                            mult_expression_return.sVarName = ((unary_expression_return2 != null) ? unary_expression_return2.sVarName : null);
                            mult_expression_return.kVarToken = ((unary_expression_return2 != null) ? unary_expression_return2.kVarToken : null);
                            break;
                        }
                }
                mult_expression_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                mult_expression_stack.Pop();
            }
            return mult_expression_return;
        }

        // Token: 0x06000C71 RID: 3185 RVA: 0x00055A4C File Offset: 0x00053C4C
        public unary_expression_return unary_expression()
        {
            unary_expression_stack.Push(new unary_expression_scope());
            var unary_expression_return = new unary_expression_return();
            unary_expression_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token UNARY_MINUS");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token NOT");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule cast_atom");
            ((unary_expression_scope)unary_expression_stack.Peek()).bisInt = false;
            try
            {
                var num = input.LA(1);
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
                var ex = new NoViableAltException("", 36, 0, input);
                throw ex;
            IL_13C:
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 16, FOLLOW_UNARY_MINUS_in_unary_expression2072);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_cast_atom_in_unary_expression2074);
                            var cast_atom_return = cast_atom();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(cast_atom_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            MarkTempVarAsUnused(cast_atom_return.kType, cast_atom_return.sVarName, (((CommonTree)cast_atom_return.Start)).Token);
                            unary_expression_return.kType = CheckNegationType(cast_atom_return.kType, commonTree3.Token);
                            unary_expression_return.sVarName = GenerateTempVariable(unary_expression_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            ((unary_expression_scope)unary_expression_stack.Peek()).bisInt = (unary_expression_return.kType.VarType == "int");
                            unary_expression_return.kVarToken = new CommonToken(commonTree3.Token);
                            unary_expression_return.kVarToken.Type = 38;
                            unary_expression_return.kVarToken.Text = unary_expression_return.sVarName;
                            unary_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", unary_expression_return.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            if (((unary_expression_scope)unary_expression_stack.Peek()).bisInt)
                            {
                                var commonTree4 = (CommonTree)adaptor.GetNilNode();
                                commonTree4 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(34, commonTree3.Token), commonTree4);
                                adaptor.AddChild(commonTree4, (CommonTree)adaptor.Create(38, commonTree3.Token, unary_expression_return.sVarName));
                                adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
                                adaptor.AddChild(commonTree, commonTree4);
                            }
                            else
                            {
                                var commonTree5 = (CommonTree)adaptor.GetNilNode();
                                commonTree5 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(35, commonTree3.Token), commonTree5);
                                adaptor.AddChild(commonTree5, (CommonTree)adaptor.Create(38, commonTree3.Token, unary_expression_return.sVarName));
                                adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
                                adaptor.AddChild(commonTree, commonTree5);
                            }
                            unary_expression_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child2 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree6 = (CommonTree)Match(input, 78, FOLLOW_NOT_in_unary_expression2123);
                            rewriteRuleNodeStream2.Add(commonTree6);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_cast_atom_in_unary_expression2125);
                            var cast_atom_return2 = cast_atom();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(cast_atom_return2.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child2);
                            MarkTempVarAsUnused(cast_atom_return2.kType, cast_atom_return2.sVarName, (((CommonTree)cast_atom_return2.Start)).Token);
                            unary_expression_return.kType = new ScriptVariableType("bool");
                            unary_expression_return.sVarName = GenerateTempVariable(unary_expression_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            unary_expression_return.kVarToken = new CommonToken(commonTree6.Token);
                            unary_expression_return.kVarToken.Type = 38;
                            unary_expression_return.kVarToken.Text = unary_expression_return.sVarName;
                            unary_expression_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", unary_expression_return.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree7 = (CommonTree)adaptor.GetNilNode();
                            commonTree7 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree7);
                            adaptor.AddChild(commonTree7, (CommonTree)adaptor.Create(38, commonTree6.Token, unary_expression_return.sVarName));
                            adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream.NextTree());
                            adaptor.AddChild(commonTree, commonTree7);
                            unary_expression_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_cast_atom_in_unary_expression2155);
                            var cast_atom_return3 = cast_atom();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, cast_atom_return3.Tree);
                            unary_expression_return.kType = ((cast_atom_return3 != null) ? cast_atom_return3.kType : null);
                            unary_expression_return.sVarName = ((cast_atom_return3 != null) ? cast_atom_return3.sVarName : null);
                            unary_expression_return.kVarToken = ((cast_atom_return3 != null) ? cast_atom_return3.kVarToken : null);
                            break;
                        }
                }
                unary_expression_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                unary_expression_stack.Pop();
            }
            return unary_expression_return;
        }

        // Token: 0x06000C72 RID: 3186 RVA: 0x0005623C File Offset: 0x0005443C
        public cast_atom_return cast_atom()
        {
            var cast_atom_return = new cast_atom_return();
            cast_atom_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token AS");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule dot_atom");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule type");
            try
            {
                var num = input.LA(1);
                int num2;
                if (num == 79)
                {
                    num2 = 1;
                }
                else
                {
                    if (num != 11 && num != 15 && num != 22 && num != 38 && num != 62 && num is < 80 or > 82 && num is < 90 or > 93)
                    {
                        var ex = new NoViableAltException("", 37, 0, input);
                        throw ex;
                    }
                    num2 = 2;
                }
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 79, FOLLOW_AS_in_cast_atom2177);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_dot_atom_in_cast_atom2179);
                            var dot_atom_return = dot_atom();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(dot_atom_return.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_type_in_cast_atom2181);
                            var type_return = type();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(type_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            if (((dot_atom_return != null) ? dot_atom_return.sVarName : null) == "")
                            {
                                OnError(
                                    $"{((dot_atom_return != null) ? dot_atom_return.kVarToken : null).Text} is not a variable", ((dot_atom_return != null) ? dot_atom_return.kVarToken : null).Line, ((dot_atom_return != null) ? dot_atom_return.kVarToken : null).CharPositionInLine);
                            }
                            MarkTempVarAsUnused((dot_atom_return != null) ? dot_atom_return.kType : null, (dot_atom_return != null) ? dot_atom_return.sVarName : null, ((dot_atom_return != null) ? ((CommonTree)dot_atom_return.Start) : null).Token);
                            cast_atom_return.kType = CheckCast((dot_atom_return != null) ? dot_atom_return.kType : null, (type_return != null) ? type_return.kType : null, commonTree3.Token);
                            cast_atom_return.sVarName = GenerateTempVariable(cast_atom_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            cast_atom_return.kVarToken = new CommonToken(commonTree3.Token);
                            cast_atom_return.kVarToken.Type = 38;
                            cast_atom_return.kVarToken.Text = cast_atom_return.sVarName;
                            cast_atom_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (cast_atom_return != null) ? cast_atom_return.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)adaptor.GetNilNode();
                            commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
                            adaptor.AddChild(commonTree4, (CommonTree)adaptor.Create(38, commonTree3.Token, cast_atom_return.sVarName));
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
                            adaptor.AddChild(commonTree, commonTree4);
                            cast_atom_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_dot_atom_in_cast_atom2211);
                            var dot_atom_return2 = dot_atom();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, dot_atom_return2.Tree);
                            if (((dot_atom_return2 != null) ? dot_atom_return2.sVarName : null) == "")
                            {
                                OnError(
                                    $"{((dot_atom_return2 != null) ? dot_atom_return2.kVarToken : null).Text} is not a variable", ((dot_atom_return2 != null) ? dot_atom_return2.kVarToken : null).Line, ((dot_atom_return2 != null) ? dot_atom_return2.kVarToken : null).CharPositionInLine);
                            }
                            cast_atom_return.kType = ((dot_atom_return2 != null) ? dot_atom_return2.kType : null);
                            cast_atom_return.sVarName = ((dot_atom_return2 != null) ? dot_atom_return2.sVarName : null);
                            cast_atom_return.kVarToken = ((dot_atom_return2 != null) ? dot_atom_return2.kVarToken : null);
                            break;
                        }
                }
                cast_atom_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return cast_atom_return;
        }

        // Token: 0x06000C73 RID: 3187 RVA: 0x00056780 File Offset: 0x00054980
        public dot_atom_return dot_atom()
        {
            var dot_atom_return = new dot_atom_return
            {
                Start = input.LT(1)
            };
            CommonTree commonTree = null;
            try
            {
                var num = input.LA(1);
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
                var ex = new NoViableAltException("", 38, 0, input);
                throw ex;
            IL_C5:
                switch (num2)
                {
                    case 1:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var treeNode = (CommonTree)Match(input, 62, FOLLOW_DOT_in_dot_atom2233);
                            var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                            commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_dot_atom_in_dot_atom2237);
                            var dot_atom_return2 = dot_atom();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree3, dot_atom_return2.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_array_func_or_id_in_dot_atom2241);
                            var array_func_or_id_return = array_func_or_id((dot_atom_return2 != null) ? dot_atom_return2.kType : null, (dot_atom_return2 != null) ? dot_atom_return2.sVarName : null);
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree3, array_func_or_id_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, commonTree3);
                            MarkTempVarAsUnused((dot_atom_return2 != null) ? dot_atom_return2.kType : null, (dot_atom_return2 != null) ? dot_atom_return2.sVarName : null, ((dot_atom_return2 != null) ? ((CommonTree)dot_atom_return2.Start) : null).Token);
                            dot_atom_return.kType = ((array_func_or_id_return != null) ? array_func_or_id_return.kType : null);
                            dot_atom_return.sVarName = ((array_func_or_id_return != null) ? array_func_or_id_return.sVarName : null);
                            dot_atom_return.kVarToken = ((array_func_or_id_return != null) ? array_func_or_id_return.kVarToken : null);
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_array_atom_in_dot_atom2254);
                            var array_atom_return = array_atom(new ScriptVariableType("none"), "!first");
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, array_atom_return.Tree);
                            dot_atom_return.kType = ((array_atom_return != null) ? array_atom_return.kType : null);
                            dot_atom_return.sVarName = ((array_atom_return != null) ? array_atom_return.sVarName : null);
                            dot_atom_return.kVarToken = ((array_atom_return != null) ? array_atom_return.kVarToken : null);
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_constant_in_dot_atom2267);
                            var constant_return = constant();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, constant_return.Tree);
                            dot_atom_return.kType = ((constant_return != null) ? constant_return.kType : null);
                            dot_atom_return.sVarName = ((constant_return != null) ? ((CommonTree)constant_return.Start) : null).Text;
                            dot_atom_return.kVarToken = ((constant_return != null) ? constant_return.kVarToken : null);
                            break;
                        }
                }
                dot_atom_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return dot_atom_return;
        }

        // Token: 0x06000C74 RID: 3188 RVA: 0x00056BF8 File Offset: 0x00054DF8
        public array_atom_return array_atom(ScriptVariableType akSelfType, string asSelfName)
        {
            array_atom_stack.Push(new array_atom_scope());
            var array_atom_return = new array_atom_return();
            array_atom_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token ARRAYGET");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule atom");
            try
            {
                var num = input.LA(1);
                int num2;
                if (num == 22)
                {
                    num2 = 1;
                }
                else
                {
                    if (num != 11 && num != 15 && num != 38 && num != 80 && num != 82)
                    {
                        var ex = new NoViableAltException("", 39, 0, input);
                        throw ex;
                    }
                    num2 = 2;
                }
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var el = (CommonTree)Match(input, 22, FOLLOW_ARRAYGET_in_array_atom2294);
                            rewriteRuleNodeStream.Add(el);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_atom_in_array_atom2296);
                            var atom_return = atom(akSelfType, asSelfName);
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(atom_return.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_array_atom2299);
                            var expression_return = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            HandleArrayElementExpression(atom_return.sVarName, atom_return.kType, atom_return.kVarToken, expression_return.sVarName, expression_return.kType, expression_return.kVarToken, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out array_atom_return.sVarName, out array_atom_return.kType, out array_atom_return.kVarToken, out ((array_atom_scope)array_atom_stack.Peek()).kautoCastTree);
                            array_atom_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (array_atom_return != null) ? array_atom_return.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree3 = (CommonTree)adaptor.GetNilNode();
                            commonTree3 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree3);
                            adaptor.AddChild(commonTree3, (CommonTree)adaptor.Create(38, array_atom_return.kVarToken));
                            adaptor.AddChild(commonTree3, (CommonTree)adaptor.Create(38, (atom_return != null) ? atom_return.kVarToken : null));
                            adaptor.AddChild(commonTree3, ((array_atom_scope)array_atom_stack.Peek()).kautoCastTree);
                            adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream2.NextTree());
                            adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream.NextTree());
                            adaptor.AddChild(commonTree, commonTree3);
                            array_atom_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_atom_in_array_atom2332);
                            var atom_return2 = atom(akSelfType, asSelfName);
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, atom_return2.Tree);
                            array_atom_return.kType = ((atom_return2 != null) ? atom_return2.kType : null);
                            array_atom_return.sVarName = ((atom_return2 != null) ? atom_return2.sVarName : null);
                            array_atom_return.kVarToken = ((atom_return2 != null) ? atom_return2.kVarToken : null);
                            break;
                        }
                }
                array_atom_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                array_atom_stack.Pop();
            }
            return array_atom_return;
        }

        // Token: 0x06000C75 RID: 3189 RVA: 0x000570D0 File Offset: 0x000552D0
        public atom_return atom(ScriptVariableType akSelfType, string asSelfName)
        {
            var atom_return = new atom_return();
            atom_return.Start = input.LT(1);
            CommonTree commonTree = null;
            CommonTree commonTree2 = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token NEW");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token INTEGER");
            var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token BASETYPE");
            var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token ID");
            try
            {
                var num = input.LA(1);
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
                var ex = new NoViableAltException("", 41, 0, input);
                throw ex;
            IL_E2:
                switch (num2)
                {
                    case 1:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree3 = (CommonTree)input.LT(1);
                            var commonTree4 = (CommonTree)adaptor.GetNilNode();
                            commonTree3 = (CommonTree)input.LT(1);
                            var treeNode = (CommonTree)Match(input, 15, FOLLOW_PAREXPR_in_atom2356);
                            var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                            commonTree4 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree4);
                            Match(input, 2, null);
                            commonTree3 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_atom2358);
                            var expression_return = expression();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree4, expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, commonTree4);
                            atom_return.kType = (expression_return.kType);
                            atom_return.sVarName = (expression_return.sVarName);
                            atom_return.kVarToken = (expression_return.kVarToken);
                            break;
                        }
                    case 2:
                        {
                            var commonTree3 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree3 = (CommonTree)input.LT(1);
                            var commonTree5 = (CommonTree)Match(input, 80, FOLLOW_NEW_in_atom2371);
                            rewriteRuleNodeStream.Add(commonTree5);
                            Match(input, 2, null);
                            var num3 = input.LA(1);
                            int num4;
                            if (num3 == 55)
                            {
                                num4 = 1;
                            }
                            else
                            {
                                if (num3 != 38)
                                {
                                    var ex2 = new NoViableAltException("", 40, 0, input);
                                    throw ex2;
                                }
                                num4 = 2;
                            }
                            switch (num4)
                            {
                                case 1:
                                    commonTree3 = (CommonTree)input.LT(1);
                                    commonTree2 = (CommonTree)Match(input, 55, FOLLOW_BASETYPE_in_atom2376);
                                    rewriteRuleNodeStream3.Add(commonTree2);
                                    break;
                                case 2:
                                    commonTree3 = (CommonTree)input.LT(1);
                                    commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_atom2382);
                                    rewriteRuleNodeStream4.Add(commonTree2);
                                    break;
                            }
                            commonTree3 = (CommonTree)input.LT(1);
                            var commonTree6 = (CommonTree)Match(input, 81, FOLLOW_INTEGER_in_atom2385);
                            rewriteRuleNodeStream2.Add(commonTree6);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            CheckArrayNew(commonTree2.Token, commonTree6.Token);
                            atom_return.kType = new ScriptVariableType($"{commonTree2.Text}[]");
                            atom_return.sVarName = GenerateTempVariable(atom_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            atom_return.kVarToken = new CommonToken(commonTree5.Token);
                            atom_return.kVarToken.Type = 38;
                            atom_return.kVarToken.Text = atom_return.sVarName;
                            atom_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", atom_return.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree7 = (CommonTree)adaptor.GetNilNode();
                            commonTree7 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree7);
                            adaptor.AddChild(commonTree7, rewriteRuleNodeStream2.NextNode());
                            adaptor.AddChild(commonTree7, (CommonTree)adaptor.Create(38, atom_return.kVarToken));
                            adaptor.AddChild(commonTree, commonTree7);
                            atom_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree3 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_func_or_id_in_atom2411);
                            var func_or_id_return = func_or_id(akSelfType, asSelfName);
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, func_or_id_return.Tree);
                            atom_return.kType = (func_or_id_return.kType);
                            atom_return.sVarName = (func_or_id_return.sVarName);
                            atom_return.kVarToken = (func_or_id_return.kVarToken);
                            break;
                        }
                }
                atom_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex3)
            {
                ReportError(ex3);
                Recover(input, ex3);
            }
            return atom_return;
        }

        // Token: 0x06000C76 RID: 3190 RVA: 0x000576E0 File Offset: 0x000558E0
        public array_func_or_id_return array_func_or_id(ScriptVariableType akSelfType, string asSelfName)
        {
            array_func_or_id_stack.Push(new array_func_or_id_scope());
            var array_func_or_id_return = new array_func_or_id_return();
            array_func_or_id_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token ARRAYGET");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule func_or_id");
            try
            {
                var num = input.LA(1);
                int num2;
                if (num == 22)
                {
                    num2 = 1;
                }
                else
                {
                    if (num != 11 && num != 38 && num != 82)
                    {
                        var ex = new NoViableAltException("", 42, 0, input);
                        throw ex;
                    }
                    num2 = 2;
                }
                switch (num2)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var el = (CommonTree)Match(input, 22, FOLLOW_ARRAYGET_in_array_func_or_id2439);
                            rewriteRuleNodeStream.Add(el);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_func_or_id_in_array_func_or_id2441);
                            var func_or_id_return = func_or_id(akSelfType, asSelfName);
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream2.Add(func_or_id_return.Tree);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_array_func_or_id2444);
                            var expression_return = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            HandleArrayElementExpression(func_or_id_return.sVarName, func_or_id_return.kType, func_or_id_return.kVarToken, expression_return.sVarName, expression_return.kType, expression_return.kVarToken, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, out array_func_or_id_return.sVarName, out array_func_or_id_return.kType, out array_func_or_id_return.kVarToken, out ((array_func_or_id_scope)array_func_or_id_stack.Peek()).kautoCastTree);
                            array_func_or_id_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", array_func_or_id_return.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree3 = (CommonTree)adaptor.GetNilNode();
                            commonTree3 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree3);
                            adaptor.AddChild(commonTree3, (CommonTree)adaptor.Create(38, array_func_or_id_return.kVarToken));
                            adaptor.AddChild(commonTree3, (CommonTree)adaptor.Create(38, func_or_id_return.kVarToken));
                            adaptor.AddChild(commonTree3, ((array_func_or_id_scope)array_func_or_id_stack.Peek()).kautoCastTree);
                            adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream2.NextTree());
                            adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream.NextTree());
                            adaptor.AddChild(commonTree, commonTree3);
                            array_func_or_id_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_func_or_id_in_array_func_or_id2477);
                            var func_or_id_return2 = func_or_id(akSelfType, asSelfName);
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, func_or_id_return2.Tree);
                            array_func_or_id_return.kType = (func_or_id_return2.kType);
                            array_func_or_id_return.sVarName = (func_or_id_return2.sVarName);
                            array_func_or_id_return.kVarToken = (func_or_id_return2.kVarToken);
                            break;
                        }
                }
                array_func_or_id_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                array_func_or_id_stack.Pop();
            }
            return array_func_or_id_return;
        }

        // Token: 0x06000C77 RID: 3191 RVA: 0x00057BAC File Offset: 0x00055DAC
        public func_or_id_return func_or_id(ScriptVariableType akSelfType, string asSelfName)
        {
            func_or_id_stack.Push(new func_or_id_scope());
            var func_or_id_return = new func_or_id_return();
            func_or_id_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token ID");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token LENGTH");
            try
            {
                var num = input.LA(1);
                int num2;
                if (num != 11)
                {
                    if (num != 38)
                    {
                        if (num != 82)
                        {
                            var ex = new NoViableAltException("", 43, 0, input);
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
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_function_call_in_func_or_id2504);
                            var function_call_return = function_call(akSelfType, asSelfName);
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, function_call_return.Tree);
                            func_or_id_return.kType = (function_call_return.kType);
                            func_or_id_return.sVarName = (function_call_return.sVarName);
                            func_or_id_return.kVarToken = (function_call_return.kVarToken);
                            break;
                        }
                    case 2:
                        {
                            var commonTree3 = (CommonTree)input.LT(1);
                            var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id2516);
                            rewriteRuleNodeStream.Add(commonTree4);
                            var knownType = GetKnownType(commonTree4.Text);
                            if (knownType != null)
                            {
                                if (asSelfName != "!first")
                                {
                                    OnError($"the type name {commonTree4.Text} cannot be used as a property", commonTree4.Line, commonTree4.CharPositionInLine);
                                }
                                func_or_id_return.kType = new ScriptVariableType(knownType.Name);
                                func_or_id_return.sVarName = "";
                                func_or_id_return.kVarToken = commonTree4.Token;
                            }
                            else if (asSelfName != "!first")
                            {
                                if (asSelfName == "")
                                {
                                    OnError("a property cannot be used directly on a type, it must be used on a variable", commonTree4.Line, commonTree4.CharPositionInLine);
                                }
                                else if (asSelfName.ToLowerInvariant() == "parent")
                                {
                                    OnError("a property cannot be used on the special parent variable, use the property directly instead", commonTree4.Line, commonTree4.CharPositionInLine);
                                }
                                GetPropertyInfo(akSelfType, commonTree4.Token, true, out func_or_id_return.kType);
                                ((func_or_id_scope)func_or_id_stack.Peek()).bisProperty = true;
                                func_or_id_return.sVarName = GenerateTempVariable(func_or_id_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                                func_or_id_return.kVarToken = new CommonToken(commonTree4.Token);
                                func_or_id_return.kVarToken.Text = func_or_id_return.sVarName;
                            }
                            else if (!((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType.bGlobal && IsLocalProperty(commonTree4.Text))
                            {
                                ((func_or_id_scope)func_or_id_stack.Peek()).bisProperty = true;
                                GetPropertyInfo(new ScriptVariableType(kObjType.Name), commonTree4.Token, true, out func_or_id_return.kType);
                                if (func_or_id_return.kType.ShadowVariableName.Length > 0)
                                {
                                    ((func_or_id_scope)func_or_id_stack.Peek()).bisLocalAutoProperty = true;
                                    func_or_id_return.sVarName = func_or_id_return.kType.ShadowVariableName;
                                }
                                else
                                {
                                    func_or_id_return.sVarName = GenerateTempVariable(func_or_id_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                                    asSelfName = "self";
                                }
                                func_or_id_return.kVarToken = new CommonToken(commonTree4.Token);
                                func_or_id_return.kVarToken.Text = func_or_id_return.sVarName;
                            }
                            else
                            {
                                func_or_id_return.kType = GetVariableType(((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, commonTree4.Token);
                                ((func_or_id_scope)func_or_id_stack.Peek()).bisProperty = false;
                                func_or_id_return.sVarName = commonTree4.Text;
                                func_or_id_return.kVarToken = commonTree4.Token;
                            }
                            func_or_id_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", func_or_id_return.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            if (((func_or_id_scope)func_or_id_stack.Peek()).bisProperty && !((func_or_id_scope)func_or_id_stack.Peek()).bisLocalAutoProperty)
                            {
                                var commonTree5 = (CommonTree)adaptor.GetNilNode();
                                commonTree5 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(20, commonTree4.Token, "propget"), commonTree5);
                                adaptor.AddChild(commonTree5, (CommonTree)adaptor.Create(38, commonTree4.Token, asSelfName));
                                adaptor.AddChild(commonTree5, rewriteRuleNodeStream.NextNode());
                                adaptor.AddChild(commonTree5, (CommonTree)adaptor.Create(38, commonTree4.Token, func_or_id_return.sVarName));
                                adaptor.AddChild(commonTree, commonTree5);
                            }
                            else if (((func_or_id_scope)func_or_id_stack.Peek()).bisProperty && ((func_or_id_scope)func_or_id_stack.Peek()).bisLocalAutoProperty)
                            {
                                var commonTree6 = (CommonTree)adaptor.GetNilNode();
                                commonTree6 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(38, func_or_id_return.kVarToken, func_or_id_return.sVarName), commonTree6);
                                adaptor.AddChild(commonTree, commonTree6);
                            }
                            else
                            {
                                adaptor.AddChild(commonTree, rewriteRuleNodeStream.NextNode());
                            }
                            func_or_id_return.Tree = commonTree;
                            break;
                        }
                    case 3:
                        {
                            var commonTree7 = (CommonTree)input.LT(1);
                            var commonTree8 = (CommonTree)Match(input, 82, FOLLOW_LENGTH_in_func_or_id2571);
                            rewriteRuleNodeStream2.Add(commonTree8);
                            if (asSelfName == "!first")
                            {
                                OnError("length must be called on an array", commonTree8.Line, commonTree8.CharPositionInLine);
                            }
                            else if (!akSelfType.IsArray)
                            {
                                OnError($"cannot get the length of {asSelfName} as it isn't an array", commonTree8.Line, commonTree8.CharPositionInLine);
                            }
                            func_or_id_return.kType = new ScriptVariableType("int");
                            func_or_id_return.sVarName = GenerateTempVariable(func_or_id_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            func_or_id_return.kVarToken = new CommonToken(commonTree8.Token);
                            func_or_id_return.kVarToken.Type = 38;
                            func_or_id_return.kVarToken.Text = func_or_id_return.sVarName;
                            func_or_id_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", func_or_id_return.Tree);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree9 = (CommonTree)adaptor.GetNilNode();
                            commonTree9 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree9);
                            adaptor.AddChild(commonTree9, (CommonTree)adaptor.Create(38, commonTree8.Token, asSelfName));
                            adaptor.AddChild(commonTree9, (CommonTree)adaptor.Create(38, commonTree8.Token, func_or_id_return.sVarName));
                            adaptor.AddChild(commonTree, commonTree9);
                            func_or_id_return.Tree = commonTree;
                            break;
                        }
                }
                func_or_id_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            finally
            {
                func_or_id_stack.Pop();
            }
            return func_or_id_return;
        }

        // Token: 0x06000C78 RID: 3192 RVA: 0x00058418 File Offset: 0x00056618
        public return_stat_return return_stat()
        {
            return_stat_stack.Push(new return_stat_scope());
            var return_stat_return = new return_stat_return();
            return_stat_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token RETURN");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
            try
            {
                var num = input.LA(1);
                if (num != 83)
                {
                    var ex = new NoViableAltException("", 44, 0, input);
                    throw ex;
                }
                var num2 = input.LA(2);
                int num3;
                if (num2 == 2)
                {
                    num3 = 1;
                }
                else
                {
                    if (num2 != 3 && num2 != 5 && num2 != 11 && num2 is < 15 or > 16 && (num2 != 22 && num2 != 38 && num2 != 41 && num2 != 62 && num2 is < 65 or > 84) && num2 != 88 && num2 is < 90 or > 93)
                    {
                        var ex2 = new NoViableAltException("", 44, 1, input);
                        throw ex2;
                    }
                    num3 = 2;
                }
                switch (num3)
                {
                    case 1:
                        {
                            var commonTree2 = (CommonTree)input.LT(1);
                            var child = (CommonTree)adaptor.GetNilNode();
                            commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 83, FOLLOW_RETURN_in_return_stat2611);
                            rewriteRuleNodeStream.Add(commonTree3);
                            Match(input, 2, null);
                            commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_expression_in_return_stat2613);
                            var expression_return = expression();
                            state.followingStackPointer--;
                            rewriteRuleSubtreeStream.Add(expression_return.Tree);
                            Match(input, 3, null);
                            adaptor.AddChild(commonTree, child);
                            CheckReturnType(((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType, (expression_return != null) ? expression_return.kType : null, commonTree3.Token);
                            ((return_stat_scope)return_stat_stack.Peek()).kautoCastTree = AutoCastReturn((expression_return != null) ? expression_return.kVarToken : null, (expression_return != null) ? expression_return.kType : null, ((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType.Name, ((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType.PropertyName, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars, commonTree3.Token);
                            return_stat_return.Tree = commonTree;
                            new RewriteRuleSubtreeStream(adaptor, "rule retval", (return_stat_return != null) ? return_stat_return.Tree : null);
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)adaptor.GetNilNode();
                            commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
                            adaptor.AddChild(commonTree4, ((return_stat_scope)return_stat_stack.Peek()).kautoCastTree);
                            adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
                            adaptor.AddChild(commonTree, commonTree4);
                            return_stat_return.Tree = commonTree;
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree5 = (CommonTree)Match(input, 83, FOLLOW_RETURN_in_return_stat2638);
                            var child2 = (CommonTree)adaptor.DupNode(commonTree5);
                            adaptor.AddChild(commonTree, child2);
                            CheckReturnType(((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType, null, commonTree5.Token);
                            break;
                        }
                }
                return_stat_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex3)
            {
                ReportError(ex3);
                Recover(input, ex3);
            }
            finally
            {
                return_stat_stack.Pop();
            }
            return return_stat_return;
        }

        // Token: 0x06000C79 RID: 3193 RVA: 0x0005889C File Offset: 0x00056A9C
        public ifBlock_return ifBlock()
        {
            ifBlock_stack.Push(new ifBlock_scope());
            var ifBlock_return = new ifBlock_return();
            ifBlock_return.Start = input.LT(1);
            ((ifBlock_scope)ifBlock_stack.Peek()).kchildScope = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.Children[((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild];
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 84, FOLLOW_IF_in_ifBlock2672);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_expression_in_ifBlock2674);
                var expression_return = expression();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, expression_return.Tree);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_codeBlock_in_ifBlock2676);
                var codeBlock_return = codeBlock(((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType, ((ifBlock_scope)ifBlock_stack.Peek()).kchildScope);
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, codeBlock_return.Tree);
                for (; ; )
                {
                    var num = 2;
                    var num2 = input.LA(1);
                    if (num2 == 86)
                    {
                        num = 1;
                    }
                    var num3 = num;
                    if (num3 != 1)
                    {
                        break;
                    }
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_elseIfBlock_in_ifBlock2679);
                    var elseIfBlock_return = elseIfBlock();
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, elseIfBlock_return.Tree);
                }
                var num4 = 2;
                var num5 = input.LA(1);
                if (num5 == 87)
                {
                    num4 = 1;
                }
                var num6 = num4;
                if (num6 == 1)
                {
                    commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_elseBlock_in_ifBlock2682);
                    var elseBlock_return = elseBlock();
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree3, elseBlock_return.Tree);
                }
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, ((expression_return != null) ? ((CommonTree)expression_return.Start) : null).Token);
                ifBlock_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
                ((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild++;
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                ifBlock_stack.Pop();
            }
            return ifBlock_return;
        }

        // Token: 0x06000C7A RID: 3194 RVA: 0x00058C5C File Offset: 0x00056E5C
        public elseIfBlock_return elseIfBlock()
        {
            elseIfBlock_stack.Push(new elseIfBlock_scope());
            var elseIfBlock_return = new elseIfBlock_return();
            elseIfBlock_return.Start = input.LT(1);
            ((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild++;
            ((elseIfBlock_scope)elseIfBlock_stack.Peek()).kchildScope = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.Children[((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild];
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 86, FOLLOW_ELSEIF_in_elseIfBlock2711);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_expression_in_elseIfBlock2713);
                var expression_return = expression();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, expression_return.Tree);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_codeBlock_in_elseIfBlock2715);
                var codeBlock_return = codeBlock(((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType, ((elseIfBlock_scope)elseIfBlock_stack.Peek()).kchildScope);
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, codeBlock_return.Tree);
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, ((expression_return != null) ? ((CommonTree)expression_return.Start) : null).Token);
                elseIfBlock_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                elseIfBlock_stack.Pop();
            }
            return elseIfBlock_return;
        }

        // Token: 0x06000C7B RID: 3195 RVA: 0x00058F34 File Offset: 0x00057134
        public elseBlock_return elseBlock()
        {
            elseBlock_stack.Push(new elseBlock_scope());
            var elseBlock_return = new elseBlock_return();
            elseBlock_return.Start = input.LT(1);
            ((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild++;
            ((elseBlock_scope)elseBlock_stack.Peek()).kchildScope = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.Children[((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild];
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 87, FOLLOW_ELSE_in_elseBlock2744);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_codeBlock_in_elseBlock2746);
                var codeBlock_return = codeBlock(((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType, ((elseBlock_scope)elseBlock_stack.Peek()).kchildScope);
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, codeBlock_return.Tree);
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                elseBlock_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                elseBlock_stack.Pop();
            }
            return elseBlock_return;
        }

        // Token: 0x06000C7C RID: 3196 RVA: 0x00059184 File Offset: 0x00057384
        public whileBlock_return whileBlock()
        {
            whileBlock_stack.Push(new whileBlock_scope());
            var whileBlock_return = new whileBlock_return();
            whileBlock_return.Start = input.LT(1);
            ((whileBlock_scope)whileBlock_stack.Peek()).kchildScope = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.Children[((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild];
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 88, FOLLOW_WHILE_in_whileBlock2777);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_expression_in_whileBlock2779);
                var expression_return = expression();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, expression_return.Tree);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_codeBlock_in_whileBlock2781);
                var codeBlock_return = codeBlock(((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType, ((whileBlock_scope)whileBlock_stack.Peek()).kchildScope);
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, codeBlock_return.Tree);
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                MarkTempVarAsUnused((expression_return != null) ? expression_return.kType : null, (expression_return != null) ? expression_return.sVarName : null, ((expression_return != null) ? ((CommonTree)expression_return.Start) : null).Token);
                whileBlock_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
                ((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild++;
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                whileBlock_stack.Pop();
            }
            return whileBlock_return;
        }

        // Token: 0x06000C7D RID: 3197 RVA: 0x0005945C File Offset: 0x0005765C
        public function_call_return function_call(ScriptVariableType akSelfType, string asSelfName)
        {
            function_call_stack.Push(new function_call_scope());
            var function_call_return = new function_call_return();
            function_call_return.Start = input.LT(1);
            CommonTree commonTree = null;
            var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token CALL");
            var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
            var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token CALLPARAMS");
            var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule parameters");
            ((function_call_scope)function_call_stack.Peek()).ktargetParamNames = new List<string>();
            ((function_call_scope)function_call_stack.Peek()).kparamTypes = new List<ScriptVariableType>();
            ((function_call_scope)function_call_stack.Peek()).kparamVarNames = new List<string>();
            ((function_call_scope)function_call_stack.Peek()).kparamTokens = new List<IToken>();
            ((function_call_scope)function_call_stack.Peek()).kparamExpressions = new List<CommonTree>();
            ((function_call_scope)function_call_stack.Peek()).kparamAutoCasts = new List<CommonTree>();
            ((function_call_scope)function_call_stack.Peek()).bisGlobal = false;
            ((function_call_scope)function_call_stack.Peek()).bisArray = false;
            try
            {
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 11, FOLLOW_CALL_in_function_call2818);
                rewriteRuleNodeStream.Add(commonTree4);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2820);
                rewriteRuleNodeStream2.Add(commonTree5);
                commonTree2 = (CommonTree)input.LT(1);
                var child = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var el = (CommonTree)Match(input, 14, FOLLOW_CALLPARAMS_in_function_call2823);
                rewriteRuleNodeStream3.Add(el);
                if (input.LA(1) == 2)
                {
                    Match(input, 2, null);
                    var num = 2;
                    var num2 = input.LA(1);
                    if (num2 == 9)
                    {
                        num = 1;
                    }
                    var num3 = num;
                    if (num3 == 1)
                    {
                        commonTree2 = (CommonTree)input.LT(1);
                        PushFollow(FOLLOW_parameters_in_function_call2825);
                        var parameters_return = parameters();
                        state.followingStackPointer--;
                        rewriteRuleSubtreeStream.Add(parameters_return.Tree);
                    }
                    Match(input, 3, null);
                }
                adaptor.AddChild(commonTree3, child);
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                var flag = false;
                if (asSelfName == "!first")
                {
                    akSelfType = FindFunctionOwningType(commonTree5.Text, out flag, commonTree5.Token);
                    asSelfName = "";
                }
                if (akSelfType != null)
                {
                    if (asSelfName != "" && akSelfType.IsArray)
                    {
                        ((function_call_scope)function_call_stack.Peek()).bisArray = true;
                        function_call_return.kType = CheckArrayFunctionCall(akSelfType, commonTree5.Text, ((function_call_scope)function_call_stack.Peek()).ktargetParamNames, ((function_call_scope)function_call_stack.Peek()).kparamTypes, ((function_call_scope)function_call_stack.Peek()).kparamTokens, ref ((function_call_scope)function_call_stack.Peek()).kparamExpressions, out ((function_call_scope)function_call_stack.Peek()).kparamAutoCasts, commonTree5.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                    }
                    else
                    {
                        ((function_call_scope)function_call_stack.Peek()).bisGlobal = IsGlobalFunction(akSelfType, commonTree5.Text);
                        if (((function_call_scope)function_call_stack.Peek()).bisGlobal)
                        {
                            function_call_return.kType = CheckGlobalFunctionCall(akSelfType, commonTree5.Text, ((function_call_scope)function_call_stack.Peek()).ktargetParamNames, ((function_call_scope)function_call_stack.Peek()).kparamTypes, ((function_call_scope)function_call_stack.Peek()).kparamTokens, ref ((function_call_scope)function_call_stack.Peek()).kparamExpressions, out ((function_call_scope)function_call_stack.Peek()).kparamAutoCasts, commonTree5.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            if (asSelfName != "")
                            {
                                OnError(
                                    $"cannot call the global function {commonTree5.Text} on the variable {asSelfName}, must call it alone or on a type", commonTree4.Line, commonTree4.CharPositionInLine);
                            }
                            asSelfName = akSelfType.VarType;
                        }
                        else
                        {
                            function_call_return.kType = CheckMemberFunctionCall(akSelfType, commonTree5.Text, ((function_call_scope)function_call_stack.Peek()).ktargetParamNames, ((function_call_scope)function_call_stack.Peek()).kparamTypes, ((function_call_scope)function_call_stack.Peek()).kparamTokens, ref ((function_call_scope)function_call_stack.Peek()).kparamExpressions, out ((function_call_scope)function_call_stack.Peek()).kparamAutoCasts, commonTree5.Token, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                            if (flag)
                            {
                                asSelfName = "self";
                            }
                            else if (asSelfName == "")
                            {
                                OnError(
                                    $"cannot call the member function {commonTree5.Text} alone or on a type, must call it on a variable", commonTree4.Line, commonTree4.CharPositionInLine);
                            }
                        }
                        if (flag && !((function_call_scope)function_call_stack.Peek()).bisGlobal && ((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType.bGlobal)
                        {
                            OnError(
                                $"cannot call member function {commonTree5.Text} in global function {((codeBlock_scope)codeBlock_stack.Peek()).kfunctionType.Name} without a variable to call it on", commonTree4.Line, commonTree4.CharPositionInLine);
                        }
                    }
                }
                else
                {
                    function_call_return.kType = new ScriptVariableType("none");
                }
                for (var i = 0; i < ((function_call_scope)function_call_stack.Peek()).kparamVarNames.Count; i++)
                {
                    MarkTempVarAsUnused(((function_call_scope)function_call_stack.Peek()).kparamTypes[i], ((function_call_scope)function_call_stack.Peek()).kparamVarNames[i], commonTree5.Token);
                }
                function_call_return.sVarName = GenerateTempVariable(function_call_return.kType, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope, ((codeBlock_scope)codeBlock_stack.Peek()).kTempVars);
                function_call_return.kVarToken = new CommonToken(commonTree5.Token);
                function_call_return.kVarToken.Text = function_call_return.sVarName;
                function_call_return.Tree = commonTree;
                new RewriteRuleSubtreeStream(adaptor, "rule retval", function_call_return.Tree);
                commonTree = (CommonTree)adaptor.GetNilNode();
                adaptor.AddChild(commonTree, CreateCallTree(commonTree4.Token, ((function_call_scope)function_call_stack.Peek()).bisGlobal, ((function_call_scope)function_call_stack.Peek()).bisArray, asSelfName, commonTree5.Token, function_call_return.kType, function_call_return.sVarName, ((function_call_scope)function_call_stack.Peek()).kparamAutoCasts, ((function_call_scope)function_call_stack.Peek()).kparamExpressions));
                function_call_return.Tree = commonTree;
                function_call_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            finally
            {
                function_call_stack.Pop();
            }
            return function_call_return;
        }

        // Token: 0x06000C7E RID: 3198 RVA: 0x00059D2C File Offset: 0x00057F2C
        public parameters_return parameters()
        {
            var parameters_return = new parameters_return();
            parameters_return.Start = input.LT(1);
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var num = 0;
                for (; ; )
                {
                    var num2 = 2;
                    var num3 = input.LA(1);
                    if (num3 == 9)
                    {
                        num2 = 1;
                    }
                    var num4 = num2;
                    if (num4 != 1)
                    {
                        break;
                    }
                    var commonTree2 = (CommonTree)input.LT(1);
                    PushFollow(FOLLOW_parameter_in_parameters2864);
                    var parameter_return = parameter();
                    state.followingStackPointer--;
                    adaptor.AddChild(commonTree, parameter_return.Tree);
                    num++;
                }
                if (num < 1)
                {
                    var ex = new EarlyExitException(48, input);
                    throw ex;
                }
                parameters_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return parameters_return;
        }

        // Token: 0x06000C7F RID: 3199 RVA: 0x00059E3C File Offset: 0x0005803C
        public parameter_return parameter()
        {
            var parameter_return = new parameter_return();
            parameter_return.Start = input.LT(1);
            try
            {
                var commonTree = (CommonTree)adaptor.GetNilNode();
                var commonTree2 = (CommonTree)input.LT(1);
                var commonTree3 = (CommonTree)adaptor.GetNilNode();
                commonTree2 = (CommonTree)input.LT(1);
                var treeNode = (CommonTree)Match(input, 9, FOLLOW_PARAM_in_parameter2878);
                var newRoot = (CommonTree)adaptor.DupNode(treeNode);
                commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
                Match(input, 2, null);
                commonTree2 = (CommonTree)input.LT(1);
                var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_parameter2880);
                var child = (CommonTree)adaptor.DupNode(commonTree4);
                adaptor.AddChild(commonTree3, child);
                commonTree2 = (CommonTree)input.LT(1);
                PushFollow(FOLLOW_expression_in_parameter2882);
                var expression_return = expression();
                state.followingStackPointer--;
                adaptor.AddChild(commonTree3, expression_return.Tree);
                Match(input, 3, null);
                adaptor.AddChild(commonTree, commonTree3);
                ((function_call_scope)function_call_stack.Peek()).ktargetParamNames.Add(commonTree4.Text);
                ((function_call_scope)function_call_stack.Peek()).kparamTypes.Add(expression_return.kType);
                ((function_call_scope)function_call_stack.Peek()).kparamVarNames.Add(expression_return.sVarName);
                ((function_call_scope)function_call_stack.Peek()).kparamTokens.Add(expression_return.kVarToken);
                ((function_call_scope)function_call_stack.Peek()).kparamExpressions.Add(((CommonTree)expression_return.Tree));
                parameter_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex)
            {
                ReportError(ex);
                Recover(input, ex);
            }
            return parameter_return;
        }

        // Token: 0x06000C80 RID: 3200 RVA: 0x0005A0E0 File Offset: 0x000582E0
        public constant_return constant()
        {
            var constant_return = new constant_return();
            constant_return.Start = input.LT(1);
            CommonTree commonTree = null;
            try
            {
                var num = input.LA(1);
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
                                var ex = new NoViableAltException("", 49, 0, input);
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
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            PushFollow(FOLLOW_number_in_constant2905);
                            var number_return = number();
                            state.followingStackPointer--;
                            adaptor.AddChild(commonTree, number_return.Tree);
                            constant_return.kType = ((number_return != null) ? number_return.kType : null);
                            constant_return.kVarToken = ((number_return != null) ? number_return.kVarToken : null);
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree3 = (CommonTree)input.LT(1);
                            var commonTree4 = (CommonTree)Match(input, 90, FOLLOW_STRING_in_constant2916);
                            var child = (CommonTree)adaptor.DupNode(commonTree4);
                            adaptor.AddChild(commonTree, child);
                            constant_return.kType = new ScriptVariableType("string");
                            constant_return.kVarToken = commonTree4.Token;
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree5 = (CommonTree)input.LT(1);
                            var commonTree6 = (CommonTree)Match(input, 91, FOLLOW_BOOL_in_constant2927);
                            var child2 = (CommonTree)adaptor.DupNode(commonTree6);
                            adaptor.AddChild(commonTree, child2);
                            constant_return.kType = new ScriptVariableType("bool");
                            constant_return.kVarToken = commonTree6.Token;
                            break;
                        }
                    case 4:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree7 = (CommonTree)input.LT(1);
                            var commonTree8 = (CommonTree)Match(input, 92, FOLLOW_NONE_in_constant2938);
                            var child3 = (CommonTree)adaptor.DupNode(commonTree8);
                            adaptor.AddChild(commonTree, child3);
                            constant_return.kType = new ScriptVariableType("none");
                            constant_return.kVarToken = commonTree8.Token;
                            break;
                        }
                }
                constant_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return constant_return;
        }

        // Token: 0x06000C81 RID: 3201 RVA: 0x0005A3EC File Offset: 0x000585EC
        public number_return number()
        {
            var number_return = new number_return();
            number_return.Start = input.LT(1);
            CommonTree commonTree = null;
            try
            {
                var num = input.LA(1);
                int num2;
                if (num == 81)
                {
                    num2 = 1;
                }
                else
                {
                    if (num != 93)
                    {
                        var ex = new NoViableAltException("", 50, 0, input);
                        throw ex;
                    }
                    num2 = 2;
                }
                switch (num2)
                {
                    case 1:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 81, FOLLOW_INTEGER_in_number2960);
                            var child = (CommonTree)adaptor.DupNode(commonTree3);
                            adaptor.AddChild(commonTree, child);
                            number_return.kType = new ScriptVariableType("int");
                            number_return.kVarToken = commonTree3.Token;
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)input.LT(1);
                            var commonTree5 = (CommonTree)Match(input, 93, FOLLOW_FLOAT_in_number2971);
                            var child2 = (CommonTree)adaptor.DupNode(commonTree5);
                            adaptor.AddChild(commonTree, child2);
                            number_return.kType = new ScriptVariableType("float");
                            number_return.kVarToken = commonTree5.Token;
                            break;
                        }
                }
                number_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex2)
            {
                ReportError(ex2);
                Recover(input, ex2);
            }
            return number_return;
        }

        // Token: 0x06000C82 RID: 3202 RVA: 0x0005A5BC File Offset: 0x000587BC
        public type_return type()
        {
            var type_return = new type_return();
            type_return.Start = input.LT(1);
            CommonTree commonTree = null;
            try
            {
                var num = input.LA(1);
                int num3;
                if (num == 38)
                {
                    var num2 = input.LA(2);
                    if (num2 == 63)
                    {
                        num3 = 2;
                    }
                    else
                    {
                        if (num2 != 3 && num2 != 38)
                        {
                            var ex = new NoViableAltException("", 51, 1, input);
                            throw ex;
                        }
                        num3 = 1;
                    }
                }
                else
                {
                    if (num != 55)
                    {
                        var ex2 = new NoViableAltException("", 51, 0, input);
                        throw ex2;
                    }
                    var num4 = input.LA(2);
                    if (num4 == 63)
                    {
                        num3 = 4;
                    }
                    else
                    {
                        if (num4 != 3 && num4 != 38)
                        {
                            var ex3 = new NoViableAltException("", 51, 2, input);
                            throw ex3;
                        }
                        num3 = 3;
                    }
                }
                switch (num3)
                {
                    case 1:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree2 = (CommonTree)input.LT(1);
                            var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_type2993);
                            var child = (CommonTree)adaptor.DupNode(commonTree3);
                            adaptor.AddChild(commonTree, child);
                            type_return.kType = new ScriptVariableType(commonTree3.Text);
                            break;
                        }
                    case 2:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree4 = (CommonTree)input.LT(1);
                            var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_type3004);
                            var child2 = (CommonTree)adaptor.DupNode(commonTree5);
                            adaptor.AddChild(commonTree, child2);
                            var commonTree6 = (CommonTree)input.LT(1);
                            var treeNode = (CommonTree)Match(input, 63, FOLLOW_LBRACKET_in_type3006);
                            var child3 = (CommonTree)adaptor.DupNode(treeNode);
                            adaptor.AddChild(commonTree, child3);
                            var commonTree7 = (CommonTree)input.LT(1);
                            var treeNode2 = (CommonTree)Match(input, 64, FOLLOW_RBRACKET_in_type3008);
                            var child4 = (CommonTree)adaptor.DupNode(treeNode2);
                            adaptor.AddChild(commonTree, child4);
                            type_return.kType = new ScriptVariableType($"{commonTree5.Text}[]");
                            break;
                        }
                    case 3:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree8 = (CommonTree)input.LT(1);
                            var commonTree9 = (CommonTree)Match(input, 55, FOLLOW_BASETYPE_in_type3019);
                            var child5 = (CommonTree)adaptor.DupNode(commonTree9);
                            adaptor.AddChild(commonTree, child5);
                            type_return.kType = new ScriptVariableType(commonTree9.Text);
                            break;
                        }
                    case 4:
                        {
                            commonTree = (CommonTree)adaptor.GetNilNode();
                            var commonTree10 = (CommonTree)input.LT(1);
                            var commonTree11 = (CommonTree)Match(input, 55, FOLLOW_BASETYPE_in_type3030);
                            var child6 = (CommonTree)adaptor.DupNode(commonTree11);
                            adaptor.AddChild(commonTree, child6);
                            var commonTree12 = (CommonTree)input.LT(1);
                            var treeNode3 = (CommonTree)Match(input, 63, FOLLOW_LBRACKET_in_type3032);
                            var child7 = (CommonTree)adaptor.DupNode(treeNode3);
                            adaptor.AddChild(commonTree, child7);
                            var commonTree13 = (CommonTree)input.LT(1);
                            var treeNode4 = (CommonTree)Match(input, 64, FOLLOW_RBRACKET_in_type3034);
                            var child8 = (CommonTree)adaptor.DupNode(treeNode4);
                            adaptor.AddChild(commonTree, child8);
                            type_return.kType = new ScriptVariableType($"{commonTree11.Text}[]");
                            break;
                        }
                }
                type_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
            }
            catch (RecognitionException ex4)
            {
                ReportError(ex4);
                Recover(input, ex4);
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
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
                    return tree;
                }
                set
                {
                    tree = (CommonTree)value;
                }
            }

            // Token: 0x040009BB RID: 2491
            public ScriptVariableType kType;

            // Token: 0x040009BC RID: 2492
            private CommonTree tree;
        }
    }
}
