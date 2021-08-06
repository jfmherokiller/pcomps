using System.IO;
using System.Text;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x0200007F RID: 127
	public class ANTLRFileStream : ANTLRStringStream
	{
		// Token: 0x060004AE RID: 1198 RVA: 0x0000E748 File Offset: 0x0000C948
		protected ANTLRFileStream()
		{
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x0000E750 File Offset: 0x0000C950
		public ANTLRFileStream(string fileName) : this(fileName, Encoding.Default)
		{
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0000E760 File Offset: 0x0000C960
		public ANTLRFileStream(string fileName, Encoding encoding)
		{
			this.fileName = fileName;
			Load(fileName, encoding);
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060004B1 RID: 1201 RVA: 0x0000E778 File Offset: 0x0000C978
		public override string SourceName
		{
			get
			{
				return fileName;
			}
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0000E780 File Offset: 0x0000C980
		public virtual void Load(string fileName, Encoding encoding)
		{
			if (fileName == null)
			{
				return;
			}
			StreamReader streamReader = null;
			try
			{
				FileInfo file = new FileInfo(fileName);
				int num = (int)GetFileLength(file);
				data = new char[num];
				if (encoding != null)
				{
					streamReader = new StreamReader(fileName, encoding);
				}
				else
				{
					streamReader = new StreamReader(fileName, Encoding.Default);
				}
				n = streamReader.Read(data, 0, data.Length);
			}
			finally
            {
                streamReader?.Close();
            }
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0000E810 File Offset: 0x0000CA10
		private long GetFileLength(FileInfo file)
		{
			if (file.Exists)
			{
				return file.Length;
			}
			return 0L;
		}

		// Token: 0x0400013B RID: 315
		protected string fileName;
	}
}
