using System;
using System.Collections;
using System.Reflection;
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
		public ASTFactory(string nodeTypeName) : this(ASTFactory.loadNodeTypeObject(nodeTypeName))
		{
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002B4C File Offset: 0x00000D4C
		public ASTFactory(Type nodeType)
		{
			this.heteroList_ = new ASTFactory.FactoryEntry[5];
			this.defaultASTNodeTypeObject_ = nodeType;
			this.defaultCreator_ = null;
			this.typename2creator_ = new Hashtable(32, 0.3f);
			this.typename2creator_["antlr.CommonAST"] = CommonAST.Creator;
			this.typename2creator_["antlr.CommonASTWithHiddenTokens"] = CommonASTWithHiddenTokens.Creator;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002BB8 File Offset: 0x00000DB8
		public void setTokenTypeASTNodeType(int tokenType, string NodeTypeName)
		{
			if (tokenType < 4)
			{
				throw new ANTLRException(
                    $"Internal parser error: Cannot change AST Node Type for Token ID '{tokenType}'");
			}
			if (tokenType > this.heteroList_.Length + 1)
			{
				this.setMaxNodeType(tokenType);
			}
			if (this.heteroList_[tokenType] == null)
			{
				this.heteroList_[tokenType] = new ASTFactory.FactoryEntry(ASTFactory.loadNodeTypeObject(NodeTypeName));
				return;
			}
			this.heteroList_[tokenType].NodeTypeObject = ASTFactory.loadNodeTypeObject(NodeTypeName);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002C2C File Offset: 0x00000E2C
		[Obsolete("Replaced by setTokenTypeASTNodeType(int, string) since version 2.7.2.6", true)]
		public void registerFactory(int NodeType, string NodeTypeName)
		{
			this.setTokenTypeASTNodeType(NodeType, NodeTypeName);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002C44 File Offset: 0x00000E44
		public void setTokenTypeASTNodeCreator(int NodeType, ASTNodeCreator creator)
		{
			if (NodeType < 4)
			{
				throw new ANTLRException(
                    $"Internal parser error: Cannot change AST Node Type for Token ID '{NodeType}'");
			}
			if (NodeType > this.heteroList_.Length + 1)
			{
				this.setMaxNodeType(NodeType);
			}
			if (this.heteroList_[NodeType] == null)
			{
				this.heteroList_[NodeType] = new ASTFactory.FactoryEntry(creator);
			}
			else
			{
				this.heteroList_[NodeType].Creator = creator;
			}
			this.typename2creator_[creator.ASTNodeTypeName] = creator;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002CC0 File Offset: 0x00000EC0
		public void setASTNodeCreator(ASTNodeCreator creator)
		{
			this.defaultCreator_ = creator;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002CD4 File Offset: 0x00000ED4
		public void setMaxNodeType(int NodeType)
		{
			if (this.heteroList_ == null)
			{
				this.heteroList_ = new ASTFactory.FactoryEntry[NodeType + 1];
				return;
			}
			int num = this.heteroList_.Length;
			if (NodeType >= num)
			{
				ASTFactory.FactoryEntry[] destinationArray = new ASTFactory.FactoryEntry[NodeType + 1];
				Array.Copy(this.heteroList_, 0, destinationArray, 0, num);
				this.heteroList_ = destinationArray;
				return;
			}
			if (NodeType < num)
			{
				ASTFactory.FactoryEntry[] destinationArray2 = new ASTFactory.FactoryEntry[NodeType + 1];
				Array.Copy(this.heteroList_, 0, destinationArray2, 0, NodeType + 1);
				this.heteroList_ = destinationArray2;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002D4C File Offset: 0x00000F4C
		public virtual void addASTChild(ref ASTPair currentAST, AST child)
		{
			if (child != null)
			{
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
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002D9C File Offset: 0x00000F9C
		public virtual AST create()
		{
			AST result;
			if (this.defaultCreator_ == null)
			{
				result = this.createFromNodeTypeObject(this.defaultASTNodeTypeObject_);
			}
			else
			{
				result = this.defaultCreator_.Create();
			}
			return result;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002DD0 File Offset: 0x00000FD0
		public virtual AST create(int type)
		{
			AST ast = this.createFromNodeType(type);
			ast.initialize(type, "");
			return ast;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002DF4 File Offset: 0x00000FF4
		public virtual AST create(int type, string txt)
		{
			AST ast = this.createFromNodeType(type);
			ast.initialize(type, txt);
			return ast;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002E14 File Offset: 0x00001014
		public virtual AST create(int type, string txt, string ASTNodeTypeName)
		{
			AST ast = this.createFromNodeName(ASTNodeTypeName);
			ast.initialize(type, txt);
			return ast;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002E34 File Offset: 0x00001034
		public virtual AST create(IToken tok, string ASTNodeTypeName)
		{
			AST ast = this.createFromNodeName(ASTNodeTypeName);
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
				ast = this.createFromAST(aNode);
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
				ast = this.createFromNodeType(tok.Type);
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
			AST ast = this.createFromAST(t);
			ast.initialize(t);
			return ast;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002EC8 File Offset: 0x000010C8
		public virtual AST dupList(AST t)
		{
			AST ast = this.dupTree(t);
			AST ast2 = ast;
			while (t != null)
			{
				t = t.getNextSibling();
				ast2.setNextSibling(this.dupTree(t));
				ast2 = ast2.getNextSibling();
			}
			return ast;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002F04 File Offset: 0x00001104
		public virtual AST dupTree(AST t)
		{
			AST ast = this.dup(t);
			if (t != null)
			{
				ast.setFirstChild(this.dupList(t.getFirstChild()));
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
			AST ast = nodes[0];
			AST ast2 = null;
			if (ast != null)
			{
				ast.setFirstChild(null);
			}
			for (int i = 1; i < nodes.Length; i++)
			{
				if (nodes[i] != null)
				{
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
			}
			return ast;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002FAC File Offset: 0x000011AC
		public virtual AST make(ASTArray nodes)
		{
			return this.make(nodes.array);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002FC8 File Offset: 0x000011C8
		public virtual void makeASTRoot(ref ASTPair currentAST, AST root)
		{
			if (root != null)
			{
				root.addChild(currentAST.root);
				currentAST.child = currentAST.root;
				currentAST.advanceChildToEnd();
				currentAST.root = root;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003000 File Offset: 0x00001200
		public virtual void setASTNodeType(string t)
		{
			if (this.defaultCreator_ != null && t != this.defaultCreator_.ASTNodeTypeName)
			{
				this.defaultCreator_ = null;
			}
			this.defaultASTNodeTypeObject_ = ASTFactory.loadNodeTypeObject(t);
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
			bool flag = false;
			if (nodeTypeName != null)
			{
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						type = assembly.GetType(nodeTypeName);
						if (type != null)
						{
							flag = true;
							break;
						}
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
			Type type = node.GetType();
			ASTNodeCreator astnodeCreator = (ASTNodeCreator)this.typename2creator_[type.FullName];
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
				ast = this.createFromNodeTypeObject(type);
			}
			return ast;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000312C File Offset: 0x0000132C
		private AST createFromNodeName(string nodeTypeName)
		{
			ASTNodeCreator astnodeCreator = (ASTNodeCreator)this.typename2creator_[nodeTypeName];
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
				ast = this.createFromNodeTypeObject(ASTFactory.loadNodeTypeObject(nodeTypeName));
			}
			return ast;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003180 File Offset: 0x00001380
		private AST createFromNodeType(int nodeTypeIndex)
		{
			ASTFactory.FactoryEntry factoryEntry = this.heteroList_[nodeTypeIndex];
			AST result;
			if (factoryEntry != null && factoryEntry.Creator != null)
			{
				result = factoryEntry.Creator.Create();
			}
			else if (factoryEntry == null || factoryEntry.NodeTypeObject == null)
			{
				if (this.defaultCreator_ == null)
				{
					result = this.createFromNodeTypeObject(this.defaultASTNodeTypeObject_);
				}
				else
				{
					result = this.defaultCreator_.Create();
				}
			}
			else
			{
				result = this.createFromNodeTypeObject(factoryEntry.NodeTypeObject);
			}
			return result;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000031F0 File Offset: 0x000013F0
		private AST createFromNodeTypeObject(Type nodeTypeObject)
		{
			AST ast = null;
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
		protected ASTFactory.FactoryEntry[] heteroList_;

		// Token: 0x0400001B RID: 27
		protected Hashtable typename2creator_;

		// Token: 0x02000008 RID: 8
		protected class FactoryEntry
		{
			// Token: 0x0600003D RID: 61 RVA: 0x0000325C File Offset: 0x0000145C
			public FactoryEntry(Type typeObj, ASTNodeCreator creator)
			{
				this.NodeTypeObject = typeObj;
				this.Creator = creator;
			}

			// Token: 0x0600003E RID: 62 RVA: 0x00003280 File Offset: 0x00001480
			public FactoryEntry(Type typeObj)
			{
				this.NodeTypeObject = typeObj;
			}

			// Token: 0x0600003F RID: 63 RVA: 0x0000329C File Offset: 0x0000149C
			public FactoryEntry(ASTNodeCreator creator)
			{
				this.Creator = creator;
			}

			// Token: 0x0400001C RID: 28
			public Type NodeTypeObject;

			// Token: 0x0400001D RID: 29
			public ASTNodeCreator Creator;
		}
	}
}
