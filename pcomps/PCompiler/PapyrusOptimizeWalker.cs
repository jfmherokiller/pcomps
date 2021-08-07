using System;
using System.Globalization;
using pcomps.Antlr.Runtime;
using pcomps.Antlr.Runtime.Collections;
using pcomps.Antlr.Runtime.Tree;

namespace pcomps.PCompiler
{
	// Token: 0x02000138 RID: 312
	public class PapyrusOptimizeWalker : TreeParser
	{
		// Token: 0x06000AE2 RID: 2786 RVA: 0x000329E0 File Offset: 0x00030BE0
		private ITree DuplicateTree(ITree akOriginal)
		{
			var tree = akOriginal.DupNode();
			for (var i = 0; i < akOriginal.ChildCount; i++)
			{
				tree.AddChild(DuplicateTree(akOriginal.GetChild(i)));
			}
			return tree;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x00032A1C File Offset: 0x00030C1C
		private ITree FixUpAutoCast(ITree akOriginalAutoCast, ITree akOptimizedTree)
		{
			ITree tree;
			if (ePassType == OptimizePass.NORMAL && akOptimizedTree != null)
			{
				if (akOriginalAutoCast.Type == 79)
				{
					if (akOriginalAutoCast.GetChild(1).Text.ToLowerInvariant() != akOptimizedTree.Text.ToLowerInvariant())
					{
						tree = DuplicateTree(akOriginalAutoCast);
						tree.SetChild(1, akOptimizedTree);
						bMadeChanges = true;
					}
					else
					{
						tree = akOriginalAutoCast;
					}
				}
				else
				{
					tree = akOptimizedTree;
				}
			}
			else
			{
				tree = akOriginalAutoCast;
			}
			return tree;
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x00032A88 File Offset: 0x00030C88
		private void FixUpAutoCastAndExpression(ITree akOrigCast, ITree akOptCast, ITree akOrigTree, ITree akOptTree, out ITree akOutCast, out ITree akOutTree)
		{
			if (akOptCast != null)
			{
				akOutCast = akOptCast;
				akOutTree = akOptCast;
				return;
			}
			akOutCast = FixUpAutoCast(akOrigCast, akOptTree);
			if (akOptTree != null)
			{
				akOutTree = akOptTree;
				return;
			}
			akOutTree = akOrigTree;
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x00032AB0 File Offset: 0x00030CB0
		private bool TryConvertToBool(CommonTree akValue, out bool abValue)
		{
			abValue = false;
			var result = false;
			var type = akValue.Token.Type;
			if (type != 81)
			{
				switch (type)
				{
				case 90:
					result = true;
					abValue = (akValue.Token.Text != "\"\"");
					break;
				case 91:
					result = true;
					abValue = (akValue.Token.Text.ToLowerInvariant() == "true");
					break;
				case 92:
					result = true;
					break;
				case 93:
					result = true;
					abValue = (Math.Abs(float.Parse(akValue.Token.Text)) > float.Epsilon);
					break;
				}
			}
			else
			{
				result = true;
				abValue = (int.Parse(akValue.Token.Text) != 0);
			}
			return result;
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x00032B6C File Offset: 0x00030D6C
		private ITree CompilerAutoIntTwoArgMathOp(CommonTree akValue1, CommonTree akValue2, IToken akOperation)
		{
			var flag = akValue1.Token.Type == 81;
			var flag2 = akValue2.Token.Type == 81;
			var num = 0;
			var num2 = 0;
			var flag3 = false;
			var num3 = 0;
			if (flag)
			{
				num = int.Parse(akValue1.Token.Text);
			}
			if (flag2)
			{
				num2 = int.Parse(akValue2.Token.Text);
			}
			if (flag && flag2)
			{
				var type = akOperation.Type;
				switch (type)
				{
				case 26:
					flag3 = true;
					num3 = num + num2;
					break;
				case 27:
				case 29:
				case 31:
					break;
				case 28:
					flag3 = true;
					num3 = num - num2;
					break;
				case 30:
					flag3 = true;
					num3 = num * num2;
					break;
				case 32:
					flag3 = true;
					if (num2 == 0)
					{
						OnError("Cannot divide by 0", akOperation.Line, akOperation.CharPositionInLine);
					}
					else
					{
						num3 = num / num2;
					}
					break;
				default:
					if (type == 77)
					{
						flag3 = true;
						if (num2 == 0)
						{
							OnError("Cannot mod by 0", akOperation.Line, akOperation.CharPositionInLine);
						}
						else
						{
							num3 = num % num2;
						}
					}
					break;
				}
			}
			ITree result = null;
			if (flag3)
			{
				result = new CommonTree(new CommonToken(akOperation)
				{
					Type = 81,
					Text = num3.ToString()
				});
				bMadeChanges = true;
			}
			return result;
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x00032CB0 File Offset: 0x00030EB0
		private ITree CompilerAutoFloatTwoArgMathOp(CommonTree akValue1, CommonTree akValue2, IToken akOperation)
		{
			var flag = akValue1.Token.Type == 93;
			var flag2 = akValue2.Token.Type == 93;
			var num = 0f;
			var num2 = 0f;
			var flag3 = false;
			var num3 = 0f;
			if (flag)
			{
				num = float.Parse(akValue1.Token.Text);
			}
			if (flag2)
			{
				num2 = float.Parse(akValue2.Token.Text);
			}
			if (flag && flag2)
			{
				switch (akOperation.Type)
				{
				case 27:
					flag3 = true;
					num3 = num + num2;
					break;
				case 29:
					flag3 = true;
					num3 = num - num2;
					break;
				case 31:
					flag3 = true;
					num3 = num * num2;
					break;
				case 33:
					flag3 = true;
					if (Math.Abs(num2) < 1E-45f)
					{
						OnError("Cannot divide by 0", akOperation.Line, akOperation.CharPositionInLine);
					}
					else
					{
						num3 = num / num2;
					}
					break;
				}
			}
			ITree result = null;
			if (flag3)
			{
				IToken token = new CommonToken(akOperation);
				token.Type = 93;
				token.Text = num3.ToString(CultureInfo.InvariantCulture);
				if (!token.Text.Contains('.'))
				{
					var token2 = token;
					token2.Text += ".0";
				}
				result = new CommonTree(token);
				bMadeChanges = true;
			}
			return result;
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x00032E00 File Offset: 0x00031000
		private ITree CompilerAutoStrCat(CommonTree akValue1, CommonTree akValue2)
		{
			var flag = akValue1.Token.Type == 90;
			var flag2 = akValue2.Token.Type == 90;
			var arg = "";
			var arg2 = "";
			if (flag)
			{
				arg = akValue1.Token.Text.Substring(1, akValue1.Token.Text.Length - 2);
			}
			if (flag2)
			{
				arg2 = akValue2.Token.Text.Substring(1, akValue2.Token.Text.Length - 2);
			}

            if (!flag || !flag2) return null;
            ITree result = new CommonTree(new CommonToken(akValue1.Token)
            {
                Type = 90,
                Text = $"\"{arg}{arg2}\""
            });
            bMadeChanges = true;
            return result;
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x00032ECC File Offset: 0x000310CC
		private ITree CompilerAutoTwoArgBoolOp(CommonTree akValue1, CommonTree akValue2, IToken akOperation)
		{
            var flag3 = TryConvertToBool(akValue1, out var flag);
            var flag4 = TryConvertToBool(akValue2, out var flag2);
			var flag5 = false;
			var flag6 = false;
			if (flag3 && !flag4)
            {
                switch (akOperation.Type)
                {
                    case 66 when !flag:
                        flag5 = true;
                        flag6 = false;
                        break;
                    case 65 when flag:
                        flag5 = true;
                        flag6 = true;
                        break;
                }
            }
			if (!flag5 && flag3 && flag4)
			{
				switch (akOperation.Type)
				{
				case 65:
					flag5 = true;
					flag6 = (flag || flag2);
					break;
				case 66:
					flag5 = true;
					flag6 = (flag && flag2);
					break;
				}
			}
			ITree result;
            if (!flag5) return null;
            result = new CommonTree(new CommonToken(akOperation)
            {
                Type = 91,
                Text = (flag6 ? "True" : "False")
            });
            bMadeChanges = true;
            return result;
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x00032FA8 File Offset: 0x000311A8
		private ITree CompilerAutoNegate(CommonTree akValue)
		{
			IToken token = null;
			var type = akValue.Token.Type;
			if (type != 81)
			{
				if (type == 93)
				{
                    token = new CommonToken(akValue.Token)
                    {
                        Text = (-float.Parse(akValue.Token.Text)).ToString()
                    };
                    if (!token.Text.Contains('.'))
					{
						var token2 = token;
						token2.Text += ".0";
					}
				}
			}
			else
			{
                token = new CommonToken(akValue.Token)
                {
                    Text = (-int.Parse(akValue.Token.Text)).ToString()
                };
            }
			ITree result = null;
            if (token == null) return null;
            result = new CommonTree(token);
            bMadeChanges = true;
            return result;
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x00033064 File Offset: 0x00031264
		private ITree CompilerAutoNot(CommonTree akValue)
		{
            var flag2 = TryConvertToBool(akValue, out var flag);
            if (!flag2) return null;
            ITree result = new CommonTree(new CommonToken(akValue.Token)
            {
                Type = 91,
                Text = !flag ? "True" : "False"
            });
            bMadeChanges = true;
            return result;
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x000330BC File Offset: 0x000312BC
		private ITree CompilerAutoCast(string asDestVar, CommonTree akValue, ScriptScope akCurrentScope)
		{
			ITree result = null;
            if (!akCurrentScope.TryGetVariable(asDestVar, out var scriptVariableType)) return result;
            IToken token = null;
            switch (scriptVariableType.VarType)
            {
                case "int":
                {
                    var flag = false;
                    var num = 0;
                    switch (akValue.Token.Type)
                    {
                        case 90:
                            flag = true;
                            try
                            {
                                num = int.Parse(akValue.Token.Text[1..^1]);
                                goto IL_E5;
                            }
                            catch (Exception)
                            {
                                OnError("String cannot be cast to an integer", akValue.Token.Line, akValue.Token.CharPositionInLine);
                                goto IL_E5;
                            }
                        case 91:
                            break;
                        case 92:
                            goto IL_E5;
                        case 93:
                            flag = true;
                            num = (int)float.Parse(akValue.Token.Text);
                            goto IL_E5;
                        default:
                            goto IL_E5;
                    }
                    flag = true;
                    num = ((akValue.Token.Text.ToLowerInvariant() == "true") ? 1 : 0);
                    IL_E5:
                    if (flag)
                    {
                            token = new CommonToken(akValue.Token)
                            {
                                Type = 81,
                                Text = num.ToString()
                            };
                    }

                    break;
                }
                case "float":
                {
                    var flag2 = false;
                    var num2 = 0f;
                    var type = akValue.Token.Type;
                    if (type != 81)
                    {
                        switch (type)
                        {
                            case 90:
                                flag2 = true;
                                try
                                {
                                    num2 = float.Parse(akValue.Token.Text.Substring(1, akValue.Token.Text.Length - 2));
                                    goto IL_1F5;
                                }
                                catch (Exception)
                                {
                                    OnError("String cannot be cast to a float", akValue.Token.Line, akValue.Token.CharPositionInLine);
                                    goto IL_1F5;
                                }
                            case 91:
                                break;
                            default:
                                goto IL_1F5;
                        }
                        flag2 = true;
                        num2 = ((akValue.Token.Text.ToLowerInvariant() == "true") ? 1f : 0f);
                    }
                    else
                    {
                        flag2 = true;
                        num2 = int.Parse(akValue.Token.Text);
                    }
                    IL_1F5:
                    if (flag2)
                    {
                        token = new CommonToken(akValue.Token);
                        token.Type = 93;
                        token.Text = num2.ToString();
                        if (!token.Text.Contains('.'))
                        {
                            var token2 = token;
                            token2.Text += ".0";
                        }
                    }

                    break;
                }
                case "string":
                {
                    if (akValue.Token.Type is 81 or 93 or 91 or 92)
                    {
                            token = new CommonToken(akValue.Token)
                            {
                                Type = 90,
                                Text = $"\"{akValue.Token.Text}\""
                            };
                    }

                    break;
                }
                case "bool":
                {
                    var flag3 = false;
                    var flag4 = akValue.Token.Type != 91 && TryConvertToBool(akValue, out flag3);
                    if (flag4)
                    {
                        token = new CommonToken(akValue.Token);
                        token.Type = 91;
                        token.Text = (flag3 ? "True" : "False");
                    }

                    break;
                }
            }

            if (token == null) return null;
            result = new CommonTree(token);
            bMadeChanges = true;
            return result;
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x0003342C File Offset: 0x0003162C
		public PapyrusOptimizeWalker(ITreeNodeStream input) : this(input, new RecognizerSharedState())
		{
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x0003343C File Offset: 0x0003163C
		public PapyrusOptimizeWalker(ITreeNodeStream input, RecognizerSharedState state) : base(input, state)
		{
			InitializeCyclicDFAs();
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000AEF RID: 2799 RVA: 0x000334DC File Offset: 0x000316DC
		// (set) Token: 0x06000AF0 RID: 2800 RVA: 0x000334E4 File Offset: 0x000316E4
		public ITreeAdaptor TreeAdaptor
		{
			get => adaptor;
            set => adaptor = value;
        }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000AF1 RID: 2801 RVA: 0x000334F0 File Offset: 0x000316F0
		public override string[] TokenNames => tokenNames;

        // Token: 0x1700013C RID: 316
		// (get) Token: 0x06000AF2 RID: 2802 RVA: 0x000334F8 File Offset: 0x000316F8
		public override string GrammarFileName => "PapyrusOptimizeWalker.g";

        // Token: 0x14000037 RID: 55
		// (add) Token: 0x06000AF3 RID: 2803 RVA: 0x00033500 File Offset: 0x00031700
		// (remove) Token: 0x06000AF4 RID: 2804 RVA: 0x0003351C File Offset: 0x0003171C
		internal event InternalErrorEventHandler ErrorHandler;

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00033538 File Offset: 0x00031738
		private void OnError(string asError, int aiLineNumber, int aiColumnNumber)
        {
            ErrorHandler?.Invoke(this, new InternalErrorEventArgs(asError, aiLineNumber, aiColumnNumber));
        }

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00033558 File Offset: 0x00031758
		public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
		{
			var errorMessage = GetErrorMessage(e, tokenNames);
			OnError(errorMessage, e.Line, e.CharPositionInLine);
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00033584 File Offset: 0x00031784
		public script_return script(ScriptObjectType akObj, OptimizePass aePassType)
		{
			var script_return = new script_return();
			script_return.Start = input.LT(1);
			kObjType = akObj;
			ePassType = aePassType;
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 4, FOLLOW_OBJECT_in_script86);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_header_in_script88);
				var header_return = header();
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, header_return.Tree);
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
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_definitionOrBlock_in_script90);
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

		// Token: 0x06000AF8 RID: 2808 RVA: 0x000337BC File Offset: 0x000319BC
		public header_return header()
		{
			var header_return = new header_return();
			header_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 38, FOLLOW_ID_in_header104);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode2 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_header106);
				var child = (CommonTree)adaptor.DupNode(treeNode2);
				adaptor.AddChild(commonTree3, child);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 == 38)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_header108);
					var child2 = (CommonTree)adaptor.DupNode(treeNode3);
					adaptor.AddChild(commonTree3, child2);
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
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode4 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_header111);
					var child3 = (CommonTree)adaptor.DupNode(treeNode4);
					adaptor.AddChild(commonTree3, child3);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				header_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return header_return;
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00033A3C File Offset: 0x00031C3C
		public definitionOrBlock_return definitionOrBlock()
		{
			var definitionOrBlock_return = new definitionOrBlock_return();
			definitionOrBlock_return.Start = input.LT(1);
			CommonTree commonTree = null;
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
						goto IL_99;
					case 6:
						num2 = 2;
						goto IL_99;
					case 7:
						num2 = 3;
						goto IL_99;
					default:
						if (num != 19)
						{
							goto IL_82;
						}
						break;
					}
				}
				else
				{
					if (num == 51)
					{
						num2 = 4;
						goto IL_99;
					}
					if (num != 54)
					{
						goto IL_82;
					}
				}
				num2 = 5;
				goto IL_99;
				IL_82:
				var ex = new NoViableAltException("", 4, 0, input);
				throw ex;
				IL_99:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_fieldDefinition_in_definitionOrBlock126);
					var fieldDefinition_return = fieldDefinition();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, fieldDefinition_return.Tree);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree3 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_function_in_definitionOrBlock132);
					var function_return = function("", "");
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, function_return.Tree);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree4 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_eventFunc_in_definitionOrBlock140);
					var eventFunc_return = eventFunc("");
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, eventFunc_return.Tree);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree5 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_stateBlock_in_definitionOrBlock147);
					var stateBlock_return = stateBlock();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, stateBlock_return.Tree);
					break;
				}
				case 5:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree6 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_propertyBlock_in_definitionOrBlock153);
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

		// Token: 0x06000AFA RID: 2810 RVA: 0x00033D40 File Offset: 0x00031F40
		public fieldDefinition_return fieldDefinition()
		{
			var fieldDefinition_return = new fieldDefinition_return();
			fieldDefinition_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 5, FOLLOW_VAR_in_fieldDefinition167);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_type_in_fieldDefinition169);
				var type_return = type();
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, type_return.Tree);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_fieldDefinition171);
				var child = (CommonTree)adaptor.DupNode(treeNode2);
				adaptor.AddChild(commonTree3, child);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode3 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_fieldDefinition173);
				var child2 = (CommonTree)adaptor.DupNode(treeNode3);
				adaptor.AddChild(commonTree3, child2);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 is 81 or >= 90 and <= 93)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_constant_in_fieldDefinition175);
					var constant_return = constant();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, constant_return.Tree);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				fieldDefinition_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return fieldDefinition_return;
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00033FF0 File Offset: 0x000321F0
		public function_return function(string asState, string asPropertyName)
		{
			function_stack.Push(new function_scope());
			var function_return = new function_return();
			function_return.Start = input.LT(1);
			((function_scope)function_stack.Peek()).sstate = asState;
			((function_scope)function_stack.Peek()).spropertyName = asPropertyName;
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 6, FOLLOW_FUNCTION_in_function207);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_functionHeader_in_function209);
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
					PushFollow(FOLLOW_codeBlock_in_function211);
					var codeBlock_return = codeBlock(((function_scope)function_stack.Peek()).kfuncType.FunctionScope);
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				function_return.sName = ((functionHeader_return != null) ? functionHeader_return.sFuncName : null);
				function_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
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

		// Token: 0x06000AFC RID: 2812 RVA: 0x0003427C File Offset: 0x0003247C
		public functionHeader_return functionHeader()
		{
			var functionHeader_return = new functionHeader_return();
			functionHeader_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 8, FOLLOW_HEADER_in_functionHeader236);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
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
				{
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_type_in_functionHeader239);
					var type_return = type();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, type_return.Tree);
					break;
				}
				case 2:
				{
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 92, FOLLOW_NONE_in_functionHeader243);
					var child = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree3, child);
					break;
				}
				}
				commonTree2 = (CommonTree)input.LT(1);
				var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_functionHeader248);
				var child2 = (CommonTree)adaptor.DupNode(commonTree4);
				adaptor.AddChild(commonTree3, child2);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode3 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_functionHeader250);
				var child3 = (CommonTree)adaptor.DupNode(treeNode3);
				adaptor.AddChild(commonTree3, child3);
				var num3 = 2;
				var num4 = input.LA(1);
				if (num4 == 9)
				{
					num3 = 1;
				}
				var num5 = num3;
				if (num5 == 1)
				{
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_callParameters_in_functionHeader252);
					var callParameters_return = callParameters();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, callParameters_return.Tree);
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
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_functionModifier_in_functionHeader255);
					var functionModifier_return = functionModifier();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, functionModifier_return.Tree);
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
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode4 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_functionHeader258);
					var child4 = (CommonTree)adaptor.DupNode(treeNode4);
					adaptor.AddChild(commonTree3, child4);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				functionHeader_return.sFuncName = commonTree4.Text;
				if (((function_scope)function_stack.Peek()).spropertyName == "")
				{
					kObjType.TryGetFunction(((function_scope)function_stack.Peek()).sstate, functionHeader_return.sFuncName, out ((function_scope)function_stack.Peek()).kfuncType);
				}
				else
				{
                    kObjType.TryGetProperty(((function_scope)function_stack.Peek()).spropertyName, out var scriptPropertyType);
                    var a = functionHeader_return.sFuncName.ToLowerInvariant();
					((function_scope)function_stack.Peek()).kfuncType = a == "get" ? scriptPropertyType.kGetFunction : scriptPropertyType.kSetFunction;
				}
				if (ePassType != OptimizePass.VARCLEANUP)
				{
					((function_scope)function_stack.Peek()).kfuncType.FunctionScope.ClearUsedVars();
				}
				functionHeader_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return functionHeader_return;
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x000347D4 File Offset: 0x000329D4
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

		// Token: 0x06000AFE RID: 2814 RVA: 0x000348E4 File Offset: 0x00032AE4
		public eventFunc_return eventFunc(string asState)
		{
			eventFunc_stack.Push(new eventFunc_scope());
			var eventFunc_return = new eventFunc_return();
			eventFunc_return.Start = input.LT(1);
			((eventFunc_scope)eventFunc_stack.Peek()).sstate = asState;
			((eventFunc_scope)eventFunc_stack.Peek()).sfuncName = "";
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 7, FOLLOW_EVENT_in_eventFunc307);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_eventHeader_in_eventFunc309);
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
					PushFollow(FOLLOW_codeBlock_in_eventFunc311);
					var codeBlock_return = codeBlock(((eventFunc_scope)eventFunc_stack.Peek()).kfuncType.FunctionScope);
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				eventFunc_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
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

		// Token: 0x06000AFF RID: 2815 RVA: 0x00034B60 File Offset: 0x00032D60
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
				var treeNode = (CommonTree)Match(input, 8, FOLLOW_HEADER_in_eventHeader327);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode2 = (CommonTree)Match(input, 92, FOLLOW_NONE_in_eventHeader329);
				var child = (CommonTree)adaptor.DupNode(treeNode2);
				adaptor.AddChild(commonTree3, child);
				commonTree2 = (CommonTree)input.LT(1);
				var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_eventHeader331);
				var child2 = (CommonTree)adaptor.DupNode(commonTree4);
				adaptor.AddChild(commonTree3, child2);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode3 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_eventHeader333);
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
					PushFollow(FOLLOW_callParameters_in_eventHeader335);
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
					var treeNode4 = (CommonTree)Match(input, 47, FOLLOW_NATIVE_in_eventHeader338);
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
					var treeNode5 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_eventHeader341);
					var child5 = (CommonTree)adaptor.DupNode(treeNode5);
					adaptor.AddChild(commonTree3, child5);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				((eventFunc_scope)eventFunc_stack.Peek()).sfuncName = commonTree4.Text;
				kObjType.TryGetFunction(((eventFunc_scope)eventFunc_stack.Peek()).sstate, ((eventFunc_scope)eventFunc_stack.Peek()).sfuncName, out ((eventFunc_scope)eventFunc_stack.Peek()).kfuncType);
				if (ePassType != OptimizePass.VARCLEANUP)
				{
					((eventFunc_scope)eventFunc_stack.Peek()).kfuncType.FunctionScope.ClearUsedVars();
				}
				eventHeader_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return eventHeader_return;
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x00034F88 File Offset: 0x00033188
		public callParameters_return callParameters()
		{
			var callParameters_return = new callParameters_return();
			callParameters_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
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
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_callParameter_in_callParameters363);
					var callParameter_return = callParameter();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, callParameter_return.Tree);
					num++;
				}
				if (num < 1)
				{
					var ex = new EarlyExitException(15, input);
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

		// Token: 0x06000B01 RID: 2817 RVA: 0x00035098 File Offset: 0x00033298
		public callParameter_return callParameter()
		{
			var callParameter_return = new callParameter_return();
			callParameter_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 9, FOLLOW_PARAM_in_callParameter378);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_type_in_callParameter380);
				var type_return = type();
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, type_return.Tree);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_callParameter382);
				var child = (CommonTree)adaptor.DupNode(treeNode2);
				adaptor.AddChild(commonTree3, child);
				var num = 2;
				var num2 = input.LA(1);
				if (num2 is 81 or >= 90 and <= 93)
				{
					num = 1;
				}
				var num3 = num;
				if (num3 == 1)
				{
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_constant_in_callParameter384);
					var constant_return = constant();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, constant_return.Tree);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				callParameter_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return callParameter_return;
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x000352F4 File Offset: 0x000334F4
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
				var treeNode = (CommonTree)Match(input, 51, FOLLOW_STATE_in_stateBlock402);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_stateBlock404);
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
					var treeNode2 = (CommonTree)Match(input, 50, FOLLOW_AUTO_in_stateBlock406);
					var child2 = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree3, child2);
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
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_stateFuncOrEvent_in_stateBlock410);
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

		// Token: 0x06000B03 RID: 2819 RVA: 0x0003557C File Offset: 0x0003377C
		public stateFuncOrEvent_return stateFuncOrEvent(string asStateName)
		{
			var stateFuncOrEvent_return = new stateFuncOrEvent_return();
			stateFuncOrEvent_return.Start = input.LT(1);
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
						var ex = new NoViableAltException("", 19, 0, input);
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
					PushFollow(FOLLOW_function_in_stateFuncOrEvent427);
					var function_return = function(asStateName, "");
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, function_return.Tree);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree3 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_eventFunc_in_stateFuncOrEvent435);
					var eventFunc_return = eventFunc(asStateName);
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

		// Token: 0x06000B04 RID: 2820 RVA: 0x0003570C File Offset: 0x0003390C
		public propertyBlock_return propertyBlock()
		{
			var propertyBlock_return = new propertyBlock_return();
			propertyBlock_return.Start = input.LT(1);
			CommonTree commonTree = null;
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
						var ex = new NoViableAltException("", 20, 0, input);
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
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 54, FOLLOW_PROPERTY_in_propertyBlock452);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_propertyHeader_in_propertyBlock454);
					var propertyHeader_return = propertyHeader();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, propertyHeader_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_propertyFunc_in_propertyBlock456);
					var propertyFunc_return = propertyFunc((propertyHeader_return != null) ? propertyHeader_return.sName : null);
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, propertyFunc_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_propertyFunc_in_propertyBlock459);
					var propertyFunc_return2 = propertyFunc((propertyHeader_return != null) ? propertyHeader_return.sName : null);
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, propertyFunc_return2.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree3);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 19, FOLLOW_AUTOPROP_in_propertyBlock468);
					var newRoot2 = (CommonTree)adaptor.DupNode(treeNode2);
					commonTree4 = (CommonTree)adaptor.BecomeRoot(newRoot2, commonTree4);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_propertyHeader_in_propertyBlock470);
					var propertyHeader_return2 = propertyHeader();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree4, propertyHeader_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_propertyBlock472);
					var child = (CommonTree)adaptor.DupNode(treeNode3);
					adaptor.AddChild(commonTree4, child);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree4);
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
			return propertyBlock_return;
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00035B14 File Offset: 0x00033D14
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
				var treeNode = (CommonTree)Match(input, 8, FOLLOW_HEADER_in_propertyHeader490);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_type_in_propertyHeader492);
				var type_return = type();
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, type_return.Tree);
				commonTree2 = (CommonTree)input.LT(1);
				var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_propertyHeader496);
				var child = (CommonTree)adaptor.DupNode(commonTree4);
				adaptor.AddChild(commonTree3, child);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode2 = (CommonTree)Match(input, 18, FOLLOW_USER_FLAGS_in_propertyHeader498);
				var child2 = (CommonTree)adaptor.DupNode(treeNode2);
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
					var treeNode3 = (CommonTree)Match(input, 40, FOLLOW_DOCSTRING_in_propertyHeader500);
					var child3 = (CommonTree)adaptor.DupNode(treeNode3);
					adaptor.AddChild(commonTree3, child3);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				propertyHeader_return.sName = commonTree4.Text;
				propertyHeader_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return propertyHeader_return;
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00035DCC File Offset: 0x00033FCC
		public propertyFunc_return propertyFunc(string asPropName)
		{
			var propertyFunc_return = new propertyFunc_return();
			propertyFunc_return.Start = input.LT(1);
			CommonTree commonTree = null;
			try
			{
				var num = input.LA(1);
				if (num != 17)
				{
					var ex = new NoViableAltException("", 22, 0, input);
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
						var ex2 = new NoViableAltException("", 22, 1, input);
						throw ex2;
					}
					num3 = 2;
				}
				switch (num3)
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 17, FOLLOW_PROPFUNC_in_propertyFunc521);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_function_in_propertyFunc523);
					var function_return = function("", asPropName);
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, function_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree3);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 17, FOLLOW_PROPFUNC_in_propertyFunc532);
					var child = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree, child);
					break;
				}
				}
				propertyFunc_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex3)
			{
				ReportError(ex3);
				Recover(input, ex3);
			}
			return propertyFunc_return;
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x0003604C File Offset: 0x0003424C
		public codeBlock_return codeBlock(ScriptScope akCurrentScope)
		{
			codeBlock_stack.Push(new codeBlock_scope());
			var codeBlock_return = new codeBlock_return();
			codeBlock_return.Start = input.LT(1);
			((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope = akCurrentScope;
			((codeBlock_scope)codeBlock_stack.Peek()).inextScopeChild = 0;
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 10, FOLLOW_BLOCK_in_codeBlock558);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
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
						commonTree2 = (CommonTree)input.LT(1);
						PushFollow(FOLLOW_statement_in_codeBlock561);
						var statement_return = statement();
						state.followingStackPointer--;
						adaptor.AddChild(commonTree3, statement_return.Tree);
					}
					Match(input, 3, null);
				}
				adaptor.AddChild(commonTree, commonTree3);
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

		// Token: 0x06000B08 RID: 2824 RVA: 0x000362D8 File Offset: 0x000344D8
		public statement_return statement()
		{
			var statement_return = new statement_return();
			statement_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token EQUALS");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule l_value");
			try
			{
				var num = input.LA(1);
				int num2;
				switch (num)
				{
				case 5:
					num2 = 1;
					goto IL_203;
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
					goto IL_1EB;
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
					goto IL_203;
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
						goto IL_1EB;
					case 83:
						num2 = 4;
						goto IL_203;
					case 84:
						num2 = 5;
						goto IL_203;
					case 88:
						num2 = 6;
						goto IL_203;
					default:
						goto IL_1EB;
					}
					break;
				}
				num2 = 3;
				goto IL_203;
				IL_1EB:
				var ex = new NoViableAltException("", 24, 0, input);
				throw ex;
				IL_203:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_localDefinition_in_statement576);
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
					var el = (CommonTree)Match(input, 41, FOLLOW_EQUALS_in_statement583);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_statement585);
					rewriteRuleNodeStream2.Add(commonTree3);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_statement587);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_l_value_in_statement589);
					var l_value_return = l_value();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(l_value_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_statement591);
					var expression_return = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree3.Text);
					statement_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (statement_return != null) ? statement_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if ((expression_return.kOptimizedTree) == null)
					{
						var commonTree4 = (CommonTree)adaptor.GetNilNode();
						commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
						adaptor.AddChild(commonTree4, rewriteRuleNodeStream2.NextNode());
						adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree4);
					}
					else if (((autoCast_return != null) ? autoCast_return.kOptimizedTree : null) != null)
					{
						var commonTree5 = (CommonTree)adaptor.GetNilNode();
						commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
						adaptor.AddChild(commonTree5, rewriteRuleNodeStream2.NextNode());
						adaptor.AddChild(commonTree5, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null);
						adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree5, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null);
						adaptor.AddChild(commonTree, commonTree5);
					}
					else
					{
						var commonTree6 = (CommonTree)adaptor.GetNilNode();
						commonTree6 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree6);
						adaptor.AddChild(commonTree6, rewriteRuleNodeStream2.NextNode());
						adaptor.AddChild(commonTree6, FixUpAutoCast((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (expression_return != null) ? expression_return.kOptimizedTree : null));
						adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree6);
					}
					statement_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_statement658);
					var expression_return2 = expression();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, expression_return2.Tree);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_return_stat_in_statement664);
					var return_stat_return = return_stat();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, return_stat_return.Tree);
					break;
				}
				case 5:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_ifBlock_in_statement670);
					var ifBlock_return = ifBlock();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, ifBlock_return.Tree);
					break;
				}
				case 6:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_whileBlock_in_statement676);
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
			return statement_return;
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x00036B34 File Offset: 0x00034D34
		public localDefinition_return localDefinition()
		{
			localDefinition_stack.Push(new localDefinition_scope());
			var localDefinition_return = new localDefinition_return();
			localDefinition_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token VAR");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule type");
			((localDefinition_scope)localDefinition_stack.Peek()).bvarUsed = true;
			try
			{
				switch (dfa25.Predict(input))
				{
				case 1:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el = (CommonTree)Match(input, 5, FOLLOW_VAR_in_localDefinition699);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_type_in_localDefinition701);
					var type_return = type();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(type_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_localDefinition705);
					rewriteRuleNodeStream2.Add(commonTree3);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_localDefinition707);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_localDefinition709);
					var expression_return = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					localDefinition_return.Tree = commonTree;
					var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token name", commonTree3);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (localDefinition_return != null) ? localDefinition_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if ((expression_return.kOptimizedTree) == null)
					{
						var commonTree4 = (CommonTree)adaptor.GetNilNode();
						commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
						adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree4, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree4);
					}
					else if (((autoCast_return != null) ? autoCast_return.kOptimizedTree : null) != null)
					{
						var commonTree5 = (CommonTree)adaptor.GetNilNode();
						commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
						adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree5, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree5, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null);
						adaptor.AddChild(commonTree5, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null);
						adaptor.AddChild(commonTree, commonTree5);
					}
					else
					{
						var commonTree6 = (CommonTree)adaptor.GetNilNode();
						commonTree6 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree6);
						adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree6, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree6, FixUpAutoCast((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (expression_return != null) ? expression_return.kOptimizedTree : null));
						adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree6);
					}
					localDefinition_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child2 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el2 = (CommonTree)Match(input, 5, FOLLOW_VAR_in_localDefinition776);
					rewriteRuleNodeStream.Add(el2);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_type_in_localDefinition778);
					var type_return2 = type();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(type_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_localDefinition782);
					rewriteRuleNodeStream2.Add(commonTree3);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child2);
					if (ePassType == OptimizePass.VARCLEANUP)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryGetVarUsed(commonTree3.Text, out ((localDefinition_scope)localDefinition_stack.Peek()).bvarUsed);
					}
					localDefinition_return.Tree = commonTree;
					var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token name", commonTree3);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (localDefinition_return != null) ? localDefinition_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (((localDefinition_scope)localDefinition_stack.Peek()).bvarUsed)
					{
						var commonTree7 = (CommonTree)adaptor.GetNilNode();
						commonTree7 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree7);
						adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree7, rewriteRuleNodeStream4.NextNode());
						adaptor.AddChild(commonTree, commonTree7);
					}
					else
					{
						commonTree = null;
					}
					localDefinition_return.Tree = commonTree;
					break;
				}
				}
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

		// Token: 0x06000B0A RID: 2826 RVA: 0x0003728C File Offset: 0x0003548C
		public l_value_return l_value()
		{
			var l_value_return = new l_value_return();
			l_value_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token PAREXPR");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ARRAYSET");
			var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			try
			{
				switch (dfa26.Predict(input))
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 62, FOLLOW_DOT_in_l_value823);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 15, FOLLOW_PAREXPR_in_l_value826);
					var newRoot2 = (CommonTree)adaptor.DupNode(treeNode2);
					commonTree4 = (CommonTree)adaptor.BecomeRoot(newRoot2, commonTree4);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_l_value828);
					var expression_return = expression();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree4, expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree3, commonTree4);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_property_set_in_l_value831);
					var property_set_return = property_set();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, property_set_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree3);
					break;
				}
				case 2:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el = (CommonTree)Match(input, 23, FOLLOW_ARRAYSET_in_l_value839);
					rewriteRuleNodeStream2.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_l_value843);
					rewriteRuleNodeStream3.Add(commonTree6);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree7 = (CommonTree)Match(input, 38, FOLLOW_ID_in_l_value847);
					rewriteRuleNodeStream3.Add(commonTree7);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_l_value849);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el2 = (CommonTree)Match(input, 15, FOLLOW_PAREXPR_in_l_value852);
					rewriteRuleNodeStream.Add(el2);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_l_value856);
					var expression_return2 = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return2.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree5, child);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_l_value861);
					var expression_return3 = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return3.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree5);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree6.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree7.Text);
					l_value_return.Tree = commonTree;
					var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token source", commonTree6);
					var rewriteRuleNodeStream5 = new RewriteRuleNodeStream(adaptor, "token self", commonTree7);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (l_value_return != null) ? l_value_return.Tree : null);
					var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule index", (expression_return3 != null) ? expression_return3.Tree : null);
					var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule array", (expression_return2 != null) ? expression_return2.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree8 = (CommonTree)adaptor.GetNilNode();
					commonTree8 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree8);
					adaptor.AddChild(commonTree8, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree8, rewriteRuleNodeStream5.NextNode());
					adaptor.AddChild(commonTree8, FixUpAutoCast((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (expression_return3 != null) ? expression_return3.kOptimizedTree : null));
					var commonTree9 = (CommonTree)adaptor.GetNilNode();
					commonTree9 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree9);
					adaptor.AddChild(commonTree9, rewriteRuleSubtreeStream4.NextTree());
					adaptor.AddChild(commonTree8, commonTree9);
					adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream3.NextTree());
					adaptor.AddChild(commonTree, commonTree8);
					l_value_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_basic_l_value_in_l_value900);
					var basic_l_value_return = basic_l_value();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, basic_l_value_return.Tree);
					break;
				}
				}
				l_value_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return l_value_return;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00037A54 File Offset: 0x00035C54
		public basic_l_value_return basic_l_value()
		{
			var basic_l_value_return = new basic_l_value_return();
			basic_l_value_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token ARRAYSET");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule func_or_id");
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
							goto IL_129;
						case 22:
							goto IL_111;
						case 23:
							num2 = 4;
							goto IL_129;
						case 24:
						case 25:
							break;
						default:
							goto IL_111;
						}
						break;
					}
					num2 = 2;
					goto IL_129;
				}
				if (num == 38)
				{
					num2 = 5;
					goto IL_129;
				}
				if (num == 62)
				{
					num2 = 1;
					goto IL_129;
				}
				IL_111:
				var ex = new NoViableAltException("", 27, 0, input);
				throw ex;
				IL_129:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 62, FOLLOW_DOT_in_basic_l_value914);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_array_func_or_id_in_basic_l_value916);
					var array_func_or_id_return = array_func_or_id();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, array_func_or_id_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_basic_l_value_in_basic_l_value918);
					var basic_l_value_return2 = basic_l_value();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, basic_l_value_return2.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree3);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_function_call_in_basic_l_value925);
					var function_call_return = function_call();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, function_call_return.Tree);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_property_set_in_basic_l_value931);
					var property_set_return = property_set();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, property_set_return.Tree);
					break;
				}
				case 4:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el = (CommonTree)Match(input, 23, FOLLOW_ARRAYSET_in_basic_l_value940);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_basic_l_value944);
					rewriteRuleNodeStream2.Add(commonTree4);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_basic_l_value948);
					rewriteRuleNodeStream2.Add(commonTree5);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_basic_l_value950);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_func_or_id_in_basic_l_value952);
					var func_or_id_return = func_or_id();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(func_or_id_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_basic_l_value954);
					var expression_return = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					basic_l_value_return.Tree = commonTree;
					var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token source", commonTree4);
					var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token self", commonTree5);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (basic_l_value_return != null) ? basic_l_value_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree6 = (CommonTree)adaptor.GetNilNode();
					commonTree6 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree6);
					adaptor.AddChild(commonTree6, rewriteRuleNodeStream3.NextNode());
					adaptor.AddChild(commonTree6, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree6, FixUpAutoCast((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (expression_return != null) ? expression_return.kOptimizedTree : null));
					adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream3.NextTree());
					adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream.NextTree());
					adaptor.AddChild(commonTree, commonTree6);
					basic_l_value_return.Tree = commonTree;
					break;
				}
				case 5:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree7 = (CommonTree)Match(input, 38, FOLLOW_ID_in_basic_l_value987);
					var child2 = (CommonTree)adaptor.DupNode(commonTree7);
					adaptor.AddChild(commonTree, child2);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree7.Text);
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
			return basic_l_value_return;
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x000381CC File Offset: 0x000363CC
		public expression_return expression()
		{
			var expression_return = new expression_return();
			expression_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token OR");
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
					if (num is (< 11 or > 13) and not 15 and not 20 and not 22 and (< 24 or > 36) and not 38 and not 62 and (< 66 or > 72) and (< 77 or > 82) and (< 90 or > 93))
					{
						var ex = new NoViableAltException("", 28, 0, input);
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
					var commonTree3 = (CommonTree)Match(input, 65, FOLLOW_OR_in_expression1010);
					rewriteRuleNodeStream2.Add(commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_expression1012);
					rewriteRuleNodeStream.Add(commonTree4);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_expression1016);
					var expression_return2 = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_and_expression_in_expression1018);
					var and_expression_return = and_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(and_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					expression_return.kOptimizedTree = CompilerAutoTwoArgBoolOp((expression_return2 != null) ? ((CommonTree)expression_return2.Tree) : null, (and_expression_return != null) ? ((CommonTree)and_expression_return.Tree) : null, commonTree3.Token);
					if (expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					}
					expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (expression_return != null) ? expression_return.Tree : null);
					var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule childExpr", (expression_return2 != null) ? expression_return2.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree5 = (CommonTree)adaptor.GetNilNode();
						commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree5);
						adaptor.AddChild(commonTree5, rewriteRuleNodeStream.NextNode());
						adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(commonTree, commonTree5);
					}
					expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_and_expression_in_expression1055);
					var and_expression_return2 = and_expression();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, and_expression_return2.Tree);
					expression_return.kOptimizedTree = ((and_expression_return2 != null) ? and_expression_return2.kOptimizedTree : null);
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

		// Token: 0x06000B0D RID: 2829 RVA: 0x00038658 File Offset: 0x00036858
		public and_expression_return and_expression()
		{
			var and_expression_return = new and_expression_return();
			and_expression_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token AND");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
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
					if (num is (< 11 or > 13) and not 15 and not 20 and not 22 and (< 24 or > 36) and not 38 and not 62 and (< 67 or > 72) and (< 77 or > 82) and (< 90 or > 93))
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
					var commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 66, FOLLOW_AND_in_and_expression1077);
					rewriteRuleNodeStream.Add(commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_and_expression1079);
					rewriteRuleNodeStream2.Add(commonTree4);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_and_expression_in_and_expression1083);
					var and_expression_return2 = and_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(and_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_bool_expression_in_and_expression1085);
					var bool_expression_return = bool_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(bool_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					and_expression_return.kOptimizedTree = CompilerAutoTwoArgBoolOp((and_expression_return2 != null) ? ((CommonTree)and_expression_return2.Tree) : null, (bool_expression_return != null) ? ((CommonTree)bool_expression_return.Tree) : null, commonTree3.Token);
					if (and_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					}
					and_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (and_expression_return != null) ? and_expression_return.Tree : null);
					var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule childExpr", (and_expression_return2 != null) ? and_expression_return2.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (and_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, and_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree5 = (CommonTree)adaptor.GetNilNode();
						commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
						adaptor.AddChild(commonTree5, rewriteRuleNodeStream2.NextNode());
						adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
						adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree5);
					}
					and_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_bool_expression_in_and_expression1122);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, bool_expression_return2.Tree);
					and_expression_return.kOptimizedTree = (bool_expression_return2.kOptimizedTree);
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

		// Token: 0x06000B0E RID: 2830 RVA: 0x00038AE4 File Offset: 0x00036CE4
		public bool_expression_return bool_expression()
		{
			bool_expression_stack.Push(new bool_expression_scope());
			var bool_expression_return = new bool_expression_return();
			bool_expression_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token GT");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token LT");
			var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token EQ");
			var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleNodeStream5 = new RewriteRuleNodeStream(adaptor, "token LTE");
			var rewriteRuleNodeStream6 = new RewriteRuleNodeStream(adaptor, "token GTE");
			var rewriteRuleNodeStream7 = new RewriteRuleNodeStream(adaptor, "token NE");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule bool_expression");
			var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule add_expression");
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
					goto IL_24C;
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
						goto IL_24C;
					case 67:
						num2 = 1;
						goto IL_264;
					case 68:
						num2 = 2;
						goto IL_264;
					case 69:
						num2 = 3;
						goto IL_264;
					case 70:
						num2 = 4;
						goto IL_264;
					case 71:
						num2 = 5;
						goto IL_264;
					case 72:
						num2 = 6;
						goto IL_264;
					default:
						goto IL_24C;
					}
					break;
				}
				num2 = 7;
				goto IL_264;
				IL_24C:
				var ex = new NoViableAltException("", 30, 0, input);
				throw ex;
				IL_264:
				switch (num2)
				{
				case 1:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el = (CommonTree)Match(input, 67, FOLLOW_EQ_in_bool_expression1148);
					rewriteRuleNodeStream3.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1150);
					rewriteRuleNodeStream4.Add(commonTree3);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1154);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1158);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_bool_expression_in_bool_expression1162);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_bool_expression1166);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(add_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree3.Text);
					FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (bool_expression_return2 != null) ? ((CommonTree)bool_expression_return2.Tree) : null, (bool_expression_return2 != null) ? bool_expression_return2.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (add_expression_return != null) ? ((CommonTree)add_expression_return.Tree) : null, (add_expression_return != null) ? add_expression_return.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree4 = (CommonTree)adaptor.GetNilNode();
					commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream3.NextNode(), commonTree4);
					adaptor.AddChild(commonTree4, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree4, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA);
					adaptor.AddChild(commonTree4, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB);
					adaptor.AddChild(commonTree4, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					adaptor.AddChild(commonTree4, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					adaptor.AddChild(commonTree, commonTree4);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child2 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el2 = (CommonTree)Match(input, 68, FOLLOW_NE_in_bool_expression1198);
					rewriteRuleNodeStream7.Add(el2);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1200);
					rewriteRuleNodeStream4.Add(commonTree5);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1204);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1208);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_bool_expression_in_bool_expression1212);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_bool_expression1216);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(add_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child2);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (bool_expression_return2 != null) ? ((CommonTree)bool_expression_return2.Tree) : null, (bool_expression_return2 != null) ? bool_expression_return2.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (add_expression_return != null) ? ((CommonTree)add_expression_return.Tree) : null, (add_expression_return != null) ? add_expression_return.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree6 = (CommonTree)adaptor.GetNilNode();
					commonTree6 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream7.NextNode(), commonTree6);
					adaptor.AddChild(commonTree6, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree6, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA);
					adaptor.AddChild(commonTree6, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB);
					adaptor.AddChild(commonTree6, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					adaptor.AddChild(commonTree6, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					adaptor.AddChild(commonTree, commonTree6);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el3 = (CommonTree)Match(input, 69, FOLLOW_GT_in_bool_expression1248);
					rewriteRuleNodeStream.Add(el3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree7 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1250);
					rewriteRuleNodeStream4.Add(commonTree7);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1254);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1258);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_bool_expression_in_bool_expression1262);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_bool_expression1266);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(add_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child3);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree7.Text);
					FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (bool_expression_return2 != null) ? ((CommonTree)bool_expression_return2.Tree) : null, (bool_expression_return2 != null) ? bool_expression_return2.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (add_expression_return != null) ? ((CommonTree)add_expression_return.Tree) : null, (add_expression_return != null) ? add_expression_return.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree8 = (CommonTree)adaptor.GetNilNode();
					commonTree8 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree8);
					adaptor.AddChild(commonTree8, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree8, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA);
					adaptor.AddChild(commonTree8, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB);
					adaptor.AddChild(commonTree8, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					adaptor.AddChild(commonTree8, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					adaptor.AddChild(commonTree, commonTree8);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 4:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child4 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el4 = (CommonTree)Match(input, 70, FOLLOW_LT_in_bool_expression1298);
					rewriteRuleNodeStream2.Add(el4);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree9 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1300);
					rewriteRuleNodeStream4.Add(commonTree9);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1304);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1308);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_bool_expression_in_bool_expression1312);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_bool_expression1316);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(add_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child4);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree9.Text);
					FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (bool_expression_return2 != null) ? ((CommonTree)bool_expression_return2.Tree) : null, (bool_expression_return2 != null) ? bool_expression_return2.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (add_expression_return != null) ? ((CommonTree)add_expression_return.Tree) : null, (add_expression_return != null) ? add_expression_return.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree10 = (CommonTree)adaptor.GetNilNode();
					commonTree10 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree10);
					adaptor.AddChild(commonTree10, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree10, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA);
					adaptor.AddChild(commonTree10, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB);
					adaptor.AddChild(commonTree10, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					adaptor.AddChild(commonTree10, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					adaptor.AddChild(commonTree, commonTree10);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 5:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child5 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el5 = (CommonTree)Match(input, 71, FOLLOW_GTE_in_bool_expression1348);
					rewriteRuleNodeStream6.Add(el5);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree11 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1350);
					rewriteRuleNodeStream4.Add(commonTree11);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1354);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1358);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_bool_expression_in_bool_expression1362);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_bool_expression1366);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(add_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child5);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree11.Text);
					FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (bool_expression_return2 != null) ? ((CommonTree)bool_expression_return2.Tree) : null, (bool_expression_return2 != null) ? bool_expression_return2.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (add_expression_return != null) ? ((CommonTree)add_expression_return.Tree) : null, (add_expression_return != null) ? add_expression_return.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree12 = (CommonTree)adaptor.GetNilNode();
					commonTree12 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream6.NextNode(), commonTree12);
					adaptor.AddChild(commonTree12, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree12, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA);
					adaptor.AddChild(commonTree12, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB);
					adaptor.AddChild(commonTree12, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					adaptor.AddChild(commonTree12, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					adaptor.AddChild(commonTree, commonTree12);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 6:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child6 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el6 = (CommonTree)Match(input, 72, FOLLOW_LTE_in_bool_expression1398);
					rewriteRuleNodeStream5.Add(el6);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree13 = (CommonTree)Match(input, 38, FOLLOW_ID_in_bool_expression1400);
					rewriteRuleNodeStream4.Add(commonTree13);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1404);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_bool_expression1408);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_bool_expression_in_bool_expression1412);
					var bool_expression_return2 = bool_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(bool_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_bool_expression1416);
					var add_expression_return = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(add_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child6);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree13.Text);
					FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (bool_expression_return2 != null) ? ((CommonTree)bool_expression_return2.Tree) : null, (bool_expression_return2 != null) ? bool_expression_return2.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (add_expression_return != null) ? ((CommonTree)add_expression_return.Tree) : null, (add_expression_return != null) ? add_expression_return.kOptimizedTree : null, out ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB, out ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					bool_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (bool_expression_return != null) ? bool_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree14 = (CommonTree)adaptor.GetNilNode();
					commonTree14 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream5.NextNode(), commonTree14);
					adaptor.AddChild(commonTree14, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree14, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeA);
					adaptor.AddChild(commonTree14, ((bool_expression_scope)bool_expression_stack.Peek()).kautoCastTreeB);
					adaptor.AddChild(commonTree14, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionA);
					adaptor.AddChild(commonTree14, ((bool_expression_scope)bool_expression_stack.Peek()).kexpressionB);
					adaptor.AddChild(commonTree, commonTree14);
					bool_expression_return.Tree = commonTree;
					break;
				}
				case 7:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_bool_expression1447);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, add_expression_return2.Tree);
					bool_expression_return.kOptimizedTree = ((add_expression_return2 != null) ? add_expression_return2.kOptimizedTree : null);
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

		// Token: 0x06000B0F RID: 2831 RVA: 0x0003A62C File Offset: 0x0003882C
		public add_expression_return add_expression()
		{
			add_expression_stack.Push(new add_expression_scope());
			var add_expression_return = new add_expression_return();
			add_expression_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token IADD");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ISUBTRACT");
			var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token FADD");
			var rewriteRuleNodeStream5 = new RewriteRuleNodeStream(adaptor, "token FSUBTRACT");
			var rewriteRuleNodeStream6 = new RewriteRuleNodeStream(adaptor, "token STRCAT");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule add_expression");
			var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule mult_expression");
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
					goto IL_1F9;
				case 26:
					num2 = 1;
					goto IL_211;
				case 27:
					num2 = 2;
					goto IL_211;
				case 28:
					num2 = 3;
					goto IL_211;
				case 29:
					num2 = 4;
					goto IL_211;
				case 36:
					num2 = 5;
					goto IL_211;
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
							goto IL_1F9;
						default:
							goto IL_1F9;
						}
					}
					break;
				}
				num2 = 6;
				goto IL_211;
				IL_1F9:
				var ex = new NoViableAltException("", 31, 0, input);
				throw ex;
				IL_211:
				switch (num2)
				{
				case 1:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 26, FOLLOW_IADD_in_add_expression1473);
					rewriteRuleNodeStream.Add(commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression1475);
					rewriteRuleNodeStream3.Add(commonTree4);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1479);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1483);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_add_expression1487);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_add_expression1491);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					add_expression_return.kOptimizedTree = CompilerAutoIntTwoArgMathOp((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, commonTree3.Token);
					if (add_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
						FixUpAutoCastAndExpression(((CommonTree)autoCast_return.Tree), autoCast_return.kOptimizedTree, ((CommonTree)add_expression_return2.Tree), add_expression_return2.kOptimizedTree, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression(((CommonTree)autoCast_return2.Tree), autoCast_return2.kOptimizedTree, ((CommonTree)mult_expression_return.Tree), mult_expression_return.kOptimizedTree, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
					}
					add_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", add_expression_return.Tree);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (add_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, add_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree5 = (CommonTree)adaptor.GetNilNode();
						commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
						adaptor.AddChild(commonTree5, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree5, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree5, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree5, ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree5, ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
						adaptor.AddChild(commonTree, commonTree5);
					}
					add_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child2 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree6 = (CommonTree)Match(input, 27, FOLLOW_FADD_in_add_expression1532);
					rewriteRuleNodeStream4.Add(commonTree6);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree7 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression1534);
					rewriteRuleNodeStream3.Add(commonTree7);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1538);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1542);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_add_expression1546);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_add_expression1550);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child2);
					add_expression_return.kOptimizedTree = CompilerAutoFloatTwoArgMathOp((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, commonTree6.Token);
					if (add_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree7.Text);
						FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (add_expression_return2 != null) ? ((CommonTree)add_expression_return2.Tree) : null, (add_expression_return2 != null) ? add_expression_return2.kOptimizedTree : null, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (mult_expression_return != null) ? ((CommonTree)mult_expression_return.Tree) : null, (mult_expression_return != null) ? mult_expression_return.kOptimizedTree : null, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
					}
					add_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (add_expression_return != null) ? add_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (add_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, add_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree8 = (CommonTree)adaptor.GetNilNode();
						commonTree8 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream4.NextNode(), commonTree8);
						adaptor.AddChild(commonTree8, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree8, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree8, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree8, ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree8, ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
						adaptor.AddChild(commonTree, commonTree8);
					}
					add_expression_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree9 = (CommonTree)Match(input, 28, FOLLOW_ISUBTRACT_in_add_expression1591);
					rewriteRuleNodeStream2.Add(commonTree9);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree10 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression1593);
					rewriteRuleNodeStream3.Add(commonTree10);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1597);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1601);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_add_expression1605);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_add_expression1609);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child3);
					add_expression_return.kOptimizedTree = CompilerAutoIntTwoArgMathOp((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, commonTree9.Token);
					if (add_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree10.Text);
						FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (add_expression_return2 != null) ? ((CommonTree)add_expression_return2.Tree) : null, (add_expression_return2 != null) ? add_expression_return2.kOptimizedTree : null, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (mult_expression_return != null) ? ((CommonTree)mult_expression_return.Tree) : null, (mult_expression_return != null) ? mult_expression_return.kOptimizedTree : null, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
					}
					add_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (add_expression_return != null) ? add_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (add_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, add_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree11 = (CommonTree)adaptor.GetNilNode();
						commonTree11 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree11);
						adaptor.AddChild(commonTree11, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree11, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree11, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree11, ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree11, ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
						adaptor.AddChild(commonTree, commonTree11);
					}
					add_expression_return.Tree = commonTree;
					break;
				}
				case 4:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child4 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree12 = (CommonTree)Match(input, 29, FOLLOW_FSUBTRACT_in_add_expression1650);
					rewriteRuleNodeStream5.Add(commonTree12);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree13 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression1652);
					rewriteRuleNodeStream3.Add(commonTree13);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1656);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1660);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_add_expression1664);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_add_expression1668);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child4);
					add_expression_return.kOptimizedTree = CompilerAutoFloatTwoArgMathOp((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, commonTree12.Token);
					if (add_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree13.Text);
						FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (add_expression_return2 != null) ? ((CommonTree)add_expression_return2.Tree) : null, (add_expression_return2 != null) ? add_expression_return2.kOptimizedTree : null, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (mult_expression_return != null) ? ((CommonTree)mult_expression_return.Tree) : null, (mult_expression_return != null) ? mult_expression_return.kOptimizedTree : null, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
					}
					add_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (add_expression_return != null) ? add_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (add_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, add_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree14 = (CommonTree)adaptor.GetNilNode();
						commonTree14 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream5.NextNode(), commonTree14);
						adaptor.AddChild(commonTree14, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree14, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree14, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree14, ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree14, ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
						adaptor.AddChild(commonTree, commonTree14);
					}
					add_expression_return.Tree = commonTree;
					break;
				}
				case 5:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child5 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el = (CommonTree)Match(input, 36, FOLLOW_STRCAT_in_add_expression1709);
					rewriteRuleNodeStream6.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree15 = (CommonTree)Match(input, 38, FOLLOW_ID_in_add_expression1711);
					rewriteRuleNodeStream3.Add(commonTree15);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1715);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_add_expression1719);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_add_expression_in_add_expression1723);
					var add_expression_return2 = add_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(add_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_add_expression1727);
					var mult_expression_return = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child5);
					add_expression_return.kOptimizedTree = CompilerAutoStrCat((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null);
					if (add_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree15.Text);
						FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (add_expression_return2 != null) ? ((CommonTree)add_expression_return2.Tree) : null, (add_expression_return2 != null) ? add_expression_return2.kOptimizedTree : null, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (mult_expression_return != null) ? ((CommonTree)mult_expression_return.Tree) : null, (mult_expression_return != null) ? mult_expression_return.kOptimizedTree : null, out ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB, out ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
					}
					add_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (add_expression_return != null) ? add_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (add_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, add_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree16 = (CommonTree)adaptor.GetNilNode();
						commonTree16 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream6.NextNode(), commonTree16);
						adaptor.AddChild(commonTree16, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree16, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree16, ((add_expression_scope)add_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree16, ((add_expression_scope)add_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree16, ((add_expression_scope)add_expression_stack.Peek()).kexpressionB);
						adaptor.AddChild(commonTree, commonTree16);
					}
					add_expression_return.Tree = commonTree;
					break;
				}
				case 6:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_add_expression1767);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, mult_expression_return2.Tree);
					add_expression_return.kOptimizedTree = ((mult_expression_return2 != null) ? mult_expression_return2.kOptimizedTree : null);
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

		// Token: 0x06000B10 RID: 2832 RVA: 0x0003BF0C File Offset: 0x0003A10C
		public mult_expression_return mult_expression()
		{
			mult_expression_stack.Push(new mult_expression_scope());
			var mult_expression_return = new mult_expression_return();
			mult_expression_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token FDIVIDE");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token FMULTIPLY");
			var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token IMULTIPLY");
			var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleNodeStream5 = new RewriteRuleNodeStream(adaptor, "token MOD");
			var rewriteRuleNodeStream6 = new RewriteRuleNodeStream(adaptor, "token IDIVIDE");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule unary_expression");
			var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule mult_expression");
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
					goto IL_1FF;
				case 30:
					num2 = 1;
					goto IL_217;
				case 31:
					num2 = 2;
					goto IL_217;
				case 32:
					num2 = 3;
					goto IL_217;
				case 33:
					num2 = 4;
					goto IL_217;
				default:
					if (num != 62)
					{
						switch (num)
						{
						case 77:
							num2 = 5;
							goto IL_217;
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
							goto IL_1FF;
						default:
							goto IL_1FF;
						}
					}
					break;
				}
				num2 = 6;
				goto IL_217;
				IL_1FF:
				var ex = new NoViableAltException("", 32, 0, input);
				throw ex;
				IL_217:
				switch (num2)
				{
				case 1:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 30, FOLLOW_IMULTIPLY_in_mult_expression1794);
					rewriteRuleNodeStream3.Add(commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression1796);
					rewriteRuleNodeStream4.Add(commonTree4);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_mult_expression1800);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_mult_expression1804);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_mult_expression1808);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_unary_expression_in_mult_expression1812);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(unary_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					mult_expression_return.kOptimizedTree = CompilerAutoIntTwoArgMathOp((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, commonTree3.Token);
					if (mult_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
						FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (mult_expression_return2 != null) ? ((CommonTree)mult_expression_return2.Tree) : null, (mult_expression_return2 != null) ? mult_expression_return2.kOptimizedTree : null, out ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeA, out ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (unary_expression_return != null) ? ((CommonTree)unary_expression_return.Tree) : null, (unary_expression_return != null) ? unary_expression_return.kOptimizedTree : null, out ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeB, out ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionB);
					}
					mult_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (mult_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, mult_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree5 = (CommonTree)adaptor.GetNilNode();
						commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream3.NextNode(), commonTree5);
						adaptor.AddChild(commonTree5, rewriteRuleNodeStream4.NextNode());
						adaptor.AddChild(commonTree5, ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree5, ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree5, ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree5, ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionB);
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
					var commonTree6 = (CommonTree)Match(input, 31, FOLLOW_FMULTIPLY_in_mult_expression1853);
					rewriteRuleNodeStream2.Add(commonTree6);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree7 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression1855);
					rewriteRuleNodeStream4.Add(commonTree7);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_mult_expression1859);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_mult_expression1863);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_mult_expression1867);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_unary_expression_in_mult_expression1871);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(unary_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child2);
					mult_expression_return.kOptimizedTree = CompilerAutoFloatTwoArgMathOp((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, commonTree6.Token);
					if (mult_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree7.Text);
						FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (mult_expression_return2 != null) ? ((CommonTree)mult_expression_return2.Tree) : null, (mult_expression_return2 != null) ? mult_expression_return2.kOptimizedTree : null, out ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeA, out ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (unary_expression_return != null) ? ((CommonTree)unary_expression_return.Tree) : null, (unary_expression_return != null) ? unary_expression_return.kOptimizedTree : null, out ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeB, out ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionB);
					}
					mult_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (mult_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, mult_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree8 = (CommonTree)adaptor.GetNilNode();
						commonTree8 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree8);
						adaptor.AddChild(commonTree8, rewriteRuleNodeStream4.NextNode());
						adaptor.AddChild(commonTree8, ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree8, ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree8, ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree8, ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionB);
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
					var commonTree9 = (CommonTree)Match(input, 32, FOLLOW_IDIVIDE_in_mult_expression1912);
					rewriteRuleNodeStream6.Add(commonTree9);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree10 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression1914);
					rewriteRuleNodeStream4.Add(commonTree10);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_mult_expression1918);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_mult_expression1922);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_mult_expression1926);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_unary_expression_in_mult_expression1930);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(unary_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child3);
					mult_expression_return.kOptimizedTree = CompilerAutoIntTwoArgMathOp((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, commonTree9.Token);
					if (mult_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree10.Text);
						FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (mult_expression_return2 != null) ? ((CommonTree)mult_expression_return2.Tree) : null, (mult_expression_return2 != null) ? mult_expression_return2.kOptimizedTree : null, out ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeA, out ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (unary_expression_return != null) ? ((CommonTree)unary_expression_return.Tree) : null, (unary_expression_return != null) ? unary_expression_return.kOptimizedTree : null, out ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeB, out ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionB);
					}
					mult_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (mult_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, mult_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree11 = (CommonTree)adaptor.GetNilNode();
						commonTree11 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream6.NextNode(), commonTree11);
						adaptor.AddChild(commonTree11, rewriteRuleNodeStream4.NextNode());
						adaptor.AddChild(commonTree11, ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree11, ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree11, ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree11, ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionB);
						adaptor.AddChild(commonTree, commonTree11);
					}
					mult_expression_return.Tree = commonTree;
					break;
				}
				case 4:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child4 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree12 = (CommonTree)Match(input, 33, FOLLOW_FDIVIDE_in_mult_expression1971);
					rewriteRuleNodeStream.Add(commonTree12);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree13 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression1973);
					rewriteRuleNodeStream4.Add(commonTree13);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_mult_expression1977);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_mult_expression1981);
					var autoCast_return2 = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(autoCast_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_mult_expression1985);
					var mult_expression_return2 = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_unary_expression_in_mult_expression1989);
					var unary_expression_return = unary_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(unary_expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child4);
					mult_expression_return.kOptimizedTree = CompilerAutoFloatTwoArgMathOp((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, commonTree12.Token);
					if (mult_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree13.Text);
						FixUpAutoCastAndExpression((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null, (mult_expression_return2 != null) ? ((CommonTree)mult_expression_return2.Tree) : null, (mult_expression_return2 != null) ? mult_expression_return2.kOptimizedTree : null, out ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeA, out ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionA);
						FixUpAutoCastAndExpression((autoCast_return2 != null) ? ((CommonTree)autoCast_return2.Tree) : null, (autoCast_return2 != null) ? autoCast_return2.kOptimizedTree : null, (unary_expression_return != null) ? ((CommonTree)unary_expression_return.Tree) : null, (unary_expression_return != null) ? unary_expression_return.kOptimizedTree : null, out ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeB, out ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionB);
					}
					mult_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (mult_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, mult_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree14 = (CommonTree)adaptor.GetNilNode();
						commonTree14 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree14);
						adaptor.AddChild(commonTree14, rewriteRuleNodeStream4.NextNode());
						adaptor.AddChild(commonTree14, ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeA);
						adaptor.AddChild(commonTree14, ((mult_expression_scope)mult_expression_stack.Peek()).kautoCastTreeB);
						adaptor.AddChild(commonTree14, ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionA);
						adaptor.AddChild(commonTree14, ((mult_expression_scope)mult_expression_stack.Peek()).kexpressionB);
						adaptor.AddChild(commonTree, commonTree14);
					}
					mult_expression_return.Tree = commonTree;
					break;
				}
				case 5:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child5 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree15 = (CommonTree)Match(input, 77, FOLLOW_MOD_in_mult_expression2030);
					rewriteRuleNodeStream5.Add(commonTree15);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree16 = (CommonTree)Match(input, 38, FOLLOW_ID_in_mult_expression2032);
					rewriteRuleNodeStream4.Add(commonTree16);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_mult_expression_in_mult_expression2036);
					var mult_expression_return3 = mult_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(mult_expression_return3.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_unary_expression_in_mult_expression2038);
					var unary_expression_return2 = unary_expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(unary_expression_return2.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child5);
					mult_expression_return.kOptimizedTree = CompilerAutoIntTwoArgMathOp((mult_expression_return3 != null) ? ((CommonTree)mult_expression_return3.Tree) : null, (unary_expression_return2 != null) ? ((CommonTree)unary_expression_return2.Tree) : null, commonTree15.Token);
					if (mult_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree16.Text);
					}
					mult_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (mult_expression_return != null) ? mult_expression_return.Tree : null);
					var rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule child_mult", (mult_expression_return3 != null) ? mult_expression_return3.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (mult_expression_return.kOptimizedTree != null)
					{
						adaptor.AddChild(commonTree, mult_expression_return.kOptimizedTree);
					}
					else
					{
						var commonTree17 = (CommonTree)adaptor.GetNilNode();
						commonTree17 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream5.NextNode(), commonTree17);
						adaptor.AddChild(commonTree17, rewriteRuleNodeStream4.NextNode());
						adaptor.AddChild(commonTree17, rewriteRuleSubtreeStream4.NextTree());
						adaptor.AddChild(commonTree17, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(commonTree, commonTree17);
					}
					mult_expression_return.Tree = commonTree;
					break;
				}
				case 6:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_unary_expression_in_mult_expression2075);
					var unary_expression_return3 = unary_expression();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, unary_expression_return3.Tree);
					mult_expression_return.kOptimizedTree = ((unary_expression_return3 != null) ? unary_expression_return3.kOptimizedTree : null);
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

		// Token: 0x06000B11 RID: 2833 RVA: 0x0003D644 File Offset: 0x0003B844
		public unary_expression_return unary_expression()
		{
			var unary_expression_return = new unary_expression_return
            {
                Start = input.LT(1)
            };
            CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token NOT");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token INEGATE");
			var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token FNEGATE");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule cast_atom");
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
						goto IL_176;
					default:
						switch (num)
						{
						case 34:
							num2 = 1;
							goto IL_18E;
						case 35:
							num2 = 2;
							goto IL_18E;
						case 36:
						case 37:
							goto IL_176;
						case 38:
							break;
						default:
							goto IL_176;
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
						goto IL_18E;
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
						goto IL_176;
					default:
						goto IL_176;
					}
				}
				num2 = 4;
				goto IL_18E;
				IL_176:
				var ex = new NoViableAltException("", 33, 0, input);
				throw ex;
				IL_18E:
				switch (num2)
				{
				case 1:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el = (CommonTree)Match(input, 34, FOLLOW_INEGATE_in_unary_expression2098);
					rewriteRuleNodeStream2.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_unary_expression2100);
					rewriteRuleNodeStream3.Add(commonTree3);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_cast_atom_in_unary_expression2102);
					var cast_atom_return = cast_atom();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(cast_atom_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					unary_expression_return.kOptimizedTree = CompilerAutoNegate((cast_atom_return != null) ? ((CommonTree)cast_atom_return.Tree) : null);
					if (unary_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree3.Text);
					}
					unary_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (unary_expression_return != null) ? unary_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (unary_expression_return.kOptimizedTree == null)
					{
						var commonTree4 = (CommonTree)adaptor.GetNilNode();
						commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream2.NextNode(), commonTree4);
						adaptor.AddChild(commonTree4, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree4);
					}
					else
					{
						adaptor.AddChild(commonTree, unary_expression_return.kOptimizedTree);
					}
					unary_expression_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child2 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el2 = (CommonTree)Match(input, 35, FOLLOW_FNEGATE_in_unary_expression2138);
					rewriteRuleNodeStream4.Add(el2);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_unary_expression2140);
					rewriteRuleNodeStream3.Add(commonTree5);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_cast_atom_in_unary_expression2142);
					var cast_atom_return2 = cast_atom();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(cast_atom_return2.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child2);
					unary_expression_return.kOptimizedTree = CompilerAutoNegate((cast_atom_return2 != null) ? ((CommonTree)cast_atom_return2.Tree) : null);
					if (unary_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					}
					unary_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (unary_expression_return != null) ? unary_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (unary_expression_return.kOptimizedTree == null)
					{
						var commonTree6 = (CommonTree)adaptor.GetNilNode();
						commonTree6 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream4.NextNode(), commonTree6);
						adaptor.AddChild(commonTree6, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree6, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree6);
					}
					else
					{
						adaptor.AddChild(commonTree, unary_expression_return.kOptimizedTree);
					}
					unary_expression_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el3 = (CommonTree)Match(input, 78, FOLLOW_NOT_in_unary_expression2178);
					rewriteRuleNodeStream.Add(el3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree7 = (CommonTree)Match(input, 38, FOLLOW_ID_in_unary_expression2180);
					rewriteRuleNodeStream3.Add(commonTree7);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_cast_atom_in_unary_expression2182);
					var cast_atom_return3 = cast_atom();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(cast_atom_return3.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child3);
					unary_expression_return.kOptimizedTree = CompilerAutoNot((cast_atom_return3 != null) ? ((CommonTree)cast_atom_return3.Tree) : null);
					if (unary_expression_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree7.Text);
					}
					unary_expression_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (unary_expression_return != null) ? unary_expression_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (unary_expression_return.kOptimizedTree == null)
					{
						var commonTree8 = (CommonTree)adaptor.GetNilNode();
						commonTree8 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree8);
						adaptor.AddChild(commonTree8, rewriteRuleNodeStream3.NextNode());
						adaptor.AddChild(commonTree8, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree8);
					}
					else
					{
						adaptor.AddChild(commonTree, unary_expression_return.kOptimizedTree);
					}
					unary_expression_return.Tree = commonTree;
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_cast_atom_in_unary_expression2217);
					var cast_atom_return4 = cast_atom();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, cast_atom_return4.Tree);
					unary_expression_return.kOptimizedTree = ((cast_atom_return4 != null) ? cast_atom_return4.kOptimizedTree : null);
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
			return unary_expression_return;
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x0003DEEC File Offset: 0x0003C0EC
		public cast_atom_return cast_atom()
		{
			var cast_atom_return = new cast_atom_return();
			cast_atom_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token AS");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule dot_atom");
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
						var ex = new NoViableAltException("", 34, 0, input);
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
					var el = (CommonTree)Match(input, 79, FOLLOW_AS_in_cast_atom2240);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_cast_atom2242);
					rewriteRuleNodeStream2.Add(commonTree3);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_dot_atom_in_cast_atom2244);
					var dot_atom_return = dot_atom();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(dot_atom_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					cast_atom_return.kOptimizedTree = CompilerAutoCast(commonTree3.Text, ((CommonTree)dot_atom_return.Tree), ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope);
					if (cast_atom_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree3.Text);
					}
					cast_atom_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (cast_atom_return != null) ? cast_atom_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (cast_atom_return.kOptimizedTree == null)
					{
						var commonTree4 = (CommonTree)adaptor.GetNilNode();
						commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
						adaptor.AddChild(commonTree4, rewriteRuleNodeStream2.NextNode());
						adaptor.AddChild(commonTree4, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree4);
					}
					else
					{
						adaptor.AddChild(commonTree, cast_atom_return.kOptimizedTree);
					}
					cast_atom_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_dot_atom_in_cast_atom2279);
					var dot_atom_return2 = dot_atom();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, dot_atom_return2.Tree);
					cast_atom_return.kOptimizedTree = ((dot_atom_return2 != null) ? dot_atom_return2.kOptimizedTree : null);
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

		// Token: 0x06000B13 RID: 2835 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
		public dot_atom_return dot_atom()
		{
			var dot_atom_return = new dot_atom_return();
			dot_atom_return.Start = input.LT(1);
			CommonTree commonTree = null;
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
						goto IL_D9;
					default:
						if (num != 38)
						{
							goto IL_D9;
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
							goto IL_CF;
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
								goto IL_D9;
							}
							break;
						}
						num2 = 3;
						goto IL_F1;
					}
					num2 = 1;
					goto IL_F1;
				}
				IL_CF:
				num2 = 2;
				goto IL_F1;
				IL_D9:
				var ex = new NoViableAltException("", 35, 0, input);
				throw ex;
				IL_F1:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 62, FOLLOW_DOT_in_dot_atom2302);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_dot_atom_in_dot_atom2306);
					var dot_atom_return2 = dot_atom();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, dot_atom_return2.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_array_func_or_id_in_dot_atom2308);
					var array_func_or_id_return = array_func_or_id();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, array_func_or_id_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree3);
					dot_atom_return.kOptimizedTree = ((dot_atom_return2 != null) ? dot_atom_return2.kOptimizedTree : null);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_array_atom_in_dot_atom2320);
					var array_atom_return = array_atom();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, array_atom_return.Tree);
					dot_atom_return.kOptimizedTree = ((array_atom_return != null) ? array_atom_return.kOptimizedTree : null);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_constant_in_dot_atom2331);
					var constant_return = constant();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, constant_return.Tree);
					dot_atom_return.kOptimizedTree = ((constant_return != null) ? ((CommonTree)constant_return.Tree) : null);
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

		// Token: 0x06000B14 RID: 2836 RVA: 0x0003E6A0 File Offset: 0x0003C8A0
		public array_atom_return array_atom()
		{
			var array_atom_return = new array_atom_return();
			array_atom_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token ARRAYGET");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule atom");
			var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
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
						var ex = new NoViableAltException("", 36, 0, input);
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
					var el = (CommonTree)Match(input, 22, FOLLOW_ARRAYGET_in_array_atom2355);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_array_atom2359);
					rewriteRuleNodeStream2.Add(commonTree3);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_array_atom2363);
					rewriteRuleNodeStream2.Add(commonTree4);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_array_atom2365);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_atom_in_array_atom2367);
					var atom_return = atom();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(atom_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_array_atom2369);
					var expression_return = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree3.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					array_atom_return.Tree = commonTree;
					var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token self", commonTree4);
					var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token retVal", commonTree3);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (array_atom_return != null) ? array_atom_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree5 = (CommonTree)adaptor.GetNilNode();
					commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
					adaptor.AddChild(commonTree5, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree5, rewriteRuleNodeStream3.NextNode());
					adaptor.AddChild(commonTree5, FixUpAutoCast((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (expression_return != null) ? expression_return.kOptimizedTree : null));
					adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream2.NextTree());
					adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
					adaptor.AddChild(commonTree, commonTree5);
					array_atom_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_atom_in_array_atom2402);
					var atom_return2 = atom();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, atom_return2.Tree);
					array_atom_return.kOptimizedTree = ((atom_return2 != null) ? atom_return2.kOptimizedTree : null);
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
			return array_atom_return;
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0003EBAC File Offset: 0x0003CDAC
		public atom_return atom()
		{
			var atom_return = new atom_return();
			atom_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token PAREXPR");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
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
						goto IL_D6;
					case 15:
						num2 = 1;
						goto IL_EE;
					default:
						if (num != 20)
						{
							goto IL_D6;
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
								goto IL_EE;
							case 81:
								goto IL_D6;
							case 82:
								break;
							default:
								goto IL_D6;
							}
						}
						break;
					}
				}
				num2 = 3;
				goto IL_EE;
				IL_D6:
				var ex = new NoViableAltException("", 37, 0, input);
				throw ex;
				IL_EE:
				switch (num2)
				{
				case 1:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el = (CommonTree)Match(input, 15, FOLLOW_PAREXPR_in_atom2425);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_atom2427);
					var expression_return = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					atom_return.kOptimizedTree = ((expression_return != null) ? expression_return.kOptimizedTree : null);
					if (atom_return.kOptimizedTree != null)
					{
						bMadeChanges = true;
					}
					atom_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (atom_return != null) ? atom_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (((expression_return != null) ? expression_return.kOptimizedTree : null) == null)
					{
						var commonTree3 = (CommonTree)adaptor.GetNilNode();
						commonTree3 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree3);
						adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree3);
					}
					else
					{
						adaptor.AddChild(commonTree, (expression_return != null) ? expression_return.kOptimizedTree : null);
					}
					atom_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 80, FOLLOW_NEW_in_atom2461);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree4 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree4);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 81, FOLLOW_INTEGER_in_atom2463);
					var child2 = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree4, child2);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_atom2465);
					var child3 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree4, child3);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree4);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_func_or_id_in_atom2477);
					var func_or_id_return = func_or_id();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, func_or_id_return.Tree);
					break;
				}
				}
				atom_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return atom_return;
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x0003F0A0 File Offset: 0x0003D2A0
		public array_func_or_id_return array_func_or_id()
		{
			var array_func_or_id_return = new array_func_or_id_return();
			array_func_or_id_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token ARRAYGET");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			var rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule func_or_id");
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
					if (num is < 11 or > 13 && (num != 20 && num is (< 24 or > 25) and not 38 and not 82))
					{
						var ex = new NoViableAltException("", 38, 0, input);
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
					var el = (CommonTree)Match(input, 22, FOLLOW_ARRAYGET_in_array_func_or_id2491);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)Match(input, 38, FOLLOW_ID_in_array_func_or_id2495);
					rewriteRuleNodeStream2.Add(commonTree3);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_array_func_or_id2499);
					rewriteRuleNodeStream2.Add(commonTree4);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_array_func_or_id2501);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_func_or_id_in_array_func_or_id2503);
					var func_or_id_return = func_or_id();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream3.Add(func_or_id_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_array_func_or_id2505);
					var expression_return = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree3.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					array_func_or_id_return.Tree = commonTree;
					var rewriteRuleNodeStream3 = new RewriteRuleNodeStream(adaptor, "token self", commonTree4);
					var rewriteRuleNodeStream4 = new RewriteRuleNodeStream(adaptor, "token retVal", commonTree3);
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (array_func_or_id_return != null) ? array_func_or_id_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree5 = (CommonTree)adaptor.GetNilNode();
					commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
					adaptor.AddChild(commonTree5, rewriteRuleNodeStream4.NextNode());
					adaptor.AddChild(commonTree5, rewriteRuleNodeStream3.NextNode());
					adaptor.AddChild(commonTree5, FixUpAutoCast((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (expression_return != null) ? expression_return.kOptimizedTree : null));
					adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream3.NextTree());
					adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
					adaptor.AddChild(commonTree, commonTree5);
					array_func_or_id_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_func_or_id_in_array_func_or_id2538);
					var func_or_id_return2 = func_or_id();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, func_or_id_return2.Tree);
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
			return array_func_or_id_return;
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x0003F58C File Offset: 0x0003D78C
		public func_or_id_return func_or_id()
		{
			var func_or_id_return = new func_or_id_return();
			func_or_id_return.Start = input.LT(1);
			CommonTree commonTree = null;
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
							goto IL_AB;
						}
						num2 = 2;
						goto IL_C3;
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
							goto IL_C3;
						}
						if (num != 82)
						{
							goto IL_AB;
						}
						num2 = 4;
						goto IL_C3;
					}
				}
				num2 = 1;
				goto IL_C3;
				IL_AB:
				var ex = new NoViableAltException("", 39, 0, input);
				throw ex;
				IL_C3:
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_function_call_in_func_or_id2551);
					var function_call_return = function_call();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, function_call_return.Tree);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 20, FOLLOW_PROPGET_in_func_or_id2558);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id2562);
					var child = (CommonTree)adaptor.DupNode(commonTree4);
					adaptor.AddChild(commonTree3, child);
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id2564);
					var child2 = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree3, child2);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id2568);
					var child3 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree3, child3);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree3);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id2580);
					var child4 = (CommonTree)adaptor.DupNode(commonTree6);
					adaptor.AddChild(commonTree, child4);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree6.Text);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree7 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode3 = (CommonTree)Match(input, 82, FOLLOW_LENGTH_in_func_or_id2592);
					var newRoot2 = (CommonTree)adaptor.DupNode(treeNode3);
					commonTree7 = (CommonTree)adaptor.BecomeRoot(newRoot2, commonTree7);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id2596);
					var child = (CommonTree)adaptor.DupNode(commonTree4);
					adaptor.AddChild(commonTree7, child);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_func_or_id2600);
					var child3 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree7, child3);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree7);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
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
			return func_or_id_return;
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0003FB44 File Offset: 0x0003DD44
		public property_set_return property_set()
		{
			var property_set_return = new property_set_return();
			property_set_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var commonTree3 = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)Match(input, 21, FOLLOW_PROPSET_in_property_set2620);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_property_set2624);
				var child = (CommonTree)adaptor.DupNode(commonTree4);
				adaptor.AddChild(commonTree3, child);
				commonTree2 = (CommonTree)input.LT(1);
				var treeNode2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_property_set2626);
				var child2 = (CommonTree)adaptor.DupNode(treeNode2);
				adaptor.AddChild(commonTree3, child2);
				commonTree2 = (CommonTree)input.LT(1);
				var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_property_set2630);
				var child3 = (CommonTree)adaptor.DupNode(commonTree5);
				adaptor.AddChild(commonTree3, child3);
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
				((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
				((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
				property_set_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return property_set_return;
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0003FDC0 File Offset: 0x0003DFC0
		public return_stat_return return_stat()
		{
			var return_stat_return = new return_stat_return();
			return_stat_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token RETURN");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			try
			{
				var num = input.LA(1);
				if (num != 83)
				{
					var ex = new NoViableAltException("", 40, 0, input);
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
					if (num2 is not 3 and not 5 and (< 11 or > 13) and not 15 and not 20 and not 22 and (< 24 or > 36) and not 38 and not 41 and not 62 and (< 65 or > 72) and (< 77 or > 84) && num2 != 88 && num2 is < 90 or > 93)
					{
						var ex2 = new NoViableAltException("", 40, 1, input);
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
					var el = (CommonTree)Match(input, 83, FOLLOW_RETURN_in_return_stat2650);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_autoCast_in_return_stat2652);
					var autoCast_return = autoCast();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream2.Add(autoCast_return.Tree);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_expression_in_return_stat2654);
					var expression_return = expression();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(expression_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child);
					return_stat_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (return_stat_return != null) ? return_stat_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (((expression_return != null) ? expression_return.kOptimizedTree : null) == null)
					{
						var commonTree3 = (CommonTree)adaptor.GetNilNode();
						commonTree3 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree3);
						adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree3);
					}
					else if (((autoCast_return != null) ? autoCast_return.kOptimizedTree : null) != null)
					{
						var commonTree4 = (CommonTree)adaptor.GetNilNode();
						commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
						adaptor.AddChild(commonTree4, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null);
						adaptor.AddChild(commonTree4, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null);
						adaptor.AddChild(commonTree, commonTree4);
					}
					else
					{
						var commonTree5 = (CommonTree)adaptor.GetNilNode();
						commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
						adaptor.AddChild(commonTree5, FixUpAutoCast((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (expression_return != null) ? expression_return.kOptimizedTree : null));
						adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree5);
					}
					return_stat_return.Tree = commonTree;
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 83, FOLLOW_RETURN_in_return_stat2704);
					var child2 = (CommonTree)adaptor.DupNode(treeNode);
					adaptor.AddChild(commonTree, child2);
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
			return return_stat_return;
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x000402A4 File Offset: 0x0003E4A4
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
				var treeNode = (CommonTree)Match(input, 84, FOLLOW_IF_in_ifBlock2732);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_expression_in_ifBlock2734);
				var expression_return = expression();
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, expression_return.Tree);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_codeBlock_in_ifBlock2736);
				var codeBlock_return = codeBlock(((ifBlock_scope)ifBlock_stack.Peek()).kchildScope);
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, codeBlock_return.Tree);
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
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_elseIfBlock_in_ifBlock2740);
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
					PushFollow(FOLLOW_elseBlock_in_ifBlock2744);
					var elseBlock_return = elseBlock();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree3, elseBlock_return.Tree);
				}
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
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

		// Token: 0x06000B1B RID: 2843 RVA: 0x00040614 File Offset: 0x0003E814
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
				var treeNode = (CommonTree)Match(input, 86, FOLLOW_ELSEIF_in_elseIfBlock2768);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_expression_in_elseIfBlock2770);
				var expression_return = expression();
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, expression_return.Tree);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_codeBlock_in_elseIfBlock2772);
				var codeBlock_return = codeBlock(((elseIfBlock_scope)elseIfBlock_stack.Peek()).kchildScope);
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
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

		// Token: 0x06000B1C RID: 2844 RVA: 0x000408A0 File Offset: 0x0003EAA0
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
				var treeNode = (CommonTree)Match(input, 87, FOLLOW_ELSE_in_elseBlock2796);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_codeBlock_in_elseBlock2798);
				var codeBlock_return = codeBlock(((elseBlock_scope)elseBlock_stack.Peek()).kchildScope);
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

		// Token: 0x06000B1D RID: 2845 RVA: 0x00040ADC File Offset: 0x0003ECDC
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
				var treeNode = (CommonTree)Match(input, 88, FOLLOW_WHILE_in_whileBlock2828);
				var newRoot = (CommonTree)adaptor.DupNode(treeNode);
				commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_expression_in_whileBlock2830);
				var expression_return = expression();
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, expression_return.Tree);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_codeBlock_in_whileBlock2832);
				var codeBlock_return = codeBlock(((whileBlock_scope)whileBlock_stack.Peek()).kchildScope);
				state.followingStackPointer--;
				adaptor.AddChild(commonTree3, codeBlock_return.Tree);
				Match(input, 3, null);
				adaptor.AddChild(commonTree, commonTree3);
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

		// Token: 0x06000B1E RID: 2846 RVA: 0x00040D68 File Offset: 0x0003EF68
		public function_call_return function_call()
		{
			var function_call_return = new function_call_return();
			function_call_return.Start = input.LT(1);
			CommonTree commonTree = null;
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
						var ex = new NoViableAltException("", 48, 0, input);
						throw ex;
					}
					}
					break;
				}
				switch (num2)
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 11, FOLLOW_CALL_in_function_call2848);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2852);
					var child = (CommonTree)adaptor.DupNode(commonTree4);
					adaptor.AddChild(commonTree3, child);
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2856);
					var child2 = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree3, child2);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2860);
					var child3 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree3, child3);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree6 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode3 = (CommonTree)Match(input, 14, FOLLOW_CALLPARAMS_in_function_call2863);
					var newRoot2 = (CommonTree)adaptor.DupNode(treeNode3);
					commonTree6 = (CommonTree)adaptor.BecomeRoot(newRoot2, commonTree6);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num3 = 2;
						var num4 = input.LA(1);
						if (num4 == 9)
						{
							num3 = 1;
						}
						num = num3;
						if (num == 1)
						{
							commonTree2 = (CommonTree)input.LT(1);
							PushFollow(FOLLOW_parameters_in_function_call2865);
							var parameters_return = parameters();
							state.followingStackPointer--;
							adaptor.AddChild(commonTree6, parameters_return.Tree);
						}
						Match(input, 3, null);
					}
					adaptor.AddChild(commonTree3, commonTree6);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree3);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree7 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode4 = (CommonTree)Match(input, 13, FOLLOW_CALLPARENT_in_function_call2880);
					var newRoot3 = (CommonTree)adaptor.DupNode(treeNode4);
					commonTree7 = (CommonTree)adaptor.BecomeRoot(newRoot3, commonTree7);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2884);
					var child = (CommonTree)adaptor.DupNode(commonTree4);
					adaptor.AddChild(commonTree7, child);
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2888);
					var child2 = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree7, child2);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2892);
					var child3 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree7, child3);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree8 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode5 = (CommonTree)Match(input, 14, FOLLOW_CALLPARAMS_in_function_call2895);
					var newRoot4 = (CommonTree)adaptor.DupNode(treeNode5);
					commonTree8 = (CommonTree)adaptor.BecomeRoot(newRoot4, commonTree8);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num5 = 2;
						var num6 = input.LA(1);
						if (num6 == 9)
						{
							num5 = 1;
						}
						num = num5;
						if (num == 1)
						{
							commonTree2 = (CommonTree)input.LT(1);
							PushFollow(FOLLOW_parameters_in_function_call2897);
							var parameters_return2 = parameters();
							state.followingStackPointer--;
							adaptor.AddChild(commonTree8, parameters_return2.Tree);
						}
						Match(input, 3, null);
					}
					adaptor.AddChild(commonTree7, commonTree8);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree7);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree9 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode6 = (CommonTree)Match(input, 12, FOLLOW_CALLGLOBAL_in_function_call2912);
					var newRoot5 = (CommonTree)adaptor.DupNode(treeNode6);
					commonTree9 = (CommonTree)adaptor.BecomeRoot(newRoot5, commonTree9);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode7 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2916);
					var child4 = (CommonTree)adaptor.DupNode(treeNode7);
					adaptor.AddChild(commonTree9, child4);
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2920);
					var child2 = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree9, child2);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2924);
					var child3 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree9, child3);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree10 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode8 = (CommonTree)Match(input, 14, FOLLOW_CALLPARAMS_in_function_call2927);
					var newRoot6 = (CommonTree)adaptor.DupNode(treeNode8);
					commonTree10 = (CommonTree)adaptor.BecomeRoot(newRoot6, commonTree10);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num7 = 2;
						var num8 = input.LA(1);
						if (num8 == 9)
						{
							num7 = 1;
						}
						num = num7;
						if (num == 1)
						{
							commonTree2 = (CommonTree)input.LT(1);
							PushFollow(FOLLOW_parameters_in_function_call2929);
							var parameters_return3 = parameters();
							state.followingStackPointer--;
							adaptor.AddChild(commonTree10, parameters_return3.Tree);
						}
						Match(input, 3, null);
					}
					adaptor.AddChild(commonTree9, commonTree10);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree9);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree11 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode9 = (CommonTree)Match(input, 24, FOLLOW_ARRAYFIND_in_function_call2944);
					var newRoot7 = (CommonTree)adaptor.DupNode(treeNode9);
					commonTree11 = (CommonTree)adaptor.BecomeRoot(newRoot7, commonTree11);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2948);
					var child = (CommonTree)adaptor.DupNode(commonTree4);
					adaptor.AddChild(commonTree11, child);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2952);
					var child3 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree11, child3);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree12 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode10 = (CommonTree)Match(input, 14, FOLLOW_CALLPARAMS_in_function_call2955);
					var newRoot8 = (CommonTree)adaptor.DupNode(treeNode10);
					commonTree12 = (CommonTree)adaptor.BecomeRoot(newRoot8, commonTree12);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num9 = 2;
						var num10 = input.LA(1);
						if (num10 == 9)
						{
							num9 = 1;
						}
						num = num9;
						if (num == 1)
						{
							commonTree2 = (CommonTree)input.LT(1);
							PushFollow(FOLLOW_parameters_in_function_call2957);
							var parameters_return4 = parameters();
							state.followingStackPointer--;
							adaptor.AddChild(commonTree12, parameters_return4.Tree);
						}
						Match(input, 3, null);
					}
					adaptor.AddChild(commonTree11, commonTree12);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree11);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					break;
				}
				case 5:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree13 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode11 = (CommonTree)Match(input, 25, FOLLOW_ARRAYRFIND_in_function_call2972);
					var newRoot9 = (CommonTree)adaptor.DupNode(treeNode11);
					commonTree13 = (CommonTree)adaptor.BecomeRoot(newRoot9, commonTree13);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2976);
					var child = (CommonTree)adaptor.DupNode(commonTree4);
					adaptor.AddChild(commonTree13, child);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_function_call2980);
					var child3 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree13, child3);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree14 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode12 = (CommonTree)Match(input, 14, FOLLOW_CALLPARAMS_in_function_call2983);
					var newRoot10 = (CommonTree)adaptor.DupNode(treeNode12);
					commonTree14 = (CommonTree)adaptor.BecomeRoot(newRoot10, commonTree14);
					if (input.LA(1) == 2)
					{
						Match(input, 2, null);
						var num11 = 2;
						var num12 = input.LA(1);
						if (num12 == 9)
						{
							num11 = 1;
						}
						num = num11;
						if (num == 1)
						{
							commonTree2 = (CommonTree)input.LT(1);
							PushFollow(FOLLOW_parameters_in_function_call2985);
							var parameters_return5 = parameters();
							state.followingStackPointer--;
							adaptor.AddChild(commonTree14, parameters_return5.Tree);
						}
						Match(input, 3, null);
					}
					adaptor.AddChild(commonTree13, commonTree14);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree13);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					break;
				}
				}
				function_call_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return function_call_return;
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x00041DAC File Offset: 0x0003FFAC
		public parameters_return parameters()
		{
			var parameters_return = new parameters_return();
			parameters_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
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
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_parameter_in_parameters3006);
					var parameter_return = parameter();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, parameter_return.Tree);
					num++;
				}
				if (num < 1)
				{
					var ex = new EarlyExitException(49, input);
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

		// Token: 0x06000B20 RID: 2848 RVA: 0x00041EBC File Offset: 0x000400BC
		public parameter_return parameter()
		{
			var parameter_return = new parameter_return();
			parameter_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token PARAM");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule expression");
			var rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule autoCast");
			try
			{
				var commonTree2 = (CommonTree)input.LT(1);
				var child = (CommonTree)adaptor.GetNilNode();
				commonTree2 = (CommonTree)input.LT(1);
				var el = (CommonTree)Match(input, 9, FOLLOW_PARAM_in_parameter3021);
				rewriteRuleNodeStream.Add(el);
				Match(input, 2, null);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_autoCast_in_parameter3023);
				var autoCast_return = autoCast();
				state.followingStackPointer--;
				rewriteRuleSubtreeStream2.Add(autoCast_return.Tree);
				commonTree2 = (CommonTree)input.LT(1);
				PushFollow(FOLLOW_expression_in_parameter3025);
				var expression_return = expression();
				state.followingStackPointer--;
				rewriteRuleSubtreeStream.Add(expression_return.Tree);
				Match(input, 3, null);
				adaptor.AddChild(commonTree, child);
				parameter_return.Tree = commonTree;
				new RewriteRuleSubtreeStream(adaptor, "rule retval", (parameter_return != null) ? parameter_return.Tree : null);
				commonTree = (CommonTree)adaptor.GetNilNode();
				if (((expression_return != null) ? expression_return.kOptimizedTree : null) == null)
				{
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree3 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree3);
					adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream2.NextTree());
					adaptor.AddChild(commonTree3, rewriteRuleSubtreeStream.NextTree());
					adaptor.AddChild(commonTree, commonTree3);
				}
				else if (((autoCast_return != null) ? autoCast_return.kOptimizedTree : null) != null)
				{
					var commonTree4 = (CommonTree)adaptor.GetNilNode();
					commonTree4 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree4);
					adaptor.AddChild(commonTree4, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null);
					adaptor.AddChild(commonTree4, (autoCast_return != null) ? autoCast_return.kOptimizedTree : null);
					adaptor.AddChild(commonTree, commonTree4);
				}
				else
				{
					var commonTree5 = (CommonTree)adaptor.GetNilNode();
					commonTree5 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree5);
					adaptor.AddChild(commonTree5, FixUpAutoCast((autoCast_return != null) ? ((CommonTree)autoCast_return.Tree) : null, (expression_return != null) ? expression_return.kOptimizedTree : null));
					adaptor.AddChild(commonTree5, rewriteRuleSubtreeStream.NextTree());
					adaptor.AddChild(commonTree, commonTree5);
				}
				parameter_return.Tree = commonTree;
				parameter_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex)
			{
				ReportError(ex);
				Recover(input, ex);
			}
			return parameter_return;
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x00042248 File Offset: 0x00040448
		public autoCast_return autoCast()
		{
			var autoCast_return = new autoCast_return();
			autoCast_return.Start = input.LT(1);
			CommonTree commonTree = null;
			var rewriteRuleNodeStream = new RewriteRuleNodeStream(adaptor, "token AS");
			var rewriteRuleNodeStream2 = new RewriteRuleNodeStream(adaptor, "token ID");
			var rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule constant");
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
							var ex = new NoViableAltException("", 50, 1, input);
							throw ex;
						}
						var num3 = input.LA(3);
						if (num3 != 38)
						{
							var ex2 = new NoViableAltException("", 50, 4, input);
							throw ex2;
						}
						var num4 = input.LA(4);
						if (num4 == 38)
						{
							num5 = 1;
							goto IL_188;
						}
						if (num4 is 81 or >= 90 and <= 93)
						{
							num5 = 2;
							goto IL_188;
						}
						var ex3 = new NoViableAltException("", 50, 5, input);
						throw ex3;
					}
					case 80:
						goto IL_170;
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
							goto IL_170;
						}
						break;
					}
					num5 = 4;
					goto IL_188;
					IL_170:
					var ex4 = new NoViableAltException("", 50, 0, input);
					throw ex4;
				}
				num5 = 3;
				IL_188:
				switch (num5)
				{
				case 1:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 79, FOLLOW_AS_in_autoCast3087);
					var newRoot = (CommonTree)adaptor.DupNode(treeNode);
					commonTree3 = (CommonTree)adaptor.BecomeRoot(newRoot, commonTree3);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree4 = (CommonTree)Match(input, 38, FOLLOW_ID_in_autoCast3091);
					var child = (CommonTree)adaptor.DupNode(commonTree4);
					adaptor.AddChild(commonTree3, child);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree5 = (CommonTree)Match(input, 38, FOLLOW_ID_in_autoCast3095);
					var child2 = (CommonTree)adaptor.DupNode(commonTree5);
					adaptor.AddChild(commonTree3, child2);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, commonTree3);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree4.Text);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree5.Text);
					break;
				}
				case 2:
				{
					var commonTree2 = (CommonTree)input.LT(1);
					var child3 = (CommonTree)adaptor.GetNilNode();
					commonTree2 = (CommonTree)input.LT(1);
					var el = (CommonTree)Match(input, 79, FOLLOW_AS_in_autoCast3108);
					rewriteRuleNodeStream.Add(el);
					Match(input, 2, null);
					commonTree2 = (CommonTree)input.LT(1);
					var commonTree6 = (CommonTree)Match(input, 38, FOLLOW_ID_in_autoCast3110);
					rewriteRuleNodeStream2.Add(commonTree6);
					commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_constant_in_autoCast3112);
					var constant_return = constant();
					state.followingStackPointer--;
					rewriteRuleSubtreeStream.Add(constant_return.Tree);
					Match(input, 3, null);
					adaptor.AddChild(commonTree, child3);
					autoCast_return.kOptimizedTree = CompilerAutoCast(commonTree6.Text, (constant_return != null) ? ((CommonTree)constant_return.Tree) : null, ((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope);
					if (autoCast_return.kOptimizedTree == null)
					{
						((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree6.Text);
					}
					autoCast_return.Tree = commonTree;
					new RewriteRuleSubtreeStream(adaptor, "rule retval", (autoCast_return != null) ? autoCast_return.Tree : null);
					commonTree = (CommonTree)adaptor.GetNilNode();
					if (autoCast_return.kOptimizedTree == null)
					{
						var commonTree7 = (CommonTree)adaptor.GetNilNode();
						commonTree7 = (CommonTree)adaptor.BecomeRoot(rewriteRuleNodeStream.NextNode(), commonTree7);
						adaptor.AddChild(commonTree7, rewriteRuleNodeStream2.NextNode());
						adaptor.AddChild(commonTree7, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, commonTree7);
					}
					else
					{
						adaptor.AddChild(commonTree, autoCast_return.kOptimizedTree);
					}
					autoCast_return.Tree = commonTree;
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					var commonTree8 = (CommonTree)Match(input, 38, FOLLOW_ID_in_autoCast3147);
					var child4 = (CommonTree)adaptor.DupNode(commonTree8);
					adaptor.AddChild(commonTree, child4);
					((codeBlock_scope)codeBlock_stack.Peek()).kcurrentScope.TryFlagVarAsUsed(commonTree8.Text);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree2 = (CommonTree)input.LT(1);
					PushFollow(FOLLOW_constant_in_autoCast3158);
					var constant_return2 = constant();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, constant_return2.Tree);
					break;
				}
				}
				autoCast_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex5)
			{
				ReportError(ex5);
				Recover(input, ex5);
			}
			return autoCast_return;
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x000428FC File Offset: 0x00040AFC
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
						var ex = new NoViableAltException("", 51, 0, input);
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
					PushFollow(FOLLOW_number_in_constant3171);
					var number_return = number();
					state.followingStackPointer--;
					adaptor.AddChild(commonTree, number_return.Tree);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree3 = (CommonTree)input.LT(1);
					var treeNode = (CommonTree)Match(input, 90, FOLLOW_STRING_in_constant3177);
					var child = (CommonTree)adaptor.DupNode(treeNode);
					adaptor.AddChild(commonTree, child);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree4 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 91, FOLLOW_BOOL_in_constant3183);
					var child2 = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree, child2);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree5 = (CommonTree)input.LT(1);
					var treeNode3 = (CommonTree)Match(input, 92, FOLLOW_NONE_in_constant3189);
					var child3 = (CommonTree)adaptor.DupNode(treeNode3);
					adaptor.AddChild(commonTree, child3);
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

		// Token: 0x06000B23 RID: 2851 RVA: 0x00042B88 File Offset: 0x00040D88
		public number_return number()
		{
			var number_return = new number_return();
			number_return.Start = input.LT(1);
			try
			{
				var commonTree = (CommonTree)adaptor.GetNilNode();
				var commonTree2 = (CommonTree)input.LT(1);
				var treeNode = (CommonTree)input.LT(1);
				if (input.LA(1) != 81 && input.LA(1) != 93)
				{
					var ex = new MismatchedSetException(null, input);
					throw ex;
				}
				input.Consume();
				var child = (CommonTree)adaptor.DupNode(treeNode);
				adaptor.AddChild(commonTree, child);
				state.errorRecovery = false;
				number_return.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
			}
			catch (RecognitionException ex2)
			{
				ReportError(ex2);
				Recover(input, ex2);
			}
			return number_return;
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x00042C98 File Offset: 0x00040E98
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
						if (num2 != 38)
						{
							var ex = new NoViableAltException("", 52, 1, input);
							throw ex;
						}
						num3 = 1;
					}
				}
				else
				{
					if (num != 55)
					{
						var ex2 = new NoViableAltException("", 52, 0, input);
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
							var ex3 = new NoViableAltException("", 52, 2, input);
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
					var treeNode = (CommonTree)Match(input, 38, FOLLOW_ID_in_type3221);
					var child = (CommonTree)adaptor.DupNode(treeNode);
					adaptor.AddChild(commonTree, child);
					break;
				}
				case 2:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree3 = (CommonTree)input.LT(1);
					var treeNode2 = (CommonTree)Match(input, 38, FOLLOW_ID_in_type3227);
					var child2 = (CommonTree)adaptor.DupNode(treeNode2);
					adaptor.AddChild(commonTree, child2);
					var commonTree4 = (CommonTree)input.LT(1);
					var treeNode3 = (CommonTree)Match(input, 63, FOLLOW_LBRACKET_in_type3229);
					var child3 = (CommonTree)adaptor.DupNode(treeNode3);
					adaptor.AddChild(commonTree, child3);
					var commonTree5 = (CommonTree)input.LT(1);
					var treeNode4 = (CommonTree)Match(input, 64, FOLLOW_RBRACKET_in_type3231);
					var child4 = (CommonTree)adaptor.DupNode(treeNode4);
					adaptor.AddChild(commonTree, child4);
					break;
				}
				case 3:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree6 = (CommonTree)input.LT(1);
					var treeNode5 = (CommonTree)Match(input, 55, FOLLOW_BASETYPE_in_type3237);
					var child5 = (CommonTree)adaptor.DupNode(treeNode5);
					adaptor.AddChild(commonTree, child5);
					break;
				}
				case 4:
				{
					commonTree = (CommonTree)adaptor.GetNilNode();
					var commonTree7 = (CommonTree)input.LT(1);
					var treeNode6 = (CommonTree)Match(input, 55, FOLLOW_BASETYPE_in_type3243);
					var child6 = (CommonTree)adaptor.DupNode(treeNode6);
					adaptor.AddChild(commonTree, child6);
					var commonTree8 = (CommonTree)input.LT(1);
					var treeNode7 = (CommonTree)Match(input, 63, FOLLOW_LBRACKET_in_type3245);
					var child7 = (CommonTree)adaptor.DupNode(treeNode7);
					adaptor.AddChild(commonTree, child7);
					var commonTree9 = (CommonTree)input.LT(1);
					var treeNode8 = (CommonTree)Match(input, 64, FOLLOW_RBRACKET_in_type3247);
					var child8 = (CommonTree)adaptor.DupNode(treeNode8);
					adaptor.AddChild(commonTree, child8);
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

		// Token: 0x06000B25 RID: 2853 RVA: 0x000430CC File Offset: 0x000412CC
		private void InitializeCyclicDFAs()
		{
			dfa25 = new DFA25(this);
			dfa26 = new DFA26(this);
		}

		// Token: 0x0400050B RID: 1291
		public const int FUNCTION = 6;

		// Token: 0x0400050C RID: 1292
		public const int LT = 70;

		// Token: 0x0400050D RID: 1293
		public const int WHILE = 88;

		// Token: 0x0400050E RID: 1294
		public const int DIVEQUALS = 60;

		// Token: 0x0400050F RID: 1295
		public const int MOD = 77;

		// Token: 0x04000510 RID: 1296
		public const int PROPSET = 21;

		// Token: 0x04000511 RID: 1297
		public const int NEW = 80;

		// Token: 0x04000512 RID: 1298
		public const int DQUOTE = 98;

		// Token: 0x04000513 RID: 1299
		public const int PARAM = 9;

		// Token: 0x04000514 RID: 1300
		public const int EQUALS = 41;

		// Token: 0x04000515 RID: 1301
		public const int NOT = 78;

		// Token: 0x04000516 RID: 1302
		public const int EOF = -1;

		// Token: 0x04000517 RID: 1303
		public const int FNEGATE = 35;

		// Token: 0x04000518 RID: 1304
		public const int LBRACKET = 63;

		// Token: 0x04000519 RID: 1305
		public const int USER_FLAGS = 18;

		// Token: 0x0400051A RID: 1306
		public const int RPAREN = 44;

		// Token: 0x0400051B RID: 1307
		public const int IMPORT = 42;

		// Token: 0x0400051C RID: 1308
		public const int EOL = 94;

		// Token: 0x0400051D RID: 1309
		public const int FADD = 27;

		// Token: 0x0400051E RID: 1310
		public const int RETURN = 83;

		// Token: 0x0400051F RID: 1311
		public const int ENDIF = 85;

		// Token: 0x04000520 RID: 1312
		public const int VAR = 5;

		// Token: 0x04000521 RID: 1313
		public const int ENDWHILE = 89;

		// Token: 0x04000522 RID: 1314
		public const int EQ = 67;

		// Token: 0x04000523 RID: 1315
		public const int IMULTIPLY = 30;

		// Token: 0x04000524 RID: 1316
		public const int COMMENT = 104;

		// Token: 0x04000525 RID: 1317
		public const int IDIVIDE = 32;

		// Token: 0x04000526 RID: 1318
		public const int DIVIDE = 76;

		// Token: 0x04000527 RID: 1319
		public const int NE = 68;

		// Token: 0x04000528 RID: 1320
		public const int SCRIPTNAME = 37;

		// Token: 0x04000529 RID: 1321
		public const int MINUSEQUALS = 58;

		// Token: 0x0400052A RID: 1322
		public const int ARRAYFIND = 24;

		// Token: 0x0400052B RID: 1323
		public const int RBRACE = 100;

		// Token: 0x0400052C RID: 1324
		public const int ELSE = 87;

		// Token: 0x0400052D RID: 1325
		public const int BOOL = 91;

		// Token: 0x0400052E RID: 1326
		public const int NATIVE = 47;

		// Token: 0x0400052F RID: 1327
		public const int FDIVIDE = 33;

		// Token: 0x04000530 RID: 1328
		public const int UNARY_MINUS = 16;

		// Token: 0x04000531 RID: 1329
		public const int MULT = 75;

		// Token: 0x04000532 RID: 1330
		public const int ENDPROPERTY = 53;

		// Token: 0x04000533 RID: 1331
		public const int CALLPARAMS = 14;

		// Token: 0x04000534 RID: 1332
		public const int ALPHA = 95;

		// Token: 0x04000535 RID: 1333
		public const int WS = 102;

		// Token: 0x04000536 RID: 1334
		public const int FMULTIPLY = 31;

		// Token: 0x04000537 RID: 1335
		public const int ARRAYSET = 23;

		// Token: 0x04000538 RID: 1336
		public const int PROPERTY = 54;

		// Token: 0x04000539 RID: 1337
		public const int AUTOREADONLY = 56;

		// Token: 0x0400053A RID: 1338
		public const int NONE = 92;

		// Token: 0x0400053B RID: 1339
		public const int OR = 65;

		// Token: 0x0400053C RID: 1340
		public const int PROPGET = 20;

		// Token: 0x0400053D RID: 1341
		public const int IADD = 26;

		// Token: 0x0400053E RID: 1342
		public const int PROPFUNC = 17;

		// Token: 0x0400053F RID: 1343
		public const int GT = 69;

		// Token: 0x04000540 RID: 1344
		public const int CALL = 11;

		// Token: 0x04000541 RID: 1345
		public const int INEGATE = 34;

		// Token: 0x04000542 RID: 1346
		public const int BASETYPE = 55;

		// Token: 0x04000543 RID: 1347
		public const int ENDEVENT = 48;

		// Token: 0x04000544 RID: 1348
		public const int MULTEQUALS = 59;

		// Token: 0x04000545 RID: 1349
		public const int CALLPARENT = 13;

		// Token: 0x04000546 RID: 1350
		public const int LBRACE = 99;

		// Token: 0x04000547 RID: 1351
		public const int GTE = 71;

		// Token: 0x04000548 RID: 1352
		public const int FLOAT = 93;

		// Token: 0x04000549 RID: 1353
		public const int ENDSTATE = 52;

		// Token: 0x0400054A RID: 1354
		public const int ID = 38;

		// Token: 0x0400054B RID: 1355
		public const int AND = 66;

		// Token: 0x0400054C RID: 1356
		public const int LTE = 72;

		// Token: 0x0400054D RID: 1357
		public const int LPAREN = 43;

		// Token: 0x0400054E RID: 1358
		public const int LENGTH = 82;

		// Token: 0x0400054F RID: 1359
		public const int IF = 84;

		// Token: 0x04000550 RID: 1360
		public const int CALLGLOBAL = 12;

		// Token: 0x04000551 RID: 1361
		public const int AS = 79;

		// Token: 0x04000552 RID: 1362
		public const int OBJECT = 4;

		// Token: 0x04000553 RID: 1363
		public const int COMMA = 49;

		// Token: 0x04000554 RID: 1364
		public const int PLUSEQUALS = 57;

		// Token: 0x04000555 RID: 1365
		public const int AUTO = 50;

		// Token: 0x04000556 RID: 1366
		public const int ISUBTRACT = 28;

		// Token: 0x04000557 RID: 1367
		public const int PLUS = 73;

		// Token: 0x04000558 RID: 1368
		public const int ENDFUNCTION = 45;

		// Token: 0x04000559 RID: 1369
		public const int DIGIT = 96;

		// Token: 0x0400055A RID: 1370
		public const int HEADER = 8;

		// Token: 0x0400055B RID: 1371
		public const int RBRACKET = 64;

		// Token: 0x0400055C RID: 1372
		public const int DOT = 62;

		// Token: 0x0400055D RID: 1373
		public const int FSUBTRACT = 29;

		// Token: 0x0400055E RID: 1374
		public const int STRCAT = 36;

		// Token: 0x0400055F RID: 1375
		public const int INTEGER = 81;

		// Token: 0x04000560 RID: 1376
		public const int STATE = 51;

		// Token: 0x04000561 RID: 1377
		public const int DOCSTRING = 40;

		// Token: 0x04000562 RID: 1378
		public const int WS_CHAR = 101;

		// Token: 0x04000563 RID: 1379
		public const int HEX_DIGIT = 97;

		// Token: 0x04000564 RID: 1380
		public const int ARRAYRFIND = 25;

		// Token: 0x04000565 RID: 1381
		public const int MINUS = 74;

		// Token: 0x04000566 RID: 1382
		public const int EVENT = 7;

		// Token: 0x04000567 RID: 1383
		public const int ARRAYGET = 22;

		// Token: 0x04000568 RID: 1384
		public const int ELSEIF = 86;

		// Token: 0x04000569 RID: 1385
		public const int AUTOPROP = 19;

		// Token: 0x0400056A RID: 1386
		public const int PAREXPR = 15;

		// Token: 0x0400056B RID: 1387
		public const int BLOCK = 10;

		// Token: 0x0400056C RID: 1388
		public const int EAT_EOL = 103;

		// Token: 0x0400056D RID: 1389
		public const int GLOBAL = 46;

		// Token: 0x0400056E RID: 1390
		public const int MODEQUALS = 61;

		// Token: 0x0400056F RID: 1391
		public const int EXTENDS = 39;

		// Token: 0x04000570 RID: 1392
		public const int STRING = 90;

		// Token: 0x04000571 RID: 1393
		private const string DFA25_eotS = "\f￿";

		// Token: 0x04000572 RID: 1394
		private const string DFA25_eofS = "\f￿";

		// Token: 0x04000573 RID: 1395
		private const string DFA25_minS = "\u0001\u0005\u0001\u0002\u0003&\u0001@\u0001\u0003\u0001@\u0001&\u0002￿\u0001&";

		// Token: 0x04000574 RID: 1396
		private const string DFA25_maxS = "\u0001\u0005\u0001\u0002\u00017\u0002?\u0001@\u0001]\u0001@\u0001&\u0002￿\u0001&";

		// Token: 0x04000575 RID: 1397
		private const string DFA25_acceptS = "\t￿\u0001\u0002\u0001\u0001\u0001￿";

		// Token: 0x04000576 RID: 1398
		private const string DFA25_specialS = "\f￿}>";

		// Token: 0x04000577 RID: 1399
		private const string DFA26_eotS = "\u0019￿";

		// Token: 0x04000578 RID: 1400
		private const string DFA26_eofS = "\u0019￿";

		// Token: 0x04000579 RID: 1401
		private const string DFA26_minS = "\u0001\v\u0002\u0002\u0001￿\u0001\v\u0001&\u0001￿\u0002&\u0001\u0002\u0005\v\u0001&\u0001￿\u0001&\u0005\u0003\u0002\v";

		// Token: 0x0400057A RID: 1402
		private const string DFA26_maxS = "\u0001>\u0002\u0002\u0001￿\u0001R\u0001&\u0001￿\u0001&\u0001]\u0001\u0002\u0005R\u0001&\u0001￿\u0001]\u0005\u0003\u0002R";

		// Token: 0x0400057B RID: 1403
		private const string DFA26_acceptS = "\u0003￿\u0001\u0003\u0002￿\u0001\u0001\t￿\u0001\u0002\b￿";

		// Token: 0x0400057C RID: 1404
		private const string DFA26_specialS = "\u0019￿}>";

		// Token: 0x0400057D RID: 1405
		private OptimizePass ePassType;

		// Token: 0x0400057E RID: 1406
		public bool bMadeChanges;

		// Token: 0x0400057F RID: 1407
		private ScriptObjectType kObjType;

		// Token: 0x04000580 RID: 1408
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

		// Token: 0x04000581 RID: 1409
		protected ITreeAdaptor adaptor = new CommonTreeAdaptor();

		// Token: 0x04000583 RID: 1411
		protected StackList function_stack = new StackList();

		// Token: 0x04000584 RID: 1412
		protected StackList eventFunc_stack = new StackList();

		// Token: 0x04000585 RID: 1413
		protected StackList codeBlock_stack = new StackList();

		// Token: 0x04000586 RID: 1414
		protected StackList localDefinition_stack = new StackList();

		// Token: 0x04000587 RID: 1415
		protected StackList bool_expression_stack = new StackList();

		// Token: 0x04000588 RID: 1416
		protected StackList add_expression_stack = new StackList();

		// Token: 0x04000589 RID: 1417
		protected StackList mult_expression_stack = new StackList();

		// Token: 0x0400058A RID: 1418
		protected StackList ifBlock_stack = new StackList();

		// Token: 0x0400058B RID: 1419
		protected StackList elseIfBlock_stack = new StackList();

		// Token: 0x0400058C RID: 1420
		protected StackList elseBlock_stack = new StackList();

		// Token: 0x0400058D RID: 1421
		protected StackList whileBlock_stack = new StackList();

		// Token: 0x0400058E RID: 1422
		protected DFA25 dfa25;

		// Token: 0x0400058F RID: 1423
		protected DFA26 dfa26;

		// Token: 0x04000590 RID: 1424
		private static readonly string[] DFA25_transitionS = new string[]
		{
			"\u0001\u0001",
			"\u0001\u0002",
			"\u0001\u0003\u0010￿\u0001\u0004",
			"\u0001\u0006\u0018￿\u0001\u0005",
			"\u0001\u0006\u0018￿\u0001\a",
			"\u0001\b",
			"\u0001\t\"￿\u0001\n(￿\u0001\n\u0001￿\u0001\n\b￿\u0004\n",
			"\u0001\v",
			"\u0001\u0006",
			"",
			"",
			"\u0001\u0006"
		};

		// Token: 0x04000591 RID: 1425
		private static readonly short[] DFA25_eot = DFA.UnpackEncodedString("\f￿");

		// Token: 0x04000592 RID: 1426
		private static readonly short[] DFA25_eof = DFA.UnpackEncodedString("\f￿");

		// Token: 0x04000593 RID: 1427
		private static readonly char[] DFA25_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\u0005\u0001\u0002\u0003&\u0001@\u0001\u0003\u0001@\u0001&\u0002￿\u0001&");

		// Token: 0x04000594 RID: 1428
		private static readonly char[] DFA25_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001\u0005\u0001\u0002\u00017\u0002?\u0001@\u0001]\u0001@\u0001&\u0002￿\u0001&");

		// Token: 0x04000595 RID: 1429
		private static readonly short[] DFA25_accept = DFA.UnpackEncodedString("\t￿\u0001\u0002\u0001\u0001\u0001￿");

		// Token: 0x04000596 RID: 1430
		private static readonly short[] DFA25_special = DFA.UnpackEncodedString("\f￿}>");

		// Token: 0x04000597 RID: 1431
		private static readonly short[][] DFA25_transition = DFA.UnpackEncodedStringArray(DFA25_transitionS);

		// Token: 0x04000598 RID: 1432
		private static readonly string[] DFA26_transitionS = new string[]
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

		// Token: 0x04000599 RID: 1433
		private static readonly short[] DFA26_eot = DFA.UnpackEncodedString("\u0019￿");

		// Token: 0x0400059A RID: 1434
		private static readonly short[] DFA26_eof = DFA.UnpackEncodedString("\u0019￿");

		// Token: 0x0400059B RID: 1435
		private static readonly char[] DFA26_min = DFA.UnpackEncodedStringToUnsignedChars("\u0001\v\u0002\u0002\u0001￿\u0001\v\u0001&\u0001￿\u0002&\u0001\u0002\u0005\v\u0001&\u0001￿\u0001&\u0005\u0003\u0002\v");

		// Token: 0x0400059C RID: 1436
		private static readonly char[] DFA26_max = DFA.UnpackEncodedStringToUnsignedChars("\u0001>\u0002\u0002\u0001￿\u0001R\u0001&\u0001￿\u0001&\u0001]\u0001\u0002\u0005R\u0001&\u0001￿\u0001]\u0005\u0003\u0002R");

		// Token: 0x0400059D RID: 1437
		private static readonly short[] DFA26_accept = DFA.UnpackEncodedString("\u0003￿\u0001\u0003\u0002￿\u0001\u0001\t￿\u0001\u0002\b￿");

		// Token: 0x0400059E RID: 1438
		private static readonly short[] DFA26_special = DFA.UnpackEncodedString("\u0019￿}>");

		// Token: 0x0400059F RID: 1439
		private static readonly short[][] DFA26_transition = DFA.UnpackEncodedStringArray(DFA26_transitionS);

		// Token: 0x040005A0 RID: 1440
		public static readonly BitSet FOLLOW_OBJECT_in_script86 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005A1 RID: 1441
		public static readonly BitSet FOLLOW_header_in_script88 = new BitSet(new ulong[]
		{
			20266198323691752UL
		});

		// Token: 0x040005A2 RID: 1442
		public static readonly BitSet FOLLOW_definitionOrBlock_in_script90 = new BitSet(new ulong[]
		{
			20266198323691752UL
		});

		// Token: 0x040005A3 RID: 1443
		public static readonly BitSet FOLLOW_ID_in_header104 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005A4 RID: 1444
		public static readonly BitSet FOLLOW_USER_FLAGS_in_header106 = new BitSet(new ulong[]
		{
			1374389534728UL
		});

		// Token: 0x040005A5 RID: 1445
		public static readonly BitSet FOLLOW_ID_in_header108 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x040005A6 RID: 1446
		public static readonly BitSet FOLLOW_DOCSTRING_in_header111 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005A7 RID: 1447
		public static readonly BitSet FOLLOW_fieldDefinition_in_definitionOrBlock126 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005A8 RID: 1448
		public static readonly BitSet FOLLOW_function_in_definitionOrBlock132 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005A9 RID: 1449
		public static readonly BitSet FOLLOW_eventFunc_in_definitionOrBlock140 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005AA RID: 1450
		public static readonly BitSet FOLLOW_stateBlock_in_definitionOrBlock147 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005AB RID: 1451
		public static readonly BitSet FOLLOW_propertyBlock_in_definitionOrBlock153 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005AC RID: 1452
		public static readonly BitSet FOLLOW_VAR_in_fieldDefinition167 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005AD RID: 1453
		public static readonly BitSet FOLLOW_type_in_fieldDefinition169 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005AE RID: 1454
		public static readonly BitSet FOLLOW_ID_in_fieldDefinition171 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x040005AF RID: 1455
		public static readonly BitSet FOLLOW_USER_FLAGS_in_fieldDefinition173 = new BitSet(new ulong[]
		{
			8UL,
			1006764032UL
		});

		// Token: 0x040005B0 RID: 1456
		public static readonly BitSet FOLLOW_constant_in_fieldDefinition175 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005B1 RID: 1457
		public static readonly BitSet FOLLOW_FUNCTION_in_function207 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005B2 RID: 1458
		public static readonly BitSet FOLLOW_functionHeader_in_function209 = new BitSet(new ulong[]
		{
			1032UL
		});

		// Token: 0x040005B3 RID: 1459
		public static readonly BitSet FOLLOW_codeBlock_in_function211 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005B4 RID: 1460
		public static readonly BitSet FOLLOW_HEADER_in_functionHeader236 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005B5 RID: 1461
		public static readonly BitSet FOLLOW_type_in_functionHeader239 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005B6 RID: 1462
		public static readonly BitSet FOLLOW_NONE_in_functionHeader243 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005B7 RID: 1463
		public static readonly BitSet FOLLOW_ID_in_functionHeader248 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x040005B8 RID: 1464
		public static readonly BitSet FOLLOW_USER_FLAGS_in_functionHeader250 = new BitSet(new ulong[]
		{
			212205744161288UL
		});

		// Token: 0x040005B9 RID: 1465
		public static readonly BitSet FOLLOW_callParameters_in_functionHeader252 = new BitSet(new ulong[]
		{
			212205744160776UL
		});

		// Token: 0x040005BA RID: 1466
		public static readonly BitSet FOLLOW_functionModifier_in_functionHeader255 = new BitSet(new ulong[]
		{
			212205744160776UL
		});

		// Token: 0x040005BB RID: 1467
		public static readonly BitSet FOLLOW_DOCSTRING_in_functionHeader258 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005BC RID: 1468
		public static readonly BitSet FOLLOW_set_in_functionModifier0 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005BD RID: 1469
		public static readonly BitSet FOLLOW_EVENT_in_eventFunc307 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005BE RID: 1470
		public static readonly BitSet FOLLOW_eventHeader_in_eventFunc309 = new BitSet(new ulong[]
		{
			1032UL
		});

		// Token: 0x040005BF RID: 1471
		public static readonly BitSet FOLLOW_codeBlock_in_eventFunc311 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005C0 RID: 1472
		public static readonly BitSet FOLLOW_HEADER_in_eventHeader327 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005C1 RID: 1473
		public static readonly BitSet FOLLOW_NONE_in_eventHeader329 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005C2 RID: 1474
		public static readonly BitSet FOLLOW_ID_in_eventHeader331 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x040005C3 RID: 1475
		public static readonly BitSet FOLLOW_USER_FLAGS_in_eventHeader333 = new BitSet(new ulong[]
		{
			141836999983624UL
		});

		// Token: 0x040005C4 RID: 1476
		public static readonly BitSet FOLLOW_callParameters_in_eventHeader335 = new BitSet(new ulong[]
		{
			141836999983112UL
		});

		// Token: 0x040005C5 RID: 1477
		public static readonly BitSet FOLLOW_NATIVE_in_eventHeader338 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x040005C6 RID: 1478
		public static readonly BitSet FOLLOW_DOCSTRING_in_eventHeader341 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005C7 RID: 1479
		public static readonly BitSet FOLLOW_callParameter_in_callParameters363 = new BitSet(new ulong[]
		{
			514UL
		});

		// Token: 0x040005C8 RID: 1480
		public static readonly BitSet FOLLOW_PARAM_in_callParameter378 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005C9 RID: 1481
		public static readonly BitSet FOLLOW_type_in_callParameter380 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005CA RID: 1482
		public static readonly BitSet FOLLOW_ID_in_callParameter382 = new BitSet(new ulong[]
		{
			8UL,
			1006764032UL
		});

		// Token: 0x040005CB RID: 1483
		public static readonly BitSet FOLLOW_constant_in_callParameter384 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005CC RID: 1484
		public static readonly BitSet FOLLOW_STATE_in_stateBlock402 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005CD RID: 1485
		public static readonly BitSet FOLLOW_ID_in_stateBlock404 = new BitSet(new ulong[]
		{
			1125899906842824UL
		});

		// Token: 0x040005CE RID: 1486
		public static readonly BitSet FOLLOW_AUTO_in_stateBlock406 = new BitSet(new ulong[]
		{
			200UL
		});

		// Token: 0x040005CF RID: 1487
		public static readonly BitSet FOLLOW_stateFuncOrEvent_in_stateBlock410 = new BitSet(new ulong[]
		{
			200UL
		});

		// Token: 0x040005D0 RID: 1488
		public static readonly BitSet FOLLOW_function_in_stateFuncOrEvent427 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005D1 RID: 1489
		public static readonly BitSet FOLLOW_eventFunc_in_stateFuncOrEvent435 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005D2 RID: 1490
		public static readonly BitSet FOLLOW_PROPERTY_in_propertyBlock452 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005D3 RID: 1491
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock454 = new BitSet(new ulong[]
		{
			131072UL
		});

		// Token: 0x040005D4 RID: 1492
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock456 = new BitSet(new ulong[]
		{
			131072UL
		});

		// Token: 0x040005D5 RID: 1493
		public static readonly BitSet FOLLOW_propertyFunc_in_propertyBlock459 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005D6 RID: 1494
		public static readonly BitSet FOLLOW_AUTOPROP_in_propertyBlock468 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005D7 RID: 1495
		public static readonly BitSet FOLLOW_propertyHeader_in_propertyBlock470 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005D8 RID: 1496
		public static readonly BitSet FOLLOW_ID_in_propertyBlock472 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005D9 RID: 1497
		public static readonly BitSet FOLLOW_HEADER_in_propertyHeader490 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005DA RID: 1498
		public static readonly BitSet FOLLOW_type_in_propertyHeader492 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005DB RID: 1499
		public static readonly BitSet FOLLOW_ID_in_propertyHeader496 = new BitSet(new ulong[]
		{
			262144UL
		});

		// Token: 0x040005DC RID: 1500
		public static readonly BitSet FOLLOW_USER_FLAGS_in_propertyHeader498 = new BitSet(new ulong[]
		{
			1099511627784UL
		});

		// Token: 0x040005DD RID: 1501
		public static readonly BitSet FOLLOW_DOCSTRING_in_propertyHeader500 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005DE RID: 1502
		public static readonly BitSet FOLLOW_PROPFUNC_in_propertyFunc521 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005DF RID: 1503
		public static readonly BitSet FOLLOW_function_in_propertyFunc523 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005E0 RID: 1504
		public static readonly BitSet FOLLOW_PROPFUNC_in_propertyFunc532 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005E1 RID: 1505
		public static readonly BitSet FOLLOW_BLOCK_in_codeBlock558 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005E2 RID: 1506
		public static readonly BitSet FOLLOW_statement_in_codeBlock561 = new BitSet(new ulong[]
		{
			4611688629756016680UL,
			1025499646UL
		});

		// Token: 0x040005E3 RID: 1507
		public static readonly BitSet FOLLOW_localDefinition_in_statement576 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005E4 RID: 1508
		public static readonly BitSet FOLLOW_EQUALS_in_statement583 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005E5 RID: 1509
		public static readonly BitSet FOLLOW_ID_in_statement585 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x040005E6 RID: 1510
		public static readonly BitSet FOLLOW_autoCast_in_statement587 = new BitSet(new ulong[]
		{
			4611686293366126592UL
		});

		// Token: 0x040005E7 RID: 1511
		public static readonly BitSet FOLLOW_l_value_in_statement589 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x040005E8 RID: 1512
		public static readonly BitSet FOLLOW_expression_in_statement591 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005E9 RID: 1513
		public static readonly BitSet FOLLOW_expression_in_statement658 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005EA RID: 1514
		public static readonly BitSet FOLLOW_return_stat_in_statement664 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005EB RID: 1515
		public static readonly BitSet FOLLOW_ifBlock_in_statement670 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005EC RID: 1516
		public static readonly BitSet FOLLOW_whileBlock_in_statement676 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040005ED RID: 1517
		public static readonly BitSet FOLLOW_VAR_in_localDefinition699 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005EE RID: 1518
		public static readonly BitSet FOLLOW_type_in_localDefinition701 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005EF RID: 1519
		public static readonly BitSet FOLLOW_ID_in_localDefinition705 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x040005F0 RID: 1520
		public static readonly BitSet FOLLOW_autoCast_in_localDefinition707 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x040005F1 RID: 1521
		public static readonly BitSet FOLLOW_expression_in_localDefinition709 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005F2 RID: 1522
		public static readonly BitSet FOLLOW_VAR_in_localDefinition776 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005F3 RID: 1523
		public static readonly BitSet FOLLOW_type_in_localDefinition778 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005F4 RID: 1524
		public static readonly BitSet FOLLOW_ID_in_localDefinition782 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005F5 RID: 1525
		public static readonly BitSet FOLLOW_DOT_in_l_value823 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005F6 RID: 1526
		public static readonly BitSet FOLLOW_PAREXPR_in_l_value826 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005F7 RID: 1527
		public static readonly BitSet FOLLOW_expression_in_l_value828 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005F8 RID: 1528
		public static readonly BitSet FOLLOW_property_set_in_l_value831 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005F9 RID: 1529
		public static readonly BitSet FOLLOW_ARRAYSET_in_l_value839 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005FA RID: 1530
		public static readonly BitSet FOLLOW_ID_in_l_value843 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040005FB RID: 1531
		public static readonly BitSet FOLLOW_ID_in_l_value847 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x040005FC RID: 1532
		public static readonly BitSet FOLLOW_autoCast_in_l_value849 = new BitSet(new ulong[]
		{
			32768UL
		});

		// Token: 0x040005FD RID: 1533
		public static readonly BitSet FOLLOW_PAREXPR_in_l_value852 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040005FE RID: 1534
		public static readonly BitSet FOLLOW_expression_in_l_value856 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040005FF RID: 1535
		public static readonly BitSet FOLLOW_expression_in_l_value861 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000600 RID: 1536
		public static readonly BitSet FOLLOW_basic_l_value_in_l_value900 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000601 RID: 1537
		public static readonly BitSet FOLLOW_DOT_in_basic_l_value914 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000602 RID: 1538
		public static readonly BitSet FOLLOW_array_func_or_id_in_basic_l_value916 = new BitSet(new ulong[]
		{
			4611686293366126592UL
		});

		// Token: 0x04000603 RID: 1539
		public static readonly BitSet FOLLOW_basic_l_value_in_basic_l_value918 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000604 RID: 1540
		public static readonly BitSet FOLLOW_function_call_in_basic_l_value925 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000605 RID: 1541
		public static readonly BitSet FOLLOW_property_set_in_basic_l_value931 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000606 RID: 1542
		public static readonly BitSet FOLLOW_ARRAYSET_in_basic_l_value940 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000607 RID: 1543
		public static readonly BitSet FOLLOW_ID_in_basic_l_value944 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000608 RID: 1544
		public static readonly BitSet FOLLOW_ID_in_basic_l_value948 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000609 RID: 1545
		public static readonly BitSet FOLLOW_autoCast_in_basic_l_value950 = new BitSet(new ulong[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x0400060A RID: 1546
		public static readonly BitSet FOLLOW_func_or_id_in_basic_l_value952 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400060B RID: 1547
		public static readonly BitSet FOLLOW_expression_in_basic_l_value954 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400060C RID: 1548
		public static readonly BitSet FOLLOW_ID_in_basic_l_value987 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400060D RID: 1549
		public static readonly BitSet FOLLOW_OR_in_expression1010 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400060E RID: 1550
		public static readonly BitSet FOLLOW_ID_in_expression1012 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400060F RID: 1551
		public static readonly BitSet FOLLOW_expression_in_expression1016 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000610 RID: 1552
		public static readonly BitSet FOLLOW_and_expression_in_expression1018 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000611 RID: 1553
		public static readonly BitSet FOLLOW_and_expression_in_expression1055 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000612 RID: 1554
		public static readonly BitSet FOLLOW_AND_in_and_expression1077 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000613 RID: 1555
		public static readonly BitSet FOLLOW_ID_in_and_expression1079 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000614 RID: 1556
		public static readonly BitSet FOLLOW_and_expression_in_and_expression1083 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000615 RID: 1557
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression1085 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000616 RID: 1558
		public static readonly BitSet FOLLOW_bool_expression_in_and_expression1122 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000617 RID: 1559
		public static readonly BitSet FOLLOW_EQ_in_bool_expression1148 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000618 RID: 1560
		public static readonly BitSet FOLLOW_ID_in_bool_expression1150 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000619 RID: 1561
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1154 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400061A RID: 1562
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1158 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400061B RID: 1563
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1162 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400061C RID: 1564
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1166 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400061D RID: 1565
		public static readonly BitSet FOLLOW_NE_in_bool_expression1198 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400061E RID: 1566
		public static readonly BitSet FOLLOW_ID_in_bool_expression1200 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400061F RID: 1567
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1204 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000620 RID: 1568
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1208 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000621 RID: 1569
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1212 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000622 RID: 1570
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1216 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000623 RID: 1571
		public static readonly BitSet FOLLOW_GT_in_bool_expression1248 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000624 RID: 1572
		public static readonly BitSet FOLLOW_ID_in_bool_expression1250 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000625 RID: 1573
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1254 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000626 RID: 1574
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1258 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000627 RID: 1575
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1262 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000628 RID: 1576
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1266 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000629 RID: 1577
		public static readonly BitSet FOLLOW_LT_in_bool_expression1298 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400062A RID: 1578
		public static readonly BitSet FOLLOW_ID_in_bool_expression1300 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400062B RID: 1579
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1304 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400062C RID: 1580
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1308 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400062D RID: 1581
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1312 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400062E RID: 1582
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1316 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400062F RID: 1583
		public static readonly BitSet FOLLOW_GTE_in_bool_expression1348 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000630 RID: 1584
		public static readonly BitSet FOLLOW_ID_in_bool_expression1350 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000631 RID: 1585
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1354 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000632 RID: 1586
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1358 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000633 RID: 1587
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1362 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000634 RID: 1588
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1366 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000635 RID: 1589
		public static readonly BitSet FOLLOW_LTE_in_bool_expression1398 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000636 RID: 1590
		public static readonly BitSet FOLLOW_ID_in_bool_expression1400 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000637 RID: 1591
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1404 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000638 RID: 1592
		public static readonly BitSet FOLLOW_autoCast_in_bool_expression1408 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000639 RID: 1593
		public static readonly BitSet FOLLOW_bool_expression_in_bool_expression1412 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400063A RID: 1594
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1416 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400063B RID: 1595
		public static readonly BitSet FOLLOW_add_expression_in_bool_expression1447 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400063C RID: 1596
		public static readonly BitSet FOLLOW_IADD_in_add_expression1473 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400063D RID: 1597
		public static readonly BitSet FOLLOW_ID_in_add_expression1475 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400063E RID: 1598
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1479 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400063F RID: 1599
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1483 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000640 RID: 1600
		public static readonly BitSet FOLLOW_add_expression_in_add_expression1487 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000641 RID: 1601
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1491 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000642 RID: 1602
		public static readonly BitSet FOLLOW_FADD_in_add_expression1532 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000643 RID: 1603
		public static readonly BitSet FOLLOW_ID_in_add_expression1534 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000644 RID: 1604
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1538 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000645 RID: 1605
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1542 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000646 RID: 1606
		public static readonly BitSet FOLLOW_add_expression_in_add_expression1546 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000647 RID: 1607
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1550 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000648 RID: 1608
		public static readonly BitSet FOLLOW_ISUBTRACT_in_add_expression1591 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000649 RID: 1609
		public static readonly BitSet FOLLOW_ID_in_add_expression1593 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400064A RID: 1610
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1597 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400064B RID: 1611
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1601 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400064C RID: 1612
		public static readonly BitSet FOLLOW_add_expression_in_add_expression1605 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400064D RID: 1613
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1609 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400064E RID: 1614
		public static readonly BitSet FOLLOW_FSUBTRACT_in_add_expression1650 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400064F RID: 1615
		public static readonly BitSet FOLLOW_ID_in_add_expression1652 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000650 RID: 1616
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1656 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000651 RID: 1617
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1660 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000652 RID: 1618
		public static readonly BitSet FOLLOW_add_expression_in_add_expression1664 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000653 RID: 1619
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1668 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000654 RID: 1620
		public static readonly BitSet FOLLOW_STRCAT_in_add_expression1709 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000655 RID: 1621
		public static readonly BitSet FOLLOW_ID_in_add_expression1711 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000656 RID: 1622
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1715 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000657 RID: 1623
		public static readonly BitSet FOLLOW_autoCast_in_add_expression1719 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000658 RID: 1624
		public static readonly BitSet FOLLOW_add_expression_in_add_expression1723 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000659 RID: 1625
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1727 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400065A RID: 1626
		public static readonly BitSet FOLLOW_mult_expression_in_add_expression1767 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400065B RID: 1627
		public static readonly BitSet FOLLOW_IMULTIPLY_in_mult_expression1794 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400065C RID: 1628
		public static readonly BitSet FOLLOW_ID_in_mult_expression1796 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400065D RID: 1629
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression1800 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400065E RID: 1630
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression1804 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400065F RID: 1631
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression1808 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000660 RID: 1632
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression1812 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000661 RID: 1633
		public static readonly BitSet FOLLOW_FMULTIPLY_in_mult_expression1853 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000662 RID: 1634
		public static readonly BitSet FOLLOW_ID_in_mult_expression1855 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000663 RID: 1635
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression1859 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000664 RID: 1636
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression1863 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000665 RID: 1637
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression1867 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000666 RID: 1638
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression1871 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000667 RID: 1639
		public static readonly BitSet FOLLOW_IDIVIDE_in_mult_expression1912 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000668 RID: 1640
		public static readonly BitSet FOLLOW_ID_in_mult_expression1914 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000669 RID: 1641
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression1918 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400066A RID: 1642
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression1922 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400066B RID: 1643
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression1926 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400066C RID: 1644
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression1930 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400066D RID: 1645
		public static readonly BitSet FOLLOW_FDIVIDE_in_mult_expression1971 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400066E RID: 1646
		public static readonly BitSet FOLLOW_ID_in_mult_expression1973 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400066F RID: 1647
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression1977 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x04000670 RID: 1648
		public static readonly BitSet FOLLOW_autoCast_in_mult_expression1981 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000671 RID: 1649
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression1985 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000672 RID: 1650
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression1989 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000673 RID: 1651
		public static readonly BitSet FOLLOW_MOD_in_mult_expression2030 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000674 RID: 1652
		public static readonly BitSet FOLLOW_ID_in_mult_expression2032 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000675 RID: 1653
		public static readonly BitSet FOLLOW_mult_expression_in_mult_expression2036 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000676 RID: 1654
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2038 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000677 RID: 1655
		public static readonly BitSet FOLLOW_unary_expression_in_mult_expression2075 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000678 RID: 1656
		public static readonly BitSet FOLLOW_INEGATE_in_unary_expression2098 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000679 RID: 1657
		public static readonly BitSet FOLLOW_ID_in_unary_expression2100 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400067A RID: 1658
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2102 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400067B RID: 1659
		public static readonly BitSet FOLLOW_FNEGATE_in_unary_expression2138 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400067C RID: 1660
		public static readonly BitSet FOLLOW_ID_in_unary_expression2140 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400067D RID: 1661
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2142 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400067E RID: 1662
		public static readonly BitSet FOLLOW_NOT_in_unary_expression2178 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400067F RID: 1663
		public static readonly BitSet FOLLOW_ID_in_unary_expression2180 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000680 RID: 1664
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2182 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000681 RID: 1665
		public static readonly BitSet FOLLOW_cast_atom_in_unary_expression2217 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000682 RID: 1666
		public static readonly BitSet FOLLOW_AS_in_cast_atom2240 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000683 RID: 1667
		public static readonly BitSet FOLLOW_ID_in_cast_atom2242 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000684 RID: 1668
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom2244 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000685 RID: 1669
		public static readonly BitSet FOLLOW_dot_atom_in_cast_atom2279 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000686 RID: 1670
		public static readonly BitSet FOLLOW_DOT_in_dot_atom2302 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000687 RID: 1671
		public static readonly BitSet FOLLOW_dot_atom_in_dot_atom2306 = new BitSet(new ulong[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x04000688 RID: 1672
		public static readonly BitSet FOLLOW_array_func_or_id_in_dot_atom2308 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000689 RID: 1673
		public static readonly BitSet FOLLOW_array_atom_in_dot_atom2320 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400068A RID: 1674
		public static readonly BitSet FOLLOW_constant_in_dot_atom2331 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400068B RID: 1675
		public static readonly BitSet FOLLOW_ARRAYGET_in_array_atom2355 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x0400068C RID: 1676
		public static readonly BitSet FOLLOW_ID_in_array_atom2359 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x0400068D RID: 1677
		public static readonly BitSet FOLLOW_ID_in_array_atom2363 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400068E RID: 1678
		public static readonly BitSet FOLLOW_autoCast_in_array_atom2365 = new BitSet(new ulong[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x0400068F RID: 1679
		public static readonly BitSet FOLLOW_atom_in_array_atom2367 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x04000690 RID: 1680
		public static readonly BitSet FOLLOW_expression_in_array_atom2369 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000691 RID: 1681
		public static readonly BitSet FOLLOW_atom_in_array_atom2402 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000692 RID: 1682
		public static readonly BitSet FOLLOW_PAREXPR_in_atom2425 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000693 RID: 1683
		public static readonly BitSet FOLLOW_expression_in_atom2427 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000694 RID: 1684
		public static readonly BitSet FOLLOW_NEW_in_atom2461 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000695 RID: 1685
		public static readonly BitSet FOLLOW_INTEGER_in_atom2463 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x04000696 RID: 1686
		public static readonly BitSet FOLLOW_ID_in_atom2465 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x04000697 RID: 1687
		public static readonly BitSet FOLLOW_func_or_id_in_atom2477 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x04000698 RID: 1688
		public static readonly BitSet FOLLOW_ARRAYGET_in_array_func_or_id2491 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x04000699 RID: 1689
		public static readonly BitSet FOLLOW_ID_in_array_func_or_id2495 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x0400069A RID: 1690
		public static readonly BitSet FOLLOW_ID_in_array_func_or_id2499 = new BitSet(new ulong[]
		{
			274877906944UL,
			1006796800UL
		});

		// Token: 0x0400069B RID: 1691
		public static readonly BitSet FOLLOW_autoCast_in_array_func_or_id2501 = new BitSet(new ulong[]
		{
			274933528576UL,
			327680UL
		});

		// Token: 0x0400069C RID: 1692
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id2503 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x0400069D RID: 1693
		public static readonly BitSet FOLLOW_expression_in_array_func_or_id2505 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x0400069E RID: 1694
		public static readonly BitSet FOLLOW_func_or_id_in_array_func_or_id2538 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x0400069F RID: 1695
		public static readonly BitSet FOLLOW_function_call_in_func_or_id2551 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006A0 RID: 1696
		public static readonly BitSet FOLLOW_PROPGET_in_func_or_id2558 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006A1 RID: 1697
		public static readonly BitSet FOLLOW_ID_in_func_or_id2562 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006A2 RID: 1698
		public static readonly BitSet FOLLOW_ID_in_func_or_id2564 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006A3 RID: 1699
		public static readonly BitSet FOLLOW_ID_in_func_or_id2568 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006A4 RID: 1700
		public static readonly BitSet FOLLOW_ID_in_func_or_id2580 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006A5 RID: 1701
		public static readonly BitSet FOLLOW_LENGTH_in_func_or_id2592 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006A6 RID: 1702
		public static readonly BitSet FOLLOW_ID_in_func_or_id2596 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006A7 RID: 1703
		public static readonly BitSet FOLLOW_ID_in_func_or_id2600 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006A8 RID: 1704
		public static readonly BitSet FOLLOW_PROPSET_in_property_set2620 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006A9 RID: 1705
		public static readonly BitSet FOLLOW_ID_in_property_set2624 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006AA RID: 1706
		public static readonly BitSet FOLLOW_ID_in_property_set2626 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006AB RID: 1707
		public static readonly BitSet FOLLOW_ID_in_property_set2630 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006AC RID: 1708
		public static readonly BitSet FOLLOW_RETURN_in_return_stat2650 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006AD RID: 1709
		public static readonly BitSet FOLLOW_autoCast_in_return_stat2652 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x040006AE RID: 1710
		public static readonly BitSet FOLLOW_expression_in_return_stat2654 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006AF RID: 1711
		public static readonly BitSet FOLLOW_RETURN_in_return_stat2704 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006B0 RID: 1712
		public static readonly BitSet FOLLOW_IF_in_ifBlock2732 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006B1 RID: 1713
		public static readonly BitSet FOLLOW_expression_in_ifBlock2734 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x040006B2 RID: 1714
		public static readonly BitSet FOLLOW_codeBlock_in_ifBlock2736 = new BitSet(new ulong[]
		{
			8UL,
			12582912UL
		});

		// Token: 0x040006B3 RID: 1715
		public static readonly BitSet FOLLOW_elseIfBlock_in_ifBlock2740 = new BitSet(new ulong[]
		{
			8UL,
			12582912UL
		});

		// Token: 0x040006B4 RID: 1716
		public static readonly BitSet FOLLOW_elseBlock_in_ifBlock2744 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006B5 RID: 1717
		public static readonly BitSet FOLLOW_ELSEIF_in_elseIfBlock2768 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006B6 RID: 1718
		public static readonly BitSet FOLLOW_expression_in_elseIfBlock2770 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x040006B7 RID: 1719
		public static readonly BitSet FOLLOW_codeBlock_in_elseIfBlock2772 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006B8 RID: 1720
		public static readonly BitSet FOLLOW_ELSE_in_elseBlock2796 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006B9 RID: 1721
		public static readonly BitSet FOLLOW_codeBlock_in_elseBlock2798 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006BA RID: 1722
		public static readonly BitSet FOLLOW_WHILE_in_whileBlock2828 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006BB RID: 1723
		public static readonly BitSet FOLLOW_expression_in_whileBlock2830 = new BitSet(new ulong[]
		{
			1024UL
		});

		// Token: 0x040006BC RID: 1724
		public static readonly BitSet FOLLOW_codeBlock_in_whileBlock2832 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006BD RID: 1725
		public static readonly BitSet FOLLOW_CALL_in_function_call2848 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006BE RID: 1726
		public static readonly BitSet FOLLOW_ID_in_function_call2852 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006BF RID: 1727
		public static readonly BitSet FOLLOW_ID_in_function_call2856 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006C0 RID: 1728
		public static readonly BitSet FOLLOW_ID_in_function_call2860 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x040006C1 RID: 1729
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call2863 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006C2 RID: 1730
		public static readonly BitSet FOLLOW_parameters_in_function_call2865 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006C3 RID: 1731
		public static readonly BitSet FOLLOW_CALLPARENT_in_function_call2880 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006C4 RID: 1732
		public static readonly BitSet FOLLOW_ID_in_function_call2884 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006C5 RID: 1733
		public static readonly BitSet FOLLOW_ID_in_function_call2888 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006C6 RID: 1734
		public static readonly BitSet FOLLOW_ID_in_function_call2892 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x040006C7 RID: 1735
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call2895 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006C8 RID: 1736
		public static readonly BitSet FOLLOW_parameters_in_function_call2897 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006C9 RID: 1737
		public static readonly BitSet FOLLOW_CALLGLOBAL_in_function_call2912 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006CA RID: 1738
		public static readonly BitSet FOLLOW_ID_in_function_call2916 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006CB RID: 1739
		public static readonly BitSet FOLLOW_ID_in_function_call2920 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006CC RID: 1740
		public static readonly BitSet FOLLOW_ID_in_function_call2924 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x040006CD RID: 1741
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call2927 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006CE RID: 1742
		public static readonly BitSet FOLLOW_parameters_in_function_call2929 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006CF RID: 1743
		public static readonly BitSet FOLLOW_ARRAYFIND_in_function_call2944 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006D0 RID: 1744
		public static readonly BitSet FOLLOW_ID_in_function_call2948 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006D1 RID: 1745
		public static readonly BitSet FOLLOW_ID_in_function_call2952 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x040006D2 RID: 1746
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call2955 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006D3 RID: 1747
		public static readonly BitSet FOLLOW_parameters_in_function_call2957 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006D4 RID: 1748
		public static readonly BitSet FOLLOW_ARRAYRFIND_in_function_call2972 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006D5 RID: 1749
		public static readonly BitSet FOLLOW_ID_in_function_call2976 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006D6 RID: 1750
		public static readonly BitSet FOLLOW_ID_in_function_call2980 = new BitSet(new ulong[]
		{
			16384UL
		});

		// Token: 0x040006D7 RID: 1751
		public static readonly BitSet FOLLOW_CALLPARAMS_in_function_call2983 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006D8 RID: 1752
		public static readonly BitSet FOLLOW_parameters_in_function_call2985 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006D9 RID: 1753
		public static readonly BitSet FOLLOW_parameter_in_parameters3006 = new BitSet(new ulong[]
		{
			514UL
		});

		// Token: 0x040006DA RID: 1754
		public static readonly BitSet FOLLOW_PARAM_in_parameter3021 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006DB RID: 1755
		public static readonly BitSet FOLLOW_autoCast_in_parameter3023 = new BitSet(new ulong[]
		{
			4611686430732761088UL,
			1007149566UL
		});

		// Token: 0x040006DC RID: 1756
		public static readonly BitSet FOLLOW_expression_in_parameter3025 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006DD RID: 1757
		public static readonly BitSet FOLLOW_AS_in_autoCast3087 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006DE RID: 1758
		public static readonly BitSet FOLLOW_ID_in_autoCast3091 = new BitSet(new ulong[]
		{
			274877906944UL
		});

		// Token: 0x040006DF RID: 1759
		public static readonly BitSet FOLLOW_ID_in_autoCast3095 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006E0 RID: 1760
		public static readonly BitSet FOLLOW_AS_in_autoCast3108 = new BitSet(new ulong[]
		{
			4UL
		});

		// Token: 0x040006E1 RID: 1761
		public static readonly BitSet FOLLOW_ID_in_autoCast3110 = new BitSet(new ulong[]
		{
			0UL,
			1006764032UL
		});

		// Token: 0x040006E2 RID: 1762
		public static readonly BitSet FOLLOW_constant_in_autoCast3112 = new BitSet(new ulong[]
		{
			8UL
		});

		// Token: 0x040006E3 RID: 1763
		public static readonly BitSet FOLLOW_ID_in_autoCast3147 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006E4 RID: 1764
		public static readonly BitSet FOLLOW_constant_in_autoCast3158 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006E5 RID: 1765
		public static readonly BitSet FOLLOW_number_in_constant3171 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006E6 RID: 1766
		public static readonly BitSet FOLLOW_STRING_in_constant3177 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006E7 RID: 1767
		public static readonly BitSet FOLLOW_BOOL_in_constant3183 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006E8 RID: 1768
		public static readonly BitSet FOLLOW_NONE_in_constant3189 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006E9 RID: 1769
		public static readonly BitSet FOLLOW_set_in_number0 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006EA RID: 1770
		public static readonly BitSet FOLLOW_ID_in_type3221 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006EB RID: 1771
		public static readonly BitSet FOLLOW_ID_in_type3227 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x040006EC RID: 1772
		public static readonly BitSet FOLLOW_LBRACKET_in_type3229 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x040006ED RID: 1773
		public static readonly BitSet FOLLOW_RBRACKET_in_type3231 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006EE RID: 1774
		public static readonly BitSet FOLLOW_BASETYPE_in_type3237 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x040006EF RID: 1775
		public static readonly BitSet FOLLOW_BASETYPE_in_type3243 = new BitSet(new ulong[]
		{
			9223372036854775808UL
		});

		// Token: 0x040006F0 RID: 1776
		public static readonly BitSet FOLLOW_LBRACKET_in_type3245 = new BitSet(new ulong[]
		{
			0UL,
			1UL
		});

		// Token: 0x040006F1 RID: 1777
		public static readonly BitSet FOLLOW_RBRACKET_in_type3247 = new BitSet(new ulong[]
		{
			2UL
		});

		// Token: 0x02000139 RID: 313
		public enum OptimizePass
		{
			// Token: 0x040006F3 RID: 1779
			NORMAL,
			// Token: 0x040006F4 RID: 1780
			VARCLEANUP
		}

		// Token: 0x0200013A RID: 314
		public class script_return : TreeRuleReturnScope
		{
			// Token: 0x1700013D RID: 317
			// (get) Token: 0x06000B27 RID: 2855 RVA: 0x00045D10 File Offset: 0x00043F10
			// (set) Token: 0x06000B28 RID: 2856 RVA: 0x00045D18 File Offset: 0x00043F18
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x040006F5 RID: 1781
			private CommonTree tree;
		}

		// Token: 0x0200013B RID: 315
		public class header_return : TreeRuleReturnScope
		{
			// Token: 0x1700013E RID: 318
			// (get) Token: 0x06000B2A RID: 2858 RVA: 0x00045D30 File Offset: 0x00043F30
			// (set) Token: 0x06000B2B RID: 2859 RVA: 0x00045D38 File Offset: 0x00043F38
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x040006F6 RID: 1782
			private CommonTree tree;
		}

		// Token: 0x0200013C RID: 316
		public class definitionOrBlock_return : TreeRuleReturnScope
		{
			// Token: 0x1700013F RID: 319
			// (get) Token: 0x06000B2D RID: 2861 RVA: 0x00045D50 File Offset: 0x00043F50
			// (set) Token: 0x06000B2E RID: 2862 RVA: 0x00045D58 File Offset: 0x00043F58
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x040006F7 RID: 1783
			private CommonTree tree;
		}

		// Token: 0x0200013D RID: 317
		public class fieldDefinition_return : TreeRuleReturnScope
		{
			// Token: 0x17000140 RID: 320
			// (get) Token: 0x06000B30 RID: 2864 RVA: 0x00045D70 File Offset: 0x00043F70
			// (set) Token: 0x06000B31 RID: 2865 RVA: 0x00045D78 File Offset: 0x00043F78
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x040006F8 RID: 1784
			private CommonTree tree;
		}

		// Token: 0x0200013E RID: 318
		protected class function_scope
		{
			// Token: 0x040006F9 RID: 1785
			protected internal string sstate;

			// Token: 0x040006FA RID: 1786
			protected internal string spropertyName;

			// Token: 0x040006FB RID: 1787
			protected internal ScriptFunctionType kfuncType;
		}

		// Token: 0x0200013F RID: 319
		public class function_return : TreeRuleReturnScope
		{
			// Token: 0x17000141 RID: 321
			// (get) Token: 0x06000B34 RID: 2868 RVA: 0x00045D98 File Offset: 0x00043F98
			// (set) Token: 0x06000B35 RID: 2869 RVA: 0x00045DA0 File Offset: 0x00043FA0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x040006FC RID: 1788
			public string sName;

			// Token: 0x040006FD RID: 1789
			private CommonTree tree;
		}

		// Token: 0x02000140 RID: 320
		public class functionHeader_return : TreeRuleReturnScope
		{
			// Token: 0x17000142 RID: 322
			// (get) Token: 0x06000B37 RID: 2871 RVA: 0x00045DB8 File Offset: 0x00043FB8
			// (set) Token: 0x06000B38 RID: 2872 RVA: 0x00045DC0 File Offset: 0x00043FC0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x040006FE RID: 1790
			public string sFuncName;

			// Token: 0x040006FF RID: 1791
			private CommonTree tree;
		}

		// Token: 0x02000141 RID: 321
		public class functionModifier_return : TreeRuleReturnScope
		{
			// Token: 0x17000143 RID: 323
			// (get) Token: 0x06000B3A RID: 2874 RVA: 0x00045DD8 File Offset: 0x00043FD8
			// (set) Token: 0x06000B3B RID: 2875 RVA: 0x00045DE0 File Offset: 0x00043FE0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000700 RID: 1792
			private CommonTree tree;
		}

		// Token: 0x02000142 RID: 322
		protected class eventFunc_scope
		{
			// Token: 0x04000701 RID: 1793
			protected internal string sstate;

			// Token: 0x04000702 RID: 1794
			protected internal string sfuncName;

			// Token: 0x04000703 RID: 1795
			protected internal ScriptFunctionType kfuncType;
		}

		// Token: 0x02000143 RID: 323
		public class eventFunc_return : TreeRuleReturnScope
		{
			// Token: 0x17000144 RID: 324
			// (get) Token: 0x06000B3E RID: 2878 RVA: 0x00045E00 File Offset: 0x00044000
			// (set) Token: 0x06000B3F RID: 2879 RVA: 0x00045E08 File Offset: 0x00044008
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000704 RID: 1796
			private CommonTree tree;
		}

		// Token: 0x02000144 RID: 324
		public class eventHeader_return : TreeRuleReturnScope
		{
			// Token: 0x17000145 RID: 325
			// (get) Token: 0x06000B41 RID: 2881 RVA: 0x00045E20 File Offset: 0x00044020
			// (set) Token: 0x06000B42 RID: 2882 RVA: 0x00045E28 File Offset: 0x00044028
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000705 RID: 1797
			private CommonTree tree;
		}

		// Token: 0x02000145 RID: 325
		public class callParameters_return : TreeRuleReturnScope
		{
			// Token: 0x17000146 RID: 326
			// (get) Token: 0x06000B44 RID: 2884 RVA: 0x00045E40 File Offset: 0x00044040
			// (set) Token: 0x06000B45 RID: 2885 RVA: 0x00045E48 File Offset: 0x00044048
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000706 RID: 1798
			private CommonTree tree;
		}

		// Token: 0x02000146 RID: 326
		public class callParameter_return : TreeRuleReturnScope
		{
			// Token: 0x17000147 RID: 327
			// (get) Token: 0x06000B47 RID: 2887 RVA: 0x00045E60 File Offset: 0x00044060
			// (set) Token: 0x06000B48 RID: 2888 RVA: 0x00045E68 File Offset: 0x00044068
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000707 RID: 1799
			private CommonTree tree;
		}

		// Token: 0x02000147 RID: 327
		public class stateBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000148 RID: 328
			// (get) Token: 0x06000B4A RID: 2890 RVA: 0x00045E80 File Offset: 0x00044080
			// (set) Token: 0x06000B4B RID: 2891 RVA: 0x00045E88 File Offset: 0x00044088
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000708 RID: 1800
			private CommonTree tree;
		}

		// Token: 0x02000148 RID: 328
		public class stateFuncOrEvent_return : TreeRuleReturnScope
		{
			// Token: 0x17000149 RID: 329
			// (get) Token: 0x06000B4D RID: 2893 RVA: 0x00045EA0 File Offset: 0x000440A0
			// (set) Token: 0x06000B4E RID: 2894 RVA: 0x00045EA8 File Offset: 0x000440A8
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000709 RID: 1801
			private CommonTree tree;
		}

		// Token: 0x02000149 RID: 329
		public class propertyBlock_return : TreeRuleReturnScope
		{
			// Token: 0x1700014A RID: 330
			// (get) Token: 0x06000B50 RID: 2896 RVA: 0x00045EC0 File Offset: 0x000440C0
			// (set) Token: 0x06000B51 RID: 2897 RVA: 0x00045EC8 File Offset: 0x000440C8
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400070A RID: 1802
			private CommonTree tree;
		}

		// Token: 0x0200014A RID: 330
		public class propertyHeader_return : TreeRuleReturnScope
		{
			// Token: 0x1700014B RID: 331
			// (get) Token: 0x06000B53 RID: 2899 RVA: 0x00045EE0 File Offset: 0x000440E0
			// (set) Token: 0x06000B54 RID: 2900 RVA: 0x00045EE8 File Offset: 0x000440E8
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400070B RID: 1803
			public string sName;

			// Token: 0x0400070C RID: 1804
			private CommonTree tree;
		}

		// Token: 0x0200014B RID: 331
		public class propertyFunc_return : TreeRuleReturnScope
		{
			// Token: 0x1700014C RID: 332
			// (get) Token: 0x06000B56 RID: 2902 RVA: 0x00045F00 File Offset: 0x00044100
			// (set) Token: 0x06000B57 RID: 2903 RVA: 0x00045F08 File Offset: 0x00044108
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400070D RID: 1805
			private CommonTree tree;
		}

		// Token: 0x0200014C RID: 332
		protected class codeBlock_scope
		{
			// Token: 0x0400070E RID: 1806
			protected internal ScriptScope kcurrentScope;

			// Token: 0x0400070F RID: 1807
			protected internal int inextScopeChild;
		}

		// Token: 0x0200014D RID: 333
		public class codeBlock_return : TreeRuleReturnScope
		{
			// Token: 0x1700014D RID: 333
			// (get) Token: 0x06000B5A RID: 2906 RVA: 0x00045F28 File Offset: 0x00044128
			// (set) Token: 0x06000B5B RID: 2907 RVA: 0x00045F30 File Offset: 0x00044130
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000710 RID: 1808
			private CommonTree tree;
		}

		// Token: 0x0200014E RID: 334
		public class statement_return : TreeRuleReturnScope
		{
			// Token: 0x1700014E RID: 334
			// (get) Token: 0x06000B5D RID: 2909 RVA: 0x00045F48 File Offset: 0x00044148
			// (set) Token: 0x06000B5E RID: 2910 RVA: 0x00045F50 File Offset: 0x00044150
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000711 RID: 1809
			private CommonTree tree;
		}

		// Token: 0x0200014F RID: 335
		protected class localDefinition_scope
		{
			// Token: 0x04000712 RID: 1810
			protected internal bool bvarUsed;
		}

		// Token: 0x02000150 RID: 336
		public class localDefinition_return : TreeRuleReturnScope
		{
			// Token: 0x1700014F RID: 335
			// (get) Token: 0x06000B61 RID: 2913 RVA: 0x00045F70 File Offset: 0x00044170
			// (set) Token: 0x06000B62 RID: 2914 RVA: 0x00045F78 File Offset: 0x00044178
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000713 RID: 1811
			private CommonTree tree;
		}

		// Token: 0x02000151 RID: 337
		public class l_value_return : TreeRuleReturnScope
		{
			// Token: 0x17000150 RID: 336
			// (get) Token: 0x06000B64 RID: 2916 RVA: 0x00045F90 File Offset: 0x00044190
			// (set) Token: 0x06000B65 RID: 2917 RVA: 0x00045F98 File Offset: 0x00044198
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000714 RID: 1812
			private CommonTree tree;
		}

		// Token: 0x02000152 RID: 338
		public class basic_l_value_return : TreeRuleReturnScope
		{
			// Token: 0x17000151 RID: 337
			// (get) Token: 0x06000B67 RID: 2919 RVA: 0x00045FB0 File Offset: 0x000441B0
			// (set) Token: 0x06000B68 RID: 2920 RVA: 0x00045FB8 File Offset: 0x000441B8
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000715 RID: 1813
			private CommonTree tree;
		}

		// Token: 0x02000153 RID: 339
		public class expression_return : TreeRuleReturnScope
		{
			// Token: 0x17000152 RID: 338
			// (get) Token: 0x06000B6A RID: 2922 RVA: 0x00045FD0 File Offset: 0x000441D0
			// (set) Token: 0x06000B6B RID: 2923 RVA: 0x00045FD8 File Offset: 0x000441D8
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000716 RID: 1814
			public ITree kOptimizedTree;

			// Token: 0x04000717 RID: 1815
			private CommonTree tree;
		}

		// Token: 0x02000154 RID: 340
		public class and_expression_return : TreeRuleReturnScope
		{
			// Token: 0x17000153 RID: 339
			// (get) Token: 0x06000B6D RID: 2925 RVA: 0x00045FF0 File Offset: 0x000441F0
			// (set) Token: 0x06000B6E RID: 2926 RVA: 0x00045FF8 File Offset: 0x000441F8
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000718 RID: 1816
			public ITree kOptimizedTree;

			// Token: 0x04000719 RID: 1817
			private CommonTree tree;
		}

		// Token: 0x02000155 RID: 341
		protected class bool_expression_scope
		{
			// Token: 0x0400071A RID: 1818
			protected internal ITree kautoCastTreeA;

			// Token: 0x0400071B RID: 1819
			protected internal ITree kexpressionA;

			// Token: 0x0400071C RID: 1820
			protected internal ITree kautoCastTreeB;

			// Token: 0x0400071D RID: 1821
			protected internal ITree kexpressionB;
		}

		// Token: 0x02000156 RID: 342
		public class bool_expression_return : TreeRuleReturnScope
		{
			// Token: 0x17000154 RID: 340
			// (get) Token: 0x06000B71 RID: 2929 RVA: 0x00046018 File Offset: 0x00044218
			// (set) Token: 0x06000B72 RID: 2930 RVA: 0x00046020 File Offset: 0x00044220
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400071E RID: 1822
			public ITree kOptimizedTree;

			// Token: 0x0400071F RID: 1823
			private CommonTree tree;
		}

		// Token: 0x02000157 RID: 343
		protected class add_expression_scope
		{
			// Token: 0x04000720 RID: 1824
			protected internal ITree kautoCastTreeA;

			// Token: 0x04000721 RID: 1825
			protected internal ITree kexpressionA;

			// Token: 0x04000722 RID: 1826
			protected internal ITree kautoCastTreeB;

			// Token: 0x04000723 RID: 1827
			protected internal ITree kexpressionB;
		}

		// Token: 0x02000158 RID: 344
		public class add_expression_return : TreeRuleReturnScope
		{
			// Token: 0x17000155 RID: 341
			// (get) Token: 0x06000B75 RID: 2933 RVA: 0x00046040 File Offset: 0x00044240
			// (set) Token: 0x06000B76 RID: 2934 RVA: 0x00046048 File Offset: 0x00044248
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000724 RID: 1828
			public ITree kOptimizedTree;

			// Token: 0x04000725 RID: 1829
			private CommonTree tree;
		}

		// Token: 0x02000159 RID: 345
		protected class mult_expression_scope
		{
			// Token: 0x04000726 RID: 1830
			protected internal ITree kautoCastTreeA;

			// Token: 0x04000727 RID: 1831
			protected internal ITree kexpressionA;

			// Token: 0x04000728 RID: 1832
			protected internal ITree kautoCastTreeB;

			// Token: 0x04000729 RID: 1833
			protected internal ITree kexpressionB;
		}

		// Token: 0x0200015A RID: 346
		public class mult_expression_return : TreeRuleReturnScope
		{
			// Token: 0x17000156 RID: 342
			// (get) Token: 0x06000B79 RID: 2937 RVA: 0x00046068 File Offset: 0x00044268
			// (set) Token: 0x06000B7A RID: 2938 RVA: 0x00046070 File Offset: 0x00044270
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400072A RID: 1834
			public ITree kOptimizedTree;

			// Token: 0x0400072B RID: 1835
			private CommonTree tree;
		}

		// Token: 0x0200015B RID: 347
		public class unary_expression_return : TreeRuleReturnScope
		{
			// Token: 0x17000157 RID: 343
			// (get) Token: 0x06000B7C RID: 2940 RVA: 0x00046088 File Offset: 0x00044288
			// (set) Token: 0x06000B7D RID: 2941 RVA: 0x00046090 File Offset: 0x00044290
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400072C RID: 1836
			public ITree kOptimizedTree;

			// Token: 0x0400072D RID: 1837
			private CommonTree tree;
		}

		// Token: 0x0200015C RID: 348
		public class cast_atom_return : TreeRuleReturnScope
		{
			// Token: 0x17000158 RID: 344
			// (get) Token: 0x06000B7F RID: 2943 RVA: 0x000460A8 File Offset: 0x000442A8
			// (set) Token: 0x06000B80 RID: 2944 RVA: 0x000460B0 File Offset: 0x000442B0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400072E RID: 1838
			public ITree kOptimizedTree;

			// Token: 0x0400072F RID: 1839
			private CommonTree tree;
		}

		// Token: 0x0200015D RID: 349
		public class dot_atom_return : TreeRuleReturnScope
		{
			// Token: 0x17000159 RID: 345
			// (get) Token: 0x06000B82 RID: 2946 RVA: 0x000460C8 File Offset: 0x000442C8
			// (set) Token: 0x06000B83 RID: 2947 RVA: 0x000460D0 File Offset: 0x000442D0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000730 RID: 1840
			public ITree kOptimizedTree;

			// Token: 0x04000731 RID: 1841
			private CommonTree tree;
		}

		// Token: 0x0200015E RID: 350
		public class array_atom_return : TreeRuleReturnScope
		{
			// Token: 0x1700015A RID: 346
			// (get) Token: 0x06000B85 RID: 2949 RVA: 0x000460E8 File Offset: 0x000442E8
			// (set) Token: 0x06000B86 RID: 2950 RVA: 0x000460F0 File Offset: 0x000442F0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000732 RID: 1842
			public ITree kOptimizedTree;

			// Token: 0x04000733 RID: 1843
			private CommonTree tree;
		}

		// Token: 0x0200015F RID: 351
		public class atom_return : TreeRuleReturnScope
		{
			// Token: 0x1700015B RID: 347
			// (get) Token: 0x06000B88 RID: 2952 RVA: 0x00046108 File Offset: 0x00044308
			// (set) Token: 0x06000B89 RID: 2953 RVA: 0x00046110 File Offset: 0x00044310
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000734 RID: 1844
			public ITree kOptimizedTree;

			// Token: 0x04000735 RID: 1845
			private CommonTree tree;
		}

		// Token: 0x02000160 RID: 352
		public class array_func_or_id_return : TreeRuleReturnScope
		{
			// Token: 0x1700015C RID: 348
			// (get) Token: 0x06000B8B RID: 2955 RVA: 0x00046128 File Offset: 0x00044328
			// (set) Token: 0x06000B8C RID: 2956 RVA: 0x00046130 File Offset: 0x00044330
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000736 RID: 1846
			private CommonTree tree;
		}

		// Token: 0x02000161 RID: 353
		public class func_or_id_return : TreeRuleReturnScope
		{
			// Token: 0x1700015D RID: 349
			// (get) Token: 0x06000B8E RID: 2958 RVA: 0x00046148 File Offset: 0x00044348
			// (set) Token: 0x06000B8F RID: 2959 RVA: 0x00046150 File Offset: 0x00044350
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000737 RID: 1847
			private CommonTree tree;
		}

		// Token: 0x02000162 RID: 354
		public class property_set_return : TreeRuleReturnScope
		{
			// Token: 0x1700015E RID: 350
			// (get) Token: 0x06000B91 RID: 2961 RVA: 0x00046168 File Offset: 0x00044368
			// (set) Token: 0x06000B92 RID: 2962 RVA: 0x00046170 File Offset: 0x00044370
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000738 RID: 1848
			private CommonTree tree;
		}

		// Token: 0x02000163 RID: 355
		public class return_stat_return : TreeRuleReturnScope
		{
			// Token: 0x1700015F RID: 351
			// (get) Token: 0x06000B94 RID: 2964 RVA: 0x00046188 File Offset: 0x00044388
			// (set) Token: 0x06000B95 RID: 2965 RVA: 0x00046190 File Offset: 0x00044390
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000739 RID: 1849
			private CommonTree tree;
		}

		// Token: 0x02000164 RID: 356
		protected class ifBlock_scope
		{
			// Token: 0x0400073A RID: 1850
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x02000165 RID: 357
		public class ifBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000160 RID: 352
			// (get) Token: 0x06000B98 RID: 2968 RVA: 0x000461B0 File Offset: 0x000443B0
			// (set) Token: 0x06000B99 RID: 2969 RVA: 0x000461B8 File Offset: 0x000443B8
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400073B RID: 1851
			private CommonTree tree;
		}

		// Token: 0x02000166 RID: 358
		protected class elseIfBlock_scope
		{
			// Token: 0x0400073C RID: 1852
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x02000167 RID: 359
		public class elseIfBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000161 RID: 353
			// (get) Token: 0x06000B9C RID: 2972 RVA: 0x000461D8 File Offset: 0x000443D8
			// (set) Token: 0x06000B9D RID: 2973 RVA: 0x000461E0 File Offset: 0x000443E0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400073D RID: 1853
			private CommonTree tree;
		}

		// Token: 0x02000168 RID: 360
		protected class elseBlock_scope
		{
			// Token: 0x0400073E RID: 1854
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x02000169 RID: 361
		public class elseBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000162 RID: 354
			// (get) Token: 0x06000BA0 RID: 2976 RVA: 0x00046200 File Offset: 0x00044400
			// (set) Token: 0x06000BA1 RID: 2977 RVA: 0x00046208 File Offset: 0x00044408
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x0400073F RID: 1855
			private CommonTree tree;
		}

		// Token: 0x0200016A RID: 362
		protected class whileBlock_scope
		{
			// Token: 0x04000740 RID: 1856
			protected internal ScriptScope kchildScope;
		}

		// Token: 0x0200016B RID: 363
		public class whileBlock_return : TreeRuleReturnScope
		{
			// Token: 0x17000163 RID: 355
			// (get) Token: 0x06000BA4 RID: 2980 RVA: 0x00046228 File Offset: 0x00044428
			// (set) Token: 0x06000BA5 RID: 2981 RVA: 0x00046230 File Offset: 0x00044430
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000741 RID: 1857
			private CommonTree tree;
		}

		// Token: 0x0200016C RID: 364
		public class function_call_return : TreeRuleReturnScope
		{
			// Token: 0x17000164 RID: 356
			// (get) Token: 0x06000BA7 RID: 2983 RVA: 0x00046248 File Offset: 0x00044448
			// (set) Token: 0x06000BA8 RID: 2984 RVA: 0x00046250 File Offset: 0x00044450
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000742 RID: 1858
			private CommonTree tree;
		}

		// Token: 0x0200016D RID: 365
		public class parameters_return : TreeRuleReturnScope
		{
			// Token: 0x17000165 RID: 357
			// (get) Token: 0x06000BAA RID: 2986 RVA: 0x00046268 File Offset: 0x00044468
			// (set) Token: 0x06000BAB RID: 2987 RVA: 0x00046270 File Offset: 0x00044470
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000743 RID: 1859
			private CommonTree tree;
		}

		// Token: 0x0200016E RID: 366
		public class parameter_return : TreeRuleReturnScope
		{
			// Token: 0x17000166 RID: 358
			// (get) Token: 0x06000BAD RID: 2989 RVA: 0x00046288 File Offset: 0x00044488
			// (set) Token: 0x06000BAE RID: 2990 RVA: 0x00046290 File Offset: 0x00044490
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000744 RID: 1860
			private CommonTree tree;
		}

		// Token: 0x0200016F RID: 367
		public class autoCast_return : TreeRuleReturnScope
		{
			// Token: 0x17000167 RID: 359
			// (get) Token: 0x06000BB0 RID: 2992 RVA: 0x000462A8 File Offset: 0x000444A8
			// (set) Token: 0x06000BB1 RID: 2993 RVA: 0x000462B0 File Offset: 0x000444B0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000745 RID: 1861
			public ITree kOptimizedTree;

			// Token: 0x04000746 RID: 1862
			private CommonTree tree;
		}

		// Token: 0x02000170 RID: 368
		public class constant_return : TreeRuleReturnScope
		{
			// Token: 0x17000168 RID: 360
			// (get) Token: 0x06000BB3 RID: 2995 RVA: 0x000462C8 File Offset: 0x000444C8
			// (set) Token: 0x06000BB4 RID: 2996 RVA: 0x000462D0 File Offset: 0x000444D0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000747 RID: 1863
			private CommonTree tree;
		}

		// Token: 0x02000171 RID: 369
		public class number_return : TreeRuleReturnScope
		{
			// Token: 0x17000169 RID: 361
			// (get) Token: 0x06000BB6 RID: 2998 RVA: 0x000462E8 File Offset: 0x000444E8
			// (set) Token: 0x06000BB7 RID: 2999 RVA: 0x000462F0 File Offset: 0x000444F0
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000748 RID: 1864
			private CommonTree tree;
		}

		// Token: 0x02000172 RID: 370
		public class type_return : TreeRuleReturnScope
		{
			// Token: 0x1700016A RID: 362
			// (get) Token: 0x06000BB9 RID: 3001 RVA: 0x00046308 File Offset: 0x00044508
			// (set) Token: 0x06000BBA RID: 3002 RVA: 0x00046310 File Offset: 0x00044510
			public override object Tree
			{
				get => tree;
                set => tree = (CommonTree)value;
            }

			// Token: 0x04000749 RID: 1865
			private CommonTree tree;
		}

		// Token: 0x02000173 RID: 371
		protected class DFA25 : DFA
		{
			// Token: 0x06000BBC RID: 3004 RVA: 0x00046328 File Offset: 0x00044528
			public DFA25(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 25;
				eot = DFA25_eot;
				eof = DFA25_eof;
				min = DFA25_min;
				max = DFA25_max;
				accept = DFA25_accept;
				special = DFA25_special;
				transition = DFA25_transition;
			}

			// Token: 0x1700016B RID: 363
			// (get) Token: 0x06000BBD RID: 3005 RVA: 0x00046398 File Offset: 0x00044598
			public override string Description => "206:1: localDefinition : ( ^( VAR type name= ID autoCast expression ) -> {$expression.kOptimizedTree == null}? ^( VAR type $name autoCast expression ) -> {$autoCast.kOptimizedTree != null}? ^( VAR type $name) -> ^( VAR type $name expression ) | ^( VAR type name= ID ) -> {$localDefinition::bvarUsed}? ^( VAR type $name) ->);";
        }

		// Token: 0x02000174 RID: 372
		protected class DFA26 : DFA
		{
			// Token: 0x06000BBE RID: 3006 RVA: 0x000463A0 File Offset: 0x000445A0
			public DFA26(BaseRecognizer recognizer)
			{
				this.recognizer = recognizer;
				decisionNumber = 26;
				eot = DFA26_eot;
				eof = DFA26_eof;
				min = DFA26_min;
				max = DFA26_max;
				accept = DFA26_accept;
				special = DFA26_special;
				transition = DFA26_transition;
			}

			// Token: 0x1700016C RID: 364
			// (get) Token: 0x06000BBF RID: 3007 RVA: 0x00046410 File Offset: 0x00044610
			public override string Description => "228:1: l_value : ( ^( DOT ^( PAREXPR expression ) property_set ) | ^( ARRAYSET source= ID self= ID autoCast ^( PAREXPR array= expression ) index= expression ) -> ^( ARRAYSET $source $self ^( PAREXPR $array) $index) | basic_l_value );";
        }
	}
}
