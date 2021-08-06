using System;
using System.IO;

namespace pcomps.Antlr.Runtime.Misc
{
	// Token: 0x020000C2 RID: 194
	public class Stats
	{
		// Token: 0x06000830 RID: 2096 RVA: 0x000170B0 File Offset: 0x000152B0
		public static double Stddev(int[] X)
		{
			int num = X.Length;
			if (num <= 1)
			{
				return 0.0;
			}
			double num2 = Stats.Avg(X);
			double num3 = 0.0;
			for (int i = 0; i < num; i++)
			{
				num3 += ((double)X[i] - num2) * ((double)X[i] - num2);
			}
			num3 /= (double)(num - 1);
			return Math.Sqrt(num3);
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x00017114 File Offset: 0x00015314
		public static double Avg(int[] X)
		{
			double num = 0.0;
			int num2 = X.Length;
			if (num2 == 0)
			{
				return 0.0;
			}
			for (int i = 0; i < num2; i++)
			{
				num += (double)X[i];
			}
			if (num >= 0.0)
			{
				return num / (double)num2;
			}
			return 0.0;
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x00017178 File Offset: 0x00015378
		public static int Min(int[] X)
		{
			int num = int.MaxValue;
			int num2 = X.Length;
			if (num2 == 0)
			{
				return 0;
			}
			for (int i = 0; i < num2; i++)
			{
				if (X[i] < num)
				{
					num = X[i];
				}
			}
			return num;
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x000171B8 File Offset: 0x000153B8
		public static int Max(int[] X)
		{
			int num = int.MinValue;
			int num2 = X.Length;
			if (num2 == 0)
			{
				return 0;
			}
			for (int i = 0; i < num2; i++)
			{
				if (X[i] > num)
				{
					num = X[i];
				}
			}
			return num;
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x000171F8 File Offset: 0x000153F8
		public static int Sum(int[] X)
		{
			int num = 0;
			int num2 = X.Length;
			if (num2 == 0)
			{
				return 0;
			}
			for (int i = 0; i < num2; i++)
			{
				num += X[i];
			}
			return num;
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0001722C File Offset: 0x0001542C
		public static void WriteReport(string filename, string data)
		{
			string absoluteFileName = Stats.GetAbsoluteFileName(filename);
			FileInfo fileInfo = new FileInfo(absoluteFileName);
			fileInfo.Directory.Create();
			try
			{
				StreamWriter streamWriter = new StreamWriter(fileInfo.FullName, true);
				streamWriter.WriteLine(data);
				streamWriter.Close();
			}
			catch (IOException e)
			{
				ErrorManager.InternalError($"can't write stats to {absoluteFileName}", e);
			}
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x00017298 File Offset: 0x00015498
		public static string GetAbsoluteFileName(string filename)
		{
			return Path.Combine(Path.Combine(Environment.CurrentDirectory, Constants.ANTLRWORKS_DIR), filename);
		}
	}
}
