using System.Collections;
using pcomps.Antlr.StringTemplate.Collections;

namespace pcomps.Antlr.StringTemplate.Language
{
	// Token: 0x02000241 RID: 577
	public class FormalArgument
	{
		// Token: 0x06001153 RID: 4435 RVA: 0x0007E838 File Offset: 0x0007CA38
		public FormalArgument(string name)
		{
			this.name = name;
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x0007E848 File Offset: 0x0007CA48
		public FormalArgument(string name, StringTemplate defaultValueST)
		{
			this.name = name;
			this.defaultValueST = defaultValueST;
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x0007E860 File Offset: 0x0007CA60
		public static string GetCardinalityName(int cardinality)
		{
			switch (cardinality)
			{
			case 1:
				return "optional";
			case 2:
				return "exactly one";
			case 3:
				break;
			case 4:
				return "zero-or-more";
			default:
				if (cardinality == 8)
				{
					return "one-or-more";
				}
				break;
			}
			return "unknown";
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x0007E8AC File Offset: 0x0007CAAC
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x0007E8B4 File Offset: 0x0007CAB4
		public override bool Equals(object o)
		{
			if (o == null || !(o is FormalArgument))
			{
				return false;
			}
			FormalArgument formalArgument = (FormalArgument)o;
			return this.name.Equals(formalArgument.name) && (this.defaultValueST == null || formalArgument.defaultValueST != null) && (this.defaultValueST != null || formalArgument.defaultValueST == null);
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x0007E910 File Offset: 0x0007CB10
		public override string ToString()
		{
			if (this.defaultValueST != null)
			{
				return this.name + "=" + this.defaultValueST;
			}
			return this.name;
		}

		// Token: 0x04000E6B RID: 3691
		public const int OPTIONAL = 1;

		// Token: 0x04000E6C RID: 3692
		public const int REQUIRED = 2;

		// Token: 0x04000E6D RID: 3693
		public const int ZERO_OR_MORE = 4;

		// Token: 0x04000E6E RID: 3694
		public const int ONE_OR_MORE = 8;

		// Token: 0x04000E6F RID: 3695
		public static readonly string[] suffixes = new string[]
		{
			null,
			"?",
			"",
			null,
			"*",
			null,
			null,
			null,
			"+"
		};

		// Token: 0x04000E70 RID: 3696
		public static IDictionary UNKNOWN = new HashList();

		// Token: 0x04000E71 RID: 3697
		public string name;

		// Token: 0x04000E72 RID: 3698
		public StringTemplate defaultValueST;
	}
}
