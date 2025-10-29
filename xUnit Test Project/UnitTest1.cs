namespace xUnit_Test_Project
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var resualt =  Math.Add(1, 2);
            var exepted = 1 + 2;

            Assert.Equal(exepted, resualt);
        }


    }
    public static class Math
    {
        public static double Add(double a, double b) => a + b;
    }
}