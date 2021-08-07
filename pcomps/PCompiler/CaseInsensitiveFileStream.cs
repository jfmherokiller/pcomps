using System.Text;
using pcomps.Antlr.Runtime;

namespace pcomps.PCompiler
{
	// Token: 0x02000134 RID: 308
	internal class CaseInsensitiveFileStream : ICharStream
	{
		// Token: 0x06000ABF RID: 2751 RVA: 0x00032500 File Offset: 0x00030700
		public CaseInsensitiveFileStream(string asFileName)
		{
			var antlrfileStream = new ANTLRFileStream(asFileName);
			var text = antlrfileStream.Substring(0, antlrfileStream.Count - 1);
			text += '\n';
			kStringStream = new ANTLRStringStream(text);
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x00032544 File Offset: 0x00030744
		public CaseInsensitiveFileStream(string asFileName, Encoding akEncoding)
		{
			var antlrfileStream = new ANTLRFileStream(asFileName, akEncoding);
			var text = antlrfileStream.Substring(0, antlrfileStream.Count - 1);
			text += '\n';
			kStringStream = new ANTLRStringStream(text);
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000AC1 RID: 2753 RVA: 0x0003258C File Offset: 0x0003078C
		// (set) Token: 0x06000AC2 RID: 2754 RVA: 0x0003259C File Offset: 0x0003079C
		public int CharPositionInLine
		{
			get => kStringStream.CharPositionInLine;
            set => kStringStream.CharPositionInLine = value;
        }

		// Token: 0x06000AC3 RID: 2755 RVA: 0x000325AC File Offset: 0x000307AC
		public int LT(int i)
		{
			return kStringStream.LT(i);
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000AC4 RID: 2756 RVA: 0x000325BC File Offset: 0x000307BC
		// (set) Token: 0x06000AC5 RID: 2757 RVA: 0x000325CC File Offset: 0x000307CC
		public int Line
		{
			get => kStringStream.Line;
            set => kStringStream.Line = value;
        }

		// Token: 0x06000AC6 RID: 2758 RVA: 0x000325DC File Offset: 0x000307DC
		public string Substring(int start, int stop)
		{
			return kStringStream.Substring(start, stop);
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x000325EC File Offset: 0x000307EC
		public void Consume()
		{
			kStringStream.Consume();
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000AC8 RID: 2760 RVA: 0x000325FC File Offset: 0x000307FC
		public int Count => kStringStream.Count;

        // Token: 0x06000AC9 RID: 2761 RVA: 0x0003260C File Offset: 0x0003080C
		public int Index()
		{
			return kStringStream.Index();
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0003261C File Offset: 0x0003081C
		public int LA(int iOffset)
		{
			var num = kStringStream.LA(iOffset);
			return num == -1 ? num : char.ToLowerInvariant((char)num);
        }

		// Token: 0x06000ACB RID: 2763 RVA: 0x00032644 File Offset: 0x00030844
		public int Mark()
		{
			return kStringStream.Mark();
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x00032654 File Offset: 0x00030854
		public void Release(int marker)
		{
			kStringStream.Release(marker);
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x00032664 File Offset: 0x00030864
		public void Rewind()
		{
			kStringStream.Rewind();
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x00032674 File Offset: 0x00030874
		public void Rewind(int marker)
		{
			kStringStream.Rewind(marker);
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x00032684 File Offset: 0x00030884
		public void Seek(int index)
		{
			kStringStream.Seek(index);
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x00032694 File Offset: 0x00030894
		public int Size() => kStringStream.Count;

        // Token: 0x17000133 RID: 307
		// (get) Token: 0x06000AD1 RID: 2769 RVA: 0x000326A4 File Offset: 0x000308A4
		public string SourceName => kStringStream.SourceName;

        // Token: 0x040004FF RID: 1279
		private ANTLRStringStream kStringStream;
	}
}
