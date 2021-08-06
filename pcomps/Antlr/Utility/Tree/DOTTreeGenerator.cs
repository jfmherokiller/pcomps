using System.Collections;
using pcomps.Antlr.Runtime.Tree;

namespace pcomps.Antlr.Utility.Tree
{
	// Token: 0x020000EA RID: 234
	public class DOTTreeGenerator
	{
		// Token: 0x06000988 RID: 2440 RVA: 0x0001B9B8 File Offset: 0x00019BB8
		public StringTemplate.StringTemplate ToDOT(object tree, ITreeAdaptor adaptor, StringTemplate.StringTemplate _treeST, StringTemplate.StringTemplate _edgeST)
		{
			StringTemplate.StringTemplate instanceOf = _treeST.GetInstanceOf();
			nodeNumber = 0;
			ToDOTDefineNodes(tree, adaptor, instanceOf);
			nodeNumber = 0;
			ToDOTDefineEdges(tree, adaptor, instanceOf);
			return instanceOf;
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0001B9F0 File Offset: 0x00019BF0
		public StringTemplate.StringTemplate ToDOT(object tree, ITreeAdaptor adaptor)
		{
			return ToDOT(tree, adaptor, _treeST, _edgeST);
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x0001BA04 File Offset: 0x00019C04
		public StringTemplate.StringTemplate ToDOT(ITree tree)
		{
			return ToDOT(tree, new CommonTreeAdaptor());
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0001BA14 File Offset: 0x00019C14
		protected void ToDOTDefineNodes(object tree, ITreeAdaptor adaptor, StringTemplate.StringTemplate treeST)
		{
			if (tree == null)
			{
				return;
			}
			int childCount = adaptor.GetChildCount(tree);
			if (childCount == 0)
			{
				return;
			}
			StringTemplate.StringTemplate nodeST = GetNodeST(adaptor, tree);
			treeST.SetAttribute("nodes", nodeST);
			for (int i = 0; i < childCount; i++)
			{
				object child = adaptor.GetChild(tree, i);
				StringTemplate.StringTemplate nodeST2 = GetNodeST(adaptor, child);
				treeST.SetAttribute("nodes", nodeST2);
				ToDOTDefineNodes(child, adaptor, treeST);
			}
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x0001BA88 File Offset: 0x00019C88
		protected void ToDOTDefineEdges(object tree, ITreeAdaptor adaptor, StringTemplate.StringTemplate treeST)
		{
			if (tree == null)
			{
				return;
			}
			int childCount = adaptor.GetChildCount(tree);
			if (childCount == 0)
			{
				return;
			}
			string val = $"n{GetNodeNumber(tree)}";
			string nodeText = adaptor.GetNodeText(tree);
			for (int i = 0; i < childCount; i++)
			{
				object child = adaptor.GetChild(tree, i);
				string nodeText2 = adaptor.GetNodeText(child);
				string val2 = $"n{GetNodeNumber(child)}";
				StringTemplate.StringTemplate instanceOf = _edgeST.GetInstanceOf();
				instanceOf.SetAttribute("parent", val);
				instanceOf.SetAttribute("child", val2);
				instanceOf.SetAttribute("parentText", nodeText);
				instanceOf.SetAttribute("childText", nodeText2);
				treeST.SetAttribute("edges", instanceOf);
				ToDOTDefineEdges(child, adaptor, treeST);
			}
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0001BB64 File Offset: 0x00019D64
		protected StringTemplate.StringTemplate GetNodeST(ITreeAdaptor adaptor, object t)
		{
			string text = adaptor.GetNodeText(t);
			StringTemplate.StringTemplate instanceOf = _nodeST.GetInstanceOf();
			string val = $"n{GetNodeNumber(t)}";
			instanceOf.SetAttribute("name", val);
			if (text != null)
			{
				text = text.Replace("\"", "\\\\\"");
			}
			instanceOf.SetAttribute("text", text);
			return instanceOf;
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x0001BBCC File Offset: 0x00019DCC
		protected int GetNodeNumber(object t)
		{
			object obj = nodeToNumberMap[t];
			if (obj != null)
			{
				return (int)obj;
			}
			nodeToNumberMap[t] = nodeNumber;
			nodeNumber++;
			return nodeNumber - 1;
		}

		// Token: 0x0400028A RID: 650
		public static StringTemplate.StringTemplate _treeST = new StringTemplate.StringTemplate("digraph {\n  ordering=out;\n  ranksep=.4;\n  node [shape=plaintext, fixedsize=true, fontsize=11, fontname=\"Courier\",\n        width=.25, height=.25];\n  edge [arrowsize=.5]\n  $nodes$\n  $edges$\n}\n");

		// Token: 0x0400028B RID: 651
		public static StringTemplate.StringTemplate _nodeST = new StringTemplate.StringTemplate("$name$ [label=\"$text$\"];\n");

		// Token: 0x0400028C RID: 652
		public static StringTemplate.StringTemplate _edgeST = new StringTemplate.StringTemplate("$parent$ -> $child$ // \"$parentText$\" -> \"$childText$\"\n");

		// Token: 0x0400028D RID: 653
		private IDictionary nodeToNumberMap = new Hashtable();

		// Token: 0x0400028E RID: 654
		private int nodeNumber = 0;
	}
}
