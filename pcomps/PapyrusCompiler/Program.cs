﻿using System;
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
			compiler.CompilerErrorHandler += CompilerErrorHandler;
			compiler.CompilerNotifyHandler += CompilerNotifyHandler;
			compiler.bDebug = kArgs.Debug;
			compiler.OutputFolder = kArgs.OutputFolder;
			compiler.ImportFolders = new List<string>(kArgs.ImportFolders.Split(new char[]
			{
				';'
			}));
			if (kArgs.NoAsm)
			{
				compiler.eAsmOption = Compiler.AssemblyOption.NoAssembly;
			}
			else if (kArgs.AsmOnly)
			{
				compiler.eAsmOption = Compiler.AssemblyOption.GenerateOnly;
			}
			else if (kArgs.KeepAsm)
			{
				compiler.eAsmOption = Compiler.AssemblyOption.AssembleAndKeep;
			}
			compiler.bQuiet = kArgs.Quiet;
			int num;
			while ((num = Interlocked.Increment(ref iLastFileNumber)) < FilenamesA.Length)
			{
				string text = FilenamesA[num];
                string FileDirectory = Path.GetDirectoryName(text);
                compiler.ImportFolders.Add(FileDirectory);
                compiler.ImportFolders = compiler.ImportFolders;
                if (Path.GetExtension(text).ToLowerInvariant() == ".psc")
				{
					text = Path.GetFileNameWithoutExtension(text);
				}
				if (!kArgs.Quiet)
				{
					Console.Write($"Compiling \"{text}\"...\n");
				}
				if (compiler.Compile(text, kArgs.FlagsFile, kArgs.Optimize))
				{
					Interlocked.Increment(ref iCompilesSucceeded);
					if (!kArgs.Quiet)
					{
						Console.Write("Compilation succeeded.\n");
					}
				}
				else
				{
					Console.Write($"No output generated for {FilenamesA[num]}, compilation failed.\n");
					FailureMutex.WaitOne();
					FailedCompiles.Add(FilenamesA[num]);
					FailureMutex.ReleaseMutex();
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002838 File Offset: 0x00000A38
		private static void Main(string[] args)
		{
			kArgs = new CommandLineArgs(args);
			if (kArgs.Valid)
			{
				bool flag = false;
				if (kArgs.All)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(kArgs.ObjectName);
					if (directoryInfo.Exists)
					{
						FileInfo[] files = directoryInfo.GetFiles("*.psc");
						if (files.Length > 0)
						{
							FilenamesA = new string[files.Length];
							for (int i = 0; i < files.Length; i++)
							{
								FilenamesA[i] = files[i].ToString();
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
					FilenamesA = new string[1];
					FilenamesA[0] = kArgs.ObjectName;
				}
				if (FilenamesA != null && FilenamesA.Length > 0)
				{
					int num = Math.Min(FilenamesA.Length, Environment.ProcessorCount + 1);
					if (!kArgs.Quiet)
					{
						Console.Write("Starting {0} compile threads for {1} files...\n", num, FilenamesA.Length);
					}
					Thread[] array = new Thread[num];
					for (int j = 0; j < num; j++)
					{
						array[j] = new Thread(new ThreadStart(CompilerThread));
						array[j].Start();
					}
					foreach (Thread thread in array)
					{
						thread.Join();
					}
					if (!kArgs.Quiet)
					{
						Console.Write("\nBatch compile of {0} files finished. {1} succeeded, {2} failed.\n", FilenamesA.Length, iCompilesSucceeded, FilenamesA.Length - iCompilesSucceeded);
						for (int l = 0; l < FailedCompiles.Count; l++)
						{
							Console.WriteLine("Failed on {0}", FailedCompiles[l]);
						}
					}
				}
				if (flag || FailedCompiles.Count > 0)
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
