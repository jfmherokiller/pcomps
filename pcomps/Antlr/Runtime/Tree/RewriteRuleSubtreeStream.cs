using System;
using System.Collections;
using System.Collections.Generic;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000D4 RID: 212
	public class RewriteRuleSubtreeStream : RewriteRuleElementStream<object>
	{
		// Token: 0x06000880 RID: 2176 RVA: 0x00018350 File Offset: 0x00016550
		public RewriteRuleSubtreeStream(ITreeAdaptor adaptor, string elementDescription) : base(adaptor, elementDescription)
		{
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x0001835C File Offset: 0x0001655C
		public RewriteRuleSubtreeStream(ITreeAdaptor adaptor, string elementDescription, object oneElement) : base(adaptor, elementDescription, oneElement)
		{
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x00018368 File Offset: 0x00016568
		public RewriteRuleSubtreeStream(ITreeAdaptor adaptor, string elementDescription, IList<object> elements) : base(adaptor, elementDescription, elements)
		{
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x00018374 File Offset: 0x00016574
		[Obsolete("This constructor is for internal use only and might be phased out soon. Use instead the one with IList<T>.")]
		public RewriteRuleSubtreeStream(ITreeAdaptor adaptor, string elementDescription, IList elements) : base(adaptor, elementDescription, elements)
		{
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x00018380 File Offset: 0x00016580
		public object NextNode()
		{
			return FetchObject((object o) => adaptor.DupNode(o));
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x00018394 File Offset: 0x00016594
		private object FetchObject(ProcessHandler ph)
		{
			if (RequiresDuplication())
			{
				return ph(_Next());
			}
			return _Next();
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x000183C0 File Offset: 0x000165C0
		private bool RequiresDuplication()
		{
			var count = Count;
			return dirty || (cursor >= count && count == 1);
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x000183F8 File Offset: 0x000165F8
		public override object NextTree()
		{
			return FetchObject((object o) => Dup(o));
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x0001840C File Offset: 0x0001660C
		private object Dup(object el)
		{
			return adaptor.DupTree(el);
		}

		// Token: 0x020000D5 RID: 213
		// (Invoke) Token: 0x0600088C RID: 2188
		private delegate object ProcessHandler(object o);
	}
}
