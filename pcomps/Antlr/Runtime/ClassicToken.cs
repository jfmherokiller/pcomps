using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000085 RID: 133
	[Serializable]
	public class ClassicToken : IToken
	{
		// Token: 0x060004EF RID: 1263 RVA: 0x0000F180 File Offset: 0x0000D380
		public ClassicToken(int type)
		{
			this.type = type;
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0000F190 File Offset: 0x0000D390
		public ClassicToken(IToken oldToken)
		{
			this.text = oldToken.Text;
			this.type = oldToken.Type;
			this.line = oldToken.Line;
			this.charPositionInLine = oldToken.CharPositionInLine;
			this.channel = oldToken.Channel;
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0000F1E0 File Offset: 0x0000D3E0
		public ClassicToken(int type, string text)
		{
			this.type = type;
			this.text = text;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x0000F1F8 File Offset: 0x0000D3F8
		public ClassicToken(int type, string text, int channel)
		{
			this.type = type;
			this.text = text;
			this.channel = channel;
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060004F3 RID: 1267 RVA: 0x0000F218 File Offset: 0x0000D418
		// (set) Token: 0x060004F4 RID: 1268 RVA: 0x0000F220 File Offset: 0x0000D420
		public virtual int Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060004F5 RID: 1269 RVA: 0x0000F22C File Offset: 0x0000D42C
		// (set) Token: 0x060004F6 RID: 1270 RVA: 0x0000F234 File Offset: 0x0000D434
		public virtual int Line
		{
			get
			{
				return this.line;
			}
			set
			{
				this.line = value;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060004F7 RID: 1271 RVA: 0x0000F240 File Offset: 0x0000D440
		// (set) Token: 0x060004F8 RID: 1272 RVA: 0x0000F248 File Offset: 0x0000D448
		public virtual int CharPositionInLine
		{
			get
			{
				return this.charPositionInLine;
			}
			set
			{
				this.charPositionInLine = value;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x0000F254 File Offset: 0x0000D454
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x0000F25C File Offset: 0x0000D45C
		public virtual int Channel
		{
			get
			{
				return this.channel;
			}
			set
			{
				this.channel = value;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x0000F268 File Offset: 0x0000D468
		// (set) Token: 0x060004FC RID: 1276 RVA: 0x0000F270 File Offset: 0x0000D470
		public virtual int TokenIndex
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x0000F27C File Offset: 0x0000D47C
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x0000F284 File Offset: 0x0000D484
		public virtual string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x0000F290 File Offset: 0x0000D490
		// (set) Token: 0x06000500 RID: 1280 RVA: 0x0000F294 File Offset: 0x0000D494
		public virtual ICharStream InputStream
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0000F298 File Offset: 0x0000D498
		public override string ToString()
		{
			string text = string.Empty;
			if (this.channel > 0)
			{
				text = $",channel={this.channel}";
			}
			string text2 = this.Text;
			if (text2 != null)
			{
				text2 = text2.Replace("\n", "\\\\n");
				text2 = text2.Replace("\r", "\\\\r");
				text2 = text2.Replace("\t", "\\\\t");
			}
			else
			{
				text2 = "<no text>";
			}
			return string.Concat(new object[]
			{
				"[@",
				this.TokenIndex,
				",'",
				text2,
				"',<",
				this.type,
				">",
				text,
				",",
				this.line,
				":",
				this.CharPositionInLine,
				"]"
			});
		}

		// Token: 0x0400014E RID: 334
		protected internal string text;

		// Token: 0x0400014F RID: 335
		protected internal int type;

		// Token: 0x04000150 RID: 336
		protected internal int line;

		// Token: 0x04000151 RID: 337
		protected internal int charPositionInLine;

		// Token: 0x04000152 RID: 338
		protected internal int channel;

		// Token: 0x04000153 RID: 339
		protected internal int index;
	}
}
