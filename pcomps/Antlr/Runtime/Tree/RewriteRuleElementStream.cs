using System;
using System.Collections;
using System.Collections.Generic;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000D3 RID: 211
	public abstract class RewriteRuleElementStream<T>
	{
		// Token: 0x06000873 RID: 2163 RVA: 0x00018090 File Offset: 0x00016290
		public RewriteRuleElementStream(ITreeAdaptor adaptor, string elementDescription)
		{
			this.elementDescription = elementDescription;
			this.adaptor = adaptor;
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x000180A8 File Offset: 0x000162A8
		public RewriteRuleElementStream(ITreeAdaptor adaptor, string elementDescription, T oneElement) : this(adaptor, elementDescription)
		{
			Add(oneElement);
		}

		// Token: 0x06000875 RID: 2165 RVA: 0x000180BC File Offset: 0x000162BC
		public RewriteRuleElementStream(ITreeAdaptor adaptor, string elementDescription, IList<T> elements) : this(adaptor, elementDescription)
		{
			singleElement = default(T);
			this.elements = elements;
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x000180E8 File Offset: 0x000162E8
		[Obsolete("This constructor is for internal use only and might be phased out soon. Use instead the one with IList<T>.")]
		public RewriteRuleElementStream(ITreeAdaptor adaptor, string elementDescription, IList elements) : this(adaptor, elementDescription)
		{
			singleElement = default(T);
			this.elements = new List<T>();
			if (elements != null)
			{
				foreach (object obj in elements)
				{
					T item = (T)((object)obj);
					this.elements.Add(item);
				}
			}
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x00018174 File Offset: 0x00016374
		public void Add(T el)
		{
			if (el == null)
			{
				return;
			}
			if (elements != null)
			{
				elements.Add(el);
				return;
			}
			if (singleElement == null)
			{
				singleElement = el;
				return;
			}
			elements = new List<T>(5);
			elements.Add(singleElement);
			singleElement = default(T);
			elements.Add(el);
		}

		// Token: 0x06000878 RID: 2168 RVA: 0x000181F8 File Offset: 0x000163F8
		public virtual void Reset()
		{
			cursor = 0;
			dirty = true;
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x00018208 File Offset: 0x00016408
		public bool HasNext()
		{
			return (singleElement != null && cursor < 1) || (elements != null && cursor < elements.Count);
		}

		// Token: 0x0600087A RID: 2170 RVA: 0x00018258 File Offset: 0x00016458
		public virtual object NextTree()
		{
			return _Next();
		}

		// Token: 0x0600087B RID: 2171 RVA: 0x00018260 File Offset: 0x00016460
		protected object _Next()
		{
			int count = Count;
			if (count == 0)
			{
				throw new RewriteEmptyStreamException(elementDescription);
			}
			if (cursor >= count)
			{
				if (count == 1)
				{
					return ToTree(singleElement);
				}
				throw new RewriteCardinalityException(elementDescription);
			}
			else
			{
				if (singleElement != null)
				{
					cursor++;
					return ToTree(singleElement);
				}
				return ToTree(elements[cursor++]);
			}
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x00018300 File Offset: 0x00016500
		protected virtual object ToTree(T el)
		{
			return el;
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600087D RID: 2173 RVA: 0x00018308 File Offset: 0x00016508
		public int Count
		{
			get
			{
				if (singleElement != null)
				{
					return 1;
				}
				if (elements != null)
				{
					return elements.Count;
				}
				return 0;
			}
		}

		// Token: 0x0600087E RID: 2174 RVA: 0x00018340 File Offset: 0x00016540
		[Obsolete("Please use property Count instead.")]
		public int Size()
		{
			return Count;
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x0600087F RID: 2175 RVA: 0x00018348 File Offset: 0x00016548
		public string Description
		{
			get
			{
				return elementDescription;
			}
		}

		// Token: 0x0400023B RID: 571
		protected int cursor;

		// Token: 0x0400023C RID: 572
		protected T singleElement;

		// Token: 0x0400023D RID: 573
		protected IList<T> elements;

		// Token: 0x0400023E RID: 574
		protected bool dirty;

		// Token: 0x0400023F RID: 575
		protected string elementDescription;

		// Token: 0x04000240 RID: 576
		protected ITreeAdaptor adaptor;
	}
}
