﻿using System;
using System.IO;
using pcomps.Antlr.StringTemplate.Collections;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000210 RID: 528
	public class AutoIndentWriter : IStringTemplateWriter
	{
		// Token: 0x06000EFE RID: 3838 RVA: 0x0006DB20 File Offset: 0x0006BD20
		public AutoIndentWriter(TextWriter output)
		{
			this.output = output;
			this.indents.Push(null);
		}

		// Token: 0x17000229 RID: 553
		// (set) Token: 0x06000EFF RID: 3839 RVA: 0x0006DB74 File Offset: 0x0006BD74
		public int LineWidth
		{
			set
			{
				this.lineWidth = value;
			}
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x0006DB80 File Offset: 0x0006BD80
		public virtual void PushIndentation(string indent)
		{
			this.indents.Push(indent);
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0006DB90 File Offset: 0x0006BD90
		public virtual string PopIndentation()
		{
			return (string)this.indents.Pop();
		}

		// Token: 0x06000F02 RID: 3842 RVA: 0x0006DBA4 File Offset: 0x0006BDA4
		public virtual void PushAnchorPoint()
		{
			if (this.anchors_sp + 1 >= this.anchors.Length)
			{
				int[] destinationArray = new int[this.anchors.Length * 2];
				Array.Copy(this.anchors, 0, destinationArray, 0, this.anchors.Length - 1);
				this.anchors = destinationArray;
			}
			this.anchors_sp++;
			this.anchors[this.anchors_sp] = this.charPosition;
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0006DC14 File Offset: 0x0006BE14
		public virtual void PopAnchorPoint()
		{
			this.anchors_sp--;
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0006DC24 File Offset: 0x0006BE24
		public virtual int GetIndentationWidth()
		{
			int num = 0;
			for (int i = 0; i < this.indents.Count; i++)
			{
				string text = (string)this.indents[i];
				if (text != null)
				{
					num += text.Length;
				}
			}
			return num;
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x0006DC68 File Offset: 0x0006BE68
		public virtual int Write(string str)
		{
			int num = 0;
			foreach (char c in str)
			{
				if (c == '\n')
				{
					this.atStartOfLine = true;
					this.charPosition = -1;
				}
				else if (this.atStartOfLine)
				{
					num += this.Indent();
					this.atStartOfLine = false;
				}
				num++;
				this.output.Write(c);
				this.charPosition++;
			}
			return num;
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x0006DCDC File Offset: 0x0006BEDC
		public virtual int WriteSeparator(string str)
		{
			return this.Write(str);
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x0006DCE8 File Offset: 0x0006BEE8
		public int Write(string str, string wrap)
		{
			int num = this.WriteWrapSeparator(wrap);
			return num + this.Write(str);
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x0006DD08 File Offset: 0x0006BF08
		public int WriteWrapSeparator(string wrap)
		{
			int num = 0;
			if (this.lineWidth != -1 && wrap != null && !this.atStartOfLine && this.charPosition >= this.lineWidth)
			{
				foreach (char c in wrap)
				{
					if (c == '\n')
					{
						num++;
						this.output.Write(c);
						this.charPosition = 0;
						int indentationWidth = this.GetIndentationWidth();
						int num2 = 0;
						if (this.anchors_sp >= 0)
						{
							num2 = this.anchors[this.anchors_sp];
						}
						if (num2 > indentationWidth)
						{
							num += this.Indent(num2);
						}
						else
						{
							num += this.Indent();
						}
					}
					else
					{
						num++;
						this.output.Write(c);
						this.charPosition++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x0006DDE0 File Offset: 0x0006BFE0
		public virtual int Indent()
		{
			int num = 0;
			for (int i = 0; i < this.indents.Count; i++)
			{
				string text = (string)this.indents[i];
				if (text != null)
				{
					num += text.Length;
					this.output.Write(text);
				}
			}
			this.charPosition += num;
			return num;
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x0006DE40 File Offset: 0x0006C040
		public int Indent(int spaces)
		{
			for (int i = 1; i <= spaces; i++)
			{
				this.output.Write(' ');
			}
			this.charPosition += spaces;
			return spaces;
		}

		// Token: 0x04000CB1 RID: 3249
		public static readonly string newline = Environment.NewLine;

		// Token: 0x04000CB2 RID: 3250
		internal StackList indents = new StackList();

		// Token: 0x04000CB3 RID: 3251
		protected int[] anchors = new int[10];

		// Token: 0x04000CB4 RID: 3252
		protected int anchors_sp = -1;

		// Token: 0x04000CB5 RID: 3253
		protected TextWriter output;

		// Token: 0x04000CB6 RID: 3254
		protected bool atStartOfLine = true;

		// Token: 0x04000CB7 RID: 3255
		protected int charPosition;

		// Token: 0x04000CB8 RID: 3256
		protected int lineWidth = -1;

		// Token: 0x04000CB9 RID: 3257
		protected int charPositionOfStartOfExpr;
	}
}
