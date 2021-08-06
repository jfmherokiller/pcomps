using System.Collections;

namespace pcomps.Antlr.Runtime.Collections
{
	// Token: 0x020000A1 RID: 161
	public class StackList : ArrayList
	{
		// Token: 0x060005C1 RID: 1473 RVA: 0x000110B8 File Offset: 0x0000F2B8
		public void Push(object item)
		{
			Add(item);
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x000110C4 File Offset: 0x0000F2C4
		public object Pop()
		{
			var result = this[Count - 1];
			RemoveAt(Count - 1);
			return result;
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x000110F0 File Offset: 0x0000F2F0
		public object Peek()
		{
			return this[Count - 1];
		}
	}
}
