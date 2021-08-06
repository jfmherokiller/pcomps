using System;
using System.Diagnostics;
using System.Text;

namespace pcomps.Antlr.Runtime.Misc
{
	// Token: 0x020000C3 RID: 195
	public class ErrorManager
	{
		// Token: 0x06000838 RID: 2104 RVA: 0x000172B8 File Offset: 0x000154B8
		public static void InternalError(object error, Exception e)
		{
			StackFrame lastNonErrorManagerCodeLocation = ErrorManager.GetLastNonErrorManagerCodeLocation(e);
			string arg = string.Concat(new object[]
			{
				"Exception ",
				e,
				"@",
				lastNonErrorManagerCodeLocation,
				": ",
				error
			});
			ErrorManager.Error(arg);
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x00017304 File Offset: 0x00015504
		public static void InternalError(object error)
		{
			StackFrame lastNonErrorManagerCodeLocation = ErrorManager.GetLastNonErrorManagerCodeLocation(new Exception());
			string arg = $"{lastNonErrorManagerCodeLocation}: {error}";
			ErrorManager.Error(arg);
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x00017330 File Offset: 0x00015530
		private static StackFrame GetLastNonErrorManagerCodeLocation(Exception e)
		{
			StackTrace stackTrace = new StackTrace(e);
			int i;
			for (i = 0; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				if (frame.ToString().IndexOf("ErrorManager") < 0)
				{
					break;
				}
			}
			return stackTrace.GetFrame(i);
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x00017388 File Offset: 0x00015588
		public static void Error(object arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("internal error: {0} ", arg);
		}
	}
}
