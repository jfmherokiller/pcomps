using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x0200023C RID: 572
	public sealed class CatIterator : IEnumerator
	{
		// Token: 0x0600111D RID: 4381 RVA: 0x0007BE60 File Offset: 0x0007A060
		public CatIterator(IList iterators)
		{
			this.iterators = iterators;
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x0600111E RID: 4382 RVA: 0x0007BE70 File Offset: 0x0007A070
		public object Current
		{
			get
			{
				if (!hasCurrent)
				{
					throw new InvalidOperationException("Enumeration not started or already finished.");
				}
				return currentObject;
			}
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x0007BE8C File Offset: 0x0007A08C
		public bool MoveNext()
		{
			if (currentIteratorIndex < iterators.Count)
			{
				var enumerator = (IEnumerator)iterators[currentIteratorIndex];
				if (enumerator.MoveNext())
				{
					currentObject = enumerator.Current;
					return hasCurrent = true;
				}
				currentIteratorIndex++;
				while (currentIteratorIndex < iterators.Count)
				{
					enumerator = (IEnumerator)iterators[currentIteratorIndex];
					if (enumerator.MoveNext())
					{
						currentObject = enumerator.Current;
						return hasCurrent = true;
					}
					currentIteratorIndex++;
				}
			}
			return hasCurrent = false;
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x0007BF58 File Offset: 0x0007A158
		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			while (MoveNext())
			{
				var value = Current;
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x0007BF88 File Offset: 0x0007A188
		public void Reset()
		{
			currentIteratorIndex = 0;
			currentObject = null;
			hasCurrent = false;
			foreach (var obj in iterators)
			{
				var enumerator2 = (IEnumerator)obj;
				enumerator2.Reset();
			}
		}

		// Token: 0x04000E45 RID: 3653
		private IList iterators;

		// Token: 0x04000E46 RID: 3654
		private object currentObject;

		// Token: 0x04000E47 RID: 3655
		private int currentIteratorIndex;

		// Token: 0x04000E48 RID: 3656
		private bool hasCurrent;
	}
}
