using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace pcomps.Antlr.StringTemplate
{
	// Token: 0x0200021C RID: 540
	public class FileSystemTemplateLoader : StringTemplateLoader
	{
		// Token: 0x06000F49 RID: 3913 RVA: 0x0006E6D4 File Offset: 0x0006C8D4
		public FileSystemTemplateLoader() : this(null, Encoding.Default, true)
		{
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x0006E6E4 File Offset: 0x0006C8E4
		public FileSystemTemplateLoader(string locationRoot) : this(locationRoot, Encoding.Default, true)
		{
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x0006E6F4 File Offset: 0x0006C8F4
		public FileSystemTemplateLoader(string locationRoot, bool raiseExceptionForEmptyTemplate) : this(locationRoot, Encoding.Default, raiseExceptionForEmptyTemplate)
		{
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x0006E704 File Offset: 0x0006C904
		public FileSystemTemplateLoader(string locationRoot, Encoding encoding) : this(locationRoot, encoding, true)
		{
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0006E710 File Offset: 0x0006C910
		public FileSystemTemplateLoader(string locationRoot, Encoding encoding, bool raiseExceptionForEmptyTemplate) : base(locationRoot, raiseExceptionForEmptyTemplate)
		{
			if (locationRoot == null || locationRoot.Trim().Length == 0)
			{
				this.locationRoot = AppDomain.CurrentDomain.BaseDirectory;
			}
			this.encoding = encoding;
			fileSet = new HybridDictionary(true);
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x0006E750 File Offset: 0x0006C950
		public override bool HasChanged(string templateName)
		{
			var key = $"{LocationRoot}/{GetLocationFromTemplateName(templateName)}".Replace('\\', '/');
			return fileSet[key] != null;
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x0006E794 File Offset: 0x0006C994
		protected override string InternalLoadTemplateContents(string templateName)
		{
			string text = null;
			string text2 = null;
			try
			{
				text2 = $"{LocationRoot}/{GetLocationFromTemplateName(templateName)}".Replace('\\', '/');
				StreamReader streamReader;
				try
				{
					streamReader = new StreamReader(text2, encoding);
				}
				catch (FileNotFoundException)
				{
					return null;
				}
				catch (DirectoryNotFoundException)
				{
					return null;
				}
				catch (Exception innerException)
				{
					throw new TemplateLoadException($"Cannot open template file: {text2}", innerException);
				}
				try
				{
					text = streamReader.ReadToEnd();
					if (text != null && text.Length > 0 && filesWatcher == null)
					{
						filesWatcher = new FileSystemWatcher(LocationRoot, "*.st");
						filesWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Security);
						filesWatcher.IncludeSubdirectories = true;
						filesWatcher.Changed += OnChanged;
						filesWatcher.Deleted += OnChanged;
						filesWatcher.Created += OnChanged;
						filesWatcher.Renamed += OnRenamed;
						filesWatcher.EnableRaisingEvents = true;
					}
					fileSet.Remove(text2);
				}
				finally
				{
                    ((IDisposable)streamReader)?.Dispose();
                    streamReader = null;
				}
			}
			catch (ArgumentException innerException2)
			{
				string message;
				if (text == null)
				{
					message = $"Invalid file character encoding: {encoding}";
				}
				else
				{
					message =
                        $"The location root '{LocationRoot}' and/or the template name '{templateName}' is invalid.";
				}
				throw new TemplateLoadException(message, innerException2);
			}
			catch (IOException innerException3)
			{
				throw new TemplateLoadException($"Cannot close template file: {text2}", innerException3);
			}
			return text;
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x0006E9B8 File Offset: 0x0006CBB8
		public override string GetLocationFromTemplateName(string templateName)
		{
			return $"{templateName}.st";
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x0006E9C8 File Offset: 0x0006CBC8
		public override string GetTemplateNameFromLocation(string location)
		{
			return Path.ChangeExtension(location, null);
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x0006E9D4 File Offset: 0x0006CBD4
		private void OnChanged(object source, FileSystemEventArgs e)
		{
			var key = e.FullPath.Replace('\\', '/');
			fileSet[key] = locationRoot;
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x0006EA04 File Offset: 0x0006CC04
		private void OnRenamed(object source, RenamedEventArgs e)
		{
			var key = e.FullPath.Replace('\\', '/');
			fileSet[key] = locationRoot;
			key = e.OldFullPath.Replace('\\', '/');
			fileSet[key] = locationRoot;
		}

		// Token: 0x04000CC9 RID: 3273
		private Encoding encoding;

		// Token: 0x04000CCA RID: 3274
		private FileSystemWatcher filesWatcher;

		// Token: 0x04000CCB RID: 3275
		private HybridDictionary fileSet;
	}
}
