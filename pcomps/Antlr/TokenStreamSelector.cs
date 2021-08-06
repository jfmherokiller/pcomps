using System;
using System.Collections;

namespace pcomps.Antlr
{
	// Token: 0x02000048 RID: 72
	public class TokenStreamSelector : TokenStream
	{
		// Token: 0x060002AF RID: 687 RVA: 0x00008ED4 File Offset: 0x000070D4
		public TokenStreamSelector()
		{
			this.inputStreamNames = new Hashtable();
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00008F00 File Offset: 0x00007100
		public virtual void addInputStream(TokenStream stream, string key)
		{
			this.inputStreamNames[key] = stream;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00008F1C File Offset: 0x0000711C
		public virtual TokenStream getCurrentStream()
		{
			return this.input;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00008F30 File Offset: 0x00007130
		public virtual TokenStream getStream(string sname)
		{
			TokenStream tokenStream = (TokenStream)this.inputStreamNames[sname];
			if (tokenStream == null)
			{
				throw new ArgumentException("TokenStream " + sname + " not found");
			}
			return tokenStream;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00008F6C File Offset: 0x0000716C
		public virtual IToken nextToken()
		{
			IToken result = null;
			try
			{
                result = this.input.nextToken();
			}
			catch (TokenStreamRetryException)
			{
                result = this.input.nextToken();
			}
			return result;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00008F9C File Offset: 0x0000719C
		public virtual TokenStream pop()
		{
			TokenStream tokenStream = (TokenStream)this.streamStack.Pop();
			this.select(tokenStream);
			return tokenStream;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00008FC4 File Offset: 0x000071C4
		public virtual void push(TokenStream stream)
		{
			this.streamStack.Push(this.input);
			this.select(stream);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00008FEC File Offset: 0x000071EC
		public virtual void push(string sname)
		{
			this.streamStack.Push(this.input);
			this.select(sname);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00009014 File Offset: 0x00007214
		public virtual void retry()
		{
			throw new TokenStreamRetryException();
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00009028 File Offset: 0x00007228
		public virtual void select(TokenStream stream)
		{
			this.input = stream;
			if (this.input is CharScanner)
			{
				((CharScanner)this.input).refresh();
			}
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000905C File Offset: 0x0000725C
		public virtual void select(string sname)
		{
			this.input = this.getStream(sname);
			if (this.input is CharScanner)
			{
				((CharScanner)this.input).refresh();
			}
		}

		// Token: 0x040000D2 RID: 210
		protected internal Hashtable inputStreamNames;

		// Token: 0x040000D3 RID: 211
		protected internal TokenStream input;

		// Token: 0x040000D4 RID: 212
		protected internal Stack streamStack = new Stack();
	}
}
