using System.IO;

namespace pcomps.Antlr
{
	// Token: 0x02000029 RID: 41
	public class LexerSharedInputState
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x00006174 File Offset: 0x00004374
		public LexerSharedInputState(InputBuffer inbuf)
		{
			this.initialize();
			this.input = inbuf;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00006194 File Offset: 0x00004394
		public LexerSharedInputState(Stream inStream) : this(new ByteBuffer(inStream))
		{
		}

		// Token: 0x060001AA RID: 426 RVA: 0x000061B0 File Offset: 0x000043B0
		public LexerSharedInputState(TextReader inReader) : this(new CharBuffer(inReader))
		{
		}

		// Token: 0x060001AB RID: 427 RVA: 0x000061CC File Offset: 0x000043CC
		private void initialize()
		{
			this.column = 1;
			this.line = 1;
			this.tokenStartColumn = 1;
			this.tokenStartLine = 1;
			this.guessing = 0;
			this.filename = null;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00006204 File Offset: 0x00004404
		public virtual void reset()
		{
			this.initialize();
			this.input.reset();
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00006224 File Offset: 0x00004424
		public virtual void resetInput(InputBuffer ib)
		{
			this.reset();
			this.input = ib;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00006240 File Offset: 0x00004440
		public virtual void resetInput(Stream s)
		{
			this.reset();
			this.input = new ByteBuffer(s);
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00006260 File Offset: 0x00004460
		public virtual void resetInput(TextReader tr)
		{
			this.reset();
			this.input = new CharBuffer(tr);
		}

		// Token: 0x0400006C RID: 108
		protected internal int column;

		// Token: 0x0400006D RID: 109
		protected internal int line;

		// Token: 0x0400006E RID: 110
		protected internal int tokenStartColumn;

		// Token: 0x0400006F RID: 111
		protected internal int tokenStartLine;

		// Token: 0x04000070 RID: 112
		protected internal InputBuffer input;

		// Token: 0x04000071 RID: 113
		protected internal string filename;

		// Token: 0x04000072 RID: 114
		public int guessing;
	}
}
