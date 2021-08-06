using System;
using System.IO;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000DA RID: 218
	public class ANTLRReaderStream : ANTLRStringStream
	{
		// Token: 0x060008C0 RID: 2240 RVA: 0x00018F30 File Offset: 0x00017130
		protected ANTLRReaderStream()
		{
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x00018F38 File Offset: 0x00017138
		public ANTLRReaderStream(TextReader reader) : this(reader, ANTLRReaderStream.INITIAL_BUFFER_SIZE, ANTLRReaderStream.READ_BUFFER_SIZE)
		{
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x00018F4C File Offset: 0x0001714C
		public ANTLRReaderStream(TextReader reader, int size) : this(reader, size, ANTLRReaderStream.READ_BUFFER_SIZE)
		{
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x00018F5C File Offset: 0x0001715C
		public ANTLRReaderStream(TextReader reader, int size, int readChunkSize)
		{
			this.Load(reader, size, readChunkSize);
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00018F88 File Offset: 0x00017188
		public virtual void Load(TextReader reader, int size, int readChunkSize)
		{
			if (reader == null)
			{
				return;
			}
			if (size <= 0)
			{
				size = ANTLRReaderStream.INITIAL_BUFFER_SIZE;
			}
			if (readChunkSize <= 0)
			{
				readChunkSize = ANTLRReaderStream.READ_BUFFER_SIZE;
			}
			try
			{
				this.data = new char[size];
				int num = 0;
				int num2;
				do
				{
					if (num + readChunkSize > this.data.Length)
					{
						char[] array = new char[this.data.Length * 2];
						Array.Copy(this.data, 0, array, 0, this.data.Length);
						this.data = array;
					}
					num2 = reader.Read(this.data, num, readChunkSize);
					num += num2;
				}
				while (num2 != 0);
				this.n = num;
			}
			finally
			{
				reader.Close();
			}
		}

		// Token: 0x0400025D RID: 605
		public static readonly int READ_BUFFER_SIZE = 1024;

		// Token: 0x0400025E RID: 606
		public static readonly int INITIAL_BUFFER_SIZE = 1024;
	}
}
