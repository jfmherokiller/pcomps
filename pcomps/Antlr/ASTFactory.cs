using System;
using System.Collections;
using pcomps.Antlr.collections;
using pcomps.Antlr.collections.impl;

namespace pcomps.Antlr
{
    // Token: 0x02000007 RID: 7
    public class ASTFactory
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002B10 File Offset: 0x00000D10
		public ASTFactory() : this(typeof(CommonAST))
		{
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002B30 File Offset: 0x00000D30
		public ASTFactory(string nodeTypeName) : this(loadNodeTypeObject(nodeTypeName))
		{
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002B4C File Offset: 0x00000D4C
		public ASTFactory(Type nodeType)
		{
			heteroList_ = new FactoryEntry[5];
			defaultASTNodeTypeObject_ = nodeType;
			defaultCreator_ = null;
			typename2creator_ = new Hashtable(32, 0.3f)
            {
                ["antlr.CommonAST"] = CommonAST.Creator,
                ["antlr.CommonASTWithHiddenTokens"] = CommonASTWithHiddenTokens.Creator
            };
        }

		// Token: 0x06000023 RID: 35 RVA: 0x00002BB8 File Offset: 0x00000DB8
		public void setTokenTypeASTNodeType(int tokenType, string NodeTypeName)
		{
			if (tokenType < 4)
			{
				throw new ANTLRException(
                    $"Internal parser error: Cannot change AST Node Type for Token ID '{tokenType}'");
			}
			if (tokenType > heteroList_.Length + 1)
			{
				setMaxNodeType(tokenType);
			}
			if (heteroList_[tokenType] == null)
			{
				heteroList_[tokenType] = new FactoryEntry(loadNodeTypeObject(NodeTypeName));
				return;
			}
			heteroList_[tokenType].NodeTypeObject = loadNodeTypeObject(NodeTypeName);
		}

        // Token: 0x06000025 RID: 37 RVA: 0x00002C44 File Offset: 0x00000E44
		public void setTokenTypeASTNodeCreator(int NodeType, ASTNodeCreator creator)
		{
			if (NodeType < 4)
			{
				throw new ANTLRException(
                    $"Internal parser error: Cannot change AST Node Type for Token ID '{NodeType}'");
			}
			if (NodeType > heteroList_.Length + 1)
			{
				setMaxNodeType(NodeType);
			}
			if (heteroList_[NodeType] == null)
			{
				heteroList_[NodeType] = new FactoryEntry(creator);
			}
			else
			{
				heteroList_[NodeType].Creator = creator;
			}
			typename2creator_[creator.ASTNodeTypeName] = creator;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002CC0 File Offset: 0x00000EC0
		public void setASTNodeCreator(ASTNodeCreator creator)
		{
			defaultCreator_ = creator;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002CD4 File Offset: 0x00000ED4
		public void setMaxNodeType(int NodeType)
		{
			if (heteroList_ == null)
			{
				heteroList_ = new FactoryEntry[NodeType + 1];
				return;
			}
			var num = heteroList_.Length;
			if (NodeType >= num)
			{
				var destinationArray = new FactoryEntry[NodeType + 1];
				Array.Copy(heteroList_, 0, destinationArray, 0, num);
				heteroList_ = destinationArray;
				return;
			}
			if (NodeType < num)
			{
				var destinationArray2 = new FactoryEntry[NodeType + 1];
				Array.Copy(heteroList_, 0, destinationArray2, 0, NodeType + 1);
				heteroList_ = destinationArray2;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002D4C File Offset: 0x00000F4C
		public virtual void addASTChild(ref ASTPair currentAST, AST child)
        {
            if (child == null) return;
            if (currentAST.root == null)
            {
                currentAST.root = child;
            }
            else if (currentAST.child == null)
            {
                currentAST.root.setFirstChild(child);
            }
            else
            {
                currentAST.child.setNextSibling(child);
            }
            currentAST.child = child;
            currentAST.advanceChildToEnd();
        }

		// Token: 0x06000029 RID: 41 RVA: 0x00002D9C File Offset: 0x00000F9C
		public virtual AST create()
        {
            var result = defaultCreator_ == null ? createFromNodeTypeObject(defaultASTNodeTypeObject_) : defaultCreator_.Create();
            return result;
        }

		// Token: 0x0600002A RID: 42 RVA: 0x00002DD0 File Offset: 0x00000FD0
		public virtual AST create(int type)
		{
			var ast = createFromNodeType(type);
			ast.initialize(type, "");
			return ast;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002DF4 File Offset: 0x00000FF4
		public virtual AST create(int type, string txt)
		{
			var ast = createFromNodeType(type);
			ast.initialize(type, txt);
			return ast;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002E14 File Offset: 0x00001014
		public virtual AST create(int type, string txt, string ASTNodeTypeName)
		{
			var ast = createFromNodeName(ASTNodeTypeName);
			ast.initialize(type, txt);
			return ast;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002E34 File Offset: 0x00001034
		public virtual AST create(IToken tok, string ASTNodeTypeName)
		{
			var ast = createFromNodeName(ASTNodeTypeName);
			ast.initialize(tok);
			return ast;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002E54 File Offset: 0x00001054
		public virtual AST create(AST aNode)
		{
			AST ast;
			if (aNode == null)
			{
				ast = null;
			}
			else
			{
				ast = createFromAST(aNode);
				ast.initialize(aNode);
			}
			return ast;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002E78 File Offset: 0x00001078
		public virtual AST create(IToken tok)
		{
			AST ast;
			if (tok == null)
			{
				ast = null;
			}
			else
			{
				ast = createFromNodeType(tok.Type);
				ast.initialize(tok);
			}
			return ast;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002EA4 File Offset: 0x000010A4
		public virtual AST dup(AST t)
		{
			if (t == null)
			{
				return null;
			}
			var ast = createFromAST(t);
			ast.initialize(t);
			return ast;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002EC8 File Offset: 0x000010C8
		public virtual AST dupList(AST t)
		{
			var ast = dupTree(t);
			var ast2 = ast;
			while (t != null)
			{
				t = t.getNextSibling();
				ast2.setNextSibling(dupTree(t));
				ast2 = ast2.getNextSibling();
			}
			return ast;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002F04 File Offset: 0x00001104
		public virtual AST dupTree(AST t)
		{
			var ast = dup(t);
			if (t != null)
			{
				ast.setFirstChild(dupList(t.getFirstChild()));
			}
			return ast;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002F30 File Offset: 0x00001130
		public virtual AST make(params AST[] nodes)
		{
			if (nodes == null || nodes.Length == 0)
			{
				return null;
			}
			var ast = nodes[0];
			AST ast2 = null;
            ast?.setFirstChild(null);
            for (var i = 1; i < nodes.Length; i++)
            {
                if (nodes[i] == null) continue;
                if (ast == null)
                {
                    ast2 = (ast = nodes[i]);
                }
                else if (ast2 == null)
                {
                    ast.setFirstChild(nodes[i]);
                    ast2 = ast.getFirstChild();
                }
                else
                {
                    ast2.setNextSibling(nodes[i]);
                    ast2 = ast2.getNextSibling();
                }
                while (ast2.getNextSibling() != null)
                {
                    ast2 = ast2.getNextSibling();
                }
            }
			return ast;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002FAC File Offset: 0x000011AC
		public virtual AST make(ASTArray nodes)
		{
			return make(nodes.array);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002FC8 File Offset: 0x000011C8
		public virtual void makeASTRoot(ref ASTPair currentAST, AST root)
        {
            if (root == null) return;
            root.addChild(currentAST.root);
            currentAST.child = currentAST.root;
            currentAST.advanceChildToEnd();
            currentAST.root = root;
        }

		// Token: 0x06000036 RID: 54 RVA: 0x00003000 File Offset: 0x00001200
		public virtual void setASTNodeType(string t)
		{
			if (defaultCreator_ != null && t != defaultCreator_.ASTNodeTypeName)
			{
				defaultCreator_ = null;
			}
			defaultASTNodeTypeObject_ = loadNodeTypeObject(t);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000303C File Offset: 0x0000123C
		public virtual void error(string e)
		{
			Console.Error.WriteLine(e);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003054 File Offset: 0x00001254
		private static Type loadNodeTypeObject(string nodeTypeName)
		{
			Type type = null;
			var flag = false;
			if (nodeTypeName != null)
			{
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						type = assembly.GetType(nodeTypeName);
                        if (type == null) continue;
                        flag = true;
                        break;
                    }
					catch
					{
						flag = false;
					}
				}
			}
			if (!flag)
			{
				throw new TypeLoadException($"Unable to load AST Node Type: '{nodeTypeName}'");
			}
			return type;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000030CC File Offset: 0x000012CC
		private AST createFromAST(AST node)
		{
			var type = node.GetType();
			var astnodeCreator = (ASTNodeCreator)typename2creator_[type.FullName ?? string.Empty];
			AST ast;
			if (astnodeCreator != null)
			{
				ast = astnodeCreator.Create();
				if (ast == null)
				{
					throw new ArgumentException($"Unable to create AST Node Type: '{type.FullName}'");
				}
			}
			else
			{
				ast = createFromNodeTypeObject(type);
			}
			return ast;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000312C File Offset: 0x0000132C
		private AST createFromNodeName(string nodeTypeName)
		{
			var astnodeCreator = (ASTNodeCreator)typename2creator_[nodeTypeName];
			AST ast;
			if (astnodeCreator != null)
			{
				ast = astnodeCreator.Create();
				if (ast == null)
				{
					throw new ArgumentException($"Unable to create AST Node Type: '{nodeTypeName}'");
				}
			}
			else
			{
				ast = createFromNodeTypeObject(loadNodeTypeObject(nodeTypeName));
			}
			return ast;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003180 File Offset: 0x00001380
		private AST createFromNodeType(int nodeTypeIndex)
		{
			var factoryEntry = heteroList_[nodeTypeIndex];
			AST result;
			if (factoryEntry is { Creator: { } })
			{
				result = factoryEntry.Creator.Create();
			}
			else if (factoryEntry?.NodeTypeObject == null)
            {
                result = defaultCreator_ == null ? createFromNodeTypeObject(defaultASTNodeTypeObject_) : defaultCreator_.Create();
            }
			else
			{
				result = createFromNodeTypeObject(factoryEntry.NodeTypeObject);
			}
			return result;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000031F0 File Offset: 0x000013F0
		private AST createFromNodeTypeObject(Type nodeTypeObject)
		{
			AST ast;
			try
			{
				ast = (AST)Activator.CreateInstance(nodeTypeObject);
				if (ast == null)
				{
					throw new ArgumentException($"Unable to create AST Node Type: '{nodeTypeObject.FullName}'");
				}
			}
			catch (Exception innerException)
			{
				throw new ArgumentException($"Unable to create AST Node Type: '{nodeTypeObject.FullName}'", innerException);
			}
			return ast;
		}

		// Token: 0x04000018 RID: 24
		protected Type defaultASTNodeTypeObject_;

		// Token: 0x04000019 RID: 25
		protected ASTNodeCreator defaultCreator_;

		// Token: 0x0400001A RID: 26
		protected FactoryEntry[] heteroList_;

		// Token: 0x0400001B RID: 27
		protected Hashtable typename2creator_;

		// Token: 0x02000008 RID: 8
		protected class FactoryEntry
		{
			// Token: 0x0600003D RID: 61 RVA: 0x0000325C File Offset: 0x0000145C
			public FactoryEntry(Type typeObj, ASTNodeCreator creator)
			{
				NodeTypeObject = typeObj;
				Creator = creator;
			}

			// Token: 0x0600003E RID: 62 RVA: 0x00003280 File Offset: 0x00001480
			public FactoryEntry(Type typeObj)
			{
				NodeTypeObject = typeObj;
			}

			// Token: 0x0600003F RID: 63 RVA: 0x0000329C File Offset: 0x0000149C
			public FactoryEntry(ASTNodeCreator creator)
			{
				Creator = creator;
			}

			// Token: 0x0400001C RID: 28
			public Type NodeTypeObject;

			// Token: 0x0400001D RID: 29
			public ASTNodeCreator Creator;
		}
	}
}
