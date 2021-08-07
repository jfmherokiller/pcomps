using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using pcomps.Antlr.Runtime;
using pcomps.Antlr.Runtime.Collections;
using pcomps.Antlr.Runtime.Tree;
using pcomps.Antlr.StringTemplate;
using pcomps.Antlr.StringTemplate.Language;

namespace pcomps.PCompiler
{
	// Token: 0x020001C4 RID: 452
	public class PapyrusGen : TreeParser
	{
		// Token: 0x06000D6E RID: 3438 RVA: 0x0005E07C File Offset: 0x0005C27C
		public PapyrusGen(ITreeNodeStream input) : this(input, new RecognizerSharedState())
		{
		}

		// Token: 0x06000D6F RID: 3439 RVA: 0x0005E08C File Offset: 0x0005C28C
		public PapyrusGen(ITreeNodeStream input, RecognizerSharedState state) : base(input, state)
		{
			InitializeCyclicDFAs();
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000D70 RID: 3440 RVA: 0x0005E194 File Offset: 0x0005C394
		// (set) Token: 0x06000D71 RID: 3441 RVA: 0x0005E19C File Offset: 0x0005C39C
		public StringTemplateGroup TemplateLib
		{
			get => templateLib;
            set => templateLib = value;
        }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000D72 RID: 3442 RVA: 0x0005E1A8 File Offset: 0x0005C3A8
		public override string[] TokenNames => tokenNames;

        // Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000D73 RID: 3443 RVA: 0x0005E1B0 File Offset: 0x0005C3B0
		public override string GrammarFileName => "PapyrusGen.g";

        // Token: 0x1400003C RID: 60
		// (add) Token: 0x06000D74 RID: 3444 RVA: 0x0005E1B8 File Offset: 0x0005C3B8
		// (remove) Token: 0x06000D75 RID: 3445 RVA: 0x0005E1D4 File Offset: 0x0005C3D4
		internal event InternalErrorEventHandler ErrorHandler;

		// Token: 0x06000D76 RID: 3446 RVA: 0x0005E1F0 File Offset: 0x0005C3F0
		private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
        {
            ErrorHandler?.Invoke(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
        }

		// Token: 0x06000D77 RID: 3447 RVA: 0x0005E210 File Offset: 0x0005C410
		public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			var errorMessage = GetErrorMessage(e, tokenNames);
			OnError(errorMessage, e.Line, e.CharPositionInLine);
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x0005E23C File Offset: 0x0005C43C
		public script_return script(string asSourceFilename, ScriptObjectType akObj)
		{
			script_stack.Push(new script_scope());
			var script_return = new script_return
            {
                Start = input.LT(1)
            };
            ((script_scope)script_stack.Peek()).sobjName = "";
			((script_scope)script_stack.Peek()).sparentName = "";
			((script_scope)script_stack.Peek()).kobjVarDefinitions = new ArrayList();
			((script_scope)script_stack.Peek()).kobjPropDefinitions = new ArrayList();
			((script_scope)script_stack.Peek()).sinitialState = null;
			((script_scope)script_stack.Peek()).kobjEmptyState = new ArrayList();
			((script_scope)script_stack.Peek()).kstates = new Hashtable();
			((script_scope)script_stack.Peek()).bhasBeginStateEvent = false;
			((script_scope)script_stack.Peek()).bhasEndStateEvent = false;
			((script_scope)script_stack.Peek()).smodTimeUnix = "";
			((script_scope)script_stack.Peek()).scompileTimeUnix = "";
			((script_scope)script_stack.Peek()).suserName = "";
			((script_scope)script_stack.Peek()).scomputerName = "";
			((script_scope)script_stack.Peek()).sobjFlags = "";
			((script_scope)script_stack.Peek()).sdocString = "";
			kObjType = akObj;
			try
			{
				Match(input, 4, FOLLOW_OBJECT_in_script80);
				Match(input, 2, null);
				PushFollow(FOLLOW_header_in_script82);
				header();
				state.followingStackPointer--;
				for (;;)
				{
					var num = 2;
					var num2 = input.LA(1);
					if (num2 is >= 5 and <= 7 or 19 or 51 or 54)
					{
						num = 1;
					}
					var num3 = num;
					if (num3 != 1)
					{
						break;
					}
					PushFollow(FOLLOW_definitionOrBlock_in_script84);
					definitionOrBlock();
					state.followingStackPointer--;
				}
				Match(input, 3, null);
				if (((script_scope)script_stack.Peek()).sparentName == "")
				{
					if (!((script_scope)script_stack.Peek()).bhasBeginStateEvent)
					{
						((script_scope)script_stack.Peek()).kobjEmptyState.Add(templateLib.GetInstanceOf("emptyBeginStateEvent"));
					}
					if (!((script_scope)script_stack.Peek()).bhasEndStateEvent)
					{
						((script_scope)script_stack.Peek()).kobjEmptyState.Add(templateLib.GetInstanceOf("emptyEndStateEvent"));
					}
				}
				((script_scope)script_stack.Peek()).smodTimeUnix = GetFileModTimeUnix(asSourceFilename);
				((script_scope)script_stack.Peek()).scompileTimeUnix = GetCompileTimeUnix();
				((script_scope)script_stack.Peek()).suserName = Environment.UserName;
				((script_scope)script_stack.Peek()).scomputerName = Environment.MachineName;
				((script_scope)script_stack.Peek()).kuserFlagsRef = ConstructUserFlagRefInfo();
				script_return.ST = templateLib.GetInstanceOf("object", new STAttrMap().Add("objName", ((script_scope)script_stack.Peek()).sobjName).Add("parent", ((script_scope)script_stack.Peek()).sparentName).Add("variableDefs", ((script_scope)script_stack.Peek()).kobjVarDefinitions).Add("properties", ((script_scope)script_stack.Peek()).kobjPropDefinitions).Add("initialState", ((script_scope)script_stack.Peek()).sinitialState).Add("emptyStateFuncs", ((script_scope)script_stack.Peek()).kobjEmptyState).Add("stateFuncs", ((script_scope)script_stack.Peek()).kstates).Add("modTimeUnix", ((script_scope)script_stack.Peek()).smodTimeUnix).Add("compileTimeUnix", ((script_scope)script_stack.Peek()).scompileTimeUnix).Add("userName", ((script_scope)script_stack.Peek()).suserName).Add("computerName", ((script_scope)script_stack.Peek()).scomputerName).Add("userFlags", ((script_scope)script_stack.Peek()).sobjFlags).Add("userFlagsRef", ((script_scope)script_stack.Peek()).kuserFlagsRef).Add("docString", ((script_scope)script_stack.Peek()).sdocString));
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			finally
			{
				script_stack.Pop();
			}
			return script_return;
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x0005E7F8 File Offset: 0x0005C9F8
		public header_return header()
		{
			var header_return = new header_return();
			header_return.Start = input.LT(1);
			CommonTree commonTree = null;
			CommonTree commonTree2 = null;
			try
			{
				var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_header224);
				Match(input, 2, null);
				var commonTree4 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_header226);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 == 38)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					commonTree = (CommonTree)Match(input, 38, FOLLOW_ID_in_header230);
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
					commonTree2 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_header233);
				}
				Match(input, 3, null);
				((script_scope)script_stack.Peek()).sobjName = commonTree3.Text;
				((script_scope)script_stack.Peek()).sobjFlags = commonTree4.Text;
				if (commonTree != null)
				{
					((script_scope)script_stack.Peek()).sparentName = commonTree.Text;
				}
				if (commonTree2 != null)
				{
					((script_scope)script_stack.Peek()).sdocString = commonTree2.Text;
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return header_return;
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x0005E99C File Offset: 0x0005CB9C
		public definitionOrBlock_return definitionOrBlock()
		{
            var definitionOrBlock_return = new definitionOrBlock_return
            {
                Start = input.LT(1)
            };
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
						goto IL_93;
					case 6:
						num2 = 2;
						goto IL_93;
					case 7:
						num2 = 3;
						goto IL_93;
					default:
						if (num != 19)
						{
							goto IL_7C;
						}
						break;
					}
				}
				else
				{
					if (num == 51)
					{
						num2 = 4;
						goto IL_93;
					}
					if (num != 54)
					{
						goto IL_7C;
					}
				}
				num2 = 5;
				goto IL_93;
				IL_7C:
				var ex = new NoViableAltException("", 4, 0, input);
				throw ex;
				IL_93:
				switch (num2)
				{
				case 1:
				{
					PushFollow(FOLLOW_fieldDefinition_in_definitionOrBlock253);
					var fieldDefinition_return = fieldDefinition();
					state.followingStackPointer--;
					((script_scope)script_stack.Peek()).kobjVarDefinitions.Add(fieldDefinition_return?.ST);
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_function_in_definitionOrBlock264);
					var function_return = function("", "");
					state.followingStackPointer--;
					if ((function_return?.sName)?.ToLowerInvariant() == "onbeginstate")
					{
						((script_scope)script_stack.Peek()).bhasBeginStateEvent = true;
					}
					else if ((function_return?.sName)?.ToLowerInvariant() == "onendstate")
					{
						((script_scope)script_stack.Peek()).bhasEndStateEvent = true;
					}
					((script_scope)script_stack.Peek()).kobjEmptyState.Add(function_return?.ST);
					break;
				}
				case 3:
				{
					PushFollow(FOLLOW_eventFunc_in_definitionOrBlock277);
					var eventFunc_return = eventFunc("");
					state.followingStackPointer--;
					if ((eventFunc_return?.sName)?.ToLowerInvariant() == "onbeginstate")
					{
						((script_scope)script_stack.Peek()).bhasBeginStateEvent = true;
					}
					else if ((eventFunc_return?.sName)?.ToLowerInvariant() == "onendstate")
					{
						((script_scope)script_stack.Peek()).bhasEndStateEvent = true;
					}
					((script_scope)script_stack.Peek()).kobjEmptyState.Add(eventFunc_return?.ST);
					break;
				}
				case 4:
					PushFollow(FOLLOW_stateBlock_in_definitionOrBlock289);
					stateBlock();
					state.followingStackPointer--;
					break;
				case 5:
				{
					PushFollow(FOLLOW_propertyBlock_in_definitionOrBlock295);
					var propertyBlock_return = propertyBlock();
					state.followingStackPointer--;
					((script_scope)script_stack.Peek()).kobjPropDefinitions.Add(propertyBlock_return?.ST);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return definitionOrBlock_return;
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x0005ECE0 File Offset: 0x0005CEE0
		public fieldDefinition_return fieldDefinition()
		{
			fieldDefinition_stack.Push(new fieldDefinition_scope());
			var fieldDefinition_return = new fieldDefinition_return();
			fieldDefinition_return.Start = input.LT(1);
			constant_return constant_return = null;
			((fieldDefinition_scope)fieldDefinition_stack.Peek()).sinitialValue = "None";
			try
			{
				Match(input, 5, FOLLOW_VAR_in_fieldDefinition323);
				Match(input, 2, null);
				PushFollow(FOLLOW_type_in_fieldDefinition325);
				var type_return = type();
				state.followingStackPointer--;
				var commonTree = (CommonTree)Match(input, 38, FOLLOW_ID_in_fieldDefinition329);
				var commonTree2 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_fieldDefinition331);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 is 81 or >= 90 and <= 93)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_constant_in_fieldDefinition333);
					constant_return = constant();
					state.followingStackPointer--;
				}
				Match(input, 3, null);
				if ((CommonTree)constant_return?.Start != null)
				{
					((fieldDefinition_scope)fieldDefinition_stack.Peek()).sinitialValue = ((CommonTree)constant_return?.Start).Text;
				}
				fieldDefinition_return.ST = templateLib.GetInstanceOf("variableDef", new STAttrMap().Add("type", type_return?.sTypeString).Add("name", commonTree.Text).Add("userFlags", commonTree2.Text).Add("initialValue", ((fieldDefinition_scope)fieldDefinition_stack.Peek()).sinitialValue));
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			finally
			{
				fieldDefinition_stack.Pop();
			}
			return fieldDefinition_return;
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x0005EF28 File Offset: 0x0005D128
		public function_return function(string asState, string asPropertyName)
		{
			function_stack.Push(new function_scope());
			var function_return = new function_return();
			function_return.Start = input.LT(1);
			((function_scope)function_stack.Peek()).sstate = asState;
			((function_scope)function_stack.Peek()).sfuncName = "";
			((function_scope)function_stack.Peek()).spropertyName = asPropertyName;
			((function_scope)function_stack.Peek()).sreturnType = "";
			((function_scope)function_stack.Peek()).bisNative = false;
			((function_scope)function_stack.Peek()).bisGlobal = false;
			((function_scope)function_stack.Peek()).kfuncParams = new ArrayList();
			((function_scope)function_stack.Peek()).kfuncVarDefinitions = new ArrayList();
			((function_scope)function_stack.Peek()).kstatements = new ArrayList();
			((function_scope)function_stack.Peek()).suserFlags = "0";
			((function_scope)function_stack.Peek()).sdocString = "";
			try
			{
				Match(input, 6, FOLLOW_FUNCTION_in_function408);
				Match(input, 2, null);
				PushFollow(FOLLOW_functionHeader_in_function410);
				functionHeader();
				state.followingStackPointer--;
				var num = 2;
				var num2 = input.LA(1);
				if (num2 == 10)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_codeBlock_in_function412);
					codeBlock(((function_scope)function_stack.Peek()).kstatements, ((function_scope)function_stack.Peek()).kfuncVarDefinitions, ((function_scope)function_stack.Peek()).kfuncType.FunctionScope);
					state.followingStackPointer--;
				}
				Match(input, 3, null);
				function_return.ST = templateLib.GetInstanceOf("functionDef", new STAttrMap().Add("funcName", ((function_scope)function_stack.Peek()).sfuncName).Add("returnType", ((function_scope)function_stack.Peek()).sreturnType).Add("isNative", ((function_scope)function_stack.Peek()).bisNative).Add("isGlobal", ((function_scope)function_stack.Peek()).bisGlobal).Add("funcParams", ((function_scope)function_stack.Peek()).kfuncParams).Add("funcVars", ((function_scope)function_stack.Peek()).kfuncVarDefinitions).Add("userFlags", ((function_scope)function_stack.Peek()).suserFlags).Add("body", ((function_scope)function_stack.Peek()).kstatements).Add("docString", ((function_scope)function_stack.Peek()).sdocString));
				function_return.sName = ((function_scope)function_stack.Peek()).sfuncName;
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

		// Token: 0x06000D7D RID: 3453 RVA: 0x0005F2FC File Offset: 0x0005D4FC
		public functionHeader_return functionHeader()
		{
			var functionHeader_return = new functionHeader_return();
			functionHeader_return.Start = input.LT(1);
			CommonTree commonTree = null;
			CommonTree commonTree2 = null;
			callParameters_return callParameters_return = null;
			type_return type_return = null;
			try
			{
				Match(input, 8, FOLLOW_HEADER_in_functionHeader504);
				Match(input, 2, null);
				var num = input.LA(1);
				int num2;
				if (num is 38 or 55)
				{
					num2 = 1;
				}
				else
				{
					if (num != 92)
					{
						var ex = new NoViableAltException("", 7, 0, input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
					PushFollow(FOLLOW_type_in_functionHeader507);
					type_return = type();
					state.followingStackPointer--;
					break;
				case 2:
					commonTree = (CommonTree)Match(input, 92, FOLLOW_NONE_in_functionHeader511);
					break;
				}
				var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_functionHeader516);
				var commonTree4 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_functionHeader518);
				var num3 = 2;
				var num4 = input.LA(1);
				if (num4 == 9)
				{
					num3 = 1;
				}
				var num5 = num3;
				if (num5 == 1)
				{
					PushFollow(FOLLOW_callParameters_in_functionHeader520);
					callParameters_return = callParameters();
					state.followingStackPointer--;
				}
				for (;;)
				{
					var num6 = 2;
					var num7 = input.LA(1);
					if (num7 is >= 46 and <= 47)
					{
						num6 = 1;
					}
					var num8 = num6;
					if (num8 != 1)
					{
						break;
					}
					PushFollow(FOLLOW_functionModifier_in_functionHeader523);
					functionModifier();
					state.followingStackPointer--;
				}
				var num9 = 2;
				var num10 = input.LA(1);
				if (num10 == 40)
				{
					num9 = 1;
				}
				var num11 = num9;
				if (num11 == 1)
				{
					commonTree2 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_functionHeader526);
				}
				Match(input, 3, null);
				((function_scope)function_stack.Peek()).kfuncParams = callParameters_return?.kParams;
				((function_scope)function_stack.Peek()).sreturnType = (((CommonTree)type_return?.Start != null) ? type_return?.sTypeString : commonTree.Text);
				((function_scope)function_stack.Peek()).sfuncName = commonTree3.Text;
				((function_scope)function_stack.Peek()).suserFlags = commonTree4.Text;
				if (commonTree2 != null)
				{
					((function_scope)function_stack.Peek()).sdocString = commonTree2.Text;
				}
				if (((function_scope)function_stack.Peek()).spropertyName == "")
				{
					kObjType.TryGetFunction(((function_scope)function_stack.Peek()).sstate, ((function_scope)function_stack.Peek()).sfuncName, out ((function_scope)function_stack.Peek()).kfuncType);
				}
				else
				{
					ScriptPropertyType scriptPropertyType;
					kObjType.TryGetProperty(((function_scope)function_stack.Peek()).spropertyName, out scriptPropertyType);
					var a = ((function_scope)function_stack.Peek()).sfuncName.ToLowerInvariant();
					if (a == "get")
					{
						((function_scope)function_stack.Peek()).kfuncType = scriptPropertyType.kGetFunction;
					}
					else
					{
						((function_scope)function_stack.Peek()).kfuncType = scriptPropertyType.kSetFunction;
					}
				}
				MangleFunctionVariables(((function_scope)function_stack.Peek()).kfuncType);
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return functionHeader_return;
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x0005F70C File Offset: 0x0005D90C
		public functionModifier_return functionModifier()
		{
			var functionModifier_return = new functionModifier_return
            {
                Start = input.LT(1)
            };
            try
			{
				var num = input.LA(1);
				int num2;
				if (num == 47)
				{
					num2 = 1;
				}
				else
				{
					if (num != 46)
					{
						var ex = new NoViableAltException("", 11, 0, input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
					Match(input, 47, FOLLOW_NATIVE_in_functionModifier545);
					((function_scope)function_stack.Peek()).bisNative = true;
					break;
				case 2:
					Match(input, 46, FOLLOW_GLOBAL_in_functionModifier553);
					((function_scope)function_stack.Peek()).bisGlobal = true;
					break;
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return functionModifier_return;
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x0005F804 File Offset: 0x0005DA04
		public eventFunc_return eventFunc(string asState)
		{
			eventFunc_stack.Push(new eventFunc_scope());
			var eventFunc_return = new eventFunc_return();
			eventFunc_return.Start = input.LT(1);
			((eventFunc_scope)eventFunc_stack.Peek()).sstate = asState;
			((eventFunc_scope)eventFunc_stack.Peek()).sfuncName = "";
			((eventFunc_scope)eventFunc_stack.Peek()).sreturnType = "";
			((eventFunc_scope)eventFunc_stack.Peek()).bisNative = false;
			((eventFunc_scope)eventFunc_stack.Peek()).bisGlobal = false;
			((eventFunc_scope)eventFunc_stack.Peek()).kfuncParams = new ArrayList();
			((eventFunc_scope)eventFunc_stack.Peek()).kfuncVarDefinitions = new ArrayList();
			((eventFunc_scope)eventFunc_stack.Peek()).kstatements = new ArrayList();
			((eventFunc_scope)eventFunc_stack.Peek()).suserFlags = "0";
			((eventFunc_scope)eventFunc_stack.Peek()).sdocString = "";
			try
			{
				Match(input, 7, FOLLOW_EVENT_in_eventFunc588);
				Match(input, 2, null);
				PushFollow(FOLLOW_eventHeader_in_eventFunc590);
				eventHeader();
				state.followingStackPointer--;
				var num = 2;
				var num2 = input.LA(1);
				if (num2 == 10)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_codeBlock_in_eventFunc592);
					codeBlock(((eventFunc_scope)eventFunc_stack.Peek()).kstatements, ((eventFunc_scope)eventFunc_stack.Peek()).kfuncVarDefinitions, ((eventFunc_scope)eventFunc_stack.Peek()).kfuncType.FunctionScope);
					state.followingStackPointer--;
				}
				Match(input, 3, null);
				eventFunc_return.ST = templateLib.GetInstanceOf("functionDef", new STAttrMap().Add("funcName", ((eventFunc_scope)eventFunc_stack.Peek()).sfuncName).Add("returnType", ((eventFunc_scope)eventFunc_stack.Peek()).sreturnType).Add("isNative", ((eventFunc_scope)eventFunc_stack.Peek()).bisNative).Add("isGlobal", ((eventFunc_scope)eventFunc_stack.Peek()).bisGlobal).Add("funcParams", ((eventFunc_scope)eventFunc_stack.Peek()).kfuncParams).Add("funcVars", ((eventFunc_scope)eventFunc_stack.Peek()).kfuncVarDefinitions).Add("userFlags", ((eventFunc_scope)eventFunc_stack.Peek()).suserFlags).Add("body", ((eventFunc_scope)eventFunc_stack.Peek()).kstatements).Add("docString", ((eventFunc_scope)eventFunc_stack.Peek()).sdocString));
				eventFunc_return.sName = ((eventFunc_scope)eventFunc_stack.Peek()).sfuncName;
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

		// Token: 0x06000D80 RID: 3456 RVA: 0x0005FBC4 File Offset: 0x0005DDC4
		public eventHeader_return eventHeader()
		{
			var eventHeader_return = new eventHeader_return();
			eventHeader_return.Start = input.LT(1);
			CommonTree commonTree = null;
			CommonTree commonTree2 = null;
			callParameters_return callParameters_return = null;
			try
			{
				Match(input, 8, FOLLOW_HEADER_in_eventHeader684);
				Match(input, 2, null);
				var commonTree3 = (CommonTree)Match(input, 92, FOLLOW_NONE_in_eventHeader686);
				var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_eventHeader688);
				var commonTree5 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_eventHeader690);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 == 9)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_callParameters_in_eventHeader692);
					callParameters_return = callParameters();
					state.followingStackPointer--;
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
					commonTree = (CommonTree)Match(input, 47, FOLLOW_NATIVE_in_eventHeader695);
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
					commonTree2 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_eventHeader698);
				}
				Match(input, 3, null);
				((eventFunc_scope)eventFunc_stack.Peek()).kfuncParams = callParameters_return?.kParams;
				((eventFunc_scope)eventFunc_stack.Peek()).sreturnType = commonTree3.Text;
				((eventFunc_scope)eventFunc_stack.Peek()).sfuncName = commonTree4.Text;
				if (commonTree != null)
				{
					((eventFunc_scope)eventFunc_stack.Peek()).bisNative = true;
				}
				((eventFunc_scope)eventFunc_stack.Peek()).suserFlags = commonTree5.Text;
				if (commonTree2 != null)
				{
					((eventFunc_scope)eventFunc_stack.Peek()).sdocString = commonTree2.Text;
				}
				kObjType.TryGetFunction(((eventFunc_scope)eventFunc_stack.Peek()).sstate, ((eventFunc_scope)eventFunc_stack.Peek()).sfuncName, out ((eventFunc_scope)eventFunc_stack.Peek()).kfuncType);
				MangleFunctionVariables(((eventFunc_scope)eventFunc_stack.Peek()).kfuncType);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return eventHeader_return;
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x0005FE84 File Offset: 0x0005E084
		public callParameters_return callParameters()
		{
			var callParameters_return = new callParameters_return();
			callParameters_return.Start = input.LT(1);
			IList list = null;
			try
			{
				var num = 0;
				for (;;)
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
					PushFollow(FOLLOW_callParameter_in_callParameters725);
					var callParameter_return = callParameter();
					state.followingStackPointer--;
					if (list == null)
					{
						list = new ArrayList();
					}
					list.Add(callParameter_return.Template);
					num++;
				}
				if (num < 1)
				{
					var ex = new EarlyExitException(16, input);
					throw ex;
				}
				callParameters_return.kParams = list;
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return callParameters_return;
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x0005FF64 File Offset: 0x0005E164
		public callParameter_return callParameter()
		{
			var callParameter_return = new callParameter_return();
			callParameter_return.Start = input.LT(1);
			try
			{
				Match(input, 9, FOLLOW_PARAM_in_callParameter742);
				Match(input, 2, null);
				PushFollow(FOLLOW_type_in_callParameter744);
				var type_return = type();
				state.followingStackPointer--;
				var commonTree = (CommonTree)Match(input, 38, FOLLOW_ID_in_callParameter748);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 is 81 or >= 90 and <= 93)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_constant_in_callParameter750);
					constant();
					state.followingStackPointer--;
				}
				Match(input, 3, null);
				callParameter_return.ST = templateLib.GetInstanceOf("funcParam", new STAttrMap().Add("type", type_return?.sTypeString).Add("name", commonTree.Text));
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return callParameter_return;
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x000600C8 File Offset: 0x0005E2C8
		public stateBlock_return stateBlock()
		{
			var stateBlock_return = new stateBlock_return();
			stateBlock_return.Start = input.LT(1);
			CommonTree commonTree = null;
			IList list = null;
			try
			{
				Match(input, 51, FOLLOW_STATE_in_stateBlock787);
				Match(input, 2, null);
				var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_stateBlock789);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 == 50)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					commonTree = (CommonTree)Match(input, 50, FOLLOW_AUTO_in_stateBlock791);
				}
				for (;;)
				{
					var num4 = 2;
					var num5 = input.LA(1);
					if (num5 is >= 6 and <= 7)
					{
						num4 = 1;
					}
					var num6 = num4;
					if (num6 != 1)
					{
						break;
					}
					PushFollow(FOLLOW_stateFuncOrEvent_in_stateBlock797);
					var stateFuncOrEvent_return = stateFuncOrEvent(commonTree2.Text);
					state.followingStackPointer--;
					list ??= new ArrayList();
					list.Add(stateFuncOrEvent_return.Template);
				}
				Match(input, 3, null);
				var key = commonTree2.Text.ToLowerInvariant();
				var obj = ((script_scope)script_stack.Peek()).kstates[key];
				var val = "";
				if (obj != null)
				{
					val = obj.ToString();
				}
				var instanceOf = TemplateLib.GetInstanceOf("stateConcatinate");
				instanceOf.SetAttribute("prevText", val);
				instanceOf.SetAttribute("funcs", list);
				((script_scope)script_stack.Peek()).kstates[key] = instanceOf.ToString();
				if (commonTree != null)
				{
					((script_scope)script_stack.Peek()).sinitialState = commonTree2.Text;
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return stateBlock_return;
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x000602D4 File Offset: 0x0005E4D4
		public stateFuncOrEvent_return stateFuncOrEvent(string asStateName)
		{
			var stateFuncOrEvent_return = new stateFuncOrEvent_return();
			stateFuncOrEvent_return.Start = input.LT(1);
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
						var ex = new NoViableAltException("", 20, 0, input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					PushFollow(FOLLOW_function_in_stateFuncOrEvent819);
					var function_return = function(asStateName, "");
					state.followingStackPointer--;
					stateFuncOrEvent_return.ST = function_return?.ST;
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_eventFunc_in_stateFuncOrEvent832);
					var eventFunc_return = eventFunc(asStateName);
					state.followingStackPointer--;
					stateFuncOrEvent_return.ST = eventFunc_return?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return stateFuncOrEvent_return;
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x000603F4 File Offset: 0x0005E5F4
		public propertyBlock_return propertyBlock()
		{
			propertyBlock_stack.Push(new propertyBlock_scope());
			var propertyBlock_return = new propertyBlock_return();
			propertyBlock_return.Start = input.LT(1);
			((propertyBlock_scope)propertyBlock_stack.Peek()).spropName = "";
			((propertyBlock_scope)propertyBlock_stack.Peek()).spropType = "";
			((propertyBlock_scope)propertyBlock_stack.Peek()).suserFlags = "0";
			((propertyBlock_scope)propertyBlock_stack.Peek()).sdocString = "";
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
						var ex = new NoViableAltException("", 21, 0, input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					Match(input, 54, FOLLOW_PROPERTY_in_propertyBlock861);
					Match(input, 2, null);
					PushFollow(FOLLOW_propertyHeader_in_propertyBlock863);
					propertyHeader();
					state.followingStackPointer--;
					PushFollow(FOLLOW_propertyFunc_in_propertyBlock867);
					var propertyFunc_return = propertyFunc(((propertyBlock_scope)propertyBlock_stack.Peek()).spropName);
					state.followingStackPointer--;
					PushFollow(FOLLOW_propertyFunc_in_propertyBlock872);
					var propertyFunc_return2 = propertyFunc(((propertyBlock_scope)propertyBlock_stack.Peek()).spropName);
					state.followingStackPointer--;
					Match(input, 3, null);
					propertyBlock_return.ST = templateLib.GetInstanceOf("fullProp", new STAttrMap().Add("name", ((propertyBlock_scope)propertyBlock_stack.Peek()).spropName).Add("type", ((propertyBlock_scope)propertyBlock_stack.Peek()).spropType).Add("get", propertyFunc_return?.ST).Add("set", propertyFunc_return2?.ST).Add("userFlags", ((propertyBlock_scope)propertyBlock_stack.Peek()).suserFlags).Add("docString", ((propertyBlock_scope)propertyBlock_stack.Peek()).sdocString));
					break;
				}
				case 2:
				{
					Match(input, 19, FOLLOW_AUTOPROP_in_propertyBlock922);
					Match(input, 2, null);
					PushFollow(FOLLOW_propertyHeader_in_propertyBlock924);
					propertyHeader();
					state.followingStackPointer--;
					var commonTree = (CommonTree)Match(input, 38, FOLLOW_ID_in_propertyBlock928);
					Match(input, 3, null);
					propertyBlock_return.ST = templateLib.GetInstanceOf("autoProp", new STAttrMap().Add("name", ((propertyBlock_scope)propertyBlock_stack.Peek()).spropName).Add("type", ((propertyBlock_scope)propertyBlock_stack.Peek()).spropType).Add("var", commonTree.Text).Add("userFlags", ((propertyBlock_scope)propertyBlock_stack.Peek()).suserFlags).Add("docString", ((propertyBlock_scope)propertyBlock_stack.Peek()).sdocString));
					break;
				}
				}
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

		// Token: 0x06000D86 RID: 3462 RVA: 0x000607FC File Offset: 0x0005E9FC
		public propertyHeader_return propertyHeader()
		{
			var propertyHeader_return = new propertyHeader_return();
			propertyHeader_return.Start = input.LT(1);
			CommonTree commonTree = null;
			try
			{
				Match(input, 8, FOLLOW_HEADER_in_propertyHeader978);
				Match(input, 2, null);
				PushFollow(FOLLOW_type_in_propertyHeader980);
				var type_return = type();
				state.followingStackPointer--;
				var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_propertyHeader984);
				var commonTree3 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_propertyHeader986);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 == 40)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					commonTree = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_propertyHeader988);
				}
				Match(input, 3, null);
				((propertyBlock_scope)propertyBlock_stack.Peek()).spropName = commonTree2.Text;
				((propertyBlock_scope)propertyBlock_stack.Peek()).spropType = type_return?.sTypeString;
				((propertyBlock_scope)propertyBlock_stack.Peek()).suserFlags = commonTree3.Text;
				if (commonTree != null)
				{
					((propertyBlock_scope)propertyBlock_stack.Peek()).sdocString = commonTree.Text;
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return propertyHeader_return;
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x000609A0 File Offset: 0x0005EBA0
		public propertyFunc_return propertyFunc(string asPropName)
		{
			var propertyFunc_return = new propertyFunc_return();
			propertyFunc_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				if (num != 17)
				{
					var ex = new NoViableAltException("", 23, 0, input);
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
					if (num2 is not 3 and not 17)
					{
						var ex2 = new NoViableAltException("", 23, 1, input);
						throw ex2;
					}
					num3 = 2;
				}
				switch (num3)
				{
				case 1:
				{
					Match(input, 17, FOLLOW_PROPFUNC_in_propertyFunc1009);
					Match(input, 2, null);
					PushFollow(FOLLOW_function_in_propertyFunc1011);
					var function_return = function("", asPropName);
					state.followingStackPointer--;
					Match(input, 3, null);
					propertyFunc_return.ST = function_return?.ST;
					break;
				}
				case 2:
					Match(input, 17, FOLLOW_PROPFUNC_in_propertyFunc1025);
					break;
				}
			}
			catch (RecognitionException ex3)
			{
				ReportError(ex3);
				Recover(input, ex3);
			}
			return propertyFunc_return;
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x00060B08 File Offset: 0x0005ED08
		public codeBlock_return codeBlock(IList akStatements, IList akVarDefinitions, ScriptScope akCurrentScope)
		{
			codeBlock_stack.Push(new codeBlock_scope());
			var codeBlock_return = new codeBlock_return();
			codeBlock_return.Start = input.LT(1);
			((codeBlock_scope)codeBlock_stack.Peek()).kvarDefs = akVarDefinitions;
			((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope = akCurrentScope;
			((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild = 0;
			try
			{
				Match(input, 10, FOLLOW_BLOCK_in_codeBlock1049);
				if (input.LA(1) == 2)
				{
					Match(input, 2, null);
					for (;;)
					{
						var num = 2;
						var num2 = input.LA(1);
						if (num2 is 5 or >= 11 and <= 13 or 15 or 20 or 22 or >= 24 and <= 36 or 38 or 41 or 62 or >= 65 and <= 72 or >= 77 and <= 84 or 88 or >= 90 and <= 93)
						{
							num = 1;
						}
						var num3 = num;
						if (num3 != 1)
						{
							break;
						}
						PushFollow(FOLLOW_statement_in_codeBlock1057);
						var statement_return = statement();
						state.followingStackPointer--;
						if (statement_return?.ST != null)
						{
							akStatements.Add(statement_return?.ST);
						}
					}
					Match(input, 3, null);
				}
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

		// Token: 0x06000D89 RID: 3465 RVA: 0x00060CE8 File Offset: 0x0005EEE8
		public statement_return statement()
		{
			statement_stack.Push(new statement_scope());
			var statement_return = new statement_return();
			statement_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num2;
				switch (num)
				{
				case 5:
					num2 = 1;
					goto IL_1B3;
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 14:
				case 16:
				case 17:
				case 18:
				case 19:
				case 21:
				case 23:
				case 37:
				case 39:
				case 40:
					goto IL_19B;
				case 11:
				case 12:
				case 13:
				case 15:
				case 20:
				case 22:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
				case 35:
				case 36:
				case 38:
					break;
				case 41:
					num2 = 2;
					goto IL_1B3;
				default:
					switch (num)
					{
					case 62:
					case 65:
					case 66:
					case 67:
					case 68:
					case 69:
					case 70:
					case 71:
					case 72:
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
					case 73:
					case 74:
					case 75:
					case 76:
					case 85:
					case 86:
					case 87:
					case 89:
						goto IL_19B;
					case 83:
						num2 = 4;
						goto IL_1B3;
					case 84:
						num2 = 5;
						goto IL_1B3;
					case 88:
						num2 = 6;
						goto IL_1B3;
					default:
						goto IL_19B;
					}
					break;
				}
				num2 = 3;
				goto IL_1B3;
				IL_19B:
				var ex = new NoViableAltException("", 25, 0, input);
				throw ex;
				IL_1B3:
				switch (num2)
				{
				case 1:
				{
					PushFollow(FOLLOW_localDefinition_in_statement1086);
					var localDefinition_return = localDefinition();
					state.followingStackPointer--;
					((codeBlock_scope)codeBlock_stack.Peek()).kvarDefs.Add(localDefinition_return?.ST);
					statement_return.ST = localDefinition_return?.sExprVar != "" ? templateLib.GetInstanceOf("singleOpCommand", new STAttrMap().Add("command", "ASSIGN").Add("target", localDefinition_return?.sVarName).Add("source", localDefinition_return?.sExprVar).Add("autoCast", localDefinition_return?.kAutoCastST).Add("extraExpressions", localDefinition_return?.kExprST).Add("lineNo", localDefinition_return?.iLineNo ?? 0)) : null;
					break;
				}
				case 2:
				{
					var commonTree = (CommonTree)Match(input, 41, FOLLOW_EQUALS_in_statement1147);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_statement1149);
					PushFollow(FOLLOW_autoCast_in_statement1151);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_l_value_in_statement1153);
					var l_value_return = l_value();
					state.followingStackPointer--;
					PushFollow(FOLLOW_expression_in_statement1155);
					var expression_return = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					((statement_scope)statement_stack.Peek()).smangledName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					statement_return.ST = templateLib.GetInstanceOf("assign", new STAttrMap().Add("target", ((statement_scope)statement_stack.Peek()).smangledName).Add("targetExpressions", l_value_return?.ST).Add("source", autoCast_return?.sRetValue).Add("sourceExpressions", expression_return?.ST).Add("autoCast", autoCast_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 3:
				{
					PushFollow(FOLLOW_expression_in_statement1204);
					var expression_return2 = expression();
					state.followingStackPointer--;
					statement_return.ST = expression_return2?.ST;
					break;
				}
				case 4:
				{
					PushFollow(FOLLOW_return_stat_in_statement1215);
					var return_stat_return = return_stat();
					state.followingStackPointer--;
					statement_return.ST = return_stat_return?.ST;
					break;
				}
				case 5:
				{
					PushFollow(FOLLOW_ifBlock_in_statement1226);
					var ifBlock_return = ifBlock();
					state.followingStackPointer--;
					statement_return.ST = ifBlock_return?.ST;
					break;
				}
				case 6:
				{
					PushFollow(FOLLOW_whileBlock_in_statement1237);
					var whileBlock_return = whileBlock();
					state.followingStackPointer--;
					statement_return.ST = whileBlock_return?.ST;
					break;
				}
				}
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

		// Token: 0x06000D8A RID: 3466 RVA: 0x000612DC File Offset: 0x0005F4DC
		public localDefinition_return localDefinition()
		{
			var localDefinition_return = new localDefinition_return();
			localDefinition_return.Start = input.LT(1);
			expression_return expression_return = null;
			autoCast_return autoCast_return = null;
			try
			{
				Match(input, 5, FOLLOW_VAR_in_localDefinition1260);
				Match(input, 2, null);
				PushFollow(FOLLOW_type_in_localDefinition1262);
				var type_return = type();
				state.followingStackPointer--;
				var commonTree = (CommonTree)Match(input, 38, FOLLOW_ID_in_localDefinition1266);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 is 38 or 79 or 81 or >= 90 and <= 93)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					PushFollow(FOLLOW_autoCast_in_localDefinition1269);
					autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_expression_in_localDefinition1271);
					expression_return = expression();
					state.followingStackPointer--;
				}
				Match(input, 3, null);
				localDefinition_return.sVarName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree.Text);
				if ((CommonTree)expression_return?.Start != null)
				{
					localDefinition_return.sExprVar = autoCast_return?.sRetValue;
					localDefinition_return.kAutoCastST = autoCast_return?.ST;
					localDefinition_return.kExprST = expression_return?.ST;
					localDefinition_return.iLineNo = commonTree.Line;
				}
				else
				{
					localDefinition_return.sExprVar = "";
					localDefinition_return.kAutoCastST = null;
					localDefinition_return.kExprST = null;
					localDefinition_return.iLineNo = commonTree.Line;
				}
				localDefinition_return.ST = templateLib.GetInstanceOf("localDef", new STAttrMap().Add("type", type_return?.sTypeString).Add("name", localDefinition_return.sVarName));
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return localDefinition_return;
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x00061520 File Offset: 0x0005F720
		public l_value_return l_value()
		{
			l_value_stack.Push(new l_value_scope());
			var l_value_return = new l_value_return();
			l_value_return.Start = input.LT(1);
			try
			{
				switch (dfa27.Predict(input))
				{
				case 1:
				{
					Match(input, 62, FOLLOW_DOT_in_l_value1318);
					Match(input, 2, null);
					Match(input, 15, FOLLOW_PAREXPR_in_l_value1321);
					Match(input, 2, null);
					PushFollow(FOLLOW_expression_in_l_value1325);
					var expression_return = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					PushFollow(FOLLOW_property_set_in_l_value1330);
					var property_set_return = property_set();
					state.followingStackPointer--;
					Match(input, 3, null);
					l_value_return.ST = templateLib.GetInstanceOf("dot", new STAttrMap().Add("aTemplate", expression_return?.ST).Add("bTemplate", property_set_return?.ST));
					break;
				}
				case 2:
				{
					var commonTree = (CommonTree)Match(input, 23, FOLLOW_ARRAYSET_in_l_value1355);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_l_value1359);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_l_value1363);
					PushFollow(FOLLOW_autoCast_in_l_value1365);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					Match(input, 15, FOLLOW_PAREXPR_in_l_value1368);
					Match(input, 2, null);
					PushFollow(FOLLOW_expression_in_l_value1372);
					var expression_return2 = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					PushFollow(FOLLOW_expression_in_l_value1377);
					var expression_return3 = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					((l_value_scope)l_value_stack.Peek()).ssourceName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					((l_value_scope)l_value_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					l_value_return.ST = templateLib.GetInstanceOf("arraySet", new STAttrMap().Add("sourceName", ((l_value_scope)l_value_stack.Peek()).ssourceName).Add("selfName", ((l_value_scope)l_value_stack.Peek()).sselfName).Add("index", autoCast_return?.sRetValue).Add("autoCast", autoCast_return?.ST).Add("arrayExpressions", expression_return2?.ST).Add("indexExpressions", expression_return3?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 3:
				{
					PushFollow(FOLLOW_basic_l_value_in_l_value1431);
					var basic_l_value_return = basic_l_value();
					state.followingStackPointer--;
					l_value_return.ST = basic_l_value_return?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			finally
			{
				l_value_stack.Pop();
			}
			return l_value_return;
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x00061980 File Offset: 0x0005FB80
		public basic_l_value_return basic_l_value()
		{
			basic_l_value_stack.Push(new basic_l_value_scope());
			var basic_l_value_return = new basic_l_value_return();
			basic_l_value_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num2;
				if (num <= 25)
				{
					switch (num)
					{
					case 11:
					case 12:
					case 13:
						break;
					default:
						switch (num)
						{
						case 21:
							num2 = 3;
							goto IL_CD;
						case 22:
							goto IL_B5;
						case 23:
							num2 = 4;
							goto IL_CD;
						case 24:
						case 25:
							break;
						default:
							goto IL_B5;
						}
						break;
					}
					num2 = 2;
					goto IL_CD;
				}
				if (num == 38)
				{
					num2 = 5;
					goto IL_CD;
				}
				if (num == 62)
				{
					num2 = 1;
					goto IL_CD;
				}
				IL_B5:
				var ex = new NoViableAltException("", 28, 0, input);
				throw ex;
				IL_CD:
				switch (num2)
				{
				case 1:
				{
					Match(input, 62, FOLLOW_DOT_in_basic_l_value1454);
					Match(input, 2, null);
					PushFollow(FOLLOW_array_func_or_id_in_basic_l_value1458);
					var array_func_or_id_return = array_func_or_id();
					state.followingStackPointer--;
					PushFollow(FOLLOW_basic_l_value_in_basic_l_value1462);
					var basic_l_value_return2 = basic_l_value();
					state.followingStackPointer--;
					Match(input, 3, null);
					basic_l_value_return.ST = templateLib.GetInstanceOf("dot", new STAttrMap().Add("aTemplate", array_func_or_id_return?.ST).Add("bTemplate", basic_l_value_return2?.ST));
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_function_call_in_basic_l_value1486);
					var function_call_return = function_call();
					state.followingStackPointer--;
					basic_l_value_return.ST = function_call_return?.ST;
					break;
				}
				case 3:
				{
					PushFollow(FOLLOW_property_set_in_basic_l_value1497);
					var property_set_return = property_set();
					state.followingStackPointer--;
					basic_l_value_return.ST = property_set_return?.ST;
					break;
				}
				case 4:
				{
					var commonTree = (CommonTree)Match(input, 23, FOLLOW_ARRAYSET_in_basic_l_value1509);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_basic_l_value1513);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_basic_l_value1517);
					PushFollow(FOLLOW_autoCast_in_basic_l_value1519);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_func_or_id_in_basic_l_value1521);
					var func_or_id_return = func_or_id();
					state.followingStackPointer--;
					PushFollow(FOLLOW_expression_in_basic_l_value1523);
					var expression_return = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					((basic_l_value_scope)basic_l_value_stack.Peek()).ssourceName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					((basic_l_value_scope)basic_l_value_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					basic_l_value_return.ST = templateLib.GetInstanceOf("arraySet", new STAttrMap().Add("sourceName", ((basic_l_value_scope)basic_l_value_stack.Peek()).ssourceName).Add("selfName", ((basic_l_value_scope)basic_l_value_stack.Peek()).sselfName).Add("index", autoCast_return?.sRetValue).Add("autoCast", autoCast_return?.ST).Add("arrayExpressions", func_or_id_return?.ST).Add("indexExpressions", expression_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 5:
					Match(input, 38, FOLLOW_ID_in_basic_l_value1577);
					break;
				}
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

		// Token: 0x06000D8D RID: 3469 RVA: 0x00061E50 File Offset: 0x00060050
		public expression_return expression()
		{
			var expression_return = new expression_return();
			expression_return.Start = input.LT(1);
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
					if (num is (< 11 or > 13) and not 15 and not 20 and not 22 and (< 24 or > 36) and not 38 and not 62 and (< 66 or > 72) and (< 77 or > 82) and (< 90 or > 93))
					{
						var ex = new NoViableAltException("", 29, 0, input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 65, FOLLOW_OR_in_expression1595);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_expression1597);
					PushFollow(FOLLOW_expression_in_expression1601);
					var expression_return2 = expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_and_expression_in_expression1605);
					var and_expression_return = and_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					expression_return.ST = templateLib.GetInstanceOf("orExpression", new STAttrMap().Add("target", expression_return.sRetValue).Add("arg1", expression_return2?.sRetValue).Add("arg2", and_expression_return?.sRetValue).Add("extraExpressions1", expression_return2?.ST).Add("extraExpressions2", and_expression_return?.ST).Add("endLabel", GenerateLabel()).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_and_expression_in_expression1659);
					var and_expression_return2 = and_expression();
					state.followingStackPointer--;
					expression_return.sRetValue = and_expression_return2?.sRetValue;
					expression_return.ST = and_expression_return2?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return expression_return;
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x00062120 File Offset: 0x00060320
		public and_expression_return and_expression()
		{
			var and_expression_return = new and_expression_return();
			and_expression_return.Start = input.LT(1);
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
					if (num is (< 11 or > 13) and not 15 and not 20 and not 22 and (< 24 or > 36) and not 38 and not 62 and (< 67 or > 72) and (< 77 or > 82) and (< 90 or > 93))
					{
						var ex = new NoViableAltException("", 30, 0, input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 66, FOLLOW_AND_in_and_expression1681);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_and_expression1683);
					PushFollow(FOLLOW_and_expression_in_and_expression1687);
					var and_expression_return2 = and_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_bool_expression_in_and_expression1691);
					var bool_expression_return = bool_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					and_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					and_expression_return.ST = templateLib.GetInstanceOf("andExpression", new STAttrMap().Add("target", and_expression_return.sRetValue).Add("arg1", and_expression_return2?.sRetValue).Add("arg2", bool_expression_return?.sRetValue).Add("extraExpressions1", and_expression_return2?.ST).Add("extraExpressions2", bool_expression_return?.ST).Add("endLabel", GenerateLabel()).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_bool_expression_in_and_expression1745);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					and_expression_return.sRetValue = bool_expression_return2?.sRetValue;
					and_expression_return.ST = bool_expression_return2?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return and_expression_return;
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x000623F0 File Offset: 0x000605F0
		public bool_expression_return bool_expression()
		{
			var bool_expression_return = new bool_expression_return();
			bool_expression_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num2;
				switch (num)
				{
				case 11:
				case 12:
				case 13:
				case 15:
				case 20:
				case 22:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
				case 35:
				case 36:
				case 38:
					break;
				case 14:
				case 16:
				case 17:
				case 18:
				case 19:
				case 21:
				case 23:
				case 37:
					goto IL_182;
				default:
					switch (num)
					{
					case 62:
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
					case 73:
					case 74:
					case 75:
					case 76:
					case 83:
					case 84:
					case 85:
					case 86:
					case 87:
					case 88:
					case 89:
						goto IL_182;
					case 67:
						num2 = 1;
						goto IL_19A;
					case 68:
						num2 = 2;
						goto IL_19A;
					case 69:
						num2 = 3;
						goto IL_19A;
					case 70:
						num2 = 4;
						goto IL_19A;
					case 71:
						num2 = 5;
						goto IL_19A;
					case 72:
						num2 = 6;
						goto IL_19A;
					default:
						goto IL_182;
					}
					break;
				}
				num2 = 7;
				goto IL_19A;
				IL_182:
				var ex = new NoViableAltException("", 31, 0, input);
				throw ex;
				IL_19A:
				switch (num2)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 67, FOLLOW_EQ_in_bool_expression1767);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1769);
					PushFollow(FOLLOW_autoCast_in_bool_expression1773);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_bool_expression1777);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_bool_expression_in_bool_expression1781);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_bool_expression1785);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					bool_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					bool_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "COMPAREEQ").Add("target", bool_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", bool_expression_return2?.ST).Add("extraExpressions2", add_expression_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					var commonTree3 = (CommonTree)Match(input, 68, FOLLOW_NE_in_bool_expression1850);
					Match(input, 2, null);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1852);
					PushFollow(FOLLOW_autoCast_in_bool_expression1856);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_bool_expression1860);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_bool_expression_in_bool_expression1864);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_bool_expression1868);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					bool_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					bool_expression_return.ST = templateLib.GetInstanceOf("notEqual", new STAttrMap().Add("target", bool_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", bool_expression_return2?.ST).Add("extraExpressions2", add_expression_return?.ST).Add("lineNo", commonTree3.Line));
					break;
				}
				case 3:
				{
					var commonTree5 = (CommonTree)Match(input, 69, FOLLOW_GT_in_bool_expression1928);
					Match(input, 2, null);
					var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1930);
					PushFollow(FOLLOW_autoCast_in_bool_expression1934);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_bool_expression1938);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_bool_expression_in_bool_expression1942);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_bool_expression1946);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					bool_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					bool_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "COMPAREGT").Add("target", bool_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", bool_expression_return2?.ST).Add("extraExpressions2", add_expression_return?.ST).Add("lineNo", commonTree5.Line));
					break;
				}
				case 4:
				{
					var commonTree7 = (CommonTree)Match(input, 70, FOLLOW_LT_in_bool_expression2011);
					Match(input, 2, null);
					var commonTree8 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression2013);
					PushFollow(FOLLOW_autoCast_in_bool_expression2017);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_bool_expression2021);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_bool_expression_in_bool_expression2025);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_bool_expression2029);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					bool_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree8.Text);
					bool_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "COMPARELT").Add("target", bool_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", bool_expression_return2?.ST).Add("extraExpressions2", add_expression_return?.ST).Add("lineNo", commonTree7.Line));
					break;
				}
				case 5:
				{
					var commonTree9 = (CommonTree)Match(input, 71, FOLLOW_GTE_in_bool_expression2094);
					Match(input, 2, null);
					var commonTree10 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression2096);
					PushFollow(FOLLOW_autoCast_in_bool_expression2100);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_bool_expression2104);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_bool_expression_in_bool_expression2108);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_bool_expression2112);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					bool_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree10.Text);
					bool_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "COMPAREGTE").Add("target", bool_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", bool_expression_return2?.ST).Add("extraExpressions2", add_expression_return?.ST).Add("lineNo", commonTree9.Line));
					break;
				}
				case 6:
				{
					var commonTree11 = (CommonTree)Match(input, 72, FOLLOW_LTE_in_bool_expression2177);
					Match(input, 2, null);
					var commonTree12 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression2179);
					PushFollow(FOLLOW_autoCast_in_bool_expression2183);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_bool_expression2187);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_bool_expression_in_bool_expression2191);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_bool_expression2195);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					bool_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree12.Text);
					bool_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "COMPARELTE").Add("target", bool_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", bool_expression_return2?.ST).Add("extraExpressions2", add_expression_return?.ST).Add("lineNo", commonTree11.Line));
					break;
				}
				case 7:
				{
					PushFollow(FOLLOW_add_expression_in_bool_expression2259);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					bool_expression_return.sRetValue = add_expression_return2?.sRetValue;
					bool_expression_return.ST = add_expression_return2?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return bool_expression_return;
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x000631DC File Offset: 0x000613DC
		public add_expression_return add_expression()
		{
			var add_expression_return = new add_expression_return();
			add_expression_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num2;
				switch (num)
				{
				case 11:
				case 12:
				case 13:
				case 15:
				case 20:
				case 22:
				case 24:
				case 25:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
				case 35:
				case 38:
					break;
				case 14:
				case 16:
				case 17:
				case 18:
				case 19:
				case 21:
				case 23:
				case 37:
					goto IL_141;
				case 26:
					num2 = 1;
					goto IL_159;
				case 27:
					num2 = 2;
					goto IL_159;
				case 28:
					num2 = 3;
					goto IL_159;
				case 29:
					num2 = 4;
					goto IL_159;
				case 36:
					num2 = 5;
					goto IL_159;
				default:
					if (num != 62)
					{
						switch (num)
						{
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
						case 84:
						case 85:
						case 86:
						case 87:
						case 88:
						case 89:
							goto IL_141;
						default:
							goto IL_141;
						}
					}
					break;
				}
				num2 = 6;
				goto IL_159;
				IL_141:
				var ex = new NoViableAltException("", 32, 0, input);
				throw ex;
				IL_159:
				switch (num2)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 26, FOLLOW_IADD_in_add_expression2281);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression2283);
					PushFollow(FOLLOW_autoCast_in_add_expression2287);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_add_expression2291);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_add_expression2295);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_add_expression2299);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					add_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					add_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "IADD").Add("target", add_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", add_expression_return2?.ST).Add("extraExpressions2", mult_expression_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					var commonTree3 = (CommonTree)Match(input, 27, FOLLOW_FADD_in_add_expression2364);
					Match(input, 2, null);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression2366);
					PushFollow(FOLLOW_autoCast_in_add_expression2370);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_add_expression2374);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_add_expression2378);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_add_expression2382);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					add_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					add_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "FADD").Add("target", add_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", add_expression_return2?.ST).Add("extraExpressions2", mult_expression_return?.ST).Add("lineNo", commonTree3.Line));
					break;
				}
				case 3:
				{
					var commonTree5 = (CommonTree)Match(input, 28, FOLLOW_ISUBTRACT_in_add_expression2447);
					Match(input, 2, null);
					var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression2449);
					PushFollow(FOLLOW_autoCast_in_add_expression2453);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_add_expression2457);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_add_expression2461);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_add_expression2465);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					add_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					add_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "ISUBTRACT").Add("target", add_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", add_expression_return2?.ST).Add("extraExpressions2", mult_expression_return?.ST).Add("lineNo", commonTree5.Line));
					break;
				}
				case 4:
				{
					var commonTree7 = (CommonTree)Match(input, 29, FOLLOW_FSUBTRACT_in_add_expression2530);
					Match(input, 2, null);
					var commonTree8 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression2532);
					PushFollow(FOLLOW_autoCast_in_add_expression2536);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_add_expression2540);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_add_expression2544);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_add_expression2548);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					add_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree8.Text);
					add_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "FSUBTRACT").Add("target", add_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", add_expression_return2?.ST).Add("extraExpressions2", mult_expression_return?.ST).Add("lineNo", commonTree7.Line));
					break;
				}
				case 5:
				{
					var commonTree9 = (CommonTree)Match(input, 36, FOLLOW_STRCAT_in_add_expression2613);
					Match(input, 2, null);
					var commonTree10 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression2615);
					PushFollow(FOLLOW_autoCast_in_add_expression2619);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_add_expression2623);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_add_expression_in_add_expression2627);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_add_expression2631);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					add_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree10.Text);
					add_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "STRCAT").Add("target", add_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", add_expression_return2?.ST).Add("extraExpressions2", mult_expression_return?.ST).Add("lineNo", commonTree9.Line));
					break;
				}
				case 6:
				{
					PushFollow(FOLLOW_mult_expression_in_add_expression2695);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					add_expression_return.sRetValue = mult_expression_return2?.sRetValue;
					add_expression_return.ST = mult_expression_return2?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return add_expression_return;
		}

		// Token: 0x06000D91 RID: 3473 RVA: 0x00063DA0 File Offset: 0x00061FA0
		public mult_expression_return mult_expression()
		{
			var mult_expression_return = new mult_expression_return();
			mult_expression_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num2;
				switch (num)
				{
				case 11:
				case 12:
				case 13:
				case 15:
				case 20:
				case 22:
				case 24:
				case 25:
				case 34:
				case 35:
				case 38:
					break;
				case 14:
				case 16:
				case 17:
				case 18:
				case 19:
				case 21:
				case 23:
				case 26:
				case 27:
				case 28:
				case 29:
				case 36:
				case 37:
					goto IL_141;
				case 30:
					num2 = 1;
					goto IL_159;
				case 31:
					num2 = 2;
					goto IL_159;
				case 32:
					num2 = 3;
					goto IL_159;
				case 33:
					num2 = 4;
					goto IL_159;
				default:
					if (num != 62)
					{
						switch (num)
						{
						case 77:
							num2 = 5;
							goto IL_159;
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
							goto IL_141;
						default:
							goto IL_141;
						}
					}
					break;
				}
				num2 = 6;
				goto IL_159;
				IL_141:
				var ex = new NoViableAltException("", 33, 0, input);
				throw ex;
				IL_159:
				switch (num2)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 30, FOLLOW_IMULTIPLY_in_mult_expression2718);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression2720);
					PushFollow(FOLLOW_autoCast_in_mult_expression2724);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_mult_expression2728);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_mult_expression2732);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_unary_expression_in_mult_expression2736);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					mult_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					mult_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "IMULTIPLY").Add("target", mult_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", mult_expression_return2?.ST).Add("extraExpressions2", unary_expression_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					var commonTree3 = (CommonTree)Match(input, 31, FOLLOW_FMULTIPLY_in_mult_expression2801);
					Match(input, 2, null);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression2803);
					PushFollow(FOLLOW_autoCast_in_mult_expression2807);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_mult_expression2811);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_mult_expression2815);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_unary_expression_in_mult_expression2819);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					mult_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					mult_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "FMULTIPLY").Add("target", mult_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", mult_expression_return2?.ST).Add("extraExpressions2", unary_expression_return?.ST).Add("lineNo", commonTree3.Line));
					break;
				}
				case 3:
				{
					var commonTree5 = (CommonTree)Match(input, 32, FOLLOW_IDIVIDE_in_mult_expression2884);
					Match(input, 2, null);
					var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression2886);
					PushFollow(FOLLOW_autoCast_in_mult_expression2890);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_mult_expression2894);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_mult_expression2898);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_unary_expression_in_mult_expression2902);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					mult_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					mult_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "IDIVIDE").Add("target", mult_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", mult_expression_return2?.ST).Add("extraExpressions2", unary_expression_return?.ST).Add("lineNo", commonTree5.Line));
					break;
				}
				case 4:
				{
					var commonTree7 = (CommonTree)Match(input, 33, FOLLOW_FDIVIDE_in_mult_expression2967);
					Match(input, 2, null);
					var commonTree8 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression2969);
					PushFollow(FOLLOW_autoCast_in_mult_expression2973);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_autoCast_in_mult_expression2977);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_mult_expression_in_mult_expression2981);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_unary_expression_in_mult_expression2985);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					mult_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree8.Text);
					mult_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "FDIVIDE").Add("target", mult_expression_return.sRetValue).Add("arg1", autoCast_return?.sRetValue).Add("arg2", autoCast_return2?.sRetValue).Add("autoCast1", autoCast_return?.ST).Add("autoCast2", autoCast_return2?.ST).Add("extraExpressions1", mult_expression_return2?.ST).Add("extraExpressions2", unary_expression_return?.ST).Add("lineNo", commonTree7.Line));
					break;
				}
				case 5:
				{
					var commonTree9 = (CommonTree)Match(input, 77, FOLLOW_MOD_in_mult_expression3050);
					Match(input, 2, null);
					var commonTree10 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression3052);
					PushFollow(FOLLOW_mult_expression_in_mult_expression3056);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					PushFollow(FOLLOW_unary_expression_in_mult_expression3060);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					mult_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree10.Text);
					mult_expression_return.ST = templateLib.GetInstanceOf("twoOpCommand", new STAttrMap().Add("command", "IMOD").Add("target", mult_expression_return.sRetValue).Add("arg1", mult_expression_return2?.sRetValue).Add("arg2", unary_expression_return?.sRetValue).Add("extraExpressions1", mult_expression_return2?.ST).Add("extraExpressions2", unary_expression_return?.ST).Add("lineNo", commonTree9.Line));
					break;
				}
				case 6:
				{
					PushFollow(FOLLOW_unary_expression_in_mult_expression3114);
					var unary_expression_return2 = unary_expression();
					state.followingStackPointer--;
					mult_expression_return.sRetValue = unary_expression_return2?.sRetValue;
					mult_expression_return.ST = unary_expression_return2?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return mult_expression_return;
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x000648E8 File Offset: 0x00062AE8
		public unary_expression_return unary_expression()
		{
			var unary_expression_return = new unary_expression_return();
			unary_expression_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num2;
				if (num <= 38)
				{
					switch (num)
					{
					case 11:
					case 12:
					case 13:
					case 15:
					case 20:
					case 22:
					case 24:
					case 25:
						break;
					case 14:
					case 16:
					case 17:
					case 18:
					case 19:
					case 21:
					case 23:
						goto IL_116;
					default:
						switch (num)
						{
						case 34:
							num2 = 1;
							goto IL_12E;
						case 35:
							num2 = 2;
							goto IL_12E;
						case 36:
						case 37:
							goto IL_116;
						case 38:
							break;
						default:
							goto IL_116;
						}
						break;
					}
				}
				else if (num != 62)
				{
					switch (num)
					{
					case 78:
						num2 = 3;
						goto IL_12E;
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
						goto IL_116;
					default:
						goto IL_116;
					}
				}
				num2 = 4;
				goto IL_12E;
				IL_116:
				var ex = new NoViableAltException("", 34, 0, input);
				throw ex;
				IL_12E:
				switch (num2)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 34, FOLLOW_INEGATE_in_unary_expression3137);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_unary_expression3139);
					PushFollow(FOLLOW_cast_atom_in_unary_expression3141);
					var cast_atom_return = cast_atom();
					state.followingStackPointer--;
					Match(input, 3, null);
					unary_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					unary_expression_return.ST = templateLib.GetInstanceOf("singleOpCommand", new STAttrMap().Add("command", "INEGATE").Add("target", unary_expression_return.sRetValue).Add("source", cast_atom_return?.sRetValue).Add("extraExpressions", cast_atom_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					var commonTree3 = (CommonTree)Match(input, 35, FOLLOW_FNEGATE_in_unary_expression3186);
					Match(input, 2, null);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_unary_expression3188);
					PushFollow(FOLLOW_cast_atom_in_unary_expression3190);
					var cast_atom_return2 = cast_atom();
					state.followingStackPointer--;
					Match(input, 3, null);
					unary_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					unary_expression_return.ST = templateLib.GetInstanceOf("singleOpCommand", new STAttrMap().Add("command", "FNEGATE").Add("target", unary_expression_return.sRetValue).Add("source", cast_atom_return2?.sRetValue).Add("extraExpressions", cast_atom_return2?.ST).Add("lineNo", commonTree3.Line));
					break;
				}
				case 3:
				{
					var commonTree5 = (CommonTree)Match(input, 78, FOLLOW_NOT_in_unary_expression3235);
					Match(input, 2, null);
					var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_unary_expression3237);
					PushFollow(FOLLOW_cast_atom_in_unary_expression3239);
					var cast_atom_return3 = cast_atom();
					state.followingStackPointer--;
					Match(input, 3, null);
					unary_expression_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					unary_expression_return.ST = templateLib.GetInstanceOf("singleOpCommand", new STAttrMap().Add("command", "NOT").Add("target", unary_expression_return.sRetValue).Add("source", cast_atom_return3?.sRetValue).Add("extraExpressions", cast_atom_return3?.ST).Add("lineNo", commonTree5.Line));
					break;
				}
				case 4:
				{
					PushFollow(FOLLOW_cast_atom_in_unary_expression3283);
					var cast_atom_return4 = cast_atom();
					state.followingStackPointer--;
					unary_expression_return.sRetValue = cast_atom_return4?.sRetValue;
					unary_expression_return.ST = cast_atom_return4?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return unary_expression_return;
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x00064E20 File Offset: 0x00063020
		public cast_atom_return cast_atom()
		{
			var cast_atom_return = new cast_atom_return();
			cast_atom_return.Start = input.LT(1);
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
					if (num is (< 11 or > 13) and not 15 and not 20 and not 22 and (< 24 or > 25) and not 38 and not 62 and (< 80 or > 82) and (< 90 or > 93))
					{
						var ex = new NoViableAltException("", 35, 0, input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 79, FOLLOW_AS_in_cast_atom3306);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_cast_atom3308);
					PushFollow(FOLLOW_dot_atom_in_cast_atom3310);
					var dot_atom_return = dot_atom();
					state.followingStackPointer--;
					Match(input, 3, null);
					cast_atom_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					cast_atom_return.ST = templateLib.GetInstanceOf("cast", new STAttrMap().Add("target", cast_atom_return.sRetValue).Add("source", dot_atom_return?.sRetValue).Add("extraExpressions", dot_atom_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_dot_atom_in_cast_atom3349);
					var dot_atom_return2 = dot_atom();
					state.followingStackPointer--;
					cast_atom_return.sRetValue = dot_atom_return2?.sRetValue;
					cast_atom_return.ST = dot_atom_return2?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return cast_atom_return;
		}

		// Token: 0x06000D94 RID: 3476 RVA: 0x0006507C File Offset: 0x0006327C
		public dot_atom_return dot_atom()
		{
			var dot_atom_return = new dot_atom_return
            {
                Start = input.LT(1)
            };
            try
			{
				var num = input.LA(1);
				int num2;
				if (num <= 38)
				{
					switch (num)
					{
					case 11:
					case 12:
					case 13:
					case 15:
					case 20:
					case 22:
					case 24:
					case 25:
						break;
					case 14:
					case 16:
					case 17:
					case 18:
					case 19:
					case 21:
					case 23:
						goto IL_CD;
					default:
						if (num != 38)
						{
							goto IL_CD;
						}
						break;
					}
				}
				else
				{
					if (num != 62)
					{
						switch (num)
						{
						case 80:
						case 82:
							goto IL_C3;
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
								goto IL_CD;
							}
							break;
						}
						num2 = 3;
						goto IL_E5;
					}
					num2 = 1;
					goto IL_E5;
				}
				IL_C3:
				num2 = 2;
				goto IL_E5;
				IL_CD:
				var ex = new NoViableAltException("", 36, 0, input);
				throw ex;
				IL_E5:
				switch (num2)
				{
				case 1:
				{
					Match(input, 62, FOLLOW_DOT_in_dot_atom3372);
					Match(input, 2, null);
					PushFollow(FOLLOW_dot_atom_in_dot_atom3376);
					var dot_atom_return2 = dot_atom();
					state.followingStackPointer--;
					PushFollow(FOLLOW_array_func_or_id_in_dot_atom3380);
					var array_func_or_id_return = array_func_or_id();
					state.followingStackPointer--;
					Match(input, 3, null);
					dot_atom_return.sRetValue = array_func_or_id_return?.sRetValue;
					dot_atom_return.ST = templateLib.GetInstanceOf("dot", new STAttrMap().Add("aTemplate", dot_atom_return2?.ST).Add("bTemplate", array_func_or_id_return?.ST));
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_array_atom_in_dot_atom3409);
					var array_atom_return = array_atom();
					state.followingStackPointer--;
					dot_atom_return.sRetValue = array_atom_return?.sRetValue;
					dot_atom_return.ST = array_atom_return?.ST;
					break;
				}
				case 3:
				{
					PushFollow(FOLLOW_constant_in_dot_atom3420);
					var constant_return = constant();
					state.followingStackPointer--;
					dot_atom_return.sRetValue = ((CommonTree)constant_return?.Start)?.Text;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return dot_atom_return;
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x00065330 File Offset: 0x00063530
		public array_atom_return array_atom()
		{
			array_atom_stack.Push(new array_atom_scope());
			var array_atom_return = new array_atom_return();
			array_atom_return.Start = input.LT(1);
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
					if (num is (< 11 or > 13) and not 15 and not 20 and (< 24 or > 25) and not 38 and not 80 and not 82)
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
					var commonTree = (CommonTree)Match(input, 22, FOLLOW_ARRAYGET_in_array_atom3447);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_array_atom3451);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_array_atom3455);
					PushFollow(FOLLOW_autoCast_in_array_atom3457);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_atom_in_array_atom3459);
					var atom_return = atom();
					state.followingStackPointer--;
					PushFollow(FOLLOW_expression_in_array_atom3461);
					var expression_return = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					array_atom_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					((array_atom_scope)array_atom_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					array_atom_return.ST = templateLib.GetInstanceOf("arrayGet", new STAttrMap().Add("retValue", array_atom_return.sRetValue).Add("selfName", ((array_atom_scope)array_atom_stack.Peek()).sselfName).Add("index", autoCast_return?.sRetValue).Add("autoCast", autoCast_return?.ST).Add("arrayExpressions", atom_return?.ST).Add("indexExpressions", expression_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_atom_in_array_atom3515);
					var atom_return2 = atom();
					state.followingStackPointer--;
					array_atom_return.sRetValue = atom_return2?.sRetValue;
					array_atom_return.ST = atom_return2?.ST;
					break;
				}
				}
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

		// Token: 0x06000D96 RID: 3478 RVA: 0x000656A0 File Offset: 0x000638A0
		public atom_return atom()
		{
			var atom_return = new atom_return();
			atom_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num2;
				if (num <= 20)
				{
					switch (num)
					{
					case 11:
					case 12:
					case 13:
						break;
					case 14:
						goto IL_A0;
					case 15:
						num2 = 1;
						goto IL_B8;
					default:
						if (num != 20)
						{
							goto IL_A0;
						}
						break;
					}
				}
				else
				{
					switch (num)
					{
					case 24:
					case 25:
						break;
					default:
						if (num != 38)
						{
							switch (num)
							{
							case 80:
								num2 = 2;
								goto IL_B8;
							case 81:
								goto IL_A0;
							case 82:
								break;
							default:
								goto IL_A0;
							}
						}
						break;
					}
				}
				num2 = 3;
				goto IL_B8;
				IL_A0:
				var ex = new NoViableAltException("", 38, 0, input);
				throw ex;
				IL_B8:
				switch (num2)
				{
				case 1:
				{
					Match(input, 15, FOLLOW_PAREXPR_in_atom3538);
					Match(input, 2, null);
					PushFollow(FOLLOW_expression_in_atom3540);
					var expression_return = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					atom_return.sRetValue = expression_return?.sRetValue;
					atom_return.ST = expression_return?.ST;
					break;
				}
				case 2:
				{
					var commonTree = (CommonTree)Match(input, 80, FOLLOW_NEW_in_atom3553);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 81, FOLLOW_INTEGER_in_atom3555);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_atom3559);
					Match(input, 3, null);
					atom_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					atom_return.ST = templateLib.GetInstanceOf("newArray", new STAttrMap().Add("dest", atom_return.sRetValue).Add("size", commonTree2.Text).Add("lineNo", commonTree.Line));
					break;
				}
				case 3:
				{
					PushFollow(FOLLOW_func_or_id_in_atom3593);
					var func_or_id_return = func_or_id();
					state.followingStackPointer--;
					atom_return.sRetValue = func_or_id_return?.sRetValue;
					atom_return.ST = func_or_id_return?.ST;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return atom_return;
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x0006596C File Offset: 0x00063B6C
		public array_func_or_id_return array_func_or_id()
		{
			array_func_or_id_stack.Push(new array_func_or_id_scope());
			var array_func_or_id_return = new array_func_or_id_return();
			array_func_or_id_return.Start = input.LT(1);
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
					if (num is (< 11 or > 13) and not 20 and (< 24 or > 25) and not 38 and not 82)
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
					var commonTree = (CommonTree)Match(input, 22, FOLLOW_ARRAYGET_in_array_func_or_id3620);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_array_func_or_id3624);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_array_func_or_id3628);
					PushFollow(FOLLOW_autoCast_in_array_func_or_id3630);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_func_or_id_in_array_func_or_id3632);
					var func_or_id_return = func_or_id();
					state.followingStackPointer--;
					PushFollow(FOLLOW_expression_in_array_func_or_id3634);
					var expression_return = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					array_func_or_id_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					((array_func_or_id_scope)array_func_or_id_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					array_func_or_id_return.ST = templateLib.GetInstanceOf("arrayGet", new STAttrMap().Add("retValue", array_func_or_id_return.sRetValue).Add("selfName", ((array_func_or_id_scope)array_func_or_id_stack.Peek()).sselfName).Add("index", autoCast_return?.sRetValue).Add("autoCast", autoCast_return?.ST).Add("arrayExpressions", func_or_id_return?.ST).Add("indexExpressions", expression_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					PushFollow(FOLLOW_func_or_id_in_array_func_or_id3688);
					var func_or_id_return2 = func_or_id();
					state.followingStackPointer--;
					array_func_or_id_return.sRetValue = func_or_id_return2?.sRetValue;
					array_func_or_id_return.ST = func_or_id_return2?.ST;
					break;
				}
				}
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

		// Token: 0x06000D98 RID: 3480 RVA: 0x00065CD0 File Offset: 0x00063ED0
		public func_or_id_return func_or_id()
		{
			func_or_id_stack.Push(new func_or_id_scope());
			var func_or_id_return = new func_or_id_return();
			func_or_id_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num2;
				if (num <= 20)
				{
					switch (num)
					{
					case 11:
					case 12:
					case 13:
						break;
					default:
						if (num != 20)
						{
							goto IL_A3;
						}
						num2 = 2;
						goto IL_BB;
					}
				}
				else
				{
					switch (num)
					{
					case 24:
					case 25:
						break;
					default:
						if (num == 38)
						{
							num2 = 3;
							goto IL_BB;
						}
						if (num != 82)
						{
							goto IL_A3;
						}
						num2 = 4;
						goto IL_BB;
					}
				}
				num2 = 1;
				goto IL_BB;
				IL_A3:
				var ex = new NoViableAltException("", 40, 0, input);
				throw ex;
				IL_BB:
				switch (num2)
				{
				case 1:
				{
					PushFollow(FOLLOW_function_call_in_func_or_id3714);
					var function_call_return = function_call();
					state.followingStackPointer--;
					func_or_id_return.sRetValue = function_call_return?.sRetValue;
					func_or_id_return.ST = function_call_return?.ST;
					break;
				}
				case 2:
				{
					var commonTree = (CommonTree)Match(input, 20, FOLLOW_PROPGET_in_func_or_id3726);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id3730);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id3734);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id3738);
					Match(input, 3, null);
					((func_or_id_scope)func_or_id_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					func_or_id_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					func_or_id_return.ST = templateLib.GetInstanceOf("propGet", new STAttrMap().Add("selfName", ((func_or_id_scope)func_or_id_stack.Peek()).sselfName).Add("name", commonTree3.Text).Add("retValue", func_or_id_return.sRetValue).Add("lineNo", commonTree.Line));
					break;
				}
				case 3:
				{
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id3777);
					func_or_id_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree5.Text);
					break;
				}
				case 4:
				{
					var commonTree6 = (CommonTree)Match(input, 82, FOLLOW_LENGTH_in_func_or_id3789);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id3793);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id3797);
					Match(input, 3, null);
					((func_or_id_scope)func_or_id_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					func_or_id_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					func_or_id_return.ST = templateLib.GetInstanceOf("arrayLength", new STAttrMap().Add("selfName", ((func_or_id_scope)func_or_id_stack.Peek()).sselfName).Add("retValue", func_or_id_return.sRetValue).Add("lineNo", commonTree6.Line));
					break;
				}
				}
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

		// Token: 0x06000D99 RID: 3481 RVA: 0x00066124 File Offset: 0x00064324
		public property_set_return property_set()
		{
			property_set_stack.Push(new property_set_scope());
			var property_set_return = new property_set_return();
			property_set_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)Match(input, 21, FOLLOW_PROPSET_in_property_set3843);
				Match(input, 2, null);
				var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_property_set3847);
				var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_property_set3851);
				var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_property_set3855);
				Match(input, 3, null);
				((property_set_scope)property_set_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
				((property_set_scope)property_set_stack.Peek()).sparamName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
				property_set_return.ST = templateLib.GetInstanceOf("propSet", new STAttrMap().Add("selfName", ((property_set_scope)property_set_stack.Peek()).sselfName).Add("name", commonTree3.Text).Add("param", ((property_set_scope)property_set_stack.Peek()).sparamName).Add("lineNo", commonTree.Line));
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			finally
			{
				property_set_stack.Pop();
			}
			return property_set_return;
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x0006632C File Offset: 0x0006452C
		public return_stat_return return_stat()
		{
			var return_stat_return = new return_stat_return();
			return_stat_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				if (num != 83)
				{
					var ex = new NoViableAltException("", 41, 0, input);
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
					if (num2 is not 3 and not 5 and (< 11 or > 13) and not 15 and not 20 and not 22 and (< 24 or > 36) and not 38 and not 41 and not 62 and (< 65 or > 72) and (< 77 or > 84) and not 88 and (< 90 or > 93))
					{
						var ex2 = new NoViableAltException("", 41, 1, input);
						throw ex2;
					}
					num3 = 2;
				}
				switch (num3)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 83, FOLLOW_RETURN_in_return_stat3903);
					Match(input, 2, null);
					PushFollow(FOLLOW_autoCast_in_return_stat3905);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					PushFollow(FOLLOW_expression_in_return_stat3907);
					var expression_return = expression();
					state.followingStackPointer--;
					Match(input, 3, null);
					return_stat_return.ST = templateLib.GetInstanceOf("return", new STAttrMap().Add("retVal", autoCast_return?.sRetValue).Add("autoCast", autoCast_return?.ST).Add("extraExpressions", expression_return?.ST).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					var commonTree2 = (CommonTree)Match(input, 83, FOLLOW_RETURN_in_return_stat3940);
					return_stat_return.ST = templateLib.GetInstanceOf("return", new STAttrMap().Add("retVal", "none").Add("lineNo", commonTree2.Line));
					break;
				}
				}
			}
			catch (RecognitionException ex3)
			{
				ReportError(ex3);
				Recover(input, ex3);
			}
			return return_stat_return;
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x000665D0 File Offset: 0x000647D0
		public ifBlock_return ifBlock()
		{
			ifBlock_stack.Push(new ifBlock_scope());
			var ifBlock_return = new ifBlock_return();
			ifBlock_return.Start = input.LT(1);
			IList list = null;
			elseBlock_return elseBlock_return = null;
			((ifBlock_scope)ifBlock_stack.Peek()).kBlockStatements = new ArrayList();
			((ifBlock_scope)ifBlock_stack.Peek()).sEndLabel = GenerateLabel();
			((ifBlock_scope)ifBlock_stack.Peek()).kchildScope = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.Children[((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				var commonTree = (CommonTree)Match(input, 84, FOLLOW_IF_in_ifBlock3984);
				Match(input, 2, null);
				PushFollow(FOLLOW_expression_in_ifBlock3986);
				var expression_return = expression();
				state.followingStackPointer--;
				PushFollow(FOLLOW_codeBlock_in_ifBlock3988);
				codeBlock(((ifBlock_scope)ifBlock_stack.Peek()).kBlockStatements, ((codeBlock_scope)codeBlock_stack.Peek()).kvarDefs, ((ifBlock_scope)ifBlock_stack.Peek()).kchildScope);
				state.followingStackPointer--;
				for (;;)
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
					PushFollow(FOLLOW_elseIfBlock_in_ifBlock3994);
					var elseIfBlock_return = elseIfBlock();
					state.followingStackPointer--;
					list ??= new ArrayList();
					list.Add(elseIfBlock_return.Template);
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
					PushFollow(FOLLOW_elseBlock_in_ifBlock3998);
					elseBlock_return = elseBlock();
					state.followingStackPointer--;
				}
				Match(input, 3, null);
				ifBlock_return.ST = templateLib.GetInstanceOf("ifBlock", new STAttrMap().Add("condition", expression_return?.sRetValue).Add("condExpressions", expression_return?.ST).Add("blockStatements", ((ifBlock_scope)ifBlock_stack.Peek()).kBlockStatements).Add("elifBlocks", list).Add("elseBlock", elseBlock_return?.ST).Add("elseLabel", GenerateLabel()).Add("endLabel", ((ifBlock_scope)ifBlock_stack.Peek()).sEndLabel).Add("lineNo", commonTree.Line));
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

		// Token: 0x06000D9C RID: 3484 RVA: 0x00066944 File Offset: 0x00064B44
		public elseIfBlock_return elseIfBlock()
		{
			elseIfBlock_stack.Push(new elseIfBlock_scope());
			var elseIfBlock_return = new elseIfBlock_return();
			elseIfBlock_return.Start = input.LT(1);
			((elseIfBlock_scope)elseIfBlock_stack.Peek()).kBlockStatements = new ArrayList();
			((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild++;
			((elseIfBlock_scope)elseIfBlock_stack.Peek()).kchildScope = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.Children[((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				var commonTree = (CommonTree)Match(input, 86, FOLLOW_ELSEIF_in_elseIfBlock4072);
				Match(input, 2, null);
				PushFollow(FOLLOW_expression_in_elseIfBlock4074);
				var expression_return = expression();
				state.followingStackPointer--;
				PushFollow(FOLLOW_codeBlock_in_elseIfBlock4076);
				codeBlock(((elseIfBlock_scope)elseIfBlock_stack.Peek()).kBlockStatements, ((codeBlock_scope)codeBlock_stack.Peek()).kvarDefs, ((elseIfBlock_scope)elseIfBlock_stack.Peek()).kchildScope);
				state.followingStackPointer--;
				Match(input, 3, null);
				elseIfBlock_return.ST = templateLib.GetInstanceOf("elseIfBlock", new STAttrMap().Add("condition", expression_return?.sRetValue).Add("condExpressions", expression_return?.ST).Add("blockStatements", ((elseIfBlock_scope)elseIfBlock_stack.Peek()).kBlockStatements).Add("elseLabel", GenerateLabel()).Add("endLabel", ((ifBlock_scope)ifBlock_stack.Peek()).sEndLabel).Add("lineNo", commonTree.Line));
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

		// Token: 0x06000D9D RID: 3485 RVA: 0x00066BC0 File Offset: 0x00064DC0
		public elseBlock_return elseBlock()
		{
			elseBlock_stack.Push(new elseBlock_scope());
			var elseBlock_return = new elseBlock_return();
			elseBlock_return.Start = input.LT(1);
			((elseBlock_scope)elseBlock_stack.Peek()).kBlockStatements = new ArrayList();
			((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild++;
			((elseBlock_scope)elseBlock_stack.Peek()).kchildScope = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.Children[((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				Match(input, 87, FOLLOW_ELSE_in_elseBlock4140);
				Match(input, 2, null);
				PushFollow(FOLLOW_codeBlock_in_elseBlock4142);
				codeBlock(((elseBlock_scope)elseBlock_stack.Peek()).kBlockStatements, ((codeBlock_scope)codeBlock_stack.Peek()).kvarDefs, ((elseBlock_scope)elseBlock_stack.Peek()).kchildScope);
				state.followingStackPointer--;
				Match(input, 3, null);
				elseBlock_return.ST = templateLib.GetInstanceOf("elseBlock", new STAttrMap().Add("blockStatements", ((elseBlock_scope)elseBlock_stack.Peek()).kBlockStatements));
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

		// Token: 0x06000D9E RID: 3486 RVA: 0x00066D8C File Offset: 0x00064F8C
		public whileBlock_return whileBlock()
		{
			whileBlock_stack.Push(new whileBlock_scope());
			var whileBlock_return = new whileBlock_return();
			whileBlock_return.Start = input.LT(1);
			((whileBlock_scope)whileBlock_stack.Peek()).kBlockStatements = new ArrayList();
			((whileBlock_scope)whileBlock_stack.Peek()).sStartLabel = GenerateLabel();
			((whileBlock_scope)whileBlock_stack.Peek()).sEndLabel = GenerateLabel();
			((whileBlock_scope)whileBlock_stack.Peek()).kchildScope = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.Children[((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				var commonTree = (CommonTree)Match(input, 88, FOLLOW_WHILE_in_whileBlock4183);
				Match(input, 2, null);
				PushFollow(FOLLOW_expression_in_whileBlock4185);
				var expression_return = expression();
				state.followingStackPointer--;
				PushFollow(FOLLOW_codeBlock_in_whileBlock4187);
				codeBlock(((whileBlock_scope)whileBlock_stack.Peek()).kBlockStatements, ((codeBlock_scope)codeBlock_stack.Peek()).kvarDefs, ((whileBlock_scope)whileBlock_stack.Peek()).kchildScope);
				state.followingStackPointer--;
				Match(input, 3, null);
				whileBlock_return.ST = templateLib.GetInstanceOf("whileBlock", new STAttrMap().Add("condition", expression_return?.sRetValue).Add("condExpressions", expression_return?.ST).Add("blockStatements", ((whileBlock_scope)whileBlock_stack.Peek()).kBlockStatements).Add("startLabel", ((whileBlock_scope)whileBlock_stack.Peek()).sStartLabel).Add("endLabel", ((whileBlock_scope)whileBlock_stack.Peek()).sEndLabel).Add("lineNo", commonTree.Line));
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

		// Token: 0x06000D9F RID: 3487 RVA: 0x0006704C File Offset: 0x0006524C
		public function_call_return function_call()
		{
			function_call_stack.Push(new function_call_scope());
			var function_call_return = new function_call_return();
			function_call_return.Start = input.LT(1);
			parameters_return parameters_return = null;
			parameters_return parameters_return2 = null;
			parameters_return parameters_return3 = null;
			parameters_return parameters_return4 = null;
			parameters_return parameters_return5 = null;
			try
			{
				var num = input.LA(1);
				int num2;
				switch (num)
				{
				case 11:
					num2 = 1;
					break;
				case 12:
					num2 = 3;
					break;
				case 13:
					num2 = 2;
					break;
				default:
					switch (num)
					{
					case 24:
						num2 = 4;
						break;
					case 25:
						num2 = 5;
						break;
					default:
					{
						NoViableAltException ex = new("", 49, 0, input);
						throw ex;
					}
					}
					break;
				}
				switch (num2)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 11, FOLLOW_CALL_in_function_call4252);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4256);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4260);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4264);
					Match(input, 14, FOLLOW_CALLPARAMS_in_function_call4267);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num3 = 2;
						var num4 = input.LA(1);
						if (num4 == 9)
						{
							num3 = 1;
						}
						var num5 = num3;
						if (num5 == 1)
						{
							PushFollow(FOLLOW_parameters_in_function_call4269);
							parameters_return = parameters();
							state.followingStackPointer--;
						}
						Match(input, 3, null);
					}
					Match(input, 3, null);
					((function_call_scope)function_call_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					function_call_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if ((CommonTree)parameters_return?.Start == null)
					{
						function_call_return.ST = templateLib.GetInstanceOf("callLocal", new STAttrMap().Add("selfName", ((function_call_scope)function_call_stack.Peek()).sselfName).Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree.Line));
					}
					else
					{
						function_call_return.ST = templateLib.GetInstanceOf("callLocal", new STAttrMap().Add("selfName", ((function_call_scope)function_call_stack.Peek()).sselfName).Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("args", parameters_return?.sParamVars).Add("autoCast", parameters_return?.kAutoCastST).Add("paramExpressions", parameters_return?.ST).Add("lineNo", commonTree.Line));
					}
					break;
				}
				case 2:
				{
					var commonTree5 = (CommonTree)Match(input, 13, FOLLOW_CALLPARENT_in_function_call4355);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4359);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4363);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4367);
					Match(input, 14, FOLLOW_CALLPARAMS_in_function_call4370);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num6 = 2;
						var num7 = input.LA(1);
						if (num7 == 9)
						{
							num6 = 1;
						}
						var num8 = num6;
						if (num8 == 1)
						{
							PushFollow(FOLLOW_parameters_in_function_call4372);
							parameters_return2 = parameters();
							state.followingStackPointer--;
						}
						Match(input, 3, null);
					}
					Match(input, 3, null);
					function_call_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if ((CommonTree)parameters_return2?.Start == null)
					{
						function_call_return.ST = templateLib.GetInstanceOf("callParent", new STAttrMap().Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree5.Line));
					}
					else
					{
						function_call_return.ST = templateLib.GetInstanceOf("callParent", new STAttrMap().Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("args", parameters_return2?.sParamVars).Add("autoCast", parameters_return2?.kAutoCastST).Add("paramExpressions", parameters_return2?.ST).Add("lineNo", commonTree5.Line));
					}
					break;
				}
				case 3:
				{
					var commonTree6 = (CommonTree)Match(input, 12, FOLLOW_CALLGLOBAL_in_function_call4448);
					Match(input, 2, null);
					var commonTree7 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4452);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4456);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4460);
					Match(input, 14, FOLLOW_CALLPARAMS_in_function_call4463);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num9 = 2;
						var num10 = input.LA(1);
						if (num10 == 9)
						{
							num9 = 1;
						}
						var num11 = num9;
						if (num11 == 1)
						{
							PushFollow(FOLLOW_parameters_in_function_call4465);
							parameters_return3 = parameters();
							state.followingStackPointer--;
						}
						Match(input, 3, null);
					}
					Match(input, 3, null);
					function_call_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if ((CommonTree)parameters_return3?.Start == null)
					{
						function_call_return.ST = templateLib.GetInstanceOf("callGlobal", new STAttrMap().Add("objType", commonTree7.Text).Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree6.Line));
					}
					else
					{
						function_call_return.ST = templateLib.GetInstanceOf("callGlobal", new STAttrMap().Add("objType", commonTree7.Text).Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("args", parameters_return3?.sParamVars).Add("autoCast", parameters_return3?.kAutoCastST).Add("paramExpressions", parameters_return3?.ST).Add("lineNo", commonTree6.Line));
					}
					break;
				}
				case 4:
				{
					var commonTree8 = (CommonTree)Match(input, 24, FOLLOW_ARRAYFIND_in_function_call4551);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4555);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4559);
					Match(input, 14, FOLLOW_CALLPARAMS_in_function_call4562);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num12 = 2;
						var num13 = input.LA(1);
						if (num13 == 9)
						{
							num12 = 1;
						}
						var num14 = num12;
						if (num14 == 1)
						{
							PushFollow(FOLLOW_parameters_in_function_call4564);
							parameters_return4 = parameters();
							state.followingStackPointer--;
						}
						Match(input, 3, null);
					}
					Match(input, 3, null);
					((function_call_scope)function_call_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					function_call_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if ((CommonTree)parameters_return4?.Start == null)
					{
						function_call_return.ST = templateLib.GetInstanceOf("arrayFind", new STAttrMap().Add("selfName", ((function_call_scope)function_call_stack.Peek()).sselfName).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree8.Line));
					}
					else
					{
						function_call_return.ST = templateLib.GetInstanceOf("arrayFind", new STAttrMap().Add("selfName", ((function_call_scope)function_call_stack.Peek()).sselfName).Add("retValue", function_call_return.sRetValue).Add("args", parameters_return4?.sParamVars).Add("autoCast", parameters_return4?.kAutoCastST).Add("paramExpressions", parameters_return4?.ST).Add("lineNo", commonTree8.Line));
					}
					break;
				}
				case 5:
				{
					var commonTree9 = (CommonTree)Match(input, 25, FOLLOW_ARRAYRFIND_in_function_call4640);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4644);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call4648);
					Match(input, 14, FOLLOW_CALLPARAMS_in_function_call4651);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num15 = 2;
						var num16 = input.LA(1);
						if (num16 == 9)
						{
							num15 = 1;
						}
						var num17 = num15;
						if (num17 == 1)
						{
							PushFollow(FOLLOW_parameters_in_function_call4653);
							parameters_return5 = parameters();
							state.followingStackPointer--;
						}
						Match(input, 3, null);
					}
					Match(input, 3, null);
					((function_call_scope)function_call_stack.Peek()).sselfName = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					function_call_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if ((CommonTree)parameters_return5?.Start == null)
					{
						function_call_return.ST = templateLib.GetInstanceOf("arrayRFind", new STAttrMap().Add("selfName", ((function_call_scope)function_call_stack.Peek()).sselfName).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree9.Line));
					}
					else
					{
						function_call_return.ST = templateLib.GetInstanceOf("arrayRFind", new STAttrMap().Add("selfName", ((function_call_scope)function_call_stack.Peek()).sselfName).Add("retValue", function_call_return.sRetValue).Add("args", parameters_return5?.sParamVars).Add("autoCast", parameters_return5?.kAutoCastST).Add("paramExpressions", parameters_return5?.ST).Add("lineNo", commonTree9.Line));
					}
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			finally
			{
				function_call_stack.Pop();
			}
			return function_call_return;
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x00067D9C File Offset: 0x00065F9C
		public parameters_return parameters()
		{
			var parameters_return = new parameters_return();
			parameters_return.Start = input.LT(1);
			IList list = null;
			parameters_return.sParamVars = new ArrayList();
			parameters_return.kAutoCastST = new ArrayList();
			try
			{
				var num = 0;
				for (;;)
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
					PushFollow(FOLLOW_parameter_in_parameters4751);
					var parameter_return = parameter();
					state.followingStackPointer--;
					list ??= new ArrayList();
					list.Add(parameter_return.Template);
					parameters_return.sParamVars.Add(parameter_return.sVarName);
					parameters_return.kAutoCastST.Add(parameter_return.kAutoCastST);
					num++;
				}
				if (num < 1)
				{
					var ex = new EarlyExitException(50, input);
					throw ex;
				}
				parameters_return.ST = templateLib.GetInstanceOf("parameterExpressions", new STAttrMap().Add("expressions", list));
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return parameters_return;
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x00067EE4 File Offset: 0x000660E4
		public parameter_return parameter()
		{
			var parameter_return = new parameter_return();
			parameter_return.Start = input.LT(1);
			try
			{
				Match(input, 9, FOLLOW_PARAM_in_parameter4793);
				Match(input, 2, null);
				PushFollow(FOLLOW_autoCast_in_parameter4795);
				var autoCast_return = autoCast();
				state.followingStackPointer--;
				PushFollow(FOLLOW_expression_in_parameter4797);
				var expression_return = expression();
				state.followingStackPointer--;
				Match(input, 3, null);
				parameter_return.ST = expression_return?.ST;
				parameter_return.sVarName = (autoCast_return?.sRetValue);
				parameter_return.kAutoCastST = (autoCast_return?.ST);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return parameter_return;
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x00067FEC File Offset: 0x000661EC
		public autoCast_return autoCast()
		{
			autoCast_stack.Push(new autoCast_scope());
			var autoCast_return = new autoCast_return();
			autoCast_return.Start = input.LT(1);
			try
			{
				var num = input.LA(1);
				int num5;
				if (num != 38)
				{
					switch (num)
					{
					case 79:
					{
						var num2 = input.LA(2);
						if (num2 != 2)
						{
							var ex = new NoViableAltException("", 51, 1, input);
							throw ex;
						}
						var num3 = input.LA(3);
						if (num3 != 38)
						{
							var ex2 = new NoViableAltException("", 51, 4, input);
							throw ex2;
						}
						var num4 = input.LA(4);
						if (num4 == 38)
						{
							num5 = 1;
							goto IL_150;
						}
						if (num4 is 81 or >= 90 and <= 93)
						{
							num5 = 2;
							goto IL_150;
						}
						var ex3 = new NoViableAltException("", 51, 5, input);
						throw ex3;
					}
					case 80:
						goto IL_138;
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
							goto IL_138;
						}
						break;
					}
					num5 = 4;
					goto IL_150;
					IL_138:
					NoViableAltException ex4 = new("", 51, 0, input);
					throw ex4;
				}
				num5 = 3;
				IL_150:
				switch (num5)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 79, FOLLOW_AS_in_autoCast4825);
					Match(input, 2, null);
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_autoCast4829);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_autoCast4833);
					Match(input, 3, null);
					((autoCast_scope)autoCast_stack.Peek()).ssource = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					autoCast_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					autoCast_return.ST = templateLib.GetInstanceOf("cast", new STAttrMap().Add("target", autoCast_return.sRetValue).Add("source", ((autoCast_scope)autoCast_stack.Peek()).ssource).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					var commonTree4 = (CommonTree)Match(input, 79, FOLLOW_AS_in_autoCast4868);
					Match(input, 2, null);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_autoCast4870);
					PushFollow(FOLLOW_constant_in_autoCast4872);
					var constant_return = constant();
					state.followingStackPointer--;
					Match(input, 3, null);
					autoCast_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree5.Text);
					autoCast_return.ST = templateLib.GetInstanceOf("cast", new STAttrMap().Add("target", autoCast_return.sRetValue).Add("source", ((CommonTree)constant_return?.Start).Text).Add("lineNo", commonTree4.Line));
					break;
				}
				case 3:
				{
					var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_autoCast4906);
					autoCast_return.sRetValue = ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					break;
				}
				case 4:
				{
					PushFollow(FOLLOW_constant_in_autoCast4917);
					var constant_return2 = constant();
					state.followingStackPointer--;
					autoCast_return.sRetValue = ((CommonTree)constant_return2?.Start)?.Text;
					break;
				}
				}
			}
			catch (RecognitionException ex5)
			{
				ReportError(ex5);
				Recover(input, ex5);
			}
			finally
			{
				autoCast_stack.Pop();
			}
			return autoCast_return;
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x00068478 File Offset: 0x00066678
		public constant_return constant()
		{
			var constant_return = new constant_return();
			constant_return.Start = input.LT(1);
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
						goto IL_70;
					case 91:
						num2 = 3;
						goto IL_70;
					case 92:
						num2 = 4;
						goto IL_70;
					case 93:
						break;
					default:
					{
						var ex = new NoViableAltException("", 52, 0, input);
						throw ex;
					}
					}
				}
				num2 = 1;
				IL_70:
				switch (num2)
				{
				case 1:
					PushFollow(FOLLOW_number_in_constant4935);
					number();
					state.followingStackPointer--;
					break;
				case 2:
					Match(input, 90, FOLLOW_STRING_in_constant4941);
					break;
				case 3:
					Match(input, 91, FOLLOW_BOOL_in_constant4947);
					break;
				case 4:
					Match(input, 92, FOLLOW_NONE_in_constant4953);
					break;
				}
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return constant_return;
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x000685A4 File Offset: 0x000667A4
		public number_return number()
		{
			var number_return = new number_return();
			number_return.Start = input.LT(1);
			try
			{
				if (input.LA(1) != 81 && input.LA(1) != 93)
				{
					var ex = new MismatchedSetException(null, input);
					throw ex;
				}
				input.Consume();
				state.errorRecovery = false;
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return number_return;
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x0006863C File Offset: 0x0006683C
		public type_return type()
		{
			var type_return = new type_return();
			type_return.Start = input.LT(1);
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
						if (num2 != 38)
						{
							var ex = new NoViableAltException("", 53, 1, input);
							throw ex;
						}
						num3 = 1;
					}
				}
				else
				{
					if (num != 55)
					{
						var ex2 = new NoViableAltException("", 53, 0, input);
						throw ex2;
					}
					var num4 = input.LA(2);
					if (num4 == 63)
					{
						num3 = 4;
					}
					else
					{
						if (num4 != 38)
						{
							var ex3 = new NoViableAltException("", 53, 2, input);
							throw ex3;
						}
						num3 = 3;
					}
				}
				switch (num3)
				{
				case 1:
				{
					var commonTree = (CommonTree)Match(input, 38, FOLLOW_ID_in_type4985);
					type_return.sTypeString = commonTree.Text;
					break;
				}
				case 2:
				{
					var commonTree2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_type4996);
					Match(input, 63, FOLLOW_LBRACKET_in_type4998);
					Match(input, 64, FOLLOW_RBRACKET_in_type5000);
					type_return.sTypeString = $"{commonTree2.Text}[]";
					break;
				}
				case 3:
				{
					var commonTree3 = (CommonTree)Match(input, 55, FOLLOW_BASETYPE_in_type5011);
					type_return.sTypeString = commonTree3.Text;
					break;
				}
				case 4:
				{
					var commonTree4 = (CommonTree)Match(input, 55, FOLLOW_BASETYPE_in_type5022);
					Match(input, 63, FOLLOW_LBRACKET_in_type5024);
					Match(input, 64, FOLLOW_RBRACKET_in_type5026);
					type_return.sTypeString = $"{commonTree4.Text}[]";
					break;
				}
				}
			}
			catch (RecognitionException ex4)
			{
				ReportError(ex4);
				Recover(input, ex4);
			}
			return type_return;
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x00068878 File Offset: 0x00066A78
		private void InitializeCyclicDFAs()
		{
			dfa27 = new DFA27(this);
		}

		// Token: 0x170001B8 RID: 440
		// (set) Token: 0x06000DA7 RID: 3495 RVA: 0x00068888 File Offset: 0x00066A88
		internal Dictionary<string, PapyrusFlag> KnownUserFlags
		{
			set => kFlagDict = value;
        }

		// Token: 0x06000DA8 RID: 3496 RVA: 0x00068894 File Offset: 0x00066A94
		private string MangleVariableName(string asOriginalName)
		{
			var result = $"::mangled_{asOriginalName}_{iCurMangleSuffix}";
			iCurMangleSuffix++;
			return result;
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x000688C8 File Offset: 0x00066AC8
		private void MangleFunctionVariables(ScriptFunctionType akFunction)
		{
			var dictionary = new Dictionary<string, bool>();
			MangleScopeVariables(akFunction.FunctionScope, ref dictionary);
		}

		// Token: 0x06000DAA RID: 3498 RVA: 0x000688EC File Offset: 0x00066AEC
		private void MangleScopeVariables(ScriptScope akCurrentScope, ref Dictionary<string, bool> akAlreadyDefinedVars)
		{
			foreach (var (s, _) in akCurrentScope.Variables)
			{
				var key = s.ToLowerInvariant();
				if (akAlreadyDefinedVars.ContainsKey(key))
				{
					akCurrentScope.kMangledVarNames.Add(key, MangleVariableName(s));
				}
				else
				{
					akAlreadyDefinedVars.Add(key, true);
				}
			}
			foreach (var akCurrentScope2 in akCurrentScope.Children)
			{
				MangleScopeVariables(akCurrentScope2, ref akAlreadyDefinedVars);
			}
		}

		// Token: 0x06000DAB RID: 3499 RVA: 0x000689B8 File Offset: 0x00066BB8
		private string GenerateLabel()
		{
			var result = $"label{iCurLabelSuffix}";
			iCurLabelSuffix++;
			return result;
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x000689EC File Offset: 0x00066BEC
		private static DateTime UnixEpoc => new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Token: 0x06000DAD RID: 3501 RVA: 0x00068A00 File Offset: 0x00066C00
		private static long ToUnixTime(DateTime akDateTime)
		{
			return Convert.ToInt64((akDateTime - UnixEpoc).TotalSeconds);
		}

		// Token: 0x06000DAE RID: 3502 RVA: 0x00068A28 File Offset: 0x00066C28
		private string GetFileModTimeUnix(string asFilename)
		{
			var fileInfo = new FileInfo(asFilename);
			return ToUnixTime(fileInfo.LastWriteTime.ToUniversalTime()).ToString();
		}

		// Token: 0x06000DAF RID: 3503 RVA: 0x00068A58 File Offset: 0x00066C58
		private string GetCompileTimeUnix()
		{
			return ToUnixTime(DateTime.UtcNow).ToString();
		}

		// Token: 0x06000DB0 RID: 3504 RVA: 0x00068A78 File Offset: 0x00066C78
		private Hashtable ConstructUserFlagRefInfo()
		{
			var hashtable = new Hashtable();
			foreach (var keyValuePair in kFlagDict)
			{
				hashtable.Add(keyValuePair.Key, keyValuePair.Value.Index);
			}
			return hashtable;
		}

		// Token: 0x040009ED RID: 2541
		public const int FUNCTION = 6;

		// Token: 0x040009EE RID: 2542
		public const int LT = 70;

		// Token: 0x040009EF RID: 2543
		public const int WHILE = 88;

		// Token: 0x040009F0 RID: 2544
		public const int DIVEQUALS = 60;

		// Token: 0x040009F1 RID: 2545
		public const int MOD = 77;

		// Token: 0x040009F2 RID: 2546
		public const int PROPSET = 21;

		// Token: 0x040009F3 RID: 2547
		public const int NEW = 80;

		// Token: 0x040009F4 RID: 2548
		public const int DQUOTE = 98;

		// Token: 0x040009F5 RID: 2549
		public const int PARAM = 9;

		// Token: 0x040009F6 RID: 2550
		public const int EQUALS = 41;

		// Token: 0x040009F7 RID: 2551
		public const int NOT = 78;

		// Token: 0x040009F8 RID: 2552
		public const int EOF = -1;

		// Token: 0x040009F9 RID: 2553
		public const int FNEGATE = 35;

		// Token: 0x040009FA RID: 2554
		public const int LBRACKET = 63;

		// Token: 0x040009FB RID: 2555
		public const int USER_FLAGS = 18;

		// Token: 0x040009FC RID: 2556
		public const int RPAREN = 44;

		// Token: 0x040009FD RID: 2557
		public const int IMPORT = 42;

		// Token: 0x040009FE RID: 2558
		public const int EOL = 94;

		// Token: 0x040009FF RID: 2559
		public const int FADD = 27;

		// Token: 0x04000A00 RID: 2560
		public const int RETURN = 83;

		// Token: 0x04000A01 RID: 2561
		public const int ENDIF = 85;

		// Token: 0x04000A02 RID: 2562
		public const int VAR = 5;

		// Token: 0x04000A03 RID: 2563
		public const int ENDWHILE = 89;

		// Token: 0x04000A04 RID: 2564
		public const int EQ = 67;

		// Token: 0x04000A05 RID: 2565
		public const int IMULTIPLY = 30;

		// Token: 0x04000A06 RID: 2566
		public const int COMMENT = 104;

		// Token: 0x04000A07 RID: 2567
		public const int IDIVIDE = 32;

		// Token: 0x04000A08 RID: 2568
		public const int DIVIDE = 76;

		// Token: 0x04000A09 RID: 2569
		public const int NE = 68;

		// Token: 0x04000A0A RID: 2570
		public const int SCRIPTNAME = 37;

		// Token: 0x04000A0B RID: 2571
		public const int MINUSEQUALS = 58;

		// Token: 0x04000A0C RID: 2572
		public const int ARRAYFIND = 24;

		// Token: 0x04000A0D RID: 2573
		public const int RBRACE = 100;

		// Token: 0x04000A0E RID: 2574
		public const int ELSE = 87;

		// Token: 0x04000A0F RID: 2575
		public const int BOOL = 91;

		// Token: 0x04000A10 RID: 2576
		public const int NATIVE = 47;

		// Token: 0x04000A11 RID: 2577
		public const int FDIVIDE = 33;

		// Token: 0x04000A12 RID: 2578
		public const int UNARY_MINUS = 16;

		// Token: 0x04000A13 RID: 2579
		public const int MULT = 75;

		// Token: 0x04000A14 RID: 2580
		public const int ENDPROPERTY = 53;

		// Token: 0x04000A15 RID: 2581
		public const int CALLPARAMS = 14;

		// Token: 0x04000A16 RID: 2582
		public const int ALPHA = 95;

		// Token: 0x04000A17 RID: 2583
		public const int WS = 102;

		// Token: 0x04000A18 RID: 2584
		public const int FMULTIPLY = 31;

		// Token: 0x04000A19 RID: 2585
		public const int ARRAYSET = 23;

		// Token: 0x04000A1A RID: 2586
		public const int PROPERTY = 54;

		// Token: 0x04000A1B RID: 2587
		public const int AUTOREADONLY = 56;

		// Token: 0x04000A1C RID: 2588
		public const int NONE = 92;

		// Token: 0x04000A1D RID: 2589
		public const int OR = 65;

		// Token: 0x04000A1E RID: 2590
		public const int PROPGET = 20;

		// Token: 0x04000A1F RID: 2591
		public const int IADD = 26;

		// Token: 0x04000A20 RID: 2592
		public const int PROPFUNC = 17;

		// Token: 0x04000A21 RID: 2593
		public const int GT = 69;

		// Token: 0x04000A22 RID: 2594
		public const int CALL = 11;

		// Token: 0x04000A23 RID: 2595
		public const int INEGATE = 34;

		// Token: 0x04000A24 RID: 2596
		public const int BASETYPE = 55;

		// Token: 0x04000A25 RID: 2597
		public const int ENDEVENT = 48;

		// Token: 0x04000A26 RID: 2598
		public const int MULTEQUALS = 59;

		// Token: 0x04000A27 RID: 2599
		public const int CALLPARENT = 13;

		// Token: 0x04000A28 RID: 2600
		public const int LBRACE = 99;

		// Token: 0x04000A29 RID: 2601
		public const int GTE = 71;

		// Token: 0x04000A2A RID: 2602
		public const int FLOAT = 93;

		// Token: 0x04000A2B RID: 2603
		public const int ENDSTATE = 52;

		// Token: 0x04000A2C RID: 2604
		public const int ID = 38;

		// Token: 0x04000A2D RID: 2605
		public const int AND = 66;

		// Token: 0x04000A2E RID: 2606
		public const int LTE = 72;

		// Token: 0x04000A2F RID: 2607
		public const int LPAREN = 43;

		// Token: 0x04000A30 RID: 2608
		public const int LENGTH = 82;

		// Token: 0x04000A31 RID: 2609
		public const int IF = 84;

		// Token: 0x04000A32 RID: 2610
		public const int CALLGLOBAL = 12;

		// Token: 0x04000A33 RID: 2611
		public const int AS = 79;

		// Token: 0x04000A34 RID: 2612
		public const int OBJECT = 4;

		// Token: 0x04000A35 RID: 2613
		public const int COMMA = 49;

		// Token: 0x04000A36 RID: 2614
		public const int PLUSEQUALS = 57;

		// Token: 0x04000A37 RID: 2615
		public const int AUTO = 50;

		// Token: 0x04000A38 RID: 2616
		public const int ISUBTRACT = 28;

		// Token: 0x04000A39 RID: 2617
		public const int PLUS = 73;

		// Token: 0x04000A3A RID: 2618
		public const int ENDFUNCTION = 45;

		// Token: 0x04000A3B RID: 2619
		public const int DIGIT = 96;

		// Token: 0x04000A3C RID: 2620
		public const int HEADER = 8;

		// Token: 0x04000A3D RID: 2621
		public const int RBRACKET = 64;

		// Token: 0x04000A3E RID: 2622
		public const int DOT = 62;

		// Token: 0x04000A3F RID: 2623
		public const int FSUBTRACT = 29;

		// Token: 0x04000A40 RID: 2624
		public const int STRCAT = 36;

		// Token: 0x04000A41 RID: 2625
		public const int INTEGER = 81;

		// Token: 0x04000A42 RID: 2626
		public const int STATE = 51;

		// Token: 0x04000A43 RID: 2627
		public const int DOCSTRING = 40;

		// Token: 0x04000A44 RID: 2628
		public const int WS_CHAR = 101;

		// Token: 0x04000A45 RID: 2629
		public const int HEX_DIGIT = 97;

		// Token: 0x04000A46 RID: 2630
		public const int ARRAYRFIND = 25;

		// Token: 0x04000A47 RID: 2631
		public const int MINUS = 74;

		// Token: 0x04000A48 RID: 2632
		public const int EVENT = 7;

		// Token: 0x04000A49 RID: 2633
		public const int ARRAYGET = 22;

		// Token: 0x04000A4A RID: 2634
		public const int ELSEIF = 86;

		// Token: 0x04000A4B RID: 2635
		public const int AUTOPROP = 19;

		// Token: 0x04000A4C RID: 2636
		public const int PAREXPR = 15;

		// Token: 0x04000A4D RID: 2637
		public const int BLOCK = 10;

		// Token: 0x04000A4E RID: 2638
		public const int EAT_EOL = 103;

		// Token: 0x04000A4F RID: 2639
		public const int GLOBAL = 46;

		// Token: 0x04000A50 RID: 2640
		public const int MODEQUALS = 61;

		// Token: 0x04000A51 RID: 2641
		public const int EXTENDS = 39;

		// Token: 0x04000A52 RID: 2642
		public const int STRING = 90;

		// Token: 0x04000A53 RID: 2643
		private const string DFA27_eotS = "\u0019￿";

		// Token: 0x04000A54 RID: 2644
		private const string DFA27_eofS = "\u0019￿";

		// Token: 0x04000A55 RID: 2645
		private const string DFA27_minS = "\u0001\v\u0002\u0002\u0001￿\u0001\v\u0001&\u0001￿\u0002&\u0001\u0002\u0005\v\u0001&\u0001￿\u0001&\u0005\u0003\u0002\v";

		// Token: 0x04000A56 RID: 2646
		private const string DFA27_maxS = "\u0001>\u0002\u0002\u0001￿\u0001R\u0001&\u0001￿\u0001&\u0001]\u0001\u0002\u0005R\u0001&\u0001￿\u0001]\u0005\u0003\u0002R";

		// Token: 0x04000A57 RID: 2647
		private const string DFA27_acceptS = "\u0003￿\u0001\u0003\u0002￿\u0001\u0001\t￿\u0001\u0002\b￿";

		// Token: 0x04000A58 RID: 2648
		private const string DFA27_specialS = "\u0019￿}>";

		// Token: 0x04000A59 RID: 2649
		public static readonly string[] tokenNames = {
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

		// Token: 0x04000A5A RID: 2650
		protected StringTemplateGroup templateLib = new("PapyrusGenTemplates", typeof(AngleBracketTemplateLexer));

		// Token: 0x04000A5C RID: 2652
		protected StackList script_stack = new();

		// Token: 0x04000A5D RID: 2653
		protected StackList fieldDefinition_stack = new();

		// Token: 0x04000A5E RID: 2654
		protected StackList function_stack = new();

		// Token: 0x04000A5F RID: 2655
		protected StackList eventFunc_stack = new();

		// Token: 0x04000A60 RID: 2656
		protected StackList propertyBlock_stack = new();

		// Token: 0x04000A61 RID: 2657
		protected StackList codeBlock_stack = new();

		// Token: 0x04000A62 RID: 2658
		protected StackList statement_stack = new();

		// Token: 0x04000A63 RID: 2659
		protected StackList l_value_stack = new();

		// Token: 0x04000A64 RID: 2660
		protected StackList basic_l_value_stack = new();

		// Token: 0x04000A65 RID: 2661
		protected StackList array_atom_stack = new();

		// Token: 0x04000A66 RID: 2662
		protected StackList array_func_or_id_stack = new();

		// Token: 0x04000A67 RID: 2663
		protected StackList func_or_id_stack = new();

		// Token: 0x04000A68 RID: 2664
		protected StackList property_set_stack = new();

		// Token: 0x04000A69 RID: 2665
		protected StackList ifBlock_stack = new();

		// Token: 0x04000A6A RID: 2666
		protected StackList elseIfBlock_stack = new();

		// Token: 0x04000A6B RID: 2667
		protected StackList elseBlock_stack = new();

		// Token: 0x04000A6C RID: 2668
		protected StackList whileBlock_stack = new();

		// Token: 0x04000A6D RID: 2669
		protected StackList function_call_stack = new();

		// Token: 0x04000A6E RID: 2670
		protected StackList autoCast_stack = new();

		// Token: 0x04000A6F RID: 2671
		protected DFA27 dfa27;

		// Token: 0x04000A70 RID: 2672
		private static readonly string[] DFA27_transitionS = {
			"\u0003\u0003\a￿\u0001\u0003\u0001￿\u0001\u0002\u0002\u0003\f￿\u0001\u0003\u0017￿\u0001\u0001",
			"\u0001\u0004",
			"\u0001\u0005",
			"",
			"\u0003\u0003\u0001￿\u0001\u0006\u0004￿\u0001\u0003\u0001￿\u0001\u0003\u0001￿\u0002\u0003\f￿\u0001\u0003+￿\u0001\u0003",
			"\u0001\a",
			"",
			"\u0001\b",
			"\u0001\n(￿\u0001\t\u0001￿\u0001\v\b￿\u0001\f\u0001\r\u0001\u000e\u0001\v",
			"\u0001\u000f",
			"\u0003\u0003\u0001￿\u0001\u0010\u0004￿\u0001\u0003\u0003￿\u0002\u0003\f￿\u0001\u0003+￿\u0001\u0003",
			"\u0003\u0003\u0001￿\u0001\u0010\u0004￿\u0001\u0003\u0003￿\u0002\u0003\f￿\u0001\u0003+￿\u0001\u0003",
			"\u0003\u0003\u0001￿\u0001\u0010\u0004￿\u0001\u0003\u0003￿\u0002\u0003\f￿\u0001\u0003+￿\u0001\u0003",
			"\u0003\u0003\u0001￿\u0001\u0010\u0004￿\u0001\u0003\u0003￿\u0002\u0003\f￿\u0001\u0003+￿\u0001\u0003",
			"\u0003\u0003\u0001￿\u0001\u0010\u0004￿\u0001\u0003\u0003￿\u0002\u0003\f￿\u0001\u0003+￿\u0001\u0003",
			"\u0001\u0011",
			"",
			"\u0001\u0012*￿\u0001\u0013\b￿\u0001\u0014\u0001\u0015\u0001\u0016\u0001\u0013",
			"\u0001\u0017",
			"\u0001\u0018",
			"\u0001\u0018",
			"\u0001\u0018",
			"\u0001\u0018",
			"\u0003\u0003\u0001￿\u0001\u0010\u0004￿\u0001\u0003\u0003￿\u0002\u0003\f￿\u0001\u0003+￿\u0001\u0003",
			"\u0003\u0003\u0001￿\u0001\u0010\u0004￿\u0001\u0003\u0003￿\u0002\u0003\f￿\u0001\u0003+￿\u0001\u0003"
		};

		// Token: 0x04000A71 RID: 2673
		private static readonly short[] DFA27_eot = DFA.UnpackEncodedString("\u0019￿");

		// Token: 0x04000A72 RID: 2674
		private static readonly short[] DFA27_eof = DFA.UnpackEncodedString("\u0019￿");

		// Token: 0x04000A73 RID: 2675
		private static readonly char[] DFA27_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\v\u0002\u0002\u0001￿\u0001\v\u0001&\u0001￿\u0002&\u0001\u0002\u0005\v\u0001&\u0001￿\u0001&\u0005\u0003\u0002\v");

		// Token: 0x04000A74 RID: 2676
		private static readonly char[] DFA27_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001>\u0002\u0002\u0001￿\u0001R\u0001&\u0001￿\u0001&\u0001]\u0001\u0002\u0005R\u0001&\u0001￿\u0001]\u0005\u0003\u0002R");

		// Token: 0x04000A75 RID: 2677
		private static readonly short[] DFA27_accept = DFA.UnpackEncodedString("\u0003￿\u0001\u0003\u0002￿\u0001\u0001\t￿\u0001\u0002\b￿");

		// Token: 0x04000A76 RID: 2678
		private static readonly short[] DFA27_special = DFA.UnpackEncodedString("\u0019￿}>");

		// Token: 0x04000A77 RID: 2679
		private static readonly short[][] DFA27_transition = DFA.UnpackEncodedStringArray(DFA27_transitionS);

		// Token: 0x04000A78 RID: 2680
		public static readonly BitSet FOLLOW_OBJECT_in_script80 = new(new[]
		{
			4UL
		});

		// Token: 0x04000A79 RID: 2681
		public static readonly BitSet FOLLOW_header_in_script82 = new(new[]
		{
			20266198323691752UL
		});

		// Token: 0x04000A7A RID: 2682
		public static readonly BitSet FOLLOW_definitionOrBlock_in_script84 = new(new[]
		{
			20266198323691752UL
		});

		// Token: 0x04000A7B RID: 2683
		public static readonly BitSet FOLLOW_ID_in_header224 = new(new[]
		{
			4UL
		});

		// Token: 0x04000A7C RID: 2684
		public static readonly BitSet FOLLOW_USER_FLAGS_in_header226 = new(new[]
		{
			1374389534728UL
		});

		// Token: 0x04000A7D RID: 2685
		public static readonly BitSet FOLLOW_ID_in_header230 = new(new[]
		{
			1099511627784UL
		});

		// Token: 0x04000A7E RID: 2686
		public static readonly BitSet FOLLOW_DOCSTRING_in_header233 = new(new[]
		{
			8UL
		});

		// Token: 0x04000A7F RID: 2687
		public static readonly BitSet FOLLOW_fieldDefinition_in_definitionOrBlock253 = new(new[]
		{
			2UL
		});

		// Token: 0x04000A80 RID: 2688
		public static readonly BitSet FOLLOW_function_in_definitionOrBlock264 = new(new[]
		{
			2UL
		});

		// Token: 0x04000A81 RID: 2689
		public static readonly BitSet FOLLOW_eventFunc_in_definitionOrBlock277 = new(new[]
		{
			2UL
		});

		// Token: 0x04000A82 RID: 2690
		public static readonly BitSet FOLLOW_stateBlock_in_definitionOrBlock289 = new(new[]
		{
			2UL
		});

		// Token: 0x04000A83 RID: 2691
		public static readonly BitSet FOLLOW_propertyBlock_in_definitionOrBlock295 = new(new[]
		{
			2UL
		});

		// Token: 0x04000A84 RID: 2692
		public static readonly BitSet FOLLOW_VAR_in_fieldDefinition323 = new(new[]
		{
			4UL
		});

		// Token: 0x04000A85 RID: 2693
		public static readonly BitSet FOLLOW_type_in_fieldDefinition325 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000A86 RID: 2694
		public static readonly BitSet FOLLOW_ID_in_fieldDefinition329 = new(new[]
		{
			262144UL
		});

		// Token: 0x04000A87 RID: 2695
		public static readonly BitSet FOLLOW_USER_FLAGS_in_fieldDefinition331 = new(new[]
		{
			8UL,
			1006764032UL
		});

		// Token: 0x04000A88 RID: 2696
		public static readonly BitSet FOLLOW_constant_in_fieldDefinition333 = new(new[]
		{
			8UL
		});

		// Token: 0x04000A89 RID: 2697
		public static readonly BitSet FOLLOW_FUNCTION_in_function408 = new(new[]
		{
			4UL
		});

		// Token: 0x04000A8A RID: 2698
		public static readonly BitSet FOLLOW_functionHeader_in_function410 = new(new[]
		{
			1032UL
		});

		// Token: 0x04000A8B RID: 2699
		public static readonly BitSet FOLLOW_codeBlock_in_function412 = new(new[]
		{
			8UL
		});

		// Token: 0x04000A8C RID: 2700
		public static readonly BitSet FOLLOW_HEADER_in_functionHeader504 = new(new[]
		{
			4UL
		});

		// Token: 0x04000A8D RID: 2701
		public static readonly BitSet FOLLOW_type_in_functionHeader507 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000A8E RID: 2702
		public static readonly BitSet FOLLOW_NONE_in_functionHeader511 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000A8F RID: 2703
		public static readonly BitSet FOLLOW_ID_in_functionHeader516 = new(new[]
		{
			262144UL
		});

		// Token: 0x04000A90 RID: 2704
		public static readonly BitSet FOLLOW_USER_FLAGS_in_functionHeader518 = new(new[]
		{
			212205744161288UL
		});

		// Token: 0x04000A91 RID: 2705
		public static readonly BitSet FOLLOW_callParameters_in_functionHeader520 = new(new[]
		{
			212205744160776UL
		});

		// Token: 0x04000A92 RID: 2706
		public static readonly BitSet FOLLOW_functionModifier_in_functionHeader523 = new(new[]
		{
			212205744160776UL
		});

		// Token: 0x04000A93 RID: 2707
		public static readonly BitSet FOLLOW_DOCSTRING_in_functionHeader526 = new(new[]
		{
			8UL
		});

		// Token: 0x04000A94 RID: 2708
		public static readonly BitSet FOLLOW_NATIVE_in_functionModifier545 = new(new[]
		{
			2UL
		});

		// Token: 0x04000A95 RID: 2709
		public static readonly BitSet FOLLOW_GLOBAL_in_functionModifier553 = new(new[]
		{
			2UL
		});

		// Token: 0x04000A96 RID: 2710
		public static readonly BitSet FOLLOW_EVENT_in_eventFunc588 = new(new[]
		{
			4UL
		});

		// Token: 0x04000A97 RID: 2711
		public static readonly BitSet FOLLOW_eventHeader_in_eventFunc590 = new(new[]
		{
			1032UL
		});

		// Token: 0x04000A98 RID: 2712
		public static readonly BitSet FOLLOW_codeBlock_in_eventFunc592 = new(new[]
		{
			8UL
		});

		// Token: 0x04000A99 RID: 2713
		public static readonly BitSet FOLLOW_HEADER_in_eventHeader684 = new(new[]
		{
			4UL
		});

		// Token: 0x04000A9A RID: 2714
		public static readonly BitSet FOLLOW_NONE_in_eventHeader686 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000A9B RID: 2715
		public static readonly BitSet FOLLOW_ID_in_eventHeader688 = new(new[]
		{
			262144UL
		});

		// Token: 0x04000A9C RID: 2716
		public static readonly BitSet FOLLOW_USER_FLAGS_in_eventHeader690 = new(new[]
		{
			141836999983624UL
		});

		// Token: 0x04000A9D RID: 2717
		public static readonly BitSet FOLLOW_callParameters_in_eventHeader692 = new(new[]
		{
			141836999983112UL
		});

		// Token: 0x04000A9E RID: 2718
		public static readonly BitSet FOLLOW_NATIVE_in_eventHeader695 = new(new[]
		{
			1099511627784UL
		});

		// Token: 0x04000A9F RID: 2719
		public static readonly BitSet FOLLOW_DOCSTRING_in_eventHeader698 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AA0 RID: 2720
		public static readonly BitSet FOLLOW_callParameter_in_callParameters725 = new(new[]
		{
			514UL
		});

		// Token: 0x04000AA1 RID: 2721
		public static readonly BitSet FOLLOW_PARAM_in_callParameter742 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AA2 RID: 2722
		public static readonly BitSet FOLLOW_type_in_callParameter744 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000AA3 RID: 2723
		public static readonly BitSet FOLLOW_ID_in_callParameter748 = new(new[]
		{
			8UL,
			1006764032UL
		});

		// Token: 0x04000AA4 RID: 2724
		public static readonly BitSet FOLLOW_constant_in_callParameter750 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AA5 RID: 2725
		public static readonly BitSet FOLLOW_STATE_in_stateBlock787 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AA6 RID: 2726
		public static readonly BitSet FOLLOW_ID_in_stateBlock789 = new(new[]
		{
			1125899906842824UL
		});

		// Token: 0x04000AA7 RID: 2727
		public static readonly BitSet FOLLOW_AUTO_in_stateBlock791 = new(new[]
		{
			200UL
		});

		// Token: 0x04000AA8 RID: 2728
		public static readonly BitSet FOLLOW_stateFuncOrEvent_in_stateBlock797 = new(new[]
		{
			200UL
		});

		// Token: 0x04000AA9 RID: 2729
		public static readonly BitSet FOLLOW_function_in_stateFuncOrEvent819 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AAA RID: 2730
		public static readonly BitSet FOLLOW_eventFunc_in_stateFuncOrEvent832 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AAB RID: 2731
		public static readonly BitSet FOLLOW_PROPERTY_in_propertyBlock861 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AAC RID: 2732
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock863 = new(new[]
		{
			131072UL
		});

		// Token: 0x04000AAD RID: 2733
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock867 = new(new[]
		{
			131072UL
		});

		// Token: 0x04000AAE RID: 2734
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock872 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AAF RID: 2735
		public static readonly BitSet FOLLOW_AUTOPROP_in_propertyBlock922 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AB0 RID: 2736
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock924 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000AB1 RID: 2737
		public static readonly BitSet FOLLOW_ID_in_propertyBlock928 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AB2 RID: 2738
		public static readonly BitSet FOLLOW_HEADER_in_propertyHeader978 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AB3 RID: 2739
		public static readonly BitSet FOLLOW_type_in_propertyHeader980 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000AB4 RID: 2740
		public static readonly BitSet FOLLOW_ID_in_propertyHeader984 = new(new[]
		{
			262144UL
		});

		// Token: 0x04000AB5 RID: 2741
		public static readonly BitSet FOLLOW_USER_FLAGS_in_propertyHeader986 = new(new[]
		{
			1099511627784UL
		});

		// Token: 0x04000AB6 RID: 2742
		public static readonly BitSet FOLLOW_DOCSTRING_in_propertyHeader988 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AB7 RID: 2743
		public static readonly BitSet FOLLOW_PROPFUNC_in_propertyFunc1009 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AB8 RID: 2744
		public static readonly BitSet FOLLOW_function_in_propertyFunc1011 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AB9 RID: 2745
		public static readonly BitSet FOLLOW_PROPFUNC_in_propertyFunc1025 = new(new[]
		{
			2UL
		});

		// Token: 0x04000ABA RID: 2746
		public static readonly BitSet FOLLOW_BLOCK_in_codeBlock1049 = new(new[]
		{
			4UL
		});

		// Token: 0x04000ABB RID: 2747
		public static readonly BitSet FOLLOW_statement_in_codeBlock1057 = new(new[]
		{
			4611688629756016680UL,
			1025499646UL
		});

		// Token: 0x04000ABC RID: 2748
		public static readonly BitSet FOLLOW_localDefinition_in_statement1086 = new(new[]
		{
			2UL
		});

		// Token: 0x04000ABD RID: 2749
		public static readonly BitSet FOLLOW_EQUALS_in_statement1147 = new(new[]
		{
			4UL
		});

		// Token: 0x04000ABE RID: 2750
		public static readonly BitSet FOLLOW_ID_in_statement1149 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000ABF RID: 2751
		public static readonly BitSet FOLLOW_autoCast_in_statement1151 = new(new[]
		{
			4611686293366126592UL
		});

		// Token: 0x04000AC0 RID: 2752
		public static readonly BitSet FOLLOW_l_value_in_statement1153 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AC1 RID: 2753
		public static readonly BitSet FOLLOW_expression_in_statement1155 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AC2 RID: 2754
		public static readonly BitSet FOLLOW_expression_in_statement1204 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AC3 RID: 2755
		public static readonly BitSet FOLLOW_return_stat_in_statement1215 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AC4 RID: 2756
		public static readonly BitSet FOLLOW_ifBlock_in_statement1226 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AC5 RID: 2757
		public static readonly BitSet FOLLOW_whileBlock_in_statement1237 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AC6 RID: 2758
		public static readonly BitSet FOLLOW_VAR_in_localDefinition1260 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AC7 RID: 2759
		public static readonly BitSet FOLLOW_type_in_localDefinition1262 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000AC8 RID: 2760
		public static readonly BitSet FOLLOW_ID_in_localDefinition1266 = new(new[]
		{
			274877906952UL,
			1006796800UL
		});

		// Token: 0x04000AC9 RID: 2761
		public static readonly BitSet FOLLOW_autoCast_in_localDefinition1269 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000ACA RID: 2762
		public static readonly BitSet FOLLOW_expression_in_localDefinition1271 = new(new[]
		{
			8UL
		});

		// Token: 0x04000ACB RID: 2763
		public static readonly BitSet FOLLOW_DOT_in_l_value1318 = new(new[]
		{
			4UL
		});

		// Token: 0x04000ACC RID: 2764
		public static readonly BitSet FOLLOW_PAREXPR_in_l_value1321 = new(new[]
		{
			4UL
		});

		// Token: 0x04000ACD RID: 2765
		public static readonly BitSet FOLLOW_expression_in_l_value1325 = new(new[]
		{
			8UL
		});

		// Token: 0x04000ACE RID: 2766
		public static readonly BitSet FOLLOW_property_set_in_l_value1330 = new(new[]
		{
			8UL
		});

		// Token: 0x04000ACF RID: 2767
		public static readonly BitSet FOLLOW_ARRAYSET_in_l_value1355 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AD0 RID: 2768
		public static readonly BitSet FOLLOW_ID_in_l_value1359 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000AD1 RID: 2769
		public static readonly BitSet FOLLOW_ID_in_l_value1363 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AD2 RID: 2770
		public static readonly BitSet FOLLOW_autoCast_in_l_value1365 = new(new[]
		{
			32768UL
		});

		// Token: 0x04000AD3 RID: 2771
		public static readonly BitSet FOLLOW_PAREXPR_in_l_value1368 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AD4 RID: 2772
		public static readonly BitSet FOLLOW_expression_in_l_value1372 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AD5 RID: 2773
		public static readonly BitSet FOLLOW_expression_in_l_value1377 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AD6 RID: 2774
		public static readonly BitSet FOLLOW_basic_l_value_in_l_value1431 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AD7 RID: 2775
		public static readonly BitSet FOLLOW_DOT_in_basic_l_value1454 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AD8 RID: 2776
		public static readonly BitSet FOLLOW_array_func_or_id_in_basic_l_value1458 = new(new[]
		{
			4611686293366126592UL
		});

		// Token: 0x04000AD9 RID: 2777
		public static readonly BitSet FOLLOW_basic_l_value_in_basic_l_value1462 = new(new[]
		{
			8UL
		});

		// Token: 0x04000ADA RID: 2778
		public static readonly BitSet FOLLOW_function_call_in_basic_l_value1486 = new(new[]
		{
			2UL
		});

		// Token: 0x04000ADB RID: 2779
		public static readonly BitSet FOLLOW_property_set_in_basic_l_value1497 = new(new[]
		{
			2UL
		});

		// Token: 0x04000ADC RID: 2780
		public static readonly BitSet FOLLOW_ARRAYSET_in_basic_l_value1509 = new(new[]
		{
			4UL
		});

		// Token: 0x04000ADD RID: 2781
		public static readonly BitSet FOLLOW_ID_in_basic_l_value1513 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000ADE RID: 2782
		public static readonly BitSet FOLLOW_ID_in_basic_l_value1517 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000ADF RID: 2783
		public static readonly BitSet FOLLOW_autoCast_in_basic_l_value1519 = new(new[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000AE0 RID: 2784
		public static readonly BitSet FOLLOW_func_or_id_in_basic_l_value1521 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AE1 RID: 2785
		public static readonly BitSet FOLLOW_expression_in_basic_l_value1523 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AE2 RID: 2786
		public static readonly BitSet FOLLOW_ID_in_basic_l_value1577 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AE3 RID: 2787
		public static readonly BitSet FOLLOW_OR_in_expression1595 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AE4 RID: 2788
		public static readonly BitSet FOLLOW_ID_in_expression1597 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AE5 RID: 2789
		public static readonly BitSet FOLLOW_expression_in_expression1601 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AE6 RID: 2790
		public static readonly BitSet FOLLOW_and_expression_in_expression1605 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AE7 RID: 2791
		public static readonly BitSet FOLLOW_and_expression_in_expression1659 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AE8 RID: 2792
		public static readonly BitSet FOLLOW_AND_in_and_expression1681 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AE9 RID: 2793
		public static readonly BitSet FOLLOW_ID_in_and_expression1683 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AEA RID: 2794
		public static readonly BitSet FOLLOW_and_expression_in_and_expression1687 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AEB RID: 2795
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression1691 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AEC RID: 2796
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression1745 = new(new[]
		{
			2UL
		});

		// Token: 0x04000AED RID: 2797
		public static readonly BitSet FOLLOW_EQ_in_bool_expression1767 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AEE RID: 2798
		public static readonly BitSet FOLLOW_ID_in_bool_expression1769 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AEF RID: 2799
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1773 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AF0 RID: 2800
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1777 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AF1 RID: 2801
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1781 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AF2 RID: 2802
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1785 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AF3 RID: 2803
		public static readonly BitSet FOLLOW_NE_in_bool_expression1850 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AF4 RID: 2804
		public static readonly BitSet FOLLOW_ID_in_bool_expression1852 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AF5 RID: 2805
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1856 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AF6 RID: 2806
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1860 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AF7 RID: 2807
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1864 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AF8 RID: 2808
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1868 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AF9 RID: 2809
		public static readonly BitSet FOLLOW_GT_in_bool_expression1928 = new(new[]
		{
			4UL
		});

		// Token: 0x04000AFA RID: 2810
		public static readonly BitSet FOLLOW_ID_in_bool_expression1930 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AFB RID: 2811
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1934 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AFC RID: 2812
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1938 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AFD RID: 2813
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1942 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AFE RID: 2814
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1946 = new(new[]
		{
			8UL
		});

		// Token: 0x04000AFF RID: 2815
		public static readonly BitSet FOLLOW_LT_in_bool_expression2011 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B00 RID: 2816
		public static readonly BitSet FOLLOW_ID_in_bool_expression2013 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B01 RID: 2817
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2017 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B02 RID: 2818
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2021 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B03 RID: 2819
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression2025 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B04 RID: 2820
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2029 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B05 RID: 2821
		public static readonly BitSet FOLLOW_GTE_in_bool_expression2094 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B06 RID: 2822
		public static readonly BitSet FOLLOW_ID_in_bool_expression2096 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B07 RID: 2823
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2100 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B08 RID: 2824
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2104 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B09 RID: 2825
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression2108 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B0A RID: 2826
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2112 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B0B RID: 2827
		public static readonly BitSet FOLLOW_LTE_in_bool_expression2177 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B0C RID: 2828
		public static readonly BitSet FOLLOW_ID_in_bool_expression2179 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B0D RID: 2829
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2183 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B0E RID: 2830
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2187 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B0F RID: 2831
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression2191 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B10 RID: 2832
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2195 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B11 RID: 2833
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2259 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B12 RID: 2834
		public static readonly BitSet FOLLOW_IADD_in_add_expression2281 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B13 RID: 2835
		public static readonly BitSet FOLLOW_ID_in_add_expression2283 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B14 RID: 2836
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2287 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B15 RID: 2837
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2291 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B16 RID: 2838
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2295 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B17 RID: 2839
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2299 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B18 RID: 2840
		public static readonly BitSet FOLLOW_FADD_in_add_expression2364 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B19 RID: 2841
		public static readonly BitSet FOLLOW_ID_in_add_expression2366 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B1A RID: 2842
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2370 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B1B RID: 2843
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2374 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B1C RID: 2844
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2378 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B1D RID: 2845
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2382 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B1E RID: 2846
		public static readonly BitSet FOLLOW_ISUBTRACT_in_add_expression2447 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B1F RID: 2847
		public static readonly BitSet FOLLOW_ID_in_add_expression2449 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B20 RID: 2848
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2453 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B21 RID: 2849
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2457 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B22 RID: 2850
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2461 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B23 RID: 2851
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2465 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B24 RID: 2852
		public static readonly BitSet FOLLOW_FSUBTRACT_in_add_expression2530 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B25 RID: 2853
		public static readonly BitSet FOLLOW_ID_in_add_expression2532 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B26 RID: 2854
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2536 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B27 RID: 2855
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2540 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B28 RID: 2856
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2544 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B29 RID: 2857
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2548 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B2A RID: 2858
		public static readonly BitSet FOLLOW_STRCAT_in_add_expression2613 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B2B RID: 2859
		public static readonly BitSet FOLLOW_ID_in_add_expression2615 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B2C RID: 2860
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2619 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B2D RID: 2861
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2623 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B2E RID: 2862
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2627 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B2F RID: 2863
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2631 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B30 RID: 2864
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2695 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B31 RID: 2865
		public static readonly BitSet FOLLOW_IMULTIPLY_in_mult_expression2718 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B32 RID: 2866
		public static readonly BitSet FOLLOW_ID_in_mult_expression2720 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B33 RID: 2867
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2724 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B34 RID: 2868
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2728 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B35 RID: 2869
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2732 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B36 RID: 2870
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2736 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B37 RID: 2871
		public static readonly BitSet FOLLOW_FMULTIPLY_in_mult_expression2801 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B38 RID: 2872
		public static readonly BitSet FOLLOW_ID_in_mult_expression2803 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B39 RID: 2873
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2807 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B3A RID: 2874
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2811 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B3B RID: 2875
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2815 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B3C RID: 2876
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2819 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B3D RID: 2877
		public static readonly BitSet FOLLOW_IDIVIDE_in_mult_expression2884 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B3E RID: 2878
		public static readonly BitSet FOLLOW_ID_in_mult_expression2886 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B3F RID: 2879
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2890 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B40 RID: 2880
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2894 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B41 RID: 2881
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2898 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B42 RID: 2882
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2902 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B43 RID: 2883
		public static readonly BitSet FOLLOW_FDIVIDE_in_mult_expression2967 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B44 RID: 2884
		public static readonly BitSet FOLLOW_ID_in_mult_expression2969 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B45 RID: 2885
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2973 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B46 RID: 2886
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2977 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B47 RID: 2887
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2981 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B48 RID: 2888
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2985 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B49 RID: 2889
		public static readonly BitSet FOLLOW_MOD_in_mult_expression3050 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B4A RID: 2890
		public static readonly BitSet FOLLOW_ID_in_mult_expression3052 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B4B RID: 2891
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression3056 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B4C RID: 2892
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression3060 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B4D RID: 2893
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression3114 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B4E RID: 2894
		public static readonly BitSet FOLLOW_INEGATE_in_unary_expression3137 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B4F RID: 2895
		public static readonly BitSet FOLLOW_ID_in_unary_expression3139 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B50 RID: 2896
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression3141 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B51 RID: 2897
		public static readonly BitSet FOLLOW_FNEGATE_in_unary_expression3186 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B52 RID: 2898
		public static readonly BitSet FOLLOW_ID_in_unary_expression3188 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B53 RID: 2899
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression3190 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B54 RID: 2900
		public static readonly BitSet FOLLOW_NOT_in_unary_expression3235 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B55 RID: 2901
		public static readonly BitSet FOLLOW_ID_in_unary_expression3237 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B56 RID: 2902
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression3239 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B57 RID: 2903
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression3283 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B58 RID: 2904
		public static readonly BitSet FOLLOW_AS_in_cast_atom3306 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B59 RID: 2905
		public static readonly BitSet FOLLOW_ID_in_cast_atom3308 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B5A RID: 2906
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom3310 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B5B RID: 2907
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom3349 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B5C RID: 2908
		public static readonly BitSet FOLLOW_DOT_in_dot_atom3372 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B5D RID: 2909
		public static readonly BitSet FOLLOW_dot_atom_in_dot_atom3376 = new(new[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000B5E RID: 2910
		public static readonly BitSet FOLLOW_array_func_or_id_in_dot_atom3380 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B5F RID: 2911
		public static readonly BitSet FOLLOW_array_atom_in_dot_atom3409 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B60 RID: 2912
		public static readonly BitSet FOLLOW_constant_in_dot_atom3420 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B61 RID: 2913
		public static readonly BitSet FOLLOW_ARRAYGET_in_array_atom3447 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B62 RID: 2914
		public static readonly BitSet FOLLOW_ID_in_array_atom3451 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B63 RID: 2915
		public static readonly BitSet FOLLOW_ID_in_array_atom3455 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B64 RID: 2916
		public static readonly BitSet FOLLOW_autoCast_in_array_atom3457 = new(new[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000B65 RID: 2917
		public static readonly BitSet FOLLOW_atom_in_array_atom3459 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B66 RID: 2918
		public static readonly BitSet FOLLOW_expression_in_array_atom3461 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B67 RID: 2919
		public static readonly BitSet FOLLOW_atom_in_array_atom3515 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B68 RID: 2920
		public static readonly BitSet FOLLOW_PAREXPR_in_atom3538 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B69 RID: 2921
		public static readonly BitSet FOLLOW_expression_in_atom3540 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B6A RID: 2922
		public static readonly BitSet FOLLOW_NEW_in_atom3553 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B6B RID: 2923
		public static readonly BitSet FOLLOW_INTEGER_in_atom3555 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B6C RID: 2924
		public static readonly BitSet FOLLOW_ID_in_atom3559 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B6D RID: 2925
		public static readonly BitSet FOLLOW_func_or_id_in_atom3593 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B6E RID: 2926
		public static readonly BitSet FOLLOW_ARRAYGET_in_array_func_or_id3620 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B6F RID: 2927
		public static readonly BitSet FOLLOW_ID_in_array_func_or_id3624 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B70 RID: 2928
		public static readonly BitSet FOLLOW_ID_in_array_func_or_id3628 = new(new[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B71 RID: 2929
		public static readonly BitSet FOLLOW_autoCast_in_array_func_or_id3630 = new(new[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000B72 RID: 2930
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id3632 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B73 RID: 2931
		public static readonly BitSet FOLLOW_expression_in_array_func_or_id3634 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B74 RID: 2932
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id3688 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B75 RID: 2933
		public static readonly BitSet FOLLOW_function_call_in_func_or_id3714 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B76 RID: 2934
		public static readonly BitSet FOLLOW_PROPGET_in_func_or_id3726 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B77 RID: 2935
		public static readonly BitSet FOLLOW_ID_in_func_or_id3730 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B78 RID: 2936
		public static readonly BitSet FOLLOW_ID_in_func_or_id3734 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B79 RID: 2937
		public static readonly BitSet FOLLOW_ID_in_func_or_id3738 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B7A RID: 2938
		public static readonly BitSet FOLLOW_ID_in_func_or_id3777 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B7B RID: 2939
		public static readonly BitSet FOLLOW_LENGTH_in_func_or_id3789 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B7C RID: 2940
		public static readonly BitSet FOLLOW_ID_in_func_or_id3793 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B7D RID: 2941
		public static readonly BitSet FOLLOW_ID_in_func_or_id3797 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B7E RID: 2942
		public static readonly BitSet FOLLOW_PROPSET_in_property_set3843 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B7F RID: 2943
		public static readonly BitSet FOLLOW_ID_in_property_set3847 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B80 RID: 2944
		public static readonly BitSet FOLLOW_ID_in_property_set3851 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B81 RID: 2945
		public static readonly BitSet FOLLOW_ID_in_property_set3855 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B82 RID: 2946
		public static readonly BitSet FOLLOW_RETURN_in_return_stat3903 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B83 RID: 2947
		public static readonly BitSet FOLLOW_autoCast_in_return_stat3905 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B84 RID: 2948
		public static readonly BitSet FOLLOW_expression_in_return_stat3907 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B85 RID: 2949
		public static readonly BitSet FOLLOW_RETURN_in_return_stat3940 = new(new[]
		{
			2UL
		});

		// Token: 0x04000B86 RID: 2950
		public static readonly BitSet FOLLOW_IF_in_ifBlock3984 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B87 RID: 2951
		public static readonly BitSet FOLLOW_expression_in_ifBlock3986 = new(new[]
		{
			1024UL
		});

		// Token: 0x04000B88 RID: 2952
		public static readonly BitSet FOLLOW_codeBlock_in_ifBlock3988 = new(new[]
		{
			8UL,
			12582912UL
		});

		// Token: 0x04000B89 RID: 2953
		public static readonly BitSet FOLLOW_elseIfBlock_in_ifBlock3994 = new(new[]
		{
			8UL,
			12582912UL
		});

		// Token: 0x04000B8A RID: 2954
		public static readonly BitSet FOLLOW_elseBlock_in_ifBlock3998 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B8B RID: 2955
		public static readonly BitSet FOLLOW_ELSEIF_in_elseIfBlock4072 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B8C RID: 2956
		public static readonly BitSet FOLLOW_expression_in_elseIfBlock4074 = new(new[]
		{
			1024UL
		});

		// Token: 0x04000B8D RID: 2957
		public static readonly BitSet FOLLOW_codeBlock_in_elseIfBlock4076 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B8E RID: 2958
		public static readonly BitSet FOLLOW_ELSE_in_elseBlock4140 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B8F RID: 2959
		public static readonly BitSet FOLLOW_codeBlock_in_elseBlock4142 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B90 RID: 2960
		public static readonly BitSet FOLLOW_WHILE_in_whileBlock4183 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B91 RID: 2961
		public static readonly BitSet FOLLOW_expression_in_whileBlock4185 = new(new[]
		{
			1024UL
		});

		// Token: 0x04000B92 RID: 2962
		public static readonly BitSet FOLLOW_codeBlock_in_whileBlock4187 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B93 RID: 2963
		public static readonly BitSet FOLLOW_CALL_in_function_call4252 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B94 RID: 2964
		public static readonly BitSet FOLLOW_ID_in_function_call4256 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B95 RID: 2965
		public static readonly BitSet FOLLOW_ID_in_function_call4260 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B96 RID: 2966
		public static readonly BitSet FOLLOW_ID_in_function_call4264 = new(new[]
		{
			16384UL
		});

		// Token: 0x04000B97 RID: 2967
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4267 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B98 RID: 2968
		public static readonly BitSet FOLLOW_parameters_in_function_call4269 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B99 RID: 2969
		public static readonly BitSet FOLLOW_CALLPARENT_in_function_call4355 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B9A RID: 2970
		public static readonly BitSet FOLLOW_ID_in_function_call4359 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B9B RID: 2971
		public static readonly BitSet FOLLOW_ID_in_function_call4363 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000B9C RID: 2972
		public static readonly BitSet FOLLOW_ID_in_function_call4367 = new(new[]
		{
			16384UL
		});

		// Token: 0x04000B9D RID: 2973
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4370 = new(new[]
		{
			4UL
		});

		// Token: 0x04000B9E RID: 2974
		public static readonly BitSet FOLLOW_parameters_in_function_call4372 = new(new[]
		{
			8UL
		});

		// Token: 0x04000B9F RID: 2975
		public static readonly BitSet FOLLOW_CALLGLOBAL_in_function_call4448 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BA0 RID: 2976
		public static readonly BitSet FOLLOW_ID_in_function_call4452 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000BA1 RID: 2977
		public static readonly BitSet FOLLOW_ID_in_function_call4456 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000BA2 RID: 2978
		public static readonly BitSet FOLLOW_ID_in_function_call4460 = new(new[]
		{
			16384UL
		});

		// Token: 0x04000BA3 RID: 2979
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4463 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BA4 RID: 2980
		public static readonly BitSet FOLLOW_parameters_in_function_call4465 = new(new[]
		{
			8UL
		});

		// Token: 0x04000BA5 RID: 2981
		public static readonly BitSet FOLLOW_ARRAYFIND_in_function_call4551 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BA6 RID: 2982
		public static readonly BitSet FOLLOW_ID_in_function_call4555 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000BA7 RID: 2983
		public static readonly BitSet FOLLOW_ID_in_function_call4559 = new(new[]
		{
			16384UL
		});

		// Token: 0x04000BA8 RID: 2984
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4562 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BA9 RID: 2985
		public static readonly BitSet FOLLOW_parameters_in_function_call4564 = new(new[]
		{
			8UL
		});

		// Token: 0x04000BAA RID: 2986
		public static readonly BitSet FOLLOW_ARRAYRFIND_in_function_call4640 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BAB RID: 2987
		public static readonly BitSet FOLLOW_ID_in_function_call4644 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000BAC RID: 2988
		public static readonly BitSet FOLLOW_ID_in_function_call4648 = new(new[]
		{
			16384UL
		});

		// Token: 0x04000BAD RID: 2989
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4651 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BAE RID: 2990
		public static readonly BitSet FOLLOW_parameters_in_function_call4653 = new(new[]
		{
			8UL
		});

		// Token: 0x04000BAF RID: 2991
		public static readonly BitSet FOLLOW_parameter_in_parameters4751 = new(new[]
		{
			514UL
		});

		// Token: 0x04000BB0 RID: 2992
		public static readonly BitSet FOLLOW_PARAM_in_parameter4793 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BB1 RID: 2993
		public static readonly BitSet FOLLOW_autoCast_in_parameter4795 = new(new[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000BB2 RID: 2994
		public static readonly BitSet FOLLOW_expression_in_parameter4797 = new(new[]
		{
			8UL
		});

		// Token: 0x04000BB3 RID: 2995
		public static readonly BitSet FOLLOW_AS_in_autoCast4825 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BB4 RID: 2996
		public static readonly BitSet FOLLOW_ID_in_autoCast4829 = new(new[]
		{
			274877906944UL
		});

		// Token: 0x04000BB5 RID: 2997
		public static readonly BitSet FOLLOW_ID_in_autoCast4833 = new(new[]
		{
			8UL
		});

		// Token: 0x04000BB6 RID: 2998
		public static readonly BitSet FOLLOW_AS_in_autoCast4868 = new(new[]
		{
			4UL
		});

		// Token: 0x04000BB7 RID: 2999
		public static readonly BitSet FOLLOW_ID_in_autoCast4870 = new(new[]
		{
			0UL,
			1006764032UL
		});

		// Token: 0x04000BB8 RID: 3000
		public static readonly BitSet FOLLOW_constant_in_autoCast4872 = new(new[]
		{
			8UL
		});

		// Token: 0x04000BB9 RID: 3001
		public static readonly BitSet FOLLOW_ID_in_autoCast4906 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BBA RID: 3002
		public static readonly BitSet FOLLOW_constant_in_autoCast4917 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BBB RID: 3003
		public static readonly BitSet FOLLOW_number_in_constant4935 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BBC RID: 3004
		public static readonly BitSet FOLLOW_STRING_in_constant4941 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BBD RID: 3005
		public static readonly BitSet FOLLOW_BOOL_in_constant4947 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BBE RID: 3006
		public static readonly BitSet FOLLOW_NONE_in_constant4953 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BBF RID: 3007
		public static readonly BitSet FOLLOW_set_in_number4964 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BC0 RID: 3008
		public static readonly BitSet FOLLOW_ID_in_type4985 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BC1 RID: 3009
		public static readonly BitSet FOLLOW_ID_in_type4996 = new(new[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000BC2 RID: 3010
		public static readonly BitSet FOLLOW_LBRACKET_in_type4998 = new(new[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000BC3 RID: 3011
		public static readonly BitSet FOLLOW_RBRACKET_in_type5000 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BC4 RID: 3012
		public static readonly BitSet FOLLOW_BASETYPE_in_type5011 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BC5 RID: 3013
		public static readonly BitSet FOLLOW_BASETYPE_in_type5022 = new(new[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000BC6 RID: 3014
		public static readonly BitSet FOLLOW_LBRACKET_in_type5024 = new(new[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000BC7 RID: 3015
		public static readonly BitSet FOLLOW_RBRACKET_in_type5026 = new(new[]
		{
			2UL
		});

		// Token: 0x04000BC8 RID: 3016
		private Dictionary<string, PapyrusFlag> kFlagDict;

		// Token: 0x04000BC9 RID: 3017
		private ScriptObjectType kObjType;

		// Token: 0x04000BCA RID: 3018
		private int iCurMangleSuffix;

		// Token: 0x04000BCB RID: 3019
		private int iCurLabelSuffix;

		// Token: 0x020001C5 RID: 453
		protected class STAttrMap : Hashtable
		{
			// Token: 0x06000DB2 RID: 3506 RVA: 0x0006B604 File Offset: 0x00069804
			public STAttrMap Add(string attrName, object value)
			{
				base.Add(attrName, value);
				return this;
			}

			// Token: 0x06000DB3 RID: 3507 RVA: 0x0006B610 File Offset: 0x00069810
			public STAttrMap Add(string attrName, int value)
			{
				base.Add(attrName, value);
				return this;
			}
		}

		// Token: 0x020001C6 RID: 454
		protected record script_scope
		{
			// Token: 0x04000BCC RID: 3020
			protected internal string sobjName;

			// Token: 0x04000BCD RID: 3021
			protected internal string sparentName;

			// Token: 0x04000BCE RID: 3022
			protected internal IList kobjVarDefinitions;

			// Token: 0x04000BCF RID: 3023
			protected internal IList kobjPropDefinitions;

			// Token: 0x04000BD0 RID: 3024
			protected internal string sinitialState;

			// Token: 0x04000BD1 RID: 3025
			protected internal IList kobjEmptyState;

			// Token: 0x04000BD2 RID: 3026
			protected internal Hashtable kstates;

			// Token: 0x04000BD3 RID: 3027
			protected internal bool bhasBeginStateEvent;

			// Token: 0x04000BD4 RID: 3028
			protected internal bool bhasEndStateEvent;

			// Token: 0x04000BD5 RID: 3029
			protected internal string smodTimeUnix;

			// Token: 0x04000BD6 RID: 3030
			protected internal string scompileTimeUnix;

			// Token: 0x04000BD7 RID: 3031
			protected internal string suserName;

			// Token: 0x04000BD8 RID: 3032
			protected internal string scomputerName;

			// Token: 0x04000BD9 RID: 3033
			protected internal string sobjFlags;

			// Token: 0x04000BDA RID: 3034
			protected internal Hashtable kuserFlagsRef;

			// Token: 0x04000BDB RID: 3035
			protected internal string sdocString;
		}

		// Token: 0x020001C7 RID: 455
		public class script_return : TreeRuleReturnScope
		{
			// Token: 0x170001BA RID: 442
			// (get) Token: 0x06000DB6 RID: 3510 RVA: 0x0006B630 File Offset: 0x00069830
			// (set) Token: 0x06000DB7 RID: 3511 RVA: 0x0006B638 File Offset: 0x00069838
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001BB RID: 443
			// (get) Token: 0x06000DB8 RID: 3512 RVA: 0x0006B644 File Offset: 0x00069844
			public override object Template => st;

            // Token: 0x06000DB9 RID: 3513 RVA: 0x0006B64C File Offset: 0x0006984C
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BDC RID: 3036
			private StringTemplate st;
		}

		// Token: 0x020001C8 RID: 456
		public class header_return : TreeRuleReturnScope
		{
			// Token: 0x170001BC RID: 444
			// (get) Token: 0x06000DBB RID: 3515 RVA: 0x0006B66C File Offset: 0x0006986C
			// (set) Token: 0x06000DBC RID: 3516 RVA: 0x0006B674 File Offset: 0x00069874
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001BD RID: 445
			// (get) Token: 0x06000DBD RID: 3517 RVA: 0x0006B680 File Offset: 0x00069880
			public override object Template => st;

            // Token: 0x06000DBE RID: 3518 RVA: 0x0006B688 File Offset: 0x00069888
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BDD RID: 3037
			private StringTemplate st;
		}

		// Token: 0x020001C9 RID: 457
		public class definitionOrBlock_return : TreeRuleReturnScope
		{
			// Token: 0x170001BE RID: 446
			// (get) Token: 0x06000DC0 RID: 3520 RVA: 0x0006B6A8 File Offset: 0x000698A8
			// (set) Token: 0x06000DC1 RID: 3521 RVA: 0x0006B6B0 File Offset: 0x000698B0
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001BF RID: 447
			// (get) Token: 0x06000DC2 RID: 3522 RVA: 0x0006B6BC File Offset: 0x000698BC
			public override object Template => st;

            // Token: 0x06000DC3 RID: 3523 RVA: 0x0006B6C4 File Offset: 0x000698C4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BDE RID: 3038
			private StringTemplate st;
		}

		// Token: 0x020001CA RID: 458
		protected record fieldDefinition_scope
		{
			// Token: 0x04000BDF RID: 3039
			protected internal string sinitialValue;
		}

		// Token: 0x020001CB RID: 459
		public class fieldDefinition_return : TreeRuleReturnScope
		{
			// Token: 0x170001C0 RID: 448
			// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x0006B6EC File Offset: 0x000698EC
			// (set) Token: 0x06000DC7 RID: 3527 RVA: 0x0006B6F4 File Offset: 0x000698F4
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001C1 RID: 449
			// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x0006B700 File Offset: 0x00069900
			public override object Template => st;

            // Token: 0x06000DC9 RID: 3529 RVA: 0x0006B708 File Offset: 0x00069908
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BE0 RID: 3040
			private StringTemplate st;
		}

		// Token: 0x020001CC RID: 460
		protected record function_scope
		{
			// Token: 0x04000BE1 RID: 3041
			protected internal string sstate;

			// Token: 0x04000BE2 RID: 3042
			protected internal string sfuncName;

			// Token: 0x04000BE3 RID: 3043
			protected internal string spropertyName;

			// Token: 0x04000BE4 RID: 3044
			protected internal string sreturnType;

			// Token: 0x04000BE5 RID: 3045
			protected internal bool bisNative;

			// Token: 0x04000BE6 RID: 3046
			protected internal bool bisGlobal;

			// Token: 0x04000BE7 RID: 3047
			protected internal IList kfuncParams;

			// Token: 0x04000BE8 RID: 3048
			protected internal IList kfuncVarDefinitions;

			// Token: 0x04000BE9 RID: 3049
			protected internal IList kstatements;

			// Token: 0x04000BEA RID: 3050
			protected internal string suserFlags;

			// Token: 0x04000BEB RID: 3051
			protected internal string sdocString;

			// Token: 0x04000BEC RID: 3052
			protected internal ScriptFunctionType kfuncType;
		}

		// Token: 0x020001CD RID: 461
		public class function_return : TreeRuleReturnScope
		{
			// Token: 0x170001C2 RID: 450
			// (get) Token: 0x06000DCC RID: 3532 RVA: 0x0006B730 File Offset: 0x00069930
			// (set) Token: 0x06000DCD RID: 3533 RVA: 0x0006B738 File Offset: 0x00069938
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001C3 RID: 451
			// (get) Token: 0x06000DCE RID: 3534 RVA: 0x0006B744 File Offset: 0x00069944
			public override object Template => st;

            // Token: 0x06000DCF RID: 3535 RVA: 0x0006B74C File Offset: 0x0006994C
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BED RID: 3053
			public string sName;

			// Token: 0x04000BEE RID: 3054
			private StringTemplate st;
		}

		// Token: 0x020001CE RID: 462
		public class functionHeader_return : TreeRuleReturnScope
		{
			// Token: 0x170001C4 RID: 452
			// (get) Token: 0x06000DD1 RID: 3537 RVA: 0x0006B76C File Offset: 0x0006996C
			// (set) Token: 0x06000DD2 RID: 3538 RVA: 0x0006B774 File Offset: 0x00069974
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001C5 RID: 453
			// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x0006B780 File Offset: 0x00069980
			public override object Template => st;

            // Token: 0x06000DD4 RID: 3540 RVA: 0x0006B788 File Offset: 0x00069988
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BEF RID: 3055
			private StringTemplate st;
		}

		// Token: 0x020001CF RID: 463
		public class functionModifier_return : TreeRuleReturnScope
		{
			// Token: 0x170001C6 RID: 454
			// (get) Token: 0x06000DD6 RID: 3542 RVA: 0x0006B7A8 File Offset: 0x000699A8
			// (set) Token: 0x06000DD7 RID: 3543 RVA: 0x0006B7B0 File Offset: 0x000699B0
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001C7 RID: 455
			// (get) Token: 0x06000DD8 RID: 3544 RVA: 0x0006B7BC File Offset: 0x000699BC
			public override object Template => st;

            // Token: 0x06000DD9 RID: 3545 RVA: 0x0006B7C4 File Offset: 0x000699C4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BF0 RID: 3056
			private StringTemplate st;
		}

		// Token: 0x020001D0 RID: 464
		protected record eventFunc_scope
		{
			// Token: 0x04000BF1 RID: 3057
			protected internal string sstate;

			// Token: 0x04000BF2 RID: 3058
			protected internal string sfuncName;

			// Token: 0x04000BF3 RID: 3059
			protected internal string sreturnType;

			// Token: 0x04000BF4 RID: 3060
			protected internal bool bisNative;

			// Token: 0x04000BF5 RID: 3061
			protected internal bool bisGlobal;

			// Token: 0x04000BF6 RID: 3062
			protected internal IList kfuncParams;

			// Token: 0x04000BF7 RID: 3063
			protected internal IList kfuncVarDefinitions;

			// Token: 0x04000BF8 RID: 3064
			protected internal IList kstatements;

			// Token: 0x04000BF9 RID: 3065
			protected internal string suserFlags;

			// Token: 0x04000BFA RID: 3066
			protected internal string sdocString;

			// Token: 0x04000BFB RID: 3067
			protected internal ScriptFunctionType kfuncType;
		}

		// Token: 0x020001D1 RID: 465
		public class eventFunc_return : TreeRuleReturnScope
		{
			// Token: 0x170001C8 RID: 456
			// (get) Token: 0x06000DDC RID: 3548 RVA: 0x0006B7EC File Offset: 0x000699EC
			// (set) Token: 0x06000DDD RID: 3549 RVA: 0x0006B7F4 File Offset: 0x000699F4
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001C9 RID: 457
			// (get) Token: 0x06000DDE RID: 3550 RVA: 0x0006B800 File Offset: 0x00069A00
			public override object Template => st;

            // Token: 0x06000DDF RID: 3551 RVA: 0x0006B808 File Offset: 0x00069A08
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BFC RID: 3068
			public string sName;

			// Token: 0x04000BFD RID: 3069
			private StringTemplate st;
		}

		// Token: 0x020001D2 RID: 466
		public class eventHeader_return : TreeRuleReturnScope
		{
			// Token: 0x170001CA RID: 458
			// (get) Token: 0x06000DE1 RID: 3553 RVA: 0x0006B828 File Offset: 0x00069A28
			// (set) Token: 0x06000DE2 RID: 3554 RVA: 0x0006B830 File Offset: 0x00069A30
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001CB RID: 459
			// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x0006B83C File Offset: 0x00069A3C
			public override object Template => st;

            // Token: 0x06000DE4 RID: 3556 RVA: 0x0006B844 File Offset: 0x00069A44
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BFE RID: 3070
			private StringTemplate st;
		}

		// Token: 0x020001D3 RID: 467
		public class callParameters_return : TreeRuleReturnScope
		{
			// Token: 0x170001CC RID: 460
			// (get) Token: 0x06000DE6 RID: 3558 RVA: 0x0006B864 File Offset: 0x00069A64
			// (set) Token: 0x06000DE7 RID: 3559 RVA: 0x0006B86C File Offset: 0x00069A6C
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001CD RID: 461
			// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x0006B878 File Offset: 0x00069A78
			public override object Template => st;

            // Token: 0x06000DE9 RID: 3561 RVA: 0x0006B880 File Offset: 0x00069A80
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000BFF RID: 3071
			public IList kParams;

			// Token: 0x04000C00 RID: 3072
			private StringTemplate st;
		}

		// Token: 0x020001D4 RID: 468
		public class callParameter_return : TreeRuleReturnScope
		{
			// Token: 0x170001CE RID: 462
			// (get) Token: 0x06000DEB RID: 3563 RVA: 0x0006B8A0 File Offset: 0x00069AA0
			// (set) Token: 0x06000DEC RID: 3564 RVA: 0x0006B8A8 File Offset: 0x00069AA8
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001CF RID: 463
			// (get) Token: 0x06000DED RID: 3565 RVA: 0x0006B8B4 File Offset: 0x00069AB4
			public override object Template => st;

            // Token: 0x06000DEE RID: 3566 RVA: 0x0006B8BC File Offset: 0x00069ABC
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C01 RID: 3073
			private StringTemplate st;
		}

		// Token: 0x020001D5 RID: 469
		public class stateBlock_return : TreeRuleReturnScope
		{
			// Token: 0x170001D0 RID: 464
			// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x0006B8DC File Offset: 0x00069ADC
			// (set) Token: 0x06000DF1 RID: 3569 RVA: 0x0006B8E4 File Offset: 0x00069AE4
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001D1 RID: 465
			// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x0006B8F0 File Offset: 0x00069AF0
			public override object Template => st;

            // Token: 0x06000DF3 RID: 3571 RVA: 0x0006B8F8 File Offset: 0x00069AF8
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C02 RID: 3074
			private StringTemplate st;
		}

		// Token: 0x020001D6 RID: 470
		public class stateFuncOrEvent_return : TreeRuleReturnScope
		{
			// Token: 0x170001D2 RID: 466
			// (get) Token: 0x06000DF5 RID: 3573 RVA: 0x0006B918 File Offset: 0x00069B18
			// (set) Token: 0x06000DF6 RID: 3574 RVA: 0x0006B920 File Offset: 0x00069B20
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001D3 RID: 467
			// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x0006B92C File Offset: 0x00069B2C
			public override object Template => st;

            // Token: 0x06000DF8 RID: 3576 RVA: 0x0006B934 File Offset: 0x00069B34
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C03 RID: 3075
			private StringTemplate st;
		}

		// Token: 0x020001D7 RID: 471
		protected record propertyBlock_scope
		{
			// Token: 0x04000C04 RID: 3076
			protected internal string spropName;

			// Token: 0x04000C05 RID: 3077
			protected internal string spropType;

			// Token: 0x04000C06 RID: 3078
			protected internal string suserFlags;

			// Token: 0x04000C07 RID: 3079
			protected internal string sdocString;
		}

		// Token: 0x020001D8 RID: 472
		public class propertyBlock_return : TreeRuleReturnScope
		{
			// Token: 0x170001D4 RID: 468
			// (get) Token: 0x06000DFB RID: 3579 RVA: 0x0006B95C File Offset: 0x00069B5C
			// (set) Token: 0x06000DFC RID: 3580 RVA: 0x0006B964 File Offset: 0x00069B64
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001D5 RID: 469
			// (get) Token: 0x06000DFD RID: 3581 RVA: 0x0006B970 File Offset: 0x00069B70
			public override object Template => st;

            // Token: 0x06000DFE RID: 3582 RVA: 0x0006B978 File Offset: 0x00069B78
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C08 RID: 3080
			private StringTemplate st;
		}

		// Token: 0x020001D9 RID: 473
		public class propertyHeader_return : TreeRuleReturnScope
		{
			// Token: 0x170001D6 RID: 470
			// (get) Token: 0x06000E00 RID: 3584 RVA: 0x0006B998 File Offset: 0x00069B98
			// (set) Token: 0x06000E01 RID: 3585 RVA: 0x0006B9A0 File Offset: 0x00069BA0
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001D7 RID: 471
			// (get) Token: 0x06000E02 RID: 3586 RVA: 0x0006B9AC File Offset: 0x00069BAC
			public override object Template => st;

            // Token: 0x06000E03 RID: 3587 RVA: 0x0006B9B4 File Offset: 0x00069BB4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C09 RID: 3081
			private StringTemplate st;
		}

		// Token: 0x020001DA RID: 474
		public class propertyFunc_return : TreeRuleReturnScope
		{
			// Token: 0x170001D8 RID: 472
			// (get) Token: 0x06000E05 RID: 3589 RVA: 0x0006B9D4 File Offset: 0x00069BD4
			// (set) Token: 0x06000E06 RID: 3590 RVA: 0x0006B9DC File Offset: 0x00069BDC
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001D9 RID: 473
			// (get) Token: 0x06000E07 RID: 3591 RVA: 0x0006B9E8 File Offset: 0x00069BE8
			public override object Template => st;

            // Token: 0x06000E08 RID: 3592 RVA: 0x0006B9F0 File Offset: 0x00069BF0
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C0A RID: 3082
			private StringTemplate st;
		}

		// Token: 0x020001DB RID: 475
		protected record codeBlock_scope
		{
			// Token: 0x04000C0B RID: 3083
			protected internal IList kvarDefs;

			// Token: 0x04000C0C RID: 3084
			protected internal ScriptScope kcurrentScope;

			// Token: 0x04000C0D RID: 3085
			protected internal int inextScopeChild;
		}

		// Token: 0x020001DC RID: 476
		public class codeBlock_return : TreeRuleReturnScope
		{
			// Token: 0x170001DA RID: 474
			// (get) Token: 0x06000E0B RID: 3595 RVA: 0x0006BA18 File Offset: 0x00069C18
			// (set) Token: 0x06000E0C RID: 3596 RVA: 0x0006BA20 File Offset: 0x00069C20
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001DB RID: 475
			// (get) Token: 0x06000E0D RID: 3597 RVA: 0x0006BA2C File Offset: 0x00069C2C
			public override object Template => st;

            // Token: 0x06000E0E RID: 3598 RVA: 0x0006BA34 File Offset: 0x00069C34
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C0E RID: 3086
			private StringTemplate st;
		}

		// Token: 0x020001DD RID: 477
		protected class statement_scope
		{
			// Token: 0x04000C0F RID: 3087
			protected internal string smangledName;
		}

		// Token: 0x020001DE RID: 478
		public class statement_return : TreeRuleReturnScope
		{
			// Token: 0x170001DC RID: 476
			// (get) Token: 0x06000E11 RID: 3601 RVA: 0x0006BA5C File Offset: 0x00069C5C
			// (set) Token: 0x06000E12 RID: 3602 RVA: 0x0006BA64 File Offset: 0x00069C64
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001DD RID: 477
			// (get) Token: 0x06000E13 RID: 3603 RVA: 0x0006BA70 File Offset: 0x00069C70
			public override object Template => st;

            // Token: 0x06000E14 RID: 3604 RVA: 0x0006BA78 File Offset: 0x00069C78
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C10 RID: 3088
			private StringTemplate st;
		}

		// Token: 0x020001DF RID: 479
		public class localDefinition_return : TreeRuleReturnScope
		{
			// Token: 0x170001DE RID: 478
			// (get) Token: 0x06000E16 RID: 3606 RVA: 0x0006BA98 File Offset: 0x00069C98
			// (set) Token: 0x06000E17 RID: 3607 RVA: 0x0006BAA0 File Offset: 0x00069CA0
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001DF RID: 479
			// (get) Token: 0x06000E18 RID: 3608 RVA: 0x0006BAAC File Offset: 0x00069CAC
			public override object Template => st;

            // Token: 0x06000E19 RID: 3609 RVA: 0x0006BAB4 File Offset: 0x00069CB4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C11 RID: 3089
			public string sVarName;

			// Token: 0x04000C12 RID: 3090
			public string sExprVar;

			// Token: 0x04000C13 RID: 3091
			public StringTemplate kAutoCastST;

			// Token: 0x04000C14 RID: 3092
			public StringTemplate kExprST;

			// Token: 0x04000C15 RID: 3093
			public int iLineNo;

			// Token: 0x04000C16 RID: 3094
			private StringTemplate st;
		}

		// Token: 0x020001E0 RID: 480
		protected record l_value_scope
		{
			// Token: 0x04000C17 RID: 3095
			protected internal string ssourceName;

			// Token: 0x04000C18 RID: 3096
			protected internal string sselfName;
		}

		// Token: 0x020001E1 RID: 481
		public class l_value_return : TreeRuleReturnScope
		{
			// Token: 0x170001E0 RID: 480
			// (get) Token: 0x06000E1C RID: 3612 RVA: 0x0006BADC File Offset: 0x00069CDC
			// (set) Token: 0x06000E1D RID: 3613 RVA: 0x0006BAE4 File Offset: 0x00069CE4
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001E1 RID: 481
			// (get) Token: 0x06000E1E RID: 3614 RVA: 0x0006BAF0 File Offset: 0x00069CF0
			public override object Template => st;

            // Token: 0x06000E1F RID: 3615 RVA: 0x0006BAF8 File Offset: 0x00069CF8
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C19 RID: 3097
			private StringTemplate st;
		}

		// Token: 0x020001E2 RID: 482
		protected record basic_l_value_scope
		{
			// Token: 0x04000C1A RID: 3098
			protected internal string ssourceName;

			// Token: 0x04000C1B RID: 3099
			protected internal string sselfName;
		}

		// Token: 0x020001E3 RID: 483
		public class basic_l_value_return : TreeRuleReturnScope
		{
			// Token: 0x170001E2 RID: 482
			// (get) Token: 0x06000E22 RID: 3618 RVA: 0x0006BB20 File Offset: 0x00069D20
			// (set) Token: 0x06000E23 RID: 3619 RVA: 0x0006BB28 File Offset: 0x00069D28
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001E3 RID: 483
			// (get) Token: 0x06000E24 RID: 3620 RVA: 0x0006BB34 File Offset: 0x00069D34
			public override object Template => st;

            // Token: 0x06000E25 RID: 3621 RVA: 0x0006BB3C File Offset: 0x00069D3C
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C1C RID: 3100
			private StringTemplate st;
		}

		// Token: 0x020001E4 RID: 484
		public class expression_return : TreeRuleReturnScope
		{
			// Token: 0x170001E4 RID: 484
			// (get) Token: 0x06000E27 RID: 3623 RVA: 0x0006BB5C File Offset: 0x00069D5C
			// (set) Token: 0x06000E28 RID: 3624 RVA: 0x0006BB64 File Offset: 0x00069D64
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001E5 RID: 485
			// (get) Token: 0x06000E29 RID: 3625 RVA: 0x0006BB70 File Offset: 0x00069D70
			public override object Template => st;

            // Token: 0x06000E2A RID: 3626 RVA: 0x0006BB78 File Offset: 0x00069D78
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C1D RID: 3101
			public string sRetValue;

			// Token: 0x04000C1E RID: 3102
			private StringTemplate st;
		}

		// Token: 0x020001E5 RID: 485
		public class and_expression_return : TreeRuleReturnScope
		{
			// Token: 0x170001E6 RID: 486
			// (get) Token: 0x06000E2C RID: 3628 RVA: 0x0006BB98 File Offset: 0x00069D98
			// (set) Token: 0x06000E2D RID: 3629 RVA: 0x0006BBA0 File Offset: 0x00069DA0
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001E7 RID: 487
			// (get) Token: 0x06000E2E RID: 3630 RVA: 0x0006BBAC File Offset: 0x00069DAC
			public override object Template => st;

            // Token: 0x06000E2F RID: 3631 RVA: 0x0006BBB4 File Offset: 0x00069DB4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C1F RID: 3103
			public string sRetValue;

			// Token: 0x04000C20 RID: 3104
			private StringTemplate st;
		}

		// Token: 0x020001E6 RID: 486
		public class bool_expression_return : TreeRuleReturnScope
		{
			// Token: 0x170001E8 RID: 488
			// (get) Token: 0x06000E31 RID: 3633 RVA: 0x0006BBD4 File Offset: 0x00069DD4
			// (set) Token: 0x06000E32 RID: 3634 RVA: 0x0006BBDC File Offset: 0x00069DDC
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001E9 RID: 489
			// (get) Token: 0x06000E33 RID: 3635 RVA: 0x0006BBE8 File Offset: 0x00069DE8
			public override object Template => st;

            // Token: 0x06000E34 RID: 3636 RVA: 0x0006BBF0 File Offset: 0x00069DF0
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C21 RID: 3105
			public string sRetValue;

			// Token: 0x04000C22 RID: 3106
			private StringTemplate st;
		}

		// Token: 0x020001E7 RID: 487
		public class add_expression_return : TreeRuleReturnScope
		{
			// Token: 0x170001EA RID: 490
			// (get) Token: 0x06000E36 RID: 3638 RVA: 0x0006BC10 File Offset: 0x00069E10
			// (set) Token: 0x06000E37 RID: 3639 RVA: 0x0006BC18 File Offset: 0x00069E18
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001EB RID: 491
			// (get) Token: 0x06000E38 RID: 3640 RVA: 0x0006BC24 File Offset: 0x00069E24
			public override object Template => st;

            // Token: 0x06000E39 RID: 3641 RVA: 0x0006BC2C File Offset: 0x00069E2C
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C23 RID: 3107
			public string sRetValue;

			// Token: 0x04000C24 RID: 3108
			private StringTemplate st;
		}

		// Token: 0x020001E8 RID: 488
		public class mult_expression_return : TreeRuleReturnScope
		{
			// Token: 0x170001EC RID: 492
			// (get) Token: 0x06000E3B RID: 3643 RVA: 0x0006BC4C File Offset: 0x00069E4C
			// (set) Token: 0x06000E3C RID: 3644 RVA: 0x0006BC54 File Offset: 0x00069E54
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001ED RID: 493
			// (get) Token: 0x06000E3D RID: 3645 RVA: 0x0006BC60 File Offset: 0x00069E60
			public override object Template => st;

            // Token: 0x06000E3E RID: 3646 RVA: 0x0006BC68 File Offset: 0x00069E68
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C25 RID: 3109
			public string sRetValue;

			// Token: 0x04000C26 RID: 3110
			private StringTemplate st;
		}

		// Token: 0x020001E9 RID: 489
		public class unary_expression_return : TreeRuleReturnScope
		{
			// Token: 0x170001EE RID: 494
			// (get) Token: 0x06000E40 RID: 3648 RVA: 0x0006BC88 File Offset: 0x00069E88
			// (set) Token: 0x06000E41 RID: 3649 RVA: 0x0006BC90 File Offset: 0x00069E90
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001EF RID: 495
			// (get) Token: 0x06000E42 RID: 3650 RVA: 0x0006BC9C File Offset: 0x00069E9C
			public override object Template => st;

            // Token: 0x06000E43 RID: 3651 RVA: 0x0006BCA4 File Offset: 0x00069EA4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C27 RID: 3111
			public string sRetValue;

			// Token: 0x04000C28 RID: 3112
			private StringTemplate st;
		}

		// Token: 0x020001EA RID: 490
		public class cast_atom_return : TreeRuleReturnScope
		{
			// Token: 0x170001F0 RID: 496
			// (get) Token: 0x06000E45 RID: 3653 RVA: 0x0006BCC4 File Offset: 0x00069EC4
			// (set) Token: 0x06000E46 RID: 3654 RVA: 0x0006BCCC File Offset: 0x00069ECC
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001F1 RID: 497
			// (get) Token: 0x06000E47 RID: 3655 RVA: 0x0006BCD8 File Offset: 0x00069ED8
			public override object Template => st;

            // Token: 0x06000E48 RID: 3656 RVA: 0x0006BCE0 File Offset: 0x00069EE0
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C29 RID: 3113
			public string sRetValue;

			// Token: 0x04000C2A RID: 3114
			private StringTemplate st;
		}

		// Token: 0x020001EB RID: 491
		public class dot_atom_return : TreeRuleReturnScope
		{
			// Token: 0x170001F2 RID: 498
			// (get) Token: 0x06000E4A RID: 3658 RVA: 0x0006BD00 File Offset: 0x00069F00
			// (set) Token: 0x06000E4B RID: 3659 RVA: 0x0006BD08 File Offset: 0x00069F08
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001F3 RID: 499
			// (get) Token: 0x06000E4C RID: 3660 RVA: 0x0006BD14 File Offset: 0x00069F14
			public override object Template => st;

            // Token: 0x06000E4D RID: 3661 RVA: 0x0006BD1C File Offset: 0x00069F1C
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C2B RID: 3115
			public string sRetValue;

			// Token: 0x04000C2C RID: 3116
			private StringTemplate st;
		}

		// Token: 0x020001EC RID: 492
		protected class array_atom_scope
		{
			// Token: 0x04000C2D RID: 3117
			protected internal string sselfName;
		}

		// Token: 0x020001ED RID: 493
		public class array_atom_return : TreeRuleReturnScope
		{
			// Token: 0x170001F4 RID: 500
			// (get) Token: 0x06000E50 RID: 3664 RVA: 0x0006BD44 File Offset: 0x00069F44
			// (set) Token: 0x06000E51 RID: 3665 RVA: 0x0006BD4C File Offset: 0x00069F4C
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001F5 RID: 501
			// (get) Token: 0x06000E52 RID: 3666 RVA: 0x0006BD58 File Offset: 0x00069F58
			public override object Template => st;

            // Token: 0x06000E53 RID: 3667 RVA: 0x0006BD60 File Offset: 0x00069F60
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C2E RID: 3118
			public string sRetValue;

			// Token: 0x04000C2F RID: 3119
			private StringTemplate st;
		}

		// Token: 0x020001EE RID: 494
		public class atom_return : TreeRuleReturnScope
		{
			// Token: 0x170001F6 RID: 502
			// (get) Token: 0x06000E55 RID: 3669 RVA: 0x0006BD80 File Offset: 0x00069F80
			// (set) Token: 0x06000E56 RID: 3670 RVA: 0x0006BD88 File Offset: 0x00069F88
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001F7 RID: 503
			// (get) Token: 0x06000E57 RID: 3671 RVA: 0x0006BD94 File Offset: 0x00069F94
			public override object Template => st;

            // Token: 0x06000E58 RID: 3672 RVA: 0x0006BD9C File Offset: 0x00069F9C
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C30 RID: 3120
			public string sRetValue;

			// Token: 0x04000C31 RID: 3121
			private StringTemplate st;
		}

		// Token: 0x020001EF RID: 495
		protected class array_func_or_id_scope
		{
			// Token: 0x04000C32 RID: 3122
			protected internal string sselfName;
		}

		// Token: 0x020001F0 RID: 496
		public class array_func_or_id_return : TreeRuleReturnScope
		{
			// Token: 0x170001F8 RID: 504
			// (get) Token: 0x06000E5B RID: 3675 RVA: 0x0006BDC4 File Offset: 0x00069FC4
			// (set) Token: 0x06000E5C RID: 3676 RVA: 0x0006BDCC File Offset: 0x00069FCC
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001F9 RID: 505
			// (get) Token: 0x06000E5D RID: 3677 RVA: 0x0006BDD8 File Offset: 0x00069FD8
			public override object Template => st;

            // Token: 0x06000E5E RID: 3678 RVA: 0x0006BDE0 File Offset: 0x00069FE0
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C33 RID: 3123
			public string sRetValue;

			// Token: 0x04000C34 RID: 3124
			private StringTemplate st;
		}

		// Token: 0x020001F1 RID: 497
		protected class func_or_id_scope
		{
			// Token: 0x04000C35 RID: 3125
			protected internal string sselfName;
		}

		// Token: 0x020001F2 RID: 498
		public class func_or_id_return : TreeRuleReturnScope
		{
			// Token: 0x170001FA RID: 506
			// (get) Token: 0x06000E61 RID: 3681 RVA: 0x0006BE08 File Offset: 0x0006A008
			// (set) Token: 0x06000E62 RID: 3682 RVA: 0x0006BE10 File Offset: 0x0006A010
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001FB RID: 507
			// (get) Token: 0x06000E63 RID: 3683 RVA: 0x0006BE1C File Offset: 0x0006A01C
			public override object Template => st;

            // Token: 0x06000E64 RID: 3684 RVA: 0x0006BE24 File Offset: 0x0006A024
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C36 RID: 3126
			public string sRetValue;

			// Token: 0x04000C37 RID: 3127
			private StringTemplate st;
		}

		// Token: 0x020001F3 RID: 499
		protected class property_set_scope
		{
			// Token: 0x04000C38 RID: 3128
			protected internal string sselfName;

			// Token: 0x04000C39 RID: 3129
			protected internal string sparamName;
		}

		// Token: 0x020001F4 RID: 500
		public class property_set_return : TreeRuleReturnScope
		{
			// Token: 0x170001FC RID: 508
			// (get) Token: 0x06000E67 RID: 3687 RVA: 0x0006BE4C File Offset: 0x0006A04C
			// (set) Token: 0x06000E68 RID: 3688 RVA: 0x0006BE54 File Offset: 0x0006A054
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001FD RID: 509
			// (get) Token: 0x06000E69 RID: 3689 RVA: 0x0006BE60 File Offset: 0x0006A060
			public override object Template => st;

            // Token: 0x06000E6A RID: 3690 RVA: 0x0006BE68 File Offset: 0x0006A068
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C3A RID: 3130
			private StringTemplate st;
		}

		// Token: 0x020001F5 RID: 501
		public class return_stat_return : TreeRuleReturnScope
		{
			// Token: 0x170001FE RID: 510
			// (get) Token: 0x06000E6C RID: 3692 RVA: 0x0006BE88 File Offset: 0x0006A088
			// (set) Token: 0x06000E6D RID: 3693 RVA: 0x0006BE90 File Offset: 0x0006A090
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x170001FF RID: 511
			// (get) Token: 0x06000E6E RID: 3694 RVA: 0x0006BE9C File Offset: 0x0006A09C
			public override object Template => st;

            // Token: 0x06000E6F RID: 3695 RVA: 0x0006BEA4 File Offset: 0x0006A0A4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C3B RID: 3131
			private StringTemplate st;
		}

		// Token: 0x020001F6 RID: 502
		protected class ifBlock_scope
		{
			// Token: 0x04000C3C RID: 3132
			protected internal IList kBlockStatements;

			// Token: 0x04000C3D RID: 3133
			protected internal string sEndLabel;

			// Token: 0x04000C3E RID: 3134
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x020001F7 RID: 503
		public class ifBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000200 RID: 512
			// (get) Token: 0x06000E72 RID: 3698 RVA: 0x0006BECC File Offset: 0x0006A0CC
			// (set) Token: 0x06000E73 RID: 3699 RVA: 0x0006BED4 File Offset: 0x0006A0D4
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x17000201 RID: 513
			// (get) Token: 0x06000E74 RID: 3700 RVA: 0x0006BEE0 File Offset: 0x0006A0E0
			public override object Template => st;

            // Token: 0x06000E75 RID: 3701 RVA: 0x0006BEE8 File Offset: 0x0006A0E8
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C3F RID: 3135
			private StringTemplate st;
		}

		// Token: 0x020001F8 RID: 504
		protected class elseIfBlock_scope
		{
			// Token: 0x04000C40 RID: 3136
			protected internal IList kBlockStatements;

			// Token: 0x04000C41 RID: 3137
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x020001F9 RID: 505
		public class elseIfBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000202 RID: 514
			// (get) Token: 0x06000E78 RID: 3704 RVA: 0x0006BF10 File Offset: 0x0006A110
			// (set) Token: 0x06000E79 RID: 3705 RVA: 0x0006BF18 File Offset: 0x0006A118
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x17000203 RID: 515
			// (get) Token: 0x06000E7A RID: 3706 RVA: 0x0006BF24 File Offset: 0x0006A124
			public override object Template => st;

            // Token: 0x06000E7B RID: 3707 RVA: 0x0006BF2C File Offset: 0x0006A12C
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C42 RID: 3138
			private StringTemplate st;
		}

		// Token: 0x020001FA RID: 506
		protected record elseBlock_scope
		{
			// Token: 0x04000C43 RID: 3139
			protected internal IList kBlockStatements;

			// Token: 0x04000C44 RID: 3140
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x020001FB RID: 507
		public class elseBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000204 RID: 516
			// (get) Token: 0x06000E7E RID: 3710 RVA: 0x0006BF54 File Offset: 0x0006A154
			// (set) Token: 0x06000E7F RID: 3711 RVA: 0x0006BF5C File Offset: 0x0006A15C
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x17000205 RID: 517
			// (get) Token: 0x06000E80 RID: 3712 RVA: 0x0006BF68 File Offset: 0x0006A168
			public override object Template => st;

            // Token: 0x06000E81 RID: 3713 RVA: 0x0006BF70 File Offset: 0x0006A170
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C45 RID: 3141
			private StringTemplate st;
		}

		// Token: 0x020001FC RID: 508
		protected record whileBlock_scope
		{
			// Token: 0x04000C46 RID: 3142
			protected internal IList kBlockStatements;

			// Token: 0x04000C47 RID: 3143
			protected internal string sStartLabel;

			// Token: 0x04000C48 RID: 3144
			protected internal string sEndLabel;

			// Token: 0x04000C49 RID: 3145
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x020001FD RID: 509
		public class whileBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000206 RID: 518
			// (get) Token: 0x06000E84 RID: 3716 RVA: 0x0006BF98 File Offset: 0x0006A198
			// (set) Token: 0x06000E85 RID: 3717 RVA: 0x0006BFA0 File Offset: 0x0006A1A0
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x17000207 RID: 519
			// (get) Token: 0x06000E86 RID: 3718 RVA: 0x0006BFAC File Offset: 0x0006A1AC
			public override object Template => st;

            // Token: 0x06000E87 RID: 3719 RVA: 0x0006BFB4 File Offset: 0x0006A1B4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C4A RID: 3146
			private StringTemplate st;
		}

		// Token: 0x020001FE RID: 510
		protected class function_call_scope
		{
			// Token: 0x04000C4B RID: 3147
			protected internal string sselfName;
		}

		// Token: 0x020001FF RID: 511
		public class function_call_return : TreeRuleReturnScope
		{
			// Token: 0x17000208 RID: 520
			// (get) Token: 0x06000E8A RID: 3722 RVA: 0x0006BFDC File Offset: 0x0006A1DC
			// (set) Token: 0x06000E8B RID: 3723 RVA: 0x0006BFE4 File Offset: 0x0006A1E4
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x17000209 RID: 521
			// (get) Token: 0x06000E8C RID: 3724 RVA: 0x0006BFF0 File Offset: 0x0006A1F0
			public override object Template => st;

            // Token: 0x06000E8D RID: 3725 RVA: 0x0006BFF8 File Offset: 0x0006A1F8
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C4C RID: 3148
			public string sRetValue;

			// Token: 0x04000C4D RID: 3149
			private StringTemplate st;
		}

		// Token: 0x02000200 RID: 512
		public class parameters_return : TreeRuleReturnScope
		{
			// Token: 0x1700020A RID: 522
			// (get) Token: 0x06000E8F RID: 3727 RVA: 0x0006C018 File Offset: 0x0006A218
			// (set) Token: 0x06000E90 RID: 3728 RVA: 0x0006C020 File Offset: 0x0006A220
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x1700020B RID: 523
			// (get) Token: 0x06000E91 RID: 3729 RVA: 0x0006C02C File Offset: 0x0006A22C
			public override object Template => st;

            // Token: 0x06000E92 RID: 3730 RVA: 0x0006C034 File Offset: 0x0006A234
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C4E RID: 3150
			public IList sParamVars;

			// Token: 0x04000C4F RID: 3151
			public IList kAutoCastST;

			// Token: 0x04000C50 RID: 3152
			private StringTemplate st;
		}

		// Token: 0x02000201 RID: 513
		public class parameter_return : TreeRuleReturnScope
		{
			// Token: 0x1700020C RID: 524
			// (get) Token: 0x06000E94 RID: 3732 RVA: 0x0006C054 File Offset: 0x0006A254
			// (set) Token: 0x06000E95 RID: 3733 RVA: 0x0006C05C File Offset: 0x0006A25C
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x1700020D RID: 525
			// (get) Token: 0x06000E96 RID: 3734 RVA: 0x0006C068 File Offset: 0x0006A268
			public override object Template => st;

            // Token: 0x06000E97 RID: 3735 RVA: 0x0006C070 File Offset: 0x0006A270
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C51 RID: 3153
			public string sVarName;

			// Token: 0x04000C52 RID: 3154
			public StringTemplate kAutoCastST;

			// Token: 0x04000C53 RID: 3155
			private StringTemplate st;
		}

		// Token: 0x02000202 RID: 514
		protected class autoCast_scope
		{
			// Token: 0x04000C54 RID: 3156
			protected internal string ssource;
		}

		// Token: 0x02000203 RID: 515
		public class autoCast_return : TreeRuleReturnScope
		{
			// Token: 0x1700020E RID: 526
			// (get) Token: 0x06000E9A RID: 3738 RVA: 0x0006C098 File Offset: 0x0006A298
			// (set) Token: 0x06000E9B RID: 3739 RVA: 0x0006C0A0 File Offset: 0x0006A2A0
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x1700020F RID: 527
			// (get) Token: 0x06000E9C RID: 3740 RVA: 0x0006C0AC File Offset: 0x0006A2AC
			public override object Template => st;

            // Token: 0x06000E9D RID: 3741 RVA: 0x0006C0B4 File Offset: 0x0006A2B4
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C55 RID: 3157
			public string sRetValue;

			// Token: 0x04000C56 RID: 3158
			private StringTemplate st;
		}

		// Token: 0x02000204 RID: 516
		public class constant_return : TreeRuleReturnScope
		{
			// Token: 0x17000210 RID: 528
			// (get) Token: 0x06000E9F RID: 3743 RVA: 0x0006C0D4 File Offset: 0x0006A2D4
			// (set) Token: 0x06000EA0 RID: 3744 RVA: 0x0006C0DC File Offset: 0x0006A2DC
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x17000211 RID: 529
			// (get) Token: 0x06000EA1 RID: 3745 RVA: 0x0006C0E8 File Offset: 0x0006A2E8
			public override object Template => st;

            // Token: 0x06000EA2 RID: 3746 RVA: 0x0006C0F0 File Offset: 0x0006A2F0
			public override string ToString()
			{
                return st?.ToString();
            }

			// Token: 0x04000C57 RID: 3159
			private StringTemplate st;
		}

		// Token: 0x02000205 RID: 517
		public class number_return : TreeRuleReturnScope
		{
			// Token: 0x17000212 RID: 530
			// (get) Token: 0x06000EA4 RID: 3748 RVA: 0x0006C110 File Offset: 0x0006A310
			// (set) Token: 0x06000EA5 RID: 3749 RVA: 0x0006C118 File Offset: 0x0006A318
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x17000213 RID: 531
			// (get) Token: 0x06000EA6 RID: 3750 RVA: 0x0006C124 File Offset: 0x0006A324
			public override object Template => st;

            // Token: 0x06000EA7 RID: 3751 RVA: 0x0006C12C File Offset: 0x0006A32C
			public override string ToString() => st?.ToString();

            // Token: 0x04000C58 RID: 3160
			private StringTemplate st;
		}

		// Token: 0x02000206 RID: 518
		public class type_return : TreeRuleReturnScope
		{
			// Token: 0x17000214 RID: 532
			// (get) Token: 0x06000EA9 RID: 3753 RVA: 0x0006C14C File Offset: 0x0006A34C
			// (set) Token: 0x06000EAA RID: 3754 RVA: 0x0006C154 File Offset: 0x0006A354
			public StringTemplate ST
			{
				get => st;
                set => st = value;
            }

			// Token: 0x17000215 RID: 533
			// (get) Token: 0x06000EAB RID: 3755 RVA: 0x0006C160 File Offset: 0x0006A360
			public override object Template => st;

            // Token: 0x06000EAC RID: 3756 RVA: 0x0006C168 File Offset: 0x0006A368
			public override string ToString() => st?.ToString();

            // Token: 0x04000C59 RID: 3161
			public string sTypeString;

			// Token: 0x04000C5A RID: 3162
			private StringTemplate st;
		}

		// Token: 0x02000207 RID: 519
		protected class DFA27 : DFA
		{
			// Token: 0x06000EAE RID: 3758 RVA: 0x0006C188 File Offset: 0x0006A388
			public DFA27(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 27;
				eot = DFA27_eot;
				eof = DFA27_eof;
				min = DFA27_min;
				max = DFA27_max;
				accept = DFA27_accept;
				special = DFA27_special;
				transition = DFA27_transition;
			}

			// Token: 0x17000216 RID: 534
			// (get) Token: 0x06000EAF RID: 3759 RVA: 0x0006C1F8 File Offset: 0x0006A3F8
			public override string Description => "437:1: l_value : ( ^( DOT ^( PAREXPR a= expression ) b= property_set ) -> dot(aTemplate=$a.stbTemplate=$b.st) | ^( ARRAYSET source= ID self= ID autoCast ^( PAREXPR array= expression ) index= expression ) -> arraySet(sourceName=$l_value::ssourceNameselfName=$l_value::sselfNameindex=$autoCast.sRetValueautoCast=$autoCast.starrayExpressions=$array.stindexExpressions=$index.stlineNo=$ARRAYSET.Line) | basic_l_value );";
        }
	}
}
