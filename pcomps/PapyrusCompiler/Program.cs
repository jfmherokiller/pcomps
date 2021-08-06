using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using pcomps.PCompiler;

namespace pcomps.PapyrusCompiler
{
	// Token: 0x02000004 RID: 4
	internal class Program
	{
		// Token: 0x06000014 RID: 20 RVA: 0x00002628 File Offset: 0x00000828
		private static void CompilerErrorHandler(object kSender, CompilerErrorEventArgs kArgs)
		{
			string value = string.Format("{0}({1},{2}): {3}\n", new object[]
			{
				kArgs.Filename,
				kArgs.LineNumber,
				kArgs.ColumnNumber,
				kArgs.Message
			});
			Console.Error.Write(value);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002684 File Offset: 0x00000884
		private static void CompilerNotifyHandler(object kSender, CompilerNotifyEventArgs kArgs)
		{
			Console.Out.WriteLine(kArgs.sMessage);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002698 File Offset: 0x00000898
		private static void CompilerThread()
		{
			Compiler compiler = new Compiler();
			compiler.CompilerErrorHandler += Program.CompilerErrorHandler;
			compiler.CompilerNotifyHandler += Program.CompilerNotifyHandler;
			compiler.bDebug = Program.kArgs.Debug;
			compiler.OutputFolder = Program.kArgs.OutputFolder;
			compiler.ImportFolders = new List<string>(Program.kArgs.ImportFolders.Split(new char[]
			{
				';'
			}));
			if (Program.kArgs.NoAsm)
			{
				compiler.eAsmOption = Compiler.AssemblyOption.NoAssembly;
			}
			else if (Program.kArgs.AsmOnly)
			{
				compiler.eAsmOption = Compiler.AssemblyOption.GenerateOnly;
			}
			else if (Program.kArgs.KeepAsm)
			{
				compiler.eAsmOption = Compiler.AssemblyOption.AssembleAndKeep;
			}
			compiler.bQuiet = Program.kArgs.Quiet;
			int num;
			while ((num = Interlocked.Increment(ref Program.iLastFileNumber)) < Program.FilenamesA.Length)
			{
				string text = Program.FilenamesA[num];
                string FileDirectory = Path.GetDirectoryName(text);
                compiler.ImportFolders.Add(FileDirectory);
                compiler.ImportFolders = compiler.ImportFolders;
                if (Path.GetExtension(text).ToLowerInvariant() == ".psc")
				{
					text = Path.GetFileNameWithoutExtension(text);
				}
				if (!Program.kArgs.Quiet)
				{
					Console.Write($"Compiling \"{text}\"...\n");
				}
				if (compiler.Compile(text, Program.kArgs.FlagsFile, Program.kArgs.Optimize))
				{
					Interlocked.Increment(ref Program.iCompilesSucceeded);
					if (!Program.kArgs.Quiet)
					{
						Console.Write("Compilation succeeded.\n");
					}
				}
				else
				{
					Console.Write($"No output generated for {Program.FilenamesA[num]}, compilation failed.\n");
					Program.FailureMutex.WaitOne();
					Program.FailedCompiles.Add(Program.FilenamesA[num]);
					Program.FailureMutex.ReleaseMutex();
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002838 File Offset: 0x00000A38
		private static void Main(string[] args)
		{
			Program.kArgs = new CommandLineArgs(args);
			if (Program.kArgs.Valid)
			{
				bool flag = false;
				if (Program.kArgs.All)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(Program.kArgs.ObjectName);
					if (directoryInfo.Exists)
					{
						FileInfo[] files = directoryInfo.GetFiles("*.psc");
						if (files.Length > 0)
						{
							Program.FilenamesA = new string[files.Length];
							for (int i = 0; i < files.Length; i++)
							{
								Program.FilenamesA[i] = files[i].ToString();
							}
						}
						else
						{
							Console.WriteLine($"Folder \"{directoryInfo.FullName}\" does not contain any script files");
							flag = true;
						}
					}
					else
					{
						Console.WriteLine($"Folder \"{directoryInfo.FullName}\" does not exist");
						flag = true;
					}
				}
				else
				{
					Program.FilenamesA = new string[1];
					Program.FilenamesA[0] = Program.kArgs.ObjectName;
				}
				if (Program.FilenamesA != null && Program.FilenamesA.Length > 0)
				{
					int num = Math.Min(Program.FilenamesA.Length, Environment.ProcessorCount + 1);
					if (!Program.kArgs.Quiet)
					{
						Console.Write("Starting {0} compile threads for {1} files...\n", num, Program.FilenamesA.Length);
					}
					Thread[] array = new Thread[num];
					for (int j = 0; j < num; j++)
					{
						array[j] = new Thread(new ThreadStart(Program.CompilerThread));
						array[j].Start();
					}
					foreach (Thread thread in array)
					{
						thread.Join();
					}
					if (!Program.kArgs.Quiet)
					{
						Console.Write("\nBatch compile of {0} files finished. {1} succeeded, {2} failed.\n", Program.FilenamesA.Length, Program.iCompilesSucceeded, Program.FilenamesA.Length - Program.iCompilesSucceeded);
						for (int l = 0; l < Program.FailedCompiles.Count; l++)
						{
							Console.WriteLine("Failed on {0}", Program.FailedCompiles[l]);
						}
					}
				}
				if (flag || Program.FailedCompiles.Count > 0)
				{
					Environment.ExitCode = -1;
				}
			}
		}

		// Token: 0x04000012 RID: 18
		private static CommandLineArgs kArgs = null;

		// Token: 0x04000013 RID: 19
		private static string[] FilenamesA = null;

		// Token: 0x04000014 RID: 20
		private static int iLastFileNumber = -1;

		// Token: 0x04000015 RID: 21
		private static int iCompilesSucceeded = 0;

		// Token: 0x04000016 RID: 22
		private static Mutex FailureMutex = new Mutex();

		// Token: 0x04000017 RID: 23
		private static List<string> FailedCompiles = new List<string>();
	}
}
