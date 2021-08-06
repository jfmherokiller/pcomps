namespace pcomps.Antlr.Runtime.Tree
{
	// Token: 0x020000E7 RID: 231
	public interface ITreeVisitorAction
	{
		// Token: 0x0600097D RID: 2429
		object Pre(object t);

		// Token: 0x0600097E RID: 2430
		object Post(object t);
	}
}
