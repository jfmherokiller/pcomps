namespace pcomps.Antlr
{
	// Token: 0x02000039 RID: 57
	public class StringUtils
	{
		// Token: 0x0600024A RID: 586 RVA: 0x00007DE8 File Offset: 0x00005FE8
		public static string stripBack(string s, char c)
		{
			while (s.Length > 0 && s[s.Length - 1] == c)
			{
				s = s.Substring(0, s.Length - 1);
			}
			return s;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00007E24 File Offset: 0x00006024
		public static string stripBack(string s, string remove)
		{
			bool flag;
			do
			{
				flag = false;
				foreach (char c in remove)
				{
					while (s.Length > 0 && s[s.Length - 1] == c)
					{
						flag = true;
						s = s.Substring(0, s.Length - 1);
					}
				}
			}
			while (flag);
			return s;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00007E80 File Offset: 0x00006080
		public static string stripFront(string s, char c)
		{
			while (s.Length > 0 && s[0] == c)
			{
				s = s.Substring(1);
			}
			return s;
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00007EAC File Offset: 0x000060AC
		public static string stripFront(string s, string remove)
		{
			bool flag;
			do
			{
				flag = false;
				foreach (char c in remove)
				{
					while (s.Length > 0 && s[0] == c)
					{
						flag = true;
						s = s.Substring(1);
					}
				}
			}
			while (flag);
			return s;
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00007EF8 File Offset: 0x000060F8
		public static string stripFrontBack(string src, string head, string tail)
		{
			int num = src.IndexOf(head);
			int num2 = src.LastIndexOf(tail);
			if (num == -1 || num2 == -1)
			{
				return src;
			}
			return src.Substring(num + 1, num2 - (num + 1));
		}
	}
}
