using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.Runtime.Collections
{
	// Token: 0x0200009C RID: 156
	public sealed class HashList : ICollection, IDictionary, IEnumerable
	{
		// Token: 0x06000592 RID: 1426 RVA: 0x000108DC File Offset: 0x0000EADC
		public HashList() : this(-1)
		{
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x000108E8 File Offset: 0x0000EAE8
		public HashList(int capacity)
		{
			if (capacity < 0)
			{
				_dictionary = new Hashtable();
				_insertionOrderList = new ArrayList();
			}
			else
			{
				_dictionary = new Hashtable(capacity);
				_insertionOrderList = new ArrayList(capacity);
			}
			_version = 0;
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x00010954 File Offset: 0x0000EB54
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new HashListEnumerator(this, HashListEnumerator.EnumerationMode.Entry);
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000595 RID: 1429 RVA: 0x00010960 File Offset: 0x0000EB60
		public bool IsReadOnly
		{
			get
			{
				return _dictionary.IsReadOnly;
			}
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00010970 File Offset: 0x0000EB70
		public IDictionaryEnumerator GetEnumerator()
		{
			return new HashListEnumerator(this, HashListEnumerator.EnumerationMode.Entry);
		}

		// Token: 0x1700006D RID: 109
		public object this[object key]
		{
			get
			{
				return _dictionary[key];
			}
			set
			{
				bool flag = !_dictionary.Contains(key);
				_dictionary[key] = value;
				if (flag)
				{
					_insertionOrderList.Add(key);
				}
				_version++;
			}
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x000109D8 File Offset: 0x0000EBD8
		public void Remove(object key)
		{
			_dictionary.Remove(key);
			_insertionOrderList.Remove(key);
			_version++;
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x00010A0C File Offset: 0x0000EC0C
		public bool Contains(object key)
		{
			return _dictionary.Contains(key);
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x00010A1C File Offset: 0x0000EC1C
		public void Clear()
		{
			_dictionary.Clear();
			_insertionOrderList.Clear();
			_version++;
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x00010A50 File Offset: 0x0000EC50
		public ICollection Values
		{
			get
			{
				return new ValueCollection(this);
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x00010A58 File Offset: 0x0000EC58
		public void Add(object key, object value)
		{
			_dictionary.Add(key, value);
			_insertionOrderList.Add(key);
			_version++;
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x00010A90 File Offset: 0x0000EC90
		public ICollection Keys
		{
			get
			{
				return new KeyCollection(this);
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x00010A98 File Offset: 0x0000EC98
		public bool IsFixedSize
		{
			get
			{
				return _dictionary.IsFixedSize;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060005A0 RID: 1440 RVA: 0x00010AA8 File Offset: 0x0000ECA8
		public bool IsSynchronized
		{
			get
			{
				return _dictionary.IsSynchronized;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x00010AB8 File Offset: 0x0000ECB8
		public int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x00010AC8 File Offset: 0x0000ECC8
		public void CopyTo(Array array, int index)
		{
			int count = _insertionOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				DictionaryEntry dictionaryEntry = new DictionaryEntry(_insertionOrderList[i], _dictionary[_insertionOrderList[i]]);
				array.SetValue(dictionaryEntry, index++);
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00010B30 File Offset: 0x0000ED30
		public object SyncRoot
		{
			get
			{
				return _dictionary.SyncRoot;
			}
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x00010B40 File Offset: 0x0000ED40
		private void CopyKeysTo(Array array, int index)
		{
			int count = _insertionOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				array.SetValue(_insertionOrderList[i], index++);
			}
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x00010B84 File Offset: 0x0000ED84
		private void CopyValuesTo(Array array, int index)
		{
			int count = _insertionOrderList.Count;
			for (int i = 0; i < count; i++)
			{
				array.SetValue(_dictionary[_insertionOrderList[i]], index++);
			}
		}

		// Token: 0x0400018E RID: 398
		private Hashtable _dictionary = new Hashtable();

		// Token: 0x0400018F RID: 399
		private ArrayList _insertionOrderList = new ArrayList();

		// Token: 0x04000190 RID: 400
		private int _version;

		// Token: 0x0200009D RID: 157
		private sealed class HashListEnumerator : IDictionaryEnumerator, IEnumerator
		{
			// Token: 0x060005A6 RID: 1446 RVA: 0x00010BD4 File Offset: 0x0000EDD4
			internal HashListEnumerator()
			{
				_index = 0;
				_key = null;
				_value = null;
			}

			// Token: 0x060005A7 RID: 1447 RVA: 0x00010BF4 File Offset: 0x0000EDF4
			internal HashListEnumerator(HashList hashList, EnumerationMode mode)
			{
				_hashList = hashList;
				_mode = mode;
				_version = hashList._version;
				_orderList = hashList._insertionOrderList;
				_index = 0;
				_key = null;
				_value = null;
			}

			// Token: 0x17000074 RID: 116
			// (get) Token: 0x060005A8 RID: 1448 RVA: 0x00010C44 File Offset: 0x0000EE44
			public object Key
			{
				get
				{
					if (_key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return _key;
				}
			}

			// Token: 0x17000075 RID: 117
			// (get) Token: 0x060005A9 RID: 1449 RVA: 0x00010C64 File Offset: 0x0000EE64
			public object Value
			{
				get
				{
					if (_key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return _value;
				}
			}

			// Token: 0x17000076 RID: 118
			// (get) Token: 0x060005AA RID: 1450 RVA: 0x00010C84 File Offset: 0x0000EE84
			public DictionaryEntry Entry
			{
				get
				{
					if (_key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					DictionaryEntry result = new DictionaryEntry(_key, _value);
					return result;
				}
			}

			// Token: 0x060005AB RID: 1451 RVA: 0x00010CBC File Offset: 0x0000EEBC
			public void Reset()
			{
				if (_version != _hashList._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				_index = 0;
				_key = null;
				_value = null;
			}

			// Token: 0x17000077 RID: 119
			// (get) Token: 0x060005AC RID: 1452 RVA: 0x00010D00 File Offset: 0x0000EF00
			public object Current
			{
				get
				{
					if (_key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					if (_mode == EnumerationMode.Key)
					{
						return _key;
					}
					if (_mode == EnumerationMode.Value)
					{
						return _value;
					}
					DictionaryEntry dictionaryEntry = new DictionaryEntry(_key, _value);
					return dictionaryEntry;
				}
			}

			// Token: 0x060005AD RID: 1453 RVA: 0x00010D64 File Offset: 0x0000EF64
			public bool MoveNext()
			{
				if (_version != _hashList._version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				if (_index < _orderList.Count)
				{
					_key = _orderList[_index];
					_value = _hashList[_key];
					_index++;
					return true;
				}
				_key = null;
				return false;
			}

			// Token: 0x04000191 RID: 401
			private HashList _hashList;

			// Token: 0x04000192 RID: 402
			private ArrayList _orderList;

			// Token: 0x04000193 RID: 403
			private EnumerationMode _mode;

			// Token: 0x04000194 RID: 404
			private int _index;

			// Token: 0x04000195 RID: 405
			private int _version;

			// Token: 0x04000196 RID: 406
			private object _key;

			// Token: 0x04000197 RID: 407
			private object _value;

			// Token: 0x0200009E RID: 158
			internal enum EnumerationMode
			{
				// Token: 0x04000199 RID: 409
				Key,
				// Token: 0x0400019A RID: 410
				Value,
				// Token: 0x0400019B RID: 411
				Entry
			}
		}

		// Token: 0x0200009F RID: 159
		private sealed class KeyCollection : ICollection, IEnumerable
		{
			// Token: 0x060005AE RID: 1454 RVA: 0x00010DF0 File Offset: 0x0000EFF0
			internal KeyCollection()
			{
			}

			// Token: 0x060005AF RID: 1455 RVA: 0x00010DF8 File Offset: 0x0000EFF8
			internal KeyCollection(HashList hashList)
			{
				_hashList = hashList;
			}

			// Token: 0x060005B0 RID: 1456 RVA: 0x00010E08 File Offset: 0x0000F008
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				ArrayList insertionOrderList = _hashList._insertionOrderList;
				for (int i = 0; i < insertionOrderList.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(insertionOrderList[i]);
				}
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}

			// Token: 0x060005B1 RID: 1457 RVA: 0x00010E80 File Offset: 0x0000F080
			public override bool Equals(object o)
			{
				if (o is KeyCollection)
				{
					KeyCollection keyCollection = (KeyCollection)o;
					if (Count == 0 && keyCollection.Count == 0)
					{
						return true;
					}
					if (Count == keyCollection.Count)
					{
						for (int i = 0; i < Count; i++)
						{
							if (_hashList._insertionOrderList[i] == keyCollection._hashList._insertionOrderList[i] || _hashList._insertionOrderList[i].Equals(keyCollection._hashList._insertionOrderList[i]))
							{
								return true;
							}
						}
					}
				}
				return false;
			}

			// Token: 0x060005B2 RID: 1458 RVA: 0x00010F38 File Offset: 0x0000F138
			public override int GetHashCode()
			{
				return _hashList._insertionOrderList.GetHashCode();
			}

			// Token: 0x17000078 RID: 120
			// (get) Token: 0x060005B3 RID: 1459 RVA: 0x00010F4C File Offset: 0x0000F14C
			public bool IsSynchronized
			{
				get
				{
					return _hashList.IsSynchronized;
				}
			}

			// Token: 0x17000079 RID: 121
			// (get) Token: 0x060005B4 RID: 1460 RVA: 0x00010F5C File Offset: 0x0000F15C
			public int Count
			{
				get
				{
					return _hashList.Count;
				}
			}

			// Token: 0x060005B5 RID: 1461 RVA: 0x00010F6C File Offset: 0x0000F16C
			public void CopyTo(Array array, int index)
			{
				_hashList.CopyKeysTo(array, index);
			}

			// Token: 0x1700007A RID: 122
			// (get) Token: 0x060005B6 RID: 1462 RVA: 0x00010F7C File Offset: 0x0000F17C
			public object SyncRoot
			{
				get
				{
					return _hashList.SyncRoot;
				}
			}

			// Token: 0x060005B7 RID: 1463 RVA: 0x00010F8C File Offset: 0x0000F18C
			public IEnumerator GetEnumerator()
			{
				return new HashListEnumerator(_hashList, HashListEnumerator.EnumerationMode.Key);
			}

			// Token: 0x0400019C RID: 412
			private HashList _hashList;
		}

		// Token: 0x020000A0 RID: 160
		private sealed class ValueCollection : ICollection, IEnumerable
		{
			// Token: 0x060005B8 RID: 1464 RVA: 0x00010F9C File Offset: 0x0000F19C
			internal ValueCollection()
			{
			}

			// Token: 0x060005B9 RID: 1465 RVA: 0x00010FA4 File Offset: 0x0000F1A4
			internal ValueCollection(HashList hashList)
			{
				_hashList = hashList;
			}

			// Token: 0x060005BA RID: 1466 RVA: 0x00010FB4 File Offset: 0x0000F1B4
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				IEnumerator enumerator = new HashListEnumerator(_hashList, HashListEnumerator.EnumerationMode.Value);
				if (enumerator.MoveNext())
				{
					stringBuilder.Append((enumerator.Current != null) ? enumerator.Current : "null");
					while (enumerator.MoveNext())
					{
						stringBuilder.Append(", ");
						stringBuilder.Append((enumerator.Current != null) ? enumerator.Current : "null");
					}
				}
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}

			// Token: 0x1700007B RID: 123
			// (get) Token: 0x060005BB RID: 1467 RVA: 0x00011060 File Offset: 0x0000F260
			public bool IsSynchronized
			{
				get
				{
					return _hashList.IsSynchronized;
				}
			}

			// Token: 0x1700007C RID: 124
			// (get) Token: 0x060005BC RID: 1468 RVA: 0x00011070 File Offset: 0x0000F270
			public int Count
			{
				get
				{
					return _hashList.Count;
				}
			}

			// Token: 0x060005BD RID: 1469 RVA: 0x00011080 File Offset: 0x0000F280
			public void CopyTo(Array array, int index)
			{
				_hashList.CopyValuesTo(array, index);
			}

			// Token: 0x1700007D RID: 125
			// (get) Token: 0x060005BE RID: 1470 RVA: 0x00011090 File Offset: 0x0000F290
			public object SyncRoot
			{
				get
				{
					return _hashList.SyncRoot;
				}
			}

			// Token: 0x060005BF RID: 1471 RVA: 0x000110A0 File Offset: 0x0000F2A0
			public IEnumerator GetEnumerator()
			{
				return new HashListEnumerator(_hashList, HashListEnumerator.EnumerationMode.Value);
			}

			// Token: 0x0400019D RID: 413
			private HashList _hashList;
		}
	}
}
