using System;
using System.Collections;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000080 RID: 128
	public class ANTLRStringStream : ICharStream, IIntStream
	{
		// Token: 0x060004B4 RID: 1204 RVA: 0x0000E828 File Offset: 0x0000CA28
		protected ANTLRStringStream()
		{
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x0000E838 File Offset: 0x0000CA38
		public ANTLRStringStream(string input)
		{
			data = input.ToCharArray();
			n = input.Length;
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x0000E860 File Offset: 0x0000CA60
		public ANTLRStringStream(char[] data, int numberOfActualCharsInArray)
		{
			this.data = data;
			n = numberOfActualCharsInArray;
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060004B7 RID: 1207 RVA: 0x0000E880 File Offset: 0x0000CA80
		// (set) Token: 0x060004B8 RID: 1208 RVA: 0x0000E888 File Offset: 0x0000CA88
		public virtual int Line
		{
			get
			{
				return line;
			}
			set
			{
				line = value;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060004B9 RID: 1209 RVA: 0x0000E894 File Offset: 0x0000CA94
		// (set) Token: 0x060004BA RID: 1210 RVA: 0x0000E89C File Offset: 0x0000CA9C
		public virtual int CharPositionInLine
		{
			get
			{
				return charPositionInLine;
			}
			set
			{
				charPositionInLine = value;
			}
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x0000E8A8 File Offset: 0x0000CAA8
		public virtual void Reset()
		{
			p = 0;
			line = 1;
			charPositionInLine = 0;
			markDepth = 0;
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0000E8C8 File Offset: 0x0000CAC8
		public virtual void Consume()
		{
			if (p < n)
			{
				charPositionInLine++;
				if (data[p] == '\n')
				{
					line++;
					charPositionInLine = 0;
				}
				p++;
			}
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0000E92C File Offset: 0x0000CB2C
		public virtual int LA(int i)
		{
			if (i == 0)
			{
				return 0;
			}
			if (i < 0)
			{
				i++;
				if (p + i - 1 < 0)
				{
					return -1;
				}
			}
			if (p + i - 1 >= n)
			{
				return -1;
			}
			return (int)data[p + i - 1];
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0000E988 File Offset: 0x0000CB88
		public virtual int LT(int i)
		{
			return LA(i);
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0000E994 File Offset: 0x0000CB94
		public virtual int Index()
		{
			return p;
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0000E99C File Offset: 0x0000CB9C
		[Obsolete("Please use property Count instead.")]
		public virtual int Size()
		{
			return Count;
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x0000E9A4 File Offset: 0x0000CBA4
		public virtual int Count
		{
			get
			{
				return n;
			}
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0000E9AC File Offset: 0x0000CBAC
		public virtual int Mark()
		{
			if (markers == null)
			{
				markers = new ArrayList();
				markers.Add(null);
			}
			markDepth++;
			CharStreamState charStreamState;
			if (markDepth >= markers.Count)
			{
				charStreamState = new CharStreamState();
				markers.Add(charStreamState);
			}
			else
			{
				charStreamState = (CharStreamState)markers[markDepth];
			}
			charStreamState.p = p;
			charStreamState.line = line;
			charStreamState.charPositionInLine = charPositionInLine;
			lastMarker = markDepth;
			return markDepth;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0000EA68 File Offset: 0x0000CC68
		public virtual void Rewind(int m)
		{
			CharStreamState charStreamState = (CharStreamState)markers[m];
			Seek(charStreamState.p);
			line = charStreamState.line;
			charPositionInLine = charStreamState.charPositionInLine;
			Release(m);
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0000EAB4 File Offset: 0x0000CCB4
		public virtual void Rewind()
		{
			Rewind(lastMarker);
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0000EAC4 File Offset: 0x0000CCC4
		public virtual void Release(int marker)
		{
			markDepth = marker;
			markDepth--;
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0000EADC File Offset: 0x0000CCDC
		public virtual void Seek(int index)
		{
			if (index <= p)
			{
				p = index;
				return;
			}
			while (p < index)
			{
				Consume();
			}
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0000EB0C File Offset: 0x0000CD0C
		public virtual string Substring(int start, int stop)
		{
			return new string(data, start, stop - start + 1);
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060004C8 RID: 1224 RVA: 0x0000EB20 File Offset: 0x0000CD20
		// (set) Token: 0x060004C9 RID: 1225 RVA: 0x0000EB28 File Offset: 0x0000CD28
		public virtual string SourceName
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		// Token: 0x0400013C RID: 316
		protected internal char[] data;

		// Token: 0x0400013D RID: 317
		protected int n;

		// Token: 0x0400013E RID: 318
		protected internal int p;

		// Token: 0x0400013F RID: 319
		protected internal int line = 1;

		// Token: 0x04000140 RID: 320
		protected internal int charPositionInLine;

		// Token: 0x04000141 RID: 321
		protected internal int markDepth;

		// Token: 0x04000142 RID: 322
		protected internal IList markers;

		// Token: 0x04000143 RID: 323
		protected int lastMarker;

		// Token: 0x04000144 RID: 324
		protected string name;
	}
}
