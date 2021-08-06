using System;

namespace pcomps.Antlr
{
	// Token: 0x02000040 RID: 64
	[Serializable]
	public class TokenStreamRecognitionException : TokenStreamException
	{
		// Token: 0x06000275 RID: 629 RVA: 0x00008614 File Offset: 0x00006814
		public TokenStreamRecognitionException(RecognitionException re) : base(re.Message)
		{
			this.recog = re;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00008634 File Offset: 0x00006834
		public override string ToString()
		{
			return this.recog.ToString();
		}

		// Token: 0x040000C4 RID: 196
		public RecognitionException recog;
	}
}
