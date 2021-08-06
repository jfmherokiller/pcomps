using System.Collections;
using System.Text;

namespace pcomps.Antlr.Runtime.Collections
{
	// Token: 0x0200009B RID: 155
	public class CollectionUtils
	{
		// Token: 0x06000590 RID: 1424 RVA: 0x000106A4 File Offset: 0x0000E8A4
		public static string ListToString(IList coll)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (coll != null)
			{
				stringBuilder.Append("[");
				for (int i = 0; i < coll.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					object obj = coll[i];
					if (obj == null)
					{
						stringBuilder.Append("null");
					}
					else if (obj is IDictionary)
					{
						stringBuilder.Append(CollectionUtils.DictionaryToString((IDictionary)obj));
					}
					else if (obj is IList)
					{
						stringBuilder.Append(CollectionUtils.ListToString((IList)obj));
					}
					else
					{
						stringBuilder.Append(obj.ToString());
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

		// Token: 0x06000591 RID: 1425 RVA: 0x00010788 File Offset: 0x0000E988
		public static string DictionaryToString(IDictionary dict)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (dict != null)
			{
				stringBuilder.Append("{");
				int num = 0;
				foreach (object obj in dict)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (num > 0)
					{
						stringBuilder.Append(", ");
					}
					if (dictionaryEntry.Value is IDictionary)
					{
						stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key.ToString(), CollectionUtils.DictionaryToString((IDictionary)dictionaryEntry.Value));
					}
					else if (dictionaryEntry.Value is IList)
					{
						stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key.ToString(), CollectionUtils.ListToString((IList)dictionaryEntry.Value));
					}
					else
					{
						stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
					}
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
	}
}
