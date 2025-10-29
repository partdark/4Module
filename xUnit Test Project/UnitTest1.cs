namespace xUnit_Test_Project
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var resualt = Math.Add(1, 2);
            var exepted = 1 + 2;

            Assert.Equal(exepted, resualt);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(5, 5, 10)]
        public void TestAdd(double a, double b, double exepted)
        {
            var result = Math.Add(a, b);
            Assert.Equal(exepted, result);
        }
    }

    public static class Math
    {
        public static double Add(double a, double b) => a + b;


    }
}