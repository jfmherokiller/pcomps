using System.Collections;

namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000C4 RID: 196
	public class TreeWizard
	{
		// Token: 0x0600083C RID: 2108 RVA: 0x000173A8 File Offset: 0x000155A8
		public TreeWizard(ITreeAdaptor adaptor)
		{
			this.adaptor = adaptor;
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x000173B8 File Offset: 0x000155B8
		public TreeWizard(ITreeAdaptor adaptor, IDictionary tokenNameToTypeMap)
		{
			this.adaptor = adaptor;
			this.tokenNameToTypeMap = tokenNameToTypeMap;
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x000173D0 File Offset: 0x000155D0
		public TreeWizard(ITreeAdaptor adaptor, string[] tokenNames)
		{
			this.adaptor = adaptor;
			tokenNameToTypeMap = ComputeTokenTypes(tokenNames);
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x000173EC File Offset: 0x000155EC
		public TreeWizard(string[] tokenNames) : this(null, tokenNames)
		{
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x000173F8 File Offset: 0x000155F8
		public IDictionary ComputeTokenTypes(string[] tokenNames)
		{
			IDictionary dictionary = new Hashtable();
			if (tokenNames == null)
			{
				return dictionary;
			}
			for (var i = Token.MIN_TOKEN_TYPE; i < tokenNames.Length; i++)
			{
				var key = tokenNames[i];
				dictionary.Add(key, i);
			}
			return dictionary;
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x00017440 File Offset: 0x00015640
		public int GetTokenType(string tokenName)
		{
            var obj = tokenNameToTypeMap?[tokenName];
			if (obj != null)
			{
				return (int)obj;
			}
			return 0;
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x00017478 File Offset: 0x00015678
		public IDictionary Index(object t)
		{
			IDictionary dictionary = new Hashtable();
			_Index(t, dictionary);
			return dictionary;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x00017494 File Offset: 0x00015694
		protected void _Index(object t, IDictionary m)
		{
			if (t == null)
			{
				return;
			}
			var nodeType = adaptor.GetNodeType(t);
			var list = m[nodeType] as IList;
			if (list == null)
			{
				list = new ArrayList();
				m[nodeType] = list;
			}
			list.Add(t);
			var childCount = adaptor.GetChildCount(t);
			for (var i = 0; i < childCount; i++)
			{
				var child = adaptor.GetChild(t, i);
				_Index(child, m);
			}
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x00017520 File Offset: 0x00015720
		public IList Find(object t, int ttype)
		{
			IList list = new ArrayList();
			Visit(t, ttype, new RecordAllElementsVisitor(list));
			return list;
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x00017544 File Offset: 0x00015744
		public IList Find(object t, string pattern)
		{
			IList list = new ArrayList();
			var tokenizer = new TreePatternLexer(pattern);
			var treePatternParser = new TreePatternParser(tokenizer, this, new TreePatternTreeAdaptor());
			var treePattern = (TreePattern)treePatternParser.Pattern();
			if (treePattern == null || treePattern.IsNil || treePattern.GetType() == typeof(WildcardTreePattern))
			{
				return null;
			}
			var type = treePattern.Type;
			Visit(t, type, new PatternMatchingContextVisitor(this, treePattern, list));
			return list;
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x000175BC File Offset: 0x000157BC
		public object FindFirst(object t, int ttype)
		{
			return null;
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x000175C0 File Offset: 0x000157C0
		public object FindFirst(object t, string pattern)
		{
			return null;
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x000175C4 File Offset: 0x000157C4
		public void Visit(object t, int ttype, ContextVisitor visitor)
		{
			_Visit(t, null, 0, ttype, visitor);
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x000175D4 File Offset: 0x000157D4
		protected void _Visit(object t, object parent, int childIndex, int ttype, ContextVisitor visitor)
		{
			if (t == null)
			{
				return;
			}
			if (adaptor.GetNodeType(t) == ttype)
			{
				visitor.Visit(t, parent, childIndex, null);
			}
			var childCount = adaptor.GetChildCount(t);
			for (var i = 0; i < childCount; i++)
			{
				var child = adaptor.GetChild(t, i);
				_Visit(child, t, i, ttype, visitor);
			}
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x00017640 File Offset: 0x00015840
		public void Visit(object t, string pattern, ContextVisitor visitor)
		{
			var tokenizer = new TreePatternLexer(pattern);
			var treePatternParser = new TreePatternParser(tokenizer, this, new TreePatternTreeAdaptor());
			var treePattern = (TreePattern)treePatternParser.Pattern();
			if (treePattern == null || treePattern.IsNil || treePattern.GetType() == typeof(WildcardTreePattern))
			{
				return;
			}
			var type = treePattern.Type;
			Visit(t, type, new InvokeVisitorOnPatternMatchContextVisitor(this, treePattern, visitor));
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x000176AC File Offset: 0x000158AC
		public bool Parse(object t, string pattern, IDictionary labels)
		{
			var tokenizer = new TreePatternLexer(pattern);
			var treePatternParser = new TreePatternParser(tokenizer, this, new TreePatternTreeAdaptor());
			var t2 = (TreePattern)treePatternParser.Pattern();
			return _Parse(t, t2, labels);
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x000176E4 File Offset: 0x000158E4
		public bool Parse(object t, string pattern)
		{
			return Parse(t, pattern, null);
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x000176F0 File Offset: 0x000158F0
		protected bool _Parse(object t1, TreePattern t2, IDictionary labels)
		{
			if (t1 == null || t2 == null)
			{
				return false;
			}
			if (t2.GetType() != typeof(WildcardTreePattern))
			{
				if (adaptor.GetNodeType(t1) != t2.Type)
				{
					return false;
				}
				if (t2.hasTextArg && !adaptor.GetNodeText(t1).Equals(t2.Text))
				{
					return false;
				}
			}
			if (t2.label != null && labels != null)
			{
				labels[t2.label] = t1;
			}
			var childCount = adaptor.GetChildCount(t1);
			var childCount2 = t2.ChildCount;
			if (childCount != childCount2)
			{
				return false;
			}
			for (var i = 0; i < childCount; i++)
			{
				var child = adaptor.GetChild(t1, i);
				var t3 = (TreePattern)t2.GetChild(i);
				if (!_Parse(child, t3, labels))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x000177E0 File Offset: 0x000159E0
		public object Create(string pattern)
		{
			var tokenizer = new TreePatternLexer(pattern);
			var treePatternParser = new TreePatternParser(tokenizer, this, adaptor);
			return treePatternParser.Pattern();
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0001780C File Offset: 0x00015A0C
		public static bool Equals(object t1, object t2, ITreeAdaptor adaptor)
		{
			return _Equals(t1, t2, adaptor);
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x00017818 File Offset: 0x00015A18
		public bool Equals(object t1, object t2)
		{
			return _Equals(t1, t2, adaptor);
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x00017828 File Offset: 0x00015A28
		protected static bool _Equals(object t1, object t2, ITreeAdaptor adaptor)
		{
			if (t1 == null || t2 == null)
			{
				return false;
			}
			if (adaptor.GetNodeType(t1) != adaptor.GetNodeType(t2))
			{
				return false;
			}
			if (!adaptor.GetNodeText(t1).Equals(adaptor.GetNodeText(t2)))
			{
				return false;
			}
			var childCount = adaptor.GetChildCount(t1);
			var childCount2 = adaptor.GetChildCount(t2);
			if (childCount != childCount2)
			{
				return false;
			}
			for (var i = 0; i < childCount; i++)
			{
				var child = adaptor.GetChild(t1, i);
				var child2 = adaptor.GetChild(t2, i);
				if (!_Equals(child, child2, adaptor))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400021C RID: 540
		protected ITreeAdaptor adaptor;

		// Token: 0x0400021D RID: 541
		protected IDictionary tokenNameToTypeMap;

		// Token: 0x020000C5 RID: 197
		public interface ContextVisitor
		{
			// Token: 0x06000852 RID: 2130
			void Visit(object t, object parent, int childIndex, IDictionary labels);
		}

		// Token: 0x020000C6 RID: 198
		public abstract class Visitor : ContextVisitor
		{
			// Token: 0x06000854 RID: 2132 RVA: 0x000178CC File Offset: 0x00015ACC
			public void Visit(object t, object parent, int childIndex, IDictionary labels)
			{
				Visit(t);
			}

			// Token: 0x06000855 RID: 2133
			public abstract void Visit(object t);
		}

		// Token: 0x020000C7 RID: 199
		private sealed class RecordAllElementsVisitor : Visitor
		{
			// Token: 0x06000856 RID: 2134 RVA: 0x000178D8 File Offset: 0x00015AD8
			public RecordAllElementsVisitor(IList list)
			{
				this.list = list;
			}

			// Token: 0x06000857 RID: 2135 RVA: 0x000178E8 File Offset: 0x00015AE8
			public override void Visit(object t)
			{
				list.Add(t);
			}

			// Token: 0x0400021E RID: 542
			private IList list;
		}

		// Token: 0x020000C8 RID: 200
		private sealed class PatternMatchingContextVisitor : ContextVisitor
		{
			// Token: 0x06000858 RID: 2136 RVA: 0x000178F8 File Offset: 0x00015AF8
			public PatternMatchingContextVisitor(TreeWizard owner, TreePattern pattern, IList list)
			{
				this.owner = owner;
				this.pattern = pattern;
				this.list = list;
			}

			// Token: 0x06000859 RID: 2137 RVA: 0x00017918 File Offset: 0x00015B18
			public void Visit(object t, object parent, int childIndex, IDictionary labels)
			{
				if (owner._Parse(t, pattern, null))
				{
					list.Add(t);
				}
			}

			// Token: 0x0400021F RID: 543
			private TreeWizard owner;

			// Token: 0x04000220 RID: 544
			private TreePattern pattern;

			// Token: 0x04000221 RID: 545
			private IList list;
		}

		// Token: 0x020000C9 RID: 201
		private sealed class InvokeVisitorOnPatternMatchContextVisitor : ContextVisitor
		{
			// Token: 0x0600085A RID: 2138 RVA: 0x00017940 File Offset: 0x00015B40
			public InvokeVisitorOnPatternMatchContextVisitor(TreeWizard owner, TreePattern pattern, ContextVisitor visitor)
			{
				this.owner = owner;
				this.pattern = pattern;
				this.visitor = visitor;
			}

			// Token: 0x0600085B RID: 2139 RVA: 0x00017974 File Offset: 0x00015B74
			public void Visit(object t, object parent, int childIndex, IDictionary unusedlabels)
			{
				labels.Clear();
				if (owner._Parse(t, pattern, labels))
				{
					visitor.Visit(t, parent, childIndex, labels);
				}
			}

			// Token: 0x04000222 RID: 546
			private TreeWizard owner;

			// Token: 0x04000223 RID: 547
			private TreePattern pattern;

			// Token: 0x04000224 RID: 548
			private ContextVisitor visitor;

			// Token: 0x04000225 RID: 549
			private Hashtable labels = new Hashtable();
		}

		// Token: 0x020000CA RID: 202
		public class TreePattern : CommonTree
		{
			// Token: 0x0600085C RID: 2140 RVA: 0x000179C0 File Offset: 0x00015BC0
			public TreePattern(IToken payload) : base(payload)
			{
			}

			// Token: 0x0600085D RID: 2141 RVA: 0x000179CC File Offset: 0x00015BCC
			public override string ToString()
			{
				if (label != null)
				{
					return $"%{label}:{base.ToString()}";
				}
				return base.ToString();
			}

			// Token: 0x04000226 RID: 550
			public string label;

			// Token: 0x04000227 RID: 551
			public bool hasTextArg;
		}

		// Token: 0x020000CB RID: 203
		public class WildcardTreePattern : TreePattern
		{
			// Token: 0x0600085E RID: 2142 RVA: 0x000179FC File Offset: 0x00015BFC
			public WildcardTreePattern(IToken payload) : base(payload)
			{
			}
		}

		// Token: 0x020000CC RID: 204
		public class TreePatternTreeAdaptor : CommonTreeAdaptor
		{
			// Token: 0x06000860 RID: 2144 RVA: 0x00017A10 File Offset: 0x00015C10
			public override object Create(IToken payload)
			{
				return new TreePattern(payload);
			}
		}
	}
}
