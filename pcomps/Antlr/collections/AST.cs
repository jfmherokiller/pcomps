using System;
using System.Collections;

namespace pcomps.Antlr.collections
{
	// Token: 0x0200000A RID: 10
	public interface AST : ICloneable
	{
		// Token: 0x06000043 RID: 67
		void addChild(AST c);

		// Token: 0x06000044 RID: 68
		bool Equals(AST t);

		// Token: 0x06000045 RID: 69
		bool EqualsList(AST t);

		// Token: 0x06000046 RID: 70
		bool EqualsListPartial(AST t);

		// Token: 0x06000047 RID: 71
		bool EqualsTree(AST t);

		// Token: 0x06000048 RID: 72
		bool EqualsTreePartial(AST t);

		// Token: 0x06000049 RID: 73
		IEnumerator findAll(AST tree);

		// Token: 0x0600004A RID: 74
		IEnumerator findAllPartial(AST subtree);

		// Token: 0x0600004B RID: 75
		AST getFirstChild();

		// Token: 0x0600004C RID: 76
		AST getNextSibling();

		// Token: 0x0600004D RID: 77
		string getText();

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004E RID: 78
		// (set) Token: 0x0600004F RID: 79
		int Type { get; set; }

		// Token: 0x06000050 RID: 80
		int getNumberOfChildren();

		// Token: 0x06000051 RID: 81
		void initialize(int t, string txt);

		// Token: 0x06000052 RID: 82
		void initialize(AST t);

		// Token: 0x06000053 RID: 83
		void initialize(IToken t);

		// Token: 0x06000054 RID: 84
		void setFirstChild(AST c);

		// Token: 0x06000055 RID: 85
		void setNextSibling(AST n);

		// Token: 0x06000056 RID: 86
		void setText(string text);

		// Token: 0x06000057 RID: 87
		void setType(int ttype);

		// Token: 0x06000058 RID: 88
		string ToString();

		// Token: 0x06000059 RID: 89
		string ToStringList();

		// Token: 0x0600005A RID: 90
		string ToStringTree();

		// Token: 0x0600005B RID: 91
		int getLine();

		// Token: 0x0600005C RID: 92
		int getColumn();
	}
}
