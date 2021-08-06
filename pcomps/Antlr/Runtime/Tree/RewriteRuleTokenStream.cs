using System;
using System.Collections;
using System.Collections.Generic;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000D6 RID: 214
	public class RewriteRuleTokenStream : RewriteRuleElementStream<IToken>
	{
		// Token: 0x0600088F RID: 2191 RVA: 0x00018438 File Offset: 0x00016638
		public RewriteRuleTokenStream(ITreeAdaptor adaptor, string elementDescription) : base(adaptor, elementDescription)
		{
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x00018444 File Offset: 0x00016644
		public RewriteRuleTokenStream(ITreeAdaptor adaptor, string elementDescription, IToken oneElement) : base(adaptor, elementDescription, oneElement)
		{
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x00018450 File Offset: 0x00016650
		public RewriteRuleTokenStream(ITreeAdaptor adaptor, string elementDescription, IList<IToken> elements) : base(adaptor, elementDescription, elements)
		{
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x0001845C File Offset: 0x0001665C
		[Obsolete("This constructor is for internal use only and might be phased out soon. Use instead the one with IList<T>.")]
		public RewriteRuleTokenStream(ITreeAdaptor adaptor, string elementDescription, IList elements) : base(adaptor, elementDescription, elements)
		{
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x00018468 File Offset: 0x00016668
		public object NextNode()
		{
			return this.adaptor.Create((IToken)base._Next());
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x00018480 File Offset: 0x00016680
		public IToken NextToken()
		{
			return (IToken)base._Next();
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x00018490 File Offset: 0x00016690
		protected override object ToTree(IToken el)
		{
			return el;
		}
	}
}
