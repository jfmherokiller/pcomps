namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000E8 RID: 232
	public class TreeVisitor
	{
		// Token: 0x0600097F RID: 2431 RVA: 0x0001B8A8 File Offset: 0x00019AA8
		public TreeVisitor(ITreeAdaptor adaptor)
		{
			this.adaptor = adaptor;
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x0001B8B8 File Offset: 0x00019AB8
		public TreeVisitor() : this(new CommonTreeAdaptor())
		{
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0001B8C8 File Offset: 0x00019AC8
		public object Visit(object t, ITreeVisitorAction action)
		{
			bool flag = adaptor.IsNil(t);
			if (action != null && !flag)
			{
				t = action.Pre(t);
			}
			int childCount = adaptor.GetChildCount(t);
			for (int i = 0; i < childCount; i++)
			{
				object child = adaptor.GetChild(t, i);
				object obj = Visit(child, action);
				object child2 = adaptor.GetChild(t, i);
				if (obj != child2)
				{
					adaptor.SetChild(t, i, obj);
				}
			}
			if (action != null && !flag)
			{
				t = action.Post(t);
			}
			return t;
		}

		// Token: 0x04000289 RID: 649
		protected ITreeAdaptor adaptor;
	}
}
