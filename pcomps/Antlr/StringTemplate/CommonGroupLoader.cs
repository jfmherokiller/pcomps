using System;
using System.Collections;
using System.IO;
using System.Text;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x02000212 RID: 530
	public class CommonGroupLoader : IStringTemplateGroupLoader
	{
		// Token: 0x06000F10 RID: 3856 RVA: 0x0006DE84 File Offset: 0x0006C084
		protected CommonGroupLoader() : this(DefaultGroupFactory.DefaultFactory, NullErrorListener.DefaultNullListener, Encoding.Default, null)
		{
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0006DE9C File Offset: 0x0006C09C
		public CommonGroupLoader(IStringTemplateErrorListener errorListener, params string[] directoryNames) : this(new DefaultGroupFactory(), errorListener, Encoding.Default, directoryNames)
		{
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x0006DEB0 File Offset: 0x0006C0B0
		public CommonGroupLoader(IStringTemplateErrorListener errorListener, Encoding encoding, params string[] directoryNames) : this(new DefaultGroupFactory(), errorListener, encoding, directoryNames)
		{
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x0006DEC0 File Offset: 0x0006C0C0
		public CommonGroupLoader(IStringTemplateGroupFactory factory, IStringTemplateErrorListener errorListener, Encoding encoding, params string[] directoryNames)
		{
			this.factory = factory;
			this.errorListener = ((errorListener == null) ? new NullErrorListener() : errorListener);
			this.encoding = encoding;
			foreach (string text in directoryNames)
			{
				string text2;
				if (Path.IsPathRooted(text))
				{
					text2 = text;
				}
				else
				{
					text2 = string.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, text);
				}
				if (File.Exists(text2) || !Directory.Exists(text2))
				{
					this.Error("group loader: no such dir " + text2);
				}
				else
				{
					if (this.directories == null)
					{
						this.directories = new ArrayList();
					}
					this.directories.Add(text2);
				}
			}
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x0006DF6C File Offset: 0x0006C16C
		public StringTemplateGroup LoadGroup(string groupName)
		{
			return this.LoadGroup(groupName, null, null);
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x0006DF78 File Offset: 0x0006C178
		public StringTemplateGroup LoadGroup(string groupName, StringTemplateGroup superGroup)
		{
			return this.LoadGroup(groupName, superGroup, null);
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x0006DF84 File Offset: 0x0006C184
		public StringTemplateGroup LoadGroup(string groupName, StringTemplateGroup superGroup, Type lexer)
		{
			StringTemplateGroup result = null;
			string text = this.LocateFile(groupName + ".stg");
			if (text == null)
			{
				this.Error("no such group file '" + groupName + ".stg'");
			}
			else
			{
				using (StreamReader streamReader = new StreamReader(text, this.encoding))
				{
					try
					{
						result = this.factory.CreateGroup(streamReader, lexer, this.errorListener, superGroup);
					}
					catch (ArgumentException e)
					{
						this.Error("Path Error: can't load group '" + groupName + "'", e);
					}
					catch (IOException e2)
					{
						this.Error("IO Error: can't load group '" + groupName + "'", e2);
					}
				}
			}
			return result;
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x0006E050 File Offset: 0x0006C250
		public StringTemplateGroupInterface LoadInterface(string interfaceName)
		{
			StringTemplateGroupInterface result = null;
			string text = this.LocateFile(interfaceName + ".sti");
			if (text == null)
			{
				this.Error("no such interface file '" + interfaceName + ".sti'");
			}
			else
			{
				using (StreamReader streamReader = new StreamReader(text, this.encoding))
				{
					try
					{
						result = this.factory.CreateInterface(streamReader, this.errorListener, null);
					}
					catch (ArgumentException e)
					{
						this.Error("Path Error: can't load interface '" + interfaceName + "'", e);
					}
					catch (IOException e2)
					{
						this.Error("IO Error: can't load interface '" + interfaceName + "'", e2);
					}
				}
			}
			return result;
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x0006E11C File Offset: 0x0006C31C
		protected string LocateFile(string filename)
		{
			for (int i = 0; i < this.directories.Count; i++)
			{
				string arg = (string)this.directories[i];
				string text = string.Format("{0}/{1}", arg, filename);
				if (File.Exists(text))
				{
					return text;
				}
			}
			return null;
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0006E16C File Offset: 0x0006C36C
		public void Error(string msg)
		{
			this.errorListener.Error(msg, null);
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x0006E17C File Offset: 0x0006C37C
		public void Error(string msg, Exception e)
		{
			this.errorListener.Error(msg, e);
		}

		// Token: 0x04000CBA RID: 3258
		protected IStringTemplateGroupFactory factory;

		// Token: 0x04000CBB RID: 3259
		protected ArrayList directories;

		// Token: 0x04000CBC RID: 3260
		protected Encoding encoding;

		// Token: 0x04000CBD RID: 3261
		protected IStringTemplateErrorListener errorListener;
	}
}
