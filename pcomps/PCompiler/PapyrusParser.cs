using System;
using System.Collections;
using System.Collections.Generic;
using pcomps.Antlr.Runtime;
using pcomps.Antlr.Runtime.Collections;
using pcomps.Antlr.Runtime.Tree;

namespace pcomps.PCompiler
{
	// Token: 0x020000EB RID: 235
	public class PapyrusParser : Parser
	{
		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600098F RID: 2447 RVA: 0x0001BC24 File Offset: 0x00019E24
		public ScriptObjectType ParsedObject => kObjType;

        // Token: 0x170000F3 RID: 243
		// (set) Token: 0x06000990 RID: 2448 RVA: 0x0001BC2C File Offset: 0x00019E2C
		internal Dictionary<string, PapyrusFlag> KnownUserFlags
		{
			set => kFlagDict = value;
        }

		// Token: 0x06000991 RID: 2449 RVA: 0x0001BC38 File Offset: 0x00019E38
		private ScriptVariableType ConstructVarType(string asType, IToken akInitialValue)
		{
			var scriptVariableType = new ScriptVariableType(asType);
            if (akInitialValue == null) return scriptVariableType;
            scriptVariableType.InitialValue = akInitialValue.Text;
            return scriptVariableType;
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x0001BC5C File Offset: 0x00019E5C
		private void SetAutoState(string asState, IToken akAutoToken)
		{
			if (!string.Equals(kObjType.sAutoState, "", StringComparison.Ordinal) && kObjType.sAutoState != asState.ToLowerInvariant())
			{
				OnError(
                    $"script already has the automatic state set to {kObjType.sAutoState}, cannot have more then one", akAutoToken.Line, akAutoToken.CharPositionInLine);
				return;
			}
			kObjType.sAutoState = asState.ToLowerInvariant();
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x0001BCD4 File Offset: 0x00019ED4
		private void AddObjectVariable(string asName, ScriptVariableType akType, IToken akSourceToken)
        {
            if (kObjType.TrySetVariable(asName, akType)) return;
            OnError($"script variable {asName} already defined", akSourceToken.Line, akSourceToken.CharPositionInLine);
        }

		// Token: 0x06000994 RID: 2452 RVA: 0x0001BD04 File Offset: 0x00019F04
		private void AddObjectProperty(ScriptVariableType akType, string asName, IToken akSourceToken, bool abHasShadowVariable)
		{
			string a = asName.ToLowerInvariant();
			if (a is "self" or "parent")
			{
				OnError($"you cannot have a property named {asName}", akSourceToken.Line, akSourceToken.CharPositionInLine);
			}
			if (abHasShadowVariable)
			{
				akType.ShadowVariableName = $"::{asName}_var";
			}
			if (!kObjType.TrySetProperty(asName, akType))
			{
				OnError($"script property {asName} already defined", akSourceToken.Line, akSourceToken.CharPositionInLine);
			}
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x0001BD98 File Offset: 0x00019F98
		private void TrySetPropertyFunction(string asPropName, ScriptFunctionType akFunctionType, IToken akSourceToken)
		{
            if (!kObjType.TryGetProperty(asPropName, out var scriptPropertyType))
			{
				OnError($"internal error: cannot find property {asPropName}", akSourceToken.Line, akSourceToken.CharPositionInLine);
				return;
			}
			string a = akFunctionType.Name.ToLowerInvariant();
			if (a == "get")
			{
				if (scriptPropertyType.kGetFunction == null)
				{
					scriptPropertyType.kGetFunction = akFunctionType;
					return;
				}
				OnError($"script property {asPropName} already has a get function defined", akSourceToken.Line, akSourceToken.CharPositionInLine);
				return;
			}
			else
			{
				if (!(a == "set"))
				{
					OnError($"script property {asPropName} can only contain get or set functions", akSourceToken.Line, akSourceToken.CharPositionInLine);
					return;
				}
				if (scriptPropertyType.kSetFunction == null)
				{
					scriptPropertyType.kSetFunction = akFunctionType;
					return;
				}
				OnError($"script property {asPropName} already has a set function defined", akSourceToken.Line, akSourceToken.CharPositionInLine);
				return;
			}
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x0001BE78 File Offset: 0x0001A078
		private void AddObjectFunction(string asName, ScriptFunctionType akType, IToken akSourceToken, bool abIsEvent)
		{
			AddObjectFunction("", asName, akType, akSourceToken, abIsEvent);
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x0001BE8C File Offset: 0x0001A08C
		private void AddObjectFunction(string asStateName, string asName, ScriptFunctionType akType, IToken akSourceToken, bool abIsEvent)
		{
			if (!kObjType.TrySetFunction(asStateName, asName, akType))
			{
				OnError(string.Format("script {0} {1} already defined in the same state", abIsEvent ? "event" : "function", asName), akSourceToken.Line, akSourceToken.CharPositionInLine);
			}
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x0001BED8 File Offset: 0x0001A0D8
		private void AddFunctionParameter(List<string> akParamNames, List<ScriptVariableType> akParamTypes, string asName, ScriptVariableType akType, IToken akDefaultValue, IToken akErrorToken)
		{
			if (akDefaultValue != null)
			{
				akType.InitialValue = akDefaultValue.Text;
			}
			int num = akParamNames.IndexOf(asName.ToLowerInvariant());
			if (num == -1)
			{
				akParamNames.Add(asName.ToLowerInvariant());
				akParamTypes.Add(akType);
				return;
			}
			OnError($"parameter {asName} already defined", akErrorToken.Line, akErrorToken.CharPositionInLine);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x0001BF44 File Offset: 0x0001A144
		private void AddLocalVariable(ScriptScope akCurrentScope, string asName, ScriptVariableType akType, IToken akSourceToken)
		{
            if (kObjType.TryGetVariable(asName, out var scriptVariableType))
			{
				OnError($"function variable {asName} already defined in the containing script", akSourceToken.Line, akSourceToken.CharPositionInLine);
				return;
			}
			if (akCurrentScope.VariableWouldShadow(asName))
			{
				OnError($"function variable {asName} may not shadow a previously defined variable", akSourceToken.Line, akSourceToken.CharPositionInLine);
				return;
			}
			if (!akCurrentScope.TryDefineVariable(asName, akType))
			{
				OnError($"function variable {asName} already defined in the same scope", akSourceToken.Line, akSourceToken.CharPositionInLine);
			}
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x0001BFD4 File Offset: 0x0001A1D4
		private void GenerateReservedFunctionVars(ScriptFunctionType akFunctionType)
		{
			if (!akFunctionType.bGlobal)
			{
				akFunctionType.FunctionScope.TryDefineVariable("self", new ScriptVariableType(kObjType.Name));
				if (sParentObjName != "")
				{
					akFunctionType.FunctionScope.TryDefineVariable("parent", new ScriptVariableType(sParentObjName));
				}
			}
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x0001C038 File Offset: 0x0001A238
		private void CheckObjectName(IToken akNameToken)
		{
			string a = akNameToken.Text.ToLowerInvariant();
            if (a is not ("self" or "parent")) return;
            OnError(string.Format("script name {0} is invalid, please pick a different one", akNameToken.Text), akNameToken.Line, akNameToken.CharPositionInLine);
        }

		// Token: 0x0600099C RID: 2460 RVA: 0x0001C090 File Offset: 0x0001A290
		private IToken CreateToken(IToken akBaseToken, int aiType, string asText) =>
            new CommonToken(akBaseToken)
            {
                Type = aiType,
                Text = asText
            };

        // Token: 0x0600099D RID: 2461 RVA: 0x0001C0B4 File Offset: 0x0001A2B4
		private CommonTree CreateAutoPropertyVarTree(string asVarName, ITree akTypeTree, ScriptVariableType akType, string asVarUserFlags, ITree akVarInitialValue, IToken akRootToken)
		{
			IToken t = CreateToken(akRootToken, 5, "variable");
			IToken t2 = CreateToken(akRootToken, 38, asVarName);
			IToken t3 = CreateToken(akRootToken, 18, asVarUserFlags);
			CommonTree commonTree = new CommonTree(t);
			commonTree.AddChild(akTypeTree);
			commonTree.AddChild(new CommonTree(t2));
			commonTree.AddChild(new CommonTree(t3));
			if (akVarInitialValue != null)
			{
				commonTree.AddChild(akVarInitialValue);
			}
			AddObjectVariable(asVarName, akType, akRootToken);
			return commonTree;
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x0001C128 File Offset: 0x0001A328
		private CommonTree CreateTypeTree(ScriptVariableType akType, IToken akRootToken)
		{
			CommonTree commonTree = new CommonTree();
			int aiType = akType.IsObjectType ? 38 : 55;
			string asText = akType.VarType;
			if (akType.IsArray)
			{
				asText = akType.ArrayElementType;
				aiType = (akType.IsElementObjectType ? 38 : 55);
			}
			commonTree.AddChild(new CommonTree(CreateToken(akRootToken, aiType, asText)));
			if (akType.IsArray)
			{
				commonTree.AddChild(new CommonTree(CreateToken(akRootToken, 63, "[")));
				commonTree.AddChild(new CommonTree(CreateToken(akRootToken, 64, "]")));
			}
			return commonTree;
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x0001C1C0 File Offset: 0x0001A3C0
		private CommonTree CreateAutoPropertyTree(ITree akHeader, string asPropertyName, ScriptVariableType akType, string asVarUserFlags, ITree akVarInitialValue)
		{
			IToken token = ((CommonTree)akHeader).Token;
			CommonTree akTypeTree = CreateTypeTree(akType, token);
			string text = "";
			ScriptPropertyType scriptPropertyType;
			if (kObjType.TryGetProperty(asPropertyName, out scriptPropertyType))
			{
				text = scriptPropertyType.kType.ShadowVariableName;
			}
			if (text.Length == 0)
			{
				OnError(string.Format("script does not appear to have a properly-constructed auto property named {0}", asPropertyName), akHeader.Line, akHeader.CharPositionInLine);
			}
			CommonTree t = CreateAutoPropertyVarTree(text, akTypeTree, akType, asVarUserFlags, akVarInitialValue, token);
			TrySetPropertyFunction(asPropertyName, new ScriptFunctionType("get", kObjType.Name, "", asPropertyName)
			{
				kRetType = akType
			}, token);
			ScriptFunctionType scriptFunctionType = new ScriptFunctionType("set", kObjType.Name, "", asPropertyName);
			scriptFunctionType.TryAddParam("value", akType);
			TrySetPropertyFunction(asPropertyName, scriptFunctionType, token);
			CommonTree commonTree = new CommonTree(CreateToken(token, 19, "autoProperty"));
			commonTree.AddChild(akHeader);
			commonTree.AddChild(new CommonTree(CreateToken(token, 38, text)));
			CommonTree commonTree2 = new CommonTree();
			commonTree2.AddChild(t);
			commonTree2.AddChild(commonTree);
			return commonTree2;
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x0001C2EC File Offset: 0x0001A4EC
		private CommonTree CreateAutoReadOnlyPropertyGetFunction(string asPropertyName, ITree akValue, ITree akTypeTree, ScriptVariableType akType, IToken akRootToken)
		{
			CommonTree commonTree = new CommonTree(CreateToken(akRootToken, 8, "header"));
			commonTree.AddChild(akTypeTree);
			commonTree.AddChild(new CommonTree(CreateToken(akRootToken, 38, "get")));
			commonTree.AddChild(new CommonTree(CreateToken(akRootToken, 18, "0")));
			CommonTree commonTree2 = new CommonTree(CreateToken(akRootToken, 83, "return"));
			commonTree2.AddChild(akValue);
			CommonTree commonTree3 = new CommonTree(CreateToken(akRootToken, 10, "block"));
			commonTree3.AddChild(commonTree2);
			CommonTree commonTree4 = new CommonTree(CreateToken(akRootToken, 6, "function"));
			commonTree4.AddChild(commonTree);
			commonTree4.AddChild(commonTree3);
			TrySetPropertyFunction(asPropertyName, new ScriptFunctionType("get", kObjType.Name, "", asPropertyName)
			{
				kRetType = akType
			}, akRootToken);
			CommonTree commonTree5 = new CommonTree(CreateToken(akRootToken, 17, "propfunc"));
			commonTree5.AddChild(commonTree4);
			return commonTree5;
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x0001C3F4 File Offset: 0x0001A5F4
		private CommonTree CreateAutoReadOnlyPropertyTree(ITree akHeader, string asPropertyName, ScriptVariableType akType, ITree akValue)
		{
			IToken token = ((CommonTree)akHeader).Token;
			CommonTree akTypeTree = CreateTypeTree(akType, token);
			CommonTree t = CreateAutoReadOnlyPropertyGetFunction(asPropertyName, akValue, akTypeTree, akType, token);
			CommonTree commonTree = new CommonTree(CreateToken(token, 54, "property"));
			commonTree.AddChild(akHeader);
			commonTree.AddChild(t);
			return commonTree;
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x0001C448 File Offset: 0x0001A648
		private string ProcessUserFlags(List<string> akFlagList, FlagIsValid akIsValid, string asErrorTypeStr, IToken akSourceToken)
		{
			uint num = 0U;
			if (akFlagList != null)
			{
				foreach (string text in akFlagList)
				{
					string key = text.ToLowerInvariant();
					if (kFlagDict.ContainsKey(key))
					{
						PapyrusFlag papyrusFlag = kFlagDict[key];
						if (!akIsValid(papyrusFlag))
						{
							OnError(string.Format("Flag {0} is not allowed on {1}", text, asErrorTypeStr), akSourceToken.Line, akSourceToken.CharPositionInLine);
						}
						else
						{
							num |= (uint)Math.Pow(2.0, papyrusFlag.Index);
						}
					}
					else
					{
						OnError(string.Format("Unknown user flag {0}", text), akSourceToken.Line, akSourceToken.CharPositionInLine);
					}
				}
			}
			return num.ToString();
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x0001C52C File Offset: 0x0001A72C
		private string ProcessObjectUserFlags(List<string> akFlagList, IToken akSourceToken)
		{
			return ProcessUserFlags(akFlagList, (PapyrusFlag akFlag) => akFlag.AllowedOnObj, "scripts", akSourceToken);
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x0001C558 File Offset: 0x0001A758
		private string ProcessVarUserFlags(List<string> akFlagList, IToken akSourceToken)
		{
			return ProcessUserFlags(akFlagList, (PapyrusFlag akFlag) => akFlag.AllowedOnVar, "variables", akSourceToken);
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x0001C584 File Offset: 0x0001A784
		private string ProcessPropUserFlags(List<string> akFlagList, IToken akSourceToken)
		{
			return ProcessUserFlags(akFlagList, (PapyrusFlag akFlag) => akFlag.AllowedOnProp, "properties", akSourceToken);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x0001C5B0 File Offset: 0x0001A7B0
		private void ProcessAutoPropUserFlags(List<string> akFlagList, IToken akSourceToken, out string asPropFlags, out string asVarFlags)
		{
			uint num = 0U;
			uint num2 = 0U;
			if (akFlagList != null)
			{
				foreach (string text in akFlagList)
				{
					string key = text.ToLowerInvariant();
					if (kFlagDict.ContainsKey(key))
					{
						PapyrusFlag papyrusFlag = kFlagDict[key];
						if (papyrusFlag.AllowedOnProp)
						{
							num |= (uint)Math.Pow(2.0, (double)papyrusFlag.Index);
						}
						if (papyrusFlag.AllowedOnVar)
						{
							num2 |= (uint)Math.Pow(2.0, (double)papyrusFlag.Index);
						}
						if (!papyrusFlag.AllowedOnProp && !papyrusFlag.AllowedOnVar)
						{
							OnError(string.Format("Flag {0} is not allowed on a auto property", text), akSourceToken.Line, akSourceToken.CharPositionInLine);
						}
					}
					else
					{
						OnError(string.Format("Unknown user flag {0}", text), akSourceToken.Line, akSourceToken.CharPositionInLine);
					}
				}
			}
			asPropFlags = num.ToString();
			asVarFlags = num2.ToString();
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x0001C6D8 File Offset: 0x0001A8D8
		private string ProcessFuncUserFlags(List<string> akFlagList, IToken akSourceToken)
		{
			return ProcessUserFlags(akFlagList, (PapyrusFlag akFlag) => akFlag.AllowedOnFunc, "functions", akSourceToken);
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x0001C704 File Offset: 0x0001A904
		public PapyrusParser(ITokenStream input) : this(input, new RecognizerSharedState())
		{
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x0001C714 File Offset: 0x0001A914
		public PapyrusParser(ITokenStream input, RecognizerSharedState state) : base(input, state)
		{
			InitializeCyclicDFAs();
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060009AA RID: 2474 RVA: 0x0001C7D4 File Offset: 0x0001A9D4
		// (set) Token: 0x060009AB RID: 2475 RVA: 0x0001C7DC File Offset: 0x0001A9DC
		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return adaptor;
			}
			set
			{
				adaptor = value;
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060009AC RID: 2476 RVA: 0x0001C7E8 File Offset: 0x0001A9E8
		public override string[] TokenNames
		{
			get
			{
				return tokenNames;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060009AD RID: 2477 RVA: 0x0001C7F0 File Offset: 0x0001A9F0
		public override string GrammarFileName
		{
			get
			{
				return "Papyrus.g";
			}
		}

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x060009AE RID: 2478 RVA: 0x0001C7F8 File Offset: 0x0001A9F8
		// (remove) Token: 0x060009AF RID: 2479 RVA: 0x0001C814 File Offset: 0x0001AA14
		internal event InternalErrorEventHandler ErrorHandler;

		// Token: 0x060009B0 RID: 2480 RVA: 0x0001C830 File Offset: 0x0001AA30
		private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
        {
            ErrorHandler?.Invoke(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
        }

		// Token: 0x060009B1 RID: 2481 RVA: 0x0001C850 File Offset: 0x0001AA50
		public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			string errorMessage = GetErrorMessage(e, tokenNames);
			OnError(errorMessage, e.Line, e.CharPositionInLine);
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x0001C87C File Offset: 0x0001AA7C
		public script_return script()
		{
			script_return script_return = new script_return();
			script_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token EOF");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule header");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule definitionOrBlock");
			try
			{
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 94)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_terminator_in_script266);
					terminator_return terminator_return = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return script_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(terminator_return.Tree);
					}
				}
				PushFollow(FOLLOW_header_in_script269);
				header_return header_return = header();
				state.followingStackPointer--;
				if (state.failed)
				{
					return script_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(header_return.Tree);
				}
				for (;;)
				{
					int num4 = 2;
					int num5 = input.LA(1);
					if ((num5 >= 6 && num5 <= 7) || (num5 == 38 || num5 == 42 || (num5 >= 50 && num5 <= 51)) || num5 == 55)
					{
						num4 = 1;
					}
					int num6 = num4;
					if (num6 != 1)
					{
						goto IL_1DC;
					}
					PushFollow(FOLLOW_definitionOrBlock_in_script271);
					definitionOrBlock_return definitionOrBlock_return = definitionOrBlock();
					state.followingStackPointer--;
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(definitionOrBlock_return.Tree);
					}
				}
				return script_return;
				IL_1DC:
				IToken el = (IToken)Match(input, -1, FOLLOW_EOF_in_script274);
				if (state.failed)
				{
					return script_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(el);
				}
				if (state.backtracking == 0)
				{
					script_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (script_return != null) ? script_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(4, (header_return != null) ? ((IToken)header_return.Start) : null, "object"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
					while (rewriteRuleSubtreeStream3.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream3.NextTree());
					}
					rewriteRuleSubtreeStream3.Reset();
					adaptor.AddChild(obj, obj2);
					script_return.Tree = obj;
					script_return.Tree = obj;
				}
				script_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					script_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(script_return.Tree, (IToken)script_return.Start, (IToken)script_return.Stop);
				}
				if (state.backtracking == 0)
				{
					kObjType.kAST = (CommonTree)script_return.Tree;
					kObjType.kTokenStream = input;
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				script_return.Tree = adaptor.ErrorNode(input, (IToken)script_return.Start, input.LT(-1), ex);
			}
			return script_return;
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x0001CC78 File Offset: 0x0001AE78
		public header_return header()
		{
			header_stack.Push(new header_scope());
			header_return header_return = new header_return();
			header_return.Start = input.LT(1);
			object obj = null;
			IToken token = null;
			userFlags_return userFlags_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token SCRIPTNAME");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token DOCSTRING");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token EXTENDS");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule userFlags");
			try
			{
				IToken token2 = (IToken)Match(input, 37, FOLLOW_SCRIPTNAME_in_header308);
				if (state.failed)
				{
					return header_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(token2);
				}
				IToken token3 = (IToken)Match(input, 38, FOLLOW_ID_in_header312);
				if (state.failed)
				{
					return header_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream3.Add(token3);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 39)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					IToken el = (IToken)Match(input, 39, FOLLOW_EXTENDS_in_header315);
					if (state.failed)
					{
						return header_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream4.Add(el);
					}
					token = (IToken)Match(input, 38, FOLLOW_ID_in_header319);
					if (state.failed)
					{
						return header_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(token);
					}
				}
				int num4 = 2;
				int num5 = input.LA(1);
				if (num5 == 38)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					PushFollow(FOLLOW_userFlags_in_header323);
					userFlags_return = userFlags();
					state.followingStackPointer--;
					if (state.failed)
					{
						return header_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(userFlags_return.Tree);
					}
				}
				PushFollow(FOLLOW_terminator_in_header326);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return header_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(terminator_return.Tree);
				}
				int num7 = 2;
				int num8 = input.LA(1);
				if (num8 == 40)
				{
					num7 = 1;
				}
				int num9 = num7;
				if (num9 == 1)
				{
					IToken el2 = (IToken)Match(input, 40, FOLLOW_DOCSTRING_in_header329);
					if (state.failed)
					{
						return header_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el2);
					}
					PushFollow(FOLLOW_terminator_in_header331);
					terminator_return terminator_return2 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return header_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(terminator_return2.Tree);
					}
				}
				if (state.backtracking == 0)
				{
					CheckObjectName(token3);
					kObjType = new ScriptObjectType(token3.Text);
					sParentObjName = ((token != null) ? token.Text : "");
					((header_scope)header_stack.Peek()).sobjFlags = ProcessObjectUserFlags((userFlags_return != null) ? userFlags_return.kFlagList : null, token2);
				}
				if (state.backtracking == 0)
				{
					header_return.Tree = obj;
					RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token objName", token3);
					RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token parent", token);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (header_return != null) ? header_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(rewriteRuleTokenStream5.NextNode(), obj2);
					adaptor.AddChild(obj2, adaptor.Create(18, token2, ((header_scope)header_stack.Peek()).sobjFlags));
					if (rewriteRuleTokenStream6.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream6.NextNode());
					}
					rewriteRuleTokenStream6.Reset();
					if (rewriteRuleTokenStream2.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
					}
					rewriteRuleTokenStream2.Reset();
					adaptor.AddChild(obj, obj2);
					header_return.Tree = obj;
					header_return.Tree = obj;
				}
				header_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					header_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(header_return.Tree, (IToken)header_return.Start, (IToken)header_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				header_return.Tree = adaptor.ErrorNode(input, (IToken)header_return.Start, input.LT(-1), ex);
			}
			finally
			{
				header_stack.Pop();
			}
			return header_return;
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x0001D268 File Offset: 0x0001B468
		public definitionOrBlock_return definitionOrBlock()
		{
			definitionOrBlock_return definitionOrBlock_return = new definitionOrBlock_return();
			definitionOrBlock_return.Start = input.LT(1);
			object obj = null;
			try
			{
				switch (dfa6.Predict(input))
				{
				case 1:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_fieldDefinition_in_definitionOrBlock370);
					fieldDefinition_return fieldDefinition_return = fieldDefinition();
					state.followingStackPointer--;
					if (state.failed)
					{
						return definitionOrBlock_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, fieldDefinition_return.Tree);
					}
					if (state.backtracking == 0)
					{
						AddObjectVariable((fieldDefinition_return != null) ? fieldDefinition_return.sVarName : null, (fieldDefinition_return != null) ? fieldDefinition_return.kVarType : null, (fieldDefinition_return != null) ? ((IToken)fieldDefinition_return.Start) : null);
					}
					break;
				}
				case 2:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_import_obj_in_definitionOrBlock381);
					import_obj_return import_obj_return = import_obj();
					state.followingStackPointer--;
					if (state.failed)
					{
						return definitionOrBlock_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, import_obj_return.Tree);
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_function_in_definitionOrBlock387);
					function_return function_return = function(true, "", "");
					state.followingStackPointer--;
					if (state.failed)
					{
						return definitionOrBlock_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, function_return.Tree);
					}
					if (state.backtracking == 0)
					{
						AddObjectFunction((function_return != null) ? function_return.sFuncName : null, (function_return != null) ? function_return.kFunction : null, (function_return != null) ? ((IToken)function_return.Start) : null, false);
					}
					break;
				}
				case 4:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_eventFunc_in_definitionOrBlock400);
					eventFunc_return eventFunc_return = eventFunc("");
					state.followingStackPointer--;
					if (state.failed)
					{
						return definitionOrBlock_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, eventFunc_return.Tree);
					}
					if (state.backtracking == 0)
					{
						AddObjectFunction((eventFunc_return != null) ? eventFunc_return.sEventName : null, (eventFunc_return != null) ? eventFunc_return.kEventFunction : null, (eventFunc_return != null) ? ((IToken)eventFunc_return.Start) : null, true);
					}
					break;
				}
				case 5:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_stateBlock_in_definitionOrBlock412);
					stateBlock_return stateBlock_return = stateBlock();
					state.followingStackPointer--;
					if (state.failed)
					{
						return definitionOrBlock_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, stateBlock_return.Tree);
					}
					break;
				}
				case 6:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_propertyBlock_in_definitionOrBlock418);
					propertyBlock_return propertyBlock_return = propertyBlock();
					state.followingStackPointer--;
					if (state.failed)
					{
						return definitionOrBlock_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, propertyBlock_return.Tree);
					}
					break;
				}
				}
				definitionOrBlock_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					definitionOrBlock_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(definitionOrBlock_return.Tree, (IToken)definitionOrBlock_return.Start, (IToken)definitionOrBlock_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				definitionOrBlock_return.Tree = adaptor.ErrorNode(input, (IToken)definitionOrBlock_return.Start, input.LT(-1), ex);
			}
			return definitionOrBlock_return;
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x0001D700 File Offset: 0x0001B900
		public userFlags_return userFlags()
		{
			userFlags_return userFlags_return = new userFlags_return();
			userFlags_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ID");
			userFlags_return.kFlagList = new List<string>();
			try
			{
				IToken token = (IToken)Match(input, 38, FOLLOW_ID_in_userFlags441);
				if (state.failed)
				{
					return userFlags_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(token);
				}
				if (state.backtracking == 0)
				{
					userFlags_return.kFlagList.Add(token.Text);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 38)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_125;
					}
					IToken token2 = (IToken)Match(input, 38, FOLLOW_ID_in_userFlags460);
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token2);
					}
					if (state.backtracking == 0)
					{
						userFlags_return.kFlagList.Add(token2.Text);
					}
				}
				return userFlags_return;
				IL_125:
				if (state.backtracking == 0)
				{
					userFlags_return.Tree = obj;
					RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token firstOne", token);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (userFlags_return != null) ? userFlags_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(18, token, "user_flags"), obj2);
					adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
					while (rewriteRuleTokenStream.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
					}
					rewriteRuleTokenStream.Reset();
					adaptor.AddChild(obj, obj2);
					userFlags_return.Tree = obj;
					userFlags_return.Tree = obj;
				}
				userFlags_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					userFlags_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(userFlags_return.Tree, (IToken)userFlags_return.Start, (IToken)userFlags_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				userFlags_return.Tree = adaptor.ErrorNode(input, (IToken)userFlags_return.Start, input.LT(-1), ex);
			}
			return userFlags_return;
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x0001D9D0 File Offset: 0x0001BBD0
		public fieldDefinition_return fieldDefinition()
		{
			fieldDefinition_stack.Push(new fieldDefinition_scope());
			fieldDefinition_return fieldDefinition_return = new fieldDefinition_return();
			fieldDefinition_return.Start = input.LT(1);
			object obj = null;
			constant_return constant_return = null;
			userFlags_return userFlags_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token EQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule constant");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule type");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule userFlags");
			fieldDefinition_return.sVarName = "";
			fieldDefinition_return.kVarType = null;
			((fieldDefinition_scope)fieldDefinition_stack.Peek()).svarFlags = "";
			try
			{
				PushFollow(FOLLOW_type_in_fieldDefinition515);
				type_return type_return = type();
				state.followingStackPointer--;
				if (state.failed)
				{
					return fieldDefinition_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream3.Add(type_return.Tree);
				}
				IToken token = (IToken)Match(input, 38, FOLLOW_ID_in_fieldDefinition517);
				if (state.failed)
				{
					return fieldDefinition_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(token);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 41)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					IToken el = (IToken)Match(input, 41, FOLLOW_EQUALS_in_fieldDefinition520);
					if (state.failed)
					{
						return fieldDefinition_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
					PushFollow(FOLLOW_constant_in_fieldDefinition522);
					constant_return = constant();
					state.followingStackPointer--;
					if (state.failed)
					{
						return fieldDefinition_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(constant_return.Tree);
					}
				}
				int num4 = 2;
				int num5 = input.LA(1);
				if (num5 == 38)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					PushFollow(FOLLOW_userFlags_in_fieldDefinition526);
					userFlags_return = userFlags();
					state.followingStackPointer--;
					if (state.failed)
					{
						return fieldDefinition_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream4.Add(userFlags_return.Tree);
					}
				}
				PushFollow(FOLLOW_terminator_in_fieldDefinition529);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return fieldDefinition_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
				}
				if (state.backtracking == 0)
				{
					fieldDefinition_return.kVarType = ((type_return != null) ? type_return.kType : null);
					if (((constant_return != null) ? ((IToken)constant_return.Start) : null) != null)
					{
						fieldDefinition_return.kVarType.InitialValue = ((constant_return != null) ? ((IToken)constant_return.Start) : null).Text;
					}
					fieldDefinition_return.sVarName = token.Text;
					((fieldDefinition_scope)fieldDefinition_stack.Peek()).svarFlags = ProcessVarUserFlags((userFlags_return != null) ? userFlags_return.kFlagList : null, token);
				}
				if (state.backtracking == 0)
				{
					fieldDefinition_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (fieldDefinition_return != null) ? fieldDefinition_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(5, token, "variable"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream3.NextTree());
					adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
					adaptor.AddChild(obj2, adaptor.Create(18, token, ((fieldDefinition_scope)fieldDefinition_stack.Peek()).svarFlags));
					if (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					adaptor.AddChild(obj, obj2);
					fieldDefinition_return.Tree = obj;
					fieldDefinition_return.Tree = obj;
				}
				fieldDefinition_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					fieldDefinition_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(fieldDefinition_return.Tree, (IToken)fieldDefinition_return.Start, (IToken)fieldDefinition_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				fieldDefinition_return.Tree = adaptor.ErrorNode(input, (IToken)fieldDefinition_return.Start, input.LT(-1), ex);
			}
			finally
			{
				fieldDefinition_stack.Pop();
			}
			return fieldDefinition_return;
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x0001DF5C File Offset: 0x0001C15C
		public import_obj_return import_obj()
		{
			import_obj_return import_obj_return = new import_obj_return();
			import_obj_return.Start = input.LT(1);
			try
			{
				object obj = adaptor.GetNilNode();
				IToken payload = (IToken)Match(input, 42, FOLLOW_IMPORT_in_import_obj568);
				if (state.failed)
				{
					return import_obj_return;
				}
				if (state.backtracking == 0)
				{
					object newRoot = adaptor.Create(payload);
					obj = adaptor.BecomeRoot(newRoot, obj);
				}
				IToken payload2 = (IToken)Match(input, 38, FOLLOW_ID_in_import_obj571);
				if (state.failed)
				{
					return import_obj_return;
				}
				if (state.backtracking == 0)
				{
					object child = adaptor.Create(payload2);
					adaptor.AddChild(obj, child);
				}
				PushFollow(FOLLOW_terminator_in_import_obj573);
				terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return import_obj_return;
				}
				import_obj_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					import_obj_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(import_obj_return.Tree, (IToken)import_obj_return.Start, (IToken)import_obj_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				import_obj_return.Tree = adaptor.ErrorNode(input, (IToken)import_obj_return.Start, input.LT(-1), ex);
			}
			return import_obj_return;
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x0001E148 File Offset: 0x0001C348
		public function_return function(bool abAllowGlobal, string asStateName, string asPropertyName)
		{
			function_stack.Push(new function_scope());
			function_return function_return = new function_return();
			function_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule functionHeader");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule functionBlock");
			((function_scope)function_stack.Peek()).ballowGlobal = abAllowGlobal;
			((function_scope)function_stack.Peek()).sstateName = asStateName;
			((function_scope)function_stack.Peek()).spropertyName = asPropertyName;
			try
			{
				PushFollow(FOLLOW_functionHeader_in_function606);
				functionHeader_return functionHeader_return = functionHeader();
				state.followingStackPointer--;
				if (state.failed)
				{
					return function_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(functionHeader_return.Tree);
				}
				PushFollow(FOLLOW_functionBlock_in_function608);
				functionBlock_return functionBlock_return = functionBlock();
				state.followingStackPointer--;
				if (state.failed)
				{
					return function_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(functionBlock_return.Tree);
				}
				if (state.backtracking == 0)
				{
					function_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (function_return != null) ? function_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(6, (functionHeader_return != null) ? ((IToken)functionHeader_return.Start) : null, "function"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
					if (rewriteRuleSubtreeStream2.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
					}
					rewriteRuleSubtreeStream2.Reset();
					adaptor.AddChild(obj, obj2);
					function_return.Tree = obj;
					function_return.Tree = obj;
				}
				function_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					function_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(function_return.Tree, (IToken)function_return.Start, (IToken)function_return.Stop);
				}
				if (state.backtracking == 0)
				{
					function_return.sFuncName = ((function_scope)function_stack.Peek()).kfunction.Name;
					function_return.kFunction = ((function_scope)function_stack.Peek()).kfunction;
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				function_return.Tree = adaptor.ErrorNode(input, (IToken)function_return.Start, input.LT(-1), ex);
			}
			finally
			{
				function_stack.Pop();
			}
			return function_return;
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x0001E498 File Offset: 0x0001C698
		public functionHeader_return functionHeader()
		{
			functionHeader_stack.Push(new functionHeader_scope());
			functionHeader_return functionHeader_return = new functionHeader_return();
			functionHeader_return.Start = input.LT(1);
			object obj = null;
			type_return type_return = null;
			userFlags_return userFlags_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token FUNCTION");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token DOCSTRING");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule callParameters");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule functionModifier");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule type");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule userFlags");
			((functionHeader_scope)functionHeader_stack.Peek()).bglobal = false;
			((functionHeader_scope)functionHeader_stack.Peek()).bnative = false;
			((functionHeader_scope)functionHeader_stack.Peek()).kparamNames = new List<string>();
			((functionHeader_scope)functionHeader_stack.Peek()).kparamTypes = new List<ScriptVariableType>();
			try
			{
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 38 || num2 == 55)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_type_in_functionHeader645);
					type_return = type();
					state.followingStackPointer--;
					if (state.failed)
					{
						return functionHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream4.Add(type_return.Tree);
					}
				}
				IToken token = (IToken)Match(input, 6, FOLLOW_FUNCTION_in_functionHeader648);
				if (state.failed)
				{
					return functionHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(token);
				}
				IToken token2 = (IToken)Match(input, 38, FOLLOW_ID_in_functionHeader652);
				if (state.failed)
				{
					return functionHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream4.Add(token2);
				}
				IToken el = (IToken)Match(input, 43, FOLLOW_LPAREN_in_functionHeader654);
				if (state.failed)
				{
					return functionHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream5.Add(el);
				}
				int num4 = 2;
				int num5 = input.LA(1);
				if (num5 == 38 || num5 == 55)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					PushFollow(FOLLOW_callParameters_in_functionHeader656);
					callParameters_return callParameters_return = callParameters(((functionHeader_scope)functionHeader_stack.Peek()).kparamNames, ((functionHeader_scope)functionHeader_stack.Peek()).kparamTypes);
					state.followingStackPointer--;
					if (state.failed)
					{
						return functionHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(callParameters_return.Tree);
					}
				}
				IToken el2 = (IToken)Match(input, 44, FOLLOW_RPAREN_in_functionHeader660);
				if (state.failed)
				{
					return functionHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(el2);
				}
				for (;;)
				{
					int num7 = 2;
					int num8 = input.LA(1);
					if (num8 == 46 && ((function_scope)function_stack.Peek()).ballowGlobal)
					{
						num7 = 1;
					}
					else if (num8 == 47)
					{
						num7 = 1;
					}
					int num9 = num7;
					if (num9 != 1)
					{
						goto IL_437;
					}
					PushFollow(FOLLOW_functionModifier_in_functionHeader662);
					functionModifier_return functionModifier_return = functionModifier();
					state.followingStackPointer--;
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(functionModifier_return.Tree);
					}
				}
				return functionHeader_return;
				IL_437:
				int num10 = 2;
				int num11 = input.LA(1);
				if (num11 == 38)
				{
					num10 = 1;
				}
				int num12 = num10;
				if (num12 == 1)
				{
					PushFollow(FOLLOW_userFlags_in_functionHeader665);
					userFlags_return = userFlags();
					state.followingStackPointer--;
					if (state.failed)
					{
						return functionHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream5.Add(userFlags_return.Tree);
					}
				}
				int num13 = dfa14.Predict(input);
				int num14 = num13;
				if (num14 == 1)
				{
					PushFollow(FOLLOW_terminator_in_functionHeader669);
					terminator_return terminator_return = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return functionHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
					}
					IToken el3 = (IToken)Match(input, 40, FOLLOW_DOCSTRING_in_functionHeader671);
					if (state.failed)
					{
						return functionHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(el3);
					}
				}
				if (state.backtracking == 0)
				{
					((function_scope)function_stack.Peek()).kfunction = new ScriptFunctionType(token2.Text, kObjType.Name, ((function_scope)function_stack.Peek()).sstateName, ((function_scope)function_stack.Peek()).spropertyName);
					if (((type_return != null) ? ((IToken)type_return.Start) : null) != null)
					{
						((function_scope)function_stack.Peek()).kfunction.kRetType = ((type_return != null) ? type_return.kType : null);
					}
					else
					{
						((function_scope)function_stack.Peek()).kfunction.kRetType = new ScriptVariableType("none");
					}
					((function_scope)function_stack.Peek()).kfunction.bNative = ((functionHeader_scope)functionHeader_stack.Peek()).bnative;
					((function_scope)function_stack.Peek()).kfunction.bGlobal = ((functionHeader_scope)functionHeader_stack.Peek()).bglobal;
					((functionHeader_scope)functionHeader_stack.Peek()).suserFlags = ProcessFuncUserFlags((userFlags_return != null) ? userFlags_return.kFlagList : null, token);
					GenerateReservedFunctionVars(((function_scope)function_stack.Peek()).kfunction);
					for (int i = 0; i < ((functionHeader_scope)functionHeader_stack.Peek()).kparamNames.Count; i++)
					{
						((function_scope)function_stack.Peek()).kfunction.TryAddParam(((functionHeader_scope)functionHeader_stack.Peek()).kparamNames[i], ((functionHeader_scope)functionHeader_stack.Peek()).kparamTypes[i]);
						AddLocalVariable(((function_scope)function_stack.Peek()).kfunction.FunctionScope, ((functionHeader_scope)functionHeader_stack.Peek()).kparamNames[i], ((functionHeader_scope)functionHeader_stack.Peek()).kparamTypes[i], token2);
					}
				}
				if (state.backtracking == 0)
				{
					functionHeader_return.Tree = obj;
					RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token name", token2);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (functionHeader_return != null) ? functionHeader_return.Tree : null);
					obj = adaptor.GetNilNode();
					if (((type_return != null) ? ((IToken)type_return.Start) : null) != null)
					{
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(8, token, "header"), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream4.NextTree());
						adaptor.AddChild(obj2, rewriteRuleTokenStream6.NextNode());
						adaptor.AddChild(obj2, adaptor.Create(18, token, ((functionHeader_scope)functionHeader_stack.Peek()).suserFlags));
						if (rewriteRuleSubtreeStream.HasNext())
						{
							adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
						}
						rewriteRuleSubtreeStream.Reset();
						while (rewriteRuleSubtreeStream3.HasNext())
						{
							adaptor.AddChild(obj2, rewriteRuleSubtreeStream3.NextTree());
						}
						rewriteRuleSubtreeStream3.Reset();
						if (rewriteRuleTokenStream3.HasNext())
						{
							adaptor.AddChild(obj2, rewriteRuleTokenStream3.NextNode());
						}
						rewriteRuleTokenStream3.Reset();
						adaptor.AddChild(obj, obj2);
					}
					else
					{
						object obj3 = adaptor.GetNilNode();
						obj3 = adaptor.BecomeRoot(adaptor.Create(8, token, "header"), obj3);
						adaptor.AddChild(obj3, adaptor.Create(92, "NONE"));
						adaptor.AddChild(obj3, rewriteRuleTokenStream4.NextNode());
						adaptor.AddChild(obj3, adaptor.Create(18, token, ((functionHeader_scope)functionHeader_stack.Peek()).suserFlags));
						if (rewriteRuleSubtreeStream.HasNext())
						{
							adaptor.AddChild(obj3, rewriteRuleSubtreeStream.NextTree());
						}
						rewriteRuleSubtreeStream.Reset();
						while (rewriteRuleSubtreeStream3.HasNext())
						{
							adaptor.AddChild(obj3, rewriteRuleSubtreeStream3.NextTree());
						}
						rewriteRuleSubtreeStream3.Reset();
						if (rewriteRuleTokenStream3.HasNext())
						{
							adaptor.AddChild(obj3, rewriteRuleTokenStream3.NextNode());
						}
						rewriteRuleTokenStream3.Reset();
						adaptor.AddChild(obj, obj3);
					}
					functionHeader_return.Tree = obj;
					functionHeader_return.Tree = obj;
				}
				functionHeader_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					functionHeader_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(functionHeader_return.Tree, (IToken)functionHeader_return.Start, (IToken)functionHeader_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				functionHeader_return.Tree = adaptor.ErrorNode(input, (IToken)functionHeader_return.Start, input.LT(-1), ex);
			}
			finally
			{
				functionHeader_stack.Pop();
			}
			return functionHeader_return;
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x0001EFD0 File Offset: 0x0001D1D0
		public functionBlock_return functionBlock()
		{
			functionBlock_return functionBlock_return = new functionBlock_return();
			functionBlock_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ENDFUNCTION");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule statement");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			try
			{
				switch (dfa16.Predict(input))
				{
				case 1:
					if (!((function_scope)function_stack.Peek()).kfunction.bNative)
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return functionBlock_return;
						}
						throw new FailedPredicateException(input, "functionBlock", "$function::kfunction.bNative");
					}
					else
					{
						PushFollow(FOLLOW_terminator_in_functionBlock748);
						terminator_return terminator_return = terminator();
						state.followingStackPointer--;
						if (state.failed)
						{
							return functionBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
						}
						if (state.backtracking == 0)
						{
							functionBlock_return.Tree = obj;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", (functionBlock_return != null) ? functionBlock_return.Tree : null);
							obj = adaptor.GetNilNode();
							obj = null;
							functionBlock_return.Tree = obj;
							functionBlock_return.Tree = obj;
						}
					}
					break;
				case 2:
					if (((function_scope)function_stack.Peek()).kfunction.bNative)
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return functionBlock_return;
						}
						throw new FailedPredicateException(input, "functionBlock", "!$function::kfunction.bNative");
					}
					else
					{
						PushFollow(FOLLOW_terminator_in_functionBlock763);
						terminator_return terminator_return2 = terminator();
						state.followingStackPointer--;
						if (state.failed)
						{
							return functionBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream2.Add(terminator_return2.Tree);
						}
						for (;;)
						{
							int num = 2;
							int num2 = input.LA(1);
							if (num2 == 38 || num2 == 43 || num2 == 55 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 84) || num2 == 88 || (num2 >= 90 && num2 <= 93))
							{
								num = 1;
							}
							int num3 = num;
							if (num3 != 1)
							{
								goto IL_305;
							}
							PushFollow(FOLLOW_statement_in_functionBlock765);
							statement_return statement_return = statement(((function_scope)function_stack.Peek()).kfunction.FunctionScope);
							state.followingStackPointer--;
							if (state.failed)
							{
								break;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream.Add(statement_return.Tree);
							}
						}
						return functionBlock_return;
						IL_305:
						IToken el = (IToken)Match(input, 45, FOLLOW_ENDFUNCTION_in_functionBlock769);
						if (state.failed)
						{
							return functionBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream.Add(el);
						}
						PushFollow(FOLLOW_terminator_in_functionBlock771);
						terminator_return terminator_return3 = terminator();
						state.followingStackPointer--;
						if (state.failed)
						{
							return functionBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream2.Add(terminator_return3.Tree);
						}
						if (state.backtracking == 0)
						{
							functionBlock_return.Tree = obj;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", (functionBlock_return != null) ? functionBlock_return.Tree : null);
							obj = adaptor.GetNilNode();
							object obj2 = adaptor.GetNilNode();
							obj2 = adaptor.BecomeRoot(adaptor.Create(10, "BLOCK"), obj2);
							while (rewriteRuleSubtreeStream.HasNext())
							{
								adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
							}
							rewriteRuleSubtreeStream.Reset();
							adaptor.AddChild(obj, obj2);
							functionBlock_return.Tree = obj;
							functionBlock_return.Tree = obj;
						}
					}
					break;
				}
				functionBlock_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					functionBlock_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(functionBlock_return.Tree, (IToken)functionBlock_return.Start, (IToken)functionBlock_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				functionBlock_return.Tree = adaptor.ErrorNode(input, (IToken)functionBlock_return.Start, input.LT(-1), ex);
			}
			return functionBlock_return;
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x0001F4F0 File Offset: 0x0001D6F0
		public functionModifier_return functionModifier()
		{
			functionModifier_return functionModifier_return = new functionModifier_return();
			functionModifier_return.Start = input.LT(1);
			object obj = null;
			try
			{
				int num = input.LA(1);
				int num2;
				if (num == 46 && ((function_scope)function_stack.Peek()).ballowGlobal)
				{
					num2 = 1;
				}
				else if (num == 47)
				{
					num2 = 2;
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return functionModifier_return;
					}
					NoViableAltException ex = new NoViableAltException("", 17, 0, input);
					throw ex;
				}
				switch (num2)
				{
				case 1:
					obj = adaptor.GetNilNode();
					if (!((function_scope)function_stack.Peek()).ballowGlobal)
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return functionModifier_return;
						}
						throw new FailedPredicateException(input, "functionModifier", "$function::ballowGlobal");
					}
					else
					{
						IToken token = (IToken)Match(input, 46, FOLLOW_GLOBAL_in_functionModifier798);
						if (state.failed)
						{
							return functionModifier_return;
						}
						if (state.backtracking == 0)
						{
							object child = adaptor.Create(token);
							adaptor.AddChild(obj, child);
						}
						if (state.backtracking == 0)
						{
							if (((functionHeader_scope)functionHeader_stack.Peek()).bglobal)
							{
								OnError("global function flag may only be listed once", token.Line, token.CharPositionInLine);
							}
							else
							{
								((functionHeader_scope)functionHeader_stack.Peek()).bglobal = true;
							}
						}
					}
					break;
				case 2:
				{
					obj = adaptor.GetNilNode();
					IToken token2 = (IToken)Match(input, 47, FOLLOW_NATIVE_in_functionModifier809);
					if (state.failed)
					{
						return functionModifier_return;
					}
					if (state.backtracking == 0)
					{
						object child2 = adaptor.Create(token2);
						adaptor.AddChild(obj, child2);
					}
					if (state.backtracking == 0)
					{
						if (((functionHeader_scope)functionHeader_stack.Peek()).bnative)
						{
							OnError("native function flag may only be listed once", token2.Line, token2.CharPositionInLine);
						}
						else
						{
							((functionHeader_scope)functionHeader_stack.Peek()).bnative = true;
						}
					}
					break;
				}
				}
				functionModifier_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					functionModifier_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(functionModifier_return.Tree, (IToken)functionModifier_return.Start, (IToken)functionModifier_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				functionModifier_return.Tree = adaptor.ErrorNode(input, (IToken)functionModifier_return.Start, input.LT(-1), ex2);
			}
			return functionModifier_return;
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x0001F840 File Offset: 0x0001DA40
		public eventFunc_return eventFunc(string asStateName)
		{
			eventFunc_stack.Push(new eventFunc_scope());
			eventFunc_return eventFunc_return = new eventFunc_return();
			eventFunc_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule eventBlock");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule eventHeader");
			((eventFunc_scope)eventFunc_stack.Peek()).sstateName = asStateName;
			try
			{
				PushFollow(FOLLOW_eventHeader_in_eventFunc846);
				eventHeader_return eventHeader_return = eventHeader();
				state.followingStackPointer--;
				if (state.failed)
				{
					return eventFunc_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(eventHeader_return.Tree);
				}
				PushFollow(FOLLOW_eventBlock_in_eventFunc848);
				eventBlock_return eventBlock_return = eventBlock();
				state.followingStackPointer--;
				if (state.failed)
				{
					return eventFunc_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(eventBlock_return.Tree);
				}
				if (state.backtracking == 0)
				{
					eventFunc_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (eventFunc_return != null) ? eventFunc_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(7, (eventHeader_return != null) ? ((IToken)eventHeader_return.Start) : null, "event"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
					if (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					adaptor.AddChild(obj, obj2);
					eventFunc_return.Tree = obj;
					eventFunc_return.Tree = obj;
				}
				eventFunc_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					eventFunc_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(eventFunc_return.Tree, (IToken)eventFunc_return.Start, (IToken)eventFunc_return.Stop);
				}
				if (state.backtracking == 0)
				{
					eventFunc_return.sEventName = ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.Name;
					eventFunc_return.kEventFunction = ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction;
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				eventFunc_return.Tree = adaptor.ErrorNode(input, (IToken)eventFunc_return.Start, input.LT(-1), ex);
			}
			finally
			{
				eventFunc_stack.Pop();
			}
			return eventFunc_return;
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x0001FB64 File Offset: 0x0001DD64
		public eventHeader_return eventHeader()
		{
			eventHeader_stack.Push(new eventHeader_scope());
			eventHeader_return eventHeader_return = new eventHeader_return();
			eventHeader_return.Start = input.LT(1);
			object obj = null;
			IToken token = null;
			userFlags_return userFlags_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token NATIVE");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token DOCSTRING");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token EVENT");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule callParameters");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule userFlags");
			((eventHeader_scope)eventHeader_stack.Peek()).kparamNames = new List<string>();
			((eventHeader_scope)eventHeader_stack.Peek()).kparamTypes = new List<ScriptVariableType>();
			try
			{
				IToken token2 = (IToken)Match(input, 7, FOLLOW_EVENT_in_eventHeader884);
				if (state.failed)
				{
					return eventHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream4.Add(token2);
				}
				IToken token3 = (IToken)Match(input, 38, FOLLOW_ID_in_eventHeader886);
				if (state.failed)
				{
					return eventHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream5.Add(token3);
				}
				IToken el = (IToken)Match(input, 43, FOLLOW_LPAREN_in_eventHeader888);
				if (state.failed)
				{
					return eventHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream6.Add(el);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 38 || num2 == 55)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_callParameters_in_eventHeader890);
					callParameters_return callParameters_return = callParameters(((eventHeader_scope)eventHeader_stack.Peek()).kparamNames, ((eventHeader_scope)eventHeader_stack.Peek()).kparamTypes);
					state.followingStackPointer--;
					if (state.failed)
					{
						return eventHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(callParameters_return.Tree);
					}
				}
				IToken el2 = (IToken)Match(input, 44, FOLLOW_RPAREN_in_eventHeader894);
				if (state.failed)
				{
					return eventHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(el2);
				}
				int num4 = 2;
				int num5 = input.LA(1);
				if (num5 == 47)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					token = (IToken)Match(input, 47, FOLLOW_NATIVE_in_eventHeader896);
					if (state.failed)
					{
						return eventHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
				}
				int num7 = 2;
				int num8 = input.LA(1);
				if (num8 == 38)
				{
					num7 = 1;
				}
				int num9 = num7;
				if (num9 == 1)
				{
					PushFollow(FOLLOW_userFlags_in_eventHeader899);
					userFlags_return = userFlags();
					state.followingStackPointer--;
					if (state.failed)
					{
						return eventHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(userFlags_return.Tree);
					}
				}
				int num10 = dfa21.Predict(input);
				int num11 = num10;
				if (num11 == 1)
				{
					PushFollow(FOLLOW_terminator_in_eventHeader903);
					terminator_return terminator_return = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return eventHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
					}
					IToken el3 = (IToken)Match(input, 40, FOLLOW_DOCSTRING_in_eventHeader905);
					if (state.failed)
					{
						return eventHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(el3);
					}
				}
				if (state.backtracking == 0)
				{
					((eventFunc_scope)eventFunc_stack.Peek()).keventFunction = new ScriptFunctionType(token3.Text, kObjType.Name, ((eventFunc_scope)eventFunc_stack.Peek()).sstateName, "");
					((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.kRetType = new ScriptVariableType("none");
					if (token != null)
					{
						((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative = true;
					}
					((eventHeader_scope)eventHeader_stack.Peek()).suserFlags = ProcessFuncUserFlags((userFlags_return != null) ? userFlags_return.kFlagList : null, token2);
					GenerateReservedFunctionVars(((eventFunc_scope)eventFunc_stack.Peek()).keventFunction);
					for (int i = 0; i < ((eventHeader_scope)eventHeader_stack.Peek()).kparamNames.Count; i++)
					{
						((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.TryAddParam(((eventHeader_scope)eventHeader_stack.Peek()).kparamNames[i], ((eventHeader_scope)eventHeader_stack.Peek()).kparamTypes[i]);
						AddLocalVariable(((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.FunctionScope, ((eventHeader_scope)eventHeader_stack.Peek()).kparamNames[i], ((eventHeader_scope)eventHeader_stack.Peek()).kparamTypes[i], token3);
					}
				}
				if (state.backtracking == 0)
				{
					eventHeader_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (eventHeader_return != null) ? eventHeader_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(8, token3, "header"), obj2);
					adaptor.AddChild(obj2, adaptor.Create(92, "NONE"));
					adaptor.AddChild(obj2, rewriteRuleTokenStream5.NextNode());
					adaptor.AddChild(obj2, adaptor.Create(18, token2, ((eventHeader_scope)eventHeader_stack.Peek()).suserFlags));
					if (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					if (rewriteRuleTokenStream.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
					}
					rewriteRuleTokenStream.Reset();
					if (rewriteRuleTokenStream3.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream3.NextNode());
					}
					rewriteRuleTokenStream3.Reset();
					adaptor.AddChild(obj, obj2);
					eventHeader_return.Tree = obj;
					eventHeader_return.Tree = obj;
				}
				eventHeader_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					eventHeader_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(eventHeader_return.Tree, (IToken)eventHeader_return.Start, (IToken)eventHeader_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				eventHeader_return.Tree = adaptor.ErrorNode(input, (IToken)eventHeader_return.Start, input.LT(-1), ex);
			}
			finally
			{
				eventHeader_stack.Pop();
			}
			return eventHeader_return;
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x000203E0 File Offset: 0x0001E5E0
		public eventBlock_return eventBlock()
		{
			eventBlock_return eventBlock_return = new eventBlock_return();
			eventBlock_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ENDEVENT");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule statement");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			try
			{
				switch (dfa23.Predict(input))
				{
				case 1:
					if (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return eventBlock_return;
						}
						throw new FailedPredicateException(input, "eventBlock", "$eventFunc::keventFunction.bNative");
					}
					else
					{
						PushFollow(FOLLOW_terminator_in_eventBlock953);
						terminator_return terminator_return = terminator();
						state.followingStackPointer--;
						if (state.failed)
						{
							return eventBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
						}
						if (state.backtracking == 0)
						{
							eventBlock_return.Tree = obj;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", (eventBlock_return != null) ? eventBlock_return.Tree : null);
							obj = adaptor.GetNilNode();
							obj = null;
							eventBlock_return.Tree = obj;
							eventBlock_return.Tree = obj;
						}
					}
					break;
				case 2:
					if (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return eventBlock_return;
						}
						throw new FailedPredicateException(input, "eventBlock", "!$eventFunc::keventFunction.bNative");
					}
					else
					{
						PushFollow(FOLLOW_terminator_in_eventBlock968);
						terminator_return terminator_return2 = terminator();
						state.followingStackPointer--;
						if (state.failed)
						{
							return eventBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream2.Add(terminator_return2.Tree);
						}
						for (;;)
						{
							int num = 2;
							int num2 = input.LA(1);
							if (num2 == 38 || num2 == 43 || num2 == 55 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 84) || num2 == 88 || (num2 >= 90 && num2 <= 93))
							{
								num = 1;
							}
							int num3 = num;
							if (num3 != 1)
							{
								goto IL_305;
							}
							PushFollow(FOLLOW_statement_in_eventBlock970);
							statement_return statement_return = statement(((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.FunctionScope);
							state.followingStackPointer--;
							if (state.failed)
							{
								break;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream.Add(statement_return.Tree);
							}
						}
						return eventBlock_return;
						IL_305:
						IToken el = (IToken)Match(input, 48, FOLLOW_ENDEVENT_in_eventBlock974);
						if (state.failed)
						{
							return eventBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream.Add(el);
						}
						PushFollow(FOLLOW_terminator_in_eventBlock976);
						terminator_return terminator_return3 = terminator();
						state.followingStackPointer--;
						if (state.failed)
						{
							return eventBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream2.Add(terminator_return3.Tree);
						}
						if (state.backtracking == 0)
						{
							eventBlock_return.Tree = obj;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", (eventBlock_return != null) ? eventBlock_return.Tree : null);
							obj = adaptor.GetNilNode();
							object obj2 = adaptor.GetNilNode();
							obj2 = adaptor.BecomeRoot(adaptor.Create(10, "BLOCK"), obj2);
							while (rewriteRuleSubtreeStream.HasNext())
							{
								adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
							}
							rewriteRuleSubtreeStream.Reset();
							adaptor.AddChild(obj, obj2);
							eventBlock_return.Tree = obj;
							eventBlock_return.Tree = obj;
						}
					}
					break;
				}
				eventBlock_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					eventBlock_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(eventBlock_return.Tree, (IToken)eventBlock_return.Start, (IToken)eventBlock_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				eventBlock_return.Tree = adaptor.ErrorNode(input, (IToken)eventBlock_return.Start, input.LT(-1), ex);
			}
			return eventBlock_return;
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x00020900 File Offset: 0x0001EB00
		public callParameters_return callParameters(List<string> akParamNames, List<ScriptVariableType> akParamTypes)
		{
			callParameters_return callParameters_return = new callParameters_return();
			callParameters_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token COMMA");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule callParameter");
			try
			{
				PushFollow(FOLLOW_callParameter_in_callParameters1002);
				callParameter_return callParameter_return = callParameter(akParamNames, akParamTypes);
				state.followingStackPointer--;
				if (state.failed)
				{
					return callParameters_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(callParameter_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 49)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_164;
					}
					IToken el = (IToken)Match(input, 49, FOLLOW_COMMA_in_callParameters1006);
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
					PushFollow(FOLLOW_callParameter_in_callParameters1008);
					callParameter_return callParameter_return2 = callParameter(akParamNames, akParamTypes);
					state.followingStackPointer--;
					if (state.failed)
					{
						goto Block_9;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(callParameter_return2.Tree);
					}
				}
				return callParameters_return;
				Block_9:
				return callParameters_return;
				IL_164:
				if (state.backtracking == 0)
				{
					callParameters_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (callParameters_return != null) ? callParameters_return.Tree : null);
					obj = adaptor.GetNilNode();
					if (!rewriteRuleSubtreeStream.HasNext())
					{
						throw new RewriteEarlyExitException();
					}
					while (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					callParameters_return.Tree = obj;
					callParameters_return.Tree = obj;
				}
				callParameters_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					callParameters_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(callParameters_return.Tree, (IToken)callParameters_return.Start, (IToken)callParameters_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				callParameters_return.Tree = adaptor.ErrorNode(input, (IToken)callParameters_return.Start, input.LT(-1), ex);
			}
			return callParameters_return;
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x00020BB4 File Offset: 0x0001EDB4
		public callParameter_return callParameter(List<string> akParamNames, List<ScriptVariableType> akParamTypes)
		{
			callParameter_return callParameter_return = new callParameter_return();
			callParameter_return.Start = input.LT(1);
			object obj = null;
			constant_return constant_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token EQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule constant");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule type");
			try
			{
				PushFollow(FOLLOW_type_in_callParameter1032);
				type_return type_return = type();
				state.followingStackPointer--;
				if (state.failed)
				{
					return callParameter_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(type_return.Tree);
				}
				IToken token = (IToken)Match(input, 38, FOLLOW_ID_in_callParameter1036);
				if (state.failed)
				{
					return callParameter_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(token);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 41)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					IToken el = (IToken)Match(input, 41, FOLLOW_EQUALS_in_callParameter1039);
					if (state.failed)
					{
						return callParameter_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
					PushFollow(FOLLOW_constant_in_callParameter1041);
					constant_return = constant();
					state.followingStackPointer--;
					if (state.failed)
					{
						return callParameter_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(constant_return.Tree);
					}
				}
				if (state.backtracking == 0)
				{
					AddFunctionParameter(akParamNames, akParamTypes, token.Text, (type_return != null) ? type_return.kType : null, (constant_return != null) ? ((IToken)constant_return.Start) : null, token);
				}
				if (state.backtracking == 0)
				{
					callParameter_return.Tree = obj;
					RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token name", token);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (callParameter_return != null) ? callParameter_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(9, token, "parameter"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
					adaptor.AddChild(obj2, rewriteRuleTokenStream3.NextNode());
					if (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					adaptor.AddChild(obj, obj2);
					callParameter_return.Tree = obj;
					callParameter_return.Tree = obj;
				}
				callParameter_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					callParameter_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(callParameter_return.Tree, (IToken)callParameter_return.Start, (IToken)callParameter_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				callParameter_return.Tree = adaptor.ErrorNode(input, (IToken)callParameter_return.Start, input.LT(-1), ex);
			}
			return callParameter_return;
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x00020F70 File Offset: 0x0001F170
		public stateBlock_return stateBlock()
		{
			stateBlock_return stateBlock_return = new stateBlock_return();
			stateBlock_return.Start = input.LT(1);
			object obj = null;
			IToken token = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token STATE");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token ENDSTATE");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token AUTO");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule stateFuncOrEvent");
			try
			{
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 50)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					token = (IToken)Match(input, 50, FOLLOW_AUTO_in_stateBlock1079);
					if (state.failed)
					{
						return stateBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream4.Add(token);
					}
				}
				IToken el = (IToken)Match(input, 51, FOLLOW_STATE_in_stateBlock1082);
				if (state.failed)
				{
					return stateBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(el);
				}
				IToken token2 = (IToken)Match(input, 38, FOLLOW_ID_in_stateBlock1084);
				if (state.failed)
				{
					return stateBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream3.Add(token2);
				}
				PushFollow(FOLLOW_terminator_in_stateBlock1086);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return stateBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(terminator_return.Tree);
				}
				for (;;)
				{
					int num4 = 2;
					int num5 = input.LA(1);
					if ((num5 >= 6 && num5 <= 7) || num5 == 38 || num5 == 55)
					{
						num4 = 1;
					}
					int num6 = num4;
					if (num6 != 1)
					{
						goto IL_275;
					}
					PushFollow(FOLLOW_stateFuncOrEvent_in_stateBlock1089);
					stateFuncOrEvent_return stateFuncOrEvent_return = stateFuncOrEvent(token2.Text);
					state.followingStackPointer--;
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(stateFuncOrEvent_return.Tree);
					}
				}
				return stateBlock_return;
				IL_275:
				IToken el2 = (IToken)Match(input, 52, FOLLOW_ENDSTATE_in_stateBlock1094);
				if (state.failed)
				{
					return stateBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(el2);
				}
				PushFollow(FOLLOW_terminator_in_stateBlock1096);
				terminator_return terminator_return2 = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return stateBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(terminator_return2.Tree);
				}
				if (state.backtracking == 0 && token != null)
				{
					SetAutoState(token2.Text, token);
				}
				if (state.backtracking == 0)
				{
					stateBlock_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (stateBlock_return != null) ? stateBlock_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(rewriteRuleTokenStream.NextNode(), obj2);
					adaptor.AddChild(obj2, rewriteRuleTokenStream3.NextNode());
					if (rewriteRuleTokenStream4.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream4.NextNode());
					}
					rewriteRuleTokenStream4.Reset();
					while (rewriteRuleSubtreeStream2.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
					}
					rewriteRuleSubtreeStream2.Reset();
					adaptor.AddChild(obj, obj2);
					stateBlock_return.Tree = obj;
					stateBlock_return.Tree = obj;
				}
				stateBlock_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					stateBlock_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(stateBlock_return.Tree, (IToken)stateBlock_return.Start, (IToken)stateBlock_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				stateBlock_return.Tree = adaptor.ErrorNode(input, (IToken)stateBlock_return.Start, input.LT(-1), ex);
			}
			return stateBlock_return;
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x0002144C File Offset: 0x0001F64C
		public stateFuncOrEvent_return stateFuncOrEvent(string asState)
		{
			stateFuncOrEvent_return stateFuncOrEvent_return = new stateFuncOrEvent_return();
			stateFuncOrEvent_return.Start = input.LT(1);
			object obj = null;
			try
			{
				int num = input.LA(1);
				int num2;
				if (num == 6 || num == 38 || num == 55)
				{
					num2 = 1;
				}
				else if (num == 7)
				{
					num2 = 2;
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return stateFuncOrEvent_return;
					}
					NoViableAltException ex = new NoViableAltException("", 28, 0, input);
					throw ex;
				}
				switch (num2)
				{
				case 1:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_function_in_stateFuncOrEvent1131);
					function_return function_return = function(false, asState, "");
					state.followingStackPointer--;
					if (state.failed)
					{
						return stateFuncOrEvent_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, function_return.Tree);
					}
					if (state.backtracking == 0)
					{
						AddObjectFunction(asState, (function_return != null) ? function_return.sFuncName : null, (function_return != null) ? function_return.kFunction : null, (function_return != null) ? ((IToken)function_return.Start) : null, false);
					}
					break;
				}
				case 2:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_eventFunc_in_stateFuncOrEvent1144);
					eventFunc_return eventFunc_return = eventFunc(asState);
					state.followingStackPointer--;
					if (state.failed)
					{
						return stateFuncOrEvent_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, eventFunc_return.Tree);
					}
					if (state.backtracking == 0)
					{
						AddObjectFunction(asState, (eventFunc_return != null) ? eventFunc_return.sEventName : null, (eventFunc_return != null) ? eventFunc_return.kEventFunction : null, (eventFunc_return != null) ? ((IToken)eventFunc_return.Start) : null, true);
					}
					break;
				}
				}
				stateFuncOrEvent_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					stateFuncOrEvent_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(stateFuncOrEvent_return.Tree, (IToken)stateFuncOrEvent_return.Start, (IToken)stateFuncOrEvent_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				stateFuncOrEvent_return.Tree = adaptor.ErrorNode(input, (IToken)stateFuncOrEvent_return.Start, input.LT(-1), ex2);
			}
			return stateFuncOrEvent_return;
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x00021718 File Offset: 0x0001F918
		public propertyBlock_return propertyBlock()
		{
			propertyBlock_stack.Push(new propertyBlock_scope());
			propertyBlock_return propertyBlock_return = new propertyBlock_return();
			propertyBlock_return.Start = input.LT(1);
			object obj = null;
			propertyFunc_return propertyFunc_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ENDPROPERTY");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule readOnlyAutoPropertyHeader");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule autoPropertyHeader");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule propertyFunc");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule propertyHeader");
			((propertyBlock_scope)propertyBlock_stack.Peek()).sname = "";
			try
			{
				switch (dfa30.Predict(input))
				{
				case 1:
				{
					PushFollow(FOLLOW_propertyHeader_in_propertyBlock1172);
					propertyHeader_return propertyHeader_return = propertyHeader();
					state.followingStackPointer--;
					if (state.failed)
					{
						return propertyBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream5.Add(propertyHeader_return.Tree);
					}
					PushFollow(FOLLOW_propertyFunc_in_propertyBlock1176);
					propertyFunc_return propertyFunc_return2 = propertyFunc(((propertyBlock_scope)propertyBlock_stack.Peek()).sname);
					state.followingStackPointer--;
					if (state.failed)
					{
						return propertyBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream4.Add(propertyFunc_return2.Tree);
					}
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 6 || num2 == 38 || num2 == 55)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 == 1)
					{
						PushFollow(FOLLOW_propertyFunc_in_propertyBlock1181);
						propertyFunc_return = propertyFunc(((propertyBlock_scope)propertyBlock_stack.Peek()).sname);
						state.followingStackPointer--;
						if (state.failed)
						{
							return propertyBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream4.Add(propertyFunc_return.Tree);
						}
					}
					IToken el = (IToken)Match(input, 53, FOLLOW_ENDPROPERTY_in_propertyBlock1185);
					if (state.failed)
					{
						return propertyBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
					PushFollow(FOLLOW_terminator_in_propertyBlock1187);
					terminator_return terminator_return = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return propertyBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(terminator_return.Tree);
					}
					if (state.backtracking == 0)
					{
						propertyBlock_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (propertyBlock_return != null) ? propertyBlock_return.Tree : null);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(adaptor, "rule func0", (propertyFunc_return2 != null) ? propertyFunc_return2.Tree : null);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream7 = new RewriteRuleSubtreeStream(adaptor, "rule func1", (propertyFunc_return != null) ? propertyFunc_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(54, (propertyHeader_return != null) ? ((IToken)propertyHeader_return.Start) : null, "property"), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream5.NextTree());
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream6.NextTree());
						if (rewriteRuleSubtreeStream7.HasNext())
						{
							adaptor.AddChild(obj2, rewriteRuleSubtreeStream7.NextTree());
						}
						rewriteRuleSubtreeStream7.Reset();
						adaptor.AddChild(obj, obj2);
						propertyBlock_return.Tree = obj;
						propertyBlock_return.Tree = obj;
					}
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_autoPropertyHeader_in_propertyBlock1216);
					autoPropertyHeader_return autoPropertyHeader_return = autoPropertyHeader();
					state.followingStackPointer--;
					if (state.failed)
					{
						return propertyBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(autoPropertyHeader_return.Tree);
					}
					if (state.backtracking == 0)
					{
						propertyBlock_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (propertyBlock_return != null) ? propertyBlock_return.Tree : null);
						obj = adaptor.GetNilNode();
						adaptor.AddChild(obj, CreateAutoPropertyTree((ITree)((autoPropertyHeader_return != null) ? autoPropertyHeader_return.Tree : null), ((propertyBlock_scope)propertyBlock_stack.Peek()).sname, ((propertyBlock_scope)propertyBlock_stack.Peek()).ktype, (autoPropertyHeader_return != null) ? autoPropertyHeader_return.sVarUserFlags : null, (autoPropertyHeader_return != null) ? autoPropertyHeader_return.kInitialValue : null));
						propertyBlock_return.Tree = obj;
						propertyBlock_return.Tree = obj;
					}
					break;
				}
				case 3:
				{
					PushFollow(FOLLOW_readOnlyAutoPropertyHeader_in_propertyBlock1229);
					readOnlyAutoPropertyHeader_return readOnlyAutoPropertyHeader_return = readOnlyAutoPropertyHeader();
					state.followingStackPointer--;
					if (state.failed)
					{
						return propertyBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(readOnlyAutoPropertyHeader_return.Tree);
					}
					if (state.backtracking == 0)
					{
						propertyBlock_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (propertyBlock_return != null) ? propertyBlock_return.Tree : null);
						obj = adaptor.GetNilNode();
						adaptor.AddChild(obj, CreateAutoReadOnlyPropertyTree((ITree)((readOnlyAutoPropertyHeader_return != null) ? readOnlyAutoPropertyHeader_return.Tree : null), ((propertyBlock_scope)propertyBlock_stack.Peek()).sname, ((propertyBlock_scope)propertyBlock_stack.Peek()).ktype, (readOnlyAutoPropertyHeader_return != null) ? readOnlyAutoPropertyHeader_return.kInitialValue : null));
						propertyBlock_return.Tree = obj;
						propertyBlock_return.Tree = obj;
					}
					break;
				}
				}
				propertyBlock_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					propertyBlock_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(propertyBlock_return.Tree, (IToken)propertyBlock_return.Start, (IToken)propertyBlock_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				propertyBlock_return.Tree = adaptor.ErrorNode(input, (IToken)propertyBlock_return.Start, input.LT(-1), ex);
			}
			finally
			{
				propertyBlock_stack.Pop();
			}
			return propertyBlock_return;
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00021E3C File Offset: 0x0002003C
		public propertyHeader_return propertyHeader()
		{
			propertyHeader_stack.Push(new propertyHeader_scope());
			propertyHeader_return propertyHeader_return = new propertyHeader_return();
			propertyHeader_return.Start = input.LT(1);
			object obj = null;
			userFlags_return userFlags_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token DOCSTRING");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token PROPERTY");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule type");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule userFlags");
			try
			{
				PushFollow(FOLLOW_type_in_propertyHeader1255);
				type_return type_return = type();
				state.followingStackPointer--;
				if (state.failed)
				{
					return propertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(type_return.Tree);
				}
				IToken token = (IToken)Match(input, 54, FOLLOW_PROPERTY_in_propertyHeader1257);
				if (state.failed)
				{
					return propertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(token);
				}
				IToken token2 = (IToken)Match(input, 38, FOLLOW_ID_in_propertyHeader1261);
				if (state.failed)
				{
					return propertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream3.Add(token2);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 38)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_userFlags_in_propertyHeader1263);
					userFlags_return = userFlags();
					state.followingStackPointer--;
					if (state.failed)
					{
						return propertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(userFlags_return.Tree);
					}
				}
				PushFollow(FOLLOW_terminator_in_propertyHeader1266);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return propertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(terminator_return.Tree);
				}
				int num4 = 2;
				int num5 = input.LA(1);
				if (num5 == 40)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					IToken el = (IToken)Match(input, 40, FOLLOW_DOCSTRING_in_propertyHeader1269);
					if (state.failed)
					{
						return propertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
					PushFollow(FOLLOW_terminator_in_propertyHeader1271);
					terminator_return terminator_return2 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return propertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(terminator_return2.Tree);
					}
				}
				if (state.backtracking == 0)
				{
					((propertyBlock_scope)propertyBlock_stack.Peek()).sname = token2.Text;
					((propertyBlock_scope)propertyBlock_stack.Peek()).ktype = ((type_return != null) ? type_return.kType : null);
					((propertyHeader_scope)propertyHeader_stack.Peek()).suserFlags = ProcessPropUserFlags((userFlags_return != null) ? userFlags_return.kFlagList : null, token);
					AddObjectProperty((type_return != null) ? type_return.kType : null, token2.Text, token2, false);
				}
				if (state.backtracking == 0)
				{
					propertyHeader_return.Tree = obj;
					RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token name", token2);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (propertyHeader_return != null) ? propertyHeader_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(8, token2, "header"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
					adaptor.AddChild(obj2, rewriteRuleTokenStream4.NextNode());
					adaptor.AddChild(obj2, adaptor.Create(18, token, ((propertyHeader_scope)propertyHeader_stack.Peek()).suserFlags));
					if (rewriteRuleTokenStream.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream.NextNode());
					}
					rewriteRuleTokenStream.Reset();
					adaptor.AddChild(obj, obj2);
					propertyHeader_return.Tree = obj;
					propertyHeader_return.Tree = obj;
				}
				propertyHeader_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					propertyHeader_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(propertyHeader_return.Tree, (IToken)propertyHeader_return.Start, (IToken)propertyHeader_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				propertyHeader_return.Tree = adaptor.ErrorNode(input, (IToken)propertyHeader_return.Start, input.LT(-1), ex);
			}
			finally
			{
				propertyHeader_stack.Pop();
			}
			return propertyHeader_return;
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x000223F8 File Offset: 0x000205F8
		public autoPropertyHeader_return autoPropertyHeader()
		{
			autoPropertyHeader_stack.Push(new autoPropertyHeader_scope());
			autoPropertyHeader_return autoPropertyHeader_return = new autoPropertyHeader_return();
			autoPropertyHeader_return.Start = input.LT(1);
			object obj = null;
			constant_return constant_return = null;
			userFlags_return userFlags_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token EQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token DOCSTRING");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token PROPERTY");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token AUTO");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule constant");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule type");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule userFlags");
			try
			{
				PushFollow(FOLLOW_type_in_autoPropertyHeader1322);
				type_return type_return = type();
				state.followingStackPointer--;
				if (state.failed)
				{
					return autoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream3.Add(type_return.Tree);
				}
				IToken token = (IToken)Match(input, 54, FOLLOW_PROPERTY_in_autoPropertyHeader1324);
				if (state.failed)
				{
					return autoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream3.Add(token);
				}
				IToken token2 = (IToken)Match(input, 38, FOLLOW_ID_in_autoPropertyHeader1328);
				if (state.failed)
				{
					return autoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream4.Add(token2);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 41)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					IToken el = (IToken)Match(input, 41, FOLLOW_EQUALS_in_autoPropertyHeader1331);
					if (state.failed)
					{
						return autoPropertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
					PushFollow(FOLLOW_constant_in_autoPropertyHeader1333);
					constant_return = constant();
					state.followingStackPointer--;
					if (state.failed)
					{
						return autoPropertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(constant_return.Tree);
					}
				}
				IToken el2 = (IToken)Match(input, 50, FOLLOW_AUTO_in_autoPropertyHeader1337);
				if (state.failed)
				{
					return autoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream5.Add(el2);
				}
				int num4 = 2;
				int num5 = input.LA(1);
				if (num5 == 38)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					PushFollow(FOLLOW_userFlags_in_autoPropertyHeader1339);
					userFlags_return = userFlags();
					state.followingStackPointer--;
					if (state.failed)
					{
						return autoPropertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream4.Add(userFlags_return.Tree);
					}
				}
				PushFollow(FOLLOW_terminator_in_autoPropertyHeader1342);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return autoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
				}
				int num7 = 2;
				int num8 = input.LA(1);
				if (num8 == 40)
				{
					num7 = 1;
				}
				int num9 = num7;
				if (num9 == 1)
				{
					IToken el3 = (IToken)Match(input, 40, FOLLOW_DOCSTRING_in_autoPropertyHeader1345);
					if (state.failed)
					{
						return autoPropertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el3);
					}
					PushFollow(FOLLOW_terminator_in_autoPropertyHeader1347);
					terminator_return terminator_return2 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return autoPropertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return2.Tree);
					}
				}
				if (state.backtracking == 0)
				{
					((propertyBlock_scope)propertyBlock_stack.Peek()).sname = token2.Text;
					((propertyBlock_scope)propertyBlock_stack.Peek()).ktype = ((type_return != null) ? type_return.kType : null);
					autoPropertyHeader_return.kInitialValue = (ITree)((constant_return != null) ? constant_return.Tree : null);
					ProcessAutoPropUserFlags((userFlags_return != null) ? userFlags_return.kFlagList : null, token, out ((autoPropertyHeader_scope)autoPropertyHeader_stack.Peek()).suserFlags, out autoPropertyHeader_return.sVarUserFlags);
					AddObjectProperty((type_return != null) ? type_return.kType : null, token2.Text, token2, true);
				}
				if (state.backtracking == 0)
				{
					autoPropertyHeader_return.Tree = obj;
					RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token name", token2);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (autoPropertyHeader_return != null) ? autoPropertyHeader_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(8, token2, "header"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream3.NextTree());
					adaptor.AddChild(obj2, rewriteRuleTokenStream6.NextNode());
					adaptor.AddChild(obj2, adaptor.Create(18, token, ((autoPropertyHeader_scope)autoPropertyHeader_stack.Peek()).suserFlags));
					if (rewriteRuleTokenStream2.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
					}
					rewriteRuleTokenStream2.Reset();
					adaptor.AddChild(obj, obj2);
					autoPropertyHeader_return.Tree = obj;
					autoPropertyHeader_return.Tree = obj;
				}
				autoPropertyHeader_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					autoPropertyHeader_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(autoPropertyHeader_return.Tree, (IToken)autoPropertyHeader_return.Start, (IToken)autoPropertyHeader_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				autoPropertyHeader_return.Tree = adaptor.ErrorNode(input, (IToken)autoPropertyHeader_return.Start, input.LT(-1), ex);
			}
			finally
			{
				autoPropertyHeader_stack.Pop();
			}
			return autoPropertyHeader_return;
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00022B18 File Offset: 0x00020D18
		public readOnlyAutoPropertyHeader_return readOnlyAutoPropertyHeader()
		{
			readOnlyAutoPropertyHeader_stack.Push(new readOnlyAutoPropertyHeader_scope());
			readOnlyAutoPropertyHeader_return readOnlyAutoPropertyHeader_return = new readOnlyAutoPropertyHeader_return();
			readOnlyAutoPropertyHeader_return.Start = input.LT(1);
			object obj = null;
			userFlags_return userFlags_return = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token EQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token DOCSTRING");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token BASETYPE");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token PROPERTY");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token AUTOREADONLY");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule constant");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule userFlags");
			try
			{
				IToken token = (IToken)Match(input, 55, FOLLOW_BASETYPE_in_readOnlyAutoPropertyHeader1396);
				if (state.failed)
				{
					return readOnlyAutoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream3.Add(token);
				}
				IToken token2 = (IToken)Match(input, 54, FOLLOW_PROPERTY_in_readOnlyAutoPropertyHeader1398);
				if (state.failed)
				{
					return readOnlyAutoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream4.Add(token2);
				}
				IToken token3 = (IToken)Match(input, 38, FOLLOW_ID_in_readOnlyAutoPropertyHeader1400);
				if (state.failed)
				{
					return readOnlyAutoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream5.Add(token3);
				}
				IToken el = (IToken)Match(input, 41, FOLLOW_EQUALS_in_readOnlyAutoPropertyHeader1402);
				if (state.failed)
				{
					return readOnlyAutoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(el);
				}
				PushFollow(FOLLOW_constant_in_readOnlyAutoPropertyHeader1404);
				constant_return constant_return = constant();
				state.followingStackPointer--;
				if (state.failed)
				{
					return readOnlyAutoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(constant_return.Tree);
				}
				IToken el2 = (IToken)Match(input, 56, FOLLOW_AUTOREADONLY_in_readOnlyAutoPropertyHeader1406);
				if (state.failed)
				{
					return readOnlyAutoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream6.Add(el2);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 38)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_userFlags_in_readOnlyAutoPropertyHeader1408);
					userFlags_return = userFlags();
					state.followingStackPointer--;
					if (state.failed)
					{
						return readOnlyAutoPropertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(userFlags_return.Tree);
					}
				}
				PushFollow(FOLLOW_terminator_in_readOnlyAutoPropertyHeader1411);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return readOnlyAutoPropertyHeader_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
				}
				int num4 = 2;
				int num5 = input.LA(1);
				if (num5 == 40)
				{
					num4 = 1;
				}
				int num6 = num4;
				if (num6 == 1)
				{
					IToken el3 = (IToken)Match(input, 40, FOLLOW_DOCSTRING_in_readOnlyAutoPropertyHeader1414);
					if (state.failed)
					{
						return readOnlyAutoPropertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el3);
					}
					PushFollow(FOLLOW_terminator_in_readOnlyAutoPropertyHeader1416);
					terminator_return terminator_return2 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return readOnlyAutoPropertyHeader_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return2.Tree);
					}
				}
				if (state.backtracking == 0)
				{
					((propertyBlock_scope)propertyBlock_stack.Peek()).sname = token3.Text;
					((propertyBlock_scope)propertyBlock_stack.Peek()).ktype = ConstructVarType(token.Text, null);
					readOnlyAutoPropertyHeader_return.kInitialValue = (ITree)((constant_return != null) ? constant_return.Tree : null);
					((readOnlyAutoPropertyHeader_scope)readOnlyAutoPropertyHeader_stack.Peek()).suserFlags = ProcessPropUserFlags((userFlags_return != null) ? userFlags_return.kFlagList : null, token2);
					AddObjectProperty(((propertyBlock_scope)propertyBlock_stack.Peek()).ktype, token3.Text, token, false);
				}
				if (state.backtracking == 0)
				{
					readOnlyAutoPropertyHeader_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (readOnlyAutoPropertyHeader_return != null) ? readOnlyAutoPropertyHeader_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(8, token3, "header"), obj2);
					adaptor.AddChild(obj2, rewriteRuleTokenStream3.NextNode());
					adaptor.AddChild(obj2, rewriteRuleTokenStream5.NextNode());
					adaptor.AddChild(obj2, adaptor.Create(18, token2, ((readOnlyAutoPropertyHeader_scope)readOnlyAutoPropertyHeader_stack.Peek()).suserFlags));
					if (rewriteRuleTokenStream2.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
					}
					rewriteRuleTokenStream2.Reset();
					adaptor.AddChild(obj, obj2);
					readOnlyAutoPropertyHeader_return.Tree = obj;
					readOnlyAutoPropertyHeader_return.Tree = obj;
				}
				readOnlyAutoPropertyHeader_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					readOnlyAutoPropertyHeader_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(readOnlyAutoPropertyHeader_return.Tree, (IToken)readOnlyAutoPropertyHeader_return.Start, (IToken)readOnlyAutoPropertyHeader_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				readOnlyAutoPropertyHeader_return.Tree = adaptor.ErrorNode(input, (IToken)readOnlyAutoPropertyHeader_return.Start, input.LT(-1), ex);
			}
			finally
			{
				readOnlyAutoPropertyHeader_stack.Pop();
			}
			return readOnlyAutoPropertyHeader_return;
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x000231F0 File Offset: 0x000213F0
		public propertyFunc_return propertyFunc(string asProperty)
		{
			propertyFunc_return propertyFunc_return = new propertyFunc_return();
			propertyFunc_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule function");
			try
			{
				PushFollow(FOLLOW_function_in_propertyFunc1461);
				function_return function_return = function(false, "", asProperty);
				state.followingStackPointer--;
				if (state.failed)
				{
					return propertyFunc_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(function_return.Tree);
				}
				if (state.backtracking == 0)
				{
					TrySetPropertyFunction(asProperty, (function_return != null) ? function_return.kFunction : null, (function_return != null) ? ((IToken)function_return.Start) : null);
				}
				if (state.backtracking == 0)
				{
					propertyFunc_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (propertyFunc_return != null) ? propertyFunc_return.Tree : null);
					RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule func", (function_return != null) ? function_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(17, (function_return != null) ? ((IToken)function_return.Start) : null, "propfunc"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
					adaptor.AddChild(obj, obj2);
					propertyFunc_return.Tree = obj;
					propertyFunc_return.Tree = obj;
				}
				propertyFunc_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					propertyFunc_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(propertyFunc_return.Tree, (IToken)propertyFunc_return.Start, (IToken)propertyFunc_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				propertyFunc_return.Tree = adaptor.ErrorNode(input, (IToken)propertyFunc_return.Start, input.LT(-1), ex);
			}
			return propertyFunc_return;
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x00023448 File Offset: 0x00021648
		public statement_return statement(ScriptScope akCurrentScope)
		{
			statement_return statement_return = new statement_return();
			statement_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token MINUSEQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token DIVEQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token MULTEQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token MODEQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token PLUSEQUALS");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule l_value");
			try
			{
				switch (dfa38.Predict(input))
				{
				case 1:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_localDefinition_in_statement1504);
					localDefinition_return localDefinition_return = localDefinition();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, localDefinition_return.Tree);
					}
					if (state.backtracking == 0)
					{
						AddLocalVariable(akCurrentScope, (localDefinition_return != null) ? localDefinition_return.sVarName : null, (localDefinition_return != null) ? localDefinition_return.kVarType : null, (localDefinition_return != null) ? ((IToken)localDefinition_return.Start) : null);
					}
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_l_value_in_statement1522);
					l_value_return l_value_return = l_value();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(l_value_return.Tree);
					}
					IToken token = (IToken)Match(input, 57, FOLLOW_PLUSEQUALS_in_statement1524);
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream5.Add(token);
					}
					PushFollow(FOLLOW_expression_in_statement1526);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return.Tree);
					}
					PushFollow(FOLLOW_terminator_in_statement1528);
					terminator_return terminator_return = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
					}
					if (state.backtracking == 0)
					{
						statement_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (statement_return != null) ? statement_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(41, token), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream3.NextTree());
						object obj3 = adaptor.GetNilNode();
						obj3 = adaptor.BecomeRoot(adaptor.Create(73, token), obj3);
						object obj4 = adaptor.GetNilNode();
						obj4 = adaptor.BecomeRoot(adaptor.Create(15, token, "()"), obj4);
						adaptor.AddChild(obj4, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(obj3, obj4);
						object obj5 = adaptor.GetNilNode();
						obj5 = adaptor.BecomeRoot(adaptor.Create(15, token, "()"), obj5);
						adaptor.AddChild(obj5, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj3, obj5);
						adaptor.AddChild(obj2, obj3);
						adaptor.AddChild(obj, obj2);
						statement_return.Tree = obj;
						statement_return.Tree = obj;
					}
					break;
				}
				case 3:
				{
					PushFollow(FOLLOW_l_value_in_statement1573);
					l_value_return l_value_return2 = l_value();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(l_value_return2.Tree);
					}
					IToken token2 = (IToken)Match(input, 58, FOLLOW_MINUSEQUALS_in_statement1575);
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token2);
					}
					PushFollow(FOLLOW_expression_in_statement1577);
					expression_return expression_return2 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return2.Tree);
					}
					PushFollow(FOLLOW_terminator_in_statement1579);
					terminator_return terminator_return2 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return2.Tree);
					}
					if (state.backtracking == 0)
					{
						statement_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (statement_return != null) ? statement_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj6 = adaptor.GetNilNode();
						obj6 = adaptor.BecomeRoot(adaptor.Create(41, token2), obj6);
						adaptor.AddChild(obj6, rewriteRuleSubtreeStream3.NextTree());
						object obj7 = adaptor.GetNilNode();
						obj7 = adaptor.BecomeRoot(adaptor.Create(74, token2), obj7);
						object obj8 = adaptor.GetNilNode();
						obj8 = adaptor.BecomeRoot(adaptor.Create(15, token2, "()"), obj8);
						adaptor.AddChild(obj8, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(obj7, obj8);
						object obj9 = adaptor.GetNilNode();
						obj9 = adaptor.BecomeRoot(adaptor.Create(15, token2, "()"), obj9);
						adaptor.AddChild(obj9, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj7, obj9);
						adaptor.AddChild(obj6, obj7);
						adaptor.AddChild(obj, obj6);
						statement_return.Tree = obj;
						statement_return.Tree = obj;
					}
					break;
				}
				case 4:
				{
					PushFollow(FOLLOW_l_value_in_statement1623);
					l_value_return l_value_return3 = l_value();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(l_value_return3.Tree);
					}
					IToken token3 = (IToken)Match(input, 59, FOLLOW_MULTEQUALS_in_statement1625);
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(token3);
					}
					PushFollow(FOLLOW_expression_in_statement1627);
					expression_return expression_return3 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return3.Tree);
					}
					PushFollow(FOLLOW_terminator_in_statement1629);
					terminator_return terminator_return3 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return3.Tree);
					}
					if (state.backtracking == 0)
					{
						statement_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (statement_return != null) ? statement_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj10 = adaptor.GetNilNode();
						obj10 = adaptor.BecomeRoot(adaptor.Create(41, token3), obj10);
						adaptor.AddChild(obj10, rewriteRuleSubtreeStream3.NextTree());
						object obj11 = adaptor.GetNilNode();
						obj11 = adaptor.BecomeRoot(adaptor.Create(75, token3), obj11);
						object obj12 = adaptor.GetNilNode();
						obj12 = adaptor.BecomeRoot(adaptor.Create(15, token3, "()"), obj12);
						adaptor.AddChild(obj12, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(obj11, obj12);
						object obj13 = adaptor.GetNilNode();
						obj13 = adaptor.BecomeRoot(adaptor.Create(15, token3, "()"), obj13);
						adaptor.AddChild(obj13, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj11, obj13);
						adaptor.AddChild(obj10, obj11);
						adaptor.AddChild(obj, obj10);
						statement_return.Tree = obj;
						statement_return.Tree = obj;
					}
					break;
				}
				case 5:
				{
					PushFollow(FOLLOW_l_value_in_statement1673);
					l_value_return l_value_return4 = l_value();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(l_value_return4.Tree);
					}
					IToken token4 = (IToken)Match(input, 60, FOLLOW_DIVEQUALS_in_statement1675);
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token4);
					}
					PushFollow(FOLLOW_expression_in_statement1677);
					expression_return expression_return4 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return4.Tree);
					}
					PushFollow(FOLLOW_terminator_in_statement1679);
					terminator_return terminator_return4 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return4.Tree);
					}
					if (state.backtracking == 0)
					{
						statement_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (statement_return != null) ? statement_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj14 = adaptor.GetNilNode();
						obj14 = adaptor.BecomeRoot(adaptor.Create(41, token4), obj14);
						adaptor.AddChild(obj14, rewriteRuleSubtreeStream3.NextTree());
						object obj15 = adaptor.GetNilNode();
						obj15 = adaptor.BecomeRoot(adaptor.Create(76, token4), obj15);
						object obj16 = adaptor.GetNilNode();
						obj16 = adaptor.BecomeRoot(adaptor.Create(15, token4, "()"), obj16);
						adaptor.AddChild(obj16, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(obj15, obj16);
						object obj17 = adaptor.GetNilNode();
						obj17 = adaptor.BecomeRoot(adaptor.Create(15, token4, "()"), obj17);
						adaptor.AddChild(obj17, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj15, obj17);
						adaptor.AddChild(obj14, obj15);
						adaptor.AddChild(obj, obj14);
						statement_return.Tree = obj;
						statement_return.Tree = obj;
					}
					break;
				}
				case 6:
				{
					PushFollow(FOLLOW_l_value_in_statement1723);
					l_value_return l_value_return5 = l_value();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream3.Add(l_value_return5.Tree);
					}
					IToken token5 = (IToken)Match(input, 61, FOLLOW_MODEQUALS_in_statement1725);
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream4.Add(token5);
					}
					PushFollow(FOLLOW_expression_in_statement1727);
					expression_return expression_return5 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return5.Tree);
					}
					PushFollow(FOLLOW_terminator_in_statement1729);
					terminator_return terminator_return5 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(terminator_return5.Tree);
					}
					if (state.backtracking == 0)
					{
						statement_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (statement_return != null) ? statement_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj18 = adaptor.GetNilNode();
						obj18 = adaptor.BecomeRoot(adaptor.Create(41, token5), obj18);
						adaptor.AddChild(obj18, rewriteRuleSubtreeStream3.NextTree());
						object obj19 = adaptor.GetNilNode();
						obj19 = adaptor.BecomeRoot(adaptor.Create(77, token5), obj19);
						object obj20 = adaptor.GetNilNode();
						obj20 = adaptor.BecomeRoot(adaptor.Create(15, token5, "()"), obj20);
						adaptor.AddChild(obj20, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(obj19, obj20);
						object obj21 = adaptor.GetNilNode();
						obj21 = adaptor.BecomeRoot(adaptor.Create(15, token5, "()"), obj21);
						adaptor.AddChild(obj21, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj19, obj21);
						adaptor.AddChild(obj18, obj19);
						adaptor.AddChild(obj, obj18);
						statement_return.Tree = obj;
						statement_return.Tree = obj;
					}
					break;
				}
				case 7:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_l_value_in_statement1773);
					l_value_return l_value_return6 = l_value();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, l_value_return6.Tree);
					}
					IToken payload = (IToken)Match(input, 41, FOLLOW_EQUALS_in_statement1775);
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						object newRoot = adaptor.Create(payload);
						obj = adaptor.BecomeRoot(newRoot, obj);
					}
					PushFollow(FOLLOW_expression_in_statement1778);
					expression_return expression_return6 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, expression_return6.Tree);
					}
					PushFollow(FOLLOW_terminator_in_statement1780);
					terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					break;
				}
				case 8:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_expression_in_statement1788);
					expression_return expression_return7 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, expression_return7.Tree);
					}
					PushFollow(FOLLOW_terminator_in_statement1790);
					terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					break;
				}
				case 9:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_return_stat_in_statement1798);
					return_stat_return return_stat_return = return_stat();
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, return_stat_return.Tree);
					}
					break;
				}
				case 10:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_ifBlock_in_statement1804);
					ifBlock_return ifBlock_return = ifBlock(akCurrentScope);
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, ifBlock_return.Tree);
					}
					break;
				}
				case 11:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_whileBlock_in_statement1811);
					whileBlock_return whileBlock_return = whileBlock(akCurrentScope);
					state.followingStackPointer--;
					if (state.failed)
					{
						return statement_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, whileBlock_return.Tree);
					}
					break;
				}
				}
				statement_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					statement_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(statement_return.Tree, (IToken)statement_return.Start, (IToken)statement_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				statement_return.Tree = adaptor.ErrorNode(input, (IToken)statement_return.Start, input.LT(-1), ex);
			}
			return statement_return;
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x00024834 File Offset: 0x00022A34
		public l_value_return l_value()
		{
			l_value_return l_value_return = new l_value_return();
			l_value_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token LBRACKET");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token RBRACKET");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token DOT");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			try
			{
				int num = input.LA(1);
				int num2;
				if (num == 43)
				{
					input.LA(2);
					if (synpred8_Papyrus())
					{
						num2 = 1;
					}
					else if (synpred9_Papyrus())
					{
						num2 = 2;
					}
					else
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return l_value_return;
						}
						NoViableAltException ex = new NoViableAltException("", 39, 1, input);
						throw ex;
					}
				}
				else if (num == 38 || num == 82)
				{
					num2 = 3;
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return l_value_return;
					}
					NoViableAltException ex2 = new NoViableAltException("", 39, 0, input);
					throw ex2;
				}
				switch (num2)
				{
				case 1:
				{
					IToken token = (IToken)Match(input, 43, FOLLOW_LPAREN_in_l_value1833);
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream6.Add(token);
					}
					PushFollow(FOLLOW_expression_in_l_value1835);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return.Tree);
					}
					IToken el = (IToken)Match(input, 44, FOLLOW_RPAREN_in_l_value1837);
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el);
					}
					IToken el2 = (IToken)Match(input, 62, FOLLOW_DOT_in_l_value1839);
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream4.Add(el2);
					}
					IToken el3 = (IToken)Match(input, 38, FOLLOW_ID_in_l_value1841);
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream5.Add(el3);
					}
					if (state.backtracking == 0)
					{
						l_value_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (l_value_return != null) ? l_value_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(rewriteRuleTokenStream4.NextNode(), obj2);
						object obj3 = adaptor.GetNilNode();
						obj3 = adaptor.BecomeRoot(adaptor.Create(15, token, "()"), obj3);
						adaptor.AddChild(obj3, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj2, obj3);
						adaptor.AddChild(obj2, rewriteRuleTokenStream5.NextNode());
						adaptor.AddChild(obj, obj2);
						l_value_return.Tree = obj;
						l_value_return.Tree = obj;
					}
					break;
				}
				case 2:
				{
					IToken token2 = (IToken)Match(input, 43, FOLLOW_LPAREN_in_l_value1877);
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream6.Add(token2);
					}
					PushFollow(FOLLOW_expression_in_l_value1881);
					expression_return expression_return2 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return2.Tree);
					}
					IToken el4 = (IToken)Match(input, 44, FOLLOW_RPAREN_in_l_value1883);
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el4);
					}
					IToken token3 = (IToken)Match(input, 63, FOLLOW_LBRACKET_in_l_value1885);
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token3);
					}
					PushFollow(FOLLOW_expression_in_l_value1889);
					expression_return expression_return3 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return3.Tree);
					}
					IToken el5 = (IToken)Match(input, 64, FOLLOW_RBRACKET_in_l_value1891);
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(el5);
					}
					if (state.backtracking == 0)
					{
						l_value_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (l_value_return != null) ? l_value_return.Tree : null);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule index", (expression_return3 != null) ? expression_return3.Tree : null);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule array", (expression_return2 != null) ? expression_return2.Tree : null);
						obj = adaptor.GetNilNode();
						object obj4 = adaptor.GetNilNode();
						obj4 = adaptor.BecomeRoot(adaptor.Create(23, token3, "[]"), obj4);
						object obj5 = adaptor.GetNilNode();
						obj5 = adaptor.BecomeRoot(adaptor.Create(15, token2, "()"), obj5);
						adaptor.AddChild(obj5, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(obj4, obj5);
						adaptor.AddChild(obj4, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(obj, obj4);
						l_value_return.Tree = obj;
						l_value_return.Tree = obj;
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_basic_l_value_in_l_value1918);
					basic_l_value_return basic_l_value_return = basic_l_value();
					state.followingStackPointer--;
					if (state.failed)
					{
						return l_value_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, basic_l_value_return.Tree);
					}
					break;
				}
				}
				l_value_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					l_value_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(l_value_return.Tree, (IToken)l_value_return.Start, (IToken)l_value_return.Stop);
				}
			}
			catch (RecognitionException ex3)
			{
				ReportError(ex3);
				Recover(input, ex3);
				l_value_return.Tree = adaptor.ErrorNode(input, (IToken)l_value_return.Start, input.LT(-1), ex3);
			}
			return l_value_return;
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x00025054 File Offset: 0x00023254
		public basic_l_value_return basic_l_value()
		{
			basic_l_value_return basic_l_value_return = new basic_l_value_return();
			basic_l_value_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token LBRACKET");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token RBRACKET");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule func_or_id");
			try
			{
				int num = input.LA(1);
				int num2;
				if (num == 38)
				{
					input.LA(2);
					if (synpred10_Papyrus())
					{
						num2 = 1;
					}
					else if (synpred11_Papyrus())
					{
						num2 = 2;
					}
					else
					{
						num2 = 3;
					}
				}
				else if (num == 82)
				{
					input.LA(2);
					if (synpred10_Papyrus())
					{
						num2 = 1;
					}
					else if (synpred11_Papyrus())
					{
						num2 = 2;
					}
					else
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return basic_l_value_return;
						}
						NoViableAltException ex = new NoViableAltException("", 40, 2, input);
						throw ex;
					}
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return basic_l_value_return;
					}
					NoViableAltException ex2 = new NoViableAltException("", 40, 0, input);
					throw ex2;
				}
				switch (num2)
				{
				case 1:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_array_func_or_id_in_basic_l_value1938);
					array_func_or_id_return array_func_or_id_return = array_func_or_id();
					state.followingStackPointer--;
					if (state.failed)
					{
						return basic_l_value_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, array_func_or_id_return.Tree);
					}
					IToken payload = (IToken)Match(input, 62, FOLLOW_DOT_in_basic_l_value1940);
					if (state.failed)
					{
						return basic_l_value_return;
					}
					if (state.backtracking == 0)
					{
						object newRoot = adaptor.Create(payload);
						obj = adaptor.BecomeRoot(newRoot, obj);
					}
					PushFollow(FOLLOW_basic_l_value_in_basic_l_value1943);
					basic_l_value_return basic_l_value_return2 = basic_l_value();
					state.followingStackPointer--;
					if (state.failed)
					{
						return basic_l_value_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, basic_l_value_return2.Tree);
					}
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_func_or_id_in_basic_l_value1956);
					func_or_id_return func_or_id_return = func_or_id();
					state.followingStackPointer--;
					if (state.failed)
					{
						return basic_l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(func_or_id_return.Tree);
					}
					IToken token = (IToken)Match(input, 63, FOLLOW_LBRACKET_in_basic_l_value1958);
					if (state.failed)
					{
						return basic_l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					PushFollow(FOLLOW_expression_in_basic_l_value1960);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return basic_l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return.Tree);
					}
					IToken el = (IToken)Match(input, 64, FOLLOW_RBRACKET_in_basic_l_value1962);
					if (state.failed)
					{
						return basic_l_value_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el);
					}
					if (state.backtracking == 0)
					{
						basic_l_value_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (basic_l_value_return != null) ? basic_l_value_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(23, token, "[]"), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj, obj2);
						basic_l_value_return.Tree = obj;
						basic_l_value_return.Tree = obj;
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					IToken payload2 = (IToken)Match(input, 38, FOLLOW_ID_in_basic_l_value1982);
					if (state.failed)
					{
						return basic_l_value_return;
					}
					if (state.backtracking == 0)
					{
						object child = adaptor.Create(payload2);
						adaptor.AddChild(obj, child);
					}
					break;
				}
				}
				basic_l_value_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					basic_l_value_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(basic_l_value_return.Tree, (IToken)basic_l_value_return.Start, (IToken)basic_l_value_return.Stop);
				}
			}
			catch (RecognitionException ex3)
			{
				ReportError(ex3);
				Recover(input, ex3);
				basic_l_value_return.Tree = adaptor.ErrorNode(input, (IToken)basic_l_value_return.Start, input.LT(-1), ex3);
			}
			return basic_l_value_return;
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x00025624 File Offset: 0x00023824
		public localDefinition_return localDefinition()
		{
			localDefinition_return localDefinition_return = new localDefinition_return();
			localDefinition_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token EQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule type");
			localDefinition_return.sVarName = "";
			localDefinition_return.kVarType = null;
			try
			{
				PushFollow(FOLLOW_type_in_localDefinition2004);
				type_return type_return = type();
				state.followingStackPointer--;
				if (state.failed)
				{
					return localDefinition_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream3.Add(type_return.Tree);
				}
				IToken token = (IToken)Match(input, 38, FOLLOW_ID_in_localDefinition2008);
				if (state.failed)
				{
					return localDefinition_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(token);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 41)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					IToken el = (IToken)Match(input, 41, FOLLOW_EQUALS_in_localDefinition2011);
					if (state.failed)
					{
						return localDefinition_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
					PushFollow(FOLLOW_expression_in_localDefinition2013);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return localDefinition_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return.Tree);
					}
				}
				PushFollow(FOLLOW_terminator_in_localDefinition2017);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return localDefinition_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
				}
				if (state.backtracking == 0)
				{
					localDefinition_return.kVarType = ((type_return != null) ? type_return.kType : null);
					localDefinition_return.sVarName = token.Text;
				}
				if (state.backtracking == 0)
				{
					localDefinition_return.Tree = obj;
					RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token name", token);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (localDefinition_return != null) ? localDefinition_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(5, token, "variable"), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream3.NextTree());
					adaptor.AddChild(obj2, rewriteRuleTokenStream3.NextNode());
					if (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					adaptor.AddChild(obj, obj2);
					localDefinition_return.Tree = obj;
					localDefinition_return.Tree = obj;
				}
				localDefinition_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					localDefinition_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(localDefinition_return.Tree, (IToken)localDefinition_return.Start, (IToken)localDefinition_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				localDefinition_return.Tree = adaptor.ErrorNode(input, (IToken)localDefinition_return.Start, input.LT(-1), ex);
			}
			return localDefinition_return;
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x00025A4C File Offset: 0x00023C4C
		public expression_return expression()
		{
			expression_return expression_return = new expression_return();
			expression_return.Start = input.LT(1);
			try
			{
				object obj = adaptor.GetNilNode();
				PushFollow(FOLLOW_and_expression_in_expression2053);
				and_expression_return and_expression_return = and_expression();
				state.followingStackPointer--;
				if (state.failed)
				{
					return expression_return;
				}
				if (state.backtracking == 0)
				{
					adaptor.AddChild(obj, and_expression_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 65)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_16A;
					}
					IToken payload = (IToken)Match(input, 65, FOLLOW_OR_in_expression2056);
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						object newRoot = adaptor.Create(payload);
						obj = adaptor.BecomeRoot(newRoot, obj);
					}
					PushFollow(FOLLOW_and_expression_in_expression2059);
					and_expression_return and_expression_return2 = and_expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						goto Block_9;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, and_expression_return2.Tree);
					}
				}
				return expression_return;
				Block_9:
				return expression_return;
				IL_16A:
				expression_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					expression_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(expression_return.Tree, (IToken)expression_return.Start, (IToken)expression_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				expression_return.Tree = adaptor.ErrorNode(input, (IToken)expression_return.Start, input.LT(-1), ex);
			}
			return expression_return;
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x00025C88 File Offset: 0x00023E88
		public and_expression_return and_expression()
		{
			and_expression_return and_expression_return = new and_expression_return();
			and_expression_return.Start = input.LT(1);
			try
			{
				object obj = adaptor.GetNilNode();
				PushFollow(FOLLOW_bool_expression_in_and_expression2073);
				bool_expression_return bool_expression_return = bool_expression();
				state.followingStackPointer--;
				if (state.failed)
				{
					return and_expression_return;
				}
				if (state.backtracking == 0)
				{
					adaptor.AddChild(obj, bool_expression_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 66)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_16A;
					}
					IToken payload = (IToken)Match(input, 66, FOLLOW_AND_in_and_expression2076);
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						object newRoot = adaptor.Create(payload);
						obj = adaptor.BecomeRoot(newRoot, obj);
					}
					PushFollow(FOLLOW_bool_expression_in_and_expression2079);
					bool_expression_return bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						goto Block_9;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, bool_expression_return2.Tree);
					}
				}
				return and_expression_return;
				Block_9:
				return and_expression_return;
				IL_16A:
				and_expression_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					and_expression_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(and_expression_return.Tree, (IToken)and_expression_return.Start, (IToken)and_expression_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				and_expression_return.Tree = adaptor.ErrorNode(input, (IToken)and_expression_return.Start, input.LT(-1), ex);
			}
			return and_expression_return;
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x00025EC4 File Offset: 0x000240C4
		public bool_expression_return bool_expression()
		{
			bool_expression_return bool_expression_return = new bool_expression_return();
			bool_expression_return.Start = input.LT(1);
			try
			{
				object obj = adaptor.GetNilNode();
				PushFollow(FOLLOW_add_expression_in_bool_expression2093);
				add_expression_return add_expression_return = add_expression();
				state.followingStackPointer--;
				if (state.failed)
				{
					return bool_expression_return;
				}
				if (state.backtracking == 0)
				{
					adaptor.AddChild(obj, add_expression_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 >= 67 && num2 <= 72)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_1CD;
					}
					IToken payload = input.LT(1);
					payload = input.LT(1);
					if (input.LA(1) < 67 || input.LA(1) > 72)
					{
						break;
					}
					input.Consume();
					if (state.backtracking == 0)
					{
						obj = adaptor.BecomeRoot(adaptor.Create(payload), obj);
					}
					state.errorRecovery = false;
					state.failed = false;
					PushFollow(FOLLOW_add_expression_in_bool_expression2121);
					add_expression_return add_expression_return2 = add_expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						goto Block_12;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, add_expression_return2.Tree);
					}
				}
				if (state.backtracking > 0)
				{
					state.failed = true;
					return bool_expression_return;
				}
				MismatchedSetException ex = new MismatchedSetException(null, input);
				throw ex;
				Block_12:
				return bool_expression_return;
				IL_1CD:
				bool_expression_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					bool_expression_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(bool_expression_return.Tree, (IToken)bool_expression_return.Start, (IToken)bool_expression_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				bool_expression_return.Tree = adaptor.ErrorNode(input, (IToken)bool_expression_return.Start, input.LT(-1), ex2);
			}
			return bool_expression_return;
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x00026164 File Offset: 0x00024364
		public add_expression_return add_expression()
		{
			add_expression_return add_expression_return = new add_expression_return();
			add_expression_return.Start = input.LT(1);
			try
			{
				object obj = adaptor.GetNilNode();
				PushFollow(FOLLOW_mult_expression_in_add_expression2135);
				mult_expression_return mult_expression_return = mult_expression();
				state.followingStackPointer--;
				if (state.failed)
				{
					return add_expression_return;
				}
				if (state.backtracking == 0)
				{
					adaptor.AddChild(obj, mult_expression_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 >= 73 && num2 <= 74)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_1CD;
					}
					IToken payload = input.LT(1);
					payload = input.LT(1);
					if (input.LA(1) < 73 || input.LA(1) > 74)
					{
						break;
					}
					input.Consume();
					if (state.backtracking == 0)
					{
						obj = adaptor.BecomeRoot(adaptor.Create(payload), obj);
					}
					state.errorRecovery = false;
					state.failed = false;
					PushFollow(FOLLOW_mult_expression_in_add_expression2147);
					mult_expression_return mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						goto Block_12;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, mult_expression_return2.Tree);
					}
				}
				if (state.backtracking > 0)
				{
					state.failed = true;
					return add_expression_return;
				}
				MismatchedSetException ex = new MismatchedSetException(null, input);
				throw ex;
				Block_12:
				return add_expression_return;
				IL_1CD:
				add_expression_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					add_expression_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(add_expression_return.Tree, (IToken)add_expression_return.Start, (IToken)add_expression_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				add_expression_return.Tree = adaptor.ErrorNode(input, (IToken)add_expression_return.Start, input.LT(-1), ex2);
			}
			return add_expression_return;
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x00026404 File Offset: 0x00024604
		public mult_expression_return mult_expression()
		{
			mult_expression_return mult_expression_return = new mult_expression_return();
			mult_expression_return.Start = input.LT(1);
			try
			{
				object obj = adaptor.GetNilNode();
				PushFollow(FOLLOW_unary_expression_in_mult_expression2161);
				unary_expression_return unary_expression_return = unary_expression();
				state.followingStackPointer--;
				if (state.failed)
				{
					return mult_expression_return;
				}
				if (state.backtracking == 0)
				{
					adaptor.AddChild(obj, unary_expression_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 >= 75 && num2 <= 77)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_1CD;
					}
					IToken payload = input.LT(1);
					payload = input.LT(1);
					if (input.LA(1) < 75 || input.LA(1) > 77)
					{
						break;
					}
					input.Consume();
					if (state.backtracking == 0)
					{
						obj = adaptor.BecomeRoot(adaptor.Create(payload), obj);
					}
					state.errorRecovery = false;
					state.failed = false;
					PushFollow(FOLLOW_unary_expression_in_mult_expression2177);
					unary_expression_return unary_expression_return2 = unary_expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						goto Block_12;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, unary_expression_return2.Tree);
					}
				}
				if (state.backtracking > 0)
				{
					state.failed = true;
					return mult_expression_return;
				}
				MismatchedSetException ex = new MismatchedSetException(null, input);
				throw ex;
				Block_12:
				return mult_expression_return;
				IL_1CD:
				mult_expression_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					mult_expression_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(mult_expression_return.Tree, (IToken)mult_expression_return.Start, (IToken)mult_expression_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				mult_expression_return.Tree = adaptor.ErrorNode(input, (IToken)mult_expression_return.Start, input.LT(-1), ex2);
			}
			return mult_expression_return;
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x000266A4 File Offset: 0x000248A4
		public unary_expression_return unary_expression()
		{
			unary_expression_return unary_expression_return = new unary_expression_return();
			unary_expression_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token MINUS");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule cast_atom");
			try
			{
				int num = input.LA(1);
				int num2;
				if (num <= 43)
				{
					if (num != 38 && num != 43)
					{
						goto IL_CC;
					}
				}
				else
				{
					switch (num)
					{
					case 74:
						num2 = 1;
						goto IL_106;
					case 75:
					case 76:
					case 77:
					case 79:
						goto IL_CC;
					case 78:
						num2 = 2;
						goto IL_106;
					case 80:
					case 81:
					case 82:
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
							goto IL_CC;
						}
						break;
					}
				}
				num2 = 3;
				goto IL_106;
				IL_CC:
				if (state.backtracking > 0)
				{
					state.failed = true;
					return unary_expression_return;
				}
				NoViableAltException ex = new NoViableAltException("", 47, 0, input);
				throw ex;
				IL_106:
				switch (num2)
				{
				case 1:
				{
					IToken token = (IToken)Match(input, 74, FOLLOW_MINUS_in_unary_expression2191);
					if (state.failed)
					{
						return unary_expression_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					PushFollow(FOLLOW_cast_atom_in_unary_expression2193);
					cast_atom_return cast_atom_return = cast_atom();
					state.followingStackPointer--;
					if (state.failed)
					{
						return unary_expression_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(cast_atom_return.Tree);
					}
					if (state.backtracking == 0)
					{
						unary_expression_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (unary_expression_return != null) ? unary_expression_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(16, token, "-"), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj, obj2);
						unary_expression_return.Tree = obj;
						unary_expression_return.Tree = obj;
					}
					break;
				}
				case 2:
				{
					obj = adaptor.GetNilNode();
					IToken payload = (IToken)Match(input, 78, FOLLOW_NOT_in_unary_expression2211);
					if (state.failed)
					{
						return unary_expression_return;
					}
					if (state.backtracking == 0)
					{
						object newRoot = adaptor.Create(payload);
						obj = adaptor.BecomeRoot(newRoot, obj);
					}
					PushFollow(FOLLOW_cast_atom_in_unary_expression2214);
					cast_atom_return cast_atom_return2 = cast_atom();
					state.followingStackPointer--;
					if (state.failed)
					{
						return unary_expression_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, cast_atom_return2.Tree);
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_cast_atom_in_unary_expression2220);
					cast_atom_return cast_atom_return3 = cast_atom();
					state.followingStackPointer--;
					if (state.failed)
					{
						return unary_expression_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, cast_atom_return3.Tree);
					}
					break;
				}
				}
				unary_expression_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					unary_expression_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(unary_expression_return.Tree, (IToken)unary_expression_return.Start, (IToken)unary_expression_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				unary_expression_return.Tree = adaptor.ErrorNode(input, (IToken)unary_expression_return.Start, input.LT(-1), ex2);
			}
			return unary_expression_return;
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x00026B00 File Offset: 0x00024D00
		public cast_atom_return cast_atom()
		{
			cast_atom_return cast_atom_return = new cast_atom_return();
			cast_atom_return.Start = input.LT(1);
			try
			{
				object obj = adaptor.GetNilNode();
				PushFollow(FOLLOW_dot_atom_in_cast_atom2232);
				dot_atom_return dot_atom_return = dot_atom();
				state.followingStackPointer--;
				if (state.failed)
				{
					return cast_atom_return;
				}
				if (state.backtracking == 0)
				{
					adaptor.AddChild(obj, dot_atom_return.Tree);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 79)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					IToken payload = (IToken)Match(input, 79, FOLLOW_AS_in_cast_atom2235);
					if (state.failed)
					{
						return cast_atom_return;
					}
					if (state.backtracking == 0)
					{
						object newRoot = adaptor.Create(payload);
						obj = adaptor.BecomeRoot(newRoot, obj);
					}
					PushFollow(FOLLOW_type_in_cast_atom2238);
					type_return type_return = type();
					state.followingStackPointer--;
					if (state.failed)
					{
						return cast_atom_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, type_return.Tree);
					}
				}
				cast_atom_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					cast_atom_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(cast_atom_return.Tree, (IToken)cast_atom_return.Start, (IToken)cast_atom_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				cast_atom_return.Tree = adaptor.ErrorNode(input, (IToken)cast_atom_return.Start, input.LT(-1), ex);
			}
			return cast_atom_return;
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x00026D34 File Offset: 0x00024F34
		public dot_atom_return dot_atom()
		{
			dot_atom_return dot_atom_return = new dot_atom_return();
			dot_atom_return.Start = input.LT(1);
			object obj = null;
			try
			{
				int num = input.LA(1);
				int num2;
				if (num == 38 || num == 43 || num == 80 || num == 82)
				{
					num2 = 1;
				}
				else if (num == 81 || (num >= 90 && num <= 93))
				{
					num2 = 2;
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return dot_atom_return;
					}
					NoViableAltException ex = new NoViableAltException("", 50, 0, input);
					throw ex;
				}
				switch (num2)
				{
				case 1:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_array_atom_in_dot_atom2252);
					array_atom_return array_atom_return = array_atom();
					state.followingStackPointer--;
					if (state.failed)
					{
						return dot_atom_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, array_atom_return.Tree);
					}
					for (;;)
					{
						int num3 = 2;
						int num4 = input.LA(1);
						if (num4 == 62)
						{
							num3 = 1;
						}
						int num5 = num3;
						if (num5 != 1)
						{
							goto IL_26D;
						}
						IToken payload = (IToken)Match(input, 62, FOLLOW_DOT_in_dot_atom2255);
						if (state.failed)
						{
							break;
						}
						if (state.backtracking == 0)
						{
							object newRoot = adaptor.Create(payload);
							obj = adaptor.BecomeRoot(newRoot, obj);
						}
						PushFollow(FOLLOW_array_func_or_id_in_dot_atom2258);
						array_func_or_id_return array_func_or_id_return = array_func_or_id();
						state.followingStackPointer--;
						if (state.failed)
						{
							goto Block_16;
						}
						if (state.backtracking == 0)
						{
							adaptor.AddChild(obj, array_func_or_id_return.Tree);
						}
					}
					return dot_atom_return;
					Block_16:
					return dot_atom_return;
				}
				case 2:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_constant_in_dot_atom2266);
					constant_return constant_return = constant();
					state.followingStackPointer--;
					if (state.failed)
					{
						return dot_atom_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, constant_return.Tree);
					}
					break;
				}
				}
				IL_26D:
				dot_atom_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					dot_atom_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(dot_atom_return.Tree, (IToken)dot_atom_return.Start, (IToken)dot_atom_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				dot_atom_return.Tree = adaptor.ErrorNode(input, (IToken)dot_atom_return.Start, input.LT(-1), ex2);
			}
			return dot_atom_return;
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x00027074 File Offset: 0x00025274
		public array_atom_return array_atom()
		{
			array_atom_return array_atom_return = new array_atom_return();
			array_atom_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token LBRACKET");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token RBRACKET");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule atom");
			try
			{
				int num = input.LA(1);
				int num2;
				if (num != 38)
				{
					if (num != 43)
					{
						switch (num)
						{
						case 80:
							input.LA(2);
							if (synpred12_Papyrus())
							{
								num2 = 1;
								goto IL_163;
							}
							num2 = 2;
							goto IL_163;
						case 82:
							input.LA(2);
							if (synpred12_Papyrus())
							{
								num2 = 1;
								goto IL_163;
							}
							num2 = 2;
							goto IL_163;
						}
						if (state.backtracking > 0)
						{
							state.failed = true;
							return array_atom_return;
						}
						NoViableAltException ex = new NoViableAltException("", 51, 0, input);
						throw ex;
					}
					else
					{
						input.LA(2);
						if (synpred12_Papyrus())
						{
							num2 = 1;
						}
						else
						{
							num2 = 2;
						}
					}
				}
				else
				{
					input.LA(2);
					if (synpred12_Papyrus())
					{
						num2 = 1;
					}
					else
					{
						num2 = 2;
					}
				}
				IL_163:
				switch (num2)
				{
				case 1:
				{
					PushFollow(FOLLOW_atom_in_array_atom2285);
					atom_return atom_return = atom();
					state.followingStackPointer--;
					if (state.failed)
					{
						return array_atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(atom_return.Tree);
					}
					IToken token = (IToken)Match(input, 63, FOLLOW_LBRACKET_in_array_atom2287);
					if (state.failed)
					{
						return array_atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					PushFollow(FOLLOW_expression_in_array_atom2289);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return array_atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return.Tree);
					}
					IToken el = (IToken)Match(input, 64, FOLLOW_RBRACKET_in_array_atom2291);
					if (state.failed)
					{
						return array_atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el);
					}
					if (state.backtracking == 0)
					{
						array_atom_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (array_atom_return != null) ? array_atom_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(22, token, "[]"), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj, obj2);
						array_atom_return.Tree = obj;
						array_atom_return.Tree = obj;
					}
					break;
				}
				case 2:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_atom_in_array_atom2311);
					atom_return atom_return2 = atom();
					state.followingStackPointer--;
					if (state.failed)
					{
						return array_atom_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, atom_return2.Tree);
					}
					break;
				}
				}
				array_atom_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					array_atom_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(array_atom_return.Tree, (IToken)array_atom_return.Start, (IToken)array_atom_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				array_atom_return.Tree = adaptor.ErrorNode(input, (IToken)array_atom_return.Start, input.LT(-1), ex2);
			}
			return array_atom_return;
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x00027510 File Offset: 0x00025710
		public atom_return atom()
		{
			atom_return atom_return = new atom_return();
			atom_return.Start = input.LT(1);
			object obj = null;
			IToken token = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token NEW");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token LBRACKET");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token INTEGER");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token RBRACKET");
			RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token BASETYPE");
			RewriteRuleTokenStream rewriteRuleTokenStream7 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream8 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			try
			{
				int num = input.LA(1);
				int num2;
				if (num != 38)
				{
					if (num == 43)
					{
						num2 = 1;
						goto IL_153;
					}
					switch (num)
					{
					case 80:
						num2 = 2;
						goto IL_153;
					case 82:
						goto IL_114;
					}
					if (state.backtracking > 0)
					{
						state.failed = true;
						return atom_return;
					}
					NoViableAltException ex = new NoViableAltException("", 53, 0, input);
					throw ex;
				}
				IL_114:
				num2 = 3;
				IL_153:
				switch (num2)
				{
				case 1:
				{
					IToken token2 = (IToken)Match(input, 43, FOLLOW_LPAREN_in_atom2321);
					if (state.failed)
					{
						return atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream8.Add(token2);
					}
					PushFollow(FOLLOW_expression_in_atom2323);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return.Tree);
					}
					IToken el = (IToken)Match(input, 44, FOLLOW_RPAREN_in_atom2325);
					if (state.failed)
					{
						return atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream4.Add(el);
					}
					if (state.backtracking == 0)
					{
						atom_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (atom_return != null) ? atom_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(15, token2, "()"), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj, obj2);
						atom_return.Tree = obj;
						atom_return.Tree = obj;
					}
					break;
				}
				case 2:
				{
					IToken el2 = (IToken)Match(input, 80, FOLLOW_NEW_in_atom2343);
					if (state.failed)
					{
						return atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el2);
					}
					int num3 = input.LA(1);
					int num4;
					if (num3 == 55)
					{
						num4 = 1;
					}
					else if (num3 == 38)
					{
						num4 = 2;
					}
					else
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return atom_return;
						}
						NoViableAltException ex2 = new NoViableAltException("", 52, 0, input);
						throw ex2;
					}
					switch (num4)
					{
					case 1:
						token = (IToken)Match(input, 55, FOLLOW_BASETYPE_in_atom2348);
						if (state.failed)
						{
							return atom_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream6.Add(token);
						}
						break;
					case 2:
						token = (IToken)Match(input, 38, FOLLOW_ID_in_atom2354);
						if (state.failed)
						{
							return atom_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream7.Add(token);
						}
						break;
					}
					IToken el3 = (IToken)Match(input, 63, FOLLOW_LBRACKET_in_atom2357);
					if (state.failed)
					{
						return atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el3);
					}
					IToken token3 = (IToken)Match(input, 81, FOLLOW_INTEGER_in_atom2359);
					if (state.failed)
					{
						return atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(token3);
					}
					IToken el4 = (IToken)Match(input, 64, FOLLOW_RBRACKET_in_atom2361);
					if (state.failed)
					{
						return atom_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream5.Add(el4);
					}
					if (state.backtracking == 0)
					{
						int num5 = int.Parse(token3.Text);
						if (num5 < 1 || num5 > 128)
						{
							OnError("arrays must be between 1 and 128 elements in size", token3.Line, token3.CharPositionInLine);
						}
					}
					if (state.backtracking == 0)
					{
						atom_return.Tree = obj;
						RewriteRuleTokenStream rewriteRuleTokenStream9 = new RewriteRuleTokenStream(adaptor, "token arrayType", token);
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (atom_return != null) ? atom_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj3 = adaptor.GetNilNode();
						obj3 = adaptor.BecomeRoot(rewriteRuleTokenStream.NextNode(), obj3);
						adaptor.AddChild(obj3, rewriteRuleTokenStream9.NextNode());
						adaptor.AddChild(obj3, rewriteRuleTokenStream3.NextNode());
						adaptor.AddChild(obj, obj3);
						atom_return.Tree = obj;
						atom_return.Tree = obj;
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_func_or_id_in_atom2386);
					func_or_id_return func_or_id_return = func_or_id();
					state.followingStackPointer--;
					if (state.failed)
					{
						return atom_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, func_or_id_return.Tree);
					}
					break;
				}
				}
				atom_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					atom_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(atom_return.Tree, (IToken)atom_return.Start, (IToken)atom_return.Stop);
				}
			}
			catch (RecognitionException ex3)
			{
				ReportError(ex3);
				Recover(input, ex3);
				atom_return.Tree = adaptor.ErrorNode(input, (IToken)atom_return.Start, input.LT(-1), ex3);
			}
			return atom_return;
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x00027C50 File Offset: 0x00025E50
		public array_func_or_id_return array_func_or_id()
		{
			array_func_or_id_return array_func_or_id_return = new array_func_or_id_return();
			array_func_or_id_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token LBRACKET");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token RBRACKET");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule function_call");
			try
			{
				int num = input.LA(1);
				int num2;
				if (num == 38)
				{
					input.LA(2);
					if (synpred13_Papyrus())
					{
						num2 = 1;
					}
					else if (synpred14_Papyrus())
					{
						num2 = 2;
					}
					else
					{
						num2 = 3;
					}
				}
				else if (num == 82)
				{
					num2 = 3;
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return array_func_or_id_return;
					}
					NoViableAltException ex = new NoViableAltException("", 54, 0, input);
					throw ex;
				}
				switch (num2)
				{
				case 1:
				{
					PushFollow(FOLLOW_function_call_in_array_func_or_id2405);
					function_call_return function_call_return = function_call();
					state.followingStackPointer--;
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(function_call_return.Tree);
					}
					IToken token = (IToken)Match(input, 63, FOLLOW_LBRACKET_in_array_func_or_id2407);
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					PushFollow(FOLLOW_expression_in_array_func_or_id2409);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return.Tree);
					}
					IToken el = (IToken)Match(input, 64, FOLLOW_RBRACKET_in_array_func_or_id2411);
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el);
					}
					if (state.backtracking == 0)
					{
						array_func_or_id_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (array_func_or_id_return != null) ? array_func_or_id_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(22, token, "[]"), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj, obj2);
						array_func_or_id_return.Tree = obj;
						array_func_or_id_return.Tree = obj;
					}
					break;
				}
				case 2:
				{
					IToken el2 = (IToken)Match(input, 38, FOLLOW_ID_in_array_func_or_id2438);
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(el2);
					}
					IToken token2 = (IToken)Match(input, 63, FOLLOW_LBRACKET_in_array_func_or_id2440);
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token2);
					}
					PushFollow(FOLLOW_expression_in_array_func_or_id2442);
					expression_return expression_return2 = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return2.Tree);
					}
					IToken el3 = (IToken)Match(input, 64, FOLLOW_RBRACKET_in_array_func_or_id2444);
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el3);
					}
					if (state.backtracking == 0)
					{
						array_func_or_id_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (array_func_or_id_return != null) ? array_func_or_id_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj3 = adaptor.GetNilNode();
						obj3 = adaptor.BecomeRoot(adaptor.Create(22, token2, "[]"), obj3);
						adaptor.AddChild(obj3, rewriteRuleTokenStream3.NextNode());
						adaptor.AddChild(obj3, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj, obj3);
						array_func_or_id_return.Tree = obj;
						array_func_or_id_return.Tree = obj;
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_func_or_id_in_array_func_or_id2464);
					func_or_id_return func_or_id_return = func_or_id();
					state.followingStackPointer--;
					if (state.failed)
					{
						return array_func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, func_or_id_return.Tree);
					}
					break;
				}
				}
				array_func_or_id_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					array_func_or_id_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(array_func_or_id_return.Tree, (IToken)array_func_or_id_return.Start, (IToken)array_func_or_id_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				array_func_or_id_return.Tree = adaptor.ErrorNode(input, (IToken)array_func_or_id_return.Start, input.LT(-1), ex2);
			}
			return array_func_or_id_return;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x00028280 File Offset: 0x00026480
		public func_or_id_return func_or_id()
		{
			func_or_id_return func_or_id_return = new func_or_id_return();
			func_or_id_return.Start = input.LT(1);
			object obj = null;
			try
			{
				int num = input.LA(1);
				int num3;
				if (num == 38)
				{
					int num2 = input.LA(2);
					if (num2 == 43)
					{
						num3 = 1;
					}
					else if (num2 == 44 || num2 == 49 || (num2 >= 62 && num2 <= 77) || num2 == 79 || num2 == 94)
					{
						num3 = 2;
					}
					else
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return func_or_id_return;
						}
						NoViableAltException ex = new NoViableAltException("", 55, 1, input);
						throw ex;
					}
				}
				else if (num == 82)
				{
					num3 = 3;
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return func_or_id_return;
					}
					NoViableAltException ex2 = new NoViableAltException("", 55, 0, input);
					throw ex2;
				}
				switch (num3)
				{
				case 1:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_function_call_in_func_or_id2476);
					function_call_return function_call_return = function_call();
					state.followingStackPointer--;
					if (state.failed)
					{
						return func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, function_call_return.Tree);
					}
					break;
				}
				case 2:
				{
					obj = adaptor.GetNilNode();
					IToken payload = (IToken)Match(input, 38, FOLLOW_ID_in_func_or_id2482);
					if (state.failed)
					{
						return func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						object child = adaptor.Create(payload);
						adaptor.AddChild(obj, child);
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					IToken payload2 = (IToken)Match(input, 82, FOLLOW_LENGTH_in_func_or_id2488);
					if (state.failed)
					{
						return func_or_id_return;
					}
					if (state.backtracking == 0)
					{
						object child2 = adaptor.Create(payload2);
						adaptor.AddChild(obj, child2);
					}
					break;
				}
				}
				func_or_id_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					func_or_id_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(func_or_id_return.Tree, (IToken)func_or_id_return.Start, (IToken)func_or_id_return.Stop);
				}
			}
			catch (RecognitionException ex3)
			{
				ReportError(ex3);
				Recover(input, ex3);
				func_or_id_return.Tree = adaptor.ErrorNode(input, (IToken)func_or_id_return.Start, input.LT(-1), ex3);
			}
			return func_or_id_return;
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x000285AC File Offset: 0x000267AC
		public return_stat_return return_stat()
		{
			return_stat_return return_stat_return = new return_stat_return();
			return_stat_return.Start = input.LT(1);
			try
			{
				object obj = adaptor.GetNilNode();
				IToken payload = (IToken)Match(input, 83, FOLLOW_RETURN_in_return_stat2501);
				if (state.failed)
				{
					return return_stat_return;
				}
				if (state.backtracking == 0)
				{
					object newRoot = adaptor.Create(payload);
					obj = adaptor.BecomeRoot(newRoot, obj);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 38 || num2 == 43 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 82) || (num2 >= 90 && num2 <= 93))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_expression_in_return_stat2504);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return return_stat_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, expression_return.Tree);
					}
				}
				PushFollow(FOLLOW_terminator_in_return_stat2507);
				terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return return_stat_return;
				}
				return_stat_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					return_stat_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(return_stat_return.Tree, (IToken)return_stat_return.Start, (IToken)return_stat_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				return_stat_return.Tree = adaptor.ErrorNode(input, (IToken)return_stat_return.Start, input.LT(-1), ex);
			}
			return return_stat_return;
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x000287E4 File Offset: 0x000269E4
		public ifBlock_return ifBlock(ScriptScope akCurrentScope)
		{
			ifBlock_stack.Push(new ifBlock_scope());
			ifBlock_return ifBlock_return = new ifBlock_return();
			ifBlock_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ENDIF");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token IF");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule statement");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule elseIfBlock");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule elseBlock");
			((ifBlock_scope)ifBlock_stack.Peek()).kparentScope = akCurrentScope;
			((ifBlock_scope)ifBlock_stack.Peek()).kifScope = new ScriptScope(akCurrentScope, "if");
			try
			{
				try
				{
					IToken el = (IToken)Match(input, 84, FOLLOW_IF_in_ifBlock2531);
					if (state.failed)
					{
						return ifBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(el);
					}
					PushFollow(FOLLOW_expression_in_ifBlock2533);
					expression_return expression_return = expression();
					state.followingStackPointer--;
					if (state.failed)
					{
						return ifBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(expression_return.Tree);
					}
					PushFollow(FOLLOW_terminator_in_ifBlock2535);
					terminator_return terminator_return = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return ifBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream4.Add(terminator_return.Tree);
					}
					for (;;)
					{
						int num = 2;
						int num2 = input.LA(1);
						if (num2 == 38 || num2 == 43 || num2 == 55 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 84) || num2 == 88 || (num2 >= 90 && num2 <= 93))
						{
							num = 1;
						}
						int num3 = num;
						if (num3 != 1)
						{
							break;
						}
						PushFollow(FOLLOW_statement_in_ifBlock2537);
						statement_return statement_return = statement(((ifBlock_scope)ifBlock_stack.Peek()).kifScope);
						state.followingStackPointer--;
						if (state.failed)
						{
							goto Block_19;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream2.Add(statement_return.Tree);
						}
					}
					for (;;)
					{
						int num4 = 2;
						int num5 = input.LA(1);
						if (num5 == 86)
						{
							num4 = 1;
						}
						int num6 = num4;
						if (num6 != 1)
						{
							goto IL_342;
						}
						PushFollow(FOLLOW_elseIfBlock_in_ifBlock2541);
						elseIfBlock_return elseIfBlock_return = elseIfBlock(((ifBlock_scope)ifBlock_stack.Peek()).kparentScope);
						state.followingStackPointer--;
						if (state.failed)
						{
							break;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream3.Add(elseIfBlock_return.Tree);
						}
					}
					return ifBlock_return;
					IL_342:
					int num7 = 2;
					int num8 = input.LA(1);
					if (num8 == 87)
					{
						num7 = 1;
					}
					int num9 = num7;
					if (num9 == 1)
					{
						PushFollow(FOLLOW_elseBlock_in_ifBlock2545);
						elseBlock_return elseBlock_return = elseBlock(((ifBlock_scope)ifBlock_stack.Peek()).kparentScope);
						state.followingStackPointer--;
						if (state.failed)
						{
							return ifBlock_return;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream5.Add(elseBlock_return.Tree);
						}
					}
					IToken el2 = (IToken)Match(input, 85, FOLLOW_ENDIF_in_ifBlock2549);
					if (state.failed)
					{
						return ifBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el2);
					}
					PushFollow(FOLLOW_terminator_in_ifBlock2551);
					terminator_return terminator_return2 = terminator();
					state.followingStackPointer--;
					if (state.failed)
					{
						return ifBlock_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream4.Add(terminator_return2.Tree);
					}
					if (state.backtracking == 0)
					{
						ifBlock_return.Tree = obj;
						new RewriteRuleSubtreeStream(adaptor, "rule retval", (ifBlock_return != null) ? ifBlock_return.Tree : null);
						obj = adaptor.GetNilNode();
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(rewriteRuleTokenStream2.NextNode(), obj2);
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
						object obj3 = adaptor.GetNilNode();
						obj3 = adaptor.BecomeRoot(adaptor.Create(10, "BLOCK"), obj3);
						while (rewriteRuleSubtreeStream2.HasNext())
						{
							adaptor.AddChild(obj3, rewriteRuleSubtreeStream2.NextTree());
						}
						rewriteRuleSubtreeStream2.Reset();
						adaptor.AddChild(obj2, obj3);
						while (rewriteRuleSubtreeStream3.HasNext())
						{
							adaptor.AddChild(obj2, rewriteRuleSubtreeStream3.NextTree());
						}
						rewriteRuleSubtreeStream3.Reset();
						if (rewriteRuleSubtreeStream5.HasNext())
						{
							adaptor.AddChild(obj2, rewriteRuleSubtreeStream5.NextTree());
						}
						rewriteRuleSubtreeStream5.Reset();
						adaptor.AddChild(obj, obj2);
						ifBlock_return.Tree = obj;
						ifBlock_return.Tree = obj;
					}
					ifBlock_return.Stop = input.LT(-1);
					if (state.backtracking == 0)
					{
						ifBlock_return.Tree = adaptor.RulePostProcessing(obj);
						adaptor.SetTokenBoundaries(ifBlock_return.Tree, (IToken)ifBlock_return.Start, (IToken)ifBlock_return.Stop);
					}
					goto IL_64D;
					Block_19:
					return ifBlock_return;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					ifBlock_return.Tree = adaptor.ErrorNode(input, (IToken)ifBlock_return.Start, input.LT(-1), ex);
				}
				IL_64D:;
			}
			finally
			{
				ifBlock_stack.Pop();
			}
			return ifBlock_return;
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x00028E88 File Offset: 0x00027088
		public elseIfBlock_return elseIfBlock(ScriptScope akCurrentScope)
		{
			elseIfBlock_stack.Push(new elseIfBlock_scope());
			elseIfBlock_return elseIfBlock_return = new elseIfBlock_return();
			elseIfBlock_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ELSEIF");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule statement");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			((elseIfBlock_scope)elseIfBlock_stack.Peek()).kelseIfScope = new ScriptScope(akCurrentScope, "elseif");
			try
			{
				IToken el = (IToken)Match(input, 86, FOLLOW_ELSEIF_in_elseIfBlock2597);
				if (state.failed)
				{
					return elseIfBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(el);
				}
				PushFollow(FOLLOW_expression_in_elseIfBlock2599);
				expression_return expression_return = expression();
				state.followingStackPointer--;
				if (state.failed)
				{
					return elseIfBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
				}
				PushFollow(FOLLOW_terminator_in_elseIfBlock2601);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return elseIfBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream3.Add(terminator_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 38 || num2 == 43 || num2 == 55 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 84) || num2 == 88 || (num2 >= 90 && num2 <= 93))
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_255;
					}
					PushFollow(FOLLOW_statement_in_elseIfBlock2603);
					statement_return statement_return = statement(((elseIfBlock_scope)elseIfBlock_stack.Peek()).kelseIfScope);
					state.followingStackPointer--;
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(statement_return.Tree);
					}
				}
				return elseIfBlock_return;
				IL_255:
				if (state.backtracking == 0)
				{
					elseIfBlock_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (elseIfBlock_return != null) ? elseIfBlock_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(rewriteRuleTokenStream.NextNode(), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
					object obj3 = adaptor.GetNilNode();
					obj3 = adaptor.BecomeRoot(adaptor.Create(10, "BLOCK"), obj3);
					while (rewriteRuleSubtreeStream2.HasNext())
					{
						adaptor.AddChild(obj3, rewriteRuleSubtreeStream2.NextTree());
					}
					rewriteRuleSubtreeStream2.Reset();
					adaptor.AddChild(obj2, obj3);
					adaptor.AddChild(obj, obj2);
					elseIfBlock_return.Tree = obj;
					elseIfBlock_return.Tree = obj;
				}
				elseIfBlock_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					elseIfBlock_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(elseIfBlock_return.Tree, (IToken)elseIfBlock_return.Start, (IToken)elseIfBlock_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				elseIfBlock_return.Tree = adaptor.ErrorNode(input, (IToken)elseIfBlock_return.Start, input.LT(-1), ex);
			}
			finally
			{
				elseIfBlock_stack.Pop();
			}
			return elseIfBlock_return;
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x000292CC File Offset: 0x000274CC
		public elseBlock_return elseBlock(ScriptScope akCurrentScope)
		{
			elseBlock_stack.Push(new elseBlock_scope());
			elseBlock_return elseBlock_return = new elseBlock_return();
			elseBlock_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ELSE");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule statement");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			((elseBlock_scope)elseBlock_stack.Peek()).kelseScope = new ScriptScope(akCurrentScope, "else");
			try
			{
				IToken el = (IToken)Match(input, 87, FOLLOW_ELSE_in_elseBlock2645);
				if (state.failed)
				{
					return elseBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(el);
				}
				PushFollow(FOLLOW_terminator_in_elseBlock2647);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return elseBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream2.Add(terminator_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 38 || num2 == 43 || num2 == 55 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 84) || num2 == 88 || (num2 >= 90 && num2 <= 93))
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_1EA;
					}
					PushFollow(FOLLOW_statement_in_elseBlock2649);
					statement_return statement_return = statement(((elseBlock_scope)elseBlock_stack.Peek()).kelseScope);
					state.followingStackPointer--;
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(statement_return.Tree);
					}
				}
				return elseBlock_return;
				IL_1EA:
				if (state.backtracking == 0)
				{
					elseBlock_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (elseBlock_return != null) ? elseBlock_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(rewriteRuleTokenStream.NextNode(), obj2);
					object obj3 = adaptor.GetNilNode();
					obj3 = adaptor.BecomeRoot(adaptor.Create(10, "BLOCK"), obj3);
					while (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj3, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					adaptor.AddChild(obj2, obj3);
					adaptor.AddChild(obj, obj2);
					elseBlock_return.Tree = obj;
					elseBlock_return.Tree = obj;
				}
				elseBlock_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					elseBlock_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(elseBlock_return.Tree, (IToken)elseBlock_return.Start, (IToken)elseBlock_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				elseBlock_return.Tree = adaptor.ErrorNode(input, (IToken)elseBlock_return.Start, input.LT(-1), ex);
			}
			finally
			{
				elseBlock_stack.Pop();
			}
			return elseBlock_return;
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x00029690 File Offset: 0x00027890
		public whileBlock_return whileBlock(ScriptScope akCurrentScope)
		{
			whileBlock_stack.Push(new whileBlock_scope());
			whileBlock_return whileBlock_return = new whileBlock_return();
			whileBlock_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ENDWHILE");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token WHILE");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule statement");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule terminator");
			((whileBlock_scope)whileBlock_stack.Peek()).kwhileScope = new ScriptScope(akCurrentScope, "while");
			try
			{
				IToken el = (IToken)Match(input, 88, FOLLOW_WHILE_in_whileBlock2690);
				if (state.failed)
				{
					return whileBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(el);
				}
				PushFollow(FOLLOW_expression_in_whileBlock2692);
				expression_return expression_return = expression();
				state.followingStackPointer--;
				if (state.failed)
				{
					return whileBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
				}
				PushFollow(FOLLOW_terminator_in_whileBlock2694);
				terminator_return terminator_return = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return whileBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream3.Add(terminator_return.Tree);
				}
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 38 || num2 == 43 || num2 == 55 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 84) || num2 == 88 || (num2 >= 90 && num2 <= 93))
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_26F;
					}
					PushFollow(FOLLOW_statement_in_whileBlock2696);
					statement_return statement_return = statement(((whileBlock_scope)whileBlock_stack.Peek()).kwhileScope);
					state.followingStackPointer--;
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(statement_return.Tree);
					}
				}
				return whileBlock_return;
				IL_26F:
				IToken el2 = (IToken)Match(input, 89, FOLLOW_ENDWHILE_in_whileBlock2700);
				if (state.failed)
				{
					return whileBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(el2);
				}
				PushFollow(FOLLOW_terminator_in_whileBlock2702);
				terminator_return terminator_return2 = terminator();
				state.followingStackPointer--;
				if (state.failed)
				{
					return whileBlock_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream3.Add(terminator_return2.Tree);
				}
				if (state.backtracking == 0)
				{
					whileBlock_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (whileBlock_return != null) ? whileBlock_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(rewriteRuleTokenStream2.NextNode(), obj2);
					adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
					object obj3 = adaptor.GetNilNode();
					obj3 = adaptor.BecomeRoot(adaptor.Create(10, "BLOCK"), obj3);
					while (rewriteRuleSubtreeStream2.HasNext())
					{
						adaptor.AddChild(obj3, rewriteRuleSubtreeStream2.NextTree());
					}
					rewriteRuleSubtreeStream2.Reset();
					adaptor.AddChild(obj2, obj3);
					adaptor.AddChild(obj, obj2);
					whileBlock_return.Tree = obj;
					whileBlock_return.Tree = obj;
				}
				whileBlock_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					whileBlock_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(whileBlock_return.Tree, (IToken)whileBlock_return.Start, (IToken)whileBlock_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				whileBlock_return.Tree = adaptor.ErrorNode(input, (IToken)whileBlock_return.Start, input.LT(-1), ex);
			}
			finally
			{
				whileBlock_stack.Pop();
			}
			return whileBlock_return;
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x00029B88 File Offset: 0x00027D88
		public function_call_return function_call()
		{
			function_call_return function_call_return = new function_call_return();
			function_call_return.Start = input.LT(1);
			object obj = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule parameters");
			try
			{
				IToken token = (IToken)Match(input, 38, FOLLOW_ID_in_function_call2733);
				if (state.failed)
				{
					return function_call_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream2.Add(token);
				}
				IToken el = (IToken)Match(input, 43, FOLLOW_LPAREN_in_function_call2735);
				if (state.failed)
				{
					return function_call_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream3.Add(el);
				}
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 38 || num2 == 43 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 82) || (num2 >= 90 && num2 <= 93))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_parameters_in_function_call2737);
					parameters_return parameters_return = parameters();
					state.followingStackPointer--;
					if (state.failed)
					{
						return function_call_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(parameters_return.Tree);
					}
				}
				IToken el2 = (IToken)Match(input, 44, FOLLOW_RPAREN_in_function_call2740);
				if (state.failed)
				{
					return function_call_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleTokenStream.Add(el2);
				}
				if (state.backtracking == 0)
				{
					function_call_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (function_call_return != null) ? function_call_return.Tree : null);
					obj = adaptor.GetNilNode();
					object obj2 = adaptor.GetNilNode();
					obj2 = adaptor.BecomeRoot(adaptor.Create(11, token, "call"), obj2);
					adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
					object obj3 = adaptor.GetNilNode();
					obj3 = adaptor.BecomeRoot(adaptor.Create(14, token, "callparams"), obj3);
					if (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj3, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					adaptor.AddChild(obj2, obj3);
					adaptor.AddChild(obj, obj2);
					function_call_return.Tree = obj;
					function_call_return.Tree = obj;
				}
				function_call_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					function_call_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(function_call_return.Tree, (IToken)function_call_return.Start, (IToken)function_call_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				function_call_return.Tree = adaptor.ErrorNode(input, (IToken)function_call_return.Start, input.LT(-1), ex);
			}
			return function_call_return;
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00029F34 File Offset: 0x00028134
		public parameters_return parameters()
		{
			parameters_return parameters_return = new parameters_return();
			parameters_return.Start = input.LT(1);
			object obj = null;
			IList list = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token COMMA");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule parameter");
			try
			{
				PushFollow(FOLLOW_parameter_in_parameters2774);
				parameter_return parameter_return = parameter();
				state.followingStackPointer--;
				if (state.failed)
				{
					return parameters_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(parameter_return.Tree);
				}
				if (list == null)
				{
					list = new ArrayList();
				}
				list.Add(parameter_return.Tree);
				for (;;)
				{
					int num = 2;
					int num2 = input.LA(1);
					if (num2 == 49)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						goto IL_190;
					}
					IToken el = (IToken)Match(input, 49, FOLLOW_COMMA_in_parameters2777);
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
					PushFollow(FOLLOW_parameter_in_parameters2781);
					parameter_return = parameter();
					state.followingStackPointer--;
					if (state.failed)
					{
						goto Block_10;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(parameter_return.Tree);
					}
					if (list == null)
					{
						list = new ArrayList();
					}
					list.Add(parameter_return.Tree);
				}
				return parameters_return;
				Block_10:
				return parameters_return;
				IL_190:
				if (state.backtracking == 0)
				{
					parameters_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (parameters_return != null) ? parameters_return.Tree : null);
					obj = adaptor.GetNilNode();
					if (!rewriteRuleSubtreeStream.HasNext())
					{
						throw new RewriteEarlyExitException();
					}
					while (rewriteRuleSubtreeStream.HasNext())
					{
						adaptor.AddChild(obj, rewriteRuleSubtreeStream.NextTree());
					}
					rewriteRuleSubtreeStream.Reset();
					parameters_return.Tree = obj;
					parameters_return.Tree = obj;
				}
				parameters_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					parameters_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(parameters_return.Tree, (IToken)parameters_return.Start, (IToken)parameters_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				parameters_return.Tree = adaptor.ErrorNode(input, (IToken)parameters_return.Start, input.LT(-1), ex);
			}
			return parameters_return;
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x0002A214 File Offset: 0x00028414
		public parameter_return parameter()
		{
			parameter_return parameter_return = new parameter_return();
			parameter_return.Start = input.LT(1);
			object obj = null;
			IToken token = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token EQUALS");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			try
			{
				int num = 2;
				int num2 = input.LA(1);
				if (num2 == 38)
				{
					int num3 = input.LA(2);
					if (num3 == 41)
					{
						num = 1;
					}
				}
				int num4 = num;
				if (num4 == 1)
				{
					token = (IToken)Match(input, 38, FOLLOW_ID_in_parameter2804);
					if (state.failed)
					{
						return parameter_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token);
					}
					IToken el = (IToken)Match(input, 41, FOLLOW_EQUALS_in_parameter2806);
					if (state.failed)
					{
						return parameter_return;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(el);
					}
				}
				PushFollow(FOLLOW_expression_in_parameter2810);
				expression_return expression_return = expression();
				state.followingStackPointer--;
				if (state.failed)
				{
					return parameter_return;
				}
				if (state.backtracking == 0)
				{
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
				}
				if (state.backtracking == 0)
				{
					parameter_return.Tree = obj;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (parameter_return != null) ? parameter_return.Tree : null);
					obj = adaptor.GetNilNode();
					if (token != null)
					{
						object obj2 = adaptor.GetNilNode();
						obj2 = adaptor.BecomeRoot(adaptor.Create(9, "PARAM"), obj2);
						adaptor.AddChild(obj2, rewriteRuleTokenStream2.NextNode());
						adaptor.AddChild(obj2, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj, obj2);
					}
					else
					{
						object obj3 = adaptor.GetNilNode();
						obj3 = adaptor.BecomeRoot(adaptor.Create(9, "PARAM"), obj3);
						adaptor.AddChild(obj3, adaptor.Create(38, (expression_return != null) ? ((IToken)expression_return.Start) : null, ""));
						adaptor.AddChild(obj3, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(obj, obj3);
					}
					parameter_return.Tree = obj;
					parameter_return.Tree = obj;
				}
				parameter_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					parameter_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(parameter_return.Tree, (IToken)parameter_return.Start, (IToken)parameter_return.Stop);
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
				parameter_return.Tree = adaptor.ErrorNode(input, (IToken)parameter_return.Start, input.LT(-1), ex);
			}
			return parameter_return;
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x0002A590 File Offset: 0x00028790
		public constant_return constant()
		{
			constant_return constant_return = new constant_return();
			constant_return.Start = input.LT(1);
			object obj = null;
			try
			{
				int num = input.LA(1);
				int num2;
				if (num != 81)
				{
					switch (num)
					{
					case 90:
						num2 = 2;
						goto IL_AE;
					case 91:
						num2 = 3;
						goto IL_AE;
					case 92:
						num2 = 4;
						goto IL_AE;
					case 93:
						break;
					default:
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return constant_return;
						}
						NoViableAltException ex = new NoViableAltException("", 66, 0, input);
						throw ex;
					}
					}
				}
				num2 = 1;
				IL_AE:
				switch (num2)
				{
				case 1:
				{
					obj = adaptor.GetNilNode();
					PushFollow(FOLLOW_number_in_constant2856);
					number_return number_return = number();
					state.followingStackPointer--;
					if (state.failed)
					{
						return constant_return;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(obj, number_return.Tree);
					}
					break;
				}
				case 2:
				{
					obj = adaptor.GetNilNode();
					IToken payload = (IToken)Match(input, 90, FOLLOW_STRING_in_constant2862);
					if (state.failed)
					{
						return constant_return;
					}
					if (state.backtracking == 0)
					{
						object child = adaptor.Create(payload);
						adaptor.AddChild(obj, child);
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					IToken payload2 = (IToken)Match(input, 91, FOLLOW_BOOL_in_constant2868);
					if (state.failed)
					{
						return constant_return;
					}
					if (state.backtracking == 0)
					{
						object child2 = adaptor.Create(payload2);
						adaptor.AddChild(obj, child2);
					}
					break;
				}
				case 4:
				{
					obj = adaptor.GetNilNode();
					IToken payload3 = (IToken)Match(input, 92, FOLLOW_NONE_in_constant2874);
					if (state.failed)
					{
						return constant_return;
					}
					if (state.backtracking == 0)
					{
						object child3 = adaptor.Create(payload3);
						adaptor.AddChild(obj, child3);
					}
					break;
				}
				}
				constant_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					constant_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(constant_return.Tree, (IToken)constant_return.Start, (IToken)constant_return.Stop);
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				constant_return.Tree = adaptor.ErrorNode(input, (IToken)constant_return.Start, input.LT(-1), ex2);
			}
			return constant_return;
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x0002A8D8 File Offset: 0x00028AD8
		public number_return number()
		{
			number_return number_return = new number_return();
			number_return.Start = input.LT(1);
			try
			{
				object nilNode = adaptor.GetNilNode();
				IToken payload = input.LT(1);
				if (input.LA(1) == 81 || input.LA(1) == 93)
				{
					input.Consume();
					if (state.backtracking == 0)
					{
						adaptor.AddChild(nilNode, adaptor.Create(payload));
					}
					state.errorRecovery = false;
					state.failed = false;
					number_return.Stop = input.LT(-1);
					if (state.backtracking == 0)
					{
						number_return.Tree = adaptor.RulePostProcessing(nilNode);
						adaptor.SetTokenBoundaries(number_return.Tree, (IToken)number_return.Start, (IToken)number_return.Stop);
					}
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return number_return;
					}
					MismatchedSetException ex = new MismatchedSetException(null, input);
					throw ex;
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				number_return.Tree = adaptor.ErrorNode(input, (IToken)number_return.Start, input.LT(-1), ex2);
			}
			return number_return;
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x0002AA7C File Offset: 0x00028C7C
		public type_return type()
		{
			type_return type_return = new type_return();
			type_return.Start = input.LT(1);
			object obj = null;
			try
			{
				int num = input.LA(1);
				int num3;
				if (num == 38)
				{
					int num2 = input.LA(2);
					if (num2 == 63)
					{
						num3 = 2;
					}
					else if (num2 == 6 || num2 == 38 || num2 == 44 || num2 == 49 || num2 == 54 || (num2 >= 64 && num2 <= 77) || num2 == 94)
					{
						num3 = 1;
					}
					else
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return type_return;
						}
						NoViableAltException ex = new NoViableAltException("", 67, 1, input);
						throw ex;
					}
				}
				else if (num == 55)
				{
					int num4 = input.LA(2);
					if (num4 == 63)
					{
						num3 = 4;
					}
					else if (num4 == 6 || num4 == 38 || num4 == 44 || num4 == 49 || num4 == 54 || (num4 >= 64 && num4 <= 77) || num4 == 94)
					{
						num3 = 3;
					}
					else
					{
						if (state.backtracking > 0)
						{
							state.failed = true;
							return type_return;
						}
						NoViableAltException ex2 = new NoViableAltException("", 67, 2, input);
						throw ex2;
					}
				}
				else
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return type_return;
					}
					NoViableAltException ex3 = new NoViableAltException("", 67, 0, input);
					throw ex3;
				}
				switch (num3)
				{
				case 1:
				{
					obj = adaptor.GetNilNode();
					IToken token = (IToken)Match(input, 38, FOLLOW_ID_in_type2908);
					if (state.failed)
					{
						return type_return;
					}
					if (state.backtracking == 0)
					{
						object child = adaptor.Create(token);
						adaptor.AddChild(obj, child);
					}
					if (state.backtracking == 0)
					{
						type_return.kType = ConstructVarType(token.Text, null);
					}
					break;
				}
				case 2:
				{
					obj = adaptor.GetNilNode();
					IToken token2 = (IToken)Match(input, 38, FOLLOW_ID_in_type2919);
					if (state.failed)
					{
						return type_return;
					}
					if (state.backtracking == 0)
					{
						object child2 = adaptor.Create(token2);
						adaptor.AddChild(obj, child2);
					}
					IToken payload = (IToken)Match(input, 63, FOLLOW_LBRACKET_in_type2921);
					if (state.failed)
					{
						return type_return;
					}
					if (state.backtracking == 0)
					{
						object child3 = adaptor.Create(payload);
						adaptor.AddChild(obj, child3);
					}
					IToken payload2 = (IToken)Match(input, 64, FOLLOW_RBRACKET_in_type2923);
					if (state.failed)
					{
						return type_return;
					}
					if (state.backtracking == 0)
					{
						object child4 = adaptor.Create(payload2);
						adaptor.AddChild(obj, child4);
					}
					if (state.backtracking == 0)
					{
						type_return.kType = ConstructVarType(token2.Text + "[]", null);
					}
					break;
				}
				case 3:
				{
					obj = adaptor.GetNilNode();
					IToken token3 = (IToken)Match(input, 55, FOLLOW_BASETYPE_in_type2934);
					if (state.failed)
					{
						return type_return;
					}
					if (state.backtracking == 0)
					{
						object child5 = adaptor.Create(token3);
						adaptor.AddChild(obj, child5);
					}
					if (state.backtracking == 0)
					{
						type_return.kType = ConstructVarType(token3.Text, null);
					}
					break;
				}
				case 4:
				{
					obj = adaptor.GetNilNode();
					IToken token4 = (IToken)Match(input, 55, FOLLOW_BASETYPE_in_type2945);
					if (state.failed)
					{
						return type_return;
					}
					if (state.backtracking == 0)
					{
						object child6 = adaptor.Create(token4);
						adaptor.AddChild(obj, child6);
					}
					IToken payload3 = (IToken)Match(input, 63, FOLLOW_LBRACKET_in_type2947);
					if (state.failed)
					{
						return type_return;
					}
					if (state.backtracking == 0)
					{
						object child7 = adaptor.Create(payload3);
						adaptor.AddChild(obj, child7);
					}
					IToken payload4 = (IToken)Match(input, 64, FOLLOW_RBRACKET_in_type2949);
					if (state.failed)
					{
						return type_return;
					}
					if (state.backtracking == 0)
					{
						object child8 = adaptor.Create(payload4);
						adaptor.AddChild(obj, child8);
					}
					if (state.backtracking == 0)
					{
						type_return.kType = ConstructVarType(token4.Text + "[]", null);
					}
					break;
				}
				}
				type_return.Stop = input.LT(-1);
				if (state.backtracking == 0)
				{
					type_return.Tree = adaptor.RulePostProcessing(obj);
					adaptor.SetTokenBoundaries(type_return.Tree, (IToken)type_return.Start, (IToken)type_return.Stop);
				}
			}
			catch (RecognitionException ex4)
			{
				ReportError(ex4);
				Recover(input, ex4);
				type_return.Tree = adaptor.ErrorNode(input, (IToken)type_return.Start, input.LT(-1), ex4);
			}
			return type_return;
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x0002B0D0 File Offset: 0x000292D0
		public terminator_return terminator()
		{
			terminator_return terminator_return = new terminator_return();
			terminator_return.Start = input.LT(1);
			try
			{
				object nilNode = adaptor.GetNilNode();
				int num = 0;
				for (;;)
				{
					int num2 = 2;
					int num3 = input.LA(1);
					if (num3 == 94)
					{
						num2 = 1;
					}
					int num4 = num2;
					if (num4 != 1)
					{
						goto IL_A7;
					}
					IToken payload = (IToken)Match(input, 94, FOLLOW_EOL_in_terminator2967);
					if (state.failed)
					{
						break;
					}
					if (state.backtracking == 0)
					{
						object child = adaptor.Create(payload);
						adaptor.AddChild(nilNode, child);
					}
					num++;
				}
				return terminator_return;
				IL_A7:
				if (num < 1)
				{
					if (state.backtracking > 0)
					{
						state.failed = true;
						return terminator_return;
					}
					EarlyExitException ex = new EarlyExitException(68, input);
					throw ex;
				}
				else
				{
					terminator_return.Stop = input.LT(-1);
					if (state.backtracking == 0)
					{
						terminator_return.Tree = adaptor.RulePostProcessing(nilNode);
						adaptor.SetTokenBoundaries(terminator_return.Tree, (IToken)terminator_return.Start, (IToken)terminator_return.Stop);
					}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
				terminator_return.Tree = adaptor.ErrorNode(input, (IToken)terminator_return.Start, input.LT(-1), ex2);
			}
			return terminator_return;
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x0002B28C File Offset: 0x0002948C
		public void synpred1_Papyrus_fragment()
		{
			PushFollow(FOLLOW_type_in_synpred1_Papyrus1498);
			type();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 38, FOLLOW_ID_in_synpred1_Papyrus1500);
			bool failed = state.failed;
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0002B2EC File Offset: 0x000294EC
		public void synpred2_Papyrus_fragment()
		{
			PushFollow(FOLLOW_l_value_in_synpred2_Papyrus1516);
			l_value();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 57, FOLLOW_PLUSEQUALS_in_synpred2_Papyrus1518);
			bool failed = state.failed;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x0002B34C File Offset: 0x0002954C
		public void synpred3_Papyrus_fragment()
		{
			PushFollow(FOLLOW_l_value_in_synpred3_Papyrus1567);
			l_value();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 58, FOLLOW_MINUSEQUALS_in_synpred3_Papyrus1569);
			bool failed = state.failed;
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x0002B3AC File Offset: 0x000295AC
		public void synpred4_Papyrus_fragment()
		{
			PushFollow(FOLLOW_l_value_in_synpred4_Papyrus1617);
			l_value();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 59, FOLLOW_MULTEQUALS_in_synpred4_Papyrus1619);
			bool failed = state.failed;
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x0002B40C File Offset: 0x0002960C
		public void synpred5_Papyrus_fragment()
		{
			PushFollow(FOLLOW_l_value_in_synpred5_Papyrus1667);
			l_value();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 60, FOLLOW_DIVEQUALS_in_synpred5_Papyrus1669);
			bool failed = state.failed;
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0002B46C File Offset: 0x0002966C
		public void synpred6_Papyrus_fragment()
		{
			PushFollow(FOLLOW_l_value_in_synpred6_Papyrus1717);
			l_value();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 61, FOLLOW_MODEQUALS_in_synpred6_Papyrus1719);
			bool failed = state.failed;
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x0002B4CC File Offset: 0x000296CC
		public void synpred7_Papyrus_fragment()
		{
			PushFollow(FOLLOW_l_value_in_synpred7_Papyrus1767);
			l_value();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 41, FOLLOW_EQUALS_in_synpred7_Papyrus1769);
			bool failed = state.failed;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x0002B52C File Offset: 0x0002972C
		public void synpred8_Papyrus_fragment()
		{
			Match(input, 43, FOLLOW_LPAREN_in_synpred8_Papyrus1823);
			if (state.failed)
			{
				return;
			}
			PushFollow(FOLLOW_expression_in_synpred8_Papyrus1825);
			expression();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 44, FOLLOW_RPAREN_in_synpred8_Papyrus1827);
			if (state.failed)
			{
				return;
			}
			Match(input, 62, FOLLOW_DOT_in_synpred8_Papyrus1829);
			bool failed = state.failed;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x0002B5D0 File Offset: 0x000297D0
		public void synpred9_Papyrus_fragment()
		{
			Match(input, 43, FOLLOW_LPAREN_in_synpred9_Papyrus1867);
			if (state.failed)
			{
				return;
			}
			PushFollow(FOLLOW_expression_in_synpred9_Papyrus1869);
			expression();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 44, FOLLOW_RPAREN_in_synpred9_Papyrus1871);
			if (state.failed)
			{
				return;
			}
			Match(input, 63, FOLLOW_LBRACKET_in_synpred9_Papyrus1873);
			bool failed = state.failed;
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0002B674 File Offset: 0x00029874
		public void synpred10_Papyrus_fragment()
		{
			PushFollow(FOLLOW_array_func_or_id_in_synpred10_Papyrus1932);
			array_func_or_id();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 62, FOLLOW_DOT_in_synpred10_Papyrus1934);
			bool failed = state.failed;
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x0002B6D4 File Offset: 0x000298D4
		public void synpred11_Papyrus_fragment()
		{
			PushFollow(FOLLOW_func_or_id_in_synpred11_Papyrus1950);
			func_or_id();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 63, FOLLOW_LBRACKET_in_synpred11_Papyrus1952);
			bool failed = state.failed;
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0002B734 File Offset: 0x00029934
		public void synpred12_Papyrus_fragment()
		{
			PushFollow(FOLLOW_atom_in_synpred12_Papyrus2279);
			atom();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 63, FOLLOW_LBRACKET_in_synpred12_Papyrus2281);
			bool failed = state.failed;
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0002B794 File Offset: 0x00029994
		public void synpred13_Papyrus_fragment()
		{
			PushFollow(FOLLOW_function_call_in_synpred13_Papyrus2399);
			function_call();
			state.followingStackPointer--;
			if (state.failed)
			{
				return;
			}
			Match(input, 63, FOLLOW_LBRACKET_in_synpred13_Papyrus2401);
			bool failed = state.failed;
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x0002B7F4 File Offset: 0x000299F4
		public void synpred14_Papyrus_fragment()
		{
			Match(input, 38, FOLLOW_ID_in_synpred14_Papyrus2432);
			if (state.failed)
			{
				return;
			}
			Match(input, 63, FOLLOW_LBRACKET_in_synpred14_Papyrus2434);
			bool failed = state.failed;
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x0002B844 File Offset: 0x00029A44
		public bool synpred2_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred2_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x0002B8DC File Offset: 0x00029ADC
		public bool synpred3_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred3_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x0002B974 File Offset: 0x00029B74
		public bool synpred7_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred7_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x0002BA0C File Offset: 0x00029C0C
		public bool synpred14_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred14_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x0002BAA4 File Offset: 0x00029CA4
		public bool synpred10_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred10_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x0002BB3C File Offset: 0x00029D3C
		public bool synpred4_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred4_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x0002BBD4 File Offset: 0x00029DD4
		public bool synpred8_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred8_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x0002BC6C File Offset: 0x00029E6C
		public bool synpred6_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred6_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x0002BD04 File Offset: 0x00029F04
		public bool synpred5_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred5_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x0002BD9C File Offset: 0x00029F9C
		public bool synpred12_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred12_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x0002BE34 File Offset: 0x0002A034
		public bool synpred13_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred13_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x0002BECC File Offset: 0x0002A0CC
		public bool synpred11_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred11_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x0002BF64 File Offset: 0x0002A164
		public bool synpred1_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred1_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x0002BFFC File Offset: 0x0002A1FC
		public bool synpred9_Papyrus()
		{
			state.backtracking++;
			int marker = input.Mark();
			try
			{
				synpred9_Papyrus_fragment();
			}
			catch (RecognitionException arg)
			{
				Console.Error.WriteLine("impossible: " + arg);
			}
			bool result = !state.failed;
			input.Rewind(marker);
			state.backtracking--;
			state.failed = false;
			return result;
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x0002C094 File Offset: 0x0002A294
		private void InitializeCyclicDFAs()
		{
			dfa6 = new DFA6(this);
			dfa14 = new DFA14(this);
			dfa16 = new DFA16(this);
			dfa21 = new DFA21(this);
			dfa23 = new DFA23(this);
			dfa30 = new DFA30(this);
			dfa38 = new DFA38(this);
			dfa16.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA16_SpecialStateTransition);
			dfa23.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA23_SpecialStateTransition);
			dfa38.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA38_SpecialStateTransition);
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x0002C13C File Offset: 0x0002A33C
		protected internal int DFA16_SpecialStateTransition(DFA dfa, int s, IIntStream _input)
		{
			ITokenStream tokenStream = (ITokenStream)_input;
			int stateNumber = s;
			switch (s)
			{
			case 0:
			{
				int num = tokenStream.LA(1);
				int index = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num == 64 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 21;
				}
				else if ((num == 38 || num == 43 || num == 74 || num == 78 || (num >= 80 && num <= 82) || (num >= 90 && num <= 93)) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 1:
			{
				int num2 = tokenStream.LA(1);
				int index2 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num2 == 64 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 22;
				}
				tokenStream.Seek(index2);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 2:
			{
				int num3 = tokenStream.LA(1);
				int index3 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num3 == 38 && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num3 == 94 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 11;
				}
				else if (((num3 >= 65 && num3 <= 77) || num3 == 79) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index3);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 3:
			{
				int num4 = tokenStream.LA(1);
				int index4 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num4 == 38 && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num4 == 94 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 11;
				}
				else if (((num4 >= 65 && num4 <= 77) || num4 == 79) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index4);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 4:
			{
				int num5 = tokenStream.LA(1);
				int index5 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num5 == 41 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 10;
				}
				else if (num5 == 94 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 11;
				}
				else if (num5 == 38 && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index5);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 5:
			{
				int num6 = tokenStream.LA(1);
				int index6 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (((num6 >= 65 && num6 <= 77) || num6 == 79) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				else if (num6 == 94 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 11;
				}
				else if (num6 == 38 && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index6);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 6:
			{
				int num7 = tokenStream.LA(1);
				int index7 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num7 == 63 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 19;
				}
				else if ((num7 == 41 || num7 == 43 || (num7 >= 57 && num7 <= 62) || (num7 >= 65 && num7 <= 77) || num7 == 79 || num7 == 94) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				else if ((num7 == 6 || num7 == 54) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num7 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index7);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 7:
			{
				int num8 = tokenStream.LA(1);
				int index8 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num8 == 94 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 1;
				}
				tokenStream.Seek(index8);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 8:
			{
				int num9 = tokenStream.LA(1);
				int index9 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num9 == -1 || (num9 >= 6 && num9 <= 7) || num9 == 42 || (num9 >= 50 && num9 <= 53)) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num9 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 3;
				}
				else if (num9 == 55 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 4;
				}
				else if (num9 == 94 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 1;
				}
				else if ((num9 == 43 || num9 == 45 || num9 == 74 || num9 == 78 || (num9 >= 80 && num9 <= 84) || num9 == 88 || (num9 >= 90 && num9 <= 93)) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index9);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 9:
			{
				int num10 = tokenStream.LA(1);
				int index10 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num10 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 7;
				}
				else if ((num10 == 6 || num10 == 54) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index10);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 10:
			{
				int num11 = tokenStream.LA(1);
				int index11 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num11 == 63 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 8;
				}
				else if ((num11 == 6 || num11 == 54) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num11 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index11);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 11:
			{
				int num12 = tokenStream.LA(1);
				int index12 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num12 == 64 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 12;
				}
				tokenStream.Seek(index12);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 12:
			{
				int num13 = tokenStream.LA(1);
				int index13 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num13 == 81 || num13 == 93) && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 13;
				}
				else if (num13 == 90 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 14;
				}
				else if (num13 == 91 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 15;
				}
				else if (num13 == 92 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 16;
				}
				else if ((num13 == 38 || num13 == 43 || num13 == 74 || num13 == 78 || num13 == 80 || num13 == 82) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index13);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 13:
			{
				int num14 = tokenStream.LA(1);
				int index14 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num14 == 6 || num14 == 54) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num14 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index14);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 14:
			{
				int num15 = tokenStream.LA(1);
				int index15 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num15 == 38 && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num15 == 94 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 11;
				}
				else if (((num15 >= 65 && num15 <= 77) || num15 == 79) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index15);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 15:
			{
				int num16 = tokenStream.LA(1);
				int index16 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num16 == 63 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 6;
				}
				else if ((num16 == 41 || num16 == 43 || (num16 >= 57 && num16 <= 62) || (num16 >= 65 && num16 <= 77) || num16 == 79 || num16 == 94) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				else if ((num16 == 6 || num16 == 54) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num16 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index16);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 16:
			{
				int num17 = tokenStream.LA(1);
				int index17 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num17 == 64 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 9;
				}
				else if ((num17 == 38 || num17 == 43 || num17 == 74 || num17 == 78 || (num17 >= 80 && num17 <= 82) || (num17 >= 90 && num17 <= 93)) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index17);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 17:
			{
				int num18 = tokenStream.LA(1);
				int index18 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num18 == 43 || num18 == 45 || num18 == 74 || num18 == 78 || (num18 >= 80 && num18 <= 84) || num18 == 88 || (num18 >= 90 && num18 <= 93)) && !((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 5;
				}
				else if (num18 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 17;
				}
				else if (num18 == 55 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 18;
				}
				else if (num18 == 94 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 11;
				}
				else if ((num18 == -1 || (num18 >= 6 && num18 <= 7) || num18 == 42 || (num18 >= 50 && num18 <= 51)) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index18);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 18:
			{
				int num19 = tokenStream.LA(1);
				int index19 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num19 == 6 || num19 == 54) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num19 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index19);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 19:
			{
				int num20 = tokenStream.LA(1);
				int index20 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num20 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 7;
				}
				else if ((num20 == 6 || num20 == 54) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index20);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 20:
			{
				int num21 = tokenStream.LA(1);
				int index21 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num21 == 63 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 20;
				}
				else if ((num21 == 6 || num21 == 54) && ((function_scope)function_stack.Peek()).kfunction.bNative)
				{
					s = 2;
				}
				else if (num21 == 38 && (((function_scope)function_stack.Peek()).kfunction.bNative || !((function_scope)function_stack.Peek()).kfunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index21);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			}
			if (state.backtracking > 0)
			{
				state.failed = true;
				return -1;
			}
			NoViableAltException ex = new NoViableAltException(dfa.Description, 16, stateNumber, tokenStream);
			dfa.Error(ex);
			throw ex;
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x0002D3C4 File Offset: 0x0002B5C4
		protected internal int DFA23_SpecialStateTransition(DFA dfa, int s, IIntStream _input)
		{
			ITokenStream tokenStream = (ITokenStream)_input;
			int stateNumber = s;
			switch (s)
			{
			case 0:
			{
				int num = tokenStream.LA(1);
				int index = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num == 64 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 21;
				}
				else if ((num == 38 || num == 43 || num == 74 || num == 78 || (num >= 80 && num <= 82) || (num >= 90 && num <= 93)) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 1:
			{
				int num2 = tokenStream.LA(1);
				int index2 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num2 == -1 || (num2 >= 6 && num2 <= 7) || num2 == 42 || (num2 >= 50 && num2 <= 52)) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num2 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 3;
				}
				else if (num2 == 55 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 4;
				}
				else if (num2 == 94 && (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 1;
				}
				else if ((num2 == 43 || num2 == 48 || num2 == 74 || num2 == 78 || (num2 >= 80 && num2 <= 84) || num2 == 88 || (num2 >= 90 && num2 <= 93)) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index2);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 2:
			{
				int num3 = tokenStream.LA(1);
				int index3 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num3 == 64 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 22;
				}
				tokenStream.Seek(index3);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 3:
			{
				int num4 = tokenStream.LA(1);
				int index4 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num4 == 63 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 6;
				}
				else if ((num4 == 41 || num4 == 43 || (num4 >= 57 && num4 <= 62) || (num4 >= 65 && num4 <= 77) || num4 == 79 || num4 == 94) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				else if ((num4 == 6 || num4 == 54) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num4 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index4);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 4:
			{
				int num5 = tokenStream.LA(1);
				int index5 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num5 == 64 && (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 12;
				}
				tokenStream.Seek(index5);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 5:
			{
				int num6 = tokenStream.LA(1);
				int index6 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num6 == 64 && (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 9;
				}
				else if ((num6 == 38 || num6 == 43 || num6 == 74 || num6 == 78 || (num6 >= 80 && num6 <= 82) || (num6 >= 90 && num6 <= 93)) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index6);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 6:
			{
				int num7 = tokenStream.LA(1);
				int index7 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num7 == 38 && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num7 == 94 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 11;
				}
				else if (((num7 >= 65 && num7 <= 77) || num7 == 79) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index7);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 7:
			{
				int num8 = tokenStream.LA(1);
				int index8 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num8 == 38 && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num8 == 94 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 11;
				}
				else if (((num8 >= 65 && num8 <= 77) || num8 == 79) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index8);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 8:
			{
				int num9 = tokenStream.LA(1);
				int index9 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num9 == 63 && (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 8;
				}
				else if ((num9 == 6 || num9 == 54) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num9 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index9);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 9:
			{
				int num10 = tokenStream.LA(1);
				int index10 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num10 == 43 || num10 == 48 || num10 == 74 || num10 == 78 || (num10 >= 80 && num10 <= 84) || num10 == 88 || (num10 >= 90 && num10 <= 93)) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				else if (num10 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 17;
				}
				else if (num10 == 55 && (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 18;
				}
				else if (num10 == 94 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 11;
				}
				else if ((num10 == -1 || (num10 >= 6 && num10 <= 7) || num10 == 42 || (num10 >= 50 && num10 <= 51)) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index10);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 10:
			{
				int num11 = tokenStream.LA(1);
				int index11 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num11 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 7;
				}
				else if ((num11 == 6 || num11 == 54) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index11);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 11:
			{
				int num12 = tokenStream.LA(1);
				int index12 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num12 == 41 && (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 10;
				}
				else if (num12 == 38 && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num12 == 94 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 11;
				}
				tokenStream.Seek(index12);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 12:
			{
				int num13 = tokenStream.LA(1);
				int index13 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (((num13 >= 65 && num13 <= 77) || num13 == 79) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				else if (num13 == 94 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 11;
				}
				else if (num13 == 38 && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index13);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 13:
			{
				int num14 = tokenStream.LA(1);
				int index14 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num14 == 63 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 19;
				}
				else if ((num14 == 41 || num14 == 43 || (num14 >= 57 && num14 <= 62) || (num14 >= 65 && num14 <= 77) || num14 == 79 || num14 == 94) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				else if ((num14 == 6 || num14 == 54) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num14 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index14);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 14:
			{
				int num15 = tokenStream.LA(1);
				int index15 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num15 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 7;
				}
				else if ((num15 == 6 || num15 == 54) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				tokenStream.Seek(index15);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 15:
			{
				int num16 = tokenStream.LA(1);
				int index16 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num16 == 38 || num16 == 43 || num16 == 74 || num16 == 78 || num16 == 80 || num16 == 82) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				else if ((num16 == 81 || num16 == 93) && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 13;
				}
				else if (num16 == 90 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 14;
				}
				else if (num16 == 91 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 15;
				}
				else if (num16 == 92 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 16;
				}
				tokenStream.Seek(index16);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 16:
			{
				int num17 = tokenStream.LA(1);
				int index17 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num17 == 6 || num17 == 54) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num17 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index17);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 17:
			{
				int num18 = tokenStream.LA(1);
				int index18 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if ((num18 == 6 || num18 == 54) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num18 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index18);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 18:
			{
				int num19 = tokenStream.LA(1);
				int index19 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num19 == 38 && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num19 == 94 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 11;
				}
				else if (((num19 >= 65 && num19 <= 77) || num19 == 79) && !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 5;
				}
				tokenStream.Seek(index19);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 19:
			{
				int num20 = tokenStream.LA(1);
				int index20 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num20 == 94 && (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 1;
				}
				tokenStream.Seek(index20);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 20:
			{
				int num21 = tokenStream.LA(1);
				int index21 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num21 == 63 && (!((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 20;
				}
				else if ((num21 == 6 || num21 == 54) && ((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative)
				{
					s = 2;
				}
				else if (num21 == 38 && (((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative || !((eventFunc_scope)eventFunc_stack.Peek()).keventFunction.bNative))
				{
					s = 7;
				}
				tokenStream.Seek(index21);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			}
			if (state.backtracking > 0)
			{
				state.failed = true;
				return -1;
			}
			NoViableAltException ex = new NoViableAltException(dfa.Description, 23, stateNumber, tokenStream);
			dfa.Error(ex);
			throw ex;
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x0002E64C File Offset: 0x0002C84C
		protected internal int DFA38_SpecialStateTransition(DFA dfa, int s, IIntStream _input)
		{
			ITokenStream tokenStream = (ITokenStream)_input;
			int stateNumber = s;
			switch (s)
			{
			case 0:
			{
				int num = tokenStream.LA(1);
				int index = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (num == 38)
				{
					s = 1;
				}
				else if (num == 55 && synpred1_Papyrus())
				{
					s = 2;
				}
				else if (num == 43)
				{
					s = 3;
				}
				else if (num == 82)
				{
					s = 4;
				}
				else if (num == 74 || num == 78 || (num >= 80 && num <= 81) || (num >= 90 && num <= 93))
				{
					s = 5;
				}
				else if (num == 83)
				{
					s = 12;
				}
				else if (num == 84)
				{
					s = 13;
				}
				else if (num == 88)
				{
					s = 14;
				}
				tokenStream.Seek(index);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 1:
			{
				tokenStream.LA(1);
				int index2 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (synpred1_Papyrus())
				{
					s = 2;
				}
				else if (synpred2_Papyrus())
				{
					s = 15;
				}
				else if (synpred3_Papyrus())
				{
					s = 16;
				}
				else if (synpred4_Papyrus())
				{
					s = 17;
				}
				else if (synpred5_Papyrus())
				{
					s = 18;
				}
				else if (synpred6_Papyrus())
				{
					s = 19;
				}
				else if (synpred7_Papyrus())
				{
					s = 20;
				}
				else
				{
					s = 5;
				}
				tokenStream.Seek(index2);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 2:
			{
				tokenStream.LA(1);
				int index3 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (synpred2_Papyrus())
				{
					s = 15;
				}
				else if (synpred3_Papyrus())
				{
					s = 16;
				}
				else if (synpred4_Papyrus())
				{
					s = 17;
				}
				else if (synpred5_Papyrus())
				{
					s = 18;
				}
				else if (synpred6_Papyrus())
				{
					s = 19;
				}
				else if (synpred7_Papyrus())
				{
					s = 20;
				}
				else
				{
					s = 5;
				}
				tokenStream.Seek(index3);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			case 3:
			{
				tokenStream.LA(1);
				int index4 = tokenStream.Index();
				tokenStream.Rewind();
				s = -1;
				if (synpred2_Papyrus())
				{
					s = 15;
				}
				else if (synpred3_Papyrus())
				{
					s = 16;
				}
				else if (synpred4_Papyrus())
				{
					s = 17;
				}
				else if (synpred5_Papyrus())
				{
					s = 18;
				}
				else if (synpred6_Papyrus())
				{
					s = 19;
				}
				else if (synpred7_Papyrus())
				{
					s = 20;
				}
				else
				{
					s = 5;
				}
				tokenStream.Seek(index4);
				if (s >= 0)
				{
					return s;
				}
				break;
			}
			}
			if (state.backtracking > 0)
			{
				state.failed = true;
				return -1;
			}
			NoViableAltException ex = new NoViableAltException(dfa.Description, 38, stateNumber, tokenStream);
			dfa.Error(ex);
			throw ex;
		}

		// Token: 0x0400028F RID: 655
		public const int FUNCTION = 6;

		// Token: 0x04000290 RID: 656
		public const int LT = 70;

		// Token: 0x04000291 RID: 657
		public const int DIVEQUALS = 60;

		// Token: 0x04000292 RID: 658
		public const int WHILE = 88;

		// Token: 0x04000293 RID: 659
		public const int MOD = 77;

		// Token: 0x04000294 RID: 660
		public const int PROPSET = 21;

		// Token: 0x04000295 RID: 661
		public const int NEW = 80;

		// Token: 0x04000296 RID: 662
		public const int DQUOTE = 98;

		// Token: 0x04000297 RID: 663
		public const int PARAM = 9;

		// Token: 0x04000298 RID: 664
		public const int EQUALS = 41;

		// Token: 0x04000299 RID: 665
		public const int NOT = 78;

		// Token: 0x0400029A RID: 666
		public const int EOF = -1;

		// Token: 0x0400029B RID: 667
		public const int FNEGATE = 35;

		// Token: 0x0400029C RID: 668
		public const int LBRACKET = 63;

		// Token: 0x0400029D RID: 669
		public const int USER_FLAGS = 18;

		// Token: 0x0400029E RID: 670
		public const int RPAREN = 44;

		// Token: 0x0400029F RID: 671
		public const int IMPORT = 42;

		// Token: 0x040002A0 RID: 672
		public const int EOL = 94;

		// Token: 0x040002A1 RID: 673
		public const int FADD = 27;

		// Token: 0x040002A2 RID: 674
		public const int RETURN = 83;

		// Token: 0x040002A3 RID: 675
		public const int ENDIF = 85;

		// Token: 0x040002A4 RID: 676
		public const int VAR = 5;

		// Token: 0x040002A5 RID: 677
		public const int ENDWHILE = 89;

		// Token: 0x040002A6 RID: 678
		public const int EQ = 67;

		// Token: 0x040002A7 RID: 679
		public const int COMMENT = 104;

		// Token: 0x040002A8 RID: 680
		public const int IMULTIPLY = 30;

		// Token: 0x040002A9 RID: 681
		public const int IDIVIDE = 32;

		// Token: 0x040002AA RID: 682
		public const int DIVIDE = 76;

		// Token: 0x040002AB RID: 683
		public const int NE = 68;

		// Token: 0x040002AC RID: 684
		public const int SCRIPTNAME = 37;

		// Token: 0x040002AD RID: 685
		public const int MINUSEQUALS = 58;

		// Token: 0x040002AE RID: 686
		public const int RBRACE = 100;

		// Token: 0x040002AF RID: 687
		public const int ARRAYFIND = 24;

		// Token: 0x040002B0 RID: 688
		public const int ELSE = 87;

		// Token: 0x040002B1 RID: 689
		public const int BOOL = 91;

		// Token: 0x040002B2 RID: 690
		public const int FDIVIDE = 33;

		// Token: 0x040002B3 RID: 691
		public const int NATIVE = 47;

		// Token: 0x040002B4 RID: 692
		public const int UNARY_MINUS = 16;

		// Token: 0x040002B5 RID: 693
		public const int MULT = 75;

		// Token: 0x040002B6 RID: 694
		public const int ENDPROPERTY = 53;

		// Token: 0x040002B7 RID: 695
		public const int CALLPARAMS = 14;

		// Token: 0x040002B8 RID: 696
		public const int ALPHA = 95;

		// Token: 0x040002B9 RID: 697
		public const int WS = 102;

		// Token: 0x040002BA RID: 698
		public const int FMULTIPLY = 31;

		// Token: 0x040002BB RID: 699
		public const int ARRAYSET = 23;

		// Token: 0x040002BC RID: 700
		public const int PROPERTY = 54;

		// Token: 0x040002BD RID: 701
		public const int AUTOREADONLY = 56;

		// Token: 0x040002BE RID: 702
		public const int NONE = 92;

		// Token: 0x040002BF RID: 703
		public const int OR = 65;

		// Token: 0x040002C0 RID: 704
		public const int PROPGET = 20;

		// Token: 0x040002C1 RID: 705
		public const int IADD = 26;

		// Token: 0x040002C2 RID: 706
		public const int PROPFUNC = 17;

		// Token: 0x040002C3 RID: 707
		public const int GT = 69;

		// Token: 0x040002C4 RID: 708
		public const int CALL = 11;

		// Token: 0x040002C5 RID: 709
		public const int INEGATE = 34;

		// Token: 0x040002C6 RID: 710
		public const int BASETYPE = 55;

		// Token: 0x040002C7 RID: 711
		public const int ENDEVENT = 48;

		// Token: 0x040002C8 RID: 712
		public const int MULTEQUALS = 59;

		// Token: 0x040002C9 RID: 713
		public const int CALLPARENT = 13;

		// Token: 0x040002CA RID: 714
		public const int LBRACE = 99;

		// Token: 0x040002CB RID: 715
		public const int GTE = 71;

		// Token: 0x040002CC RID: 716
		public const int FLOAT = 93;

		// Token: 0x040002CD RID: 717
		public const int ENDSTATE = 52;

		// Token: 0x040002CE RID: 718
		public const int ID = 38;

		// Token: 0x040002CF RID: 719
		public const int AND = 66;

		// Token: 0x040002D0 RID: 720
		public const int LTE = 72;

		// Token: 0x040002D1 RID: 721
		public const int LPAREN = 43;

		// Token: 0x040002D2 RID: 722
		public const int LENGTH = 82;

		// Token: 0x040002D3 RID: 723
		public const int IF = 84;

		// Token: 0x040002D4 RID: 724
		public const int CALLGLOBAL = 12;

		// Token: 0x040002D5 RID: 725
		public const int AS = 79;

		// Token: 0x040002D6 RID: 726
		public const int OBJECT = 4;

		// Token: 0x040002D7 RID: 727
		public const int COMMA = 49;

		// Token: 0x040002D8 RID: 728
		public const int PLUSEQUALS = 57;

		// Token: 0x040002D9 RID: 729
		public const int AUTO = 50;

		// Token: 0x040002DA RID: 730
		public const int ISUBTRACT = 28;

		// Token: 0x040002DB RID: 731
		public const int PLUS = 73;

		// Token: 0x040002DC RID: 732
		public const int ENDFUNCTION = 45;

		// Token: 0x040002DD RID: 733
		public const int DIGIT = 96;

		// Token: 0x040002DE RID: 734
		public const int HEADER = 8;

		// Token: 0x040002DF RID: 735
		public const int RBRACKET = 64;

		// Token: 0x040002E0 RID: 736
		public const int DOT = 62;

		// Token: 0x040002E1 RID: 737
		public const int FSUBTRACT = 29;

		// Token: 0x040002E2 RID: 738
		public const int STRCAT = 36;

		// Token: 0x040002E3 RID: 739
		public const int INTEGER = 81;

		// Token: 0x040002E4 RID: 740
		public const int STATE = 51;

		// Token: 0x040002E5 RID: 741
		public const int DOCSTRING = 40;

		// Token: 0x040002E6 RID: 742
		public const int WS_CHAR = 101;

		// Token: 0x040002E7 RID: 743
		public const int HEX_DIGIT = 97;

		// Token: 0x040002E8 RID: 744
		public const int ARRAYRFIND = 25;

		// Token: 0x040002E9 RID: 745
		public const int MINUS = 74;

		// Token: 0x040002EA RID: 746
		public const int ARRAYGET = 22;

		// Token: 0x040002EB RID: 747
		public const int EVENT = 7;

		// Token: 0x040002EC RID: 748
		public const int ELSEIF = 86;

		// Token: 0x040002ED RID: 749
		public const int AUTOPROP = 19;

		// Token: 0x040002EE RID: 750
		public const int PAREXPR = 15;

		// Token: 0x040002EF RID: 751
		public const int BLOCK = 10;

		// Token: 0x040002F0 RID: 752
		public const int EAT_EOL = 103;

		// Token: 0x040002F1 RID: 753
		public const int GLOBAL = 46;

		// Token: 0x040002F2 RID: 754
		public const int EXTENDS = 39;

		// Token: 0x040002F3 RID: 755
		public const int MODEQUALS = 61;

		// Token: 0x040002F4 RID: 756
		public const int STRING = 90;

		// Token: 0x040002F5 RID: 757
		private const string DFA6_eotS = "\r￿";

		// Token: 0x040002F6 RID: 758
		private const string DFA6_eofS = "\r￿";

		// Token: 0x040002F7 RID: 759
		private const string DFA6_minS = "\u0003\u0006\u0004￿\u0001@\u0002￿\u0001@\u0002\u0006";

		// Token: 0x040002F8 RID: 760
		private const string DFA6_maxS = "\u00017\u0002?\u0004￿\u0001@\u0002￿\u0001@\u00026";

		// Token: 0x040002F9 RID: 761
		private const string DFA6_acceptS = "\u0003￿\u0001\u0002\u0001\u0003\u0001\u0004\u0001\u0005\u0001￿\u0001\u0006\u0001\u0001\u0003￿";

		// Token: 0x040002FA RID: 762
		private const string DFA6_specialS = "\r￿}>";

		// Token: 0x040002FB RID: 763
		private const string DFA14_eotS = "\u0004￿";

		// Token: 0x040002FC RID: 764
		private const string DFA14_eofS = "\u0001￿\u0001\u0002\u0002￿";

		// Token: 0x040002FD RID: 765
		private const string DFA14_minS = "\u0001^\u0001\u0006\u0002￿";

		// Token: 0x040002FE RID: 766
		private const string DFA14_maxS = "\u0002^\u0002￿";

		// Token: 0x040002FF RID: 767
		private const string DFA14_acceptS = "\u0002￿\u0001\u0002\u0001\u0001";

		// Token: 0x04000300 RID: 768
		private const string DFA14_specialS = "\u0004￿}>";

		// Token: 0x04000301 RID: 769
		private const string DFA16_eotS = "\u0017￿";

		// Token: 0x04000302 RID: 770
		private const string DFA16_eofS = "\u0001￿\u0001\u0002\t￿\u0001\u0002\v￿";

		// Token: 0x04000303 RID: 771
		private const string DFA16_minS = "\u0001^\u0001\u0006\u0001￿\u0002\u0006\u0001￿\u0002&\u0001@\u0001\u0006\u0001&\u0002\u0006\u0004&\u0002\u0006\u0001&\u0001@\u0002\u0006";

		// Token: 0x04000304 RID: 772
		private const string DFA16_maxS = "\u0002^\u0001￿\u0001^\u0001?\u0001￿\u0001]\u0001^\u0001@\u00016\u0001]\u0001^\u00016\u0005^\u0001?\u0001]\u0001@\u00026";

		// Token: 0x04000305 RID: 773
		private const string DFA16_acceptS = "\u0002￿\u0001\u0001\u0002￿\u0001\u0002\u0011￿";

		// Token: 0x04000306 RID: 774
		private const string DFA16_specialS = "\u0001\a\u0001\b\u0001￿\u0001\u000f\u0001\n\u0001￿\u0001\u0010\u0001\u0004\u0001\v\u0001\u0013\u0001\f\u0001\u0011\u0001\u0012\u0001\u0002\u0001\u0005\u0001\u0003\u0001\u000e\u0001\u0006\u0001\u0014\u0001\0\u0001\u0001\u0001\t\u0001\r}>";

		// Token: 0x04000307 RID: 775
		private const string DFA21_eotS = "\u0004￿";

		// Token: 0x04000308 RID: 776
		private const string DFA21_eofS = "\u0001￿\u0001\u0002\u0002￿";

		// Token: 0x04000309 RID: 777
		private const string DFA21_minS = "\u0001^\u0001\u0006\u0002￿";

		// Token: 0x0400030A RID: 778
		private const string DFA21_maxS = "\u0002^\u0002￿";

		// Token: 0x0400030B RID: 779
		private const string DFA21_acceptS = "\u0002￿\u0001\u0002\u0001\u0001";

		// Token: 0x0400030C RID: 780
		private const string DFA21_specialS = "\u0004￿}>";

		// Token: 0x0400030D RID: 781
		private const string DFA23_eotS = "\u0017￿";

		// Token: 0x0400030E RID: 782
		private const string DFA23_eofS = "\u0001￿\u0001\u0002\t￿\u0001\u0002\v￿";

		// Token: 0x0400030F RID: 783
		private const string DFA23_minS = "\u0001^\u0001\u0006\u0001￿\u0002\u0006\u0001￿\u0002&\u0001@\u0001\u0006\u0001&\u0002\u0006\u0004&\u0002\u0006\u0001&\u0001@\u0002\u0006";

		// Token: 0x04000310 RID: 784
		private const string DFA23_maxS = "\u0002^\u0001￿\u0001^\u0001?\u0001￿\u0001]\u0001^\u0001@\u00016\u0001]\u0001^\u00016\u0005^\u0001?\u0001]\u0001@\u00026";

		// Token: 0x04000311 RID: 785
		private const string DFA23_acceptS = "\u0002￿\u0001\u0001\u0002￿\u0001\u0002\u0011￿";

		// Token: 0x04000312 RID: 786
		private const string DFA23_specialS = "\u0001\u0013\u0001\u0001\u0001￿\u0001\u0003\u0001\b\u0001￿\u0001\u0005\u0001\v\u0001\u0004\u0001\n\u0001\u000f\u0001\t\u0001\u0011\u0001\u0006\u0001\f\u0001\a\u0001\u0012\u0001\r\u0001\u0014\u0001\0\u0001\u0002\u0001\u000e\u0001\u0010}>";

		// Token: 0x04000313 RID: 787
		private const string DFA30_eotS = "\u0013￿";

		// Token: 0x04000314 RID: 788
		private const string DFA30_eofS = "\u0013￿";

		// Token: 0x04000315 RID: 789
		private const string DFA30_minS = "\u0001&\u00026\u0001@\u0001&\u0001@\u0001&\u00016\u0001&\u00016\u0001&\u0002￿\u0001Q\u00042\u0001￿";

		// Token: 0x04000316 RID: 790
		private const string DFA30_maxS = "\u00017\u0002?\u0001@\u0001&\u0001@\u0001&\u00016\u0001^\u00016\u0001^\u0002￿\u0001]\u00048\u0001￿";

		// Token: 0x04000317 RID: 791
		private const string DFA30_acceptS = "\v￿\u0001\u0002\u0001\u0001\u0005￿\u0001\u0003";

		// Token: 0x04000318 RID: 792
		private const string DFA30_specialS = "\u0013￿}>";

		// Token: 0x04000319 RID: 793
		private const string DFA38_eotS = "\u0015￿";

		// Token: 0x0400031A RID: 794
		private const string DFA38_eofS = "\u0015￿";

		// Token: 0x0400031B RID: 795
		private const string DFA38_minS = "\u0001&\u0001\0\u0001￿\u0002\0\u0010￿";

		// Token: 0x0400031C RID: 796
		private const string DFA38_maxS = "\u0001]\u0001\0\u0001￿\u0002\0\u0010￿";

		// Token: 0x0400031D RID: 797
		private const string DFA38_acceptS = "\u0002￿\u0001\u0001\u0002￿\u0001\b\u0006￿\u0001\t\u0001\n\u0001\v\u0001\u0002\u0001\u0003\u0001\u0004\u0001\u0005\u0001\u0006\u0001\a";

		// Token: 0x0400031E RID: 798
		private const string DFA38_specialS = "\u0001\0\u0001\u0001\u0001￿\u0001\u0002\u0001\u0003\u0010￿}>";

		// Token: 0x0400031F RID: 799
		private ScriptObjectType kObjType;

		// Token: 0x04000320 RID: 800
		private string sParentObjName;

		// Token: 0x04000321 RID: 801
		private Dictionary<string, PapyrusFlag> kFlagDict;

		// Token: 0x04000322 RID: 802
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

		// Token: 0x04000323 RID: 803
		protected ITreeAdaptor adaptor = new CommonTreeAdaptor();

		// Token: 0x04000325 RID: 805
		protected StackList header_stack = new StackList();

		// Token: 0x04000326 RID: 806
		protected StackList fieldDefinition_stack = new StackList();

		// Token: 0x04000327 RID: 807
		protected StackList function_stack = new StackList();

		// Token: 0x04000328 RID: 808
		protected StackList functionHeader_stack = new StackList();

		// Token: 0x04000329 RID: 809
		protected StackList eventFunc_stack = new StackList();

		// Token: 0x0400032A RID: 810
		protected StackList eventHeader_stack = new StackList();

		// Token: 0x0400032B RID: 811
		protected StackList propertyBlock_stack = new StackList();

		// Token: 0x0400032C RID: 812
		protected StackList propertyHeader_stack = new StackList();

		// Token: 0x0400032D RID: 813
		protected StackList autoPropertyHeader_stack = new StackList();

		// Token: 0x0400032E RID: 814
		protected StackList readOnlyAutoPropertyHeader_stack = new StackList();

		// Token: 0x0400032F RID: 815
		protected StackList ifBlock_stack = new StackList();

		// Token: 0x04000330 RID: 816
		protected StackList elseIfBlock_stack = new StackList();

		// Token: 0x04000331 RID: 817
		protected StackList elseBlock_stack = new StackList();

		// Token: 0x04000332 RID: 818
		protected StackList whileBlock_stack = new StackList();

		// Token: 0x04000333 RID: 819
		protected DFA6 dfa6;

		// Token: 0x04000334 RID: 820
		protected DFA14 dfa14;

		// Token: 0x04000335 RID: 821
		protected DFA16 dfa16;

		// Token: 0x04000336 RID: 822
		protected DFA21 dfa21;

		// Token: 0x04000337 RID: 823
		protected DFA23 dfa23;

		// Token: 0x04000338 RID: 824
		protected DFA30 dfa30;

		// Token: 0x04000339 RID: 825
		protected DFA38 dfa38;

		// Token: 0x0400033A RID: 826
		private static readonly string[] DFA6_transitionS = new string[]
		{
			"\u0001\u0004\u0001\u0005\u001e￿\u0001\u0001\u0003￿\u0001\u0003\a￿\u0002\u0006\u0003￿\u0001\u0002",
			"\u0001\u0004\u001f￿\u0001\t\u000f￿\u0001\b\b￿\u0001\a",
			"\u0001\u0004\u001f￿\u0001\t\u000f￿\u0001\b\b￿\u0001\n",
			"",
			"",
			"",
			"",
			"\u0001\v",
			"",
			"",
			"\u0001\f",
			"\u0001\u0004\u001f￿\u0001\t\u000f￿\u0001\b",
			"\u0001\u0004\u001f￿\u0001\t\u000f￿\u0001\b"
		};

		// Token: 0x0400033B RID: 827
		private static readonly short[] DFA6_eot = DFA.UnpackEncodedString("\r￿");

		// Token: 0x0400033C RID: 828
		private static readonly short[] DFA6_eof = DFA.UnpackEncodedString("\r￿");

		// Token: 0x0400033D RID: 829
		private static readonly char[] DFA6_min = DFA.UnpackEncodedStringToUnsignedChars("\u0003\u0006\u0004￿\u0001@\u0002￿\u0001@\u0002\u0006");

		// Token: 0x0400033E RID: 830
		private static readonly char[] DFA6_max = DFA.UnpackEncodedStringToUnsignedChars("\u00017\u0002?\u0004￿\u0001@\u0002￿\u0001@\u00026");

		// Token: 0x0400033F RID: 831
		private static readonly short[] DFA6_accept = DFA.UnpackEncodedString("\u0003￿\u0001\u0002\u0001\u0003\u0001\u0004\u0001\u0005\u0001￿\u0001\u0006\u0001\u0001\u0003￿");

		// Token: 0x04000340 RID: 832
		private static readonly short[] DFA6_special = DFA.UnpackEncodedString("\r￿}>");

		// Token: 0x04000341 RID: 833
		private static readonly short[][] DFA6_transition = DFA.UnpackEncodedStringArray(DFA6_transitionS);

		// Token: 0x04000342 RID: 834
		private static readonly string[] DFA14_transitionS = new string[]
		{
			"\u0001\u0001",
			"\u0002\u0002\u001e￿\u0001\u0002\u0001￿\u0001\u0003\u0001￿\u0002\u0002\u0001￿\u0001\u0002\u0004￿\u0004\u0002\u0001￿\u0001\u0002\u0012￿\u0001\u0002\u0003￿\u0001\u0002\u0001￿\u0005\u0002\u0003￿\u0001\u0002\u0001￿\u0004\u0002\u0001\u0001",
			"",
			""
		};

		// Token: 0x04000343 RID: 835
		private static readonly short[] DFA14_eot = DFA.UnpackEncodedString("\u0004￿");

		// Token: 0x04000344 RID: 836
		private static readonly short[] DFA14_eof = DFA.UnpackEncodedString("\u0001￿\u0001\u0002\u0002￿");

		// Token: 0x04000345 RID: 837
		private static readonly char[] DFA14_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001^\u0001\u0006\u0002￿");

		// Token: 0x04000346 RID: 838
		private static readonly char[] DFA14_max = DFA.UnpackEncodedStringToUnsignedChars("\u0002^\u0002￿");

		// Token: 0x04000347 RID: 839
		private static readonly short[] DFA14_accept = DFA.UnpackEncodedString("\u0002￿\u0001\u0002\u0001\u0001");

		// Token: 0x04000348 RID: 840
		private static readonly short[] DFA14_special = DFA.UnpackEncodedString("\u0004￿}>");

		// Token: 0x04000349 RID: 841
		private static readonly short[][] DFA14_transition = DFA.UnpackEncodedStringArray(DFA14_transitionS);

		// Token: 0x0400034A RID: 842
		private static readonly string[] DFA16_transitionS = new string[]
		{
			"\u0001\u0001",
			"\u0002\u0002\u001e￿\u0001\u0003\u0003￿\u0001\u0002\u0001\u0005\u0001￿\u0001\u0005\u0004￿\u0004\u0002\u0001￿\u0001\u0004\u0012￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0005\u0005\u0003￿\u0001\u0005\u0001￿\u0004\u0005\u0001\u0001",
			"",
			"\u0001\u0002\u001f￿\u0001\a\u0002￿\u0001\u0005\u0001￿\u0001\u0005\n￿\u0001\u0002\u0002￿\u0006\u0005\u0001\u0006\u0001￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\u0005",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002\b￿\u0001\b",
			"",
			"\u0001\u0005\u0004￿\u0001\u0005\u0014￿\u0001\t\t￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0003\u0005\a￿\u0004\u0005",
			"\u0001\u0002\u0002￿\u0001\n4￿\u0001\v",
			"\u0001\f",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002",
			"\u0001\u0005\u0004￿\u0001\u0005\u001e￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0001\u0005\u0001\r\u0001\u0005\a￿\u0001\u000e\u0001\u000f\u0001\u0010\u0001\r",
			"\u0002\u0002\u001e￿\u0001\u0011\u0003￿\u0001\u0002\u0001\u0005\u0001￿\u0001\u0005\u0004￿\u0002\u0002\u0003￿\u0001\u0012\u0012￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0005\u0005\u0003￿\u0001\u0005\u0001￿\u0004\u0005\u0001\v",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002",
			"\u0001\u0002\u001a￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\v",
			"\u0001\u0002\u001a￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\v",
			"\u0001\u0002\u001a￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\v",
			"\u0001\u0002\u001a￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\v",
			"\u0001\u0002\u001f￿\u0001\a\u0002￿\u0001\u0005\u0001￿\u0001\u0005\n￿\u0001\u0002\u0002￿\u0006\u0005\u0001\u0013\u0001￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\u0005",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002\b￿\u0001\u0014",
			"\u0001\u0005\u0004￿\u0001\u0005\u0014￿\u0001\u0015\t￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0003\u0005\a￿\u0004\u0005",
			"\u0001\u0016",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002"
		};

		// Token: 0x0400034B RID: 843
		private static readonly short[] DFA16_eot = DFA.UnpackEncodedString("\u0017￿");

		// Token: 0x0400034C RID: 844
		private static readonly short[] DFA16_eof = DFA.UnpackEncodedString("\u0001￿\u0001\u0002\t￿\u0001\u0002\v￿");

		// Token: 0x0400034D RID: 845
		private static readonly char[] DFA16_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001^\u0001\u0006\u0001￿\u0002\u0006\u0001￿\u0002&\u0001@\u0001\u0006\u0001&\u0002\u0006\u0004&\u0002\u0006\u0001&\u0001@\u0002\u0006");

		// Token: 0x0400034E RID: 846
		private static readonly char[] DFA16_max = DFA.UnpackEncodedStringToUnsignedChars("\u0002^\u0001￿\u0001^\u0001?\u0001￿\u0001]\u0001^\u0001@\u00016\u0001]\u0001^\u00016\u0005^\u0001?\u0001]\u0001@\u00026");

		// Token: 0x0400034F RID: 847
		private static readonly short[] DFA16_accept = DFA.UnpackEncodedString("\u0002￿\u0001\u0001\u0002￿\u0001\u0002\u0011￿");

		// Token: 0x04000350 RID: 848
		private static readonly short[] DFA16_special = DFA.UnpackEncodedString("\u0001\a\u0001\b\u0001￿\u0001\u000f\u0001\n\u0001￿\u0001\u0010\u0001\u0004\u0001\v\u0001\u0013\u0001\f\u0001\u0011\u0001\u0012\u0001\u0002\u0001\u0005\u0001\u0003\u0001\u000e\u0001\u0006\u0001\u0014\u0001\0\u0001\u0001\u0001\t\u0001\r}>");

		// Token: 0x04000351 RID: 849
		private static readonly short[][] DFA16_transition = DFA.UnpackEncodedStringArray(DFA16_transitionS);

		// Token: 0x04000352 RID: 850
		private static readonly string[] DFA21_transitionS = new string[]
		{
			"\u0001\u0001",
			"\u0002\u0002\u001e￿\u0001\u0002\u0001￿\u0001\u0003\u0001￿\u0002\u0002\u0004￿\u0001\u0002\u0001￿\u0003\u0002\u0002￿\u0001\u0002\u0012￿\u0001\u0002\u0003￿\u0001\u0002\u0001￿\u0005\u0002\u0003￿\u0001\u0002\u0001￿\u0004\u0002\u0001\u0001",
			"",
			""
		};

		// Token: 0x04000353 RID: 851
		private static readonly short[] DFA21_eot = DFA.UnpackEncodedString("\u0004￿");

		// Token: 0x04000354 RID: 852
		private static readonly short[] DFA21_eof = DFA.UnpackEncodedString("\u0001￿\u0001\u0002\u0002￿");

		// Token: 0x04000355 RID: 853
		private static readonly char[] DFA21_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001^\u0001\u0006\u0002￿");

		// Token: 0x04000356 RID: 854
		private static readonly char[] DFA21_max = DFA.UnpackEncodedStringToUnsignedChars("\u0002^\u0002￿");

		// Token: 0x04000357 RID: 855
		private static readonly short[] DFA21_accept = DFA.UnpackEncodedString("\u0002￿\u0001\u0002\u0001\u0001");

		// Token: 0x04000358 RID: 856
		private static readonly short[] DFA21_special = DFA.UnpackEncodedString("\u0004￿}>");

		// Token: 0x04000359 RID: 857
		private static readonly short[][] DFA21_transition = DFA.UnpackEncodedStringArray(DFA21_transitionS);

		// Token: 0x0400035A RID: 858
		private static readonly string[] DFA23_transitionS = new string[]
		{
			"\u0001\u0001",
			"\u0002\u0002\u001e￿\u0001\u0003\u0003￿\u0001\u0002\u0001\u0005\u0004￿\u0001\u0005\u0001￿\u0003\u0002\u0002￿\u0001\u0004\u0012￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0005\u0005\u0003￿\u0001\u0005\u0001￿\u0004\u0005\u0001\u0001",
			"",
			"\u0001\u0002\u001f￿\u0001\a\u0002￿\u0001\u0005\u0001￿\u0001\u0005\n￿\u0001\u0002\u0002￿\u0006\u0005\u0001\u0006\u0001￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\u0005",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002\b￿\u0001\b",
			"",
			"\u0001\u0005\u0004￿\u0001\u0005\u0014￿\u0001\t\t￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0003\u0005\a￿\u0004\u0005",
			"\u0001\u0002\u0002￿\u0001\n4￿\u0001\v",
			"\u0001\f",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002",
			"\u0001\u0005\u0004￿\u0001\u0005\u001e￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0001\u0005\u0001\r\u0001\u0005\a￿\u0001\u000e\u0001\u000f\u0001\u0010\u0001\r",
			"\u0002\u0002\u001e￿\u0001\u0011\u0003￿\u0001\u0002\u0001\u0005\u0004￿\u0001\u0005\u0001￿\u0002\u0002\u0003￿\u0001\u0012\u0012￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0005\u0005\u0003￿\u0001\u0005\u0001￿\u0004\u0005\u0001\v",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002",
			"\u0001\u0002\u001a￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\v",
			"\u0001\u0002\u001a￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\v",
			"\u0001\u0002\u001a￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\v",
			"\u0001\u0002\u001a￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\v",
			"\u0001\u0002\u001f￿\u0001\a\u0002￿\u0001\u0005\u0001￿\u0001\u0005\n￿\u0001\u0002\u0002￿\u0006\u0005\u0001\u0013\u0001￿\r\u0005\u0001￿\u0001\u0005\u000e￿\u0001\u0005",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002\b￿\u0001\u0014",
			"\u0001\u0005\u0004￿\u0001\u0005\u0014￿\u0001\u0015\t￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0003\u0005\a￿\u0004\u0005",
			"\u0001\u0016",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002",
			"\u0001\u0002\u001f￿\u0001\a\u000f￿\u0001\u0002"
		};

		// Token: 0x0400035B RID: 859
		private static readonly short[] DFA23_eot = DFA.UnpackEncodedString("\u0017￿");

		// Token: 0x0400035C RID: 860
		private static readonly short[] DFA23_eof = DFA.UnpackEncodedString("\u0001￿\u0001\u0002\t￿\u0001\u0002\v￿");

		// Token: 0x0400035D RID: 861
		private static readonly char[] DFA23_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001^\u0001\u0006\u0001￿\u0002\u0006\u0001￿\u0002&\u0001@\u0001\u0006\u0001&\u0002\u0006\u0004&\u0002\u0006\u0001&\u0001@\u0002\u0006");

		// Token: 0x0400035E RID: 862
		private static readonly char[] DFA23_max = DFA.UnpackEncodedStringToUnsignedChars("\u0002^\u0001￿\u0001^\u0001?\u0001￿\u0001]\u0001^\u0001@\u00016\u0001]\u0001^\u00016\u0005^\u0001?\u0001]\u0001@\u00026");

		// Token: 0x0400035F RID: 863
		private static readonly short[] DFA23_accept = DFA.UnpackEncodedString("\u0002￿\u0001\u0001\u0002￿\u0001\u0002\u0011￿");

		// Token: 0x04000360 RID: 864
		private static readonly short[] DFA23_special = DFA.UnpackEncodedString("\u0001\u0013\u0001\u0001\u0001￿\u0001\u0003\u0001\b\u0001￿\u0001\u0005\u0001\v\u0001\u0004\u0001\n\u0001\u000f\u0001\t\u0001\u0011\u0001\u0006\u0001\f\u0001\a\u0001\u0012\u0001\r\u0001\u0014\u0001\0\u0001\u0002\u0001\u000e\u0001\u0010}>");

		// Token: 0x04000361 RID: 865
		private static readonly short[][] DFA23_transition = DFA.UnpackEncodedStringArray(DFA23_transitionS);

		// Token: 0x04000362 RID: 866
		private static readonly string[] DFA30_transitionS = new string[]
		{
			"\u0001\u0001\u0010￿\u0001\u0002",
			"\u0001\u0004\b￿\u0001\u0003",
			"\u0001\u0006\b￿\u0001\u0005",
			"\u0001\a",
			"\u0001\b",
			"\u0001\t",
			"\u0001\n",
			"\u0001\u0004",
			"\u0001\f\u0002￿\u0001\v\b￿\u0001\v+￿\u0001\f",
			"\u0001\u0004",
			"\u0001\f\u0002￿\u0001\r\b￿\u0001\v+￿\u0001\f",
			"",
			"",
			"\u0001\u000e\b￿\u0001\u000f\u0001\u0010\u0001\u0011\u0001\u000e",
			"\u0001\v\u0005￿\u0001\u0012",
			"\u0001\v\u0005￿\u0001\u0012",
			"\u0001\v\u0005￿\u0001\u0012",
			"\u0001\v\u0005￿\u0001\u0012",
			""
		};

		// Token: 0x04000363 RID: 867
		private static readonly short[] DFA30_eot = DFA.UnpackEncodedString("\u0013￿");

		// Token: 0x04000364 RID: 868
		private static readonly short[] DFA30_eof = DFA.UnpackEncodedString("\u0013￿");

		// Token: 0x04000365 RID: 869
		private static readonly char[] DFA30_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001&\u00026\u0001@\u0001&\u0001@\u0001&\u00016\u0001&\u00016\u0001&\u0002￿\u0001Q\u00042\u0001￿");

		// Token: 0x04000366 RID: 870
		private static readonly char[] DFA30_max = DFA.UnpackEncodedStringToUnsignedChars("\u00017\u0002?\u0001@\u0001&\u0001@\u0001&\u00016\u0001^\u00016\u0001^\u0002￿\u0001]\u00048\u0001￿");

		// Token: 0x04000367 RID: 871
		private static readonly short[] DFA30_accept = DFA.UnpackEncodedString("\v￿\u0001\u0002\u0001\u0001\u0005￿\u0001\u0003");

		// Token: 0x04000368 RID: 872
		private static readonly short[] DFA30_special = DFA.UnpackEncodedString("\u0013￿}>");

		// Token: 0x04000369 RID: 873
		private static readonly short[][] DFA30_transition = DFA.UnpackEncodedStringArray(DFA30_transitionS);

		// Token: 0x0400036A RID: 874
		private static readonly string[] DFA38_transitionS = new string[]
		{
			"\u0001\u0001\u0004￿\u0001\u0003\v￿\u0001\u0002\u0012￿\u0001\u0005\u0003￿\u0001\u0005\u0001￿\u0002\u0005\u0001\u0004\u0001\f\u0001\r\u0003￿\u0001\u000e\u0001￿\u0004\u0005",
			"\u0001￿",
			"",
			"\u0001￿",
			"\u0001￿",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			""
		};

		// Token: 0x0400036B RID: 875
		private static readonly short[] DFA38_eot = DFA.UnpackEncodedString("\u0015￿");

		// Token: 0x0400036C RID: 876
		private static readonly short[] DFA38_eof = DFA.UnpackEncodedString("\u0015￿");

		// Token: 0x0400036D RID: 877
		private static readonly char[] DFA38_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001&\u0001\0\u0001￿\u0002\0\u0010￿");

		// Token: 0x0400036E RID: 878
		private static readonly char[] DFA38_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001]\u0001\0\u0001￿\u0002\0\u0010￿");

		// Token: 0x0400036F RID: 879
		private static readonly short[] DFA38_accept = DFA.UnpackEncodedString("\u0002￿\u0001\u0001\u0002￿\u0001\b\u0006￿\u0001\t\u0001\n\u0001\v\u0001\u0002\u0001\u0003\u0001\u0004\u0001\u0005\u0001\u0006\u0001\a");

		// Token: 0x04000370 RID: 880
		private static readonly short[] DFA38_special = DFA.UnpackEncodedString("\u0001\0\u0001\u0001\u0001￿\u0001\u0002\u0001\u0003\u0010￿}>");

		// Token: 0x04000371 RID: 881
		private static readonly short[][] DFA38_transition = DFA.UnpackEncodedStringArray(DFA38_transitionS);

		// Token: 0x04000372 RID: 882
		public static readonly BitSet FOLLOW_terminator_in_script266 = new BitSet(new ulong[]
		{
			137438953472UL
		});

		// Token: 0x04000373 RID: 883
		public static readonly BitSet FOLLOW_header_in_script269 = new BitSet(new ulong[]
		{
			39411169663910080UL
		});

		// Token: 0x04000374 RID: 884
		public static readonly BitSet FOLLOW_definitionOrBlock_in_script271 = new BitSet(new ulong[]
		{
			39411169663910080UL
		});

		// Token: 0x04000375 RID: 885
		public static readonly BitSet FOLLOW_EOF_in_script274 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000376 RID: 886
		public static readonly BitSet FOLLOW_SCRIPTNAME_in_header308 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000377 RID: 887
		public static readonly BitSet FOLLOW_ID_in_header312 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x04000378 RID: 888
		public static readonly BitSet FOLLOW_EXTENDS_in_header315 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000379 RID: 889
		public static readonly BitSet FOLLOW_ID_in_header319 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400037A RID: 890
		public static readonly BitSet FOLLOW_userFlags_in_header323 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400037B RID: 891
		public static readonly BitSet FOLLOW_terminator_in_header326 = new BitSet(new ulong[]
		{
			1099511627778UL
		});

		// Token: 0x0400037C RID: 892
		public static readonly BitSet FOLLOW_DOCSTRING_in_header329 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400037D RID: 893
		public static readonly BitSet FOLLOW_terminator_in_header331 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400037E RID: 894
		public static readonly BitSet FOLLOW_fieldDefinition_in_definitionOrBlock370 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400037F RID: 895
		public static readonly BitSet FOLLOW_import_obj_in_definitionOrBlock381 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000380 RID: 896
		public static readonly BitSet FOLLOW_function_in_definitionOrBlock387 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000381 RID: 897
		public static readonly BitSet FOLLOW_eventFunc_in_definitionOrBlock400 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000382 RID: 898
		public static readonly BitSet FOLLOW_stateBlock_in_definitionOrBlock412 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000383 RID: 899
		public static readonly BitSet FOLLOW_propertyBlock_in_definitionOrBlock418 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000384 RID: 900
		public static readonly BitSet FOLLOW_ID_in_userFlags441 = new BitSet(new ulong[]
		{
			274877906946UL
		});

		// Token: 0x04000385 RID: 901
		public static readonly BitSet FOLLOW_ID_in_userFlags460 = new BitSet(new ulong[]
		{
			274877906946UL
		});

		// Token: 0x04000386 RID: 902
		public static readonly BitSet FOLLOW_type_in_fieldDefinition515 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000387 RID: 903
		public static readonly BitSet FOLLOW_ID_in_fieldDefinition517 = new BitSet(new ulong[]
		{
			3023656976384UL,
			1073741824UL
		});

		// Token: 0x04000388 RID: 904
		public static readonly BitSet FOLLOW_EQUALS_in_fieldDefinition520 = new BitSet(new ulong[]
		{
			0UL,
			1006764032UL
		});

		// Token: 0x04000389 RID: 905
		public static readonly BitSet FOLLOW_constant_in_fieldDefinition522 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400038A RID: 906
		public static readonly BitSet FOLLOW_userFlags_in_fieldDefinition526 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400038B RID: 907
		public static readonly BitSet FOLLOW_terminator_in_fieldDefinition529 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400038C RID: 908
		public static readonly BitSet FOLLOW_IMPORT_in_import_obj568 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x0400038D RID: 909
		public static readonly BitSet FOLLOW_ID_in_import_obj571 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400038E RID: 910
		public static readonly BitSet FOLLOW_terminator_in_import_obj573 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400038F RID: 911
		public static readonly BitSet FOLLOW_functionHeader_in_function606 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x04000390 RID: 912
		public static readonly BitSet FOLLOW_functionBlock_in_function608 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000391 RID: 913
		public static readonly BitSet FOLLOW_type_in_functionHeader645 = new BitSet(new ulong[]
		{
			64UL
		});

		// Token: 0x04000392 RID: 914
		public static readonly BitSet FOLLOW_FUNCTION_in_functionHeader648 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000393 RID: 915
		public static readonly BitSet FOLLOW_ID_in_functionHeader652 = new BitSet(new ulong[]
		{
			8796093022208UL
		});

		// Token: 0x04000394 RID: 916
		public static readonly BitSet FOLLOW_LPAREN_in_functionHeader654 = new BitSet(new ulong[]
		{
			36046664082915328UL
		});

		// Token: 0x04000395 RID: 917
		public static readonly BitSet FOLLOW_callParameters_in_functionHeader656 = new BitSet(new ulong[]
		{
			17592186044416UL
		});

		// Token: 0x04000396 RID: 918
		public static readonly BitSet FOLLOW_RPAREN_in_functionHeader660 = new BitSet(new ulong[]
		{
			211930866253826UL,
			1073741824UL
		});

		// Token: 0x04000397 RID: 919
		public static readonly BitSet FOLLOW_functionModifier_in_functionHeader662 = new BitSet(new ulong[]
		{
			211930866253826UL,
			1073741824UL
		});

		// Token: 0x04000398 RID: 920
		public static readonly BitSet FOLLOW_userFlags_in_functionHeader665 = new BitSet(new ulong[]
		{
			824633720834UL,
			1073741824UL
		});

		// Token: 0x04000399 RID: 921
		public static readonly BitSet FOLLOW_terminator_in_functionHeader669 = new BitSet(new ulong[]
		{
			1099511627776UL
		});

		// Token: 0x0400039A RID: 922
		public static readonly BitSet FOLLOW_DOCSTRING_in_functionHeader671 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400039B RID: 923
		public static readonly BitSet FOLLOW_terminator_in_functionBlock748 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400039C RID: 924
		public static readonly BitSet FOLLOW_terminator_in_functionBlock763 = new BitSet(new ulong[]
		{
			36073052361981952UL,
			1025459200UL
		});

		// Token: 0x0400039D RID: 925
		public static readonly BitSet FOLLOW_statement_in_functionBlock765 = new BitSet(new ulong[]
		{
			36073052361981952UL,
			1025459200UL
		});

		// Token: 0x0400039E RID: 926
		public static readonly BitSet FOLLOW_ENDFUNCTION_in_functionBlock769 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400039F RID: 927
		public static readonly BitSet FOLLOW_terminator_in_functionBlock771 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003A0 RID: 928
		public static readonly BitSet FOLLOW_GLOBAL_in_functionModifier798 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003A1 RID: 929
		public static readonly BitSet FOLLOW_NATIVE_in_functionModifier809 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003A2 RID: 930
		public static readonly BitSet FOLLOW_eventHeader_in_eventFunc846 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003A3 RID: 931
		public static readonly BitSet FOLLOW_eventBlock_in_eventFunc848 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003A4 RID: 932
		public static readonly BitSet FOLLOW_EVENT_in_eventHeader884 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040003A5 RID: 933
		public static readonly BitSet FOLLOW_ID_in_eventHeader886 = new BitSet(new ulong[]
		{
			8796093022208UL
		});

		// Token: 0x040003A6 RID: 934
		public static readonly BitSet FOLLOW_LPAREN_in_eventHeader888 = new BitSet(new ulong[]
		{
			36046664082915328UL
		});

		// Token: 0x040003A7 RID: 935
		public static readonly BitSet FOLLOW_callParameters_in_eventHeader890 = new BitSet(new ulong[]
		{
			17592186044416UL
		});

		// Token: 0x040003A8 RID: 936
		public static readonly BitSet FOLLOW_RPAREN_in_eventHeader894 = new BitSet(new ulong[]
		{
			141562122076162UL,
			1073741824UL
		});

		// Token: 0x040003A9 RID: 937
		public static readonly BitSet FOLLOW_NATIVE_in_eventHeader896 = new BitSet(new ulong[]
		{
			824633720834UL,
			1073741824UL
		});

		// Token: 0x040003AA RID: 938
		public static readonly BitSet FOLLOW_userFlags_in_eventHeader899 = new BitSet(new ulong[]
		{
			824633720834UL,
			1073741824UL
		});

		// Token: 0x040003AB RID: 939
		public static readonly BitSet FOLLOW_terminator_in_eventHeader903 = new BitSet(new ulong[]
		{
			1099511627776UL
		});

		// Token: 0x040003AC RID: 940
		public static readonly BitSet FOLLOW_DOCSTRING_in_eventHeader905 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003AD RID: 941
		public static readonly BitSet FOLLOW_terminator_in_eventBlock953 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003AE RID: 942
		public static readonly BitSet FOLLOW_terminator_in_eventBlock968 = new BitSet(new ulong[]
		{
			36319342966603776UL,
			1025459200UL
		});

		// Token: 0x040003AF RID: 943
		public static readonly BitSet FOLLOW_statement_in_eventBlock970 = new BitSet(new ulong[]
		{
			36319342966603776UL,
			1025459200UL
		});

		// Token: 0x040003B0 RID: 944
		public static readonly BitSet FOLLOW_ENDEVENT_in_eventBlock974 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003B1 RID: 945
		public static readonly BitSet FOLLOW_terminator_in_eventBlock976 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003B2 RID: 946
		public static readonly BitSet FOLLOW_callParameter_in_callParameters1002 = new BitSet(new ulong[]
		{
			562949953421314UL
		});

		// Token: 0x040003B3 RID: 947
		public static readonly BitSet FOLLOW_COMMA_in_callParameters1006 = new BitSet(new ulong[]
		{
			36029071896870912UL
		});

		// Token: 0x040003B4 RID: 948
		public static readonly BitSet FOLLOW_callParameter_in_callParameters1008 = new BitSet(new ulong[]
		{
			562949953421314UL
		});

		// Token: 0x040003B5 RID: 949
		public static readonly BitSet FOLLOW_type_in_callParameter1032 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040003B6 RID: 950
		public static readonly BitSet FOLLOW_ID_in_callParameter1036 = new BitSet(new ulong[]
		{
			2199023255554UL
		});

		// Token: 0x040003B7 RID: 951
		public static readonly BitSet FOLLOW_EQUALS_in_callParameter1039 = new BitSet(new ulong[]
		{
			0UL,
			1006764032UL
		});

		// Token: 0x040003B8 RID: 952
		public static readonly BitSet FOLLOW_constant_in_callParameter1041 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003B9 RID: 953
		public static readonly BitSet FOLLOW_AUTO_in_stateBlock1079 = new BitSet(new ulong[]
		{
			2251799813685248UL
		});

		// Token: 0x040003BA RID: 954
		public static readonly BitSet FOLLOW_STATE_in_stateBlock1082 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040003BB RID: 955
		public static readonly BitSet FOLLOW_ID_in_stateBlock1084 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003BC RID: 956
		public static readonly BitSet FOLLOW_terminator_in_stateBlock1086 = new BitSet(new ulong[]
		{
			40532671524241600UL
		});

		// Token: 0x040003BD RID: 957
		public static readonly BitSet FOLLOW_stateFuncOrEvent_in_stateBlock1089 = new BitSet(new ulong[]
		{
			40532671524241600UL
		});

		// Token: 0x040003BE RID: 958
		public static readonly BitSet FOLLOW_ENDSTATE_in_stateBlock1094 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003BF RID: 959
		public static readonly BitSet FOLLOW_terminator_in_stateBlock1096 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003C0 RID: 960
		public static readonly BitSet FOLLOW_function_in_stateFuncOrEvent1131 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003C1 RID: 961
		public static readonly BitSet FOLLOW_eventFunc_in_stateFuncOrEvent1144 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003C2 RID: 962
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock1172 = new BitSet(new ulong[]
		{
			36029071896870976UL
		});

		// Token: 0x040003C3 RID: 963
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock1176 = new BitSet(new ulong[]
		{
			45036271151611968UL
		});

		// Token: 0x040003C4 RID: 964
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock1181 = new BitSet(new ulong[]
		{
			9007199254740992UL
		});

		// Token: 0x040003C5 RID: 965
		public static readonly BitSet FOLLOW_ENDPROPERTY_in_propertyBlock1185 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003C6 RID: 966
		public static readonly BitSet FOLLOW_terminator_in_propertyBlock1187 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003C7 RID: 967
		public static readonly BitSet FOLLOW_autoPropertyHeader_in_propertyBlock1216 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003C8 RID: 968
		public static readonly BitSet FOLLOW_readOnlyAutoPropertyHeader_in_propertyBlock1229 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003C9 RID: 969
		public static readonly BitSet FOLLOW_type_in_propertyHeader1255 = new BitSet(new ulong[]
		{
			18014398509481984UL
		});

		// Token: 0x040003CA RID: 970
		public static readonly BitSet FOLLOW_PROPERTY_in_propertyHeader1257 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040003CB RID: 971
		public static readonly BitSet FOLLOW_ID_in_propertyHeader1261 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003CC RID: 972
		public static readonly BitSet FOLLOW_userFlags_in_propertyHeader1263 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003CD RID: 973
		public static readonly BitSet FOLLOW_terminator_in_propertyHeader1266 = new BitSet(new ulong[]
		{
			1099511627778UL
		});

		// Token: 0x040003CE RID: 974
		public static readonly BitSet FOLLOW_DOCSTRING_in_propertyHeader1269 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003CF RID: 975
		public static readonly BitSet FOLLOW_terminator_in_propertyHeader1271 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003D0 RID: 976
		public static readonly BitSet FOLLOW_type_in_autoPropertyHeader1322 = new BitSet(new ulong[]
		{
			18014398509481984UL
		});

		// Token: 0x040003D1 RID: 977
		public static readonly BitSet FOLLOW_PROPERTY_in_autoPropertyHeader1324 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040003D2 RID: 978
		public static readonly BitSet FOLLOW_ID_in_autoPropertyHeader1328 = new BitSet(new ulong[]
		{
			1128098930098176UL
		});

		// Token: 0x040003D3 RID: 979
		public static readonly BitSet FOLLOW_EQUALS_in_autoPropertyHeader1331 = new BitSet(new ulong[]
		{
			0UL,
			1006764032UL
		});

		// Token: 0x040003D4 RID: 980
		public static readonly BitSet FOLLOW_constant_in_autoPropertyHeader1333 = new BitSet(new ulong[]
		{
			1125899906842624UL
		});

		// Token: 0x040003D5 RID: 981
		public static readonly BitSet FOLLOW_AUTO_in_autoPropertyHeader1337 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003D6 RID: 982
		public static readonly BitSet FOLLOW_userFlags_in_autoPropertyHeader1339 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003D7 RID: 983
		public static readonly BitSet FOLLOW_terminator_in_autoPropertyHeader1342 = new BitSet(new ulong[]
		{
			1099511627778UL
		});

		// Token: 0x040003D8 RID: 984
		public static readonly BitSet FOLLOW_DOCSTRING_in_autoPropertyHeader1345 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003D9 RID: 985
		public static readonly BitSet FOLLOW_terminator_in_autoPropertyHeader1347 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003DA RID: 986
		public static readonly BitSet FOLLOW_BASETYPE_in_readOnlyAutoPropertyHeader1396 = new BitSet(new ulong[]
		{
			18014398509481984UL
		});

		// Token: 0x040003DB RID: 987
		public static readonly BitSet FOLLOW_PROPERTY_in_readOnlyAutoPropertyHeader1398 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040003DC RID: 988
		public static readonly BitSet FOLLOW_ID_in_readOnlyAutoPropertyHeader1400 = new BitSet(new ulong[]
		{
			2199023255552UL
		});

		// Token: 0x040003DD RID: 989
		public static readonly BitSet FOLLOW_EQUALS_in_readOnlyAutoPropertyHeader1402 = new BitSet(new ulong[]
		{
			0UL,
			1006764032UL
		});

		// Token: 0x040003DE RID: 990
		public static readonly BitSet FOLLOW_constant_in_readOnlyAutoPropertyHeader1404 = new BitSet(new ulong[]
		{
			72057594037927936UL
		});

		// Token: 0x040003DF RID: 991
		public static readonly BitSet FOLLOW_AUTOREADONLY_in_readOnlyAutoPropertyHeader1406 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003E0 RID: 992
		public static readonly BitSet FOLLOW_userFlags_in_readOnlyAutoPropertyHeader1408 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003E1 RID: 993
		public static readonly BitSet FOLLOW_terminator_in_readOnlyAutoPropertyHeader1411 = new BitSet(new ulong[]
		{
			1099511627778UL
		});

		// Token: 0x040003E2 RID: 994
		public static readonly BitSet FOLLOW_DOCSTRING_in_readOnlyAutoPropertyHeader1414 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003E3 RID: 995
		public static readonly BitSet FOLLOW_terminator_in_readOnlyAutoPropertyHeader1416 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003E4 RID: 996
		public static readonly BitSet FOLLOW_function_in_propertyFunc1461 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003E5 RID: 997
		public static readonly BitSet FOLLOW_localDefinition_in_statement1504 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003E6 RID: 998
		public static readonly BitSet FOLLOW_l_value_in_statement1522 = new BitSet(new ulong[]
		{
			144115188075855872UL
		});

		// Token: 0x040003E7 RID: 999
		public static readonly BitSet FOLLOW_PLUSEQUALS_in_statement1524 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x040003E8 RID: 1000
		public static readonly BitSet FOLLOW_expression_in_statement1526 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003E9 RID: 1001
		public static readonly BitSet FOLLOW_terminator_in_statement1528 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003EA RID: 1002
		public static readonly BitSet FOLLOW_l_value_in_statement1573 = new BitSet(new ulong[]
		{
			288230376151711744UL
		});

		// Token: 0x040003EB RID: 1003
		public static readonly BitSet FOLLOW_MINUSEQUALS_in_statement1575 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x040003EC RID: 1004
		public static readonly BitSet FOLLOW_expression_in_statement1577 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003ED RID: 1005
		public static readonly BitSet FOLLOW_terminator_in_statement1579 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003EE RID: 1006
		public static readonly BitSet FOLLOW_l_value_in_statement1623 = new BitSet(new ulong[]
		{
			576460752303423488UL
		});

		// Token: 0x040003EF RID: 1007
		public static readonly BitSet FOLLOW_MULTEQUALS_in_statement1625 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x040003F0 RID: 1008
		public static readonly BitSet FOLLOW_expression_in_statement1627 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003F1 RID: 1009
		public static readonly BitSet FOLLOW_terminator_in_statement1629 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003F2 RID: 1010
		public static readonly BitSet FOLLOW_l_value_in_statement1673 = new BitSet(new ulong[]
		{
			1152921504606846976UL
		});

		// Token: 0x040003F3 RID: 1011
		public static readonly BitSet FOLLOW_DIVEQUALS_in_statement1675 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x040003F4 RID: 1012
		public static readonly BitSet FOLLOW_expression_in_statement1677 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003F5 RID: 1013
		public static readonly BitSet FOLLOW_terminator_in_statement1679 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003F6 RID: 1014
		public static readonly BitSet FOLLOW_l_value_in_statement1723 = new BitSet(new ulong[]
		{
			2305843009213693952UL
		});

		// Token: 0x040003F7 RID: 1015
		public static readonly BitSet FOLLOW_MODEQUALS_in_statement1725 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x040003F8 RID: 1016
		public static readonly BitSet FOLLOW_expression_in_statement1727 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003F9 RID: 1017
		public static readonly BitSet FOLLOW_terminator_in_statement1729 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003FA RID: 1018
		public static readonly BitSet FOLLOW_l_value_in_statement1773 = new BitSet(new ulong[]
		{
			2199023255552UL
		});

		// Token: 0x040003FB RID: 1019
		public static readonly BitSet FOLLOW_EQUALS_in_statement1775 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x040003FC RID: 1020
		public static readonly BitSet FOLLOW_expression_in_statement1778 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003FD RID: 1021
		public static readonly BitSet FOLLOW_terminator_in_statement1780 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040003FE RID: 1022
		public static readonly BitSet FOLLOW_expression_in_statement1788 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x040003FF RID: 1023
		public static readonly BitSet FOLLOW_terminator_in_statement1790 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000400 RID: 1024
		public static readonly BitSet FOLLOW_return_stat_in_statement1798 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000401 RID: 1025
		public static readonly BitSet FOLLOW_ifBlock_in_statement1804 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000402 RID: 1026
		public static readonly BitSet FOLLOW_whileBlock_in_statement1811 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000403 RID: 1027
		public static readonly BitSet FOLLOW_LPAREN_in_l_value1833 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000404 RID: 1028
		public static readonly BitSet FOLLOW_expression_in_l_value1835 = new BitSet(new ulong[]
		{
			17592186044416UL
		});

		// Token: 0x04000405 RID: 1029
		public static readonly BitSet FOLLOW_RPAREN_in_l_value1837 = new BitSet(new ulong[]
		{
			4611686018427387904UL
		});

		// Token: 0x04000406 RID: 1030
		public static readonly BitSet FOLLOW_DOT_in_l_value1839 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000407 RID: 1031
		public static readonly BitSet FOLLOW_ID_in_l_value1841 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000408 RID: 1032
		public static readonly BitSet FOLLOW_LPAREN_in_l_value1877 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000409 RID: 1033
		public static readonly BitSet FOLLOW_expression_in_l_value1881 = new BitSet(new ulong[]
		{
			17592186044416UL
		});

		// Token: 0x0400040A RID: 1034
		public static readonly BitSet FOLLOW_RPAREN_in_l_value1883 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x0400040B RID: 1035
		public static readonly BitSet FOLLOW_LBRACKET_in_l_value1885 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400040C RID: 1036
		public static readonly BitSet FOLLOW_expression_in_l_value1889 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x0400040D RID: 1037
		public static readonly BitSet FOLLOW_RBRACKET_in_l_value1891 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400040E RID: 1038
		public static readonly BitSet FOLLOW_basic_l_value_in_l_value1918 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400040F RID: 1039
		public static readonly BitSet FOLLOW_array_func_or_id_in_basic_l_value1938 = new BitSet(new ulong[]
		{
			4611686018427387904UL
		});

		// Token: 0x04000410 RID: 1040
		public static readonly BitSet FOLLOW_DOT_in_basic_l_value1940 = new BitSet(new ulong[]
		{
			9070970929152UL,
			262144UL
		});

		// Token: 0x04000411 RID: 1041
		public static readonly BitSet FOLLOW_basic_l_value_in_basic_l_value1943 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000412 RID: 1042
		public static readonly BitSet FOLLOW_func_or_id_in_basic_l_value1956 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000413 RID: 1043
		public static readonly BitSet FOLLOW_LBRACKET_in_basic_l_value1958 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000414 RID: 1044
		public static readonly BitSet FOLLOW_expression_in_basic_l_value1960 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000415 RID: 1045
		public static readonly BitSet FOLLOW_RBRACKET_in_basic_l_value1962 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000416 RID: 1046
		public static readonly BitSet FOLLOW_ID_in_basic_l_value1982 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000417 RID: 1047
		public static readonly BitSet FOLLOW_type_in_localDefinition2004 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000418 RID: 1048
		public static readonly BitSet FOLLOW_ID_in_localDefinition2008 = new BitSet(new ulong[]
		{
			3023656976384UL,
			1073741824UL
		});

		// Token: 0x04000419 RID: 1049
		public static readonly BitSet FOLLOW_EQUALS_in_localDefinition2011 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400041A RID: 1050
		public static readonly BitSet FOLLOW_expression_in_localDefinition2013 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400041B RID: 1051
		public static readonly BitSet FOLLOW_terminator_in_localDefinition2017 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400041C RID: 1052
		public static readonly BitSet FOLLOW_and_expression_in_expression2053 = new BitSet(new ulong[]
		{
			2UL,
			2UL
		});

		// Token: 0x0400041D RID: 1053
		public static readonly BitSet FOLLOW_OR_in_expression2056 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400041E RID: 1054
		public static readonly BitSet FOLLOW_and_expression_in_expression2059 = new BitSet(new ulong[]
		{
			2UL,
			2UL
		});

		// Token: 0x0400041F RID: 1055
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression2073 = new BitSet(new ulong[]
		{
			2UL,
			4UL
		});

		// Token: 0x04000420 RID: 1056
		public static readonly BitSet FOLLOW_AND_in_and_expression2076 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000421 RID: 1057
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression2079 = new BitSet(new ulong[]
		{
			2UL,
			4UL
		});

		// Token: 0x04000422 RID: 1058
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2093 = new BitSet(new ulong[]
		{
			2UL,
			504UL
		});

		// Token: 0x04000423 RID: 1059
		public static readonly BitSet FOLLOW_set_in_bool_expression2096 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000424 RID: 1060
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2121 = new BitSet(new ulong[]
		{
			2UL,
			504UL
		});

		// Token: 0x04000425 RID: 1061
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2135 = new BitSet(new ulong[]
		{
			2UL,
			1536UL
		});

		// Token: 0x04000426 RID: 1062
		public static readonly BitSet FOLLOW_set_in_add_expression2138 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000427 RID: 1063
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2147 = new BitSet(new ulong[]
		{
			2UL,
			1536UL
		});

		// Token: 0x04000428 RID: 1064
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2161 = new BitSet(new ulong[]
		{
			2UL,
			14336UL
		});

		// Token: 0x04000429 RID: 1065
		public static readonly BitSet FOLLOW_set_in_mult_expression2164 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400042A RID: 1066
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2177 = new BitSet(new ulong[]
		{
			2UL,
			14336UL
		});

		// Token: 0x0400042B RID: 1067
		public static readonly BitSet FOLLOW_MINUS_in_unary_expression2191 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400042C RID: 1068
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2193 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400042D RID: 1069
		public static readonly BitSet FOLLOW_NOT_in_unary_expression2211 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400042E RID: 1070
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2214 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400042F RID: 1071
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2220 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000430 RID: 1072
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom2232 = new BitSet(new ulong[]
		{
			2UL,
			32768UL
		});

		// Token: 0x04000431 RID: 1073
		public static readonly BitSet FOLLOW_AS_in_cast_atom2235 = new BitSet(new ulong[]
		{
			36029071896870912UL
		});

		// Token: 0x04000432 RID: 1074
		public static readonly BitSet FOLLOW_type_in_cast_atom2238 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000433 RID: 1075
		public static readonly BitSet FOLLOW_array_atom_in_dot_atom2252 = new BitSet(new ulong[]
		{
			4611686018427387906UL
		});

		// Token: 0x04000434 RID: 1076
		public static readonly BitSet FOLLOW_DOT_in_dot_atom2255 = new BitSet(new ulong[]
		{
			274877906944UL,
			262144UL
		});

		// Token: 0x04000435 RID: 1077
		public static readonly BitSet FOLLOW_array_func_or_id_in_dot_atom2258 = new BitSet(new ulong[]
		{
			4611686018427387906UL
		});

		// Token: 0x04000436 RID: 1078
		public static readonly BitSet FOLLOW_constant_in_dot_atom2266 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000437 RID: 1079
		public static readonly BitSet FOLLOW_atom_in_array_atom2285 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000438 RID: 1080
		public static readonly BitSet FOLLOW_LBRACKET_in_array_atom2287 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000439 RID: 1081
		public static readonly BitSet FOLLOW_expression_in_array_atom2289 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x0400043A RID: 1082
		public static readonly BitSet FOLLOW_RBRACKET_in_array_atom2291 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400043B RID: 1083
		public static readonly BitSet FOLLOW_atom_in_array_atom2311 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400043C RID: 1084
		public static readonly BitSet FOLLOW_LPAREN_in_atom2321 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400043D RID: 1085
		public static readonly BitSet FOLLOW_expression_in_atom2323 = new BitSet(new ulong[]
		{
			17592186044416UL
		});

		// Token: 0x0400043E RID: 1086
		public static readonly BitSet FOLLOW_RPAREN_in_atom2325 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400043F RID: 1087
		public static readonly BitSet FOLLOW_NEW_in_atom2343 = new BitSet(new ulong[]
		{
			36029071896870912UL
		});

		// Token: 0x04000440 RID: 1088
		public static readonly BitSet FOLLOW_BASETYPE_in_atom2348 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000441 RID: 1089
		public static readonly BitSet FOLLOW_ID_in_atom2354 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000442 RID: 1090
		public static readonly BitSet FOLLOW_LBRACKET_in_atom2357 = new BitSet(new ulong[]
		{
			0UL,
			131072UL
		});

		// Token: 0x04000443 RID: 1091
		public static readonly BitSet FOLLOW_INTEGER_in_atom2359 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000444 RID: 1092
		public static readonly BitSet FOLLOW_RBRACKET_in_atom2361 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000445 RID: 1093
		public static readonly BitSet FOLLOW_func_or_id_in_atom2386 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000446 RID: 1094
		public static readonly BitSet FOLLOW_function_call_in_array_func_or_id2405 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000447 RID: 1095
		public static readonly BitSet FOLLOW_LBRACKET_in_array_func_or_id2407 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000448 RID: 1096
		public static readonly BitSet FOLLOW_expression_in_array_func_or_id2409 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000449 RID: 1097
		public static readonly BitSet FOLLOW_RBRACKET_in_array_func_or_id2411 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400044A RID: 1098
		public static readonly BitSet FOLLOW_ID_in_array_func_or_id2438 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x0400044B RID: 1099
		public static readonly BitSet FOLLOW_LBRACKET_in_array_func_or_id2440 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400044C RID: 1100
		public static readonly BitSet FOLLOW_expression_in_array_func_or_id2442 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x0400044D RID: 1101
		public static readonly BitSet FOLLOW_RBRACKET_in_array_func_or_id2444 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400044E RID: 1102
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id2464 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400044F RID: 1103
		public static readonly BitSet FOLLOW_function_call_in_func_or_id2476 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000450 RID: 1104
		public static readonly BitSet FOLLOW_ID_in_func_or_id2482 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000451 RID: 1105
		public static readonly BitSet FOLLOW_LENGTH_in_func_or_id2488 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000452 RID: 1106
		public static readonly BitSet FOLLOW_RETURN_in_return_stat2501 = new BitSet(new ulong[]
		{
			9620726743040UL,
			2080850944UL
		});

		// Token: 0x04000453 RID: 1107
		public static readonly BitSet FOLLOW_expression_in_return_stat2504 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x04000454 RID: 1108
		public static readonly BitSet FOLLOW_terminator_in_return_stat2507 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000455 RID: 1109
		public static readonly BitSet FOLLOW_IF_in_ifBlock2531 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000456 RID: 1110
		public static readonly BitSet FOLLOW_expression_in_ifBlock2533 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x04000457 RID: 1111
		public static readonly BitSet FOLLOW_terminator_in_ifBlock2535 = new BitSet(new ulong[]
		{
			36037867989893120UL,
			1040139264UL
		});

		// Token: 0x04000458 RID: 1112
		public static readonly BitSet FOLLOW_statement_in_ifBlock2537 = new BitSet(new ulong[]
		{
			36037867989893120UL,
			1040139264UL
		});

		// Token: 0x04000459 RID: 1113
		public static readonly BitSet FOLLOW_elseIfBlock_in_ifBlock2541 = new BitSet(new ulong[]
		{
			0UL,
			14680064UL
		});

		// Token: 0x0400045A RID: 1114
		public static readonly BitSet FOLLOW_elseBlock_in_ifBlock2545 = new BitSet(new ulong[]
		{
			0UL,
			2097152UL
		});

		// Token: 0x0400045B RID: 1115
		public static readonly BitSet FOLLOW_ENDIF_in_ifBlock2549 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400045C RID: 1116
		public static readonly BitSet FOLLOW_terminator_in_ifBlock2551 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400045D RID: 1117
		public static readonly BitSet FOLLOW_ELSEIF_in_elseIfBlock2597 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x0400045E RID: 1118
		public static readonly BitSet FOLLOW_expression_in_elseIfBlock2599 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x0400045F RID: 1119
		public static readonly BitSet FOLLOW_terminator_in_elseIfBlock2601 = new BitSet(new ulong[]
		{
			36037867989893122UL,
			1025459200UL
		});

		// Token: 0x04000460 RID: 1120
		public static readonly BitSet FOLLOW_statement_in_elseIfBlock2603 = new BitSet(new ulong[]
		{
			36037867989893122UL,
			1025459200UL
		});

		// Token: 0x04000461 RID: 1121
		public static readonly BitSet FOLLOW_ELSE_in_elseBlock2645 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x04000462 RID: 1122
		public static readonly BitSet FOLLOW_terminator_in_elseBlock2647 = new BitSet(new ulong[]
		{
			36037867989893122UL,
			1025459200UL
		});

		// Token: 0x04000463 RID: 1123
		public static readonly BitSet FOLLOW_statement_in_elseBlock2649 = new BitSet(new ulong[]
		{
			36037867989893122UL,
			1025459200UL
		});

		// Token: 0x04000464 RID: 1124
		public static readonly BitSet FOLLOW_WHILE_in_whileBlock2690 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000465 RID: 1125
		public static readonly BitSet FOLLOW_expression_in_whileBlock2692 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x04000466 RID: 1126
		public static readonly BitSet FOLLOW_terminator_in_whileBlock2694 = new BitSet(new ulong[]
		{
			36037867989893120UL,
			1059013632UL
		});

		// Token: 0x04000467 RID: 1127
		public static readonly BitSet FOLLOW_statement_in_whileBlock2696 = new BitSet(new ulong[]
		{
			36037867989893120UL,
			1059013632UL
		});

		// Token: 0x04000468 RID: 1128
		public static readonly BitSet FOLLOW_ENDWHILE_in_whileBlock2700 = new BitSet(new ulong[]
		{
			824633720832UL,
			1073741824UL
		});

		// Token: 0x04000469 RID: 1129
		public static readonly BitSet FOLLOW_terminator_in_whileBlock2702 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400046A RID: 1130
		public static readonly BitSet FOLLOW_ID_in_function_call2733 = new BitSet(new ulong[]
		{
			8796093022208UL
		});

		// Token: 0x0400046B RID: 1131
		public static readonly BitSet FOLLOW_LPAREN_in_function_call2735 = new BitSet(new ulong[]
		{
			26663156973568UL,
			1007109120UL
		});

		// Token: 0x0400046C RID: 1132
		public static readonly BitSet FOLLOW_parameters_in_function_call2737 = new BitSet(new ulong[]
		{
			17592186044416UL
		});

		// Token: 0x0400046D RID: 1133
		public static readonly BitSet FOLLOW_RPAREN_in_function_call2740 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400046E RID: 1134
		public static readonly BitSet FOLLOW_parameter_in_parameters2774 = new BitSet(new ulong[]
		{
			562949953421314UL
		});

		// Token: 0x0400046F RID: 1135
		public static readonly BitSet FOLLOW_COMMA_in_parameters2777 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000470 RID: 1136
		public static readonly BitSet FOLLOW_parameter_in_parameters2781 = new BitSet(new ulong[]
		{
			562949953421314UL
		});

		// Token: 0x04000471 RID: 1137
		public static readonly BitSet FOLLOW_ID_in_parameter2804 = new BitSet(new ulong[]
		{
			2199023255552UL
		});

		// Token: 0x04000472 RID: 1138
		public static readonly BitSet FOLLOW_EQUALS_in_parameter2806 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000473 RID: 1139
		public static readonly BitSet FOLLOW_expression_in_parameter2810 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000474 RID: 1140
		public static readonly BitSet FOLLOW_number_in_constant2856 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000475 RID: 1141
		public static readonly BitSet FOLLOW_STRING_in_constant2862 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000476 RID: 1142
		public static readonly BitSet FOLLOW_BOOL_in_constant2868 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000477 RID: 1143
		public static readonly BitSet FOLLOW_NONE_in_constant2874 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000478 RID: 1144
		public static readonly BitSet FOLLOW_set_in_number0 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000479 RID: 1145
		public static readonly BitSet FOLLOW_ID_in_type2908 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400047A RID: 1146
		public static readonly BitSet FOLLOW_ID_in_type2919 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x0400047B RID: 1147
		public static readonly BitSet FOLLOW_LBRACKET_in_type2921 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x0400047C RID: 1148
		public static readonly BitSet FOLLOW_RBRACKET_in_type2923 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400047D RID: 1149
		public static readonly BitSet FOLLOW_BASETYPE_in_type2934 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400047E RID: 1150
		public static readonly BitSet FOLLOW_BASETYPE_in_type2945 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x0400047F RID: 1151
		public static readonly BitSet FOLLOW_LBRACKET_in_type2947 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000480 RID: 1152
		public static readonly BitSet FOLLOW_RBRACKET_in_type2949 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000481 RID: 1153
		public static readonly BitSet FOLLOW_EOL_in_terminator2967 = new BitSet(new ulong[]
		{
			2UL,
			1073741824UL
		});

		// Token: 0x04000482 RID: 1154
		public static readonly BitSet FOLLOW_type_in_synpred1_Papyrus1498 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000483 RID: 1155
		public static readonly BitSet FOLLOW_ID_in_synpred1_Papyrus1500 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000484 RID: 1156
		public static readonly BitSet FOLLOW_l_value_in_synpred2_Papyrus1516 = new BitSet(new ulong[]
		{
			144115188075855872UL
		});

		// Token: 0x04000485 RID: 1157
		public static readonly BitSet FOLLOW_PLUSEQUALS_in_synpred2_Papyrus1518 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000486 RID: 1158
		public static readonly BitSet FOLLOW_l_value_in_synpred3_Papyrus1567 = new BitSet(new ulong[]
		{
			288230376151711744UL
		});

		// Token: 0x04000487 RID: 1159
		public static readonly BitSet FOLLOW_MINUSEQUALS_in_synpred3_Papyrus1569 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000488 RID: 1160
		public static readonly BitSet FOLLOW_l_value_in_synpred4_Papyrus1617 = new BitSet(new ulong[]
		{
			576460752303423488UL
		});

		// Token: 0x04000489 RID: 1161
		public static readonly BitSet FOLLOW_MULTEQUALS_in_synpred4_Papyrus1619 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400048A RID: 1162
		public static readonly BitSet FOLLOW_l_value_in_synpred5_Papyrus1667 = new BitSet(new ulong[]
		{
			1152921504606846976UL
		});

		// Token: 0x0400048B RID: 1163
		public static readonly BitSet FOLLOW_DIVEQUALS_in_synpred5_Papyrus1669 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400048C RID: 1164
		public static readonly BitSet FOLLOW_l_value_in_synpred6_Papyrus1717 = new BitSet(new ulong[]
		{
			2305843009213693952UL
		});

		// Token: 0x0400048D RID: 1165
		public static readonly BitSet FOLLOW_MODEQUALS_in_synpred6_Papyrus1719 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400048E RID: 1166
		public static readonly BitSet FOLLOW_l_value_in_synpred7_Papyrus1767 = new BitSet(new ulong[]
		{
			2199023255552UL
		});

		// Token: 0x0400048F RID: 1167
		public static readonly BitSet FOLLOW_EQUALS_in_synpred7_Papyrus1769 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000490 RID: 1168
		public static readonly BitSet FOLLOW_LPAREN_in_synpred8_Papyrus1823 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000491 RID: 1169
		public static readonly BitSet FOLLOW_expression_in_synpred8_Papyrus1825 = new BitSet(new ulong[]
		{
			17592186044416UL
		});

		// Token: 0x04000492 RID: 1170
		public static readonly BitSet FOLLOW_RPAREN_in_synpred8_Papyrus1827 = new BitSet(new ulong[]
		{
			4611686018427387904UL
		});

		// Token: 0x04000493 RID: 1171
		public static readonly BitSet FOLLOW_DOT_in_synpred8_Papyrus1829 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000494 RID: 1172
		public static readonly BitSet FOLLOW_LPAREN_in_synpred9_Papyrus1867 = new BitSet(new ulong[]
		{
			9070970929152UL,
			1007109120UL
		});

		// Token: 0x04000495 RID: 1173
		public static readonly BitSet FOLLOW_expression_in_synpred9_Papyrus1869 = new BitSet(new ulong[]
		{
			17592186044416UL
		});

		// Token: 0x04000496 RID: 1174
		public static readonly BitSet FOLLOW_RPAREN_in_synpred9_Papyrus1871 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000497 RID: 1175
		public static readonly BitSet FOLLOW_LBRACKET_in_synpred9_Papyrus1873 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000498 RID: 1176
		public static readonly BitSet FOLLOW_array_func_or_id_in_synpred10_Papyrus1932 = new BitSet(new ulong[]
		{
			4611686018427387904UL
		});

		// Token: 0x04000499 RID: 1177
		public static readonly BitSet FOLLOW_DOT_in_synpred10_Papyrus1934 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400049A RID: 1178
		public static readonly BitSet FOLLOW_func_or_id_in_synpred11_Papyrus1950 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x0400049B RID: 1179
		public static readonly BitSet FOLLOW_LBRACKET_in_synpred11_Papyrus1952 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400049C RID: 1180
		public static readonly BitSet FOLLOW_atom_in_synpred12_Papyrus2279 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x0400049D RID: 1181
		public static readonly BitSet FOLLOW_LBRACKET_in_synpred12_Papyrus2281 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400049E RID: 1182
		public static readonly BitSet FOLLOW_function_call_in_synpred13_Papyrus2399 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x0400049F RID: 1183
		public static readonly BitSet FOLLOW_LBRACKET_in_synpred13_Papyrus2401 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040004A0 RID: 1184
		public static readonly BitSet FOLLOW_ID_in_synpred14_Papyrus2432 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x040004A1 RID: 1185
		public static readonly BitSet FOLLOW_LBRACKET_in_synpred14_Papyrus2434 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x020000EC RID: 236
		// (Invoke) Token: 0x06000A0A RID: 2570
		private delegate bool FlagIsValid(PapyrusFlag akFlag);

		// Token: 0x020000ED RID: 237
		public class script_return : ParserRuleReturnScope
		{
			// Token: 0x170000F7 RID: 247
			// (get) Token: 0x06000A0D RID: 2573 RVA: 0x00031BD0 File Offset: 0x0002FDD0
			// (set) Token: 0x06000A0E RID: 2574 RVA: 0x00031BD8 File Offset: 0x0002FDD8
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004A6 RID: 1190
			private object tree;
		}

		// Token: 0x020000EE RID: 238
		protected class header_scope
		{
			// Token: 0x040004A7 RID: 1191
			protected internal string sobjFlags;
		}

		// Token: 0x020000EF RID: 239
		public class header_return : ParserRuleReturnScope
		{
			// Token: 0x170000F8 RID: 248
			// (get) Token: 0x06000A11 RID: 2577 RVA: 0x00031BF4 File Offset: 0x0002FDF4
			// (set) Token: 0x06000A12 RID: 2578 RVA: 0x00031BFC File Offset: 0x0002FDFC
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004A8 RID: 1192
			private object tree;
		}

		// Token: 0x020000F0 RID: 240
		public class definitionOrBlock_return : ParserRuleReturnScope
		{
			// Token: 0x170000F9 RID: 249
			// (get) Token: 0x06000A14 RID: 2580 RVA: 0x00031C10 File Offset: 0x0002FE10
			// (set) Token: 0x06000A15 RID: 2581 RVA: 0x00031C18 File Offset: 0x0002FE18
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004A9 RID: 1193
			private object tree;
		}

		// Token: 0x020000F1 RID: 241
		public class userFlags_return : ParserRuleReturnScope
		{
			// Token: 0x170000FA RID: 250
			// (get) Token: 0x06000A17 RID: 2583 RVA: 0x00031C2C File Offset: 0x0002FE2C
			// (set) Token: 0x06000A18 RID: 2584 RVA: 0x00031C34 File Offset: 0x0002FE34
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004AA RID: 1194
			public List<string> kFlagList;

			// Token: 0x040004AB RID: 1195
			private object tree;
		}

		// Token: 0x020000F2 RID: 242
		protected class fieldDefinition_scope
		{
			// Token: 0x040004AC RID: 1196
			protected internal string svarFlags;
		}

		// Token: 0x020000F3 RID: 243
		public class fieldDefinition_return : ParserRuleReturnScope
		{
			// Token: 0x170000FB RID: 251
			// (get) Token: 0x06000A1B RID: 2587 RVA: 0x00031C50 File Offset: 0x0002FE50
			// (set) Token: 0x06000A1C RID: 2588 RVA: 0x00031C58 File Offset: 0x0002FE58
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004AD RID: 1197
			public string sVarName;

			// Token: 0x040004AE RID: 1198
			public ScriptVariableType kVarType;

			// Token: 0x040004AF RID: 1199
			private object tree;
		}

		// Token: 0x020000F4 RID: 244
		public class import_obj_return : ParserRuleReturnScope
		{
			// Token: 0x170000FC RID: 252
			// (get) Token: 0x06000A1E RID: 2590 RVA: 0x00031C6C File Offset: 0x0002FE6C
			// (set) Token: 0x06000A1F RID: 2591 RVA: 0x00031C74 File Offset: 0x0002FE74
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004B0 RID: 1200
			private object tree;
		}

		// Token: 0x020000F5 RID: 245
		protected class function_scope
		{
			// Token: 0x040004B1 RID: 1201
			protected internal bool ballowGlobal;

			// Token: 0x040004B2 RID: 1202
			protected internal string sstateName;

			// Token: 0x040004B3 RID: 1203
			protected internal string spropertyName;

			// Token: 0x040004B4 RID: 1204
			protected internal ScriptFunctionType kfunction;
		}

		// Token: 0x020000F6 RID: 246
		public class function_return : ParserRuleReturnScope
		{
			// Token: 0x170000FD RID: 253
			// (get) Token: 0x06000A22 RID: 2594 RVA: 0x00031C90 File Offset: 0x0002FE90
			// (set) Token: 0x06000A23 RID: 2595 RVA: 0x00031C98 File Offset: 0x0002FE98
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004B5 RID: 1205
			public string sFuncName;

			// Token: 0x040004B6 RID: 1206
			public ScriptFunctionType kFunction;

			// Token: 0x040004B7 RID: 1207
			private object tree;
		}

		// Token: 0x020000F7 RID: 247
		protected class functionHeader_scope
		{
			// Token: 0x040004B8 RID: 1208
			protected internal bool bglobal;

			// Token: 0x040004B9 RID: 1209
			protected internal bool bnative;

			// Token: 0x040004BA RID: 1210
			protected internal string suserFlags;

			// Token: 0x040004BB RID: 1211
			protected internal List<string> kparamNames;

			// Token: 0x040004BC RID: 1212
			protected internal List<ScriptVariableType> kparamTypes;
		}

		// Token: 0x020000F8 RID: 248
		public class functionHeader_return : ParserRuleReturnScope
		{
			// Token: 0x170000FE RID: 254
			// (get) Token: 0x06000A26 RID: 2598 RVA: 0x00031CB4 File Offset: 0x0002FEB4
			// (set) Token: 0x06000A27 RID: 2599 RVA: 0x00031CBC File Offset: 0x0002FEBC
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004BD RID: 1213
			private object tree;
		}

		// Token: 0x020000F9 RID: 249
		public class functionBlock_return : ParserRuleReturnScope
		{
			// Token: 0x170000FF RID: 255
			// (get) Token: 0x06000A29 RID: 2601 RVA: 0x00031CD0 File Offset: 0x0002FED0
			// (set) Token: 0x06000A2A RID: 2602 RVA: 0x00031CD8 File Offset: 0x0002FED8
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004BE RID: 1214
			private object tree;
		}

		// Token: 0x020000FA RID: 250
		public class functionModifier_return : ParserRuleReturnScope
		{
			// Token: 0x17000100 RID: 256
			// (get) Token: 0x06000A2C RID: 2604 RVA: 0x00031CEC File Offset: 0x0002FEEC
			// (set) Token: 0x06000A2D RID: 2605 RVA: 0x00031CF4 File Offset: 0x0002FEF4
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004BF RID: 1215
			private object tree;
		}

		// Token: 0x020000FB RID: 251
		protected class eventFunc_scope
		{
			// Token: 0x040004C0 RID: 1216
			protected internal ScriptFunctionType keventFunction;

			// Token: 0x040004C1 RID: 1217
			protected internal string sstateName;
		}

		// Token: 0x020000FC RID: 252
		public class eventFunc_return : ParserRuleReturnScope
		{
			// Token: 0x17000101 RID: 257
			// (get) Token: 0x06000A30 RID: 2608 RVA: 0x00031D10 File Offset: 0x0002FF10
			// (set) Token: 0x06000A31 RID: 2609 RVA: 0x00031D18 File Offset: 0x0002FF18
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004C2 RID: 1218
			public string sEventName;

			// Token: 0x040004C3 RID: 1219
			public ScriptFunctionType kEventFunction;

			// Token: 0x040004C4 RID: 1220
			private object tree;
		}

		// Token: 0x020000FD RID: 253
		protected class eventHeader_scope
		{
			// Token: 0x040004C5 RID: 1221
			protected internal string suserFlags;

			// Token: 0x040004C6 RID: 1222
			protected internal List<string> kparamNames;

			// Token: 0x040004C7 RID: 1223
			protected internal List<ScriptVariableType> kparamTypes;
		}

		// Token: 0x020000FE RID: 254
		public class eventHeader_return : ParserRuleReturnScope
		{
			// Token: 0x17000102 RID: 258
			// (get) Token: 0x06000A34 RID: 2612 RVA: 0x00031D34 File Offset: 0x0002FF34
			// (set) Token: 0x06000A35 RID: 2613 RVA: 0x00031D3C File Offset: 0x0002FF3C
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004C8 RID: 1224
			private object tree;
		}

		// Token: 0x020000FF RID: 255
		public class eventBlock_return : ParserRuleReturnScope
		{
			// Token: 0x17000103 RID: 259
			// (get) Token: 0x06000A37 RID: 2615 RVA: 0x00031D50 File Offset: 0x0002FF50
			// (set) Token: 0x06000A38 RID: 2616 RVA: 0x00031D58 File Offset: 0x0002FF58
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004C9 RID: 1225
			private object tree;
		}

		// Token: 0x02000100 RID: 256
		public class callParameters_return : ParserRuleReturnScope
		{
			// Token: 0x17000104 RID: 260
			// (get) Token: 0x06000A3A RID: 2618 RVA: 0x00031D6C File Offset: 0x0002FF6C
			// (set) Token: 0x06000A3B RID: 2619 RVA: 0x00031D74 File Offset: 0x0002FF74
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004CA RID: 1226
			private object tree;
		}

		// Token: 0x02000101 RID: 257
		public class callParameter_return : ParserRuleReturnScope
		{
			// Token: 0x17000105 RID: 261
			// (get) Token: 0x06000A3D RID: 2621 RVA: 0x00031D88 File Offset: 0x0002FF88
			// (set) Token: 0x06000A3E RID: 2622 RVA: 0x00031D90 File Offset: 0x0002FF90
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004CB RID: 1227
			private object tree;
		}

		// Token: 0x02000102 RID: 258
		public class stateBlock_return : ParserRuleReturnScope
		{
			// Token: 0x17000106 RID: 262
			// (get) Token: 0x06000A40 RID: 2624 RVA: 0x00031DA4 File Offset: 0x0002FFA4
			// (set) Token: 0x06000A41 RID: 2625 RVA: 0x00031DAC File Offset: 0x0002FFAC
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004CC RID: 1228
			private object tree;
		}

		// Token: 0x02000103 RID: 259
		public class stateFuncOrEvent_return : ParserRuleReturnScope
		{
			// Token: 0x17000107 RID: 263
			// (get) Token: 0x06000A43 RID: 2627 RVA: 0x00031DC0 File Offset: 0x0002FFC0
			// (set) Token: 0x06000A44 RID: 2628 RVA: 0x00031DC8 File Offset: 0x0002FFC8
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004CD RID: 1229
			private object tree;
		}

		// Token: 0x02000104 RID: 260
		protected class propertyBlock_scope
		{
			// Token: 0x040004CE RID: 1230
			protected internal string sname;

			// Token: 0x040004CF RID: 1231
			protected internal ScriptVariableType ktype;
		}

		// Token: 0x02000105 RID: 261
		public class propertyBlock_return : ParserRuleReturnScope
		{
			// Token: 0x17000108 RID: 264
			// (get) Token: 0x06000A47 RID: 2631 RVA: 0x00031DE4 File Offset: 0x0002FFE4
			// (set) Token: 0x06000A48 RID: 2632 RVA: 0x00031DEC File Offset: 0x0002FFEC
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004D0 RID: 1232
			private object tree;
		}

		// Token: 0x02000106 RID: 262
		protected class propertyHeader_scope
		{
			// Token: 0x040004D1 RID: 1233
			protected internal string suserFlags;
		}

		// Token: 0x02000107 RID: 263
		public class propertyHeader_return : ParserRuleReturnScope
		{
			// Token: 0x17000109 RID: 265
			// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00031E08 File Offset: 0x00030008
			// (set) Token: 0x06000A4C RID: 2636 RVA: 0x00031E10 File Offset: 0x00030010
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004D2 RID: 1234
			private object tree;
		}

		// Token: 0x02000108 RID: 264
		protected class autoPropertyHeader_scope
		{
			// Token: 0x040004D3 RID: 1235
			protected internal string suserFlags;
		}

		// Token: 0x02000109 RID: 265
		public class autoPropertyHeader_return : ParserRuleReturnScope
		{
			// Token: 0x1700010A RID: 266
			// (get) Token: 0x06000A4F RID: 2639 RVA: 0x00031E2C File Offset: 0x0003002C
			// (set) Token: 0x06000A50 RID: 2640 RVA: 0x00031E34 File Offset: 0x00030034
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004D4 RID: 1236
			public string sVarUserFlags;

			// Token: 0x040004D5 RID: 1237
			public ITree kInitialValue;

			// Token: 0x040004D6 RID: 1238
			private object tree;
		}

		// Token: 0x0200010A RID: 266
		protected class readOnlyAutoPropertyHeader_scope
		{
			// Token: 0x040004D7 RID: 1239
			protected internal string suserFlags;
		}

		// Token: 0x0200010B RID: 267
		public class readOnlyAutoPropertyHeader_return : ParserRuleReturnScope
		{
			// Token: 0x1700010B RID: 267
			// (get) Token: 0x06000A53 RID: 2643 RVA: 0x00031E50 File Offset: 0x00030050
			// (set) Token: 0x06000A54 RID: 2644 RVA: 0x00031E58 File Offset: 0x00030058
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004D8 RID: 1240
			public ITree kInitialValue;

			// Token: 0x040004D9 RID: 1241
			private object tree;
		}

		// Token: 0x0200010C RID: 268
		public class propertyFunc_return : ParserRuleReturnScope
		{
			// Token: 0x1700010C RID: 268
			// (get) Token: 0x06000A56 RID: 2646 RVA: 0x00031E6C File Offset: 0x0003006C
			// (set) Token: 0x06000A57 RID: 2647 RVA: 0x00031E74 File Offset: 0x00030074
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004DA RID: 1242
			private object tree;
		}

		// Token: 0x0200010D RID: 269
		public class statement_return : ParserRuleReturnScope
		{
			// Token: 0x1700010D RID: 269
			// (get) Token: 0x06000A59 RID: 2649 RVA: 0x00031E88 File Offset: 0x00030088
			// (set) Token: 0x06000A5A RID: 2650 RVA: 0x00031E90 File Offset: 0x00030090
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004DB RID: 1243
			private object tree;
		}

		// Token: 0x0200010E RID: 270
		public class l_value_return : ParserRuleReturnScope
		{
			// Token: 0x1700010E RID: 270
			// (get) Token: 0x06000A5C RID: 2652 RVA: 0x00031EA4 File Offset: 0x000300A4
			// (set) Token: 0x06000A5D RID: 2653 RVA: 0x00031EAC File Offset: 0x000300AC
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004DC RID: 1244
			private object tree;
		}

		// Token: 0x0200010F RID: 271
		public class basic_l_value_return : ParserRuleReturnScope
		{
			// Token: 0x1700010F RID: 271
			// (get) Token: 0x06000A5F RID: 2655 RVA: 0x00031EC0 File Offset: 0x000300C0
			// (set) Token: 0x06000A60 RID: 2656 RVA: 0x00031EC8 File Offset: 0x000300C8
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004DD RID: 1245
			private object tree;
		}

		// Token: 0x02000110 RID: 272
		public class localDefinition_return : ParserRuleReturnScope
		{
			// Token: 0x17000110 RID: 272
			// (get) Token: 0x06000A62 RID: 2658 RVA: 0x00031EDC File Offset: 0x000300DC
			// (set) Token: 0x06000A63 RID: 2659 RVA: 0x00031EE4 File Offset: 0x000300E4
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004DE RID: 1246
			public string sVarName;

			// Token: 0x040004DF RID: 1247
			public ScriptVariableType kVarType;

			// Token: 0x040004E0 RID: 1248
			private object tree;
		}

		// Token: 0x02000111 RID: 273
		public class expression_return : ParserRuleReturnScope
		{
			// Token: 0x17000111 RID: 273
			// (get) Token: 0x06000A65 RID: 2661 RVA: 0x00031EF8 File Offset: 0x000300F8
			// (set) Token: 0x06000A66 RID: 2662 RVA: 0x00031F00 File Offset: 0x00030100
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E1 RID: 1249
			private object tree;
		}

		// Token: 0x02000112 RID: 274
		public class and_expression_return : ParserRuleReturnScope
		{
			// Token: 0x17000112 RID: 274
			// (get) Token: 0x06000A68 RID: 2664 RVA: 0x00031F14 File Offset: 0x00030114
			// (set) Token: 0x06000A69 RID: 2665 RVA: 0x00031F1C File Offset: 0x0003011C
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E2 RID: 1250
			private object tree;
		}

		// Token: 0x02000113 RID: 275
		public class bool_expression_return : ParserRuleReturnScope
		{
			// Token: 0x17000113 RID: 275
			// (get) Token: 0x06000A6B RID: 2667 RVA: 0x00031F30 File Offset: 0x00030130
			// (set) Token: 0x06000A6C RID: 2668 RVA: 0x00031F38 File Offset: 0x00030138
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E3 RID: 1251
			private object tree;
		}

		// Token: 0x02000114 RID: 276
		public class add_expression_return : ParserRuleReturnScope
		{
			// Token: 0x17000114 RID: 276
			// (get) Token: 0x06000A6E RID: 2670 RVA: 0x00031F4C File Offset: 0x0003014C
			// (set) Token: 0x06000A6F RID: 2671 RVA: 0x00031F54 File Offset: 0x00030154
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E4 RID: 1252
			private object tree;
		}

		// Token: 0x02000115 RID: 277
		public class mult_expression_return : ParserRuleReturnScope
		{
			// Token: 0x17000115 RID: 277
			// (get) Token: 0x06000A71 RID: 2673 RVA: 0x00031F68 File Offset: 0x00030168
			// (set) Token: 0x06000A72 RID: 2674 RVA: 0x00031F70 File Offset: 0x00030170
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E5 RID: 1253
			private object tree;
		}

		// Token: 0x02000116 RID: 278
		public class unary_expression_return : ParserRuleReturnScope
		{
			// Token: 0x17000116 RID: 278
			// (get) Token: 0x06000A74 RID: 2676 RVA: 0x00031F84 File Offset: 0x00030184
			// (set) Token: 0x06000A75 RID: 2677 RVA: 0x00031F8C File Offset: 0x0003018C
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E6 RID: 1254
			private object tree;
		}

		// Token: 0x02000117 RID: 279
		public class cast_atom_return : ParserRuleReturnScope
		{
			// Token: 0x17000117 RID: 279
			// (get) Token: 0x06000A77 RID: 2679 RVA: 0x00031FA0 File Offset: 0x000301A0
			// (set) Token: 0x06000A78 RID: 2680 RVA: 0x00031FA8 File Offset: 0x000301A8
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E7 RID: 1255
			private object tree;
		}

		// Token: 0x02000118 RID: 280
		public class dot_atom_return : ParserRuleReturnScope
		{
			// Token: 0x17000118 RID: 280
			// (get) Token: 0x06000A7A RID: 2682 RVA: 0x00031FBC File Offset: 0x000301BC
			// (set) Token: 0x06000A7B RID: 2683 RVA: 0x00031FC4 File Offset: 0x000301C4
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E8 RID: 1256
			private object tree;
		}

		// Token: 0x02000119 RID: 281
		public class array_atom_return : ParserRuleReturnScope
		{
			// Token: 0x17000119 RID: 281
			// (get) Token: 0x06000A7D RID: 2685 RVA: 0x00031FD8 File Offset: 0x000301D8
			// (set) Token: 0x06000A7E RID: 2686 RVA: 0x00031FE0 File Offset: 0x000301E0
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004E9 RID: 1257
			private object tree;
		}

		// Token: 0x0200011A RID: 282
		public class atom_return : ParserRuleReturnScope
		{
			// Token: 0x1700011A RID: 282
			// (get) Token: 0x06000A80 RID: 2688 RVA: 0x00031FF4 File Offset: 0x000301F4
			// (set) Token: 0x06000A81 RID: 2689 RVA: 0x00031FFC File Offset: 0x000301FC
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004EA RID: 1258
			private object tree;
		}

		// Token: 0x0200011B RID: 283
		public class array_func_or_id_return : ParserRuleReturnScope
		{
			// Token: 0x1700011B RID: 283
			// (get) Token: 0x06000A83 RID: 2691 RVA: 0x00032010 File Offset: 0x00030210
			// (set) Token: 0x06000A84 RID: 2692 RVA: 0x00032018 File Offset: 0x00030218
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004EB RID: 1259
			private object tree;
		}

		// Token: 0x0200011C RID: 284
		public class func_or_id_return : ParserRuleReturnScope
		{
			// Token: 0x1700011C RID: 284
			// (get) Token: 0x06000A86 RID: 2694 RVA: 0x0003202C File Offset: 0x0003022C
			// (set) Token: 0x06000A87 RID: 2695 RVA: 0x00032034 File Offset: 0x00030234
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004EC RID: 1260
			private object tree;
		}

		// Token: 0x0200011D RID: 285
		public class return_stat_return : ParserRuleReturnScope
		{
			// Token: 0x1700011D RID: 285
			// (get) Token: 0x06000A89 RID: 2697 RVA: 0x00032048 File Offset: 0x00030248
			// (set) Token: 0x06000A8A RID: 2698 RVA: 0x00032050 File Offset: 0x00030250
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004ED RID: 1261
			private object tree;
		}

		// Token: 0x0200011E RID: 286
		protected class ifBlock_scope
		{
			// Token: 0x040004EE RID: 1262
			protected internal ScriptScope kparentScope;

			// Token: 0x040004EF RID: 1263
			protected internal ScriptScope kifScope;
		}

		// Token: 0x0200011F RID: 287
		public class ifBlock_return : ParserRuleReturnScope
		{
			// Token: 0x1700011E RID: 286
			// (get) Token: 0x06000A8D RID: 2701 RVA: 0x0003206C File Offset: 0x0003026C
			// (set) Token: 0x06000A8E RID: 2702 RVA: 0x00032074 File Offset: 0x00030274
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004F0 RID: 1264
			private object tree;
		}

		// Token: 0x02000120 RID: 288
		protected class elseIfBlock_scope
		{
			// Token: 0x040004F1 RID: 1265
			protected internal ScriptScope kelseIfScope;
		}

		// Token: 0x02000121 RID: 289
		public class elseIfBlock_return : ParserRuleReturnScope
		{
			// Token: 0x1700011F RID: 287
			// (get) Token: 0x06000A91 RID: 2705 RVA: 0x00032090 File Offset: 0x00030290
			// (set) Token: 0x06000A92 RID: 2706 RVA: 0x00032098 File Offset: 0x00030298
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004F2 RID: 1266
			private object tree;
		}

		// Token: 0x02000122 RID: 290
		protected class elseBlock_scope
		{
			// Token: 0x040004F3 RID: 1267
			protected internal ScriptScope kelseScope;
		}

		// Token: 0x02000123 RID: 291
		public class elseBlock_return : ParserRuleReturnScope
		{
			// Token: 0x17000120 RID: 288
			// (get) Token: 0x06000A95 RID: 2709 RVA: 0x000320B4 File Offset: 0x000302B4
			// (set) Token: 0x06000A96 RID: 2710 RVA: 0x000320BC File Offset: 0x000302BC
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004F4 RID: 1268
			private object tree;
		}

		// Token: 0x02000124 RID: 292
		protected class whileBlock_scope
		{
			// Token: 0x040004F5 RID: 1269
			protected internal ScriptScope kwhileScope;
		}

		// Token: 0x02000125 RID: 293
		public class whileBlock_return : ParserRuleReturnScope
		{
			// Token: 0x17000121 RID: 289
			// (get) Token: 0x06000A99 RID: 2713 RVA: 0x000320D8 File Offset: 0x000302D8
			// (set) Token: 0x06000A9A RID: 2714 RVA: 0x000320E0 File Offset: 0x000302E0
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004F6 RID: 1270
			private object tree;
		}

		// Token: 0x02000126 RID: 294
		public class function_call_return : ParserRuleReturnScope
		{
			// Token: 0x17000122 RID: 290
			// (get) Token: 0x06000A9C RID: 2716 RVA: 0x000320F4 File Offset: 0x000302F4
			// (set) Token: 0x06000A9D RID: 2717 RVA: 0x000320FC File Offset: 0x000302FC
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004F7 RID: 1271
			private object tree;
		}

		// Token: 0x02000127 RID: 295
		public class parameters_return : ParserRuleReturnScope
		{
			// Token: 0x17000123 RID: 291
			// (get) Token: 0x06000A9F RID: 2719 RVA: 0x00032110 File Offset: 0x00030310
			// (set) Token: 0x06000AA0 RID: 2720 RVA: 0x00032118 File Offset: 0x00030318
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004F8 RID: 1272
			private object tree;
		}

		// Token: 0x02000128 RID: 296
		public class parameter_return : ParserRuleReturnScope
		{
			// Token: 0x17000124 RID: 292
			// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x0003212C File Offset: 0x0003032C
			// (set) Token: 0x06000AA3 RID: 2723 RVA: 0x00032134 File Offset: 0x00030334
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004F9 RID: 1273
			private object tree;
		}

		// Token: 0x02000129 RID: 297
		public class constant_return : ParserRuleReturnScope
		{
			// Token: 0x17000125 RID: 293
			// (get) Token: 0x06000AA5 RID: 2725 RVA: 0x00032148 File Offset: 0x00030348
			// (set) Token: 0x06000AA6 RID: 2726 RVA: 0x00032150 File Offset: 0x00030350
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004FA RID: 1274
			private object tree;
		}

		// Token: 0x0200012A RID: 298
		public class number_return : ParserRuleReturnScope
		{
			// Token: 0x17000126 RID: 294
			// (get) Token: 0x06000AA8 RID: 2728 RVA: 0x00032164 File Offset: 0x00030364
			// (set) Token: 0x06000AA9 RID: 2729 RVA: 0x0003216C File Offset: 0x0003036C
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004FB RID: 1275
			private object tree;
		}

		// Token: 0x0200012B RID: 299
		public class type_return : ParserRuleReturnScope
		{
			// Token: 0x17000127 RID: 295
			// (get) Token: 0x06000AAB RID: 2731 RVA: 0x00032180 File Offset: 0x00030380
			// (set) Token: 0x06000AAC RID: 2732 RVA: 0x00032188 File Offset: 0x00030388
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004FC RID: 1276
			public ScriptVariableType kType;

			// Token: 0x040004FD RID: 1277
			private object tree;
		}

		// Token: 0x0200012C RID: 300
		public class terminator_return : ParserRuleReturnScope
		{
			// Token: 0x17000128 RID: 296
			// (get) Token: 0x06000AAE RID: 2734 RVA: 0x0003219C File Offset: 0x0003039C
			// (set) Token: 0x06000AAF RID: 2735 RVA: 0x000321A4 File Offset: 0x000303A4
			public override object Tree
			{
				get
				{
					return tree;
				}
				set
				{
					tree = value;
				}
			}

			// Token: 0x040004FE RID: 1278
			private object tree;
		}

		// Token: 0x0200012D RID: 301
		protected class DFA6 : DFA
		{
			// Token: 0x06000AB1 RID: 2737 RVA: 0x000321B8 File Offset: 0x000303B8
			public DFA6(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 6;
				eot = DFA6_eot;
				eof = DFA6_eof;
				min = DFA6_min;
				max = DFA6_max;
				accept = DFA6_accept;
				special = DFA6_special;
				transition = DFA6_transition;
			}

			// Token: 0x17000129 RID: 297
			// (get) Token: 0x06000AB2 RID: 2738 RVA: 0x00032228 File Offset: 0x00030428
			public override string Description
			{
				get
				{
					return "149:1: definitionOrBlock : ( fieldDefinition | import_obj | function[true, \"\", \"\"] | eventFunc[\"\"] | stateBlock | propertyBlock );";
				}
			}
		}

		// Token: 0x0200012E RID: 302
		protected class DFA14 : DFA
		{
			// Token: 0x06000AB3 RID: 2739 RVA: 0x00032230 File Offset: 0x00030430
			public DFA14(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 14;
				eot = DFA14_eot;
				eof = DFA14_eof;
				min = DFA14_min;
				max = DFA14_max;
				accept = DFA14_accept;
				special = DFA14_special;
				transition = DFA14_transition;
			}

			// Token: 0x1700012A RID: 298
			// (get) Token: 0x06000AB4 RID: 2740 RVA: 0x000322A0 File Offset: 0x000304A0
			public override string Description
			{
				get
				{
					return "243:147: ( terminator DOCSTRING )?";
				}
			}
		}

		// Token: 0x0200012F RID: 303
		protected class DFA16 : DFA
		{
			// Token: 0x06000AB5 RID: 2741 RVA: 0x000322A8 File Offset: 0x000304A8
			public DFA16(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 16;
				eot = DFA16_eot;
				eof = DFA16_eof;
				min = DFA16_min;
				max = DFA16_max;
				accept = DFA16_accept;
				special = DFA16_special;
				transition = DFA16_transition;
			}

			// Token: 0x1700012B RID: 299
			// (get) Token: 0x06000AB6 RID: 2742 RVA: 0x00032318 File Offset: 0x00030518
			public override string Description
			{
				get
				{
					return "266:1: functionBlock : ({...}? => terminator -> | {...}? => terminator ( statement[$function::kfunction.FunctionScope] )* ENDFUNCTION terminator -> ^( BLOCK ( statement )* ) );";
				}
			}
		}

		// Token: 0x02000130 RID: 304
		protected class DFA21 : DFA
		{
			// Token: 0x06000AB7 RID: 2743 RVA: 0x00032320 File Offset: 0x00030520
			public DFA21(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 21;
				eot = DFA21_eot;
				eof = DFA21_eof;
				min = DFA21_min;
				max = DFA21_max;
				accept = DFA21_accept;
				special = DFA21_special;
				transition = DFA21_transition;
			}

			// Token: 0x1700012C RID: 300
			// (get) Token: 0x06000AB8 RID: 2744 RVA: 0x00032390 File Offset: 0x00030590
			public override string Description
			{
				get
				{
					return "322:117: ( terminator DOCSTRING )?";
				}
			}
		}

		// Token: 0x02000131 RID: 305
		protected class DFA23 : DFA
		{
			// Token: 0x06000AB9 RID: 2745 RVA: 0x00032398 File Offset: 0x00030598
			public DFA23(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 23;
				eot = DFA23_eot;
				eof = DFA23_eof;
				min = DFA23_min;
				max = DFA23_max;
				accept = DFA23_accept;
				special = DFA23_special;
				transition = DFA23_transition;
			}

			// Token: 0x1700012D RID: 301
			// (get) Token: 0x06000ABA RID: 2746 RVA: 0x00032408 File Offset: 0x00030608
			public override string Description
			{
				get
				{
					return "341:1: eventBlock : ({...}? => terminator -> | {...}? => terminator ( statement[$eventFunc::keventFunction.FunctionScope] )* ENDEVENT terminator -> ^( BLOCK ( statement )* ) );";
				}
			}
		}

		// Token: 0x02000132 RID: 306
		protected class DFA30 : DFA
		{
			// Token: 0x06000ABB RID: 2747 RVA: 0x00032410 File Offset: 0x00030610
			public DFA30(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 30;
				eot = DFA30_eot;
				eof = DFA30_eof;
				min = DFA30_min;
				max = DFA30_max;
				accept = DFA30_accept;
				special = DFA30_special;
				transition = DFA30_transition;
			}

			// Token: 0x1700012E RID: 302
			// (get) Token: 0x06000ABC RID: 2748 RVA: 0x00032480 File Offset: 0x00030680
			public override string Description
			{
				get
				{
					return "370:1: propertyBlock : ( propertyHeader func0= propertyFunc[$propertyBlock::sname] (func1= propertyFunc[$propertyBlock::sname] )? ENDPROPERTY terminator -> ^( PROPERTY[$propertyHeader.start, \"property\"] propertyHeader $func0 ( $func1)? ) | autoPropertyHeader -> | readOnlyAutoPropertyHeader ->);";
				}
			}
		}

		// Token: 0x02000133 RID: 307
		protected class DFA38 : DFA
		{
			// Token: 0x06000ABD RID: 2749 RVA: 0x00032488 File Offset: 0x00030688
			public DFA38(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 38;
				eot = DFA38_eot;
				eof = DFA38_eof;
				min = DFA38_min;
				max = DFA38_max;
				accept = DFA38_accept;
				special = DFA38_special;
				transition = DFA38_transition;
			}

			// Token: 0x1700012F RID: 303
			// (get) Token: 0x06000ABE RID: 2750 RVA: 0x000324F8 File Offset: 0x000306F8
			public override string Description
			{
				get
				{
					return "448:1: statement[ScriptScope akCurrentScope] : ( ( type ID )=> localDefinition | ( l_value PLUSEQUALS )=> l_value PLUSEQUALS expression terminator -> ^( EQUALS[$PLUSEQUALS] l_value ^( PLUS[$PLUSEQUALS] ^( PAREXPR[$PLUSEQUALS, \"()\"] l_value ) ^( PAREXPR[$PLUSEQUALS, \"()\"] expression ) ) ) | ( l_value MINUSEQUALS )=> l_value MINUSEQUALS expression terminator -> ^( EQUALS[$MINUSEQUALS] l_value ^( MINUS[$MINUSEQUALS] ^( PAREXPR[$MINUSEQUALS, \"()\"] l_value ) ^( PAREXPR[$MINUSEQUALS, \"()\"] expression ) ) ) | ( l_value MULTEQUALS )=> l_value MULTEQUALS expression terminator -> ^( EQUALS[$MULTEQUALS] l_value ^( MULT[$MULTEQUALS] ^( PAREXPR[$MULTEQUALS, \"()\"] l_value ) ^( PAREXPR[$MULTEQUALS, \"()\"] expression ) ) ) | ( l_value DIVEQUALS )=> l_value DIVEQUALS expression terminator -> ^( EQUALS[$DIVEQUALS] l_value ^( DIVIDE[$DIVEQUALS] ^( PAREXPR[$DIVEQUALS, \"()\"] l_value ) ^( PAREXPR[$DIVEQUALS, \"()\"] expression ) ) ) | ( l_value MODEQUALS )=> l_value MODEQUALS expression terminator -> ^( EQUALS[$MODEQUALS] l_value ^( MOD[$MODEQUALS] ^( PAREXPR[$MODEQUALS, \"()\"] l_value ) ^( PAREXPR[$MODEQUALS, \"()\"] expression ) ) ) | ( l_value EQUALS )=> l_value EQUALS expression terminator | expression terminator | return_stat | ifBlock[$akCurrentScope] | whileBlock[$akCurrentScope] );";
				}
			}
		}
	}
}
