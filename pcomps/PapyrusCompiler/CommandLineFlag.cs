using System;

namespace pcomps.PapyrusCompiler
{
	// Token: 0x02000002 RID: 2
	[AttributeUsage(AttributeTargets.Field)]
	internal class CommandLineFlag : Attribute
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public CommandLineFlag(string asFlag, string asDescription)
		{
			this.sFlags = new string[]
			{
				asFlag
			};
			this.sDescription = asDescription;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000207C File Offset: 0x0000027C
		public CommandLineFlag(string[] asFlags, string asDescription)
		{
			this.sFlags = asFlags;
			this.sDescription = asDescription;
		}

		// Token: 0x04000001 RID: 1
		public readonly string[] sFlags;

		// Token: 0x04000002 RID: 2
		public readonly string sDescription;
	}
}
