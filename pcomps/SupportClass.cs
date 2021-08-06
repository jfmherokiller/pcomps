

// Token: 0x0200007E RID: 126
namespace pcomps
{
    internal class SupportClass
    {
        // Token: 0x060004A9 RID: 1193 RVA: 0x0000E6B4 File Offset: 0x0000C8B4
        public static int URShift(int number, int bits)
        {
            if (number >= 0)
            {
                return number >> bits;
            }
            return (number >> bits) + (2 << ~bits);
        }

        // Token: 0x060004AA RID: 1194 RVA: 0x0000E6DC File Offset: 0x0000C8DC
        public static int URShift(int number, long bits)
        {
            return SupportClass.URShift(number, (int)bits);
        }

        // Token: 0x060004AB RID: 1195 RVA: 0x0000E6F4 File Offset: 0x0000C8F4
        public static long URShift(long number, int bits)
        {
            if (number >= 0L)
            {
                return number >> bits;
            }
            return (number >> bits) + (2L << ~bits);
        }

        // Token: 0x060004AC RID: 1196 RVA: 0x0000E71C File Offset: 0x0000C91C
        public static long URShift(long number, long bits)
        {
            return SupportClass.URShift(number, (int)bits);
        }
    }
}
