using System.IO;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x0200021F RID: 543
	public class NoIndentWriter : AutoIndentWriter
	{
		// Token: 0x06000F5A RID: 3930 RVA: 0x0006EA58 File Offset: 0x0006CC58
		public NoIndentWriter(TextWriter output) : base(output)
		{
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0006EA64 File Offset: 0x0006CC64
		public override int Write(string str)
		{
			output.Write(str);
			return str.Length;
		}
	}
}
