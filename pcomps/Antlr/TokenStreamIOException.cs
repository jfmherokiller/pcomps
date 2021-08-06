using System;
using System.IO;

namespace pcomps.Antlr
{
	// Token: 0x0200003F RID: 63
	[Serializable]
	public class TokenStreamIOException : TokenStreamException
	{
		// Token: 0x06000274 RID: 628 RVA: 0x000085F4 File Offset: 0x000067F4
		public TokenStreamIOException(IOException io) : base(io.Message)
		{
			this.io = io;
		}

		// Token: 0x040000C3 RID: 195
		public IOException io;
	}
}
