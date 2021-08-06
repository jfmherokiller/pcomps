using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using pcomps.Antlr.Runtime;
using pcomps.Antlr.Runtime.Tree;
using pcomps.Antlr.StringTemplate;
using pcomps.Antlr.Utility.Tree;

namespace pcomps.PCompiler
{
	// Token: 0x020001BE RID: 446
	public class Compiler
	{
		// Token: 0x1400003A RID: 58
		// (add) Token: 0x06000D2D RID: 3373 RVA: 0x0005CBAC File Offset: 0x0005ADAC
		// (remove) Token: 0x06000D2E RID: 3374 RVA: 0x0005CBC8 File Offset: 0x0005ADC8
		public event CompilerErrorEventHandler CompilerErrorHandler;

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x06000D2F RID: 3375 RVA: 0x0005CBE4 File Offset: 0x0005ADE4
		// (remove) Token: 0x06000D30 RID: 3376 RVA: 0x0005CC00 File Offset: 0x0005AE00
		public event CompilerNotifyEventHandler CompilerNotifyHandler;

		// Token: 0x06000D31 RID: 3377 RVA: 0x0005CC1C File Offset: 0x0005AE1C
		public bool Compile(string asObjectName, bool abOptimize)
		{
			return Compile(asObjectName, "", abOptimize);
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x0005CC2C File Offset: 0x0005AE2C
		public bool Compile(string asObjectName, string asFlagsFile, bool abOptimize)
		{
			if (bOutputValid && bImportValid)
			{
				sFileStack.Clear();
				iNumErrors = 0;
				if (asFlagsFile != "")
				{
					ParseFlags(asFlagsFile);
				}
				Dictionary<string, ScriptObjectType> dictionary = new Dictionary<string, ScriptObjectType>();
				ScriptObjectType akObj = LoadObject(asObjectName, dictionary);
				if (bDebug)
				{
					var stringBuilder = new StringBuilder("Known types after parsing:\n");
					foreach (var arg in dictionary.Keys)
					{
						stringBuilder.Append($"\t-{arg}\n");
					}
					OnCompilerNotify(stringBuilder.ToString());
				}
				if (abOptimize && iNumErrors == 0)
				{
					Optimize(akObj);
				}
				string value = "";
				if (iNumErrors == 0)
				{
					value = GenerateCode(akObj);
				}
				var text = $"{asObjectName}.pas";
				if (sOutputFolder != "")
				{
					text = Path.Combine(sOutputFolder, text);
				}
				if (iNumErrors == 0 && eAsmOption != AssemblyOption.NoAssembly)
				{
					try
					{
						if (sOutputFolder != "" && !Directory.Exists(sOutputFolder))
						{
							Directory.CreateDirectory(sOutputFolder);
						}
						StreamWriter streamWriter = new StreamWriter(text);
						streamWriter.Write(value);
						streamWriter.Close();
					}
					catch (Exception ex)
					{
						OnCompilerError(ex.Message);
					}
					if (iNumErrors == 0 && (eAsmOption == AssemblyOption.AssembleAndDelete || eAsmOption == AssemblyOption.AssembleAndKeep) && Assemble(asObjectName) && !bDebug && eAsmOption == AssemblyOption.AssembleAndDelete && File.Exists(text))
					{
						File.Delete(text);
					}
				}
			}
			return iNumErrors == 0 && bOutputValid && bImportValid;
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x0005CE1C File Offset: 0x0005B01C
		internal ScriptObjectType LoadObject(string asObjectName, Dictionary<string, ScriptObjectType> akKnownTypes)
		{
			return LoadObject(asObjectName, akKnownTypes, new Stack<string>(), true, null);
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x0005CE30 File Offset: 0x0005B030
		internal ScriptObjectType LoadObject(string asObjectName, Dictionary<string, ScriptObjectType> akKnownTypes, bool abErrorOnNoObject)
		{
			return LoadObject(asObjectName, akKnownTypes, new Stack<string>(), abErrorOnNoObject, null);
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x0005CE44 File Offset: 0x0005B044
		internal ScriptObjectType LoadObject(string asObjectName, Dictionary<string, ScriptObjectType> akKnownTypes, Stack<string> akChildren, bool abErrorOnNoObject, ScriptObjectType akImmediateChild)
		{
			ScriptObjectType scriptObjectType;
			if (!akKnownTypes.TryGetValue(asObjectName.ToLowerInvariant(), out scriptObjectType))
			{
				string text;
				if (GetFilename(asObjectName, out text))
				{
					StartImportFile(text);
					int num = iNumErrors;
					iNumErrors = 0;
					try
					{
						if (asObjectName.Length > 38)
						{
							string asMessage =
                                $"\"{asObjectName}\" is too long, please shorten it to {38} characters or less";
							OnCompilerError(asMessage);
						}
						CaseInsensitiveFileStream akInput = new CaseInsensitiveFileStream(text);
						CommonTokenStream akTokenStream = GenerateTokenStream(akInput);
						Parse(akTokenStream, out scriptObjectType);
						if (scriptObjectType.Name != asObjectName.ToLowerInvariant())
						{
							OnCompilerError($"filename does not match script name: {scriptObjectType.Name}");
						}
						else
						{
							bool flag = iNumErrors == 0;
							if (bDebug && sFileStack.Count == 1)
							{
								string asFilename = $"{asObjectName}.preTypeCheck.dot";
								string arg = OutputAST(asFilename, scriptObjectType.kAST);
								StringBuilder stringBuilder = new StringBuilder();
								stringBuilder.AppendFormat("Pre-typecheck AST is located in \"{0}\"", arg);
								OnCompilerNotify(stringBuilder.ToString());
							}
							bool flag2 = false;
							if (flag)
							{
								if (akImmediateChild != null)
								{
									akImmediateChild.kParent = scriptObjectType;
								}
								TypeCheck(scriptObjectType, akKnownTypes, akChildren);
								flag2 = (iNumErrors == 0);
							}
							if (bDebug && flag2 && sFileStack.Count == 1)
							{
								string asFilename2 = $"{asObjectName}.postTypeCheck.dot";
								string arg2 = OutputAST(asFilename2, scriptObjectType.kAST);
								StringBuilder stringBuilder2 = new StringBuilder();
								stringBuilder2.AppendFormat("Post-typecheck AST is located in \"{0}\"", arg2);
								OnCompilerNotify(stringBuilder2.ToString());
							}
						}
					}
					catch (Exception ex)
					{
						OnCompilerError($"error while attempting to read script {asObjectName}: {ex.Message}");
						scriptObjectType = null;
					}
					iNumErrors += num;
					FinishImportFile();
				}
				else if (abErrorOnNoObject)
				{
					OnCompilerError($"unable to locate script {asObjectName}");
					scriptObjectType = null;
				}
			}
			if (scriptObjectType == null && akImmediateChild != null)
			{
				akImmediateChild.kParent = null;
			}
			return scriptObjectType;
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x0005D050 File Offset: 0x0005B250
		private CommonTokenStream GenerateTokenStream(ICharStream akInput)
		{
			PapyrusLexer papyrusLexer = new PapyrusLexer(akInput);
			papyrusLexer.ErrorHandler += OnInternalError;
			return new CommonTokenStream(papyrusLexer);
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x0005D07C File Offset: 0x0005B27C
		private void Parse(ITokenStream akTokenStream, out ScriptObjectType akParsedObj)
		{
			PapyrusParser papyrusParser = new PapyrusParser(akTokenStream);
			papyrusParser.ErrorHandler += OnInternalError;
			papyrusParser.KnownUserFlags = kFlagDict;
			papyrusParser.script();
			akParsedObj = papyrusParser.ParsedObject;
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x0005D0C0 File Offset: 0x0005B2C0
		private void TypeCheck(ScriptObjectType akObj, Dictionary<string, ScriptObjectType> akKnownTypes)
		{
			TypeCheck(akObj, akKnownTypes, new Stack<string>());
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x0005D0D0 File Offset: 0x0005B2D0
		private void TypeCheck(ScriptObjectType akObj, Dictionary<string, ScriptObjectType> akKnownTypes, Stack<string> akChildren)
		{
			PapyrusTypeWalker papyrusTypeWalker = new PapyrusTypeWalker(new CommonTreeNodeStream(akObj.kAST)
			{
				TokenStream = akObj.kTokenStream
			});
			papyrusTypeWalker.ErrorHandler += OnInternalError;
			papyrusTypeWalker.script(akObj, this, akKnownTypes, akChildren);
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x0005D11C File Offset: 0x0005B31C
		private void ParseFlags(string asFlagsFile)
		{
			string text;
			if (FindFile(asFlagsFile, out text))
			{
				StartImportFile(text);
				CaseInsensitiveFileStream input = new CaseInsensitiveFileStream(text);
				FlagsLexer flagsLexer = new FlagsLexer(input);
				flagsLexer.ErrorHandler += OnInternalError;
				CommonTokenStream input2 = new CommonTokenStream(flagsLexer);
				FlagsParser flagsParser = new FlagsParser(input2);
				flagsParser.ErrorHandler += OnInternalError;
				flagsParser.flags();
				FinishImportFile();
				kFlagDict = flagsParser.DefinedFlags;
				if (bDebug)
				{
					if (kFlagDict.Count > 0)
					{
						using (Dictionary<string, PapyrusFlag>.Enumerator enumerator = kFlagDict.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<string, PapyrusFlag> keyValuePair = enumerator.Current;
								OnCompilerNotify($"Flag {keyValuePair.Key}: {keyValuePair.Value.Index}");
							}
							return;
						}
					}
					OnCompilerNotify("No user flags defined");
					return;
				}
			}
			else
			{
				OnCompilerError($"Unable to find flags file: {asFlagsFile}");
			}
		}

		// Token: 0x06000D3B RID: 3387 RVA: 0x0005D23C File Offset: 0x0005B43C
		private bool FindFile(string asFilename, out string asFoundFilename)
		{
			asFoundFilename = asFilename;
			bool flag = File.Exists(asFilename);
			int num = 0;
			while (num < kImportFolders.Count && !flag)
			{
				asFoundFilename = Path.Combine(kImportFolders[num], asFilename);
				flag = File.Exists(asFoundFilename);
				num++;
			}
			return flag;
		}

		// Token: 0x06000D3C RID: 3388 RVA: 0x0005D28C File Offset: 0x0005B48C
		private bool GetFilename(string asObjectName, out string asFilename)
		{
			return kObjectToPath.TryGetValue(asObjectName.ToLower(), out asFilename);
		}

		// Token: 0x06000D3D RID: 3389 RVA: 0x0005D2A0 File Offset: 0x0005B4A0
		private void Optimize(ScriptObjectType akObj)
		{
			string asFilename;
			GetFilename(akObj.Name, out asFilename);
			StartOptimizeFile(asFilename);
			while (RunOptimizePass(akObj, PapyrusOptimizeWalker.OptimizePass.NORMAL) && iNumErrors == 0)
			{
			}
			if (iNumErrors == 0)
			{
				RunOptimizePass(akObj, PapyrusOptimizeWalker.OptimizePass.VARCLEANUP);
			}
			FinishOptimizeFile();
			if (bDebug && iNumErrors == 0)
			{
				string asFilename2 = $"{akObj.Name}.postOptimize.dot";
				string arg = OutputAST(asFilename2, akObj.kAST);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Post-optimize AST is located in \"{0}\"", arg);
				OnCompilerNotify(stringBuilder.ToString());
			}
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x0005D33C File Offset: 0x0005B53C
		private bool RunOptimizePass(ScriptObjectType akObj, PapyrusOptimizeWalker.OptimizePass aePass)
		{
			PapyrusOptimizeWalker papyrusOptimizeWalker = new PapyrusOptimizeWalker(new CommonTreeNodeStream(akObj.kAST)
			{
				TokenStream = akObj.kTokenStream
			});
			bool result = false;
			try
			{
				papyrusOptimizeWalker.ErrorHandler += OnInternalError;
				papyrusOptimizeWalker.script(akObj, aePass);
				result = papyrusOptimizeWalker.bMadeChanges;
			}
			catch (Exception ex)
			{
				OnCompilerError($"error while attempting to optimize script {akObj.Name}: {ex.Message}");
			}
			return result;
		}

		// Token: 0x06000D3F RID: 3391 RVA: 0x0005D3C0 File Offset: 0x0005B5C0
		private string GenerateCode(ScriptObjectType akObj)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("pcomps.PCompiler.PapyrusAssembly.stg");
			string asSourceFilename;
			GetFilename(akObj.Name, out asSourceFilename);
			PapyrusGen papyrusGen = new PapyrusGen(new CommonTreeNodeStream(akObj.kAST)
			{
				TokenStream = akObj.kTokenStream
			});
			string result = "";
			try
			{
				papyrusGen.ErrorHandler += OnInternalError;
				papyrusGen.TemplateLib = new StringTemplateGroup(new StreamReader(manifestResourceStream));
				papyrusGen.KnownUserFlags = kFlagDict;
				StringTemplate stringTemplate = (StringTemplate)papyrusGen.script(asSourceFilename, akObj).Template;
				if (iNumErrors == 0 && stringTemplate != null)
				{
					result = stringTemplate.ToString();
				}
			}
			catch (Exception ex)
			{
				OnCompilerError($"error while attempting to optimize script {akObj.Name}: {ex.Message}");
			}
			return result;
		}

		// Token: 0x06000D40 RID: 3392 RVA: 0x0005D4A8 File Offset: 0x0005B6A8
		private string OutputAST(string asFilename, CommonTree akTree)
		{
			OnCompilerNotify($"Generating {asFilename}...");
			StringTemplate treeST = new StringTemplate("digraph {\n ordering=out;\n ranksep=.4\n rankdir=LR\n bgcolor=\"lightgrey\";\n node [shape=box, fixedsize=false, fontsize=12, fontname=\"Helvetica-bold\", fontcolor=\"blue\"\n       width=.25, height=.25, color=\"black\", style=\"bold\"]\n $nodes$\n $edges$\n}\n");
			StringTemplate edgeST = new StringTemplate("$parent$ -> $child$ // \"$parentText$\" -> \"$childText$\"\n");
			DOTTreeGenerator dottreeGenerator = new DOTTreeGenerator();
			StringTemplate stringTemplate = dottreeGenerator.ToDOT(akTree, new CommonTreeAdaptor(), treeST, edgeST);
			string text = stringTemplate.ToString();
			text = text.Replace("\\\\\"", "\\\"");
			StreamWriter streamWriter = new StreamWriter(asFilename);
			streamWriter.Write(text);
			streamWriter.Close();
			string text2 = Path.ChangeExtension(asFilename, ".png");
			try
			{
				Process process = new Process();
				process.StartInfo.FileName = "dot.exe";
				process.StartInfo.Arguments = $"-Tpng -o{text2} {asFilename}";
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.Start();
				string text3 = process.StandardOutput.ReadToEnd();
				if (text3 != "")
				{
					OnCompilerNotify(text3);
				}
				process.WaitForExit();
				OnCompilerNotify("Conversion to png succeeded.");
			}
			catch (Exception)
			{
				text2 = asFilename;
				OnCompilerNotify("Conversion to png failed.");
			}
			return text2;
		}

		// Token: 0x06000D41 RID: 3393 RVA: 0x0005D5E0 File Offset: 0x0005B7E0
		private bool Assemble(string asObjectName)
		{
			bool flag = true;
			string path = "PapyrusAssembler.exe";
			Uri uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
			string directoryName = Path.GetDirectoryName(uri.LocalPath);
			string text = Path.Combine(directoryName, path);
			if (!File.Exists(text))
			{
				text = Path.Combine(Environment.CurrentDirectory, path);
				if (!File.Exists(text))
				{
					OnCompilerError("Assembly failed - could not find the assembler");
					flag = false;
				}
			}
			if (flag)
			{
				try
				{
					Process process = new Process();
					process.StartInfo.FileName = text;
					process.StartInfo.Arguments = asObjectName;
					if (bDebug)
					{
						ProcessStartInfo startInfo = process.StartInfo;
						startInfo.Arguments += " /V";
					}
					if (bQuiet)
					{
						ProcessStartInfo startInfo2 = process.StartInfo;
						startInfo2.Arguments += " /Q";
					}
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.WorkingDirectory = sOutputFolder;
					process.Start();
					string text2 = process.StandardOutput.ReadToEnd();
					if (text2 != "")
					{
						OnCompilerNotify(text2);
					}
					process.WaitForExit();
					if (process.ExitCode != 0)
					{
						flag = false;
						iNumErrors++;
					}
				}
				catch (Exception)
				{
					OnCompilerError("Assembly failed");
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x0005D758 File Offset: 0x0005B958
		private void OnInternalError(object kSender, InternalErrorEventArgs kArgs)
		{
			iNumErrors++;
			if (CompilerErrorHandler != null)
			{
				if (sFileStack.Count == 0)
				{
					CompilerErrorHandler(kSender, new CompilerErrorEventArgs(kArgs.ErrorText));
					return;
				}
				CompilerErrorHandler(kSender, new CompilerErrorEventArgs(kArgs.ErrorText, sFileStack.Peek(), kArgs.LineNumber, kArgs.ColumnNumber));
			}
		}

		// Token: 0x06000D43 RID: 3395 RVA: 0x0005D7D0 File Offset: 0x0005B9D0
		private void OnCompilerError(string asMessage)
		{
			iNumErrors++;
			if (CompilerErrorHandler != null)
			{
				if (sFileStack.Count > 0)
				{
					CompilerErrorHandler(this, new CompilerErrorEventArgs(asMessage, sFileStack.Peek()));
					return;
				}
				CompilerErrorHandler(this, new CompilerErrorEventArgs(asMessage));
			}
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x0005D834 File Offset: 0x0005BA34
		private void OnCompilerNotify(string asMessage)
		{
			if (CompilerNotifyHandler != null)
			{
				CompilerNotifyHandler(this, new CompilerNotifyEventArgs(asMessage));
			}
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x0005D850 File Offset: 0x0005BA50
		private void StartImportFile(string asFilename)
		{
			sFileStack.Push(asFilename);
			if (bDebug)
			{
				string arg = new string(' ', sFileStack.Count);
				OnCompilerNotify($"{arg}Starting import of {asFilename}...");
			}
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x0005D898 File Offset: 0x0005BA98
		private void FinishImportFile()
		{
			if (bDebug)
			{
				string arg = new string(' ', sFileStack.Count);
				OnCompilerNotify($"{arg}Finished import");
			}
			sFileStack.Pop();
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x0005D8E0 File Offset: 0x0005BAE0
		private void StartOptimizeFile(string asFilename)
		{
			sFileStack.Push(asFilename);
			if (bDebug)
			{
				string arg = new string(' ', sFileStack.Count);
				OnCompilerNotify($"{arg}Starting optimize of {asFilename}...");
			}
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x0005D928 File Offset: 0x0005BB28
		private void FinishOptimizeFile()
		{
			if (bDebug)
			{
				string arg = new string(' ', sFileStack.Count);
				OnCompilerNotify($"{arg}Finished optimize");
			}
			sFileStack.Pop();
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000D49 RID: 3401 RVA: 0x0005D970 File Offset: 0x0005BB70
		// (set) Token: 0x06000D4A RID: 3402 RVA: 0x0005D978 File Offset: 0x0005BB78
		public string OutputFolder
		{
			get
			{
				return sOutputFolder;
			}
			set
			{
				try
				{
					if (value != "")
					{
						sOutputFolder = Path.GetFullPath(value);
					}
					else
					{
						sOutputFolder = "";
					}
					bOutputValid = true;
				}
				catch (Exception ex)
				{
					OnCompilerNotify($"Cannot set output folder to \"{value}\" - {ex.Message}");
					bOutputValid = false;
				}
			}
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x0005D9EC File Offset: 0x0005BBEC
		private void ScanImportPath(string asPath)
		{
			string[] files = Directory.GetFiles(asPath, "*.psc");
			foreach (string text in files)
			{
				string key = Path.GetFileNameWithoutExtension(text).ToLower();
				if (!kObjectToPath.ContainsKey(key))
				{
					kObjectToPath.Add(key, text);
				}
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000D4C RID: 3404 RVA: 0x0005DA48 File Offset: 0x0005BC48
		// (set) Token: 0x06000D4D RID: 3405 RVA: 0x0005DA50 File Offset: 0x0005BC50
		public List<string> ImportFolders
		{
			get
			{
				return kImportFolders;
			}
			set
			{
				List<string> list = new List<string>(value);
				bImportValid = true;
				kObjectToPath = new Dictionary<string, string>();
				ScanImportPath(Environment.CurrentDirectory);
				int num = 0;
				while (num < list.Count && bImportValid)
				{
					try
					{
						if (list[num] != "")
						{
							list[num] = Path.GetFullPath(list[num]);
							ScanImportPath(list[num]);
						}
					}
					catch (Exception ex)
					{
						OnCompilerNotify($"Cannot use import folder \"{list[num]}\" - {ex.Message}");
						bImportValid = false;
					}
					num++;
				}
				if (bImportValid)
				{
					kImportFolders = list;
				}
			}
		}

		// Token: 0x040009C3 RID: 2499
		private const int uiMaxScriptnameLengthC = 38;

		// Token: 0x040009C6 RID: 2502
		public bool bDebug;

		// Token: 0x040009C7 RID: 2503
		public AssemblyOption eAsmOption;

		// Token: 0x040009C8 RID: 2504
		public bool bQuiet;

		// Token: 0x040009C9 RID: 2505
		private string sOutputFolder = "";

		// Token: 0x040009CA RID: 2506
		private bool bOutputValid = true;

		// Token: 0x040009CB RID: 2507
		private List<string> kImportFolders = new List<string>();

		// Token: 0x040009CC RID: 2508
		private bool bImportValid = true;

		// Token: 0x040009CD RID: 2509
		private Dictionary<string, string> kObjectToPath;

		// Token: 0x040009CE RID: 2510
		private Stack<string> sFileStack = new Stack<string>();

		// Token: 0x040009CF RID: 2511
		private int iNumErrors;

		// Token: 0x040009D0 RID: 2512
		private Dictionary<string, PapyrusFlag> kFlagDict = new Dictionary<string, PapyrusFlag>();

		// Token: 0x020001BF RID: 447
		public enum AssemblyOption
		{
			// Token: 0x040009D2 RID: 2514
			AssembleAndDelete,
			// Token: 0x040009D3 RID: 2515
			AssembleAndKeep,
			// Token: 0x040009D4 RID: 2516
			GenerateOnly,
			// Token: 0x040009D5 RID: 2517
			NoAssembly
		}
	}
}
