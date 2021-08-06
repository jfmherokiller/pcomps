using System.Collections;
using System.Linq;
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
				for (var i = 0; i < coll.Count; i++)
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
						stringBuilder.Append(DictionaryToString((IDictionary)obj));
					}
					else if (obj is IList)
					{
						stringBuilder.Append(ListToString((IList)obj));
					}
					else
					{
						stringBuilder.Append(obj);
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
				stringBuilder.Append('{');
				var num = 0;
				foreach (var dictionaryEntry in dict.Cast<DictionaryEntry>())
                {
                    if (num > 0)
                    {
                        stringBuilder.Append(", ");
                    }
                    switch (dictionaryEntry.Value)
                    {
                        case IDictionary dictionary:
                            stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key, DictionaryToString(dictionary));
                            break;
                        case IList list:
                            stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key, ListToString(list));
                            break;
                        default:
                            stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key, dictionaryEntry.Value);
                            break;
                    }
                    num++;
                }
				stringBuilder.Append('}');
			}
			else
			{
				stringBuilder.Insert(0, "null");
			}
			return stringBuilder.ToString();
		}
	}
}
