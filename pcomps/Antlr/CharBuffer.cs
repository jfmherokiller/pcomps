using System;
using System.IO;

namespace pcomps.Antlr
{
	// Token: 0x02000011 RID: 17
	public class CharBuffer : InputBuffer
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x00004334 File Offset: 0x00002534
		public CharBuffer(TextReader input_)
		{
			this.input = input_;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000435C File Offset: 0x0000255C
		public override void fill(int amount)
		{
			try
			{
				this.syncConsume();
				int num;
				for (int i = amount + this.markerOffset - this.queue.Count; i > 0; i -= num)
				{
					num = this.input.Read(this.buf, 0, 16);
					for (int j = 0; j < num; j++)
					{
						this.queue.Add(this.buf[j]);
					}
					if (num < 16)
					{
						while (i-- > 0)
						{
							if (this.queue.Count >= 16)
							{
								break;
							}
							this.queue.Add(CharScanner.EOF_CHAR);
						}
						break;
					}
				}
			}
			catch (IOException io)
			{
				throw new CharStreamIOException(io);
			}
		}

		// Token: 0x0400002B RID: 43
		private const int BUF_SIZE = 16;

		// Token: 0x0400002C RID: 44
		[NonSerialized]
		internal TextReader input;

		// Token: 0x0400002D RID: 45
		private char[] buf = new char[16];
	}
}
