using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x0200008A RID: 138
	[Serializable]
	public class FailedPredicateException : RecognitionException
	{
		// Token: 0x06000529 RID: 1321 RVA: 0x0000FA54 File Offset: 0x0000DC54
		public FailedPredicateException()
		{
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0000FA5C File Offset: 0x0000DC5C
		public FailedPredicateException(IIntStream input, string ruleName, string predicateText) : base(input)
		{
			this.ruleName = ruleName;
			this.predicateText = predicateText;
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0000FA74 File Offset: 0x0000DC74
		public override string ToString()
		{
			return string.Concat("FailedPredicateException(", ruleName, ",{", predicateText, "}?)");
		}

		// Token: 0x04000169 RID: 361
		public string ruleName;

		// Token: 0x0400016A RID: 362
		public string predicateText;
	}
}
