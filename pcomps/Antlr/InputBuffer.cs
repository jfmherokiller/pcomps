using System.Collections;
using System.Text;

namespace pcomps.Antlr
{
	// Token: 0x0200000F RID: 15
	public abstract class InputBuffer
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x00004040 File Offset: 0x00002240
		public InputBuffer()
		{
			this.queue = new ArrayList();
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004074 File Offset: 0x00002274
		public virtual void commit()
		{
			this.nMarkers--;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004090 File Offset: 0x00002290
		public virtual char consume()
		{
			this.numToConsume++;
			return this.LA(1);
		}

		// Token: 0x060000AB RID: 171
		public abstract void fill(int amount);

		// Token: 0x060000AC RID: 172 RVA: 0x000040B4 File Offset: 0x000022B4
		public virtual string getLAChars()
		{
			StringBuilder stringBuilder = new StringBuilder();
			char[] array = new char[this.queue.Count - this.markerOffset];
			this.queue.CopyTo(array, this.markerOffset);
			stringBuilder.Append(array);
			return stringBuilder.ToString();
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004100 File Offset: 0x00002300
		public virtual string getMarkedChars()
		{
			StringBuilder stringBuilder = new StringBuilder();
			char[] array = new char[this.queue.Count - this.markerOffset];
			this.queue.CopyTo(array, this.markerOffset);
			stringBuilder.Append(array);
			return stringBuilder.ToString();
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000414C File Offset: 0x0000234C
		public virtual bool isMarked()
		{
			return this.nMarkers != 0;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00004168 File Offset: 0x00002368
		public virtual char LA(int i)
		{
			this.fill(i);
			return (char)this.queue[this.markerOffset + i - 1];
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00004198 File Offset: 0x00002398
		public virtual int mark()
		{
			this.syncConsume();
			this.nMarkers++;
			return this.markerOffset;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000041C0 File Offset: 0x000023C0
		public virtual void rewind(int mark)
		{
			this.syncConsume();
			this.markerOffset = mark;
			this.nMarkers--;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000041E8 File Offset: 0x000023E8
		public virtual void reset()
		{
			this.nMarkers = 0;
			this.markerOffset = 0;
			this.numToConsume = 0;
			this.queue.Clear();
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004218 File Offset: 0x00002418
		protected internal virtual void syncConsume()
		{
			if (this.numToConsume > 0)
			{
				if (this.nMarkers > 0)
				{
					this.markerOffset += this.numToConsume;
				}
				else
				{
					this.queue.RemoveRange(0, this.numToConsume);
				}
				this.numToConsume = 0;
			}
		}

		// Token: 0x04000024 RID: 36
		protected internal int nMarkers = 0;

		// Token: 0x04000025 RID: 37
		protected internal int markerOffset = 0;

		// Token: 0x04000026 RID: 38
		protected internal int numToConsume = 0;

		// Token: 0x04000027 RID: 39
		protected ArrayList queue;
	}
}
