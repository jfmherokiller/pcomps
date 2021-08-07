using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.StringTemplate.Collections
{
	// Token: 0x0200022B RID: 555
	public sealed class HashList : IDictionary, ICollection, IEnumerable
	{
		// Token: 0x0600102F RID: 4143 RVA: 0x00071C70 File Offset: 0x0006FE70
		public HashList() : this(-1)
		{
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x00071C7C File Offset: 0x0006FE7C
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

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06001031 RID: 4145 RVA: 0x00071CE0 File Offset: 0x0006FEE0
		public bool IsReadOnly
		{
			get
			{
				return _dictionary.IsReadOnly;
			}
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00071CF0 File Offset: 0x0006FEF0
		public IDictionaryEnumerator GetEnumerator()
		{
			return new HashListEnumerator(this, HashListEnumerator.EnumerationMode.Entry);
		}

		// Token: 0x1700024E RID: 590
		public object this[object key]
		{
			get
			{
				return _dictionary[key];
			}
			set
			{
				var flag = !_dictionary.Contains(key);
				_dictionary[key] = value;
				if (flag)
				{
					_insertionOrderList.Add(key);
				}
				_version++;
			}
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x00071D54 File Offset: 0x0006FF54
		public void Remove(object key)
		{
			_dictionary.Remove(key);
			_insertionOrderList.Remove(key);
			_version++;
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00071D7C File Offset: 0x0006FF7C
		public bool Contains(object key)
		{
			return _dictionary.Contains(key);
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x00071D8C File Offset: 0x0006FF8C
		public void Clear()
		{
			_dictionary.Clear();
			_insertionOrderList.Clear();
			_version++;
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06001038 RID: 4152 RVA: 0x00071DB4 File Offset: 0x0006FFB4
		public ICollection Values
		{
			get
			{
				return new ValueCollection(this);
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x00071DBC File Offset: 0x0006FFBC
		public void Add(object key, object value)
		{
			_dictionary.Add(key, value);
			_insertionOrderList.Add(key);
			_version++;
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x0600103A RID: 4154 RVA: 0x00071DE8 File Offset: 0x0006FFE8
		public ICollection Keys
		{
			get
			{
				return new KeyCollection(this);
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x0600103B RID: 4155 RVA: 0x00071DF0 File Offset: 0x0006FFF0
		public IList KeysList
		{
			get
			{
				return new KeyCollection(this);
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x0600103C RID: 4156 RVA: 0x00071DF8 File Offset: 0x0006FFF8
		public bool IsFixedSize
		{
			get
			{
				return _dictionary.IsFixedSize;
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600103D RID: 4157 RVA: 0x00071E08 File Offset: 0x00070008
		public bool IsSynchronized
		{
			get
			{
				return _dictionary.IsSynchronized;
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x0600103E RID: 4158 RVA: 0x00071E18 File Offset: 0x00070018
		public int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x00071E28 File Offset: 0x00070028
		public void CopyTo(Array array, int index)
		{
			var count = _insertionOrderList.Count;
			for (var i = 0; i < count; i++)
			{
				var dictionaryEntry = new DictionaryEntry(_insertionOrderList[i], _dictionary[_insertionOrderList[i]]);
				array.SetValue(dictionaryEntry, index++);
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06001040 RID: 4160 RVA: 0x00071E8C File Offset: 0x0007008C
		public object SyncRoot
		{
			get
			{
				return _dictionary.SyncRoot;
			}
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x00071E9C File Offset: 0x0007009C
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new HashListEnumerator(this, HashListEnumerator.EnumerationMode.Entry);
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x00071EA8 File Offset: 0x000700A8
		private void CopyKeysTo(Array array, int index)
		{
			var count = _insertionOrderList.Count;
			for (var i = 0; i < count; i++)
			{
				array.SetValue(_insertionOrderList[i], index++);
			}
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x00071EE8 File Offset: 0x000700E8
		private void CopyValuesTo(Array array, int index)
		{
			var count = _insertionOrderList.Count;
			for (var i = 0; i < count; i++)
			{
				array.SetValue(_dictionary[_insertionOrderList[i]], index++);
			}
		}

		// Token: 0x04000D08 RID: 3336
		private Hashtable _dictionary = new Hashtable();

		// Token: 0x04000D09 RID: 3337
		private ArrayList _insertionOrderList = new ArrayList();

		// Token: 0x04000D0A RID: 3338
		private int _version;

		// Token: 0x0200022C RID: 556
		private sealed class HashListEnumerator : IDictionaryEnumerator, IEnumerator
		{
			// Token: 0x06001044 RID: 4164 RVA: 0x00071F30 File Offset: 0x00070130
			internal HashListEnumerator()
			{
				_index = 0;
				_key = null;
				_value = null;
			}

			// Token: 0x06001045 RID: 4165 RVA: 0x00071F50 File Offset: 0x00070150
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

			// Token: 0x17000256 RID: 598
			// (get) Token: 0x06001046 RID: 4166 RVA: 0x00071FA0 File Offset: 0x000701A0
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

			// Token: 0x17000257 RID: 599
			// (get) Token: 0x06001047 RID: 4167 RVA: 0x00071FBC File Offset: 0x000701BC
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

			// Token: 0x17000258 RID: 600
			// (get) Token: 0x06001048 RID: 4168 RVA: 0x00071FD8 File Offset: 0x000701D8
			public DictionaryEntry Entry
			{
				get
				{
					if (_key == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return new DictionaryEntry(_key, _value);
				}
			}

			// Token: 0x06001049 RID: 4169 RVA: 0x00072000 File Offset: 0x00070200
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

			// Token: 0x17000259 RID: 601
			// (get) Token: 0x0600104A RID: 4170 RVA: 0x00072038 File Offset: 0x00070238
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
					return new DictionaryEntry(_key, _value);
				}
			}

			// Token: 0x0600104B RID: 4171 RVA: 0x00072090 File Offset: 0x00070290
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

			// Token: 0x04000D0B RID: 3339
			private HashList _hashList;

			// Token: 0x04000D0C RID: 3340
			private ArrayList _orderList;

			// Token: 0x04000D0D RID: 3341
			private EnumerationMode _mode;

			// Token: 0x04000D0E RID: 3342
			private int _index;

			// Token: 0x04000D0F RID: 3343
			private int _version;

			// Token: 0x04000D10 RID: 3344
			private object _key;

			// Token: 0x04000D11 RID: 3345
			private object _value;

			// Token: 0x0200022D RID: 557
			internal enum EnumerationMode
			{
				// Token: 0x04000D13 RID: 3347
				Key,
				// Token: 0x04000D14 RID: 3348
				Value,
				// Token: 0x04000D15 RID: 3349
				Entry
			}
		}

		// Token: 0x0200022E RID: 558
		private sealed class KeyCollection : IList, ICollection, IEnumerable
		{
			// Token: 0x0600104C RID: 4172 RVA: 0x00072114 File Offset: 0x00070314
			internal KeyCollection()
			{
			}

			// Token: 0x0600104D RID: 4173 RVA: 0x0007211C File Offset: 0x0007031C
			internal KeyCollection(HashList hashList)
			{
				_hashList = hashList;
			}

			// Token: 0x0600104E RID: 4174 RVA: 0x0007212C File Offset: 0x0007032C
			public override string ToString()
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.Append('[');
				var insertionOrderList = _hashList._insertionOrderList;
				for (var i = 0; i < insertionOrderList.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(insertionOrderList[i]);
				}
				stringBuilder.Append(']');
				return stringBuilder.ToString();
			}

			// Token: 0x0600104F RID: 4175 RVA: 0x00072198 File Offset: 0x00070398
			public override bool Equals(object o)
			{
				if (o is KeyCollection)
				{
					var keyCollection = (KeyCollection)o;
					if (Count == 0 && keyCollection.Count == 0)
					{
						return true;
					}
					if (Count == keyCollection.Count)
					{
						for (var i = 0; i < Count; i++)
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

			// Token: 0x06001050 RID: 4176 RVA: 0x00072238 File Offset: 0x00070438
			public override int GetHashCode()
			{
				return _hashList._insertionOrderList.GetHashCode();
			}

			// Token: 0x1700025A RID: 602
			// (get) Token: 0x06001051 RID: 4177 RVA: 0x0007224C File Offset: 0x0007044C
			public bool IsSynchronized
			{
				get
				{
					return _hashList.IsSynchronized;
				}
			}

			// Token: 0x1700025B RID: 603
			// (get) Token: 0x06001052 RID: 4178 RVA: 0x0007225C File Offset: 0x0007045C
			public int Count
			{
				get
				{
					return _hashList.Count;
				}
			}

			// Token: 0x06001053 RID: 4179 RVA: 0x0007226C File Offset: 0x0007046C
			public void CopyTo(Array array, int index)
			{
				_hashList.CopyKeysTo(array, index);
			}

			// Token: 0x1700025C RID: 604
			// (get) Token: 0x06001054 RID: 4180 RVA: 0x0007227C File Offset: 0x0007047C
			public object SyncRoot
			{
				get
				{
					return _hashList.SyncRoot;
				}
			}

			// Token: 0x06001055 RID: 4181 RVA: 0x0007228C File Offset: 0x0007048C
			public IEnumerator GetEnumerator()
			{
				return new HashListEnumerator(_hashList, HashListEnumerator.EnumerationMode.Key);
			}

			// Token: 0x1700025D RID: 605
			// (get) Token: 0x06001056 RID: 4182 RVA: 0x0007229C File Offset: 0x0007049C
			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			// Token: 0x1700025E RID: 606
			public object this[int index]
			{
				get
				{
					return _hashList._insertionOrderList[index];
				}
				set
				{
					throw new NotSupportedException("IList is ReadOnly");
				}
			}

			// Token: 0x06001059 RID: 4185 RVA: 0x000722C0 File Offset: 0x000704C0
			public void RemoveAt(int index)
			{
				throw new NotSupportedException("IList is ReadOnly");
			}

			// Token: 0x0600105A RID: 4186 RVA: 0x000722CC File Offset: 0x000704CC
			public void Insert(int index, object obj)
			{
				throw new NotSupportedException("IList is ReadOnly");
			}

			// Token: 0x0600105B RID: 4187 RVA: 0x000722D8 File Offset: 0x000704D8
			public void Remove(object obj)
			{
				throw new NotSupportedException("IList is ReadOnly");
			}

			// Token: 0x0600105C RID: 4188 RVA: 0x000722E4 File Offset: 0x000704E4
			public bool Contains(object obj)
			{
				return _hashList._insertionOrderList.Contains(obj);
			}

			// Token: 0x0600105D RID: 4189 RVA: 0x000722F8 File Offset: 0x000704F8
			public void Clear()
			{
				throw new NotSupportedException("IList is ReadOnly");
			}

			// Token: 0x0600105E RID: 4190 RVA: 0x00072304 File Offset: 0x00070504
			public int IndexOf(object obj)
			{
				return _hashList._insertionOrderList.IndexOf(obj);
			}

			// Token: 0x0600105F RID: 4191 RVA: 0x00072318 File Offset: 0x00070518
			public int Add(object obj)
			{
				throw new NotSupportedException("IList is ReadOnly");
			}

			// Token: 0x1700025F RID: 607
			// (get) Token: 0x06001060 RID: 4192 RVA: 0x00072324 File Offset: 0x00070524
			public bool IsFixedSize
			{
				get
				{
					return false;
				}
			}

			// Token: 0x04000D16 RID: 3350
			private HashList _hashList;
		}

		// Token: 0x0200022F RID: 559
		private sealed class ValueCollection : ICollection, IEnumerable
		{
			// Token: 0x06001061 RID: 4193 RVA: 0x00072328 File Offset: 0x00070528
			internal ValueCollection()
			{
			}

			// Token: 0x06001062 RID: 4194 RVA: 0x00072330 File Offset: 0x00070530
			internal ValueCollection(HashList hashList)
			{
				_hashList = hashList;
			}

			// Token: 0x06001063 RID: 4195 RVA: 0x00072340 File Offset: 0x00070540
			public override string ToString()
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.Append('[');
				IEnumerator enumerator = new HashListEnumerator(_hashList, HashListEnumerator.EnumerationMode.Value);
				if (enumerator.MoveNext())
				{
					stringBuilder.Append((enumerator.Current == null) ? "null" : enumerator.Current);
					while (enumerator.MoveNext())
					{
						stringBuilder.Append(", ");
						stringBuilder.Append((enumerator.Current == null) ? "null" : enumerator.Current);
					}
				}
				stringBuilder.Append(']');
				return stringBuilder.ToString();
			}

			// Token: 0x17000260 RID: 608
			// (get) Token: 0x06001064 RID: 4196 RVA: 0x000723D4 File Offset: 0x000705D4
			public bool IsSynchronized
			{
				get
				{
					return _hashList.IsSynchronized;
				}
			}

			// Token: 0x17000261 RID: 609
			// (get) Token: 0x06001065 RID: 4197 RVA: 0x000723E4 File Offset: 0x000705E4
			public int Count
			{
				get
				{
					return _hashList.Count;
				}
			}

			// Token: 0x06001066 RID: 4198 RVA: 0x000723F4 File Offset: 0x000705F4
			public void CopyTo(Array array, int index)
			{
				_hashList.CopyValuesTo(array, index);
			}

			// Token: 0x17000262 RID: 610
			// (get) Token: 0x06001067 RID: 4199 RVA: 0x00072404 File Offset: 0x00070604
			public object SyncRoot
			{
				get
				{
					return _hashList.SyncRoot;
				}
			}

			// Token: 0x06001068 RID: 4200 RVA: 0x00072414 File Offset: 0x00070614
			public IEnumerator GetEnumerator()
			{
				return new HashListEnumerator(_hashList, HashListEnumerator.EnumerationMode.Value);
			}

			// Token: 0x04000D17 RID: 3351
			private HashList _hashList;
		}
	}
}
