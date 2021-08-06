using System.IO;
using System.Text;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000D9 RID: 217
	public class ANTLRInputStream : ANTLRReaderStream
	{
		// Token: 0x060008BA RID: 2234 RVA: 0x00018EB8 File Offset: 0x000170B8
		protected ANTLRInputStream()
		{
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x00018EC0 File Offset: 0x000170C0
		public ANTLRInputStream(Stream istream) : this(istream, null)
		{
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x00018ECC File Offset: 0x000170CC
		public ANTLRInputStream(Stream istream, Encoding encoding) : this(istream, ANTLRReaderStream.INITIAL_BUFFER_SIZE, encoding)
		{
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x00018EDC File Offset: 0x000170DC
		public ANTLRInputStream(Stream istream, int size) : this(istream, size, null)
		{
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x00018EE8 File Offset: 0x000170E8
		public ANTLRInputStream(Stream istream, int size, Encoding encoding) : this(istream, size, ANTLRReaderStream.READ_BUFFER_SIZE, encoding)
		{
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x00018EF8 File Offset: 0x000170F8
		public ANTLRInputStream(Stream istream, int size, int readBufferSize, Encoding encoding)
		{
			StreamReader reader;
			if (encoding != null)
			{
				reader = new StreamReader(istream, encoding);
			}
			else
			{
				reader = new StreamReader(istream);
			}
			this.Load(reader, size, readBufferSize);
		}
	}
}
