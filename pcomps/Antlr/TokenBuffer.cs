namespace pcomps.Antlr
{
	// Token: 0x0200003A RID: 58
	public class TokenBuffer
	{
		// Token: 0x06000250 RID: 592 RVA: 0x00007F44 File Offset: 0x00006144
		public TokenBuffer(TokenStream input_)
		{
			input = input_;
			queue = new TokenQueue(1);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00007F80 File Offset: 0x00006180
		public virtual void reset()
		{
			nMarkers = 0;
			markerOffset = 0;
			numToConsume = 0;
			queue.reset();
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00007FB0 File Offset: 0x000061B0
		public virtual void consume()
		{
			numToConsume++;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00007FCC File Offset: 0x000061CC
		protected virtual void fill(int amount)
		{
			syncConsume();
			while (queue.nbrEntries < amount + markerOffset)
			{
				queue.append(input.nextToken());
			}
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000800C File Offset: 0x0000620C
		public virtual TokenStream getInput()
		{
			return input;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00008020 File Offset: 0x00006220
		public virtual int LA(int i)
		{
			fill(i);
			return queue.elementAt(markerOffset + i - 1).Type;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00008050 File Offset: 0x00006250
		public virtual IToken LT(int i)
		{
			fill(i);
			return queue.elementAt(markerOffset + i - 1);
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000807C File Offset: 0x0000627C
		public virtual int mark()
		{
			syncConsume();
			nMarkers++;
			return markerOffset;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x000080A4 File Offset: 0x000062A4
		public virtual void rewind(int mark)
		{
			syncConsume();
			markerOffset = mark;
			nMarkers--;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x000080CC File Offset: 0x000062CC
		protected virtual void syncConsume()
		{
			while (numToConsume > 0)
			{
				if (nMarkers > 0)
				{
					markerOffset++;
				}
				else
				{
					queue.removeFirst();
				}
				numToConsume--;
			}
		}

		// Token: 0x040000B4 RID: 180
		protected internal TokenStream input;

		// Token: 0x040000B5 RID: 181
		protected internal int nMarkers = 0;

		// Token: 0x040000B6 RID: 182
		protected internal int markerOffset = 0;

		// Token: 0x040000B7 RID: 183
		protected internal int numToConsume = 0;

		// Token: 0x040000B8 RID: 184
		internal TokenQueue queue;
	}
}
