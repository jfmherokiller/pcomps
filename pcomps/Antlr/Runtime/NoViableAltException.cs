using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000091 RID: 145
	[Serializable]
	public class NoViableAltException : RecognitionException
	{
		// Token: 0x0600054C RID: 1356 RVA: 0x0000FCF0 File Offset: 0x0000DEF0
		public NoViableAltException()
		{
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0000FCF8 File Offset: 0x0000DEF8
		public NoViableAltException(string grammarDecisionDescription, int decisionNumber, int stateNumber, IIntStream input) : base(input)
		{
			this.grammarDecisionDescription = grammarDecisionDescription;
			this.decisionNumber = decisionNumber;
			this.stateNumber = stateNumber;
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x0000FD18 File Offset: 0x0000DF18
		public override string ToString()
		{
			if (input is ICharStream)
			{
				return string.Concat("NoViableAltException('", (char)UnexpectedType, "'@[", grammarDecisionDescription, "])");
			}
			return string.Concat("NoViableAltException(", UnexpectedType, "@[", grammarDecisionDescription, "])");
		}

		// Token: 0x04000170 RID: 368
		public string grammarDecisionDescription;

		// Token: 0x04000171 RID: 369
		public int decisionNumber;

		// Token: 0x04000172 RID: 370
		public int stateNumber;
	}
}
