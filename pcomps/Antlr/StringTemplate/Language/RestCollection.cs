using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x0200024B RID: 587
	internal sealed class RestCollection : ICollection, IEnumerable
	{
		// Token: 0x060011BD RID: 4541 RVA: 0x00082334 File Offset: 0x00080534
		internal RestCollection(ICollection collection)
		{
			if (collection == null || collection.Count < 2)
			{
				_inner = new object[0];
				return;
			}
			_inner = collection;
			_count = _inner.Count - 1;
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00082370 File Offset: 0x00080570
		internal RestCollection(IEnumerator enumerator)
		{
			if (enumerator == null)
			{
				_inner = new object[0];
				return;
			}
			ArrayList arrayList = new ArrayList();
			while (enumerator.MoveNext())
			{
				object value = enumerator.Current;
				arrayList.Add(value);
			}
			_inner = arrayList;
			_count = _inner.Count - 1;
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x000823CC File Offset: 0x000805CC
		public override bool Equals(object o)
		{
			if (o is RestCollection)
			{
				RestCollection restCollection = (RestCollection)o;
				if (Count == 0 && restCollection.Count == 0)
				{
					return true;
				}
				if (Count == restCollection.Count)
				{
					IEnumerator enumerator = GetEnumerator();
					IEnumerator enumerator2 = restCollection.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (!enumerator2.MoveNext() || !enumerator.Current.Equals(enumerator2.Current))
						{
							return false;
						}
					}
					return !enumerator2.MoveNext();
				}
			}
			return false;
		}

		// Token: 0x060011C0 RID: 4544 RVA: 0x00082448 File Offset: 0x00080648
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060011C1 RID: 4545 RVA: 0x00082450 File Offset: 0x00080650
		public bool IsSynchronized
		{
			get
			{
				return _inner.IsSynchronized;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x00082460 File Offset: 0x00080660
		public int Count
		{
			get
			{
				return _count;
			}
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x00082468 File Offset: 0x00080668
		public void CopyTo(Array array, int index)
		{
			_inner.CopyTo(array, index);
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x00082478 File Offset: 0x00080678
		public object SyncRoot
		{
			get
			{
				return _inner.SyncRoot;
			}
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x00082488 File Offset: 0x00080688
		public IEnumerator GetEnumerator()
		{
			return new RestCollectionIterator(this);
		}

		// Token: 0x04000EFC RID: 3836
		private ICollection _inner;

		// Token: 0x04000EFD RID: 3837
		private int _count;

		// Token: 0x0200024C RID: 588
		internal sealed class RestCollectionIterator : IEnumerator
		{
			// Token: 0x060011C6 RID: 4550 RVA: 0x00082490 File Offset: 0x00080690
			public RestCollectionIterator(RestCollection collection)
			{
				this.collection = collection._inner;
				Reset();
			}

			// Token: 0x060011C7 RID: 4551 RVA: 0x000824AC File Offset: 0x000806AC
			public void Reset()
			{
				if (inner == null)
				{
					inner = collection.GetEnumerator();
				}
				inner.Reset();
				inner.MoveNext();
				beforeMoveNext = true;
			}

			// Token: 0x1700026F RID: 623
			// (get) Token: 0x060011C8 RID: 4552 RVA: 0x000824E8 File Offset: 0x000806E8
			public object Current
			{
				get
				{
					if (beforeMoveNext)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return inner.Current;
				}
			}

			// Token: 0x060011C9 RID: 4553 RVA: 0x00082508 File Offset: 0x00080708
			public bool MoveNext()
			{
				beforeMoveNext = false;
				return inner.MoveNext();
			}

			// Token: 0x060011CA RID: 4554 RVA: 0x0008251C File Offset: 0x0008071C
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (MoveNext())
				{
					object value = Current;
					stringBuilder.Append(value);
				}
				return stringBuilder.ToString();
			}

			// Token: 0x04000EFE RID: 3838
			private ICollection collection;

			// Token: 0x04000EFF RID: 3839
			private IEnumerator inner;

			// Token: 0x04000F00 RID: 3840
			private bool beforeMoveNext;
		}
	}
}
