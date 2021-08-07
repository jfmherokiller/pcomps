using System.Collections.Generic;
using pcomps.Antlr.Runtime;
using pcomps.Antlr.Runtime.Tree;

namespace pcomps.PCompiler
{
	// Token: 0x020001C0 RID: 448
	public class ScriptObjectType
	{
		// Token: 0x06000D4F RID: 3407 RVA: 0x0005DB6C File Offset: 0x0005BD6C
		public ScriptObjectType(string asName)
		{
			sName = asName.ToLowerInvariant();
			kNonOverridableFunctions = new List<string>();
			kNonOverridableFunctions.Add("getstate");
			kNonOverridableFunctions.Add("gotostate");
			var dictionary = new Dictionary<string, ScriptFunctionType>
            { { "getstate", new ScriptFunctionType("getstate", asName, "", "")
            {
                kRetType = new ScriptVariableType("string")
            } } };
            var scriptFunctionType = new ScriptFunctionType("gotostate", asName, "", "");
			scriptFunctionType.TryAddParam("newstate", new ScriptVariableType("string"));
			dictionary.Add("gotostate", scriptFunctionType);
			kStateFunctions.Add("", dictionary);
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000D50 RID: 3408 RVA: 0x0005DC64 File Offset: 0x0005BE64
		public string Name => sName;

        // Token: 0x06000D51 RID: 3409 RVA: 0x0005DC6C File Offset: 0x0005BE6C
		public bool IsChildOf(string asParentName)
		{
			var scriptObjectType = kParent;
			var flag = false;
			while (scriptObjectType != null && !flag)
			{
				if (scriptObjectType.Name == asParentName.ToLowerInvariant())
				{
					flag = true;
				}
				scriptObjectType = scriptObjectType.kParent;
			}
			return flag;
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x0005DCA8 File Offset: 0x0005BEA8
		public bool TryGetVariable(string asName, out ScriptVariableType akType)
		{
			var text = asName.ToLowerInvariant();
			bool result;
			if (text == "self")
			{
				result = true;
				akType = new ScriptVariableType(Name);
			}
			else
			{
				result = kInstanceVariables.TryGetValue(text, out akType);
			}
			return result;
		}

		// Token: 0x06000D53 RID: 3411 RVA: 0x0005DCEC File Offset: 0x0005BEEC
		public bool TrySetVariable(string asName, ScriptVariableType akType)
		{
			var flag = kInstanceVariables.ContainsKey(asName.ToLowerInvariant());
			if (!flag)
			{
				kInstanceVariables.Add(asName.ToLowerInvariant(), akType);
			}
			return !flag;
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x0005DD24 File Offset: 0x0005BF24
		public bool TryGetProperty(string asName, out ScriptPropertyType akType) => kProperties.TryGetValue(asName.ToLowerInvariant(), out akType);

        // Token: 0x06000D55 RID: 3413 RVA: 0x0005DD38 File Offset: 0x0005BF38
		public bool TrySetProperty(string asName, ScriptVariableType akType)
		{
			var flag = kProperties.ContainsKey(asName.ToLowerInvariant());
			if (!flag)
			{
				kProperties.Add(asName.ToLowerInvariant(), new ScriptPropertyType(akType));
			}
			return !flag;
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x0005DD78 File Offset: 0x0005BF78
		public bool TryGetFunction(string asName, out ScriptFunctionType akType) => TryGetFunction("", asName, out akType);

        // Token: 0x06000D57 RID: 3415 RVA: 0x0005DD88 File Offset: 0x0005BF88
		public bool TryGetFunction(string asStateName, string asName, out ScriptFunctionType akType)
		{
			akType = null;
            var flag = kStateFunctions.TryGetValue(asStateName.ToLowerInvariant(), out var dictionary);
			return flag && dictionary.TryGetValue(asName.ToLowerInvariant(), out akType);
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x0005DDC0 File Offset: 0x0005BFC0
		public bool TrySetFunction(string asName, ScriptFunctionType akType)
		{
			return TrySetFunction("", asName, akType);
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x0005DDD0 File Offset: 0x0005BFD0
		public bool TrySetFunction(string asStateName, string asName, ScriptFunctionType akType)
		{
			var flag = false;
            if (kStateFunctions.TryGetValue(asStateName.ToLowerInvariant(), out var dictionary))
			{
				if (!dictionary.ContainsKey(asName.ToLowerInvariant()))
				{
					dictionary.Add(asName.ToLowerInvariant(), akType);
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				dictionary = new Dictionary<string, ScriptFunctionType> { { asName.ToLowerInvariant(), akType } };
                kStateFunctions.Add(asStateName.ToLowerInvariant(), dictionary);
			}
			return !flag;
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000D5A RID: 3418 RVA: 0x0005DE40 File Offset: 0x0005C040
		public Dictionary<string, Dictionary<string, ScriptFunctionType>> StateFunctions => kStateFunctions;

        // Token: 0x040009D6 RID: 2518
		public readonly List<string> kNonOverridableFunctions;

		// Token: 0x040009D7 RID: 2519
		public CommonTree kAST;

		// Token: 0x040009D8 RID: 2520
		public ITokenStream kTokenStream;

		// Token: 0x040009D9 RID: 2521
		public ScriptObjectType kParent;

		// Token: 0x040009DA RID: 2522
		private string sName;

		// Token: 0x040009DB RID: 2523
		private Dictionary<string, ScriptVariableType> kInstanceVariables = new();

		// Token: 0x040009DC RID: 2524
		private Dictionary<string, ScriptPropertyType> kProperties = new();

		// Token: 0x040009DD RID: 2525
		public string sAutoState = "";

		// Token: 0x040009DE RID: 2526
		private Dictionary<string, Dictionary<string, ScriptFunctionType>> kStateFunctions = new();
	}
}
