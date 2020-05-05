using NUnit.Framework;
using FSMS.Util;

namespace Tests
{
    public class TestDateTimeHelper
    {
        [Test]
        public void TestFormatTime()
        {
            long ticks = 333333000;     // Environment.TickCount;

            string result = DateTimeHelper.FormatTime(ticks);

            Assert.AreEqual("03 �� 20 Сʱ 35 �� 33 ��", result);
        }
    }
}