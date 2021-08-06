using System;
using System.IO;

namespace pcomps.Antlr
{
	// Token: 0x02000010 RID: 16
	public class ByteBuffer : InputBuffer
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x00004268 File Offset: 0x00002468
		public ByteBuffer(Stream input_)
		{
			input = input_;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00004290 File Offset: 0x00002490
		public override void fill(int amount)
		{
			syncConsume();
			int num;
			for (var i = amount + markerOffset - queue.Count; i > 0; i -= num)
			{
				num = input.Read(buf, 0, 16);
				for (var j = 0; j < num; j++)
				{
					queue.Add((char)buf[j]);
				}
				if (num < 16)
				{
					while (i-- > 0)
					{
						if (queue.Count >= 16)
						{
							return;
						}
						queue.Add(CharScanner.EOF_CHAR);
					}
					break;
				}
			}
		}

		// Token: 0x04000028 RID: 40
		private const int BUF_SIZE = 16;

		// Token: 0x04000029 RID: 41
		[NonSerialized]
		internal Stream input;

		// Token: 0x0400002A RID: 42
		private byte[] buf = new byte[16];
	}
}
