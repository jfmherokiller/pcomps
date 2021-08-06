using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace pcomps.Antlr.Runtime
{
	// Token: 0x020000B5 RID: 181
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Messages
	{
		// Token: 0x06000748 RID: 1864 RVA: 0x00013F14 File Offset: 0x00012114
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Messages()
		{
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x00013F1C File Offset: 0x0001211C
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Messages.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("Antlr.Runtime.Messages", typeof(Messages).Assembly);
					Messages.resourceMan = resourceManager;
				}
				return Messages.resourceMan;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x00013F60 File Offset: 0x00012160
		// (set) Token: 0x0600074B RID: 1867 RVA: 0x00013F68 File Offset: 0x00012168
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Messages.resourceCulture;
			}
			set
			{
				Messages.resourceCulture = value;
			}
		}

		// Token: 0x040001D7 RID: 471
		private static ResourceManager resourceMan;

		// Token: 0x040001D8 RID: 472
		private static CultureInfo resourceCulture;
	}
}
