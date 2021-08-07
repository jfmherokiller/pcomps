using System;
using System.ComponentModel;
using pcomps.Antlr.collections;
using pcomps.Antlr.collections.impl;
using pcomps.Antlr.debug;

namespace pcomps.Antlr
{
	// Token: 0x0200002B RID: 43
	public abstract class Parser : IParserDebugSubject, IDebugSubject
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00006280 File Offset: 0x00004480
		protected internal EventHandlerList Events => events_;

        // Token: 0x060001BD RID: 445 RVA: 0x00006294 File Offset: 0x00004494
		public Parser()
		{
			inputState = new ParserSharedInputState();
		}

		// Token: 0x060001BE RID: 446 RVA: 0x000062D8 File Offset: 0x000044D8
		public Parser(ParserSharedInputState state)
		{
			inputState = state;
		}

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x060001BF RID: 447 RVA: 0x00006318 File Offset: 0x00004518
		// (remove) Token: 0x060001C0 RID: 448 RVA: 0x00006338 File Offset: 0x00004538
		public event TraceEventHandler EnterRule
		{
			add => Events.AddHandler(EnterRuleEventKey, value);
            remove => Events.RemoveHandler(EnterRuleEventKey, value);
        }

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x060001C1 RID: 449 RVA: 0x00006358 File Offset: 0x00004558
		// (remove) Token: 0x060001C2 RID: 450 RVA: 0x00006378 File Offset: 0x00004578
		public event TraceEventHandler ExitRule
		{
			add => Events.AddHandler(ExitRuleEventKey, value);
            remove => Events.RemoveHandler(ExitRuleEventKey, value);
        }

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x060001C3 RID: 451 RVA: 0x00006398 File Offset: 0x00004598
		// (remove) Token: 0x060001C4 RID: 452 RVA: 0x000063B8 File Offset: 0x000045B8
		public event TraceEventHandler Done
		{
			add => Events.AddHandler(DoneEventKey, value);
            remove => Events.RemoveHandler(DoneEventKey, value);
        }

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x060001C5 RID: 453 RVA: 0x000063D8 File Offset: 0x000045D8
		// (remove) Token: 0x060001C6 RID: 454 RVA: 0x000063F8 File Offset: 0x000045F8
		public event MessageEventHandler ErrorReported
		{
			add => Events.AddHandler(ReportErrorEventKey, value);
            remove => Events.RemoveHandler(ReportErrorEventKey, value);
        }

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x060001C7 RID: 455 RVA: 0x00006418 File Offset: 0x00004618
		// (remove) Token: 0x060001C8 RID: 456 RVA: 0x00006438 File Offset: 0x00004638
		public event MessageEventHandler WarningReported
		{
			add => Events.AddHandler(ReportWarningEventKey, value);
            remove => Events.RemoveHandler(ReportWarningEventKey, value);
        }

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x060001C9 RID: 457 RVA: 0x00006458 File Offset: 0x00004658
		// (remove) Token: 0x060001CA RID: 458 RVA: 0x00006478 File Offset: 0x00004678
		public event MatchEventHandler MatchedToken
		{
			add => Events.AddHandler(MatchEventKey, value);
            remove => Events.RemoveHandler(MatchEventKey, value);
        }

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x060001CB RID: 459 RVA: 0x00006498 File Offset: 0x00004698
		// (remove) Token: 0x060001CC RID: 460 RVA: 0x000064B8 File Offset: 0x000046B8
		public event MatchEventHandler MatchedNotToken
		{
			add => Events.AddHandler(MatchNotEventKey, value);
            remove => Events.RemoveHandler(MatchNotEventKey, value);
        }

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x060001CD RID: 461 RVA: 0x000064D8 File Offset: 0x000046D8
		// (remove) Token: 0x060001CE RID: 462 RVA: 0x000064F8 File Offset: 0x000046F8
		public event MatchEventHandler MisMatchedToken
		{
			add => Events.AddHandler(MisMatchEventKey, value);
            remove => Events.RemoveHandler(MisMatchEventKey, value);
        }

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x060001CF RID: 463 RVA: 0x00006518 File Offset: 0x00004718
		// (remove) Token: 0x060001D0 RID: 464 RVA: 0x00006538 File Offset: 0x00004738
		public event MatchEventHandler MisMatchedNotToken
		{
			add => Events.AddHandler(MisMatchNotEventKey, value);
            remove => Events.RemoveHandler(MisMatchNotEventKey, value);
        }

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x060001D1 RID: 465 RVA: 0x00006558 File Offset: 0x00004758
		// (remove) Token: 0x060001D2 RID: 466 RVA: 0x00006578 File Offset: 0x00004778
		public event TokenEventHandler ConsumedToken
		{
			add => Events.AddHandler(ConsumeEventKey, value);
            remove => Events.RemoveHandler(ConsumeEventKey, value);
        }

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x060001D3 RID: 467 RVA: 0x00006598 File Offset: 0x00004798
		// (remove) Token: 0x060001D4 RID: 468 RVA: 0x000065B8 File Offset: 0x000047B8
		public event TokenEventHandler TokenLA
		{
			add => Events.AddHandler(LAEventKey, value);
            remove => Events.RemoveHandler(LAEventKey, value);
        }

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x060001D5 RID: 469 RVA: 0x000065D8 File Offset: 0x000047D8
		// (remove) Token: 0x060001D6 RID: 470 RVA: 0x000065F8 File Offset: 0x000047F8
		public event SemanticPredicateEventHandler SemPredEvaluated
		{
			add => Events.AddHandler(SemPredEvaluatedEventKey, value);
            remove => Events.RemoveHandler(SemPredEvaluatedEventKey, value);
        }

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x060001D7 RID: 471 RVA: 0x00006618 File Offset: 0x00004818
		// (remove) Token: 0x060001D8 RID: 472 RVA: 0x00006638 File Offset: 0x00004838
		public event SyntacticPredicateEventHandler SynPredStarted
		{
			add => Events.AddHandler(SynPredStartedEventKey, value);
            remove => Events.RemoveHandler(SynPredStartedEventKey, value);
        }

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x060001D9 RID: 473 RVA: 0x00006658 File Offset: 0x00004858
		// (remove) Token: 0x060001DA RID: 474 RVA: 0x00006678 File Offset: 0x00004878
		public event SyntacticPredicateEventHandler SynPredFailed
		{
			add => Events.AddHandler(SynPredFailedEventKey, value);
            remove => Events.RemoveHandler(SynPredFailedEventKey, value);
        }

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x060001DB RID: 475 RVA: 0x00006698 File Offset: 0x00004898
		// (remove) Token: 0x060001DC RID: 476 RVA: 0x000066B8 File Offset: 0x000048B8
		public event SyntacticPredicateEventHandler SynPredSucceeded
		{
			add => Events.AddHandler(SynPredSucceededEventKey, value);
            remove => Events.RemoveHandler(SynPredSucceededEventKey, value);
        }

		// Token: 0x060001DD RID: 477 RVA: 0x000066D8 File Offset: 0x000048D8
		public virtual void addMessageListener(MessageListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("addMessageListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001DE RID: 478 RVA: 0x000066F8 File Offset: 0x000048F8
		public virtual void addParserListener(ParserListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("addParserListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00006718 File Offset: 0x00004918
		public virtual void addParserMatchListener(ParserMatchListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("addParserMatchListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00006738 File Offset: 0x00004938
		public virtual void addParserTokenListener(ParserTokenListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("addParserTokenListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00006758 File Offset: 0x00004958
		public virtual void addSemanticPredicateListener(SemanticPredicateListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("addSemanticPredicateListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00006778 File Offset: 0x00004978
		public virtual void addSyntacticPredicateListener(SyntacticPredicateListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("addSyntacticPredicateListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00006798 File Offset: 0x00004998
		public virtual void addTraceListener(TraceListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("addTraceListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001E4 RID: 484
		public abstract void consume();

		// Token: 0x060001E5 RID: 485 RVA: 0x000067B8 File Offset: 0x000049B8
		public virtual void consumeUntil(int tokenType)
		{
			while (LA(1) != 1 && LA(1) != tokenType)
			{
				consume();
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x000067E4 File Offset: 0x000049E4
		public virtual void consumeUntil(BitSet bset)
		{
			while (LA(1) != 1 && !bset.member(LA(1)))
			{
				consume();
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00006814 File Offset: 0x00004A14
		protected internal virtual void defaultDebuggingSetup(TokenStream lexer, TokenBuffer tokBuf)
		{
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00006824 File Offset: 0x00004A24
		public virtual AST getAST()
		{
			return returnAST;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00006838 File Offset: 0x00004A38
		public virtual ASTFactory getASTFactory()
		{
			return astFactory;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000684C File Offset: 0x00004A4C
		public virtual string getFilename()
		{
			return inputState.filename;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00006864 File Offset: 0x00004A64
		public virtual ParserSharedInputState getInputState()
		{
			return inputState;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00006878 File Offset: 0x00004A78
		public virtual void setInputState(ParserSharedInputState state)
		{
			inputState = state;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000688C File Offset: 0x00004A8C
		public virtual void resetState()
		{
			traceDepth = 0;
			inputState.reset();
		}

		// Token: 0x060001EE RID: 494 RVA: 0x000068AC File Offset: 0x00004AAC
		public virtual string getTokenName(int num)
		{
			return tokenNames[num];
		}

		// Token: 0x060001EF RID: 495 RVA: 0x000068C4 File Offset: 0x00004AC4
		public virtual string[] getTokenNames()
		{
			return tokenNames;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x000068D8 File Offset: 0x00004AD8
		public virtual bool isDebugMode()
		{
			return false;
		}

		// Token: 0x060001F1 RID: 497
		public abstract int LA(int i);

		// Token: 0x060001F2 RID: 498
		public abstract IToken LT(int i);

		// Token: 0x060001F3 RID: 499 RVA: 0x000068E8 File Offset: 0x00004AE8
		public virtual int mark()
		{
			return inputState.input.mark();
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00006908 File Offset: 0x00004B08
		public virtual void match(int t)
		{
			if (LA(1) != t)
			{
				throw new MismatchedTokenException(tokenNames, LT(1), t, false, getFilename());
			}
			consume();
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00006940 File Offset: 0x00004B40
		public virtual void match(BitSet b)
		{
			if (!b.member(LA(1)))
			{
				throw new MismatchedTokenException(tokenNames, LT(1), b, false, getFilename());
			}
			consume();
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00006980 File Offset: 0x00004B80
		public virtual void matchNot(int t)
		{
			if (LA(1) == t)
			{
				throw new MismatchedTokenException(tokenNames, LT(1), t, true, getFilename());
			}
			consume();
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x000069B8 File Offset: 0x00004BB8
		[Obsolete("De-activated since version 2.7.2.6 as it cannot be overidden.", true)]
		public static void panic()
		{
			Console.Error.WriteLine("Parser: panic");
			Environment.Exit(1);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x000069DC File Offset: 0x00004BDC
		public virtual void removeMessageListener(MessageListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new SystemException("removeMessageListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x000069FC File Offset: 0x00004BFC
		public virtual void removeParserListener(ParserListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new SystemException("removeParserListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x00006A1C File Offset: 0x00004C1C
		public virtual void removeParserMatchListener(ParserMatchListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new SystemException("removeParserMatchListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00006A3C File Offset: 0x00004C3C
		public virtual void removeParserTokenListener(ParserTokenListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new SystemException("removeParserTokenListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00006A5C File Offset: 0x00004C5C
		public virtual void removeSemanticPredicateListener(SemanticPredicateListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("removeSemanticPredicateListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00006A7C File Offset: 0x00004C7C
		public virtual void removeSyntacticPredicateListener(SyntacticPredicateListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new ArgumentException("removeSyntacticPredicateListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00006A9C File Offset: 0x00004C9C
		public virtual void removeTraceListener(TraceListener l)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new SystemException("removeTraceListener() is only valid if parser built for debugging");
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00006ABC File Offset: 0x00004CBC
		public virtual void reportError(RecognitionException ex)
		{
			Console.Error.WriteLine(ex);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00006AD4 File Offset: 0x00004CD4
		public virtual void reportError(string s)
		{
			if (getFilename() == null)
			{
				Console.Error.WriteLine($"error: {s}");
				return;
			}
			Console.Error.WriteLine($"{getFilename()}: error: {s}");
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00006B1C File Offset: 0x00004D1C
		public virtual void reportWarning(string s)
		{
			if (getFilename() == null)
			{
				Console.Error.WriteLine($"warning: {s}");
				return;
			}
			Console.Error.WriteLine($"{getFilename()}: warning: {s}");
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00006B64 File Offset: 0x00004D64
		public virtual void recover(RecognitionException ex, BitSet tokenSet)
		{
			consume();
			consumeUntil(tokenSet);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00006B80 File Offset: 0x00004D80
		public virtual void rewind(int pos)
		{
			inputState.input.rewind(pos);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00006BA0 File Offset: 0x00004DA0
		public virtual void setASTFactory(ASTFactory f)
		{
			astFactory = f;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00006BB4 File Offset: 0x00004DB4
		public virtual void setASTNodeClass(string cl)
		{
			astFactory.setASTNodeType(cl);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00006BD0 File Offset: 0x00004DD0
		[Obsolete("Replaced by setASTNodeClass(string) since version 2.7.1", true)]
		public virtual void setASTNodeType(string nodeType)
		{
			setASTNodeClass(nodeType);
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00006BE4 File Offset: 0x00004DE4
		public virtual void setDebugMode(bool debugMode)
		{
			if (!ignoreInvalidDebugCalls)
			{
				throw new SystemException("setDebugMode() only valid if parser built for debugging");
			}
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00006C04 File Offset: 0x00004E04
		public virtual void setFilename(string f)
		{
			inputState.filename = f;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00006C20 File Offset: 0x00004E20
		public virtual void setIgnoreInvalidDebugCalls(bool Value)
		{
			ignoreInvalidDebugCalls = Value;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00006C34 File Offset: 0x00004E34
		public virtual void setTokenBuffer(TokenBuffer t)
		{
			inputState.input = t;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00006C50 File Offset: 0x00004E50
		public virtual void traceIndent()
		{
			for (var i = 0; i < traceDepth; i++)
			{
				Console.Out.Write(" ");
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00006C80 File Offset: 0x00004E80
		public virtual void traceIn(string rname)
		{
			traceDepth++;
			traceIndent();
			Console.Out.WriteLine(string.Concat("> ", rname, "; LA(1)==", LT(1).getText(), (inputState.guessing > 0) ? " [guessing]" : ""));
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00006CF8 File Offset: 0x00004EF8
		public virtual void traceOut(string rname)
		{
			traceIndent();
			Console.Out.WriteLine(
                $"< {rname}; LA(1)=={LT(1).getText()}{((inputState.guessing > 0) ? " [guessing]" : "")}");
			traceDepth--;
		}

		// Token: 0x04000073 RID: 115
		private EventHandlerList events_ = new EventHandlerList();

		// Token: 0x04000074 RID: 116
		internal static readonly object EnterRuleEventKey = new object();

		// Token: 0x04000075 RID: 117
		internal static readonly object ExitRuleEventKey = new object();

		// Token: 0x04000076 RID: 118
		internal static readonly object DoneEventKey = new object();

		// Token: 0x04000077 RID: 119
		internal static readonly object ReportErrorEventKey = new object();

		// Token: 0x04000078 RID: 120
		internal static readonly object ReportWarningEventKey = new object();

		// Token: 0x04000079 RID: 121
		internal static readonly object NewLineEventKey = new object();

		// Token: 0x0400007A RID: 122
		internal static readonly object MatchEventKey = new object();

		// Token: 0x0400007B RID: 123
		internal static readonly object MatchNotEventKey = new object();

		// Token: 0x0400007C RID: 124
		internal static readonly object MisMatchEventKey = new object();

		// Token: 0x0400007D RID: 125
		internal static readonly object MisMatchNotEventKey = new object();

		// Token: 0x0400007E RID: 126
		internal static readonly object ConsumeEventKey = new object();

		// Token: 0x0400007F RID: 127
		internal static readonly object LAEventKey = new object();

		// Token: 0x04000080 RID: 128
		internal static readonly object SemPredEvaluatedEventKey = new object();

		// Token: 0x04000081 RID: 129
		internal static readonly object SynPredStartedEventKey = new object();

		// Token: 0x04000082 RID: 130
		internal static readonly object SynPredFailedEventKey = new object();

		// Token: 0x04000083 RID: 131
		internal static readonly object SynPredSucceededEventKey = new object();

		// Token: 0x04000084 RID: 132
		protected internal ParserSharedInputState inputState;

		// Token: 0x04000085 RID: 133
		protected internal string[] tokenNames;

		// Token: 0x04000086 RID: 134
		protected internal AST returnAST;

		// Token: 0x04000087 RID: 135
		protected internal ASTFactory astFactory = new ASTFactory();

		// Token: 0x04000088 RID: 136
		private bool ignoreInvalidDebugCalls = false;

		// Token: 0x04000089 RID: 137
		protected internal int traceDepth = 0;
	}
}
