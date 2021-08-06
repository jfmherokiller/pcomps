using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x0200008B RID: 139
	public interface IIntStream
	{
		// Token: 0x0600052C RID: 1324
		void Consume();

		// Token: 0x0600052D RID: 1325
		int LA(int i);

		// Token: 0x0600052E RID: 1326
		int Mark();

		// Token: 0x0600052F RID: 1327
		int Index();

		// Token: 0x06000530 RID: 1328
		void Rewind(int marker);

		// Token: 0x06000531 RID: 1329
		void Rewind();

		// Token: 0x06000532 RID: 1330
		void Release(int marker);

		// Token: 0x06000533 RID: 1331
		void Seek(int index);

		// Token: 0x06000534 RID: 1332
		[Obsolete("Please use property Count instead.")]
		int Size();

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000535 RID: 1333
		int Count { get; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000536 RID: 1334
		string SourceName { get; }
	}
}
