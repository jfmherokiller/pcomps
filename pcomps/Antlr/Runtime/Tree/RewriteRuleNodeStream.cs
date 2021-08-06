using System;
using System.Collections;
using System.Collections.Generic;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000CE RID: 206
	public class RewriteRuleNodeStream : RewriteRuleElementStream<object>
	{
		// Token: 0x06000864 RID: 2148 RVA: 0x00017CE0 File Offset: 0x00015EE0
		public RewriteRuleNodeStream(ITreeAdaptor adaptor, string elementDescription) : base(adaptor, elementDescription)
		{
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x00017CEC File Offset: 0x00015EEC
		public RewriteRuleNodeStream(ITreeAdaptor adaptor, string elementDescription, object oneElement) : base(adaptor, elementDescription, oneElement)
		{
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x00017CF8 File Offset: 0x00015EF8
		public RewriteRuleNodeStream(ITreeAdaptor adaptor, string elementDescription, IList<object> elements) : base(adaptor, elementDescription, elements)
		{
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x00017D04 File Offset: 0x00015F04
		[Obsolete("This constructor is for internal use only and might be phased out soon. Use instead the one with IList<T>.")]
		public RewriteRuleNodeStream(ITreeAdaptor adaptor, string elementDescription, IList elements) : base(adaptor, elementDescription, elements)
		{
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x00017D10 File Offset: 0x00015F10
		public object NextNode()
		{
			return base._Next();
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x00017D18 File Offset: 0x00015F18
		protected override object ToTree(object el)
		{
			return this.adaptor.DupNode(el);
		}
	}
}
