using System;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000086 RID: 134
	[Serializable]
	public class CommonToken : IToken
	{
		// Token: 0x06000502 RID: 1282 RVA: 0x0000F3A0 File Offset: 0x0000D5A0
		public CommonToken(int type)
		{
			this.type = type;
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0000F3C0 File Offset: 0x0000D5C0
		public CommonToken(ICharStream input, int type, int channel, int start, int stop)
		{
			this.input = input;
			this.type = type;
			this.channel = channel;
			this.start = start;
			this.stop = stop;
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0000F3FC File Offset: 0x0000D5FC
		public CommonToken(int type, string text)
		{
			this.type = type;
			this.channel = 0;
			this.text = text;
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0000F428 File Offset: 0x0000D628
		public CommonToken(IToken oldToken)
		{
			this.text = oldToken.Text;
			this.type = oldToken.Type;
			this.line = oldToken.Line;
			this.index = oldToken.TokenIndex;
			this.charPositionInLine = oldToken.CharPositionInLine;
			this.channel = oldToken.Channel;
			if (oldToken is CommonToken)
			{
				this.start = ((CommonToken)oldToken).start;
				this.stop = ((CommonToken)oldToken).stop;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000506 RID: 1286 RVA: 0x0000F4C0 File Offset: 0x0000D6C0
		// (set) Token: 0x06000507 RID: 1287 RVA: 0x0000F4C8 File Offset: 0x0000D6C8
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x0000F4D4 File Offset: 0x0000D6D4
		// (set) Token: 0x06000509 RID: 1289 RVA: 0x0000F4DC File Offset: 0x0000D6DC
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600050A RID: 1290 RVA: 0x0000F4E8 File Offset: 0x0000D6E8
		// (set) Token: 0x0600050B RID: 1291 RVA: 0x0000F4F0 File Offset: 0x0000D6F0
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

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0000F4FC File Offset: 0x0000D6FC
		// (set) Token: 0x0600050D RID: 1293 RVA: 0x0000F504 File Offset: 0x0000D704
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600050E RID: 1294 RVA: 0x0000F510 File Offset: 0x0000D710
		// (set) Token: 0x0600050F RID: 1295 RVA: 0x0000F518 File Offset: 0x0000D718
		public virtual int StartIndex
		{
			get
			{
				return this.start;
			}
			set
			{
				this.start = value;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000510 RID: 1296 RVA: 0x0000F524 File Offset: 0x0000D724
		// (set) Token: 0x06000511 RID: 1297 RVA: 0x0000F52C File Offset: 0x0000D72C
		public virtual int StopIndex
		{
			get
			{
				return this.stop;
			}
			set
			{
				this.stop = value;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x0000F538 File Offset: 0x0000D738
		// (set) Token: 0x06000513 RID: 1299 RVA: 0x0000F540 File Offset: 0x0000D740
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

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000514 RID: 1300 RVA: 0x0000F54C File Offset: 0x0000D74C
		// (set) Token: 0x06000515 RID: 1301 RVA: 0x0000F554 File Offset: 0x0000D754
		public virtual ICharStream InputStream
		{
			get
			{
				return this.input;
			}
			set
			{
				this.input = value;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000516 RID: 1302 RVA: 0x0000F560 File Offset: 0x0000D760
		// (set) Token: 0x06000517 RID: 1303 RVA: 0x0000F5B0 File Offset: 0x0000D7B0
		public virtual string Text
		{
			get
			{
				if (this.text != null)
				{
					return this.text;
				}
				if (this.input == null)
				{
					return null;
				}
				this.text = this.input.Substring(this.start, this.stop);
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0000F5BC File Offset: 0x0000D7BC
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
				",",
				this.start,
				":",
				this.stop,
				"='",
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

		// Token: 0x04000154 RID: 340
		protected internal int type;

		// Token: 0x04000155 RID: 341
		protected internal int line;

		// Token: 0x04000156 RID: 342
		protected internal int charPositionInLine = -1;

		// Token: 0x04000157 RID: 343
		protected internal int channel;

		// Token: 0x04000158 RID: 344
		[NonSerialized]
		protected internal ICharStream input;

		// Token: 0x04000159 RID: 345
		protected internal string text;

		// Token: 0x0400015A RID: 346
		protected internal int index = -1;

		// Token: 0x0400015B RID: 347
		protected internal int start;

		// Token: 0x0400015C RID: 348
		protected internal int stop;
	}
}
