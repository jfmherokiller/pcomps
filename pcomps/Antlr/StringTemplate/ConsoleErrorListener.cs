using System;
using System.Reflection;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000215 RID: 533
	public class ConsoleErrorListener : IStringTemplateErrorListener
	{
		// Token: 0x06000F23 RID: 3875 RVA: 0x0006E2B0 File Offset: 0x0006C4B0
		public virtual void Error(string s, Exception e)
		{
			Console.Error.WriteLine(s);
			if (e != null)
			{
				if (e is TargetInvocationException)
				{
					e = ((TargetInvocationException)e).InnerException;
				}
				Console.Error.WriteLine(e.StackTrace);
			}
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x0006E2E8 File Offset: 0x0006C4E8
		public virtual void Warning(string s)
		{
			Console.Out.WriteLine(s);
		}

		// Token: 0x04000CBF RID: 3263
		public static IStringTemplateErrorListener DefaultConsoleListener = new ConsoleErrorListener();
	}
}
