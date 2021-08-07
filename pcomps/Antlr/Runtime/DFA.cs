namespace pcomps.Antlr.Runtime
{
	// Token: 0x02000087 RID: 135
	public abstract class DFA
	{
		// Token: 0x0600051A RID: 1306 RVA: 0x0000F6FC File Offset: 0x0000D8FC
		public int Predict(IIntStream input)
		{
			var marker = input.Mark();
			var num = 0;
			int result;
			try
			{
				char c;
				for (;;)
				{
					var num2 = (int)special[num];
					if (num2 >= 0)
					{
						num = specialStateTransitionHandler(this, num2, input);
						if (num == -1)
						{
							break;
						}
						input.Consume();
					}
					else
					{
						if (accept[num] >= 1)
						{
							goto Block_4;
						}
						c = (char)input.LA(1);
						if (c >= min[num] && c <= max[num])
						{
							var num3 = (int)transition[num][c - min[num]];
							if (num3 < 0)
							{
								if (eot[num] < 0)
								{
									goto IL_CB;
								}
								num = eot[num];
								input.Consume();
							}
							else
							{
								num = num3;
								input.Consume();
							}
						}
						else
						{
							if (eot[num] < 0)
							{
								goto IL_10B;
							}
							num = eot[num];
							input.Consume();
						}
					}
				}
				NoViableAlt(num, input);
				return 0;
				Block_4:
				return accept[num];
				IL_CB:
				NoViableAlt(num, input);
				return 0;
				IL_10B:
				if (c == (char)Token.EOF && eof[num] >= 0)
				{
					result = accept[eof[num]];
				}
				else
				{
					NoViableAlt(num, input);
					result = 0;
				}
			}
			finally
			{
				input.Rewind(marker);
			}
			return result;
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0000F884 File Offset: 0x0000DA84
		protected void NoViableAlt(int s, IIntStream input)
		{
			if (recognizer.state.backtracking > 0)
			{
				recognizer.state.failed = true;
				return;
			}
			var ex = new NoViableAltException(Description, decisionNumber, s, input);
			Error(ex);
			throw ex;
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0000F8D8 File Offset: 0x0000DAD8
		public virtual void Error(NoViableAltException nvae)
		{
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0000F8DC File Offset: 0x0000DADC
		public virtual int SpecialStateTransition(int s, IIntStream input)
		{
			return -1;
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600051E RID: 1310 RVA: 0x0000F8E0 File Offset: 0x0000DAE0
		public virtual string Description => "n/a";

        // Token: 0x0600051F RID: 1311 RVA: 0x0000F8E8 File Offset: 0x0000DAE8
		public static short[] UnpackEncodedString(string encodedString)
		{
			var num = 0;
			for (var i = 0; i < encodedString.Length; i += 2)
			{
				num += encodedString[i];
			}
			var array = new short[num];
			var num2 = 0;
			for (var j = 0; j < encodedString.Length; j += 2)
			{
				var c = encodedString[j];
				var c2 = encodedString[j + 1];
				for (var k = 1; k <= c; k++)
				{
					array[num2++] = (short)c2;
				}
			}
			return array;
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0000F974 File Offset: 0x0000DB74
		public static short[][] UnpackEncodedStringArray(string[] encodedStrings)
		{
			var array = new short[encodedStrings.Length][];
			for (var i = 0; i < encodedStrings.Length; i++)
			{
				array[i] = UnpackEncodedString(encodedStrings[i]);
			}
			return array;
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0000F9AC File Offset: 0x0000DBAC
		public static char[] UnpackEncodedStringToUnsignedChars(string encodedString)
		{
			var num = 0;
			for (var i = 0; i < encodedString.Length; i += 2)
			{
				num += encodedString[i];
			}
			var array = new char[num];
			var num2 = 0;
			for (var j = 0; j < encodedString.Length; j += 2)
			{
				var c = encodedString[j];
				var c2 = encodedString[j + 1];
				for (var k = 1; k <= c; k++)
				{
					array[num2++] = c2;
				}
			}
			return array;
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0000FA38 File Offset: 0x0000DC38
		public static int SpecialTransition(int state, int symbol)
		{
			return 0;
		}

		// Token: 0x0400015D RID: 349
		public const bool debug = false;

		// Token: 0x0400015E RID: 350
		protected short[] eot;

		// Token: 0x0400015F RID: 351
		protected short[] eof;

		// Token: 0x04000160 RID: 352
		protected char[] min;

		// Token: 0x04000161 RID: 353
		protected char[] max;

		// Token: 0x04000162 RID: 354
		protected short[] accept;

		// Token: 0x04000163 RID: 355
		protected short[] special;

		// Token: 0x04000164 RID: 356
		protected short[][] transition;

		// Token: 0x04000165 RID: 357
		protected int decisionNumber;

		// Token: 0x04000166 RID: 358
		public SpecialStateTransitionHandler specialStateTransitionHandler;

		// Token: 0x04000167 RID: 359
		protected BaseRecognizer recognizer;

		// Token: 0x02000088 RID: 136
		// (Invoke) Token: 0x06000524 RID: 1316
		public delegate int SpecialStateTransitionHandler(DFA dfa, int s, IIntStream input);
	}
}
