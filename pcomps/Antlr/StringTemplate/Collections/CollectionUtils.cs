using System;
using System.Collections;
using System.Text;

namespace pcomps.Antlr.StringTemplate.Collections
{
	// Token: 0x0200022A RID: 554
	internal class CollectionUtils
	{
		// Token: 0x0600102B RID: 4139 RVA: 0x00071AC0 File Offset: 0x0006FCC0
		public static string ListToString(IList coll)
		{
			var stringBuilder = new StringBuilder();
			if (coll != null)
			{
				stringBuilder.Append("[");
				for (var i = 0; i < coll.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					if (coll[i] == null)
					{
						stringBuilder.Append("null");
					}
					else
					{
						stringBuilder.Append(coll[i].ToString());
					}
				}
				stringBuilder.Append("]");
			}
			else
			{
				stringBuilder.Insert(0, "null");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600102C RID: 4140 RVA: 0x00071B50 File Offset: 0x0006FD50
		public static string DictionaryToString(IDictionary dict)
		{
			var stringBuilder = new StringBuilder();
			if (dict != null)
			{
				stringBuilder.Append("{");
				var num = 0;
				foreach (var obj in dict)
				{
					var dictionaryEntry = (DictionaryEntry)obj;
					if (num > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
					num++;
				}
				stringBuilder.Append("}");
			}
			else
			{
				stringBuilder.Insert(0, "null");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600102D RID: 4141 RVA: 0x00071C14 File Offset: 0x0006FE14
		public static bool IsEmptyEnumerator(IEnumerator enumerator)
		{
			try
			{
				var obj = enumerator.Current;
				return false;
			}
			catch (InvalidOperationException)
			{
				try
				{
					if (enumerator.MoveNext())
					{
						enumerator.Reset();
						return false;
					}
				}
				catch (InvalidOperationException)
				{
				}
			}
			return true;
		}
	}
}
