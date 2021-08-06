using System;
using System.Collections;
using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x0200000B RID: 11
	public class ASTNULLType : AST, ICloneable
	{
		// Token: 0x0600005D RID: 93 RVA: 0x000032CC File Offset: 0x000014CC
		public virtual void addChild(AST c)
		{
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000032DC File Offset: 0x000014DC
		public virtual bool Equals(AST t)
		{
			return false;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000032EC File Offset: 0x000014EC
		public virtual bool EqualsList(AST t)
		{
			return false;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000032FC File Offset: 0x000014FC
		public virtual bool EqualsListPartial(AST t)
		{
			return false;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000330C File Offset: 0x0000150C
		public virtual bool EqualsTree(AST t)
		{
			return false;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000331C File Offset: 0x0000151C
		public virtual bool EqualsTreePartial(AST t)
		{
			return false;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000332C File Offset: 0x0000152C
		public virtual IEnumerator findAll(AST tree)
		{
			return null;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000333C File Offset: 0x0000153C
		public virtual IEnumerator findAllPartial(AST subtree)
		{
			return null;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000334C File Offset: 0x0000154C
		public virtual AST getFirstChild()
		{
			return this;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000335C File Offset: 0x0000155C
		public virtual AST getNextSibling()
		{
			return this;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000336C File Offset: 0x0000156C
		public virtual int getLine()
		{
			return 0;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000337C File Offset: 0x0000157C
		public virtual int getColumn()
		{
			return 0;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000338C File Offset: 0x0000158C
		public virtual string getText()
		{
			return "<ASTNULL>";
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600006A RID: 106 RVA: 0x000033A0 File Offset: 0x000015A0
		// (set) Token: 0x0600006B RID: 107 RVA: 0x000033B0 File Offset: 0x000015B0
		public virtual int Type
		{
			get
			{
				return 3;
			}
			set
			{
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000033C0 File Offset: 0x000015C0
		public int getNumberOfChildren()
		{
			return 0;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000033D0 File Offset: 0x000015D0
		public virtual void initialize(int t, string txt)
		{
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000033E0 File Offset: 0x000015E0
		public virtual void initialize(AST t)
		{
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000033F0 File Offset: 0x000015F0
		public virtual void initialize(IToken t)
		{
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003400 File Offset: 0x00001600
		public virtual void setFirstChild(AST c)
		{
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003410 File Offset: 0x00001610
		public virtual void setNextSibling(AST n)
		{
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003420 File Offset: 0x00001620
		public virtual void setText(string text)
		{
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003430 File Offset: 0x00001630
		public virtual void setType(int ttype)
		{
			this.Type = ttype;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00003444 File Offset: 0x00001644
		public override string ToString()
		{
			return this.getText();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003458 File Offset: 0x00001658
		public virtual string ToStringList()
		{
			return this.getText();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000346C File Offset: 0x0000166C
		public virtual string ToStringTree()
		{
			return this.getText();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003480 File Offset: 0x00001680
		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
