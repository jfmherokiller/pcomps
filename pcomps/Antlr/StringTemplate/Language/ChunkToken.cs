namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x0200023D RID: 573
	public class ChunkToken : CommonToken
	{
		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06001122 RID: 4386 RVA: 0x0007BFF8 File Offset: 0x0007A1F8
		// (set) Token: 0x06001123 RID: 4387 RVA: 0x0007C000 File Offset: 0x0007A200
		public virtual string Indentation
		{
			get => _indentation;
            set => _indentation = value;
        }

		// Token: 0x06001124 RID: 4388 RVA: 0x0007C00C File Offset: 0x0007A20C
		public ChunkToken()
		{
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x0007C014 File Offset: 0x0007A214
		public ChunkToken(int type, string text, string indentation) : base(type, text)
		{
			_indentation = indentation;
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x0007C028 File Offset: 0x0007A228
		public override string ToString()
		{
			return $"{base.ToString()}<indent='{_indentation}'>";
		}

		// Token: 0x04000E49 RID: 3657
		public new static readonly ChunkTokenCreator Creator = new ChunkTokenCreator();

		// Token: 0x04000E4A RID: 3658
		private string _indentation;

		// Token: 0x0200023E RID: 574
		public class ChunkTokenCreator : TokenCreator
		{
			// Token: 0x17000268 RID: 616
			// (get) Token: 0x06001129 RID: 4393 RVA: 0x0007C05C File Offset: 0x0007A25C
			public override string TokenTypeName => typeof(ChunkToken).FullName;

            // Token: 0x0600112A RID: 4394 RVA: 0x0007C070 File Offset: 0x0007A270
			public override IToken Create()
			{
				return new ChunkToken();
			}
		}
	}
}
