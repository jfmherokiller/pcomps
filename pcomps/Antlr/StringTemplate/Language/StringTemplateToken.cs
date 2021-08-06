using System.Collections;
using pcomps.Antlr.StringTemplate.Collections;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x0200024F RID: 591
	public class StringTemplateToken : CommonToken
	{
		// Token: 0x060011D3 RID: 4563 RVA: 0x000825B0 File Offset: 0x000807B0
		public StringTemplateToken()
		{
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x000825B8 File Offset: 0x000807B8
		public StringTemplateToken(int type, string text) : base(type, text)
		{
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x000825C4 File Offset: 0x000807C4
		public StringTemplateToken(string text) : base(text)
		{
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x000825D0 File Offset: 0x000807D0
		public StringTemplateToken(int type, string text, IList args) : base(type, text)
		{
			this.args = args;
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x000825E4 File Offset: 0x000807E4
		public override string ToString()
		{
			return $"{base.ToString()}; args={CollectionUtils.ListToString(args)}";
		}

		// Token: 0x04000F03 RID: 3843
		public new static readonly StringTemplateTokenCreator Creator = new StringTemplateTokenCreator();

		// Token: 0x04000F04 RID: 3844
		public IList args;

		// Token: 0x02000250 RID: 592
		public class StringTemplateTokenCreator : TokenCreator
		{
			// Token: 0x17000272 RID: 626
			// (get) Token: 0x060011DA RID: 4570 RVA: 0x00082618 File Offset: 0x00080818
			public override string TokenTypeName
			{
				get
				{
					return typeof(StringTemplateToken).FullName;
				}
			}

			// Token: 0x060011DB RID: 4571 RVA: 0x0008262C File Offset: 0x0008082C
			public override IToken Create()
			{
				return new StringTemplateToken();
			}
		}
	}
}
