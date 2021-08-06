using System.Collections;

namespace pcomps.Antlr.StringTemplate.Collections
{
	// Token: 0x02000230 RID: 560
	public class StackList : ArrayList
	{
		// Token: 0x0600106A RID: 4202 RVA: 0x0007242C File Offset: 0x0007062C
		public void Push(object item)
		{
			this.Add(item);
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x00072438 File Offset: 0x00070638
		public object Pop()
		{
			object result = this[this.Count - 1];
			this.RemoveAt(this.Count - 1);
			return result;
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x00072464 File Offset: 0x00070664
		public object Peek()
		{
			return this[this.Count - 1];
		}
	}
}
