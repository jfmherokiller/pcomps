using System.Text;

namespace pcomps.Antlr
{
	// Token: 0x02000027 RID: 39
	public class DefaultFileLineFormatter : FileLineFormatter
	{
		// Token: 0x060001A3 RID: 419 RVA: 0x00005F9C File Offset: 0x0000419C
		public override string getFormatString(string fileName, int line, int column)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (fileName != null)
			{
				stringBuilder.Append(fileName + ":");
			}
			if (line != -1)
			{
				if (fileName == null)
				{
					stringBuilder.Append("line ");
				}
				stringBuilder.Append(line);
				if (column != -1)
				{
					stringBuilder.Append(":" + column);
				}
				stringBuilder.Append(":");
			}
			stringBuilder.Append(" ");
			return stringBuilder.ToString();
		}
	}
}
