﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace pcomps.PapyrusCompiler
{
	// Token: 0x02000003 RID: 3
	public class CommandLineArgs
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002094 File Offset: 0x00000294
		public bool Valid
		{
			get
			{
				return bValid;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000209C File Offset: 0x0000029C
		public bool Debug
		{
			get
			{
				bool result = false;
				if (bValid)
				{
					result = bDebug;
				}
				return result;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020BC File Offset: 0x000002BC
		public bool Optimize
		{
			get
			{
				bool result = false;
				if (bValid)
				{
					result = bOptimize;
				}
				return result;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020DC File Offset: 0x000002DC
		public string OutputFolder
		{
			get
			{
				string result = "";
				if (bValid)
				{
					result = sOutputFolder;
				}
				return result;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002100 File Offset: 0x00000300
		public string ImportFolders
		{
			get
			{
				string result = "";
				if (bValid)
				{
					result = sImportFolders;
				}
				return result;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002124 File Offset: 0x00000324
		public string ObjectName
		{
			get
			{
				string result = "";
				if (bValid)
				{
					result = sObjectName;
				}
				return result;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002148 File Offset: 0x00000348
		public string FlagsFile
		{
			get
			{
				string result = "";
				if (bValid)
				{
					result = sFlagsFile;
				}
				return result;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000216C File Offset: 0x0000036C
		public bool NoAsm
		{
			get
			{
				bool result = false;
				if (bValid)
				{
					result = bNoAsm;
				}
				return result;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000218C File Offset: 0x0000038C
		public bool KeepAsm
		{
			get
			{
				bool result = false;
				if (bValid)
				{
					result = bKeepAsm;
				}
				return result;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000021AC File Offset: 0x000003AC
		public bool AsmOnly
		{
			get
			{
				bool result = false;
				if (bValid)
				{
					result = bAsmOnly;
				}
				return result;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000021CC File Offset: 0x000003CC
		public bool All
		{
			get
			{
				return bAll;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000021D4 File Offset: 0x000003D4
		public bool Quiet => bQuiet;

        // Token: 0x0600000F RID: 15 RVA: 0x000021DC File Offset: 0x000003DC
		public CommandLineArgs(IReadOnlyList<string> args)
		{
			BuildCommandLineStructures();
			if (args.Count > 0)
			{
				sObjectName = args[0];
				if (sObjectName != "-?" && sObjectName != "/?")
				{
					bValid = true;
				}
				if (bValid && args.Count > 1)
				{
					var num = 1;
					while (bValid)
					{
						if (num >= args.Count)
						{
							break;
						}

                        bValid = ParseArgument(args[num], out var asFlag, out var asValue) && HandleArgument(asFlag, asValue);
						num++;
					}
				}
			}
			else
			{
				Console.Error.Write("You must specify an object or folder name.\n");
			}
			if (bHelp || !bValid)
			{
				PrintUsage();
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022DC File Offset: 0x000004DC
		private static void PrintUsage()
		{
			Console.Write("Usage:\n");
			Console.Write("PapyrusCompiler <object or folder> [<arguments>]\n");
			Console.Write("\n");
			Console.Write("  object     Specifies the object to compile. (-all is not specified)\n");
			Console.Write("  folder     Specifies the folder to compile. (-all is specified)\n");
			Console.Write("  arguments  One or more of the following:\n");
			foreach (var commandLineFlag in kCommandLineFlagInfo.Keys)
			{
				Console.Write("   -");
				bool flag = true;
				foreach (var text in commandLineFlag.sFlags)
				{
					if (flag)
					{
						Console.Write(text);
						flag = false;
					}
					else
					{
						Console.Write("|{0}", text);
					}
				}
				Type fieldType = kCommandLineFlagInfo[commandLineFlag].FieldType;
				if (fieldType == typeof(string))
				{
					Console.Write("=<string>");
				}
				else if (fieldType != typeof(bool))
				{
					Console.Write("=??");
				}
				Console.Write("\n    {0}\n", commandLineFlag.sDescription);
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002408 File Offset: 0x00000608
		private bool HandleArgument(string asFlag, string asValue)
		{
			bool result = true;
            if (kCommandLineFields.TryGetValue(asFlag, out var fieldInfo))
			{
				Type fieldType = fieldInfo.FieldType;
				if (fieldType == typeof(bool))
				{
					if (asValue != "")
					{
						Console.Error.Write("{0} does not accept a value.\n", asFlag);
						result = false;
					}
					else
					{
						fieldInfo.SetValue(this, true);
					}
				}
				else if (fieldType == typeof(string))
				{
					fieldInfo.SetValue(this, asValue);
				}
				else
				{
					Console.Error.Write($"Internal Error: Cannot handle command line argument type {{0}} for flag {{1}}.\n", fieldInfo.GetType().ToString(), asFlag);
					result = false;
				}
			}
			else
			{
				Console.Error.Write($"Unknown command line argument: {{0}}\n", asFlag);
				result = false;
			}
			return result;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000024B4 File Offset: 0x000006B4
		private static bool ParseArgument(string asArg, out string asFlag, out string asValue)
		{
			bool result = true;
			string text;
			asValue = (text = "");
			asFlag = text;
			var array = asArg.Split(new[]
			{
				'='
			});
			if (array.Length is < 1 or > 2)
			{
				Console.Error.Write("Improperly formed command line argument: {0}\n", asArg);
				result = false;
			}
			else
			{
				asFlag = array[0].ToLowerInvariant();
				asValue = "";
				if (array.Length == 2)
				{
					asValue = array[1];
				}
				if (asFlag[0] != '-' && asFlag[0] != '/')
				{
					Console.Error.Write("Improperly formed command line argument: {0}\n", asArg);
					result = false;
				}
				else
				{
					asFlag = asFlag.Substring(1);
				}
			}
			return result;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002558 File Offset: 0x00000758
		private static void BuildCommandLineStructures()
        {
            if (kCommandLineFields != null) return;
            var fields = typeof(CommandLineArgs).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            kCommandLineFields = new Dictionary<string, FieldInfo>();
            kCommandLineFlagInfo = new Dictionary<CommandLineFlag, FieldInfo>();
            foreach (var fieldInfo in fields)
            {
                var customAttributes = fieldInfo.GetCustomAttributes(false);
                foreach (var obj in customAttributes)
                {
                    CommandLineFlag commandLineFlag = (CommandLineFlag)obj;
                    foreach (var text in commandLineFlag.sFlags)
                    {
                        kCommandLineFields.Add(text.ToLowerInvariant(), fieldInfo);
                    }
                    kCommandLineFlagInfo.Add(commandLineFlag, fieldInfo);
                }
            }
        }

		// Token: 0x04000003 RID: 3
		private readonly bool bValid;

		// Token: 0x04000004 RID: 4
		private readonly string sObjectName = "";

		// Token: 0x04000005 RID: 5
		[CommandLineFlag(new[]
		{
			"debug",
			"d"
		}, "Turns on compiler debugging, outputting dev information to the screen.")]
		private readonly bool bDebug;

		// Token: 0x04000006 RID: 6
		[CommandLineFlag(new[]
		{
			"optimize",
			"op"
		}, "Turns on optimization of scripts.")]
		private readonly bool bOptimize;

		// Token: 0x04000007 RID: 7
		[CommandLineFlag(new[]
		{
			"output",
			"o"
		}, "Sets the compiler's output directory.")]
		private readonly string sOutputFolder = "";

		// Token: 0x04000008 RID: 8
		[CommandLineFlag(new[]
		{
			"import",
			"i"
		}, "Sets the compiler's import directories, separated by semicolons.")]
		private readonly string sImportFolders = "";

		// Token: 0x04000009 RID: 9
		[CommandLineFlag(new[]
		{
			"flags",
			"f"
		}, "Sets the file to use for user-defined flags.")]
		private readonly string sFlagsFile = "";

		// Token: 0x0400000A RID: 10
		[CommandLineFlag(new[]
		{
			"all",
			"a"
		}, "Invokes the compiler against all psc files in the specified directory\n    (interprets object as the folder).")]
		private readonly bool bAll;

		// Token: 0x0400000B RID: 11
		[CommandLineFlag(new[]
		{
			"quiet",
			"q"
		}, "Does not report progress or success (only failures).")]
		private readonly bool bQuiet;

		// Token: 0x0400000C RID: 12
		[CommandLineFlag(new[]
		{
			"noasm"
		}, "Does not generate an assembly file and does not run the assembler.")]
		private readonly bool bNoAsm;

		// Token: 0x0400000D RID: 13
		[CommandLineFlag(new[]
		{
			"keepasm"
		}, "Keeps the assembly file after running the assembler.")]
		private readonly bool bKeepAsm;

		// Token: 0x0400000E RID: 14
		[CommandLineFlag(new[]
		{
			"asmonly"
		}, "Generates an assembly file but does not run the assembler.")]
		private readonly bool bAsmOnly;

		// Token: 0x0400000F RID: 15
		[CommandLineFlag(new[]
		{
			"?"
		}, "Prints usage information.")]
		private readonly bool bHelp;

		// Token: 0x04000010 RID: 16
		private static Dictionary<string, FieldInfo> kCommandLineFields;

		// Token: 0x04000011 RID: 17
		private static Dictionary<CommandLineFlag, FieldInfo> kCommandLineFlagInfo;
	}
}
