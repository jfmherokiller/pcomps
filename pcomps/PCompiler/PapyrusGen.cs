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
			this.InitializeCyclicDFAs();
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000D70 RID: 3440 RVA: 0x0005E194 File Offset: 0x0005C394
		// (set) Token: 0x06000D71 RID: 3441 RVA: 0x0005E19C File Offset: 0x0005C39C
		public StringTemplateGroup TemplateLib
		{
			get
			{
				return this.templateLib;
			}
			set
			{
				this.templateLib = value;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000D72 RID: 3442 RVA: 0x0005E1A8 File Offset: 0x0005C3A8
		public override string[] TokenNames
		{
			get
			{
				return PapyrusGen.tokenNames;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000D73 RID: 3443 RVA: 0x0005E1B0 File Offset: 0x0005C3B0
		public override string GrammarFileName
		{
			get
			{
				return "PapyrusGen.g";
			}
		}

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06000D74 RID: 3444 RVA: 0x0005E1B8 File Offset: 0x0005C3B8
		// (remove) Token: 0x06000D75 RID: 3445 RVA: 0x0005E1D4 File Offset: 0x0005C3D4
		internal event InternalErrorEventHandler ErrorHandler;

		// Token: 0x06000D76 RID: 3446 RVA: 0x0005E1F0 File Offset: 0x0005C3F0
		private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
		{
			if (this.ErrorHandler != null)
			{
				this.ErrorHandler(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
			}
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x0005E210 File Offset: 0x0005C410
		public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			string errorMessage = this.GetErrorMessage(e, tokenNames);
			this.OnError(errorMessage, e.Line, e.CharPositionInLine);
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x0005E23C File Offset: 0x0005C43C
		public PapyrusGen.script_return script(string asSourceFilename, ScriptObjectType akObj)
		{
			this.script_stack.Push(new PapyrusGen.script_scope());
			PapyrusGen.script_return script_return = new PapyrusGen.script_return();
			script_return.Start = this.input.LT(1);
			((PapyrusGen.script_scope)this.script_stack.Peek()).sobjName = "";
			((PapyrusGen.script_scope)this.script_stack.Peek()).sparentName = "";
			((PapyrusGen.script_scope)this.script_stack.Peek()).kobjVarDefinitions = new ArrayList();
			((PapyrusGen.script_scope)this.script_stack.Peek()).kobjPropDefinitions = new ArrayList();
			((PapyrusGen.script_scope)this.script_stack.Peek()).sinitialState = null;
			((PapyrusGen.script_scope)this.script_stack.Peek()).kobjEmptyState = new ArrayList();
			((PapyrusGen.script_scope)this.script_stack.Peek()).kstates = new Hashtable();
			((PapyrusGen.script_scope)this.script_stack.Peek()).bhasBeginStateEvent = false;
			((PapyrusGen.script_scope)this.script_stack.Peek()).bhasEndStateEvent = false;
			((PapyrusGen.script_scope)this.script_stack.Peek()).smodTimeUnix = "";
			((PapyrusGen.script_scope)this.script_stack.Peek()).scompileTimeUnix = "";
			((PapyrusGen.script_scope)this.script_stack.Peek()).suserName = "";
			((PapyrusGen.script_scope)this.script_stack.Peek()).scomputerName = "";
			((PapyrusGen.script_scope)this.script_stack.Peek()).sobjFlags = "";
			((PapyrusGen.script_scope)this.script_stack.Peek()).sdocString = "";
			this.kObjType = akObj;
			try
			{
				this.Match(this.input, 4, PapyrusGen.FOLLOW_OBJECT_in_script80);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_header_in_script82);
				this.header();
				this.state.followingStackPointer--;
				for (;;)
				{
					int num = 2;
					int num2 = this.input.LA(1);
					if ((num2 >= 5 && num2 <= 7) || num2 == 19 || num2 == 51 || num2 == 54)
					{
						num = 1;
					}
					int num3 = num;
					if (num3 != 1)
					{
						break;
					}
					base.PushFollow(PapyrusGen.FOLLOW_definitionOrBlock_in_script84);
					this.definitionOrBlock();
					this.state.followingStackPointer--;
				}
				this.Match(this.input, 3, null);
				if (((PapyrusGen.script_scope)this.script_stack.Peek()).sparentName == "")
				{
					if (!((PapyrusGen.script_scope)this.script_stack.Peek()).bhasBeginStateEvent)
					{
						((PapyrusGen.script_scope)this.script_stack.Peek()).kobjEmptyState.Add(this.templateLib.GetInstanceOf("emptyBeginStateEvent"));
					}
					if (!((PapyrusGen.script_scope)this.script_stack.Peek()).bhasEndStateEvent)
					{
						((PapyrusGen.script_scope)this.script_stack.Peek()).kobjEmptyState.Add(this.templateLib.GetInstanceOf("emptyEndStateEvent"));
					}
				}
				((PapyrusGen.script_scope)this.script_stack.Peek()).smodTimeUnix = this.GetFileModTimeUnix(asSourceFilename);
				((PapyrusGen.script_scope)this.script_stack.Peek()).scompileTimeUnix = this.GetCompileTimeUnix();
				((PapyrusGen.script_scope)this.script_stack.Peek()).suserName = Environment.UserName;
				((PapyrusGen.script_scope)this.script_stack.Peek()).scomputerName = Environment.MachineName;
				((PapyrusGen.script_scope)this.script_stack.Peek()).kuserFlagsRef = this.ConstructUserFlagRefInfo();
				script_return.ST = this.templateLib.GetInstanceOf("object", new PapyrusGen.STAttrMap().Add("objName", ((PapyrusGen.script_scope)this.script_stack.Peek()).sobjName).Add("parent", ((PapyrusGen.script_scope)this.script_stack.Peek()).sparentName).Add("variableDefs", ((PapyrusGen.script_scope)this.script_stack.Peek()).kobjVarDefinitions).Add("properties", ((PapyrusGen.script_scope)this.script_stack.Peek()).kobjPropDefinitions).Add("initialState", ((PapyrusGen.script_scope)this.script_stack.Peek()).sinitialState).Add("emptyStateFuncs", ((PapyrusGen.script_scope)this.script_stack.Peek()).kobjEmptyState).Add("stateFuncs", ((PapyrusGen.script_scope)this.script_stack.Peek()).kstates).Add("modTimeUnix", ((PapyrusGen.script_scope)this.script_stack.Peek()).smodTimeUnix).Add("compileTimeUnix", ((PapyrusGen.script_scope)this.script_stack.Peek()).scompileTimeUnix).Add("userName", ((PapyrusGen.script_scope)this.script_stack.Peek()).suserName).Add("computerName", ((PapyrusGen.script_scope)this.script_stack.Peek()).scomputerName).Add("userFlags", ((PapyrusGen.script_scope)this.script_stack.Peek()).sobjFlags).Add("userFlagsRef", ((PapyrusGen.script_scope)this.script_stack.Peek()).kuserFlagsRef).Add("docString", ((PapyrusGen.script_scope)this.script_stack.Peek()).sdocString));
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.script_stack.Pop();
			}
			return script_return;
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x0005E7F8 File Offset: 0x0005C9F8
		public PapyrusGen.header_return header()
		{
			PapyrusGen.header_return header_return = new PapyrusGen.header_return();
			header_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			CommonTree commonTree2 = null;
			try
			{
				CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_header224);
				this.Match(this.input, 2, null);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 18, PapyrusGen.FOLLOW_USER_FLAGS_in_header226);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 38)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_header230);
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
					commonTree2 = (CommonTree)this.Match(this.input, 40, PapyrusGen.FOLLOW_DOCSTRING_in_header233);
				}
				this.Match(this.input, 3, null);
				((PapyrusGen.script_scope)this.script_stack.Peek()).sobjName = commonTree3.Text;
				((PapyrusGen.script_scope)this.script_stack.Peek()).sobjFlags = commonTree4.Text;
				if (commonTree != null)
				{
					((PapyrusGen.script_scope)this.script_stack.Peek()).sparentName = commonTree.Text;
				}
				if (commonTree2 != null)
				{
					((PapyrusGen.script_scope)this.script_stack.Peek()).sdocString = commonTree2.Text;
				}
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return header_return;
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x0005E99C File Offset: 0x0005CB9C
		public PapyrusGen.definitionOrBlock_return definitionOrBlock()
		{
			PapyrusGen.definitionOrBlock_return definitionOrBlock_return = new PapyrusGen.definitionOrBlock_return();
			definitionOrBlock_return.Start = this.input.LT(1);
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
				NoViableAltException ex = new NoViableAltException("", 4, 0, this.input);
				throw ex;
				IL_93:
				switch (num2)
				{
				case 1:
				{
					base.PushFollow(PapyrusGen.FOLLOW_fieldDefinition_in_definitionOrBlock253);
					PapyrusGen.fieldDefinition_return fieldDefinition_return = this.fieldDefinition();
					this.state.followingStackPointer--;
					((PapyrusGen.script_scope)this.script_stack.Peek()).kobjVarDefinitions.Add((fieldDefinition_return != null) ? fieldDefinition_return.ST : null);
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_function_in_definitionOrBlock264);
					PapyrusGen.function_return function_return = this.function("", "");
					this.state.followingStackPointer--;
					if (((function_return != null) ? function_return.sName : null).ToLowerInvariant() == "onbeginstate")
					{
						((PapyrusGen.script_scope)this.script_stack.Peek()).bhasBeginStateEvent = true;
					}
					else if (((function_return != null) ? function_return.sName : null).ToLowerInvariant() == "onendstate")
					{
						((PapyrusGen.script_scope)this.script_stack.Peek()).bhasEndStateEvent = true;
					}
					((PapyrusGen.script_scope)this.script_stack.Peek()).kobjEmptyState.Add((function_return != null) ? function_return.ST : null);
					break;
				}
				case 3:
				{
					base.PushFollow(PapyrusGen.FOLLOW_eventFunc_in_definitionOrBlock277);
					PapyrusGen.eventFunc_return eventFunc_return = this.eventFunc("");
					this.state.followingStackPointer--;
					if (((eventFunc_return != null) ? eventFunc_return.sName : null).ToLowerInvariant() == "onbeginstate")
					{
						((PapyrusGen.script_scope)this.script_stack.Peek()).bhasBeginStateEvent = true;
					}
					else if (((eventFunc_return != null) ? eventFunc_return.sName : null).ToLowerInvariant() == "onendstate")
					{
						((PapyrusGen.script_scope)this.script_stack.Peek()).bhasEndStateEvent = true;
					}
					((PapyrusGen.script_scope)this.script_stack.Peek()).kobjEmptyState.Add((eventFunc_return != null) ? eventFunc_return.ST : null);
					break;
				}
				case 4:
					base.PushFollow(PapyrusGen.FOLLOW_stateBlock_in_definitionOrBlock289);
					this.stateBlock();
					this.state.followingStackPointer--;
					break;
				case 5:
				{
					base.PushFollow(PapyrusGen.FOLLOW_propertyBlock_in_definitionOrBlock295);
					PapyrusGen.propertyBlock_return propertyBlock_return = this.propertyBlock();
					this.state.followingStackPointer--;
					((PapyrusGen.script_scope)this.script_stack.Peek()).kobjPropDefinitions.Add((propertyBlock_return != null) ? propertyBlock_return.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return definitionOrBlock_return;
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x0005ECE0 File Offset: 0x0005CEE0
		public PapyrusGen.fieldDefinition_return fieldDefinition()
		{
			this.fieldDefinition_stack.Push(new PapyrusGen.fieldDefinition_scope());
			PapyrusGen.fieldDefinition_return fieldDefinition_return = new PapyrusGen.fieldDefinition_return();
			fieldDefinition_return.Start = this.input.LT(1);
			PapyrusGen.constant_return constant_return = null;
			((PapyrusGen.fieldDefinition_scope)this.fieldDefinition_stack.Peek()).sinitialValue = "None";
			try
			{
				this.Match(this.input, 5, PapyrusGen.FOLLOW_VAR_in_fieldDefinition323);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_type_in_fieldDefinition325);
				PapyrusGen.type_return type_return = this.type();
				this.state.followingStackPointer--;
				CommonTree commonTree = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_fieldDefinition329);
				CommonTree commonTree2 = (CommonTree)this.Match(this.input, 18, PapyrusGen.FOLLOW_USER_FLAGS_in_fieldDefinition331);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 81 || (num2 >= 90 && num2 <= 93))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					base.PushFollow(PapyrusGen.FOLLOW_constant_in_fieldDefinition333);
					constant_return = this.constant();
					this.state.followingStackPointer--;
				}
				this.Match(this.input, 3, null);
				if (((constant_return != null) ? ((CommonTree)constant_return.Start) : null) != null)
				{
					((PapyrusGen.fieldDefinition_scope)this.fieldDefinition_stack.Peek()).sinitialValue = ((constant_return != null) ? ((CommonTree)constant_return.Start) : null).Text;
				}
				fieldDefinition_return.ST = this.templateLib.GetInstanceOf("variableDef", new PapyrusGen.STAttrMap().Add("type", (type_return != null) ? type_return.sTypeString : null).Add("name", commonTree.Text).Add("userFlags", commonTree2.Text).Add("initialValue", ((PapyrusGen.fieldDefinition_scope)this.fieldDefinition_stack.Peek()).sinitialValue));
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.fieldDefinition_stack.Pop();
			}
			return fieldDefinition_return;
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x0005EF28 File Offset: 0x0005D128
		public PapyrusGen.function_return function(string asState, string asPropertyName)
		{
			this.function_stack.Push(new PapyrusGen.function_scope());
			PapyrusGen.function_return function_return = new PapyrusGen.function_return();
			function_return.Start = this.input.LT(1);
			((PapyrusGen.function_scope)this.function_stack.Peek()).sstate = asState;
			((PapyrusGen.function_scope)this.function_stack.Peek()).sfuncName = "";
			((PapyrusGen.function_scope)this.function_stack.Peek()).spropertyName = asPropertyName;
			((PapyrusGen.function_scope)this.function_stack.Peek()).sreturnType = "";
			((PapyrusGen.function_scope)this.function_stack.Peek()).bisNative = false;
			((PapyrusGen.function_scope)this.function_stack.Peek()).bisGlobal = false;
			((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncParams = new ArrayList();
			((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncVarDefinitions = new ArrayList();
			((PapyrusGen.function_scope)this.function_stack.Peek()).kstatements = new ArrayList();
			((PapyrusGen.function_scope)this.function_stack.Peek()).suserFlags = "0";
			((PapyrusGen.function_scope)this.function_stack.Peek()).sdocString = "";
			try
			{
				this.Match(this.input, 6, PapyrusGen.FOLLOW_FUNCTION_in_function408);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_functionHeader_in_function410);
				this.functionHeader();
				this.state.followingStackPointer--;
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 10)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					base.PushFollow(PapyrusGen.FOLLOW_codeBlock_in_function412);
					this.codeBlock(((PapyrusGen.function_scope)this.function_stack.Peek()).kstatements, ((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncVarDefinitions, ((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncType.FunctionScope);
					this.state.followingStackPointer--;
				}
				this.Match(this.input, 3, null);
				function_return.ST = this.templateLib.GetInstanceOf("functionDef", new PapyrusGen.STAttrMap().Add("funcName", ((PapyrusGen.function_scope)this.function_stack.Peek()).sfuncName).Add("returnType", ((PapyrusGen.function_scope)this.function_stack.Peek()).sreturnType).Add("isNative", ((PapyrusGen.function_scope)this.function_stack.Peek()).bisNative).Add("isGlobal", ((PapyrusGen.function_scope)this.function_stack.Peek()).bisGlobal).Add("funcParams", ((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncParams).Add("funcVars", ((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncVarDefinitions).Add("userFlags", ((PapyrusGen.function_scope)this.function_stack.Peek()).suserFlags).Add("body", ((PapyrusGen.function_scope)this.function_stack.Peek()).kstatements).Add("docString", ((PapyrusGen.function_scope)this.function_stack.Peek()).sdocString));
				function_return.sName = ((PapyrusGen.function_scope)this.function_stack.Peek()).sfuncName;
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

		// Token: 0x06000D7D RID: 3453 RVA: 0x0005F2FC File Offset: 0x0005D4FC
		public PapyrusGen.functionHeader_return functionHeader()
		{
			PapyrusGen.functionHeader_return functionHeader_return = new PapyrusGen.functionHeader_return();
			functionHeader_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			CommonTree commonTree2 = null;
			PapyrusGen.callParameters_return callParameters_return = null;
			PapyrusGen.type_return type_return = null;
			try
			{
				this.Match(this.input, 8, PapyrusGen.FOLLOW_HEADER_in_functionHeader504);
				this.Match(this.input, 2, null);
				int num = this.input.LA(1);
				int num2;
				if (num == 38 || num == 55)
				{
					num2 = 1;
				}
				else
				{
					if (num != 92)
					{
						NoViableAltException ex = new NoViableAltException("", 7, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
					base.PushFollow(PapyrusGen.FOLLOW_type_in_functionHeader507);
					type_return = this.type();
					this.state.followingStackPointer--;
					break;
				case 2:
					commonTree = (CommonTree)this.Match(this.input, 92, PapyrusGen.FOLLOW_NONE_in_functionHeader511);
					break;
				}
				CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_functionHeader516);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 18, PapyrusGen.FOLLOW_USER_FLAGS_in_functionHeader518);
				int num3 = 2;
				int num4 = this.input.LA(1);
				if (num4 == 9)
				{
					num3 = 1;
				}
				int num5 = num3;
				if (num5 == 1)
				{
					base.PushFollow(PapyrusGen.FOLLOW_callParameters_in_functionHeader520);
					callParameters_return = this.callParameters();
					this.state.followingStackPointer--;
				}
				for (;;)
				{
					int num6 = 2;
					int num7 = this.input.LA(1);
					if (num7 >= 46 && num7 <= 47)
					{
						num6 = 1;
					}
					int num8 = num6;
					if (num8 != 1)
					{
						break;
					}
					base.PushFollow(PapyrusGen.FOLLOW_functionModifier_in_functionHeader523);
					this.functionModifier();
					this.state.followingStackPointer--;
				}
				int num9 = 2;
				int num10 = this.input.LA(1);
				if (num10 == 40)
				{
					num9 = 1;
				}
				int num11 = num9;
				if (num11 == 1)
				{
					commonTree2 = (CommonTree)this.Match(this.input, 40, PapyrusGen.FOLLOW_DOCSTRING_in_functionHeader526);
				}
				this.Match(this.input, 3, null);
				((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncParams = ((callParameters_return != null) ? callParameters_return.kParams : null);
				((PapyrusGen.function_scope)this.function_stack.Peek()).sreturnType = ((((type_return != null) ? ((CommonTree)type_return.Start) : null) != null) ? ((type_return != null) ? type_return.sTypeString : null) : commonTree.Text);
				((PapyrusGen.function_scope)this.function_stack.Peek()).sfuncName = commonTree3.Text;
				((PapyrusGen.function_scope)this.function_stack.Peek()).suserFlags = commonTree4.Text;
				if (commonTree2 != null)
				{
					((PapyrusGen.function_scope)this.function_stack.Peek()).sdocString = commonTree2.Text;
				}
				if (((PapyrusGen.function_scope)this.function_stack.Peek()).spropertyName == "")
				{
					this.kObjType.TryGetFunction(((PapyrusGen.function_scope)this.function_stack.Peek()).sstate, ((PapyrusGen.function_scope)this.function_stack.Peek()).sfuncName, out ((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncType);
				}
				else
				{
					ScriptPropertyType scriptPropertyType;
					this.kObjType.TryGetProperty(((PapyrusGen.function_scope)this.function_stack.Peek()).spropertyName, out scriptPropertyType);
					string a = ((PapyrusGen.function_scope)this.function_stack.Peek()).sfuncName.ToLowerInvariant();
					if (a == "get")
					{
						((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncType = scriptPropertyType.kGetFunction;
					}
					else
					{
						((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncType = scriptPropertyType.kSetFunction;
					}
				}
				this.MangleFunctionVariables(((PapyrusGen.function_scope)this.function_stack.Peek()).kfuncType);
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return functionHeader_return;
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x0005F70C File Offset: 0x0005D90C
		public PapyrusGen.functionModifier_return functionModifier()
		{
			PapyrusGen.functionModifier_return functionModifier_return = new PapyrusGen.functionModifier_return();
			functionModifier_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
				int num2;
				if (num == 47)
				{
					num2 = 1;
				}
				else
				{
					if (num != 46)
					{
						NoViableAltException ex = new NoViableAltException("", 11, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
					this.Match(this.input, 47, PapyrusGen.FOLLOW_NATIVE_in_functionModifier545);
					((PapyrusGen.function_scope)this.function_stack.Peek()).bisNative = true;
					break;
				case 2:
					this.Match(this.input, 46, PapyrusGen.FOLLOW_GLOBAL_in_functionModifier553);
					((PapyrusGen.function_scope)this.function_stack.Peek()).bisGlobal = true;
					break;
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return functionModifier_return;
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x0005F804 File Offset: 0x0005DA04
		public PapyrusGen.eventFunc_return eventFunc(string asState)
		{
			this.eventFunc_stack.Push(new PapyrusGen.eventFunc_scope());
			PapyrusGen.eventFunc_return eventFunc_return = new PapyrusGen.eventFunc_return();
			eventFunc_return.Start = this.input.LT(1);
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sstate = asState;
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sfuncName = "";
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sreturnType = "";
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).bisNative = false;
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).bisGlobal = false;
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncParams = new ArrayList();
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncVarDefinitions = new ArrayList();
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kstatements = new ArrayList();
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).suserFlags = "0";
			((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sdocString = "";
			try
			{
				this.Match(this.input, 7, PapyrusGen.FOLLOW_EVENT_in_eventFunc588);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_eventHeader_in_eventFunc590);
				this.eventHeader();
				this.state.followingStackPointer--;
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 10)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					base.PushFollow(PapyrusGen.FOLLOW_codeBlock_in_eventFunc592);
					this.codeBlock(((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kstatements, ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncVarDefinitions, ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncType.FunctionScope);
					this.state.followingStackPointer--;
				}
				this.Match(this.input, 3, null);
				eventFunc_return.ST = this.templateLib.GetInstanceOf("functionDef", new PapyrusGen.STAttrMap().Add("funcName", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sfuncName).Add("returnType", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sreturnType).Add("isNative", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).bisNative).Add("isGlobal", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).bisGlobal).Add("funcParams", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncParams).Add("funcVars", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncVarDefinitions).Add("userFlags", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).suserFlags).Add("body", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kstatements).Add("docString", ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sdocString));
				eventFunc_return.sName = ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sfuncName;
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

		// Token: 0x06000D80 RID: 3456 RVA: 0x0005FBC4 File Offset: 0x0005DDC4
		public PapyrusGen.eventHeader_return eventHeader()
		{
			PapyrusGen.eventHeader_return eventHeader_return = new PapyrusGen.eventHeader_return();
			eventHeader_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			CommonTree commonTree2 = null;
			PapyrusGen.callParameters_return callParameters_return = null;
			try
			{
				this.Match(this.input, 8, PapyrusGen.FOLLOW_HEADER_in_eventHeader684);
				this.Match(this.input, 2, null);
				CommonTree commonTree3 = (CommonTree)this.Match(this.input, 92, PapyrusGen.FOLLOW_NONE_in_eventHeader686);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_eventHeader688);
				CommonTree commonTree5 = (CommonTree)this.Match(this.input, 18, PapyrusGen.FOLLOW_USER_FLAGS_in_eventHeader690);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 9)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					base.PushFollow(PapyrusGen.FOLLOW_callParameters_in_eventHeader692);
					callParameters_return = this.callParameters();
					this.state.followingStackPointer--;
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
					commonTree = (CommonTree)this.Match(this.input, 47, PapyrusGen.FOLLOW_NATIVE_in_eventHeader695);
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
					commonTree2 = (CommonTree)this.Match(this.input, 40, PapyrusGen.FOLLOW_DOCSTRING_in_eventHeader698);
				}
				this.Match(this.input, 3, null);
				((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncParams = ((callParameters_return != null) ? callParameters_return.kParams : null);
				((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sreturnType = commonTree3.Text;
				((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sfuncName = commonTree4.Text;
				if (commonTree != null)
				{
					((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).bisNative = true;
				}
				((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).suserFlags = commonTree5.Text;
				if (commonTree2 != null)
				{
					((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sdocString = commonTree2.Text;
				}
				this.kObjType.TryGetFunction(((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sstate, ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).sfuncName, out ((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncType);
				this.MangleFunctionVariables(((PapyrusGen.eventFunc_scope)this.eventFunc_stack.Peek()).kfuncType);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return eventHeader_return;
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x0005FE84 File Offset: 0x0005E084
		public PapyrusGen.callParameters_return callParameters()
		{
			PapyrusGen.callParameters_return callParameters_return = new PapyrusGen.callParameters_return();
			callParameters_return.Start = this.input.LT(1);
			IList list = null;
			try
			{
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
					base.PushFollow(PapyrusGen.FOLLOW_callParameter_in_callParameters725);
					PapyrusGen.callParameter_return callParameter_return = this.callParameter();
					this.state.followingStackPointer--;
					if (list == null)
					{
						list = new ArrayList();
					}
					list.Add(callParameter_return.Template);
					num++;
				}
				if (num < 1)
				{
					EarlyExitException ex = new EarlyExitException(16, this.input);
					throw ex;
				}
				callParameters_return.kParams = list;
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return callParameters_return;
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x0005FF64 File Offset: 0x0005E164
		public PapyrusGen.callParameter_return callParameter()
		{
			PapyrusGen.callParameter_return callParameter_return = new PapyrusGen.callParameter_return();
			callParameter_return.Start = this.input.LT(1);
			try
			{
				this.Match(this.input, 9, PapyrusGen.FOLLOW_PARAM_in_callParameter742);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_type_in_callParameter744);
				PapyrusGen.type_return type_return = this.type();
				this.state.followingStackPointer--;
				CommonTree commonTree = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_callParameter748);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 81 || (num2 >= 90 && num2 <= 93))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					base.PushFollow(PapyrusGen.FOLLOW_constant_in_callParameter750);
					this.constant();
					this.state.followingStackPointer--;
				}
				this.Match(this.input, 3, null);
				callParameter_return.ST = this.templateLib.GetInstanceOf("funcParam", new PapyrusGen.STAttrMap().Add("type", (type_return != null) ? type_return.sTypeString : null).Add("name", commonTree.Text));
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return callParameter_return;
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x000600C8 File Offset: 0x0005E2C8
		public PapyrusGen.stateBlock_return stateBlock()
		{
			PapyrusGen.stateBlock_return stateBlock_return = new PapyrusGen.stateBlock_return();
			stateBlock_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			IList list = null;
			try
			{
				this.Match(this.input, 51, PapyrusGen.FOLLOW_STATE_in_stateBlock787);
				this.Match(this.input, 2, null);
				CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_stateBlock789);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 50)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree = (CommonTree)this.Match(this.input, 50, PapyrusGen.FOLLOW_AUTO_in_stateBlock791);
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
					base.PushFollow(PapyrusGen.FOLLOW_stateFuncOrEvent_in_stateBlock797);
					PapyrusGen.stateFuncOrEvent_return stateFuncOrEvent_return = this.stateFuncOrEvent(commonTree2.Text);
					this.state.followingStackPointer--;
					if (list == null)
					{
						list = new ArrayList();
					}
					list.Add(stateFuncOrEvent_return.Template);
				}
				this.Match(this.input, 3, null);
				string key = commonTree2.Text.ToLowerInvariant();
				object obj = ((PapyrusGen.script_scope)this.script_stack.Peek()).kstates[key];
				string val = "";
				if (obj != null)
				{
					val = obj.ToString();
				}
				StringTemplate instanceOf = this.TemplateLib.GetInstanceOf("stateConcatinate");
				instanceOf.SetAttribute("prevText", val);
				instanceOf.SetAttribute("funcs", list);
				((PapyrusGen.script_scope)this.script_stack.Peek()).kstates[key] = instanceOf.ToString();
				if (commonTree != null)
				{
					((PapyrusGen.script_scope)this.script_stack.Peek()).sinitialState = commonTree2.Text;
				}
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return stateBlock_return;
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x000602D4 File Offset: 0x0005E4D4
		public PapyrusGen.stateFuncOrEvent_return stateFuncOrEvent(string asStateName)
		{
			PapyrusGen.stateFuncOrEvent_return stateFuncOrEvent_return = new PapyrusGen.stateFuncOrEvent_return();
			stateFuncOrEvent_return.Start = this.input.LT(1);
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
						NoViableAltException ex = new NoViableAltException("", 20, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					base.PushFollow(PapyrusGen.FOLLOW_function_in_stateFuncOrEvent819);
					PapyrusGen.function_return function_return = this.function(asStateName, "");
					this.state.followingStackPointer--;
					stateFuncOrEvent_return.ST = ((function_return != null) ? function_return.ST : null);
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_eventFunc_in_stateFuncOrEvent832);
					PapyrusGen.eventFunc_return eventFunc_return = this.eventFunc(asStateName);
					this.state.followingStackPointer--;
					stateFuncOrEvent_return.ST = ((eventFunc_return != null) ? eventFunc_return.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return stateFuncOrEvent_return;
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x000603F4 File Offset: 0x0005E5F4
		public PapyrusGen.propertyBlock_return propertyBlock()
		{
			this.propertyBlock_stack.Push(new PapyrusGen.propertyBlock_scope());
			PapyrusGen.propertyBlock_return propertyBlock_return = new PapyrusGen.propertyBlock_return();
			propertyBlock_return.Start = this.input.LT(1);
			((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropName = "";
			((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropType = "";
			((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).suserFlags = "0";
			((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).sdocString = "";
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
						NoViableAltException ex = new NoViableAltException("", 21, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					this.Match(this.input, 54, PapyrusGen.FOLLOW_PROPERTY_in_propertyBlock861);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_propertyHeader_in_propertyBlock863);
					this.propertyHeader();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_propertyFunc_in_propertyBlock867);
					PapyrusGen.propertyFunc_return propertyFunc_return = this.propertyFunc(((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropName);
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_propertyFunc_in_propertyBlock872);
					PapyrusGen.propertyFunc_return propertyFunc_return2 = this.propertyFunc(((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropName);
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					propertyBlock_return.ST = this.templateLib.GetInstanceOf("fullProp", new PapyrusGen.STAttrMap().Add("name", ((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropName).Add("type", ((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropType).Add("get", (propertyFunc_return != null) ? propertyFunc_return.ST : null).Add("set", (propertyFunc_return2 != null) ? propertyFunc_return2.ST : null).Add("userFlags", ((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).suserFlags).Add("docString", ((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).sdocString));
					break;
				}
				case 2:
				{
					this.Match(this.input, 19, PapyrusGen.FOLLOW_AUTOPROP_in_propertyBlock922);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_propertyHeader_in_propertyBlock924);
					this.propertyHeader();
					this.state.followingStackPointer--;
					CommonTree commonTree = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_propertyBlock928);
					this.Match(this.input, 3, null);
					propertyBlock_return.ST = this.templateLib.GetInstanceOf("autoProp", new PapyrusGen.STAttrMap().Add("name", ((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropName).Add("type", ((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropType).Add("var", commonTree.Text).Add("userFlags", ((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).suserFlags).Add("docString", ((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).sdocString));
					break;
				}
				}
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

		// Token: 0x06000D86 RID: 3462 RVA: 0x000607FC File Offset: 0x0005E9FC
		public PapyrusGen.propertyHeader_return propertyHeader()
		{
			PapyrusGen.propertyHeader_return propertyHeader_return = new PapyrusGen.propertyHeader_return();
			propertyHeader_return.Start = this.input.LT(1);
			CommonTree commonTree = null;
			try
			{
				this.Match(this.input, 8, PapyrusGen.FOLLOW_HEADER_in_propertyHeader978);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_type_in_propertyHeader980);
				PapyrusGen.type_return type_return = this.type();
				this.state.followingStackPointer--;
				CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_propertyHeader984);
				CommonTree commonTree3 = (CommonTree)this.Match(this.input, 18, PapyrusGen.FOLLOW_USER_FLAGS_in_propertyHeader986);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 40)
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					commonTree = (CommonTree)this.Match(this.input, 40, PapyrusGen.FOLLOW_DOCSTRING_in_propertyHeader988);
				}
				this.Match(this.input, 3, null);
				((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropName = commonTree2.Text;
				((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).spropType = ((type_return != null) ? type_return.sTypeString : null);
				((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).suserFlags = commonTree3.Text;
				if (commonTree != null)
				{
					((PapyrusGen.propertyBlock_scope)this.propertyBlock_stack.Peek()).sdocString = commonTree.Text;
				}
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return propertyHeader_return;
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x000609A0 File Offset: 0x0005EBA0
		public PapyrusGen.propertyFunc_return propertyFunc(string asPropName)
		{
			PapyrusGen.propertyFunc_return propertyFunc_return = new PapyrusGen.propertyFunc_return();
			propertyFunc_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
				if (num != 17)
				{
					NoViableAltException ex = new NoViableAltException("", 23, 0, this.input);
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
					if (num2 != 3 && num2 != 17)
					{
						NoViableAltException ex2 = new NoViableAltException("", 23, 1, this.input);
						throw ex2;
					}
					num3 = 2;
				}
				switch (num3)
				{
				case 1:
				{
					this.Match(this.input, 17, PapyrusGen.FOLLOW_PROPFUNC_in_propertyFunc1009);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_function_in_propertyFunc1011);
					PapyrusGen.function_return function_return = this.function("", asPropName);
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					propertyFunc_return.ST = ((function_return != null) ? function_return.ST : null);
					break;
				}
				case 2:
					this.Match(this.input, 17, PapyrusGen.FOLLOW_PROPFUNC_in_propertyFunc1025);
					break;
				}
			}
			catch (RecognitionException ex3)
			{
				this.ReportError(ex3);
				this.Recover(this.input, ex3);
			}
			return propertyFunc_return;
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x00060B08 File Offset: 0x0005ED08
		public PapyrusGen.codeBlock_return codeBlock(IList akStatements, IList akVarDefinitions, ScriptScope akCurrentScope)
		{
			this.codeBlock_stack.Push(new PapyrusGen.codeBlock_scope());
			PapyrusGen.codeBlock_return codeBlock_return = new PapyrusGen.codeBlock_return();
			codeBlock_return.Start = this.input.LT(1);
			((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kvarDefs = akVarDefinitions;
			((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope = akCurrentScope;
			((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild = 0;
			try
			{
				this.Match(this.input, 10, PapyrusGen.FOLLOW_BLOCK_in_codeBlock1049);
				if (this.input.LA(1) == 2)
				{
					this.Match(this.input, 2, null);
					for (;;)
					{
						int num = 2;
						int num2 = this.input.LA(1);
						if (num2 == 5 || (num2 >= 11 && num2 <= 13) || (num2 == 15 || num2 == 20 || num2 == 22 || (num2 >= 24 && num2 <= 36)) || (num2 == 38 || num2 == 41 || num2 == 62 || (num2 >= 65 && num2 <= 72)) || (num2 >= 77 && num2 <= 84) || num2 == 88 || (num2 >= 90 && num2 <= 93))
						{
							num = 1;
						}
						int num3 = num;
						if (num3 != 1)
						{
							break;
						}
						base.PushFollow(PapyrusGen.FOLLOW_statement_in_codeBlock1057);
						PapyrusGen.statement_return statement_return = this.statement();
						this.state.followingStackPointer--;
						if (((statement_return != null) ? statement_return.ST : null) != null)
						{
							akStatements.Add((statement_return != null) ? statement_return.ST : null);
						}
					}
					this.Match(this.input, 3, null);
				}
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

		// Token: 0x06000D89 RID: 3465 RVA: 0x00060CE8 File Offset: 0x0005EEE8
		public PapyrusGen.statement_return statement()
		{
			this.statement_stack.Push(new PapyrusGen.statement_scope());
			PapyrusGen.statement_return statement_return = new PapyrusGen.statement_return();
			statement_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 25, 0, this.input);
				throw ex;
				IL_1B3:
				switch (num2)
				{
				case 1:
				{
					base.PushFollow(PapyrusGen.FOLLOW_localDefinition_in_statement1086);
					PapyrusGen.localDefinition_return localDefinition_return = this.localDefinition();
					this.state.followingStackPointer--;
					((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kvarDefs.Add((localDefinition_return != null) ? localDefinition_return.ST : null);
					if (((localDefinition_return != null) ? localDefinition_return.sExprVar : null) != "")
					{
						statement_return.ST = this.templateLib.GetInstanceOf("singleOpCommand", new PapyrusGen.STAttrMap().Add("command", "ASSIGN").Add("target", (localDefinition_return != null) ? localDefinition_return.sVarName : null).Add("source", (localDefinition_return != null) ? localDefinition_return.sExprVar : null).Add("autoCast", (localDefinition_return != null) ? localDefinition_return.kAutoCastST : null).Add("extraExpressions", (localDefinition_return != null) ? localDefinition_return.kExprST : null).Add("lineNo", (localDefinition_return != null) ? localDefinition_return.iLineNo : 0));
					}
					else
					{
						statement_return.ST = null;
					}
					break;
				}
				case 2:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 41, PapyrusGen.FOLLOW_EQUALS_in_statement1147);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_statement1149);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_statement1151);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_l_value_in_statement1153);
					PapyrusGen.l_value_return l_value_return = this.l_value();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_statement1155);
					PapyrusGen.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					((PapyrusGen.statement_scope)this.statement_stack.Peek()).smangledName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					statement_return.ST = this.templateLib.GetInstanceOf("assign", new PapyrusGen.STAttrMap().Add("target", ((PapyrusGen.statement_scope)this.statement_stack.Peek()).smangledName).Add("targetExpressions", (l_value_return != null) ? l_value_return.ST : null).Add("source", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("sourceExpressions", (expression_return != null) ? expression_return.ST : null).Add("autoCast", (autoCast_return != null) ? autoCast_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 3:
				{
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_statement1204);
					PapyrusGen.expression_return expression_return2 = this.expression();
					this.state.followingStackPointer--;
					statement_return.ST = ((expression_return2 != null) ? expression_return2.ST : null);
					break;
				}
				case 4:
				{
					base.PushFollow(PapyrusGen.FOLLOW_return_stat_in_statement1215);
					PapyrusGen.return_stat_return return_stat_return = this.return_stat();
					this.state.followingStackPointer--;
					statement_return.ST = ((return_stat_return != null) ? return_stat_return.ST : null);
					break;
				}
				case 5:
				{
					base.PushFollow(PapyrusGen.FOLLOW_ifBlock_in_statement1226);
					PapyrusGen.ifBlock_return ifBlock_return = this.ifBlock();
					this.state.followingStackPointer--;
					statement_return.ST = ((ifBlock_return != null) ? ifBlock_return.ST : null);
					break;
				}
				case 6:
				{
					base.PushFollow(PapyrusGen.FOLLOW_whileBlock_in_statement1237);
					PapyrusGen.whileBlock_return whileBlock_return = this.whileBlock();
					this.state.followingStackPointer--;
					statement_return.ST = ((whileBlock_return != null) ? whileBlock_return.ST : null);
					break;
				}
				}
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

		// Token: 0x06000D8A RID: 3466 RVA: 0x000612DC File Offset: 0x0005F4DC
		public PapyrusGen.localDefinition_return localDefinition()
		{
			PapyrusGen.localDefinition_return localDefinition_return = new PapyrusGen.localDefinition_return();
			localDefinition_return.Start = this.input.LT(1);
			PapyrusGen.expression_return expression_return = null;
			PapyrusGen.autoCast_return autoCast_return = null;
			try
			{
				this.Match(this.input, 5, PapyrusGen.FOLLOW_VAR_in_localDefinition1260);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_type_in_localDefinition1262);
				PapyrusGen.type_return type_return = this.type();
				this.state.followingStackPointer--;
				CommonTree commonTree = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_localDefinition1266);
				int num = 2;
				int num2 = this.input.LA(1);
				if (num2 == 38 || num2 == 79 || num2 == 81 || (num2 >= 90 && num2 <= 93))
				{
					num = 1;
				}
				int num3 = num;
				if (num3 == 1)
				{
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_localDefinition1269);
					autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_localDefinition1271);
					expression_return = this.expression();
					this.state.followingStackPointer--;
				}
				this.Match(this.input, 3, null);
				localDefinition_return.sVarName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree.Text);
				if (((expression_return != null) ? ((CommonTree)expression_return.Start) : null) != null)
				{
					localDefinition_return.sExprVar = ((autoCast_return != null) ? autoCast_return.sRetValue : null);
					localDefinition_return.kAutoCastST = ((autoCast_return != null) ? autoCast_return.ST : null);
					localDefinition_return.kExprST = ((expression_return != null) ? expression_return.ST : null);
					localDefinition_return.iLineNo = commonTree.Line;
				}
				else
				{
					localDefinition_return.sExprVar = "";
					localDefinition_return.kAutoCastST = null;
					localDefinition_return.kExprST = null;
					localDefinition_return.iLineNo = commonTree.Line;
				}
				localDefinition_return.ST = this.templateLib.GetInstanceOf("localDef", new PapyrusGen.STAttrMap().Add("type", (type_return != null) ? type_return.sTypeString : null).Add("name", localDefinition_return.sVarName));
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return localDefinition_return;
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x00061520 File Offset: 0x0005F720
		public PapyrusGen.l_value_return l_value()
		{
			this.l_value_stack.Push(new PapyrusGen.l_value_scope());
			PapyrusGen.l_value_return l_value_return = new PapyrusGen.l_value_return();
			l_value_return.Start = this.input.LT(1);
			try
			{
				switch (this.dfa27.Predict(this.input))
				{
				case 1:
				{
					this.Match(this.input, 62, PapyrusGen.FOLLOW_DOT_in_l_value1318);
					this.Match(this.input, 2, null);
					this.Match(this.input, 15, PapyrusGen.FOLLOW_PAREXPR_in_l_value1321);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_l_value1325);
					PapyrusGen.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					base.PushFollow(PapyrusGen.FOLLOW_property_set_in_l_value1330);
					PapyrusGen.property_set_return property_set_return = this.property_set();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					l_value_return.ST = this.templateLib.GetInstanceOf("dot", new PapyrusGen.STAttrMap().Add("aTemplate", (expression_return != null) ? expression_return.ST : null).Add("bTemplate", (property_set_return != null) ? property_set_return.ST : null));
					break;
				}
				case 2:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 23, PapyrusGen.FOLLOW_ARRAYSET_in_l_value1355);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_l_value1359);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_l_value1363);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_l_value1365);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					this.Match(this.input, 15, PapyrusGen.FOLLOW_PAREXPR_in_l_value1368);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_l_value1372);
					PapyrusGen.expression_return expression_return2 = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_l_value1377);
					PapyrusGen.expression_return expression_return3 = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					((PapyrusGen.l_value_scope)this.l_value_stack.Peek()).ssourceName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					((PapyrusGen.l_value_scope)this.l_value_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					l_value_return.ST = this.templateLib.GetInstanceOf("arraySet", new PapyrusGen.STAttrMap().Add("sourceName", ((PapyrusGen.l_value_scope)this.l_value_stack.Peek()).ssourceName).Add("selfName", ((PapyrusGen.l_value_scope)this.l_value_stack.Peek()).sselfName).Add("index", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("autoCast", (autoCast_return != null) ? autoCast_return.ST : null).Add("arrayExpressions", (expression_return2 != null) ? expression_return2.ST : null).Add("indexExpressions", (expression_return3 != null) ? expression_return3.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 3:
				{
					base.PushFollow(PapyrusGen.FOLLOW_basic_l_value_in_l_value1431);
					PapyrusGen.basic_l_value_return basic_l_value_return = this.basic_l_value();
					this.state.followingStackPointer--;
					l_value_return.ST = ((basic_l_value_return != null) ? basic_l_value_return.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.l_value_stack.Pop();
			}
			return l_value_return;
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x00061980 File Offset: 0x0005FB80
		public PapyrusGen.basic_l_value_return basic_l_value()
		{
			this.basic_l_value_stack.Push(new PapyrusGen.basic_l_value_scope());
			PapyrusGen.basic_l_value_return basic_l_value_return = new PapyrusGen.basic_l_value_return();
			basic_l_value_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 28, 0, this.input);
				throw ex;
				IL_CD:
				switch (num2)
				{
				case 1:
				{
					this.Match(this.input, 62, PapyrusGen.FOLLOW_DOT_in_basic_l_value1454);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_array_func_or_id_in_basic_l_value1458);
					PapyrusGen.array_func_or_id_return array_func_or_id_return = this.array_func_or_id();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_basic_l_value_in_basic_l_value1462);
					PapyrusGen.basic_l_value_return basic_l_value_return2 = this.basic_l_value();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					basic_l_value_return.ST = this.templateLib.GetInstanceOf("dot", new PapyrusGen.STAttrMap().Add("aTemplate", (array_func_or_id_return != null) ? array_func_or_id_return.ST : null).Add("bTemplate", (basic_l_value_return2 != null) ? basic_l_value_return2.ST : null));
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_function_call_in_basic_l_value1486);
					PapyrusGen.function_call_return function_call_return = this.function_call();
					this.state.followingStackPointer--;
					basic_l_value_return.ST = ((function_call_return != null) ? function_call_return.ST : null);
					break;
				}
				case 3:
				{
					base.PushFollow(PapyrusGen.FOLLOW_property_set_in_basic_l_value1497);
					PapyrusGen.property_set_return property_set_return = this.property_set();
					this.state.followingStackPointer--;
					basic_l_value_return.ST = ((property_set_return != null) ? property_set_return.ST : null);
					break;
				}
				case 4:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 23, PapyrusGen.FOLLOW_ARRAYSET_in_basic_l_value1509);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_basic_l_value1513);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_basic_l_value1517);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_basic_l_value1519);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_func_or_id_in_basic_l_value1521);
					PapyrusGen.func_or_id_return func_or_id_return = this.func_or_id();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_basic_l_value1523);
					PapyrusGen.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					((PapyrusGen.basic_l_value_scope)this.basic_l_value_stack.Peek()).ssourceName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					((PapyrusGen.basic_l_value_scope)this.basic_l_value_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					basic_l_value_return.ST = this.templateLib.GetInstanceOf("arraySet", new PapyrusGen.STAttrMap().Add("sourceName", ((PapyrusGen.basic_l_value_scope)this.basic_l_value_stack.Peek()).ssourceName).Add("selfName", ((PapyrusGen.basic_l_value_scope)this.basic_l_value_stack.Peek()).sselfName).Add("index", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("autoCast", (autoCast_return != null) ? autoCast_return.ST : null).Add("arrayExpressions", (func_or_id_return != null) ? func_or_id_return.ST : null).Add("indexExpressions", (expression_return != null) ? expression_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 5:
					this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_basic_l_value1577);
					break;
				}
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

		// Token: 0x06000D8D RID: 3469 RVA: 0x00061E50 File Offset: 0x00060050
		public PapyrusGen.expression_return expression()
		{
			PapyrusGen.expression_return expression_return = new PapyrusGen.expression_return();
			expression_return.Start = this.input.LT(1);
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
					if ((num < 11 || num > 13) && (num != 15 && num != 20 && num != 22 && (num < 24 || num > 36)) && (num != 38 && num != 62 && (num < 66 || num > 72)) && (num < 77 || num > 82) && (num < 90 || num > 93))
					{
						NoViableAltException ex = new NoViableAltException("", 29, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 65, PapyrusGen.FOLLOW_OR_in_expression1595);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_expression1597);
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_expression1601);
					PapyrusGen.expression_return expression_return2 = this.expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_and_expression_in_expression1605);
					PapyrusGen.and_expression_return and_expression_return = this.and_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					expression_return.ST = this.templateLib.GetInstanceOf("orExpression", new PapyrusGen.STAttrMap().Add("target", expression_return.sRetValue).Add("arg1", (expression_return2 != null) ? expression_return2.sRetValue : null).Add("arg2", (and_expression_return != null) ? and_expression_return.sRetValue : null).Add("extraExpressions1", (expression_return2 != null) ? expression_return2.ST : null).Add("extraExpressions2", (and_expression_return != null) ? and_expression_return.ST : null).Add("endLabel", this.GenerateLabel()).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_and_expression_in_expression1659);
					PapyrusGen.and_expression_return and_expression_return2 = this.and_expression();
					this.state.followingStackPointer--;
					expression_return.sRetValue = ((and_expression_return2 != null) ? and_expression_return2.sRetValue : null);
					expression_return.ST = ((and_expression_return2 != null) ? and_expression_return2.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return expression_return;
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x00062120 File Offset: 0x00060320
		public PapyrusGen.and_expression_return and_expression()
		{
			PapyrusGen.and_expression_return and_expression_return = new PapyrusGen.and_expression_return();
			and_expression_return.Start = this.input.LT(1);
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
					if ((num < 11 || num > 13) && (num != 15 && num != 20 && num != 22 && (num < 24 || num > 36)) && (num != 38 && num != 62 && (num < 67 || num > 72)) && (num < 77 || num > 82) && (num < 90 || num > 93))
					{
						NoViableAltException ex = new NoViableAltException("", 30, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 66, PapyrusGen.FOLLOW_AND_in_and_expression1681);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_and_expression1683);
					base.PushFollow(PapyrusGen.FOLLOW_and_expression_in_and_expression1687);
					PapyrusGen.and_expression_return and_expression_return2 = this.and_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_bool_expression_in_and_expression1691);
					PapyrusGen.bool_expression_return bool_expression_return = this.bool_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					and_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					and_expression_return.ST = this.templateLib.GetInstanceOf("andExpression", new PapyrusGen.STAttrMap().Add("target", and_expression_return.sRetValue).Add("arg1", (and_expression_return2 != null) ? and_expression_return2.sRetValue : null).Add("arg2", (bool_expression_return != null) ? bool_expression_return.sRetValue : null).Add("extraExpressions1", (and_expression_return2 != null) ? and_expression_return2.ST : null).Add("extraExpressions2", (bool_expression_return != null) ? bool_expression_return.ST : null).Add("endLabel", this.GenerateLabel()).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_bool_expression_in_and_expression1745);
					PapyrusGen.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					and_expression_return.sRetValue = ((bool_expression_return2 != null) ? bool_expression_return2.sRetValue : null);
					and_expression_return.ST = ((bool_expression_return2 != null) ? bool_expression_return2.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return and_expression_return;
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x000623F0 File Offset: 0x000605F0
		public PapyrusGen.bool_expression_return bool_expression()
		{
			PapyrusGen.bool_expression_return bool_expression_return = new PapyrusGen.bool_expression_return();
			bool_expression_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 31, 0, this.input);
				throw ex;
				IL_19A:
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 67, PapyrusGen.FOLLOW_EQ_in_bool_expression1767);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_bool_expression1769);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression1773);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression1777);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_bool_expression_in_bool_expression1781);
					PapyrusGen.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_bool_expression1785);
					PapyrusGen.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					bool_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					bool_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "COMPAREEQ").Add("target", bool_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (bool_expression_return2 != null) ? bool_expression_return2.ST : null).Add("extraExpressions2", (add_expression_return != null) ? add_expression_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 68, PapyrusGen.FOLLOW_NE_in_bool_expression1850);
					this.Match(this.input, 2, null);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_bool_expression1852);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression1856);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression1860);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_bool_expression_in_bool_expression1864);
					PapyrusGen.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_bool_expression1868);
					PapyrusGen.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					bool_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					bool_expression_return.ST = this.templateLib.GetInstanceOf("notEqual", new PapyrusGen.STAttrMap().Add("target", bool_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (bool_expression_return2 != null) ? bool_expression_return2.ST : null).Add("extraExpressions2", (add_expression_return != null) ? add_expression_return.ST : null).Add("lineNo", commonTree3.Line));
					break;
				}
				case 3:
				{
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 69, PapyrusGen.FOLLOW_GT_in_bool_expression1928);
					this.Match(this.input, 2, null);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_bool_expression1930);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression1934);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression1938);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_bool_expression_in_bool_expression1942);
					PapyrusGen.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_bool_expression1946);
					PapyrusGen.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					bool_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					bool_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "COMPAREGT").Add("target", bool_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (bool_expression_return2 != null) ? bool_expression_return2.ST : null).Add("extraExpressions2", (add_expression_return != null) ? add_expression_return.ST : null).Add("lineNo", commonTree5.Line));
					break;
				}
				case 4:
				{
					CommonTree commonTree7 = (CommonTree)this.Match(this.input, 70, PapyrusGen.FOLLOW_LT_in_bool_expression2011);
					this.Match(this.input, 2, null);
					CommonTree commonTree8 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_bool_expression2013);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression2017);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression2021);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_bool_expression_in_bool_expression2025);
					PapyrusGen.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_bool_expression2029);
					PapyrusGen.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					bool_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree8.Text);
					bool_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "COMPARELT").Add("target", bool_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (bool_expression_return2 != null) ? bool_expression_return2.ST : null).Add("extraExpressions2", (add_expression_return != null) ? add_expression_return.ST : null).Add("lineNo", commonTree7.Line));
					break;
				}
				case 5:
				{
					CommonTree commonTree9 = (CommonTree)this.Match(this.input, 71, PapyrusGen.FOLLOW_GTE_in_bool_expression2094);
					this.Match(this.input, 2, null);
					CommonTree commonTree10 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_bool_expression2096);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression2100);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression2104);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_bool_expression_in_bool_expression2108);
					PapyrusGen.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_bool_expression2112);
					PapyrusGen.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					bool_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree10.Text);
					bool_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "COMPAREGTE").Add("target", bool_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (bool_expression_return2 != null) ? bool_expression_return2.ST : null).Add("extraExpressions2", (add_expression_return != null) ? add_expression_return.ST : null).Add("lineNo", commonTree9.Line));
					break;
				}
				case 6:
				{
					CommonTree commonTree11 = (CommonTree)this.Match(this.input, 72, PapyrusGen.FOLLOW_LTE_in_bool_expression2177);
					this.Match(this.input, 2, null);
					CommonTree commonTree12 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_bool_expression2179);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression2183);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_bool_expression2187);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_bool_expression_in_bool_expression2191);
					PapyrusGen.bool_expression_return bool_expression_return2 = this.bool_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_bool_expression2195);
					PapyrusGen.add_expression_return add_expression_return = this.add_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					bool_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree12.Text);
					bool_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "COMPARELTE").Add("target", bool_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (bool_expression_return2 != null) ? bool_expression_return2.ST : null).Add("extraExpressions2", (add_expression_return != null) ? add_expression_return.ST : null).Add("lineNo", commonTree11.Line));
					break;
				}
				case 7:
				{
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_bool_expression2259);
					PapyrusGen.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					bool_expression_return.sRetValue = ((add_expression_return2 != null) ? add_expression_return2.sRetValue : null);
					bool_expression_return.ST = ((add_expression_return2 != null) ? add_expression_return2.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return bool_expression_return;
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x000631DC File Offset: 0x000613DC
		public PapyrusGen.add_expression_return add_expression()
		{
			PapyrusGen.add_expression_return add_expression_return = new PapyrusGen.add_expression_return();
			add_expression_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 32, 0, this.input);
				throw ex;
				IL_159:
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 26, PapyrusGen.FOLLOW_IADD_in_add_expression2281);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_add_expression2283);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2287);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2291);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_add_expression2295);
					PapyrusGen.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_add_expression2299);
					PapyrusGen.mult_expression_return mult_expression_return = this.mult_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					add_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					add_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "IADD").Add("target", add_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (add_expression_return2 != null) ? add_expression_return2.ST : null).Add("extraExpressions2", (mult_expression_return != null) ? mult_expression_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 27, PapyrusGen.FOLLOW_FADD_in_add_expression2364);
					this.Match(this.input, 2, null);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_add_expression2366);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2370);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2374);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_add_expression2378);
					PapyrusGen.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_add_expression2382);
					PapyrusGen.mult_expression_return mult_expression_return = this.mult_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					add_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					add_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "FADD").Add("target", add_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (add_expression_return2 != null) ? add_expression_return2.ST : null).Add("extraExpressions2", (mult_expression_return != null) ? mult_expression_return.ST : null).Add("lineNo", commonTree3.Line));
					break;
				}
				case 3:
				{
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 28, PapyrusGen.FOLLOW_ISUBTRACT_in_add_expression2447);
					this.Match(this.input, 2, null);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_add_expression2449);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2453);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2457);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_add_expression2461);
					PapyrusGen.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_add_expression2465);
					PapyrusGen.mult_expression_return mult_expression_return = this.mult_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					add_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					add_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "ISUBTRACT").Add("target", add_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (add_expression_return2 != null) ? add_expression_return2.ST : null).Add("extraExpressions2", (mult_expression_return != null) ? mult_expression_return.ST : null).Add("lineNo", commonTree5.Line));
					break;
				}
				case 4:
				{
					CommonTree commonTree7 = (CommonTree)this.Match(this.input, 29, PapyrusGen.FOLLOW_FSUBTRACT_in_add_expression2530);
					this.Match(this.input, 2, null);
					CommonTree commonTree8 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_add_expression2532);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2536);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2540);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_add_expression2544);
					PapyrusGen.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_add_expression2548);
					PapyrusGen.mult_expression_return mult_expression_return = this.mult_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					add_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree8.Text);
					add_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "FSUBTRACT").Add("target", add_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (add_expression_return2 != null) ? add_expression_return2.ST : null).Add("extraExpressions2", (mult_expression_return != null) ? mult_expression_return.ST : null).Add("lineNo", commonTree7.Line));
					break;
				}
				case 5:
				{
					CommonTree commonTree9 = (CommonTree)this.Match(this.input, 36, PapyrusGen.FOLLOW_STRCAT_in_add_expression2613);
					this.Match(this.input, 2, null);
					CommonTree commonTree10 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_add_expression2615);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2619);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_add_expression2623);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_add_expression_in_add_expression2627);
					PapyrusGen.add_expression_return add_expression_return2 = this.add_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_add_expression2631);
					PapyrusGen.mult_expression_return mult_expression_return = this.mult_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					add_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree10.Text);
					add_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "STRCAT").Add("target", add_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (add_expression_return2 != null) ? add_expression_return2.ST : null).Add("extraExpressions2", (mult_expression_return != null) ? mult_expression_return.ST : null).Add("lineNo", commonTree9.Line));
					break;
				}
				case 6:
				{
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_add_expression2695);
					PapyrusGen.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					add_expression_return.sRetValue = ((mult_expression_return2 != null) ? mult_expression_return2.sRetValue : null);
					add_expression_return.ST = ((mult_expression_return2 != null) ? mult_expression_return2.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return add_expression_return;
		}

		// Token: 0x06000D91 RID: 3473 RVA: 0x00063DA0 File Offset: 0x00061FA0
		public PapyrusGen.mult_expression_return mult_expression()
		{
			PapyrusGen.mult_expression_return mult_expression_return = new PapyrusGen.mult_expression_return();
			mult_expression_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 33, 0, this.input);
				throw ex;
				IL_159:
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 30, PapyrusGen.FOLLOW_IMULTIPLY_in_mult_expression2718);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_mult_expression2720);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_mult_expression2724);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_mult_expression2728);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_mult_expression2732);
					PapyrusGen.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_unary_expression_in_mult_expression2736);
					PapyrusGen.unary_expression_return unary_expression_return = this.unary_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					mult_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					mult_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "IMULTIPLY").Add("target", mult_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (mult_expression_return2 != null) ? mult_expression_return2.ST : null).Add("extraExpressions2", (unary_expression_return != null) ? unary_expression_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 31, PapyrusGen.FOLLOW_FMULTIPLY_in_mult_expression2801);
					this.Match(this.input, 2, null);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_mult_expression2803);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_mult_expression2807);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_mult_expression2811);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_mult_expression2815);
					PapyrusGen.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_unary_expression_in_mult_expression2819);
					PapyrusGen.unary_expression_return unary_expression_return = this.unary_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					mult_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					mult_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "FMULTIPLY").Add("target", mult_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (mult_expression_return2 != null) ? mult_expression_return2.ST : null).Add("extraExpressions2", (unary_expression_return != null) ? unary_expression_return.ST : null).Add("lineNo", commonTree3.Line));
					break;
				}
				case 3:
				{
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 32, PapyrusGen.FOLLOW_IDIVIDE_in_mult_expression2884);
					this.Match(this.input, 2, null);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_mult_expression2886);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_mult_expression2890);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_mult_expression2894);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_mult_expression2898);
					PapyrusGen.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_unary_expression_in_mult_expression2902);
					PapyrusGen.unary_expression_return unary_expression_return = this.unary_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					mult_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					mult_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "IDIVIDE").Add("target", mult_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (mult_expression_return2 != null) ? mult_expression_return2.ST : null).Add("extraExpressions2", (unary_expression_return != null) ? unary_expression_return.ST : null).Add("lineNo", commonTree5.Line));
					break;
				}
				case 4:
				{
					CommonTree commonTree7 = (CommonTree)this.Match(this.input, 33, PapyrusGen.FOLLOW_FDIVIDE_in_mult_expression2967);
					this.Match(this.input, 2, null);
					CommonTree commonTree8 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_mult_expression2969);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_mult_expression2973);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_mult_expression2977);
					PapyrusGen.autoCast_return autoCast_return2 = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_mult_expression2981);
					PapyrusGen.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_unary_expression_in_mult_expression2985);
					PapyrusGen.unary_expression_return unary_expression_return = this.unary_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					mult_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree8.Text);
					mult_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "FDIVIDE").Add("target", mult_expression_return.sRetValue).Add("arg1", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("arg2", (autoCast_return2 != null) ? autoCast_return2.sRetValue : null).Add("autoCast1", (autoCast_return != null) ? autoCast_return.ST : null).Add("autoCast2", (autoCast_return2 != null) ? autoCast_return2.ST : null).Add("extraExpressions1", (mult_expression_return2 != null) ? mult_expression_return2.ST : null).Add("extraExpressions2", (unary_expression_return != null) ? unary_expression_return.ST : null).Add("lineNo", commonTree7.Line));
					break;
				}
				case 5:
				{
					CommonTree commonTree9 = (CommonTree)this.Match(this.input, 77, PapyrusGen.FOLLOW_MOD_in_mult_expression3050);
					this.Match(this.input, 2, null);
					CommonTree commonTree10 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_mult_expression3052);
					base.PushFollow(PapyrusGen.FOLLOW_mult_expression_in_mult_expression3056);
					PapyrusGen.mult_expression_return mult_expression_return2 = this.mult_expression();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_unary_expression_in_mult_expression3060);
					PapyrusGen.unary_expression_return unary_expression_return = this.unary_expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					mult_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree10.Text);
					mult_expression_return.ST = this.templateLib.GetInstanceOf("twoOpCommand", new PapyrusGen.STAttrMap().Add("command", "IMOD").Add("target", mult_expression_return.sRetValue).Add("arg1", (mult_expression_return2 != null) ? mult_expression_return2.sRetValue : null).Add("arg2", (unary_expression_return != null) ? unary_expression_return.sRetValue : null).Add("extraExpressions1", (mult_expression_return2 != null) ? mult_expression_return2.ST : null).Add("extraExpressions2", (unary_expression_return != null) ? unary_expression_return.ST : null).Add("lineNo", commonTree9.Line));
					break;
				}
				case 6:
				{
					base.PushFollow(PapyrusGen.FOLLOW_unary_expression_in_mult_expression3114);
					PapyrusGen.unary_expression_return unary_expression_return2 = this.unary_expression();
					this.state.followingStackPointer--;
					mult_expression_return.sRetValue = ((unary_expression_return2 != null) ? unary_expression_return2.sRetValue : null);
					mult_expression_return.ST = ((unary_expression_return2 != null) ? unary_expression_return2.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return mult_expression_return;
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x000648E8 File Offset: 0x00062AE8
		public PapyrusGen.unary_expression_return unary_expression()
		{
			PapyrusGen.unary_expression_return unary_expression_return = new PapyrusGen.unary_expression_return();
			unary_expression_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 34, 0, this.input);
				throw ex;
				IL_12E:
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 34, PapyrusGen.FOLLOW_INEGATE_in_unary_expression3137);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_unary_expression3139);
					base.PushFollow(PapyrusGen.FOLLOW_cast_atom_in_unary_expression3141);
					PapyrusGen.cast_atom_return cast_atom_return = this.cast_atom();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					unary_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					unary_expression_return.ST = this.templateLib.GetInstanceOf("singleOpCommand", new PapyrusGen.STAttrMap().Add("command", "INEGATE").Add("target", unary_expression_return.sRetValue).Add("source", (cast_atom_return != null) ? cast_atom_return.sRetValue : null).Add("extraExpressions", (cast_atom_return != null) ? cast_atom_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 35, PapyrusGen.FOLLOW_FNEGATE_in_unary_expression3186);
					this.Match(this.input, 2, null);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_unary_expression3188);
					base.PushFollow(PapyrusGen.FOLLOW_cast_atom_in_unary_expression3190);
					PapyrusGen.cast_atom_return cast_atom_return2 = this.cast_atom();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					unary_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					unary_expression_return.ST = this.templateLib.GetInstanceOf("singleOpCommand", new PapyrusGen.STAttrMap().Add("command", "FNEGATE").Add("target", unary_expression_return.sRetValue).Add("source", (cast_atom_return2 != null) ? cast_atom_return2.sRetValue : null).Add("extraExpressions", (cast_atom_return2 != null) ? cast_atom_return2.ST : null).Add("lineNo", commonTree3.Line));
					break;
				}
				case 3:
				{
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 78, PapyrusGen.FOLLOW_NOT_in_unary_expression3235);
					this.Match(this.input, 2, null);
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_unary_expression3237);
					base.PushFollow(PapyrusGen.FOLLOW_cast_atom_in_unary_expression3239);
					PapyrusGen.cast_atom_return cast_atom_return3 = this.cast_atom();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					unary_expression_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					unary_expression_return.ST = this.templateLib.GetInstanceOf("singleOpCommand", new PapyrusGen.STAttrMap().Add("command", "NOT").Add("target", unary_expression_return.sRetValue).Add("source", (cast_atom_return3 != null) ? cast_atom_return3.sRetValue : null).Add("extraExpressions", (cast_atom_return3 != null) ? cast_atom_return3.ST : null).Add("lineNo", commonTree5.Line));
					break;
				}
				case 4:
				{
					base.PushFollow(PapyrusGen.FOLLOW_cast_atom_in_unary_expression3283);
					PapyrusGen.cast_atom_return cast_atom_return4 = this.cast_atom();
					this.state.followingStackPointer--;
					unary_expression_return.sRetValue = ((cast_atom_return4 != null) ? cast_atom_return4.sRetValue : null);
					unary_expression_return.ST = ((cast_atom_return4 != null) ? cast_atom_return4.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return unary_expression_return;
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x00064E20 File Offset: 0x00063020
		public PapyrusGen.cast_atom_return cast_atom()
		{
			PapyrusGen.cast_atom_return cast_atom_return = new PapyrusGen.cast_atom_return();
			cast_atom_return.Start = this.input.LT(1);
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
					if ((num < 11 || num > 13) && (num != 15 && num != 20 && num != 22 && (num < 24 || num > 25)) && (num != 38 && num != 62 && (num < 80 || num > 82)) && (num < 90 || num > 93))
					{
						NoViableAltException ex = new NoViableAltException("", 35, 0, this.input);
						throw ex;
					}
					num2 = 2;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 79, PapyrusGen.FOLLOW_AS_in_cast_atom3306);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_cast_atom3308);
					base.PushFollow(PapyrusGen.FOLLOW_dot_atom_in_cast_atom3310);
					PapyrusGen.dot_atom_return dot_atom_return = this.dot_atom();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					cast_atom_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					cast_atom_return.ST = this.templateLib.GetInstanceOf("cast", new PapyrusGen.STAttrMap().Add("target", cast_atom_return.sRetValue).Add("source", (dot_atom_return != null) ? dot_atom_return.sRetValue : null).Add("extraExpressions", (dot_atom_return != null) ? dot_atom_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_dot_atom_in_cast_atom3349);
					PapyrusGen.dot_atom_return dot_atom_return2 = this.dot_atom();
					this.state.followingStackPointer--;
					cast_atom_return.sRetValue = ((dot_atom_return2 != null) ? dot_atom_return2.sRetValue : null);
					cast_atom_return.ST = ((dot_atom_return2 != null) ? dot_atom_return2.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return cast_atom_return;
		}

		// Token: 0x06000D94 RID: 3476 RVA: 0x0006507C File Offset: 0x0006327C
		public PapyrusGen.dot_atom_return dot_atom()
		{
			PapyrusGen.dot_atom_return dot_atom_return = new PapyrusGen.dot_atom_return();
			dot_atom_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 36, 0, this.input);
				throw ex;
				IL_E5:
				switch (num2)
				{
				case 1:
				{
					this.Match(this.input, 62, PapyrusGen.FOLLOW_DOT_in_dot_atom3372);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_dot_atom_in_dot_atom3376);
					PapyrusGen.dot_atom_return dot_atom_return2 = this.dot_atom();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_array_func_or_id_in_dot_atom3380);
					PapyrusGen.array_func_or_id_return array_func_or_id_return = this.array_func_or_id();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					dot_atom_return.sRetValue = ((array_func_or_id_return != null) ? array_func_or_id_return.sRetValue : null);
					dot_atom_return.ST = this.templateLib.GetInstanceOf("dot", new PapyrusGen.STAttrMap().Add("aTemplate", (dot_atom_return2 != null) ? dot_atom_return2.ST : null).Add("bTemplate", (array_func_or_id_return != null) ? array_func_or_id_return.ST : null));
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_array_atom_in_dot_atom3409);
					PapyrusGen.array_atom_return array_atom_return = this.array_atom();
					this.state.followingStackPointer--;
					dot_atom_return.sRetValue = ((array_atom_return != null) ? array_atom_return.sRetValue : null);
					dot_atom_return.ST = ((array_atom_return != null) ? array_atom_return.ST : null);
					break;
				}
				case 3:
				{
					base.PushFollow(PapyrusGen.FOLLOW_constant_in_dot_atom3420);
					PapyrusGen.constant_return constant_return = this.constant();
					this.state.followingStackPointer--;
					dot_atom_return.sRetValue = ((constant_return != null) ? ((CommonTree)constant_return.Start) : null).Text;
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return dot_atom_return;
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x00065330 File Offset: 0x00063530
		public PapyrusGen.array_atom_return array_atom()
		{
			this.array_atom_stack.Push(new PapyrusGen.array_atom_scope());
			PapyrusGen.array_atom_return array_atom_return = new PapyrusGen.array_atom_return();
			array_atom_return.Start = this.input.LT(1);
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
					if ((num < 11 || num > 13) && (num != 15 && num != 20 && (num < 24 || num > 25)) && num != 38 && num != 80 && num != 82)
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
					CommonTree commonTree = (CommonTree)this.Match(this.input, 22, PapyrusGen.FOLLOW_ARRAYGET_in_array_atom3447);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_array_atom3451);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_array_atom3455);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_array_atom3457);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_atom_in_array_atom3459);
					PapyrusGen.atom_return atom_return = this.atom();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_array_atom3461);
					PapyrusGen.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					array_atom_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					((PapyrusGen.array_atom_scope)this.array_atom_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					array_atom_return.ST = this.templateLib.GetInstanceOf("arrayGet", new PapyrusGen.STAttrMap().Add("retValue", array_atom_return.sRetValue).Add("selfName", ((PapyrusGen.array_atom_scope)this.array_atom_stack.Peek()).sselfName).Add("index", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("autoCast", (autoCast_return != null) ? autoCast_return.ST : null).Add("arrayExpressions", (atom_return != null) ? atom_return.ST : null).Add("indexExpressions", (expression_return != null) ? expression_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_atom_in_array_atom3515);
					PapyrusGen.atom_return atom_return2 = this.atom();
					this.state.followingStackPointer--;
					array_atom_return.sRetValue = ((atom_return2 != null) ? atom_return2.sRetValue : null);
					array_atom_return.ST = ((atom_return2 != null) ? atom_return2.ST : null);
					break;
				}
				}
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

		// Token: 0x06000D96 RID: 3478 RVA: 0x000656A0 File Offset: 0x000638A0
		public PapyrusGen.atom_return atom()
		{
			PapyrusGen.atom_return atom_return = new PapyrusGen.atom_return();
			atom_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 38, 0, this.input);
				throw ex;
				IL_B8:
				switch (num2)
				{
				case 1:
				{
					this.Match(this.input, 15, PapyrusGen.FOLLOW_PAREXPR_in_atom3538);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_atom3540);
					PapyrusGen.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					atom_return.sRetValue = ((expression_return != null) ? expression_return.sRetValue : null);
					atom_return.ST = ((expression_return != null) ? expression_return.ST : null);
					break;
				}
				case 2:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 80, PapyrusGen.FOLLOW_NEW_in_atom3553);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 81, PapyrusGen.FOLLOW_INTEGER_in_atom3555);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_atom3559);
					this.Match(this.input, 3, null);
					atom_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					atom_return.ST = this.templateLib.GetInstanceOf("newArray", new PapyrusGen.STAttrMap().Add("dest", atom_return.sRetValue).Add("size", commonTree2.Text).Add("lineNo", commonTree.Line));
					break;
				}
				case 3:
				{
					base.PushFollow(PapyrusGen.FOLLOW_func_or_id_in_atom3593);
					PapyrusGen.func_or_id_return func_or_id_return = this.func_or_id();
					this.state.followingStackPointer--;
					atom_return.sRetValue = ((func_or_id_return != null) ? func_or_id_return.sRetValue : null);
					atom_return.ST = ((func_or_id_return != null) ? func_or_id_return.ST : null);
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return atom_return;
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x0006596C File Offset: 0x00063B6C
		public PapyrusGen.array_func_or_id_return array_func_or_id()
		{
			this.array_func_or_id_stack.Push(new PapyrusGen.array_func_or_id_scope());
			PapyrusGen.array_func_or_id_return array_func_or_id_return = new PapyrusGen.array_func_or_id_return();
			array_func_or_id_return.Start = this.input.LT(1);
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
					if ((num < 11 || num > 13) && (num != 20 && (num < 24 || num > 25)) && num != 38 && num != 82)
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
					CommonTree commonTree = (CommonTree)this.Match(this.input, 22, PapyrusGen.FOLLOW_ARRAYGET_in_array_func_or_id3620);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_array_func_or_id3624);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_array_func_or_id3628);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_array_func_or_id3630);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_func_or_id_in_array_func_or_id3632);
					PapyrusGen.func_or_id_return func_or_id_return = this.func_or_id();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_array_func_or_id3634);
					PapyrusGen.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					array_func_or_id_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					((PapyrusGen.array_func_or_id_scope)this.array_func_or_id_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					array_func_or_id_return.ST = this.templateLib.GetInstanceOf("arrayGet", new PapyrusGen.STAttrMap().Add("retValue", array_func_or_id_return.sRetValue).Add("selfName", ((PapyrusGen.array_func_or_id_scope)this.array_func_or_id_stack.Peek()).sselfName).Add("index", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("autoCast", (autoCast_return != null) ? autoCast_return.ST : null).Add("arrayExpressions", (func_or_id_return != null) ? func_or_id_return.ST : null).Add("indexExpressions", (expression_return != null) ? expression_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					base.PushFollow(PapyrusGen.FOLLOW_func_or_id_in_array_func_or_id3688);
					PapyrusGen.func_or_id_return func_or_id_return2 = this.func_or_id();
					this.state.followingStackPointer--;
					array_func_or_id_return.sRetValue = ((func_or_id_return2 != null) ? func_or_id_return2.sRetValue : null);
					array_func_or_id_return.ST = ((func_or_id_return2 != null) ? func_or_id_return2.ST : null);
					break;
				}
				}
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

		// Token: 0x06000D98 RID: 3480 RVA: 0x00065CD0 File Offset: 0x00063ED0
		public PapyrusGen.func_or_id_return func_or_id()
		{
			this.func_or_id_stack.Push(new PapyrusGen.func_or_id_scope());
			PapyrusGen.func_or_id_return func_or_id_return = new PapyrusGen.func_or_id_return();
			func_or_id_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
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
				NoViableAltException ex = new NoViableAltException("", 40, 0, this.input);
				throw ex;
				IL_BB:
				switch (num2)
				{
				case 1:
				{
					base.PushFollow(PapyrusGen.FOLLOW_function_call_in_func_or_id3714);
					PapyrusGen.function_call_return function_call_return = this.function_call();
					this.state.followingStackPointer--;
					func_or_id_return.sRetValue = ((function_call_return != null) ? function_call_return.sRetValue : null);
					func_or_id_return.ST = ((function_call_return != null) ? function_call_return.ST : null);
					break;
				}
				case 2:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 20, PapyrusGen.FOLLOW_PROPGET_in_func_or_id3726);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_func_or_id3730);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_func_or_id3734);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_func_or_id3738);
					this.Match(this.input, 3, null);
					((PapyrusGen.func_or_id_scope)this.func_or_id_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					func_or_id_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					func_or_id_return.ST = this.templateLib.GetInstanceOf("propGet", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.func_or_id_scope)this.func_or_id_stack.Peek()).sselfName).Add("name", commonTree3.Text).Add("retValue", func_or_id_return.sRetValue).Add("lineNo", commonTree.Line));
					break;
				}
				case 3:
				{
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_func_or_id3777);
					func_or_id_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree5.Text);
					break;
				}
				case 4:
				{
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 82, PapyrusGen.FOLLOW_LENGTH_in_func_or_id3789);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_func_or_id3793);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_func_or_id3797);
					this.Match(this.input, 3, null);
					((PapyrusGen.func_or_id_scope)this.func_or_id_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					func_or_id_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					func_or_id_return.ST = this.templateLib.GetInstanceOf("arrayLength", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.func_or_id_scope)this.func_or_id_stack.Peek()).sselfName).Add("retValue", func_or_id_return.sRetValue).Add("lineNo", commonTree6.Line));
					break;
				}
				}
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

		// Token: 0x06000D99 RID: 3481 RVA: 0x00066124 File Offset: 0x00064324
		public PapyrusGen.property_set_return property_set()
		{
			this.property_set_stack.Push(new PapyrusGen.property_set_scope());
			PapyrusGen.property_set_return property_set_return = new PapyrusGen.property_set_return();
			property_set_return.Start = this.input.LT(1);
			try
			{
				CommonTree commonTree = (CommonTree)this.Match(this.input, 21, PapyrusGen.FOLLOW_PROPSET_in_property_set3843);
				this.Match(this.input, 2, null);
				CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_property_set3847);
				CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_property_set3851);
				CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_property_set3855);
				this.Match(this.input, 3, null);
				((PapyrusGen.property_set_scope)this.property_set_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
				((PapyrusGen.property_set_scope)this.property_set_stack.Peek()).sparamName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
				property_set_return.ST = this.templateLib.GetInstanceOf("propSet", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.property_set_scope)this.property_set_stack.Peek()).sselfName).Add("name", commonTree3.Text).Add("param", ((PapyrusGen.property_set_scope)this.property_set_stack.Peek()).sparamName).Add("lineNo", commonTree.Line));
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			finally
			{
				this.property_set_stack.Pop();
			}
			return property_set_return;
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x0006632C File Offset: 0x0006452C
		public PapyrusGen.return_stat_return return_stat()
		{
			PapyrusGen.return_stat_return return_stat_return = new PapyrusGen.return_stat_return();
			return_stat_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
				if (num != 83)
				{
					NoViableAltException ex = new NoViableAltException("", 41, 0, this.input);
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
					if (num2 != 3 && num2 != 5 && (num2 < 11 || num2 > 13) && (num2 != 15 && num2 != 20 && num2 != 22 && (num2 < 24 || num2 > 36)) && (num2 != 38 && num2 != 41 && num2 != 62 && (num2 < 65 || num2 > 72)) && (num2 < 77 || num2 > 84) && num2 != 88 && (num2 < 90 || num2 > 93))
					{
						NoViableAltException ex2 = new NoViableAltException("", 41, 1, this.input);
						throw ex2;
					}
					num3 = 2;
				}
				switch (num3)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 83, PapyrusGen.FOLLOW_RETURN_in_return_stat3903);
					this.Match(this.input, 2, null);
					base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_return_stat3905);
					PapyrusGen.autoCast_return autoCast_return = this.autoCast();
					this.state.followingStackPointer--;
					base.PushFollow(PapyrusGen.FOLLOW_expression_in_return_stat3907);
					PapyrusGen.expression_return expression_return = this.expression();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					return_stat_return.ST = this.templateLib.GetInstanceOf("return", new PapyrusGen.STAttrMap().Add("retVal", (autoCast_return != null) ? autoCast_return.sRetValue : null).Add("autoCast", (autoCast_return != null) ? autoCast_return.ST : null).Add("extraExpressions", (expression_return != null) ? expression_return.ST : null).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 83, PapyrusGen.FOLLOW_RETURN_in_return_stat3940);
					return_stat_return.ST = this.templateLib.GetInstanceOf("return", new PapyrusGen.STAttrMap().Add("retVal", "none").Add("lineNo", commonTree2.Line));
					break;
				}
				}
			}
			catch (RecognitionException ex3)
			{
				this.ReportError(ex3);
				this.Recover(this.input, ex3);
			}
			return return_stat_return;
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x000665D0 File Offset: 0x000647D0
		public PapyrusGen.ifBlock_return ifBlock()
		{
			this.ifBlock_stack.Push(new PapyrusGen.ifBlock_scope());
			PapyrusGen.ifBlock_return ifBlock_return = new PapyrusGen.ifBlock_return();
			ifBlock_return.Start = this.input.LT(1);
			IList list = null;
			PapyrusGen.elseBlock_return elseBlock_return = null;
			((PapyrusGen.ifBlock_scope)this.ifBlock_stack.Peek()).kBlockStatements = new ArrayList();
			((PapyrusGen.ifBlock_scope)this.ifBlock_stack.Peek()).sEndLabel = this.GenerateLabel();
			((PapyrusGen.ifBlock_scope)this.ifBlock_stack.Peek()).kchildScope = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.Children[((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				CommonTree commonTree = (CommonTree)this.Match(this.input, 84, PapyrusGen.FOLLOW_IF_in_ifBlock3984);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_expression_in_ifBlock3986);
				PapyrusGen.expression_return expression_return = this.expression();
				this.state.followingStackPointer--;
				base.PushFollow(PapyrusGen.FOLLOW_codeBlock_in_ifBlock3988);
				this.codeBlock(((PapyrusGen.ifBlock_scope)this.ifBlock_stack.Peek()).kBlockStatements, ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kvarDefs, ((PapyrusGen.ifBlock_scope)this.ifBlock_stack.Peek()).kchildScope);
				this.state.followingStackPointer--;
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
					base.PushFollow(PapyrusGen.FOLLOW_elseIfBlock_in_ifBlock3994);
					PapyrusGen.elseIfBlock_return elseIfBlock_return = this.elseIfBlock();
					this.state.followingStackPointer--;
					if (list == null)
					{
						list = new ArrayList();
					}
					list.Add(elseIfBlock_return.Template);
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
					base.PushFollow(PapyrusGen.FOLLOW_elseBlock_in_ifBlock3998);
					elseBlock_return = this.elseBlock();
					this.state.followingStackPointer--;
				}
				this.Match(this.input, 3, null);
				ifBlock_return.ST = this.templateLib.GetInstanceOf("ifBlock", new PapyrusGen.STAttrMap().Add("condition", (expression_return != null) ? expression_return.sRetValue : null).Add("condExpressions", (expression_return != null) ? expression_return.ST : null).Add("blockStatements", ((PapyrusGen.ifBlock_scope)this.ifBlock_stack.Peek()).kBlockStatements).Add("elifBlocks", list).Add("elseBlock", (elseBlock_return != null) ? elseBlock_return.ST : null).Add("elseLabel", this.GenerateLabel()).Add("endLabel", ((PapyrusGen.ifBlock_scope)this.ifBlock_stack.Peek()).sEndLabel).Add("lineNo", commonTree.Line));
				((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild++;
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

		// Token: 0x06000D9C RID: 3484 RVA: 0x00066944 File Offset: 0x00064B44
		public PapyrusGen.elseIfBlock_return elseIfBlock()
		{
			this.elseIfBlock_stack.Push(new PapyrusGen.elseIfBlock_scope());
			PapyrusGen.elseIfBlock_return elseIfBlock_return = new PapyrusGen.elseIfBlock_return();
			elseIfBlock_return.Start = this.input.LT(1);
			((PapyrusGen.elseIfBlock_scope)this.elseIfBlock_stack.Peek()).kBlockStatements = new ArrayList();
			((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild++;
			((PapyrusGen.elseIfBlock_scope)this.elseIfBlock_stack.Peek()).kchildScope = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.Children[((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				CommonTree commonTree = (CommonTree)this.Match(this.input, 86, PapyrusGen.FOLLOW_ELSEIF_in_elseIfBlock4072);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_expression_in_elseIfBlock4074);
				PapyrusGen.expression_return expression_return = this.expression();
				this.state.followingStackPointer--;
				base.PushFollow(PapyrusGen.FOLLOW_codeBlock_in_elseIfBlock4076);
				this.codeBlock(((PapyrusGen.elseIfBlock_scope)this.elseIfBlock_stack.Peek()).kBlockStatements, ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kvarDefs, ((PapyrusGen.elseIfBlock_scope)this.elseIfBlock_stack.Peek()).kchildScope);
				this.state.followingStackPointer--;
				this.Match(this.input, 3, null);
				elseIfBlock_return.ST = this.templateLib.GetInstanceOf("elseIfBlock", new PapyrusGen.STAttrMap().Add("condition", (expression_return != null) ? expression_return.sRetValue : null).Add("condExpressions", (expression_return != null) ? expression_return.ST : null).Add("blockStatements", ((PapyrusGen.elseIfBlock_scope)this.elseIfBlock_stack.Peek()).kBlockStatements).Add("elseLabel", this.GenerateLabel()).Add("endLabel", ((PapyrusGen.ifBlock_scope)this.ifBlock_stack.Peek()).sEndLabel).Add("lineNo", commonTree.Line));
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

		// Token: 0x06000D9D RID: 3485 RVA: 0x00066BC0 File Offset: 0x00064DC0
		public PapyrusGen.elseBlock_return elseBlock()
		{
			this.elseBlock_stack.Push(new PapyrusGen.elseBlock_scope());
			PapyrusGen.elseBlock_return elseBlock_return = new PapyrusGen.elseBlock_return();
			elseBlock_return.Start = this.input.LT(1);
			((PapyrusGen.elseBlock_scope)this.elseBlock_stack.Peek()).kBlockStatements = new ArrayList();
			((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild++;
			((PapyrusGen.elseBlock_scope)this.elseBlock_stack.Peek()).kchildScope = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.Children[((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				this.Match(this.input, 87, PapyrusGen.FOLLOW_ELSE_in_elseBlock4140);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_codeBlock_in_elseBlock4142);
				this.codeBlock(((PapyrusGen.elseBlock_scope)this.elseBlock_stack.Peek()).kBlockStatements, ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kvarDefs, ((PapyrusGen.elseBlock_scope)this.elseBlock_stack.Peek()).kchildScope);
				this.state.followingStackPointer--;
				this.Match(this.input, 3, null);
				elseBlock_return.ST = this.templateLib.GetInstanceOf("elseBlock", new PapyrusGen.STAttrMap().Add("blockStatements", ((PapyrusGen.elseBlock_scope)this.elseBlock_stack.Peek()).kBlockStatements));
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

		// Token: 0x06000D9E RID: 3486 RVA: 0x00066D8C File Offset: 0x00064F8C
		public PapyrusGen.whileBlock_return whileBlock()
		{
			this.whileBlock_stack.Push(new PapyrusGen.whileBlock_scope());
			PapyrusGen.whileBlock_return whileBlock_return = new PapyrusGen.whileBlock_return();
			whileBlock_return.Start = this.input.LT(1);
			((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).kBlockStatements = new ArrayList();
			((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).sStartLabel = this.GenerateLabel();
			((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).sEndLabel = this.GenerateLabel();
			((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).kchildScope = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.Children[((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild];
			try
			{
				CommonTree commonTree = (CommonTree)this.Match(this.input, 88, PapyrusGen.FOLLOW_WHILE_in_whileBlock4183);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_expression_in_whileBlock4185);
				PapyrusGen.expression_return expression_return = this.expression();
				this.state.followingStackPointer--;
				base.PushFollow(PapyrusGen.FOLLOW_codeBlock_in_whileBlock4187);
				this.codeBlock(((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).kBlockStatements, ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kvarDefs, ((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).kchildScope);
				this.state.followingStackPointer--;
				this.Match(this.input, 3, null);
				whileBlock_return.ST = this.templateLib.GetInstanceOf("whileBlock", new PapyrusGen.STAttrMap().Add("condition", (expression_return != null) ? expression_return.sRetValue : null).Add("condExpressions", (expression_return != null) ? expression_return.ST : null).Add("blockStatements", ((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).kBlockStatements).Add("startLabel", ((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).sStartLabel).Add("endLabel", ((PapyrusGen.whileBlock_scope)this.whileBlock_stack.Peek()).sEndLabel).Add("lineNo", commonTree.Line));
				((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).inextScopeChild++;
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

		// Token: 0x06000D9F RID: 3487 RVA: 0x0006704C File Offset: 0x0006524C
		public PapyrusGen.function_call_return function_call()
		{
			this.function_call_stack.Push(new PapyrusGen.function_call_scope());
			PapyrusGen.function_call_return function_call_return = new PapyrusGen.function_call_return();
			function_call_return.Start = this.input.LT(1);
			PapyrusGen.parameters_return parameters_return = null;
			PapyrusGen.parameters_return parameters_return2 = null;
			PapyrusGen.parameters_return parameters_return3 = null;
			PapyrusGen.parameters_return parameters_return4 = null;
			PapyrusGen.parameters_return parameters_return5 = null;
			try
			{
				int num = this.input.LA(1);
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
						NoViableAltException ex = new NoViableAltException("", 49, 0, this.input);
						throw ex;
					}
					}
					break;
				}
				switch (num2)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 11, PapyrusGen.FOLLOW_CALL_in_function_call4252);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4256);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4260);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4264);
					this.Match(this.input, 14, PapyrusGen.FOLLOW_CALLPARAMS_in_function_call4267);
					if (this.input.LA(1) == 2)
					{
						this.Match(this.input, 2, null);
						int num3 = 2;
						int num4 = this.input.LA(1);
						if (num4 == 9)
						{
							num3 = 1;
						}
						int num5 = num3;
						if (num5 == 1)
						{
							base.PushFollow(PapyrusGen.FOLLOW_parameters_in_function_call4269);
							parameters_return = this.parameters();
							this.state.followingStackPointer--;
						}
						this.Match(this.input, 3, null);
					}
					this.Match(this.input, 3, null);
					((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					function_call_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if (((parameters_return != null) ? ((CommonTree)parameters_return.Start) : null) == null)
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("callLocal", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName).Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree.Line));
					}
					else
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("callLocal", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName).Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("args", (parameters_return != null) ? parameters_return.sParamVars : null).Add("autoCast", (parameters_return != null) ? parameters_return.kAutoCastST : null).Add("paramExpressions", (parameters_return != null) ? parameters_return.ST : null).Add("lineNo", commonTree.Line));
					}
					break;
				}
				case 2:
				{
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 13, PapyrusGen.FOLLOW_CALLPARENT_in_function_call4355);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4359);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4363);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4367);
					this.Match(this.input, 14, PapyrusGen.FOLLOW_CALLPARAMS_in_function_call4370);
					if (this.input.LA(1) == 2)
					{
						this.Match(this.input, 2, null);
						int num6 = 2;
						int num7 = this.input.LA(1);
						if (num7 == 9)
						{
							num6 = 1;
						}
						int num8 = num6;
						if (num8 == 1)
						{
							base.PushFollow(PapyrusGen.FOLLOW_parameters_in_function_call4372);
							parameters_return2 = this.parameters();
							this.state.followingStackPointer--;
						}
						this.Match(this.input, 3, null);
					}
					this.Match(this.input, 3, null);
					function_call_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if (((parameters_return2 != null) ? ((CommonTree)parameters_return2.Start) : null) == null)
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("callParent", new PapyrusGen.STAttrMap().Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree5.Line));
					}
					else
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("callParent", new PapyrusGen.STAttrMap().Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("args", (parameters_return2 != null) ? parameters_return2.sParamVars : null).Add("autoCast", (parameters_return2 != null) ? parameters_return2.kAutoCastST : null).Add("paramExpressions", (parameters_return2 != null) ? parameters_return2.ST : null).Add("lineNo", commonTree5.Line));
					}
					break;
				}
				case 3:
				{
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 12, PapyrusGen.FOLLOW_CALLGLOBAL_in_function_call4448);
					this.Match(this.input, 2, null);
					CommonTree commonTree7 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4452);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4456);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4460);
					this.Match(this.input, 14, PapyrusGen.FOLLOW_CALLPARAMS_in_function_call4463);
					if (this.input.LA(1) == 2)
					{
						this.Match(this.input, 2, null);
						int num9 = 2;
						int num10 = this.input.LA(1);
						if (num10 == 9)
						{
							num9 = 1;
						}
						int num11 = num9;
						if (num11 == 1)
						{
							base.PushFollow(PapyrusGen.FOLLOW_parameters_in_function_call4465);
							parameters_return3 = this.parameters();
							this.state.followingStackPointer--;
						}
						this.Match(this.input, 3, null);
					}
					this.Match(this.input, 3, null);
					function_call_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if (((parameters_return3 != null) ? ((CommonTree)parameters_return3.Start) : null) == null)
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("callGlobal", new PapyrusGen.STAttrMap().Add("objType", commonTree7.Text).Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree6.Line));
					}
					else
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("callGlobal", new PapyrusGen.STAttrMap().Add("objType", commonTree7.Text).Add("name", commonTree3.Text).Add("retValue", function_call_return.sRetValue).Add("args", (parameters_return3 != null) ? parameters_return3.sParamVars : null).Add("autoCast", (parameters_return3 != null) ? parameters_return3.kAutoCastST : null).Add("paramExpressions", (parameters_return3 != null) ? parameters_return3.ST : null).Add("lineNo", commonTree6.Line));
					}
					break;
				}
				case 4:
				{
					CommonTree commonTree8 = (CommonTree)this.Match(this.input, 24, PapyrusGen.FOLLOW_ARRAYFIND_in_function_call4551);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4555);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4559);
					this.Match(this.input, 14, PapyrusGen.FOLLOW_CALLPARAMS_in_function_call4562);
					if (this.input.LA(1) == 2)
					{
						this.Match(this.input, 2, null);
						int num12 = 2;
						int num13 = this.input.LA(1);
						if (num13 == 9)
						{
							num12 = 1;
						}
						int num14 = num12;
						if (num14 == 1)
						{
							base.PushFollow(PapyrusGen.FOLLOW_parameters_in_function_call4564);
							parameters_return4 = this.parameters();
							this.state.followingStackPointer--;
						}
						this.Match(this.input, 3, null);
					}
					this.Match(this.input, 3, null);
					((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					function_call_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if (((parameters_return4 != null) ? ((CommonTree)parameters_return4.Start) : null) == null)
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("arrayFind", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree8.Line));
					}
					else
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("arrayFind", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName).Add("retValue", function_call_return.sRetValue).Add("args", (parameters_return4 != null) ? parameters_return4.sParamVars : null).Add("autoCast", (parameters_return4 != null) ? parameters_return4.kAutoCastST : null).Add("paramExpressions", (parameters_return4 != null) ? parameters_return4.ST : null).Add("lineNo", commonTree8.Line));
					}
					break;
				}
				case 5:
				{
					CommonTree commonTree9 = (CommonTree)this.Match(this.input, 25, PapyrusGen.FOLLOW_ARRAYRFIND_in_function_call4640);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4644);
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_function_call4648);
					this.Match(this.input, 14, PapyrusGen.FOLLOW_CALLPARAMS_in_function_call4651);
					if (this.input.LA(1) == 2)
					{
						this.Match(this.input, 2, null);
						int num15 = 2;
						int num16 = this.input.LA(1);
						if (num16 == 9)
						{
							num15 = 1;
						}
						int num17 = num15;
						if (num17 == 1)
						{
							base.PushFollow(PapyrusGen.FOLLOW_parameters_in_function_call4653);
							parameters_return5 = this.parameters();
							this.state.followingStackPointer--;
						}
						this.Match(this.input, 3, null);
					}
					this.Match(this.input, 3, null);
					((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					function_call_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree4.Text);
					if (((parameters_return5 != null) ? ((CommonTree)parameters_return5.Start) : null) == null)
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("arrayRFind", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName).Add("retValue", function_call_return.sRetValue).Add("lineNo", commonTree9.Line));
					}
					else
					{
						function_call_return.ST = this.templateLib.GetInstanceOf("arrayRFind", new PapyrusGen.STAttrMap().Add("selfName", ((PapyrusGen.function_call_scope)this.function_call_stack.Peek()).sselfName).Add("retValue", function_call_return.sRetValue).Add("args", (parameters_return5 != null) ? parameters_return5.sParamVars : null).Add("autoCast", (parameters_return5 != null) ? parameters_return5.kAutoCastST : null).Add("paramExpressions", (parameters_return5 != null) ? parameters_return5.ST : null).Add("lineNo", commonTree9.Line));
					}
					break;
				}
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			finally
			{
				this.function_call_stack.Pop();
			}
			return function_call_return;
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x00067D9C File Offset: 0x00065F9C
		public PapyrusGen.parameters_return parameters()
		{
			PapyrusGen.parameters_return parameters_return = new PapyrusGen.parameters_return();
			parameters_return.Start = this.input.LT(1);
			IList list = null;
			parameters_return.sParamVars = new ArrayList();
			parameters_return.kAutoCastST = new ArrayList();
			try
			{
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
					base.PushFollow(PapyrusGen.FOLLOW_parameter_in_parameters4751);
					PapyrusGen.parameter_return parameter_return = this.parameter();
					this.state.followingStackPointer--;
					if (list == null)
					{
						list = new ArrayList();
					}
					list.Add(parameter_return.Template);
					parameters_return.sParamVars.Add((parameter_return != null) ? parameter_return.sVarName : null);
					parameters_return.kAutoCastST.Add((parameter_return != null) ? parameter_return.kAutoCastST : null);
					num++;
				}
				if (num < 1)
				{
					EarlyExitException ex = new EarlyExitException(50, this.input);
					throw ex;
				}
				parameters_return.ST = this.templateLib.GetInstanceOf("parameterExpressions", new PapyrusGen.STAttrMap().Add("expressions", list));
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return parameters_return;
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x00067EE4 File Offset: 0x000660E4
		public PapyrusGen.parameter_return parameter()
		{
			PapyrusGen.parameter_return parameter_return = new PapyrusGen.parameter_return();
			parameter_return.Start = this.input.LT(1);
			try
			{
				this.Match(this.input, 9, PapyrusGen.FOLLOW_PARAM_in_parameter4793);
				this.Match(this.input, 2, null);
				base.PushFollow(PapyrusGen.FOLLOW_autoCast_in_parameter4795);
				PapyrusGen.autoCast_return autoCast_return = this.autoCast();
				this.state.followingStackPointer--;
				base.PushFollow(PapyrusGen.FOLLOW_expression_in_parameter4797);
				PapyrusGen.expression_return expression_return = this.expression();
				this.state.followingStackPointer--;
				this.Match(this.input, 3, null);
				parameter_return.ST = ((expression_return != null) ? expression_return.ST : null);
				parameter_return.sVarName = ((autoCast_return != null) ? autoCast_return.sRetValue : null);
				parameter_return.kAutoCastST = ((autoCast_return != null) ? autoCast_return.ST : null);
			}
			catch (RecognitionException ex)
			{
				this.ReportError(ex);
				this.Recover(this.input, ex);
			}
			return parameter_return;
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x00067FEC File Offset: 0x000661EC
		public PapyrusGen.autoCast_return autoCast()
		{
			this.autoCast_stack.Push(new PapyrusGen.autoCast_scope());
			PapyrusGen.autoCast_return autoCast_return = new PapyrusGen.autoCast_return();
			autoCast_return.Start = this.input.LT(1);
			try
			{
				int num = this.input.LA(1);
				int num5;
				if (num != 38)
				{
					switch (num)
					{
					case 79:
					{
						int num2 = this.input.LA(2);
						if (num2 != 2)
						{
							NoViableAltException ex = new NoViableAltException("", 51, 1, this.input);
							throw ex;
						}
						int num3 = this.input.LA(3);
						if (num3 != 38)
						{
							NoViableAltException ex2 = new NoViableAltException("", 51, 4, this.input);
							throw ex2;
						}
						int num4 = this.input.LA(4);
						if (num4 == 38)
						{
							num5 = 1;
							goto IL_150;
						}
						if (num4 == 81 || (num4 >= 90 && num4 <= 93))
						{
							num5 = 2;
							goto IL_150;
						}
						NoViableAltException ex3 = new NoViableAltException("", 51, 5, this.input);
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
					NoViableAltException ex4 = new NoViableAltException("", 51, 0, this.input);
					throw ex4;
				}
				num5 = 3;
				IL_150:
				switch (num5)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 79, PapyrusGen.FOLLOW_AS_in_autoCast4825);
					this.Match(this.input, 2, null);
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_autoCast4829);
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_autoCast4833);
					this.Match(this.input, 3, null);
					((PapyrusGen.autoCast_scope)this.autoCast_stack.Peek()).ssource = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree3.Text);
					autoCast_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree2.Text);
					autoCast_return.ST = this.templateLib.GetInstanceOf("cast", new PapyrusGen.STAttrMap().Add("target", autoCast_return.sRetValue).Add("source", ((PapyrusGen.autoCast_scope)this.autoCast_stack.Peek()).ssource).Add("lineNo", commonTree.Line));
					break;
				}
				case 2:
				{
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 79, PapyrusGen.FOLLOW_AS_in_autoCast4868);
					this.Match(this.input, 2, null);
					CommonTree commonTree5 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_autoCast4870);
					base.PushFollow(PapyrusGen.FOLLOW_constant_in_autoCast4872);
					PapyrusGen.constant_return constant_return = this.constant();
					this.state.followingStackPointer--;
					this.Match(this.input, 3, null);
					autoCast_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree5.Text);
					autoCast_return.ST = this.templateLib.GetInstanceOf("cast", new PapyrusGen.STAttrMap().Add("target", autoCast_return.sRetValue).Add("source", ((constant_return != null) ? ((CommonTree)constant_return.Start) : null).Text).Add("lineNo", commonTree4.Line));
					break;
				}
				case 3:
				{
					CommonTree commonTree6 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_autoCast4906);
					autoCast_return.sRetValue = ((PapyrusGen.codeBlock_scope)this.codeBlock_stack.Peek()).kcurrentScope.GetMangledVariableName(commonTree6.Text);
					break;
				}
				case 4:
				{
					base.PushFollow(PapyrusGen.FOLLOW_constant_in_autoCast4917);
					PapyrusGen.constant_return constant_return2 = this.constant();
					this.state.followingStackPointer--;
					autoCast_return.sRetValue = ((constant_return2 != null) ? ((CommonTree)constant_return2.Start) : null).Text;
					break;
				}
				}
			}
			catch (RecognitionException ex5)
			{
				this.ReportError(ex5);
				this.Recover(this.input, ex5);
			}
			finally
			{
				this.autoCast_stack.Pop();
			}
			return autoCast_return;
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x00068478 File Offset: 0x00066678
		public PapyrusGen.constant_return constant()
		{
			PapyrusGen.constant_return constant_return = new PapyrusGen.constant_return();
			constant_return.Start = this.input.LT(1);
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
						NoViableAltException ex = new NoViableAltException("", 52, 0, this.input);
						throw ex;
					}
					}
				}
				num2 = 1;
				IL_70:
				switch (num2)
				{
				case 1:
					base.PushFollow(PapyrusGen.FOLLOW_number_in_constant4935);
					this.number();
					this.state.followingStackPointer--;
					break;
				case 2:
					this.Match(this.input, 90, PapyrusGen.FOLLOW_STRING_in_constant4941);
					break;
				case 3:
					this.Match(this.input, 91, PapyrusGen.FOLLOW_BOOL_in_constant4947);
					break;
				case 4:
					this.Match(this.input, 92, PapyrusGen.FOLLOW_NONE_in_constant4953);
					break;
				}
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return constant_return;
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x000685A4 File Offset: 0x000667A4
		public PapyrusGen.number_return number()
		{
			PapyrusGen.number_return number_return = new PapyrusGen.number_return();
			number_return.Start = this.input.LT(1);
			try
			{
				if (this.input.LA(1) != 81 && this.input.LA(1) != 93)
				{
					MismatchedSetException ex = new MismatchedSetException(null, this.input);
					throw ex;
				}
				this.input.Consume();
				this.state.errorRecovery = false;
			}
			catch (RecognitionException ex2)
			{
				this.ReportError(ex2);
				this.Recover(this.input, ex2);
			}
			return number_return;
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x0006863C File Offset: 0x0006683C
		public PapyrusGen.type_return type()
		{
			PapyrusGen.type_return type_return = new PapyrusGen.type_return();
			type_return.Start = this.input.LT(1);
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
						if (num2 != 38)
						{
							NoViableAltException ex = new NoViableAltException("", 53, 1, this.input);
							throw ex;
						}
						num3 = 1;
					}
				}
				else
				{
					if (num != 55)
					{
						NoViableAltException ex2 = new NoViableAltException("", 53, 0, this.input);
						throw ex2;
					}
					int num4 = this.input.LA(2);
					if (num4 == 63)
					{
						num3 = 4;
					}
					else
					{
						if (num4 != 38)
						{
							NoViableAltException ex3 = new NoViableAltException("", 53, 2, this.input);
							throw ex3;
						}
						num3 = 3;
					}
				}
				switch (num3)
				{
				case 1:
				{
					CommonTree commonTree = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_type4985);
					type_return.sTypeString = commonTree.Text;
					break;
				}
				case 2:
				{
					CommonTree commonTree2 = (CommonTree)this.Match(this.input, 38, PapyrusGen.FOLLOW_ID_in_type4996);
					this.Match(this.input, 63, PapyrusGen.FOLLOW_LBRACKET_in_type4998);
					this.Match(this.input, 64, PapyrusGen.FOLLOW_RBRACKET_in_type5000);
					type_return.sTypeString = $"{commonTree2.Text}[]";
					break;
				}
				case 3:
				{
					CommonTree commonTree3 = (CommonTree)this.Match(this.input, 55, PapyrusGen.FOLLOW_BASETYPE_in_type5011);
					type_return.sTypeString = commonTree3.Text;
					break;
				}
				case 4:
				{
					CommonTree commonTree4 = (CommonTree)this.Match(this.input, 55, PapyrusGen.FOLLOW_BASETYPE_in_type5022);
					this.Match(this.input, 63, PapyrusGen.FOLLOW_LBRACKET_in_type5024);
					this.Match(this.input, 64, PapyrusGen.FOLLOW_RBRACKET_in_type5026);
					type_return.sTypeString = $"{commonTree4.Text}[]";
					break;
				}
				}
			}
			catch (RecognitionException ex4)
			{
				this.ReportError(ex4);
				this.Recover(this.input, ex4);
			}
			return type_return;
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x00068878 File Offset: 0x00066A78
		private void InitializeCyclicDFAs()
		{
			this.dfa27 = new PapyrusGen.DFA27(this);
		}

		// Token: 0x170001B8 RID: 440
		// (set) Token: 0x06000DA7 RID: 3495 RVA: 0x00068888 File Offset: 0x00066A88
		internal Dictionary<string, PapyrusFlag> KnownUserFlags
		{
			set
			{
				this.kFlagDict = value;
			}
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x00068894 File Offset: 0x00066A94
		private string MangleVariableName(string asOriginalName)
		{
			string result = $"::mangled_{asOriginalName}_{this.iCurMangleSuffix}";
			this.iCurMangleSuffix++;
			return result;
		}

		// Token: 0x06000DA9 RID: 3497 RVA: 0x000688C8 File Offset: 0x00066AC8
		private void MangleFunctionVariables(ScriptFunctionType akFunction)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			this.MangleScopeVariables(akFunction.FunctionScope, ref dictionary);
		}

		// Token: 0x06000DAA RID: 3498 RVA: 0x000688EC File Offset: 0x00066AEC
		private void MangleScopeVariables(ScriptScope akCurrentScope, ref Dictionary<string, bool> akAlreadyDefinedVars)
		{
			foreach (KeyValuePair<string, ScriptScope.ScopeVariable> keyValuePair in akCurrentScope.Variables)
			{
				string key = keyValuePair.Key.ToLowerInvariant();
				if (akAlreadyDefinedVars.ContainsKey(key))
				{
					akCurrentScope.kMangledVarNames.Add(key, this.MangleVariableName(keyValuePair.Key));
				}
				else
				{
					akAlreadyDefinedVars.Add(key, true);
				}
			}
			foreach (ScriptScope akCurrentScope2 in akCurrentScope.Children)
			{
				this.MangleScopeVariables(akCurrentScope2, ref akAlreadyDefinedVars);
			}
		}

		// Token: 0x06000DAB RID: 3499 RVA: 0x000689B8 File Offset: 0x00066BB8
		private string GenerateLabel()
		{
			string result = string.Format("label{0}", this.iCurLabelSuffix);
			this.iCurLabelSuffix++;
			return result;
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x000689EC File Offset: 0x00066BEC
		private static DateTime UnixEpoc
		{
			get
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			}
		}

		// Token: 0x06000DAD RID: 3501 RVA: 0x00068A00 File Offset: 0x00066C00
		private static long ToUnixTime(DateTime akDateTime)
		{
			return Convert.ToInt64((akDateTime - PapyrusGen.UnixEpoc).TotalSeconds);
		}

		// Token: 0x06000DAE RID: 3502 RVA: 0x00068A28 File Offset: 0x00066C28
		private string GetFileModTimeUnix(string asFilename)
		{
			FileInfo fileInfo = new FileInfo(asFilename);
			return PapyrusGen.ToUnixTime(fileInfo.LastWriteTime.ToUniversalTime()).ToString();
		}

		// Token: 0x06000DAF RID: 3503 RVA: 0x00068A58 File Offset: 0x00066C58
		private string GetCompileTimeUnix()
		{
			return PapyrusGen.ToUnixTime(DateTime.UtcNow).ToString();
		}

		// Token: 0x06000DB0 RID: 3504 RVA: 0x00068A78 File Offset: 0x00066C78
		private Hashtable ConstructUserFlagRefInfo()
		{
			Hashtable hashtable = new Hashtable();
			foreach (KeyValuePair<string, PapyrusFlag> keyValuePair in this.kFlagDict)
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

		// Token: 0x04000A5A RID: 2650
		protected StringTemplateGroup templateLib = new StringTemplateGroup("PapyrusGenTemplates", typeof(AngleBracketTemplateLexer));

		// Token: 0x04000A5C RID: 2652
		protected StackList script_stack = new StackList();

		// Token: 0x04000A5D RID: 2653
		protected StackList fieldDefinition_stack = new StackList();

		// Token: 0x04000A5E RID: 2654
		protected StackList function_stack = new StackList();

		// Token: 0x04000A5F RID: 2655
		protected StackList eventFunc_stack = new StackList();

		// Token: 0x04000A60 RID: 2656
		protected StackList propertyBlock_stack = new StackList();

		// Token: 0x04000A61 RID: 2657
		protected StackList codeBlock_stack = new StackList();

		// Token: 0x04000A62 RID: 2658
		protected StackList statement_stack = new StackList();

		// Token: 0x04000A63 RID: 2659
		protected StackList l_value_stack = new StackList();

		// Token: 0x04000A64 RID: 2660
		protected StackList basic_l_value_stack = new StackList();

		// Token: 0x04000A65 RID: 2661
		protected StackList array_atom_stack = new StackList();

		// Token: 0x04000A66 RID: 2662
		protected StackList array_func_or_id_stack = new StackList();

		// Token: 0x04000A67 RID: 2663
		protected StackList func_or_id_stack = new StackList();

		// Token: 0x04000A68 RID: 2664
		protected StackList property_set_stack = new StackList();

		// Token: 0x04000A69 RID: 2665
		protected StackList ifBlock_stack = new StackList();

		// Token: 0x04000A6A RID: 2666
		protected StackList elseIfBlock_stack = new StackList();

		// Token: 0x04000A6B RID: 2667
		protected StackList elseBlock_stack = new StackList();

		// Token: 0x04000A6C RID: 2668
		protected StackList whileBlock_stack = new StackList();

		// Token: 0x04000A6D RID: 2669
		protected StackList function_call_stack = new StackList();

		// Token: 0x04000A6E RID: 2670
		protected StackList autoCast_stack = new StackList();

		// Token: 0x04000A6F RID: 2671
		protected PapyrusGen.DFA27 dfa27;

		// Token: 0x04000A70 RID: 2672
		private static readonly string[] DFA27_transitionS = new string[]
		{
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
		private static readonly short[][] DFA27_transition = DFA.UnpackEncodedStringArray(PapyrusGen.DFA27_transitionS);

		// Token: 0x04000A78 RID: 2680
		public static readonly BitSet FOLLOW_OBJECT_in_script80 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000A79 RID: 2681
		public static readonly BitSet FOLLOW_header_in_script82 = new BitSet(new ulong[]
		{
			20266198323691752UL
		});

		// Token: 0x04000A7A RID: 2682
		public static readonly BitSet FOLLOW_definitionOrBlock_in_script84 = new BitSet(new ulong[]
		{
			20266198323691752UL
		});

		// Token: 0x04000A7B RID: 2683
		public static readonly BitSet FOLLOW_ID_in_header224 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000A7C RID: 2684
		public static readonly BitSet FOLLOW_USER_FLAGS_in_header226 = new BitSet(new ulong[]
		{
			1374389534728UL
		});

		// Token: 0x04000A7D RID: 2685
		public static readonly BitSet FOLLOW_ID_in_header230 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x04000A7E RID: 2686
		public static readonly BitSet FOLLOW_DOCSTRING_in_header233 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000A7F RID: 2687
		public static readonly BitSet FOLLOW_fieldDefinition_in_definitionOrBlock253 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000A80 RID: 2688
		public static readonly BitSet FOLLOW_function_in_definitionOrBlock264 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000A81 RID: 2689
		public static readonly BitSet FOLLOW_eventFunc_in_definitionOrBlock277 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000A82 RID: 2690
		public static readonly BitSet FOLLOW_stateBlock_in_definitionOrBlock289 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000A83 RID: 2691
		public static readonly BitSet FOLLOW_propertyBlock_in_definitionOrBlock295 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000A84 RID: 2692
		public static readonly BitSet FOLLOW_VAR_in_fieldDefinition323 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000A85 RID: 2693
		public static readonly BitSet FOLLOW_type_in_fieldDefinition325 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000A86 RID: 2694
		public static readonly BitSet FOLLOW_ID_in_fieldDefinition329 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x04000A87 RID: 2695
		public static readonly BitSet FOLLOW_USER_FLAGS_in_fieldDefinition331 = new BitSet(new ulong[]
		{
			8UL,
			1006764032UL
		});

		// Token: 0x04000A88 RID: 2696
		public static readonly BitSet FOLLOW_constant_in_fieldDefinition333 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000A89 RID: 2697
		public static readonly BitSet FOLLOW_FUNCTION_in_function408 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000A8A RID: 2698
		public static readonly BitSet FOLLOW_functionHeader_in_function410 = new BitSet(new ulong[]
		{
			1032UL
		});

		// Token: 0x04000A8B RID: 2699
		public static readonly BitSet FOLLOW_codeBlock_in_function412 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000A8C RID: 2700
		public static readonly BitSet FOLLOW_HEADER_in_functionHeader504 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000A8D RID: 2701
		public static readonly BitSet FOLLOW_type_in_functionHeader507 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000A8E RID: 2702
		public static readonly BitSet FOLLOW_NONE_in_functionHeader511 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000A8F RID: 2703
		public static readonly BitSet FOLLOW_ID_in_functionHeader516 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x04000A90 RID: 2704
		public static readonly BitSet FOLLOW_USER_FLAGS_in_functionHeader518 = new BitSet(new ulong[]
		{
			212205744161288UL
		});

		// Token: 0x04000A91 RID: 2705
		public static readonly BitSet FOLLOW_callParameters_in_functionHeader520 = new BitSet(new ulong[]
		{
			212205744160776UL
		});

		// Token: 0x04000A92 RID: 2706
		public static readonly BitSet FOLLOW_functionModifier_in_functionHeader523 = new BitSet(new ulong[]
		{
			212205744160776UL
		});

		// Token: 0x04000A93 RID: 2707
		public static readonly BitSet FOLLOW_DOCSTRING_in_functionHeader526 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000A94 RID: 2708
		public static readonly BitSet FOLLOW_NATIVE_in_functionModifier545 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000A95 RID: 2709
		public static readonly BitSet FOLLOW_GLOBAL_in_functionModifier553 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000A96 RID: 2710
		public static readonly BitSet FOLLOW_EVENT_in_eventFunc588 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000A97 RID: 2711
		public static readonly BitSet FOLLOW_eventHeader_in_eventFunc590 = new BitSet(new ulong[]
		{
			1032UL
		});

		// Token: 0x04000A98 RID: 2712
		public static readonly BitSet FOLLOW_codeBlock_in_eventFunc592 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000A99 RID: 2713
		public static readonly BitSet FOLLOW_HEADER_in_eventHeader684 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000A9A RID: 2714
		public static readonly BitSet FOLLOW_NONE_in_eventHeader686 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000A9B RID: 2715
		public static readonly BitSet FOLLOW_ID_in_eventHeader688 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x04000A9C RID: 2716
		public static readonly BitSet FOLLOW_USER_FLAGS_in_eventHeader690 = new BitSet(new ulong[]
		{
			141836999983624UL
		});

		// Token: 0x04000A9D RID: 2717
		public static readonly BitSet FOLLOW_callParameters_in_eventHeader692 = new BitSet(new ulong[]
		{
			141836999983112UL
		});

		// Token: 0x04000A9E RID: 2718
		public static readonly BitSet FOLLOW_NATIVE_in_eventHeader695 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x04000A9F RID: 2719
		public static readonly BitSet FOLLOW_DOCSTRING_in_eventHeader698 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AA0 RID: 2720
		public static readonly BitSet FOLLOW_callParameter_in_callParameters725 = new BitSet(new ulong[]
		{
			514UL
		});

		// Token: 0x04000AA1 RID: 2721
		public static readonly BitSet FOLLOW_PARAM_in_callParameter742 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AA2 RID: 2722
		public static readonly BitSet FOLLOW_type_in_callParameter744 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000AA3 RID: 2723
		public static readonly BitSet FOLLOW_ID_in_callParameter748 = new BitSet(new ulong[]
		{
			8UL,
			1006764032UL
		});

		// Token: 0x04000AA4 RID: 2724
		public static readonly BitSet FOLLOW_constant_in_callParameter750 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AA5 RID: 2725
		public static readonly BitSet FOLLOW_STATE_in_stateBlock787 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AA6 RID: 2726
		public static readonly BitSet FOLLOW_ID_in_stateBlock789 = new BitSet(new ulong[]
		{
			1125899906842824UL
		});

		// Token: 0x04000AA7 RID: 2727
		public static readonly BitSet FOLLOW_AUTO_in_stateBlock791 = new BitSet(new ulong[]
		{
			200UL
		});

		// Token: 0x04000AA8 RID: 2728
		public static readonly BitSet FOLLOW_stateFuncOrEvent_in_stateBlock797 = new BitSet(new ulong[]
		{
			200UL
		});

		// Token: 0x04000AA9 RID: 2729
		public static readonly BitSet FOLLOW_function_in_stateFuncOrEvent819 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AAA RID: 2730
		public static readonly BitSet FOLLOW_eventFunc_in_stateFuncOrEvent832 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AAB RID: 2731
		public static readonly BitSet FOLLOW_PROPERTY_in_propertyBlock861 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AAC RID: 2732
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock863 = new BitSet(new ulong[]
		{
			131072UL
		});

		// Token: 0x04000AAD RID: 2733
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock867 = new BitSet(new ulong[]
		{
			131072UL
		});

		// Token: 0x04000AAE RID: 2734
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock872 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AAF RID: 2735
		public static readonly BitSet FOLLOW_AUTOPROP_in_propertyBlock922 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AB0 RID: 2736
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock924 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000AB1 RID: 2737
		public static readonly BitSet FOLLOW_ID_in_propertyBlock928 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AB2 RID: 2738
		public static readonly BitSet FOLLOW_HEADER_in_propertyHeader978 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AB3 RID: 2739
		public static readonly BitSet FOLLOW_type_in_propertyHeader980 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000AB4 RID: 2740
		public static readonly BitSet FOLLOW_ID_in_propertyHeader984 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x04000AB5 RID: 2741
		public static readonly BitSet FOLLOW_USER_FLAGS_in_propertyHeader986 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x04000AB6 RID: 2742
		public static readonly BitSet FOLLOW_DOCSTRING_in_propertyHeader988 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AB7 RID: 2743
		public static readonly BitSet FOLLOW_PROPFUNC_in_propertyFunc1009 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AB8 RID: 2744
		public static readonly BitSet FOLLOW_function_in_propertyFunc1011 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AB9 RID: 2745
		public static readonly BitSet FOLLOW_PROPFUNC_in_propertyFunc1025 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000ABA RID: 2746
		public static readonly BitSet FOLLOW_BLOCK_in_codeBlock1049 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000ABB RID: 2747
		public static readonly BitSet FOLLOW_statement_in_codeBlock1057 = new BitSet(new ulong[]
		{
			4611688629756016680UL,
			1025499646UL
		});

		// Token: 0x04000ABC RID: 2748
		public static readonly BitSet FOLLOW_localDefinition_in_statement1086 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000ABD RID: 2749
		public static readonly BitSet FOLLOW_EQUALS_in_statement1147 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000ABE RID: 2750
		public static readonly BitSet FOLLOW_ID_in_statement1149 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000ABF RID: 2751
		public static readonly BitSet FOLLOW_autoCast_in_statement1151 = new BitSet(new ulong[]
		{
			4611686293366126592UL
		});

		// Token: 0x04000AC0 RID: 2752
		public static readonly BitSet FOLLOW_l_value_in_statement1153 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AC1 RID: 2753
		public static readonly BitSet FOLLOW_expression_in_statement1155 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AC2 RID: 2754
		public static readonly BitSet FOLLOW_expression_in_statement1204 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AC3 RID: 2755
		public static readonly BitSet FOLLOW_return_stat_in_statement1215 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AC4 RID: 2756
		public static readonly BitSet FOLLOW_ifBlock_in_statement1226 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AC5 RID: 2757
		public static readonly BitSet FOLLOW_whileBlock_in_statement1237 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AC6 RID: 2758
		public static readonly BitSet FOLLOW_VAR_in_localDefinition1260 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AC7 RID: 2759
		public static readonly BitSet FOLLOW_type_in_localDefinition1262 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000AC8 RID: 2760
		public static readonly BitSet FOLLOW_ID_in_localDefinition1266 = new BitSet(new ulong[]
		{
			274877906952UL,
			1006796800UL
		});

		// Token: 0x04000AC9 RID: 2761
		public static readonly BitSet FOLLOW_autoCast_in_localDefinition1269 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000ACA RID: 2762
		public static readonly BitSet FOLLOW_expression_in_localDefinition1271 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000ACB RID: 2763
		public static readonly BitSet FOLLOW_DOT_in_l_value1318 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000ACC RID: 2764
		public static readonly BitSet FOLLOW_PAREXPR_in_l_value1321 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000ACD RID: 2765
		public static readonly BitSet FOLLOW_expression_in_l_value1325 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000ACE RID: 2766
		public static readonly BitSet FOLLOW_property_set_in_l_value1330 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000ACF RID: 2767
		public static readonly BitSet FOLLOW_ARRAYSET_in_l_value1355 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AD0 RID: 2768
		public static readonly BitSet FOLLOW_ID_in_l_value1359 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000AD1 RID: 2769
		public static readonly BitSet FOLLOW_ID_in_l_value1363 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AD2 RID: 2770
		public static readonly BitSet FOLLOW_autoCast_in_l_value1365 = new BitSet(new ulong[]
		{
			32768UL
		});

		// Token: 0x04000AD3 RID: 2771
		public static readonly BitSet FOLLOW_PAREXPR_in_l_value1368 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AD4 RID: 2772
		public static readonly BitSet FOLLOW_expression_in_l_value1372 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AD5 RID: 2773
		public static readonly BitSet FOLLOW_expression_in_l_value1377 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AD6 RID: 2774
		public static readonly BitSet FOLLOW_basic_l_value_in_l_value1431 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AD7 RID: 2775
		public static readonly BitSet FOLLOW_DOT_in_basic_l_value1454 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AD8 RID: 2776
		public static readonly BitSet FOLLOW_array_func_or_id_in_basic_l_value1458 = new BitSet(new ulong[]
		{
			4611686293366126592UL
		});

		// Token: 0x04000AD9 RID: 2777
		public static readonly BitSet FOLLOW_basic_l_value_in_basic_l_value1462 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000ADA RID: 2778
		public static readonly BitSet FOLLOW_function_call_in_basic_l_value1486 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000ADB RID: 2779
		public static readonly BitSet FOLLOW_property_set_in_basic_l_value1497 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000ADC RID: 2780
		public static readonly BitSet FOLLOW_ARRAYSET_in_basic_l_value1509 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000ADD RID: 2781
		public static readonly BitSet FOLLOW_ID_in_basic_l_value1513 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000ADE RID: 2782
		public static readonly BitSet FOLLOW_ID_in_basic_l_value1517 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000ADF RID: 2783
		public static readonly BitSet FOLLOW_autoCast_in_basic_l_value1519 = new BitSet(new ulong[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000AE0 RID: 2784
		public static readonly BitSet FOLLOW_func_or_id_in_basic_l_value1521 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AE1 RID: 2785
		public static readonly BitSet FOLLOW_expression_in_basic_l_value1523 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AE2 RID: 2786
		public static readonly BitSet FOLLOW_ID_in_basic_l_value1577 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AE3 RID: 2787
		public static readonly BitSet FOLLOW_OR_in_expression1595 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AE4 RID: 2788
		public static readonly BitSet FOLLOW_ID_in_expression1597 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AE5 RID: 2789
		public static readonly BitSet FOLLOW_expression_in_expression1601 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AE6 RID: 2790
		public static readonly BitSet FOLLOW_and_expression_in_expression1605 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AE7 RID: 2791
		public static readonly BitSet FOLLOW_and_expression_in_expression1659 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AE8 RID: 2792
		public static readonly BitSet FOLLOW_AND_in_and_expression1681 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AE9 RID: 2793
		public static readonly BitSet FOLLOW_ID_in_and_expression1683 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AEA RID: 2794
		public static readonly BitSet FOLLOW_and_expression_in_and_expression1687 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AEB RID: 2795
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression1691 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AEC RID: 2796
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression1745 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000AED RID: 2797
		public static readonly BitSet FOLLOW_EQ_in_bool_expression1767 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AEE RID: 2798
		public static readonly BitSet FOLLOW_ID_in_bool_expression1769 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AEF RID: 2799
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1773 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AF0 RID: 2800
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1777 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AF1 RID: 2801
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1781 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AF2 RID: 2802
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1785 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AF3 RID: 2803
		public static readonly BitSet FOLLOW_NE_in_bool_expression1850 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AF4 RID: 2804
		public static readonly BitSet FOLLOW_ID_in_bool_expression1852 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AF5 RID: 2805
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1856 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AF6 RID: 2806
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1860 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AF7 RID: 2807
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1864 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AF8 RID: 2808
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1868 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AF9 RID: 2809
		public static readonly BitSet FOLLOW_GT_in_bool_expression1928 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000AFA RID: 2810
		public static readonly BitSet FOLLOW_ID_in_bool_expression1930 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AFB RID: 2811
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1934 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000AFC RID: 2812
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1938 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AFD RID: 2813
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1942 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000AFE RID: 2814
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1946 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000AFF RID: 2815
		public static readonly BitSet FOLLOW_LT_in_bool_expression2011 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B00 RID: 2816
		public static readonly BitSet FOLLOW_ID_in_bool_expression2013 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B01 RID: 2817
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2017 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B02 RID: 2818
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2021 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B03 RID: 2819
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression2025 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B04 RID: 2820
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2029 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B05 RID: 2821
		public static readonly BitSet FOLLOW_GTE_in_bool_expression2094 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B06 RID: 2822
		public static readonly BitSet FOLLOW_ID_in_bool_expression2096 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B07 RID: 2823
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2100 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B08 RID: 2824
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2104 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B09 RID: 2825
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression2108 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B0A RID: 2826
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2112 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B0B RID: 2827
		public static readonly BitSet FOLLOW_LTE_in_bool_expression2177 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B0C RID: 2828
		public static readonly BitSet FOLLOW_ID_in_bool_expression2179 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B0D RID: 2829
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2183 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B0E RID: 2830
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression2187 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B0F RID: 2831
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression2191 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B10 RID: 2832
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2195 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B11 RID: 2833
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression2259 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B12 RID: 2834
		public static readonly BitSet FOLLOW_IADD_in_add_expression2281 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B13 RID: 2835
		public static readonly BitSet FOLLOW_ID_in_add_expression2283 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B14 RID: 2836
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2287 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B15 RID: 2837
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2291 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B16 RID: 2838
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2295 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B17 RID: 2839
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2299 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B18 RID: 2840
		public static readonly BitSet FOLLOW_FADD_in_add_expression2364 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B19 RID: 2841
		public static readonly BitSet FOLLOW_ID_in_add_expression2366 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B1A RID: 2842
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2370 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B1B RID: 2843
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2374 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B1C RID: 2844
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2378 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B1D RID: 2845
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2382 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B1E RID: 2846
		public static readonly BitSet FOLLOW_ISUBTRACT_in_add_expression2447 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B1F RID: 2847
		public static readonly BitSet FOLLOW_ID_in_add_expression2449 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B20 RID: 2848
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2453 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B21 RID: 2849
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2457 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B22 RID: 2850
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2461 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B23 RID: 2851
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2465 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B24 RID: 2852
		public static readonly BitSet FOLLOW_FSUBTRACT_in_add_expression2530 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B25 RID: 2853
		public static readonly BitSet FOLLOW_ID_in_add_expression2532 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B26 RID: 2854
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2536 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B27 RID: 2855
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2540 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B28 RID: 2856
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2544 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B29 RID: 2857
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2548 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B2A RID: 2858
		public static readonly BitSet FOLLOW_STRCAT_in_add_expression2613 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B2B RID: 2859
		public static readonly BitSet FOLLOW_ID_in_add_expression2615 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B2C RID: 2860
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2619 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B2D RID: 2861
		public static readonly BitSet FOLLOW_autoCast_in_add_expression2623 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B2E RID: 2862
		public static readonly BitSet FOLLOW_add_expression_in_add_expression2627 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B2F RID: 2863
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2631 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B30 RID: 2864
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression2695 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B31 RID: 2865
		public static readonly BitSet FOLLOW_IMULTIPLY_in_mult_expression2718 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B32 RID: 2866
		public static readonly BitSet FOLLOW_ID_in_mult_expression2720 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B33 RID: 2867
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2724 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B34 RID: 2868
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2728 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B35 RID: 2869
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2732 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B36 RID: 2870
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2736 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B37 RID: 2871
		public static readonly BitSet FOLLOW_FMULTIPLY_in_mult_expression2801 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B38 RID: 2872
		public static readonly BitSet FOLLOW_ID_in_mult_expression2803 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B39 RID: 2873
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2807 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B3A RID: 2874
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2811 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B3B RID: 2875
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2815 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B3C RID: 2876
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2819 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B3D RID: 2877
		public static readonly BitSet FOLLOW_IDIVIDE_in_mult_expression2884 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B3E RID: 2878
		public static readonly BitSet FOLLOW_ID_in_mult_expression2886 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B3F RID: 2879
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2890 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B40 RID: 2880
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2894 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B41 RID: 2881
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2898 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B42 RID: 2882
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2902 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B43 RID: 2883
		public static readonly BitSet FOLLOW_FDIVIDE_in_mult_expression2967 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B44 RID: 2884
		public static readonly BitSet FOLLOW_ID_in_mult_expression2969 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B45 RID: 2885
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2973 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B46 RID: 2886
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression2977 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B47 RID: 2887
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2981 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B48 RID: 2888
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2985 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B49 RID: 2889
		public static readonly BitSet FOLLOW_MOD_in_mult_expression3050 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B4A RID: 2890
		public static readonly BitSet FOLLOW_ID_in_mult_expression3052 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B4B RID: 2891
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression3056 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B4C RID: 2892
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression3060 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B4D RID: 2893
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression3114 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B4E RID: 2894
		public static readonly BitSet FOLLOW_INEGATE_in_unary_expression3137 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B4F RID: 2895
		public static readonly BitSet FOLLOW_ID_in_unary_expression3139 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B50 RID: 2896
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression3141 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B51 RID: 2897
		public static readonly BitSet FOLLOW_FNEGATE_in_unary_expression3186 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B52 RID: 2898
		public static readonly BitSet FOLLOW_ID_in_unary_expression3188 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B53 RID: 2899
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression3190 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B54 RID: 2900
		public static readonly BitSet FOLLOW_NOT_in_unary_expression3235 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B55 RID: 2901
		public static readonly BitSet FOLLOW_ID_in_unary_expression3237 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B56 RID: 2902
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression3239 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B57 RID: 2903
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression3283 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B58 RID: 2904
		public static readonly BitSet FOLLOW_AS_in_cast_atom3306 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B59 RID: 2905
		public static readonly BitSet FOLLOW_ID_in_cast_atom3308 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B5A RID: 2906
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom3310 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B5B RID: 2907
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom3349 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B5C RID: 2908
		public static readonly BitSet FOLLOW_DOT_in_dot_atom3372 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B5D RID: 2909
		public static readonly BitSet FOLLOW_dot_atom_in_dot_atom3376 = new BitSet(new ulong[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000B5E RID: 2910
		public static readonly BitSet FOLLOW_array_func_or_id_in_dot_atom3380 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B5F RID: 2911
		public static readonly BitSet FOLLOW_array_atom_in_dot_atom3409 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B60 RID: 2912
		public static readonly BitSet FOLLOW_constant_in_dot_atom3420 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B61 RID: 2913
		public static readonly BitSet FOLLOW_ARRAYGET_in_array_atom3447 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B62 RID: 2914
		public static readonly BitSet FOLLOW_ID_in_array_atom3451 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B63 RID: 2915
		public static readonly BitSet FOLLOW_ID_in_array_atom3455 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B64 RID: 2916
		public static readonly BitSet FOLLOW_autoCast_in_array_atom3457 = new BitSet(new ulong[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000B65 RID: 2917
		public static readonly BitSet FOLLOW_atom_in_array_atom3459 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B66 RID: 2918
		public static readonly BitSet FOLLOW_expression_in_array_atom3461 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B67 RID: 2919
		public static readonly BitSet FOLLOW_atom_in_array_atom3515 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B68 RID: 2920
		public static readonly BitSet FOLLOW_PAREXPR_in_atom3538 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B69 RID: 2921
		public static readonly BitSet FOLLOW_expression_in_atom3540 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B6A RID: 2922
		public static readonly BitSet FOLLOW_NEW_in_atom3553 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B6B RID: 2923
		public static readonly BitSet FOLLOW_INTEGER_in_atom3555 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B6C RID: 2924
		public static readonly BitSet FOLLOW_ID_in_atom3559 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B6D RID: 2925
		public static readonly BitSet FOLLOW_func_or_id_in_atom3593 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B6E RID: 2926
		public static readonly BitSet FOLLOW_ARRAYGET_in_array_func_or_id3620 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B6F RID: 2927
		public static readonly BitSet FOLLOW_ID_in_array_func_or_id3624 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B70 RID: 2928
		public static readonly BitSet FOLLOW_ID_in_array_func_or_id3628 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000B71 RID: 2929
		public static readonly BitSet FOLLOW_autoCast_in_array_func_or_id3630 = new BitSet(new ulong[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000B72 RID: 2930
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id3632 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B73 RID: 2931
		public static readonly BitSet FOLLOW_expression_in_array_func_or_id3634 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B74 RID: 2932
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id3688 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B75 RID: 2933
		public static readonly BitSet FOLLOW_function_call_in_func_or_id3714 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B76 RID: 2934
		public static readonly BitSet FOLLOW_PROPGET_in_func_or_id3726 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B77 RID: 2935
		public static readonly BitSet FOLLOW_ID_in_func_or_id3730 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B78 RID: 2936
		public static readonly BitSet FOLLOW_ID_in_func_or_id3734 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B79 RID: 2937
		public static readonly BitSet FOLLOW_ID_in_func_or_id3738 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B7A RID: 2938
		public static readonly BitSet FOLLOW_ID_in_func_or_id3777 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B7B RID: 2939
		public static readonly BitSet FOLLOW_LENGTH_in_func_or_id3789 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B7C RID: 2940
		public static readonly BitSet FOLLOW_ID_in_func_or_id3793 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B7D RID: 2941
		public static readonly BitSet FOLLOW_ID_in_func_or_id3797 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B7E RID: 2942
		public static readonly BitSet FOLLOW_PROPSET_in_property_set3843 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B7F RID: 2943
		public static readonly BitSet FOLLOW_ID_in_property_set3847 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B80 RID: 2944
		public static readonly BitSet FOLLOW_ID_in_property_set3851 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B81 RID: 2945
		public static readonly BitSet FOLLOW_ID_in_property_set3855 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B82 RID: 2946
		public static readonly BitSet FOLLOW_RETURN_in_return_stat3903 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B83 RID: 2947
		public static readonly BitSet FOLLOW_autoCast_in_return_stat3905 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000B84 RID: 2948
		public static readonly BitSet FOLLOW_expression_in_return_stat3907 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B85 RID: 2949
		public static readonly BitSet FOLLOW_RETURN_in_return_stat3940 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000B86 RID: 2950
		public static readonly BitSet FOLLOW_IF_in_ifBlock3984 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B87 RID: 2951
		public static readonly BitSet FOLLOW_expression_in_ifBlock3986 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x04000B88 RID: 2952
		public static readonly BitSet FOLLOW_codeBlock_in_ifBlock3988 = new BitSet(new ulong[]
		{
			8UL,
			12582912UL
		});

		// Token: 0x04000B89 RID: 2953
		public static readonly BitSet FOLLOW_elseIfBlock_in_ifBlock3994 = new BitSet(new ulong[]
		{
			8UL,
			12582912UL
		});

		// Token: 0x04000B8A RID: 2954
		public static readonly BitSet FOLLOW_elseBlock_in_ifBlock3998 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B8B RID: 2955
		public static readonly BitSet FOLLOW_ELSEIF_in_elseIfBlock4072 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B8C RID: 2956
		public static readonly BitSet FOLLOW_expression_in_elseIfBlock4074 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x04000B8D RID: 2957
		public static readonly BitSet FOLLOW_codeBlock_in_elseIfBlock4076 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B8E RID: 2958
		public static readonly BitSet FOLLOW_ELSE_in_elseBlock4140 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B8F RID: 2959
		public static readonly BitSet FOLLOW_codeBlock_in_elseBlock4142 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B90 RID: 2960
		public static readonly BitSet FOLLOW_WHILE_in_whileBlock4183 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B91 RID: 2961
		public static readonly BitSet FOLLOW_expression_in_whileBlock4185 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x04000B92 RID: 2962
		public static readonly BitSet FOLLOW_codeBlock_in_whileBlock4187 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B93 RID: 2963
		public static readonly BitSet FOLLOW_CALL_in_function_call4252 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B94 RID: 2964
		public static readonly BitSet FOLLOW_ID_in_function_call4256 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B95 RID: 2965
		public static readonly BitSet FOLLOW_ID_in_function_call4260 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B96 RID: 2966
		public static readonly BitSet FOLLOW_ID_in_function_call4264 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x04000B97 RID: 2967
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4267 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B98 RID: 2968
		public static readonly BitSet FOLLOW_parameters_in_function_call4269 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B99 RID: 2969
		public static readonly BitSet FOLLOW_CALLPARENT_in_function_call4355 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B9A RID: 2970
		public static readonly BitSet FOLLOW_ID_in_function_call4359 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B9B RID: 2971
		public static readonly BitSet FOLLOW_ID_in_function_call4363 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000B9C RID: 2972
		public static readonly BitSet FOLLOW_ID_in_function_call4367 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x04000B9D RID: 2973
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4370 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000B9E RID: 2974
		public static readonly BitSet FOLLOW_parameters_in_function_call4372 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000B9F RID: 2975
		public static readonly BitSet FOLLOW_CALLGLOBAL_in_function_call4448 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BA0 RID: 2976
		public static readonly BitSet FOLLOW_ID_in_function_call4452 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000BA1 RID: 2977
		public static readonly BitSet FOLLOW_ID_in_function_call4456 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000BA2 RID: 2978
		public static readonly BitSet FOLLOW_ID_in_function_call4460 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x04000BA3 RID: 2979
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4463 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BA4 RID: 2980
		public static readonly BitSet FOLLOW_parameters_in_function_call4465 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000BA5 RID: 2981
		public static readonly BitSet FOLLOW_ARRAYFIND_in_function_call4551 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BA6 RID: 2982
		public static readonly BitSet FOLLOW_ID_in_function_call4555 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000BA7 RID: 2983
		public static readonly BitSet FOLLOW_ID_in_function_call4559 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x04000BA8 RID: 2984
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4562 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BA9 RID: 2985
		public static readonly BitSet FOLLOW_parameters_in_function_call4564 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000BAA RID: 2986
		public static readonly BitSet FOLLOW_ARRAYRFIND_in_function_call4640 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BAB RID: 2987
		public static readonly BitSet FOLLOW_ID_in_function_call4644 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000BAC RID: 2988
		public static readonly BitSet FOLLOW_ID_in_function_call4648 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x04000BAD RID: 2989
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call4651 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BAE RID: 2990
		public static readonly BitSet FOLLOW_parameters_in_function_call4653 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000BAF RID: 2991
		public static readonly BitSet FOLLOW_parameter_in_parameters4751 = new BitSet(new ulong[]
		{
			514UL
		});

		// Token: 0x04000BB0 RID: 2992
		public static readonly BitSet FOLLOW_PARAM_in_parameter4793 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BB1 RID: 2993
		public static readonly BitSet FOLLOW_autoCast_in_parameter4795 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000BB2 RID: 2994
		public static readonly BitSet FOLLOW_expression_in_parameter4797 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000BB3 RID: 2995
		public static readonly BitSet FOLLOW_AS_in_autoCast4825 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BB4 RID: 2996
		public static readonly BitSet FOLLOW_ID_in_autoCast4829 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000BB5 RID: 2997
		public static readonly BitSet FOLLOW_ID_in_autoCast4833 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000BB6 RID: 2998
		public static readonly BitSet FOLLOW_AS_in_autoCast4868 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000BB7 RID: 2999
		public static readonly BitSet FOLLOW_ID_in_autoCast4870 = new BitSet(new ulong[]
		{
			0UL,
			1006764032UL
		});

		// Token: 0x04000BB8 RID: 3000
		public static readonly BitSet FOLLOW_constant_in_autoCast4872 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000BB9 RID: 3001
		public static readonly BitSet FOLLOW_ID_in_autoCast4906 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BBA RID: 3002
		public static readonly BitSet FOLLOW_constant_in_autoCast4917 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BBB RID: 3003
		public static readonly BitSet FOLLOW_number_in_constant4935 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BBC RID: 3004
		public static readonly BitSet FOLLOW_STRING_in_constant4941 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BBD RID: 3005
		public static readonly BitSet FOLLOW_BOOL_in_constant4947 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BBE RID: 3006
		public static readonly BitSet FOLLOW_NONE_in_constant4953 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BBF RID: 3007
		public static readonly BitSet FOLLOW_set_in_number4964 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BC0 RID: 3008
		public static readonly BitSet FOLLOW_ID_in_type4985 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BC1 RID: 3009
		public static readonly BitSet FOLLOW_ID_in_type4996 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000BC2 RID: 3010
		public static readonly BitSet FOLLOW_LBRACKET_in_type4998 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000BC3 RID: 3011
		public static readonly BitSet FOLLOW_RBRACKET_in_type5000 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BC4 RID: 3012
		public static readonly BitSet FOLLOW_BASETYPE_in_type5011 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000BC5 RID: 3013
		public static readonly BitSet FOLLOW_BASETYPE_in_type5022 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x04000BC6 RID: 3014
		public static readonly BitSet FOLLOW_LBRACKET_in_type5024 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x04000BC7 RID: 3015
		public static readonly BitSet FOLLOW_RBRACKET_in_type5026 = new BitSet(new ulong[]
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
			public PapyrusGen.STAttrMap Add(string attrName, object value)
			{
				base.Add(attrName, value);
				return this;
			}

			// Token: 0x06000DB3 RID: 3507 RVA: 0x0006B610 File Offset: 0x00069810
			public PapyrusGen.STAttrMap Add(string attrName, int value)
			{
				base.Add(attrName, value);
				return this;
			}
		}

		// Token: 0x020001C6 RID: 454
		protected class script_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001BB RID: 443
			// (get) Token: 0x06000DB8 RID: 3512 RVA: 0x0006B644 File Offset: 0x00069844
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DB9 RID: 3513 RVA: 0x0006B64C File Offset: 0x0006984C
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001BD RID: 445
			// (get) Token: 0x06000DBD RID: 3517 RVA: 0x0006B680 File Offset: 0x00069880
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DBE RID: 3518 RVA: 0x0006B688 File Offset: 0x00069888
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001BF RID: 447
			// (get) Token: 0x06000DC2 RID: 3522 RVA: 0x0006B6BC File Offset: 0x000698BC
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DC3 RID: 3523 RVA: 0x0006B6C4 File Offset: 0x000698C4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

			// Token: 0x04000BDE RID: 3038
			private StringTemplate st;
		}

		// Token: 0x020001CA RID: 458
		protected class fieldDefinition_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001C1 RID: 449
			// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x0006B700 File Offset: 0x00069900
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DC9 RID: 3529 RVA: 0x0006B708 File Offset: 0x00069908
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

			// Token: 0x04000BE0 RID: 3040
			private StringTemplate st;
		}

		// Token: 0x020001CC RID: 460
		protected class function_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001C3 RID: 451
			// (get) Token: 0x06000DCE RID: 3534 RVA: 0x0006B744 File Offset: 0x00069944
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DCF RID: 3535 RVA: 0x0006B74C File Offset: 0x0006994C
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001C5 RID: 453
			// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x0006B780 File Offset: 0x00069980
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DD4 RID: 3540 RVA: 0x0006B788 File Offset: 0x00069988
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001C7 RID: 455
			// (get) Token: 0x06000DD8 RID: 3544 RVA: 0x0006B7BC File Offset: 0x000699BC
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DD9 RID: 3545 RVA: 0x0006B7C4 File Offset: 0x000699C4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

			// Token: 0x04000BF0 RID: 3056
			private StringTemplate st;
		}

		// Token: 0x020001D0 RID: 464
		protected class eventFunc_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001C9 RID: 457
			// (get) Token: 0x06000DDE RID: 3550 RVA: 0x0006B800 File Offset: 0x00069A00
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DDF RID: 3551 RVA: 0x0006B808 File Offset: 0x00069A08
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001CB RID: 459
			// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x0006B83C File Offset: 0x00069A3C
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DE4 RID: 3556 RVA: 0x0006B844 File Offset: 0x00069A44
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001CD RID: 461
			// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x0006B878 File Offset: 0x00069A78
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DE9 RID: 3561 RVA: 0x0006B880 File Offset: 0x00069A80
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001CF RID: 463
			// (get) Token: 0x06000DED RID: 3565 RVA: 0x0006B8B4 File Offset: 0x00069AB4
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DEE RID: 3566 RVA: 0x0006B8BC File Offset: 0x00069ABC
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001D1 RID: 465
			// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x0006B8F0 File Offset: 0x00069AF0
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DF3 RID: 3571 RVA: 0x0006B8F8 File Offset: 0x00069AF8
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001D3 RID: 467
			// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x0006B92C File Offset: 0x00069B2C
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DF8 RID: 3576 RVA: 0x0006B934 File Offset: 0x00069B34
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

			// Token: 0x04000C03 RID: 3075
			private StringTemplate st;
		}

		// Token: 0x020001D7 RID: 471
		protected class propertyBlock_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001D5 RID: 469
			// (get) Token: 0x06000DFD RID: 3581 RVA: 0x0006B970 File Offset: 0x00069B70
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000DFE RID: 3582 RVA: 0x0006B978 File Offset: 0x00069B78
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001D7 RID: 471
			// (get) Token: 0x06000E02 RID: 3586 RVA: 0x0006B9AC File Offset: 0x00069BAC
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E03 RID: 3587 RVA: 0x0006B9B4 File Offset: 0x00069BB4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001D9 RID: 473
			// (get) Token: 0x06000E07 RID: 3591 RVA: 0x0006B9E8 File Offset: 0x00069BE8
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E08 RID: 3592 RVA: 0x0006B9F0 File Offset: 0x00069BF0
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

			// Token: 0x04000C0A RID: 3082
			private StringTemplate st;
		}

		// Token: 0x020001DB RID: 475
		protected class codeBlock_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001DB RID: 475
			// (get) Token: 0x06000E0D RID: 3597 RVA: 0x0006BA2C File Offset: 0x00069C2C
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E0E RID: 3598 RVA: 0x0006BA34 File Offset: 0x00069C34
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001DD RID: 477
			// (get) Token: 0x06000E13 RID: 3603 RVA: 0x0006BA70 File Offset: 0x00069C70
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E14 RID: 3604 RVA: 0x0006BA78 File Offset: 0x00069C78
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001DF RID: 479
			// (get) Token: 0x06000E18 RID: 3608 RVA: 0x0006BAAC File Offset: 0x00069CAC
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E19 RID: 3609 RVA: 0x0006BAB4 File Offset: 0x00069CB4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
		protected class l_value_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001E1 RID: 481
			// (get) Token: 0x06000E1E RID: 3614 RVA: 0x0006BAF0 File Offset: 0x00069CF0
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E1F RID: 3615 RVA: 0x0006BAF8 File Offset: 0x00069CF8
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

			// Token: 0x04000C19 RID: 3097
			private StringTemplate st;
		}

		// Token: 0x020001E2 RID: 482
		protected class basic_l_value_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001E3 RID: 483
			// (get) Token: 0x06000E24 RID: 3620 RVA: 0x0006BB34 File Offset: 0x00069D34
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E25 RID: 3621 RVA: 0x0006BB3C File Offset: 0x00069D3C
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001E5 RID: 485
			// (get) Token: 0x06000E29 RID: 3625 RVA: 0x0006BB70 File Offset: 0x00069D70
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E2A RID: 3626 RVA: 0x0006BB78 File Offset: 0x00069D78
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001E7 RID: 487
			// (get) Token: 0x06000E2E RID: 3630 RVA: 0x0006BBAC File Offset: 0x00069DAC
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E2F RID: 3631 RVA: 0x0006BBB4 File Offset: 0x00069DB4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001E9 RID: 489
			// (get) Token: 0x06000E33 RID: 3635 RVA: 0x0006BBE8 File Offset: 0x00069DE8
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E34 RID: 3636 RVA: 0x0006BBF0 File Offset: 0x00069DF0
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001EB RID: 491
			// (get) Token: 0x06000E38 RID: 3640 RVA: 0x0006BC24 File Offset: 0x00069E24
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E39 RID: 3641 RVA: 0x0006BC2C File Offset: 0x00069E2C
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001ED RID: 493
			// (get) Token: 0x06000E3D RID: 3645 RVA: 0x0006BC60 File Offset: 0x00069E60
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E3E RID: 3646 RVA: 0x0006BC68 File Offset: 0x00069E68
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001EF RID: 495
			// (get) Token: 0x06000E42 RID: 3650 RVA: 0x0006BC9C File Offset: 0x00069E9C
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E43 RID: 3651 RVA: 0x0006BCA4 File Offset: 0x00069EA4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001F1 RID: 497
			// (get) Token: 0x06000E47 RID: 3655 RVA: 0x0006BCD8 File Offset: 0x00069ED8
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E48 RID: 3656 RVA: 0x0006BCE0 File Offset: 0x00069EE0
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001F3 RID: 499
			// (get) Token: 0x06000E4C RID: 3660 RVA: 0x0006BD14 File Offset: 0x00069F14
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E4D RID: 3661 RVA: 0x0006BD1C File Offset: 0x00069F1C
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001F5 RID: 501
			// (get) Token: 0x06000E52 RID: 3666 RVA: 0x0006BD58 File Offset: 0x00069F58
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E53 RID: 3667 RVA: 0x0006BD60 File Offset: 0x00069F60
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001F7 RID: 503
			// (get) Token: 0x06000E57 RID: 3671 RVA: 0x0006BD94 File Offset: 0x00069F94
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E58 RID: 3672 RVA: 0x0006BD9C File Offset: 0x00069F9C
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001F9 RID: 505
			// (get) Token: 0x06000E5D RID: 3677 RVA: 0x0006BDD8 File Offset: 0x00069FD8
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E5E RID: 3678 RVA: 0x0006BDE0 File Offset: 0x00069FE0
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001FB RID: 507
			// (get) Token: 0x06000E63 RID: 3683 RVA: 0x0006BE1C File Offset: 0x0006A01C
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E64 RID: 3684 RVA: 0x0006BE24 File Offset: 0x0006A024
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001FD RID: 509
			// (get) Token: 0x06000E69 RID: 3689 RVA: 0x0006BE60 File Offset: 0x0006A060
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E6A RID: 3690 RVA: 0x0006BE68 File Offset: 0x0006A068
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x170001FF RID: 511
			// (get) Token: 0x06000E6E RID: 3694 RVA: 0x0006BE9C File Offset: 0x0006A09C
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E6F RID: 3695 RVA: 0x0006BEA4 File Offset: 0x0006A0A4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x17000201 RID: 513
			// (get) Token: 0x06000E74 RID: 3700 RVA: 0x0006BEE0 File Offset: 0x0006A0E0
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E75 RID: 3701 RVA: 0x0006BEE8 File Offset: 0x0006A0E8
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x17000203 RID: 515
			// (get) Token: 0x06000E7A RID: 3706 RVA: 0x0006BF24 File Offset: 0x0006A124
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E7B RID: 3707 RVA: 0x0006BF2C File Offset: 0x0006A12C
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

			// Token: 0x04000C42 RID: 3138
			private StringTemplate st;
		}

		// Token: 0x020001FA RID: 506
		protected class elseBlock_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x17000205 RID: 517
			// (get) Token: 0x06000E80 RID: 3712 RVA: 0x0006BF68 File Offset: 0x0006A168
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E81 RID: 3713 RVA: 0x0006BF70 File Offset: 0x0006A170
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

			// Token: 0x04000C45 RID: 3141
			private StringTemplate st;
		}

		// Token: 0x020001FC RID: 508
		protected class whileBlock_scope
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x17000207 RID: 519
			// (get) Token: 0x06000E86 RID: 3718 RVA: 0x0006BFAC File Offset: 0x0006A1AC
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E87 RID: 3719 RVA: 0x0006BFB4 File Offset: 0x0006A1B4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x17000209 RID: 521
			// (get) Token: 0x06000E8C RID: 3724 RVA: 0x0006BFF0 File Offset: 0x0006A1F0
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E8D RID: 3725 RVA: 0x0006BFF8 File Offset: 0x0006A1F8
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x1700020B RID: 523
			// (get) Token: 0x06000E91 RID: 3729 RVA: 0x0006C02C File Offset: 0x0006A22C
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E92 RID: 3730 RVA: 0x0006C034 File Offset: 0x0006A234
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x1700020D RID: 525
			// (get) Token: 0x06000E96 RID: 3734 RVA: 0x0006C068 File Offset: 0x0006A268
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E97 RID: 3735 RVA: 0x0006C070 File Offset: 0x0006A270
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x1700020F RID: 527
			// (get) Token: 0x06000E9C RID: 3740 RVA: 0x0006C0AC File Offset: 0x0006A2AC
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000E9D RID: 3741 RVA: 0x0006C0B4 File Offset: 0x0006A2B4
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x17000211 RID: 529
			// (get) Token: 0x06000EA1 RID: 3745 RVA: 0x0006C0E8 File Offset: 0x0006A2E8
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000EA2 RID: 3746 RVA: 0x0006C0F0 File Offset: 0x0006A2F0
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x17000213 RID: 531
			// (get) Token: 0x06000EA6 RID: 3750 RVA: 0x0006C124 File Offset: 0x0006A324
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000EA7 RID: 3751 RVA: 0x0006C12C File Offset: 0x0006A32C
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

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
				get
				{
					return this.st;
				}
				set
				{
					this.st = value;
				}
			}

			// Token: 0x17000215 RID: 533
			// (get) Token: 0x06000EAB RID: 3755 RVA: 0x0006C160 File Offset: 0x0006A360
			public override object Template
			{
				get
				{
					return this.st;
				}
			}

			// Token: 0x06000EAC RID: 3756 RVA: 0x0006C168 File Offset: 0x0006A368
			public override string ToString()
			{
				if (this.st != null)
				{
					return this.st.ToString();
				}
				return null;
			}

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
				this.decisionNumber = 27;
				this.eot = PapyrusGen.DFA27_eot;
				this.eof = PapyrusGen.DFA27_eof;
				this.min = PapyrusGen.DFA27_min;
				this.max = PapyrusGen.DFA27_max;
				this.accept = PapyrusGen.DFA27_accept;
				this.special = PapyrusGen.DFA27_special;
				this.transition = PapyrusGen.DFA27_transition;
			}

			// Token: 0x17000216 RID: 534
			// (get) Token: 0x06000EAF RID: 3759 RVA: 0x0006C1F8 File Offset: 0x0006A3F8
			public override string Description
			{
				get
				{
					return "437:1: l_value : ( ^( DOT ^( PAREXPR a= expression ) b= property_set ) -> dot(aTemplate=$a.stbTemplate=$b.st) | ^( ARRAYSET source= ID self= ID autoCast ^( PAREXPR array= expression ) index= expression ) -> arraySet(sourceName=$l_value::ssourceNameselfName=$l_value::sselfNameindex=$autoCast.sRetValueautoCast=$autoCast.starrayExpressions=$array.stindexExpressions=$index.stlineNo=$ARRAYSET.Line) | basic_l_value );";
				}
			}
		}
	}
}
