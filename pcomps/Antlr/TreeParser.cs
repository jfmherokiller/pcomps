using System;
using pcomps.Antlr.collections;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr
{
	// Token: 0x0200004A RID: 74
	public class TreeParser
	{
		// Token: 0x060002BF RID: 703 RVA: 0x00009178 File Offset: 0x00007378
		public TreeParser()
		{
			inputState = new TreeParserSharedInputState();
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x000091A8 File Offset: 0x000073A8
		public virtual AST getAST()
		{
			return returnAST;
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x000091BC File Offset: 0x000073BC
		public virtual ASTFactory getASTFactory()
		{
			return astFactory;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x000091D0 File Offset: 0x000073D0
		public virtual void resetState()
		{
			traceDepth = 0;
			returnAST = null;
			retTree_ = null;
			inputState.reset();
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00009200 File Offset: 0x00007400
		public virtual string getTokenName(int num)
		{
			return tokenNames[num];
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00009218 File Offset: 0x00007418
		public virtual string[] getTokenNames()
		{
			return tokenNames;
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000922C File Offset: 0x0000742C
		protected internal virtual void match(AST t, int ttype)
		{
			if (t == null || t == ASTNULL || t.Type != ttype)
			{
				throw new MismatchedTokenException(getTokenNames(), t, ttype, false);
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000925C File Offset: 0x0000745C
		public virtual void match(AST t, BitSet b)
		{
			if (t == null || t == ASTNULL || !b.member(t.Type))
			{
				throw new MismatchedTokenException(getTokenNames(), t, b, false);
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00009294 File Offset: 0x00007494
		protected internal virtual void matchNot(AST t, int ttype)
		{
			if (t == null || t == ASTNULL || t.Type == ttype)
			{
				throw new MismatchedTokenException(getTokenNames(), t, ttype, true);
			}
		}

        // Token: 0x060002C9 RID: 713 RVA: 0x000092E8 File Offset: 0x000074E8
		public virtual void reportError(RecognitionException ex)
		{
			Console.Error.WriteLine(ex.ToString());
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00009308 File Offset: 0x00007508
		public virtual void reportError(string s)
		{
			Console.Error.WriteLine($"error: {s}");
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000932C File Offset: 0x0000752C
		public virtual void reportWarning(string s)
		{
			Console.Error.WriteLine($"warning: {s}");
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00009350 File Offset: 0x00007550
		public virtual void setASTFactory(ASTFactory f)
		{
			astFactory = f;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00009364 File Offset: 0x00007564
		public virtual void setASTNodeType(string nodeType)
		{
			setASTNodeClass(nodeType);
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00009378 File Offset: 0x00007578
		public virtual void setASTNodeClass(string nodeType)
		{
			astFactory.setASTNodeType(nodeType);
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00009394 File Offset: 0x00007594
		public virtual void traceIndent()
		{
			for (var i = 0; i < traceDepth; i++)
			{
				Console.Out.Write(" ");
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x000093C4 File Offset: 0x000075C4
		public virtual void traceIn(string rname, AST t)
		{
			traceDepth++;
			traceIndent();
			Console.Out.WriteLine(string.Concat("> ", rname, "(", (t != null) ? t.ToString() : "null", ")", (inputState.guessing > 0) ? " [guessing]" : ""));
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00009448 File Offset: 0x00007648
		public virtual void traceOut(string rname, AST t)
		{
			traceIndent();
			Console.Out.WriteLine(
                $"< {rname}({((t != null) ? t.ToString() : "null")}){((inputState.guessing > 0) ? " [guessing]" : "")}");
			traceDepth--;
		}

		// Token: 0x040000D6 RID: 214
		public static ASTNULLType ASTNULL = new ASTNULLType();

		// Token: 0x040000D7 RID: 215
		protected internal AST retTree_;

		// Token: 0x040000D8 RID: 216
		protected internal TreeParserSharedInputState inputState;

		// Token: 0x040000D9 RID: 217
		protected internal string[] tokenNames;

		// Token: 0x040000DA RID: 218
		protected internal AST returnAST;

		// Token: 0x040000DB RID: 219
		protected internal ASTFactory astFactory = new ASTFactory();

		// Token: 0x040000DC RID: 220
		protected internal int traceDepth = 0;
	}
}
