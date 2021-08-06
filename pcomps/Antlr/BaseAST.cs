using System;
using System.Collections;
using System.IO;
using System.Text;
using pcomps.Antlr.collections;

namespace pcomps.Antlr
{
	// Token: 0x0200000E RID: 14
	[Serializable]
	public abstract class BaseAST : AST, ICloneable
	{
		// Token: 0x0600007E RID: 126 RVA: 0x0000359C File Offset: 0x0000179C
		public virtual void addChild(AST node)
		{
			if (node == null)
			{
				return;
			}
			var baseAST = down;
			if (baseAST != null)
			{
				while (baseAST.right != null)
				{
					baseAST = baseAST.right;
				}
				baseAST.right = (BaseAST)node;
				return;
			}
			down = (BaseAST)node;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000035E4 File Offset: 0x000017E4
		private void doWorkForFindAll(ArrayList v, AST target, bool partialMatch)
		{
			for (AST ast = this; ast != null; ast = ast.getNextSibling())
			{
				if ((partialMatch && ast.EqualsTreePartial(target)) || (!partialMatch && ast.EqualsTree(target)))
				{
					v.Add(ast);
				}
				if (ast.getFirstChild() != null)
				{
					((BaseAST)ast.getFirstChild()).doWorkForFindAll(v, target, partialMatch);
				}
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000363C File Offset: 0x0000183C
		public override bool Equals(object obj)
		{
			return obj != null && GetType() == obj.GetType() && Equals((AST)obj);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000366C File Offset: 0x0000186C
		public virtual bool Equals(AST t)
		{
			return t != null && Equals(getText(), t.getText()) && Type == t.Type;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000036A4 File Offset: 0x000018A4
		public virtual bool EqualsList(AST t)
		{
			if (t == null)
			{
				return false;
			}
			AST ast = this;
			while (ast != null && t != null)
			{
				if (!ast.Equals(t))
				{
					return false;
				}
				if (ast.getFirstChild() != null)
				{
					if (!ast.getFirstChild().EqualsList(t.getFirstChild()))
					{
						return false;
					}
				}
				else if (t.getFirstChild() != null)
				{
					return false;
				}
				ast = ast.getNextSibling();
				t = t.getNextSibling();
			}
			return ast == null && t == null;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000370C File Offset: 0x0000190C
		public virtual bool EqualsListPartial(AST sub)
		{
			if (sub == null)
			{
				return true;
			}
			AST ast = this;
			while (ast != null && sub != null)
			{
				if (!ast.Equals(sub))
				{
					return false;
				}
				if (ast.getFirstChild() != null && !ast.getFirstChild().EqualsListPartial(sub.getFirstChild()))
				{
					return false;
				}
				ast = ast.getNextSibling();
				sub = sub.getNextSibling();
			}
			return ast != null || sub == null;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003768 File Offset: 0x00001968
		public virtual bool EqualsTree(AST t)
		{
			if (!Equals(t))
			{
				return false;
			}
			if (getFirstChild() != null)
			{
				if (!getFirstChild().EqualsList(t.getFirstChild()))
				{
					return false;
				}
			}
			else if (t.getFirstChild() != null)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000037A8 File Offset: 0x000019A8
		public virtual bool EqualsTreePartial(AST sub)
		{
			return sub == null || (Equals(sub) && (getFirstChild() == null || getFirstChild().EqualsListPartial(sub.getFirstChild())));
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000037E4 File Offset: 0x000019E4
		public virtual IEnumerator findAll(AST target)
		{
			var arrayList = new ArrayList(10);
			if (target == null)
			{
				return null;
			}
			doWorkForFindAll(arrayList, target, false);
			return arrayList.GetEnumerator();
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003810 File Offset: 0x00001A10
		public virtual IEnumerator findAllPartial(AST sub)
		{
			var arrayList = new ArrayList(10);
			if (sub == null)
			{
				return null;
			}
			doWorkForFindAll(arrayList, sub, true);
			return arrayList.GetEnumerator();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000383C File Offset: 0x00001A3C
		public virtual AST getFirstChild()
		{
			return down;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00003850 File Offset: 0x00001A50
		public virtual AST getNextSibling()
		{
			return right;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003864 File Offset: 0x00001A64
		public virtual int getLine()
		{
			return 0;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003874 File Offset: 0x00001A74
		public virtual int getColumn()
		{
			return 0;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00003884 File Offset: 0x00001A84
		public virtual string getText()
		{
			return "";
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00003898 File Offset: 0x00001A98
		// (set) Token: 0x0600008E RID: 142 RVA: 0x000038A8 File Offset: 0x00001AA8
		public virtual int Type
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000038B8 File Offset: 0x00001AB8
		public int getNumberOfChildren()
		{
			var baseAST = down;
			var num = 0;
			if (baseAST != null)
			{
				num = 1;
				while (baseAST.right != null)
				{
					baseAST = baseAST.right;
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000090 RID: 144
		public abstract void initialize(int t, string txt);

		// Token: 0x06000091 RID: 145
		public abstract void initialize(AST t);

		// Token: 0x06000092 RID: 146
		public abstract void initialize(IToken t);

		// Token: 0x06000093 RID: 147 RVA: 0x000038EC File Offset: 0x00001AEC
		public virtual void removeChildren()
		{
			down = null;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003900 File Offset: 0x00001B00
		public virtual void setFirstChild(AST c)
		{
			down = (BaseAST)c;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000391C File Offset: 0x00001B1C
		public virtual void setNextSibling(AST n)
		{
			right = (BaseAST)n;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003938 File Offset: 0x00001B38
		public virtual void setText(string text)
		{
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003948 File Offset: 0x00001B48
		public virtual void setType(int ttype)
		{
			Type = ttype;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000395C File Offset: 0x00001B5C
		public static void setVerboseStringConversion(bool verbose, string[] names)
		{
			verboseStringConversion = verbose;
			tokenNames = names;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00003978 File Offset: 0x00001B78
		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			if (verboseStringConversion && string.Compare(getText(), tokenNames[Type], true) != 0 && string.Compare(getText(), StringUtils.stripFrontBack(tokenNames[Type], "\"", "\""), true) != 0)
			{
				stringBuilder.Append('[');
				stringBuilder.Append(getText());
				stringBuilder.Append(",<");
				stringBuilder.Append(tokenNames[Type]);
				stringBuilder.Append(">]");
				return stringBuilder.ToString();
			}
			return getText();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003A28 File Offset: 0x00001C28
		public virtual string ToStringList()
		{
			var text = "";
			if (((AST)this).getFirstChild() != null)
			{
				text += " (";
			}
			text = $"{text} {this}";
			if (((AST)this).getFirstChild() != null)
			{
				text += ((BaseAST)((AST)this).getFirstChild()).ToStringList();
			}
			if (((AST)this).getFirstChild() != null)
			{
				text += " )";
			}
			if (((AST)this).getNextSibling() != null)
			{
				text += ((BaseAST)((AST)this).getNextSibling()).ToStringList();
			}
			return text;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00003AB8 File Offset: 0x00001CB8
		public virtual string ToStringTree()
		{
			var text = "";
			if (((AST)this).getFirstChild() != null)
			{
				text += " (";
			}
			text = $"{text} {this}";
			if (((AST)this).getFirstChild() != null)
			{
				text += ((BaseAST)((AST)this).getFirstChild()).ToStringList();
			}
			if (((AST)this).getFirstChild() != null)
			{
				text += " )";
			}
			return text;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00003B28 File Offset: 0x00001D28
		public virtual string ToTree()
		{
			return ToTree(string.Empty);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00003B40 File Offset: 0x00001D40
		public virtual string ToTree(string prefix)
		{
			var stringBuilder = new StringBuilder(prefix);
			if (getNextSibling() == null)
			{
				stringBuilder.Append("+--");
			}
			else
			{
				stringBuilder.Append("|--");
			}
			stringBuilder.Append(ToString());
			stringBuilder.Append(Environment.NewLine);
			if (getFirstChild() != null)
			{
				if (getNextSibling() == null)
				{
					stringBuilder.Append(((BaseAST)getFirstChild()).ToTree($"{prefix}   "));
				}
				else
				{
					stringBuilder.Append(((BaseAST)getFirstChild()).ToTree($"{prefix}|  "));
				}
			}
			if (getNextSibling() != null)
			{
				stringBuilder.Append(((BaseAST)getNextSibling()).ToTree(prefix));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00003C0C File Offset: 0x00001E0C
		public static string decode(string text)
		{
			var stringBuilder = new StringBuilder();
			for (var i = 0; i < text.Length; i++)
			{
				var c = text[i];
				if (c == '&')
				{
					var c2 = text[i + 1];
					var c3 = text[i + 2];
					var c4 = text[i + 3];
					var c5 = text[i + 4];
					var c6 = text[i + 5];
					if (c2 == 'a' && c3 == 'm' && c4 == 'p' && c5 == ';')
					{
						stringBuilder.Append("&");
						i += 5;
					}
					else if (c2 == 'l' && c3 == 't' && c4 == ';')
					{
						stringBuilder.Append("<");
						i += 4;
					}
					else if (c2 == 'g' && c3 == 't' && c4 == ';')
					{
						stringBuilder.Append(">");
						i += 4;
					}
					else if (c2 == 'q' && c3 == 'u' && c4 == 'o' && c5 == 't' && c6 == ';')
					{
						stringBuilder.Append("\"");
						i += 6;
					}
					else if (c2 == 'a' && c3 == 'p' && c4 == 'o' && c5 == 's' && c6 == ';')
					{
						stringBuilder.Append("'");
						i += 6;
					}
					else
					{
						stringBuilder.Append("&");
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003D7C File Offset: 0x00001F7C
		public static string encode(string text)
		{
			var stringBuilder = new StringBuilder();
			foreach (var c in text)
			{
				var c2 = c;
				if (c2 != '"')
				{
					switch (c2)
					{
					case '&':
						stringBuilder.Append("&amp;");
						break;
					case '\'':
						stringBuilder.Append("&apos;");
						break;
					default:
						switch (c2)
						{
						case '<':
							stringBuilder.Append("&lt;");
							goto IL_92;
						case '>':
							stringBuilder.Append("&gt;");
							goto IL_92;
						}
						stringBuilder.Append(c);
						break;
					}
				}
				else
				{
					stringBuilder.Append("&quot;");
				}
				IL_92:;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00003E34 File Offset: 0x00002034
		public virtual void xmlSerializeNode(TextWriter outWriter)
		{
			var stringBuilder = new StringBuilder(100);
			stringBuilder.Append("<");
			stringBuilder.Append($"{GetType().FullName} ");
			stringBuilder.Append(string.Concat(new object[]
			{
				"text=\"",
				encode(getText()),
				"\" type=\"",
				Type,
				"\"/>"
			}));
			outWriter.Write(stringBuilder.ToString());
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00003EC8 File Offset: 0x000020C8
		public virtual void xmlSerializeRootOpen(TextWriter outWriter)
		{
			var stringBuilder = new StringBuilder(100);
			stringBuilder.Append("<");
			stringBuilder.Append($"{GetType().FullName} ");
			stringBuilder.Append(string.Concat(new object[]
			{
				"text=\"",
				encode(getText()),
				"\" type=\"",
				Type,
				"\">\n"
			}));
			outWriter.Write(stringBuilder.ToString());
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00003F5C File Offset: 0x0000215C
		public virtual void xmlSerializeRootClose(TextWriter outWriter)
		{
			outWriter.Write($"</{GetType().FullName}>\n");
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003F8C File Offset: 0x0000218C
		public virtual void xmlSerialize(TextWriter outWriter)
		{
			for (AST ast = this; ast != null; ast = ast.getNextSibling())
			{
				if (ast.getFirstChild() == null)
				{
					((BaseAST)ast).xmlSerializeNode(outWriter);
				}
				else
				{
					((BaseAST)ast).xmlSerializeRootOpen(outWriter);
					((BaseAST)ast.getFirstChild()).xmlSerialize(outWriter);
					((BaseAST)ast).xmlSerializeRootClose(outWriter);
				}
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003FE8 File Offset: 0x000021E8
		[Obsolete("Deprecated since version 2.7.2. Use ASTFactory.dup() instead.", false)]
		public virtual object Clone()
		{
			return MemberwiseClone();
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003FFC File Offset: 0x000021FC
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x04000020 RID: 32
		protected internal BaseAST down;

		// Token: 0x04000021 RID: 33
		protected internal BaseAST right;

		// Token: 0x04000022 RID: 34
		private static bool verboseStringConversion = false;

		// Token: 0x04000023 RID: 35
		private static string[] tokenNames = null;
	}
}
