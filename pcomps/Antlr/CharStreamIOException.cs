using System;
using System.IO;

namespace pcomps.Antlr
{
	// Token: 0x0200001A RID: 26
	[Serializable]
	public class CharStreamIOException : CharStreamException
	{
		// Token: 0x0600014A RID: 330 RVA: 0x000057E0 File Offset: 0x000039E0
		public CharStreamIOException(IOException io) : base(io.Message)
		{
			this.io = io;
		}

		// Token: 0x04000055 RID: 85
		public IOException io;
	}
}
